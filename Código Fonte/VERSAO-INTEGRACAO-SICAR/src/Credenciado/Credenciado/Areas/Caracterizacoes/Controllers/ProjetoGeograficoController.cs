using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class ProjetoGeograficoController : DefaultController
	{
		#region Propriedade

		ProjetoGeograficoBus _bus = new ProjetoGeograficoBus();
		EmpreendimentoCredenciadoBus _busEmpreendimento = new EmpreendimentoCredenciadoBus();
		ProjetoGeograficoValidar _validar = new ProjetoGeograficoValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		#region Index

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		public ActionResult Index(int id, int empreendimento, int projetoDigitalId, int tipo, bool isCadastrarCaracterizacao = true, bool mostrarModalDependencias = true, bool retornarVisualizar = true)
		{
			PermissaoValidar permissaoValidar = new PermissaoValidar();

			if (isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoCriar }, false))
			{
				return Criar(empreendimento, tipo, isCadastrarCaracterizacao);
			}

			if (!isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoEditar }, false))
			{
				return Editar(id, empreendimento, projetoDigitalId, tipo, isCadastrarCaracterizacao, mostrarModalDependencias);
			}

			if (permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoVisualizar }, false))
			{
				return Visualizar(id, empreendimento, projetoDigitalId, tipo, isCadastrarCaracterizacao, mostrarModalDependencias, retornarVisualizar);
			}

			permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar });
			return RedirectToAction("", "Caracterizacao", new { id = id, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
		}

		#endregion

		#region Salvar

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar })]
		public ActionResult Criar(int empreendimento, int tipo, bool isCadastrarCaracterizacao = true, int projetoDigitalId = 0)
		{
			if (empreendimento <= 0)
			{
				return RedirectToAction("Operar", "ProjetoDigital", new { area = "", Id = projetoDigitalId });
			}

			if (!_validar.Dependencias(empreendimento, projetoDigitalId, tipo))
			{
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			ProjetoGeograficoVM vm = new ProjetoGeograficoVM();
			vm.isCadastrarCaracterizacao = isCadastrarCaracterizacao;
			vm.Projeto.EmpreendimentoId = empreendimento;
			vm.Projeto.CaracterizacaoId = tipo;

			//Carregar os dados do projeto geográfico
			return Carregar(vm, projetoDigitalId);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar })]
		public ActionResult CriarParcial(ProjetoGeografico projeto)
		{
			_bus.Salvar(projeto);

			return Json(new { @Id = projeto.Id, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Editar(int id, int empreendimento, int projetoDigitalId, int tipo, bool isCadastrarCaracterizacao = true, bool mostrarModalDependencias = true)
		{
			if (!_validar.Dependencias(empreendimento, projetoDigitalId, tipo))
			{
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			ProjetoGeograficoVM vm = new ProjetoGeograficoVM();
			vm.Projeto = _bus.ObterProjeto(id) ?? new ProjetoGeografico();
			vm.isCadastrarCaracterizacao = isCadastrarCaracterizacao;
			vm.Projeto.EmpreendimentoId = empreendimento;
			vm.Projeto.CaracterizacaoId = tipo;
			vm.IsProcessado = _bus.IsProcessado(vm.Projeto.Id, (eCaracterizacao)tipo);

			Dominialidade dominialidade = null;

			if (vm.IsFinalizado)
			{
				dominialidade = new DominialidadeBus().ObterDadosGeo(empreendimento);
			}
			else
			{
				dominialidade = new DominialidadeBus().ObterDadosGeoTMP(empreendimento);
			}

			vm.PossuiAPPNaoCaracterizada = dominialidade.AreaAPPNaoCaracterizada.MaiorToleranciaM2();
			vm.PossuiARLNaoCaracterizada = dominialidade.ARLNaoCaracterizada.MaiorToleranciaM2();

			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			vm.MostrarAvancar = !projetoDigitalCredenciadoBus.ObterDependencias(projetoDigitalId).Exists(x => x.DependenciaCaracterizacao == tipo);

			//Carregar os dados do projeto geográfico
			return Carregar(vm, projetoDigitalId, mostrarModalDependencias);
		}

		#endregion

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Salvar(ProjetoGeografico projeto)
		{
			if (!_validar.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Operar", "ProjetoDigital", new { area = "", Id = projeto.ProjetoDigitalId }) });
			}

			_bus.Salvar(projeto);
			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		public ActionResult Carregar(ProjetoGeograficoVM vm, int projetoDigitalId, bool mostrarModalDependencias = true, bool isVisualizar = false)
		{
			Empreendimento emp = _busEmpreendimento.Obter(vm.Projeto.EmpreendimentoId);
			if (emp.Coordenada.EastingUtm.GetValueOrDefault() <= 0 || emp.Coordenada.NorthingUtm.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.CoordenadaObrigatoria);
				return RedirectToAction("", "Caracterizacao", new { id = vm.Projeto.EmpreendimentoId, Msg = Validacao.QueryParam() });
			}

			/*if (!isVisualizar && !_caracterizacaoValidar.Dependencias(vm.Projeto.EmpreendimentoId, vm.Projeto.CaracterizacaoId, validarProjetoGeoProprio: false))
			{
				return RedirectToAction("", "Caracterizacao", new { id = vm.Projeto.EmpreendimentoId, Msg = Validacao.QueryParam() });
			}*/

			vm.Projeto.ProjetoDigitalId = projetoDigitalId;
			eCaracterizacao tipo = (eCaracterizacao)vm.Projeto.CaracterizacaoId;

			vm.ArquivoEnviadoTipo = (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado;
			vm.ArquivoEnviadoFilaTipo = (tipo == eCaracterizacao.Dominialidade) ? (int)eFilaTipoGeo.Dominialidade : (int)eFilaTipoGeo.Atividade;

			vm.NiveisPrecisao = ViewModelHelper.CriarSelectList(_bus.ObterNiveisPrecisao());
			vm.SistemaCoordenada = ViewModelHelper.CriarSelectList(_bus.ObterSistemaCoordenada());

			vm.Projeto.EmpreendimentoEasting = emp.Coordenada.EastingUtm.Value;
			vm.Projeto.EmpreendimentoNorthing = emp.Coordenada.NorthingUtm.Value;

			vm.Projeto.CaracterizacaoTexto = (ListaCredenciadoBus.Caracterizacoes.SingleOrDefault(x => x.Id == vm.Projeto.CaracterizacaoId) ?? new CaracterizacaoLst()).Texto;
			vm.Projeto.SistemaCoordenada = (emp.Coordenada.Datum.Texto + " - " + emp.Coordenada.Tipo.Texto + " - Fuso " + ListaCredenciadoBus.Fusos[0].Texto);

			vm.Dependentes = _caracterizacaoBus.Dependentes(vm.Projeto.EmpreendimentoId, projetoDigitalId, tipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			vm.UrlBaixarOrtofoto = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/DownloadArquivoOrtoFoto";
			vm.UrlValidarOrtofoto = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/ValidarChaveArquivoOrtoFoto";

			#region Verificar o Redirecionamento

			vm.UrlAvancar = CaracterizacaoVM.GerarUrl(projetoDigitalId, vm.Projeto.EmpreendimentoId, vm.isCadastrarCaracterizacao, (eCaracterizacao)tipo);

			List<DependenciaLst> dependencias = _caracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);

			/*if (dependencias.Exists(x => x.DependenteTipo == (int)tipo && x.TipoDetentorId == (int)eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade))
			{
				String url = "Visualizar";
				PermissaoValidar permissaoValidar = new PermissaoValidar();

				if (permissaoValidar.ValidarAny(new[] { ePermissao.DescricaoLicenciamentoAtividadeCriar, ePermissao.DescricaoLicenciamentoAtividadeEditar }, false))
				{
					url = "Criar";
				}

				vm.UrlAvancar = Url.Action(url, "DescricaoLicenciamentoAtividade", new { empreendimento = vm.Projeto.EmpreendimentoId, tipo = (int)tipo, isCadastrarCaracterizacao = vm.isCadastrarCaracterizacao });
			}*/

			#endregion

			if (vm.Projeto.Dependencias == null || vm.Projeto.Dependencias.Count == 0)
			{
				vm.Projeto.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(vm.Projeto.EmpreendimentoId, tipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
			}

			if (vm.Projeto.Id > 0 && mostrarModalDependencias)
			{
				vm.TextoMerge = _caracterizacaoValidar.DependenciasAlteradas(
						vm.Projeto.EmpreendimentoId,
						vm.Projeto.CaracterizacaoId,
						eCaracterizacaoDependenciaTipo.ProjetoGeografico,
						vm.Projeto.Dependencias, isVisualizar);
			}

			vm.Projeto.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(vm.Projeto.EmpreendimentoId, tipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);

			//Busca as dependencias desse projeto geográfico
			_bus.ObterDependencias(vm.Projeto, true);

			if (vm.Projeto.Id > 0)
			{
				vm.CarregarVMs();
			}

			return View("ProjetoGeografico", vm);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoVisualizar })]
		public ActionResult Visualizar(int id, int empreendimento, int projetoDigitalId, int tipo, bool isCadastrarCaracterizacao = true, bool mostrarModalDependencias = true, bool retornarVisualizar = true)
		{
			ProjetoGeograficoVM vm = new ProjetoGeograficoVM();
			vm.isCadastrarCaracterizacao = isCadastrarCaracterizacao;

			ProjetoDigitalCredenciadoBus projetoDigitalBus = new ProjetoDigitalCredenciadoBus();
			List<Dependencia> dependenciasPD = projetoDigitalBus.ObterDependencias(projetoDigitalId);

			Dependencia dependencia = dependenciasPD.SingleOrDefault(x => x.DependenciaCaracterizacao == tipo && x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico) ?? new Dependencia();
			vm.Projeto = _bus.ObterHistorico(dependencia.DependenciaId, dependencia.DependenciaTid);

			vm.IsVisualizar = true;
			vm.Desenhador.IsVisualizar = true;
			vm.Sobreposicoes.MostrarVerificar = false;
			vm.BaseReferencia.IsVisualizar = true;
			vm.Importador.IsVisualizar = true;
			vm.IsProcessado = _bus.IsProcessado(vm.Projeto.Id, (eCaracterizacao)tipo);

			Boolean podeCriarEditar = new PermissaoValidar().ValidarAny(new[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar }, false);
			if (!String.IsNullOrWhiteSpace(vm.TextoMerge) && !podeCriarEditar)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.NaoPodeVisualizarComDependenciasAlteradas(vm.Projeto.CaracterizacaoTexto));
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, projetoDigitalId = projetoDigitalId, Msg = Validacao.QueryParam() });
			}

			Dominialidade dominialidade = null;

			if (vm.IsFinalizado)
			{
				dominialidade = new DominialidadeBus().ObterDadosGeo(empreendimento);
			}
			else
			{
				dominialidade = new DominialidadeBus().ObterDadosGeoTMP(empreendimento);
			}

			vm.PossuiAPPNaoCaracterizada = dominialidade.AreaAPPNaoCaracterizada.MaiorToleranciaM2();
			vm.PossuiARLNaoCaracterizada = dominialidade.ARLNaoCaracterizada.MaiorToleranciaM2();

			vm.MostrarAvancar = !dependenciasPD.Exists(x => x.DependenciaCaracterizacao == tipo);
			vm.RetornarVisualizar = retornarVisualizar;

			//Carregar os dados do projeto geográfico
			return Carregar(vm, projetoDigitalId, mostrarModalDependencias, isVisualizar: true);
		}

		#endregion

		#region Excluir Rascunho

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ExcluirRascunho(ProjetoGeografico projeto, bool isCadastrarCaracterizacao)
		{
			if (!_validar.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Operar", "ProjetoDigital", new { area = "", Id = projeto.ProjetoDigitalId }) });
			}

			_bus.ExcluirRascunho(projeto);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, Url = Url.Action("Index", "Caracterizacao", Validacao.QueryParamSerializer(new { id = projeto.EmpreendimentoId, projetoDigitalId = projeto.ProjetoDigitalId })) });
		}

		#endregion

		#region Finalizar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Finalizar(ProjetoGeografico projeto, string url)
		{
			if (!_validar.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Operar", "ProjetoDigital", new { area = "", Id = projeto.ProjetoDigitalId }) });
			}

			if (!projeto.EmpreendimentoEstaDentroAreaAbrangencia)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoForaAbrangencia);
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
			}

			//Valida as dependências do projeto geográfico
			string mensagem = _caracterizacaoValidar.DependenciasAlteradas(projeto.EmpreendimentoId, projeto.CaracterizacaoId,
				eCaracterizacaoDependenciaTipo.ProjetoGeografico, projeto.Dependencias);

			if (!String.IsNullOrWhiteSpace(mensagem))
			{
				return Json(new { @Mensagem = mensagem, Msg = Validacao.Erros });
			}

			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			_bus.Finalizar(projeto, projeto.ProjetoDigitalId, projetoDigitalCredenciadoBus.AlterarCaracterizacao);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, Url = url });
		}

		#endregion

		#region Merge

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ObterMerge(int id)
		{
			ProjetoGeografico projetoGeo = _bus.ObterProjeto(id);

			//Busca as dependencias desse projeto geográfico
			_bus.ObterDependencias(projetoGeo);
			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido, @Projeto = projetoGeo });
		}

		#endregion

		#region Arquivo/ Ortofoto

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult SalvarOrtofoto(ProjetoGeografico projeto)
		{
			_bus.SalvarOrtofoto(projeto);

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ObterOrtoFotoMosaico(int projetoId)
		{
			List<ArquivoProcessamentoVM> listaVM = new List<ArquivoProcessamentoVM>();

			foreach (ArquivoProjeto item in _bus.ObterOrtofotos(projetoId))
			{
				listaVM.Add(new ArquivoProcessamentoVM(item));
			}

			return Json(new { @Lista = listaVM, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		public FileResult BaixarArquivoOrtofoto(int id, int situacao)
		{
			try
			{
				ProjetoGeograficoBus _bus = new ProjetoGeograficoBus();
				bool finalizado = situacao == (int)eProjetoGeograficoSituacao.Finalizado;

				Arquivo arquivo = _bus.ArquivoOrtofoto(id, finalizado, false);
				if (arquivo != null && arquivo.Id > 0)
				{
					return ViewModelHelper.GerarArquivo(arquivo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		public FilePathResult BaixarArquivoModelo()
		{
			try
			{
				FilePathResult file = new FilePathResult("~/Content/_zipModeloGeo/ModeloShape.zip", ContentType.ZIP);
				file.FileDownloadName = "ModeloShape.zip";
				return file;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		#endregion

		#region Refazer

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Refazer(ProjetoGeografico projeto, bool isCadastrarCaracterizacao)
		{
			if (!_validar.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Operar", "ProjetoDigital", new { area = "", Id = projeto.ProjetoDigitalId }) });
			}

			_bus.Refazer(projeto);

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				Url = Url.Action("Editar", "ProjetoGeografico", Validacao.QueryParamSerializer(new
				{
					id = projeto.Id,
					empreendimento = projeto.EmpreendimentoId,
					tipo = projeto.CaracterizacaoId,
					isCadastrarCaracterizacao = isCadastrarCaracterizacao,
					mostrarModalDependencias = false,
					projetoDigitalId = projeto.ProjetoDigitalId

				}))
			});
		}

		#endregion

		#region Recarregar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Recarregar(ProjetoGeografico projeto, bool isCadastrarCaracterizacao)
		{
			if (!_validar.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Operar", "ProjetoDigital", new { area = "", Id = projeto.ProjetoDigitalId }) });
			}

			_bus.Recarregar(projeto);

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				Url = Url.Action("Editar", "ProjetoGeografico", Validacao.QueryParamSerializer(
					new
					{
						id = projeto.Id,
						empreendimento = projeto.EmpreendimentoId,
						tipo = projeto.CaracterizacaoId,
						isCadastrarCaracterizacao = isCadastrarCaracterizacao,
						projetoDigitalId = projeto.ProjetoDigitalId
					}))
			});
		}

		#endregion

		#region Area de Abrangencia

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult AlterarAreaAbrangencia(ProjetoGeografico projeto)
		{
			_bus.AlterarAreaAbrangencia(projeto);

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#endregion

		#region Processamento

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult EnviarArquivo(ArquivoProjeto arquivo)
		{
			_bus.EnviarArquivo(arquivo);

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, @Arquivo = arquivo });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		public ActionResult ObterArquivos(int projetoGeograficoId, bool isFinalizado = false)
		{
			List<ArquivoProjeto> arquivos = _bus.ObterArquivos(projetoGeograficoId, isFinalizado).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado ||
																								   x.Tipo == (int)eProjetoGeograficoArquivoTipo.RelatorioImportacao ||
																								   x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoProcessadoTrackMaker ||
																								   x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui ||
																								   x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoProcessadoSoftwareGIS).ToList();

			arquivos.ForEach(x =>
			{
				if (x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui)
				{
					x.Nome = "Croqui da Dominialidade (PDF)";
				}
			});

			return Json(new
			{
				@Arquivos = arquivos,
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		public ActionResult ObterSituacao(ProcessamentoGeo processamentoGeo)
		{
			_bus.ObterSituacaoFila(processamentoGeo);

			return Json(new
			{
				@ProcessamentoGeo = processamentoGeo,
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Processar(ProcessamentoGeo processamentoGeo)
		{
			_bus.Processar(processamentoGeo);

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, @ProcessamentoGeo = processamentoGeo });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult CancelarProcessamento(ProcessamentoGeo processamentoGeo)
		{
			_bus.CancelarProcessamento(processamentoGeo);

			return Json(new { @ProcessamentoGeo = processamentoGeo, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		#endregion

		#region Verificacoes

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult VerificarSobreposicao(int projetoId, int tipo)
		{
			Sobreposicoes sobreposicoes = _bus.ObterGeoSobreposiacao(projetoId, (eCaracterizacao)tipo);
			_bus.SalvarSobreposicoes(new ProjetoGeografico() { Id = projetoId, Sobreposicoes = sobreposicoes });

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, Sobreposicoes = sobreposicoes });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoEditar })]
		public ActionResult VerificarAreaNaoCaracterizada(int empreendimento, int projeto)
		{
			Dominialidade dominialidade = new DominialidadeBus().ObterDadosGeoTMP(empreendimento);
			return Json(new
			{
				Msg = Validacao.Erros,
				PossuiAPPNaoCaracterizada = dominialidade.AreaAPPNaoCaracterizada.MaiorToleranciaM2(),
				PossuiARLNaoCaracterizada = dominialidade.ARLNaoCaracterizada.MaiorToleranciaM2(),
				IsProcessado = _bus.IsProcessado(projeto, eCaracterizacao.Dominialidade)
			});
		}

		#endregion
	}
}