using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace lonefire.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [NotMapped]
        public string IdBase64 { get => Id.Base64UrlEncode(); }

        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(128)]
        public string NameZh { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }
        [StringLength(1024)]
        public string DescriptionZh { get; set; }

        public Guid? AvatarId { get; set; }

        public DateTimeOffset RegisterTime { get; set; }
        public DateTimeOffset? LastLoginTime { get; set; }

        // Navigation
        public virtual Image Avatar { get; set; }
        public virtual List<Article> Articles { get; set; }
        public virtual List<Note> Notes { get; set; }
    }
}
