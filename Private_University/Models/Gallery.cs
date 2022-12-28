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
    public class Gallery
    {
        public int count { get; set; }
        public int photo_ID { get; set; }
        [DisplayName("Photo Caption Name")]
        [Required(ErrorMessage = "Photo Caption Name can not be blank")]
        public string Name { get; set; }

        [DisplayName("Photo Upload")]
        public string Picture { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        public int Entry_By { get; set; }

    }

    public class Gallery_Insert
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please Enter Name.")]
        public string Name { get; set; }

        [Display(Name = "Picture")]
        [Required(ErrorMessage = "Please Select Picture.")]
        public string Picture { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Choose File")]
        [Required(ErrorMessage = "Please select Image File Only JPG/JPEG File Format.")]
        [ValidateImgFile]
        public HttpPostedFileBase File_Upload { get; set; }
        public int Entry_By { get; set; }
    }

    public class Gallery_Delete
    {
        public int photo_ID { get; set; }

        public int Deleted_By { get; set; }
    }
}