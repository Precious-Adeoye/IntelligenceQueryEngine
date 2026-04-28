using IntelligenceQueryEngine.Model.Entity;
using IntelligenceQueryEngine.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Text.Json;

namespace IntelligenceQueryEngine.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Profile> profiles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<RequestLog> RequestLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.HasIndex(e => e.name).IsUnique();
        });

        

        // NEW - RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.ToTable("refresh_tokens");
        });

        // NEW - RequestLog configuration
        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("request_logs");
        });
    }
}


   
