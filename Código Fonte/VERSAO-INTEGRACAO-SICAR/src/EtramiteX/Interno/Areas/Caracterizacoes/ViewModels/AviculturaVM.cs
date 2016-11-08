using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAvicultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class AviculturaVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public Decimal AreaTotalConfinamento
		{
			get 
			{
				Decimal retorno = 0;
				if (Caracterizacao.Confinamentos != null && Caracterizacao.Confinamentos.Count > 0)
				{
					Decimal aux = 0;
					foreach (ConfinamentoAves confinamento in Caracterizacao.Confinamentos)
					{
						if (Decimal.TryParse(confinamento.Area, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno;
			}
		}

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }

		private Avicultura _caracterizacao = new Avicultura();
		public Avicultura Caracterizacao
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

		private List<SelectListItem> _finalidades = new List<SelectListItem>();
		public List<SelectListItem> Finalidades
		{
			get { return _finalidades; }
			set { _finalidades = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ConfinamentoFinalidadeObrigatorio = Mensagem.Avicultura.ConfinamentoFinalidadeObrigatorio,
					@ConfinamentoFinalidadeDuplicada = Mensagem.Avicultura.ConfinamentoFinalidadeDuplicada,

					@ConfinamentoAreaObrigatoria = Mensagem.Avicultura.ConfinamentoAreaObrigatoria,
					@ConfinamentoAreaInvalida = Mensagem.Avicultura.ConfinamentoAreaInvalida,
					@ConfinamentoAreaMaiorZero = Mensagem.Avicultura.ConfinamentoAreaMaiorZero
				});
			}
		}

		public AviculturaVM(Avicultura caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> confinamentofinalidadeslst, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, bool isVisualizar = false, bool isEditar = false)
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;
			Finalidades = ViewModelHelper.CriarSelectList(confinamentofinalidadeslst, true, true);

			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Codigo == (int)eAtividadeCodigo.Avicultura).ToList(), true, true, selecionado: (ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.Avicultura)).ToString());
			CoodernadaAtividade = new CoordenadaAtividadeVM(Caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
		}
	}
}