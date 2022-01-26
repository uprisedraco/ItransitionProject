using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class EditItemViewModel
    {
        public EditItem EditItem { get; set; }

        public List<CollectionProperty> CollectionProperties { get; set; }

        public EditItemViewModel(ApplicationContext applicationContext, UserItem userItem)
        {
            EditItem = new EditItem()
            {
                Id = userItem.Id,
                Name = userItem.Name,
                Tags = new SelectList(applicationContext.UserTags.ToList(), "Tag", "Tag"),
                ItemProperties = applicationContext.ItemProperties.Where(x => x.UserItemId == userItem.Id).ToList()
            };

            var itemTags = applicationContext.ItemTags.Include(t => t.UserTag).Where(i => i.ItemId == userItem.Id).ToList();
            foreach (var tag in EditItem.Tags)
            {
                if (itemTags.FirstOrDefault(w => w.UserTag.Tag == tag.Value) != null)
                {
                    tag.Selected = true;
                }
            }
            CollectionProperties = applicationContext.CollectionProperties.Where(x => x.CollectionId == userItem.CollectionId).ToList();
        }
    }

    public class EditItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public SelectList Tags { get; set; }

        public List<ItemProperty> ItemProperties { get; set; }
    }
}
