using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;

namespace EstudoRabbitMQ.Services
{
    public static class MessageBrokerService
    {
        private const int RetryCount = 5;
        private static IConnection _connection;

        public static IConnection GetConnection()
        {
            TryConnection();

            return _connection;
        }

        private static void TryConnection()
        {
            if (IsConnected())
            {
                return;
            }

            var factory = GetConnectionFactory();

            Policy.Handle<BrokerUnreachableException>()
                   .Or<SocketException>()
                   .Or<TimeoutException>()
                   .Or<OperationInterruptedException>()
                   .WaitAndRetry(RetryCount, retryAttempt =>
                   {
                       Console.WriteLine("RabbitMQ Client is trying to connect...");
                       return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                   })
                   .Execute(() => _connection = factory.CreateConnection());
        }

        private static ConnectionFactory GetConnectionFactory()
         => new ConnectionFactory
         {
             UserName = "guest",
             Password = "guest",
             Port = 5672,
             HostName = "localhost",
             VirtualHost = "test-example",
             AutomaticRecoveryEnabled = true,
             DispatchConsumersAsync = true // consumidor async
         };

        public static bool IsConnected()
            => _connection != null && _connection.IsOpen;
    }
}
