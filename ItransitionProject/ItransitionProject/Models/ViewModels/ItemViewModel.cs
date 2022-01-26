using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class ItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ItemTag> Tags { get; set; }

        public List<ItemProperty> ItemProperties { get; set; }

        public List<CollectionProperty> CollectionProperties { get; set; }

        public List<Like> Likes { get; set; }

        public List<Comment> Comments { get; set; }

        public AddCommentView CommentView { get; set; }

        public ItemViewModel(ApplicationContext applicationContext, UserItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Tags = applicationContext.ItemTags.Include(u => u.UserTag).Where(i => i.ItemId == item.Id).ToList();
            ItemProperties = applicationContext.ItemProperties.Where(x => x.UserItemId == item.Id).ToList();
            CollectionProperties = applicationContext.CollectionProperties.Where(x => x.CollectionId == item.CollectionId).ToList();
            Comments = applicationContext.Comments.Include(u => u.User).Where(i => i.ItemId == item.Id).ToList();
            Likes = applicationContext.Likes.Where(i => i.ItemId == item.Id).ToList();
        }
    }

    public class AddCommentView
    {
        public int ItemId { get; set; }

        public string Comment { get; set; }
    }
}
