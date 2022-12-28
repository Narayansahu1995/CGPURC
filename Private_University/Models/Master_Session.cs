using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Private_University.Models
{
    public class Master_Session
    {
        public int Count { get; set; }
        public int Session_ID { get; set; }

        [Display(Name = "Academic Session")]
        public string Session_Name { get; set; }
    }

    public class Master_Session_Insert
    {
        [Display(Name = "Academic Session")]
        public string Session_Name { get; set; }
        public int Entry_By { get; set; }
    }

    public class Master_Session_Delete
    {
        public int Session_ID { get; set; }
        public int Deleted_By { get; set; }
    }


}