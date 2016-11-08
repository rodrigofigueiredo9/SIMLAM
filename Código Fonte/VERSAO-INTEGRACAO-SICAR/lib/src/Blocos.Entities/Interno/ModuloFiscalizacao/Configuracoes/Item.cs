using System;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class Item : Lista
	{
		public String SituacaoTexto 
		{
			get 
			{
				return IsAtivo ? "Ativado" : "Desativado";
			} 
		}

		public Int32 TipoCampo { get; set; }
		public String TipoCampoTexto { get; set; }

		public Int32 UnidadeMedida { get; set; }
		public String UnidadeMedidaTexto { get; set; }
	}
}
