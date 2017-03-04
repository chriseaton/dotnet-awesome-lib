/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Awesome.Library.Utilities {

	public static class Objects {

		/// <summary>
		/// Returns the default value of T if the object is null, otherwise directly casts
		/// </summary>
		public static T SafeCast<T>( this object obj ) {
			if ( obj == null )
				return default( T );
			return (T)obj;
		}

		/// <summary>
		/// Returns the default value of T if the object is null, otherwise directly casts
		/// </summary>
		public static T SafeCast<T>( this object obj, T defaultValue ) {
			if ( obj == null )
				return defaultValue;
			return (T)obj;
		}


	}

}
