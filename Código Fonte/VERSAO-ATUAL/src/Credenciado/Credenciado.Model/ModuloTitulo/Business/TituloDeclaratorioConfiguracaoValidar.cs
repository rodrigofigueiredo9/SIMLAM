using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business
{
	public class TituloDeclaratorioConfiguracaoValidar
	{
		public bool Filtrar(RelatorioTituloDecListarFiltro filtro)
		{
			if (!string.IsNullOrEmpty(filtro.DataSituacaoAtual.DataTexto))
			{
				ValidacoesGenericasBus.DataMensagem(new DateTecno() { DataTexto = filtro.DataSituacaoAtual.DataTexto }, "Filtros_DataSituacaoAtual", "Situação atual", false);
			}

			return Validacao.EhValido;
		}
	}
}