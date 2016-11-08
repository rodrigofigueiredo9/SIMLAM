using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Oficio : IAnexoPdf
	{
		public int Id { get; set; }
		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public PessoaPDF Destinatario { get; set; }

		public String DestinatarioPGE { get; set; }
		public String Descricao { get; set; }
		public String Dimensao { get; set; }
		public Int32 EmpreendimentoTipo { get; set; }

		public String EmpreendimentoTipoTexto
		{
			get
			{
				if (EmpreendimentoTipo == 1)
				{
					return "Urbano";
				}

				if (EmpreendimentoTipo == 2)
				{
					return "Rural";
				}

				return String.Empty;
			}
		}

		private List<AnaliseSituacaoGrupoPDF> _situacoesGrupo = new List<AnaliseSituacaoGrupoPDF>();
		public List<AnaliseSituacaoGrupoPDF> SituacoesGrupo
		{
			get { return _situacoesGrupo; }
			set { _situacoesGrupo = value; }
		}

		private List<AnexoPDF> _anexos = new List<AnexoPDF>();
		public List<AnexoPDF> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		private List<Arquivo.Arquivo> _anexosPdfs = new List<Arquivo.Arquivo>();
		public List<Arquivo.Arquivo> AnexosPdfs
		{
			get { return _anexosPdfs; }
			set { _anexosPdfs = value; }
		}

		public Oficio()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
		}
	}
}