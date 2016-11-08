using System;
using System.Collections.Generic;
using System.Linq;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class ColecaoDados
	{
		Dictionary<int, Dictionary<int, object>> _dados;
		public List<int> Linhas { get; private set; }
		public Dictionary<int, string> Colunas { get; set; }

		public ColecaoDados()
		{
			_dados = new Dictionary<int, Dictionary<int, object>>();
			Linhas = new List<int>();
			Colunas = new Dictionary<int, string>();
		}

		public ColecaoDados(Dictionary<int, string> colunas)
		{
			_dados = new Dictionary<int, Dictionary<int, object>>();
			Linhas = new List<int>();
			Colunas = colunas;
		}

		public object this[int linha, int coluna]
		{
			get
			{
				if (!_dados.ContainsKey(linha)) return null;
				if (!_dados[linha].ContainsKey(coluna)) return null;
				return _dados[linha][coluna];
			}
			set
			{
				if (!_dados.ContainsKey(linha))
				{
					if (!Linhas.Exists(x => x == linha))
					{
						Linhas.Add(linha);
					}
					_dados.Add(linha, new Dictionary<int, object>());
				}
				_dados[linha][coluna] = value;
			}
		}

		public double Somar(int coluna)
		{
			double result = 0;

			result = Linhas.Sum(x => Convert.ToDouble(((this[x, coluna] != null && this[x, coluna].ToString() != string.Empty) ? this[x, coluna] : 0)));

			return result;
		}

		public double Media(int coluna)
		{
			double result = 0;

			result = Linhas.Average(x => Convert.ToDouble(((this[x, coluna] != null && this[x, coluna].ToString() != string.Empty) ? this[x, coluna] : 0)));

			return result;
		}

		public double Maximo(int coluna)
		{
			double result = 0;

			result = Linhas.Max(x => Convert.ToDouble(((this[x, coluna] != null && this[x, coluna].ToString() != string.Empty) ? this[x, coluna] : 0)));

			return result;
		}

		public double Minimo(int coluna)
		{
			double result = 0;

			result = Linhas.Min(x => Convert.ToDouble(((this[x, coluna] != null && this[x, coluna].ToString() != string.Empty) ? this[x, coluna] : 0)));

			return result;
		}

		public IEnumerable<object> Itens(int coluna)
		{
			return Linhas.Select(x => this[x, coluna]);
		}

		public double ItensDistintos(int coluna)
		{
			double result = 0;

			result = Linhas.Select(x => this[x, coluna]).Distinct().Count();

			return result;
		}
	}
}