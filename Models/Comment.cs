using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using lonefire.Models;

namespace lonefire.Models
{
    public class Comment
    {
        [Display(Name = "编号")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Article Article { get; set; }

        [Required]
        public Guid? ParentId { get; set; }

        public string Content { get; set; }
        public string ContentZh { get; set; }

        [Required]
        public Guid Owner { get; set; }

        public string Website { get; set; }

        public string Email { get; set; }

        [ForeignKey("ParentId")]
        public List<Comment> Comments { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Content == null && ContentZh == null)
                yield return new ValidationResult("Content must not be empty");
        }
    }
}
