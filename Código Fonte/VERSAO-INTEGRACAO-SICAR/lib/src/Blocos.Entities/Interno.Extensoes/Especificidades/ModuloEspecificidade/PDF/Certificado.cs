using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Certificado
	{
		public int Id { get; set; }
		public String AnoExercicio { get; set; }
		public String Vias { set; get; }
		public String DataRevalidacao { set; get; }

		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public PessoaPDF Destinatario { set; get; }
		public AgrotoxicoPDF Agrotoxico { set; get; }

		private List<AnaliseItemPDF> _itens = new List<AnaliseItemPDF>();
		public List<AnaliseItemPDF> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}
		
		private RegistroAtividadeFlorestalPDF _registroAtividadeFlorestal = new RegistroAtividadeFlorestalPDF();
		public RegistroAtividadeFlorestalPDF RegistroAtividadeFlorestal
		{
			get { return _registroAtividadeFlorestal; }
			set { _registroAtividadeFlorestal = value; }
		}

		public Certificado()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
		}
	}
}