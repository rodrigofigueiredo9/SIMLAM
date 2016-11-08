using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class EspecificidadeController : DefaultController
	{
		IEspecificidadeBus _bus = null;
		TituloModeloInternoBus _busModelo = new TituloModeloInternoBus();

		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar })]
		public ActionResult Salvar(int modeloId)
		{
			TituloModelo modelo = _busModelo.ObterSimplificado(modeloId);

			if (modelo == null || !modelo.Codigo.HasValue || !EspecificiadadeBusFactory.Possui(modelo.Codigo.Value))
			{
				return Json(new { Possui = false }, JsonRequestBehavior.AllowGet);
			}

			_bus = EspecificiadadeBusFactory.Criar(modelo.Codigo.Value);

			eEspecificidade eTelaEsp = (eEspecificidade)modelo.Codigo.Value;
			string url = Url.Action(eTelaEsp.ToString(), _bus.Tipo.ToString(), new { Area = "Especificidades" });

			return Json(new { Possui = true, Url = url, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}
	}
}