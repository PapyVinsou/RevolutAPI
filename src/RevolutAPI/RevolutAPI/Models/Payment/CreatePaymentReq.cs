using System.ComponentModel.DataAnnotations;

namespace RevolutAPI.Models.Payment
{
    public class CreatePaymentReq
    {
        [Required(AllowEmptyStrings = false)] public string RequestId { get; set; }

        [Required(AllowEmptyStrings = false)] public string AccountId { get; set; }

        [Required] public ReceiverData Receiver { get; set; }

        [Required] public double Amount { get; set; }

        [Required(AllowEmptyStrings = false)] public string Currency { get; set; }

        [Required(AllowEmptyStrings = false)] public string Reference { get; set; }
    }
}