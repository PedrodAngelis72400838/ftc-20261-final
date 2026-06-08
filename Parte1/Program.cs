using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Parte1;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        string arquivoJson = args.Length > 0 ? args[0] : "afd.json";
        string arquivoEntradas = args.Length > 1 ? args[1] : "entradas.txt";

        Console.WriteLine("==================================================");
        Console.WriteLine("              PARTE 1 - EXECUTOR AFD              ");
        Console.WriteLine("==================================================");

        try
        {
            // Carrega JSON e constrói o autômato
            Console.WriteLine($"\n[+] Lendo configuração: '{arquivoJson}'...");
            AFD afd = AFD.CarregarDeJson(arquivoJson);
            Console.WriteLine("[✓] Estrutura do autômato carregada com sucesso.");

            // Imprime diagrama de transicoes
            afd.ExibirDiagrama();
            Console.WriteLine("\n==================================================");

            // Processamento por cadeia das palavras de teste
            if (!File.Exists(arquivoEntradas))
            {
                Console.WriteLine($"[!] Erro: O arquivo de cadeias '{arquivoEntradas}' não existe na pasta de execução.");
                return;
            }

            Console.WriteLine($"[+] Processando cadeias de: '{arquivoEntradas}'\n");
            string[] linhas = File.ReadAllLines(arquivoEntradas);

            foreach (string linha in linhas)
            {
                string cadeia = linha.Trim();
                string visualizacao = string.IsNullOrEmpty(cadeia) ? "ε (Cadeia Vazia)" : $"\"{cadeia}\"";

                bool aceita = afd.ProcessarCadeia(cadeia, out List<string> rastro);

                Console.WriteLine($"Cadeia:    {visualizacao}");
                Console.WriteLine($"Rastro:    {string.Join(" ➔ ", rastro)}");
                Console.WriteLine($"Resultado: {(aceita ? "ACEITA" : "REJEITA")}");
                Console.WriteLine(new string('-', 45));
            }

            // Testes manuais
            ExecutarModoInterativo(afd);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[!] Ocorreu um erro crítico de execução: {ex.Message}");
        }
    }

    static void ExecutarModoInterativo(AFD afd)
    {
        Console.Write("\nDeseja testar uma cadeia customizada manualmente? (S/N): ");
        string? resposta = Console.ReadLine()?.Trim().ToUpper();

        if (resposta != "S") return;

        Console.WriteLine("\n--- Modo Interativo (Digite 'sair' para encerrar) ---");
        while (true)
        {
            Console.Write("\nDigite a cadeia de entrada ➔ ");
            string? entrada = Console.ReadLine();

            if (entrada == null || entrada.Equals("sair", StringComparison.OrdinalIgnoreCase))
                break;

            string cadeia = entrada.Trim();
            string exibicao = string.IsNullOrEmpty(cadeia) ? "ε" : $"\"{cadeia}\"";

            bool aceita = afd.ProcessarCadeia(cadeia, out List<string> rastro);

            Console.WriteLine($"  Cadeia informada: {exibicao}");
            Console.WriteLine($"  Caminho dos estados: {string.Join(" ➔ ", rastro)}");
            Console.WriteLine($"  Veredito final:   {(aceita ? "✓ ACEITA" : "✕ REJEITA")}");
        }

        Console.WriteLine("\n[+] Parte 1 concluída com sucesso! Pressione qualquer tecla para sair.");
        Console.ReadKey();
    }
}