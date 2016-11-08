using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSuinocultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class SuinoculturaVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsEditar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		public CoordenadaAtividadeVM CoodernadaAtividade { get; set; }
		public MateriaPrimaFlorestalConsumidaVM MateriaPrimaFlorestalConsumida { get; set; }

		private Suinocultura _caracterizacao = new Suinocultura();
		public Suinocultura Caracterizacao
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

		private List<SelectListItem> _fases = new List<SelectListItem>();
		public List<SelectListItem> Fases
		{
			get { return _fases; }
			set { _fases = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@NumeroCabecaObrigatorio = Mensagem.Suinocultura.NumeroCabecaObrigatorio,
					@NumeroCabecaInvalido = Mensagem.Suinocultura.NumeroCabecaInvalido,
					@NumeroCabecaMaiorZero = Mensagem.Suinocultura.NumeroCabecaMaiorZero,

					@NumeroMatrizesObrigatorio = Mensagem.Suinocultura.NumeroMatrizesObrigatorio,
					@NumeroMatrizesInvalido = Mensagem.Suinocultura.NumeroMatrizesInvalido,
					@NumeroMatrizesMaiorZero = Mensagem.Suinocultura.NumeroMatrizesMaiorZero,
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SuinoculturaCicloCompleto = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaCicloCompleto), //Atividade 1
					@SuinoculturaExclusivoProducaoLeitoes = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaExclusivoProducaoLeitoes), //Atividade 2
					@SuinoculturaExclusivoTerminacao = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaExclusivoTerminacao), //Atividade 3
					@SuinoculturaComLançamentoEfluenteLiquidos = ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaComLançamentoEfluenteLiquidos) //Atividade 4
				});
			}
		}

		public bool MostrarNumeroMatrizes { get { return Caracterizacao.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaExclusivoProducaoLeitoes); } }
		public bool MostrarNumeroCabecas { get { return !MostrarNumeroMatrizes; } }

		public SuinoculturaVM(Suinocultura caracterizacao, List<ProcessoAtividadeItem> atividades, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, List<Lista> fases, bool isVisualizar = false, bool isEditar = false)
		{

			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
			IsEditar = isEditar;

			atividades = atividades.Where(x => x.Id == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaCicloCompleto) || 
										x.Id == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaExclusivoProducaoLeitoes) || 
										x.Id == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaExclusivoTerminacao) || 
										x.Id == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.SuinoculturaComLançamentoEfluenteLiquidos)).ToList();

			Atividade = ViewModelHelper.CriarSelectList(atividades, true, true, selecionado: caracterizacao.Atividade.ToString());
			Fases = ViewModelHelper.CriarSelectList(fases, true, true, selecionado: caracterizacao.Fase.ToString());

			CoodernadaAtividade = new CoordenadaAtividadeVM(Caracterizacao.CoordenadaAtividade, coordenadaslst, tipoGeometrico, IsVisualizar);
		}
		
	}
}