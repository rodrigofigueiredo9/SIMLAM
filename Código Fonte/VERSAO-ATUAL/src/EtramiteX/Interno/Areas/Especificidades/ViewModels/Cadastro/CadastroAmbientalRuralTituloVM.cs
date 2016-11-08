using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCadastro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Cadastro
{
	public class CadastroAmbientalRuralTituloVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsCondicionantes { get; set; }

		private CadastroAmbientalRuralTitulo _entidade = new CadastroAmbientalRuralTitulo();
		public CadastroAmbientalRuralTitulo Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}

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

		public CadastroAmbientalRuralTituloVM(CadastroAmbientalRuralTitulo cadastroRuralAmbiental, List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, List<TituloCondicionante> condicionantes, 
			string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Entidade = cadastroRuralAmbiental;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, Entidade.Destinatario.ToString());

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
		}
	}
}