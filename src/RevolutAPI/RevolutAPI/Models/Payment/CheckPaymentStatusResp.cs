using System;
using System.Collections.Generic;

namespace RevolutAPI.Models.Payment
{
    public class CheckPaymentStatusResp
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string RequestId { get; set; }
        public string State { get; set; } // TODO: try parse as enum 
        public string ReasonCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public string ScheduledFor { get; set; } // optional
        public string Reference { get; set; }
        public MerchantData Merchant { get; set; } // optional
        public CardData Card { get; set; } // optional
        public List<LegData> Legs { get; set; }
    }

    public class CounterPartyData
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string AccountId { get; set; }
    }

    public class CardData
    {
        public string CardNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}