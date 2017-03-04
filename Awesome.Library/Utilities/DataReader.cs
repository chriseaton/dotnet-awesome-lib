using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Awesome.Library.Utilities {

	public static class DataReader {

		public static T ReadValue<T>( this IDataReader dr, string column, T defaultValue ) {
			int index = dr.GetOrdinal( column );
			if ( dr.IsDBNull( index ) ) {
				return defaultValue;
			}
			return (T)dr.GetValue( index );
		}

	}
}
