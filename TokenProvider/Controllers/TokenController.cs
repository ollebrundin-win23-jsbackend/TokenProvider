using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TokenProvider.Filters;

namespace TokenProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GetToken")]
        [UseApiKey]

        public IActionResult GetToken()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler(); //Instansierar en JwtSecurityTokenHandler, som sedan kommer kunna utföra saker som t.ex CreateToken
                var tokenDescriptor = new SecurityTokenDescriptor() //Skapar descriptor objektet som kommer innehålla all information som man vill ha i en token
                {
                    Issuer = "Provider",
                    Audience = "User",
                    Expires = DateTime.Now.AddMinutes(15),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Secret")!)), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(tokenHandler.WriteToken(token)); //Eftersom CreateToken inte skapar en token i string format, så omvandlar jag den här
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return BadRequest();
        }
    }
}
