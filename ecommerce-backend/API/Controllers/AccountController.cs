using Core.Entities.IdentitiyEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Authentication;

using NETCore.MailKit.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using API.MailSettingsService;
using API.DTOS;
using Repository.Identity;
using API.Filters;
using ecommerce_backend.DTOS;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Service;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [LogSensitiveAction]//it will apply on the whole controller but if you put it above an action it will apply on a specific action
    public class AccountController(UserManager<AppUser> userMananger, SignInManager<AppUser> signInManager,
           /* IEmailService emailService,*/ IConfiguration configuration, 
           IdentityContext identityContext, IEmailSettings emailsettings, ILogger<AccountController> logger,
           IAuthService authService) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager  = userMananger;
        private readonly SignInManager<AppUser> _signInManager= signInManager;
       // private readonly IEmailService _emailService= emailService;
        private readonly IConfiguration _configuration= configuration;
        private readonly IdentityContext _identityContext= identityContext;
        private readonly IEmailSettings _emailsettings= emailsettings;
        private readonly ILogger<AccountController> _logger= logger;


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (model == null) return NotFound("use not found");

            if (await CheckEmailExist(model.Email)) return BadRequest("Email already found");
            //create user
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email.Split('@')[0],
                // TwoFactorEnabled=true
            };
            _logger.LogInformation("Registering the user");
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest();
            // _userManager.AddToRoleAsync(user, "admin");
            var token = await authService.createtokenAsync(user, _userManager);
            return Ok(new AppUserDTO { Email = user.Email, DisplayName = user.DisplayName, Token = token });

        }


        [HttpPost("login")]
        public async Task<ActionResult<AppUserDTO>> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return Unauthorized("User not foune");
            _logger.LogDebug("checking the password for the user");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);//********************************
            if (!result.Succeeded) return Unauthorized("username or password isn't found");
          
             return Ok(new AppUserDTO { Email = user.Email!, DisplayName = user.DisplayName, Token = "" });
        }


        [HttpGet("emailexists")]
        private async Task<bool> CheckEmailExist(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        [HttpPost("forgetpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ForgetPassword(EmailDto emailDto)
        {
            if (!IsValidEmail(emailDto.Email))
                return BadRequest(new ApiResponse(400, "Invalid email format."));

            var user = await _userManager.FindByEmailAsync(emailDto.Email);

            if (user is null)
                return Ok(new ApiResponse(200, "If your email is registered with us, a password reset email has been successfully sent."));
            
            var code = GenerateSecureCode();

            Email emailToSend = new Email()
            {
                To = emailDto.Email,
                Subject = $"{user.DisplayName}, Reset Your Password - Verification Code: {code}",
                Body = EmailBody(code, user.DisplayName, "Reset Password", "You have requested to reset your password.")
            };

            try
            {
              var result=  await _identityContext.IdentityCode.AddAsync(new IdentityCode()
                {
                    Code = HashCode(code),
                    AppUserId = user.Id
                });

              var rsult=  await _identityContext.SaveChangesAsync();

                await _emailsettings.SendEmailMessage(emailToSend);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending password reset email.");
                return StatusCode(500, new ApiResponse(500, "An error occurred while processing your request."));
            }

            return Ok(new ApiResponse(200, "If your email is registered with us, a password reset email has been successfully sent."));
        }

        private bool ConstantTimeComparison(string a, string b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        [HttpPost("VerifyResetCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyResetCode(VerifyCode model)
        {
            if (!IsValidEmail(model.Email))
                return BadRequest(new ApiResponse(400, "Invalid email format."));

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return BadRequest(new ApiResponse(400, "Invalid Email."));

            var identityCode = await _identityContext.IdentityCode
                                .Where(P => P.AppUserId == user.Id)
                                .OrderBy(d => d.CreationTime)
                                .LastOrDefaultAsync();

            if (identityCode is null)
                return BadRequest(new ApiResponse(400, "No valid reset code found."));

           if (identityCode.IsActive)
                return BadRequest(new ApiResponse(400, "You already have an active code."));

            var lastCode = identityCode.Code;

            if (!ConstantTimeComparison(lastCode, HashCode(model.VerificationCode)))
                return BadRequest(new ApiResponse(400, "Invalid reset code."));

            if (identityCode.CreationTime.Minute + 5 < DateTime.UtcNow.Minute)
                return BadRequest(new ApiResponse(400, "This code has expired."));

            identityCode.IsActive = true;
            identityCode.ActivationTime = DateTime.UtcNow;
            _identityContext.IdentityCode.Update(identityCode);
            await _identityContext.SaveChangesAsync();

            return Ok(new ApiResponse(200, "Code verified successfully."));
        }

        [HttpPost("resetpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (model == null)
            {
                return BadRequest(new ApiResponse(400, "Invalid request."));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse(400, "Invalid input."));
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user!);

            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "User not found."));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse(200, "Password has been reset successfully."));
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new ApiResponse(400, "Password reset failed: " + string.Join(", ", errors)));
            }
        }

        [HttpGet("ConfirmEmail")]
         public async Task<IActionResult> ConfirmEmail(string email, string token)
         {
            if (email == null || token == null) return BadRequest(new ApiResponse(400, "Invalid Email confirmation request."));
             var user = await _userManager.FindByEmailAsync(email);
             if (user == null)
             {
                 return BadRequest( new ApiResponse ( 400, "User not found" ));
             }

             var result = await _userManager.ConfirmEmailAsync(user, token);
             if (result.Succeeded)
             {
                 return Ok(new ApiResponse (200, "Email Verified Successfully" ));
             }

             return BadRequest( new ApiResponse ( 400,  "Error verifying email" ));
         }



       

        private string HashCode(string code)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string EmailBody(string code, string userName, string title, string message)
        {
            return $@"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Email Verification</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    line-height: 1.6;
                    background-color: #f5f5f5;
                    margin: 0;
                    padding: 0;
                }}
                .container {{
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    background-color: #ffffff;
                    border-radius: 8px;
                    box-shadow: 0 0 10px rgba(0,0,0,0.1);
                }}
                .header {{
                    background-color: #007bff;
                    color: #ffffff;
                    padding: 10px;
                    text-align: center;
                    border-top-left-radius: 8px;
                    border-top-right-radius: 8px;
                }}
                .content {{
                    padding: 20px;
                }}
                .code {{
                    font-size: 24px;
                    font-weight: bold;
                    text-align: center;
                    margin-top: 20px;
                    margin-bottom: 30px;
                    color: #007bff;
                }}
                .footer {{
                    background-color: #f7f7f7;
                    padding: 10px;
                    text-align: center;
                    border-top: 1px solid #dddddd;
                    font-size: 12px;
                    color: #777777;
                }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <h2>{title}</h2>
                </div>
                <div class=""content"">
                    <p>Dear {userName},</p>
                    <p>{message}, please use the following verification code:</p>
                    <div class=""code"">{code}</div>
                    <p>This code will expire in 5 minutes. Please use it promptly to verify your email address.</p>
                    <p>If you did not request this verification, please ignore this email.</p>
                </div>
                <div class=""footer"">
                    <p>&copy; 2024 TwoAxis. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";
        }

        private string GenerateSecureCode()
        {
            byte[] randomNumber = new byte[4];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            int result = BitConverter.ToInt32(randomNumber, 0);
            int positiveResult = Math.Abs(result);

            int sixDigitCode = positiveResult % 1000000;
            return sixDigitCode.ToString("D6");
        }
    }
}

