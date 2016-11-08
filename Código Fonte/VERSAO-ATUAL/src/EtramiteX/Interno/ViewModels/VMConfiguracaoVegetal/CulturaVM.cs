using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal
{
	public class CulturaVM
	{

		private Cultura _cultura = new Cultura();

		public Cultura Cultura
		{
			get { return _cultura; }
			set { _cultura = value; }
		}

		private List<SelectListItem> _tipoProducao = new List<SelectListItem>();
		public List<SelectListItem> TipoProducao
		{
			get { return _tipoProducao; }
			set { _tipoProducao = value; }
		}

		private List<SelectListItem> _declaracaoAdicional = new List<SelectListItem>();
		public List<SelectListItem> DeclaracaoAdicional
		{
			get { return _declaracaoAdicional; }
			set { _declaracaoAdicional = value; }
		}
		
		public List<SelectListItem> LsPragas { get; set; }

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@CultivarJaAdicionado = Mensagem.Cultura.CultivarJaAdicionado,
					@CultivarObrigatorio = Mensagem.Cultura.CultivarObrigatorio,
					@CulturaObrigatorio = Mensagem.Cultura.CulturaObrigatorio
				});

			}
		}

		public CulturaVM()
		{

		}

		public CulturaVM(Cultura cultura, List<Lista> tipoProducao, List<Lista> declaracaoAdicional)
		{
			Cultura = cultura;
			TipoProducao = ViewModelHelper.CriarSelectList(tipoProducao, selecionado: "0");
			DeclaracaoAdicional = ViewModelHelper.CriarSelectList(declaracaoAdicional, selecionado: "0");
		}

		public CulturaVM(List<CultivarConfiguracao> item, List<ListaValor> LsPragas, List<Lista> tipoProducao, List<Lista> declaracaoAdicional)
		{			
			this.LsPragas = ViewModelHelper.CriarSelectList(LsPragas,false,true);
			_cultura.Cultivar.LsCultivarConfiguracao = item;
			
			TipoProducao = ViewModelHelper.CriarSelectList(tipoProducao, selecionado: "0");
			DeclaracaoAdicional = ViewModelHelper.CriarSelectList(declaracaoAdicional, selecionado: "0");
		}
	}
}