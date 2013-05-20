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
		protected SqlDataReader _reader;
		protected List<ColumnDescriptor> _columnDescriptors { get; private set; }

		public Ordinal (SqlDataReader reader)
		{
			_reader = reader;
			_columnDescriptors = new List<ColumnDescriptor> ();
		}

		protected void SetColumns (IEnumerable<string> columnNames)
		{
			_columnDescriptors.AddRange (columnNames.Select (cn => new ColumnDescriptor { ColumnName = cn }));
		}

		public int GetOridinalFor (string columnName)
		{
			return _columnDescriptors.First (cd => cd.ColumnName == columnName).Ordinal;
		}

		public void SetAliasForColumn (string columnName, string alias)
		{
			ColumnDescriptor descriptor = _columnDescriptors.First (cd => cd.ColumnName == columnName);
			descriptor.Alias = alias;
		}

		public virtual bool LoadOrdinals ()
		{
			bool hasRows = _reader.HasRows;
			if (hasRows)
			{
				foreach (ColumnDescriptor cd in _columnDescriptors)
				{
					cd.Ordinal = _reader.GetOrdinal (cd.Alias);
				}
			}

			return hasRows;
		}
	}

	internal class ColumnDescriptor
	{
		public int Ordinal { get; set; }

		private string _columnName;
		public string ColumnName
		{
			get { return _columnName; }
			set
			{
				_columnName = value;
				Alias = value;
			}
		}

		public string Alias { get; set; }
	}
}
