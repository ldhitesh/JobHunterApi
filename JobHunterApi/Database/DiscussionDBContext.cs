using Microsoft.EntityFrameworkCore;
using JobHunterApi.Models;

namespace JobHunterApi.Database
{
    public class DiscussionDBContext:DbContext
    {
         public DiscussionDBContext(DbContextOptions<DiscussionDBContext> options)
            : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PostsModel>()
                .HasKey(e => e.post_id);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RepliesModel>()
                .HasKey(e => e.reply_id);
        }

        public DbSet<PostsModel> UserPosts{ get; set; }
        public DbSet<RepliesModel> UserPostReplies  { get; set; }

    }
}