using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using lonefire.Models;

namespace lonefire.Models
{
    public class Comment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }
        public Guid? ArticleId { get; set; }

        public string Content { get; set; }
        public string ContentZh { get; set; }

        [Required]
        public Guid OwnerId { get; set; }

        [Url]
        public string Website { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DefaultValue(0)]
        public int LikeCount { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }

        // Navigation
        public virtual ApplicationUser Owner { get; set; }
        public virtual Article Article { get; set; }
        public virtual List<Comment> Comments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Content == null && ContentZh == null)
                yield return new ValidationResult("Content must not be empty");
        }

      
    }
}
