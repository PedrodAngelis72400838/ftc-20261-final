using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parte1;

// DTO pra ler o JSON
public class AfdDto
{
    [JsonPropertyName("estados")]
    public HashSet<string> Estados { get; set; } = new();

    [JsonPropertyName("alfabeto")]
    public HashSet<char> Alfabeto { get; set; } = new();

    [JsonPropertyName("estadoInicial")]
    public string EstadoInicial { get; set; } = string.Empty;

    [JsonPropertyName("estadosAceitacao")]
    public HashSet<string> EstadosAceitacao { get; set; } = new();

    [JsonPropertyName("transicoes")]
    public List<TransicaoDto> Transicoes { get; set; } = new();
}

public class TransicaoDto
{
    [JsonPropertyName("origem")]
    public string Origem { get; set; } = string.Empty;

    [JsonPropertyName("simbolo")]
    public string Simbolo { get; set; } = string.Empty;

    [JsonPropertyName("destino")]
    public string Destino { get; set; } = string.Empty;
}

public class AFD
{
    // 5-tupla formal: M = (Q, Σ, δ, q0, F)
    public HashSet<string> Q { get; }
    public HashSet<char> Sigma { get; }
    public Dictionary<(string estado, char simbolo), string> Delta { get; }
    public string q0 { get; }
    public HashSet<string> F { get; }

    public AFD(HashSet<string> q, HashSet<char> sigma, Dictionary<(string, char), string> delta, string estadoInicial, HashSet<string> f)
    {
        Q = q;
        Sigma = sigma;
        Delta = delta;
        q0 = estadoInicial;
        F = f;
    }

    // Método de aceitação
    public bool Aceitar(string cadeia)
    {
        return ProcessarCadeia(cadeia, out _);
    }

    // Leitura de cadeias
    public bool ProcessarCadeia(string cadeia, out List<string> rastro)
    {
        rastro = new List<string> { q0 };
        string estadoAtual = q0;

        foreach (char simbolo in cadeia)
        {
            // Se o símbolo não pertence ao alfabeto ou a transição não existe, vai para o estado de erro
            if (!Sigma.Contains(simbolo) || !Delta.TryGetValue((estadoAtual, simbolo), out string? proximoEstado))
            {
                rastro.Add("ERRO");
                return false;
            }

            estadoAtual = proximoEstado;
            rastro.Add(estadoAtual);
        }

        return F.Contains(estadoAtual);
    }

    // Exibe em formato de diagrama
    public void ExibirDiagrama()
    {
        Console.WriteLine("\n┌──────────────────────────────────────────┐");
        Console.WriteLine("│       TABELA DE TRANSIÇÕES DO AFD        │");
        Console.WriteLine("└──────────────────────────────────────────┘");

        Console.Write("Estado\t│");
        foreach (char s in Sigma) Console.Write($"   {s}\t│");
        Console.WriteLine("\n" + new string('-', 16 + (Sigma.Count * 9)));

        foreach (string estado in Q)
        {
            string prefixo = "";
            if (estado == q0) prefixo += "->";
            if (F.Contains(estado)) prefixo += "*";

            Console.Write($"{prefixo}{estado}\t│");

            foreach (char s in Sigma)
            {
                string destino = Delta.TryGetValue((estado, s), out string? dest) ? dest : "-";
                Console.Write($"   {destino}\t│");
            }
            Console.WriteLine();
        }
        //Console.WriteLine("\nLegenda: -> (Inicial) | * (Final/Aceitação)\n");
    }

    // Carga automatizada do JSon
    public static AFD CarregarDeJson(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException($"O arquivo de configuração '{caminhoArquivo}' não foi encontrado.");

        string jsonString = File.ReadAllText(caminhoArquivo);
        var dto = JsonSerializer.Deserialize<AfdDto>(jsonString);

        if (dto == null)
            throw new Exception("Falha ao interpretar a estrutura do arquivo JSON.");

        var deltaConvertido = new Dictionary<(string, char), string>();
        foreach (var t in dto.Transicoes)
        {
            if (string.IsNullOrEmpty(t.Simbolo)) continue;
            deltaConvertido[(t.Origem, t.Simbolo[0])] = t.Destino;
        }

        return new AFD(dto.Estados, dto.Alfabeto, deltaConvertido, dto.EstadoInicial, dto.EstadosAceitacao);
    }
}