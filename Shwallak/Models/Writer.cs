using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shwallak.Models
{
    public class Writer
    {
        [Key, Display(Name = "writer id")]
        public int WriterID { get; set; }

        [Display(Name = "full name")]
        public string FullName { get; set; }

        [Display(Name = "gender")]
        public Gender Gender { get; set; }

        [Display(Name = "email")]
        public string Email { get; set; }

        [Display(Name = "Year of Hiring")]
        public int Year { get; set; }

        [Display(Name = "password")]
        public string Password { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}