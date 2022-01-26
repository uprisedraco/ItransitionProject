namespace ItransitionProject.Models
{
    public class UsersViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime RegistrationDate { get; set; }

        public UserRole Role { get; set; }

        public Status Status { get; set; }

        public UsersViewModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            RegistrationDate = user.RegistrationDate;
            Role = user.UserRole;
            Status = user.Status;
        }
    }
}
