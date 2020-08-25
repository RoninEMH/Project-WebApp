using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Razor.Generator;

namespace Shwallak.Models
{
    public class Article
    {
        [Key, Display(Name = "id")]
        public int ArticleID { get; set; }

        [Display(Name = "title")]
        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        [DataType(DataType.MultilineText), Display(Name = "content")]
        [Required(AllowEmptyStrings = false)]
        public string Content { get; set; }

        [Display(Name = "upload year")]
        public int Year { get; set; }

        [Display(Name = "upload month")]
        public int Month { get; set; }

        [Display(Name = "upload day")]
        public int Day { get; set; }

        [Display(Name = "subscribers only")]
        public bool SubscribersOnly { get; set; }

        [Display(Name = "section")]
        public Section Section { get; set; }

        public ICollection<Comment> Comments { get; set; }

        [Display(Name = "watches")]
        public int Watches { get; set; }

        [Display(Name = "writer")]
        public Writer Writer { get; set; }

        [Display(Name = "writer id")]
        public int WriterID { get; set; }

    }
}