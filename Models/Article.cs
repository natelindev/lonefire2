using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using lonefire.Models.UtilModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace lonefire.Models
{
    public class Article : IEntityDate
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(128)]
        public string Title { get; set; }

        [StringLength(128)]
        public string TitleZh { get; set; }

        public Guid? OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public string Content { get; set; }
        public string ContentZh { get; set; }

        [StringLength(32)]
        public string StatusValue { get; set; }

        [NotMapped]
        [JsonIgnore]
        public Status Status
        {
            get => Status.From(StatusValue);
            set { StatusValue = value; }
        }

        public Guid? HeaderImageId { get; set; }
        public Image HeaderImage { get; set; }

        [DefaultValue(0)]
        public int ViewCount { get; set; }

        [DefaultValue(0)]
        public int LikeCount { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }

        // Navigaation
        public virtual List<ArticleImage> ArticleImages { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<ArticleTag> ArticleTags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Title == null && TitleZh == null)
                yield return new ValidationResult("Title must not be empty");
        }
    }
}