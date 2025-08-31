using Myblog.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Myblog.ViewModel

{
    public class BlogFormModel
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public string? RejectionReason { get; set; } // Store reason for rejection
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }

}

