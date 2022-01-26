using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class SearchViewModel
    {
        public List<Comment> Comments { get; set; }

        public List<UserItem> UserItems { get; set; }

        public List<ItemProperty> ItemProperties { get; set; }

        public List<ItemTag> Tags { get; set; }

        public SearchViewModel(ApplicationContext applicationContext, string searchText)
        {
            UserItems = applicationContext.UserItems.Where(i => i.Name.Contains(searchText)).ToList();
            ItemProperties = applicationContext.ItemProperties.Include(i => i.Item).Where(p => p.Value.Contains(searchText)).ToList().DistinctBy(i => i.Item).ToList();
            Comments = applicationContext.Comments.Include(i => i.UserItem).Where(c => c.UserComment.Contains(searchText)).ToList().DistinctBy(i => i.ItemId).ToList();
            Tags = applicationContext.ItemTags.Include(u => u.UserItem).Include(t => t.UserTag).Where(m => m.UserTag.Tag.Contains(searchText)).ToList().DistinctBy(x => x.ItemId).ToList();
        }
    }
}
