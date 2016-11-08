using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class TramitacoesVM
	{
		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		private List<SelectListItem> _funcionarios = new List<SelectListItem>();
		public List<SelectListItem> Funcionarios
		{
			get { return _funcionarios; }
			set { _funcionarios = value; }
		}

		 
		private List<FuncionarioLst> _funcionarioslst = new List<FuncionarioLst>();

		public List<FuncionarioLst> FuncionariosLst
		{
			get { return _funcionarioslst; }
			set { _funcionarioslst = value; }
		}

		private Tramitacao _tramitacao = new Tramitacao();
		public Tramitacao Tramitacao
		{
			get { return _tramitacao; }
			set { _tramitacao = value; }
		}

		public int SetorId { get; set; }
		public int FuncionarioId { get; set; }
		public int MostrarSetor { get; set; }
		public bool MostrarFuncionario { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					Cancelar = Mensagem.Tramitacao.Cancelar
				});
			}
		}

		public TramitacoesVM() { }

		public TramitacoesVM(List<Setor> setores)
		{
			MostrarSetor = setores.Count;
			Setores = ViewModelHelper.CriarSelectList(setores);
		}

		public void CarregarListas(List<FuncionarioLst> funcionarios = null, List<Setor> setores = null)
		{
			if (setores != null)
			{
				MostrarSetor = setores.Count;
				Setores = ViewModelHelper.CriarSelectList(setores, isFiltrarAtivo: true, textoPadrao: TextoDefaultDropDow.Todos);
			}

			if (funcionarios != null)
			{
				MostrarFuncionario = funcionarios.Count > 1;
				FuncionariosLst = funcionarios;
				Funcionarios = ViewModelHelper.CriarSelectList(funcionarios, isFiltrarAtivo: true, textoPadrao: TextoDefaultDropDow.Todos);
			}
		}
	}
}