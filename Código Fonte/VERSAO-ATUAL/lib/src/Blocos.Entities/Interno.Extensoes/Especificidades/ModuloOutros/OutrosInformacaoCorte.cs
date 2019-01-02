using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros
{
	public class OutrosInformacaoCorte : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		public Int32 InformacaoCorte { get; set; }
		public Int32 Atividade { get; set; }
		public Int32 Validade { get; set; }
		public String Interessado { get; set; }

		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public Int32 VinculoPropriedade { get; set; }
	}
}