namespace ItransitionProject.Models
{
    public class Like
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public User? User { get; set; }

        public int? ItemId { get; set; }

        public UserItem? Item { get; set; }
    }
}
