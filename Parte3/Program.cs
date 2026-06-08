
using System;

namespace Parte3
{
    class Program
    {
        static void Main()
        {
            MaquinaTuring mt = new MaquinaTuring();

            string[] testes =
            {
                "abc",
                "aabbcc",
                "aaabbbccc",
                "aabbc",
                "ab",
                "abcabc"
            };

            foreach (string cadeia in testes)
            {
                Console.WriteLine($"\nEntrada: {cadeia}");

                bool resultado = mt.Executar(cadeia);

                Console.WriteLine(resultado ? "ACEITA" : "REJEITA");
                Console.WriteLine("------------------------");
            }

            Console.WriteLine("\nPressione ENTER para fechar...");
            Console.ReadLine();
        }
    }
}