using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Private_University.Models
{
    public class university_course
    {
        public int count { get; set; }
        public int Univ_Course_ID { get; set; }
        public int Univ_subCourse_ID { get; set; }
        public int University_ID { get; set; }
        [Display(Name = "Course Name")]
        public string Univ_Course_Name { get; set; }
        [Display(Name = "Course Code")]
        public string Univ_Course_Code { get; set; }
        [Display(Name = "Mode of Course")]
        public string Course_Mode { get; set; }
        [Display(Name = "Duration of Course")]
        public string Number_of_Year_Sem { get; set; }
        [Display(Name = "Course/Sub-Course/Branch")]
        public List<university_sub_course> Sub_course { get; set; }
    }

    public class University_Course_details
    {
        [Display(Name = "S.No.")]
        public int count { get; set; }        
        public int Univ_Course_ID { get; set; }       
     
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }
        public string Univ_Course_Code { get; set; }

        [Display(Name = "Duration of Course")]
        public string CourseDuration { get; set; }

        public int NoOfBranches { get; set; }
        public int Univ_subCourse_ID { get; set; }

        [Display(Name = "Branch/specialization Name")]
        public string SubCourseName { get; set; }
        public string Univ_subCourse_Code { get; set; }
        [Display(Name = "No of students")]
        public Int64 NoOfStudent { get; set; }


        [Display(Name = "Course Mode")]
        public int CourseMode { get; set; }
        public int NOYS { get; set; }

   

    }
    public class university_course_insert
    {
        [Required(ErrorMessage = "Please login as University User.")]
        public int University_ID { get; set; }

        [Display(Name = "Course Name")]
        [Required(ErrorMessage = "Please Enter Course Name.")]
        [StringLength(100, ErrorMessage = "Course Name cannot exceed 100 characters")]
        public string Univ_Course_Name { get; set; }

        [Display(Name = "Course Code")]
        [Required(ErrorMessage = "Please Enter Course Code.")]
        [StringLength(15, ErrorMessage = "Course Code cannot exceed 15 characters")]
        public string Univ_Course_Code { get; set; }
        [Display(Name = "Mode of Course")]
        [Required(ErrorMessage = "Please select Mode of Course.")]

        public int Course_Mode_ID { get; set; }
        [Display(Name = "Duration of Course (No. Of Years/Semesters)")]
        [Required(ErrorMessage = "Please Enter No. Of Years/Semesters")]
        
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Only Numbers allowed. ex: 3,6,8.")]


        public decimal Number_of_Year_Sem { get; set; }
        public int Entry_By { get; set; }
    }

    public class university_course_delete
    {
        public int Univ_Course_ID { get; set; }
        public int Deleted_By { get; set; }
    }

    public class university_sub_course
    {
        public int count { get; set; }
        public int Univ_subCourse_ID { get; set; }
        public int Univ_Course_ID { get; set; }
        [Display(Name = "Course/Branch")]
        public string Univ_subCourse_Name { get; set; }
        [Display(Name = "Course Code")]
        public string Univ_subCourse_Code { get; set; }
    }

    public class university_sub_course_Insert
    {
        [Display(Name = "Course Name")]
        public int Univ_Course_ID { get; set; }

        [Required(ErrorMessage = "Please login as University User.")]
        public int University_ID { get; set; }

        [Display(Name = "Course / Sub-Course / Branch")]
        [Required(ErrorMessage = "Please Enter Course/Sub-Course/Branch Name.")]
        [StringLength(100, ErrorMessage = "Course/Sub-Course/Branch Name cannot exceed 100 characters")]
        public string Univ_subCourse_Name { get; set; }

        [Display(Name = "Course Code")]
        [Required(ErrorMessage = "Please Enter Course/Sub-Course/Branch Code.")]
        [StringLength(15, ErrorMessage = "Course/Sub-Course/Branch Code cannot exceed 15 characters")]
        public string Univ_subCourse_Code { get; set; }

        public int Entry_By { get; set; }
    }

    public class university_course_Single
    {
        public int Univ_Course_ID { get; set; }
        public string Univ_Course_Name { get; set; }
    }



}