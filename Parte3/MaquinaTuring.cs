using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parte3
{
	internal class MaquinaTuring
	{
		private Dictionary<int, char> fita = new();

		private int cabecote;

		private void CarregarFita(string entrada)
		{
			fita.Clear();

			for (int i = 0; i < entrada.Length; i++)
			{
				fita[i] = entrada[i];
			}

			cabecote = 0;
		}

		private char Ler(int pos)
		{
			if (fita.ContainsKey(pos))
				return fita[pos];

			return '_';
		}

		private void MostrarFita()
		{
			int min = 0;
			int max = fita.Count > 0 ? fita.Keys.Max() : 0;

			for (int i = min; i <= max; i++)
			{
				if (i == cabecote)
					Console.Write($"[{Ler(i)}]");
				else
					Console.Write($" {Ler(i)} ");
			}

			Console.WriteLine();
		}

		public bool Executar(string entrada)
		{
			Console.WriteLine("Executando MT para: " + entrada);

			if (string.IsNullOrEmpty(entrada))
				return false;

			bool encontrouB = false;
			bool encontrouC = false;

			foreach (char ch in entrada)
			{
				if (ch == 'b')
					encontrouB = true;

				if (ch == 'c')
					encontrouC = true;

				if (ch == 'a' && (encontrouB || encontrouC))
					return false;

				if (ch == 'b' && encontrouC)
					return false;
			}


			CarregarFita(entrada);

			while (true)
			{
				MostrarFita();

				int posA = -1;

				for (int i = 0; i < fita.Count; i++)
				{
					if (Ler(i) == 'a')
					{
						posA = i;
						break;
					}
				}

				if (posA == -1)
					break;

				fita[posA] = 'X';

				int posB = -1;

				for (int i = posA + 1; i < fita.Count; i++)
				{
					if (Ler(i) == 'b')
					{
						posB = i;
						break;
					}
				}

				if (posB == -1)
					return false;

				fita[posB] = 'Y';

				int posC = -1;

				for (int i = posB + 1; i < fita.Count; i++)
				{
					if (Ler(i) == 'c')
					{
						posC = i;
						break;
					}
				}

				if (posC == -1)
					return false;

				fita[posC] = 'Z';
			}

			foreach (var simbolo in fita.Values)
			{
				if (simbolo == 'a' ||
					simbolo == 'b' ||
					simbolo == 'c')
				{
					return false;
				}
			}

			return true;
		}
	}
}