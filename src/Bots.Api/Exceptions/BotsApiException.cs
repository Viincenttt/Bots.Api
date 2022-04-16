using System;

namespace Bots.Api.Exceptions {
    public class BotsApiException : Exception {
        public string Response { get; private set; }
        
        public BotsApiException(string message, string json) : base(message) {
            Response = json;
        }
    }
}