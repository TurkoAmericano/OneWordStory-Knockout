using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using OneWordStory.Concrete;
using OneWordStory.Domain.Abstract;
using OneWordStory.Domain.Concrete;
using OneWordStory.WebUI.Controllers.Adapters;

namespace OneWordStory.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {

        private IKernel _kernel;

        public NinjectControllerFactory()
        {
            _kernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)_kernel.Get(controllerType);

        }

        private void AddBindings()
        {
            _kernel.Bind<IUserRepository>().To<UserRepository>();
            _kernel.Bind<IStoryRepository>().To<StoryRepository>();
            _kernel.Bind<IFormsAuthentication>().To<FormsAuthenticationWrapper>();
            _kernel.Bind<IIdentity>().ToMethod(c => HttpContext.Current.User.Identity);
            
        }






    }
}