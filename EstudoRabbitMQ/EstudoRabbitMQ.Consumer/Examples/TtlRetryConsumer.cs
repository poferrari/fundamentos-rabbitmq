using EstudoRabbitMQ.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace EstudoRabbitMQ.Consumer.Examples
{
    public static class TtlRetryConsumer
    {
        private static string _queueName = PrefixMessageBrokerConst.FileZip.GetQueue();
        private static IModel _channel;
        private static ExchangePublisher _queuePublisher;
        private const ushort DefaultPrefetchCount = 1;
        private static string _exchangeNameDeadLetter = PrefixMessageBrokerConst.FileEventDeadLetter.GetExchange();

        public static void Run()
        {
            _channel = BuildModel();

            IModel channel = MessageBrokerService.GetConnection().CreateModel();
            _queuePublisher = new ExchangePublisher(channel);

            IBasicConsumer consumer = BuildConsumer();

            string consumerTag = consumer.Model.BasicConsume(
                    queue: _queueName,
                    autoAck: false,//optar pelo controle manual
                    consumer: consumer);

            Console.WriteLine($"Queue [{_queueName}] is waiting for messages.");
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();

            _channel.BasicCancelNoWait(consumerTag);
        }

        private static IModel BuildModel()
        {
            IModel model = MessageBrokerService.GetConnection().CreateModel();
            model.QueueDeclarePassive(_queueName);
            model.BasicQos(0, DefaultPrefetchCount, false);
            return model;
        }

        private static IBasicConsumer BuildConsumer()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += Receive;
            return consumer;
        }

        private static async Task Receive(object sender, BasicDeliverEventArgs @event)
        {
            ExampleMessage message;

            try
            {
                message = MessageSerializerUtil.Deserialize<ExampleMessage>(@event);

                Console.WriteLine($" [x] '{@event.DeliveryTag}' - {message.Description}");
            }
            catch (Exception exception)
            {
                _channel.BasicReject(@event.DeliveryTag, false);

                Console.WriteLine($"Exception: {exception.Message}");
                return;
            }

            await HandleTtlRetry(@event, message);
        }

        private static async Task HandleTtlRetry(BasicDeliverEventArgs @event, ExampleMessage message)
        {
            try
            {
                await Task.Delay(3000); //processar algo com a messagem recebida

                var randomNumber = new Random().Next();
                if (randomNumber % 3 == 0)//simular algum problema, mensagem cai para tratamento
                {
                    throw new Exception("Any exception");
                }
            }
            catch (Exception exception)
            {
                PublishRetryMessage(@event);

                Console.WriteLine($"Exception on processing message '{_queueName}' - {message.Description} ==> ({@event.RoutingKey}): {exception.Message}");
            }
            finally
            {
                _channel.BasicAck(@event.DeliveryTag, false);
            }
        }

        private static void PublishRetryMessage(BasicDeliverEventArgs @event)
        {
            _queuePublisher.Publish(new MessageData(_exchangeNameDeadLetter, @event.RoutingKey, @event.Body)
            {
                Headers = @event.BasicProperties.Headers
            });
        }
    }
}
