using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;

using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMDUA;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class DUAController : DefaultController
	{
		#region Propriedades

		DuaBus _bus = new DuaBus();

		public static EtramitePrincipal Usuario
		{
			get { return (System.Web.HttpContext.Current.User as EtramitePrincipal); }
		}
		#endregion

		[Permite(RoleArray = new Object[] {  })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.Municipios(ViewModelHelper.EstadoDefaultId()));

			vm.Situacoes = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.CARSolicitacaoSituacoes, true, true);
            vm.SituacoesSicar = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.SicarSituacoes, true, true);

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult EmitirDua(int id)
		{
			DUAVM vm = new DUAVM();
			vm.Titulo.Id = id;

			//CARSolicitacao solicitacao = new CARSolicitacao();
			//List<PessoaLst> declarantes = new List<PessoaLst>();
			//List<Lista> atividades = new List<Lista>();

			//solicitacao.DataEmissao.DataTexto = DateTime.Now.ToShortDateString();

			//CARSolicitacaoVM vm = new CARSolicitacaoVM(solicitacao, ListaCredenciadoBus.CARSolicitacaoSituacoes, atividades, declarantes);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult Criar(CARSolicitacao car)
		{
			

			string urlRetorno = Url.Action("Index", "CARSolicitacao") + "?Msg=" + Validacao.QueryParam();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

	}
}