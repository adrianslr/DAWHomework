using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Boardy.Models
{
    public class Reply
    {
        [Key]
        public int ReplyID { get; set; }
        [Required]
        public string Text { get; set; }

        public virtual Discussion Discussion { get; set; }
        [Required]
        public virtual ApplicationUser Author { get; set; }
        [Required]
        public DateTime Date { get; set; }

        public Reply()
        {

        }

        public Reply(string text, Discussion discussion, ApplicationUser author, DateTime date)
        {
            Text = text;
            Discussion = discussion;
            Author = author;
            Date = date;
        }
    }
}