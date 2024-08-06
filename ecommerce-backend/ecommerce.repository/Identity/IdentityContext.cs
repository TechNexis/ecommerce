using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ecommerce.core.Entities.IdentitiyEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace ecommerce.repository.Identity
{
    public  class IdentityContext:IdentityDbContext<AppUser>
    {

        public IdentityContext(DbContextOptions <IdentityContext>options):base(options) { }////****chainig constructor
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserAddress>().ToTable("Addresses");
        }
       // public DbSet<AppUser> users { get; set; } //= new DbSet<AppUser>();

    }
}
