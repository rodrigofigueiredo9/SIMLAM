using System.Collections.Generic;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	class AnalisadorSintatico
	{
		public AnalisadorSintatico(IEnumerable<Termo> termos)
		{
			_pos = 0;
			_termos = new List<Termo>();
			_termos.AddRange(termos);
			Erros = new List<ErroSintatico>();
		}

		int _pos;
		public List<Termo> _termos;
		public List<ErroSintatico> Erros { get; private set; }
		
		eTipoTermo Atual
		{
			get
			{
				if (_pos >= _termos.Count) return eTipoTermo.Fim;
				return (eTipoTermo)_termos[_pos].Tipo;
			}
		}

		public void Analisar()
		{
			Erros.Clear();
			if (_termos.Count == 0) return;
			Expressao();
			if (Atual != eTipoTermo.Fim) Erro();
		}

		void Expressao()
		{
			Termo();
			switch (Atual)
			{
				case eTipoTermo.OperadorOu:
					Aceitar(eTipoTermo.OperadorOu);
					Expressao();
					break;
			}
		}

		void Termo()
		{
			Fator();
			switch (Atual)
			{
				case eTipoTermo.OperadorE:
					Aceitar(eTipoTermo.OperadorE);
					Termo();
					break;
			}
		}

		void Fator()
		{
			switch (Atual)
			{
				case eTipoTermo.AbreParenteses:
					Aceitar(eTipoTermo.AbreParenteses);
					Expressao();
					Aceitar(eTipoTermo.FechaParenteses);
					break;
				case eTipoTermo.Filtro:
					Aceitar(eTipoTermo.Filtro);
					break;
				default:
					Erro();
					break;
			}
		}

		void Aceitar(eTipoTermo token)
		{
			if (token == Atual)
			{
				_pos++;
				return;
			}
			Erro(token);
		}

		void Erro(eTipoTermo esperado = eTipoTermo.Fim)
		{
			ErroSintatico erro = new ErroSintatico(_pos, esperado, Atual);
			Erros.Add(erro);
		}
	}
}