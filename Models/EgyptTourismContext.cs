using Microsoft.EntityFrameworkCore;

namespace EgyptTourism.Models
{
    public class EgyptTourismContext : DbContext
    {
        public EgyptTourismContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Comment> Comments { get; set; }

    }
}