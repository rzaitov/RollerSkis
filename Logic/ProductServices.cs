using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using Logic.Domain;
using Logic.Data;

namespace Logic.Service
{
	public class ProductServices
	{
		private string _login;
		private string _password;
		private string _host;

		private string _connectionString;

		private object _myLocker = new object ();
		private static IEnumerable<ProductTypeNode> ProductTypesHierarchy;
		private static Dictionary<ProductType, ProductTypeValue> ProductTypesRelations;

		public ProductServices (string login, string pwd, string host, string dbName)
		{
			_login = login;
			_password = pwd;
			_host = host;

			_connectionString = string.Format ("Server={0};Database={1};User Id={2};Password={3};", host, dbName, login, pwd);

			LoadProductTypesRelations ();
		}

		private void LoadProductTypesRelations ()
		{
			if (ProductTypesRelations == null)
			{
				lock (_myLocker)
				{
					if (ProductTypesRelations == null)
					{
						ProductTypesRelations = new Dictionary<ProductType, ProductTypeValue> ();

						string cmdText = @"
WITH PTHierarchy ([id], [parent_id], [name], [level])
AS
(
	SELECT PT.id, PT.parent_id, PT.name, 0 AS level
	FROM [ProductType] AS PT
	WHERE PT.parent_id IS NULL

	UNION ALL

	SELECT PT.id, PT.parent_id, PT.name, level + 1 AS lecel
	FROM [ProductType] AS PT
	INNER JOIN PTHierarchy AS PTH
		ON PTH.id = PT.parent_id
)
SELECT [id], [parent_id], [name]
FROM PTHierarchy
ORDER BY level, parent_id ASC";


						Action<SqlDataReader> fetcher = reader =>
						{
							ProductTypeOridinal pto = new ProductTypeOridinal (reader);
							pto.LoadOrdinals ();

							while (reader.Read ())
							{
								ProductTypeValue ptv = pto.GetProductTypeValue ();
								ProductTypesRelations[ptv.ProductType] = ptv;
							}
						};

						ExecuteQuery (cmdText, null, fetcher);
					}
				}
			}
		}

		public IEnumerable<Product> GetProductsByType (ProductType type)
		{
			// Получает товары чья категория или родительская категория совпадает с заданной
			string cmdText = string.Format(@"
SELECT {0}
FROM Product AS P
INNER JOIN ProductType AS PT
	ON P.product_type = PT.id
WHERE
	P.[product_type] = @product_type OR
	PT.[parent_id] = @product_type
", GetEscapedFieldNames("P"));

			List<Product> products = new List<Product>();

			using (SqlConnection connection = new SqlConnection (_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand( cmdText, connection) )
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue ("@product_type", (int)type);
					connection.Open ();

					using (SqlDataReader reader = cmd.ExecuteReader ())
					{
						ProductFieldOridinal pfo = new ProductFieldOridinal (reader);
						pfo.LoadOrdinals ();

						while (reader.Read ())
						{
							Product p = pfo.GetProduct ();
							products.Add (p);
						}
					}
				}
			}

			return products;
		}

		public Product GetProduct (ProductType exactType, string name)
		{
			string cmdText = string.Format (@"
SELECT {0}, SPEC.[name] as spec_name, SPEC.[value] as spec_value
FROM Product AS P
LEFT JOIN Specification AS SPEC
	ON SPEC.[product] = P.[id]
WHERE
	P.[product_type] = @product_type
	AND P.[name] = @product_name
", GetEscapedFieldNames ("P"));
			
			Product foundProduct = null;
			
			Dictionary<string, object> parameters = new Dictionary<string, object>
			{
				{ "@product_type", (int)exactType },
				{ "@product_name", name }
			};

			Action<SqlDataReader> fetcher = reader =>
			{
				ProductFieldOridinal pfo = new ProductFieldOridinal (reader);
				pfo.LoadOrdinals ();

				SpecificationFieldOridinal sfo = new SpecificationFieldOridinal (reader);
				sfo.SetAliasForColumn ("name", "spec_name");
				sfo.SetAliasForColumn ("value", "spec_value");
				sfo.LoadOrdinals ();

				bool isFirstRow = true;
				while (reader.Read ())
				{
					if (isFirstRow)
					{
						foundProduct = pfo.GetProduct ();
						isFirstRow = false;
					}
					sfo.LoadOneSpecificationTo (foundProduct);
				}
			};

			ExecuteQuery (cmdText, parameters, fetcher);

			return foundProduct;
		}

		public bool IsValidParentTypeFor (ProductType parentType, ProductType product)
		{
			ProductType? realParentType = ProductTypesRelations[product].ParentType;

			bool result = realParentType.HasValue && realParentType.Value == parentType;
			return result;
		}

		public List<ProductType> GetAncestorTypesAndSelf (ProductType type)
		{
			List<ProductType> types = new List<ProductType> ();

			ProductTypeValue ptv = null;
			ProductType? pt = type;
			do
			{
				// не помещаем в lock тк в ProductTypesRelations не произодится запись
				ptv = ProductTypesRelations[pt.Value];
				types.Add (ptv.ProductType);

				pt = ptv.ParentType;
			} while (pt.HasValue);

			types.Reverse();
			return types;
		}

		[Obsolete]
		public IEnumerable<ProductTypeNode> GetProductTypesHierarchy ()
		{
			if (ProductTypesHierarchy == null)
			{
				ProductTypesHierarchy = LoadProductTypesHierarchy ();
			}

			return ProductTypesHierarchy;
		}
		[Obsolete]
		private IEnumerable<ProductTypeNode> LoadProductTypesHierarchy ()
		{
			string cmdText = @"
SELECT *
FROM ProductType AS PT
ORDER BY PT.[parent_id]";

			List<ProductTypeNode> nodes = new List<ProductTypeNode> ();

			Action<SqlDataReader> fetcher = reader =>
			{
				ProductTypeOridinal pto = new ProductTypeOridinal (reader);
				pto.LoadOrdinals ();

				while (reader.Read ())
				{
					ProductTypeValue ptv = pto.GetProductTypeValue ();

					ProductTypeNode node = new ProductTypeNode ();
					node.ProductTypeValue = ptv;

					nodes.Add (node);
				}
			};

			ExecuteQuery (cmdText, null, fetcher);

			var lookup = nodes.ToLookup(ptv => ptv.ProductTypeValue.ParentType);
			nodes.Clear ();

			var parents = lookup[(ProductType?)null];
			nodes.AddRange (parents);

			foreach (var item in lookup)
			{
				if (!item.Key.HasValue)
					continue;

				ProductTypeNode parent = nodes.FirstOrDefault (ptn => ptn.ProductTypeValue.ProductType == item.Key.Value);
				if (parent == null)
					continue;

				parent.AddRange (lookup[item.Key]);
			}

			return nodes;
		}

		#region Utils
		private void ExecuteQuery (string cmdText, Dictionary<string, object> parameters, Action<SqlDataReader> fetcher)
		{
			using (SqlConnection connection = new SqlConnection (_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand (cmdText, connection))
				{
					cmd.CommandType = CommandType.Text;
					if (parameters != null)
					{
						foreach (var p in parameters)
						{
							cmd.Parameters.AddWithValue (p.Key, p.Value);
						}
					}

					connection.Open ();

					using (SqlDataReader reader = cmd.ExecuteReader ())
					{
						fetcher (reader);
					}
				}
			}
		}

		private string GetEscapedFieldNames(string tableName, params string[] additionalFields)
		{
			Func<string, string> normilizer = s => string.Format("{0}.{1}", tableName, s);
			IEnumerable<string> fields = Product.EscapedFieldsNames.Select(normilizer);
			IEnumerable<string> resultFields = (additionalFields != null) ? additionalFields.Union(fields) : fields;
			
			string result = string.Join(", ", resultFields);

			return result;
		}
		#endregion
	}
}
