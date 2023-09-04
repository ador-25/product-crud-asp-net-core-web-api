using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductCrud.Models;
using ProductCrud.Repositories;
using ProductCrud.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ProductCrud.Controllers
{
    [Route("api/TokenAuth/[controller]/")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<Tenant> _tenantRepository;
        public AuthenticateController(IGenericRepository<Tenant> tenantRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tenantRepository = tenantRepository;

        }


        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterSchool([FromBody] RegisterViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserNameOrEmailAddress);
            if (userExists != null)
                return Ok(new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.UserNameOrEmailAddress,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserNameOrEmailAddress,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (!await _roleManager.RoleExistsAsync(UserRoles.Tenant))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Tenant));

            if (await _roleManager.RoleExistsAsync(UserRoles.Tenant))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Tenant);
            }

            Tenant tenant = new Tenant()
            {
                TenantName = model.TenantName,
            };
            _tenantRepository.Add(tenant);
            var temp = await _tenantRepository.SaveChangesAsync();

            if (temp)
            {
                var newUser = await _userManager.FindByEmailAsync(model.UserNameOrEmailAddress);
                newUser.TenantId = tenant.TenantId;
                var res = await _userManager.UpdateAsync(newUser);
                if (!res.Succeeded)
                {
                    return Ok("SOMETHING WENT WRONG");
                }
            }
            return temp ? Ok(new Response { Status = "Success", Message = "User created successfully!" }) : Ok("Could not add school");
           
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserNameOrEmailAddress);
            if (user == null)
            {
                return Ok(new Response()
                {
                    Status="Error",
                    Message="User Not Found"
                });
            }
            int headerId = 0;
            if (HttpContext.Request.Headers.TryGetValue("Abp.TenantId", out var tenantIdValues))
            {
                string tenantId = tenantIdValues.FirstOrDefault();
                headerId = int.Parse(tenantId);
            }
            else
            {
                return BadRequest("Abp.TenantId header not found in the request.");
            }
            if (user.TenantId != headerId)
            {
                return Unauthorized(new Response()
                {
                    Status = "Error",
                    Message = "TenantId Not Correct"
                });
            }
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddSeconds(31536000),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                var obj = new Object();

                return Ok(new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = obj,
                    TenantId = headerId
                });
            }

            return Ok(new Response()
            {
                Status = "Error",
                Message = "Login not success"
            });
        }



    }
}
