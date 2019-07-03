using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCaracterizacoes;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCadastroAmbientalRural
{
	public class CARSolicitacaoRelatorio : IAssinanteDataSource
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Numero { get; set; }
		public String DataEmissao { get; set; }
		public String DataEnvio { get; set; }
		public Int32 DominialidadeId { get; set; }
		public String DominialidadeTid { get; set; }
		public Int32 RequerimentoNumero { get; set; }
		public String Motivo { get; set; }

		public String SituacaoTexto { get; set; }
		private DateTime _situacaoData = new DateTime();
		public DateTime SituacaoData
		{
			get { return _situacaoData; }
			set { _situacaoData = value; }
		}

		public Boolean DeclarantePossuiOutros { get; set; }
		private String _declaranteOutros = "e Outros";
		public String DeclaranteOutros
		{
			get { return _declaranteOutros; }
			set { _declaranteOutros = value; }
		}

		private String _doisPontos = ":";
		public String DoisPontos
		{
			get { return _doisPontos; }
			set { _doisPontos = value; }
		}

		private PessoaRelatorio _declarante = new PessoaRelatorio();
		public PessoaRelatorio Declarante
		{
			get { return _declarante; }
			set { _declarante = value; }
		}

		private EmpreendimentoRelatorio _empreendimento = new EmpreendimentoRelatorio();
		public EmpreendimentoRelatorio Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}

		private DominialidadeRelatorio _dominialidade = new DominialidadeRelatorio();
		public DominialidadeRelatorio Dominialidade
		{
			get { return _dominialidade; }
			set { _dominialidade = value; }
		}

		private DominialidadePDF _dominialidadePDF = new DominialidadePDF();
		public DominialidadePDF DominialidadePDF
		{
			get { return _dominialidadePDF; }
			set { _dominialidadePDF = value; }
		}

		private List<AssinanteDefault> _assinantes = new List<AssinanteDefault>();
		public List<AssinanteDefault> Assinantes
		{
			get { return _assinantes; }
			set { _assinantes = value; }
		}

		public IAssinante Assinante { get; set; }
		public List<IAssinante> AssinanteSource { get; set; }
		public List<IAssinante> Assinantes1 { get; set; }
		public List<IAssinante> Assinantes2 { get; set; }

        private SicarRelatorio _sicarRelatorio = new SicarRelatorio();
        public SicarRelatorio Sicar
        {
            get { return _sicarRelatorio; }
            set { _sicarRelatorio = value; }
        }
	}
}