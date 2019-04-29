using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OVD.API.Data;
using OVD.API.Dtos;

namespace OVD.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo;

        public AuthController(IConfiguration config, IAuthRepository repo)
        {
            _config = config;
            _repo = repo;
        }

        /*
        [HttpPost("register")]
        public async Task<IActionResult> Register(AdminForRegisterDto adminForRegisterDto)
        {
            // validate request

            adminForRegisterDto.Username = adminForRegisterDto.Username.ToLower();

            if (await _repo.AdminExists(adminForRegisterDto.Username))
                return BadRequest("Username already exists");

            var adminToCreate = new Admin
            {
                Username = adminForRegisterDto.Username
            };

            var createdUser = await _repo.Register(adminToCreate, adminForRegisterDto.Password);

            return StatusCode(201);
        } /* * */

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            userForLoginDto.Username = userForLoginDto.Username.ToLower();
            // Check if dawgtag or not
            // SIU85[0-9]{7}
            Console.WriteLine("\n\n\n\nLOGGING IN");
            Console.WriteLine(userForLoginDto.Username);
            Console.WriteLine(userForLoginDto.Password);

            Claim idClaim;
            Claim nameClaim;
            Claim roleClaim;
            Regex dawgtagRx = new Regex("siu85[0-9]{7}", RegexOptions.Compiled);


            if (dawgtagRx.IsMatch(userForLoginDto.Username))
            {
                Console.WriteLine("Determined to be User.");
                // LDAP login
                LdapAuth ldapAuth = new LdapAuth();

                // Validate user via LDAP
               if (!ldapAuth.validateUser(userForLoginDto))
                    return Unauthorized();

                // Assign security claims
                idClaim = new Claim(ClaimTypes.NameIdentifier, userForLoginDto.Username);
                nameClaim = new Claim(ClaimTypes.Name, "user");
                roleClaim = new Claim(ClaimTypes.Role, "standard");
            } else 
            {
                // Admin login
                if (userForLoginDto.Username != _config.GetSection("AdminPassword:Username").Value || userForLoginDto.Password != _config.GetSection("AdminPassword:Password").Value)
                    return Unauthorized();

                Console.WriteLine("Determined to be Admin");
                idClaim = new Claim(ClaimTypes.NameIdentifier, userForLoginDto.Username);
                nameClaim = new Claim(ClaimTypes.Name, userForLoginDto.Username);
                roleClaim = new Claim(ClaimTypes.Role, "admin");
            }

            var claims = new [] 
            {
                idClaim,
                nameClaim,
                roleClaim
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
        /* */
    }
}
