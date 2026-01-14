using ForumApp.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data.Services
{
    public class CommentService
    {
        private readonly ForumDbContext _context;

        public CommentService(ForumDbContext context)
        {
            _context = context;
        }

        public List<CommentViewDTO> GetCommentsByPostId(int postId)
        {
            return _context.Comments
                .Where(c => c.PostId == postId)
                .Select(c => new CommentViewDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    AuthorName = c.Author.Username,
                    CreatedOn = c.CreatedOn
                })
                .OrderBy(c => c.CreatedOn)
                .ToList();
        }

        public void AddComment(CommentCreateDTO dto)
        {
            var comment = new Comment
            {
                Content = dto.Content,
                PostId = dto.PostId,
                AuthorId = dto.AuthorId,
                CreatedOn = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();
        }
    }
}
