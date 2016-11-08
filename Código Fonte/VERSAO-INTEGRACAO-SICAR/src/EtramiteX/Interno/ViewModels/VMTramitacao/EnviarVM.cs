using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class EnviarVM
	{
		public List<Tramitacao> Tramitacoes
		{
			get { return _tramitacoes; }
			set { _tramitacoes = value; }
		}
		private List<Tramitacao> _tramitacoes = new List<Tramitacao>();

		public List<Tramitacao> TramitacoesEnviadas
		{
			get { return _tramitacoesEnviadas; }
			set { _tramitacoesEnviadas = value; }
		}
		private List<Tramitacao> _tramitacoesEnviadas = new List<Tramitacao>();

		public String NumeroProtocolo { get; set; }
		public int? OpcaoBusca
		{
			get { return _opcaoBusca; }
			set { _opcaoBusca = value; }
		}

		private int? _opcaoBusca = null;

		#region Listas

		public List<SelectListItem> Objetivos
		{
			get { return _objetivos; }
			set { _objetivos = value; }
		}
		private List<SelectListItem> _objetivos = new List<SelectListItem>();

		public List<SelectListItem> RemetenteFuncionarios
		{
			get { return _remetenteFuncionarios; }
			set { _remetenteFuncionarios = value; }
		}
		private List<SelectListItem> _remetenteFuncionarios = new List<SelectListItem>();

		public List<SelectListItem> DestinatarioFuncionarios
		{
			get { return _destinatarioFuncionarios; }
			set { _destinatarioFuncionarios = value; }
		}
		private List<SelectListItem> _destinatarioFuncionarios = new List<SelectListItem>();

		public List<SelectListItem> SetoresRemente
		{
			get { return _setoresRemente; }
			set { _setoresRemente = value; }
		}
		private List<SelectListItem> _setoresRemente = new List<SelectListItem>();

		public List<SelectListItem> SetoresDestinatario
		{
			get { return _setoresDestinatario; }
			set { _setoresDestinatario = value; }
		}
		private List<SelectListItem> _setoresDestinatario = new List<SelectListItem>();

		public List<SelectListItem> TiposProcesso
		{
			get { return _tiposProcesso; }
			set { _tiposProcesso = value; }
		}
		private List<SelectListItem> _tiposProcesso = new List<SelectListItem>();

		public List<SelectListItem> TiposDocumento
		{
			get { return _tiposDocumento; }
			set { _tiposDocumento = value; }
		}
		private List<SelectListItem> _tiposDocumento = new List<SelectListItem>();

		public List<SelectListItem> OrgaosExterno
		{
			get { return _orgaosExterno; }
			set { _orgaosExterno = value; }
		}
		private List<SelectListItem> _orgaosExterno = new List<SelectListItem>();

		#endregion

		private Enviar _enviar = new Enviar();
		public Enviar Enviar
		{
			get { return _enviar; }
			set { _enviar = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ProtocoloJaAdicionado = Mensagem.Tramitacao.ProtocoloJaAdicionado
				});
			}
		}

		public EnviarVM() { }

		public EnviarVM(Funcionario executor, Funcionario remetente, List<Motivo> objetivos, List<ProtocoloTipo> tiposProcesso, List<ProtocoloTipo> tiposDocumento,
						List<Setor> setoresRemetente, List<Setor> setoresDestinario, List<OrgaoClasse> orgaosExterno = null)
		{
			Enviar.Executor = executor;
			Enviar.Remetente = remetente;

			Objetivos = ViewModelHelper.CriarSelectList(objetivos, true);
			TiposProcesso = ViewModelHelper.CriarSelectList(tiposProcesso, true);
			TiposDocumento = ViewModelHelper.CriarSelectList(tiposDocumento, true);
			CarregarSetoresRemetente(setoresRemetente);
			CarregarSetoresDestinario(setoresDestinario);
			DestinatarioFuncionarios = new List<SelectListItem>(){ ViewModelHelper.SelecionePadrao };

			if (orgaosExterno != null)
			{
				OrgaosExterno = ViewModelHelper.CriarSelectList(orgaosExterno, true);
			}

			Enviar.DataEnvio = new DateTecno() { Data = DateTime.Now };
		}

		public void CarregarSetoresRemetente(List<Setor> setoresOrigem)
		{
			SetoresRemente = ViewModelHelper.CriarSelectList(setoresOrigem, true);
		}

		public void CarregarSetoresDestinario(List<Setor> setoresDestinario)
		{
			SetoresDestinatario = ViewModelHelper.CriarSelectList(setoresDestinario, true);
		}

		public void CarregarRemetentesFuncionarios(List<FuncionarioLst> setoresDestinario)
		{
			RemetenteFuncionarios = ViewModelHelper.CriarSelectList(setoresDestinario, true);
		}
	}
}