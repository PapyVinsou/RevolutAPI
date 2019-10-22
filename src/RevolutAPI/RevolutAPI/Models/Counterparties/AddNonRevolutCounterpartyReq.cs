namespace RevolutAPI.Models.Counterparties
{
    public class AddNonRevolutCounterpartyReq
    {
        public string CompanyName { get; set; }
        public IndividualNameData IndividualName { get; set; }
        public string BankCountry { get; set; }
        public string Currency { get; set; }
        public string AccountNo { get; set; }
        public string SortCode { get; set; }
        public string RoutingNumber { get; set; }
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public AddressData Address { get; set; }
    }
}