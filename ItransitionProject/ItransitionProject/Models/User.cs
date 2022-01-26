using Microsoft.AspNetCore.Identity;

namespace ItransitionProject.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public DateTime RegistrationDate { get; set; }

        public Status Status { get; set; }

        public UserRole UserRole { get; set; }

        public List<UserCollection>? UserCollections { get; set; }

        public List<Comment>? Comments { get; set; }

        public List<Like>? Likes { get; set; }
    }

    public enum Status
    {
        Block,
        Active
    }

    public enum UserRole
    {
        User,
        Admin
    }
}
