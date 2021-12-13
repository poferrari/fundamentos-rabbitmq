using EstudoRabbitMQ.Services;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace EstudoRabbitMQ.Publisher.Examples
{
    public static class FanoutExchange
    {
        public static void Run()
        {
            IModel channel = MessageBrokerService.GetConnection().CreateModel();

            var prefixDirect = PrefixMessageBrokerConst.ExampleFanout;

            var exchangeName = prefixDirect.GetExchange();
            channel.ExchangeDeclare(exchangeName, "fanout", durable: true, autoDelete: false);

            var queueName1 = $"{prefixDirect}-1".GetQueue();
            AddQueueAndBind(channel, exchangeName, queueName1);

            var queueName2 = $"{prefixDirect}-2".GetQueue();
            AddQueueAndBind(channel, exchangeName, queueName2);

            var queueName3 = $"{prefixDirect}-3".GetQueue();
            AddQueueAndBind(channel, exchangeName, queueName3);

            var publisher = new ExchangePublisher(channel);
            publisher.Publish(exchangeName, string.Empty,
                new ExampleMessage { Description = "Exemplo Fanout Exchange" });
        }

        private static void AddQueueAndBind(IModel channel, string exchangeName, string queueName)
        {
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false,
                 arguments: new Dictionary<string, object>()
                 {
                     ["x-queue-mode"] = "lazy" //trazer o mínimo possível para memória
                 });

            channel.QueueBind(queueName, exchangeName, string.Empty);
        }
    }
}
