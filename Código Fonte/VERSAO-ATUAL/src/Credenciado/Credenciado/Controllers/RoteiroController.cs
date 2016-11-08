using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRoteiro.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRoteiro;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class RoteiroController : DefaultController
	{
		RoteiroInternoBus _bus = new RoteiroInternoBus();
		RoteiroInternoValidar _validar = new RoteiroInternoValidar();

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.ProtocoloVisualizar })]
		public ActionResult Visualizar(int id, string tid = null)
		{
			SalvarVM vm = new SalvarVM(ListaCredenciadoBus.Setores, _bus.Obter(id, tid), ListaCredenciadoBus.Finalidades);

			if (vm.Roteiro == null || vm.Roteiro.Id <= 0)
			{
				Validacao.Add(Mensagem.Roteiro.NaoEncontrado);
			}
			else
			{
				vm.Roteiro.Padrao = ListaCredenciadoBus.RoteiroPadrao.Count > 0;
				vm.IsVisualizar = true;
			}

			_validar.PossuiModelosAtividades(vm.Roteiro.ModelosAtuais, id);
			vm.Roteiro.Observacoes = Regex.Replace(vm.Roteiro.Observacoes, @"<[^>]*>", String.Empty);

			if (Request.IsAjaxRequest())
			{
				return PartialView("VisualizarPartial", vm);
			}

			return View("Visualizar", vm);
		}

		#endregion

		#region Baixar Arquivo

		[Permite(Tipo = ePermiteTipo.Logado)]
		public FileResult Baixar(int id)
		{
			return ViewModelHelper.BaixarArquivo(id);
		}

		#endregion

		#region Relatório de Roteiro

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult RelatorioRoteiro(int id, string tid = "")
		{
			try
			{
				PdfRoteiroOrientativoInterno pdfRoteiro = new PdfRoteiroOrientativoInterno();
				return ViewModelHelper.GerarArquivoPdf(pdfRoteiro.Gerar(id, tid), "Relatorio de Roteiro Orientativo");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}