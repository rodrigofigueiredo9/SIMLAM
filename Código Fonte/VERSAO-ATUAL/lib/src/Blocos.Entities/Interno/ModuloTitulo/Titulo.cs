using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class Titulo
	{
		public int Id { get; set; }
		public int? IdRelacionamento { get; set; }
		public string Tid { get; set; }
		public int? EmpreendimentoId { get; set; }
		public long EmpreendimentoCodigo { get; set; }
		public string EmpreendimentoTexto { get; set; }
		public int? Prazo { get; set; }
		public string PrazoUnidade { get; set; }
		public int? DiasProrrogados { get; set; }
		public int? MotivoEncerramentoId { get; set; }
		public string MotivoSuspensao { get; set; }
		public int? RequerimentoAtividades { get; set; }
		public bool IgnorarIntegracao { get; set; }

		private TituloNumero _numero = new TituloNumero();
		public TituloNumero Numero { get { return _numero; } set { _numero = value; } }

		private Situacao _situacao = new Situacao();
		public Situacao Situacao { get { return _situacao; } set { _situacao = value; } }

		private Setor _setor = new Setor();
		public Setor Setor { get { return _setor; } set { _setor = value; } }

		private Funcionario _autor = new Funcionario();
		public Funcionario Autor { get { return _autor; } set { _autor = value; } }

		private Municipio _localEmissao = new Municipio();
		public Municipio LocalEmissao { get { return _localEmissao; } set { _localEmissao = value; } }

		public TituloModelo Modelo { get; set; }
		public IProtocolo Protocolo { get; set; }

		#region Datas

		private DateTecno _dataCriacao = new DateTecno();
		public DateTecno DataCriacao { get { return _dataCriacao; } set { _dataCriacao = value; } }

		private DateTecno _dataInicioPrazo = new DateTecno();
		public DateTecno DataInicioPrazo { get { return _dataInicioPrazo; } set { _dataInicioPrazo = value; } }

		private DateTecno _dataVencimento = new DateTecno();
		public DateTecno DataVencimento { get { return _dataVencimento; } set { _dataVencimento = value; } }

		private DateTecno _dataEmissao = new DateTecno();
		public DateTecno DataEmissao { get { return _dataEmissao; } set { _dataEmissao = value; } }

		private DateTecno _dataAssinatura = new DateTecno();
		public DateTecno DataAssinatura { get { return _dataAssinatura; } set { _dataAssinatura = value; } }

		private DateTecno _dataEncerramento = new DateTecno();
		public DateTecno DataEncerramento { get { return _dataEncerramento; } set { _dataEncerramento = value; } }

		private DateTecno _dataEntrega = new DateTecno();
		public DateTecno DataEntrega { get { return _dataEntrega; } set { _dataEntrega = value; } }

		/// <summary>
		/// Somente para titulo declaratótio
		/// </summary>
		private DateTecno _dataSituacao = new DateTecno();
		public DateTecno DataSituacao { get { return _dataSituacao; } set { _dataSituacao = value; } }

		#endregion

		private List<String> _condicionantesJson = new List<String>();
		public List<String> CondicionantesJson { get { return _condicionantesJson; } set { _condicionantesJson = value; } }

		private List<TituloCondicionante> _condicionantes = new List<TituloCondicionante>();
		public List<TituloCondicionante> Condicionantes { get { return _condicionantes; } set { _condicionantes = value; } }

		private List<Setor> _setores = new List<Setor>();
		public List<Setor> Setores { get { return _setores; } set { _setores = value; } }

		private List<TituloAssinante> _assinantes = new List<TituloAssinante>();
		public List<TituloAssinante> Assinantes { get { return _assinantes; } set { _assinantes = value; } }

		private List<DestinatarioEmail> _destinatarioEmails = new List<DestinatarioEmail>();
		public List<DestinatarioEmail> DestinatarioEmails { get { return _destinatarioEmails; } set { _destinatarioEmails = value; } }

		private Pessoa _representante = new Pessoa();
		public Pessoa Representante { get { return _representante; } set { _representante = value; } }

		private List<Titulo> _associados = new List<Titulo>();
		public List<Titulo> Associados { get { return _associados; } set { _associados = value; } }

		private List<Atividade> _atividades = new List<Atividade>();
		public List<Atividade> Atividades { get { return _atividades; } set { _atividades = value; } }

		private List<TituloExploracaoFlorestal> _exploracoes = new List<TituloExploracaoFlorestal>();
		public List<TituloExploracaoFlorestal> Exploracoes { get { return _exploracoes; } set { _exploracoes = value; } }

		private Especificidade _especificidade = new Especificidade();
		public Especificidade Especificidade { get { return _especificidade; } set { _especificidade = value; } }

		private Arquivo.Arquivo _arquivoPdf = new Arquivo.Arquivo();
		public Arquivo.Arquivo ArquivoPdf { get { return _arquivoPdf; } set { _arquivoPdf = value; } }

		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		public int? RequerimetoId { get; set; }

		public int? CredenciadoId { get; set; }

		public string CodigoSinaflor { get; set; }

		public Titulo()
		{
			this.Protocolo = new Protocolo();
			this.Modelo = new TituloModelo();
		}
	}
}