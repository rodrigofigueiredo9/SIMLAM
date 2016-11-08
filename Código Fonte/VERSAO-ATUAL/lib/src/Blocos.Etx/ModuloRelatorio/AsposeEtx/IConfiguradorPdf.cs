using System.Collections.Generic;
using Aspose.Words;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public interface IConfiguradorPdf
	{
		bool ExibirSimplesConferencia { get; set; }
		bool CondicionanteRemovePageBreakAnterior { get; set; }
		ICabecalhoRodape CabecalhoRodape { get; set; }
		void Load(Document doc, object dataSource);
		void Configurar(Document doc);
		void Executed(Document _doc, object dataSource);
		List<IAssinante> Assinantes { get; set; }
	}
}
