using System.ComponentModel.DataAnnotations;

namespace ItransitionProject.Models
{
    public class ItemTag
    {
        [Key]
        public int ItemId { get; set; }

        public UserItem UserItem { get; set; }

        [Key]
        public int TagId { get; set; }

        public UserTag UserTag { get; set; }
    }
}
