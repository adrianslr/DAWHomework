using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Boardy.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Discussion> Discussions { get; set; }

        public Category(string name)
        {
            Name = name;
        }

        public Category()
        {

        }
    }
}