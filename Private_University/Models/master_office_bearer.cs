using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Private_University.Models
{
    public class master_office_bearer
    {
        public int count { get; set; }
        public int office_bearer_ID { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Picture")]
        public string Picture { get; set; }
        [Display(Name = "Other Details")]
        public string Other { get; set; }
    }

    public class master_office_bearer_Insert
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please Enter Name.")]
        public string Name { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Picture")]
        [Required(ErrorMessage = "Please Select Picture.")]
        public string Picture { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Choose File")]
        [Required(ErrorMessage = "Please select Image File Only JPG/JPEG File Format.")]
        [ValidateImgFile]
        public HttpPostedFileBase File_Upload { get; set; }

        [Display(Name = "Other Details")]
        [Required(ErrorMessage = "Please Enter Other Details.")]
        public string Other { get; set; }

        public int Entry_By { get; set; }
    }

    public class master_office_bearer_Delete
    {
        public int office_bearer_ID { get; set; }

        public int Deleted_By { get; set; }
    }




}