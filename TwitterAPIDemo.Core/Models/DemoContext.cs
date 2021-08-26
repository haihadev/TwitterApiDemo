using Microsoft.EntityFrameworkCore;

namespace TwitterAPIDemo.Core.Models
{
    public class DemoContext : DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options)
            : base(options)
        {
        }

        public DbSet<Url> Urls { get; set; }
        public DbSet<Hashtag> Hashtags { get; set; }
        public DbSet<Tweet> Tweets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hashtag>()
                .HasOne<Tweet>(h => h.Tweet)
                .WithMany(t => t.Hashtags)
                .HasForeignKey(h => h.TweetId);

            modelBuilder.Entity<Url>()
                .HasOne<Tweet>(u => u.Tweet)
                .WithMany(t => t.Urls)
                .HasForeignKey(u => u.TweetId);

            //modelBuilder.Entity<Tweet>()
            //    .HasMany<Hashtag>(t => t.Hashtags)
            //    .WithOne(h => h.Tweet)
            //    .HasForeignKey(h => h.TweetId);

            //modelBuilder.Entity<Tweet>()
            //    .HasMany<Url>(t => t.Urls)
            //    .WithOne(u => u.Tweet)
            //    .HasForeignKey(u => u.TweetId);
        }
    }
}
