using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento
{
	public class RequerimentoRelatorio: IAssinanteDataSource
	{
		public int Id { get; set; }
		public int Numero
		{
			get { return Id; }
		}
		public String Tid { get; set; }
		public Int32 HistoricoId { get; set; }
		public DateTime DataCadastro { get; set; }
		public string DataCadastroResumida { get { return DataCadastro.ToShortDateString(); } }
		public String DataCadastroExtenso { get; set; }
		public String AgendamentoVistoria { get; set; }
		public int MunicipioId { get; set; }
		public string Municipio { get; set; }
		public int EmpreendimentoId { get; set; }
		public String EmpreendimentoTid { get; set; }
		public String DiaCadastro { get; set; }
		public String MesCadastro { get; set; }
		public String AnoCadastro { get; set; }

		public int ProjetoDigitalSituacaoId { get; set; }
		public int SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }

		public int SetorId { get; set; }
		public String Informacoes { get; set; }

		public Int32 AutorId { get; set; }
		public String AutorTid { get; set; }
		public Boolean IsCredenciado { get { return (AutorId > 0); } }

		private CredenciadoRelatorio _usuarioCredenciado = new CredenciadoRelatorio();
		public CredenciadoRelatorio UsuarioCredenciado
		{
			get { return _usuarioCredenciado; }
			set { _usuarioCredenciado = value; }
		}

		private PessoaRelatorio _interessado = new PessoaRelatorio();
		public PessoaRelatorio Interessado
		{
			get { return _interessado; }
			set { _interessado = value; }
		}

		private EmpreendimentoRelatorio _empreendimento = new EmpreendimentoRelatorio();
		public EmpreendimentoRelatorio Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}

		private List<RoteiroRelatorio> _roteiros = new List<RoteiroRelatorio>();
		public List<RoteiroRelatorio> Roteiros
		{
			get { return _roteiros; }
			set { _roteiros = value; }
		}

		private List<RequerimentoAtividadeRelatorio> _atividades = new List<RequerimentoAtividadeRelatorio>();
		public List<RequerimentoAtividadeRelatorio> Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<ResponsavelTecnicoRelatorio> _responsaveis = new List<ResponsavelTecnicoRelatorio>();
		public List<ResponsavelTecnicoRelatorio> Responsaveis
		{
			get { return _responsaveis; }
			set { _responsaveis = value; }
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
	}
}