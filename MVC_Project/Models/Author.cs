using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class Author
    {
        [DisplayName("Author ID")]
        public string au_id { get; set; }

        [DisplayName("Author Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name must contain only letters.")]
        public string au_lname { get; set; }

        [DisplayName("Author First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name must contain only letters.")]
        public string au_fname { get; set; }

        [DisplayName("Author Phone Number")]
        public string phone { get; set; }

        [DisplayName("Author Address")]
        public string address { get; set; }

        [DisplayName("Author City")]
        public string city { get; set; }

        [DisplayName("Author State")]
        public string state { get; set; }

        [DisplayName("Author Zip")]
        public string zip { get; set; }


        [DisplayName("Contract")]
        public bool contract { get; set; }
    }
}