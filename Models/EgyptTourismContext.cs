using Microsoft.EntityFrameworkCore;

namespace EgyptTourism.Models
{
    public class EgyptTourismContext : DbContext
    {
        public EgyptTourismContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

    }
}