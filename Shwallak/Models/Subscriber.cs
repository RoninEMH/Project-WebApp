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

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "nickname")]
        public string Nickname { get; set; }

        [Display(Name = "password")]
        public string Password { get; set; }
    }
}