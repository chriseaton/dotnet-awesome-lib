/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using System.Web.Routing;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Awesome.Library.Mvc {

	public static class MvcSiteMap {

		private static string CurrentUrl = null;
		private static string FileName = null;
		private static DateTime FileLastModified = DateTime.MinValue;
		private static readonly Type RouteCollectionType = typeof( RouteCollection );

		private static MvcSiteMapNode m_RootNode = null;
		private static MvcSiteMapNode m_CurrentNode = null;

		#region " Properties "

		public static MvcSiteMapNode RootNode {
			get {
				EnsureLatestSiteMap();
				return MvcSiteMap.m_RootNode;
			}
		}

		public static MvcSiteMapNode CurrentNode {
			get {
				EnsureLatestSiteMap();
				if ( HttpContext.Current != null ) {
					string url = HttpContext.Current.Request.Url.AbsolutePath;
					if ( m_CurrentNode != null && url.Equals( CurrentUrl, StringComparison.InvariantCultureIgnoreCase ) ) {
						return m_CurrentNode;
					} else {
						//find current node
						string controller = null;
						string action = null;
						string area = null;
						string routeName = null;
						CurrentRoute( out controller, out action, out area, out routeName );
						if ( controller != null && action != null ) {
							CurrentUrl = url;
							m_CurrentNode = FindNode( action, controller, area, routeName );
							if ( m_CurrentNode == null && String.IsNullOrEmpty( routeName ) == false ) {
								//try to get the current node w/o a route name specified since no exact match could
								//be found in the node collection
								m_CurrentNode = FindNode( action, controller, area, null );
							}
							return m_CurrentNode;
						}
					}
				}
				return null;
			}
		}

		private static MvcHandler Handler {
			get {
				if ( HttpContext.Current != null && HttpContext.Current.Handler != null ) {
					return HttpContext.Current.Handler as MvcHandler;
				}
				return null;
			}
		}

		#endregion

		#region " Constructor(s) "

		static MvcSiteMap() {
			MvcSiteMap.FileName = System.Web.Hosting.HostingEnvironment.MapPath( "~/App_Data/SiteMap.xml" );
			LoadSiteMap( MvcSiteMap.FileName );
		}

		#endregion

		#region " Methods "

		/// <summary>
		/// Returns the page title element for use in the html head element.
		/// </summary>
		/// <param name="prefix">The default site title.</param>
		public static MvcHtmlString HeadTitle( string prefix ) {
			string title = MvcSiteMap.Title();
			if ( String.IsNullOrEmpty( title ) ) {
				return new MvcHtmlString( "<title>" + prefix + "</title>" );
			}
			return new MvcHtmlString( "<title>" + prefix + " - " + title + "</title>" );
		}

		public static string Title() {
			if ( MvcSiteMap.CurrentNode != null ) {
				return TitleFor( m_CurrentNode.Action, m_CurrentNode.Controller, m_CurrentNode.Area, m_CurrentNode.RouteName );
			}
			return String.Empty;
		}

		public static string TitleFor( string action, string controller ) {
			return TitleFor( action, controller, String.Empty, null );
		}

		public static string TitleFor( string action, string controller, string area ) {
			return TitleFor( action, controller, area, null );
		}

		public static string TitleFor( string action, string controller, string area, string routeName ) {
			MvcSiteMapNode node = FindNode( action, controller, area, routeName );
			if ( node != null ) {
				return node.Title;
			}
			return String.Empty;
		}

		public static MvcSiteMapNode FindNode( string action, string controller ) {
			return FindNode( action, controller, String.Empty, null );
		}

		public static MvcSiteMapNode FindNode( string action, string controller, string area, string routeName ) {
			bool isRouteNameEmpty = String.IsNullOrWhiteSpace( routeName );
			if ( m_RootNode.Controller.Equals( controller, StringComparison.InvariantCultureIgnoreCase )
				&& m_RootNode.Action.Equals( action, StringComparison.InvariantCultureIgnoreCase )
				&& ( m_RootNode.Area ?? String.Empty ).Equals( area ?? String.Empty, StringComparison.InvariantCultureIgnoreCase )
				&& ( isRouteNameEmpty || ( m_RootNode.RouteName ?? String.Empty ).Equals( routeName ?? String.Empty, StringComparison.InvariantCultureIgnoreCase ) ) ) {
				return m_RootNode;
			} else {
				foreach ( MvcSiteMapNode node in m_RootNode.Descendents() ) {
					if ( node.Controller.Equals( controller, StringComparison.InvariantCultureIgnoreCase )
						&& node.Action.Equals( action, StringComparison.InvariantCultureIgnoreCase )
						&& ( node.Area ?? String.Empty ).Equals( ( area ?? String.Empty ), StringComparison.InvariantCultureIgnoreCase )
						&& ( isRouteNameEmpty || ( node.RouteName ?? String.Empty ).Equals( routeName ?? String.Empty, StringComparison.InvariantCultureIgnoreCase ) ) ) {
						return node;
					}
				}
			}
			return null;
		}

		public static void CurrentRoute( out string controller, out string action, out string area, out string routeName ) {
			MvcHandler handler = MvcSiteMap.Handler;
			if ( handler != null ) {
				CurrentRoute( handler.RequestContext, out controller, out action, out area, out routeName );
				return;
			}
			controller = action = area = routeName = null;
		}

		public static void CurrentRoute( RequestContext reqContext, out string controller, out string action, out string area, out string routeName ) {
			if ( reqContext != null ) {
				controller = reqContext.RouteData.Values["controller"] as string;
				action = reqContext.RouteData.Values["action"] as string;
				area = reqContext.RouteData.DataTokens["area"] as string;
				routeName = FindRouteName( RouteTable.Routes, reqContext.RouteData.Route );
				return;
			}
			controller = action = area = routeName = null;
		}

		/// <summary>
		/// Finds the route name for the given RouteBase object in the given route collection.
		/// </summary>
		private static string FindRouteName( RouteCollection routes, RouteBase match ) {
			System.Reflection.FieldInfo rcField = RouteCollectionType.GetField( "_namedMap", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance );
			object mapObj = rcField.GetValue( routes );
			Dictionary<string, RouteBase> map = (Dictionary<string, RouteBase>)mapObj;
			if ( map != null ) {
				foreach ( var kvp in map ) {
					if ( kvp.Value == match ) {
						return kvp.Key;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Checks if the sitemap file has been modified, and if so, reloads it.
		/// </summary>
		private static void EnsureLatestSiteMap() {
			if ( File.GetLastWriteTime( MvcSiteMap.FileName ) > FileLastModified ) {
				LoadSiteMap( MvcSiteMap.FileName );
			}
		}

		/// <summary>
		/// Loads the site map from the xml file.
		/// </summary>
		public static void LoadSiteMap( string fileName ) {
			if ( File.Exists( fileName ) ) {
				XDocument xdoc = XDocument.Load( fileName );
				if ( xdoc.Root != null ) {
					XElement firstNode = xdoc.Root.Element( "Node" );
					if ( firstNode != null ) {
						m_RootNode = NodeFromElement( firstNode, null );
						FileLastModified = File.GetLastWriteTime( fileName );
					}
				}
			}
		}

		/// <summary>
		/// Parses a sitemap xml element into a site node object.
		/// </summary>
		private static MvcSiteMapNode NodeFromElement( XElement nodeElement, MvcSiteMapNode parent ) {
			MvcSiteMapNode node = new MvcSiteMapNode();
			node.Parent = parent;
			if ( nodeElement.Attribute( "controller" ) != null ) {
				node.Controller = nodeElement.Attribute( "controller" ).Value;
			}
			if ( nodeElement.Attribute( "action" ) != null ) {
				node.Action = nodeElement.Attribute( "action" ).Value;
			}
			if ( nodeElement.Attribute( "area" ) != null ) {
				node.Area = nodeElement.Attribute( "area" ).Value;
			} else {
				node.Area = String.Empty;
			}
			if ( nodeElement.Attribute( "routeName" ) != null ) {
				node.RouteName = nodeElement.Attribute( "routeName" ).Value;
				if ( RouteTable.Routes[node.RouteName] == null ) {
					throw new ApplicationException( "The route '" + node.RouteName + "' specifed in the sitemap is invalid." );
				}
			}
			if ( nodeElement.Attribute( "title" ) != null ) {
				node.Title = nodeElement.Attribute( "title" ).Value;
			}
			if ( nodeElement.Attribute( "description" ) != null ) {
				node.Description = nodeElement.Attribute( "description" ).Value;
			}
			if ( nodeElement.Attribute( "keywords" ) != null ) {
				node.Keywords = nodeElement.Attribute( "keywords" ).Value;
			}
			if ( nodeElement.Attribute( "display" ) != null ) {
				node.Display = (MvcSiteMapDisplay)Enum.Parse( typeof( MvcSiteMapDisplay ), nodeElement.Attribute( "display" ).Value );
			}
			if ( nodeElement.HasElements ) {
				foreach ( XElement childElement in nodeElement.Elements( "Node" ) ) {
					node.ChildNodes.Add( NodeFromElement( childElement, node ) );
				}
			}
			return node;
		}

		#endregion

	}

}
