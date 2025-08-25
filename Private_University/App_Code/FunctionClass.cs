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
namespace Private_University.App_Code
{
    public class FunctionClass
    {
        public MySqlTransaction CmdTxn;
        public MySqlConnection conn;
        public MySqlConnection MySqlCon()
        {
            conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString);
            return conn;
        }

        #region 'Encode Decode Concept'

        public string Encrypt(string input)//, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = System.Text.Encoding.Unicode.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }
        public string Decrypt(string hexInput)//, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return System.Text.Encoding.Unicode.GetString(bytes);
        }

        public string GetMd5HashWithMySecurityAlgo(string input)
        {

            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.  
            return sBuilder.ToString();
        }

        #endregion

        #region 'OTP and Recaptcha'

        //public VerificationCode Generate_OTP_Code()
        //{
        //    Random rnd = new Random();
        //    string otp = rnd.Next(100000, 999999).ToString();
        //    VerificationCode Vr = new VerificationCode()
        //    {
        //        Mobile_Code = otp,
        //        Email_Code = Guid.NewGuid().ToString(),
        //    };
        //    return Vr;
        //}

        public string[] Recaptcha_Math()
        {
            Random rnd = new Random();
            int ValueA = rnd.Next(10, 50);
            int ValueB = rnd.Next(10, 50);
            int Value = ValueA + ValueB;
            return new string[] { ValueA.ToString() + "+" + ValueB.ToString() + "=", Value.ToString() };
        }

        /// <summary>
        /// Get Velid Rendom String of 10 Char.
        /// </summary>
        /// <returns>10 Char String includes UpperCase, LowerCase and Numbers.</returns>
        public string ValidRendomChar()
        {
            // Create a string of characters, numbers, special characters that allowed in the password  
            //  string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-"; 
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            // Select one random character at a time from the string  
            // and create an array of chars  
            char[] chars = new char[10];
            for (int i = 0; i < 10; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }


        #endregion

        #region 'Commands'

        #region 'Get Data'

        /// <summary>
        /// Get Data Table by passing MySqlCommand Query
        /// </summary>
        /// <param name="Command">SqlCommand</param>
        /// <returns>Data Table as Result</returns>
        public DataTable GetDataTable(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                MySqlDataAdapter da = new MySqlDataAdapter(Command);
                DataTable Result = new DataTable();
                da.Fill(Result);
                return Result;
            }
        }
        /// <summary>
        /// Get Data Table by passing MySqlCommand Query
        /// </summary>
        /// <param name="Command">SqlCommand</param>
        /// <returns>Reader as Result</returns>
        public MySqlDataReader GetDataReader(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                return Command.ExecuteReader();
            }
        }

        /// <summary>
        /// Get Data Table by passing String Query
        /// </summary>
        /// <param name="Query">Query only as string</param>
        /// <returns>Data Table as Result</returns>
        public DataTable GetDataTable_FromQuery(string Query)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                MySqlCommand Command = new MySqlCommand(Query);
                Command.Connection = conn;
                MySqlDataAdapter da = new MySqlDataAdapter(Command);
                DataTable Result = new DataTable();
                da.Fill(Result);
                return Result;
            }
        }
        /// <summary>
        /// Check Data Exist or not by passing MySqlCommand Query
        /// </summary>
        /// <param name="Command">SqlCommand</param>
        /// <returns>True if exist or False</returns>
        public bool GetCheckDataExist(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                MySqlDataAdapter da = new MySqlDataAdapter(Command);
                DataTable Result = new DataTable();
                da.Fill(Result);
                return Result.Rows.Count > 0 ? true : false;
            }
        }
        /// <summary>
        /// Check Value is exist or Not
        /// </summary>
        /// <param name="Table_Name">Name of Table</param>
        /// <param name="Column_Name">Name of Column of table</param>
        /// <param name="Column_Value">Value of Parameter</param>
        /// <returns></returns>
        public bool GetCheckDataExist_By_Column(string Table_Name, string Column_Name, string Column_Value)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                MySqlCommand Command = new MySqlCommand();
                Command.CommandText = "Select " + Column_Name + " from " + Table_Name + " where " + Column_Name + "=" + Column_Value;
                // Command.CommandText = string.Format("Select {1} from {0} where {1}='{2}'", Table_Name, Column_Name, Column_Value);
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                MySqlDataAdapter da = new MySqlDataAdapter(Command);
                DataTable Result = new DataTable();
                da.Fill(Result);
                return Result.Rows.Count > 0 ? true : false;
            }
        }
        /// <summary>
        /// Get Single value by MySqlCommand
        /// </summary>
        /// <param name="Command">SqlCommand</param>
        /// <returns>Single String Value</returns>
        public string GetDataTable_SingleString(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                return Command.ExecuteScalar().ToString();
            }
        }
        public int GetDataTable_Singleint(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                return (int)(Command.ExecuteScalar());
            }
        }
        public decimal GetDataTable_SingleDecimal(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                return (decimal)(Command.ExecuteScalar());
            }
        }
        public string DataTable_to_Json_String(DataTable Dt)
        {
            return JsonConvert.SerializeObject(Dt);
        }

        #endregion

        #region 'Curd / Insert Without Transection'

        /// <summary>
        /// Insert Command
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public bool InsertCommands(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                int Result = Command.ExecuteNonQuery();
                if (Result > 0)
                    return true;
                else
                    return false;
            }
        }
        public int InsertCommands_AndGetting_ID(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                Command.ExecuteNonQuery();              
                int Result = Convert.ToInt32(Command.LastInsertedId);
                if (Result > 0)
                    return Result;
                else
                    return 0;
            }
        }
        /// <summary>
        /// Update and Delete Command
        /// </summary>
        /// <param name="Command"></param>
        /// <returns>LAst inserted ID in Int</returns>
        public bool CURDCommands(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                int Result = Command.ExecuteNonQuery();
                if (Result > 0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// Inser with getting last inserted ID
        /// </summary>
        /// <param name="Command"></param>
        /// <returns>Int Value</returns>
        public int InsertCommands_LastInsertID(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                //int Result = Command.ExecuteNonQuery();
                return Convert.ToInt32(Command.ExecuteScalar());
            }
        }
        public List<ListItem> GetDropDownFilling(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                List<ListItem> DropDownFill = new List<ListItem>();
                using (MySqlDataReader sdr = Command.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        DropDownFill.Add(new ListItem
                        {
                            Value = sdr["SelectValue"].ToString(),
                            Text = sdr["SelectText"].ToString()
                        });
                    }
                }
                return DropDownFill;
            }
        }

        #endregion

        #region 'Curd / Insert With Transection'

        public bool InsertCommands_txn(MySqlCommand Command, MySqlTransaction CmdTxn)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                Command.Transaction = CmdTxn;
                int Result = Command.ExecuteNonQuery();
                if (Result > 0)
                    return true;
                else
                    return false;
            }

        }
        public bool CURDCommands_txn(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }

                Command.Connection = conn;
                CmdTxn = conn.BeginTransaction();
                Command.Transaction = CmdTxn;
                int Result = Command.ExecuteNonQuery();
                if (Result > 0)
                    return true;
                else
                    return false;
            }
        }
        public int InsertCommands_LastInsertID_txn(MySqlCommand Command)
        {
            using (MySqlConnection conn = MySqlCon())
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                Command.Connection = conn;
                CmdTxn = conn.BeginTransaction();
                Command.Transaction = CmdTxn;
                int Result = Convert.ToInt32(Command.ExecuteScalar());
                if (Result > 0)
                    return Result;
                else
                    return 0;
            }
        }

        #endregion

        #endregion

        #region 'Send SMS and Email'

        public void SendEmail(string To, string Subject, string Message, bool IsHtml)
        {
            try
            {
                string from = ConfigurationManager.AppSettings["smtp_sender_email"];
                string password = ConfigurationManager.AppSettings["smtp_password"];
                string hostname = ConfigurationManager.AppSettings["smtp_host"];
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["smtp_port"]);

                string Msg = @"<div style=';min-height:950px;margin:100px 0px 0px 100px;'><div style='background-color: #e9ecef;width:600px;border-radius:20px;min-height:250px;'>
<div style='background-color:#000;width:100%;min-height:50px;color:white;text-align:center;padding-top:20px;border-top-left-radius: 20px;border-top-right-radius: 20px;font-family:Monospace;font-size:1.8em;'>" + Subject + @"</div>
<div style='padding:15px;width:100%;font-family:Courier New;font-size:1em;font-weight:bold;'>" + Message + "</div><div style='padding:15px;width:100%;font-family:Courier New;font-size:1em;font-weight:bold;'><i>Thnaks,<br/>Directorate of Technical Education<br/>{Government of Chattisgarh}<br/>Naya Raipur (Chhattisgarh)</i></div></div></div>";

                MailMessage mailMsg = new MailMessage();
                mailMsg.From = new System.Net.Mail.MailAddress(from);
                mailMsg.IsBodyHtml = true;
                mailMsg.BodyEncoding = UTF8Encoding.UTF8;
                mailMsg.Body = Msg;// Message;
                mailMsg.To.Add(new MailAddress(To));
                mailMsg.Subject = Subject;
                System.Net.Mail.SmtpClient Smtp = new SmtpClient();
                Smtp.Host = hostname; // for example gmail smtp server... "smtp.gmail.com"
                Smtp.EnableSsl = IsHtml;// Convert.ToBoolean(ConfigurationManager.AppSettings["smtp_EnableSSL"]);
                Smtp.Port = port;
                Smtp.Credentials = new System.Net.NetworkCredential(from, password);
                Smtp.Send(mailMsg);
            }
            catch 
            {
            }
        }
        public string SendSMS_T(string mobileNo, string templateid, string message)
        {
            if (!string.IsNullOrEmpty(templateid))
            {

                //  string apiUrl = ConfigurationManager.AppSettings["SMS_API"];
                string username = "CGCHIPS-CMFLG";
                string senderid = "CGSSDG";
                string password = "Chips@12345";
                string secureKey = "36185c45-27b1-4f18-b6f1-9680cf5b99c3";

                //string username = "CGCHIPS-COVID19";
                //string senderid = "CGSSDG";
                //string password = "chips@123456";
                //string secureKey = "e5a571e9-382f-4a14-b670-5d5cf26a88d5";

                //Latest Generated Secure Key

                Stream dataStream;
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);//SecurityProtocolType.Tls12; //forcing .Net framework to use TLSv1.2
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://msdgweb.mgov.gov.in/esms/sendsmsrequestDLT");
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                request.ServicePoint.ConnectionLimit = 1;
                ((HttpWebRequest)request).UserAgent = "Mozilla/4.0 (compatible; MSIE 5.0; Windows 98; DigExt)";
                request.Method = "POST";
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();

                string U_Convertedmessage = "";
                foreach (char c in message)
                {
                    int j = (int)c;
                    string sss = "&#" + j + ";";
                    U_Convertedmessage = U_Convertedmessage + sss;
                }

                string encryptedPassword = EncryptedPasswod(password);
                string NewsecureKey = HashGenerator(username.Trim(), senderid.Trim(), U_Convertedmessage.Trim(), secureKey.Trim());
                //  String smsservicetype = "singlemsg"; //For single message.
                string smsservicetype = "unicodemsg";
                string query = "username=" + HttpUtility.UrlEncode(username.Trim()) +
                    "&password=" + HttpUtility.UrlEncode(encryptedPassword) +
                    "&smsservicetype=" + HttpUtility.UrlEncode(smsservicetype) +
                   "&content=" + HttpUtility.UrlEncode(U_Convertedmessage.Trim()) +
                    "&mobileno=" + HttpUtility.UrlEncode(mobileNo) +
                    "&senderid=" + HttpUtility.UrlEncode(senderid.Trim()) +
                  "&key=" + HttpUtility.UrlEncode(NewsecureKey.Trim()) +
                  "&templateid=" + HttpUtility.UrlEncode(templateid.Trim());

                byte[] byteArray = Encoding.ASCII.GetBytes(query);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                string Status = ((HttpWebResponse)response).StatusDescription;
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseFromServer;
            }
            else
                return "Template ID is Empty.";
        }
        protected String EncryptedPasswod(String password)
        {

            byte[] encPwd = Encoding.UTF8.GetBytes(password);
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA1");
            byte[] pp = sha1.ComputeHash(encPwd);
            // static string result = System.Text.Encoding.UTF8.GetString(pp);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in pp)
            {

                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();

        }
        /// <summary>
        /// Method to Generate hash code  
        /// </summary>
        /// <param name= "secure_key">your last generated Secure_key </param>
        protected String HashGenerator(String Username, String sender_id, String message, String secure_key)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append(Username).Append(sender_id).Append(message).Append(secure_key);
            byte[] genkey = Encoding.UTF8.GetBytes(sb.ToString());
            //static byte[] pwd = new byte[encPwd.Length];
            HashAlgorithm sha1 = HashAlgorithm.Create("SHA512");
            byte[] sec_key = sha1.ComputeHash(genkey);

            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < sec_key.Length; i++)
            {
                sb1.Append(sec_key[i].ToString("x2"));
            }
            return sb1.ToString();
        }

        #endregion
    }

    class MyPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            return true;
        }

    }
}