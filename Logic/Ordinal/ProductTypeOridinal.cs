using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using Logic.Domain;

namespace Logic.Data
{
	class ProductTypeOridinal : Ordinal
	{
		public string idColumnName { get; private set; }
		public string parentIdColumnName { get; private set; }
		public string nameColumnName { get; private set; }

		public ProductTypeOridinal (SqlDataReader reader)
			: base(reader)
		{
			idColumnName = "id";
			parentIdColumnName = "parent_id";
			nameColumnName = "name";

			SetColumns (new string[]
			{
				idColumnName,
				parentIdColumnName,
				nameColumnName
			});
		}

		public ProductTypeValue GetProductTypeValue ()
		{
			ProductTypeValue ptv = new ProductTypeValue ();
			ptv.ProductType = (ProductType)_reader.GetInt32 (GetOridinalFor (idColumnName));

			int parentIdOridinal = GetOridinalFor (parentIdColumnName);
			ptv.ParentType = (ProductType?)(_reader.IsDBNull (parentIdOridinal) ? (int?)null : _reader.GetInt32 (parentIdOridinal));

			ptv.ProductTypeName = _reader.GetString (GetOridinalFor (nameColumnName));

			return ptv;
		}
	}
}
