using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Private_University.Models
{
    public class RazorPG_Request
    {
        public string TxnID { get; set; }
        public DateTime TxnDate { get; set; }
        public string Order_ID { get; set; }
        public decimal One_Percent_Amt { get; set; }
        public decimal Penal_Interest { get; set; }
        public decimal Payble_Amt { get; set; }
        public int UniversityID { get; set; }
        public string signature { get; set; } //login, pass, ttype, prodid, txnid, amt, txncurr
        public string UniversityName { get; set; }
        public string UniversityEmail { get; set; }
        public string UniversityMobile { get; set; }
        public string BillingAddress { get; set; }
        public string BillingMonth { get; set; }
        public string BillingYear { get; set; }         
        public string PG_Status_id { get; set; }
        public string RazorpayKey { get; internal set; }
    }

        public class RazorPG_Update
        {
            public string OrderId { get; set; }
            public string PG_Status_id { get; set; }
            public string razorpay_payment_id { get; set; }       
            public string RP_signature { get; set; }
            public string Amount { get; set; }
        }

        public class TxnCheck
        {
            public string OrderId { get; set; }
            public string TxnNumber { get; set; }
            public string TxnAmount { get; set; }
            public string PG_Status_id { get; set; }
            public int UniversityId { get; set; }
            public int FMonth { get; set; } 
            public int FYear { get; set; }
        }

        public class PGTxnList
        {
             public int count { get; set;}
             public string FeesMonth { get; set; }
             public string TxnNumber { get; set; }
             public string OrderId { get; set; }
             public string OnePerAmt { get; set; }
             public string Penal_Interest { get; set; }
             public string TotalAmount { get; set; }
             public string PG_Status { get; set; }
             public string TxnDateTime { get; set; }
             public string Attempts { get; set; }
        }
}