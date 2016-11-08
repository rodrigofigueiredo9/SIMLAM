using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca
{
	public class LicencaOperacaoFomentoVM
	{
		public bool IsVisualizar { set; get; }
		public bool IsCondicionantes { get; set; }

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

		private LicencaOperacaoFomento _licenca = new LicencaOperacaoFomento();
		public LicencaOperacaoFomento Licenca
		{
			get { return _licenca; }
			set { _licenca = value; }
		}

		public LicencaOperacaoFomentoVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, LicencaOperacaoFomento licenca, List<TituloCondicionante> condicionantes, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();

			Licenca = licenca;
		}

		public LicencaOperacaoFomentoVM() { }
	}
}