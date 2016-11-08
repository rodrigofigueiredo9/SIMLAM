using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Especificidades.ViewModels.Certidao
{
	public class CertidaoDispensaLicenciamentoAmbientalVM
	{
		public bool IsVisualizar { get; set; }

		private CertidaoDispensaLicenciamentoAmbiental _certidao = new CertidaoDispensaLicenciamentoAmbiental();
		public CertidaoDispensaLicenciamentoAmbiental Certidao
		{
			get { return _certidao; }
			set { _certidao = value; }
		}

		private List<SelectListItem> _atividade = new List<SelectListItem>();
		public List<SelectListItem> Atividade
		{
			get { return _atividade; }
			set { _atividade = value; }
		}

		private List<SelectListItem> _vinculoPropriedade = new List<SelectListItem>();
		public List<SelectListItem> VinculoPropriedade
		{
			get { return _vinculoPropriedade; }
			set { _vinculoPropriedade = value; }
		}

		public CertidaoDispensaLicenciamentoAmbientalVM() { }

		public CertidaoDispensaLicenciamentoAmbientalVM(CertidaoDispensaLicenciamentoAmbiental certidao, List<AtividadeSolicitada> atividades, List<Lista> vincPropriedade, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Certidao = certidao;
			Atividade = ViewModelHelper.CriarSelectList(atividades, true, true, certidao.Atividade.ToString());
			VinculoPropriedade = ViewModelHelper.CriarSelectList(vincPropriedade, true, true, certidao.VinculoPropriedade.ToString());
		}
	}
}