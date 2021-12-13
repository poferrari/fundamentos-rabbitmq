using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;

namespace EstudoRabbitMQ.Services
{
    public static class MessageSerializerUtil
    {
        public static TResponse Deserialize<TResponse>(BasicDeliverEventArgs eventArgs)
        {
            string message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonConvert.DeserializeObject<TResponse>(message);
        }

        public static byte[] Serialize<T>(T objectToSerialize)
        {
            var serializedMessage = JsonConvert.SerializeObject(objectToSerialize);
            return Encoding.UTF8.GetBytes(serializedMessage);
        }
    }
}
