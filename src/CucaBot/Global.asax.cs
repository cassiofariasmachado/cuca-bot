using System.Web.Http;

namespace CucaBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start() => GlobalConfiguration.Configure(WebApiConfig.Register);
    }
}
