using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemRoteiro
{
	public class ChecagemRoteiroRelatorio
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Interessado { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		
		public List<RoteiroRelatorio> Roteiros { get; set; }
		public List<ItemRelatorio> Itens { get; set; }

		public ChecagemRoteiroRelatorio() 
		{
			Roteiros = new List<RoteiroRelatorio>();
			Itens = new List<ItemRelatorio>();
		}
	}
}