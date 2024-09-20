using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class Employee
    {
        [Display(Name = "Employee ID")]
        public string emp_id { get; set; }
        [Display(Name = "First Name")]
        public string fname { get; set; }
        [Display(Name = "Minit")]
        public string minit { get; set; }
        [Display(Name = "Last Name")]
        public string lname { get; set; }
        [Display(Name = "Job ID")]
        public short job_id { get; set; }
        [Display(Name = "Job LVL")]
        public Nullable<byte> job_lvl { get; set; }
        [Display(Name = "Pub ID")]
        public string pub_id { get; set; }
        [Display(Name = "Hire Date")]
        public System.DateTime hire_date { get; set; }
    }
}