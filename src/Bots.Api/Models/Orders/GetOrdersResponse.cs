using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bots.Api.Models.Orders {
    public class GetOrdersResponse {
        [JsonProperty("success")]
        public bool Success { get; set; }
        
        [JsonProperty("orders")]
        public IEnumerable<GetOrderInfoResponse> Orders { get; set; }
    }
}