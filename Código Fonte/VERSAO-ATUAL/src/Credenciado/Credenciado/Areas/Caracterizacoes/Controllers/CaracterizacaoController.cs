using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class CaracterizacaoController : DefaultController
	{
		#region Propriedades
		BarragemDispensaLicencaBus _busB = new BarragemDispensaLicencaBus();
		CaracterizacaoBus _bus = new CaracterizacaoBus(new CaracterizacaoValidar());
		CaracterizacaoInternoBus _internoBus = new CaracterizacaoInternoBus();
		CaracterizacaoValidar _validar = new CaracterizacaoValidar();

		#endregion

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult Index(int id, int projetoDigitalId, bool visualizar = false)
		{
			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			if (id == 0)
			{
				if (!visualizar)
				{
					ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.AlterarEtapa(projetoDigitalId, eProjetoDigitalEtapa.Envio);
				}
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			EmpreendimentoCaracterizacao empreendimento = _bus.ObterEmpreendimentoSimplificado(id);
			CaracterizacaoVM vm = new CaracterizacaoVM(empreendimento);

			if (empreendimento.InternoID > 0)
			{
				Empreendimento empreendimentoInterno = new EmpreendimentoInternoBus().ObterSimplificado(empreendimento.InternoID);

				if (empreendimento.InternoTID != empreendimentoInterno.Tid)
				{
					vm.MensagensNotificacoes.Add(Mensagem.Caracterizacao.EmpreendimentoDesatualizado(empreendimentoInterno.Denominador).Texto);
				}
			}

			if (empreendimento.Id == 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoNaoEncontrado);
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			vm.ProjetoDigitalId = projetoDigitalId;
			vm.IsVisualizar = visualizar;

			List<CaracterizacaoLst> caracterizacoes = _bus.ObterCaracterizacoesPorProjetoDigital(projetoDigitalId);
			
			if (caracterizacoes.All(carac => carac.Permissao == ePermissaoTipo.NaoPermitido))
			{
				Validacao.Add(Mensagem.Caracterizacao.UsuarioNaoPermitidoParaCaracterizacao);
			}

			caracterizacoes.RemoveAll(caract => caract.Permissao == ePermissaoTipo.NaoPermitido);

			#region Projeto Digital

			if (caracterizacoes == null || caracterizacoes.Count <= 0)
			{
				ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.AlterarEtapa(projetoDigitalId, eProjetoDigitalEtapa.Envio);

				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}

			ProjetoDigitalCredenciadoValidar projetoDigitalCredenciadoValidar = new ProjetoDigitalCredenciadoValidar();
			if (!projetoDigitalCredenciadoValidar.PossuiCaracterizacaoAtividade(caracterizacoes, projetoDigitalId))
			{
				if (!visualizar)
				{
					ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.AlterarEtapa(projetoDigitalId, eProjetoDigitalEtapa.Envio);
				}
				return RedirectToAction("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalId, area = "" }));
			}
			else
			{
				vm.MostrarFinalizar = (projetoDigitalCredenciadoBus.Obter(projetoDigitalId).Etapa == (int)eProjetoDigitalEtapa.Caracterizacao) && !visualizar;
			}

			#endregion

			List<Caracterizacao> caracterizacoesAssociadas = _bus.ObterCaracterizacoesAssociadasProjetoDigital(projetoDigitalId);
			List<Caracterizacao> cadastradas = _bus.ObterCaracterizacoesEmpreendimento(id, projetoDigitalId) ?? new List<Caracterizacao>();
			List<Caracterizacao> cadastradasInterno = _internoBus.ObterCaracterizacoesAtuais(empreendimento.InternoID, caracterizacoes);

			if (cadastradas != null && cadastradas.Count > 0)
			{
				List<int> desatualizadas = new List<int>();
				string msgDesatualizadas = _validar.CaracterizacoesCadastradasDesatualizadas(empreendimento.Id, cadastradas, cadastradasInterno, out desatualizadas);
				if (!string.IsNullOrEmpty(msgDesatualizadas))
				{
					vm.MensagensNotificacoes.Add(msgDesatualizadas);
				}
				vm.CaracterizacoesPossivelCopiar = _bus.ObterPossivelCopiar(cadastradas);
				vm.CaracterizacoesPossivelCopiar.AddRange(desatualizadas.Where(x => !vm.CaracterizacoesPossivelCopiar.Exists(y => y == x)));
			}

			vm.CaracterizacoesNaoCadastradas = caracterizacoes
							.Where(x => !cadastradas.Any(y => y.Id > 0 && (int)y.Tipo == x.Id))
							.Select(z => new CaracterizacaoVME() { Tipo = (eCaracterizacao)z.Id, Nome = z.Texto }).ToList();

			vm.CaracterizacoesCadastradas = caracterizacoes
							.Where(x => cadastradas.Where(y => !caracterizacoesAssociadas.Exists(z => z.Tipo == y.Tipo)).Any(y => y.Id > 0 && (int)y.Tipo == x.Id))
							.Select(z => new CaracterizacaoVME() { Tipo = (eCaracterizacao)z.Id, Nome = z.Texto }).ToList();

			vm.CaracterizacoesAssociadas = caracterizacoesAssociadas
							.Select(z => new CaracterizacaoVME() { Tipo = (eCaracterizacao)z.Tipo, Nome = z.Nome }).ToList();

			List<DependenciaLst> dependencias = _bus.CaracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);

			#region Não Cadastradas

			vm.CaracterizacoesNaoCadastradas.ForEach(x =>
			{
				ICaracterizacaoBus caracterizacaoBus = CaracterizacaoBusFactory.Criar(x.Tipo);
				if (caracterizacaoBus != null)
				{
					x.PodeCopiar = cadastradasInterno.Exists(y => y.Tipo == x.Tipo) && caracterizacaoBus.PodeCopiar(empreendimento.InternoID);
				}

				x.PodeCadastrar = User.IsInRole(String.Format("{0}Criar", x.Tipo.ToString()));
				x.ProjetoGeografico = User.IsInRole("ProjetoGeograficoCriar");
				x.ProjetoGeoObrigatorio = dependencias.Exists(y =>
					y.DependenteTipo == (int)x.Tipo &&
					y.DependenciaTipo == (int)x.Tipo &&
					y.TipoId == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico);

				x.ProjetoGeograficoId = (cadastradas.FirstOrDefault(y => y.Tipo == x.Tipo) ?? new Caracterizacao()).ProjetoRascunhoId;

				if (x.ProjetoGeograficoId <= 0)
				{
					x.ProjetoGeograficoId = (cadastradas.SingleOrDefault(y => y.Tipo == x.Tipo) ?? new Caracterizacao()).ProjetoId;
				}

				if (x.Tipo == eCaracterizacao.BarragemDispensaLicenca)
				{
					x.UrlListar = Url.Action("Listar", x.Tipo.ToString());
					x.UrlCriar = Url.Action("Listar", x.Tipo.ToString());

				}
				else
				{
					x.UrlCriar = Url.Action("Criar", x.Tipo.ToString());
				}
			});

			#endregion

			#region Cadastradas

			vm.CaracterizacoesCadastradas.ForEach(x =>
			{
				if (User.IsInRole("ProjetoGeograficoEditar") || User.IsInRole("ProjetoGeograficoVisualizar"))
				{
					x.ProjetoGeografico = true;
				}

				x.ProjetoGeograficoVisualizar = visualizar;
				x.PodeCadastrar = User.IsInRole(String.Format("{0}Listar", x.Tipo.ToString()));
				x.PodeEditar = User.IsInRole(String.Format("{0}Editar", x.Tipo.ToString()));
				x.PodeVisualizar = User.IsInRole(String.Format("{0}Visualizar", x.Tipo.ToString()));
				x.PodeExcluir = (User.IsInRole(String.Format("{0}Excluir", x.Tipo.ToString())) && (x.Tipo == eCaracterizacao.UnidadeConsolidacao || x.Tipo == eCaracterizacao.UnidadeProducao));

				x.PodeCopiar = cadastradasInterno.Exists(y => y.Tipo == x.Tipo);
				x.PodeAssociar = !caracterizacoesAssociadas.Exists(y => y.Tipo == x.Tipo);

				x.ProjetoGeograficoId = cadastradas.SingleOrDefault(y => y.Tipo == x.Tipo).ProjetoId;

				x.UrlEditar = Url.Action("Editar", x.Tipo.ToString());
				x.UrlVisualizar = Url.Action("Visualizar", x.Tipo.ToString());
				x.UrlExcluirConfirm = Url.Action("ExcluirConfirm", x.Tipo.ToString());
				x.UrlExcluir = Url.Action("Excluir", x.Tipo.ToString());

				if (x.Tipo == eCaracterizacao.BarragemDispensaLicenca)
				{
					x.UrlListar = Url.Action("Listar", x.Tipo.ToString());
				}

			});

			#endregion

			#region Associadas

			vm.CaracterizacoesAssociadas.ForEach(x =>
			{
				if (User.IsInRole("ProjetoGeograficoVisualizar"))
				{
					x.ProjetoGeografico = true;
					x.ProjetoGeograficoVisualizar = true;
					x.ProjetoGeograficoId = cadastradas.SingleOrDefault(y => y.Tipo == x.Tipo).ProjetoId;
				}

				x.PodeVisualizar = User.IsInRole(String.Format("{0}Visualizar", x.Tipo.ToString()));
				x.UrlVisualizar = Url.Action("Visualizar", x.Tipo.ToString());

				if (x.Tipo == eCaracterizacao.BarragemDispensaLicenca)
				{
					x.UrlListar = Url.Action("Listar", x.Tipo.ToString());
				}
			});

			#endregion

			return View("Caracterizacao", vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AssociarCaracterizacaoProjetoDigital(ProjetoDigital projetoDigital)
		{
			ProjetoDigitalCredenciadoBus projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			projetoDigitalBus.AssociarDependencias(projetoDigital);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = projetoDigital.EmpreendimentoId, projetoDigitalId = projetoDigital.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult DesassociarCaracterizacaoProjetoDigital(ProjetoDigital projetoDigital)
		{
			ProjetoDigitalCredenciadoBus projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			projetoDigitalBus.DesassociarDependencias(projetoDigital);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = projetoDigital.EmpreendimentoId, projetoDigitalId = projetoDigital.Id, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult CopiarDadosInstitucional(int id, int projetoDigitalID, int caracterizacaoTipo)
		{
			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();

			_bus.CopiarDadosInstitucional(id, (eCaracterizacao)caracterizacaoTipo, projetoDigitalID, projetoDigitalCredenciadoBus.AlterarCaracterizacao);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalID, Msg = Validacao.QueryParam() })
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult FinalizarPasso(int id, int projetoDigitalID)
		{
			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			projetoDigitalCredenciadoBus.FinalizarPassoCaracterizacao(id, projetoDigitalID);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = Url.Action("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { id = projetoDigitalID, area = "" }))
			}, JsonRequestBehavior.AllowGet);
		}
	}
}