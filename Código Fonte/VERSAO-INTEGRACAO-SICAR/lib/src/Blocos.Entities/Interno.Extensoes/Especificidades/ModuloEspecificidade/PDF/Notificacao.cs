using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Notificacao 
	{
		public int Id { get; set; }
		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public List<PessoaPDF> Destinatarios { get; set; }
		public String AtividadeEmbargada { get; set; }

		private List<CondicionantePDF> _condicionantes = new List<CondicionantePDF>();
		public List<CondicionantePDF> Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}

		private List<AnaliseItemPDF> _itens = new List<AnaliseItemPDF>();
		public List<AnaliseItemPDF> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		public Notificacao()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatarios = new List<PessoaPDF>();
		}
	}
}