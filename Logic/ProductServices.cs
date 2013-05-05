using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using Logic.Domain;

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
						pfo.LoadOriginals ();

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
SELECT {0}
FROM Product AS P
WHERE
	P.[product_type] = @product_type
	AND P.[name] = @product_name
", GetEscapedFieldNames("P"));
			
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
						pfo.LoadOriginals ();

						if (reader.Read ())
						{
							foundProduct = pfo.GetProduct();
						}
					}
				}
			}

			return foundProduct;
		}

		private string GetEscapedFieldNames(string tableName)
		{
			IEnumerable<string> fields = Product.EscapedFieldsNames.Select(fn => string.Format("{0}.{1}", tableName, fn));
			string result = string.Join(", ", fields);

			return result;
		}
	}

	internal class ProductFieldOridinal
	{
		public int idOridinal = -1;
		public int descriptionOridinal = -1;
		public int imageNameOridinal = -1;
		public int nameOridinal = -1;
		public int priceOridinal = -1;
		public int productTypeOridinal = -1;

		private SqlDataReader _reader;

		public ProductFieldOridinal (SqlDataReader reader)
		{
			_reader = reader;
		}

		public bool LoadOriginals ()
		{
			bool hasRows = _reader.HasRows;
			if (hasRows)
			{
				idOridinal = _reader.GetOrdinal ("id");
				descriptionOridinal = _reader.GetOrdinal ("description");
				imageNameOridinal = _reader.GetOrdinal ("image_name");
				nameOridinal = _reader.GetOrdinal ("name");
				priceOridinal = _reader.GetOrdinal ("price");
				productTypeOridinal = _reader.GetOrdinal ("product_type");
			}

			return hasRows;
		}

		public Product GetProduct ()
		{
			Product p = new Product ();

			p.Id = _reader.GetInt32 (idOridinal);
			p.Name = _reader.GetString (nameOridinal);
			p.Description = _reader.IsDBNull (descriptionOridinal) ? null : _reader.GetString (descriptionOridinal);
			p.ImageName = _reader.IsDBNull (imageNameOridinal) ? null : _reader.GetString (imageNameOridinal);
			p.Price = _reader.GetDecimal (priceOridinal);
			p.ProductType = (ProductType)_reader.GetInt32 (productTypeOridinal);

			return p;
		}
	}
}
