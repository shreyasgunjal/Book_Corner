using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class Publisher
    {
        [DisplayName("Publisher ID")]
        public string pub_id { get; set; }
        [DisplayName("Name")]
        public string pub_name { get; set; }
        [DisplayName("City")]
        public string city { get; set; }
        [DisplayName("State")]
        public string state { get; set; }
        [DisplayName("Country")]
        public string country { get; set; }
    }
}