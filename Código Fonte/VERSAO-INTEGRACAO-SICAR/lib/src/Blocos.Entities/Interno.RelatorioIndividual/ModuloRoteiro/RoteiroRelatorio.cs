using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro
{
	public class RoteiroRelatorio
	{
		public int Id{ get; set; }
		public int Numero { get; set; }
		public String Nome { get; set; }
		public int? Versao { get; set; }
		public int? AtividadeId { get; set; }
		public String AtividadeTexto { get; set; }
		public String Observacoes { get; set; }
		public String ObservacoesHtml { get; set; }
		public List<ItemRelatorio> Itens { get; set; }
		public List<AnexoRelatorio> Anexos { get; set; }
		public int	SetorId { get; set; }
		public String SetorTexto { get; set; }

		public List<ItemRelatorio> ItensTecnicos { get; set; }
		public List<ItemRelatorio> ItensAdministrativos { get; set; }

		public RoteiroRelatorio()
		{
			Itens = new List<ItemRelatorio>();
			Anexos = new List<AnexoRelatorio>();			
		}
	}
}
