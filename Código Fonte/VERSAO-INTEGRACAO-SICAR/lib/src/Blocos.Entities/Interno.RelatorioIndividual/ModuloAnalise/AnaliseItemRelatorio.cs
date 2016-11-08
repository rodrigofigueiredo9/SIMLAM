using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloAnalise
{
	public class AnaliseItemRelatorio
	{
		public string Id { get; set; }
		public string Numero { get; set; }
		public string Tipo { get; set; }
		public bool IsProcesso { get; set; }
		public int SetorId { get; set; }
		public int InteressadoId { get; set; }
		public string InteressadoNome { get; set; }
		public string InteressadoCPFCNPJ { get; set; }
		public string RequerimentoNumero { get; set; }
		public string RequerimentoData{ get; set; }
		public string RequerimentoOrigem { get; set; }
		public string ChecagemNumero { get; set; }

		private List<AtividadeRelatorio> _atividades = new List<AtividadeRelatorio>();
		public List<AtividadeRelatorio> Atividades { get { return _atividades; } set { _atividades = value; } }

		private List<RoteiroRelatorio> _roteiros = new List<RoteiroRelatorio>();
		public List<RoteiroRelatorio> Roteiros { get { return _roteiros; } set { _roteiros = value; } }

		private List<ItemRelatorio> _itens = new List<ItemRelatorio>();
		public List<ItemRelatorio> Itens { get { return _itens; } set { _itens = value; } }

		private List<ItemRelatorio> _itensTecnicos = new List<ItemRelatorio>();
		public List<ItemRelatorio> ItensTecnicos { get { return _itensTecnicos; } set { _itensTecnicos = value; } }

		private List<ItemRelatorio> _itensAdministrativos = new List<ItemRelatorio>();
		public List<ItemRelatorio> ItensAdministrativos { get { return _itensAdministrativos; } set { _itensAdministrativos = value; } }

		private List<ItemRelatorio> _itensProjetoDigital = new List<ItemRelatorio>();
		public List<ItemRelatorio> ItensProjetoDigital { get { return _itensProjetoDigital; } set { _itensProjetoDigital = value; } }
		
	}
}
