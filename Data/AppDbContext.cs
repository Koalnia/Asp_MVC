using Asp_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Asp_MVC.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Advertisement> Advertisements { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Advertisements)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>().ToTable("AspNetUsers");
            modelBuilder.Entity<IdentityRole<long>>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserClaim<long>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserRole<long>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserLogin<long>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserToken<long>>().ToTable("AspNetUserTokens");
            modelBuilder.Entity<IdentityRoleClaim<long>>().ToTable("AspNetRoleClaims");
        }}


}

