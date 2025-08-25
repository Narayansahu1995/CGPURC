using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Private_University.Models
{
	public class Atom_PG_Request_common
	{
		public int Login { get; set; }
		public string Password { get; set; }
		public string TxnType { get; set; }
		public string ProductId { get; set; }
		public string TxnCurrency { get; set; }
		public int txnServiceChargeamt { get; set; }
		public string Clientcode { get; set; }
		public int CustomerAccountNo { get; set; }
		public string ReturnURL { get; set; }
	}

	public class Atom_PG_Request
	{
		public string txnID { get; set; }
		public DateTime TxnDate { get; set; }
		public string Ayong_Txn_ID { get; set; }
		public string Amount { get; set; }
		public string signature { get; set; } //login, pass, ttype, prodid, txnid, amt, txncurr
		public string CustomerName { get; set; }
		public string CustomerEmail { get; set; }
		public string CustomerMobile { get; set; }
		public string BillingAddress { get; set; }
		public string BillingMonth { get; set; }
		public string BillingYear { get; set; }
		public int BankIdentifier { get; set; }
		public string UnEncryptedURLData { get; set; }
	}


	public class PG_String_Insert
	{
		public int Ayong_Txn_ID { get; set; }
		public int University_ID { get; set; }
		public string Txn_Number { get; set; }
		public string RequestString { get; set; }
		public string ReqStringData { get; set; }
        public string Amount { get; set; }
    }

    public class RazorPay_String_Insert
    {
        public int Order_ID { get; set; }
        public int University_ID { get; set; }
        public string Txn_Number { get; set; }
        public string RequestString { get; set; }
        public string ReqStringData { get; set; }
        public string Amount { get; set; }
    }

    public class PG_String_Update
	{
		public int Ayong_Txn_ID { get; set; }
		public string Txn_Number { get; set; }
		public string ResponseString { get; set; }
		public string RespoStringData { get; set; }
		public string PG_Status { get; set; }
	}

}