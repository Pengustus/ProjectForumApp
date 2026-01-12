using Azure;
using Microsoft.EntityFrameworkCore;

namespace ForumApp.Data
{
    public class ForumDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ProjectForumApp;User Id=sa;Password=SuperStrongPass!23;TrustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author) 
                .WithMany(u => u.Comments) 
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.NoAction); 

            base.OnModelCreating(modelBuilder);
        }
    }
}
