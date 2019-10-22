using System.ComponentModel.DataAnnotations;

namespace RevolutAPI.Models.Payment
{
    public class ReceiverData
    {
        [Required(AllowEmptyStrings = false)] public string CounterpartyId { get; set; }

        [Required(AllowEmptyStrings = false)] public string AccountId { get; set; }
    }
}