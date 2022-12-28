using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Private_University.Models
{
    public class UploadFileValidate
    {
    }



    //Customized data annotation validator for uploading file
    public class ValidateFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int MaxContentLength = 1024 * 1024 * 3; //3 MB Max Size of File
            string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".pdf" };

            var file = value as HttpPostedFileBase;

            if (file == null)
                return false;
            else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = "Please upload Your Photo of type: " + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = "Your Photo is too large, maximum allowed size is : " + (MaxContentLength / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
    }


    //Customized data annotation validator for uploading file
    public class ValidateImgFileAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int MaxContentLength = 102400*5; //500 KB Max Size of File, 102400 is a 100 KB.
            string[] AllowedFileExtensions = new string[] { ".jpg", ".jpeg" };

            var file = value as HttpPostedFileBase;

            if (file == null)
                return false;
            else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
            {
                ErrorMessage = "Please upload Your Photo of type: " + string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.ContentLength > MaxContentLength)
            {
                ErrorMessage = "Your Photo is too large, maximum allowed size is : " + (MaxContentLength).ToString() + "KB";
                return false;
            }
            else
                return true;
        }
    }
}