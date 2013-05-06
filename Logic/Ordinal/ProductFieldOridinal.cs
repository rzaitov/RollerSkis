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
		public int idOrdinal = -1;
		public int descriptionOrdinal = -1;
		public int imageNameOrdinal = -1;
		public int nameOrdinal = -1;
		public int priceOrdinal = -1;
		public int productTypeOrdinal = -1;

		private SqlDataReader _reader;

		public ProductFieldOridinal (SqlDataReader reader)
			: base(reader)
		{
			_reader = reader;
		}

		public Product GetProduct ()
		{
			Product p = new Product ();

			p.Id = _reader.GetInt32 (idOrdinal);
			p.Name = _reader.GetString (nameOrdinal);
			p.Description = _reader.IsDBNull (descriptionOrdinal) ? null : _reader.GetString (descriptionOrdinal);
			p.ImageName = _reader.IsDBNull (imageNameOrdinal) ? null : _reader.GetString (imageNameOrdinal);
			p.Price = _reader.GetDecimal (priceOrdinal);
			p.ProductType = (ProductType)_reader.GetInt32 (productTypeOrdinal);

			return p;
		}

		protected override void LoadOrdinalValues ()
		{
			idOrdinal = _reader.GetOrdinal ("id");
			descriptionOrdinal = _reader.GetOrdinal ("description");
			imageNameOrdinal = _reader.GetOrdinal ("image_name");
			nameOrdinal = _reader.GetOrdinal ("name");
			priceOrdinal = _reader.GetOrdinal ("price");
			productTypeOrdinal = _reader.GetOrdinal ("product_type");
		}
	}
}
