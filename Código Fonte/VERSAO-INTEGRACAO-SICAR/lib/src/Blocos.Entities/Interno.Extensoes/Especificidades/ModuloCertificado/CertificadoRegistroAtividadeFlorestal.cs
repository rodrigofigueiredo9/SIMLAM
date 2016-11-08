using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado
{
	public class CertificadoRegistroAtividadeFlorestal : Especificidade
	{
		public Int32 Id { set; get; }
		public Int32 Requerimento { get; set; }
		public Int32 Destinatario { set; get; }
		public String DestinatarioNomeRazao { get; set; }
		public String AnoExercicio { get; set; }
		public String Vias { set; get; }
		public String Tid { set; get; }
	}
}