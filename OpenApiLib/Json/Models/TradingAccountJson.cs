using System.Runtime.Serialization;

namespace OpenApiLib.Json.Models
{
    [DataContract]
    public class TradingAccountJson : AbstractJson
	{
        [DataMember(Name = "accountId")]
        public long AccountId { get; set; }

        [DataMember(Name = "accountNumber")]
        public long AccountNumber{ get; set; }

        [DataMember(Name = "live")]
        public bool Live { get; set; }

        [DataMember(Name = "brokerName")]
        public string brokerName { get; set; }

        [DataMember(Name = "brokerTitle")]
        public string BrokerTitle { get; set; }

        [DataMember(Name = "brokerCode")]
        public long? BrokerCode { get; set; }

        [DataMember(Name = "depositCurrency")]
        public string DepositCurrency { get; set; }

        [DataMember(Name = "traderRegistrationTimestamp")]
        public long TraderRegistrationTimestamp { get; set; }

        [DataMember(Name = "traderAccountType")]
        public string TraderAccountType { get; set; }

        [DataMember(Name = "leverage")]
        public int Leverage { get; set; }

        [DataMember(Name = "balance")]
        public long Balance { get; set; }

        [DataMember(Name = "deleted")]
        public bool Deleted { get; set; }

        [DataMember(Name = "accountStatus")]
        public string AccountStatus { get; set; }
    }
}
