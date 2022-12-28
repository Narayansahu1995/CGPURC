using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Private_University.Models
{
    public class ChangePassword
    {
        public int Login_ID { get; set; }

        [Display(Name = "Current Password")]
        [Required(ErrorMessage = "Please enter Current Password.")]
        public string CurrentPassword { get; set; }


        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Please enter New Password.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [Required(ErrorMessage = "Please Reenter New Password.")]
        public string ConfirmPassword { get; set; }


        //[Display(Name = "Captcha")]
        //[Required(ErrorMessage = "Please enter Captcha Value.")]
        //public string captchaValue { get; set; }
        //public string IpAddress { get; set; }
    }
}