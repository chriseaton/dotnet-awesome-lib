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
                pc = new PrincipalContext(ContextType.Domain);
            } catch {
                pc = new PrincipalContext(ContextType.Machine);
            }
            this.Process = new ProcessPrincipal(pc, pident);
        }

        public DirectoryContext(ContextType ct, string name, string userName, string password) {
            PrincipalContext pc = new PrincipalContext(ct, name, userName, password);
            WindowsIdentity pident = WindowsIdentity.GetCurrent();
            this.Process = new ProcessPrincipal(pc, pident);
        }

        public DirectoryContext(string domain, string userName, string password)
            : this(ContextType.Domain, domain, userName, password) { }

        #endregion

        #region " Methods "

        public UserAccount GetUser(string userName) {
            UserAccount user = null;
            if (String.IsNullOrEmpty(userName) == false) {
                UserPrincipal up = UserPrincipal.FindByIdentity(this.Process.PrincipalContext, userName);
                if (up != null) {
                    user = new UserAccount(up);
                }
            }
            return user;
        }

        public UserAccount GetWebUser() {
            if (HttpContext.Current != null
                && HttpContext.Current.User.Identity != null
                && HttpContext.Current.User.Identity.IsAuthenticated) {
                return this.GetUser(HttpContext.Current.User.Identity.Name);
            }
            return null;
        }

        public UserSearchResult[] SearchUsers(string partial, int? maxResults) {
            List<UserSearchResult> results = new List<UserSearchResult>();
            UserPrincipal user = new UserPrincipal(this.Process.PrincipalContext);
            user.Name = (partial ?? String.Empty) + "*"; // builds 'Ja*', which finds names starting with 'Ja'
            using (var searcher = new PrincipalSearcher(user)) {
                if (maxResults.HasValue) {
                    ((DirectorySearcher)searcher.GetUnderlyingSearcher()).SizeLimit = maxResults.Value;
                }
                foreach (var result in searcher.FindAll()) {
                    DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                    if (de != null) {
                        results.Add(new UserSearchResult(de));
                    }
                }
            }
            return results.ToArray();
        }

        #endregion

        #region " Static Methods "

        public static T GetPropertyValue<T>(Principal p, string propertyName) {
            object value = DirectoryContext.GetPropertyValue(p, propertyName);
            if (value != null) {
                return (T)value;
            }
            return default(T);
        }

        public static T GetPropertyValue<T>(DirectoryEntry de, string propertyName) {
            object value = DirectoryContext.GetPropertyValue(de, propertyName);
            if (value != null) {
                return (T)value;
            }
            return default(T);
        }

        public static object GetPropertyValue(Principal p, string propertyName) {
            return DirectoryContext.GetPropertyValue(p, propertyName, true);
        }

        public static object GetPropertyValue(DirectoryEntry de, string propertyName) {
            return DirectoryContext.GetPropertyValue(de, propertyName, true);
        }

        public static object GetPropertyValue(Principal p, string propertyName, bool useCache) {
            return GetPropertyValue(p.GetUnderlyingObject() as DirectoryEntry, propertyName, useCache);
        }

        public static object GetPropertyValue(DirectoryEntry de, string propertyName, bool useCache) {
            de.UsePropertyCache = useCache;
            if (de.Properties.Contains(propertyName)) {
                return de.Properties[propertyName].Value;
            }
            return null;
        }

        public static object SearchUser(Principal p, string sAMAccountName, string property) {
            DirectoryEntry de = p.GetUnderlyingObject() as DirectoryEntry;
            de.UsePropertyCache = false;
            DirectorySearcher search = new DirectorySearcher();
            search.SearchRoot = de;
            search.CacheResults = false;
            search.Filter = "(&(objectClass=user)(objectCategory=person)(sAMAccountName=" + sAMAccountName + "))";
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add(property);
            SearchResultCollection mult = search.FindAll();
            //SearchResult r = search.FindOne();
            foreach (SearchResult r in mult) {
                if (r != null && r.Properties.Contains(property)) {
                    if (r.Properties[property].Count == 1) {
                        return r.Properties[property][0];
                    } else {
                        object[] results = new object[r.Properties[property].Count];
                        r.Properties[property].CopyTo(results, 0);
                        return results;
                    }
                }
            }
            return null;
        }

        #endregion

    }

}
