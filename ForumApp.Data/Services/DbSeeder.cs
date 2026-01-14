using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data.Services
{
    public static class DbSeeder
    {
        public static void SeedCategories(ForumDbContext context)
        {
            if (context.Categories.Any()) return;

            string filePath = "categories.json";
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var categories = JsonConvert.DeserializeObject<List<Category>>(json);

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        public static void SeedAdmin(ForumDbContext context)
        {
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Password = "123456",
                    Role = Role.Administrator
                };
                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
    }
}
