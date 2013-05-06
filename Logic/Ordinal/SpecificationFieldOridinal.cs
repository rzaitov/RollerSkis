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
	internal class SpecificationFieldOridinal : Ordinal
	{
		public int idOrdinal = -1;
		public int nameOrdinal = -1;
		public int valueOrdinal = -1;

		private SqlDataReader _reader;

		public SpecificationFieldOridinal (SqlDataReader reader)
			: base(reader)
		{
			_reader = reader;
		}

		protected override void LoadOrdinalValues ()
		{
			idOrdinal = _reader.GetOrdinal ("id");
			nameOrdinal = _reader.GetOrdinal ("name");
			valueOrdinal = _reader.GetOrdinal ("value");
		}

		public void LoadOneSpecificationTo (Product product)
		{
			string key = _reader.GetString(nameOrdinal);
			product.Specification[key] = _reader.GetString (valueOrdinal);
		}
	}
}
