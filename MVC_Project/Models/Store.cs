using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class Store
    {
        [Display(Name= "Store ID")]
        public string stor_id { get; set; }

        [Display(Name = "Store Name")]
        public string stor_name { get; set; }

        [Display(Name = "Store Address")]
        public string stor_address { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Zipcode")]
        public string zip { get; set; }
    }
}