using System.ComponentModel.DataAnnotations;

namespace ItransitionProject.Models
{
    public class ItemProperty
    {
        [Key]
        public int UserItemId { get; set; }

        [Key]
        public int ColletctionPropertyId { get; set; }

        public string Value { get; set; }

        public UserItem Item { get; set; }

        public CollectionProperty Property { get; set; }
    }
}
