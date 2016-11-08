using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Interno.ViewModels
{
	public class FinalidadeVM
	{
		public int AtividadeId { get; set; }
		public String SiglaOrgao { get; set; }
		public bool TituloAnteriorObrigatorio { get; set; }

		private List<SelectListItem> _finalidades = new List<SelectListItem>();
		public List<SelectListItem> Finalidades
		{
			get { return _finalidades; }
			set { _finalidades = value; }
		}

		private List<SelectListItem> _titulos = new List<SelectListItem>();
		public List<SelectListItem> Titulos
		{
			get { return _titulos; }
			set { _titulos = value; }
		}

		private List<SelectListItem> _titulosAnterior = new List<SelectListItem>();
		public List<SelectListItem> TitulosAnterior
		{
			get { return _titulosAnterior; }
			set { _titulosAnterior = value; }
		}

		private List<SelectListItem> _numerosDocumentoAnterior = new List<SelectListItem>();
		public List<SelectListItem> NumerosDocumentoAnterior
		{
			get { return _numerosDocumentoAnterior; }
			set { _numerosDocumentoAnterior = value; }
		}

		public FinalidadeVM() { }

		public void SetLista(List<Finalidade> finalidades)
		{
			Finalidades = ViewModelHelper.CriarSelectList(finalidades, true, true);
			Titulos = ViewModelHelper.CriarSelectList(new List<TituloModeloLst>(), false, true);
			TitulosAnterior = ViewModelHelper.CriarSelectList(new List<TituloModeloLst>(), false, true);
		}
	}
}