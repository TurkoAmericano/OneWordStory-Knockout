using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneWordStory.WebUI.Controllers.Adapters
{
    public interface IFormsAuthentication
    {
        void SetAuthCookie(string userName, bool createPersistantCookie);

    }
}
