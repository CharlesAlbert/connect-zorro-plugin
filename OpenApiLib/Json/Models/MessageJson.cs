namespace OpenApiLib.Json.Models
{
    public class MessageJson<T> : AbstractJson
	{
		public T Data { get; set; }
		public ErrorJson Error { get; set; }
	}
}
