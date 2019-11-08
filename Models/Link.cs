using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lonefire.Models
{
    public class Link
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(512, ErrorMessage = "{0} must have a length between {1} and {2} ", MinimumLength = 0)]
        public string Url { get; set; }

        [StringLength(256, ErrorMessage = "{0} must have a length between {1} and {2} ", MinimumLength = 0)]
        public string Description { get; set; }

        [StringLength(256, ErrorMessage = "{0} must have a length between {1} and {2} ", MinimumLength = 0)]
        public string DescriptionZh { get; set; }

        public string IconUrl { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }
    }
}
