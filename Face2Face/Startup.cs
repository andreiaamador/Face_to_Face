using Face2Face.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
            createRolesandUsers();
        }
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
           
            var roleManager = new RoleManager<CustomRole,int>(new CustomRoleStore(context));
            var UserManager = new UserManager<ApplicationUser,int>(new CustomUserStore(context));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new CustomRole();
                role.Name = "Admin";
                roleManager.Create(role);

            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new CustomRole();
                role.Name = "User";
                roleManager.Create(role);
            }

            ApplicationUser newUser = new ApplicationUser();
            newUser.Email = "saralopesr@gmail.com";
            

            newUser.UserName = "saralopesr";
            var password = "socializar";
            var result = UserManager.Create(newUser, password);
            if(result.Succeeded)
            {
                UserManager.AddToRole(newUser.Id, "Admin");
            }
        }
    }
}
