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

        [Display(Name = "age")]
        public int Age { get; set; }

        [Display(Name = "gender")]
        public Gender Gender { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "nickname")]
        public string Nickname { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "password")]
        public string Password { get; set; }
    }
}