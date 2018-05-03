using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class LocalInfracaoVM
	{
		public Int32 EstadoDefault { get; set; }
		public String EstadoDefaultSigla { get; set; }
		public Int32 MunicipioDefault { get; set; }

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private List<SelectListItem> _coordenadasSistema = new List<SelectListItem>();
		public List<SelectListItem> CoordenadasSistema
		{
			get { return _coordenadasSistema; }
			set { _coordenadasSistema = value; }
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

		private List<SelectListItem> _hemisferios = new List<SelectListItem>();
		public List<SelectListItem> Hemisferios
		{
			get { return _hemisferios; }
			set { _hemisferios = value; }
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

		private List<SelectListItem> _responsavel = new List<SelectListItem>();
		public List<SelectListItem> Responsavel
		{
			get { return _responsavel; }
			set { _responsavel = value; }
		}

		private List<SelectListItem> _assinante = new List<SelectListItem>();
		public List<SelectListItem> Assinante
		{
			get { return _assinante; }
			set { _assinante = value; }
		}

		public LocalInfracao LocalInfracao { get; set; }
		public Pessoa Pessoa { get; set; }

		public bool IsVisualizar { get; set; }

		public LocalInfracaoVM()
		{
			EstadoDefault = ViewModelHelper.EstadoDefaultId();
			MunicipioDefault = ViewModelHelper.MunicipioDefaultId();
			LocalInfracao = new LocalInfracao();
			Pessoa = new Pessoa();
		}

		public LocalInfracaoVM(LocalInfracao localInfracao, List<Estado> lstEstados, List<Municipio> lstMunicipios, List<Segmento> lstSegmentos, List<CoordenadaTipo> lstTiposCoordenada, List<Datum> lstDatuns, List<Fuso> lstFusos, List<CoordenadaHemisferio> lstHemisferios, List<Setor> lstSetores, Pessoa pessoa, List<PessoaLst> lstResponsaveis, List<PessoaLst> lstAssinantes)
		{
			LocalInfracao = localInfracao;

			EstadoDefault = ViewModelHelper.EstadoDefaultId();
			EstadoDefaultSigla = ViewModelHelper.EstadoDefaultSigla();
			MunicipioDefault = ViewModelHelper.MunicipioDefaultId();

			Estados = ViewModelHelper.CriarSelectList(lstEstados, true, selecionado: EstadoDefault.ToString());
			Municipios = ViewModelHelper.CriarSelectList(lstMunicipios, true, selecionado: MunicipioDefault.ToString());

			CoordenadasSistema = ViewModelHelper.CriarSelectList(lstTiposCoordenada.Where(x => x.Id == 3).ToList(), true, false);
			Datuns = ViewModelHelper.CriarSelectList(lstDatuns.Where(x => x.Id == 1).ToList(), true, false);
			Fusos = ViewModelHelper.CriarSelectList(lstFusos.Where(x => x.Id == 24).ToList(), true, false);
			Hemisferios = ViewModelHelper.CriarSelectList(lstHemisferios.Where(x => x.Id == 1).ToList(), true, false);
			
			if (lstSetores.Count == 1)
			{
				Setores = ViewModelHelper.CriarSelectList(lstSetores, true, false, lstSetores[0].Id.ToString());
			}
			else if(lstSetores.Count > 0)
			{
				Setores = ViewModelHelper.CriarSelectList(lstSetores);
			}

			Pessoa = pessoa;
			if (this.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0)
			{
				Responsavel = lstResponsaveis.Count == 1 ? ViewModelHelper.CriarSelectList(lstResponsaveis, true, false) : ViewModelHelper.CriarSelectList(lstResponsaveis);
				Assinante = lstResponsaveis.Count == 1 ? ViewModelHelper.CriarSelectList(lstResponsaveis.FindAll(x => !string.IsNullOrWhiteSpace(x.CPFCNPJ)), true, false) : ViewModelHelper.CriarSelectList(lstResponsaveis.FindAll(x => !string.IsNullOrWhiteSpace(x.CPFCNPJ)));
			}
			if (lstAssinantes?.Count > 0)
				Assinante = ViewModelHelper.CriarSelectList(lstAssinantes, true, false);
		}
	}
}
