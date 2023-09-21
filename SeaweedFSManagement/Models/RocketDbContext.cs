using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace SeaweedFSManagement.Models
{
    public class RocketDbContext : DbContext
    {
        public RocketDbContext(DbContextOptions<RocketDbContext> options)
            : base(options)
        {
        }

        public DbSet<FileItem> Files { get; set; }
    }
}
