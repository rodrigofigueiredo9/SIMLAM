using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class EdificacaoPDF
	{
		public String Tipo { get; set; }
		public Int32 Quantidade { get; set; }

		public EdificacaoPDF() { }

		public EdificacaoPDF(Edificacao edificacao)
		{
			Tipo = edificacao.Tipo;
			Quantidade = edificacao.Quantidade;
		}
	}
}