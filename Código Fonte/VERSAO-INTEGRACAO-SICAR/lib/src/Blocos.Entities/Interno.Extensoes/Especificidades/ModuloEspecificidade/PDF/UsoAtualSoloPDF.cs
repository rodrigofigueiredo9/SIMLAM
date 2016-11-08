using System;
using System.Globalization;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class UsoAtualSoloPDF
	{
		public String TipoDeUsoTexto { get; set; }
		public String AreaPorcentagem { get; set; }

		public UsoAtualSoloPDF(){}

		public UsoAtualSoloPDF(UsoAtualSolo usoSolo)
		{
			TipoDeUsoTexto = (!String.IsNullOrEmpty(usoSolo.TipoDeUsoTexto)) ? usoSolo.TipoDeUsoTexto : usoSolo.TipoDeUsoGeo;
			TipoDeUsoTexto = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TipoDeUsoTexto);

			AreaPorcentagem = usoSolo.AreaPorcentagem.ToString();
		}
	}
}