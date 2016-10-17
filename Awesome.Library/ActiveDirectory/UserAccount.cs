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
			this.LoadProperties( up );	
		}

		#endregion

		#region " Methods "

		protected virtual void LoadProperties( UserPrincipal up ) {
			this.SID = up.Sid.Value;
			if ( String.IsNullOrEmpty( up.SamAccountName ) == false ) {
				this.UserName = up.SamAccountName;
			} else if ( String.IsNullOrEmpty( up.UserPrincipalName ) == false ) {
				this.UserName = up.UserPrincipalName;
			}
			if ( this.UserName != null ) {
				this.UserName = this.UserName.ToLower();
			}
			this.DisplayName = up.DisplayName;
			this.EmailAddress = ( up.EmailAddress != null ? up.EmailAddress.ToLower() : null );
			this.FirstName = up.GivenName.Capitilize();
			this.LastName = up.Surname.Capitilize();
			this.Company = DirectoryContext.GetPropertyValue<string>( up, "company" );
			this.Department = DirectoryContext.GetPropertyValue<string>( up, "department" );
			this.Title = DirectoryContext.GetPropertyValue<string>( up, "title" );
			this.Photo = this.RetrievePhoto( up );
		}

		protected Image RetrievePhoto( UserPrincipal up ) {
			object photoBytes = DirectoryContext.SearchUser( up, up.SamAccountName, "thumbnailPhoto" );
			if ( photoBytes == null ) {
				photoBytes = DirectoryContext.SearchUser( up, up.SamAccountName, "jpegPhoto" );
			}
			if ( photoBytes != null && photoBytes is byte[] ) {
				MemoryStream ms = new MemoryStream( (byte[])photoBytes );
				return Image.FromStream( ms );
			}
			return null;
		}

		#endregion

	}
}
