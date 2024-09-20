using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class Sale
    {
        [Display(Name = "Store ID")]
        public string stor_id { get; set; }

        [Display(Name = "Order No.")]
        public string ord_num { get; set; }

        [Display(Name = "Order Date")]
        public System.DateTime ord_date { get; set; }

        [Display(Name = "Quantity")]
        public short qty { get; set; }

        [Display(Name = "Payterms")]
        public string payterms { get; set; }

        [Display(Name = "Title ID")]
        public string title_id { get; set; }
    }
}