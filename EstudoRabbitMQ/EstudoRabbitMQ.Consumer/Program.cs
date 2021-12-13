using EstudoRabbitMQ.Consumer.Examples;
using EstudoRabbitMQ.Services;
using System;
using System.Collections.Generic;

namespace EstudoRabbitMQ.Consumer
{
    static class Program
    {
        static void Main(string[] args)
        {
            var central = new RunExamples(new Dictionary<string, Action>()
            {
                { "Basic Queue Consumer", BasicQueueConsumer.Run },
                { "TTL Retry Consumer", TtlRetryConsumer.Run },
                { "TTL Retry and Manual Queue Consumer", TtlMaxRetryAndManualQueueConsumer.Run },
            });
            central.ChooseAndRun();

            Console.WriteLine("Main program done.");
            Console.ReadKey();
        }
    }
}
