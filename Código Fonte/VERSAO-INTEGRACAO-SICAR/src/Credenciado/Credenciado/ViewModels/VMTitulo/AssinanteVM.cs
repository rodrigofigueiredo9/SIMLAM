﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMTitulo
{
	public class AssinantesVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsCnfAssinante { get; set; }

		private List<TituloAssinante> _assinantes = new List<TituloAssinante>();
		public List<TituloAssinante> Assinantes
		{
			get { return _assinantes; }
			set { _assinantes = value; }
		}

		private List<List<SelectListItem>> _assinantesCargos = new List<List<SelectListItem>>();
		public List<List<SelectListItem>> AssinantesCargos { get { return _assinantesCargos; } set { _assinantesCargos = value; } }

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private List<SelectListItem> _cargos = new List<SelectListItem>();
		public List<SelectListItem> Cargos
		{
			get { return _cargos; }
			set { _cargos = value; }
		}

		private List<SelectListItem> _funcionarios = new List<SelectListItem>();
		public List<SelectListItem> Funcionarios
		{
			get { return _funcionarios; }
			set { _funcionarios = value; }
		}

		public AssinantesVM() { }

		internal void MergeAssinantesCargos(List<TituloAssinante> assinantesTitulo)
		{
			//Seleciona os assinates
			foreach (TituloAssinante assinante in this.Assinantes)
			{
				// tenta encontrar funcionário do título dentro da lista de funcionários
				TituloAssinante assinanteEncontrado = assinantesTitulo.FirstOrDefault(x => x.FuncionarioId == assinante.FuncionarioId);
				if (assinanteEncontrado != null)
				{
					assinante.Selecionado = true;
					assinante.Id = assinanteEncontrado.Id;
					assinante.FuncionarioCargoId = assinanteEncontrado.FuncionarioCargoId;
					assinante.FuncionarioCargoNome = assinanteEncontrado.FuncionarioCargoNome;
				}
			}

			foreach (TituloAssinante assinanteTitulo in assinantesTitulo)
			{
				if (!this.Assinantes.Exists(x => x.FuncionarioId == assinanteTitulo.FuncionarioId))
				{
					assinanteTitulo.Selecionado = true;
					this.Assinantes.Add(assinanteTitulo);
				}
			}
		}
	}
}