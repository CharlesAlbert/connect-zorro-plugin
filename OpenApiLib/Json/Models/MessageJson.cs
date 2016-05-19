using System.Runtime.Serialization;

namespace OpenApiLib.Json.Models
{
    [DataContract]
    public class MessageJson<T> : AbstractJson
	{
        [DataMember(Name = "data")]
        public T Data { get; set; }

        [DataMember(Name = "error")]
        public ErrorJson Error { get; set; }
	}
}
