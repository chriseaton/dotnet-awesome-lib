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

		#endregion

		#region " Methods "

		public UserAccount GetWebUser() {
			string userName = null;
			UserAccount user = null;
			if ( HttpContext.Current != null
				&& HttpContext.Current.User.Identity != null
				&& HttpContext.Current.User.Identity.IsAuthenticated ) {
				userName = HttpContext.Current.User.Identity.Name;
				UserPrincipal up = UserPrincipal.FindByIdentity( this.Process.PrincipalContext, userName );
				user = new UserAccount( up );
			}
			return user;
		}

		#endregion

		#region " Static Methods "

		public static T GetProperty<T>( Principal p, string property ) {
			DirectoryEntry de = p.GetUnderlyingObject() as DirectoryEntry;
			if ( de.Properties.Contains( property ) ) {
				return (T)de.Properties[property].Value;
			}
			return default( T );
		}

		public static MemoryStream GetPropertyStream( Principal p, string property ) {
			DirectoryEntry de = p.GetUnderlyingObject() as DirectoryEntry;
			if ( de.Properties.Contains( property ) ) {
				if ( de.Properties[property].Value is byte[] ) {
					return new MemoryStream( de.Properties[property].Value as byte[] );
				} else if ( de.Properties[property].Value is string) {
					return new MemoryStream( Encoding.UTF8.GetBytes( de.Properties[property].Value as string ) );
				}
			}
			return null;
		}

		#endregion

	}

}
