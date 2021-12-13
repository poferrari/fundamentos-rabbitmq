using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace EstudoRabbitMQ.Services
{
    public class ExchangePublisher
    {
        private const byte Persistent = 2;//armazena a mensagem
        private const int DefaultWaitForConfirmsOrDieInSeconds = 30;
        private readonly IModel _channel;

        public ExchangePublisher(IModel channel)
        {
            _channel = channel;
        }

        public void Publish<TMessage>(string exchangeName, string routingKey, TMessage message, Dictionary<string, object> headers = null)
        {
            var body = MessageSerializerUtil.Serialize(message);
            Publish(new MessageData(exchangeName, routingKey, body, headers));
        }

        /// <summary>
        /// https://www.rabbitmq.com/tutorials/tutorial-seven-dotnet.html
        /// </summary>        
        public void Publish(MessageData messageData)
        {
            _channel.ConfirmSelect();//publicação com confirmação

            var properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = Persistent;
            AttachHeaders(properties, messageData.Headers);

            _channel.BasicPublish(
                exchange: messageData.ExchangeName,
                routingKey: messageData.RoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: messageData.Body);

            _channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(DefaultWaitForConfirmsOrDieInSeconds));//aguarda confirmação
        }

        private static void AttachHeaders(IBasicProperties properties, IDictionary<string, object> headersToAdd)
        {
            if (headersToAdd != null)
            {
                if (properties.Headers is null)
                {
                    properties.Headers = new Dictionary<string, object>();
                }

                foreach (var header in headersToAdd)
                {
                    properties.Headers.Add(header);
                }
            }
        }
    }
}
