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
		private string idColumnName;
		private string parentIdColumnName;
		private string nameColumnName;

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

		public ProductTypeNode GetProductTypeNode ()
		{
			ProductTypeNode node = new ProductTypeNode ();
			node.Id = _reader.GetInt32 (GetOridinalFor (idColumnName));
			
			int parentIdOridinal = GetOridinalFor(parentIdColumnName);
			node.ParentId = _reader.IsDBNull (parentIdOridinal) ? (int?)null : _reader.GetInt32 (parentIdOridinal);

			node.Name = _reader.GetString (GetOridinalFor (nameColumnName));

			return node;
		}
	}
}
