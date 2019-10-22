namespace RevolutAPI.Models.Payment
{
    public class LegData
    {
        public string LegId { get; set; }
        public string AccountId { get; set; }
        public string Description { get; set; }
        public CounterPartyData CounterParty { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public double BillAmount { get; set; }
        public string BillCurrency { get; set; }
    }
}