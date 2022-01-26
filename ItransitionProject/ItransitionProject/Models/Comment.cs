namespace ItransitionProject.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string UserComment { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }

        public int? ItemId { get; set; }

        public UserItem? UserItem { get; set; }
    }
}
