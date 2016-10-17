/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Security.Principal;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Awesome.Library.ActiveDirectory {

	public class DirectoryContext {

		#region " Properties "

		public ProcessPrincipal Process { get; private set; }

		#endregion

		#region " Constructor(s) "

		public DirectoryContext() {
			//populate process principal information
			WindowsIdentity pident = WindowsIdentity.GetCurrent();
			PrincipalContext pc = null;
			try {
				pc = new PrincipalContext( ContextType.Domain );
			} catch {
				pc = new PrincipalContext( ContextType.Machine );
			}
			this.Process = new ProcessPrincipal( pc, pident );
		}

		public DirectoryContext( ContextType ct, string name, string userName, string password ) {
			PrincipalContext pc = new PrincipalContext( ct, name, userName, password );
			WindowsIdentity pident = WindowsIdentity.GetCurrent();
			this.Process = new ProcessPrincipal( pc, pident );
		}

		public DirectoryContext( string domain, string userName, string password )
			: this( ContextType.Domain, domain, userName, password ) { }

		#endregion

		#region " Methods "

		public UserAccount GetUser( string userName ) {
			UserAccount user = null;
			if ( String.IsNullOrEmpty( userName ) == false ) {
				UserPrincipal up = UserPrincipal.FindByIdentity( this.Process.PrincipalContext, userName );
				if ( up != null ) {
					user = new UserAccount( up );
				}
			}
			return user;
		}

		public UserAccount GetWebUser() {
			if ( HttpContext.Current != null
				&& HttpContext.Current.User.Identity != null
				&& HttpContext.Current.User.Identity.IsAuthenticated ) {
				return this.GetUser( HttpContext.Current.User.Identity.Name );
			}
			return null;
		}

		#endregion

		#region " Static Methods "

		public static T GetPropertyValue<T>( Principal p, string property ) {
			object value = DirectoryContext.GetPropertyValue( p, property );
			if ( value != null ) {
				return (T)value;
			}
			return default( T );
		}

		public static object GetPropertyValue( Principal p, string property ) {
			return DirectoryContext.GetPropertyValue( p, property, true );
		}

		public static object GetPropertyValue( Principal p, string property, bool useCache ) {
			DirectoryEntry de = p.GetUnderlyingObject() as DirectoryEntry;
			de.UsePropertyCache = useCache;
			if ( de.Properties.Contains( property ) ) {
				return de.Properties[property].Value;
			}
			return null;
		}

		public static object SearchUser( Principal p, string sAMAccountName, string property ) {
			DirectoryEntry de = p.GetUnderlyingObject() as DirectoryEntry;
			de.UsePropertyCache = false;
			DirectorySearcher search = new DirectorySearcher();
			search.SearchRoot = de;
			search.Filter = "(&(objectClass=user)(objectCategory=person)(sAMAccountName=" + sAMAccountName + "))";
			search.PropertiesToLoad.Add( "samaccountname" );
			search.PropertiesToLoad.Add( property );
			SearchResult r = search.FindOne();
			if ( r != null && r.Properties.Contains(property)) {
				if ( r.Properties[property].Count == 1 ) {
					return r.Properties[property][0];
				} else {
					object[] results = new object[r.Properties[property].Count];
					r.Properties[property].CopyTo( results, 0 );
					return results;
				}
			}
			return null;
		}

		#endregion

	}

}
