using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.App_Code;
using Private_University.Resources;

namespace Private_University.Models
{
    public class Feedback
    {

        //[DisplayName("Name")]
        [Display(ResourceType = typeof(Feedbackform), Name = "Name")]
        [Required(ErrorMessage = "Name can not be blank")]
        public string Name { get; set; }

        //[DisplayName("Contact Number")]
        [Display(ResourceType = typeof(Feedbackform), Name = "ContactNumber")]
        [Required(ErrorMessage = "Contact Number can not be blank")]
        public string Contact_Number { get; set; }

        //[DisplayName("E-Mail ID")]
        [Display(ResourceType = typeof(Feedbackform), Name = "EMailID")]
        [Required(ErrorMessage = "E-Mail ID can not be blank")]
        public string Email_ID { get; set; }

        //[DisplayName("Subject")]
        [Display(ResourceType = typeof(Feedbackform), Name = "Subject")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Subject { get; set; }

        //[DisplayName("Feedback")]
        [Display(ResourceType = typeof(Feedbackform), Name = "Feedback")]
        [Required(ErrorMessage = "Feedback can not be blank")]
        public string Feedback_data { get; set; }
        public string Ip_address { get; set; }

    }

}