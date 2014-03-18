using System.Web;
using System.Web.Mvc;
using OneWordStory.Domain.Concrete;
using OneWordStory.WebUI.Attributes;

namespace OneWordStory.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomErrorHandlerAttribute(new InfrastructureRepository()));
        }
    }
}