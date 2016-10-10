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
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using Awesome.Library.Utilities;

namespace Awesome.Library.Mvc {

	public static class HtmlHelperExtensions {

		/// <summary>
		/// Detect the current browser from the http request and returns a css class tag for it. 
		/// If the browser is not known, an empty string is returned.
		/// </summary>
		public static string BrowserCssClass( this HtmlHelper helper ) {
			if ( helper.ViewContext.HttpContext is HttpContextWrapper ) {
				HttpBrowserCapabilitiesBase browser = ( (HttpContextWrapper)helper.ViewContext.HttpContext ).Request.Browser;
				if ( browser.Browser.Equals( "IE", StringComparison.InvariantCultureIgnoreCase ) ) {
					return "ie ie-" + ( browser.MajorVersion > 6 ? browser.MajorVersion.ToString() : "legacy" );
				} else if ( browser.Browser.Equals( "Firefox", StringComparison.InvariantCultureIgnoreCase ) ) {
					return "ff ff-" + ( browser.MajorVersion > 12 ? browser.MajorVersion.ToString() : "legacy" );
				} else if ( browser.Browser.Equals( "Opera", StringComparison.InvariantCultureIgnoreCase ) ) {
					return "op op-" + ( browser.MajorVersion > 9 ? browser.MajorVersion.ToString() : "legacy" );
				} else if ( browser.Browser.Equals( "Chrome", StringComparison.InvariantCultureIgnoreCase ) ) {
					return "ch ch-" + ( browser.MajorVersion > 17 ? browser.MajorVersion.ToString() : "legacy" );
				}
			}
			return String.Empty;
		}

		/// <summary>
		/// Adds a conditional css class to a set of css classes if the condition value is true.
		/// </summary>
		public static string ConditionalCssClass( this HtmlHelper helper, bool condition, string conditionalClass, params string[] regularClasses ) {
			StringBuilder sb = new StringBuilder();
			if ( regularClasses != null ) {
				sb.Append( String.Join( " ", regularClasses ) );
			}
			if ( condition ) {
				if ( sb.Length > 0 )
					sb.Append( ' ' );
				sb.Append( conditionalClass );
			}
			return sb.ToString();
		}

		/// <summary>
		/// Adds a conditional css class to a set of css classes if the the current controller and action match those specified
		/// </summary>
		public static string ConditionalCssClass( this HtmlHelper helper, string action, string controller, string area, string conditionalClass, params string[] regularClasses ) {
			StringBuilder sb = new StringBuilder();
			if ( regularClasses != null ) {
				sb.Append( String.Join( " ", regularClasses ) );
			}
			string currentController;
			string currentAction;
			string currentArea;
			string currentRouteName;
			bool addConditional = false;
			MvcSiteMap.CurrentRoute( helper.ViewContext.RequestContext, out currentController, out currentAction, out currentArea, out currentRouteName );
			if (
				( controller == null || currentController.Equals( controller, StringComparison.InvariantCultureIgnoreCase ) )
				&& ( action == null || currentAction.Equals( action, StringComparison.InvariantCultureIgnoreCase ) )
				&& ( currentArea ?? String.Empty ).Equals( ( area ?? String.Empty ), StringComparison.InvariantCultureIgnoreCase ) ) {
				addConditional = true;
			}
			if ( addConditional ) {
				if ( sb.Length > 0 )
					sb.Append( ' ' );
				sb.Append( conditionalClass );
			}
			return sb.ToString();
		}

		/// <summary>
		/// Renders an MS Chart to the html document.
		/// </summary>
		//public static IHtmlString Chart( this HtmlHelper helper, Chart chart ) {
		//	if ( chart.RenderType == RenderType.BinaryStreaming ) {
		//		throw new NotSupportedException( "The RenderType 'BinaryStreaming' is not supported when rendering the chart to an HTML output." );
		//	}
		//	StringBuilder sb = new StringBuilder();
		//	HtmlTextWriter writer = new HtmlTextWriter( new StringWriter( sb, System.Globalization.CultureInfo.InvariantCulture ) );
		//	chart.RenderControl( writer );
		//	return MvcHtmlString.Create( sb.ToString() );
		//}

		/// <summary>
		/// Renders out navigation breadcrums.
		/// </summary>
		public static IHtmlString Breadcrumbs( this HtmlHelper helper ) {
			return Breadcrumbs( helper, null, true );
		}

		/// <summary>
		/// Renders out navigation breadcrums.
		/// </summary>
		public static IHtmlString Breadcrumbs( this HtmlHelper helper, string seperator ) {
			return Breadcrumbs( helper, seperator, true );
		}

		/// <summary>
		/// Renders out navigation breadcrums.
		/// </summary>
		public static IHtmlString Breadcrumbs( this HtmlHelper helper, string seperator, bool currentNodeAsLink ) {
			StringBuilder sb = new StringBuilder();
			//create parent node chain
			List<MvcSiteMapNode> nodesOrdered = new List<MvcSiteMapNode>();
			if ( MvcSiteMap.CurrentNode != null ) {
				nodesOrdered.AddRange( MvcSiteMap.CurrentNode.ParentChain() );
				if ( ( MvcSiteMap.CurrentNode.Display & MvcSiteMapDisplay.Crumbs ) != MvcSiteMapDisplay.Crumbs ) {
					return MvcHtmlString.Empty;
				}
			} else if ( MvcSiteMap.RootNode != null ) {
				nodesOrdered.Add( MvcSiteMap.RootNode );
			}
			//render the breadcrumbs
			sb.Append( "<span class=\"sitemap-breadcrumbs\">" );
			UrlHelper url = new UrlHelper( helper.ViewContext.RequestContext );
			foreach ( MvcSiteMapNode node in nodesOrdered ) {
				bool createLink = true;
				bool createSeperator = true;
				sb.Append( "<span class=\"sitemap-crumb\">" );
				if ( node == MvcSiteMap.CurrentNode ) {
					createSeperator = false;
					if ( currentNodeAsLink == false ) {
						createLink = false;
						sb.Append( node.Title );
					}
				}
				if ( createLink ) {
					sb.Append( "<a href=\"" );
					if ( String.IsNullOrWhiteSpace( node.RouteName ) ) {
						sb.Append( url.Action( node.Action, node.Controller, new { area = node.Area } ) );
					} else {
						sb.Append( url.RouteUrl( node.RouteName ) );
					}
					sb.Append( "\">" );
					sb.Append( node.Title );
					sb.Append( "</a>" );
				}
				sb.Append( "</span>" );
				if ( createSeperator ) {
					sb.Append( "<span class=\"sitemap-crumb-sep\">" );
					sb.Append( seperator ?? "&nbsp;>&nbsp;" );
					sb.Append( "</span>" );
				}
			}
			sb.Append( "</span>" );
			return MvcHtmlString.Create( sb.ToString() );
		}

		/// <summary>
		/// Renders out a ul tag with all site map nodes underneath.
		/// </summary>
		public static IHtmlString SiteMapList( this HtmlHelper helper ) {
			MvcSiteMapNode node = MvcSiteMap.CurrentNode;
			return SiteMapList( helper, node.Action, node.Controller, node.Area ?? String.Empty, null, true, -1, true, true );
		}

		/// <summary>
		/// Renders out a ul tag with all site map nodes underneath.
		/// </summary>
		public static IHtmlString SiteMapList( this HtmlHelper helper, bool showRootNode, int levels ) {
			MvcSiteMapNode node = MvcSiteMap.CurrentNode;
			return SiteMapList( helper, node.Action, node.Controller, node.Area ?? String.Empty, null, showRootNode, levels, true, true );
		}

		/// <summary>
		/// Renders out a ul tag with all site map nodes underneath.
		/// </summary>
		public static IHtmlString SiteMapList( this HtmlHelper helper, bool showRootNode, int levels, bool asLinks ) {
			MvcSiteMapNode node = MvcSiteMap.CurrentNode;
			return SiteMapList( helper, node.Action, node.Controller, node.Area ?? String.Empty, null, showRootNode, levels, asLinks, true );
		}

		/// <summary>
		/// Renders out a ul tag with all site map nodes underneath.
		/// </summary>
		public static IHtmlString SiteMapList( this HtmlHelper helper, string action, string controller, string area, string routeName, bool showRootNode, int levels, bool asLinks, bool showDescriptions ) {
			StringBuilder sb = new StringBuilder();
			MvcSiteMapNode startNode = MvcSiteMap.FindNode( action, controller, area, routeName );
			//render the list
			UrlHelper url = new UrlHelper( helper.ViewContext.RequestContext );
			if ( ( startNode.Display & MvcSiteMapDisplay.SiteMap ) == MvcSiteMapDisplay.SiteMap ) {
				SiteMapListRenderNode( url, sb, startNode, true, showRootNode, levels, 0, asLinks, showDescriptions );
			}
			return MvcHtmlString.Create( sb.ToString() );
		}

		/// <summary>
		/// Recursive rendering method for building out a sitemap list.
		/// </summary>
		private static void SiteMapListRenderNode( UrlHelper helper, StringBuilder sb, MvcSiteMapNode node, bool isRoot, bool showRootNode, int levels, int currentLevel, bool asLinks, bool showDescriptions ) {
			if ( isRoot ) {
				sb.AppendLine( "<ul class=\"sitemap-list\">" );
			}
			if ( isRoot == false || ( isRoot && showRootNode ) ) {
				if ( node == MvcSiteMap.CurrentNode ) {
					sb.Append( "<li class=\"selected\">" );
				} else {
					sb.Append( "<li>" );
				}
				if ( asLinks ) {
					sb.Append( "<a href=\"" );
					if ( String.IsNullOrWhiteSpace( node.RouteName ) ) {
						sb.Append( helper.Action( node.Action, node.Controller, new { area = node.Area } ) );
					} else {
						sb.Append( helper.RouteUrl( node.RouteName ) );
					}
					sb.Append( "\">" );
					sb.Append( node.Title );
					sb.Append( "</a>" );
				} else {
					sb.Append( node.Title );
				}
				if ( showDescriptions && String.IsNullOrWhiteSpace( node.Description ) == false ) {
					sb.Append( "<p>" );
					sb.Append( node.Description.Trim() );
					sb.Append( "</p>" );
				}
			}
			currentLevel++;
			if ( levels > 0 && levels >= currentLevel ) {
				if ( node.ChildNodes.Count > 0 ) {
					if ( isRoot == false || ( isRoot && showRootNode ) ) {
						sb.AppendLine();
						sb.AppendLine( "<ul>" );
					}
					foreach ( MvcSiteMapNode n in node.ChildNodes ) {
						if ( ( n.Display & MvcSiteMapDisplay.SiteMap ) == MvcSiteMapDisplay.SiteMap ) {
							SiteMapListRenderNode( helper, sb, n, false, false, levels, currentLevel, asLinks, showDescriptions );
						}
					}
					if ( isRoot == false || ( isRoot && showRootNode ) ) {
						sb.AppendLine( "</ul>" );
					}
				}
			}
			sb.AppendLine( "</li>" );
			if ( isRoot ) {
				sb.AppendLine( "</ul>" );
			}
		}

		/// <summary>
		/// Gets the ShortName value from the DisplayAttribute on a model property
		/// </summary>
		public static IHtmlString ShortNameFor<TModel, TValue>( this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression ) {
			var metadata = ModelMetadata.FromLambdaExpression( expression, helper.ViewData );
			var content = metadata.ShortDisplayName ?? String.Empty;
			return new HtmlString( content );
		}

		/// <summary>
		/// Gets the Description value from the DisplayAttribute on a model property
		/// </summary>
		public static IHtmlString DescriptionFor<TModel, TValue>( this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression ) {
			var metadata = ModelMetadata.FromLambdaExpression( expression, helper.ViewData );
			var content = metadata.Description ?? String.Empty;
			return new HtmlString( content );
		}

		/// <summary>
		/// Creates a link to google maps with the given address information
		/// </summary>
		public static IHtmlString GoogleMapLink( this HtmlHelper helper, string text, string address, string city, string state, string zip, double latitude, double longitude ) {
			return GoogleMapLink( helper, text, address, city, state, zip, latitude, longitude, 14 );
		}

		/// <summary>
		/// Creates a link to google maps with the given address information
		/// </summary>
		public static IHtmlString GoogleMapLink( this HtmlHelper helper, string text, string address, string city, string state, string zip, double latitude, double longitude, int zoom ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "<a href=\"http://maps.google.com/?t=h&q=loc:" );
			if ( latitude != -1 && latitude != 0 && longitude != 0 && longitude != -1 ) {
				sb.Append( latitude );
				sb.Append( ',' );
				sb.Append( longitude );
			} else {
				sb.Append( HttpUtility.UrlEncode( address.SafePostfix( " " ) ) );
				sb.Append( HttpUtility.UrlEncode( Strings.CityStateZip( city, state, zip ) ) );
			}
			sb.Append( "&z=" );
			sb.Append( zoom );
			sb.Append( "\" class=\"google-map-link\" target=\"_blank\">" );
			sb.Append( text );
			sb.Append( "</a>" );
			return new HtmlString( sb.ToString() );
		}

		/// <summary>
		/// Creates a static google map with one marker on it.
		/// </summary>
		public static IHtmlString GoogleMap( this HtmlHelper helper, string address, string city, string state, string zip, double latitude, double longitude ) {
			return GoogleMap( helper, address, city, state, zip, latitude, longitude, 14, 200, 200, true );
		}

		/// <summary>
		/// Creates a static google map with one marker on it.
		/// </summary>
		public static IHtmlString GoogleMap( this HtmlHelper helper, string address, string city, string state, string zip, double latitude, double longitude, int zoom ) {
			return GoogleMap( helper, address, city, state, zip, latitude, longitude, zoom, 200, 200, true );
		}

		/// <summary>
		/// Creates a static google map with one marker on it.
		/// </summary>
		public static IHtmlString GoogleMap( this HtmlHelper helper, string address, string city, string state, string zip, double latitude, double longitude, int zoom, int width, int height, bool asLink ) {
			StringBuilder sb = new StringBuilder();
			bool valid = ( latitude != -1 && latitude != 0 && longitude != 0 && longitude != -1 )
						 || ( String.IsNullOrWhiteSpace( address ) == false && ( ( String.IsNullOrWhiteSpace( city ) == false && String.IsNullOrWhiteSpace( state ) == false ) || ( String.IsNullOrWhiteSpace( zip ) == false ) ) );
			if ( valid ) {
				if ( asLink ) {
					sb.Append( "<a href=\"http://maps.google.com/?t=h&q=loc:" );
					if ( latitude != -1 && latitude != 0 && longitude != 0 && longitude != -1 ) {
						sb.Append( latitude );
						sb.Append( ',' );
						sb.Append( longitude );
					} else {
						sb.Append( HttpUtility.UrlEncode( address.SafePostfix( " " ) ) );
						sb.Append( HttpUtility.UrlEncode( Strings.CityStateZip( city, state, zip ) ) );
					}
					sb.Append( "&z=" );
					sb.Append( zoom );
					sb.Append( "\" class=\"google-map-link\" target=\"_blank\">" );
				}
				sb.Append( "<img src=\"http://maps.googleapis.com/maps/api/staticmap?center=" );
				StringBuilder center = new StringBuilder();
				if ( latitude != -1 && latitude != 0 && longitude != 0 && longitude != -1 ) {
					center.Append( latitude );
					center.Append( ',' );
					center.Append( longitude );
				} else {
					center.Append( HttpUtility.UrlEncode( address.SafePostfix( " " ) ) );
					center.Append( HttpUtility.UrlEncode( Strings.CityStateZip( city, state, zip ) ) );
				}
				sb.Append( center );
				sb.Append( "&zoom=" );
				sb.Append( zoom );
				sb.Append( "&sensor=false&markers=" );
				sb.Append( HttpUtility.UrlEncode( "color:0xFF2E29|" + center.ToString() ) );
				sb.Append( "&size=" );
				sb.Append( width );
				sb.Append( 'x' );
				sb.Append( height );
				sb.Append( "\" class=\"google-map-img " );
				if ( width == height ) {
					sb.Append( "size" );
					sb.Append( width );
				} else {
					sb.Append( "size" );
					sb.Append( width );
					sb.Append( "-" );
					sb.Append( height );
				}
				sb.Append( "\" alt=\"View in Google Maps\">" );
				if ( asLink ) {
					sb.Append( "</a>" );
				}
			}
			return new HtmlString( sb.ToString() );
		}

	}

}
