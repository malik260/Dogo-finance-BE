using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogoFinance.DataAccess.Layer.Models.ViewModels
{
    public class FundRequest
    {
        public decimal Amount { get; set; }
        public string Email { get; set; }
    }

    public class ChargeRequest
    {
        public string Reference { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string CVV { get; set; }
        public string Pin { get; set; }
    }

    public class AuthorizeRequest
    {
        public string Reference { get; set; }
        public string Otp { get; set; }
    }
}
