using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }


        public ICollection<Post> Posts { get; set; } = new List<Post>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public enum Role
    {
        RegisteredUser,
        Administrator
    }
}
