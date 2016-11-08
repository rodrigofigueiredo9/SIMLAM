using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class ComplementacaoDadosVM
	{
		public Boolean IsVisualizar { get; set; }

		private ComplementacaoDados _entidade = new ComplementacaoDados();
		public ComplementacaoDados Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}

		private List<SelectListItem> _residePropriedadeTipoLst = new List<SelectListItem>();
		public List<SelectListItem> ResidePropriedadeTipoLst
		{
			get { return _residePropriedadeTipoLst; }
			set { _residePropriedadeTipoLst = value; }
		}

		private List<SelectListItem> _rendaMensalTipoLst = new List<SelectListItem>();
		public List<SelectListItem> RendaMensalTipoLst
		{
			get { return _rendaMensalTipoLst; }
			set { _rendaMensalTipoLst = value; }
		}

		private List<SelectListItem> _nivelEscolaridadeTipoLst = new List<SelectListItem>();
		public List<SelectListItem> NivelEscolaridadeTipoLst
		{
			get { return _nivelEscolaridadeTipoLst; }
			set { _nivelEscolaridadeTipoLst = value; }
		}

		private List<SelectListItem> _vinculoPropriedadeTipoLst = new List<SelectListItem>();
		public List<SelectListItem> VinculoPropriedadeTipoLst
		{
			get { return _vinculoPropriedadeTipoLst; }
			set { _vinculoPropriedadeTipoLst = value; }
		}

		private List<SelectListItem> _conhecimentoLegislacaoTipoLst = new List<SelectListItem>();
		public List<SelectListItem> ConhecimentoLegislacaoTipoLst
		{
			get { return _conhecimentoLegislacaoTipoLst; }
			set { _conhecimentoLegislacaoTipoLst = value; }
		}

		private List<ReservaLegalLst> _reservaLegalTipoLst = new List<ReservaLegalLst>();
		public List<ReservaLegalLst> ReservaLegalTipoLst
		{
			get { return _reservaLegalTipoLst; }
			set { _reservaLegalTipoLst = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Salvar = Mensagem.ComplementacaoDados.Salvar
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					 @RespostasDefaultSim = eRespostasDefault.Sim,
					 @RespostasDefaultNao = eRespostasDefault.Nao,
					 @RespostasDefaultNaoSeAplica = eRespostasDefault.NaoSeAplica,
					 @VinculoPropriedadeOutro = eVinculoPropriedade.Outro,
					 @TipoAutuadoPessoa = eTipoAutuado.Pessoa,
					 @TipoAutuadoEmpreendimento = eTipoAutuado.Empreendimento
				});
			}
		}

		public ComplementacaoDadosVM(){}

		public ComplementacaoDadosVM(ComplementacaoDados entidade, List<Lista> residePropriedadeTipoLst, List<Lista> rendaMensalTipoLst, List<Lista> nivelEscolaridadeTipoLst, List<TipoResponsavel> vinculoPropriedadeTipoLst, List<Lista> conhecimentoLegislacaoTipoLst, List<ReservaLegalLst> reservaLegalTipoLst, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Entidade = entidade;

			ResidePropriedadeTipoLst = ViewModelHelper.CriarSelectList(residePropriedadeTipoLst, true, true, selecionado: entidade.ResidePropriedadeTipo.ToString());
			RendaMensalTipoLst = ViewModelHelper.CriarSelectList(rendaMensalTipoLst, true, true, selecionado: entidade.RendaMensalFamiliarTipo.ToString());
			NivelEscolaridadeTipoLst = ViewModelHelper.CriarSelectList(nivelEscolaridadeTipoLst, true, true, selecionado: entidade.NivelEscolaridadeTipo.ToString());
			VinculoPropriedadeTipoLst = ViewModelHelper.CriarSelectList(vinculoPropriedadeTipoLst, true, true, selecionado: entidade.VinculoComPropriedadeTipo.ToString());
			ConhecimentoLegislacaoTipoLst = ViewModelHelper.CriarSelectList(conhecimentoLegislacaoTipoLst, true, true, selecionado: entidade.ConhecimentoLegislacaoTipo.ToString());
			ReservaLegalTipoLst = reservaLegalTipoLst;
		}
	}
}