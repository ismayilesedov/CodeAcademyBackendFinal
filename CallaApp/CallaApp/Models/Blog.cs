﻿namespace CallaApp.Models
{
    public class Blog: BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public ICollection<BlogImage> BlogImage { get; set; }
        public ICollection<BlogComment> BlogComments { get; set; }
    }
}
