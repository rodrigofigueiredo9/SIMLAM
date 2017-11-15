using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFuncionario;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class FiscalizacaoRelatorioNovo : IAssinanteDataSource
	{
		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public String NumeroFiscalizacao { get; set; }
		public String DataConclusao { get; set; }
		public String OrgaoMunicipio { get; set; }
		public String OrgaoUF { get; set; }

		private FuncionarioRelatorio _usuarioCadastro = new FuncionarioRelatorio();
		public FuncionarioRelatorio UsuarioCadastro
		{
			get { return _usuarioCadastro; }
			set { _usuarioCadastro = value; }
		}

		private ComplementacaoDadosRelatorio _complementacaoDados = new ComplementacaoDadosRelatorio();
		public ComplementacaoDadosRelatorio ComplementacaoDados
		{
			get { return _complementacaoDados; }
			set { _complementacaoDados = value; }
		}

		private ConsideracoesFinaisRelatorio _consideracoesFinais = new ConsideracoesFinaisRelatorio();
		public ConsideracoesFinaisRelatorio ConsideracoesFinais
		{
			get { return _consideracoesFinais; }
			set { _consideracoesFinais = value; }
		}

		private InfracaoRelatorio _infracao = new InfracaoRelatorio();
		public InfracaoRelatorio Infracao
		{
			get { return _infracao; }
			set { _infracao = value; }
		}

		private LocalInfracaoRelatorioNovo _localInfracao = new LocalInfracaoRelatorioNovo();
		public LocalInfracaoRelatorioNovo LocalInfracao
		{
			get { return _localInfracao; }
			set { _localInfracao = value; }
		}

		private MaterialApreendidoRelatorio _materialApreendido = new MaterialApreendidoRelatorio();
		public MaterialApreendidoRelatorio MaterialApreendido
		{
			get { return _materialApreendido; }
			set { _materialApreendido = value; }
		}

		private ObjetoInfracaoRelatorioNovo _objetoInfracao = new ObjetoInfracaoRelatorioNovo();
		public ObjetoInfracaoRelatorioNovo ObjetoInfracao
		{
			get { return _objetoInfracao; }
			set { _objetoInfracao = value; }
		}

        private MultaRelatorio _multa = new MultaRelatorio();
        public MultaRelatorio Multa
        {
            get { return _multa; }
            set { _multa = value; }
        }

		private AcompanhamentoRelatorio _acompanhamento = new AcompanhamentoRelatorio();
		public AcompanhamentoRelatorio Acompanhamento
		{
			get { return _acompanhamento; }
			set { _acompanhamento = value; }
		}

		public IAssinante Assinante { get; set; }
		public List<IAssinante> Assinantes1 { get; set; }
		public List<IAssinante> Assinantes2 { get; set; }

		private List<IAssinante> _assinanteSource = new List<IAssinante>();
		public List<IAssinante> AssinanteSource
		{
			get { return _assinanteSource; }
			set { _assinanteSource = value; }
		}
	}
}
