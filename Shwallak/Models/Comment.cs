using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Shwallak.Models
{
    public class Comment
    {
        [Key, Display(Name = "comment id")]
        public int CommentID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "author")]
        public string Author { get; set; }

        [Display(Name = "upload year")]
        public int Year { get; set; }

        [Display(Name = "upload month")]
        public int Month { get; set; }

        [Display(Name = "upload day")]
        public int Day { get; set; }

        [Display(Name = "upload hour")]
        public int Hour { get; set; }

        [Display(Name = "upload minute")]
        public int Minute { get; set; }

        [Display(Name = "upload second")]
        public int Second { get; set; }

        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.MultilineText), Display(Name = "content")]
        public string Content { get; set; }

        [Display(Name = "watches")]
        public int Watches { get; set; }

        [Display(Name = "article")]
        public Article Article { get; set; }

        [Display(Name = "article id")]
        public int ArticleID { get; set; }
    }
}