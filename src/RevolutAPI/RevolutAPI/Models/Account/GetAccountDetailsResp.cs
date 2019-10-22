using System.Collections.Generic;

namespace RevolutAPI.Models.Account
{
    public class GetAccountDetailsResp
    {
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string UniqueReference { get; set; }
        public string AccountNo { get; set; }
        public string SortCode { get; set; }
        public string RoutingNumber { get; set; }
        public string Beneficiary { get; set; }
        public BeneficiaryAddress BeneficiaryAddress { get; set; }
        public string BankCountry { get; set; }
        public List<string> Schemas { get; set; } // TODO: try map to enum
        public bool Pooled { get; set; }
        public EstimatedTime EstimatedTime { get; set; }
    }
}