using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFO
{
	public class ListarVM
	{
		public Paginacao Paginacao { get; set; }
		public List<EmissaoCFO> Resultados { get; set; }
		public EmissaoCFO Filtros { get; set; }
		public List<SelectListItem> SituacaoLista { get; set; }

		public String UltimaBusca { get; set; }
		public bool PodeVisualizar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeGerarPDF { get; set; }
		public bool PodeAtivar { get; set; }

		public string IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EmElaboracao = eDocumentoFitossanitarioSituacao.EmElaboracao,
					@Valido = eDocumentoFitossanitarioSituacao.Valido
				});
			}
		}

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@AtivarSituacaoInvalida = Mensagem.EmissaoCFO.AtivarSituacaoInvalida
				});
			}
		}

		public ListarVM() : this(new List<QuantPaginacao>(), new List<Lista>()) { }

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> situacoes)
		{
			Paginacao = new Paginacao();
			Resultados = new List<EmissaoCFO>();
			Filtros = new EmissaoCFO();

			SituacaoLista = ViewModelHelper.CriarSelectList(situacoes, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}