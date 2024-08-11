using System.ComponentModel.DataAnnotations;

namespace API.DTOS
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }=string.Empty;
    }


}
