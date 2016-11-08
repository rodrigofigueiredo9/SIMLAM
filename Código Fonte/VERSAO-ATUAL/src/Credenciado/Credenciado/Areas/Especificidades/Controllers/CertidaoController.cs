using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Especificidades.ViewModels.Certidao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloCertidao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class CertidaoController : DefaultController
	{
		#region Propriedades

		TituloInternoBus _busTitulo = new TituloInternoBus();
		AtividadeCredenciadoBus _busAtividade = new AtividadeCredenciadoBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar, ePermissao.TituloDeclaratorioVisualizar })]
		public ActionResult CertidaoDispensaLicenciamentoAmbiental(EspecificidadeVME especificidade)
		{
			var _busCertidao = new CertidaoDispensaLicenciamentoAmbientalBus();
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			var titulo = new Titulo();
			var certidao = new CertidaoDispensaLicenciamentoAmbiental();

			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				certidao = _busCertidao.Obter(especificidade.TituloId) as CertidaoDispensaLicenciamentoAmbiental;

				if (titulo.CredenciadoId > 0)
				{
					lstAtividades = _busAtividade.ObterAtividadesListaReq(titulo.RequerimetoId.GetValueOrDefault());
				}
				else
				{
					AtividadeInternoBus atividadeInternoBus = new AtividadeInternoBus();
					lstAtividades = atividadeInternoBus.ObterAtividadesListaReq(titulo.RequerimetoId.GetValueOrDefault());
				}
			}

			CertidaoDispensaLicenciamentoAmbientalVM vm = new CertidaoDispensaLicenciamentoAmbientalVM(certidao, lstAtividades, ListaCredenciadoBus.ObterVinculoPropriedade, especificidade.IsVisualizar);
			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certidao/CertidaoDispensaLicenciamentoAmbiental.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		#endregion
	}
}