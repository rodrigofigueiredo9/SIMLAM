using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMLocalVistoria
{
	public class LocalVistoriaListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeEditar { get; set; }
        public Boolean PodeVisualizar { get; set; }

        private List<SelectListItem> _diaSemanaLista = new List<SelectListItem>();


        public List<SelectListItem> DiaSemanaLista
        {
            get { return _diaSemanaLista; }
            set { _diaSemanaLista = value; }
        }
        
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

        private LocalVistoriaListar _filtros = new LocalVistoriaListar();

        public LocalVistoriaListar Filtros
        {
            get { return _filtros; }
            set { _filtros = value; }

        }

        private List<LocalVistoriaListar> _resultados = new List<LocalVistoriaListar>();
        public List<LocalVistoriaListar> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}


        public string Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @SemPermissaoEditar = Mensagem.Sistema.SemPermissao("Operar Local Vistoria"),
                    @SemPermissaoVisualizar = Mensagem.Sistema.SemPermissao("Visualizar Local Vistoria")
                });

            }
        }


		public LocalVistoriaListarVM() { }

        public LocalVistoriaListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> diasSemana)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
            DiaSemanaLista = ViewModelHelper.CriarSelectList(diasSemana, true);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public String ObterJSon(LocalVistoria item)
		{
			object objeto = new
			{
				Id = item.SetorID,
				Texto = item.SetorTexto
			};

			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(objeto));
		}
	}
}