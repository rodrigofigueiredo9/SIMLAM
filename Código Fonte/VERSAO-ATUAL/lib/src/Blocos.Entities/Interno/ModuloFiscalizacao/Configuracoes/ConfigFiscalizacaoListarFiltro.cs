

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ConfigFiscalizacaoListarFiltro
	{
		public String NumeroConfiguracao { get; set; }
		public Int32 ClassificacaoId { get; set; }
		public Int32 TipoId { get; set; }
		public Int32 ItemId { get; set; }

		public ConfigFiscalizacaoListarFiltro()
		{
			this.NumeroConfiguracao = string.Empty;
		}
	}
}
