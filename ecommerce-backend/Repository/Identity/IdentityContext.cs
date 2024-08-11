
using Core.Entities.IdentitiyEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Repository.Identity
{
    public  class IdentityContext:IdentityDbContext<AppUser>
    {

        public IdentityContext(DbContextOptions <IdentityContext>options):base(options) { }////****chainig constructor
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserAddress>().ToTable("Addresses");
        }
       public DbSet<IdentityCode> IdentityCode { get; set; } //= new DbSet<AppUser>();
       
    }
}
