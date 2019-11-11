﻿using System;
using Microsoft.AspNetCore.Identity;

namespace lonefire.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string NameZh { get; set; }

        public string Description { get; set; }
        public string DescriptionZh { get; set; }

        public Image Avatar { get; set; }

        public DateTimeOffset RegisterTime { get; set; }

        public DateTimeOffset? LastLoginTime { get; set; }
    }
}
