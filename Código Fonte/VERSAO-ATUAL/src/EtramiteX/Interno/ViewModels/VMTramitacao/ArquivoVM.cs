using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class TramitacaoArquivoVM
	{
		private bool _podeEditarSetor = true;
		public bool PodeEditarSetor
		{
			get { return _podeEditarSetor; }
			set { _podeEditarSetor = value; }
		}

		private TramitacaoArquivo _tramitacaoArquivo = new TramitacaoArquivo();
		public TramitacaoArquivo TramitacaoArquivo
		{
			get { return _tramitacaoArquivo; }
		}

		private EstanteVM _estante = new EstanteVM();
		public EstanteVM EstanteTemplate
		{
			get { return _estante; }
			set { _estante = value; }
		}

		private List<EstanteVM> _estantes = new List<EstanteVM>();
		public List<EstanteVM> EstantesVM
		{
			get { return _estantes; }
			set { _estantes = value; }
		}

		public bool IsVisualizar { get; set; }

		#region Listas

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private List<SelectListItem> _tipo = new List<SelectListItem>();
		public List<SelectListItem> Tipos
		{
			get { return _tipo; }
			set { _tipo = value; }
		}

		private List<Situacao> _situacoesProcessoAtividade = new List<Situacao>();
		public List<Situacao> SituacoesProcessoAtividade
		{
			get { return _situacoesProcessoAtividade; }
			set { _situacoesProcessoAtividade = value; }
		}

		private List<Situacao> _situacoesDocumentoAtividade = new List<Situacao>();
		public List<Situacao> SituacoesDocumentoAtividade
		{
			get { return _situacoesDocumentoAtividade; }
			set { _situacoesDocumentoAtividade = value; }
		}

		#endregion

		public TramitacaoArquivoVM(List<Setor> setores, List<TramitacaoArquivoTipo> tipos, List<Situacao> situacoesProcessoAtiv, List<Situacao> situacoesDocumentoAtiv)
		{
			Setores = ViewModelHelper.CriarSelectList(setores, true);
			Tipos = ViewModelHelper.CriarSelectList(tipos, true);
			SituacoesProcessoAtividade = situacoesProcessoAtiv;
			SituacoesDocumentoAtividade = situacoesDocumentoAtiv;
		}

		public void CarregarTramitacaoArquivo(TramitacaoArquivo arquivo, List<TramitacaoArquivoModo> modos)
		{
			_tramitacaoArquivo = arquivo;
			CarregarEstantes(arquivo.Estantes, modos);
		}

		private void CarregarEstantes(List<Estante> Estantes, List<TramitacaoArquivoModo> modos)
		{
			List<SelectListItem> listaModos = ViewModelHelper.CriarSelectList(modos, true);
			EstanteTemplate.Modos = listaModos;
			
			EstanteVM estanteVM = null;

			foreach (Estante estante in Estantes)
			{
				estanteVM = new EstanteVM();
				estanteVM.IsVisualizar = IsVisualizar;
				estanteVM.Modos = listaModos;
				estanteVM.Carregar(estante);
				EstantesVM.Add(estanteVM);
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					SetorSemArquivo = Mensagem.Arquivamento.SetorSemArquivo,

					EstanteItemArquivoObrigratorio = Mensagem.TramitacaoArquivo.EstanteItemArquivoObrigratorio,
					PrateleiraItemArquivoObrigratorio = Mensagem.TramitacaoArquivo.PrateleiraItemArquivoObrigratorio,
					EstanteItemArquivoJaAdicionada = Mensagem.TramitacaoArquivo.EstanteItemArquivoJaAdicionada,
					PrateleiraItemArquivoJaAdicionada = Mensagem.TramitacaoArquivo.PrateleiraItemArquivoJaAdicionada,

					ModoObrigratorio = Mensagem.TramitacaoArquivo.ModoObrigratorio,
					IdentificacaoObrigratorio = Mensagem.TramitacaoArquivo.IdentificacaoObrigratorio,
					ExcluirEstante = Mensagem.TramitacaoArquivo.ExcluirEstante,
					CamposObrigatorio  = Mensagem.TramitacaoArquivo.CamposObrigatorio,

					NomeArquivoObrigratorio = Mensagem.TramitacaoArquivo.NomeArquivoObrigratorio,
					SetorArquivoObrigratorio = Mensagem.TramitacaoArquivo.SetorArquivoObrigratorio,
					TipoArquivoObrigratorio = Mensagem.TramitacaoArquivo.TipoArquivoObrigratorio,
					EstanteArquivoObrigratorio = Mensagem.TramitacaoArquivo.EstanteArquivoObrigratorio,
					PrateleiraArquivoObrigratorio = Mensagem.TramitacaoArquivo.PrateleiraArquivoObrigratorio,
					ProcessoOuDocumentoArqObrigratorio = Mensagem.TramitacaoArquivo.ProcessoOuDocumentoArqObrigratorio,

					SalvarArquivo = Mensagem.TramitacaoArquivo.SalvarArquivo,
					EditarArquivo = Mensagem.TramitacaoArquivo.EditarArquivo,
					ExcluirArquivo = Mensagem.TramitacaoArquivo.ExcluirArquivo()
				});
			}
		}

		public string ObterJsonItem(object item)
		{
			if (item is Estante)
			{
				Estante objeto = item as Estante;
				return HttpUtility.HtmlEncode(ViewModelHelper.Json(new { @Id = objeto.Id, @Texto = objeto.Texto, @Arquivo = objeto.Arquivo }));
			}
			else
			{
				Prateleira objeto = item as Prateleira;
				return HttpUtility.HtmlEncode(ViewModelHelper.Json(new { @Id = objeto.Id, @Texto = objeto.Texto, @Arquivo = objeto.Arquivo }));
			}
		}
	}
}