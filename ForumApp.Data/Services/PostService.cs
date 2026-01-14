using ForumApp.Data.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data.Services
{
    public class PostService
    {
        private readonly ForumDbContext _context;

        public PostService(ForumDbContext context)
        {
            _context = context;
        }

        public List<PostViewDTO> GetAllPosts()
        {
            return _context.Posts
                .Select(p => new PostViewDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    AuthorName = p.Author.Username,
                    CategoryName = p.Category.Name,
                    CreatedOn = p.CreatedOn
                })
                .OrderByDescending(p => p.CreatedOn)
                .ToList();
        }

        public void CreatePost(PostCreateDTO dto)
        {
            var post = new Post
            {
                Title = dto.Title,
                Content = dto.Content,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                CreatedOn = DateTime.Now
            };

            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public string ImportTags(string jsonContent)
        {
            var tagDtos = JsonConvert.DeserializeObject<List<TagImportDTO>>(jsonContent);
            int addedCount = 0;

            foreach (var dto in tagDtos)
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Length > 20) continue;

                if (!_context.Tags.Any(t => t.Name == dto.Name))
                {
                    _context.Tags.Add(new Tag { Name = dto.Name });
                    addedCount++;
                }
            }

            _context.SaveChanges();
            return $"Успешно добавени тагове: {addedCount}";
        }

        public void ExportPosts(string filePath)
        {
            var data = _context.Posts
                .Select(p => new
                {
                    Title = p.Title,
                    Author = p.Author.Username,
                    Category = p.Category.Name,
                    Date = p.CreatedOn.ToString("dd/MM/yyyy")
                })
                .ToList();

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public Dictionary<int, string> GetCategories()
        {
            return _context.Categories.ToDictionary(c => c.Id, c => c.Name);
        }
    }
}
