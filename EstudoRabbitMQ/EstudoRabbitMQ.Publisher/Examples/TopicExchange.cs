using EstudoRabbitMQ.Services;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace EstudoRabbitMQ.Publisher.Examples
{
    public static class TopicExchange
    {
        public static void Run()
        {
            IModel channel = MessageBrokerService.GetConnection().CreateModel();
            var publisher = new ExchangePublisher(channel);

            var prefixDirect = PrefixMessageBrokerConst.ExampleTopic;

            var exchangeName = prefixDirect.GetExchange();
            channel.ExchangeDeclare(exchangeName, "topic", durable: true, autoDelete: false);

            var queueNameUserUpdated = "user-updated".GetQueue();
            var routingKeyUserUpdated = "user.updated";
            AddQueueAndBind(channel, exchangeName, routingKeyUserUpdated, queueNameUserUpdated);

            var queueNameUserCreated = "user-created".GetQueue();
            var routingKeyUserCreated = "user.created";
            AddQueueAndBind(channel, exchangeName, routingKeyUserCreated, queueNameUserCreated);

            var queueNameOrderUpdated = "order-updated".GetQueue();
            var routingKeyOrderUpdated = "order.updated";
            AddQueueAndBind(channel, exchangeName, routingKeyOrderUpdated, queueNameOrderUpdated);

            var queueNameAllUpdated = "all-updated".GetQueue();
            var routingKeyAllUpdated = "*.updated";
            AddQueueAndBind(channel, exchangeName, routingKeyAllUpdated, queueNameAllUpdated);

            var queueNameAllUser = "all-user".GetQueue();
            var routingKeyAllUser = "user.*";
            AddQueueAndBind(channel, exchangeName, routingKeyAllUser, queueNameAllUser);

            publisher.Publish(exchangeName, routingKeyUserCreated,
              new ExampleMessage { Description = $"Exemplo Topic Exchange ({exchangeName}) --> {routingKeyUserCreated}" });

            publisher.Publish(exchangeName, routingKeyUserUpdated,
                new ExampleMessage { Description = $"Exemplo Topic Exchange ({exchangeName}) --> {routingKeyUserUpdated}" });

            publisher.Publish(exchangeName, "order.updated",
                new ExampleMessage { Description = $"Exemplo Topic Exchange ({exchangeName}) --> order.updated" });

            publisher.Publish(exchangeName, "user.deleted",
                new ExampleMessage { Description = $"Exemplo Topic Exchange ({exchangeName}) --> user.deleted" });

            publisher.Publish(exchangeName, "user.deleted",
                new ExampleMessage { Description = $"Exemplo Topic Exchange ({exchangeName}) --> user.deleted" });
        }

        private static void AddQueueAndBind(IModel channel, string exchangeName, string routingKey, string queueName)
        {
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false,
                             arguments: new Dictionary<string, object>()
                             {
                                 ["x-queue-mode"] = "lazy" //trazer o mínimo possível para memória
                             });

            channel.QueueBind(queueName, exchangeName, routingKey);
        }
    }
}
