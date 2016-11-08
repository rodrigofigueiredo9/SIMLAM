using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProducaoCarvaoVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class ProducaoCarvaoVegetalVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public Decimal TotalVolumeFornos
		{
			get
			{
				Decimal retorno = 0;
				if (Caracterizacao.Fornos != null && Caracterizacao.Fornos.Count > 0)
				{
					Decimal aux = 0;
					foreach (Forno forno in Caracterizacao.Fornos)
					{
						if (Decimal.TryParse(forno.Volume, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno;
			}
		}

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }
		public MateriaPrimaFlorestalConsumidaVM MateriaPrimaFlorestalConsumida { get; set; }

		private ProducaoCarvaoVegetal _caracterizacao = new ProducaoCarvaoVegetal();
		public ProducaoCarvaoVegetal Caracterizacao
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
					@NumeroFornosObrigatorio = Mensagem.ProducaoCarvaoVegetal.NumeroFornosObrigatorio,
					@NumeroFornosInvalido = Mensagem.ProducaoCarvaoVegetal.NumeroFornosInvalido,
					@NumeroFornosMaiorZero = Mensagem.ProducaoCarvaoVegetal.NumeroFornosMaiorZero,
					@NumeroFornosMenorQueFornosAdicionados = Mensagem.ProducaoCarvaoVegetal.NumeroFornosMenorQueFornosAdicionados,
					@FornosJaAdicionados = Mensagem.ProducaoCarvaoVegetal.FornosJaAdicionados,

					@VolumeFornoObrigatorio = Mensagem.ProducaoCarvaoVegetal.VolumeFornoObrigatorio,
					@VolumeFornoInvalido = Mensagem.ProducaoCarvaoVegetal.VolumeFornoInvalido,
					@VolumeFornoMaiorZero = Mensagem.ProducaoCarvaoVegetal.VolumeFornoMaiorZero,
				});
			}
		}

		public ProducaoCarvaoVegetalVM(ProducaoCarvaoVegetal caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> materiaPrimaConsumida, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, List<Lista> unidadeMedida, bool isVisualizar = false, bool isEditar = false)
		{
			int AtividadeId = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ProducaoCarvaoVegetalExclusivoParaFornosNaoIndustriaisLicenciamento);

			IsVisualizar = isVisualizar;
			IsEditar = isEditar;
			Caracterizacao = caracterizacao;
			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Id == AtividadeId).ToList(), true, true, selecionado: AtividadeId.ToString());

			CoodernadaAtividade = new CoordenadaAtividadeVM(caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, isVisualizar);
			MateriaPrimaFlorestalConsumida = new MateriaPrimaFlorestalConsumidaVM(caracterizacao.MateriasPrimasFlorestais, materiaPrimaConsumida, unidadeMedida, isVisualizar);
		}
	}
}