using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Credenciado.Areas.Especificidades.ViewModels.Outros;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class OutrosController : DefaultController
	{
		#region Propriedades

		TituloInternoBus _busTitulo = new TituloInternoBus();
		AtividadeCredenciadoBus _busAtividade = new AtividadeCredenciadoBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloDeclaratorioCriar, ePermissao.TituloDeclaratorioEditar, ePermissao.TituloDeclaratorioVisualizar })]
		public ActionResult OutrosInformacaoCorte(EspecificidadeVME especificidade)
		{
			var _busOutros = new OutrosInformacaoCorteBus();
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			var titulo = new Titulo();
			var outros = new OutrosInformacaoCorte();

			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				outros = _busOutros.Obter(especificidade.TituloId) as OutrosInformacaoCorte;

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
			OutrosInformacaoCorteVM vm = new OutrosInformacaoCorteVM(outros, lstAtividades, ListaCredenciadoBus.ObterVinculoPropriedade, especificidade.IsVisualizar);
			try
			{
				htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Outros/OutrosInformacaoCorte.ascx", vm);

			}catch(Exception e)
			{
				var ex = e;
			}
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		#endregion
	}
}