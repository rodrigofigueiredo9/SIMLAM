using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeAlterarSituacao { get; set; }

		private List<SelectListItem> _listaMunicipios = new List<SelectListItem>();
		public List<SelectListItem> ListaMunicipios
		{
			get { return _listaMunicipios; }
			set { _listaMunicipios = value; }
		}

		private List<SelectListItem> _situacaoLst = new List<SelectListItem>();
		public List<SelectListItem> SituacaoLst
		{
			get { return _situacaoLst; }
			set { _situacaoLst = value; }
		}

		private List<SelectListItem> _origemLst = new List<SelectListItem>();
		public List<SelectListItem> OrigemLst
		{
			get { return _origemLst; }
			set { _origemLst = value; }
		}

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private SolicitacaoListarFiltro _filtros = new SolicitacaoListarFiltro();
		public SolicitacaoListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SolicitacaoListarResultados> _resultados = new List<SolicitacaoListarResultados>();
		public List<SolicitacaoListarResultados> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public ListarVM() : this(new List<QuantPaginacao>(), new List<Municipio>(), new List<Lista>(), new List<Lista>()) { }

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Municipio> listaMunicipios, List<Lista> situacoes, List<Lista> origens)
		{
			ListaMunicipios = ViewModelHelper.CriarSelectList(listaMunicipios, true, true);
			SituacaoLst = ViewModelHelper.CriarSelectList(situacoes, true, true);
			OrigemLst = ViewModelHelper.CriarSelectList(origens, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

        public string Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @GerarPdfSICARUrlNaoEncontrada = Mensagem.CARSolicitacao.GerarPdfSICARUrlNaoEncontrada,
                    @ReenviarMsgConfirmacao = Mensagem.CARSolicitacao.@ReenviarMsgConfirmacao

                });
            }
        }

        public string IdsTela
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @SolicitacaoEmCadastro = (int)eCARSolicitacaoSituacao.EmCadastro,
                    @SolicitacaoPendente = (int)eCARSolicitacaoSituacao.Pendente,
                    @ArquivoArquivoReprovado = (int)eStatusArquivoSICAR.ArquivoReprovado,
                    @ArquivoAguardandoEnvio = (int)eStatusArquivoSICAR.AguardandoEnvio
                });
            }
        }
	}
}