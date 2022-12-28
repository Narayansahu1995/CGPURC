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
    public class News
    {
        public int count { get; set; }
        public int news_ID { get; set; }
        [DisplayName("News Subject Name")]
        [Required(ErrorMessage = "News Subject Name can not be blank")]
        public string News_Title { get; set; }

        [DisplayName("News Description")]
        [Required(ErrorMessage = "News Descrption can not be blank")]
        public string News_Descrip { get; set; }

        [DisplayName("News Upload")]
        public string News_File { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        public int Entry_By { get; set; }
    }


    public class News_Insert
    {
        [DisplayName("News Subject Name")]
        [Required(ErrorMessage = "News Subject Name can not be blank")]
        public string News_Title { get; set; }

        [DisplayName("News Description")]
        [Required(ErrorMessage = "News Descrption can not be blank")]
        public string News_Descrip { get; set; }

        [Display(Name = "News File")]
        [Required(ErrorMessage = "Please Select File.")]
        public string News_File { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Choose File")]
        [Required(ErrorMessage = "Please select File Only JPG/JPEG or PDF File Format.")]
        [ValidateFile]
        public HttpPostedFileBase File_Upload { get; set; }
        public int Entry_By { get; set; }
    }

    public class News_Delete
    {
        public int news_ID { get; set; }
        public int Deleted_By { get; set; }
    }
}