using Core.Entities.IdentitiyEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public  interface IAuthService
    {
        Task<string> createtokenAsync(AppUser user, UserManager<AppUser>userManager);

    }
}
