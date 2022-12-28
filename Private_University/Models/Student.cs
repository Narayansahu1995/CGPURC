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
	public class Student
	{
		[DisplayName("Student Name")]
		[Required(ErrorMessage = "Student Name can not be blank")]
		public string Student_Name { get; set; }

		[DisplayName("Father Name")]
		[Required(ErrorMessage = "Father Name can not be blank")]
		public string Father_Name { get; set; }

        [DisplayName("Gender")]
        [Required(ErrorMessage = "Gender can not be blank")]
        public int Gender { get; set; }

		[DisplayName("Date of Birth")]
		[Required(ErrorMessage = "Date of Birth can not be blank")]
		[DataType(DataType.Date)]
		public string Date_of_Birth { get; set; }

		[DisplayName("Aadhar Number (Optional)")]
		public string Aadhar_Number { get; set; }

		[DisplayName("Enrollment Number")]
		[Required(ErrorMessage = "Enrollment Number can not be blank")]
		public string Enrollment_Number { get; set; }

		[DisplayName("Date of Addmission")]
		[Required(ErrorMessage = "Date of Addmission can not be blank")]
		[DataType(DataType.Date)]
		public string Date_of_Admission { get; set; }

		[DisplayName("Student Mobile Number")]
		[Required(ErrorMessage = "Student Mobile Number can not be blank")]
		public string Mobile_Number_Stu { get; set; }

		[DisplayName("Mobile Number Of Father/Mother")]
		//[Required(ErrorMessage = "Father's Mobile Number can not be blank")]
		public string Mobile_Number_Father { get; set; }

		[DisplayName("E-Mail ID")]
		//[Required(ErrorMessage = "E-Mail ID can not be blank")]
		//[EmailAddress(ErrorMessage = "Invalid Email Address.")]
		public string Email_ID { get; set; }

		[DisplayName("Permanent Address")]
		[Required(ErrorMessage = "Permanent Address can not be blank")]
		public string Address { get; set; }

		[DisplayName("University")]
		//[Required(ErrorMessage = "University Name can not be blank")]
		public int University_ID { get; set; }

		//public Nullable<int> Univ_Course_ID { get; set; }
		//public int? Course_ID { get; set; }

		public List<SelectListItem> session { get; set; }
		public List<SelectListItem> PopulateCourse { get; set; }

		[DisplayName("Course Name")]
		[Required(ErrorMessage = "Select Course Name")]
		public Nullable<int> Univ_Course_ID { get; set; }

		[DisplayName("Course Branch Name")]
		[Required(ErrorMessage = "Select Course Branch Name")]
		public Nullable <int> Univ_subCourse_ID { get; set; }

		[DisplayName("Session")]
		[Required(ErrorMessage = "Session can not be blank")]
		public Nullable<int> Session_ID { get; set; }

		[DisplayName("Year/Semester")]
		[Required(ErrorMessage = "Select Course Mode")]
		public Nullable<int> Course_Mode_ID { get; set; }

		[DisplayName("Entry By")]
		//[Required(ErrorMessage = "Entry By can not be blank")]
		public int Entry_By { get; set; }

		[DisplayName("Persuing Year/Semester")]
        [Required(ErrorMessage = "Please select persuing Year/Semester")]
        public Nullable<int> Semester_Year { get; set; }
	}

    public class StudentEdit
    {
        public int Student_Id { get; set; }

        [DisplayName("Student Name")]
        [Required(ErrorMessage = "Student Name can not be blank")]
        public string Student_Name { get; set; }

        [DisplayName("Father Name")]
        [Required(ErrorMessage = "Father Name can not be blank")]
        public string Father_Name { get; set; }

        [DisplayName("Gender")]
        [Required(ErrorMessage = "Gender can not be blank")]
        public Nullable<int> Gender { get; set; }

        [DisplayName("Date of Birth")]
        [Required(ErrorMessage = "Date of Birth can not be blank")]
        [DataType(DataType.Date)]
        public string Date_of_Birth { get; set; }

        [DisplayName("Aadhar Number (Optional)")]
        public string Aadhar_Number { get; set; }

        [DisplayName("Enrollment Number")]
        [Required(ErrorMessage = "Enter Enrollment Number")]
        public string Enrollment_Number { get; set; }

        [DisplayName("Date of Addmission")]
        [Required(ErrorMessage = "Date of Addmission can not be blank")]
        [DataType(DataType.Date)]
        public string Date_of_Admission { get; set; }

        [DisplayName("Student Mobile Number")]
        [Required(ErrorMessage = "Student Mobile Number can not be blank")]
        public string Mobile_Number_Stu { get; set; }

        [DisplayName("Mobile Number of Father/Mother")]
        //[Required(ErrorMessage = "Father's Mobile Number can not be blank")]
        public string Mobile_Number_Father { get; set; }

        [DisplayName("E-Mail ID")]
        //[Required(ErrorMessage = "E-Mail ID can not be blank")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email_ID { get; set; }

        [DisplayName("Permanent Address")]
        [Required(ErrorMessage = "Permanent Address can not be blank")]
        public string Address { get; set; }

        [DisplayName("University")]
        //[Required(ErrorMessage = "University Name can not be blank")]
        public int University_ID { get; set; }

        //public Nullable<int> Univ_Course_ID { get; set; }
        //public int? Course_ID { get; set; }

        public List<SelectListItem> session { get; set; }
        public List<SelectListItem> PopulateCourse { get; set; }

        [DisplayName("Course Name")]
        [Required(ErrorMessage = "Select Course Name")]
        public Nullable<int> Univ_Course_ID { get; set; }

        [DisplayName("Course Branch Name")]
        [Required(ErrorMessage = "Select Course Branch Name")]
        public Nullable<int> Univ_subCourse_ID { get; set; }

        [DisplayName("Session")]
        [Required(ErrorMessage = "Session can not be blank")]
        public Nullable<int> Session_ID { get; set; }

        [DisplayName("Year/Semester")]
        [Required(ErrorMessage = "Select Course Mode")]
        public Nullable<int> Course_Mode_ID { get; set; }

        [DisplayName("Entry By")]
        //[Required(ErrorMessage = "Entry By can not be blank")]
        public int Entry_By { get; set; }

        [DisplayName("Persuing Year/Semester")]
        [Required(ErrorMessage = "Please select persuing Year/Semester")]
        public Nullable<int> Semester_Year { get; set; }
    }

    public class StudentDelete
    {
        public int Student_Id { get; set; }

        [DisplayName("Enrollment Number")]
        public string Enrollment_Number { get; set; }

        [DisplayName("Student Name")]      
        public string Student_Name { get; set; }

        [DisplayName("Father Name")]    
        public string Father_Name { get; set; }

        [DisplayName("Gender")]      
        public string Gender { get; set; }

        [DisplayName("Date of Birth")]      
        public string Date_of_Birth { get; set; }

        [DisplayName("Aadhar Number (Optional)")]
        public string Aadhar_Number { get; set; }        

        [DisplayName("Date of Addmission")]      
        public string Date_of_Admission { get; set; }

        [DisplayName("Student Mobile Number")] 
        public string Mobile_Number_Stu { get; set; }

        [DisplayName("Mobile Number of Father/Mother")]      
        public string Mobile_Number_Father { get; set; }

        [DisplayName("E-Mail ID")]     
        public string Email_ID { get; set; }

        [DisplayName("Permanent Address")]     
        public string Address { get; set; }        

        [DisplayName("Course Name")]      
        public string Course_Name { get; set; }

        [DisplayName("Course Branch Name")]      
        public string Branch_Name { get; set; }      

        [DisplayName("Course Mode")]      
        public string Course_Mode { get; set; }     

        [DisplayName("Persuing Year/Semester")]       
        public Nullable<int> Semester_Year { get; set; }
    }
    public class StudentList
	{
		[DisplayName("S.No.")]
		public int count { get; set; }

		[DisplayName("Student Name")]
		public string Student_Name { get; set; }

		[DisplayName("Father Name")]
		public string Father_Name { get; set; }

        [DisplayName("Gender")]
        public string Gender { get; set; }

		[DisplayName("Date of Birth")]
		public string Date_of_Birth { get; set; }

		[DisplayName("Aadhar Number")]
		public string Aadhar_Number { get; set; }
		[DisplayName("Enrollment Number")]
		public string Enrollment_Number { get; set; }

		[DisplayName("Date of Admission")]
		[DataType(DataType.Date)]
		public string Date_of_Admission { get; set; }

		[DisplayName("Student Mobile Number")]
		public string Mobile_Number_Stu { get; set; }

		[DisplayName("Father's Mobile Number")]
		public string Mobile_Number_Father { get; set; }

		[DisplayName("E-Mail ID")]
		public string Email_ID { get; set; }

		[DisplayName("Permanent Address")]
		public string Address { get; set; }

		[DisplayName("University Name")]
		public string university_Name { get; set; }

		[DisplayName("University Address")]
		public string university_Address { get; set; }

		[DisplayName("University Contact")]
		public string university_Contact { get; set; }
		[DisplayName("University E-Mail")]
		public string university_Email { get; set; }

		[DisplayName("PIN Code")]
		public string university_PIN { get; set; }

		[DisplayName("Course Name")]
		public string Univ_Course_Name { get; set; }

		[DisplayName("Course Branch Name")]
		public string Univ_subCourse_Name { get; set; }

		[DisplayName("Session")]
		public string Session_Name { get; set; }

		[DisplayName("Year/Semester")]
		public string Course_Mode { get; set; }

		[DisplayName("Persuing Year/Semester")]
		public string Semester_Year { get; set; }

        public int Student_ID { get; set; }
	}

	public class student_promote
	{
		public int Student_Id { get; set; }
		public Nullable<int> Session_ID { get; set; }
		public Nullable<int> Semester_Year { get; set; }
		public int Is_Passout { get; set; }
		public int Result { get; set; }
		public int Promoted_By { get; set; }
	}

	public class student_Details_promote
	{
		public int Student_Id { get; set; }
		public int Session_ID { get; set; }
		public int Course_Mode_ID { get; set; }
		public int Semester_Year { get; set; }
		public int Max_Semester_Year { get; set; }
	}

    public class Upload_Student_data
    {
       public int University_ID { get; set; }
       public string File_Name { get; set; }
       public string File_path { get; set; }
       public string Uploaded_DateTime { get; set; }
        
    }
    public class Uploaded_Student_data_files
    {
        public int count { get; set; }
        public int File_ID { get; set; }
        public int University_ID { get; set; }
        public string File_Name { get; set; }
        public string File_path { get; set; }
        public string Uploaded_DateTime { get; set; }
        public int Records_exported_to_Server { get; set; }
        public int Records_left { get; set; }
        public string Processed { get; set; }
        public string Remark { get; set; }


    }

}