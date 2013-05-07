using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using Logic.Domain;

namespace Logic.Data
{
	internal class ProductFieldOridinal : Ordinal
	{
		private string idColumnName;
		private string descriptionColumnName;
		private string imageNameColumnName;
		private string nameColumnName;
		private string priceColumnName;
		private string productTypeColumnName;

		public ProductFieldOridinal (SqlDataReader reader)
			: base(reader)
		{
			idColumnName = "id";
			descriptionColumnName = "description";
			imageNameColumnName = "image_name";
			nameColumnName = "name";
			priceColumnName = "price";
			productTypeColumnName = "product_type";

			SetColumns (new string[]
			{
				idColumnName,
				descriptionColumnName,
				imageNameColumnName,
				nameColumnName,
				priceColumnName,
				productTypeColumnName
			});
		}

		public Product GetProduct ()
		{
			Product p = new Product ();

			p.Id = _reader.GetInt32 (GetOridinalFor(idColumnName));
			p.Name = _reader.GetString (GetOridinalFor(nameColumnName));
			p.Description = _reader.IsDBNull (GetOridinalFor (descriptionColumnName)) ? null : _reader.GetString (GetOridinalFor (descriptionColumnName));
			p.ImageName = _reader.IsDBNull (GetOridinalFor (imageNameColumnName)) ? null : _reader.GetString (GetOridinalFor (imageNameColumnName));
			p.Price = _reader.GetDecimal (GetOridinalFor(priceColumnName));
			p.ProductType = (ProductType)_reader.GetInt32 (GetOridinalFor(productTypeColumnName));
			
			return p;
		}
	}
}
