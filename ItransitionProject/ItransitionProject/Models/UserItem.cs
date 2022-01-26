using System.ComponentModel.DataAnnotations;

namespace ItransitionProject.Models
{
    public class UserItem
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int CollectionId { get; set; }

        public UserCollection Collection { get; set; }

        public List<ItemProperty> ItemProperties { get; set; }

        public List<Comment> Comments { get; set; }

        public List<Like> Likes { get; set; }

        public List<ItemTag> Tags { get; set; }
    }
}
