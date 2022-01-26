using ItransitionProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ItransitionProject
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
            await roleManager.CreateAsync(new IdentityRole("user"));
        }
    }
}
