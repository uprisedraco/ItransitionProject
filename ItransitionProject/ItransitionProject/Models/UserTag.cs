namespace ItransitionProject.Models
{
    public class UserTag
    {
        public int Id { get; set; }

        public string Tag { get; set; }

        public List<ItemTag> Tags { get; set; }
    }
}
