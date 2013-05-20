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
		private string idColumnName;
		private string nameColumnName;
		private string valueColumnName;

		public SpecificationFieldOridinal (SqlDataReader reader)
			: base(reader)
		{
			idColumnName = "id";
			nameColumnName = "name";
			valueColumnName = "value";

			SetColumns (new string[] { idColumnName, nameColumnName, valueColumnName });
		}

		public void LoadOneSpecificationTo (Product product)
		{
			int oridinal = GetOridinalFor(nameColumnName);

			// Не для всех товаров есть технические характеристики
			bool specExist = !_reader.IsDBNull(oridinal);

			if (specExist)
			{
				string key = _reader.GetString(oridinal);
				product.Specification[key] = _reader.GetString(GetOridinalFor(valueColumnName));
			}
		}
	}
}
