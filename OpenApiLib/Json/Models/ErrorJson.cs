using System.Runtime.Serialization;

namespace OpenApiLib.Json.Models
{
    [DataContract]
    public class ErrorJson : AbstractJson
	{
        [DataMember(Name = "errorCode")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
	}
}
