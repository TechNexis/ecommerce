using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.core.Entities.IdentitiyEntities
{
    public  class UserAddress
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
       public string Country { get; set; } = string.Empty;
        public string AppUserId { get; set; } //= string.Empty;// FK For User Table | and we don't need to write it in fluent api because his name => 'tableName+Id' AppUserId
    }
}
