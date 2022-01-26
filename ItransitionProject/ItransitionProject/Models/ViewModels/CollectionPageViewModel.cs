using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class CollectionPageViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public CollectionType CollectionType { get; set; }

        public FileModel FileModel { get; set; }

        public List<CollectionViewProperty> CollectionViewProperties { get; set; } = new List<CollectionViewProperty>();

        public List<UserItem> CollectionItems { get; set; }

        public CollectionPageViewModel(ApplicationContext applicationContext, UserCollection userCollection)
        {
            Name = userCollection.Name;
            Description = userCollection.Descritpion;
            CollectionType = userCollection.CollectionType;
            FileModel = applicationContext.Files.Find(userCollection.FileId);
            CollectionItems = applicationContext.UserItems.Where(x => x.CollectionId == userCollection.Id).ToList();
            foreach (var item in CollectionItems)
            {
                item.ItemProperties = applicationContext.ItemProperties.Where(x => x.UserItemId == item.Id).ToList();
                item.Tags = applicationContext.ItemTags.Include(u => u.UserTag).Where(i => i.ItemId == item.Id).Take(3).ToList();
            }
            List<CollectionProperty> properties = applicationContext.CollectionProperties.Where(p => p.CollectionId == userCollection.Id).ToList();
            foreach (var property in properties)
            {
                CollectionViewProperties.Add(new CollectionViewProperty
                {
                    Name = property.Name,
                    Property = property.Property
                });
            }
        }
    }

    public class CollectionViewProperty
    {
        public string Name { get; set; }

        public PropertyType Property { get; set; }
    }
}
