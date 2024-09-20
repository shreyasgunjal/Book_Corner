using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class StoreRequest
    {
        public int RequestId { get; set; }
        public string StoreId { get; set; }
        public string PublisherId { get; set; }
        public string TitleId { get; set; }
        public string RequestDetails { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }

        
    }
}