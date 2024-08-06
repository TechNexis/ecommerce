using ecommerce.core.Entities.IdentitiyEntities;
using ecommerce_backend.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser>userMananger,SignInManager<AppUser>signInManager)
        {
            _userManager = userMananger;
            _signInManager = signInManager;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (model == null) return NotFound("use not found");

            if (await CheckEmailExist(model.Email)) return BadRequest("Email isn't found");
            //create user
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email.Split('@')[0]
            };
            var result=await  _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded) return BadRequest();
            return Ok(new AppUserDTO { Email=user.Email,DisplayName=user.DisplayName,Token=""});
           
        }


        [HttpPost("login")]
        public async Task<ActionResult<AppUserDTO>> Login(loginDTO model)
        {
           var user=await _userManager.FindByEmailAsync(model.Email);
            if (user==null) return Unauthorized("User not foune");
            var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);//********************************
            if(!result.Succeeded)return Unauthorized ("username or password isn't found");
            return Ok(new AppUserDTO { Email = user.Email, DisplayName = user.DisplayName, Token = "" });
        }



        //[HttpGet("emailexists")]
        private  async Task<bool> CheckEmailExist(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
    }
}
