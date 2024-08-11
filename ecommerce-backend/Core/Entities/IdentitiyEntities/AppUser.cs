using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.IdentitiyEntities
{
    public  class AppUser:IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public UserAddress Address { get; set; } = null!;
    }
}
