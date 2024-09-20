using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Project.Models
{
    public class TitleAuthor
    {
        public string au_id { get; set; }
        public string title_id { get; set; }
        public Nullable<byte> au_ord { get; set; }
        public Nullable<int> royaltyper { get; set; }

        
    }
}