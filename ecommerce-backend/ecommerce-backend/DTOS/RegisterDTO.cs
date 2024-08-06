using System.ComponentModel.DataAnnotations;

namespace ecommerce_backend.DTOS
{
    public class RegisterDTO
    {
        public string DisplayName { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; }

        public string Password { get; set; }
    }
}
