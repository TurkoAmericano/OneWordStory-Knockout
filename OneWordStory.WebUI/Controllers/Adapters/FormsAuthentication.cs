using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;


namespace OneWordStory.WebUI.Controllers.Adapters
{
    public class FormsAuthenticationWrapper : IFormsAuthentication
    {
        public void SetAuthCookie(string userName, bool createPersistantCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistantCookie);
            
        }
    }
}