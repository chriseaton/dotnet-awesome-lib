/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;

namespace Awesome.Library.Mvc {

	public static class SessionManager {

		#region " Properties "

		private static HttpSessionState Session {
			get {
				if ( HttpContext.Current != null ) {
					return HttpContext.Current.Session;
				}
				return null;
			}
		} 

		#endregion

		#region " Methods "

		public static void Clear() {
			if ( Session != null ) {
				Session.Clear();
			}
		}

		public static void Remove( string key ) {
			if ( Session != null ) {
				Session.Remove( key );
			}
		}

		public static T Get<T>( string key ) where T : class {
			if ( Session != null ) {
				return Session[key] as T;
			}
			return default( T );
		}

		public static void Set<T>( string key, T value ) {
			if ( Session != null ) {
				Session[key] = value;
			}
		}

		public static T LazyLoad<T>( Func<T> f, string key ) {
			if ( Session != null && Session[key] != null ) {
				return (T)Session[key];
			} else {
				T result = f();
				if ( Session != null ) {
					Session.Add( key, result );
				}
				return result;
			}
		}

		#endregion

	}

}