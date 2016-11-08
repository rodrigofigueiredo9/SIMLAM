using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMSetor
{
	public class SalvarVM
	{
		private SetorLocalizacao _setor = new SetorLocalizacao();
		public SetorLocalizacao Setor
		{
			get { return _setor; }
			set { _setor = value; }
		}

		private List<SelectListItem> _agrupador = new List<SelectListItem>();
		public List<SelectListItem> Agrupador
		{
			get { return _agrupador; }
			set { _agrupador = value; }
		}

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private List<SelectListItem> _estados = new List<SelectListItem>();
		public List<SelectListItem> Estados
		{
			get { return _estados; }
			set { _estados = value; }
		}

		private List<SelectListItem> _municipios = new List<SelectListItem>();
		public List<SelectListItem> Municipios
		{
			get { return _municipios; }
			set { _municipios = value; }
		}

		public SalvarVM(SetorLocalizacao setor, List<SetorAgrupador> agrupador, List<Setor> setores, List<Estado> estados, List<Municipio> municipios )
		{
			Setor = setor;
			Agrupador = ViewModelHelper.CriarSelectList(agrupador, true, true, null, Setor.AgrupadorTexto);
			Setores = ViewModelHelper.CriarSelectList(setores, true, true, null, Setor.Nome);
			Estados = ViewModelHelper.CriarSelectList(estados, true, true, null, Setor.Endereco.EstadoTexto);
			Municipios = ViewModelHelper.CriarSelectList(municipios, true, true, null, Setor.Endereco.MunicipioTexto);
		}

		public SalvarVM(){}


		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@LogradouroObrigatorio = Mensagem.Setor.LogradouroObrigatorio
				});
			}
		}

		public String UrlAcao { get; set; }
		public bool ExibirBotoes { get; set; }
		public bool ExibirMensagensPartial { get; set; }
		public bool ExibirLimparPessoa { get; set; }

	}
}