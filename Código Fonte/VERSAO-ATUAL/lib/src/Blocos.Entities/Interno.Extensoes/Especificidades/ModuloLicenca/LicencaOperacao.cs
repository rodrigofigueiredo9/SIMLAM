using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca
{
	public class LicencaOperacao : Especificidade, ILicenca
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		public Int32 Destinatario { set; get; }
		public String DestinatarioNomeRazao { get; set; }
		public Int32? BarragemId { set; get; }
		public String CaracterizacaoTid { set; get; }
	}
}