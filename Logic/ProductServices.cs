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
						int idOridinal = -1;
						int descriptionOridinal = -1;
						int imageNameOridinal = -1;
						int nameOridinal = -1;
						int priceOridinal = -1;
						int productTypeOridinal = -1;

						if (reader.HasRows)
						{
							idOridinal = reader.GetOrdinal ("id");
							descriptionOridinal = reader.GetOrdinal ("description");
							imageNameOridinal = reader.GetOrdinal ("image_name");
							nameOridinal = reader.GetOrdinal ("name");
							priceOridinal = reader.GetOrdinal ("price");
							productTypeOridinal = reader.GetOrdinal ("product_type");
						}

						while (reader.Read ())
						{
							Product p = new Product ();
							p.Id = reader.GetInt32 (idOridinal);
							p.Name = reader.GetString (nameOridinal);
							p.Description = reader.IsDBNull (descriptionOridinal) ? null : reader.GetString (descriptionOridinal);
							p.ImageName = reader.IsDBNull (imageNameOridinal) ? null : reader.GetString (imageNameOridinal);
							p.Price = reader.GetDecimal (priceOridinal);
							p.ProductType = (ProductType)reader.GetInt32 (productTypeOridinal);

							products.Add (p);
						}
					}
				}
			}

			return products;
		}

		public Product GetProduct (ProductType type, string name)
		{

			throw new NotImplementedException ();
		}

		private string GetEscapedFieldNames(string tableName)
		{
			IEnumerable<string> fields = Product.EscapedFieldsNames.Select(fn => string.Format("{0}.{1}", tableName, fn));
			string result = string.Join(", ", fields);

			return result;
		}
	}
}
