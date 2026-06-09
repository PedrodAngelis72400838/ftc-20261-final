using System;
using System.Collections.Generic;
using System.IO;

namespace Parte2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("       PARTE 2 - EXECUTOR Automato de Pilha       ");
            Console.WriteLine("==================================================");

            //L2
            var Q_L2 = new HashSet<string> { "q0", "q1" };
            var Sigma_L2 = new HashSet<char> { 'a', 'b' };
            var Gamma_L2 = new HashSet<char> { 'a', 'Z' };

            AP apL2 = new AP(Q_L2, Sigma_L2, Gamma_L2, "q0", 'Z');

            // Delta L2
            apL2.AdicionarTransicao("q0", 'a', 'Z', "q0", "aZ");
            apL2.AdicionarTransicao("q0", 'a', 'a', "q0", "aa");
            apL2.AdicionarTransicao("q0", 'b', 'a', "q1", "");  
            apL2.AdicionarTransicao("q1", 'b', 'a', "q1", ""); 
            apL2.AdicionarTransicao("q1", '\0', 'Z', "q1", ""); 

            //L3
            var Q_L3 = new HashSet<string> { "q_init", "q0", "q1" };
            var Sigma_L3 = new HashSet<char> { 'a', 'b' };
            var Gamma_L3 = new HashSet<char> { 'a', 'b', 'Z' };

            AP apL3 = new AP(Q_L3, Sigma_L3, Gamma_L3, "q_init", 'Z');

            apL3.AdicionarTransicao("q_init", 'a', 'Z', "q0", "aZ"); 
            apL3.AdicionarTransicao("q_init", 'a', 'Z',  "q1", "Z"); 
            apL3.AdicionarTransicao("q_init", 'b', 'Z', "q0", "bZ");
            apL3.AdicionarTransicao("q_init", 'b', 'Z', "q1", "Z");

     
            foreach (char s in new[] { 'a', 'b' })
            {
                foreach (char t in new[] { 'a', 'b', 'Z' })
                {
                    apL3.AdicionarTransicao("q0", s, t, "q0", s.ToString() + t.ToString());
                    apL3.AdicionarTransicao("q0", s, t, "q1", t.ToString());
                }
            }

            apL3.AdicionarTransicao("q0", '\0', 'a', "q1", "a");
            apL3.AdicionarTransicao("q0", '\0', 'b', "q1", "b");

            apL3.AdicionarTransicao("q1", 'a', 'a', "q1", ""); 
            apL3.AdicionarTransicao("q1", 'b', 'b', "q1", ""); 
            apL3.AdicionarTransicao("q1", '\0', 'Z', "q1", "");


            string arquivoEntradas = "entradas_ap.txt";
            Console.WriteLine($"--- Executando Testes de L2 (a^n b^n) a partir de '{arquivoEntradas}' ---");

            if (File.Exists(arquivoEntradas))
            {
                string[] linhas = File.ReadAllLines(arquivoEntradas);
                foreach (string linha in linhas)
                {
                    string cadeia = linha.Trim();
                    if (cadeia == "") cadeia = "ε";

                    ExecutarEExibir(apL2, cadeia);
                }
            }
            else
            {
                Console.WriteLine($"ERRO: Arquivo '{arquivoEntradas}' não foi encontrado na pasta de execução.");
            }

            Console.Clear();
            Console.WriteLine("\n--- Executando Testes do Desafio L3 (Palíndromos |w| >= 1) ---");
            string[] testesL3 = { "a", "aba", "abba", "ab", "aab"};
            foreach (string cadeia in testesL3)
            {
                ExecutarEExibir(apL3, cadeia);
            }
        }

        static void ExecutarEExibir(AP ap, string cadeia)
        {
            Console.WriteLine($"\n==================================================");
            Console.WriteLine($" TESTANDO CADEIA: \"{cadeia}\"");
            Console.WriteLine($"==================================================");

            bool aceito = ap.Aceitar(cadeia, out List<string> rastro);

            Console.WriteLine("Rastro de Configurações Instantâneas percorrido:");
            foreach (string passo in rastro)
            {
                Console.WriteLine($"  {passo}");
            }

            Console.WriteLine("--------------------------------------------------");
            if (aceito)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" RESULTADO: ACEITA (A entrada terminou e a pilha foi limpa!)");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" RESULTADO: REJEITA (Nenhum caminho computacional esvaziou a pilha)");
            }

            Console.ResetColor();
            Console.WriteLine("\n==================================================");
            Console.WriteLine("Pressione qualquer tecla para prosseguir...");
            Console.ReadKey();
        }
    }
}