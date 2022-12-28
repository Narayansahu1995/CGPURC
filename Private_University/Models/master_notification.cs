using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Private_University.Models
{
    public class master_notification
    {
        public int count { get; set; }
        public int Notification_ID { get; set; }
        [Display(Name = "Notification Subject/Head")]
        public string Notification_Head { get; set; }
        [Display(Name = "Notification")]
        public string Notification_Details { get; set; }
        [Display(Name = "File")]
        public string Notification_File { get; set; }
        [Display(Name = "Date of Notification")]
        [DataType(DataType.Date)]
        public string Notification_Date { get; set; }
    }

    public class master_notification_Insert
    {
        [Display(Name = "Notification Subject/Head")]
        [Required(ErrorMessage = "Please enter Notification Subject/Head.")]
        public string Notification_Head { get; set; }

        [Display(Name = "Notification Description")]
        [Required(ErrorMessage = "Please enter Notification Details.")]
        public string Notification_Details { get; set; }

        [Display(Name = "Notification File")]
        [Required(ErrorMessage = "Please select Notification File.")]
        public string Notification_File { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Upload File")]
        [Required(ErrorMessage = "Please select Notification File.")]
        [ValidateFile]
        public HttpPostedFileBase File_Upload { get; set; }

        [Display(Name = "Date of Notification")]
        [Required(ErrorMessage = "Please enter Notification Date.")]
        [DataType(DataType.Date)]
        public string Notification_Date { get; set; }

        public int Entry_By { get; set; }
    }

    public class master_notification_Delete
    {
        public int Notification_ID { get; set; }

        public int Deleted_By { get; set; }
    }


   

}