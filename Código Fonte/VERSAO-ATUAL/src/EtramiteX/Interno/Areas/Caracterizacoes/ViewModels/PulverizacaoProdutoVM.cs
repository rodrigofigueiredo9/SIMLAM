using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class PulverizacaoProdutoVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }
		public bool TemARL { get; set; }
		public bool TemARLDesconhecida { get; set; }

		public String AreaTotal
		{
			get
			{
				Decimal retorno = 0;
				if (Caracterizacao.Culturas != null && Caracterizacao.Culturas.Count > 0)
				{
					Decimal aux = 0;
					foreach (Cultura cultura in Caracterizacao.Culturas)
					{
						if (Decimal.TryParse(cultura.Area, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno.ToStringTrunc(4);
			}
		}

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }

		private PulverizacaoProduto _caracterizacao = new PulverizacaoProduto();
		public PulverizacaoProduto Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<SelectListItem> _culturas = new List<SelectListItem>();
		public List<SelectListItem> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
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
					@CulturaTipoObrigatorio = Mensagem.PulverizacaoProduto.CulturaTipoObrigatorio,
					@CulturaTipoDuplicado = Mensagem.PulverizacaoProduto.CulturaTipoDuplicado,
					@CulturaEspecificarTextoObrigatorio = Mensagem.PulverizacaoProduto.CulturaEspecificarTextoObrigatorio,

					@CulturaAreaObrigatoria = Mensagem.PulverizacaoProduto.CulturaAreaObrigatoria,
					@CulturaAreaInvalida = Mensagem.PulverizacaoProduto.CulturaAreaInvalida,
					@CulturaAreaMaiorZero = Mensagem.PulverizacaoProduto.CulturaAreaMaiorZero,

					@SemARLConfirm = Mensagem.PulverizacaoProduto.SemARLConfirm,
					@ARLDesconhecidaConfirm = Mensagem.PulverizacaoProduto.ARLDesconhecidaConfirm
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Outros = eCultura.Outros
				});
			}
		}

		public PulverizacaoProdutoVM(PulverizacaoProduto caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, List<Lista> culturasLst, bool isVisualizar = false, bool isEditar = false)
		{
			atividades = atividades.Where(x => x.Codigo == (int)eAtividadeCodigo.PulverizacaoProduto).ToList();

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;
			Culturas = Atividade = ViewModelHelper.CriarSelectList(culturasLst, true, true);
			Atividade = ViewModelHelper.CriarSelectList(atividades, true, true, atividades.First().Id.ToString());
			CoodernadaAtividade = new CoordenadaAtividadeVM(Caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
		}
		
	}
}