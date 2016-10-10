/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Awesome.Library.Mvc {

	public enum MvcSiteMapDisplay {
		All = 0x0F,
		SiteMap = 0x01,
		Crumbs = 0x02,
		Header = 0x04,
		Footer = 0x08
	}

	public class MvcSiteMapNode {

		private List<MvcSiteMapNode> m_ChildNodes = null;
		private MvcSiteMapDisplay m_Display = MvcSiteMapDisplay.All;

		#region " Properties "

		public MvcSiteMapNode Parent { get; set; }

		public List<MvcSiteMapNode> ChildNodes {
			get { return m_ChildNodes; }
			protected set { m_ChildNodes = value; }
		}

		public string Title { get; set; }

		public string RouteName { get; set; }

		public string Action { get; set; }

		public string Controller { get; set; }

		public string Area { get; set; }

		public string Description { get; set; }

		public string Keywords { get; set; }

		public MvcSiteMapDisplay Display {
			get { return m_Display; }
			set { m_Display = value; }
		}

		#endregion

		#region " Constructor(s) "

		public MvcSiteMapNode() {
			m_ChildNodes = new List<MvcSiteMapNode>();
		}

		#endregion

		#region " Methods "

		/// <summary>
		/// Returns an ordered chain from the top-most parent to the current node.
		/// </summary>
		public IEnumerable<MvcSiteMapNode> ParentChain() {
			List<MvcSiteMapNode> nodesOrdered = new List<MvcSiteMapNode>();
			MvcSiteMapNode node = this;
			while ( node != null ) {
				nodesOrdered.Insert( 0, node );
				node = node.Parent;
			}
			return nodesOrdered;
		}

		/// <summary>
		/// Returns all descendant nodes.
		/// </summary>
		public IEnumerable<MvcSiteMapNode> Descendents() {
			return Descendents( this );
		}

		#endregion

		#region " Static Methods "

		public static IEnumerable<MvcSiteMapNode> Descendents( MvcSiteMapNode node ) {
			if ( node == null )
				throw new ArgumentNullException( "node" );
			if ( node.ChildNodes.Count > 0 ) {
				foreach ( MvcSiteMapNode child in node.ChildNodes ) {
					yield return child;
					foreach ( MvcSiteMapNode desc in Descendents( child ) ) {
						yield return desc;
					}
				}
			}
		}

		#endregion

	}

}
