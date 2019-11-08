using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace lonefire.Models
{
    public class ApplicationUser : IdentityUser
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
