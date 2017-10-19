using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Face2Face.Startup))]
namespace Face2Face
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
