using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class IndexViewModel
    {
        public List<UserCollection> UserCollections { get; set; }

        public List<UserItem> UserItems { get; set; }

        public List<UserTag> UserTags { get; set; }

        public List<Search> Searches { get; set; }

        public IndexViewModel(ApplicationContext applicationContext, int collectionsCount, int itemsCount, int searchesCount)
        {
            UserCollections = applicationContext.UserCollections.Include(f => f.FileModel).OrderByDescending(x => x.UserItems.Count).Take(collectionsCount).ToList();
            UserItems = applicationContext.UserItems.OrderByDescending(x => x.Id).Take(itemsCount).ToList();
            UserTags = applicationContext.UserTags.ToList();
            Searches = applicationContext.Searches.OrderByDescending(x => x.Counter).Take(searchesCount).ToList();
        }
    }
}
