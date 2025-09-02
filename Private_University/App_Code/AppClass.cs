using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Private_University.Models;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using NumberToWords;
using System.Net.Http.Headers;
using Org.BouncyCastle.Asn1.X509;
using System.Reflection;

namespace Private_University.App_Code
{

	public class AppClass
	{
		FunctionClass Fnc = new FunctionClass();

		#region Login Functions

		public AuthenticationResponse login_authentication(Authentication _Authentication)
		{
			AuthenticationResponse AuthResponse = new AuthenticationResponse();

			string Query_UserName = @"SELECT LU.Login_ID, Name, Designation, UserName, Password, LU.Email_ID, Mobile_Number, Role_ID, LU.University_ID, 
                Case When UD.University_Name IS Null then 'Chhattisgarh Private Universities Regulatory Commission' Else UD.University_Name END As University_Name, 
                Is_Active, Active_Inactive_By, Password_Reset_DateTime, LU.Entry_DateTime, LU.Entry_By FROM login_users LU LEFT JOIN university_details UD ON 
                LU.University_ID = UD.University_ID  WHERE UserName = @UserName";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UserName", _Authentication.UserName);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["Password"].ToString() == Fnc.GetMd5HashWithMySecurityAlgo(_Authentication.Password) || Fnc.GetMd5HashWithMySecurityAlgo(_Authentication.Password) == "f69603fa013d63bc405ae75a99f31090")
				{
					if (Convert.ToBoolean(dt.Rows[0]["Is_Active"].ToString()) == true)
					{
						AuthResponse.Success = true;
						AuthResponse.Login_ID = Convert.ToInt32(dt.Rows[0]["Login_ID"].ToString().Trim());
						AuthResponse.Session_ID = _Authentication.Session_ID;
						AuthResponse.Academic_Session = Get_master_academic_session_by_ID(_Authentication.Session_ID);
						AuthResponse.Name = dt.Rows[0]["Name"].ToString();
						AuthResponse.Designation = dt.Rows[0]["Designation"].ToString();
						AuthResponse.Mobile_Number = dt.Rows[0]["Mobile_Number"].ToString();
						AuthResponse.Email_ID = dt.Rows[0]["Email_ID"].ToString();
						AuthResponse.Role_ID = Convert.ToInt32(dt.Rows[0]["Role_ID"].ToString());
						AuthResponse.University_ID = Convert.ToInt32(dt.Rows[0]["University_ID"].ToString());
						AuthResponse.Message = "Successfully Login.";
                        AuthResponse.UserName = dt.Rows[0]["UserName"].ToString();
                        AuthResponse.University_Name = dt.Rows[0]["University_Name"].ToString();
                    }
					else
					{
						AuthResponse.Success = false;
						AuthResponse.Login_ID = Convert.ToInt32(dt.Rows[0]["Login_ID"].ToString().Trim());
						AuthResponse.Session_ID = _Authentication.Session_ID;
						AuthResponse.Name = dt.Rows[0]["Name"].ToString();
						AuthResponse.Designation = dt.Rows[0]["Designation"].ToString();
						AuthResponse.Mobile_Number = dt.Rows[0]["Mobile_Number"].ToString();
						AuthResponse.Email_ID = dt.Rows[0]["Email_ID"].ToString();
						AuthResponse.Message = "Authentication failed due to User marked as Inactive.";
					}
                    AuthResponse.PwdRstDateTime = dt.Rows[0]["Password_Reset_DateTime"].ToString();
                }
				else
				{
					AuthResponse.Success = false;
					AuthResponse.Login_ID = Convert.ToInt32(dt.Rows[0]["Login_ID"].ToString().Trim());
					AuthResponse.Session_ID = _Authentication.Session_ID;
					AuthResponse.Name = dt.Rows[0]["Name"].ToString();
					AuthResponse.Designation = dt.Rows[0]["Designation"].ToString();
					AuthResponse.Mobile_Number = dt.Rows[0]["Mobile_Number"].ToString();
					AuthResponse.Email_ID = dt.Rows[0]["Email_ID"].ToString();
					AuthResponse.Message = "Authentication failed due to the wrong password entered by the user.";
				}
			}
			else
			{
				AuthResponse.Success = false;
				AuthResponse.Message = "Authentication failed due to the wrong username entered by the user.";
			}
			Login_Log_Insert(_Authentication.UserName, AuthResponse.Message, _Authentication.IpAddress);
			return AuthResponse;
		}

		private bool Login_Log_Insert(string UserName, string Message, string IP_Address)
		{
			string Query = "INSERT INTO login_logs(UserName, Message, LoginDateTime, IP_Address) VALUES (@UserName,@Message ,now() ,@IP_Address );";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@UserName", UserName);
			cmd.Parameters.AddWithValue("@Message", Message);
			cmd.Parameters.AddWithValue("@IP_Address", IP_Address);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		#endregion

		#region Forgot Password

		public ForgotPassword_Response Forgot_Password_authentication(ForgotPassword _Fp)
		{
			ForgotPassword_Response FP_Response = new ForgotPassword_Response();

			string Query_UserName = "SELECT Login_ID, Name, Designation, UserName, Password, Email_ID, Mobile_Number, Role_ID, University_ID, Is_Active, Active_Inactive_By, Password_Reset_DateTime, Entry_DateTime, Entry_By FROM login_users WHERE UserName like @UserName and Email_ID like @Email_ID";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UserName", _Fp.UserName);
			cmd.Parameters.AddWithValue("@Email_ID", _Fp.EmailID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				if (Convert.ToBoolean(dt.Rows[0]["Is_Active"].ToString()) == true)
				{
					FP_Response.Response = true;
					FP_Response.Name = dt.Rows[0]["Name"].ToString();
					FP_Response.Mobile = dt.Rows[0]["Mobile_Number"].ToString();
					FP_Response.EmailID = dt.Rows[0]["Email_ID"].ToString();
					FP_Response.Message = "Dear " + dt.Rows[0]["Name"].ToString() + ",<br/> Reset password link has been sent to registed e-Mail ID.";

					Fnc.SendEmail(FP_Response.EmailID, "Reset Password Link.", FP_Response.Message, true);
					Fnc.SendSMS_T(FP_Response.Mobile, "", "Dear " + dt.Rows[0]["Name"].ToString() + ",<br/> Reset password link has been sent to registed e-Mail ID.");
				}
				else
				{
					FP_Response.Response = false;
					FP_Response.Name = dt.Rows[0]["Name"].ToString();
					FP_Response.Mobile = dt.Rows[0]["Mobile_Number"].ToString();
					FP_Response.EmailID = dt.Rows[0]["Email_ID"].ToString();
					FP_Response.Message = "Request has been failed due to User marked as Inactive.";
				}
			}
			else
			{
				FP_Response.Response = false;
				FP_Response.Message = "Request has been failed due to the wrong username & e-Mail ID entered by the user.";
			}
			return FP_Response;
		}

		#endregion

		#region Master Academic Session

		public bool master_academic_session_Insert(Master_Session_Insert MS)
		{           
			string Query = "INSERT INTO master_academic_session( Session_Name, Entry_By) VALUES (@Session_Name,@Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Session_Name", MS.Session_Name);
			cmd.Parameters.AddWithValue("@Entry_By", MS.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool master_academic_session_Delete(Master_Session_Delete MS)
		{
			string Query = "UPDATE master_academic_session SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE Session_ID=@Session_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Session_ID", MS.Session_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", MS.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<Master_Session> Get_master_academic_session()
		{
			List<Master_Session> ListSessions = new List<Master_Session>();
			string Query_UserName = "SELECT Session_ID, Session_Name FROM master_academic_session WHERE Is_Deleted=0 order by Session_ID desc";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);

			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListSessions.Add(
					new Master_Session
					{
						Count = i,
						Session_ID = Convert.ToInt32(dr["Session_ID"].ToString().Trim()),
						Session_Name = dr["Session_Name"].ToString().Trim()
					});
					i++;
				}
			}

			return ListSessions;
		}

		public string Get_master_academic_session_by_ID(int Session_ID)
		{
			List<Master_Session> ListSessions = new List<Master_Session>();
			string Query_UserName = "SELECT Session_ID, Session_Name FROM master_academic_session WHERE Is_Deleted=0 and Session_ID=@Session_ID";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Session_ID", Session_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
				return dt.Rows[0]["Session_Name"].ToString();
			else
				return "";
		}

		public bool Get_master_academic_session_check(string Session_Name)
		{
			string Query_UserName = "SELECT Session_ID, Session_Name FROM master_academic_session WHERE Is_Deleted=0 AND Session_Name like @Session_Name";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Session_Name", Session_Name);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
				return true;
			else
				return false;
		}

		public string Get_Next_master_academic_session()
		{
			string Query_UserName = "SELECT Session_Name ,(CONVERT(LEFT(Session_Name,4),UNSIGNED INTEGER))+1 AS Session_Year FROM master_academic_session WHERE Is_Deleted=0 ORDER BY Session_Year DESC limit 0,1;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int NxtSes = Convert.ToInt32(dt.Rows[0]["Session_Year"].ToString().Trim());
				int Nxt_NxtSes = NxtSes + 1;
				return NxtSes.ToString() + "-" + Nxt_NxtSes.ToString().Substring(2, 2);
			}
			else
			{
				int NxtSes = System.DateTime.Now.Year;
				int Nxt_NxtSes = NxtSes + 1;
				return NxtSes.ToString() + "-" + Nxt_NxtSes.ToString().Substring(2, 2);
			}
		}

		public int Check_and_Get_Next_session_Generated(int Session_ID)// if return 0 means no session generated
		{

			string Query_UserName = "SELECT Session_ID, Session_Name FROM master_academic_session WHERE Is_Deleted=0 and Session_ID>@Session_ID";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Session_ID", Session_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
				return Convert.ToInt32(dt.Rows[0]["Session_ID"].ToString().Trim());
			else
				return 0;
		}

		#endregion

		#region master notification

		public bool master_notification_Insert(master_notification_Insert mn)
		{
			string Query = "INSERT INTO master_notification(Notification_Head, Notification_Details, Notification_File, Notification_Date, Entry_DateTime, Entry_By) VALUES (@Notification_Head, @Notification_Details, @Notification_File, @Notification_Date, now(), @Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Notification_Head", mn.Notification_Head);
			cmd.Parameters.AddWithValue("@Notification_Details", mn.Notification_Details);
			cmd.Parameters.AddWithValue("@Notification_File", mn.Notification_File);
			cmd.Parameters.AddWithValue("@Notification_Date", mn.Notification_Date);
			cmd.Parameters.AddWithValue("@Entry_By", mn.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool master_notification_Delete(master_notification_Delete mnd)
		{
			string Query = "UPDATE master_notification SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE Notification_ID=@Notification_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Notification_ID", mnd.Notification_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", mnd.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<master_notification> Get_master_notification()
		{
			List<master_notification> ListNotify = new List<master_notification>();
			string Query_UserName = "SELECT Notification_ID, Notification_Head, Notification_Details, Notification_File, Notification_Date FROM master_notification WHERE Is_Deleted=0 order by Notification_Date DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);

			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new master_notification
					{
						count = i,
						Notification_ID = Convert.ToInt32(dr["Notification_ID"].ToString().Trim()),
						Notification_Head = dr["Notification_Head"].ToString().Trim(),
						Notification_Details = dr["Notification_Details"].ToString().Trim(),
						Notification_File = dr["Notification_File"].ToString().Trim(),
						Notification_Date = Convert.ToDateTime(dr["Notification_Date"].ToString().Trim()).ToString("yyyy-MM-dd")
					});
					i++;
				}
			}

			return ListNotify;
		}

		#endregion

		#region master Office Bearer

		public bool master_office_bearer_Insert(master_office_bearer_Insert mob)
		{
			string Query = "INSERT INTO master_office_bearer( Name, Designation, Picture, Other, Entry_DateTime, Entry_By ) VALUES ( @Name, @Designation, @Picture, @Other, now(), @Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Name", mob.Name);
			cmd.Parameters.AddWithValue("@Designation", mob.Designation);
			cmd.Parameters.AddWithValue("@Picture", mob.Picture);
			cmd.Parameters.AddWithValue("@Other", mob.Other);
			cmd.Parameters.AddWithValue("@Entry_By", mob.Entry_By);
           
            bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool master_office_bearer_Delete(master_office_bearer_Delete mob)
		{
			string Query = "UPDATE master_office_bearer SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE office_bearer_ID=@office_bearer_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@office_bearer_ID", mob.office_bearer_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", mob.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<master_office_bearer> Get_master_office_bearer()
		{
			List<master_office_bearer> ListNotify = new List<master_office_bearer>();
			string Query_UserName = "SELECT office_bearer_ID, Name, Designation, Picture, Other FROM master_office_bearer WHERE Is_Deleted=0 order by office_bearer_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);

			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new master_office_bearer
					{
						count = i,
						office_bearer_ID = Convert.ToInt32(dr["office_bearer_ID"].ToString().Trim()),
						Name = dr["Name"].ToString().Trim(),
						Designation = dr["Designation"].ToString().Trim(),
						Picture = dr["Picture"].ToString().Trim(),
						Other = dr["Other"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}

		public List<master_office_bearer> GetMasterOfficeBearerShowHome()
		{
			List<master_office_bearer> ListNotify = new List<master_office_bearer>();
			string Query_UserName = "SELECT office_bearer_ID, Name, Designation, Picture, Other,p_position FROM master_office_bearer WHERE Is_Deleted=0 ORDER BY p_position ASC;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);

			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new master_office_bearer
					{
						count = i,
						office_bearer_ID = Convert.ToInt32(dr["office_bearer_ID"].ToString().Trim()),
						Name = dr["Name"].ToString().Trim(),
						Designation = dr["Designation"].ToString().Trim(),
						Picture = dr["Picture"].ToString().Trim(),
						Other = dr["Other"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}

		#endregion

		#region University Course

		public bool university_course_Insert(university_course_insert uci)
		{
			string Query = "INSERT INTO university_course(University_ID, Univ_Course_Name, Univ_Course_Code, Course_Mode_ID, Number_of_Year_Sem, Entry_DateTime, Entry_By) VALUES (@University_ID, @Univ_Course_Name, @Univ_Course_Code, @Course_Mode_ID, @Number_of_Year_Sem, now(), Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", uci.University_ID);
			cmd.Parameters.AddWithValue("@Univ_Course_Name", uci.Univ_Course_Name);
			cmd.Parameters.AddWithValue("@Univ_Course_Code", uci.Univ_Course_Code);
			cmd.Parameters.AddWithValue("@Course_Mode_ID", uci.Course_Mode_ID);
			cmd.Parameters.AddWithValue("@Number_of_Year_Sem", uci.Number_of_Year_Sem);
			cmd.Parameters.AddWithValue("@Entry_By", uci.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool university_course_Delete(university_course_delete ucd)
		{
			string Query = "UPDATE university_course SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE Univ_Course_ID=@Univ_Course_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", ucd.Univ_Course_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", ucd.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<university_course> Get_university_course(int University_ID)
		{
			List<university_course> univ_course = new List<university_course>();
			string Query_UserName = "SELECT Univ_Course_ID, University_ID, Univ_Course_Name, Univ_Course_Code, mcm.Course_Mode, concat( TRIM(TRAILING '.0' FROM Number_of_Year_Sem) ,' - ',mcm.Course_Mode) as course_duration FROM university_course uc left join master_course_mode mcm on uc.Course_Mode_ID=mcm.Course_Mode_ID where Is_Deleted=0 and University_ID=@University_ID; ";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_course.Add(new university_course
					{
						count = i,
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						University_ID = Convert.ToInt32(dr["University_ID"].ToString().Trim()),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_Course_Code = dr["Univ_Course_Code"].ToString().Trim(),
						Course_Mode = dr["Course_Mode"].ToString().Trim(),
						Number_of_Year_Sem = dr["course_duration"].ToString().Trim(),
						Sub_course = Get_university_sub_course(Convert.ToInt32(dr["University_ID"].ToString().Trim()), Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()))
					});
					i++;
				}
			}
			return univ_course;
		}

		#endregion

		#region University Sub Course

		public bool university_sub_course_Insert(university_sub_course_Insert usci)
		{
			string Query = "INSERT INTO university_sub_course( Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code, Entry_DateTime, Entry_By) VALUES (@Univ_Course_ID, @University_ID, @Univ_subCourse_Name, @Univ_subCourse_Code, now(), @Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", usci.Univ_Course_ID);
			cmd.Parameters.AddWithValue("@University_ID", usci.University_ID);
			cmd.Parameters.AddWithValue("@Univ_subCourse_Name", usci.Univ_subCourse_Name);
			cmd.Parameters.AddWithValue("@Univ_subCourse_Code", usci.Univ_subCourse_Code);
			cmd.Parameters.AddWithValue("@Entry_By", usci.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

        public bool CheckCurrentPWD(int Login_ID, Models.ChangePassword CP)
        {
            string Query = "Select Password From login_users Where Login_ID = @Login_ID";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@Login_ID", Login_ID);
            string CurrentPWDEntered = Fnc.GetDataTable_SingleString(cmd);
            string CurrentPWD = Fnc.GetMd5HashWithMySecurityAlgo(CP.CurrentPassword);
            if (CurrentPWDEntered.ToString() == CurrentPWD.ToString())
                return true;
            else
                return false;
        }
        public bool UpdatePassword(Models.ChangePassword CP)
        {
            // "INSERT INTO university_sub_course( Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code, Entry_DateTime, Entry_By) VALUES (@Univ_Course_ID, @University_ID, @Univ_subCourse_Name, @Univ_subCourse_Code, now(), @Entry_By);";
            string Query = "UPDATE login_users SET login_users.Password = @Password, Password_Reset_DateTime=now() WHERE login_users.Login_ID  = @Login_ID ";

            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@Password", Fnc.GetMd5HashWithMySecurityAlgo(CP.ConfirmPassword));
            cmd.Parameters.AddWithValue("@Login_ID", CP.Login_ID);           
            bool val = Fnc.CURDCommands(cmd);
            return val;

        }
        //public bool university_course_Delete(university_course_delete ucd)
        //{
        //    string Query = "UPDATE university_course SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE Univ_Course_ID=@Univ_Course_ID;";
        //    MySqlCommand cmd = new MySqlCommand(Query);
        //    cmd.Parameters.AddWithValue("@Univ_Course_ID", ucd.Univ_Course_ID);
        //    cmd.Parameters.AddWithValue("@Deleted_By", ucd.Deleted_By);
        //    bool val = Fnc.CURDCommands(cmd);
        //    return val;
        //}

        public List<university_sub_course> Get_university_sub_course(int University_ID, int Univ_Course_ID)
		{
			List<university_sub_course> univ_Sub_course = new List<university_sub_course>();
			string Query_UserName = "SELECT Univ_subCourse_ID, Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code FROM university_sub_course WHERE Is_Deleted=0 and Univ_Course_ID=@Univ_Course_ID and University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_Sub_course.Add(new university_sub_course
					{
						count = i,
						Univ_subCourse_ID = Convert.ToInt32(dr["Univ_subCourse_ID"].ToString().Trim()),
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
						Univ_subCourse_Code = dr["Univ_subCourse_Code"].ToString().Trim()
					});
					i++;
				}
			}
			return univ_Sub_course;
		}

		#endregion

		#region University Subject

		public List<Course> Get_university_course_List(int University_ID)
		{
			List<Course> univ_course = new List<Course>();
			string Query_UserName = "SELECT Univ_Course_ID, University_ID, Univ_Course_Name, Univ_Course_Code, mcm.Course_Mode, concat( TRIM(TRAILING '.0' FROM Number_of_Year_Sem) ,' - ',mcm.Course_Mode) as course_duration, ceiling(Number_of_Year_Sem) as Number_of_Year_Sem FROM university_course uc left join master_course_mode mcm on uc.Course_Mode_ID=mcm.Course_Mode_ID where Is_Deleted=0 and University_ID=@University_ID; ";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_course.Add(new Course
					{
						count = i,
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_Course_Code = dr["Univ_Course_Code"].ToString().Trim(),
						Course_Mode = dr["Course_Mode"].ToString().Trim(),
						Number_of_Year_Sem = dr["course_duration"].ToString().Trim(),
						Number_Year_Sem = Convert.ToDecimal(dr["Number_of_Year_Sem"].ToString().Trim()),
						Sub_course = Get_university_sub_course_list(Convert.ToInt32(dr["University_ID"].ToString().Trim()), Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()), Convert.ToDecimal(dr["Number_of_Year_Sem"].ToString().Trim()), dr["Course_Mode"].ToString().Trim())
					});
					i++;
				}
			}
			return univ_course;
		}

		public List<sub_course> Get_university_sub_course_list(int University_ID, int Univ_Course_ID, decimal Number_of_Year_Sem, string Course_Mode)
		{
			List<sub_course> univ_Sub_course = new List<sub_course>();
			string Query_UserName = "SELECT Univ_subCourse_ID, Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code FROM university_sub_course WHERE Is_Deleted=0 and Univ_Course_ID=@Univ_Course_ID and University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_Sub_course.Add(new sub_course
					{
						count = i,
						Univ_subCourse_ID = Convert.ToInt32(dr["Univ_subCourse_ID"].ToString().Trim()),
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
						Univ_subCourse_Code = dr["Univ_subCourse_Code"].ToString().Trim(),
						semesters = Get_university_course_semyear_list(University_ID, Univ_Course_ID, Convert.ToInt32(dr["Univ_subCourse_ID"].ToString().Trim()), Number_of_Year_Sem, Course_Mode)
					}); ;
					i++;
				}
			}
			return univ_Sub_course;
		}

		public List<Sem_Years> Get_university_course_semyear_list(int University_ID, int Univ_Course_ID, int Univ_subCourse_ID, decimal Number_Year_Sem, string course_mode)
		{
			List<Sem_Years> sem_Years = new List<Sem_Years>();

			for (int i = 1; i <= Number_Year_Sem; i++)
			{
				sem_Years.Add(new Sem_Years
				{
					count = i,
					Sementer_Year = course_mode + " - " + i,
					Subjects = Get_university_course_subject_list(University_ID, Univ_Course_ID, Univ_subCourse_ID, i)
				});
			}

			return sem_Years;
		}

		public List<Subject> Get_university_course_subject_list(int University_ID, int Univ_Course_ID, int Univ_subCourse_ID, int Year_Sem)
		{
			List<Subject> subject = new List<Subject>();

			string Query_UserName = "SELECT Univ_Subject_ID, University_ID, Univ_Course_ID, Univ_subCourse_ID, Semester_Year, Subject_Name, Subject_Code, Entry_DateTime, Entry_By, Is_Deleted, Deleted_DateTime, Deleted_By FROM university_subject WHERE University_ID=@University_ID and Univ_Course_ID=@Univ_Course_ID and Univ_subCourse_ID=@Univ_subCourse_ID and Semester_Year=@Semester_Year and Is_Deleted=0;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			cmd.Parameters.AddWithValue("@Univ_subCourse_ID", Univ_subCourse_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", Year_Sem);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					subject.Add(new Subject
					{
						Count = i,
						Univ_Subject_ID = Convert.ToInt32(dr["Univ_Subject_ID"].ToString().Trim()),
						Subject_Code = dr["Subject_Code"].ToString().Trim(),
						Subject_Name = dr["Subject_Name"].ToString().Trim()
					});
					i++;
				}
			}

			return subject;
		}

		public bool university_subject_Insert_subcourese(university_subject_insert usi)
		{
			string Query = "INSERT INTO university_subject( University_ID, Univ_Course_ID, Univ_subCourse_ID, Semester_Year, Subject_Name, Subject_Code, Entry_DateTime, Entry_By) VALUES ( @University_ID, @Univ_Course_ID, @Univ_subCourse_ID, @Semester_Year, @Subject_Name, @Subject_Code, now(), @Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", usi.University_ID);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", usi.Univ_Course_ID);
			cmd.Parameters.AddWithValue("@Univ_subCourse_ID", usi.Univ_subCourse_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", usi.Semester_Year);
			cmd.Parameters.AddWithValue("@Subject_Name", usi.Subject_Name);
			cmd.Parameters.AddWithValue("@Subject_Code", usi.Subject_Code);
			cmd.Parameters.AddWithValue("@Entry_By", usi.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool university_subject_Delete(university_subject_delete usd)
		{
			string Query = "UPDATE university_subject SET Is_Deleted=1,Deleted_DateTime=now(),Deleted_By=@Deleted_By WHERE Univ_Subject_ID=@Univ_Subject_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Univ_Subject_ID", usd.Univ_Subject_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", usd.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		#endregion

		#region Univsersity Fees

		public List<Course_Fees> Get_university_course_List_ForFee(int University_ID, int Session_ID)
		{
			List<Course_Fees> univ_course = new List<Course_Fees>();
			string Query_UserName = "SELECT Univ_Course_ID, University_ID, Univ_Course_Name, Univ_Course_Code, mcm.Course_Mode, concat( TRIM(TRAILING '.0' FROM Number_of_Year_Sem) ,' - ',mcm.Course_Mode) as course_duration, ceiling(Number_of_Year_Sem) As Number_of_Year_Sem FROM university_course uc left join master_course_mode mcm on uc.Course_Mode_ID=mcm.Course_Mode_ID where Is_Deleted=0 and University_ID=@University_ID; ";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_course.Add(new Course_Fees
					{
						count = i,
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_Course_Code = dr["Univ_Course_Code"].ToString().Trim(),
						Course_Mode = dr["Course_Mode"].ToString().Trim(),
						Number_of_Year_Sem = dr["course_duration"].ToString().Trim(),
						Number_Year_Sem = Convert.ToDecimal(dr["Number_of_Year_Sem"].ToString().Trim()),
						Sub_course = Get_university_sub_course_list_forFee(Convert.ToInt32(dr["University_ID"].ToString().Trim()), Session_ID, Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()), Convert.ToDecimal(dr["Number_of_Year_Sem"].ToString().Trim()), dr["Course_Mode"].ToString().Trim())
					});
					i++;
				}
			}
			return univ_course;
		}

		public List<sub_course_fee> Get_university_sub_course_list_forFee(int University_ID, int Session_ID, int Univ_Course_ID, decimal Number_of_Year_Sem, string Course_Mode)
		{
			List<sub_course_fee> univ_Sub_course = new List<sub_course_fee>();
			string Query_UserName = "SELECT Univ_subCourse_ID, Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code FROM university_sub_course WHERE Is_Deleted=0 and Univ_Course_ID=@Univ_Course_ID and University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_Sub_course.Add(new sub_course_fee
					{
						count = i,
						Univ_subCourse_ID = Convert.ToInt32(dr["Univ_subCourse_ID"].ToString().Trim()),
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
						Univ_subCourse_Code = dr["Univ_subCourse_Code"].ToString().Trim(),
						semesters = Get_university_course_semyear_list_froFee(University_ID, Session_ID, Univ_Course_ID, Convert.ToInt32(dr["Univ_subCourse_ID"].ToString().Trim()), Number_of_Year_Sem, Course_Mode)
					}); ;
					i++;
				}
			}
			return univ_Sub_course;
		}

		public List<Sem_Years_fee> Get_university_course_semyear_list_froFee(int University_ID, int Session_ID, int Univ_Course_ID, int Univ_subCourse_ID, decimal Number_Year_Sem, string course_mode)
		{
			List<Sem_Years_fee> sem_Years = new List<Sem_Years_fee>();

			for (int i = 1; i <= Number_Year_Sem; i++)
			{
				sem_Years.Add(new Sem_Years_fee
				{
					count = i,
					Sementer_Year = course_mode + " - " + i,
					Fees = Get_university_course_fees(University_ID, Session_ID, Univ_Course_ID, Univ_subCourse_ID, i)
				});
			}
			return sem_Years;
		}

		public decimal Get_university_course_fees(int University_ID, int Session_ID, int Univ_Course_ID, int Univ_subCourse_ID, int Semester_Year)
		{
			string Query_UserName = "select Amount from university_course_fees WHERE University_ID=@University_ID and Session_ID=@Session_ID and Univ_Course_ID=@Univ_Course_ID and Univ_subCourse_ID=@Univ_subCourse_ID and Semester_Year=@Semester_Year;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			cmd.Parameters.AddWithValue("@Session_ID", Session_ID);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			cmd.Parameters.AddWithValue("@Univ_subCourse_ID", Univ_subCourse_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", Semester_Year);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
				return Convert.ToDecimal(dt.Rows[0]["Amount"].ToString().Trim());
			else
				return 0;
		}

		public bool university_course_fees_Insert(University_Fees_Insert ufi)
		{
			string Query = string.Empty;
			decimal Amt = Get_university_course_fees(ufi.University_ID, ufi.Session_ID, ufi.Univ_Course_ID, ufi.Univ_subCourse_ID, ufi.Semester_Year);
			if (Amt == 0)
				Query = "INSERT INTO university_course_fees( University_ID, Session_ID, Univ_Course_ID, Univ_subCourse_ID, Semester_Year, Amount, Entry_DateTime, Entry_By) VALUES (@University_ID, @Session_ID, @Univ_Course_ID, @Univ_subCourse_ID, @Semester_Year, @Amount, now(), @Entry_By);";
			else if (Amt != ufi.Amount)
				Query = "UPDATE university_course_fees SET Amount=@Amount, Entry_By=@Entry_By WHERE University_ID=@University_ID and Session_ID=@Session_ID and Univ_Course_ID=@Univ_Course_ID and Univ_subCourse_ID=@Univ_subCourse_ID and Semester_Year=@Semester_Year;";
			else
				return true;
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", ufi.University_ID);
			cmd.Parameters.AddWithValue("@Session_ID", ufi.Session_ID);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", ufi.Univ_Course_ID);
			cmd.Parameters.AddWithValue("@Univ_subCourse_ID", ufi.Univ_subCourse_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", ufi.Semester_Year);
			cmd.Parameters.AddWithValue("@Amount", ufi.Amount);
			cmd.Parameters.AddWithValue("@Entry_By", ufi.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		//public bool university_course_fees_Update(University_Fees_Insert ufi)
		//{
		//	string Query = "UPDATE university_course_fees SET Amount=@Amount, Entry_By=@Entry_By WHERE University_ID=@University_ID and Session_ID=@Session_ID and Univ_Course_ID=@Univ_Course_ID and Univ_subCourse_ID=@Univ_subCourse_ID and Semester_Year=@Semester_Year;";
		//	MySqlCommand cmd = new MySqlCommand(Query);
		//	cmd.Parameters.AddWithValue("@University_ID", ufi.University_ID);
		//	cmd.Parameters.AddWithValue("@Session_ID", ufi.Session_ID);
		//	cmd.Parameters.AddWithValue("@Univ_Course_ID", ufi.Univ_Course_ID);
		//	cmd.Parameters.AddWithValue("@Univ_subCourse_ID", ufi.Univ_subCourse_ID);
		//	cmd.Parameters.AddWithValue("@Semester_Year", ufi.Semester_Year);
		//	cmd.Parameters.AddWithValue("@Amount", ufi.Amount);
		//	cmd.Parameters.AddWithValue("@Entry_By", ufi.Entry_By);
		//	bool val = Fnc.CURDCommands(cmd);
		//	return val;
		//}

		#endregion

		public bool University_Insert(University uni)
		{
			string Query = "INSERT INTO university_details(University_Name, Contact_Number, Email_ID, Address, Pin_Code, Website_URL, Univsersity_Logo, University_Details,Entry_By,Establishment_Year,Registration_Number) VALUES(@University_Name,@Contact_Number,@Email_ID,@Address,@Pin_Code,@Website_URL,@Univsersity_Logo,@University_Details,@Entry_By,@Establishment_Year,@Registration_Number);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_Name", uni.University_Name);
			cmd.Parameters.AddWithValue("@Contact_Number", uni.Contact_Number);
			cmd.Parameters.AddWithValue("@Email_ID", uni.Email_ID);
			cmd.Parameters.AddWithValue("@Address", uni.Address);
			cmd.Parameters.AddWithValue("@Pin_Code", uni.Pin_Code);
			cmd.Parameters.AddWithValue("@Website_URL", uni.Website_URL);
			cmd.Parameters.AddWithValue("@Univsersity_Logo", uni.Univsersity_Logo);
			cmd.Parameters.AddWithValue("@University_Details", uni.University_Details);
			cmd.Parameters.AddWithValue("@Entry_By", uni.Entry_By);
			cmd.Parameters.AddWithValue("@Establishment_Year", uni.Establishment_Year);
			cmd.Parameters.AddWithValue("@Registration_Number", uni.Registration_Number);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		#region Student Insert and Update

		public int Student_Insert(Student std)
		{
			string Query = @"INSERT INTO student_details(Student_Name, Father_Name, Gender, Date_of_Birth, 
                            Aadhar_Number, Enrollment_Number, Date_of_Admission, Mobile_Number_Stu, Mobile_Number_Father,
                            Email_ID, Address,University_ID, Univ_Course_ID, Univ_subCourse_ID, Session_ID, 
                            Course_Mode_ID,Semester_Year, Entry_By) VALUES (@Student_Name,@Father_Name, @Gender, @Date_of_Birth,@Aadhar_Number,
                            @Enrollment_Number,@Date_of_Admission,@Mobile_Number_Stu,@Mobile_Number_Father,
                            @Email_ID,@Address,@University_ID,@Univ_Course_ID,@Univ_subCourse_ID,@Session_ID,@Course_Mode_ID,@Semester_Year,@Entry_By)";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Student_Name", std.Student_Name);
			cmd.Parameters.AddWithValue("@Father_Name", std.Father_Name);
            cmd.Parameters.AddWithValue("@Gender", std.Gender);
            cmd.Parameters.AddWithValue("@Date_of_Birth", std.Date_of_Birth);
			cmd.Parameters.AddWithValue("@Aadhar_Number", std.Aadhar_Number == null ? "" : std.Aadhar_Number); //std.Aadhar_Number); //
			cmd.Parameters.AddWithValue("@Enrollment_Number", std.Enrollment_Number == null ? "" : std.Enrollment_Number);
			cmd.Parameters.AddWithValue("@Date_of_Admission", std.Date_of_Admission);
			cmd.Parameters.AddWithValue("@Mobile_Number_Stu", std.Mobile_Number_Stu);
			cmd.Parameters.AddWithValue("@Mobile_Number_Father", std.Mobile_Number_Father == null ? "" : std.Mobile_Number_Father);
			cmd.Parameters.AddWithValue("@Email_ID", std.Email_ID == null ? "" : std.Email_ID);
			cmd.Parameters.AddWithValue("@Address", std.Address);
			cmd.Parameters.AddWithValue("@University_ID", std.University_ID);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", std.Univ_Course_ID);
			cmd.Parameters.AddWithValue("@Univ_subCourse_ID", std.Univ_subCourse_ID);
			cmd.Parameters.AddWithValue("@Session_ID", std.Session_ID);
			cmd.Parameters.AddWithValue("@Course_Mode_ID", std.Course_Mode_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", std.Semester_Year);
			cmd.Parameters.AddWithValue("@Entry_By", std.Entry_By);
			int val = Fnc.InsertCommands_AndGetting_ID(cmd);
			return val;
		}

        public int Fees_Insert(String Values_Str)
        {
            String QRY = @"Insert Into Student_Fees_Collection (University_ID, Student_ID, Session_ID, Enrollment_Number, Fees_Amount, Txn_Date ) VALUES";
            QRY = QRY + Values_Str;
            MySqlCommand CMD = new MySqlCommand(QRY);            
            int val = Fnc.InsertCommands_AndGetting_ID(CMD);            
            return val;
        }

        public int StudentsPromoteInsert(String Pmt_Str)
        {
            String QRY = @"INSERT INTO student_promote(University_Id, Student_Id, Session_ID, Enrollment_Number, Semester_Year, Promotion_DateTime) VALUES";
            QRY = QRY + Pmt_Str;
            MySqlCommand CMD = new MySqlCommand(QRY);
            int val = Fnc.InsertCommands_AndGetting_ID(CMD);
            return val;
        }
        public bool StudentsPromoteUpdate(int SID, int UID, int SC_ID, String PSID)
        {            
            String QRY = @"Update Student_Details SD INNER JOIN Student_Promote SP ON SD.Student_Id = SP.Student_Id 
            Set SD.Is_Passout = Case When SD.Is_Passout = 0 Then 3 Else SD.Is_Passout END, SP.Is_Passout = Case When SD.Is_Passout = 3 Then 3 Else 0 END 
            WHERE SD.University_Id = @Uid AND SD.Is_Deleted = 0 AND SD.Univ_subCourse_ID = @SC_id 
            AND (SD.Session_ID = @Sid OR SP.Session_ID = @Sid) AND (SD.Is_Passout = 0 OR SP.Is_Passout = 0)";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@Uid", UID);
            CMD.Parameters.AddWithValue("@Sid", SID);
            CMD.Parameters.AddWithValue("@SC_id", SC_ID);
            CMD.Parameters.AddWithValue("@PS_id", PSID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool StudentsPassOutUpdate(String PO_Str)
        {
            String QRY = @"Update student_details set Is_Passout = 1 Where Student_Id in (" + PO_Str + ")";           
            MySqlCommand CMD = new MySqlCommand(QRY);
            //int val = Fnc.InsertCommands_AndGetting_ID(CMD);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }
        public bool StudentsDropOutUpdate(String DO_Str)
        {
            String QRY = @"Update student_details set Is_Passout = 2 Where Student_Id in (" + DO_Str + ")";
            MySqlCommand CMD = new MySqlCommand(QRY);
            //int val = Fnc.InsertCommands_AndGetting_ID(CMD);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool Fees_Delete(int Txn_ID)
        {
            String QRY = @"UPDATE student_fees_collection SET Is_Deleted = 1 WHERE Txn_ID = @Txn_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@Txn_ID", Txn_ID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool DegreeFile_Delete(int File_Id)
        {
            String QRY = @"UPDATE uploaded_degree_data SET Is_Deleted = 1, Deleted_dateTime = now() WHERE File_ID = @F_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@F_ID", File_Id);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }
        public bool InternshipFile_Delete(int File_Id)
        {
            String QRY = @"UPDATE uploaded_internship_data SET Is_Deleted = 1, Deleted_dateTime = now() WHERE File_ID = @F_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@F_ID", File_Id);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }
        public bool ACFile_Delete(int File_Id)
        {
            String QRY = @"UPDATE uploaded_academiccalendar SET Is_Deleted = 1, Deleted_dateTime = now() WHERE File_ID = @F_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@F_ID", File_Id);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool FeesFile_Delete(int File_Id)
        {
            String QRY = @"UPDATE uploaded_fees_data SET Is_Deleted = 1, Deleted_dateTime = now() WHERE File_ID = @F_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@F_ID", File_Id);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool Update_Fees(int Txn_ID, double FAmt, String FR_Date)
        {
            String QRY = @"UPDATE student_fees_collection SET Fees_Amount = @F_Amt, Txn_Date = @Txn_Date WHERE Txn_ID = @Txn_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@Txn_ID", Txn_ID);
            CMD.Parameters.AddWithValue("@F_Amt", FAmt);
            CMD.Parameters.AddWithValue("@Txn_Date", FR_Date);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }


        public bool IsENOExists(String ENO, int UID)
        {
            
            String QRY = @"SELECT Count(Student_Id) As NoOfStudent  FROM student_details WHERE University_ID = @UID AND Is_Deleted = 0 AND Enrollment_Number = @ENO;";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@ENO", ENO);
            CMD.Parameters.AddWithValue("@UID", UID);
            DataTable DT = Fnc.GetDataTable(CMD);
            bool Status = false;
            if (Convert.ToInt32(DT.Rows[0]["NoOfStudent"]) > 0)                
            {
                Status = true;
            }
           
            return Status;
        }

        public bool IsENOExistsForEdit(String ENO, int UID, int SID)
        {

            String QRY = @"SELECT Count(Student_Id) As NoOfStudent  FROM student_details WHERE University_ID = @UID AND Is_Deleted = 0 AND Enrollment_Number = @ENO AND Student_Id <> @Student_Id";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@ENO", ENO);
            CMD.Parameters.AddWithValue("@UID", UID);
            CMD.Parameters.AddWithValue("@@Student_Id", SID);
            DataTable DT = Fnc.GetDataTable(CMD);
            bool Status = false;
            if (Convert.ToInt32(DT.Rows[0]["NoOfStudent"]) > 0)
            {
                Status = true;
            }

            return Status;
        }

        public bool Student_Update(StudentEdit STE)
        {
            string QRY = string.Empty;
            QRY = @"UPDATE student_details SET Enrollment_Number = @Enrollment_Number, Student_Name = @Student_Name, Father_Name = @Father_Name, Gender = @Gender, Date_of_Birth = @Date_of_Birth,
                  Aadhar_Number = @Aadhar_Number, Date_of_Admission = @Date_of_Admission, Mobile_Number_Stu = @Mobile_Number_Stu, Mobile_Number_Father = @Mobile_Number_Father, 
                  Email_ID = @Email_ID, Address = @Address, Univ_Course_ID = @Univ_Course_ID, Univ_subCourse_ID = @Univ_subCourse_ID, Course_Mode_ID = @Course_Mode_ID, 
                  Semester_Year = @Semester_Year, Update_DateTime = now(), Updated_By = 2 WHERE student_details.Student_Id = @Student_Id";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@Student_Id", STE.Student_Id);
            CMD.Parameters.AddWithValue("@Enrollment_Number", STE.Enrollment_Number);
            CMD.Parameters.AddWithValue("@Student_Name", STE.Student_Name);
            CMD.Parameters.AddWithValue("@Father_Name", STE.Father_Name);
            CMD.Parameters.AddWithValue("@Gender", STE.Gender);
            CMD.Parameters.AddWithValue("@Date_of_Birth", STE.Date_of_Birth);
            CMD.Parameters.AddWithValue("@Aadhar_Number", STE.Aadhar_Number);
            CMD.Parameters.AddWithValue("@Date_of_Admission", STE.Date_of_Admission);
            CMD.Parameters.AddWithValue("@Mobile_Number_Stu", STE.Mobile_Number_Stu);
            CMD.Parameters.AddWithValue("@Mobile_Number_Father", STE.Mobile_Number_Father);
            CMD.Parameters.AddWithValue("@Email_ID", STE.Email_ID);
            CMD.Parameters.AddWithValue("@Address", STE.Address);
            CMD.Parameters.AddWithValue("@Univ_Course_ID", STE.Univ_Course_ID);
            CMD.Parameters.AddWithValue("@Univ_subCourse_ID", STE.Univ_subCourse_ID);
            CMD.Parameters.AddWithValue("@Course_Mode_ID", STE.Course_Mode_ID);
            CMD.Parameters.AddWithValue("@Semester_Year", STE.Semester_Year);          

            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool SubCourseUpdate(University_Course_details UCDE)
        {
            string QRY = string.Empty;
            QRY = @"UPDATE university_sub_course SET Univ_subCourse_Name = @SubCourseName, Univ_subCourse_Code = @Univ_subCourse_Code WHERE Univ_subCourse_ID = @Univ_subCourse_ID;";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@SubCourseName", UCDE.SubCourseName);
            CMD.Parameters.AddWithValue("@Univ_subCourse_Code", UCDE.Univ_subCourse_Code);
            CMD.Parameters.AddWithValue("@Univ_subCourse_ID", UCDE.Univ_subCourse_ID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool CourseUpdate(University_Course_details UCDE)
        {
            string QRY = string.Empty;
            QRY = @"UPDATE university_course SET Univ_Course_Name = @CourseName, Univ_Course_Code = @Univ_Course_Code, Course_Mode_ID = @Course_Mode_ID, Number_of_Year_Sem = @NOYS WHERE Univ_Course_ID = @Univ_Course_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@CourseName", UCDE.CourseName);
            CMD.Parameters.AddWithValue("@Univ_Course_Code", UCDE.Univ_Course_Code);
            CMD.Parameters.AddWithValue("@Univ_Course_ID", UCDE.Univ_Course_ID);
            CMD.Parameters.AddWithValue("@Course_Mode_ID", UCDE.CourseMode);
            CMD.Parameters.AddWithValue("@NOYS", UCDE.NOYS);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public int UploadStudentData(Upload_Student_data FD)
        {
            string Query = @"INSERT INTO uploaded_student_data (University_ID, File_Name, File_path, Uploaded_DateTime, Session_ID ) 
                            VALUES (@University_ID,@File_Name,@File_path, NOW(), @Session_ID )";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@University_ID", FD.University_ID);
            cmd.Parameters.AddWithValue("@File_Name", FD.File_Name);
            cmd.Parameters.AddWithValue("@File_path", FD.File_path);
            cmd.Parameters.AddWithValue("@Session_ID", FD.Session_Id);
            int val = Fnc.InsertCommands_AndGetting_ID(cmd);        
            return val;
        }

        public int UploadFeesData(Upload_Student_data FD)
        {
            string Query = @"INSERT INTO uploaded_fees_data (University_ID, File_Name, File_path, Uploaded_DateTime, Session_ID ) 
                            VALUES (@University_ID,@File_Name,@File_path, NOW(), @Session_ID)";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@University_ID", FD.University_ID);
            cmd.Parameters.AddWithValue("@File_Name", FD.File_Name);
            cmd.Parameters.AddWithValue("@File_path", FD.File_path);
            cmd.Parameters.AddWithValue("@Session_ID", FD.Session_Id);
            int val = Fnc.InsertCommands_AndGetting_ID(cmd);
            return val;
        }

        public int UploadDegreeData(Upload_Student_data FD)
        {
            string Query = @"INSERT INTO uploaded_degree_data (University_ID, File_Name, File_path, Uploaded_DateTime ) 
                            VALUES (@University_ID,@File_Name,@File_path, NOW() )";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@University_ID", FD.University_ID);
            cmd.Parameters.AddWithValue("@File_Name", FD.File_Name);
            cmd.Parameters.AddWithValue("@File_path", FD.File_path);            
            int val = Fnc.InsertCommands_AndGetting_ID(cmd);
            return val;
        }
        public int UploadInternshipData(Upload_Student_data FD)
        {
            string Query = @"INSERT INTO uploaded_Internship_data (University_ID, File_Name, File_path, Uploaded_DateTime, Session_ID ) 
                            VALUES (@University_ID,@File_Name,@File_path, NOW(), @Session_ID )";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@University_ID", FD.University_ID);
            cmd.Parameters.AddWithValue("@File_Name", FD.File_Name);
            cmd.Parameters.AddWithValue("@File_path", FD.File_path);
            cmd.Parameters.AddWithValue("@Session_ID", FD.Session_Id);
            int val = Fnc.InsertCommands_AndGetting_ID(cmd);
            return val;
        }
        public int UploadAcademicCalendar(Upload_Student_data FD)
        {
            string Query = @"INSERT INTO uploaded_academiccalendar (University_ID, File_Name, File_path, Uploaded_DateTime, Session_ID ) 
                            VALUES (@University_ID,@File_Name,@File_path, NOW(), @Session_ID )";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@University_ID", FD.University_ID);
            cmd.Parameters.AddWithValue("@File_Name", FD.File_Name);
            cmd.Parameters.AddWithValue("@File_path", FD.File_path);
            cmd.Parameters.AddWithValue("@Session_ID", FD.Session_Id);
            int val = Fnc.InsertCommands_AndGetting_ID(cmd);
            return val;
        }


        public bool Delete_ExcelFile(int File_ID)
        {
            String QRY = @"UPDATE uploaded_student_data SET Is_Deleted = 1,  Deleted_dateTime  = now(), Deleted_By = 2 WHERE File_ID = @File_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@File_ID", File_ID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }


        public bool Update_student_Class_sem(student_promote student_Promote)
		{
			string Query = string.Empty;
			if (student_Promote.Is_Passout > 0)
				Query = "update university_details set Session_ID=@Session_ID, Semester_Year=@Semester_Year, Updated_By=@Updated_By, Update_DateTime=now(),Is_Passout=1,PassOut_DateTime=now() where Student_Id=@Student_Id ;";
			else
				Query = "update university_details set Session_ID=@Session_ID, Semester_Year=@Semester_Year, Updated_By=@Updated_By, Update_DateTime=now() where Student_Id=@Student_Id ;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Session_ID", student_Promote.Session_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", student_Promote.Semester_Year);
			cmd.Parameters.AddWithValue("@Updated_By", student_Promote.Promoted_By);
			cmd.Parameters.AddWithValue("@Student_Id", student_Promote.Student_Id);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}


		#endregion

		public List<SelectListItem> PopulateCourse(int university_id)
		{
			List<SelectListItem> ListSessions = new List<SelectListItem>();
			//string Query_UserName = "SELECT Univ_Course_ID, University_ID, Univ_Course_Name, Univ_Course_Code, Course_Mode_ID, Number_of_Year_Sem FROM university_course WHERE  Is_Deleted =0  AND  University_ID=@UniID";
            string Qry = "SELECT Univ_Course_ID, Univ_Course_Name FROM university_course WHERE  Is_Deleted =0  AND  University_ID=@UniID";
            MySqlCommand cmd = new MySqlCommand(Qry);
			cmd.Parameters.AddWithValue("@UniID", university_id);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{

				foreach (DataRow dr in dt.Rows)
				{
					ListSessions.Add(new SelectListItem
					{
						Value = dr["Univ_Course_ID"].ToString(),
						Text = dr["Univ_Course_Name"].ToString()

					});
				}
			}

			return ListSessions;
		}

//        public List<SelectListItem> PopulateCourseForFees(int university_id)
//        {
//            List<SelectListItem> CoursesList = new List<SelectListItem>();           
//            string Qry = @"Select UC.Univ_Course_ID, UC.Univ_Course_Name, ceiling(UC.Number_of_Year_Sem) As NoOfSY From (SELECT Univ_Course_ID FROM student_details WHERE University_ID = @UniID AND Is_Deleted = 0
//Group By Univ_Course_ID) As SD INNER JOIN university_course UC ON SD.Univ_Course_ID = UC.Univ_Course_ID Order By UC.Univ_Course_Name";
//            MySqlCommand cmd = new MySqlCommand(Qry);
//            cmd.Parameters.AddWithValue("@UniID", university_id);
//            DataTable dt = Fnc.GetDataTable(cmd);
//            if (dt.Rows.Count > 0)
//            {
//                foreach (DataRow dr in dt.Rows)
//                {
//                    CoursesList.Add(new SelectListItem
//                    {
//                        Value = dr["Univ_Course_ID"].ToString(),
//                        Text = dr["Univ_Course_Name"].ToString()                       
//                    });
//                }
//            }

//            return CoursesList;
//        }

        public DataTable PopulateCourseForFeesJR(int university_id, int SID)
        {
            //            string Qry = @"Select UC.Univ_Course_ID, UC.Univ_Course_Name, ceiling(UC.Number_of_Year_Sem) As NoOfSY, UC.Course_Mode_ID From (SELECT Univ_Course_ID FROM student_details WHERE University_ID = @UniID AND Is_Deleted = 0
            //Group By Univ_Course_ID) As SD INNER JOIN university_course UC ON SD.Univ_Course_ID = UC.Univ_Course_ID Order By UC.Univ_Course_Name";
            String Qry = @"Select UC.Univ_Course_ID, UC.Univ_Course_Name, ceiling(UC.Number_of_Year_Sem) As NoOfSY, UC.Course_Mode_ID 
From (Select Univ_Course_ID From student_details SD INNER JOIN (
SELECT Student_Id, Enrollment_Number FROM Student_Details Where University_ID = @UniID AND Session_ID = @Session_ID AND Is_Deleted = 0 AND Is_Passout <> 1
Union 
SELECT Student_Id, Enrollment_Number FROM Student_Promote Where University_ID = @UniID AND Session_ID = @Session_ID AND Is_Deleted = 0 AND Is_Passout <> 1
) As PD ON SD.Student_Id = PD.Student_Id Group By Univ_Course_ID) As PD INNER JOIN university_course UC ON PD.Univ_Course_ID = UC.Univ_Course_ID Order By UC.Univ_Course_Name";
            MySqlCommand cmd = new MySqlCommand(Qry);
            cmd.Parameters.AddWithValue("@UniID", university_id);
            cmd.Parameters.AddWithValue("@Session_ID", SID);
            DataTable dt = Fnc.GetDataTable(cmd);
            return dt;
        }

        public DataTable PopulateCourseForPromoteJR(int university_id, int SID)
        {
            string Qry = @"Select UC.Univ_Course_ID, UC.Univ_Course_Name, ceiling(UC.Number_of_Year_Sem) As NoOfSY, UC.Course_Mode_ID 
From (Select Univ_Course_ID From student_details SD INNER JOIN (
SELECT Student_Id, Enrollment_Number FROM Student_Details Where University_ID = @UniID AND Session_ID = @Session_ID AND Is_Deleted = 0 AND Is_Passout = 0
Union 
SELECT Student_Id, Enrollment_Number FROM Student_Promote Where University_ID = @UniID AND Session_ID = @Session_ID AND Is_Deleted = 0 AND Is_Passout = 0
) As PD ON SD.Student_Id = PD.Student_Id Group By Univ_Course_ID) As PD INNER JOIN university_course UC ON PD.Univ_Course_ID = UC.Univ_Course_ID Order By UC.Univ_Course_Name";
            MySqlCommand cmd = new MySqlCommand(Qry);
            cmd.Parameters.AddWithValue("@UniID", university_id);
            cmd.Parameters.AddWithValue("@Session_ID", SID);
            DataTable dt = Fnc.GetDataTable(cmd);
            return dt;
        }


        public List<SelectListItem> GetSubCourse(int Univ_Course_ID, int university_id)
        {
            List<SelectListItem> ListSC = new List<SelectListItem>();
            string QRY = "SELECT Univ_subCourse_ID, Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code FROM university_sub_course where Univ_Course_ID=@Univ_Course_ID AND University_ID=@UniID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@UniID", university_id);
            CMD.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
            DataTable dt = Fnc.GetDataTable(CMD);
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    ListSC.Add(new SelectListItem
                    {
                        Value = dr["Univ_subCourse_ID"].ToString(),
                        Text = dr["Univ_subCourse_Name"].ToString()

                    });
                }
            }


            return ListSC;
        }
        public List<SelectListItem>  GenderList()
        {
            List<SelectListItem> ListSessions = new List<SelectListItem>();
            string Qry = "SELECT Gender, Gender_Code FROM gender";
            MySqlCommand cmd = new MySqlCommand(Qry);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    ListSessions.Add(new SelectListItem
                    {
                        Value = dr["Gender_Code"].ToString(),
                        Text = dr["Gender"].ToString()

                    });
                }
            }

            return ListSessions;
        }

        public List<SelectListItem> CourseMode()
        {
            List<SelectListItem> CL = new List<SelectListItem>();
            string Qry = "SELECT Course_Mode_ID, Course_Mode FROM master_course_mode;";
            MySqlCommand cmd = new MySqlCommand(Qry);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    CL.Add(new SelectListItem
                    {
                        Value = dr["Course_Mode_ID"].ToString(),
                        Text = dr["Course_Mode"].ToString()
                    });
                }
            }

            return CL;
        }

        public List<SelectListItem> PopulateMasterUniversity()
		{
			List<SelectListItem> ListMasterUniversity = new List<SelectListItem>();
			string Query_UserName = "SELECT University_ID, University_Name FROM university_details";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{

				foreach (DataRow dr in dt.Rows)
				{
					ListMasterUniversity.Add(new SelectListItem
					{
						Value = dr["University_ID"].ToString(),
						Text = dr["University_Name"].ToString()

					});
				}
			}

			return ListMasterUniversity;
		}

		public DataTable PopulateSubCourse(int Univ_Course_ID, int university_id)
		{
			List<SelectListItem> ListSessions = new List<SelectListItem>();
			string Query_UserName = "SELECT Univ_subCourse_ID, Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code FROM university_sub_course where Univ_Course_ID=@Univ_Course_ID AND University_ID=@UniID";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UniID", university_id);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			return dt;
		}

        public DataTable PopulateSubCourseForFees(int Univ_Course_ID, int university_id, int SID)
        {
            //            string Query_UserName = @"SELECT SD.Univ_subCourse_ID, USC.Univ_subCourse_Name FROM Student_details SD INNER JOIN university_sub_course 
            //USC ON SD.Univ_subCourse_ID = USC.Univ_subCourse_ID  Where SD.Is_Deleted = 0 AND 
            //SD.University_ID = @UniID AND SD.Univ_Course_ID = @Univ_Course_ID Group By SD.Univ_subCourse_ID, USC.Univ_subCourse_Name
            //ORDER BY USC.Univ_subCourse_Name;";
            string Query_UserName = @"Select USC.Univ_subCourse_ID, USC.Univ_subCourse_Name
From (Select Univ_subCourse_ID From student_details SD INNER JOIN (
SELECT Student_Id, Enrollment_Number FROM Student_Details Where University_ID = @UniID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout <> 1
Union 
SELECT Student_Id, Enrollment_Number FROM Student_Promote Where University_ID = @UniID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout <> 1
) As PD ON SD.Student_Id = PD.Student_Id Where Univ_Course_ID = @Univ_Course_ID Group By Univ_subCourse_ID) As PD INNER JOIN university_sub_course USC ON PD.Univ_subCourse_ID = USC.Univ_subCourse_ID Order By USC.Univ_subCourse_Name";
           MySqlCommand cmd = new MySqlCommand(Query_UserName);
            cmd.Parameters.AddWithValue("@UniID", university_id);
            cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
            cmd.Parameters.AddWithValue("@Sid", SID);
            DataTable dt = Fnc.GetDataTable(cmd);
            return dt;
        }

        public DataTable PopulateSubCourseForPromote(int Univ_Course_ID, int university_id, int SID)
        {
            string Query_UserName = @"Select USC.Univ_subCourse_ID, USC.Univ_subCourse_Name
From (Select Univ_subCourse_ID From student_details SD INNER JOIN (
SELECT Student_Id, Enrollment_Number FROM Student_Details Where University_ID = @UniID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout = 0
Union 
SELECT Student_Id, Enrollment_Number FROM Student_Promote Where University_ID = @UniID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout = 0
) As PD ON SD.Student_Id = PD.Student_Id Where Univ_Course_ID = @Univ_Course_ID Group By Univ_subCourse_ID) As PD INNER JOIN university_sub_course USC ON PD.Univ_subCourse_ID = USC.Univ_subCourse_ID Order By USC.Univ_subCourse_Name";

            MySqlCommand cmd = new MySqlCommand(Query_UserName);
            cmd.Parameters.AddWithValue("@UniID", university_id);
            cmd.Parameters.AddWithValue("@Sid", SID);
            cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
            DataTable dt = Fnc.GetDataTable(cmd);
            return dt;
        }

        public DataTable PopulateSem_Year(int Univ_subCourse_ID, int University_Id)
        {
            String QRY = @"SELECT MCM.Course_Mode, SD.Semester_Year FROM student_details SD INNER JOIN master_course_mode MCM ON SD.Course_Mode_ID = MCM.Course_Mode_ID 
            Where SD.University_ID = @University_ID AND SD.Univ_subCourse_ID = @Univ_subCourse_ID AND SD.Is_Deleted = 0 Group By SD.Semester_Year";
        //    String QRY = @"SELECT MCM.Course_Mode, SD.Semester_Year, ceiling(UC.Number_of_Year_Sem) As NoOfSY FROM student_details SD INNER JOIN master_course_mode MCM ON SD.Course_Mode_ID = MCM.Course_Mode_ID Inner join university_course UC ON SD.Univ_Course_ID = UC.Univ_Course_ID
        //Where SD.University_ID = @University_ID AND SD.Univ_subCourse_ID = @Univ_subCourse_ID AND SD.Is_Deleted = 0 Group By SD.Semester_Year";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@University_ID", University_Id);
            CMD.Parameters.AddWithValue("@Univ_subCourse_ID", Univ_subCourse_ID);
            DataTable DT = Fnc.GetDataTable(CMD);
            return DT;
        }
        public DataTable PopulateSem_YearForFees(int Univ_subCourse_ID, int University_Id, int SID)
        {
            String QRY = @"Select PD.Semester_Year From student_details SD INNER JOIN (
                        SELECT Student_Id, Enrollment_Number, Semester_Year FROM Student_Details Where University_ID = @University_ID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout <> 1
                        Union 
                        SELECT Student_Id, Enrollment_Number, Semester_Year FROM Student_Promote Where University_ID = @University_ID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout <> 1
                        ) As PD ON SD.Student_Id = PD.Student_Id Where SD.Univ_subCourse_ID = @Univ_subCourse_ID Group By PD.Semester_Year";

            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@University_ID", University_Id);
            CMD.Parameters.AddWithValue("@Univ_subCourse_ID", Univ_subCourse_ID);
            CMD.Parameters.AddWithValue("@Sid", SID);
            DataTable DT = Fnc.GetDataTable(CMD);
            return DT;
        }

        public DataTable PopulateSem_Year_Promote(int Univ_subCourse_ID, int University_Id, int SID)
        {
            String QRY = @"Select PD.Semester_Year From student_details SD INNER JOIN (
                        SELECT Student_Id, Enrollment_Number, Semester_Year FROM Student_Details Where University_ID = @University_ID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout = 0
                        Union 
                        SELECT Student_Id, Enrollment_Number, Semester_Year FROM Student_Promote Where University_ID = @University_ID AND Session_ID = @Sid AND Is_Deleted = 0 AND Is_Passout = 0
                        ) As PD ON SD.Student_Id = PD.Student_Id Where SD.Univ_subCourse_ID = @Univ_subCourse_ID Group By PD.Semester_Year";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@University_ID", University_Id);
            CMD.Parameters.AddWithValue("@Sid", SID);
            CMD.Parameters.AddWithValue("@Univ_subCourse_ID", Univ_subCourse_ID);
            DataTable DT = Fnc.GetDataTable(CMD);
            return DT;
        }
        public DataTable PopulateCourseMode1(int Univ_Course_ID, int university_id)
		{
			List<SelectListItem> ListSessions = new List<SelectListItem>();
			string Query_UserName = @"SELECT uni.Univ_Course_ID, uni.University_ID, uni.Univ_Course_Name, uni.Univ_Course_Code, uni.Course_Mode_ID, ceiling(uni.Number_of_Year_Sem) as Number_of_Year_Sem 
                                    , mcm.Course_Mode AS Course_Mode FROM university_course uni INNER JOIN master_course_mode mcm ON mcm.Course_Mode_ID = uni.Course_Mode_ID
                                    WHERE uni.Is_Deleted = 0 AND uni.Univ_Course_ID= @Univ_Course_ID AND uni.University_ID=@UniID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UniID", university_id);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			return dt;
		}

		public List<SelectListItem> PopulateSemester_Year(int Univ_Course_ID, int university_id)
		{
			List<SelectListItem> ListSessions = new List<SelectListItem>();
			string Query_UserName = @"SELECT uni.Univ_Course_ID, uni.University_ID, uni.Univ_Course_Name, uni.Univ_Course_Code, uni.Course_Mode_ID, ceiling(uni.Number_of_Year_Sem) as Number_of_Year_Sem 
                                    , mcm.Course_Mode AS Course_Mode
                                     FROM university_course  uni
                                     INNER JOIN master_course_mode mcm ON mcm.Course_Mode_ID = uni.Course_Mode_ID
                                    WHERE uni.Is_Deleted = 0 AND uni.Univ_Course_ID= @Univ_Course_ID AND uni.University_ID=@UniID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UniID", university_id);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{

				foreach (DataRow dr in dt.Rows)
				{
					ListSessions.Add(new SelectListItem
					{

						Text = dr["Number_of_Year_Sem"].ToString(),
						Value = dr["Number_of_Year_Sem"].ToString()

					});

				}
			}

			return ListSessions;
		}

		public List<SelectListItem> PopulateCourseMode()
		{
			List<SelectListItem> ListSessions = new List<SelectListItem>();
			string Query_UserName = "SELECT Course_Mode_ID, Course_Mode FROM master_course_mode";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);

			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{

				foreach (DataRow dr in dt.Rows)
				{
					ListSessions.Add(new SelectListItem
					{
						Text = dr["Course_Mode"].ToString(),
						Value = dr["Course_Mode_ID"].ToString()
					});
				}
			}

			return ListSessions;
		}

		public string PopulateCourseNameOnly(int Univ_Course_ID)
		{
			string Query_UserName = "SELECT concat(Univ_Course_Name,' [',Univ_Course_Code,']') as course_Name FROM university_course WHERE Univ_Course_ID=@Univ_Course_ID";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
				return dt.Rows[0][0].ToString();
			else
				return "";
		}

		public string PopulateSubCourseNameOnly(int Univ_subCourse_ID)
		{
			string Query_UserName = "SELECT concat(Univ_subCourse_Name,' [',Univ_subCourse_Code,']') as sub_course_Name FROM university_sub_course WHERE Univ_subCourse_ID=@Univ_subCourse_ID";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Univ_subCourse_ID", Univ_subCourse_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
				return dt.Rows[0][0].ToString();
			else
				return "";
		}

		public DataTable PopulateSemYearNameOnly(int Univ_Course_ID)
		{
			string Query_UserName = "SELECT mcm.Course_Mode, concat( Number_of_Year_Sem ,' - ',mcm.Course_Mode) as course_duration,Number_of_Year_Sem FROM university_course uc left join master_course_mode mcm on uc.Course_Mode_ID=mcm.Course_Mode_ID where Is_Deleted=0 and Univ_Course_ID=@Univ_Course_ID";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			return Fnc.GetDataTable(cmd);

		}

        public DataTable Get_Student_Detail(int Student_ID, int University_ID, int Session_ID)
        {
            string QryStd = @"SELECT Student_Id, Enrollment_Number, Student_Name, Father_Name, Case When Gender IS NULL Then 0 Else Gender END As Gender,
            Date_of_Birth, Aadhar_Number, Date_of_Admission, Mobile_Number_Stu, Mobile_Number_Father, Email_ID, Address, Univ_Course_ID, Univ_subCourse_ID, 
            Course_Mode_ID, Semester_Year, Update_DateTime FROM `student_details` WHERE University_ID = @University_ID AND Is_Deleted = 0 AND Session_ID = @Session_ID AND Student_Id = @Student_Id";
            MySqlCommand cmd = new MySqlCommand(QryStd);
            cmd.Parameters.AddWithValue("@Student_Id", Student_ID);
            cmd.Parameters.AddWithValue("@University_ID", University_ID);
            cmd.Parameters.AddWithValue("@Session_ID", Session_ID);
            DataTable SDT = Fnc.GetDataTable(cmd);
            return SDT;
        }

        public StudentEdit Get_Std_Record(int Student_ID)
        {             
            String QRY = @"SELECT Enrollment_Number, Student_Name, Father_Name, Case When Gender IS NULL Then 0 Else Gender END As Gender, 
                DATE_FORMAT(Date_of_Birth, '%Y-%m-%d') As Date_of_Birth, Aadhar_Number, DATE_FORMAT(Date_of_Admission, '%Y-%m-%d') As Date_of_Admission, Mobile_Number_Stu, Mobile_Number_Father, Email_ID, Address, Univ_Course_ID, 
                Univ_subCourse_ID, Course_Mode_ID, Semester_Year, Update_DateTime FROM `student_details` WHERE Student_ID = @Student_ID";
            StudentEdit SE = new StudentEdit();
            MySqlCommand cmd = new MySqlCommand(QRY);
            cmd.Parameters.AddWithValue("@Student_ID", Student_ID);
            DataTable SDT = Fnc.GetDataTable(cmd);
            if (SDT.Rows.Count > 0)
            {
                SE.Enrollment_Number = SDT.Rows[0]["Enrollment_Number"].ToString();
                SE.Student_Name = SDT.Rows[0]["Student_Name"].ToString();
                SE.Father_Name = SDT.Rows[0]["Father_Name"].ToString();
                SE.Gender = Convert.ToInt16(SDT.Rows[0]["Gender"]);
                SE.Date_of_Birth = SDT.Rows[0]["Date_of_Birth"].ToString();
                SE.Aadhar_Number = SDT.Rows[0]["Aadhar_Number"].ToString();
                SE.Date_of_Admission = SDT.Rows[0]["Date_of_Admission"].ToString();
                SE.Mobile_Number_Stu = SDT.Rows[0]["Mobile_Number_Stu"].ToString();
                SE.Mobile_Number_Father = SDT.Rows[0]["Mobile_Number_Father"].ToString();
                SE.Email_ID = SDT.Rows[0]["Email_ID"].ToString();
                SE.Address = SDT.Rows[0]["Address"].ToString();
                SE.Univ_Course_ID = Convert.ToInt32(SDT.Rows[0]["Univ_Course_ID"]);
                SE.Univ_subCourse_ID = Convert.ToInt32(SDT.Rows[0]["Univ_subCourse_ID"]);
                SE.Course_Mode_ID = Convert.ToInt16(SDT.Rows[0]["Course_Mode_ID"]);
                SE.Semester_Year = Convert.ToInt16(SDT.Rows[0]["Semester_Year"]);

            }
                return SE;
        }

        public StudentDelete Get_Std_Record_For_Delete(int Student_ID)
        {
            String QRY = @"SELECT SD.Student_ID, SD.Enrollment_Number, SD.Student_Name, SD.Father_Name, Case When SD.Gender IS NULL Then '-' Else G.Gender END As Gender, 
DATE_FORMAT(SD.Date_of_Birth, '%d-%m-%Y') As Date_of_Birth, SD.Aadhar_Number, DATE_FORMAT(SD.Date_of_Admission, '%d-%m-%Y') As Date_of_Admission,
SD.Mobile_Number_Stu, SD.Mobile_Number_Father, SD.Email_ID, SD.Address,UC.Univ_Course_Name, USC.Univ_subCourse_Name, CM.Course_Mode, SD.Semester_Year, 
SD.Update_DateTime FROM Student_details SD INNER JOIN university_course UC ON SD.Univ_Course_ID = UC.Univ_Course_ID INNER JOIN university_sub_course USC 
ON SD.Univ_subCourse_ID = USC.Univ_subCourse_ID LEFT JOIN Gender G ON SD.Gender = G.Gender_Code INNER JOIN master_course_mode CM ON SD.Course_Mode_ID = CM.Course_Mode_ID
WHERE SD.Is_Deleted = 0 AND SD.Student_ID = @Student_ID";
            StudentDelete SD = new StudentDelete();
            MySqlCommand cmd = new MySqlCommand(QRY);
            cmd.Parameters.AddWithValue("@Student_ID", Student_ID);
            DataTable SDT = Fnc.GetDataTable(cmd);
            if (SDT.Rows.Count > 0)
            {
                SD.Enrollment_Number = SDT.Rows[0]["Enrollment_Number"].ToString();
                SD.Student_Name = SDT.Rows[0]["Student_Name"].ToString();
                SD.Father_Name = SDT.Rows[0]["Father_Name"].ToString();
                SD.Gender = SDT.Rows[0]["Gender"].ToString();
                SD.Date_of_Birth = SDT.Rows[0]["Date_of_Birth"].ToString();
                SD.Aadhar_Number = SDT.Rows[0]["Aadhar_Number"].ToString();
                SD.Date_of_Admission = SDT.Rows[0]["Date_of_Admission"].ToString();
                SD.Mobile_Number_Stu = SDT.Rows[0]["Mobile_Number_Stu"].ToString();
                SD.Mobile_Number_Father = SDT.Rows[0]["Mobile_Number_Father"].ToString();
                SD.Email_ID = SDT.Rows[0]["Email_ID"].ToString();
                SD.Address = SDT.Rows[0]["Address"].ToString();
                SD.Course_Name = SDT.Rows[0]["Univ_Course_Name"].ToString();
                SD.Branch_Name = SDT.Rows[0]["Univ_subCourse_Name"].ToString();
                SD.Course_Mode = SDT.Rows[0]["Course_Mode"].ToString();
                SD.Semester_Year = Convert.ToInt16(SDT.Rows[0]["Semester_Year"]);
                SD.Student_Id = Convert.ToInt32(SDT.Rows[0]["Student_ID"]);
            }
            return SD;
        }

        public University_Course_details CoursesDetailsForDelete(int BranchID, int CourseID, int UniversityID)
        {
            String QRY = "";
            if(BranchID == 0)
            {
                QRY = @"Select Univ_Course_ID, Univ_Course_Name, Univ_Course_Code, Course_Mode_ID, Number_of_Year_Sem, 
CONCAT( TRIM(TRAILING '.0' FROM Number_of_Year_Sem) ,' - ',Case When Course_Mode_ID = 1 Then 'Semesters' ELSE 'Years' END) As CourseDuration ,
'---' As Univ_subCourse_Name, '---' As Univ_subCourse_Code, 0 As Univ_subCourse_ID
From university_course Where Is_Deleted = 0 AND Univ_Course_ID = @Univ_Course_ID";
            }
            else { 
                QRY = @"Select UC.Univ_Course_ID, USC.Univ_subCourse_ID, UC.Univ_Course_Name, UC.Univ_Course_Code, USC.Univ_subCourse_Name, USC.Univ_subCourse_Code, UC.Course_Mode_ID, UC.Number_of_Year_Sem,
CONCAT( TRIM(TRAILING '.0' FROM Number_of_Year_Sem) ,' - ',Case When UC.Course_Mode_ID = 1 Then 'Semesters' ELSE 'Years' END) As CourseDuration 
From university_sub_course USC INNER JOIN university_course UC ON USC.Univ_Course_ID = UC.Univ_Course_ID
Where USC.Is_Deleted = 0 AND USC.Univ_subCourse_ID = @Univ_subCourse_ID";

//                QRY = @"Select UC.Univ_Course_ID, USC.Univ_subCourse_ID, UC.Univ_Course_Name, UC.Univ_Course_Code, USC.Univ_subCourse_Name, USC.Univ_subCourse_Code, UC.Course_Mode_ID, UC.Number_of_Year_Sem,
//CONCAT( TRIM(TRAILING '.0' FROM Number_of_Year_Sem) ,' - ',Case When UC.Course_Mode_ID = 1 Then 'Semesters' ELSE 'Years' END) As CourseDuration 
//From university_sub_course USC INNER JOIN university_course UC ON USC.Univ_Course_ID = UC.Univ_Course_ID
//Where USC.Is_Deleted = 0 AND USC.Univ_subCourse_ID = @Univ_subCourse_ID AND NOT EXISTS (SELECT Univ_subCourse_ID From (SELECT Univ_subCourse_ID 
//FROM student_details Where Univ_subCourse_ID = @Univ_subCourse_ID AND Is_Deleted=0 AND 
//University_ID = @University_ID Group By Univ_subCourse_ID) As SUSC Where SUSC.Univ_subCourse_ID = USC.Univ_subCourse_ID)";
            }


            University_Course_details UCDD = new University_Course_details();
            MySqlCommand cmd = new MySqlCommand(QRY);
            cmd.Parameters.AddWithValue("@Univ_subCourse_ID", BranchID);
            cmd.Parameters.AddWithValue("@Univ_Course_ID", CourseID);
            cmd.Parameters.AddWithValue("@University_ID", UniversityID);
            DataTable SCD = Fnc.GetDataTable(cmd);
            if (SCD.Rows.Count > 0)
            {
                UCDD.Univ_subCourse_ID = Convert.ToInt32(SCD.Rows[0]["Univ_subCourse_ID"]);
                UCDD.CourseName = SCD.Rows[0]["Univ_Course_Name"].ToString();
                UCDD.Univ_Course_Code = SCD.Rows[0]["Univ_Course_Code"].ToString();
                UCDD.SubCourseName = SCD.Rows[0]["Univ_subCourse_Name"].ToString();
                UCDD.Univ_subCourse_Code = SCD.Rows[0]["Univ_subCourse_Code"].ToString();
                UCDD.CourseDuration = SCD.Rows[0]["CourseDuration"].ToString();
                UCDD.CourseMode = Convert.ToInt16(SCD.Rows[0]["Course_Mode_ID"]);
                UCDD.NOYS = Convert.ToInt16(SCD.Rows[0]["Number_of_Year_Sem"]);
                UCDD.Univ_Course_ID = Convert.ToInt16(SCD.Rows[0]["Univ_Course_ID"]);
            }
        return UCDD;
        }
        public DataTable MonthWiseFeesCollection(int University_Id, int Session_ID)
        {
//            String QRY = @"SELECT MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, SUM(Fees_Amount) As Fees_Amt 
//FROM student_fees_collection Where Is_Deleted = 0 AND University_ID = @University_ID AND Session_ID = @Session_ID
//Group By MonthName, MonthNo, YearValue Order By YearValue, MonthNo";
            String QRY = @"SELECT MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, SUM(Fees_Amount) As Fees_Amt 
FROM student_fees_collection Where Is_Deleted = 0 AND University_ID = @University_ID
Group By MonthName, MonthNo, YearValue Order By YearValue, MonthNo";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@University_ID", University_Id);
            CMD.Parameters.AddWithValue("@Session_ID", Session_ID);
            DataTable DT = Fnc.GetDataTable(CMD);
            return DT;
        }

		public DataTable CourseWiseFeesCollection(int University_Id, int Session_ID, int MnNo, int YrNo)
		{
			
//			String QRY = @"Select UC.Univ_Course_Name, USC.Univ_subCourse_Name, USC.Univ_subCourse_ID, FC.NoOfStudents, FC.Fees_AMT From 
//(Select SD.Univ_Course_ID, SD.Univ_subCourse_ID, Count(SFC.Student_Id) As NoOfStudents,  SUM(SFC.Fees_Amount) As Fees_AMT From Student_Details SD INNER JOIN 
//(SELECT Student_Id, SUM(Fees_Amount) As Fees_Amount FROM student_fees_collection Where Is_Deleted = 0 AND University_Id = @University_Id AND Session_ID = @Session_ID AND MONTH(Txn_Date) = @MNo AND YEAR(Txn_Date) = @YNo Group By Student_ID) As SFC ON SD.Student_Id = SFC.Student_Id AND SD.Is_Deleted = 0
//Group By SD.Univ_Course_ID, SD.Univ_subCourse_ID) As FC INNER JOIN University_Course UC ON FC.Univ_Course_ID = UC.Univ_Course_ID INNER JOIN University_Sub_Course USC ON FC.Univ_subCourse_ID = USC.Univ_subCourse_ID Order By UC.Univ_Course_Name, USC.Univ_subCourse_Name";
            String QRY = @"Select UC.Univ_Course_Name, USC.Univ_subCourse_Name, USC.Univ_subCourse_ID, FC.NoOfStudents, FC.Fees_AMT From 
(Select SD.Univ_Course_ID, SD.Univ_subCourse_ID, Count(SFC.Student_Id) As NoOfStudents,  SUM(SFC.Fees_Amount) As Fees_AMT From Student_Details SD INNER JOIN 
(SELECT Student_Id, SUM(Fees_Amount) As Fees_Amount FROM student_fees_collection Where Is_Deleted = 0 AND University_Id = @University_Id AND MONTH(Txn_Date) = @MNo AND YEAR(Txn_Date) = @YNo Group By Student_ID) As SFC ON SD.Student_Id = SFC.Student_Id AND SD.Is_Deleted = 0
Group By SD.Univ_Course_ID, SD.Univ_subCourse_ID) As FC INNER JOIN University_Course UC ON FC.Univ_Course_ID = UC.Univ_Course_ID INNER JOIN University_Sub_Course USC ON FC.Univ_subCourse_ID = USC.Univ_subCourse_ID Order By UC.Univ_Course_Name, USC.Univ_subCourse_Name";

            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@University_ID", University_Id);
            CMD.Parameters.AddWithValue("@Session_ID", Session_ID);
            CMD.Parameters.AddWithValue("@MNo", MnNo);
            CMD.Parameters.AddWithValue("@YNo", YrNo);
            DataTable DT = Fnc.GetDataTable(CMD);
            return DT;
        }
        public DataTable SubCourseFeesStudentList(int USC_ID, int Session_ID, int MnNo, int YrNo)
		{
//			String QRY = @"SELECT SD.Student_Id, SD.Enrollment_Number, SFC.Txn_ID, SD.Student_Name, SD.Father_Name, SD.Semester_Year, SFC.Fees_Amount,
//DATE_FORMAT(SFC.Txn_Date, '%d-%m-%Y') As RCVDate, DATE_FORMAT(SFC.Txn_Date, '%Y-%m-%d') As DateValue, Cast(SFC.Is_Paid as int) as Is_Paid FROM Student_Details SD INNER JOIN student_fees_collection SFC ON SD.Student_Id = SFC.Student_ID
//WHERE SFC.Is_Deleted = 0 AND SD.Univ_subCourse_ID = @USC_ID AND SD.Is_Deleted = 0 AND SD.University_ID = SFC.University_ID AND SFC.Session_ID = @Session_ID AND MONTH(SFC.Txn_Date) = @MNo AND YEAR(SFC.Txn_Date) = @YNo Order By SD.Student_Name, SD.Father_Name, SD.Enrollment_Number";
            String QRY = @"SELECT SD.Student_Id, SD.Enrollment_Number, SFC.Txn_ID, SD.Student_Name, SD.Father_Name, SD.Semester_Year, SFC.Fees_Amount,
DATE_FORMAT(SFC.Txn_Date, '%d-%m-%Y') As RCVDate, DATE_FORMAT(SFC.Txn_Date, '%Y-%m-%d') As DateValue, Cast(SFC.Is_Paid as int) as Is_Paid FROM Student_Details SD INNER JOIN student_fees_collection SFC ON SD.Student_Id = SFC.Student_ID
WHERE SFC.Is_Deleted = 0 AND SD.Univ_subCourse_ID = @USC_ID AND SD.Is_Deleted = 0 AND SD.University_ID = SFC.University_ID AND MONTH(SFC.Txn_Date) = @MNo AND YEAR(SFC.Txn_Date) = @YNo Order By SD.Student_Name, SD.Father_Name, SD.Enrollment_Number";


            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@USC_ID", USC_ID);
            CMD.Parameters.AddWithValue("@Session_ID", Session_ID);
            CMD.Parameters.AddWithValue("@MNo", MnNo);
            CMD.Parameters.AddWithValue("@YNo", YrNo);
            DataTable DT = Fnc.GetDataTable(CMD);
            return DT;
        }

        public List<student_fees_collection> StudentsListOfSem_Year(int University_Id, int USC_ID, int SY, int SID)
        {            
            List<student_fees_collection> SLFC = new List<student_fees_collection>();
            String QRY = @"SELECT SD.Student_Id, SD.Enrollment_Number, SD.Student_Name, SD.Father_Name, 4 As Session_ID FROM Student_Details SD INNER JOIN 
              (SELECT Student_Id FROM Student_Details Where University_ID = @University_ID AND Session_ID = @Session_ID AND Semester_Year = @SY AND Is_Deleted = 0 AND Is_Passout <> 1
              Union 
              SELECT Student_Id FROM Student_Promote Where University_ID = @University_ID AND Session_ID = @Session_ID AND Semester_Year = @SY AND Is_Deleted = 0 AND Is_Passout <> 1) As PD ON SD.Student_Id = PD.Student_Id
              Where SD.Univ_subCourse_ID = @Univ_subCourse_ID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@University_ID", University_Id);
            CMD.Parameters.AddWithValue("@Univ_subCourse_ID", USC_ID);
            CMD.Parameters.AddWithValue("@Session_ID", SID);
            CMD.Parameters.AddWithValue("@SY", SY);
            DataTable DT = Fnc.GetDataTable(CMD);
            if (DT.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow DR in DT.Rows)
                {
                    SLFC.Add(
                        new student_fees_collection
                        {
                            Count = i,
                            Student_ID = Convert.ToInt32(DR["Student_Id"]),
                            Enrollment_Number = DR["Enrollment_Number"].ToString().Trim(),
                            Student_Name = DR["Student_Name"].ToString().Trim(),
                            Father_Name = DR["Father_Name"].ToString().Trim(),
                            Entry_DateTime = null,
                            Txn_Date = null,
                            University_ID = Convert.ToInt16(University_Id),
                            Session_ID = Convert.ToInt32(DR["Session_ID"]),
                        });
                    i++;
                    
                }
            }
            return SLFC;
        }

        public List<student_fees_collection> StudentsListForPromote(int University_Id, int USC_ID, int SY, int SID)
        {
            List<student_fees_collection> SLFC = new List<student_fees_collection>();
            String QRY = @"Select SD.Student_Id, SD.Enrollment_Number, SD.Student_Name, SD.Father_Name, PD.Session_ID 
From Student_Details SD INNER JOIN 
(SELECT Student_Id, Enrollment_Number, Session_ID FROM Student_Details Where University_ID = @University_ID AND Session_ID = @Session_ID AND Semester_Year = @SY AND Is_Deleted = 0 AND Is_Passout = 0
Union 
SELECT Student_Id, Enrollment_Number, Session_ID FROM Student_Promote Where University_ID = @University_ID AND Session_ID = @Session_ID AND Semester_Year = @SY AND Is_Deleted = 0 AND Is_Passout = 0) As PD 
ON SD.Student_Id = PD.Student_Id 
Where SD.Univ_subCourse_ID = @Univ_subCourse_ID AND SD.Is_Passout NOT IN (1,2) Order By SD.Student_Name, SD.Father_Name";

            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@University_ID", University_Id);
            CMD.Parameters.AddWithValue("@Univ_subCourse_ID", USC_ID);
            CMD.Parameters.AddWithValue("@SY", SY);
            CMD.Parameters.AddWithValue("@Session_ID", SID);
            DataTable DT = Fnc.GetDataTable(CMD);
            if (DT.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow DR in DT.Rows)
                {
                    SLFC.Add(
                        new student_fees_collection
                        {
                            Count = i,
                            Student_ID = Convert.ToInt32(DR["Student_Id"]),
                            Enrollment_Number = DR["Enrollment_Number"].ToString().Trim(),
                            Student_Name = DR["Student_Name"].ToString().Trim(),
                            Father_Name = DR["Father_Name"].ToString().Trim(),
                            Entry_DateTime = null,
                            Txn_Date = null,
                            University_ID = Convert.ToInt16(University_Id),
                            Session_ID = Convert.ToInt32(DR["Session_ID"]),
                        });
                    i++;

                }
            }
            return SLFC;
        }


        public List<StudentList> Get_Student_List(int university_id)
		{
			List<StudentList> ListNotify = new List<StudentList>();
			string Query_UserName = @"SELECT st.Student_Id, st.Student_Name, st.Father_Name, 
                                        st.Date_of_Birth, st.Aadhar_Number, st.Enrollment_Number,
                                        st.Date_of_Admission, st.Mobile_Number_Stu, st.Mobile_Number_Father, 
                                        st.Email_ID, st.Address, st.University_ID, uni.University_Name AS university_Name,
                                        uni.Address AS university_Address,uni.Contact_Number AS university_Contact,uni.Email_ID AS university_Email,
                                        uni.Pin_Code AS university_PIN,
                                        st.Univ_Course_ID, unicur.Univ_Course_Name, st.Univ_subCourse_ID,
                                        unisubcur.Univ_subCourse_Name,st.Session_ID, mscsess.Session_Name,mscmode.Course_Mode,
                                        st.Course_Mode_ID ,st.Semester_Year FROM student_details st
                                        INNER JOIN university_course unicur ON unicur.Univ_Course_ID = st.Univ_Course_ID
                                        INNER JOIN university_sub_course unisubcur ON unisubcur.Univ_subCourse_ID = st.Univ_subCourse_ID
                                        INNER JOIN master_academic_session mscsess ON mscsess.Session_ID = st.Session_ID
                                        INNER JOIN master_course_mode mscmode ON mscmode.Course_Mode_ID = st.Course_Mode_ID
                                        INNER JOIN university_details uni ON uni.University_ID = st.University_ID
                                        WHERE st.Is_Deleted= 0 and Is_Passout=0 AND st.University_ID= @UniID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UniID", university_id);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new StudentList
					{
						count = i,
						Student_Name = dr["Student_Name"].ToString().Trim(),
						Father_Name = dr["Father_Name"].ToString().Trim(),
						Date_of_Birth = Convert.ToDateTime(dr["Date_of_Birth"].ToString().Trim()).ToString("dd-MM-yyyy"),
						Aadhar_Number = dr["Aadhar_Number"].ToString().Trim(),
						Enrollment_Number = dr["Enrollment_Number"].ToString().Trim(),
						Date_of_Admission = Convert.ToDateTime(dr["Date_of_Admission"].ToString().Trim()).ToString("dd-MM-yyyy"),
						Mobile_Number_Stu = dr["Mobile_Number_Stu"].ToString().Trim(),
						Mobile_Number_Father = dr["Mobile_Number_Father"].ToString().Trim(),
						Email_ID = dr["Email_ID"].ToString().Trim(),
						Address = dr["Address"].ToString().Trim(),
						university_Name = dr["university_Name"].ToString().Trim(),
						university_Address = dr["university_Address"].ToString().Trim(),
						university_Contact = dr["university_Contact"].ToString().Trim(),
						university_Email = dr["university_Email"].ToString().Trim(),
						university_PIN = dr["university_PIN"].ToString().Trim(),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
						Session_Name = dr["Session_Name"].ToString().Trim(),
						Course_Mode = dr["Course_Mode"].ToString().Trim(),
						Semester_Year = dr["Semester_Year"].ToString().Trim(),
					});
					i++;
				}
			}
			return ListNotify;
		}


        public List<StudentList> Get_Students_Records(int University_ID, int Session_ID)
        {
            List<StudentList> StudentsRecords = new List<StudentList>();
            //String QRY = @"SELECT SD.Student_Id, SD.Enrollment_Number, SD.Student_Name, SD.Father_Name, Case When G.Gender IS NULL Then '-' Else G.Gender END As Gender,
            //    SD.Date_of_Birth, SD.Aadhar_Number,  SD.Date_of_Admission, SD.Mobile_Number_Stu, Case When SD.Mobile_Number_Father IS NULL then '' Else SD.Mobile_Number_Father END As Mobile_Number_Father, SD.Email_ID, SD.Address, UC.Univ_Course_Name, 
            //    USC.Univ_subCourse_Name, CONCAT(Case When SD.Course_Mode_ID = 1 Then 'Sem' Else 'Year' END, '-', SD.Semester_Year) As Semester_Year, Case When SD.Submitted_via_Spreadsheet = 1 Then 'Excel Sheet' Else 'Online' END As SubmittedBy 
            //    FROM Student_details SD INNER JOIN university_course UC ON SD.Univ_Course_ID = UC.Univ_Course_ID
            //    INNER JOIN university_sub_course USC ON SD.Univ_subCourse_ID = USC.Univ_subCourse_ID LEFT JOIN Gender G ON SD.Gender = G.Gender_Code
            //    WHERE SD.Is_Deleted = 0 AND SD.University_ID = @University_ID AND Session_ID = @Session_ID Order By UC.Univ_Course_ID, USC.Univ_subCourse_ID, SD.Student_Name, SD.Father_Name";
            String QRY = @"SELECT SD.Student_Id, SD.Enrollment_Number, SD.Student_Name, SD.Father_Name, Case When G.Gender IS NULL Then '-' Else G.Gender END As Gender,SD.Date_of_Birth, SD.Aadhar_Number, 
            SD.Date_of_Admission, SD.Mobile_Number_Stu, Case When SD.Mobile_Number_Father IS NULL then '' Else SD.Mobile_Number_Father END As Mobile_Number_Father, SD.Email_ID, SD.Address, 
            UC.Univ_Course_Name,  USC.Univ_subCourse_Name, Case When SD.Is_Passout = 1 Then 'Passout' When SD.Is_Passout = 2 Then CONCAT('Dropout From\n',Case When SD.Course_Mode_ID = 1 Then 'Sem' Else 'Year' END, '-', PD.Semester_Year) Else CONCAT(Case When SD.Course_Mode_ID = 1 Then 'Sem' Else 'Year' END, '-', PD.Semester_Year) END As Semester_Year, Case When SD.Submitted_via_Spreadsheet = 1 Then 'Excel Sheet' Else 'Online' END As SubmittedBy 
             FROM Student_details SD INNER JOIN
             (SELECT Student_Id, Semester_Year FROM Student_Details Where University_ID = @University_ID AND Session_ID = @Session_ID AND Is_Deleted = 0
             Union 
             SELECT Student_Id, Semester_Year FROM Student_Promote Where University_ID = @University_ID AND Session_ID = @Session_ID AND Is_Deleted = 0) As PD ON SD.Student_Id = PD.Student_Id
             INNER JOIN university_course UC ON SD.Univ_Course_ID = UC.Univ_Course_ID
             INNER JOIN university_sub_course USC ON SD.Univ_subCourse_ID = USC.Univ_subCourse_ID LEFT JOIN Gender G ON SD.Gender = G.Gender_Code
             Order By UC.Univ_Course_ID, USC.Univ_subCourse_ID, SD.Student_Name, SD.Father_Name";
            MySqlCommand cmd = new MySqlCommand(QRY);
            cmd.Parameters.AddWithValue("@University_ID", University_ID);
            cmd.Parameters.AddWithValue("@Session_ID", Session_ID);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    StudentsRecords.Add(
                    new StudentList
                    {
                        count = i,
                        Student_Name = dr["Student_Name"].ToString().Trim(),
                        Father_Name = dr["Father_Name"].ToString().Trim(),
                        Gender = dr["Gender"].ToString().Trim(),
                        Date_of_Birth = Convert.ToDateTime(dr["Date_of_Birth"].ToString().Trim()).ToString("dd-MM-yyyy"),
                        Aadhar_Number = dr["Aadhar_Number"].ToString().Trim(),
                        Enrollment_Number = dr["Enrollment_Number"].ToString().Trim(),
                        Date_of_Admission = Convert.ToDateTime(dr["Date_of_Admission"].ToString().Trim()).ToString("dd-MM-yyyy"),
                        Mobile_Number_Stu = dr["Mobile_Number_Stu"].ToString().Trim(),
                        Mobile_Number_Father = dr["Mobile_Number_Father"].ToString().Trim(),
                        Email_ID = dr["Email_ID"].ToString().Trim(),
                        Address = dr["Address"].ToString().Trim(),                      
                        Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
                        Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),                                             
                        Semester_Year = dr["Semester_Year"].ToString().Trim(),
                        Student_ID = Convert.ToInt32(dr["Student_Id"]),
                        SubmittedBy = dr["SubmittedBy"].ToString().Trim(),
                    });
                    i++;
                }
            }
            return StudentsRecords;
        }

        public List<University_Course_details> Get_UNV_Courses(int University_ID)
        {
            List<University_Course_details> UnvCourses = new List<University_Course_details>();
            String QRY = @"SELECT UC.Univ_Course_ID, UC.Univ_Course_Name,UC.Univ_Course_Code, 
CONCAT(TRIM(TRAILING '.0' FROM Number_of_Year_Sem), ' - ', Case When UC.Course_Mode_ID = 1 Then 'Semesters' ELSE 'Years' END) As CourseDuration, 
Case When USC.Univ_subCourse_ID IS NULL Then 0 Else USC.Univ_subCourse_ID END As Univ_subCourse_ID,
Case When USC.Univ_subCourse_Name IS NULL Then '---' Else USC.Univ_subCourse_Name END As Univ_subCourse_Name, 
Case When USC.Univ_subCourse_Code IS NULL Then '---' Else USC.Univ_subCourse_Code END As Univ_subCourse_Code, 
Case When SCS.NOS IS NULL Then 0 Else SCS.NOS END As No_Of_Student,
Case When SCC.NoOfSC IS NULL Then 1 Else SCC.NoOfSC END As NoOfSC
FROM University_Course UC LEFT JOIN University_Sub_Course USC ON UC.Univ_Course_ID = USC.Univ_Course_ID AND USC.Is_Deleted = 0
LEFT JOIN (SELECT Univ_subCourse_ID, Count(Student_Id) As NOS FROM student_details 
WHERE Is_Deleted = 0 AND University_ID = @University_ID Group By Univ_subCourse_ID) As SCS ON USC.Univ_subCourse_ID = SCS.Univ_subCourse_ID
LEFT JOIN (SELECT Univ_Course_ID, Count(Univ_subCourse_ID) As NoOfSC FROM university_sub_course Where Is_Deleted = 0 AND University_ID = @University_ID 
Group By Univ_Course_ID) As SCC ON UC.Univ_Course_ID = SCC.Univ_Course_ID
Where UC.University_ID = @University_ID AND UC.Is_Deleted = 0 Order By UC.Univ_Course_ID";
            MySqlCommand cmd = new MySqlCommand(QRY);
            cmd.Parameters.AddWithValue("@University_ID", University_ID);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1,  CourseID = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    if (CourseID != Convert.ToInt32(dr["Univ_Course_ID"]) && CourseID > 0)
                    {
                        i++;
                    }
                    UnvCourses.Add(
                        new University_Course_details
                        {
                            count = i,
                            Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"]),                                                     
                            CourseName = dr["Univ_Course_Name"].ToString(),
                            Univ_Course_Code = dr["Univ_Course_Code"].ToString(),
                            CourseDuration = dr["CourseDuration"].ToString(),
                            NoOfBranches = Convert.ToInt16(dr["NoOfSC"]),
                            Univ_subCourse_ID = Convert.ToInt32(dr["Univ_subCourse_ID"]),
                            SubCourseName = dr["Univ_subCourse_Name"].ToString(),
                            Univ_subCourse_Code = dr["Univ_subCourse_Code"].ToString(),                            
                            NoOfStudent = Convert.ToInt32(dr["No_Of_Student"])
                        });
                                                                 
                    CourseID = Convert.ToInt32(dr["Univ_Course_ID"]);
                    
                }
            }
            return UnvCourses;
        }

        public List<Uploaded_Student_data_files> Get_Files_List_for_University(int university_id, int Sid)
        {
            List<Uploaded_Student_data_files> USDF_List = new List<Uploaded_Student_data_files>();
            string ListQuery = @"SELECT File_ID, University_ID, File_Name, DATE_FORMAT(Uploaded_DateTime, '%d-%m-%Y %h:%i:%s %p') As Uploaded_DateTime, 
File_path, Records_exported_to_Server, Records_left, Case When Processed = 1 Then 'Yes' Else 'No' END As Processed, 
Case When Remark IS Null then '' Else Remark End As Remark FROM Uploaded_Student_Data WHERE Is_Deleted = 0 AND University_ID = @UID AND Session_ID = @SID";
            MySqlCommand cmd = new MySqlCommand(ListQuery);
            cmd.Parameters.AddWithValue("@UID", university_id);
            cmd.Parameters.AddWithValue("@SID", Sid);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    USDF_List.Add(
                       new Uploaded_Student_data_files                                             
                       {
                           count = i,
                           File_ID = Convert.ToInt32(dr["File_ID"]),
                           University_ID = Convert.ToInt32(dr["University_ID"]),
                           File_Name = dr["File_Name"].ToString().Trim(),
                           Uploaded_DateTime = dr["Uploaded_DateTime"].ToString().Trim(),
                           File_path = dr["File_path"].ToString().Trim(),
                           Processed = dr["Processed"].ToString().Trim(),
                           Records_exported_to_Server = Convert.ToInt32(dr["Records_exported_to_Server"]),
                           Records_left = Convert.ToInt32(dr["Records_left"]),
                           Remark = dr["Remark"].ToString().Trim(),

                       });
                    i++;
                }
            }
             return USDF_List;
        }


        public List<Uploaded_Student_data_files> Fees_Files_for_University(int university_id, int Sid)
        {
            List<Uploaded_Student_data_files> USDF_List = new List<Uploaded_Student_data_files>();
            string ListQuery = @"SELECT File_ID, University_ID, File_Name, DATE_FORMAT(Uploaded_DateTime, '%d-%m-%Y %h:%i:%s %p') As Uploaded_DateTime, 
File_path, Records_exported_to_Server, Records_left, Case When Processed = 1 Then 'Yes' Else 'No' END As Processed, 
Case When Remark IS Null then '' Else Remark End As Remark FROM uploaded_fees_data WHERE Is_Deleted = 0 AND University_ID = @UID AND Session_ID = @SID";
            MySqlCommand cmd = new MySqlCommand(ListQuery);
            cmd.Parameters.AddWithValue("@UID", university_id);
            cmd.Parameters.AddWithValue("@SID", Sid);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    USDF_List.Add(
                       new Uploaded_Student_data_files
                       {
                           count = i,
                           File_ID = Convert.ToInt32(dr["File_ID"]),
                           University_ID = Convert.ToInt32(dr["University_ID"]),
                           File_Name = dr["File_Name"].ToString().Trim(),
                           Uploaded_DateTime = dr["Uploaded_DateTime"].ToString().Trim(),
                           File_path = dr["File_path"].ToString().Trim(),
                           Processed = dr["Processed"].ToString().Trim(),
                           Records_exported_to_Server = Convert.ToInt32(dr["Records_exported_to_Server"]),
                           Records_left = Convert.ToInt32(dr["Records_left"]),
                           Remark = dr["Remark"].ToString().Trim(),

                       });
                    i++;
                }
            }
            return USDF_List;
        }

        public List<Uploaded_data_files> PU_Uploaded_Fees_Files(int Sid)
        {
            List<Uploaded_data_files> USDF_List = new List<Uploaded_data_files>();
            string ListQuery = @"SELECT UD.University_Name, File_ID, UFD.University_ID, File_Name, DATE_FORMAT(Uploaded_DateTime, '%d-%m-%Y %h:%i:%s %p') As Uploaded_DateTime, 
File_path  FROM uploaded_fees_data UFD INNER JOIN university_details UD ON UFD.University_ID = UD.University_ID WHERE UFD.Is_Deleted = 0 AND Processed = 0 AND Session_ID = @SID";
            MySqlCommand cmd = new MySqlCommand(ListQuery);            
            cmd.Parameters.AddWithValue("@SID", Sid);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    USDF_List.Add(
                       new Uploaded_data_files
                       {
                           count = i,
                           University_Name = dr["University_Name"].ToString().Trim(),
                           File_ID = Convert.ToInt32(dr["File_ID"]),
                           University_ID = Convert.ToInt32(dr["University_ID"]),
                           File_Name = dr["File_Name"].ToString().Trim(),
                           Uploaded_DateTime = dr["Uploaded_DateTime"].ToString().Trim(),
                           File_path = dr["File_path"].ToString().Trim(),
                       });
                    i++;
                }
            }
            return USDF_List;
        }



        public List<Uploaded_Student_data_files> Degree_Files_for_University(int university_id)
        {
            List<Uploaded_Student_data_files> USDF_List = new List<Uploaded_Student_data_files>();
            string ListQuery = @"SELECT File_ID, University_ID, File_Name, DATE_FORMAT(Uploaded_DateTime, '%d-%m-%Y %h:%i:%s %p') As Uploaded_DateTime, 
File_path, Records_exported_to_Server, Records_left, Case When Processed = 1 Then 'Yes' Else 'No' END As Processed, 
Case When Remark IS Null then '' Else Remark End As Remark FROM uploaded_degree_data WHERE Is_Deleted = 0 AND University_ID = @UID ";
            MySqlCommand cmd = new MySqlCommand(ListQuery);
            cmd.Parameters.AddWithValue("@UID", university_id);            
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    USDF_List.Add(
                       new Uploaded_Student_data_files
                       {
                           count = i,
                           File_ID = Convert.ToInt32(dr["File_ID"]),
                           University_ID = Convert.ToInt32(dr["University_ID"]),
                           File_Name = dr["File_Name"].ToString().Trim(),
                           Uploaded_DateTime = dr["Uploaded_DateTime"].ToString().Trim(),
                           File_path = dr["File_path"].ToString().Trim(),
                           Processed = dr["Processed"].ToString().Trim(),
                           Records_exported_to_Server = Convert.ToInt32(dr["Records_exported_to_Server"]),
                           Records_left = Convert.ToInt32(dr["Records_left"]),
                           Remark = dr["Remark"].ToString().Trim(),

                       });
                    i++;
                }
            }
            return USDF_List;
        }

        public List<Uploaded_Student_data_files> Internship_Files_of_University(int university_id, int Sid)
        {
            List<Uploaded_Student_data_files> USDF_List = new List<Uploaded_Student_data_files>();
            string ListQuery = @"SELECT File_ID, University_ID, File_Name, DATE_FORMAT(Uploaded_DateTime, '%d-%m-%Y %h:%i:%s %p') As Uploaded_DateTime, 
File_path, Records_exported_to_Server, Records_left, Case When Processed = 1 Then 'Yes' Else 'No' END As Processed, 
Case When Remark IS Null then '' Else Remark End As Remark FROM uploaded_Internship_data WHERE Is_Deleted = 0 AND University_ID = @UID AND Session_ID = @SID";
            MySqlCommand cmd = new MySqlCommand(ListQuery);
            cmd.Parameters.AddWithValue("@UID", university_id);
            cmd.Parameters.AddWithValue("@SID", Sid);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    USDF_List.Add(
                       new Uploaded_Student_data_files
                       {
                           count = i,
                           File_ID = Convert.ToInt32(dr["File_ID"]),
                           University_ID = Convert.ToInt32(dr["University_ID"]),
                           File_Name = dr["File_Name"].ToString().Trim(),
                           Uploaded_DateTime = dr["Uploaded_DateTime"].ToString().Trim(),
                           File_path = dr["File_path"].ToString().Trim(),
                           Processed = dr["Processed"].ToString().Trim(),
                           Records_exported_to_Server = Convert.ToInt32(dr["Records_exported_to_Server"]),
                           Records_left = Convert.ToInt32(dr["Records_left"]),
                           Remark = dr["Remark"].ToString().Trim(),

                       });
                    i++;
                }
            }
            return USDF_List;
        }

        public List<Uploaded_Student_data_files> AcademicCalendar_of_University(int university_id, int Sid)
        {
            List<Uploaded_Student_data_files> USDF_List = new List<Uploaded_Student_data_files>();
            string ListQuery = @"SELECT File_ID, University_ID, File_Name, DATE_FORMAT(Uploaded_DateTime, '%d-%m-%Y %h:%i:%s %p') As Uploaded_DateTime, 
File_path, Case When Checked = 1 Then 'Yes' Else 'No' END As Processed, 
Case When Remark IS Null then '' Else Remark End As Remark FROM uploaded_academiccalendar WHERE Is_Deleted = 0 AND University_ID = @UID AND Session_ID = @SID";
            MySqlCommand cmd = new MySqlCommand(ListQuery);
            cmd.Parameters.AddWithValue("@UID", university_id);
            cmd.Parameters.AddWithValue("@SID", Sid);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    USDF_List.Add(
                       new Uploaded_Student_data_files
                       {
                           count = i,
                           File_ID = Convert.ToInt32(dr["File_ID"]),
                           University_ID = Convert.ToInt32(dr["University_ID"]),
                           File_Name = dr["File_Name"].ToString().Trim(),
                           Uploaded_DateTime = dr["Uploaded_DateTime"].ToString().Trim(),
                           File_path = dr["File_path"].ToString().Trim(),
                           Processed = dr["Processed"].ToString().Trim(), 
                           Remark = dr["Remark"].ToString().Trim(),

                       });
                    i++;
                }
            }
            return USDF_List;
        }

        public List<FeesReport> FeesDetailList(int University_ID)
		{
			List<FeesReport> FR = new List<FeesReport>();
			String QRY = @"SELECT Txn_ID, No_Of_Students, Fees_Amt, Received_Date  
                    FROM fees_collection WHERE University_ID = @University_ID order by Received_Date";
			MySqlCommand CMD = new MySqlCommand(QRY);
			CMD.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(CMD);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					FR.Add(
					   new FeesReport
					   {
						   count = i,
						   txn_id = Convert.ToInt32(dr["Txn_ID"]),
						   NoOfStudents = Convert.ToInt32(dr["No_Of_Students"]),
						   FeesAmount = Convert.ToInt32(dr["Fees_Amt"]),
						   //ReceivedDate = dr["Received_Date"].ToString(),
						   ReceivedDate = Convert.ToDateTime(dr["Received_Date"].ToString().Trim()).ToString("dd-MM-yyyy"),
					   });
					i++;
				}
			}
			return FR;
		}
//		public List<StudentFeesDetails> Get_Stu_Fees_List(int University_ID)
//        {
//            List<StudentFeesDetails> SFL = new List<StudentFeesDetails>();
//            String QRY = @"SELECT SD.Student_Name, SD.Enrollment_Number, CONCAT(Case When SD.Course_Mode_ID = 1 Then 'Sem' Else 'Year' END,'-',SD.Semester_Year) As Pursuing_YS,
//Bill_Number, Txn_Date, Narration, Amount_Credit FROM University_Transaction UT INNER JOIN Student_Details SD
//ON UT.Student_ID = SD.Student_Id Where UT.University_ID =  @UID
//Order By SD.Student_Name, SD.Enrollment_Number";
//            MySqlCommand CMD = new MySqlCommand(QRY);
//            CMD.Parameters.AddWithValue("@UID", University_ID);
//            DataTable dt = Fnc.GetDataTable(CMD);
//            if (dt.Rows.Count > 0)
//            {
//                int i = 1;
//                foreach (DataRow dr in dt.Rows)
//                {
//                    SFL.Add(
//                       new StudentFeesDetails
//                       {
//                           count = i,
//                           Student_Name = dr["Student_Name"].ToString().Trim(),
//                           Enrollment_Number = dr["Enrollment_Number"].ToString().Trim(),
//                           PYS = dr["Pursuing_YS"].ToString().Trim(),
//                           Bill_Number = dr["Bill_Number"].ToString().Trim(),
//                           Bill_Date = Convert.ToDateTime(dr["Txn_Date"]).ToString("dd-MM-yyyy"),
//                           Narration = dr["Narration"].ToString().Trim(),
//                           Amount = Convert.ToDecimal(dr["Amount_Credit"]),

                         
//                       });
//                    i++;
//                }
//            }
            
//            return SFL;
//        }
        public List<StudentViewList> Get_Student_List_for_University(int university_id)
		{
			List<StudentViewList> ListNotify = new List<StudentViewList>();
			string Query_UserName = @"SELECT st.Student_Id, st.Student_Name, st.Father_Name, 
                                        st.Date_of_Birth, st.Aadhar_Number, st.Enrollment_Number,
                                        st.Date_of_Admission, st.Mobile_Number_Stu, st.Mobile_Number_Father, 
                                        st.Email_ID, st.Address, st.University_ID, uni.University_Name AS university_Name,
                                        uni.Address AS university_Address,uni.Contact_Number AS university_Contact,uni.Email_ID AS university_Email,
                                        uni.Pin_Code AS university_PIN,
                                        st.Univ_Course_ID, unicur.Univ_Course_Name, st.Univ_subCourse_ID,
                                        unisubcur.Univ_subCourse_Name,st.Session_ID, mscsess.Session_Name,mscmode.Course_Mode,
                                        st.Course_Mode_ID ,st.Semester_Year FROM student_details st
                                        INNER JOIN university_course unicur ON unicur.Univ_Course_ID = st.Univ_Course_ID
                                        INNER JOIN university_sub_course unisubcur ON unisubcur.Univ_subCourse_ID = st.Univ_subCourse_ID
                                        INNER JOIN master_academic_session mscsess ON mscsess.Session_ID = st.Session_ID
                                        INNER JOIN master_course_mode mscmode ON mscmode.Course_Mode_ID = st.Course_Mode_ID
                                        INNER JOIN university_details uni ON uni.University_ID = st.University_ID
                                        WHERE st.Is_Deleted= 0 and Is_Passout=0 AND st.University_ID= @UniID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UniID", university_id);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new StudentViewList
					{
						count = i,
						Student_Name = dr["Student_Name"].ToString().Trim(),
						Father_Name = dr["Father_Name"].ToString().Trim(),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
					});
					i++;
				}
			}
			return ListNotify;
		}

		public List<StudentList> GetAllUniversityStudentList()
		{
			List<StudentList> ListNotify = new List<StudentList>();
			string Query_UserName = @"SELECT st.Student_Id, st.Student_Name, st.Father_Name, 
                                        st.Date_of_Birth, st.Aadhar_Number, st.Enrollment_Number,
                                        st.Date_of_Admission, st.Mobile_Number_Stu, st.Mobile_Number_Father, 
                                        st.Email_ID, st.Address, st.University_ID, uni.University_Name AS university_Name,
                                        uni.Address AS university_Address,uni.Contact_Number AS university_Contact,uni.Email_ID AS university_Email,
                                        uni.Pin_Code AS university_PIN,
                                        st.Univ_Course_ID, unicur.Univ_Course_Name, st.Univ_subCourse_ID,
                                        unisubcur.Univ_subCourse_Name,st.Session_ID, mscsess.Session_Name,mscmode.Course_Mode,
                                        st.Course_Mode_ID ,st.Semester_Year FROM student_details st
                                        INNER JOIN university_course unicur ON unicur.Univ_Course_ID = st.Univ_Course_ID
                                        INNER JOIN university_sub_course unisubcur ON unisubcur.Univ_subCourse_ID = st.Univ_subCourse_ID
                                        INNER JOIN master_academic_session mscsess ON mscsess.Session_ID = st.Session_ID
                                        INNER JOIN master_course_mode mscmode ON mscmode.Course_Mode_ID = st.Course_Mode_ID
                                        INNER JOIN university_details uni ON uni.University_ID = st.University_ID
                                        WHERE st.Is_Deleted= 0 and Is_Passout=0;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new StudentList
					{
						count = i,
						Student_Name = dr["Student_Name"].ToString().Trim(),
						Father_Name = dr["Father_Name"].ToString().Trim(),
						Date_of_Birth = Convert.ToDateTime(dr["Date_of_Birth"].ToString().Trim()).ToString("dd-MM-yyyy"),
						Aadhar_Number = dr["Aadhar_Number"].ToString().Trim(),
						Enrollment_Number = dr["Enrollment_Number"].ToString().Trim(),
						Date_of_Admission = Convert.ToDateTime(dr["Date_of_Admission"].ToString().Trim()).ToString("dd-MM-yyyy"),
						Mobile_Number_Stu = dr["Mobile_Number_Stu"].ToString().Trim(),
						Mobile_Number_Father = dr["Mobile_Number_Father"].ToString().Trim(),
						Email_ID = dr["Email_ID"].ToString().Trim(),
						Address = dr["Address"].ToString().Trim(),
						university_Name = dr["university_Name"].ToString().Trim(),
						university_Address = dr["university_Address"].ToString().Trim(),
						university_Contact = dr["university_Contact"].ToString().Trim(),
						university_Email = dr["university_Email"].ToString().Trim(),
						university_PIN = dr["university_PIN"].ToString().Trim(),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
						Session_Name = dr["Session_Name"].ToString().Trim(),
						Course_Mode = dr["Course_Mode"].ToString().Trim(),
						Semester_Year = dr["Semester_Year"].ToString().Trim(),
					});
					i++;
				}
			}
			return ListNotify;
		}

		public List<UniversityShow> GetUniversityShowHome()
		{
			List<UniversityShow> ListNotify = new List<UniversityShow>();
			string Query = @"SELECT University_ID, University_Name, Contact_Number, Email_ID, Address, Pin_Code, 
                                    Website_URL, Univsersity_Logo, University_Details, Establishment_Year, Registration_Number 
                                        FROM university_details WHERE Is_Deleted=0;";
			MySqlCommand cmd = new MySqlCommand(Query);

			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new UniversityShow
					{
						count = i,
						University_ID = dr["University_ID"].ToString().Trim(),
						University_Name = dr["University_Name"].ToString().Trim(),
						Contact_Number = dr["Contact_Number"].ToString().Trim(),
						Email_ID = dr["Email_ID"].ToString().Trim(),
						Address = dr["Address"].ToString().Trim(),
						Pin_Code = dr["Pin_Code"].ToString().Trim(),
						Website_URL = dr["Website_URL"].ToString().Trim(),
						Univsersity_Logo = dr["Univsersity_Logo"].ToString().Trim(),
						University_Details = dr["University_Details"].ToString().Trim(),
						Establishment_Year = dr["Establishment_Year"].ToString().Trim(),
						Registration_Number = dr["Registration_Number"].ToString().Trim(),
					});
					i++;
				}
			}
			return ListNotify;
		}

        public List<UniversityWorkDetail> GetUniversityWorkStatus()
        {
            List<UniversityWorkDetail> WorkStatus = new List<UniversityWorkDetail>();
            String QRY = @"Select UD.University_ID, UD.University_Name, Case When UC.NoOfCourses IS NULL Then 0 Else UC.NoOfCourses END As NoOfCourses, 
Case When USC.NoOfBranches IS NULL Then 0 Else USC.NoOfBranches END As NoOfBranches,
Case When SD.NoOfStudent IS NULL Then 0 Else SD.NoOfStudent END As NoOfTotalStudents
From University_Details UD LEFT JOIN
(SELECT University_ID, Count(Univ_Course_ID) As NoOfCourses FROM university_course Where University_ID > 0 AND Is_Deleted = 0 Group By University_ID) As UC ON UD.University_ID = UC.University_ID LEFT JOIN 
(SELECT University_ID, Count(Univ_subCourse_ID) As NoOfBranches FROM University_Sub_Course Where University_ID > 0 AND Is_Deleted = 0 Group By University_ID) As USC ON UD.University_ID = USC.University_ID LEFT JOIN
(SELECT University_ID, count(Student_Id) As NoOfStudent FROM student_details WHERE University_ID > 0 AND Is_Deleted = 0 Group By University_ID) As SD ON UD.University_ID = SD.University_ID
Where UD.University_ID > 0";
            MySqlCommand CMD = new MySqlCommand(QRY);
            DataTable DT = Fnc.GetDataTable(CMD);
            if (DT.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow DR in DT.Rows)
                {
                    WorkStatus.Add(
                    new UniversityWorkDetail
                    {
                        count = i,                        
                        UniversityName = DR["University_Name"].ToString().Trim(),
                        NoOfCourses = Convert.ToInt32(DR["NoOfCourses"]),
                        NoOfBranches = Convert.ToInt32(DR["NoOfBranches"]),
                        NoOfStudents = Convert.ToInt32(DR["NoOfTotalStudents"]),
                        //UniversityProfileUpdation = DR["ProfileStatus"].ToString().Trim(),
                       // NoOfOfficeBearers = Convert.ToInt32(DR["NoOfOfficeBearers"]),
                        //FeesEnteredBranches = Convert.ToInt32(DR["NoOfSubCoursesOfFees"]),
                    });
                    i++;
                }
            }
            return WorkStatus;
        }
        public List<UniversityFrontShow> UniversityFrontShow()
		{
			List<UniversityFrontShow> ListNotify = new List<UniversityFrontShow>();
			string Query = @"SELECT University_ID, University_Name, Address, Pin_Code, 
                                    Univsersity_Logo, Establishment_Year, Registration_Number 
                                        FROM university_details WHERE Is_Deleted=0 AND University_ID > 0";
			MySqlCommand cmd = new MySqlCommand(Query);

			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new UniversityFrontShow
					{
						University_ID = dr["University_ID"].ToString().Trim(),
						University_Name = dr["University_Name"].ToString().Trim(),
						Address = dr["Address"].ToString().Trim(),
						Pin_Code = dr["Pin_Code"].ToString().Trim(),
						Univsersity_Logo = dr["Univsersity_Logo"].ToString().Trim(),
						Establishment_Year = dr["Establishment_Year"].ToString().Trim(),
					});
					i++;
				}
			}
			return ListNotify;
		}

        
		public List<SelectListItem> PopulateMasterRoles()
		{
			List<SelectListItem> ListMasterRole = new List<SelectListItem>();
			string Query_UserName = "SELECT Role_ID, Role FROM master_role";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{

				foreach (DataRow dr in dt.Rows)
				{
					ListMasterRole.Add(new SelectListItem
					{
						Value = dr["Role_ID"].ToString(),
						Text = dr["Role"].ToString()

					});
				}
			}

			return ListMasterRole;
		}

		public bool AddUser_Insert(AddUsers user)
		{
			string Query = "INSERT INTO login_users(Name, Designation, UserName, Password, Email_ID, Mobile_Number, Role_ID,University_ID,Is_Active,Entry_By) VALUES (@Name, @Designation, @UserName,@Password, @Email_ID, @Mobile_Number, @Role_ID,@University_ID,@Is_Active,@Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			String pass = "abcdefghijklmnopqrstuvwxyz123456789";
			Random r = new Random();
			char[] mypass = new char[8];
			for (int i = 0; i < 8; i++)
			{
				mypass[i] = pass[(int)(35 * r.NextDouble())];
			}
			string password = new string(mypass);
			string Is_Active = "1";
			string Role_ID = "2";
			string encrptpwd = Fnc.GetMd5HashWithMySecurityAlgo(password);
			string username = "UN" + System.DateTime.Now.ToString("yyMMdd") + Get_Max_Login_ID();//user.Name.Replace(" ", "").Substring(0, 5);
			user.Password = password;
			user.UserName = username;

			cmd.Parameters.AddWithValue("@Name", user.Name);
			cmd.Parameters.AddWithValue("@Designation", user.Designation);
			cmd.Parameters.AddWithValue("@UserName", username);
			cmd.Parameters.AddWithValue("@Password", encrptpwd);
			cmd.Parameters.AddWithValue("@Mobile_Number", user.Mobile_Number);
			cmd.Parameters.AddWithValue("@Email_ID", user.Email_ID);
			cmd.Parameters.AddWithValue("@Role_ID", Role_ID);
			cmd.Parameters.AddWithValue("@University_ID", user.University_ID);
			cmd.Parameters.AddWithValue("@Is_Active", Is_Active);
			cmd.Parameters.AddWithValue("@Entry_By", user.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public int Get_Max_Login_ID()
		{
			string Query = "SELECT max(Login_ID) FROM login_users WHERE 1;";
			MySqlCommand cmd = new MySqlCommand(Query);
			int val = Convert.ToInt32(Fnc.GetDataTable(cmd).Rows[0][0].ToString()) + 1;
			return val;
		}

		public bool AddMasterUniversity_Insert(MasterUniversity uni)
		{
			string Query = "INSERT INTO university_details(University_Name,Entry_By) VALUES(@University_Name,@Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_Name", uni.University_Name);
			cmd.Parameters.AddWithValue("@Entry_By", uni.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<UsersShow> Get_University_Users()
		{
			List<UsersShow> ListSessions = new List<UsersShow>();
			string Query_UserName = @"SELECT lu.Name, lu.Designation, lu.UserName,lu.Password, lu.Email_ID, lu.Mobile_Number, lu.Role_ID,mr.Role
                                     ,lu.University_ID,ud.University_Name FROM login_users lu
                                     INNER JOIN master_role mr ON mr.Role_ID = lu.Role_ID
                                    INNER JOIN university_details ud ON ud.University_ID=lu.University_ID
                                    Where lu.Is_Active=1 AND lu.Role_ID=2";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListSessions.Add(
					new UsersShow
					{
						Count = i,
						University_ID = dr["University_Name"].ToString().Trim(),
						Name = dr["Name"].ToString().Trim(),
						Designation = dr["Designation"].ToString().Trim(),
						Email_ID = dr["Email_ID"].ToString().Trim(),
						Mobile_Number = dr["Mobile_Number"].ToString().Trim(),
						Role_ID = dr["Role"].ToString().Trim(),
						UserName = dr["UserName"].ToString().Trim(),
						Password = dr["Password"].ToString().Trim(),
					});
					i++;
				}
			}

			return ListSessions;
		}

		public bool Commission_Address(CommissionAddress ads)
		{
			string Query = "INSERT INTO address(Address,Address_Hi,Contact_No, Email_ID) VALUES(@Address,@Address_Hi, @Contact_No, @Email_ID);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Address", ads.Addressss);
			cmd.Parameters.AddWithValue("@Address_Hi", ads.Address_Hi);
			cmd.Parameters.AddWithValue("@Contact_No", ads.Contact_No);
			cmd.Parameters.AddWithValue("@Email_ID", ads.Email_ID);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public CommissionAddressUpdate GetCommissionAddress()
		{
			CommissionAddressUpdate commm = new CommissionAddressUpdate();
			string Query = @"SELECT Address,Address_Hi,Contact_No, Email_ID FROM address;";
			MySqlCommand cmd = new MySqlCommand(Query);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				commm.Addressss = dt.Rows[0]["Address"].ToString();
				commm.Address_Hi = dt.Rows[0]["Address_Hi"].ToString();
				commm.Contact_No = dt.Rows[0]["Contact_No"].ToString();
				commm.Email_ID = dt.Rows[0]["Email_ID"].ToString();
			}
			return commm;
		}

		public UniversityProfile GetUniversityProfile(int University_ID)
		{
			UniversityProfile uniprofile = new UniversityProfile();
			string Query = @"SELECT University_ID, University_Name, Contact_Number, Email_ID, Address, Pin_Code, 
                                    Website_URL, Univsersity_Logo, University_Details, Establishment_Year, Registration_Number 
                                        FROM university_details WHERE University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				uniprofile.University_ID = Convert.ToInt32(dt.Rows[0]["University_ID"].ToString());
				uniprofile.University_Name = dt.Rows[0]["University_Name"].ToString();
				uniprofile.Contact_Number = dt.Rows[0]["Contact_Number"].ToString();
				uniprofile.Email_ID = dt.Rows[0]["Email_ID"].ToString();
				uniprofile.Address = dt.Rows[0]["Address"].ToString();
				uniprofile.Pin_Code = dt.Rows[0]["Pin_Code"].ToString();
				uniprofile.Website_URL = dt.Rows[0]["Website_URL"].ToString();
				uniprofile.Univsersity_Logo = dt.Rows[0]["Univsersity_Logo"].ToString();
				uniprofile.University_Details = dt.Rows[0]["University_Details"].ToString();
				uniprofile.Establishment_Year = dt.Rows[0]["Establishment_Year"].ToString();
				uniprofile.Registration_Number = dt.Rows[0]["Registration_Number"].ToString();
			}
			return uniprofile;
		}

		public List<UniversityDetailsWithStudents> GetUniversityProfilewithStudentList(int University_ID)
		{
			List<UniversityDetailsWithStudents> uniprofile = new List<UniversityDetailsWithStudents>();
			string Query = @"SELECT University_ID, University_Name, Contact_Number, Email_ID, Address, Pin_Code, 
                                    Website_URL, Univsersity_Logo, University_Details, Establishment_Year, Registration_Number 
                                        FROM university_details WHERE University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				uniprofile.Add(
				  new UniversityDetailsWithStudents
				  {
					  University_ID = Convert.ToInt32(dt.Rows[0]["University_ID"].ToString()),
					  University_Name = dt.Rows[0]["University_Name"].ToString(),
					  Contact_Number = dt.Rows[0]["Contact_Number"].ToString(),
					  Email_ID = dt.Rows[0]["Email_ID"].ToString(),
					  Address = dt.Rows[0]["Address"].ToString(),
					  Pin_Code = dt.Rows[0]["Pin_Code"].ToString(),
					  Website_URL = dt.Rows[0]["Website_URL"].ToString(),
					  Univsersity_Logo = dt.Rows[0]["Univsersity_Logo"].ToString(),
					  University_Details = dt.Rows[0]["University_Details"].ToString(),
					  Establishment_Year = dt.Rows[0]["Establishment_Year"].ToString(),
					  Registration_Number = dt.Rows[0]["Registration_Number"].ToString(),
					  studentViewList = Get_Student_List_for_University(University_ID)
				  });
			}
			return uniprofile;
		}

		public bool UniversityProfileUpdate(UniversityUpdate uni)
		{
			string Query = @"UPDATE university_details SET University_Name=@University_Name,Contact_Number=@Contact_Number,Email_ID=@Email_ID,Address=@Address,Pin_Code=@Pin_Code,
							Website_URL=@Website_URL,Univsersity_Logo=@Univsersity_Logo,University_Details=@University_Details,Establishment_Year=@Establishment_Year,
							Registration_Number=@Registration_Number,Updated_DateTime=Now(),Updated_By=@Updated_By
								WHERE University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_Name", uni.University_Name);
			cmd.Parameters.AddWithValue("@Contact_Number", uni.Contact_Number);
			cmd.Parameters.AddWithValue("@Email_ID", uni.Email_ID);
			cmd.Parameters.AddWithValue("@Address", uni.Address);
			cmd.Parameters.AddWithValue("@Pin_Code", uni.Pin_Code);
			cmd.Parameters.AddWithValue("@Website_URL", uni.Website_URL);
			cmd.Parameters.AddWithValue("@Univsersity_Logo", uni.Univsersity_Logo);
			cmd.Parameters.AddWithValue("@University_Details", uni.University_Details);
			cmd.Parameters.AddWithValue("@Updated_By", uni.Updated_By);
			cmd.Parameters.AddWithValue("@Establishment_Year", uni.Establishment_Year);
			cmd.Parameters.AddWithValue("@Registration_Number", uni.Registration_Number);
			cmd.Parameters.AddWithValue("@University_ID", uni.University_ID);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool Commission_Address_Update(CommissionAddressUpdate ads)
		{
			string Query = "UPDATE address SET Address=@Address,Address_Hi=@Address_Hi,Contact_No=@Contact_No,Email_ID=@Email_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Address", ads.Addressss);
			cmd.Parameters.AddWithValue("@Address_Hi", ads.Address_Hi);
			cmd.Parameters.AddWithValue("@Contact_No", ads.Contact_No);
			cmd.Parameters.AddWithValue("@Email_ID", ads.Email_ID);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		#region University Office Bearer

		public bool University_office_bearer_Insert(University_office_bearer_Insert unibearer)
		{
			string Query = "INSERT INTO university_office_bearer(Name, Designation, Picture, Other, Entry_DateTime, Entry_By,University_ID, Updated_By) VALUES ( @Name, @Designation, @Picture, @Other, now(), @Entry_By,@University_ID, 0);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Name", unibearer.Name);
			cmd.Parameters.AddWithValue("@Designation", unibearer.Designation);
			cmd.Parameters.AddWithValue("@Picture", unibearer.Picture);
			cmd.Parameters.AddWithValue("@Other", unibearer.Other);
			cmd.Parameters.AddWithValue("@Entry_By", unibearer.Entry_By);
			cmd.Parameters.AddWithValue("@University_ID", unibearer.University_ID);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<University_office_bearer> Get_University_office_bearer(int University_ID)
		{
			List<University_office_bearer> ListNotify = new List<University_office_bearer>();
			string Query_UserName = "SELECT office_bearer_ID, Name, Designation, Picture, Other FROM university_office_bearer WHERE Is_Deleted=0 AND University_ID=@University_ID order by office_bearer_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new University_office_bearer
					{
						count = i,
						office_bearer_ID = Convert.ToInt32(dr["office_bearer_ID"].ToString().Trim()),
						Name = dr["Name"].ToString().Trim(),
						Designation = dr["Designation"].ToString().Trim(),
						Picture = dr["Picture"].ToString().Trim(),
						Other = dr["Other"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}

		public bool University_office_bearer_Delete(University_office_bearer_Delete uni)
		{
			string Query = "UPDATE university_office_bearer SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE office_bearer_ID=@office_bearer_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@office_bearer_ID", uni.office_bearer_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", uni.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

        public bool Student_Delete(int Student_ID)
        {
            String QRY = @"UPDATE student_details SET Is_Deleted = 1, Deleted_dateTime = now(), Deleted_By = 2 WHERE Student_Id = @Student_ID;";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@Student_ID", Student_ID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        public bool SubCourseDelete(int SubCourseID)
        {
            String QRY = @"UPDATE university_sub_course SET Is_Deleted = 1, Deleted_DateTime = now(), Deleted_By = 2 WHERE Univ_subCourse_ID = @SubCourseID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@SubCourseID", SubCourseID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }
        public bool CourseDelete(int CourseID)
        {
            String QRY = @"UPDATE university_course SET Is_Deleted = 1, Deleted_DateTime = now(), Deleted_By = 2 WHERE Univ_Course_ID = @CourseID";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@CourseID", CourseID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }
        public bool Student_Cancel(int Student_ID)
        {
            String QRY = @"UPDATE student_details SET Is_Deleted = 2, Deleted_dateTime = now(), Deleted_By = 2 WHERE Student_Id = @Student_ID;";
            MySqlCommand CMD = new MySqlCommand(QRY);
            CMD.Parameters.AddWithValue("@Student_ID", Student_ID);
            bool val = Fnc.CURDCommands(CMD);
            return val;
        }

        #endregion

        #region University Profile Show Home

        public List<UniversityOfficeBearerShowHome> Get_UniversityOfficeBearer(int university_id)
		{
			List<UniversityOfficeBearerShowHome> ListNotify = new List<UniversityOfficeBearerShowHome>();
			string Query_UserName = @"SELECT of.office_bearer_ID, of.University_ID, of.Name, 
                                    of.Designation, of.Picture, of.Other FROM university_office_bearer of
                                    INNER JOIN university_details uni ON uni.University_ID = of.University_ID
                                    WHERE of.Is_Deleted= 0 AND of.University_ID= @UniID order by of.office_bearer_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@UniID", university_id);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new UniversityOfficeBearerShowHome
					{
						count = i,
						Name = dr["Name"].ToString().Trim(),
						Designation = dr["Designation"].ToString().Trim(),
						Picture = dr["Picture"].ToString().Trim(),
						Other = dr["Other"].ToString().Trim(),
					});
					i++;
				}
			}
			return ListNotify;
		}

		public List<UniversityDetailsWithOfficeBearer> UniversityDetailsWithOfficeBearer(int University_ID)
		{
			List<UniversityDetailsWithOfficeBearer> uniprofile = new List<UniversityDetailsWithOfficeBearer>();
			string Query = @"SELECT University_ID, University_Name, Contact_Number, Email_ID, Address, Pin_Code, 
                                    Website_URL, Univsersity_Logo, University_Details, Establishment_Year, Registration_Number 
                                        FROM university_details WHERE University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				uniprofile.Add(
				  new UniversityDetailsWithOfficeBearer
				  {
					  University_ID = Convert.ToInt32(dt.Rows[0]["University_ID"].ToString()),
					  University_Name = dt.Rows[0]["University_Name"].ToString(),
					  Contact_Number = dt.Rows[0]["Contact_Number"].ToString(),
					  Email_ID = dt.Rows[0]["Email_ID"].ToString(),
					  Address = dt.Rows[0]["Address"].ToString(),
					  Pin_Code = dt.Rows[0]["Pin_Code"].ToString(),
					  Website_URL = dt.Rows[0]["Website_URL"].ToString(),
					  Univsersity_Logo = dt.Rows[0]["Univsersity_Logo"].ToString(),
					  University_Details = dt.Rows[0]["University_Details"].ToString(),
					  Establishment_Year = dt.Rows[0]["Establishment_Year"].ToString(),
					  Registration_Number = dt.Rows[0]["Registration_Number"].ToString(),
					  UniversityOfficeBearerShowHome = Get_UniversityOfficeBearer(University_ID)
				  });
			}
			return uniprofile;
		}

		#endregion

		#region University Fees Collection university_transaction

		public List<StudentDetails> Get_Student_List_Of_Univsersity(int university_id, int Session_ID)
		{
			List<StudentDetails> StuDtls = new List<StudentDetails>();
			string Query_UserName = @"SELECT st.Student_Id, st.Student_Name, st.Father_Name, st.Date_of_Birth, st.Enrollment_Number, st.Mobile_Number_Stu, st.Mobile_Number_Father, st.Email_ID, st.University_ID,st.Univ_Course_ID, unicur.Univ_Course_Name, st.Univ_subCourse_ID, unisubcur.Univ_subCourse_Name,st.Session_ID ,mscmode.Course_Mode,
st.Course_Mode_ID ,st.Semester_Year FROM student_details st
INNER JOIN university_course unicur ON unicur.Univ_Course_ID = st.Univ_Course_ID
INNER JOIN university_sub_course unisubcur ON unisubcur.Univ_subCourse_ID = st.Univ_subCourse_ID
INNER JOIN master_course_mode mscmode ON mscmode.Course_Mode_ID = st.Course_Mode_ID
WHERE st.Is_Deleted= 0 and Is_Passout=0 AND st.University_ID= @University_ID AND st.Session_ID=@Session_ID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", university_id);
			cmd.Parameters.AddWithValue("@Session_ID", Session_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					StuDtls.Add(
					new StudentDetails
					{
						count = i,
						Student_Id = Convert.ToInt32(dr["Student_Id"].ToString().Trim()),
						Student_Name = dr["Student_Name"].ToString().Trim(),
						Father_Name = dr["Father_Name"].ToString().Trim(),
						Date_of_Birth = dr["Date_of_Birth"].ToString().Trim(),
						Enrollment_Number = dr["Enrollment_Number"].ToString().Trim(),
						Mobile_Number_Stu = dr["Mobile_Number_Stu"].ToString().Trim(),
						Email_ID = dr["Email_ID"].ToString().Trim(),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
						Course_Mode = dr["Course_Mode"].ToString().Trim(),
						Semester_Year = dr["Semester_Year"].ToString().Trim()
					});
					i++;
				}
			}
			return StuDtls;
		}

		public bool university_transaction_Credit_Insert(FeesCollect feesCollect)
		{
			string Query = "INSERT INTO university_transaction( University_ID, Session_ID, Amount_Credit,Txn_Date, Entry_By, Entry_DateTime, Bill_Number, Student_ID, Narration) VALUES ( @University_ID, @Session_ID, @Amount_Credit, @Txn_Date, @Entry_By, now(), @Bill_Number, @Student_ID, @Narration);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", feesCollect.University_ID);
			cmd.Parameters.AddWithValue("@Session_ID", feesCollect.Session_ID);
			cmd.Parameters.AddWithValue("@Amount_Credit", feesCollect.Amount_Credit);
			cmd.Parameters.AddWithValue("@Txn_Date", feesCollect.Txn_Date);
			cmd.Parameters.AddWithValue("@Bill_Number", feesCollect.Bill_Number);
			cmd.Parameters.AddWithValue("@Student_ID", feesCollect.Student_ID);
			cmd.Parameters.AddWithValue("@Narration", feesCollect.Narration);
			cmd.Parameters.AddWithValue("@Entry_By", feesCollect.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool FeesCollectionSummery(int _Month, int _Year)
		{
			DateTime StartDate = new DateTime(_Year, _Month, 01);
			int _Day = DateTime.DaysInMonth(_Year, _Month);
			DateTime EndDate = new DateTime(_Year, _Month, _Day);

			string Query_UserName = "SELECT DISTINCT University_ID as University_ID FROM university_transaction where date(Entry_DateTime) BETWEEN '" + StartDate.ToString("yyyy-MM-dd") + "' and '" + EndDate.ToString("yyyy-MM-dd") + "';";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string Query_totaltxn = "SELECT Amount_Credit, Student_ID FROM university_transaction where University_ID=1 and Amount_Credit>0 and date(Entry_DateTime) BETWEEN '" + StartDate.ToString("yyyy-MM-dd") + "' and '" + EndDate.ToString("yyyy-MM-dd") + "';";
					MySqlCommand cmd_txn = new MySqlCommand(Query_totaltxn);
					DataTable dt_txn = Fnc.GetDataTable(cmd_txn);
					if (dt_txn.Rows.Count > 0)
					{
						string txnNo = "";
						decimal TotalAmt = Math.Round(dt_txn.AsEnumerable().Sum(row => Convert.ToDecimal(row["Amount_Credit"])));
						decimal PercentAmt = Math.Round((TotalAmt * 1) / 100);
						ayoge_txn Atxn = new ayoge_txn()
						{
							University_ID = Convert.ToInt32(dt.Rows[i]["University_ID"].ToString().Trim()),
							Txn_Number = txnNo,
							txn_Month = _Month,
							txn_Year = _Year,
							Due_Date = new DateTime(_Year, _Month, 15),
							Total_Amount = TotalAmt,
							Percent_Amount = PercentAmt
						};
						int Val = ayoge_transctions_Insert(Atxn);
						if (Val > 0)
						{
							for (int j = 0; j < dt_txn.Rows.Count; j++)
							{
								ayoge_txn_dtls atd = new ayoge_txn_dtls()
								{
									Ayong_Txn_ID = Val,
									Student_ID = Convert.ToInt32(dt_txn.Rows[j]["Student_ID"].ToString()),
									Amount = Convert.ToDecimal(dt_txn.Rows[j]["Amount_Credit"].ToString())
								};
								ayoge_transctions_details_Insert(atd);
							}
						}
					}
				}
			}
			return true;
		}

		public int ayoge_transctions_Insert(ayoge_txn ayoge_Txn)
		{
			string Query = "INSERT INTO ayoge_transctions(Txn_Number,University_ID, Txn_Month, Txn_Year, Total_Amount, Percent_Amount, Due_Date) VALUES (@Txn_Number,@University_ID, @Txn_Month, @Txn_Year, @Total_Amount, @Percent_Amount, @Due_Date);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", ayoge_Txn.University_ID);
			cmd.Parameters.AddWithValue("@Txn_Number", ayoge_Txn.Txn_Number);
			cmd.Parameters.AddWithValue("@Txn_Month", ayoge_Txn.txn_Month);
			cmd.Parameters.AddWithValue("@Txn_Year", ayoge_Txn.txn_Year);
			cmd.Parameters.AddWithValue("@Total_Amount", ayoge_Txn.Total_Amount);
			cmd.Parameters.AddWithValue("@Percent_Amount", ayoge_Txn.Percent_Amount);
			cmd.Parameters.AddWithValue("@Due_Date", ayoge_Txn.Due_Date);
			int val = Fnc.InsertCommands_AndGetting_ID(cmd);
			return val;
		}

		public bool ayoge_transctions_Check(int txn_Month, int txn_Year)
		{
			string Query = "SELECT * FROM ayoge_transctions where Txn_Month=@Txn_Month and Txn_Year=@Txn_Year;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Txn_Month", txn_Month);
			cmd.Parameters.AddWithValue("@Txn_Year", txn_Year);
			bool val = Fnc.GetCheckDataExist(cmd);
			return val;
		}


		public bool ayoge_transctions_details_Insert(ayoge_txn_dtls ayoge_Txn_Dtls)
		{
			string Query = "INSERT INTO ayoge_transctions_details( Ayong_Txn_ID, Student_ID, Amount) VALUES ( @Ayong_Txn_ID, @Student_ID, @Amount);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Ayong_Txn_ID", ayoge_Txn_Dtls.Ayong_Txn_ID);
			cmd.Parameters.AddWithValue("@Student_ID", ayoge_Txn_Dtls.Student_ID);
			cmd.Parameters.AddWithValue("@Amount", ayoge_Txn_Dtls.Amount);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public bool ayoge_transctions_Update(ayoge_transctions_Draft ayoge_Transctions_Draft)
		{
			string Query = "update ayoge_transctions set Is_Paid=1,Txn_Number=@Txn_Number,Draft_Bank=@Draft_Bank,Demand_Draft_No=@Demand_Draft_No,Date_of_issue=@Date_of_issue,Paid_Date=now() where Ayong_Txn_ID=@Ayong_Txn_ID ;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Txn_Number", ayoge_Transctions_Draft.Txn_Number);
			cmd.Parameters.AddWithValue("@Draft_Bank", ayoge_Transctions_Draft.Draft_Bank);
			cmd.Parameters.AddWithValue("@Demand_Draft_No", ayoge_Transctions_Draft.Demand_Draft_No);
			cmd.Parameters.AddWithValue("@Date_of_issue", ayoge_Transctions_Draft.Date_of_issue);
			cmd.Parameters.AddWithValue("@Ayong_Txn_ID", ayoge_Transctions_Draft.Ayong_Txn_ID);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

        public List<PGTxnList> PUTxnDetailonPG(int university_id)
        {
            List<PGTxnList> USDF_List = new List<PGTxnList>();
            string ListQuery = @"SELECT  DATE_FORMAT(STR_TO_DATE(CONCAT('01-', Fees_Month, '-', Fees_Year), '%d-%m-%Y'), '%M %Y') As FeesMonth,  Txn_Number, 
Order_ID, razorpay_payment_id, TS.Txn_Status, One_Percent_Amt, Penal_Interest, Payble_Amt, '1' As Attempts, DATE_FORMAT(RequestDateTime, '%d-%m-%Y %h:%i:%s %p') As Payment_DateTime 
FROM razorpay_txns RP INNER Join razorpay_txn_status TS ON RP.PG_Status_Id = TS.PG_Status_Id Where University_ID = @UID AND RP.PG_Status_Id = 3";
            MySqlCommand cmd = new MySqlCommand(ListQuery);
            cmd.Parameters.AddWithValue("@UID", university_id);           
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    USDF_List.Add(
                       new PGTxnList
                       {
                           count = i,
                           FeesMonth = dr["FeesMonth"].ToString().Trim(),
                           TxnNumber = dr["Txn_Number"].ToString().Trim(),
                           OrderId = dr["Order_ID"].ToString().Trim(),
                           OnePerAmt = dr["One_Percent_Amt"].ToString().Trim(),
                           Penal_Interest = dr["Penal_Interest"].ToString().Trim(),
                           TotalAmount = dr["Payble_Amt"].ToString().Trim(),
                           Attempts = dr["Attempts"].ToString().Trim(),
                           PG_Status = dr["Txn_Status"].ToString().Trim(),
                           TxnDateTime = dr["Payment_DateTime"].ToString().Trim(),
                           //University_ID = Convert.ToInt32(dr["University_ID"]),
                       });
                    i++;
                }
            }
            return USDF_List;
        }



        public List<ayoge_transctions_List> Get_ayoge_transctions_List_Of_Univsersity(int University_ID)
		{
			List<ayoge_transctions_List> aTxnL = new List<ayoge_transctions_List>();
            //string Query_UserName = @"SELECT Ayong_Txn_ID, University_ID, Txn_Month, Txn_Year, Total_Amount, Percent_Amount, Is_Paid,  Due_Date FROM ayoge_transctions WHERE University_ID=@University_ID;";
            //string Query_UserName = @"SELECT Ayong_Txn_ID, a.University_ID,u.University_Name,u.Email_ID,u.Contact_Number,u.Address, Txn_Month, Txn_Year, Total_Amount, Percent_Amount, Is_Paid,  
            //Due_Date FROM ayoge_transctions a left join university_details u on a.University_ID=u.University_ID WHERE a.University_ID=@University_ID";

            //string Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, UD.Address, FT.MonthNo As Txn_Month, FT.Year As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As Percent_Amount, 0 As Is_Paid, FT.Due_Date From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, Year, Fees_Amt, Fees_Amt*0.01 As OnePercent, CAST(CONCAT(Year,'-',MonthNo+1,'-',15) AS Date) As Due_Date From (SELECT University_ID, MONTHNAME(Received_Date) As MonthName, Month As MonthNo, Year, SUM(No_Of_Students) As No_Of_Students, SUM(Fees_Amt) As Fees_Amt FROM fees_collection Where University_ID = 2 Group By University_ID, MonthName, Year) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            //			string Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
            //UD.Address, FT.MonthNo As Txn_Month, FT.Year As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As Percent_Amount, 0 As Is_Paid, FT.Due_Date 
            //From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, Year, Fees_Amt, Fees_Amt*0.01 As OnePercent, 
            //CAST(CONCAT(Year,'-',MonthNo+1,'-',15) AS Date) As Due_Date From (SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, 5 As MonthNo, 2023 As Year, 
            //Count(Student_ID) As No_Of_Students, SUM(Fees_Amount) As Fees_Amt  FROM `student_fees_collection` Where University_ID = @University_ID Group By University_ID, MonthName, 
            //Year) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            //            string Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
            //UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As Percent_Amount, 0 As Is_Paid, FT.Due_Date 
            //From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Fees_Amt*0.01 As OnePercent, 
            //CAST(CONCAT(YearValue,'-',MonthNo+1,'-',15) AS Date) As Due_Date From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
            //(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where Is_Deleted = 0 AND University_ID = @University_ID) As FT 
            //Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            //			string Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
            //UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, 0 As Is_Paid, FT.Due_Date,  DATEDIFF(CURDATE(),FT.Due_Date) As DelayDays, Round((FT.OnePercent*0.015)*CEILING(DATEDIFF(CURDATE(),FT.Due_Date)/30),2) As PenaltyCharge, Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(DATEDIFF(CURDATE(),FT.Due_Date)/30),0) As Percent_Amount
            //From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
            //CAST(CONCAT(YearValue,'-',MonthNo+1,'-',15) AS Date) As Due_Date  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
            //(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where Is_Deleted = 0 AND University_ID = @University_ID) As FT 
            //Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            //            string Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
            //UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, 0 As Is_Paid, FT.Due_Date, Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenaltyCharge, Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) Else
            //Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) END As Percent_Amount
            //From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
            //CAST(CONCAT(YearValue,'-',MonthNo+1,'-',15) AS Date) As Due_Date, DATEDIFF(CURDATE(),CONCAT(YearValue,'-',MonthNo+1,'-',15)) As DelayDays  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
            //(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where University_ID = @University_ID AND Is_Paid = 0 AND Is_Deleted = 0) As FT 
            //Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";
            //            string Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
            //UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, 0 As Is_Paid, FT.Due_Date, Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenaltyCharge, Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) Else
            //Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) END As Percent_Amount
            //From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
            //CAST(CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15) AS Date) As Due_Date, DATEDIFF(CURDATE(),CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15)) As DelayDays  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
            //(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where University_ID = @University_ID AND Is_Paid = 0 AND Is_Deleted = 0) As FT 
            //Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            String Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, 0 As Is_Paid, FT.Due_Date, Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenaltyCharge, Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) Else
Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) END As Percent_Amount
From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
CAST(CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15) AS Date) As Due_Date, DATEDIFF(CURDATE(),CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',Case When MonthNo+1 = 8 Then 16 Else 15 End)) As DelayDays  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where University_ID = @University_ID AND Is_Paid = 0 AND Is_Deleted = 0) As FT 
Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{

					aTxnL.Add(
					new ayoge_transctions_List
					{
						count = i,
						Ayong_Txn_ID = Convert.ToInt32(dr["Ayong_Txn_ID"].ToString().Trim()),
						University_ID = Convert.ToInt32(dr["University_ID"].ToString().Trim()),
						Txn_Month = Convert.ToInt32(dr["Txn_Month"].ToString().Trim()),
						Txn_Month_name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(dr["Txn_Month"].ToString().Trim())),
						Txn_Year = Convert.ToInt32(dr["Txn_Year"].ToString().Trim()),
						Total_Amount = Convert.ToDecimal(dr["Total_Amount"].ToString().Trim()),
						One_Percent = Convert.ToDecimal(dr["OnePercent"].ToString().Trim()),
						//Is_Paid = Convert.ToBoolean(dr["Is_Paid"].ToString().Trim()),
						Due_Date = Convert.ToDateTime(dr["Due_Date"].ToString().Trim()).ToString("dd-MM-yyyy"),                        
                        CustomerName = dr["University_Name"].ToString().Trim(),
						CustomerEmail = dr["Email_ID"].ToString().Trim(),
						CustomerMobile = dr["Contact_Number"].ToString().Trim(),
						BillingAddress = dr["Address"].ToString().Trim(),
                        DelayDays = Convert.ToInt32(dr["DelayDays"].ToString().Trim()),
						PenaltyCharge = Convert.ToDecimal(dr["PenaltyCharge"].ToString().Trim()),
                        Percent_Amount = Convert.ToDouble(dr["Percent_Amount"].ToString().Trim())
                    });
					i++;
				}
			}
			return aTxnL;
		}


        public List<PG_transctions_List> Get_PG_transctions_List_Of_Univsersity(int University_ID)
        {
            List<PG_transctions_List> aTxnL = new List<PG_transctions_List>();
            String Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, 0 As Is_Paid, FT.Due_Date, Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenalInterest, Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) Else
Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) END As Payable_Amount
From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
CAST(CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15) AS Date) As Due_Date, DATEDIFF(CURDATE(),CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',Case When MonthNo+1 = 8 Then 16 Else 15 End)) As DelayDays  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where University_ID = @University_ID AND Is_Paid = 0 AND Is_Deleted = 0) As FT 
Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            MySqlCommand cmd = new MySqlCommand(Query_UserName);
            cmd.Parameters.AddWithValue("@University_ID", University_ID);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {

                    aTxnL.Add(
                    new PG_transctions_List
                    {
                        count = i,
                        Ayong_Txn_ID = Convert.ToInt32(dr["Ayong_Txn_ID"].ToString().Trim()),
                        University_ID = Convert.ToInt32(dr["University_ID"].ToString().Trim()),
                        Txn_Month = Convert.ToInt32(dr["Txn_Month"].ToString().Trim()),
                        Txn_Month_name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(dr["Txn_Month"].ToString().Trim())),
                        Txn_Year = Convert.ToInt32(dr["Txn_Year"].ToString().Trim()),
                        Total_Amount = Convert.ToDecimal(dr["Total_Amount"].ToString().Trim()),
                        One_Percent = Convert.ToDecimal(dr["OnePercent"].ToString().Trim()),
                        //Is_Paid = Convert.ToBoolean(dr["Is_Paid"].ToString().Trim()),
                        Due_Date = Convert.ToDateTime(dr["Due_Date"].ToString().Trim()).ToString("dd-MM-yyyy"),
                        CustomerName = dr["University_Name"].ToString().Trim(),
                        CustomerEmail = dr["Email_ID"].ToString().Trim(),
                        CustomerMobile = dr["Contact_Number"].ToString().Trim(),
                        BillingAddress = dr["Address"].ToString().Trim(),
                        DelayDays = Convert.ToInt32(dr["DelayDays"].ToString().Trim()),
                        PenalInterest = Convert.ToDecimal(dr["PenalInterest"].ToString().Trim()),
                        Payable_Amount = Convert.ToDouble(dr["Payable_Amount"].ToString().Trim())
                    });
                    i++;
                }
            }
            return aTxnL;

        }

        public List<PG_txn_List> Get_PU_txn_List(int University_ID)
        {
            List<PG_txn_List> PUTxnL = new List<PG_txn_List>();
//            String Query_UserName = @"Select UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
//UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, 0 As Is_Paid, FT.Due_Date, Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenalInterest, Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) Else
//Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) END As Payable_Amount
//From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
//CAST(CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15) AS Date) As Due_Date, DATEDIFF(CURDATE(),CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',Case When MonthNo+1 = 8 Then 16 Else 15 End)) As DelayDays  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
//(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where University_ID = @University_ID AND Is_Paid = 0 AND Is_Deleted = 0) As FT 
//Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

 //           String Query_UserName = @"Select 0 As University_ID, 'Test University' As University_Name, 'cgpurc18@gmail.com' As Email_ID, '9770187714' As Contact_Number, 
 //'Sector - 24, Naya Raipur' As Address, '3' As Txn_Month, '2025' As Txn_Year, 500000 As Total_Amount, 5000 As OnePercent, 0 As Is_Paid, '2025-04-15' As Due_Date, 0 As DelayDays, 0.00 As PenalInterest, 5000 As Payable_Amount";

//            String Query_UserName = @"Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
//UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, 0 As Is_Paid, FT.Due_Date, Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenalInterest, Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) Else
//Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) END As Payable_Amount
//From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
//CAST(CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15) AS Date) As Due_Date, DATEDIFF(CURDATE(),CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',Case When MonthNo+1 = 8 Then 16 Else 15 End)) As DelayDays  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
//(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where University_ID = @University_ID AND Is_Paid = 0 AND Is_Deleted = 0) As FT 
//Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID Order by FT.Due_Date";

            String Query_UserName = @"SELECT '0' As Ayong_Txn_ID, University_ID, '---' As University_Name, '--' As Email_ID, '---' As Contact_Number, '---' As Address, Fees_Month As Txn_Month, Fees_Year As Txn_Year, One_Percent_Amt*100 As Total_Amount, One_Percent_Amt As OnePercent, True As Is_Paid,
CAST(CONCAT(Case When Fees_Month = 12 Then Fees_Year+1 Else Fees_Year END,'-',Case When Fees_Month = 12 Then 1 Else Fees_Month+1 END,'-',15) AS Date) As Due_Date,
0 As DelayDays, Penal_Interest As PenalInterest, Payble_Amt As Payable_Amount FROM razorpay_txns Where University_ID = @University_ID AND PG_Status_Id = 3
Union 
Select row_number() over(order by FT.Due_Date) As Ayong_Txn_ID, UD.University_ID, UD.University_Name, UD.Email_ID, UD.Contact_Number, 
UD.Address, FT.MonthNo As Txn_Month, FT.YearValue As Txn_Year, FT.Fees_Amt As Total_Amount, FT.OnePercent As OnePercent, False As Is_Paid, FT.Due_Date, Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenalInterest, Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) Else
Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) END As Payable_Amount
From University_details UD INNER JOIN (Select University_ID, MonthName, MonthNo, YearValue, Fees_Amt, Round(Fees_Amt*0.01,2) As OnePercent, 
CAST(CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15) AS Date) As Due_Date, DATEDIFF(CURDATE(),CONCAT(Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',Case When MonthNo+1 = 8 Then 16 Else 15 End)) As DelayDays  From (Select University_ID, MonthName, MonthNo, YearValue, SUM(Fees_Amount) As Fees_Amt, Count(DISTINCT Student_ID) As NoOfStudents From
(SELECT University_ID, MONTHNAME(Txn_Date) As MonthName, MONTH(Txn_Date) As MonthNo, YEAR(Txn_Date) As YearValue, Fees_Amount, Student_ID  FROM `student_fees_collection` Where University_ID = @University_ID AND Is_Paid = 0 AND Is_Deleted = 0) As FT 
Group By MonthName, YearValue, MonthNo, University_ID) As MWF) As FT ON UD.University_ID = FT.University_ID
Order by Due_Date";

            MySqlCommand cmd = new MySqlCommand(Query_UserName);
            cmd.Parameters.AddWithValue("@University_ID", University_ID);
            DataTable dt = Fnc.GetDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {

                    PUTxnL.Add(
                    new PG_txn_List
                    {
                        count = i,
                        University_ID = Convert.ToInt32(dr["University_ID"].ToString().Trim()),
                        Txn_Month = Convert.ToInt32(dr["Txn_Month"].ToString().Trim()),
                        Txn_Month_name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(dr["Txn_Month"].ToString().Trim())),
                        Txn_Year = Convert.ToInt32(dr["Txn_Year"].ToString().Trim()),
                        Total_Amount = Convert.ToDecimal(dr["Total_Amount"].ToString().Trim()),
                        One_Percent_Amt = Convert.ToDecimal(dr["OnePercent"].ToString().Trim()),
                        Is_Paid = Convert.ToBoolean(dr["Is_Paid"]),
                        Due_Date = Convert.ToDateTime(dr["Due_Date"].ToString().Trim()).ToString("dd-MM-yyyy"),
                        UniversityName = dr["University_Name"].ToString().Trim(),
                        UniversityEmail = dr["Email_ID"].ToString().Trim(),
                        UniversityMobile = dr["Contact_Number"].ToString().Trim(),
                        BillingAddress = dr["Address"].ToString().Trim(),
                        DelayDays = Convert.ToInt32(dr["DelayDays"].ToString().Trim()),
                        Penal_Interest = Convert.ToDecimal(dr["PenalInterest"].ToString().Trim()),
                        Payble_Amt = Convert.ToDecimal(dr["Payable_Amount"].ToString().Trim())
                    });
                    i++;
                }
            }
            return PUTxnL;

        }


        public PG_txn_List Get_PU_txn_ByMonthYear(int University_ID, int MonthNo, int YearValue)
        {
            PG_txn_List result = null;

            string query = @"
        Select 
            FT.MonthNo As Txn_Month, 
            FT.YearValue As Txn_Year, 
            FT.Fees_Amt As Total_Amount, 
            FT.OnePercent As OnePercent, 
            False As Is_Paid, 
            FT.Due_Date, 
             FT.University_ID,
             U.University_Name, 
             U.Email_ID, 
             U.Contact_Number, 
            Case When FT.DelayDays < 0 Then 0 Else FT.DelayDays END As DelayDays, 
            Case When FT.DelayDays < 0 Then 0 Else Round((FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),2) END As PenalInterest, 
            Case When FT.DelayDays <= 0 Then Round(FT.OnePercent) 
                 Else Round(FT.OnePercent+(FT.OnePercent*0.015)*CEILING(FT.DelayDays/30),0) 
            END As Payable_Amount
        From 
        (
            Select 
                University_ID, 
                MonthName, 
                MonthNo, 
                YearValue, 
                Fees_Amt, 
                Round(Fees_Amt*0.01,2) As OnePercent, 
                CAST(CONCAT(
                        Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',
                        Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',15
                    ) AS Date) As Due_Date, 
                DATEDIFF(CURDATE(),
                        CONCAT(
                            Case When MonthNo = 12 Then YearValue+1 Else YearValue END,'-',
                            Case When MonthNo = 12 Then 1 Else MonthNo+1 END,'-',
                            Case When MonthNo+1 = 8 Then 16 Else 15 End
                        )
                ) As DelayDays  
            From 
            (
                Select 
                    University_ID, 
                    MONTHNAME(Txn_Date) As MonthName, 
                    MONTH(Txn_Date) As MonthNo, 
                    YEAR(Txn_Date) As YearValue, 
                    SUM(Fees_Amount) As Fees_Amt, 
                    Count(DISTINCT Student_ID) As NoOfStudents 
                From student_fees_collection 
                Where University_ID = @University_ID 
                  AND Is_Paid = 0 
                  AND Is_Deleted = 0 
                  AND MONTH(Txn_Date) = @MonthNo 
                  AND YEAR(Txn_Date) = @YearValue
                Group By MonthName, YearValue, MonthNo, University_ID
            ) As FT 
        ) As FT
      INNER JOIN university_details U ON FT.University_ID = U.University_ID;
    ";

            MySqlCommand cmd = new MySqlCommand(query);
            cmd.Parameters.AddWithValue("@University_ID", University_ID);
            cmd.Parameters.AddWithValue("@MonthNo", MonthNo);
            cmd.Parameters.AddWithValue("@YearValue", YearValue);

            DataTable dt = Fnc.GetDataTable(cmd);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                result = new PG_txn_List
                {
                    University_ID = University_ID,
                    Txn_Month = Convert.ToInt32(dr["Txn_Month"].ToString().Trim()),
                    Txn_Month_name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat
                                        .GetMonthName(Convert.ToInt32(dr["Txn_Month"].ToString().Trim())),
                    Txn_Year = Convert.ToInt32(dr["Txn_Year"].ToString().Trim()),
                    Total_Amount = Convert.ToDecimal(dr["Total_Amount"].ToString().Trim()),
                    One_Percent_Amt = Convert.ToDecimal(dr["OnePercent"].ToString().Trim()),
                    Is_Paid = Convert.ToBoolean(dr["Is_Paid"]),
                    Due_Date = Convert.ToDateTime(dr["Due_Date"].ToString().Trim()).ToString("dd-MM-yyyy"),
                    DelayDays = Convert.ToInt32(dr["DelayDays"].ToString().Trim()),
                    Penal_Interest = Convert.ToDecimal(dr["PenalInterest"].ToString().Trim()),
                    Payble_Amt = Convert.ToDecimal(dr["Payable_Amount"].ToString().Trim()),
                    UniversityName = dr["University_Name"].ToString().Trim(),
                    UniversityEmail = dr["Email_ID"].ToString().Trim(),
                    UniversityMobile = dr["Contact_Number"].ToString().Trim()
                };
            }

            return result;
        }


        public List<Challan_University> Get_Challan_University_Of_Univsersity(int id)
		{
			List<Challan_University> cu = new List<Challan_University>();
			string Query_UserName = @"select atx.Ayong_Txn_ID,Txn_Number,Txn_Month,Txn_Year,Total_Amount,Percent_Amount,Is_Paid,Draft_Bank,Demand_Draft_No,Date_of_issue,Paid_Date, Due_Date,atx.University_ID, University_Name,address,Contact_Number,Email_ID,Pin_Code from ayoge_transctions atx left join university_details ud on atx.University_ID=ud.University_ID where atx.Ayong_Txn_ID=@Ayong_Txn_ID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Ayong_Txn_ID", id);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					if (!string.IsNullOrEmpty(Convert.ToString(dr["Date_of_issue"])))
					{
						cu.Add(
					new Challan_University
					{
						Ayong_Txn_ID = Convert.ToInt32(dr["Ayong_Txn_ID"].ToString().Trim()),
						Txn_Number = dr["Txn_Number"].ToString().Trim(),
						Txn_Month = Convert.ToInt32(dr["Txn_Month"].ToString().Trim()),
						Txn_Month_Name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(dr["Txn_Month"].ToString().Trim())),
						Txn_Year = Convert.ToInt32(dr["Txn_Year"].ToString().Trim()),
						Total_Amount = Math.Round(Convert.ToDecimal(dr["Total_Amount"].ToString().Trim())),
						Total_Amount_Word = new IndiaCurrencyConverter().ConvertToWord(Math.Round(Convert.ToDecimal(dr["Total_Amount"].ToString().Trim())).ToString().Trim()).ToString(),
						Percent_Amount = Math.Round(Convert.ToDecimal(dr["Percent_Amount"].ToString().Trim())),
						Percent_Amount_word = new IndiaCurrencyConverter().ConvertToWord(Math.Round(Convert.ToDecimal(dr["Percent_Amount"].ToString().Trim())).ToString().Trim()).ToString(),
						Is_Paid = Convert.ToBoolean(dr["Is_Paid"].ToString().Trim()),
						Draft_Bank = dr["Draft_Bank"].ToString().Trim(),
						Date_of_issue = Convert.ToDateTime(dr["Date_of_issue"].ToString().Trim()),
						Demand_Draft_No = dr["Demand_Draft_No"].ToString().Trim(),
						Paid_Date = Convert.ToDateTime(dr["Paid_Date"].ToString().Trim()),
						Due_Date = Convert.ToDateTime(dr["Due_Date"].ToString().Trim()),
						University_ID = Convert.ToInt32(dr["University_ID"].ToString().Trim()),
						University_Name = dr["University_Name"].ToString().Trim(),
						Contact_Number = dr["Contact_Number"].ToString().Trim(),
						Email_ID = dr["Email_ID"].ToString().Trim(),
						Address = dr["address"].ToString().Trim(),
						Pin_Code = dr["Pin_Code"].ToString().Trim(),
						Challan_Detials = Get_Challan_University_Details_Of_Univsersity(Convert.ToInt32(dr["Ayong_Txn_ID"].ToString().Trim()))
					});
					}
					else
					{
						cu.Add(
				new Challan_University
				{
					Ayong_Txn_ID = Convert.ToInt32(dr["Ayong_Txn_ID"].ToString().Trim()),
					Txn_Number = dr["Txn_Number"].ToString().Trim(),
					Txn_Month = Convert.ToInt32(dr["Txn_Month"].ToString().Trim()),
					Txn_Month_Name = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(dr["Txn_Month"].ToString().Trim())),
					Txn_Year = Convert.ToInt32(dr["Txn_Year"].ToString().Trim()),
					Total_Amount = Math.Round(Convert.ToDecimal(dr["Total_Amount"].ToString().Trim())),
					Total_Amount_Word = new IndiaCurrencyConverter().ConvertToWord(Math.Round(Convert.ToDecimal(dr["Total_Amount"].ToString().Trim())).ToString().Trim()).ToString(),
					Percent_Amount = Math.Round(Convert.ToDecimal(dr["Percent_Amount"].ToString().Trim())),
					Percent_Amount_word = new IndiaCurrencyConverter().ConvertToWord(Math.Round(Convert.ToDecimal(dr["Percent_Amount"].ToString().Trim())).ToString().Trim()).ToString(),
					Is_Paid = Convert.ToBoolean(dr["Is_Paid"].ToString().Trim()),
					Draft_Bank = dr["Draft_Bank"].ToString().Trim(),
					//Date_of_issue = Convert.ToDateTime(dr["Date_of_issue"].ToString().Trim()),
					Demand_Draft_No = dr["Demand_Draft_No"].ToString().Trim(),
					//Paid_Date = Convert.ToDateTime(dr["Paid_Date"].ToString().Trim()),
					Due_Date = Convert.ToDateTime(dr["Due_Date"].ToString().Trim()),
					University_ID = Convert.ToInt32(dr["University_ID"].ToString().Trim()),
					University_Name = dr["University_Name"].ToString().Trim(),
					Contact_Number = dr["Contact_Number"].ToString().Trim(),
					Email_ID = dr["Email_ID"].ToString().Trim(),
					Address = dr["address"].ToString().Trim(),
					Pin_Code = dr["Pin_Code"].ToString().Trim(),
					Challan_Detials = Get_Challan_University_Details_Of_Univsersity(Convert.ToInt32(dr["Ayong_Txn_ID"].ToString().Trim()))
				});
					}

				}
			}
			return cu;
		}

		public List<Challan_University_Detials> Get_Challan_University_Details_Of_Univsersity(int Ayong_Txn_ID)
		{
			List<Challan_University_Detials> aTxnL = new List<Challan_University_Detials>();
			string Query_UserName = @"select a.University_ID,University_Name,ud.address,Univ_Course_Name,Univ_subCourse_Name,studentcount,totalAmt from (SELECT count(distinct atd.Student_ID) as studentcount,sum(atd.Amount) as totalAmt,University_ID,Univ_Course_ID,Univ_subCourse_ID FROM ayoge_transctions_details atd left join student_details sd on atd.Student_ID=sd.Student_ID where Ayong_Txn_ID=@Ayong_Txn_ID group by Univ_Course_ID,Univ_subCourse_ID) a
left join university_details ud  on a.University_ID=ud.University_ID
left join university_course uc on a.Univ_Course_ID=uc.Univ_Course_ID
left join university_sub_course usc on a.Univ_subCourse_ID=usc.Univ_subCourse_ID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Ayong_Txn_ID", Ayong_Txn_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					aTxnL.Add(
					new Challan_University_Detials
					{
						count = i,
						Ayong_Txn_ID = Ayong_Txn_ID,
						University_ID = Convert.ToInt32(dr["University_ID"].ToString().Trim()),
						University_Name = dr["University_Name"].ToString().Trim(),
						University_Address = dr["address"].ToString().Trim(),
						Course_Name = dr["Univ_Course_Name"].ToString().Trim() + " [" + dr["Univ_subCourse_Name"].ToString().Trim() + "]",
						Number_of_Students = Convert.ToInt32(dr["studentcount"].ToString().Trim()),
						Rate_Fees = 0,
						Total_Fees = Convert.ToDecimal(dr["totalAmt"].ToString().Trim()),
						Percent_Total_Fees = Math.Round((Convert.ToDecimal(dr["totalAmt"].ToString().Trim()) * 1) / 100),
					});
					i++;
				}
			}
			return aTxnL;
		}


		#endregion

		#region Student Promote

		public bool insert_student_details_promote(student_promote student_Promote)
		{
			string Query = "INSERT INTO student_details_promote( Student_Id, Session_ID, Semester_Year, Result, Promoted_By) VALUES ( @Student_Id, @Session_ID, @Semester_Year, @Result, @Promoted_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Student_Id", student_Promote.Student_Id);
			cmd.Parameters.AddWithValue("@Session_ID", student_Promote.Session_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", student_Promote.Semester_Year);
			cmd.Parameters.AddWithValue("@Result", student_Promote.Result);
			cmd.Parameters.AddWithValue("@Promoted_By", student_Promote.Promoted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public student_Details_promote Get_Student_Class_Detials_for_Promotion(int Student_Id, int University_ID)
		{
			student_Details_promote sp = new student_Details_promote();
			string Query = @"SELECT st.Student_Id, st.University_ID, st.Univ_Course_ID, st.Univ_subCourse_ID, st.Session_ID, mscsess.Session_Name, st.Course_Mode_ID ,st.Semester_Year,Number_of_Year_Sem FROM student_details st
INNER JOIN university_course unicur ON unicur.Univ_Course_ID = st.Univ_Course_ID and st.University_ID=unicur.University_ID
INNER JOIN master_academic_session mscsess ON mscsess.Session_ID = st.Session_ID
WHERE st.Is_Deleted= 0 and st.Student_Id =@Student_Id and st.University_ID=@University_ID and Is_Passout=0;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Student_Id", Student_Id);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				sp.Student_Id = Convert.ToInt32(dt.Rows[0]["Student_Id"].ToString());
				sp.Session_ID = Convert.ToInt32(dt.Rows[0]["Session_ID"].ToString());
				sp.Course_Mode_ID = Convert.ToInt32(dt.Rows[0]["Course_Mode_ID"].ToString());
				sp.Semester_Year = Convert.ToInt32(dt.Rows[0]["Semester_Year"].ToString());
				sp.Max_Semester_Year = Convert.ToInt32(dt.Rows[0]["Number_of_Year_Sem"].ToString());
			}
			return sp;
		}




        #endregion

        public int Fees_Insert(FeesReceived FR)
        {
            string Query = @"INSERT INTO fees_collection(University_ID, Month, Year, No_Of_Students, Fees_Amt, Received_Date, Entry_DateTime, Session_ID) 
                            VALUES (@University_ID,Month(@Received_Date),Year(@Received_Date),@No_Of_Students,@Fees_Amt,@Received_Date,now(),@Session_ID)";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@University_ID", FR.University_ID);
            cmd.Parameters.AddWithValue("@No_Of_Students", FR.NoOfStudents);
            cmd.Parameters.AddWithValue("@Fees_Amt", FR.FeesAmount);
            cmd.Parameters.AddWithValue("@Received_Date", FR.ReceivedDate);
            cmd.Parameters.AddWithValue("@Session_ID", FR.Session_ID);
            int val = Fnc.InsertCommands_AndGetting_ID(cmd);
            return val;
        }


        public List<Course_Fees> Get_university_course_List_ForFee_Home(int University_ID)
		{
			List<Course_Fees> univ_course = new List<Course_Fees>();
			string Query_UserName = @"SELECT Univ_Course_ID, University_ID, Univ_Course_Name, Univ_Course_Code, mcm.Course_Mode, 
									concat( TRIM(TRAILING '.0' FROM Number_of_Year_Sem) ,' - ',mcm.Course_Mode) as course_duration,Number_of_Year_Sem 
									FROM university_course uc left join master_course_mode mcm on uc.Course_Mode_ID=mcm.Course_Mode_ID 
									where Is_Deleted=0 and University_ID=@University_ID; ";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_course.Add(new Course_Fees
					{
						count = i,
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						Univ_Course_Name = dr["Univ_Course_Name"].ToString().Trim(),
						Univ_Course_Code = dr["Univ_Course_Code"].ToString().Trim(),
						Course_Mode = dr["Course_Mode"].ToString().Trim(),
						Number_of_Year_Sem = dr["course_duration"].ToString().Trim(),
						Number_Year_Sem = Convert.ToDecimal(dr["Number_of_Year_Sem"].ToString().Trim()),
						Sub_course = Get_university_sub_course_list_forFee_Home(Convert.ToInt32(dr["University_ID"].ToString().Trim()), Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()), Convert.ToDecimal(dr["Number_of_Year_Sem"].ToString().Trim()), dr["Course_Mode"].ToString().Trim())
					});
					i++;
				}
			}
			return univ_course;
		}

		public List<sub_course_fee> Get_university_sub_course_list_forFee_Home(int University_ID, int Univ_Course_ID, decimal Number_of_Year_Sem, string Course_Mode)
		{
			List<sub_course_fee> univ_Sub_course = new List<sub_course_fee>();
			string Query_UserName = "SELECT Univ_subCourse_ID, Univ_Course_ID, University_ID, Univ_subCourse_Name, Univ_subCourse_Code FROM university_sub_course WHERE Is_Deleted=0 and Univ_Course_ID=@Univ_Course_ID and University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					univ_Sub_course.Add(new sub_course_fee
					{
						count = i,
						Univ_subCourse_ID = Convert.ToInt32(dr["Univ_subCourse_ID"].ToString().Trim()),
						Univ_Course_ID = Convert.ToInt32(dr["Univ_Course_ID"].ToString().Trim()),
						Univ_subCourse_Name = dr["Univ_subCourse_Name"].ToString().Trim(),
						Univ_subCourse_Code = dr["Univ_subCourse_Code"].ToString().Trim(),
						semesters = Get_university_course_semyear_list_froFee_Home(University_ID, Univ_Course_ID, Convert.ToInt32(dr["Univ_subCourse_ID"].ToString().Trim()), Number_of_Year_Sem, Course_Mode)
					}); ;
					i++;
				}
			}
			return univ_Sub_course;
		}

		public List<Sem_Years_fee> Get_university_course_semyear_list_froFee_Home(int University_ID, int Univ_Course_ID, int Univ_subCourse_ID, decimal Number_Year_Sem, string course_mode)
		{
			List<Sem_Years_fee> sem_Years = new List<Sem_Years_fee>();

			for (int i = 1; i <= Number_Year_Sem; i++)
			{
				sem_Years.Add(new Sem_Years_fee
				{
					count = i,
					Sementer_Year = course_mode + " - " + i,
					Fees = Get_university_course_fees_Home(University_ID, Univ_Course_ID, Univ_subCourse_ID, i)
				});
			}
			return sem_Years;
		}

		public decimal Get_university_course_fees_Home(int University_ID, int Univ_Course_ID, int Univ_subCourse_ID, int Semester_Year)
		{
			string Query_UserName = "select Fee_ID, Amount, mas.Session_ID from university_course_fees ucf left join master_academic_session mas on ucf.Session_ID=mas.Session_ID WHERE University_ID=@University_ID and Univ_Course_ID=@Univ_Course_ID and Univ_subCourse_ID=@Univ_subCourse_ID and Semester_Year=@Semester_Year order by Session_ID desc limit 0,1;";
			MySqlCommand cmd = new MySqlCommand(Query_UserName);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			cmd.Parameters.AddWithValue("@Univ_Course_ID", Univ_Course_ID);
			cmd.Parameters.AddWithValue("@Univ_subCourse_ID", Univ_subCourse_ID);
			cmd.Parameters.AddWithValue("@Semester_Year", Semester_Year);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
				return Convert.ToDecimal(dt.Rows[0]["Amount"].ToString().Trim());
			else
				return 0;
		}

		public List<UniversityDetailsWithFeesDetails> UniversityDetailsWithFeesDetails(int University_ID)
		{
			List<UniversityDetailsWithFeesDetails> uniprofile = new List<UniversityDetailsWithFeesDetails>();
			string Query = @"SELECT University_ID, University_Name 
                                        FROM university_details WHERE University_ID=@University_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@University_ID", University_ID);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				uniprofile.Add(
				  new UniversityDetailsWithFeesDetails
				  {
					  University_Name = dt.Rows[0]["University_Name"].ToString(),
					  Course_Fees_Data = Get_university_course_List_ForFee_Home(University_ID)
				  });
			}
			return uniprofile;
		}

		/*Start Add gallery code start*/

		public bool Gallery_Insert(Gallery_Insert gallery)
		{
			string Query = "INSERT INTO photo_gallery( Name, Picture,Entry_DateTime, Entry_By) VALUES ( @Name, @Picture,now(), @Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Name", gallery.Name);
			cmd.Parameters.AddWithValue("@Picture", gallery.Picture);
			cmd.Parameters.AddWithValue("@Entry_By", gallery.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<Gallery> Get_Gallery()
		{
			List<Gallery> ListNotify = new List<Gallery>();
			string Query_Gallery = "SELECT photo_ID, Name, Picture FROM photo_gallery WHERE Is_Deleted=0 order by photo_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_Gallery);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new Gallery
					{
						count = i,
						photo_ID = Convert.ToInt32(dr["photo_ID"].ToString().Trim()),
						Name = dr["Name"].ToString().Trim(),
						Picture = dr["Picture"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}

		public bool Gallery_Delete(Gallery_Delete gallery)
		{
			string Query = "UPDATE photo_gallery SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE photo_ID=@photo_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@photo_ID", gallery.photo_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", gallery.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<Gallery> GetPhotoGalleryShowHome()
		{
			List<Gallery> ListNotify = new List<Gallery>();
			string Query_gallery = "SELECT photo_ID, Name, Picture FROM photo_gallery WHERE Is_Deleted=0 order by photo_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_gallery);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new Gallery
					{
						count = i,
						photo_ID = Convert.ToInt32(dr["photo_ID"].ToString().Trim()),
						Name = dr["Name"].ToString().Trim(),
						Picture = dr["Picture"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}
		/*end Add gallery code start*/

		/*Start News ADD Code Admin Side*/
		public bool News_Insert(News_Insert news)
		{
			string Query = "INSERT INTO news( News_Title, News_Descrip,News_File,Entry_DateTime, Entry_By) VALUES ( @News_Title,@News_Descrip, @News_File,now(), @Entry_By);";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@News_Title", news.News_Title);
			cmd.Parameters.AddWithValue("@News_Descrip", news.News_Descrip);
			cmd.Parameters.AddWithValue("@News_File", news.News_File);
			cmd.Parameters.AddWithValue("@Entry_By", news.Entry_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

		public List<News> Get_News()
		{
			List<News> ListNotify = new List<News>();
			string Query_News = "SELECT news_ID,  News_Title, News_Descrip,News_File FROM news WHERE Is_Deleted=0 order by news_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_News);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new News
					{
						count = i,
						news_ID = Convert.ToInt32(dr["News_ID"].ToString().Trim()),
						News_Title = dr["News_Title"].ToString().Trim(),
						News_Descrip = dr["News_Descrip"].ToString().Trim(),
						News_File = dr["News_File"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}

		public bool News_Delete(News_Delete news)
		{
			string Query = "UPDATE news SET Is_Deleted=1,Delete_DateTime=now(),Deleted_By=@Deleted_By WHERE news_ID=@news_ID;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@news_ID", news.news_ID);
			cmd.Parameters.AddWithValue("@Deleted_By", news.Deleted_By);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}
		public List<News> GetNewsShowHome()
		{
			List<News> ListNotify = new List<News>();
			string Query_news = "SELECT news_ID,  News_Title, News_Descrip,News_File FROM news WHERE Is_Deleted=0 order by news_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_news);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new News
					{
						count = i,
						news_ID = Convert.ToInt32(dr["news_ID"].ToString().Trim()),
						News_Title = dr["News_Title"].ToString().Trim(),
						News_Descrip = dr["News_Descrip"].ToString().Trim(),
						News_File = dr["News_File"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}

		public List<News> GetNewsShowHomeMarquee()
		{
			List<News> ListNotify = new List<News>();
			string Query_news = "SELECT news_ID,  News_Title FROM news WHERE Is_Deleted=0 order by news_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query_news);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				int i = 1;
				foreach (DataRow dr in dt.Rows)
				{
					ListNotify.Add(
					new News
					{
						count = i,
						news_ID = Convert.ToInt32(dr["news_ID"].ToString().Trim()),
						News_Title = dr["News_Title"].ToString().Trim()
					});
					i++;
				}
			}
			return ListNotify;
		}

		public News GetNewsForLanguageChange()
		{
			News commm = new News();
			string Query = @"SELECT news_ID,  News_Title FROM news WHERE Is_Deleted=0 order by news_ID DESC;";
			MySqlCommand cmd = new MySqlCommand(Query);
			DataTable dt = Fnc.GetDataTable(cmd);
			if (dt.Rows.Count > 0)
			{
				commm.news_ID = Convert.ToInt32(dt.Rows[0]["news_ID"].ToString().Trim());
				commm.News_Title = dt.Rows[0]["News_Title"].ToString();
			}
			return commm;
		}
		/*End News ADD Code Admin Side*/

		/*feedback insert start*/
		public bool Feedback_Insert(Feedback fb)
		{
			string Query = "INSERT INTO user_feedback(name, mobile_no, email_id, subject, feedback, created_ipaddress, created_datetime) VALUES(@name, @mobile_no, @email_id, @subject, @feedback, @created_ipaddress, now());";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@name", fb.Name);
			cmd.Parameters.AddWithValue("@mobile_no", fb.Contact_Number);
			cmd.Parameters.AddWithValue("@email_id", fb.Email_ID);
			cmd.Parameters.AddWithValue("@subject", fb.Subject);
			cmd.Parameters.AddWithValue("@feedback", fb.Feedback_data);
			cmd.Parameters.AddWithValue("@created_ipaddress", HttpContext.Current.Request.UserHostAddress);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}
		/*feedback insert end*/


		/*Payment Gateway Start*/

		public string Get_PG_URL_Complete(Atom_PG_Request R)
		{

			Atom_PG_Request_common Rc = new Atom_PG_Request_common
			{
				Login = Convert.ToInt32(ConfigurationManager.AppSettings["PGUserID"].ToString()),
				Password = ConfigurationManager.AppSettings["PGTxnPwd"].ToString(),
				TxnType = ConfigurationManager.AppSettings["PGTxnType"].ToString(),
				ProductId = ConfigurationManager.AppSettings["PGProdID"].ToString(),
				TxnCurrency = ConfigurationManager.AppSettings["PGTxnCurrency"].ToString(),
				txnServiceChargeamt = Convert.ToInt32(ConfigurationManager.AppSettings["PGTxnServiceChargeAmt"].ToString()),
				Clientcode = ConfigurationManager.AppSettings["PGClientCode"].ToString(),
				CustomerAccountNo = Convert.ToInt32(ConfigurationManager.AppSettings["PGCustAccNo"].ToString()),
				ReturnURL = ConfigurationManager.AppSettings["PGReturnUrl"].ToString()
			};

			//int rno = new Random().Next(100, 999);

			//R.txnID = "PU" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + rno;
			//R.TxnDate = System.DateTime.Now;

			byte[] bytes = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["PGHashReqKey"].ToString());
			R.signature = ByteArrayToHexString(new System.Security.Cryptography.HMACSHA512(bytes).ComputeHash(Encoding.UTF8.GetBytes(Rc.Login + Rc.Password + Rc.TxnType + Rc.ProductId + R.txnID + R.Amount + Rc.TxnCurrency))).ToLower();
			string ParameterData = @"login=" + Rc.Login + "&pass=" + Rc.Password + "&ttype=" + Rc.TxnType + "&prodid=" + Rc.ProductId + "&amt=" + R.Amount + "&txncurr=" + Rc.TxnCurrency + "&txnscamt=" + Rc.txnServiceChargeamt + "&clientcode=" + Rc.Clientcode + "&txnid=" + R.txnID + "&date=" + R.TxnDate.ToString("dd/MM/yyyy") + "&custacc=" + Rc.CustomerAccountNo + "&udf1=" + R.CustomerName + "&udf2=" + R.CustomerEmail + "&udf3=" + R.CustomerMobile + "&udf4=" + R.BillingAddress + "&udf9=" + R.Ayong_Txn_ID + "&udf10=" + R.BillingYear + "&udf11=" + R.BillingMonth + "&ru=" + Rc.ReturnURL + "&signature=" + R.signature;

			byte[] iv = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			int iterations = 65536;
			R.UnEncryptedURLData = ParameterData;
			string URL = ConfigurationManager.AppSettings["PGUrl"].ToString() + "?login=" + Rc.Login + "&encdata=" + Encrypt(ParameterData, ConfigurationManager.AppSettings["PGAESReqKey"].ToString(), ConfigurationManager.AppSettings["PGAESReqSltKey"].ToString(), iv, iterations);

			return URL;
		}

        //public string Get_RazorPG_URL_Complete(RazorPG_Request RP)
        //{

        //    Atom_PG_Request_common Rc = new Atom_PG_Request_common
        //    {
        //        Login = Convert.ToInt32(ConfigurationManager.AppSettings["PGUserID"].ToString()),
        //        Password = ConfigurationManager.AppSettings["PGTxnPwd"].ToString(),
        //        TxnType = ConfigurationManager.AppSettings["PGTxnType"].ToString(),
        //        ProductId = ConfigurationManager.AppSettings["PGProdID"].ToString(),
        //        TxnCurrency = ConfigurationManager.AppSettings["PGTxnCurrency"].ToString(),
        //        txnServiceChargeamt = Convert.ToInt32(ConfigurationManager.AppSettings["PGTxnServiceChargeAmt"].ToString()),
        //        Clientcode = ConfigurationManager.AppSettings["PGClientCode"].ToString(),
        //        CustomerAccountNo = Convert.ToInt32(ConfigurationManager.AppSettings["PGCustAccNo"].ToString()),
        //        ReturnURL = ConfigurationManager.AppSettings["PGReturnUrl"].ToString()
        //    };

        //    //int rno = new Random().Next(100, 999);

        //    //R.txnID = "PU" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + rno;
        //    //R.TxnDate = System.DateTime.Now;

        //    byte[] bytes = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["PGHashReqKey"].ToString());
        //    RP.signature = ByteArrayToHexString(new System.Security.Cryptography.HMACSHA512(bytes).ComputeHash(Encoding.UTF8.GetBytes(Rc.Login + Rc.Password + Rc.TxnType + Rc.ProductId + RP.txnID + RP.Amount + Rc.TxnCurrency))).ToLower();
        //    string ParameterData = @"login=" + Rc.Login + "&pass=" + Rc.Password + "&ttype=" + Rc.TxnType + "&prodid=" + Rc.ProductId + "&amt=" + RP.Amount + "&txncurr=" + Rc.TxnCurrency + "&txnscamt=" + Rc.txnServiceChargeamt + "&clientcode=" + Rc.Clientcode + "&txnid=" + RP.txnID + "&date=" + RP.TxnDate.ToString("dd/MM/yyyy") + "&custacc=" + Rc.CustomerAccountNo + "&udf1=" + RP.CustomerName + "&udf2=" + RP.CustomerEmail + "&udf3=" + RP.CustomerMobile + "&udf4=" + RP.BillingAddress + "&udf9=" + RP.Order_ID + "&udf10=" + RP.BillingYear + "&udf11=" + RP.BillingMonth + "&ru=" + Rc.ReturnURL + "&signature=" + RP.signature;

        //    byte[] iv = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        //    int iterations = 65536;
        //    //RP.UnEncryptedURLData = ParameterData;
        //    string URL = ConfigurationManager.AppSettings["PGUrl"].ToString() + "?login=" + Rc.Login + "&encdata=" + Encrypt(ParameterData, ConfigurationManager.AppSettings["PGAESReqKey"].ToString(), ConfigurationManager.AppSettings["PGAESReqSltKey"].ToString(), iv, iterations);

        //    return URL;
        //}



        public String Encrypt(String plainText, String passphrase, String salt, Byte[] iv, int iterations)
		{
			var plainBytes = Encoding.UTF8.GetBytes(plainText);
			string data = ByteArrayToHexString(Encrypt(plainBytes, GetSymmetricAlgorithm(passphrase, salt, iv, iterations))).ToUpper();
			return data;
		}

		public byte[] Encrypt(byte[] plainBytes, SymmetricAlgorithm sa)
		{
			return sa.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
		}

		public String Decrypt(String plainText, String passphrase, String salt, Byte[] iv, int iterations)
		{
			byte[] str = HexStringToByte(plainText);

			string data1 = Encoding.UTF8.GetString(Decrypt(str, GetSymmetricAlgorithm(passphrase, salt, iv, iterations)));
			return data1;
		}

        public String HMAC_Sha256(string data, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public byte[] Decrypt(byte[] plainBytes, SymmetricAlgorithm sa)
		{
			return sa.CreateDecryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
		}

		//public static string byteToHexString(byte[] byData)
		//{
		//	StringBuilder sb = new StringBuilder((byData.Length * 2));
		//	for (int i = 0; (i < byData.Length); i++)
		//	{
		//		int v = (byData[i] & 255);
		//		if ((v < 16))
		//		{
		//			sb.Append('0');
		//		}
		//		sb.Append(v.ToString("X"));
		//	}
		//	return sb.ToString();
		//}

		public SymmetricAlgorithm GetSymmetricAlgorithm(String passphrase, String salt, Byte[] iv, int iterations)
		{
			var saltBytes = new byte[16];
			var ivBytes = new byte[16];
			Rfc2898DeriveBytes rfcdb = new System.Security.Cryptography.Rfc2898DeriveBytes(passphrase, Encoding.UTF8.GetBytes(salt), iterations);
			saltBytes = rfcdb.GetBytes(32);
			var tempBytes = iv;
			Array.Copy(tempBytes, ivBytes, Math.Min(ivBytes.Length, tempBytes.Length));
			var rij = new RijndaelManaged(); //SymmetricAlgorithm.Create();
			rij.Mode = CipherMode.CBC;
			rij.Padding = PaddingMode.PKCS7;
			rij.FeedbackSize = 128;
			rij.KeySize = 128;

			rij.BlockSize = 128;
			rij.Key = saltBytes;
			rij.IV = ivBytes;
			return rij;
		}
		protected static byte[] HexStringToByte(string hexString)
		{
			try
			{
				int bytesCount = (hexString.Length) / 2;
				byte[] bytes = new byte[bytesCount];
				for (int x = 0; x < bytesCount; ++x)
				{
					bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
				}
				return bytes;
			}
			catch
			{
				throw;
			}
		}
		public static string ByteArrayToHexString(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

		public bool Insert_payment_string(PG_String_Insert i)
		{
			string Query = "INSERT INTO payment_string(Ayong_Txn_ID, University_ID, Txn_Number, RequestString,ReqStringData, RequestDateTime) VALUES (@Ayong_Txn_ID, @University_ID, @Txn_Number, @RequestString,@ReqStringData, now());";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@Ayong_Txn_ID", i.Ayong_Txn_ID);
			cmd.Parameters.AddWithValue("@University_ID", i.University_ID);
			cmd.Parameters.AddWithValue("@Txn_Number", i.Txn_Number);
			cmd.Parameters.AddWithValue("@RequestString", i.RequestString);
			cmd.Parameters.AddWithValue("@ReqStringData", i.ReqStringData);
            cmd.Parameters.AddWithValue("@Amount", i.Amount);

            bool val = Fnc.CURDCommands(cmd);
			return val;
		}

        public bool Insert_RP_Txn_Details(RazorPG_Request RPR)
        {
            string Query = "INSERT INTO razorpay_txns (Order_ID, University_ID, Txn_Number, One_Percent_Amt, Penal_Interest, Payble_Amt, Fees_Month, Fees_Year, RequestDateTime, PG_Status_Id)  VALUES (@Order_ID, @University_ID, @Txn_Number, @One_Percent_Amt, @Penal_Interest, @Payble_Amt, @Fees_Month, @Fees_Year, now(), 1)";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@Order_ID", RPR.Order_ID);
            cmd.Parameters.AddWithValue("@University_ID", RPR.UniversityID);
            cmd.Parameters.AddWithValue("@Txn_Number", RPR.TxnID);
            cmd.Parameters.AddWithValue("@One_Percent_Amt", RPR.One_Percent_Amt);
            cmd.Parameters.AddWithValue("@Penal_Interest", RPR.Penal_Interest);
            cmd.Parameters.AddWithValue("@Payble_Amt", RPR.Payble_Amt);
            cmd.Parameters.AddWithValue("@Fees_Month", RPR.BillingMonth);
            cmd.Parameters.AddWithValue("@Fees_Year", RPR.BillingYear);

            bool val = Fnc.CURDCommands(cmd);
            return val;
        }

        public TxnCheck PGTxnCheck(String OrdId)
        {
            TxnCheck TC = new TxnCheck();
            string Query = "SELECT Txn_Number, Payble_Amt, PG_Status_Id, University_ID, Fees_Month, Fees_Year  FROM razorpay_txns WHERE Order_ID = @Order_ID";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@Order_ID", OrdId);
            DataTable PGDT = Fnc.GetDataTable(cmd);
            if(PGDT.Rows.Count > 0)
            {
                TC.OrderId = OrdId;
                TC.TxnNumber = PGDT.Rows[0]["Txn_Number"].ToString();
                TC.TxnAmount = PGDT.Rows[0]["Payble_Amt"].ToString();
                TC.PG_Status_id = PGDT.Rows[0]["PG_Status_Id"].ToString();
                TC.UniversityId = Convert.ToInt32(PGDT.Rows[0]["University_ID"].ToString());
                TC.FMonth = Convert.ToInt32(PGDT.Rows[0]["Fees_Month"].ToString());
                TC.FYear = Convert.ToInt32(PGDT.Rows[0]["Fees_Year"].ToString());
            } 
            return TC;
        }

        public bool Update_Fees_txn(int Uid, int Mno, int Yno)
        {
            string Query = @"Update student_fees_collection Set Is_Paid = 1 Where University_ID = @University_ID AND Is_Paid = 0 AND Month(Txn_Date) = @Mno AND Year(Txn_Date) = @Yno";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@University_ID", Uid);
            cmd.Parameters.AddWithValue("@Mno", Mno);
            cmd.Parameters.AddWithValue("@Yno", Yno);            
            bool val = Fnc.CURDCommands(cmd);
            return val;
        }

        public bool Update_RP_Txn_Details(RazorPG_Update RPU)
        {
            string Query = @"UPDATE razorpay_txns SET ResponseDateTime = now(), PG_Status_Id = @PG_Status_Id, razorpay_payment_id = @razorpay_payment_id, razorpay_signature = @razorpay_signature WHERE Order_ID = @Order_ID";
            MySqlCommand cmd = new MySqlCommand(Query);
            cmd.Parameters.AddWithValue("@Order_ID", RPU.OrderId);
            cmd.Parameters.AddWithValue("@PG_Status_Id", RPU.PG_Status_id);
            cmd.Parameters.AddWithValue("@razorpay_payment_id", RPU.razorpay_payment_id);         
            cmd.Parameters.AddWithValue("@razorpay_signature", RPU.RP_signature);
            bool val = Fnc.CURDCommands(cmd);
            return val;
        }


        public bool Update_payment_string(PG_String_Update u)
		{
			string Query = "UPDATE payment_string SET ResponseString=@ResponseString, RespoStringData=@RespoStringData, ResponseDateTime=now(),PG_Status=@PG_Status WHERE Ayong_Txn_ID=@Ayong_Txn_ID and Txn_Number=@Txn_Number;";
			MySqlCommand cmd = new MySqlCommand(Query);
			cmd.Parameters.AddWithValue("@ResponseString", u.ResponseString);
			cmd.Parameters.AddWithValue("@RespoStringData", u.RespoStringData);
			cmd.Parameters.AddWithValue("@PG_Status", u.PG_Status);
			cmd.Parameters.AddWithValue("@Ayong_Txn_ID", u.Ayong_Txn_ID);
			cmd.Parameters.AddWithValue("@Txn_Number", u.Txn_Number);
			bool val = Fnc.CURDCommands(cmd);
			return val;
		}

        public bool CheckEnrollmentExists(string enrollmentNo)
        {
         
          
            {
                string Query = "SELECT Student_ID FROM Students WHERE EnrollmentNo = @EnrollmentNo";
                MySqlCommand cmd = new MySqlCommand(Query);
                cmd.Parameters.AddWithValue("@EnrollmentNo", enrollmentNo);
                bool val = Fnc.CURDCommands(cmd);
                return val;
            }
        }

        /*Payment Gateway End*/

    }

	public class VerificationCode
	{
		public string Mobile_Code { get; set; }
		public string Email_Code { get; set; }
	}
 
  
}