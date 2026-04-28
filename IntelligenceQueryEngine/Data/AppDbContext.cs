using IntelligenceQueryEngine.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json;

namespace IntelligenceQueryEngine.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Profile> profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.HasIndex(e => e.name).IsUnique();
        });
    }
}


   
