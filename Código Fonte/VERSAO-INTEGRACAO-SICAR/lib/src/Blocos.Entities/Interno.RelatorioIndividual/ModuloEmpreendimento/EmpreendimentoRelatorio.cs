using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento
{
	public class EmpreendimentoRelatorio
	{
		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }
		public String Tid { get; set; }
		public Int32? Codigo { get; set; }
		public Int32? Segmento { get; set; }
		public String SegmentoTexto { get; set; }
		public String CNPJ { get; set; }
		public String AtividadeTexto { get; set; }
		public String NomeFantasia { get; set; }
		public String Denominador { get; set; }
		public String NumeroCCIRStragg { get; set; }

		//responsável
		public String NomeRazao { get; set; }
		public String CpfCnpj { get; set; }
		public Int32 Tipo { get; set; }
		public String DataVencimento { get; set; }
		
		private Int32 _temCorrespondencia = 0;
		private List<ResponsavelRelatorio> _responsaveis =  new List<ResponsavelRelatorio>();
		private List<EnderecoRelatorio> _enderecos = new List<EnderecoRelatorio>();
		private CoordenadaRelatorioPDF _coordenada = new CoordenadaRelatorioPDF();
		private List<ContatoRelatorio> _meios = new List<ContatoRelatorio>();

		public Int32 TemCorrespondencia
		{
			get { return _temCorrespondencia; }
			set { _temCorrespondencia = value; }
		}
		public List<ResponsavelRelatorio> Responsaveis
		{
			get { return _responsaveis; }
			set { _responsaveis = value; }
		}

		public String ResponsaveisStrag { get; set; }

		public List<EnderecoRelatorio> Enderecos
		{
			get { return _enderecos; }
			set { _enderecos = value; }
		}

		public CoordenadaRelatorioPDF Coordenada
		{
			get { return _coordenada; }
			set { _coordenada = value; }
		}
		public List<ContatoRelatorio> MeiosContatos
		{
			get { return _meios; }
			set { _meios = value; }

		}

		public EnderecoRelatorio Endereco { get { return (Enderecos.SingleOrDefault(x => x.Correspondencia == 0) ?? new EnderecoRelatorio()); } }

		public EnderecoRelatorio EnderecoCorresp{ get { return (Enderecos.SingleOrDefault(x => x.Correspondencia == 1) ?? new EnderecoRelatorio()); } }

		public string TelFax { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneFax) ?? new ContatoRelatorio()).Valor; } }
		public string Telefone { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneResidencial) ?? new ContatoRelatorio()).Valor; } }
		public string Email { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.Email) ?? new ContatoRelatorio()).Valor; } }
		public string ContatoNome { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.NomeContato) ?? new ContatoRelatorio()).Valor; } }

		public EmpreendimentoRelatorio() { }

		public int EastingUtm { get; set; }

		public int NorthingUtm { get; set; }
	}
}