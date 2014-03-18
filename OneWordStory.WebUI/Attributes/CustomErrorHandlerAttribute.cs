using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Infrastructure;

namespace OneWordStory.WebUI.Attributes
{
    public class CustomErrorHandlerAttribute : IExceptionFilter
    {


        private IInfrastructureRepository _repository;

        public CustomErrorHandlerAttribute(IInfrastructureRepository repository)
        {
            _repository = repository;
        }
        
        public void OnException(ExceptionContext filterContext)
        {

            if (filterContext.ExceptionHandled) return;

            _repository.LogError(filterContext.Exception, "Web error");

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Error" }));

            filterContext.ExceptionHandled = true;

            


        }
    }
}