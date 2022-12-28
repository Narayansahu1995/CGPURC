using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Private_University.Models
{
	public class University_Fees_Insert
	{
		[Display(Name = "University")] 
		public int University_ID { get; set; }
		[Display(Name = "Session")]
		public int Session_ID { get; set; }
		[Display(Name = "Course")]
		public int Univ_Course_ID { get; set; }
		[Display(Name = "Course/Sub-Course/Branch")]
		public int Univ_subCourse_ID { get; set; }
		[Display(Name = "Semester / Year")]
		public int Semester_Year { get; set; }
		[Display(Name = "Fees {Amount}")]
		public decimal Amount { get; set; }
		public int Entry_By { get; set; }

	}

	public class University_Fees_Insert_Test
	{
		public int University_ID { get; set; }
		public int Session_ID { get; set; }
		public int Univ_Course_ID { get; set; }
		public int Univ_subCourse_ID { get; set; }
		public List<int> Semester_Year { get; set; }
		public List<decimal> Amount { get; set; }
		public int Entry_By { get; set; }

	}

	public class Course_Fees
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
		public List<sub_course_fee> Sub_course { get; set; }
	}

	public class sub_course_fee
	{
		public int count { get; set; }
		public int Univ_subCourse_ID { get; set; }
		public int Univ_Course_ID { get; set; }
		[Display(Name = "Course/Branch")]
		public string Univ_subCourse_Name { get; set; }
		[Display(Name = "Course Code")]
		public string Univ_subCourse_Code { get; set; }
		public List<Sem_Years_fee> semesters { get; set; }
	}

	public class Sem_Years_fee
	{
		public int count { get; set; }
		[Display(Name = "Semester/Year")]
		public string Sementer_Year { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
		[RegularExpression("([1-9]+)", ErrorMessage = "Please enter valid Amount.")]
		[Display(Name = "Fees {Amount}")]
		public decimal Fees { get; set; }
	}

	public class UniversityDetailsWithFeesDetails
	{
		[Display(Name = "University Name")]
		public string University_Name { get; set; }

		public List<Course_Fees> Course_Fees_Data { get; set; }
	}
}