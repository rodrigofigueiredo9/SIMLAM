using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDespolpamentoCafe;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class DespolpamentoCafeVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }

		private DespolpamentoCafe _caracterizacao = new DespolpamentoCafe();
		public DespolpamentoCafe Caracterizacao
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

		public DespolpamentoCafeVM(DespolpamentoCafe caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, bool isVisualizar = false, bool isEditar = false)
		{

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;

			int ativDespCafeId = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.DespolpamentoDescascamentoCafeViaUmida);
			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Id == ativDespCafeId).ToList(), true, true, selecionado: ativDespCafeId.ToString());
			CoodernadaAtividade = new CoordenadaAtividadeVM(Caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
		}
	}
}