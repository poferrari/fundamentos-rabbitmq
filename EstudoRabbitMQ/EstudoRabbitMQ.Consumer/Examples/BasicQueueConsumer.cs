using EstudoRabbitMQ.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace EstudoRabbitMQ.Consumer.Examples
{
    public static class BasicQueueConsumer
    {
        private static string _queueName = PrefixMessageBrokerConst.FileZip.GetQueue();
        private static IModel _channel;
        private const ushort DefaultPrefetchCount = 5;//calcular melhor opção, ajuda a otimizar consumo de rede

        public static void Run()
        {
            _channel = BuildModel();

            IBasicConsumer consumer = BuildConsumer();

            Console.WriteLine($"Queue [{_queueName}] initialize Basic Consume.");
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
            Console.WriteLine($"Queue [{_queueName}] create model.");

            model.QueueDeclarePassive(_queueName);
            Console.WriteLine($"Queue [{_queueName}] set queue declare passive.");

            model.BasicQos(0, DefaultPrefetchCount, false);//carrega mensagens em backlog
            Console.WriteLine($"Queue [{_queueName}] set basic qos with prefetch {DefaultPrefetchCount}.");
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
            //status: unack
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
            }

            await Task.Delay(3000); //processar algo com a messagem recebida

            var randomNumber = new Random().Next();
            if (randomNumber % 3 == 0)//simular algum problema, mensagem retorna para fila
            {
                _channel.BasicNack(@event.DeliveryTag, false, requeue: false);//status: nack
                return;
            }

            _channel.BasicAck(@event.DeliveryTag, false);//status: ack            
        }
    }
}
