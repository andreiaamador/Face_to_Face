using Face2Face.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

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

                var user = new ApplicationUser();
                user.UserName = "Admin";
                user.Email = "altario13@outlook.com";

                string userPWD = "Upacademy1!";

                var chkUser = UserManager.Create(user,userPWD);

                if (chkUser.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, "Admin");
                }
            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new CustomRole();
                role.Name = "User";
                roleManager.Create(role);
            }
        }
    }
}
