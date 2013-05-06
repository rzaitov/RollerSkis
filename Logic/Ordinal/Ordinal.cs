using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace Logic.Data
{
	internal abstract class Ordinal
	{
		SqlDataReader _reader;

		public Ordinal (SqlDataReader reader)
		{
			_reader = reader;
		}

		public virtual bool LoadOrdinals ()
		{
			bool hasRows = _reader.HasRows;
			if (hasRows)
			{
				LoadOrdinalValues ();
			}

			return hasRows;
		}

		protected abstract void LoadOrdinalValues ();
	}
}
