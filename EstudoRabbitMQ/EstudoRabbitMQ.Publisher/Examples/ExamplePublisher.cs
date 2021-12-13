using EstudoRabbitMQ.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EstudoRabbitMQ.Publisher.Examples
{
    public static class ExamplePublisher
    {
        private const string ExchangeTypeFanout = "fanout";
        private const string ExchangeTypeTopic = "topic";
        private const string QueueMode = "x-queue-mode";
        private const string LazyQueueMode = "lazy";
        private const int DefaultTimeToLiveSeconds = 100000;

        public static void Run()
        {
            IModel channel = MessageBrokerService.GetConnection().CreateModel();

            string fileEventUnroutedExchange = CreateFileEventUnroutedExchangeAndQueue(channel);

            string eventExchange = CreateFileEventExchange(channel, fileEventUnroutedExchange);

            CreateFileEventDeadLetter(channel, eventExchange);

            CreateFileEventManualExchangeAndQueue(channel);

            CreateFileEventQueue(channel, eventExchange, PrefixMessageBrokerConst.FileZip);
            CreateFileEventQueue(channel, eventExchange, PrefixMessageBrokerConst.FileXml);

            CreateFileUnmappedExchangeAndQueue(channel);

            var publisher = new ExchangePublisher(channel);

            Console.Write("Digite a quantidade de mensagens? ");
            int.TryParse(Console.ReadLine(), out int total);

            var routeFileZip = PrefixMessageBrokerConst.FileZip.GetRoutingKey();
            var routeFileXml = PrefixMessageBrokerConst.FileXml.GetRoutingKey();
            var routes = new[] { routeFileZip, routeFileXml };

            for (int i = 0; i < total; i++)
            {
                var routingKey = routes.OrderBy(t => Guid.NewGuid()).First();
                var message = $" [{i}] Exemplo caso real {eventExchange} --> {routingKey}";

                Console.WriteLine(message);
                publisher.Publish(eventExchange, routingKey,
                    new ExampleMessage { Description = message });
            }
        }

        private static string CreateFileEventUnroutedExchangeAndQueue(IModel channel)
        {
            var prefixFileEventUnrouted = PrefixMessageBrokerConst.FileEventUnrouted;
            var fileEventUnroutedExchange = prefixFileEventUnrouted.GetExchange();
            channel.ExchangeDeclare(fileEventUnroutedExchange, ExchangeTypeFanout, durable: true, autoDelete: false);

            var fileEventUnroutedQueue = prefixFileEventUnrouted.GetQueue();
            channel.QueueDeclare(fileEventUnroutedQueue, durable: true, exclusive: false, autoDelete: false,
              arguments: new Dictionary<string, object>()
              {
                  [QueueMode] = LazyQueueMode
              });
            channel.QueueBind(fileEventUnroutedQueue, fileEventUnroutedExchange, string.Empty);
            return fileEventUnroutedExchange;
        }

        private static string CreateFileEventExchange(IModel channel, string fileEventUnroutedExchange)
        {
            var prefixFileEvent = PrefixMessageBrokerConst.FileEvent;
            var eventExchange = prefixFileEvent.GetExchange();
            channel.ExchangeDeclare(eventExchange, ExchangeTypeTopic, durable: true, autoDelete: false,
              arguments: new Dictionary<string, object>()
              {
                  ["alternate-exchange"] = fileEventUnroutedExchange
              });
            return eventExchange;
        }

        private static void CreateFileEventDeadLetter(IModel channel, string analysisScoreEventExchange)
        {
            var prefixFileEventDeadLetter = PrefixMessageBrokerConst.FileEventDeadLetter;
            var fileEventDeadLetterExchange = prefixFileEventDeadLetter.GetExchange();
            channel.ExchangeDeclare(fileEventDeadLetterExchange, ExchangeTypeTopic, durable: true, autoDelete: false);

            var fileEventDeadLetterQueue = prefixFileEventDeadLetter.GetQueue();
            channel.QueueDeclare(fileEventDeadLetterQueue, durable: true, exclusive: false, autoDelete: false,
              arguments: new Dictionary<string, object>()
              {
                  [QueueMode] = LazyQueueMode,
                  ["x-message-ttl"] = DefaultTimeToLiveSeconds,
                  ["x-dead-letter-exchange"] = analysisScoreEventExchange,
              });

            channel.QueueBind(fileEventDeadLetterQueue, fileEventDeadLetterExchange, "file.*");
        }

        private static void CreateFileEventQueue(IModel channel, string fileEventEventExchange, string prefixFileQueue)
        {
            var fileQueue = prefixFileQueue.GetQueue();
            channel.QueueDeclare(fileQueue, durable: true, exclusive: false, autoDelete: false,
              arguments: new Dictionary<string, object>()
              {
                  [QueueMode] = LazyQueueMode
              });

            var fileRoutingKey = prefixFileQueue.GetRoutingKey();
            channel.QueueBind(fileQueue, fileEventEventExchange, fileRoutingKey);
        }

        private static void CreateFileEventManualExchangeAndQueue(IModel channel)
        {
            var prefixFileEventManual = PrefixMessageBrokerConst.FileEventManual;
            var fileEventManualExchange = prefixFileEventManual.GetExchange();
            channel.ExchangeDeclare(fileEventManualExchange, ExchangeTypeFanout, durable: true, autoDelete: false);

            var fileEventManualQueue = prefixFileEventManual.GetQueue();
            channel.QueueDeclare(fileEventManualQueue, durable: true, exclusive: false, autoDelete: false,
              arguments: new Dictionary<string, object>()
              {
                  [QueueMode] = LazyQueueMode
              });
            channel.QueueBind(fileEventManualQueue, fileEventManualExchange, string.Empty);
        }

        private static void CreateFileUnmappedExchangeAndQueue(IModel channel)
        {
            var prefixFileUnmapped = PrefixMessageBrokerConst.FileUnmapped;
            var fileUnmappedExchange = prefixFileUnmapped.GetExchange();
            channel.ExchangeDeclare(fileUnmappedExchange, ExchangeTypeFanout, durable: true, autoDelete: false);

            var fileUnmappedQueue = prefixFileUnmapped.GetQueue();
            channel.QueueDeclare(fileUnmappedQueue, durable: true, exclusive: false, autoDelete: false,
              arguments: new Dictionary<string, object>()
              {
                  [QueueMode] = LazyQueueMode
              });
            channel.QueueBind(fileUnmappedQueue, fileUnmappedExchange, string.Empty);
        }
    }

}
