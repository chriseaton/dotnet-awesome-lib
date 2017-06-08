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
using System.DirectoryServices.AccountManagement;
using Awesome.Library.Utilities;

namespace Awesome.Library.ActiveDirectory {

    public class UserAccount : UserSearchResult {

        #region " Properties "

        public Image Photo { get; set; }

        #endregion

        #region " Constructor(s) "

        public UserAccount() { }

        public UserAccount(UserPrincipal up) {
            this.LoadProperties(up);
        }

        #endregion

        #region " Methods "

        protected override void LoadProperties(UserPrincipal up) {
            base.LoadProperties(up);
            this.Photo = this.RetrievePhoto(up);
        }

        protected Image RetrievePhoto(UserPrincipal up) {
            object photoBytes = DirectoryContext.SearchUser(up, up.SamAccountName, "thumbnailPhoto");
            if (photoBytes == null) {
                photoBytes = DirectoryContext.SearchUser(up, up.SamAccountName, "jpegPhoto");
            }
            if (photoBytes != null && photoBytes is byte[]) {
                MemoryStream ms = new MemoryStream((byte[])photoBytes);
                return Image.FromStream(ms);
            }
            return null;
        }

        #endregion

    }
}
