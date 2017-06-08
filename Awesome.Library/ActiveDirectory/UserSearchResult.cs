/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Security.Principal;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Awesome.Library.Utilities;

namespace Awesome.Library.ActiveDirectory {

    public class UserSearchResult {

        #region " Properties "

        public string SID { get; set; }

        public string Domain { get; set; }

        public string NETBIOSUserName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string DisplayName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public string Department { get; set; }

        #endregion

        #region " Constructor(s) "

        public UserSearchResult() { }

        public UserSearchResult(UserPrincipal up) {
            this.LoadProperties(up);
        }

        public UserSearchResult(DirectoryEntry de) {
            this.LoadProperties(de);
        }

        #endregion

        #region " Methods "

        protected virtual void LoadProperties(UserPrincipal up) {
            this.LoadProperties(up.GetUnderlyingObject() as DirectoryEntry);
        }

        protected virtual void LoadProperties(DirectoryEntry de) {
            this.SID = new SecurityIdentifier(DirectoryContext.GetPropertyValue<byte[]>(de, "objectSid"), 0).ToString();
            string samAccountName = DirectoryContext.GetPropertyValue<string>(de, "sAMAccountName"); ;
            string upn = DirectoryContext.GetPropertyValue<string>(de, "userPrincipalName"); ;
            //get the username as best possible.
            if (String.IsNullOrEmpty(samAccountName) == false) {
                this.UserName = samAccountName.ToLower();
            } else if (String.IsNullOrEmpty(upn) == false) {
                this.UserName = upn.ToLower();
            }
            //attempt to discover the domain name
            DirectoryEntry domainDE = this.GetDomainEntry(de);
            if (domainDE != null) {
                int dcNameIndex = domainDE.Name.LastIndexOf("DC=");
                if (dcNameIndex > -1) {
                    this.Domain = domainDE.Name.Substring(dcNameIndex + 3).ToLower();
                } else {
                    this.Domain = domainDE.Name.ToLower();
                }
                this.NETBIOSUserName = (this.Domain + "\\" + this.UserName).ToLower();
            }
            //get other properties.
            this.DisplayName = DirectoryContext.GetPropertyValue<string>(de, "displayName");
            this.EmailAddress = DirectoryContext.GetPropertyValue<string>(de, "mail");
            if (this.EmailAddress != null) {
                this.EmailAddress = this.EmailAddress.ToLower();
            }
            this.FirstName = DirectoryContext.GetPropertyValue<string>(de, "givenName").Capitilize();
            this.LastName = DirectoryContext.GetPropertyValue<string>(de, "sn").Capitilize();
            this.Company = DirectoryContext.GetPropertyValue<string>(de, "company");
            this.Department = DirectoryContext.GetPropertyValue<string>(de, "department");
            this.Title = DirectoryContext.GetPropertyValue<string>(de, "title");
        }

        protected virtual DirectoryEntry GetDomainEntry(DirectoryEntry de) {
            de = de.Parent;
            while (de != null) {
                if (de.SchemaClassName == "domainDNS") {
                    return de;
                }
                de = de.Parent;
            }
            return null;
        }

        #endregion

    }
}
