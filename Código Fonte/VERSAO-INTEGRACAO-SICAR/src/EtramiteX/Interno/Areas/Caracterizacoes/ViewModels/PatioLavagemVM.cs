using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPatioLavagem;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class PatioLavagemVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }
		public bool TemARL { get; set; }
		public bool TemARLDesconhecida { get; set; }

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }

		private PatioLavagem _caracterizacao = new PatioLavagem();
		public PatioLavagem Caracterizacao
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

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@AreaTotalObrigatoria = Mensagem.PatioLavagem.AreaTotalObrigatoria,
					@AreaTotalInvalida = Mensagem.PatioLavagem.AreaTotalInvalida,
					@AreaTotalMaiorZero = Mensagem.PatioLavagem.AreaTotalMaiorZero
				});
			}
		}

		public PatioLavagemVM(PatioLavagem caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, bool isVisualizar = false, bool isEditar = false)
		{
			atividades = atividades.Where(x => x.Codigo == (int)eAtividadeCodigo.PatioLavagem).ToList();

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;
			Atividade = ViewModelHelper.CriarSelectList(atividades, true, true, atividades.First().Id.ToString());
			CoodernadaAtividade = new CoordenadaAtividadeVM(Caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
		}
		
	}
}