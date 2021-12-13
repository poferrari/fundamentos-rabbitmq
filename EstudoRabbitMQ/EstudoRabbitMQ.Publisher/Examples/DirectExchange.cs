using EstudoRabbitMQ.Services;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace EstudoRabbitMQ.Publisher.Examples
{
    public static class DirectExchange
    {
        public static void Run()
        {
            IModel channel = MessageBrokerService.GetConnection().CreateModel();

            var prefixDirect = PrefixMessageBrokerConst.ExampleDirect;

            var exchangeName = prefixDirect.GetExchange();
            channel.ExchangeDeclare(exchangeName, "direct", durable: true, autoDelete: false);

            var queueName = prefixDirect.GetQueue();
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false,
                 arguments: new Dictionary<string, object>()
                 {
                     ["x-queue-mode"] = "lazy" //trazer o mínimo possível para memória
                 });

            var routingKey = prefixDirect.GetRoutingKey();
            channel.QueueBind(queueName, exchangeName, routingKey);

            var publisher = new ExchangePublisher(channel);
            publisher.Publish(exchangeName, routingKey,
                new ExampleMessage { Description = "Exemplo Direct Exchange" });
        }
    }
}
