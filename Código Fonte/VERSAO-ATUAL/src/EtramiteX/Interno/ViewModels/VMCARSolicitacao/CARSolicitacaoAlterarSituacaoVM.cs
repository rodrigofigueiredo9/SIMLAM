using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao
{
	public class CARSolicitacaoAlterarSituacaoVM
	{
		private CARSolicitacao _solicitacao = new CARSolicitacao();
		public CARSolicitacao Solicitacao
		{
			get { return _solicitacao; }
			set { _solicitacao = value; }
		}

		public bool isVisualizar { get; set; }

		public List<SelectListItem> Situacoes { get; private set; }
		public List<SelectListItem> Motivos { get; private set; }

		public CARSolicitacaoAlterarSituacaoVM() : this(new CARSolicitacao(), new List<Lista>(), new List<Lista>()) { }

		public CARSolicitacaoAlterarSituacaoVM(CARSolicitacao solicitacao, List<Lista> _situacoes, List<Lista> _motivos)
		{
			Solicitacao = solicitacao;
			Situacoes = ViewModelHelper.CriarSelectList(_situacoes, true, true);
			Motivos = ViewModelHelper.CriarSelectList(_motivos, true, true);
		}

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@AlterarSituacaoMsgConfirmacao = Mensagem.CARSolicitacao.@AlterarSituacaoMsgConfirmacao

				});
			}
		}

		private Arquivo _arquivo = new Arquivo();
		public Arquivo ArquivoAnexo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}
		private Funcionario _autor = new Funcionario();
		public Funcionario Autor
		{
			get { return _autor; }
			set { _autor = value; }
		}

		public string AutorizacaoJson
		{
			get
			{
				if (ArquivoAnexo == null || ArquivoAnexo.Id <= 0)
					return "";
				return ViewModelHelper.Json(new
				{
					@Id = ArquivoAnexo.Id,
					@Raiz = ArquivoAnexo.Raiz,
					@Nome = ArquivoAnexo.Nome,
					@Extensao = ArquivoAnexo.Extensao,
					@Caminho = ArquivoAnexo.Caminho,
					@Diretorio = ArquivoAnexo.Diretorio,
					@TemporarioPathNome = ArquivoAnexo.TemporarioPathNome,
					@ContentType = ArquivoAnexo.ContentType,
					@ContentLength = ArquivoAnexo.ContentLength,
					@Tid = ArquivoAnexo.Tid,
					@Apagar = ArquivoAnexo.Apagar,
					@Conteudo = ArquivoAnexo.Conteudo,
					@DiretorioConfiguracao = ArquivoAnexo.DiretorioConfiguracao,
					@TemporarioNome = ArquivoAnexo.TemporarioNome
				});
			}
		}

	}
}