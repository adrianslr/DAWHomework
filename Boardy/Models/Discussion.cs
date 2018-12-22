using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Boardy.Models
{
    public class Discussion
    {
        [Key]
        public int DiscussionID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public virtual Category Category { get; set; }
        [Required]
        public virtual ApplicationUser Author { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public virtual ICollection<Reply> Replies { get; set; }

        public Discussion()
        {

        }

        public Discussion(string title, string text, Category category, ApplicationUser author, DateTime date)
        {
            Title = title;
            Text = text;
            Category = category;
            Author = author;
            Date = date;
        }
    }
}