using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloTerraplanagem;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class TerraplanagemVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }

		private Terraplanagem _caracterizacao = new Terraplanagem();
		public Terraplanagem Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<SelectListItem> _atividade = new List<SelectListItem>();
		public List<SelectListItem> Atividade
		{
			get { return _atividade; }
			set { _atividade = value; }
		}

		public TerraplanagemVM(Terraplanagem caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, List<Lista> unidadeMedida, bool isVisualizar = false, bool isEditar = false)
		{

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;
			int idAtividade = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.TerraplanagemCorteAterro);

			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Id == idAtividade).ToList(), true, true, selecionado: idAtividade.ToString());

			CoodernadaAtividade = new CoordenadaAtividadeVM(Caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
		}
		
	}
}