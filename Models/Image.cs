using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using lonefire.Models.UtilModels;

namespace lonefire.Models
{
    public class Image : IEntityDate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(512)]
        public string Path { get; set; }

        [Required]
        [StringLength(256)]
        public string Filename { get; set; }

        //Image lazy loading
        public int Width { get; set; }
        public int Height { get; set; }
        public string Color { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }

        //Navigation
        public virtual Note Note { get; set; }
        public virtual ApplicationUser UserAvatar { get; set; }
        public virtual Article Article { get; set; }
        public virtual List<ArticleImage> ArticleImages { get; set; }
        public virtual List<NoteImage> NoteImages { get; set; }

    }
}
