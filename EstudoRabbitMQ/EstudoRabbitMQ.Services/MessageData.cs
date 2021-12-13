using System;
using System.Collections.Generic;

namespace EstudoRabbitMQ.Services
{
    public class MessageData
    {
        public MessageData(string exchangeName, string routingKey, ReadOnlyMemory<byte> body, Dictionary<string, object> headers = null)
        {
            ExchangeName = exchangeName;
            RoutingKey = routingKey;
            Body = body;
            Headers = headers ?? new Dictionary<string, object>
            {
                ["content-type"] = "application/json"
            };
        }

        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public ReadOnlyMemory<byte> Body { get; set; }
        public IDictionary<string, object> Headers { get; set; }
    }
}
