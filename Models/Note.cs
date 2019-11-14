using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using lonefire.Models.UtilModels;

namespace lonefire.Models
{
    public class Note
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [DefaultValue(0)]
        public int LikeCount { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }

        // Navigation
        public virtual List<NoteImage> NoteImages { get; set; }
        public virtual List<NoteTag> NoteTags { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Content == null && ContentZh == null)
                yield return new ValidationResult("Content must not be empty");
        }

    }
}