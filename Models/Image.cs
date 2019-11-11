using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace lonefire.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(512, MinimumLength = 0)]
        public string Path { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 0)]
        public string Filename { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset EditTime { get; set; }


        //These are for lazy loading, Not yet implemented so not mapped
        [NotMapped]
        public int Width { get; set; }

        [NotMapped]
        public int Height { get; set; }

        //Should store hex color string
        [NotMapped]
        public string Color { get; set; }
    }
}
