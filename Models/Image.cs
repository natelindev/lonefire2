using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace lonefire.Models
{
    public class Image
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? ArticleId { get; set; }
        public Guid? NoteId { get; set; }

        [StringLength(512)]
        public string Path { get; set; }

        [Required]
        [StringLength(256)]
        public string Filename { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }

        public virtual Article Article { get; set; }

        [ForeignKey("ArticleId")]
        public virtual Article Articles { get; set; }

        [ForeignKey("NoteId")]
        public virtual Note Note { get; set; }

        [InverseProperty("Avatar")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        //Image lazy loading
        public int Width { get; set; }

        public int Height { get; set; }

        public string Color { get; set; }
    }
}
