using ForumApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data.DTO
{
    public class UserSessionDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }

        public bool IsAdmin => Role == Role.Administrator;
        public bool IsRegistered => Role == Role.RegisteredUser || Role == Role.Administrator;
        public bool IsGuest => Id == 0;
    }
}
