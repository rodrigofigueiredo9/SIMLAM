using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal
{
	public class ConfiguracaoVegetalVM
	{
		private ConfiguracaoVegetalItem _configuracaoVegetalItem = new ConfiguracaoVegetalItem();
		private List<ConfiguracaoVegetalItem> _itens = new List<ConfiguracaoVegetalItem>();

		public string Titulo { get; set; }
		public string Label { get; set; }
		public string UrlSalvar { get; set; }
		public string UrlEditar { get; set; }
		public string UrlCancelar { get; set; }

		private bool _mostrarGrid = true;
		public bool MostrarGrid
		{
			get { return _mostrarGrid; }
			set { _mostrarGrid = value; }
		}

		public List<ConfiguracaoVegetalItem> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		public ConfiguracaoVegetalItem ConfiguracaoVegetalItem
		{
			get { return _configuracaoVegetalItem; }
			set { _configuracaoVegetalItem = value; }
		}

		private List<SelectListItem> _situacoesLista = new List<SelectListItem>();
		public List<SelectListItem> SituacoesLista
		{
			get { return _situacoesLista; }
			set { _situacoesLista = value; }
		}

		public ConfiguracaoVegetalVM() { }

		public ConfiguracaoVegetalVM(eConfiguracaoVegetalItemTipo tipo, string acaoTela = "Cadastrar")
		{
			Titulo = acaoTela;

			switch (tipo)
			{
				case eConfiguracaoVegetalItemTipo.GrupoQuimico:
					Titulo += " Grupo Químico";
					break;
				case eConfiguracaoVegetalItemTipo.ClasseUso:
					Titulo += " Classe de Uso";
					break;
				case eConfiguracaoVegetalItemTipo.PericulosidadeAmbiental:
					Titulo += " Periculosidade Ambiental";
					break;
				case eConfiguracaoVegetalItemTipo.ClassificacaoToxicologica:
					Titulo += " Classificação Toxicológica";
					break;
				case eConfiguracaoVegetalItemTipo.ModalidadeAplicacao:
					Titulo += " Modalidade de Aplicação";
					break;
				case eConfiguracaoVegetalItemTipo.FormasApresentacao:
					Titulo += " Formas de Apresentação";
					break;
				case eConfiguracaoVegetalItemTipo.IngredienteAtivo:
					Titulo += " Ingrediente Ativo";
					break;
			}

			Label = Titulo.Substring(Titulo.IndexOf(' ')).Trim();
		}

		public void SetListItens(List<Situacao> situacoes)
		{
			SituacoesLista = ViewModelHelper.CriarSelectList(situacoes, true);
		}
	}
}