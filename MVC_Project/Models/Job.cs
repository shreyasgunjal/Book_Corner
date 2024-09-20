using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class Job
    {
        [Display(Name = "Job ID")]
        public short job_id { get; set; }

        [Display(Name = "Job Description")]
        public string job_desc { get; set; }

        [Display(Name = "Minimum level")]
        public byte min_lvl { get; set; }

        [Display(Name = "Maximum Level")]
        public byte max_lvl { get; set; }
    }
}