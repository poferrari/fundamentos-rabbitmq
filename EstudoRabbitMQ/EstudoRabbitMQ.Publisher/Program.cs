using EstudoRabbitMQ.Publisher.Examples;
using EstudoRabbitMQ.Services;
using System;
using System.Collections.Generic;

namespace EstudoRabbitMQ.Publisher
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var central = new RunExamples(new Dictionary<string, Action>()
            {
                { "Direct Exchange", DirectExchange.Run },
                { "Fanout Exchange", FanoutExchange.Run },
                { "Topic Exchange", TopicExchange.Run },
                { "Caso real", ExamplePublisher.Run },
            });
            central.ChooseAndRun();

            Console.WriteLine("Main program done.");
            Console.ReadKey();
        }
    }
}
