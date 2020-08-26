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
        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter name")]
        public string FullName { get; set; }

        [Display(Name = "gender")]
        [Required(AllowEmptyStrings = false)]
        public Gender Gender { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter email address")]
        public string Email { get; set; }

        [Display(Name = "Year of Hiring")]
        public int Year { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter password")]
        public string Password { get; set; }

        [Display(Name = "address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter adress")]
        public string Address { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter age")]
        [Range(1, 120, ErrorMessage = "you must enter number in a range of 1-120")]
        [Display(Name = "Age")]
        public int Age { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}