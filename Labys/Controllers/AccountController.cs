using Labys.DTO;
using Labys.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Labys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser>userManager,IConfiguration configuration )
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }


        [HttpPost]
        public async Task <IActionResult> Register(RegisterDTO registerDTO)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser appuser = new ApplicationUser()
                {
                    UserName=registerDTO.Email,
                    Email=registerDTO.Email,
                    PasswordHash=registerDTO.Password
                
                };

                IdentityResult result = await userManager.CreateAsync(appuser, registerDTO.Password);
                if (result.Succeeded)
                {
                    return Ok("Account Created :)");
                }
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);



        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]RegisterDTO RegisterDto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = await userManager.FindByNameAsync(RegisterDto.Email);
                if (userFromDb != null)
                {
                    bool found = await userManager.CheckPasswordAsync(userFromDb, RegisterDto.Password);
                    if (found)
                    {
                        //create list of Claim
                        List<Claim> myclaims = new List<Claim>();
                        myclaims.Add(new Claim(ClaimTypes.Name, userFromDb.UserName));
                        myclaims.Add(new Claim(ClaimTypes.NameIdentifier, userFromDb.Id));
                        myclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));


                        var roles = await userManager.GetRolesAsync(userFromDb);
                        foreach (var role in roles)
                        {
                            myclaims.Add(new Claim(ClaimTypes.Role, role));
                        }
                        var SignKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JwtSetting:SecritKey"]));

                        SigningCredentials signingCredentials =
                            new SigningCredentials(SignKey, SecurityAlgorithms.HmacSha256);


                        JwtSecurityToken mytoken = new JwtSecurityToken
                            (
                                issuer: configuration["JwtSetting:issuer"],
                                audience: configuration["JwtSetting:audience"],
                                expires: DateTime.Now.AddHours(5),
                                claims: myclaims,
                                signingCredentials: signingCredentials
                            );
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(mytoken),
                            expired = mytoken.ValidTo
                        });
                    }
                }
                return Unauthorized("UserName or Password Invalid");
            }
            return BadRequest(ModelState);
        }
    }
}
