/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.IO;

namespace Awesome.Library.Mvc {

	public static class UrlHelperExtensions {

		public static string ToAbsoluteUrl( this UrlHelper helper, string relativeUrl ) {
			if ( string.IsNullOrEmpty( relativeUrl ) )
				return relativeUrl;
			if ( HttpContext.Current == null )
				return relativeUrl;
			if ( relativeUrl.StartsWith( "/" ) )
				relativeUrl = relativeUrl.Insert( 0, "~" );
			if ( !relativeUrl.StartsWith( "~/" ) )
				relativeUrl = relativeUrl.Insert( 0, "~/" );
			var url = HttpContext.Current.Request.Url;
			var port = url.Port != 80 ? ( ":" + url.Port ) : String.Empty;
			return String.Format( "{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute( relativeUrl ) );
		}

	}

}
