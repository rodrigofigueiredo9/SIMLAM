using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca
{
	public class LicencaPorteUsoMotosserra : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public Int32? Vias { set; get; }
		public String AnoExercicio { set; get; }

		private Motosserra _motosserra = new Motosserra();
		public Motosserra Motosserra
		{
			get { return _motosserra; }
			set { _motosserra = value; }
		}
	}
}