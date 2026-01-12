using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumApp.Data
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public User Author { get; set; }

        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
