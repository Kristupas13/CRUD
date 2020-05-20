using Newtonsoft.Json;
using System.Net;

namespace CRUDWebService.BusinessLayer.Base.DTO
{
    public class ErrorDTO
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IsError { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public HttpStatusCode StatusCode { get; set; }
    }
}
