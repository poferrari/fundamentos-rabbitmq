using System;
using System.Collections.Generic;
using System.Linq;

namespace EstudoRabbitMQ.Services
{
    public class RunExamples
    {
        private readonly Dictionary<string, Action> Examples;

        public RunExamples(Dictionary<string, Action> examples)
        {
            Examples = examples;
        }

        public void ChooseAndRun()
        {
            int i = 1;

            foreach (var example in Examples)
            {
                Console.WriteLine("{0}) {1}", i, example.Key);
                i++;
            }

            Console.Write("Digite o número (ou vazio para o último)? ");

            int.TryParse(Console.ReadLine(), out int num);
            bool numValid = num > 0 && num <= Examples.Count;
            num = numValid ? num - 1 : Examples.Count - 1;

            string exampleName = Examples.ElementAt(num).Key;

            Console.Write("\nExecutando exemplo: ");
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(exampleName);
            Console.ResetColor();

            Console.WriteLine(string.Concat(Enumerable.Repeat("=", exampleName.Length + 21)) + "\n");

            Action executar = Examples.ElementAt(num).Value;

            try
            {
                executar();
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Ocorreu um erro: {0}", e.Message);
                Console.ResetColor();

                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
