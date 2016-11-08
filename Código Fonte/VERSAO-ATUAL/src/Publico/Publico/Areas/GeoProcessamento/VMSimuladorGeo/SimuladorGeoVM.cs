using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;
using Tecnomapas.EtramiteX.Publico.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMSimuladorGeo
{
	public class SimuladorGeoVM
	{
		private SimuladorGeo _simuladorGeo = new SimuladorGeo();
		public SimuladorGeo SimuladorGeo
		{
			get { return _simuladorGeo; }
			set { _simuladorGeo = value; }
		}

		public int Id 
		{ 
			get { return _simuladorGeo.Id; } 
			set { _simuladorGeo.Id = value; } 
		}

		public int MecanismoElaboracaoId 
		{ 
			get { return _simuladorGeo.MecanismoElaboracaoId; } 
			set { _simuladorGeo.MecanismoElaboracaoId = value; } 
		}

		public int SituacaoId 
		{ 
			get { return _simuladorGeo.SituacaoId; } 
			set { _simuladorGeo.SituacaoId = value; } 
		}

		public String Easting 
		{ 
			get { return _simuladorGeo.Easting; } 
			set { _simuladorGeo.Easting = value; } 
		}

		public String Northing 
		{ 
			get { return _simuladorGeo.Northing; } 
			set { _simuladorGeo.Northing = value; } 
		}

		private List<ArquivoItemGridVM> _arquivosVetoriais = new List<ArquivoItemGridVM>();
		public List<ArquivoItemGridVM> ArquivosVetoriais
		{
			get { return _arquivosVetoriais; }
			set { _arquivosVetoriais = value; }
		}

		SimuladorGeoArquivo _arquivoEnviado = new SimuladorGeoArquivo();
		public SimuladorGeoArquivo ArquivoEnviado 
		{
			get { return _arquivoEnviado; }
			set { _arquivoEnviado = value; } 
		}

		private List<ArquivoItemGridVM> _arquivosProcessados = new List<ArquivoItemGridVM>();
		public List<ArquivoItemGridVM> ArquivosProcessados
		{
			get { return _arquivosProcessados; }
			set { _arquivosProcessados = value; }
		}

		#region Coordenada

		private List<SelectListItem> _sistemasCoordenada = new List<SelectListItem>();
		public List<SelectListItem> SistemaCoordenada
		{
			get { return _sistemasCoordenada; }
			set { _sistemasCoordenada = value; }
		}

		private List<SelectListItem> _datuns = new List<SelectListItem>();
		public List<SelectListItem> Datuns
		{
			get { return _datuns; }
			set { _datuns = value; }
		}

		private List<SelectListItem> _fusos = new List<SelectListItem>();
		public List<SelectListItem> Fusos
		{
			get { return _fusos; }
			set { _fusos = value; }
		}

		#endregion

		#region Mensagem Json

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					NivelPrecisaoObrigatorio = Mensagem.ProjetoGeografico.NivelPrecisaoObrigatorio,
					AreaAbrangenciaObrigatorio = Mensagem.ProjetoGeografico.AreaDeAbrangenciaObrigatorio,
					MecanismoObrigatorio = Mensagem.ProjetoGeografico.MecanismoObrigatorio,
					ConfirmarExcluir = Mensagem.ProjetoGeografico.ConfirmarExcluir,
					ConfirmarAreaAbrangencia = Mensagem.ProjetoGeografico.ConfirmarAreaAbrangencia,
					ConfirmacaoRecarregar = Mensagem.ProjetoGeografico.ConfirmacaoRecarregar(),
					ConfirmacaoRefazer = Mensagem.ProjetoGeografico.ConfirmacaoRefazer(),
					ConfirmacaoReenviar = Mensagem.ProjetoGeografico.ConfirmacaoReenviar,
					EmpreendimentoForaAbrangencia = Mensagem.ProjetoGeografico.EmpreendimentoForaAbrangencia
				});
			}
		}

		public String MensagensImportador
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ArquivoAnexoNaoEhZip = Mensagem.SimuladorGeo.ArquivoAnexoNaoEhZip,
					EastingObrigatorio = Mensagem.SimuladorGeo.EastingObrigatorio,
					NorthingObrigatorio = Mensagem.SimuladorGeo.NorthingObrigatorio
				});
			}
		}

		#endregion

		public String SituacoesValidasJson
		{
			get { return ViewModelHelper.Json(new List<int>() { 1, 2, 6, 7, 11, 12 }); }
		}

		public bool MostrarReenviar { get; set; }
		public bool MostrarReprocessar { get; set; }
		public bool MostrarCancelar { get; set; }

		public SimuladorGeoVM()
		{
			ArquivosVetoriais.Add(new ArquivoItemGridVM() { Texto = "Dados GEOBASES", SituacaoTexto = "Aguardando solicitação ", Tipo = (int)eSimuladorGeoArquivoTipo.DadosGEOBASES, MostrarGerar = true });
		}
	}
}