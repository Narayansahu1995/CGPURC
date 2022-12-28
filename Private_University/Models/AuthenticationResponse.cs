using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Private_University.Models
{
	public class AuthenticationResponse
	{
		public bool Success { get; set; }//if success is true value else success will be false for fail while login authentication
		public int Login_ID { get; set; }
		public int Session_ID { get; set; }
		public string Academic_Session { get; set; }
		public string Name { get; set; }
		public string Designation { get; set; }
		public string Email_ID { get; set; }
		public string Mobile_Number { get; set; }
		public int Role_ID { get; set; }
		public int University_ID { get; set; }
		public string Message { get; set; }
        public string PwdRstDateTime { get; set; }
        public string UserName { get; set; }
	}
}