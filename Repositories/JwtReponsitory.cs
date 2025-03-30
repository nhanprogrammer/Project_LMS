using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project_LMS.Repositories
{
    public class JwtReponsitory : IJwtReponsitory
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public JwtReponsitory(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Role", user.Role.Name.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            // Trả token, trả list roles, trả list permissons
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<User> AuthenticateAsync(string userName, string password)
        {
            var account = await _context.Users
                .FirstOrDefaultAsync(a => a.Username == userName);

            if (account != null && BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                return account;
            }

            return null;
        }
        public async Task<User> RegisterAsync(string username, string password/*, string email*/)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                return null;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                FullName = username,
                Password = password,
                //Email = email,
                //Role = 1
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}
