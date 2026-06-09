using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Parte2
{
    public class AP
    {
        //7-upla formal
        public HashSet<string> Q { get; }
        public HashSet<char> Sigma { get; }
        public HashSet<char> Gamma { get; }
        public String Q0 { get; }
        public char Z0 { get; }

        public Dictionary<(string estado, char simbolo, char topo), List<(string proximoEstado, string empilhar)>> Delta { get; }

        public AP(HashSet<string> q, HashSet<char> sigma, HashSet<char> gamma, String q0, char z0)
        {
            Q = q;
            Sigma = sigma;
            Gamma = gamma;
            Q0 = q0;
            Z0 = z0;
            Delta = new Dictionary<(string estado, char simbolo, char topo), List<(string proximoEstado, string empilhar)>>();
        }

        public void AdicionarTransicao(string deEstado, char simbolo, char topoPilha, string paraEstado, string empilhar)
        {
            var chave = (deEstado, simbolo, topoPilha);
            if (!Delta.ContainsKey(chave))
            {
                Delta[chave] = new List<(string, string)>();
            }
            Delta[chave].Add((paraEstado, empilhar));
        }

        public bool Aceitar(string cadeia, out List<string> historicoConfiguracoes)
        {
            // Instancia da pilha
            Stack<char> pilhaInicial = new Stack<char>();
            pilhaInicial.Push(Z0);

            historicoConfiguracoes = new List<string>();

            string entradaTratada = cadeia == "ε" ? "" : cadeia;

            return SimularComBacktracking(Q0, entradaTratada, pilhaInicial, historicoConfiguracoes);
        }

        private bool SimularComBacktracking(string estadoAtual, string entradaRestante, Stack<char> pilhaAtual, List<string> rastro)
        {
            // 1. Captura e formatação da Configuração Instantânea Atual (Exigência D)
            char[] arrayPilha = pilhaAtual.ToArray();
            string stringPilha = arrayPilha.Length == 0 ? "Ø" : string.Join("", arrayPilha);
            string exibicaoEntrada = string.IsNullOrEmpty(entradaRestante) ? "ε" : entradaRestante;

            string configInstantanea = $"δ({estadoAtual}, {(entradaRestante.Length > 0 ? entradaRestante[0].ToString() : "ε")}, {(pilhaAtual.Count > 0 ? pilhaAtual.Peek().ToString() : "Ø")}) " +
                                       $"➔ Estado: {estadoAtual} | Entrada Restante: {exibicaoEntrada.PadRight(10)} | Pilha: [{stringPilha}]";
            rastro.Add(configInstantanea);

            // 2. CRITÉRIO ÚNICO DE ACEITAÇÃO: Entrada totalmente consumida E Pilha Vazia (Exigência B)
            if (entradaRestante.Length == 0 && pilhaAtual.Count == 0)
            {
                return true;
            }

            // Se a pilha esvaziou mas a entrada não acabou, este caminho falhou
            if (pilhaAtual.Count == 0)
            {
                rastro.RemoveAt(rastro.Count - 1);
                return false;
            }

            char topoAtual = ArrayPilhaNoTopo(pilhaAtual);

            // CAMINHO A: Tentar transições por ε-movimento (símbolo '\0') de forma prioritária ou alternativa
            var chaveEpsilon = (estadoAtual, '\0', topoAtual);
            if (Delta.ContainsKey(chaveEpsilon))
            {
                foreach (var transicao in Delta[chaveEpsilon])
                {
                    Stack<char> novaPilha = ClonarPilha(pilhaAtual);
                    novaPilha.Pop(); // Consome o topo atualizado

                    // Empilha os novos caracteres na ordem inversa para manter o topo correto
                    string cadeiaEmpilhar = transicao.empilhar;
                    for (int i = cadeiaEmpilhar.Length - 1; i >= 0; i--)
                    {
                        novaPilha.Push(cadeiaEmpilhar[i]);
                    }

                    if (SimularComBacktracking(transicao.proximoEstado, entradaRestante, novaPilha, rastro))
                    {
                        return true;
                    }
                }
            }

            // CAMINHO B: Tentar transições consumindo o próximo caractere da entrada
            if (entradaRestante.Length > 0)
            {
                char proximoSimbolo = entradaRestante[0];
                var chaveSimbolo = (estadoAtual, proximoSimbolo, topoAtual);

                if (Delta.ContainsKey(chaveSimbolo))
                {
                    foreach (var transicao in Delta[chaveSimbolo])
                    {
                        Stack<char> novaPilha = ClonarPilha(pilhaAtual);
                        novaPilha.Pop(); // Consome o topo atualizado

                        string cadeiaEmpilhar = transicao.empilhar;
                        for (int i = cadeiaEmpilhar.Length - 1; i >= 0; i--)
                        {
                            novaPilha.Push(cadeiaEmpilhar[i]);
                        }

                        if (SimularComBacktracking(transicao.proximoEstado, entradaRestante.Substring(1), novaPilha, rastro))
                        {
                            return true;
                        }
                    }
                }
            }

            // Se nenhum dos caminhos possíveis levou à aceitação, remove do histórico e desfaz o passo (Backtrack)
            rastro.RemoveAt(rastro.Count - 1);
            return false;
        }

        private char ArrayPilhaNoTopo(Stack<char> stack)
        {
            return stack.Peek();
        }

        private Stack<char> ClonarPilha(Stack<char> original)
        {
            char[] arr = original.ToArray();
            Array.Reverse(arr);
            return new Stack<char>(arr);
        }

    }
}