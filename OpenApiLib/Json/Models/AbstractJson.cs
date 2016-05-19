using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace OpenApiLib.Json.Models
{
    [DataContract]
    public abstract class AbstractJson
	{

        public override string ToString()
		{
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(this.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, this);
            return ms.ToString();
		}
	}
}

