using Newtonsoft.Json;

namespace OpenApiLib.Json.Models
{
    public abstract class AbstractJson
	{
		public override string ToString()
		{
			return JsonConvert.SerializeObject (this);
		}
	}
}

