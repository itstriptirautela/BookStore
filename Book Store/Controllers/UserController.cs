using Book_Store.Models;
using Microsoft.AspNetCore.Mvc;
using AuthenticationPlugin;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Book_Store.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private BookStoreContext dbcontext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        public UserController(BookStoreContext BookstoreContext, IConfiguration configuration)
        {
            dbcontext = BookstoreContext;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpGet]
        public IEnumerable<User> GetUserDetails()
        {
            return dbcontext.Users;
        }
        //<summary>
        /// Registering a User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register([FromBody] User user)

        {
            //// var userName = dbcontext.Users.Where(u => u.Username == user.Username).FirstOrDefault();
            // if (userName != null)
            // {
            //     return BadRequest("UserName already Exist in DB");
            // }
            // var UserObj = new User

            // {
            //     Name = user.Name,
            //     Address = user.Address,
            //     Role = user.Role,
            //     Username = user.Username,
            //     Password = SecurePasswordHasherHelper.Hash(user.Password),
            //     PhoneNumber = user.PhoneNumber,
            //     Email = user.Email

            // };
            var userName = dbcontext.Users.Where(u => u.Username == user.Username).FirstOrDefault();
            if (userName != null)

            {
                return BadRequest("UserName already Exist in DB");
            }
            else
            {
                user.Password = SecurePasswordHasherHelper.Hash(user.Password);
                dbcontext.Users.Add(user);
                dbcontext.SaveChanges();
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Sign Up Successfully"
                }
                    );

            }
        }
        //dbcontext.Users.Add(UserObj);
        //    dbcontext.SaveChanges();
        //    return StatusCode(StatusCodes.Status201Created);

        //}
        /// <summary>
        /// Updating the user details
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPut]
        public IActionResult UpdateUserDetails(int UserId, [FromBody] User user)
        {


            var data = dbcontext.Users.Where(u => u.UserId == UserId).FirstOrDefault();
            if (data == null)
            {


                return NotFound("No record found against this Id");
            }
            else
            {
                data.Name = user.Name;
                data.Address = user.Address;
                data.Password = SecurePasswordHasherHelper.Hash(user.Password);
                data.PhoneNumber = user.PhoneNumber;
                data.Email = user.Email;

                dbcontext.Users.Update(data);
                dbcontext.SaveChanges();

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "User Update Successfully"
                }
                     );
            }
        }



        /// <summary>
        /// Login the user with user id and password
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login([FromBody] User user)

        {

            //  && u.Role == user.Role
            var data = dbcontext.Users.FirstOrDefault(u => u.Username == user.Username);
            if (data == null)
            {
                return NotFound();
            }
            if (!SecurePasswordHasherHelper.Verify(user.Password, data.Password))
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Username),
                new Claim(ClaimTypes.Email,user.Username),
                new Claim(ClaimTypes.Role ,data.Role),

            };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,

                Role = data.Role,
                Name = data.Name,
                Username = data.Username,
                Email = data.Email,
                userId = data.UserId,


            }

            );
        }






    }
}



