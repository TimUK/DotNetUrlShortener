using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using URLShortener.DataModels;

namespace URLShortener.Data
{
    public class LinkAPIContext : IdentityUserContext<IdentityUser>
    {
        public LinkAPIContext(DbContextOptions<LinkAPIContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Link> Links { get; set; }
    }
}
