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
using System.Drawing;
using System.DirectoryServices.AccountManagement;
using Awesome.Library.Utilities;

namespace Awesome.Library.ActiveDirectory {

	public class UserAccount {

		#region " Properties "

		public string SID { get; set; }

		public string UserName { get; set; }

		public string EmailAddress { get; set; }

		public string DisplayName { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Title { get; set; }

		public string Company { get; set; }

		public string Department { get; set; }

		public Image Photo { get; set; }

		#endregion

		#region " Constructor(s) "

		public UserAccount() { }

		public UserAccount( UserPrincipal up ) {
			this.SID = up.Sid.Value;
			if ( String.IsNullOrEmpty( up.UserPrincipalName ) == false ) {
				this.UserName = up.UserPrincipalName;
			} else if ( String.IsNullOrEmpty( up.SamAccountName ) == false ) {
				this.UserName = up.SamAccountName;
			} else {
				this.UserName = up.Name;
			}
			if ( this.UserName != null ) {
				this.UserName = this.UserName.ToLower();
			}
			this.DisplayName = up.DisplayName;
			this.EmailAddress = ( up.EmailAddress != null ? up.EmailAddress.ToLower() : null );
			this.FirstName = up.GivenName.Capitilize();
			this.LastName = up.Surname.Capitilize();
			this.Company = DirectoryContext.GetProperty<string>( up, "company" );
			this.Department = DirectoryContext.GetProperty<string>( up, "department" );
			this.Title = DirectoryContext.GetProperty<string>( up, "title" );
			Stream photoStream = DirectoryContext.GetPropertyStream( up, "jpegPhoto" );
			if ( photoStream != null ) {
				this.Photo = Image.FromStream( photoStream );
			}
		}

		#endregion

		#region " Methods "

		#endregion

	}
}
