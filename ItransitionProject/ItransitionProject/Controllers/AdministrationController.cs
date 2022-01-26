using ItransitionProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItransitionProject.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _applicationContext;

        public AdministrationController(UserManager<User> userManager, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _applicationContext = applicationContext;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(List<string> userId)
        {
            if (userId.Count != 0)
            {
                foreach (var id in userId)
                {
                    User user = await _userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        await DeleteComments(user);
                        await DeleteLikes(user);
                        await _userManager.UpdateSecurityStampAsync(user);
                        await _userManager.DeleteAsync(user);
                    }
                }
            }
            return RedirectToAction("Users", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BlockUser(List<string> userId)
        {
            if (userId.Count != 0)
            {
                foreach (var id in userId)
                {
                    User user = await _userManager.FindByIdAsync(id);
                    if (user != null && user.Status != Status.Block)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        user.LockoutEnd = DateTime.Now + TimeSpan.FromMinutes(6000);
                        user.Status = Status.Block;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            return RedirectToAction("Users", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnblockUser(List<string> userId)
        {
            if (userId.Count != 0)
            {
                foreach (var id in userId)
                {
                    User user = await _userManager.FindByIdAsync(id);
                    if (user != null && user.Status != Status.Active)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);
                        user.LockoutEnd = null;
                        user.Status = Status.Active;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            return RedirectToAction("Users", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpgradeToAdmin(List<string> userId)
        {
            if (userId.Count != 0)
            {
                foreach (var id in userId)
                {
                    User user = await _userManager.FindByIdAsync(id);
                    if (user != null && user.UserRole != UserRole.Admin)
                    {
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                        await _userManager.AddToRoleAsync(user, "admin");
                        user.UserRole = UserRole.Admin;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            return RedirectToAction("Users", "Home");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DowngradeToUser(List<string> userId)
        {
            if (userId.Count != 0)
            {
                foreach (var id in userId)
                {
                    User user = await _userManager.FindByIdAsync(id);
                    if (user != null && user.UserRole != UserRole.User)
                    {
                        await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                        await _userManager.AddToRoleAsync(user, "user");
                        user.UserRole = UserRole.User;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }
            return RedirectToAction("Users", "Home");
        }

        public async Task DeleteComments(User user)
        {
            var comments = _applicationContext.Comments.Where(u => u.UserId == user.Id).ToList();
            foreach (var comment in comments)
            {
                comment.UserId = null;
            }
            _applicationContext.Comments.UpdateRange(comments);
        }

        public async Task DeleteLikes(User user)
        {
            var likes = _applicationContext.Likes.Where(u => u.UserId == user.Id).ToList();
            foreach (var like in likes)
            {
                like.UserId = null;
            }
            _applicationContext.Likes.UpdateRange(likes);
        }
    }
}
