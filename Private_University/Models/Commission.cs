using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Private_University.App_Code;

namespace Private_University.Models
{
    public class AddUsers
    {
        [DisplayName("Login User Name")]
        [Required(ErrorMessage = "Login User Name can not be blank")]
        public string Name { get; set; }

        [DisplayName("Designation")]
        [Required(ErrorMessage = "Designation can not be blank")]
        public string Designation { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        [DisplayName("Email ID")]
        [Required(ErrorMessage = "Email ID can not be blank")]
        public string Email_ID { get; set; }

        [DisplayName("Mobile Number")]
        public string Mobile_Number { get; set; }
        public List<SelectListItem> PopulateMasterUniversity { get; set; }
        public List<SelectListItem> PopulateMasterRoles { get; set; }
        public string Role_ID { get; set; }
        public string Is_Active { get; set; }
        public string Entry_By { get; set; }

        [DisplayName("University ID")]
        public string University_ID { get; set; }
    }

    public class UsersShow
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email_ID { get; set; }
        public string Mobile_Number { get; set; }
        public string Role_ID { get; set; }
        public string University_ID { get; set; }
    }

    public class MasterUniversity
    {
        public int University_ID { get; set; }
        [DisplayName("University Name")]
        [Required(ErrorMessage = "Name can not be blank")]
   
        public string University_Name { get; set; }
        public int Entry_By { get; set; }
    }

    public class MasterUniversityShow
    {
        public int count { get; set; }
        public string University_ID { get; set; }
        public string University_Name { get; set; }
        public string Contact_Number { get; set; }
        public string Email_ID { get; set; }
        public string Address { get; set; }
        public string Pin_Code { get; set; }
        public string Website_URL { get; set; }
        public string Univsersity_Logo { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }
        public string University_Details { get; set; }
        public string Establishment_Year { get; set; }
        public string Registration_Number { get; set; }
        public int Entry_By { get; set; }

    }

    public class CommissionAddress
    {
        [DisplayName("Address")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Addressss { get; set; }

        [DisplayName("Address Hindi")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Address_Hi { get; set; }

        [DisplayName("Contact Number")]
        [Required(ErrorMessage = "Contact Number can not be blank")]
        public string Contact_No { get; set; }

        [DisplayName("Email ID")]
        [Required(ErrorMessage = "Email ID can not be blank")]
        public string Email_ID { get; set; }
    }

    public class CommissionAddressUpdate
    {
        [DisplayName("Address")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Addressss { get; set; }

        [DisplayName("Address Hindi")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Address_Hi { get; set; }

        [DisplayName("Contact Number")]
        [Required(ErrorMessage = "Contact Number can not be blank")]
        public string Contact_No { get; set; }
        [DisplayName("Email ID")]
        [Required(ErrorMessage = "Email ID can not be blank")]
        public string Email_ID { get; set; }
    }


}