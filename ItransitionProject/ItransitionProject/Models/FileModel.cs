namespace ItransitionProject.Models
{
    public class FileModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //public string Path { get; set; }

        public byte[] Data { get; set; }

        public List<UserCollection> UserCollections { get; set; }
    }
}
