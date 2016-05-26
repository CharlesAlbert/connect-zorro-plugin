namespace OpenApiLib.Json.Models
{
    public class ErrorJson : AbstractJson
	{
		public string ErrorCode { get; set; }
		public string Description { get; set; }
	}
}
