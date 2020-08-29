using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

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

        public string Favorite { get; set; }

        public Section Favor()
        {
            string[] favorite = Favorite.Split(',');
            int max = 0;
            int index = -1;
            for(int i=0;i<9;i++)
            {
                if (int.Parse(favorite[i]) > max)
                {
                    max = int.Parse(favorite[i]);
                    index = i;
                }
            }

            switch (index)
            {
                case 0:
                    return Section.Sport;
                case 1:
                    return Section.Business;
                case 2:
                    return Section.Culture;
                case 3:
                    return Section.Food;
                case 4:
                    return Section.Celebs;
                case 5:
                    return Section.Fashion;
                case 6:
                    return Section.Health;
                case 7:
                    return Section.Tourism;
                case 8:
                    return Section.Other;
            }

            return Section.Other;
        }
    }
}