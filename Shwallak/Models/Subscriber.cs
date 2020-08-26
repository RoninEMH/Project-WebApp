using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shwallak.Models
{
    public class Subscriber
    {
        [Key, Display(Name = "subscriber id")]
        public int SubscriberID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter age")]
        [Range(1, 120, ErrorMessage = "you must enter number in a range of 1-120")]
        [Display(Name = "age")]
        public int Age { get; set; }

        [Display(Name = "gender")]
        public Gender Gender { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter nickname")]
        [Display(Name = "nickname")]
        public string Nickname { get; set; }

        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "you must enter password")]
        [Display(Name = "password")]
        public string Password { get; set; }
    }
}