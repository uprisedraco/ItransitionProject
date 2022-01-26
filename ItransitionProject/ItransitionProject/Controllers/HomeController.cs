using ItransitionProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ItransitionProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _applicationContext;

        public HomeController(UserManager<User> userManager, ApplicationContext applicationContext)
        {
            _userManager = userManager;
            _applicationContext = applicationContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel(_applicationContext, 3, 6, 10);
            return View(indexViewModel);
        }


        [HttpGet]
        public IActionResult Users()
        {
            List<UsersViewModel> usersView = new List<UsersViewModel>();
            foreach (var user in _userManager.Users)
            {
                usersView.Add(new UsersViewModel(user));
            }
            return View(usersView);
        }

        [HttpGet]
        public async Task<IActionResult> UserPage(string? userId)
        {
            User user = (userId == null) ? await _userManager.FindByEmailAsync(User.Identity.Name) : await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                UserPageViewModel userPage = new UserPageViewModel(_applicationContext, user);
                return View(userPage);
            }
            return BadRequest();
        }

        public async Task<IActionResult> SearchResults(string? searchText)
        {
            if (searchText != null)
            {
                await AddSearch(searchText);
                SearchViewModel searchViewModel = new SearchViewModel(_applicationContext, searchText);
                return View(searchViewModel);
            }
            return RedirectToAction("Index");
        }

        public async Task AddSearch(string searchText)
        {
            Search search = _applicationContext.Searches.FirstOrDefault(i => i.SearchText == searchText);
            if (search != null)
            {
                search.Counter += 1;
                _applicationContext.Searches.Update(search);
            }
            else
            {
                Search newSearch = new Search() { SearchText = searchText, Counter = 1 };
                await _applicationContext.AddAsync(newSearch);
            }
            await _applicationContext.SaveChangesAsync();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}