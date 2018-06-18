

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ParametrizacaoListarFiltro
	{
		public Int32 CodigoReceitaId { get; set; }
		public String CodigoReceitaTexto { get; set; }

		public DateTime InicioVigencia { get; set; }
		public DateTime? FimVigencia { get; set; }
	}
}