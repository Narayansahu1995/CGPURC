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
	public class FeesCollect
	{
		public int University_ID { get; set; }
		public int Session_ID { get; set; }
		public decimal Amount_Credit { get; set; }
		public DateTime Txn_Date { get; set; }
		public int Entry_By { get; set; }
		public string Bill_Number { get; set; }
		public int Student_ID { get; set; }
		public string Narration { get; set; }
	}

    public class StudentFeesDetails
    {
        public int Student_Id { get; set; }

        [DisplayName("S.No.")]
        public int count { get; set; }

        [DisplayName("Student Name")]
        public string Student_Name { get; set; }

        [DisplayName("Father Name")]
        public string Father_Name { get; set; }

        [DisplayName("Enrollment Number")]
        public string Enrollment_Number { get; set; }

        [DisplayName("Pursuing Year/Semester")]
        public string PYS { get; set; }        

        [DisplayName("Bill Number")]
        public string Bill_Number { get; set; }

        [DisplayName("Billinig Date")]
        public string Bill_Date { get; set; }

        [DisplayName("Narration")]
        public string Narration { get; set; }

        [DisplayName("Fees Amount")]
        public decimal Amount { get; set; }

    }

	public class StudentDetails
	{
		public int Student_Id { get; set; }

		[DisplayName("S.No.")]
		public int count { get; set; }

		[DisplayName("Student Name")]
		public string Student_Name { get; set; }

		[DisplayName("Father Name")]
		public string Father_Name { get; set; }

		[DisplayName("Date of Birth")]
		public string Date_of_Birth { get; set; }

		[DisplayName("Enrollment Number")]
		public string Enrollment_Number { get; set; }

		[DisplayName("Student Mobile Number")]
		public string Mobile_Number_Stu { get; set; }

		[DisplayName("E-Mail ID")]
		public string Email_ID { get; set; }

		[DisplayName("Course Name")]
		public string Univ_Course_Name { get; set; }

		[DisplayName("Course Branch Name")]
		public string Univ_subCourse_Name { get; set; }

		[DisplayName("Year/Semester")]
		public string Course_Mode { get; set; }

		[DisplayName("Pursuing Year/Semester")]
		public string Semester_Year { get; set; }
	}


	public class ayoge_txn
	{
		public string Txn_Number { get; set; }
		public int University_ID { get; set; }
		public int txn_Month { get; set; }
		public int txn_Year { get; set; }
		public decimal Total_Amount { get; set; }
		public decimal Percent_Amount { get; set; }
		public DateTime Due_Date { get; set; }
	}

	public class ayoge_txn_dtls
	{
		public int Ayong_Txn_ID { get; set; }
		public int Student_ID { get; set; }
		public decimal Amount { get; set; }
	}


	public class ayoge_transctions_List
	{
		public int count { get; set; }
		public int Ayong_Txn_ID { get; set; }
		public int University_ID { get; set; }
		[Display(Name = "Month")]
		public int Txn_Month { get; set; }
		public string Txn_Month_name { get; set; }
		[Display(Name = "Year")]
		public int Txn_Year { get; set; }
		[Display(Name = "Total Amount Collected")]
		public decimal Total_Amount { get; set; }
		[Display(Name = "1% Percent of Total")]
		public decimal Percent_Amount { get; set; }
		public bool Is_Paid { get; set; }
		[Display(Name = "Due Date")]
		public DateTime Due_Date { get; set; }
		public string CustomerName { get; set; }
		public string CustomerEmail { get; set; }
		public string CustomerMobile { get; set; }
		public string BillingAddress { get; set; }
	}

	public class ayoge_transctions_Draft
	{
		public int Ayong_Txn_ID { get; set; }
		public string Txn_Number { get; set; }
		public string Draft_Bank { get; set; }
		public string Demand_Draft_No { get; set; }
		public DateTime Date_of_issue { get; set; }
	}


	public class Challan_University
	{

		public int Ayong_Txn_ID { get; set; }
		public string Txn_Number { get; set; }
		public int Txn_Month { get; set; }
		public string Txn_Month_Name { get; set; }
		public int Txn_Year { get; set; }
		public decimal Total_Amount { get; set; }
		public string Total_Amount_Word { get; set; }
		public decimal Percent_Amount { get; set; }
		public string Percent_Amount_word { get; set; }
		public bool Is_Paid { get; set; }
		public string Draft_Bank { get; set; }
		[DisplayFormat(DataFormatString = "{0:d}")] 
		public DateTime Date_of_issue { get; set; }
		public string Demand_Draft_No { get; set; }
		[DisplayFormat(DataFormatString = "{0:d}")] 
		public DateTime Paid_Date { get; set; }
		public DateTime Due_Date { get; set; }
		public int University_ID { get; set; }
		public string University_Name { get; set; }
		public string Contact_Number { get; set; }
		public string Email_ID { get; set; }
		public string Address { get; set; }
		public string Pin_Code { get; set; }
		public List<Challan_University_Detials> Challan_Detials { get; set; }

	}

	public class Challan_University_Detials
	{
		public int count{ get; set; }
		public int Ayong_Txn_ID { get; set; }
		public int University_ID { get; set; }
		public string University_Name { get; set; }
		public string University_Address { get; set; }
		public string Course_Name { get; set; }
		public int Number_of_Students { get; set; }
		public decimal Rate_Fees { get; set; }
		public decimal Total_Fees { get; set; }
		public decimal Percent_Total_Fees { get; set; }

	}

}