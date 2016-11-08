using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Sobreposicoes
	{
		public String DataVerificacao { get; set; }
		public DateTecno DataVerificacaoBanco { get; set; }
		public List<Sobreposicao> Itens { get; set; }

		public Sobreposicoes()
		{
			Itens = new List<Sobreposicao>();
		}
	}
}
