using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Parte1;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        string arquivoJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "afd.json");
        string arquivoEntradas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "entradas.txt");

        Console.WriteLine("==================================================");
        Console.WriteLine("              PARTE 1 - EXECUTOR AFD              ");
        Console.WriteLine("==================================================");

        try
        {
            // Carrega JSON e constrói o autômato
            Console.WriteLine($"\n-Lendo configuração: '{arquivoJson}'...");
            AFD afd = AFD.CarregarDeJson(arquivoJson);
            Console.WriteLine("-Estrutura do autômato carregada com sucesso.");

            // Imprime diagrama de transicoes
            afd.ExibirDiagrama();
            Console.WriteLine("\n==================================================");

            // Processamento por cadeia das palavras de teste
            if (!File.Exists(arquivoEntradas))
            {
                Console.WriteLine($"-Erro: O arquivo de cadeias '{arquivoEntradas}' não existe na pasta de execução.");
                return;
            }
            string[] linhas = File.ReadAllLines(arquivoEntradas);

            foreach (string linha in linhas)
            {
                string cadeia = linha.Trim();
                if (cadeia == "")
                    cadeia = "ε";
                ExecutarEExibir(afd, cadeia);
            }

            static void ExecutarEExibir(AFD afd, string cadeia)
            {
                Console.WriteLine($"\n==================================================");
                Console.WriteLine($" TESTANDO CADEIA: \"{(string.IsNullOrEmpty(cadeia) ? "ε" : cadeia)}\"");
                Console.WriteLine($"==================================================");

                string cadeiaProcessamento = cadeia == "ε" ? "" : cadeia;
                bool aceita = afd.ProcessarCadeia(cadeia, out List<string> rastro);

                Console.WriteLine("Rastro de estados percorrido:");
                Console.WriteLine($"  {string.Join(" -> ", rastro)}");

                Console.WriteLine("--------------------------------------------------");
                if (aceita)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" RESULTADO: ACEITA");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" RESULTADO: REJEITA");
                }

                Console.ResetColor();
                Console.WriteLine("\n==================================================");
                Console.WriteLine("Pressione qualquer tecla para prosseguir...");
                Console.ReadKey();
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n-Ocorreu um erro crítico de execução: {ex.Message}");
        }

        Console.WriteLine("\nPressione qualquer tecla para encerrar...");
        Console.ReadKey();
    }
}