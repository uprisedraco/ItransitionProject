using System.ComponentModel.DataAnnotations;

namespace ItransitionProject.Models
{
    public class UserCollection
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Descritpion { get; set; }

        public CollectionType CollectionType { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public List<UserItem> UserItems { get; set; }

        public List<CollectionProperty> CollectionProperties { get; set; }

        public int? FileId { get; set; }

        public FileModel? FileModel { get; set; }
    }

    public enum CollectionType
    {
        Alcohol,
        Books,
        Games,
        Movies,
        Cars,
        Rocks
    }
}
