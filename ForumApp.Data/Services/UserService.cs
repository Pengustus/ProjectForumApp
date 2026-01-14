using ForumApp.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data.Services
{
    public class UserService
    {
        private readonly ForumDbContext _context;

        public UserService(ForumDbContext context)
        {
            _context = context;
        }

        public bool RegisterUser(UserRegisterDTO dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
            {
                return false;
            }

            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                Role = Role.RegisteredUser
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public UserSessionDTO LoginUser(UserLoginDTO dto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == dto.Username && u.Password == dto.Password);

            if (user == null) return null;

            return new UserSessionDTO
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}
