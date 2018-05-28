using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class TramitacaoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		ProtocoloBus _busProtocolo = new ProtocoloBus();
		TramitacaoBus _bus = new TramitacaoBus();
		OrgaoExternoBus _busExt = new OrgaoExternoBus();
		TramitacaoArquivoBus _busArquivo = new TramitacaoArquivoBus();
		TramitacaoValidar _validar = new TramitacaoValidar();
		ArquivarBus _busArquivamento = new ArquivarBus();
		ArquivarValidar _arquivarValidar = new ArquivarValidar();
		PdfTramitacao _pdf = new PdfTramitacao();

		#endregion

		#region Tramitacao

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoCancelar, ePermissao.TramitacaoEnviar, ePermissao.TramitacaoEnviarRegistro, ePermissao.TramitacaoReceber, ePermissao.TramitacaoConfigurar, ePermissao.TramitacaoReceberRegistro })]
		public ActionResult Index()
		{
			if (User.IsInRole(ePermissao.TramitacaoConfigurar.ToString()))
			{
				return RedirectToAction("Configurar");
			}

			return View("Tramitacoes", ConfigurarTramitacao());
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoCancelar, ePermissao.TramitacaoEnviar, ePermissao.TramitacaoEnviarRegistro, ePermissao.TramitacaoReceber, ePermissao.TramitacaoReceberRegistro })]
		public ActionResult Tramitacoes()
		{
			return View("Tramitacoes", ConfigurarTramitacao());
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoCancelar, ePermissao.TramitacaoEnviar, ePermissao.TramitacaoEnviarRegistro, ePermissao.TramitacaoReceber, ePermissao.TramitacaoReceberRegistro })]
		public ActionResult ObterTramitacaoSetor(TramitacaoFiltroVM filtro)
		{
			TramitacoesVM vm = new TramitacoesVM();
			vm.SetorId = filtro.SetorId;

			if ((filtro.SetorId == 0 && filtro.FuncionarioId == 0) ||
				(_bus.Registrador(_bus.User.FuncionarioId, filtro.SetorId)))
			{
				vm.FuncionarioId = filtro.FuncionarioId;
				ConfigurarRegistrador(vm);
			}
			else
			{
				vm.FuncionarioId = (filtro.FuncionarioId == 0 ? _bus.User.FuncionarioId : filtro.FuncionarioId);
				ConfigurarFuncionario(vm);

				if (filtro.SetorId == 0 && _bus.Registrador(vm.FuncionarioId))
				{
					vm.CarregarListas(_bus.ObterFuncionariosRegistrador(vm.FuncionarioId), null);
					vm.MostrarFuncionario = false;
				}
			}

			ConfigurarAcoesGrid(vm.Tramitacao);

			Tramitacao tramitacao = vm.Tramitacao;
			vm.Tramitacao = new Tramitacao();

			JsonResult json = Json(new
			{
				@Msg = Validacao.Erros,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "TramitacaoConteudoPartial", tramitacao),
				@VM = new { MostrarSetor = vm.MostrarSetor, MostrarFuncionario = vm.MostrarFuncionario, FuncionariosLst = vm.FuncionariosLst },
			}, JsonRequestBehavior.AllowGet);

			return json;
		}

		private void ConfigurarAcoesGrid(Tramitacao tramitacao)
		{
			if (!_bus.Registrador(_bus.User.FuncionarioId) &&
				!(User as EtramitePrincipal).IsInAnyRole(String.Join(",", new[]{
					ePermissao.TramitacaoEnviar.ToString(),
					ePermissao.TramitacaoReceber.ToString(),
					ePermissao.TramitacaoCancelar.ToString()
					})))
			{
				return;
			}

			List<int> setoresRegistrador = _bus.ObterSetoresRegistrador(_bus.User.FuncionarioId).Select(x => x.Id).ToList();
			bool isPermEnviar = User.IsInRole(ePermissao.TramitacaoEnviar.ToString());
			bool isPermCancelar = User.IsInRole(ePermissao.TramitacaoCancelar.ToString());
			bool isPermReceber = User.IsInRole(ePermissao.TramitacaoReceber.ToString());

			tramitacao.ProtocolosPosse.ForEach(x =>
				x.IsExibirBotao = setoresRegistrador.Any(setoresId => setoresId == x.DestinatarioSetor.Id) || isPermEnviar
				);

			tramitacao.ProtocolosEnviado.ForEach(x =>
				x.IsExibirBotao = setoresRegistrador.Any(setoresId => setoresId == x.DestinatarioSetor.Id) || isPermCancelar
				);

			tramitacao.ProtocolosReceber.ForEach(x =>
				x.IsExibirBotao = setoresRegistrador.Any(setoresId => setoresId == x.DestinatarioSetor.Id) || isPermReceber
				);

			tramitacao.ProtocolosReceberSetor.ForEach(x =>
				x.IsExibirBotao = setoresRegistrador.Any(setoresId => setoresId == x.DestinatarioSetor.Id) || isPermReceber
				);
		}

		private TramitacoesVM ConfigurarTramitacao()
		{
			TramitacoesVM vm = new TramitacoesVM();
			vm.FuncionarioId = _bus.User.FuncionarioId;

			List<Setor> setores = _bus.ObterSetoresFuncionario(vm.FuncionarioId);

			if (setores.Count == 1)
				vm.SetorId = setores.First().Id;

			if (_bus.Registrador(vm.FuncionarioId))
			{
				ConfigurarRegistrador(vm, setores);
			}
			else
			{
				ConfigurarFuncionario(vm, setores);
			}

			ConfigurarAcoesGrid(vm.Tramitacao);

			return vm;
		}

		private void ConfigurarRegistrador(TramitacoesVM vm, List<Setor> setores = null)
		{
			List<FuncionarioLst> lstFuncionario = (vm.SetorId == 0 ? _bus.ObterFuncionariosRegistrador(_bus.User.FuncionarioId) : _bus.ObterFuncionariosSetor(vm.SetorId));

			vm.CarregarListas(lstFuncionario, setores);

			ValidarTramitacao(vm, _bus.ObterTramitacoes(vm.SetorId, vm.FuncionarioId));
		}

		private void ConfigurarFuncionario(TramitacoesVM vm, List<Setor> setores = null)
		{
			vm.CarregarListas(new List<FuncionarioLst>(), setores);
			ValidarTramitacao(vm, _bus.ObterTramitacoes(vm.SetorId, vm.FuncionarioId));
		}

		private void ValidarTramitacao(TramitacoesVM vm, Tramitacao tramitacao)
		{
			if (tramitacao != null)
			{
				vm.Tramitacao = tramitacao;
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar, ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult TramitacaoEnviar(int? protocoloId)
		{
			IProtocolo protocolo = _busProtocolo.ObterSimplificado(protocoloId.GetValueOrDefault());

			bool permissaoTramitar = User.IsInRole(ePermissao.TramitacaoEnviar.ToString());

			bool EnviarRegistro = _bus.ValidarRedirecionamentoEnviar(protocolo, permissaoTramitar);

			return Json(new { Msg = Validacao.Erros, @EnviarRegistro = EnviarRegistro, @Protocolo = protocolo }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoReceber, ePermissao.TramitacaoReceberRegistro })]
		public ActionResult TramitacaoReceber(int id)
		{
			Tramitacao tramitacao = _bus.Obter(id);

			bool permissaoTramitar = User.IsInRole(ePermissao.TramitacaoReceber.ToString());

			bool ReceberRegistro = _bus.ValidarRedirecionamentoReceber(tramitacao, permissaoTramitar);

			return Json(new { Msg = Validacao.Erros, @ReceberRegistro = ReceberRegistro, @Id = id }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Cancelar

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoCancelar })]
		public ActionResult Cancelar(TramitacaoFiltroVM vm)
		{
			Tramitacao tramitacao = new Tramitacao();
			tramitacao.Id = vm.TramitacaoId;
			tramitacao.Protocolo.Id = vm.ProtocoloId;
			tramitacao.Protocolo.IsProcesso = vm.IsProcesso;
			ProtocoloNumero prot = new ProtocoloNumero(vm.ProtocoloNumero);
			tramitacao.Protocolo.NumeroProtocolo = prot.Numero;
			tramitacao.Protocolo.Ano = prot.Ano;
			tramitacao.DestinatarioSetor.Id = vm.SetorId;

			bool permissaoTramitar = User.IsInRole(ePermissao.TramitacaoCancelar.ToString());

			_bus.Cancelar(tramitacao, permissaoTramitar);

			return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Enviar

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar })]
		public ActionResult Enviar(int? protocoloId = null, int? setorId = null)
		{
			List<Setor> lstSetoresFunc = _bus.ObterSetoresFuncionario(_bus.ObterExecutor().Id);

			EnviarVM vm = new EnviarVM(_bus.ObterExecutor(), _bus.ObterExecutor(), _bus.ObterMotivos(), _busLista.TiposProcesso, _busLista.TiposDocumento, lstSetoresFunc, _busLista.SetoresAtuais);

			if (lstSetoresFunc != null && lstSetoresFunc.Count == 1 && setorId == null && protocoloId == null)
			{
				setorId = lstSetoresFunc.First().Id;
			}

			if (setorId.HasValue)
			{
				vm.Enviar.Remetente = new Funcionario { Id = _bus.User.FuncionarioId, Nome = _bus.User.Name };
				vm.Enviar.RemetenteSetor = new Setor { Id = setorId.Value };

				if (_validar.SetorSelecionadoMesmoTipoTramitacao((int)eTramitacaoTipo.Normal, setorId.GetValueOrDefault()))
				{
					ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro() { EmposseId = _bus.User.FuncionarioId, EmposseSetorId = setorId ?? 0 };

					Resultados<Tramitacao> resultados = _bus.ObterTramitacoes(filtro);

					if (resultados != null && resultados.Itens != null && resultados.Itens.Count > 0)
					{
						vm.Tramitacoes = resultados.Itens;
						foreach (Tramitacao tramit in vm.Tramitacoes)
						{
							if (tramit.Protocolo.Id == protocoloId)
							{
								tramit.IsSelecionado = true;
								break;
							}
						}
					}
					if ((vm.Tramitacoes?.Count(x => x.Protocolo?.Tipo?.Texto == "Documento Avulso" || x.Protocolo?.Tipo?.Texto == "Ofício (Administrativo)") ?? 0) == 0)
						vm.SetoresDestinatario.Remove(vm.SetoresDestinatario.Find(x => x.Value == "258"));
				}
			}

			return View("Enviar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar })]
		public ActionResult Enviar(EnviarVM vm)
		{
			vm.Enviar.TramitacaoTipo = (int)eTramitacaoTipo.Normal;
			vm.Enviar.SituacaoId = (int)eTramitacaoSituacao.Tramitando;

			if (_validar.EnviarPreValidar(vm.Enviar))
			{
				PreencherEnviar(vm);
				if (_bus.Enviar(vm.Tramitacoes))
				{
					List<int> ids = new List<int>();
					string urlRedireciona = string.Empty;

					vm.Tramitacoes.ForEach(x =>
					{
						ids.Add(x.Id);
					});

					urlRedireciona = Url.Action("Index", "Tramitacao");
					urlRedireciona += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + HttpUtility.HtmlEncode(String.Join(",", ids));

					return Json(new
					{
						IsTramitacoesEnviadas = Validacao.EhValido,
						UrlRedireciona = urlRedireciona,
						//@Tramitacoes = vm.Tramitacoes, 
						Msg = Validacao.Erros
					});
				}
			}
			return Json(new { IsTramitacoesEnviadas = false, Msg = Validacao.Erros });
		}

		#region Métodos Auxiliares

		private void PreencherEnviar(EnviarVM vm)
		{
			foreach (Tramitacao tramitacao in vm.Tramitacoes)
			{
				tramitacao.Executor = _bus.ObterExecutor();
				tramitacao.Remetente = vm.Enviar.Remetente;
				tramitacao.RemetenteSetor.Id = vm.Enviar.RemetenteSetor.Id;
				tramitacao.DataEnvio.Data = DateTime.Now;
				tramitacao.Objetivo.Id = vm.Enviar.ObjetivoId;
				tramitacao.SituacaoId = vm.Enviar.SituacaoId; //tramitando
				tramitacao.Tipo = vm.Enviar.TramitacaoTipo;
				tramitacao.Despacho = vm.Enviar.Despacho;
				tramitacao.DestinatarioSetor.Id = vm.Enviar.DestinatarioSetor.Id;
				tramitacao.Destinatario = vm.Enviar.Destinatario;
				tramitacao.OrgaoExterno = vm.Enviar.OrgaoExterno;//somente para tramitação para órgão externo
				tramitacao.DestinoExterno = vm.Enviar.DestinoExterno;
				tramitacao.CodigoRastreio = vm.Enviar.CodigoRastreio;
				tramitacao.FormaEnvio = vm.Enviar.FormaEnvio;
				tramitacao.NumeroAutuacao = vm.Enviar.NumeroAutuacao;
				if (tramitacao.Protocolo?.Id > 0)
				{
					var protocolo = _busProtocolo.Obter(tramitacao.Protocolo.Id.GetValueOrDefault(0));
					if (protocolo != null)
						tramitacao.Protocolo.Tipo = protocolo.Tipo;
				}
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar })]
		public ActionResult ObterTodasTramitacoes(int remetenteId, int SetorId)
		{
			EnviarVM vm = new EnviarVM();
			ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro() { EmposseId = remetenteId, EmposseSetorId = SetorId };//, TramitacaoTipoId = (int)eTramitacaoTipo.Normal 

			Resultados<Tramitacao> resultados = _bus.ObterTramitacoes(filtro);

			if (resultados == null || resultados.Quantidade <= 0)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Tramitacoes = resultados.Itens;

			return PartialView("EnviarEmPosse", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult ObterTodasTramitacoesRegistro(int remetenteId, int SetorId)
		{
			EnviarVM vm = new EnviarVM();
			ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro() { EmposseId = remetenteId, EmposseSetorId = SetorId };//TramitacaoTipoId = (int)eTramitacaoTipo.Registro

			Resultados<Tramitacao> resultados = _bus.ObterTramitacoes(filtro);

			if (resultados == null || resultados.Quantidade <= 0)
			{
				return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
			vm.Tramitacoes = resultados.Itens;

			return PartialView("EnviarEmPosse", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar, ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult AdicionarTramitacaoPorNumeroProtocolo(int setorId, string numero, int? funcionarioId = null)
		{
			List<Tramitacao> tramitacoes = new List<Tramitacao>();
			Tramitacao tramitacao = null;
			if (funcionarioId == null)
			{
				funcionarioId = _bus.ObterExecutor().Id;
			}

			Protocolo protocolo = _validar.AdicionarProtocolo(funcionarioId, setorId, numero);

			if (Validacao.EhValido && protocolo.Id > 0)
			{
				Resultados<Tramitacao> resultados = _bus.FiltrarEmPosse(new ListarTramitacaoFiltro() { EmposseId = funcionarioId ?? 0, EmposseSetorId = setorId, Protocolo = new ProtocoloNumero(numero, protocolo.Id.Value, protocolo.IsProcesso) });
				if (resultados == null)
				{
					return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
				}
				tramitacoes = resultados.Itens;
				tramitacao = (tramitacoes.Count > 0) ? tramitacoes[0] : null;
			}

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				Tramitacao = tramitacao
			},
				JsonRequestBehavior.AllowGet
			);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar, ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult ObterTramitacaoTemplate()
		{
			EnviarVM vm = new EnviarVM();
			return PartialView("EnviarEmPosse", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar, ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult ObterFuncionariosDoSetor(int Id)
		{
			return Json(_bus.ObterFuncionariosSetor(Id), JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar })]
		public ActionResult ObterSetoresDoFuncionario(int Id)
		{
			return Json(_bus.ObterSetoresFuncionario(Id), JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviar })]
		public ActionResult ValidarTipoSetorNormal(int setorId)
		{
			_validar.SetorSelecionadoMesmoTipoTramitacao((int)eTramitacaoTipo.Normal, setorId);
			return Json(new { IsSetorValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult ValidarTipoSetorRegistro(int setorId)
		{
			_validar.SetorSelecionadoMesmoTipoTramitacao((int)eTramitacaoTipo.Registro, setorId);
			return Json(new { IsSetorValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#endregion

		#region Enviar por Registro

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult EnviarRegistro(int? protocoloId = null, int? setorId = null)
		{
			List<Setor> setores = _bus.ObterSetoresRegistrador(_bus.User.FuncionarioId);

			EnviarVM vm = new EnviarVM(_bus.ObterExecutor(), null, _bus.ObterMotivos(), _busLista.TiposProcesso, _busLista.TiposDocumento, setores, _busLista.SetoresAtuais);

			if (protocoloId.HasValue && setorId.HasValue)
			{
				vm.Enviar.Remetente = new Funcionario { Id = _bus.ObterFuncionarioIdPosse(protocoloId.Value) };
				vm.Enviar.RemetenteSetor = new Setor { Id = setorId.Value };
				vm.CarregarRemetentesFuncionarios(_bus.ObterFuncionariosSetor(setorId.Value));

				if (_validar.SetorSelecionadoMesmoTipoTramitacao((int)eTramitacaoTipo.Registro, setorId.GetValueOrDefault()))
				{
					ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro() { EmposseId = vm.Enviar.Remetente.Id, EmposseSetorId = setorId ?? 0 };//, TramitacaoTipoId = (int)eTramitacaoTipo.Registro

					Resultados<Tramitacao> retorno = _bus.FiltrarEmPosse(filtro);

					if (retorno != null && retorno.Itens != null && retorno.Itens.Count > 0)
					{
						vm.Tramitacoes = retorno.Itens;
						foreach (Tramitacao tramit in vm.Tramitacoes)
						{
							if (tramit.Protocolo.Id == protocoloId)
							{
								tramit.IsSelecionado = true;
								break;
							}
						}
					}
				}
			}
			else
			{
				if (setores != null && setores.Count == 1)
				{
					List<FuncionarioLst> funcionarios = _bus.ObterFuncionariosSetor(setores.First().Id);
					vm.CarregarRemetentesFuncionarios(funcionarios);

					if (funcionarios != null && funcionarios.Count == 1)
					{
						vm.Enviar.Remetente = new Funcionario { Id = funcionarios.First().Id };
						vm.Enviar.RemetenteSetor = new Setor { Id = setores.First().Id };

						if (_validar.SetorSelecionadoMesmoTipoTramitacao((int)eTramitacaoTipo.Registro, setorId.GetValueOrDefault()))
						{
							ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro() { EmposseId = vm.Enviar.Remetente.Id, EmposseSetorId = vm.Enviar.RemetenteSetor.Id };//, TramitacaoTipoId = (int)eTramitacaoTipo.Registro

							Resultados<Tramitacao> retorno = _bus.ObterTramitacoes(filtro);

							if (retorno != null && retorno.Itens != null && retorno.Itens.Count > 0)
							{
								vm.Tramitacoes = retorno.Itens;
							}
						}
					}
					else
					{
						vm.CarregarRemetentesFuncionarios(new List<FuncionarioLst>());
					}
				}
				else
				{
					vm.CarregarRemetentesFuncionarios(new List<FuncionarioLst>());
				}
			}

			return View("EnviarRegistro", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviarRegistro })]
		public ActionResult EnviarRegistro(EnviarVM vm)
		{
			vm.Enviar.TramitacaoTipo = (int)eTramitacaoTipo.Registro;
			vm.Enviar.SituacaoId = (int)eTramitacaoSituacao.Tramitando;

			if (_validar.EnviarPreValidar(vm.Enviar))
			{
				PreencherEnviar(vm);
				if (_bus.Enviar(vm.Tramitacoes))
				{
					// Abre o PDF da Tramitação
					string urlRedireciona = Url.Action("Index", "Tramitacao");
					urlRedireciona += "?Msg=" + Validacao.QueryParam();

					return Json(new { IsTramitacoesEnviadas = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Tramitacoes = vm.Tramitacoes, Msg = Validacao.Erros });
				}
			}
			return Json(new { IsTramitacoesEnviadas = false, Msg = Validacao.Erros });
		}

		#endregion

		#region Enviar para Orgão Externo

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviarOrgaoExterno })]
		public ActionResult EnviarExterno()
		{
			List<Setor> funcionarioSetores = _bus.ObterSetoresFuncionario(_bus.ObterExecutor().Id);
			EnviarVM vm = new EnviarVM(_bus.ObterExecutor(), _bus.ObterExecutor(), _bus.ObterMotivos(), _busLista.TiposProcesso,
				_busLista.TiposDocumento, funcionarioSetores, _busLista.SetoresAtuais, _busExt.ObterOrgaosExterno());

			if (funcionarioSetores != null && funcionarioSetores.Count == 1)
			{
				vm.Enviar.Remetente = new Funcionario { Id = _bus.User.FuncionarioId, Nome = _bus.User.Name };
				vm.Enviar.RemetenteSetor = new Setor { Id = funcionarioSetores.First().Id };

				ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro() { RemetenteId = _bus.User.FuncionarioId, RemetenteSetorId = funcionarioSetores.First().Id, TramitacaoTipoId = (int)eTramitacaoTipo.Normal };

				Resultados<Tramitacao> resultados = new Resultados<Tramitacao>();

				if (resultados != null)
				{
					vm.Tramitacoes = resultados.Itens;
				}
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("EnviarExternoPartial", vm);
			}
			else
			{
				return View("EnviarExterno", vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoEnviarOrgaoExterno })]
		public ActionResult EnviarExterno(EnviarVM vm)
		{
			vm.Enviar.TramitacaoTipo = (int)eTramitacaoTipo.Normal;
			vm.Enviar.SituacaoId = (int)eTramitacaoSituacao.ParaOrgaoExterno;

			if (_validar.EnviarExternoPreValidar(vm.Enviar))
			{
				PreencherEnviar(vm);
				if (_busExt.EnviarExterno(vm.Tramitacoes))
				{
					string urlRedireciona = Url.Action("Index", "Tramitacao");
					urlRedireciona += "?Msg=" + Validacao.QueryParam();

					return Json(new { IsTramitacoesEnviadas = Validacao.EhValido, UrlRedireciona = urlRedireciona });
				}
			}
			return Json(new { IsTramitacoesEnviadas = false, Msg = Validacao.Erros });
		}

		#endregion

		#region Receber

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoReceber })]
		public ActionResult Receber(int? id)
		{
			List<Setor> funcionarioSetores = _bus.ObterSetoresFuncionario(_bus.User.FuncionarioId);
			ReceberVM vm = new ReceberVM(_bus.ObterExecutor(), funcionarioSetores);
			vm.DataRecebimento.Data = DateTime.Today;

			if (id.HasValue && id.Value > 0)
			{
				Tramitacao tramitacao = _bus.Obter(id.Value);
				vm.SetorDestinatario = tramitacao.DestinatarioSetor;
				vm.TramitacaoSelecionadaId = id.Value;
				vm.MarcarCheckTodos = true;
			}
			else if (funcionarioSetores.Count == 1)
			{
				vm.SetorDestinatario = funcionarioSetores.First();
			}

			if (_validar.SetorSelecionadoMesmoTipoTramitacao((int)eTramitacaoTipo.Normal, vm.SetorDestinatario.Id))
			{
				if (vm.SetorDestinatario.Id > 0)
				{
					vm.Tramitacoes = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioSetorId = vm.SetorDestinatario.Id, DestinatarioId = _bus.User.FuncionarioId, DestinatarioNaoNulo = true });

					vm.TramitacoesSetor = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioSetorId = vm.SetorDestinatario.Id, DestinatarioNulo = true });

					if (vm.NumeroTramitacoes <= 0)
					{
						Validacao.Add(Mensagem.Tramitacao.NaoExisteProtocoloAReceber);
					}
				}
			}

			vm.DataRecebimento.Data = DateTime.Now;
			return View("Receber", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoReceber })]
		public ActionResult ReceberFiltrar(int setorId)
		{
			if (_validar.SetorPorRegistrado(setorId))
			{
				return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
			}

			ReceberVM vm = new ReceberVM();
			vm.Tramitacoes = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioId = _bus.User.FuncionarioId, DestinatarioSetorId = setorId, DestinatarioNaoNulo = true });

			vm.TramitacoesSetor = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioSetorId = setorId, DestinatarioNulo = true });

			vm.SetorDestinatario.Id = setorId;

			if (vm.NumeroTramitacoes == 0)
			{
				Validacao.Add(Mensagem.Tramitacao.NaoExisteProtocoloAReceber);
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@HtmlTramitacoes = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ReceberTramitacoes", vm),
				@HtmlTramitacoesSetor = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ReceberTramitacoesSetor", vm),
				@SetorDestinatarioId = vm.SetorDestinatario.Id,
				@NumeroTramitacoes = vm.NumeroTramitacoes,
				//@vm = vm,
				@EhValido = Validacao.EhValido
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoReceber })]
		public ActionResult ReceberSalvar(ReceberVM vm)
		{
			string urlRedireciona = string.Empty;
			vm.DataRecebimento.Data = DateTime.Now;
			if (_bus.Receber(vm.Tramitacoes.Itens))
			{
				Validacao.Add(Mensagem.Tramitacao.ReceberSucesso);
				urlRedireciona = Url.Action("Index", "Tramitacao") + "?Msg=" + Validacao.QueryParam();
			}

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@urlRedireciona = urlRedireciona
			});
		}

		#endregion

		#region Receber por Registro

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoReceberRegistro })]
		public ActionResult ReceberRegistro(int? id)
		{
			List<Setor> funcionarioSetores = _bus.ObterSetoresRegistrador(_bus.User.FuncionarioId);
			ReceberRegistroVM vm = new ReceberRegistroVM(_bus.ObterExecutor(), funcionarioSetores);
			vm.DataRecebimento.Data = DateTime.Today;

			if (id.HasValue && id.Value > 0)
			{
				Tramitacao tramitacao = _bus.Obter(id.Value);
				vm.SetorDestinatario = tramitacao.DestinatarioSetor;
				vm.TramitacaoSelecionadaId = id.Value;
			}
			else if (funcionarioSetores.Count == 1)
			{
				vm.SetorDestinatario = funcionarioSetores.First();
			}

			if (_validar.SetorSelecionadoMesmoTipoTramitacao((int)eTramitacaoTipo.Registro, vm.SetorDestinatario.Id))
			{
				if (vm.SetorDestinatario.Id > 0)
				{
					vm.Tramitacoes = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioSetorId = vm.SetorDestinatario.Id, DestinatarioNaoNulo = true });

					vm.TramitacoesSetor = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioSetorId = vm.SetorDestinatario.Id, DestinatarioNulo = true });

					List<FuncionarioLst> funcionarios = _bus.ObterFuncionariosSetor(vm.SetorDestinatario.Id);
					vm.SetorFuncionarios = ViewModelHelper.CriarSelectList(funcionarios, true);

					if (funcionarios != null && funcionarios.Count == 1)
					{
						vm.FuncionarioDestinatario = new Funcionario() { Id = funcionarios.First().Id };
					}

					if (vm.NumeroTramitacoes <= 0)
					{
						Validacao.Add(Mensagem.Tramitacao.NaoExisteProtocoloAReceberRegistro);
					}
				}
			}

			return View("ReceberRegistro", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoReceberRegistro })]
		public ActionResult ReceberRegistroFiltrar(int setorId)
		{
			ReceberRegistroVM vm = new ReceberRegistroVM();

			vm.Tramitacoes = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioSetorId = setorId, DestinatarioNaoNulo = true });

			vm.TramitacoesSetor = _bus.Filtrar(new ListarTramitacaoFiltro() { DestinatarioSetorId = setorId, DestinatarioNulo = true });

			List<FuncionarioLst> funcs = _bus.ObterFuncionariosSetor(setorId);
			vm.SetorFuncionarios = ViewModelHelper.CriarSelectList(funcs, true);
			vm.SetorDestinatario.Id = setorId;

			if (funcs != null && funcs.Count == 1)
			{
				vm.FuncionarioDestinatario = new Funcionario() { Id = funcs.First().Id };
			}

			if (vm.NumeroTramitacoes <= 0)
			{
				Validacao.Add(Mensagem.Tramitacao.NaoExisteProtocoloAReceberRegistro);
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@HtmlTramitacoes = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ReceberRegistroTramitacoes", vm),
				@HtmlTramitacoesSetor = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ReceberRegistroTramitacoesSetor", vm),
				@vm = vm,
				@EhValido = Validacao.EhValido
			},
				JsonRequestBehavior.AllowGet
			);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoReceberRegistro })]
		public ActionResult ReceberRegistroSalvar(ReceberRegistroVM vm)
		{
			vm.DataRecebimento.Data = DateTime.Now;
			if (_bus.ReceberRegistro(vm.Tramitacoes.Itens, vm.TramitacoesSetor.Itens, vm.FuncionarioDestinatario.Id))
			{
				Validacao.Add(Mensagem.Tramitacao.ReceberSucesso);
			}

			string urlRedireciona = Url.Action("Index", "Tramitacao") + "?Msg=" + Validacao.QueryParam();

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@vm = vm,
				@urlRedireciona = urlRedireciona
			});
		}

		#endregion

		#region Retirar de Órgao Externo

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoRetirarOrgaoExterno })]
		public ActionResult ReceberExterno(int? id)
		{
			List<Setor> setores = _bus.ObterSetoresFuncionario(_bus.User.FuncionarioId);
			ReceberVM vm = new ReceberVM(_bus.ObterExecutor(), setores, _busExt.ObterOrgaosExterno());

			vm.DataRecebimento.Data = DateTime.Now;

			if (setores != null && setores.Count == 1)
			{
				vm.SetorDestinatario = setores.First();
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("ReceberExternoPartial", vm);
			}
			else
			{
				return View("ReceberExterno", vm);
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoRetirarOrgaoExterno })]
		public ActionResult ReceberExternoFiltrar(int orgaoId, string orgaoTexto = null)
		{
			ReceberVM vm = new ReceberVM();

			if (orgaoId > 0)
			{
				vm.Tramitacoes = _bus.Filtrar(new ListarTramitacaoFiltro() { TramitacaoSituacaoId = (int)eTramitacaoSituacao.ParaOrgaoExterno, OrgaoExternoId = orgaoId });

				if (vm.NumeroTramitacoes <= 0)
				{
					if (!string.IsNullOrEmpty(orgaoTexto))
					{
						Validacao.Add(Mensagem.Tramitacao.NaoEncontrouRegistrosOrgaoExterno);
					}
					else
					{
						Validacao.Add(Mensagem.Tramitacao.NaoEncontrouRegistros);
					}
				}
			}
			else
			{
				vm.Tramitacoes = new Resultados<Tramitacao>();
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@NumeroTramitacoes = vm.NumeroTramitacoes,
				@HtmlTramitacoes = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ReceberTramitacoesExterno", vm),
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoRetirarOrgaoExterno })]
		public ActionResult ReceberExterno(ReceberVM vm)
		{
			_busExt.ReceberExterno(vm.Tramitacoes.Itens, vm.SetorDestinatario.Id, vm.OrgaoExterno.Id);

			string urlRedireciona = Url.Action("Index", "Tramitacao");
			urlRedireciona += "?Msg=" + Validacao.QueryParam();

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedireciona = urlRedireciona
			});
		}

		#endregion

		#region Arquivo

		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoListar })]
		public ActionResult ArquivoListar()
		{
			ArquivoListarVM vm = new ArquivoListarVM(_busLista.QuantPaginacao, _busLista.SetoresAtuais);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoListar })]
		public ActionResult ArquivoListar(ArquivoListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ArquivoListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<TramitacaoArquivo> resultados = _busArquivo.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.ArquivoTramitacaoEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.ArquivoTramitacaoExcluir.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.ArquivoTramitacaoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ArquivoListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoCriar })]
		public ActionResult ArquivoCriar()
		{
			TramitacaoArquivoVM viewModel = new TramitacaoArquivoVM(_busLista.SetoresAtuais, _busLista.TiposTramitacaoArquivo, _busLista.SituacoesProcessoAtividade, _busLista.SituacoesProcessoAtividade);

			viewModel.CarregarTramitacaoArquivo(new TramitacaoArquivo(), _busLista.TiposTramitacaoArquivoModo);

			return View("ArquivoCriar", viewModel);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoEditar })]
		public ActionResult ArquivoEditar(int id)
		{
			TramitacaoArquivoVM vm = new TramitacaoArquivoVM(_busLista.SetoresAtuais, _busLista.TiposTramitacaoArquivo, _busLista.SituacoesProcessoAtividade, _busLista.SituacoesProcessoAtividade);

			vm.CarregarTramitacaoArquivo(_busArquivo.ObterTramitacaoArquivo(id), _busLista.TiposTramitacaoArquivoModo);

			_busArquivo.ValidarEditarTela(vm.TramitacaoArquivo);

			if (_busArquivo.PossuiProtocolo(vm.TramitacaoArquivo.Id ?? 0))
			{
				Validacao.Add(Mensagem.Tramitacao.ArquivoEditarSetorNaoEhPossivelPorPossuirProtocolo(eTipoMensagem.Informacao));
				vm.PodeEditarSetor = false;
			}

			return View("ArquivoEditar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoCriar, ePermissao.ArquivoTramitacaoEditar })]
		public ActionResult ArquivoSalvar(TramitacaoArquivo tramitacaoArquivo)
		{
			string urlRedirecionar = tramitacaoArquivo.Id == 0 ? Url.Action("ArquivoCriar") : Url.Action("ArquivoListar");

			_busArquivo.SalvarArquivo(tramitacaoArquivo);

			urlRedirecionar += "?Msg=" + Validacao.QueryParam();

			return Json(new { @IsArquivoSalvo = Validacao.EhValido, @UrlRedireciona = urlRedirecionar, @Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoVisualizar })]
		public ActionResult ArquivoVisualizar(int? id)
		{
			TramitacaoArquivoVM vm = new TramitacaoArquivoVM(_busLista.SetoresAtuais, _busLista.TiposTramitacaoArquivo, _busLista.SituacoesProcessoAtividade, _busLista.SituacoesProcessoAtividade);
			vm.IsVisualizar = true;

			if (id == null || !id.HasValue)
			{
				Validacao.Add(Mensagem.Arquivo.ArquivoInvalido);
			}
			else
			{
				vm.CarregarTramitacaoArquivo(_busArquivo.ObterTramitacaoArquivo(id.Value), _busLista.TiposTramitacaoArquivoModo);
			}

			return View(vm);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoExcluir })]
		public ActionResult ArquivoConfirmarExcluir(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			vm.Titulo = "Excluir Arquivo";
			vm.Id = id;
			TramitacaoArquivo arquivo = _busArquivo.ObterTramitacaoArquivo(id);
			vm.Mensagem = Mensagem.TramitacaoArquivo.MensagemExcluir(arquivo.Nome);
			return PartialView("Excluir", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoExcluir })]
		public ActionResult ArquivoExcluir(TramitacaoArquivo arquivo)
		{
			_busArquivo.ExcluirArquivo(arquivo.Id.Value);
			return Json(new { Msg = Validacao.Erros, @arquivo = arquivo, @EhValido = Validacao.EhValido });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoExcluir, ePermissao.ArquivoTramitacaoCriar, ePermissao.ArquivoTramitacaoEditar })]
		public ActionResult ArquivoValidarExcluirEstante(int idEstante)
		{
			_busArquivo.ValidarExcluirEstante(idEstante);
			return Json(new { Msg = Validacao.Erros, @EhValido = Validacao.EhValido });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ArquivoTramitacaoExcluir, ePermissao.ArquivoTramitacaoCriar, ePermissao.ArquivoTramitacaoEditar })]
		public ActionResult ArquivoValidarExcluirPrateleira(int idPrateleira)
		{
			_busArquivo.ValidarExcluirPrateleira(idPrateleira);
			return Json(new { Msg = Validacao.Erros, @EhValido = Validacao.EhValido });
		}

		#endregion

		#region Configurar

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoConfigurar })]
		public ActionResult Configurar()
		{
			ConfigurarVM vm = new ConfigurarVM();
			vm.Setores = _bus.ObterSetores();

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoConfigurar })]
		public ActionResult Configurar(List<Setor> setores, string urlRedirecionar)
		{
			if (_bus.MudarTipoTramitacaoSetor(setores))
			{
				urlRedirecionar += "?Msg=" + Validacao.QueryParam();
				return Json(new { @Salvo = Validacao.EhValido, @UrlRedireciona = urlRedirecionar });
			}
			else
			{
				return Json(new { @Salvo = false, @Msg = Validacao.Erros });
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoConfigurar })]
		public ActionResult ValidarFuncionarioContidoSetor(int funcionario, int setor, string funcionarioNome, string setorSigla)
		{
			_validar.FuncionarioContidoSetor(funcionario, setor, funcionarioNome, setorSigla);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Motivo

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoMotivoConfigurar })]
		public ActionResult ConfigurarMotivo()
		{
			MotivoTramitacaoVM Model = new MotivoTramitacaoVM();
			Model.Motivos = _bus.ObterMotivos();
			return View(Model);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoMotivoConfigurar })]
		public ActionResult ConfigurarMotivoSalvar(Motivo motivo)
		{
			_bus.SalvarMotivo(motivo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Id = motivo.Id });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoMotivoConfigurar })]
		public ActionResult ConfigurarMotivoAtivar(Motivo motivo)
		{
			_bus.AtivarMotivo(motivo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Id = motivo.Id });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoMotivoConfigurar })]
		public ActionResult ConfigurarMotivoDesativar(Motivo motivo)
		{
			_bus.DesativarMotivo(motivo);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Id = motivo.Id });
		}

		#endregion

		#region Historico

		[Permite(RoleArray = new Object[] { 
		ePermissao.ProcessoListar,
		ePermissao.DocumentoListar,
		ePermissao.TramitacaoEnviar,
		ePermissao.TramitacaoReceber,
		ePermissao.TramitacaoEnviarRegistro,
		ePermissao.TramitacaoReceberRegistro,
		ePermissao.TramitacaoEnviarOrgaoExterno,
		ePermissao.TramitacaoRetirarOrgaoExterno,
		ePermissao.TramitacaoArquivar,
		ePermissao.TramitacaoDesarquivar })]
		public ActionResult Historico(int id, int tipo)
		{
			HistoricoVM vm = new HistoricoVM();

			ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro();
			filtro.Protocolo.Id = id;
			filtro.Protocolo.IsProcesso = (tipo == (int)eTipoProtocolo.Processo);

			Resultados<Tramitacao> hstTramitacao = _bus.FiltrarHistorico(filtro);
			Resultados<HistoricoProtocolo> hstProtocolo;
			IProtocolo protocolo = new Protocolo();

			hstProtocolo = _busProtocolo.FiltrarHistoricoAssociados(new ListarProtocoloFiltro()
			{
				Id = id,
				ProtocoloId = filtro.Protocolo.IsProcesso ? (int)eTipoProtocolo.Processo : (int)eTipoProtocolo.Documento
			});

			protocolo = _busProtocolo.ObterSimplificado(id);
			vm.TipoHistorico = filtro.Protocolo.IsProcesso ? "Processo" : "documento";
			vm.TipoHistoricoId = tipo;
			vm.AcaoHistoricoMostrarPdf = _bus.ObterHistoricoAcoesMostrarPdf();
			vm.CarregarHistorico(hstTramitacao.Itens, hstProtocolo, protocolo.Numero, protocolo.Tipo.Texto, protocolo: id);

			return PartialView("HistoricoPartial", vm);
		}

		#endregion

		#region Métodos Auxiliares

		//id do histórico ou da tramitacao; Tipo = 1: processo, e 2: documento
		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult GerarPdf(int id, int tipo, bool obterHistorico = false, bool isGerarArquivamento = false)
		{
			try
			{
				if (isGerarArquivamento)
				{
					return GerarPdfArquivamento(id, tipo, obterHistorico);
				}

				return ViewModelHelper.GerarArquivoPdf(_pdf.Gerar(id, tipo, obterHistorico), "Despacho de " + ((tipo == 1) ? "Processo" : "Documento"));

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
			}
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult GerarPdfZip(String id, int tipo = 0, bool obterHistorico = false)
		{
			try
			{
				if (id.Contains(','))
				{
					//id = HttpUtility.HtmlDecode(id);
					List<Arquivo> pdfs = new List<Arquivo>();
					string[] ids = id.Split(',');

					String nomeArquivo = String.Empty;

					for (int i = 0; i < ids.Length; i++)
					{
						Tramitacao tramitacao = new Tramitacao();
						tramitacao.Id = Convert.ToInt32(ids[i]);
						tramitacao = _bus.Obter(tramitacao.Id);

						if (tramitacao.Protocolo.IsProcesso)
						{
							nomeArquivo = "Despacho de Processo " + tramitacao.Protocolo.NumeroProtocolo + "-" + tramitacao.Protocolo.Ano;
						}
						else
						{
							nomeArquivo = "Despacho de Documento " + tramitacao.Protocolo.NumeroProtocolo + "-" + tramitacao.Protocolo.Ano;
						}


						pdfs.Add(new Arquivo() { Nome = nomeArquivo, Extensao = "pdf", Buffer = _pdf.Gerar2(tramitacao.Id, tramitacao.Protocolo.IsProcesso ? 1 : 0, obterHistorico), ContentType = ContentType.PDF });
					}

					return ViewModelHelper.GerarArquivo("PdfsDespacho.zip", new ArquivoZip().Create(pdfs), ContentType.ZIP);
				}
				else
				{
					Tramitacao tramitacao = new Tramitacao();
					tramitacao.Id = Convert.ToInt32(id);
					tramitacao = _bus.Obter(tramitacao.Id);

					String nomeArquivo = "Despacho de " + ((tramitacao.Protocolo.IsProcesso) ? "Processo " : "Documento ") + tramitacao.Protocolo.NumeroProtocolo + "-" + tramitacao.Protocolo.Ano;

					return ViewModelHelper.GerarArquivoPdf(_pdf.Gerar(tramitacao.Id, tramitacao.Protocolo.IsProcesso ? 1 : 0, obterHistorico), nomeArquivo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
			}
		}

		//id do histórico ou da tramitacao; Tipo = 1: processo, e 2: documento, situcao 1 tramitando
		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult GerarPdfArquivamento(int id, int tipo, bool obterHistorico = false)
		{
			try
			{
				PdfTramitacaoArquivamento pdf = new PdfTramitacaoArquivamento();
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, tipo, obterHistorico), "Tramitacao de Arquivamento de " + ((tipo == 1) ? "Processo" : "Documento"));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
			}
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult GerarPdfHistorico(int id, int tipo)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(_pdf.GerarHistorico(id, tipo), "Historico de Tramitacao");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "Home", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Arquivar

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoArquivar })]
		public ActionResult Arquivar()
		{
			ArquivarVM vm = new ArquivarVM();

			vm.Arquivar.Funcionario = new Funcionario { Id = _bus.User.FuncionarioId, Nome = _bus.User.Name };
			List<Setor> setoresFuncionario = _busArquivamento.ObterSetoresFuncionario(vm.Arquivar.Funcionario.Id);
			vm.SetoresOrigem = ViewModelHelper.CriarSelectList(setoresFuncionario);
			vm.PrateleiraModos = ViewModelHelper.CriarSelectList(_busLista.TiposTramitacaoArquivoModo);
			vm.Objetivos = ViewModelHelper.CriarSelectList(_bus.ObterMotivos());
			vm.Arquivar.DataArquivamento.Data = DateTime.Today;

			if (setoresFuncionario.Count == 1)
			{
				vm.Arquivar.SetorId = setoresFuncionario.First().Id;
			}

			return View("Arquivar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoArquivar })]
		public ActionResult Arquivar(ArquivarVM vm)
		{
			vm.Arquivar.DataArquivamento.Data = DateTime.Today;
			vm.Arquivar.Funcionario.Id = _busArquivamento.User.FuncionarioId;

			foreach (Tramitacao tramitacao in vm.Itens)
			{
				tramitacao.Arquivamento = vm.Arquivar;
				tramitacao.Objetivo.Id = vm.Arquivar.ObjetivoId;
				tramitacao.Despacho = vm.Arquivar.Despacho;
				tramitacao.Tipo = (int)eTramitacaoTipo.Normal;
				tramitacao.SituacaoId = (int)eTramitacaoSituacao.Arquivado;
				tramitacao.Executor.Id = _busArquivamento.User.FuncionarioId;
				tramitacao.Remetente.Id = _busArquivamento.User.FuncionarioId;
				tramitacao.RemetenteSetor.Id = vm.Arquivar.SetorId;
			}

			_busArquivamento.Arquivar(vm.Arquivar, vm.Itens);

			return Json(
				new
				{
					EhValido = Validacao.EhValido,
					Msg = Validacao.Erros,
					UrlRedireciona = Url.Action("Index", "Tramitacao") + "?Msg=" + Validacao.QueryParam(),
				}
			);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoArquivar })]
		public ActionResult ArquivarAdicionarItem(int setorId, string numero)
		{
			ArquivarVM vm = new ArquivarVM();
			Tramitacao tramitacao = null;

			Protocolo protocolo = _validar.AdicionarProtocolo(_busArquivamento.User.FuncionarioId, setorId, numero);

			if (Validacao.EhValido && protocolo.Id > 0)
			{
				tramitacao = _busArquivamento.ObterItem(_busArquivamento.User.FuncionarioId, setorId, protocolo);
				JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
				if (tramitacao != null)
				{
					vm.ItensJson.Add(jsSerializer.Serialize(new
					{
						@TramitacaoArquivamento = jsSerializer.Serialize(tramitacao.Arquivamento),
						@TramitacaoObjetivoId = tramitacao.Objetivo.Id,
						@TramitacaoDespacho = tramitacao.Despacho,
						@TramitacaoTipo = tramitacao.Tipo,
						@TramitacaoSituacaoId = tramitacao.SituacaoId,
						@TramitacaoExecutorId = tramitacao.Executor.Id,
						@TramitacaoRemetenteId = tramitacao.Remetente.Id,
						@TramitacaoRemetenteSetorId = tramitacao.RemetenteSetor.Id,
						@TramitacaoProtocoloId = tramitacao.Protocolo.Id
					}));

					vm.Itens.Add(tramitacao);
				}
			}

			return Json(
				new
				{
					Msg = Validacao.Erros,
					EhValido = Validacao.EhValido,
					Tramitacao = tramitacao,
					ItemHtml = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ArquivarItens", vm)
				},
				JsonRequestBehavior.AllowGet
			);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoArquivar })]
		public ActionResult ArquivarObterTodos(int setorId)
		{
			ArquivarVM vm = new ArquivarVM();
			vm.Itens = _bus.FiltrarEmPosse(new ListarTramitacaoFiltro() { EmposseId = _busArquivamento.User.FuncionarioId, EmposseSetorId = setorId }).Itens;

			if (vm.Itens.Count <= 0)
			{
				Validacao.Add(Mensagem.Arquivamento.ArquivarSetorNaoContemProtocolo);
			}

			return Json(
				new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros,
					@ItensHtml = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ArquivarItens", vm),
				},
				JsonRequestBehavior.AllowGet
			);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoArquivar })]
		public ActionResult ArquivarBuscarEstantes(int Id)
		{
			return Json(_busArquivamento.ObterEstantesArquivo(Id), JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoArquivar })]
		public ActionResult ArquivarBuscarPrateleiras(int Id, int estanteId)
		{
			return Json(_busArquivamento.ObterPrateleirasArquivo(estanteId, Id), JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Desarquivar

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoDesarquivar })]
		public ActionResult Desarquivar()
		{
			List<Setor> remetenteSetores = _busArquivamento.ObterSetoresFuncionario(_bus.User.FuncionarioId);
			List<Setor> destinatarioSetores = _busArquivamento.ObterSetoresFuncionario(_bus.User.FuncionarioId);

			List<TramitacaoArquivoLista> arquivosCadastrados = destinatarioSetores.Count == 1 ? _busArquivamento.ObterArquivosCadastrados(destinatarioSetores.First().Id) : new List<TramitacaoArquivoLista>();

			DesarquivarVM vm = new DesarquivarVM(
				_busArquivamento.ObterExecutor(),
				remetenteSetores,
				destinatarioSetores,
				arquivosCadastrados,
				_busLista.AtividadesSolicitada,
				_busLista.AtividadeSolicitadaSituacoes,
				_busLista.Estados,
				_busLista.EstadoDefault,
				_busLista.Municipios(0),
				new List<Lista>(),
				_busLista.TiposTramitacaoArquivoModo,
				new List<Lista>());

			if (destinatarioSetores.Count == 1)
			{
				vm.DestinatarioSetor.Id = destinatarioSetores.First().Id;
			}

			if (arquivosCadastrados.Count <= 0 && destinatarioSetores.Count == 1)
			{
				Validacao.Add(Mensagem.Arquivamento.SetorDestinoSemArquivo);
			}

			return View("Desarquivar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoDesarquivar })]
		public ActionResult Desarquivar(DesarquivarVM vm)
		{
			string urlRedireciona = Url.Action("Index", "Tramitacao");

			if (_busArquivamento.Desarquivar(vm.Arquivo.Id, vm.Tramitacoes, vm.DestinatarioSetor.Id))
			{
				urlRedireciona += "?Msg=" + Validacao.QueryParam();
			}

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlRedireciona = urlRedireciona });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoDesarquivar })]
		public ActionResult DesarquivarAdicionarItem(int setorId, int arquivoId, int arquivoEstanteId, int arquivoPrateleiraModoId, string arquivoIdentificacao, string numero)
		{
			DesarquivarVM vm = new DesarquivarVM();

			ListarTramitacaoFiltro filtros = new ListarTramitacaoFiltro();
			filtros.TramitacaoSituacaoId = (int)eTramitacaoSituacao.Arquivado;
			filtros.Protocolo.NumeroTexto = numero;
			filtros.ArquivoId = arquivoId;
			filtros.RemetenteSetorId = setorId;
			filtros.ArquivoEstanteId = arquivoEstanteId;
			filtros.ArquivoPrateleiraModoId = arquivoPrateleiraModoId;
			filtros.ArquivoIdentificacao = arquivoIdentificacao;

			if (_arquivarValidar.PreDesarquivar(filtros))
			{
				Resultados<Tramitacao> resultados = _bus.Filtrar(filtros);
				if (Validacao.EhValido && resultados != null)
				{
					vm.Tramitacoes = resultados.Itens;
				}
			}
			else
			{
				return Json(new
				{
					@Msg = Validacao.Erros,
					@EhValido = Validacao.EhValido,
				}, JsonRequestBehavior.AllowGet);
			}

			Tramitacao tramitacao = vm.Tramitacoes.FirstOrDefault();

			if (tramitacao == null || tramitacao.Protocolo == null || tramitacao.Protocolo.Id <= 0)
			{
				IProtocolo protocolo = _busProtocolo.Obter(filtros.Protocolo.NumeroTexto);
				if (protocolo.Id.HasValue && protocolo.Id > 0)
				{
					if (_bus.ExisteTramitacao(protocolo.Id.Value, eTramitacaoSituacao.Tramitando))
					{
						Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloEmTramitacao);
					}
					else
					{
						int setorProtocolo = protocolo.SetorId > 0 ? protocolo.SetorId : protocolo.SetorCriacaoId;

						if (setorProtocolo != filtros.RemetenteSetorId)
						{
							Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloEmOutroSetor);
						}
						else
						{
							Validacao.Add(Mensagem.Arquivamento.DesarquivarProtocoloNaoEstaEmArquivoPrateleiraEstanteInformados);
						}
					}
				}
			}


			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Tramitacao = tramitacao,
				@ItemHtml = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DesarquivarItens", vm),
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoDesarquivar })]
		public ActionResult DesarquivarObterItens(ListarTramitacaoFiltro filtros)
		{
			DesarquivarVM vm = new DesarquivarVM();
			filtros.TramitacaoSituacaoId = (int)eTramitacaoSituacao.Arquivado;
			filtros.RegistradorRemetenteSetorId = filtros.DestinatarioSetorId;//O destino é o mesmo que a origem
			filtros.DestinatarioSetorId = 0;
			if (_arquivarValidar.PreDesarquivar(filtros.ArquivoId))
			{
				Resultados<Tramitacao> resultados = _bus.Filtrar(filtros);

				if (Validacao.EhValido && resultados != null)
				{
					vm.Tramitacoes = resultados.Itens;
				}
			}

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@HtmlItens = ViewModelHelper.RenderPartialViewToString(ControllerContext, "DesarquivarItens", vm),
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TramitacaoArquivar, ePermissao.TramitacaoDesarquivar })]
		public ActionResult ObterArquivosCadastradosSetor(int id)
		{
			List<TramitacaoArquivoLista> arquivos = _busArquivamento.ObterArquivosCadastrados(id);
			return Json(arquivos, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}