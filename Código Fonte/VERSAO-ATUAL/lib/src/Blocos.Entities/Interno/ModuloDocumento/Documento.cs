using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo
{
	public class Documento : IProtocolo
	{
		private Protocolo _protocolo = null;		
		
		public Int32? Id
		{
			get { return _protocolo.Id; }
			set { _protocolo.Id = value; }
		}

		public String Tid
		{
			get { return _protocolo.Tid; }
			set { _protocolo.Tid = value; }
		}
		public Int32 IdRelacionamento
		{
			get { return _protocolo.IdRelacionamento; }
			set { _protocolo.IdRelacionamento = value; }
		}
		public Int32? Ano
		{
			get { return _protocolo.Ano; }
			set { _protocolo.Ano = value; }
		}
		public Int32? NumeroProtocolo
		{
			get { return _protocolo.NumeroProtocolo; }
			set { _protocolo.NumeroProtocolo = value; }
		}
		public String NumeroAutuacao
		{
			get { return _protocolo.NumeroAutuacao; }
			set { _protocolo.NumeroAutuacao = value; }
		}
		public Int32? Volume
		{
			get { return _protocolo.Volume; }
			set { _protocolo.Volume = value; }
		}
		public Int32 SituacaoId
		{
			get { return _protocolo.SituacaoId; }
			set { _protocolo.SituacaoId = value; }
		}
		public String SituacaoTexto
		{
			get { return _protocolo.SituacaoTexto; }
			set { _protocolo.SituacaoTexto = value; }
		}
		public Boolean IsArquivado
		{
			get { return _protocolo.IsArquivado; }
			set { _protocolo.IsArquivado = value; }
		}
		public Boolean IsProcesso
		{
			get { return _protocolo.IsProcesso; }
			set { _protocolo.IsProcesso = value; }
		}
		public Int32 SetorId
		{
			get { return _protocolo.SetorId; }
			set { _protocolo.SetorId = value; }
		}
		public Int32 SetorCriacaoId
		{
			get { return _protocolo.SetorCriacaoId; }
			set { _protocolo.SetorCriacaoId = value; }
		}
		public String Numero
		{
			get { return _protocolo.Numero; }
		}
		public ProtocoloTipo Tipo
		{
			get { return _protocolo.Tipo; }
			set { _protocolo.Tipo = value; }
		}
		public DateTecno DataCadastro
		{
			get { return _protocolo.DataCadastro; }
			set { _protocolo.DataCadastro = value; }
		}
		public DateTecno DataAutuacao
		{
			get { return _protocolo.DataAutuacao; }
			set { _protocolo.DataAutuacao = value; }
		}
		public Pessoa Interessado
		{
			get { return _protocolo.Interessado; }
			set { _protocolo.Interessado= value; }
		}
		public Requerimento Requerimento
		{
			get { return _protocolo.Requerimento; }
			set { _protocolo.Requerimento = value; }
		}
		public Fiscalizacao Fiscalizacao
		{
			get { return _protocolo.Fiscalizacao; }
			set { _protocolo.Fiscalizacao = value; }
		}
		public Empreendimento Empreendimento
		{
			get { return _protocolo.Empreendimento; }
			set { _protocolo.Empreendimento = value; }
		}
		public ChecagemRoteiro ChecagemRoteiro
		{
			get { return _protocolo.ChecagemRoteiro; }
			set { _protocolo.ChecagemRoteiro = value; }
		}
		public List<Atividade> Atividades
		{
			get { return _protocolo.Atividades; }
			set { _protocolo.Atividades = value; }
		}
		public List<ResponsavelTecnico> Responsaveis
		{
			get { return _protocolo.Responsaveis; }
			set { _protocolo.Responsaveis = value; }
		}
		public Arquivo.Arquivo Arquivo
		{
			get { return _protocolo.Arquivo; }
			set { _protocolo.Arquivo = value; }
		}		
		public Funcionario Emposse
		{
			get { return _protocolo.Emposse; }
			set { _protocolo.Emposse = value; }
		}

		public String InteressadoLivre
		{
			get { return _protocolo.InteressadoLivre; }
			set { _protocolo.InteressadoLivre = value; }
		}

		public String InteressadoLivreTelefone
		{
			get { return _protocolo.InteressadoLivreTelefone; }
			set { _protocolo.InteressadoLivreTelefone = value; }
		}

		public Int32 Folhas
		{
			get { return _protocolo.Folhas; }
			set { _protocolo.Folhas = value; }
		}

		private Setor _destinatarioSetor = new Setor();
		public Setor DestinatarioSetor
		{
			get { return _destinatarioSetor; }
			set { _destinatarioSetor = value; }
		}

		private Funcionario _destinatario = new Funcionario();
		public Funcionario Destinatario
		{
			get { return _destinatario; }
			set { _destinatario = value; }
		}

		#region Apenas Documento

		private ChecagemPendencia _checagemPendencia = new ChecagemPendencia();
		public ChecagemPendencia ChecagemPendencia
		{
			get { return _checagemPendencia; }
			set { _checagemPendencia = value; }
		}

		private Protocolo _protocoloAssociado = new Protocolo();
		public Protocolo ProtocoloAssociado
		{
			get { return _protocoloAssociado; }
			set { _protocoloAssociado = value; }
		}

		public String Nome { get; set; }
		public String Assunto { get; set; }
		public String Descricao { get; set; }

		public String OrgaoDestino { get; set; }
		public String CargoFuncaoDestinatario { get; set; }
		public String NomeDestinatario { get; set; }
		public String EnderecoDestinatario { get; set; }

		private List<TituloAssinante> _assinantes = new List<TituloAssinante>();
		public List<TituloAssinante> Assinantes { get { return _assinantes; } set { _assinantes = value; } }

		#endregion

		public Documento(Protocolo protocolo)
		{
			_protocolo = protocolo;
		}

		public Documento()
		{
			_protocolo = new Protocolo();
		}
	}
}