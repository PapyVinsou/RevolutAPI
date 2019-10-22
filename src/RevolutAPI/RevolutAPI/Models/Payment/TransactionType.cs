namespace RevolutAPI.Models.Payment
{
    public class TransactionType
    {
        public const string Atm = "atm";
        public const string CardPayment = "card_payment";
        public const string CardRefund = "card_refund";
        public const string CardChargeback = "card_chargeback";
        public const string CardCredit = "card_credit";
        public const string Exchange = "exchange";
        public const string Transfer = "transfer";
        public const string Loan = "loan";
        public const string Fee = "fee";
        public const string Refund = "refund";
        public const string Topup = "topup";
        public const string TopupReturn = "topup_return";
        public const string Tax = "tax";
        public const string TaxRefund = "tax_refund";
    }
}