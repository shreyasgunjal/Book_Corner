using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class Title
    {
        [Display(Name = "Title ID")]
        public string title_id { get; set; }

        [Display(Name = "Title")]
        public string title1 { get; set; }

        [Display(Name = "Type")]
        public string type { get; set; }

        [Display(Name = "Pub ID")]
        public string pub_id { get; set; }

        [Display(Name = "Price")]
        public Nullable<decimal> price { get; set; }

        [Display(Name = "Advance")]
        public Nullable<decimal> advance { get; set; }

        [Display(Name = "Royalty")]
        public Nullable<int> royalty { get; set; }

        [Display(Name = "Ytd Sales")]
        public Nullable<int> ytd_sales { get; set; }

        [Display(Name = "Notes")]
        public string notes { get; set; }

        [Display(Name = "Pub Date")]
        public System.DateTime pubdate { get; set; }
    }
}