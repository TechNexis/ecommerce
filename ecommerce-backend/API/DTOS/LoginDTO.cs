using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class LoginDTO
    {

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
