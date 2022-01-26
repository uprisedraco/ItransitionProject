using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class EditCollectionViewModel
    {
        public EditCollection EditCollection { get; set; } = new EditCollection();

        public List<UserItem> CollectionItems { get; set; }

        public CollectionProperty  CollectionProperty { get; set; }

        public UserItem UserItem { get; set; }

        public EditCollectionViewModel(ApplicationContext applicationContext, UserCollection userCollection)
        {
            EditCollection.Id = userCollection.Id;
            EditCollection.Name = userCollection.Name;
            EditCollection.Description = userCollection.Descritpion;
            EditCollection.CollectionType = userCollection.CollectionType;
            CollectionItems = applicationContext.UserItems.Where(x => x.CollectionId == userCollection.Id).ToList();
            foreach (var item in CollectionItems)
            {
                item.ItemProperties = applicationContext.ItemProperties.Where(x => x.UserItemId == item.Id).ToList();
                item.Tags = applicationContext.ItemTags.Include(u => u.UserTag).Where(x => x.ItemId == item.Id).Take(3).ToList();
            }
            List<CollectionProperty> properties = applicationContext.CollectionProperties.Where(p => p.CollectionId == userCollection.Id).ToList();
            foreach (var property in properties)
            {
                EditCollection.EditCollectionProperties.Add(new EditCollectionProperty
                {
                    Id = property.Id,
                    Name = property.Name,
                    Property = property.Property
                });
            }

        }
    }

    public class EditCollection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public CollectionType CollectionType { get; set; }

        public List<EditCollectionProperty> EditCollectionProperties { get; set; } = new List<EditCollectionProperty>();
    }

    public class EditCollectionProperty
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public PropertyType Property { get; set; }
    }
}
