using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloAutorizacao
{
	public class AutorizacaoExploracaoFlorestal : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		public Int32 Destinatario { set; get; }
		public String DestinatarioNomeRazao { get; set; }
		public String Observacao { set; get; }
	}
}