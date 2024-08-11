namespace ecommerce_backend.DTOS
{
    public class VerifyCode
    {
        public string Email { get; set; }=string.Empty;
        public string VerificationCode { get; set; }= string.Empty;
    }
}
