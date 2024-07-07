using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SportEventsAPI.Data;
using SportEventsAPI.Data.SportEventsAPI.Data;
using SportEventsAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SportEventsAPI.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, IConfiguration configuration, ILogger<UsersController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // GET: api/v1/Users
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                })
                .ToListAsync();

            _logger.LogInformation("Get All User");

            return Ok(users);
        }

        // GET: api/v1/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/v1/Users
        [AllowAnonymous]
        [HttpPost]
        //public async Task<ActionResult<UserResponse>> PostUser(UserRequest userRequest)
        public async Task<IActionResult> PostUser([FromBody] UserRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if password and repeatPassword match
            if (userRequest.Password != userRequest.RepeatPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            // Check if user with the same email already exists
            if (await _context.Users.AnyAsync(u => u.Email == userRequest.Email))
            {
                return Conflict("User with this email already exists.");
            }

            try 
            {
                // Hash the password
                string hashedPassword = HashPassword(userRequest.Password);

                // Create user entity from request model
                var user = new User
                {
                    FirstName = userRequest.FirstName,
                    LastName = userRequest.LastName,
                    Email = userRequest.Email,
                    Password = hashedPassword
                };

                // Add user to database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Prepare response model
                var userResponse = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                //return Ok(userResponse);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PostUser");
                throw;
            }
            
        }

        // PUT: api/v1/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest userRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FirstName = userRequest.FirstName;
            user.LastName = userRequest.LastName;
            user.Email = userRequest.Email;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/v1/Users/5/update-password
        [HttpPut("{id}/update-password")]
        public async Task<IActionResult> UpdatePassword(int id, UpdatePasswordRequest updatePasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if new password and repeatPassword match
            if (updatePasswordRequest.NewPassword != updatePasswordRequest.RepeatNewPassword)
            {
                return BadRequest("New passwords do not match.");
            }

            // Verify password
            if (!VerifyPassword(updatePasswordRequest.OldPassword, user.Password))
            {
                return BadRequest("Old password is incorrect.");
            }

            // Hash the new password
            string hashedPassword = HashPassword(updatePasswordRequest.NewPassword);

            // Update user password
            user.Password = hashedPassword;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/v1/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/v1/Users/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verify password
            if (!VerifyPassword(loginRequest.Password, user.Password))
            {
                return BadRequest("Invalid password.");
            }

            // Authentication successful, generate JWT token
            var token = GenerateJwtToken(user);

            // Prepare response object
            var response = new
            {
                id = user.Id,
                email = user.Email,
                token = token
            };

            // Return token in response
            return Ok(response);
        }

        private string GenerateJwtToken(User user)
        {
            if (user == null || user.Id == 0 || string.IsNullOrEmpty(user.Email))
            {
                throw new ArgumentNullException(nameof(user), "User object is null or incomplete.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            // Retrieve salt from configuration
            string salt = _configuration["HashingSettings:Salt"];

            // Convert salt to byte array
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Hash the password using PBKDF2 with 10000 iterations and a 256-bit salt
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            // Retrieve salt from configuration
            string salt = _configuration["HashingSettings:Salt"];

            // Convert salt to byte array
            byte[] saltBytes = Convert.FromBase64String(salt);

            // Hash the input password using PBKDF2 with 10000 iterations and a 256-bit salt
            string hashedInputPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Compare the hashed passwords
            return hashedInputPassword.Equals(hashedPassword);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
