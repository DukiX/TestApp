using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TestApp.DB
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Proizvod> Proizvodi { get; set; }
        public DbSet<Narudzbina> Narudzbine { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(a =>
            {
                a.HasKey(x => x.Id);
                a.Property("UserId");

                a.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey("UserId")
                .HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
