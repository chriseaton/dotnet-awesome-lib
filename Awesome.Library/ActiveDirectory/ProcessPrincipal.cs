/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;

namespace Awesome.Library.ActiveDirectory {

	public class ProcessPrincipal {

		#region " Properties "

		public PrincipalContext PrincipalContext { get; private set; }

		public string Identity { get; private set; }

		public string IdentitySID { get; private set; }

		public string DomainSID { get; private set; }

		#endregion

		#region " Constructor(s) "

		public ProcessPrincipal( PrincipalContext pc, WindowsIdentity ident) {
			this.PrincipalContext = pc;
			this.Identity = ident.Name;
			this.IdentitySID = ident.User.Value;
			if ( ident.User.AccountDomainSid != null ) {
				this.DomainSID = ident.User.AccountDomainSid.Value;
			}

		}

		#endregion

	}

}
