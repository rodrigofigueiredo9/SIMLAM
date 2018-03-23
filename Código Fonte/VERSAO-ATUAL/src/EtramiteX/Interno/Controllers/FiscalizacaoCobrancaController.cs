using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class FiscalizacaoCobrancaController : DefaultController
	{
		#region Propriedades

		FiscalizacaoBus _busFiscalizacao = new FiscalizacaoBus();
		ListaBus _busLista = new ListaBus();
		CobrancaDUADa _duaDa = new CobrancaDUADa();
		CobrancaBus _bus = new CobrancaBus();
		NotificacaoBus _busNotificacao = new NotificacaoBus();

		#endregion

		#region Cobranca
		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
		public ActionResult Cobranca(int? id) => View(this.GetCobrancaVM(id));

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
		public ActionResult CobrancaVisualizar(int id, int? index) => View(this.GetCobrancaVM(id, true, index));

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
		public ActionResult CobrancaCriar(Cobranca cobranca)
		{
			_bus.Salvar(cobranca);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("CobrancaVisualizar", "FiscalizacaoCobranca", new { id = cobranca.NumeroFiscalizacao, Msg = Validacao.QueryParam(new List<Mensagem>() { Mensagem.CobrancaMsg.Salvar }) })
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
		public ActionResult CobrancaRecalcular(Cobranca cobranca) =>
			Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "CobrancaPartial", this.GetCobrancaVM(cobranca.NumeroFiscalizacao, false, null, cobranca)) }, JsonRequestBehavior.AllowGet);

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
		public ActionResult CobrancaNovoParcelamento(Cobranca cobranca)
		{
			_bus.NovoParcelamento(cobranca);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("Cobranca", "FiscalizacaoCobranca", new { id = cobranca.NumeroFiscalizacao, Msg = Validacao.QueryParam(new List<Mensagem>() { Mensagem.CobrancaMsg.NovoParcelamento }) })
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PessoaVisualizar })]
		public ActionResult FiscalizacaoPessoaModal(Cobranca cobranca) => PartialView("FiscalizacaoPessoaModal", this.GetCobrancaVM(cobranca.NumeroFiscalizacao, true));

		[Permite(RoleArray = new Object[] { ePermissao.ProcessoCriar, ePermissao.ProcessoEditar })]
		public ActionResult ObterFiscalizacao(int fiscalizacaoId)
		{
			_bus.ValidarAssociar(fiscalizacaoId);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, Fiscalizacao = _busFiscalizacao.Obter(fiscalizacaoId), Notificacao = _busNotificacao.Obter(fiscalizacaoId) }, JsonRequestBehavior.AllowGet);
		}

		private CobrancaVM GetCobrancaVM(int? fiscalizacaoId, bool visualizar = false, int? index = null, Cobranca entidade = null)
		{
			var cobranca = entidade ?? _bus.Obter(fiscalizacaoId.GetValueOrDefault(0)) ?? new Cobranca();
			var maximoParcelas = 0;

			if (fiscalizacaoId > 0)
			{
				var fiscalizacao = _busFiscalizacao.Obter(fiscalizacaoId.Value);
				var notificacao = _busNotificacao.Obter(fiscalizacaoId.Value) ?? new Notificacao();
				if (cobranca.Id == 0)
				{
					cobranca = entidade ?? new Cobranca(fiscalizacao, notificacao);
					if((cobranca.Parcelamentos?.Count ?? 0) == 0)
					{
						cobranca.Parcelamentos = new List<CobrancaParcelamento>();
						if (cobranca.UltimoParcelamento.ValorMulta == 0)
							cobranca.UltimoParcelamento.ValorMulta = cobranca.UltimoParcelamento.ValorMultaAtualizado;
						cobranca.Parcelamentos.Add(cobranca.UltimoParcelamento);
					}

					var parcelamento = cobranca.Parcelamentos[0];
					maximoParcelas = _bus.GetMaximoParcelas(cobranca, parcelamento);
					if (parcelamento.QuantidadeParcelas == 0)
						parcelamento.QuantidadeParcelas = maximoParcelas;					
					parcelamento.DUAS = new List<CobrancaDUA>();
				}
				else
				{
					if ((cobranca.Parcelamentos?.Count ?? 0) == 0)
					{
						cobranca.Parcelamentos = new List<CobrancaParcelamento>();
						if (cobranca.UltimoParcelamento.ValorMulta == 0)
							cobranca.UltimoParcelamento.ValorMulta = cobranca.UltimoParcelamento.ValorMultaAtualizado;
						cobranca.Parcelamentos.Add(cobranca.UltimoParcelamento);
					}

					var dataVencimento = notificacao?.DataIUF?.Data?.AddDays(30) ?? cobranca.DataIUF.Data.Value.AddDays(30);
					if (dataVencimento.DayOfWeek == DayOfWeek.Saturday)
						dataVencimento = dataVencimento.AddDays(2);
					else if (dataVencimento.DayOfWeek == DayOfWeek.Monday)
						dataVencimento = dataVencimento.AddDays(1);

					var parcelamento = index.HasValue ? cobranca.Parcelamentos[index.Value] : cobranca.Parcelamentos?.FindLast(x => x.DataEmissao.IsValido) ?? new CobrancaParcelamento(fiscalizacao, dataVencimento);
					maximoParcelas = _bus.GetMaximoParcelas(cobranca, parcelamento);

					if (cobranca.Parcelamentos.Count == 0 || parcelamento?.DUAS?.Count == 0)
					{
						parcelamento.QuantidadeParcelas = maximoParcelas;
						parcelamento.DUAS = new List<CobrancaDUA>();
						if ((cobranca.Parcelamentos?.Count ?? 0) == 0)
						{
							cobranca.Parcelamentos = new List<CobrancaParcelamento>();
							cobranca.Parcelamentos.Add(parcelamento);
						}
					}
				}

				var ultimoParcelamento = cobranca.Parcelamentos.FindLast(x => x.DataEmissao.IsValido);
				if (ultimoParcelamento.QuantidadeParcelas > 0 && ultimoParcelamento.DUAS.Count == 0)
				{
					ultimoParcelamento.DUAS = _bus.GerarParcelas(cobranca, ultimoParcelamento);
					_bus.CalcularParcelas(cobranca, ultimoParcelamento);
				}
				else if(entidade != null)
					_bus.CalcularParcelas(cobranca, ultimoParcelamento);
			}
			var vm = new CobrancaVM(cobranca, _busLista.InfracaoCodigoReceita, maximoParcelas, visualizar, index);

			return vm;
		}

		#region Filtrar
		
		public ActionResult CobrancaListar()
		{
			ListarCobrancasVM vm = new ListarCobrancasVM(_busLista.QuantPaginacao, _busLista.InfracaoCodigoReceita, _busLista.FiscalizacaoSituacao.Where(x => x.Id != "4"/*Cancelar Conclusão*/).ToList());
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return PartialView(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoListar })]
		public ActionResult CobrancaFiltrar(ListarCobrancasVM vm, string SituacaoFiscalizacao, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarCobrancasVM>(vm.UltimaBusca).Filtros;
			
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);

			Resultados<CobrancasResultado> resultados = _bus.CobrancaFiltrar(vm.Filtros, vm.Paginacao);

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			if (resultados == null)
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			
			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;
			vm.PodeEditar = User.IsInRole(ePermissao.FiscalizacaoEditar.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.FiscalizacaoVisualizar.ToString());

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "CobrancaListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#endregion Cobranca
	}
}
