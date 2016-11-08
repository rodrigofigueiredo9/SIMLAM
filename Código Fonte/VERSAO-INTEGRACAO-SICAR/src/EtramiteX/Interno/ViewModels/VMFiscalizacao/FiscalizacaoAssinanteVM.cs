using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class FiscalizacaoAssinanteVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsCnfAssinante { get; set; }

		private List<FiscalizacaoAssinante> _assinantes = new List<FiscalizacaoAssinante>();
		public List<FiscalizacaoAssinante> Assinantes
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

		public FiscalizacaoAssinanteVM() 
		{ 
			Cargos = ViewModelHelper.CriarSelectList(new List<Cargo>());
			Funcionarios = ViewModelHelper.CriarSelectList(new List<FuncionarioLst>());
		}
	}
}