using ForumApp.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data.Services
{
    public class AdminService
    {
        private readonly ForumDbContext _context;

        public AdminService(ForumDbContext context)
        {
            _context = context;
        }

        public List<UserSessionDTO> GetAllUsers()
        {
            return _context.Users
                .Select(u => new UserSessionDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role
                })
                .ToList();
        }

        public bool DeletePost(int postId)
        {
            var post = _context.Posts.Find(postId);
            if (post == null) return false;

            _context.Posts.Remove(post);
            _context.SaveChanges();
            return true;
        }
    }
}
