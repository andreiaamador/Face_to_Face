using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Face_to_Face.Startup))]
namespace Face_to_Face
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
