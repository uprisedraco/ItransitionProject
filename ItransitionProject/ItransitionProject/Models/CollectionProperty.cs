using System.ComponentModel.DataAnnotations;

namespace ItransitionProject.Models
{
    public class CollectionProperty
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public PropertyType Property { get; set; }

        public int CollectionId { get; set; }

        public UserCollection UserCollection { get; set; }

        public List<ItemProperty> ItemProperties { get; set; }
    }

    public enum PropertyType
    {
        text,
        number,
        checkbox,
        markdown,
        date
    }
}
