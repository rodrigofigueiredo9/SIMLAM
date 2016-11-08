using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.View;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento
{
	public class SalvarVM
	{
		public List<SelectListItem> Segmentos { get; set; }
		public List<TipoResponsavel> TiposResponsavel { get; set; }
		public List<SelectListItem> TiposCoordenada { get; set; }
		public List<SelectListItem> Datuns { get; set; }
		public List<SelectListItem> Fusos { get; set; }
		public List<SelectListItem> Hemisferios { get; set; }
		public List<SelectListItem> EstadosLocalizacao { get; set; }
		public List<SelectListItem> MunicipiosLocalizacao { get; set; }
		public List<SelectListItem> EstadosCorrespondencia { get; set; }
		public List<SelectListItem> MunicipiosCorrespondencia { get; set; }
		public List<SelectListItem> LocaisColetaPonto { get; set; }
		public List<SelectListItem> FormasColetaPonto { get; set; }
		public List<SelectListItem> EnderecoCadastradoTipoLst { get; set; }
		public String DenominadoresSegmentos { get; set; }
		public Int32 EstadoDefault { get; set; }

		public String EstadoDefaultSigla { get; set; }
		public Boolean IsVisualizarHistorico { get; set; }

		private List<SelectListItem> _responsaveis = new List<SelectListItem>();
		public List<SelectListItem> Responsaveis
		{
			get { return _responsaveis; }
			set { _responsaveis = value; }
		}

		private bool _isEditar = false;
		public bool IsEditar
		{
			get { return _isEditar; }
			set { _isEditar = value; }
		}

		private bool _isVisualizar= false;
		public bool IsVisualizar
		{
			get { return _isVisualizar; }
			set { _isVisualizar = value; }
		}

		private bool _mostrarTituloTela = true;
		public bool MostrarTituloTela
		{
			get { return _mostrarTituloTela; }
			set { _mostrarTituloTela = value; }
		}
		
		private ContatoVME _contato = new ContatoVME();
		public ContatoVME Contato
		{
			get { return _contato; }
			set { _contato = value; }
		}

		public List<Sessao> EmpreendimentoInterno { get; set; }
		
		public List<Sessao> EmpreendimentoCredenciado { get; set; }

		private Empreendimento _empreendimento = new Empreendimento();
		public Empreendimento Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}

		public String Mensagens
		{
			get { return new EmpreendimentoVM().Mensagens; }
		}

		public String IdsTela 
		{
			get { return new EmpreendimentoVM().IdsTela; }
		}

		public bool IsAjaxRequest { get; set; }		

		public SalvarVM()
		{
			EmpreendimentoInterno = new List<Sessao>();
			EmpreendimentoCredenciado = new List<Sessao>();
		}

		public SalvarVM(List<Estado> lstEstados, List<Municipio> lstMunicipiosLocalizacao, List<Municipio> lstMunicipiosCorrespondencia, List<Segmento> lstSegmentos, List<CoordenadaTipo> lstTiposCoordenada,
			List<Datum> lstDatuns, List<Fuso> lstFusos, List<CoordenadaHemisferio> lstHemisferios, List<TipoResponsavel> lstTiposResponsavel, List<Lista> localColetaPonto, List<Lista> formaColetaPonto,
			int? estadoLocalizacaoId = null, int? municipioLocalizacaoId = null, int? estadoCorrespondenciaId = null, int? municipioCorrespondenciaId = null, int localColetaPontoId = 0, int formaColetaPontoId = 0)
		{
			EstadoDefault = ViewModelHelper.EstadoDefaultId();
			EstadoDefaultSigla = ViewModelHelper.EstadoDefaultSigla();
			EstadosCorrespondencia = ViewModelHelper.CriarSelectList(lstEstados, true, selecionado: estadoCorrespondenciaId.ToString());

			if (estadoLocalizacaoId != null && estadoLocalizacaoId != EstadoDefault)
			{
				lstEstados = lstEstados.Where(x => x.Id != EstadoDefault).ToList();
			}

			EstadosLocalizacao = ViewModelHelper.CriarSelectList(lstEstados, true, selecionado: estadoLocalizacaoId.ToString());
			MunicipiosLocalizacao = ViewModelHelper.CriarSelectList(lstMunicipiosLocalizacao, true, selecionado: municipioLocalizacaoId.ToString());
			MunicipiosCorrespondencia = ViewModelHelper.CriarSelectList(lstMunicipiosCorrespondencia, true, selecionado: municipioCorrespondenciaId.ToString());
			Segmentos = ViewModelHelper.CriarSelectList(lstSegmentos, true);

			LocaisColetaPonto = ViewModelHelper.CriarSelectList(localColetaPonto, true, selecionado: localColetaPontoId.ToString());
			FormasColetaPonto = ViewModelHelper.CriarSelectList(formaColetaPonto, true, selecionado: formaColetaPontoId.ToString());

			TiposCoordenada = ViewModelHelper.CriarSelectList(lstTiposCoordenada.Where(x => x.Id == 3).ToList(), true, false);//UTM
			Datuns = ViewModelHelper.CriarSelectList(lstDatuns.Where(x => x.Id == 1).ToList(), true, false);//SIRGAS2000
			Fusos = ViewModelHelper.CriarSelectList(lstFusos.Where(x => x.Id == 24).ToList(), true, false);
			Hemisferios = ViewModelHelper.CriarSelectList(lstHemisferios.Where(x => x.Id == 1).ToList(), true, false);//Sul

			TiposResponsavel = lstTiposResponsavel;


			List<Lista> enderecoCadastradoTipo = new List<Lista>();
			enderecoCadastradoTipo.Add(new Lista(){Id = "1", Texto="Interessado", IsAtivo = true});
			enderecoCadastradoTipo.Add(new Lista(){Id = "2", Texto="Representante", IsAtivo = true});
			enderecoCadastradoTipo.Add(new Lista(){Id = "3", Texto="Responsável Técnico", IsAtivo = true});
			enderecoCadastradoTipo.Add(new Lista(){Id = "4", Texto="Empreendimento", IsAtivo = true});

			EnderecoCadastradoTipoLst = ViewModelHelper.CriarSelectList(enderecoCadastradoTipo, true, true);

			SetarDenominadores(lstSegmentos);

			EmpreendimentoInterno = new List<Sessao>();
			EmpreendimentoCredenciado = new List<Sessao>();
		}

		public void SetarDenominadores(List<Segmento> lstSegmentos)
		{
			List<SelectListItem> denominadores = new List<SelectListItem>();
			lstSegmentos.ForEach(x =>
			{
				denominadores.Add(new SelectListItem() { Value = x.Id, Text = x.Denominador });
			});

			DenominadoresSegmentos = ViewModelHelper.Json(new { @Denominadores = denominadores });
		}

		public void SetCoordenada()
		{
			switch (this.Empreendimento.Coordenada.Tipo.Id)
			{
				case 1:
					this.Empreendimento.Coordenada.LatitudeGdec = null;
					this.Empreendimento.Coordenada.LongitudeGdec = null;
					this.Empreendimento.Coordenada.EastingUtm = null;
					this.Empreendimento.Coordenada.NorthingUtm = null;
					this.Empreendimento.Coordenada.FusoUtm = null;
					this.Empreendimento.Coordenada.HemisferioUtm = null;
					break;

				case 2:
					this.Empreendimento.Coordenada.LatitudeGms = null;
					this.Empreendimento.Coordenada.LongitudeGms = null;
					this.Empreendimento.Coordenada.EastingUtm = null;
					this.Empreendimento.Coordenada.NorthingUtm = null;
					this.Empreendimento.Coordenada.FusoUtm = null;
					this.Empreendimento.Coordenada.HemisferioUtm = null;
					break;

				case 3:
					this.Empreendimento.Coordenada.LatitudeGms = null;
					this.Empreendimento.Coordenada.LongitudeGms = null;
					this.Empreendimento.Coordenada.LatitudeGdec = null;
					this.Empreendimento.Coordenada.LongitudeGdec = null;
					break;
			}
		}

		public void SetLocalizarVm(ListarEmpreendimentoFiltro filtros)
		{
			this.Empreendimento.Segmento = filtros.Segmento;
			this.Empreendimento.Denominador = filtros.Denominador;

			Endereco endereco = new Endereco();
			endereco.EstadoId = filtros.EstadoId.GetValueOrDefault();
			endereco.MunicipioId = filtros.MunicipioId.GetValueOrDefault();

			this.Empreendimento.Enderecos.Add(endereco);
			this.Empreendimento.Responsaveis.Add(filtros.Responsavel);
			this.Empreendimento.Atividade = filtros.Atividade;
			this.Empreendimento.Coordenada = filtros.Coordenada;
		}
	}
}