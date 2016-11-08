using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico
{
	public class Sobreposicoes
	{
		public String DataVerificacao { get; set; }
		public DateTecno DataVerificacaoBanco { get; set; }
		public List<Sobreposicao> Itens { get; set; }

		public Sobreposicoes()
		{
			DataVerificacaoBanco = new DateTecno();
			Itens = new List<Sobreposicao>();
		}
	}
}