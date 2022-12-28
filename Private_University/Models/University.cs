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
    public class University
    {

        [DisplayName("University Name")]
        [Required(ErrorMessage = "Name can not be blank")]
        public string University_Name { get; set; }

        [DisplayName("University Contact Number")]
        [Required(ErrorMessage = "Contact Number can not be blank")]
        public string Contact_Number { get; set; }

        [DisplayName("University E-Mail ID")]
        [Required(ErrorMessage = "E-Mail ID can not be blank")]
        public string Email_ID { get; set; }

        [DisplayName("University Address")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Address { get; set; }


        [DisplayName("University Pin Code")]
        [Required(ErrorMessage = "Pin Code can not be blank")]
        public string Pin_Code { get; set; }

        [DisplayName("University Website URL")]
        [Required(ErrorMessage = "Website URL can not be blank")]
        public string Website_URL { get; set; }

        [DisplayName("University Logo Upload")]
        //[Required(ErrorMessage = "University Logo can not be blank")]
        public string Univsersity_Logo { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        [DisplayName("University Details")]
        [Required(ErrorMessage = "Details can not be blank")]
        public string University_Details { get; set; }

        [DisplayName("Establishment Year")]
        [Required(ErrorMessage = "Establish Year can not be blank")]
        public string Establishment_Year{ get; set; }

        [DisplayName("Registration Number")]
        [Required(ErrorMessage = "Registration Number can not be blank")]
        public string Registration_Number { get; set; }
        public int Entry_By { get; set; }
       
    }

    public class UniversityShow
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

    public class UniversityUpdate
    {
        public string University_ID { get; set; }

        [DisplayName("University Name")]
        [Required(ErrorMessage = "Name can not be blank")]
        public string University_Name { get; set; }

        [DisplayName("University Contact Number")]
        [Required(ErrorMessage = "Contact Number can not be blank")]
        public string Contact_Number { get; set; }

        [DisplayName("University E-Mail ID")]
        [Required(ErrorMessage = "E-Mail ID can not be blank")]
        public string Email_ID { get; set; }

        [DisplayName("University Address")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Address { get; set; }


        [DisplayName("University Pin Code")]
        [Required(ErrorMessage = "Pin Code can not be blank")]
        public string Pin_Code { get; set; }

        [DisplayName("University Website URL")]
        [Required(ErrorMessage = "Website URL can not be blank")]
        public string Website_URL { get; set; }

        [DisplayName("University Logo Upload")]
        //[Required(ErrorMessage = "University Logo can not be blank")]
        public string Univsersity_Logo { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        [DisplayName("University Details")]
        [Required(ErrorMessage = "Details can not be blank")]
        public string University_Details { get; set; }

        [DisplayName("Establishment Year")]
        [Required(ErrorMessage = "Establish Year can not be blank")]
        public string Establishment_Year { get; set; }

        [DisplayName("Registration Number")]
        [Required(ErrorMessage = "Registration Number can not be blank")]
        public string Registration_Number { get; set; }
        public int Updated_By { get; set; }

    }

    public class UniversityProfile { 
        public int University_ID { get; set; }
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

    }

    public class UniversityDetailsWithStudents
    {
        public int University_ID { get; set; }

        [DisplayName("University Name")]
        [Required(ErrorMessage = "Name can not be blank")]
        public string University_Name { get; set; }

        [DisplayName("University Contact Number")]
        [Required(ErrorMessage = "Contact Number can not be blank")]
        public string Contact_Number { get; set; }

        [DisplayName("University E-Mail ID")]
        [Required(ErrorMessage = "E-Mail ID can not be blank")]
        public string Email_ID { get; set; }

        [DisplayName("University Address")]
        [Required(ErrorMessage = "Address can not be blank")]
        public string Address { get; set; }


        [DisplayName("University Pin Code")]
        [Required(ErrorMessage = "Pin Code can not be blank")]
        public string Pin_Code { get; set; }

        [DisplayName("University Website URL")]
        [Required(ErrorMessage = "Website URL can not be blank")]
        public string Website_URL { get; set; }

        [DisplayName("University Logo Upload")]
        //[Required(ErrorMessage = "University Logo can not be blank")]
        public string Univsersity_Logo { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        [DisplayName("University Details")]
        [Required(ErrorMessage = "Details can not be blank")]
        public string University_Details { get; set; }

        [DisplayName("Establishment Year")]
        [Required(ErrorMessage = "Establish Year can not be blank")]
        public string Establishment_Year { get; set; }

        [DisplayName("Registration Number")]
        [Required(ErrorMessage = "Registration Number can not be blank")]
        public string Registration_Number { get; set; }
        public List<StudentViewList> studentViewList { get; set; }
    }

    public class StudentViewList
    {
        [DisplayName("S.No.")]
        public int count { get; set; }

        [DisplayName("Student Name")]
        public string Student_Name { get; set; }

        [DisplayName("Father Name")]
        public string Father_Name { get; set; }
  
        [DisplayName("Course Name")]
        public string Univ_Course_Name { get; set; }

        [DisplayName("Course Branch Name")]
        public string Univ_subCourse_Name { get; set; }

    }


    public class UniversityFrontShow
    {
        public string University_ID { get; set; }
        public string University_Name { get; set; }
        public string Address { get; set; }
        public string Pin_Code { get; set; }
        public string Univsersity_Logo { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }
        public string Establishment_Year { get; set; }
    }

    public class University_office_bearer
    {
        public int count { get; set; }
        public string University_ID { get; set; }
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

    public class University_office_bearer_Insert
    {
        public string University_ID { get; set; }

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

        public int Updated_By { get; set; }
    }

    public class University_office_bearer_Delete
    {
        public string University_ID { get; set; }
        public int office_bearer_ID { get; set; }

        public int Deleted_By { get; set; }
    }


    public class UniversityDetailsWithOfficeBearer
    {
        public int University_ID { get; set; }

        [DisplayName("University Name")]
        public string University_Name { get; set; }

        [DisplayName("University Contact Number")]
        public string Contact_Number { get; set; }

        [DisplayName("University E-Mail ID")]
        public string Email_ID { get; set; }

        [DisplayName("University Address")]
        public string Address { get; set; }

        [DisplayName("University Pin Code")]
        public string Pin_Code { get; set; }

        [DisplayName("University Website URL")]
        public string Website_URL { get; set; }

        [DisplayName("University Logo Upload")]
        //[Required(ErrorMessage = "University Logo can not be blank")]
        public string Univsersity_Logo { get; set; }
        public HttpPostedFileBase UploadFile { get; set; }

        [DisplayName("University Details")]
        public string University_Details { get; set; }

        [DisplayName("Establishment Year")]
        public string Establishment_Year { get; set; }

        [DisplayName("Registration Number")]
        public string Registration_Number { get; set; }
        public List<UniversityOfficeBearerShowHome> UniversityOfficeBearerShowHome { get; set; }
    }

    public class UniversityOfficeBearerShowHome
    {
        public int count { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Picture")]
        public string Picture { get; set; }
        [Display(Name = "Other Details")]
        public string Other { get; set; }
    }



}