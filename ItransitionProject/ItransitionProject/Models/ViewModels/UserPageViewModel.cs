using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class UserPageViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Status { get; set; }

        public string Role { get; set; }

        public List<UserCollection> Collections { get; set; }

        public UserPageViewModel(ApplicationContext applicationContext, User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            RegistrationDate = user.RegistrationDate;
            Status = user.Status.ToString();
            Role = user.UserRole.ToString();
            Collections = applicationContext.UserCollections.Include(f => f.FileModel).Where(u => u.User == user).ToList();
        }
    }
}
