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
        [Required(AllowEmptyStrings = false)]
        public string FullName { get; set; }

        [Display(Name = "gender")]
        [Required(AllowEmptyStrings = false)]
        public Gender Gender { get; set; }

        [Display(Name = "email")]
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Display(Name = "Year of Hiring")]
        public int Year { get; set; }

        [Display(Name = "password")]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        [Display(Name = "address")]
        [Required(AllowEmptyStrings = false)]
        public string Address { get; set; }

        [Display(Name = "Age")]
        public int Age { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}