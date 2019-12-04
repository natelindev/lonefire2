using System;

namespace lonefire.Models
{
    public class ArticleImage
    {
        public Guid ArticleId { get; set; }

        public Article Article { get; set; }

        public Guid ImageId { get; set; }

        public Image Image { get; set; }
    }
}
