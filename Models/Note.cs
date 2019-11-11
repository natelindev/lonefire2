using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using lonefire.Models.UtilModels;

namespace lonefire.Models
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string TitleZh { get; set; }

        [Required]
        public Guid Owner { get; set; }

        public string Content { get; set; }

        public string ContentZh { get; set; }

        public Status Status { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Content == null && ContentZh == null)
                yield return new ValidationResult($"{Startup.Localizer["Content"]} {Startup.Localizer["must not be empty"]}");
        }

    }
}