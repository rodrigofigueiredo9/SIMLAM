using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class BeneficiamentoMadeiraBeneficiamentoVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }
		public MateriaPrimaFlorestalConsumidaVM MateriaPrimaFlorestalConsumida { get; set; }

		private BeneficiamentoMadeiraBeneficiamento _caracterizacao = new BeneficiamentoMadeiraBeneficiamento();
		public BeneficiamentoMadeiraBeneficiamento Caracterizacao
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

		private int _serrariasQuandoNaoAssociadasAFabricacaoDeEstruturas = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SerrariasQuandoNaoAssociadasAFabricacaoDeEstruturas);
		private int _fabricacaoDeEstruturasDeMadeiraComAplicacaoRural = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.FabricacaoDeEstruturasDeMadeiraComAplicacaoRural);

		public bool MostrarVolumeMadeiraSerrar { get { return Caracterizacao.Atividade == _serrariasQuandoNaoAssociadasAFabricacaoDeEstruturas; } }
		public bool MostrarVolumeMadeiraProcessar { get { return !MostrarVolumeMadeiraSerrar; } }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SerrariasQuandoNaoAssociadasAFabricacaoDeEstruturas = _serrariasQuandoNaoAssociadasAFabricacaoDeEstruturas,
					@FabricacaoDeEstruturasDeMadeiraComAplicacaoRural = _fabricacaoDeEstruturasDeMadeiraComAplicacaoRural,
				});
			}
		}

		public BeneficiamentoMadeiraBeneficiamentoVM(List<ProcessoAtividadeItem> atividades, List<Lista> materiaPrimaConsumida, List<Lista> tipoGeometrico, List<Lista> unidadeMedida)
		{
			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Id == _serrariasQuandoNaoAssociadasAFabricacaoDeEstruturas || x.Id == _fabricacaoDeEstruturasDeMadeiraComAplicacaoRural).ToList(), true, true);
			CoodernadaAtividade = new CoordenadaAtividadeVM(new CoordenadaAtividade(), new List<Lista>(), tipoGeometrico);
			MateriaPrimaFlorestalConsumida = new MateriaPrimaFlorestalConsumidaVM(new List<MateriaPrima>(), materiaPrimaConsumida, unidadeMedida);
		}

		public BeneficiamentoMadeiraBeneficiamentoVM(BeneficiamentoMadeiraBeneficiamento caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> materiaPrimaConsumida, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, List<Lista> unidadeMedida, bool isVisualizar = false, bool isEditar = false)
		{

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;

			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Id == _serrariasQuandoNaoAssociadasAFabricacaoDeEstruturas || x.Id == _fabricacaoDeEstruturasDeMadeiraComAplicacaoRural).ToList(), true, true, selecionado: caracterizacao.Atividade.ToString());

			CoodernadaAtividade = new CoordenadaAtividadeVM(caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
			MateriaPrimaFlorestalConsumida = new MateriaPrimaFlorestalConsumidaVM(caracterizacao.MateriasPrimasFlorestais, materiaPrimaConsumida, unidadeMedida, isVisualizar);

		}

	}
}