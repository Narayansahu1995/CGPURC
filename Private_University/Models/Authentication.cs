using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Private_University.Models
{
    public class Authentication
    {
        [Display(Name = "Academic Session")]
        [Required(ErrorMessage = "Select Academic Session.")]
        public int Session_ID { get; set; }

        [Display(Name = "Login ID")]
        [Required(ErrorMessage = "Enter Login ID.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter Password.")]
        public string Password { get; set; }

        [Display(Name = "Captcha")]
        [Required(ErrorMessage = "Please enter Captcha Value.")]
        public string captchaValue { get; set; }
        public string IpAddress { get; set; }
        // public DateTime Login_DateTime { get; set; }
    }
}