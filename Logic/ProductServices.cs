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

		public ProductServices (string login, string pwd, string host, string dbName)
		{
			_login = login;
			_password = pwd;
			_host = host;

			_connectionString = string.Format ("Server={0};Database={1};User Id={2};Password={3};", host, dbName, login, pwd);
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

			using (SqlConnection connection = new SqlConnection (_connectionString))
			{
				using (SqlCommand cmd = new SqlCommand (cmdText, connection))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue ("@product_type", (int)exactType);
					cmd.Parameters.AddWithValue ("@product_name", name);

					connection.Open ();

					using (SqlDataReader reader = cmd.ExecuteReader ())
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
					}
				}
			}

			return foundProduct;
		}

		private string GetEscapedFieldNames(string tableName, params string[] additionalFields)
		{
			Func<string, string> normilizer = s => string.Format("{0}.{1}", tableName, s);
			IEnumerable<string> fields = Product.EscapedFieldsNames.Select(normilizer);
			IEnumerable<string> resultFields = (additionalFields != null) ? additionalFields.Union(fields) : fields;
			
			string result = string.Join(", ", resultFields);

			return result;
		}
	}
}
