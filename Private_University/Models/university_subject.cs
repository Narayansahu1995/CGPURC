using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Private_University.Models
{
	public class Course
	{
		public int count { get; set; }
		public int Univ_Course_ID { get; set; }
		[Display(Name = "Course")]
		public string Univ_Course_Name { get; set; }
		[Display(Name = "Course Code")]
		public string Univ_Course_Code { get; set; }
		[Display(Name = "Mode of Course")]
		public string Course_Mode { get; set; }
		[Display(Name = "Duration of Course")]
		public string Number_of_Year_Sem { get; set; }
		public decimal Number_Year_Sem { get; set; }
		[Display(Name = "Course/Sub-Course/Branch")]
		public List<sub_course> Sub_course { get; set; }
	}

	public class sub_course
	{
		public int count { get; set; }
		public int Univ_subCourse_ID { get; set; }
		public int Univ_Course_ID { get; set; }
		[Display(Name = "Course/Branch")]
		public string Univ_subCourse_Name { get; set; }
		[Display(Name = "Course Code")]
		public string Univ_subCourse_Code { get; set; }
		public List<Sem_Years> semesters { get; set; }
	}

	public class Sem_Years
	{
		public int count { get; set; }
		[Display(Name = "Semester/Year")]
		public string Sementer_Year { get; set; }
		[Display(Name = "Subjects")]
		public List<Subject> Subjects { get; set; }
	}

	public class Subject
	{
		public int Count { get; set; }
		public int Univ_Subject_ID { get; set; }
		[Display(Name = "Subject")]
		public string Subject_Name { get; set; }
		[Display(Name = "Subject Code")]
		public string Subject_Code { get; set; }
	}

	public class university_subject_insert
	{

		[Display(Name = "University")]
		public int University_ID { get; set; }
		[Display(Name = "Course")]
		[Required(ErrorMessage = "Please choose appropriate Course.")]
		public int Univ_Course_ID { get; set; }
		[Display(Name = "Course/Sub-Course/Branch Name")]
		[Required(ErrorMessage = "Please choose appropriate Course/Sub-Course/Branch Name.")]
		public int Univ_subCourse_ID { get; set; }
		[Display(Name = "Semester / Year")]
		[Required(ErrorMessage = "Please choose appropriate Semester / Year.")]
		public int Semester_Year { get; set; }
		[Display(Name = "Subject")]
		[Required(ErrorMessage = "Please Enter Subject Name.")]
		[StringLength(100, ErrorMessage = "Subject Name cannot exceed 100 characters")]
		public string Subject_Name { get; set; }
		[Display(Name = "Subject Code")]
		[Required(ErrorMessage = "Please Enter Subject Code.")]
		[StringLength(15, ErrorMessage = "Subject Code cannot exceed 15 characters")]
		public string Subject_Code { get; set; }
		public string Entry_By { get; set; }
	}


	public class university_subject_delete
	{
		public int Univ_Subject_ID { get; set; }
		public int Deleted_By { get; set; }
	}


}