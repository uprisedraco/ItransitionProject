using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ItransitionProject.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<UserCollection> UserCollections { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<CollectionProperty> CollectionProperties { get; set; }
        public DbSet<ItemProperty> ItemProperties { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<Search> Searches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserCollection>()
                .HasOne(u => u.User)
                .WithMany(x => x.UserCollections)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserCollection>()
                .HasOne(u => u.FileModel)
                .WithMany(x => x.UserCollections)
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserItem>()
                .HasOne(u => u.Collection)
                .WithMany(x => x.UserItems)
                .HasForeignKey(x => x.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CollectionProperty>()
                .HasOne(x =>x.UserCollection)
                .WithMany(x=>x.CollectionProperties)
                .HasForeignKey(x=>x.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ItemProperty>().HasKey(a => new { a.UserItemId, a.ColletctionPropertyId });

            builder.Entity<ItemProperty>()
                .HasOne(x => x.Item)
                .WithMany(x => x.ItemProperties)
                .HasForeignKey(x => x.UserItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ItemProperty>()
                .HasOne(x => x.Property)
                .WithMany(x => x.ItemProperties)
                .HasForeignKey(x => x.ColletctionPropertyId)
                .OnDelete(DeleteBehavior.ClientCascade);

            builder.Entity<Comment>()
                .HasOne(u=>u.User)
                .WithMany(u=>u.Comments)
                .HasForeignKey(u=>u.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Comment>()
                .HasOne(u => u.UserItem)
                .WithMany(u => u.Comments)
                .HasForeignKey(u => u.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Like>()
                .HasOne(u => u.User)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Like>()
                .HasOne(i => i.Item)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ItemTag>().HasKey(a => new { a.ItemId, a.TagId });

            builder.Entity<ItemTag>()
                .HasOne(i => i.UserItem)
                .WithMany(l => l.Tags)
                .HasForeignKey(t => t.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
           
            builder.Entity<ItemTag>()
                .HasOne(i => i.UserTag)
                .WithMany(l => l.Tags)
                .HasForeignKey(t => t.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
