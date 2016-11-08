using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca
{
	public class LicencaAmbientalUnicaVM
	{
		public bool IsVisualizar { set; get; }
		public bool IsCondicionantes { get; set; }

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private TituloCondicionanteVM _condicionantes = new TituloCondicionanteVM();
		public TituloCondicionanteVM Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}

		private LicencaAmbientalUnica _licenca = new LicencaAmbientalUnica();
		public LicencaAmbientalUnica Licenca
		{
			get { return _licenca; }
			set { _licenca = value; }
		}

		public string GetLicencaJSON { get { return ViewModelHelper.Json(this.Licenca); } }

		public LicencaAmbientalUnicaVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, LicencaAmbientalUnica licenca, List<TituloCondicionante> condicionantes, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true);

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();

			Licenca = licenca;
		}

		public LicencaAmbientalUnicaVM() { }
	}
}