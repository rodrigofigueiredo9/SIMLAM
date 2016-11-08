using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class SecagemMecanicaGraosVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }
		public MateriaPrimaFlorestalConsumidaVM MateriaPrimaFlorestalConsumida { get; set; }

		public Decimal CapacidadeTotalSecadores
		{
			get 
			{
				Decimal retorno = 0;
				if (Caracterizacao.Secadores != null && Caracterizacao.Secadores.Count > 0)
				{
					Decimal aux = 0;
					foreach (Secador secador in Caracterizacao.Secadores)
					{
						if (Decimal.TryParse(secador.Capacidade, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno;
			}
		}

		private SecagemMecanicaGraos _caracterizacao = new SecagemMecanicaGraos();
		public SecagemMecanicaGraos Caracterizacao
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
					@NumeroSecadorObrigatorio = Mensagem.SecagemMecanicaGraos.NumeroSecadorObrigatorio,
					@NumeroSecadorInvalido = Mensagem.SecagemMecanicaGraos.NumeroSecadorInvalido,
					@NumeroSecadorMaiorZero = Mensagem.SecagemMecanicaGraos.NumeroSecadorMaiorZero,
					@NumeroSecadorMenorQueSecadoresAdicionados = Mensagem.SecagemMecanicaGraos.NumeroSecadorMenorQueSecadoresAdicionados,
					@SecadoresJaAdicionados = Mensagem.SecagemMecanicaGraos.SecadoresJaAdicionados,

					@CapacidadeSecadorObrigatorio = Mensagem.SecagemMecanicaGraos.CapacidadeSecadorObrigatorio,
					@CapacidadeSecadorInvalido = Mensagem.SecagemMecanicaGraos.CapacidadeSecadorInvalido,
					@CapacidadeSecadorMaiorZero = Mensagem.SecagemMecanicaGraos.CapacidadeSecadorMaiorZero,
				});
			}
		}

		public SecagemMecanicaGraosVM(SecagemMecanicaGraos caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> materiaPrimaConsumida, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, List<Lista> unidadeMedida, bool isVisualizar = false, bool isEditar = false)
		{

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;

			Atividade = ViewModelHelper.CriarSelectList(atividades.Where(x => x.Codigo == (int)eAtividadeCodigo.SecagemMecanicaGraos || x.Codigo == (int)eAtividadeCodigo.SecagemMecanicaGraosAssociadosPilagem).ToList(), true, true, selecionado: caracterizacao.Atividade.ToString());

			CoodernadaAtividade = new CoordenadaAtividadeVM(Caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
			MateriaPrimaFlorestalConsumida = new MateriaPrimaFlorestalConsumidaVM(caracterizacao.MateriasPrimasFlorestais, materiaPrimaConsumida, unidadeMedida, isVisualizar);
		}
		
	}
}