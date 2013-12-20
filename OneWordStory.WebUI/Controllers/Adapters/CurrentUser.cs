using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace OneWordStory.WebUI.Controllers.Adapters
{
    public class CurrentUser
    {

        
            public CurrentUser(IIdentity identity)
            {
                IsAuthenticated = identity.IsAuthenticated;
                UserID = identity.Name;

            }

            public CurrentUser(string userID)
            {
                IsAuthenticated = true;
                UserID = userID;
    
            }

            public bool IsAuthenticated { get; private set; }
            public string UserID { get; private set; }
        }
    
}