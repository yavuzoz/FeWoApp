using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace API.Data
{
    public class FeWoDbContext : DbContext
    {
        public FeWoDbContext(DbContextOptions<FeWoDbContext> options) : base(options)
        {
        }

        public DbSet<FeWo> FeWos { get; set; }
        public DbSet<Buchung> Buchungen { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buchung>()
                .HasOne(b => b.FeWo)
                .WithMany(f => f.Buchungen)
                .HasForeignKey(b => b.FeWoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Buchung>()
                .HasIndex(b => new { b.FeWoId, b.KalenderWoche })
                .IsUnique();
        }
    }
}
