namespace OpenApiLib.Json.Models
{
	public class CursorMessageJson<T> : MessageJson<T>
	{
		public string Next { get; set; }
	}
}
