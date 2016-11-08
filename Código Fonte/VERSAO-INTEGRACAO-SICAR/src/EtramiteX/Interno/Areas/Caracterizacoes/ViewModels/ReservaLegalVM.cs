using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class ReservaLegalVM
	{
		public Boolean IsVisualizar { get; set; }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SituacaoNaoInformadaId = eReservaLegalSituacao.NaoInformada,
					@SituacaoPropostaId = eReservaLegalSituacao.Proposta,
					@SituacaoRegistradaId = eReservaLegalSituacao.Registrada,
					@LocalizacaoCompensacaoMatriculaCedenteId = eReservaLegalLocalizacao.CompensacaoMatriculaCedente,
					@LocalizacaoCompensacaoEmpreendimentoCedenteId = eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente,
					@LocalizacaoCompensacaoMatriculaReceptoraId = eReservaLegalLocalizacao.CompensacaoMatriculaReceptora,
					@LocalizacaoCompensacaoEmpreendimentoReceptora = eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora,
					@LocalizacaoCompensacaoNestaPosse = eReservaLegalLocalizacao.NestaPosse,
					@LocalizacaoCompensacaoNestaMatricula = eReservaLegalLocalizacao.NestaMatricula

				});
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EmpreendimentoCedente = Mensagem.Dominialidade.ReservaLegalEmpreendimentoReceptorNaoPodeSerCedente,

				});
			}
		}


		private ReservaLegal _reservaLegal = new ReservaLegal();
		public ReservaLegal ReservaLegal
		{
			get { return _reservaLegal; }
			set { _reservaLegal = value; }
		}

		private List<SelectListItem> _situacoes = new List<SelectListItem>();
		public List<SelectListItem> Situacoes
		{
			get { return _situacoes; }
			set { _situacoes = value; }
		}

		public List<SelectListItem> MatriculaCompensacao { get; set; }

		public List<SelectListItem> IdentificacaoARLCompensacao { get; set; }

		private List<SelectListItem> _localizacoes = new List<SelectListItem>();
		public List<SelectListItem> Localizacoes
		{
			get { return _localizacoes; }
			set { _localizacoes = value; }
		}

		private List<SelectListItem> _cartorios = new List<SelectListItem>();
		public List<SelectListItem> Cartorios
		{
			get { return _cartorios; }
			set { _cartorios = value; }
		}

		public List<SelectListItem> TiposCoordenada { get; set; }

		public List<SelectListItem> Datuns { get; set; }


		public ReservaLegalVM(List<Lista> situacoes, List<Lista> localizacoes, List<Lista> cartorios, ReservaLegal reserva,
			bool isVisualizar = false, int dominioTipo = 0, List<CoordenadaTipo> lstTiposCoordenada = null, List<Datum> lstDatuns = null,
			List<Lista> lstMatriculaCompensacao = null, List<Lista> lstIdentificacaoARLCompensacao = null, List<Lista> booleanLista = null, List<Lista> lstSituacoesVegetal = null)
		{
			string situacaoSelecionada = "0";
			string localizacaoSelecionada = "0";
			if (!string.IsNullOrEmpty(reserva.Identificacao))
			{
				situacoes = situacoes.Where(x => x.Id != ((int)eReservaLegalSituacao.NaoInformada).ToString()).ToList();

				if (reserva.Compensada)
				{
					localizacoes = localizacoes.Where(x => (new string[] { 
					((int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente).ToString() }).Contains(x.Id)).ToList(); ;
				}
				else
				{
					if (dominioTipo == 2)//2 - Posse
					{
						localizacoes = localizacoes.Where(x => x.Id == ((int)eReservaLegalLocalizacao.NestaPosse).ToString()).ToList(); ;
					}
					else if (dominioTipo == 1)//1 - Matricula
					{
						localizacoes = localizacoes.Where(x => x.Id == ((int)eReservaLegalLocalizacao.NestaMatricula).ToString()).ToList(); ;
					}
					localizacaoSelecionada = (localizacoes.FirstOrDefault() ?? new Lista()).Id.ToString();
				}
			}
			else
			{
				situacaoSelecionada = ((int)eReservaLegalSituacao.NaoInformada).ToString();
				localizacoes = localizacoes.Where(x => (new string[] { 
					((int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora).ToString() }).Contains(x.Id)).ToList();

				lstSituacoesVegetal = lstSituacoesVegetal.Where(x =>
					(new string[] { ((int)eReservaLegalSituacaoVegetal.EmRecuperacao).ToString(), ((int)eReservaLegalSituacaoVegetal.Preservada).ToString() }).Contains(x.Id)).ToList();
			}

			if (reserva.SituacaoId > 0)
			{
				situacaoSelecionada = reserva.SituacaoId.ToString();
			}

			if (reserva.LocalizacaoId > 0)
			{
				localizacaoSelecionada = reserva.LocalizacaoId.ToString();
			}

			Situacoes = ViewModelHelper.CriarSelectList(situacoes, true, selecionado: situacaoSelecionada);
			Localizacoes = ViewModelHelper.CriarSelectList(localizacoes, true, selecionado: localizacaoSelecionada);
			Cartorios = ViewModelHelper.CriarSelectList(cartorios, true, selecionado: reserva.TipoCartorioId.ToString());
			MatriculaCompensacao = ViewModelHelper.CriarSelectList(lstMatriculaCompensacao, itemTextoPadrao: true, selecionado: reserva.MatriculaId.ToString());
			IdentificacaoARLCompensacao = ViewModelHelper.CriarSelectList(lstIdentificacaoARLCompensacao, true, true, reserva.IdentificacaoARLCedente.ToString());

			ReservaLegal = reserva;
			IsVisualizar = isVisualizar;
			TiposCoordenada = ViewModelHelper.CriarSelectList(lstTiposCoordenada.Where(x => x.Id == 3).ToList(), true, false);//UTM
			Datuns = ViewModelHelper.CriarSelectList(lstDatuns.Where(x => x.Id == 1).ToList(), true, false);//SIRGAS2000
			SituacoesVegetal = ViewModelHelper.CriarSelectList(lstSituacoesVegetal, itemTextoPadrao: false, selecionado: reserva.SituacaoVegetalId.GetValueOrDefault().ToString());

			int selecionado = -1;
			if (reserva.CedentePossuiEmpreendimento.HasValue)
			{
				selecionado = Convert.ToInt32(reserva.CedentePossuiEmpreendimento.Value);
			}
			ListaBooleana = ViewModelHelper.CriarSelectList(booleanLista, itemTextoPadrao: false, selecionado: selecionado.ToString());

		}

		public List<SelectListItem> ListaBooleana { get; set; }

		public List<SelectListItem> SituacoesVegetal { get; set; }

		public string ReservaCedenteJson
		{
			get
			{
				return ViewModelHelper.Json(
					new
					{
						@SituacaoVegetalTexto = ReservaLegal.SituacaoVegetalTexto,
						@SituacaoVegetalId = ReservaLegal.SituacaoVegetalId,
						@Area = ReservaLegal.ARLCedida
					});

			}
		}
	}
}