﻿namespace WuYanWebApi.Models
{
    public class Article
    {
        public int Id {get; set;}
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Auther {get; set;}
        public string? Content { get; set; }
        public DateTime PublishDate { get; set; }
    }
}               
