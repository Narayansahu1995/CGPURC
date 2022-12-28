using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Private_University.Models
{
    public class ForgotPassword
    {
        [Display(Name = "Login ID")]
        [Required(ErrorMessage = "Please enter Login ID.")]
        public string UserName { get; set; }

        [Display(Name = "e-Mail ID")]
        [Required(ErrorMessage = "Please enter e-Mail ID.")]
        [EmailAddress(ErrorMessage = "Please enter valid e-Mail ID (Ex: abcd@xyz.com).")]
        public string EmailID { get; set; }

    }

    public class ForgotPassword_Response
    {
        public string Name { get; set; }
        public string EmailID { get; set; }
        public string Mobile { get; set; }
        public bool Response { get; set; }
        public string Message { get; set; }

    }
}