using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao
{
	public class TramitacaoRelatorioPDF
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public int Tipo { get; set; }
		public String Despacho { get; set; }
		public int SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public bool IsSelecionado { get; set; }
		public int HistoricoId { get; set; }
		public String AcaoExecutada { get; set; }
		public int AcaoExecutadaId { get; set; }
		public int SetorId { get; set; }

		#region Cabeçalho

		public String GovernoNome { get; set; }
		public String SecretariaNome { get; set; }
		public String OrgaoNome { get; set; }
		public String SetorNome { get; set; }
		public Byte[] LogoBrasao { get; set; }

		#endregion

		private DateTecno _dataEnvio = new DateTecno();
		public DateTecno DataEnvio
		{
			get { return _dataEnvio; }
			set { _dataEnvio = value; }
		}

		private DateTecno _dataRecebido = new DateTecno();
		public DateTecno DataRecebido
		{
			get { return _dataRecebido; }
			set { _dataRecebido = value; }
		}

		private ObjetivoRelatorio _objetivo = new ObjetivoRelatorio();
		public ObjetivoRelatorio Objetivo
		{
			get { return _objetivo; }
			set { _objetivo = value; }
		}

		private SetorRelatorio _remetenteSetor = new SetorRelatorio();
		public SetorRelatorio RemetenteSetor
		{
			get { return _remetenteSetor; }
			set { _remetenteSetor = value; }
		}

		private SetorRelatorio _destinatarioSetor = new SetorRelatorio();
		public SetorRelatorio DestinatarioSetor
		{
			get { return _destinatarioSetor; }
			set { _destinatarioSetor = value; }
		}

		private SetorRelatorio _executorSetor = new SetorRelatorio();
		public SetorRelatorio ExecutorSetor
		{
			get { return _executorSetor; }
			set { _executorSetor = value; }
		}

		Protocolo _processoDocumento = new Protocolo();
		public Protocolo Protocolo
		{
			get { return _processoDocumento; }
			set { _processoDocumento = value; }
		}

		TramitacaoArquivoRelatorio _arquivo = new TramitacaoArquivoRelatorio();
		public TramitacaoArquivoRelatorio Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

		private FuncionarioRelatorio _executor = new FuncionarioRelatorio();
		public FuncionarioRelatorio Executor
		{
			get { return _executor; }
			set { _executor = value; }
		}

		private FuncionarioRelatorio _remetente = new FuncionarioRelatorio();
		public FuncionarioRelatorio Remetente
		{
			get { return _remetente; }
			set { _remetente = value; }
		}

		private FuncionarioRelatorio _destinatario = new FuncionarioRelatorio();
		public FuncionarioRelatorio Destinatario
		{
			get { return _destinatario; }
			set { _destinatario = value; }
		}

		private OrgaoClasseRelatorio _orgaoExterno = new OrgaoClasseRelatorio();
		public OrgaoClasseRelatorio OrgaoExterno
		{
			get { return _orgaoExterno; }
			set { _orgaoExterno = value; }
		}

		public TramitacaoRelatorioPDF() { }
	}
}