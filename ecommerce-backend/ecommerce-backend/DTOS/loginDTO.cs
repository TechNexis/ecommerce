using System.ComponentModel.DataAnnotations;

namespace ecommerce_backend.DTOS
{
    public class loginDTO
    {

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; }
    }
}
