using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class Tramitacao
	{
		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }
		public String Tid { get; set; }
		public Int32 Tipo { get; set; }
		public Int32 SituacaoId { get; set; }
		public Boolean IsSelecionado { get; set; }
		public Int32 AcaoId { get; set; }
		public String AcaoExecutada { get; set; }
		public bool IsExibirBotao { get; set; }
		public bool IsExisteHistorico { get; set; }

		private Arquivar _arquivamento = new Arquivar();
		public Arquivar Arquivamento
		{
			get { return _arquivamento; }
			set { _arquivamento = value; }
		}

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

		private DateTecno _dataExecucao = new DateTecno();
		public DateTecno DataExecucao
		{
			get { return _dataExecucao; }
			set { _dataExecucao = value; }
		}

		public String Despacho { get; set; }

		private Objetivo _objetivo = new Objetivo();
		public Objetivo Objetivo
		{
			get { return _objetivo; }
			set { _objetivo = value; }
		}

		private Setor _remetenteSetor = new Setor();
		public Setor RemetenteSetor
		{
			get { return _remetenteSetor; }
			set { _remetenteSetor = value; }
		}

		private Setor _destinatarioSetor = new Setor();
		public Setor DestinatarioSetor
		{
			get { return _destinatarioSetor; }
			set { _destinatarioSetor = value; }
		}

		private Setor _executorSetor = new Setor();
		public Setor ExecutorSetor
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

		private Funcionario _executor = new Funcionario();
		public Funcionario Executor
		{
			get { return _executor; }
			set { _executor = value; }
		}

		private Funcionario _remetente = new Funcionario();
		public Funcionario Remetente
		{
			get { return _remetente; }
			set { _remetente = value; }
		}

		private Funcionario _destinatario = new Funcionario();
		public Funcionario Destinatario
		{
			get { return _destinatario; }
			set { _destinatario = value; }
		}

		private List<Tramitacao> _protocolosPosse = new List<Tramitacao>();
		public List<Tramitacao> ProtocolosPosse
		{
			get { return _protocolosPosse; }
			set { _protocolosPosse = value; }
		}

		private List<Tramitacao> _protocolosEnviado = new List<Tramitacao>();
		public List<Tramitacao> ProtocolosEnviado
		{
			get { return _protocolosEnviado; }
			set { _protocolosEnviado = value; }
		}

		private List<Tramitacao> _protocosloReceber= new List<Tramitacao>();
		public List<Tramitacao> ProtocolosReceber
		{
			get { return _protocosloReceber; }
			set { _protocosloReceber = value; }
		}

		private List<Tramitacao> _protocolosReceberSetor = new List<Tramitacao>();
		public List<Tramitacao> ProtocolosReceberSetor
		{
			get { return _protocolosReceberSetor; }
			set { _protocolosReceberSetor = value; }
		}

		private OrgaoClasse _orgaoExterno = new OrgaoClasse();
		public OrgaoClasse OrgaoExterno
		{
			get { return _orgaoExterno; }
			set { _orgaoExterno = value; }
		}

		private TramitacaoArquivo _arquivo = new TramitacaoArquivo();
		public TramitacaoArquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

		public Tramitacao() {}
	}
}