namespace RevolutAPI.Models.Payment
{
    public class TransferState
    {
        public static string Pending = "pending";
        public static string Completed = "completed";
        public static string Declined = "declined";
        public static string Failed = "failed";
        public static string Cancelled = "cancelled";
    }
}