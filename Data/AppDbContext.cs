using Microsoft.EntityFrameworkCore;
using LixoZero.Models;

namespace LixoZero.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Descarte> Descartes { get; set; }
    }
}
