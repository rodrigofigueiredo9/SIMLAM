using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Cred = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class ProjetoGeograficoController : DefaultController
	{
		#region Propriedade

		ListaBus _listaBus = new ListaBus();
		ProjetoGeograficoBus _bus = new ProjetoGeograficoBus();

		EmpreendimentoBus _busEmpreendimento = new EmpreendimentoBus();
		ProjetoGeograficoValidar _validar = new ProjetoGeograficoValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		ExploracaoFlorestalBus _exploracaoFlorestalBus = new ExploracaoFlorestalBus();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		public ActionResult Index(int id, int empreendimento, int tipo, bool isCadastrarCaracterizacao = true, bool mostrarModalDependencias = true)
		{
			PermissaoValidar permissaoValidar = new PermissaoValidar();

			if (isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoCriar }, false))
			{
				return Criar(empreendimento, tipo, isCadastrarCaracterizacao);
			}

			if (!isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoEditar }, false))
			{
				return Editar(id, empreendimento, tipo, isCadastrarCaracterizacao, mostrarModalDependencias);
			}

			if (permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoVisualizar }, false))
			{
				return Visualizar(id, empreendimento, tipo, isCadastrarCaracterizacao, mostrarModalDependencias);
			}

			permissaoValidar.ValidarAny(new[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar });
			return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
		}

		#region Projeto Greográfico

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Carregar(ProjetoGeograficoVM vm, bool mostrarModalDependencias = true, bool isVisualizar = false)
		{
			Empreendimento emp = _busEmpreendimento.Obter(vm.Projeto.EmpreendimentoId);
			if(emp.Coordenada.EastingUtm.GetValueOrDefault() <= 0 ||emp.Coordenada.NorthingUtm.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.CoordenadaObrigatoria);
				return RedirectToAction("", "Caracterizacao", new { id = vm.Projeto.EmpreendimentoId, Msg = Validacao.QueryParam() });
			}

			if (!isVisualizar && !_caracterizacaoValidar.Dependencias(vm.Projeto.EmpreendimentoId, vm.Projeto.CaracterizacaoId, validarProjetoGeoProprio: false))
			{
				return RedirectToAction("", "Caracterizacao", new { id = vm.Projeto.EmpreendimentoId, Msg = Validacao.QueryParam() });
			}

			eCaracterizacao tipo = (eCaracterizacao)vm.Projeto.CaracterizacaoId;

			vm.CaracterizacaoTipo = vm.Projeto.CaracterizacaoId;
			vm.ArquivoEnviadoTipo = (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado;
			vm.ArquivoEnviadoFilaTipo = (tipo == eCaracterizacao.Dominialidade) ? (int)eFilaTipoGeo.Dominialidade : (int)eFilaTipoGeo.Atividade;

			vm.NiveisPrecisao = ViewModelHelper.CriarSelectList(_bus.ObterNiveisPrecisao());
			vm.SistemaCoordenada = ViewModelHelper.CriarSelectList(_bus.ObterSistemaCoordenada());

			vm.Projeto.EmpreendimentoEasting = emp.Coordenada.EastingUtm.Value;
			vm.Projeto.EmpreendimentoNorthing = emp.Coordenada.NorthingUtm.Value;

			vm.Projeto.CaracterizacaoTexto = (_listaBus.Caracterizacoes.SingleOrDefault(x => x.Id == vm.Projeto.CaracterizacaoId) ?? new CaracterizacaoLst()).Texto;
			vm.Projeto.SistemaCoordenada = ConcatenarSistemaCoordenada(emp);

			vm.Dependentes = _caracterizacaoBus.Dependentes(vm.Projeto.EmpreendimentoId, tipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			vm.UrlBaixarOrtofoto = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/DownloadArquivoOrtoFoto";
			vm.UrlValidarOrtofoto = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/ValidarChaveArquivoOrtoFoto";

			vm.UrlsArquivo = ViewModelHelper.Json(ObterUrlsArquivo());

			#region Verificar o Redirecionamento

			if (tipo == eCaracterizacao.ExploracaoFlorestal && !vm.isCadastrarCaracterizacao)
				vm.isCadastrarCaracterizacao = _exploracaoFlorestalBus.ExisteExploracaoGeoNaoCadastrada(vm.Projeto.Id);
			vm.UrlAvancar = CaracterizacaoVM.GerarUrl(vm.Projeto.EmpreendimentoId, vm.isCadastrarCaracterizacao, (eCaracterizacao)tipo);

			List<DependenciaLst> dependencias = _caracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);

			if (dependencias.Exists(x => x.DependenteTipo == (int)tipo && x.TipoDetentorId == (int)eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade))
			{
				String url = "Visualizar";
				PermissaoValidar permissaoValidar = new PermissaoValidar();

				if (permissaoValidar.ValidarAny(new[] { ePermissao.DescricaoLicenciamentoAtividadeCriar, ePermissao.DescricaoLicenciamentoAtividadeEditar }, false))
				{
					url = "Criar";
				}

				vm.UrlAvancar = Url.Action(url, "DescricaoLicenciamentoAtividade", new { empreendimento = vm.Projeto.EmpreendimentoId, tipo = (int)tipo, isCadastrarCaracterizacao = vm.isCadastrarCaracterizacao });
			}

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
				var exploracao = _exploracaoFlorestalBus.ObterPorEmpreendimento(vm.Projeto.EmpreendimentoId, simplificado: true);
				if (exploracao.Id > 0)
					vm.ExibirBotaoNovo = false;
				else
					vm.ExibirBotaoNovo = vm.IsFinalizado && !vm.IsVisualizar && vm.CaracterizacaoTipo == (int)eCaracterizacao.ExploracaoFlorestal;
			}

			return View("ProjetoGeografico", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar })]
		public ActionResult Criar(int empreendimento, int tipo, bool isCadastrarCaracterizacao = true)
		{
			if (empreendimento <= 0 || empreendimento <= 0)
			{
				return RedirectToAction("Index", "../Empreendimento");
			}

			if (!_validar.Dependencias(empreendimento, tipo))
			{
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
			}

			ProjetoGeograficoVM vm = new ProjetoGeograficoVM();
			vm.isCadastrarCaracterizacao = isCadastrarCaracterizacao;
			vm.Projeto.EmpreendimentoId = empreendimento;
			vm.Projeto.CaracterizacaoId = tipo;
            vm.UrlVoltar = Url.Action("../Caracterizacao/Index", new { id = empreendimento });

			//Carregar os dados do projeto geográfico
			return Carregar(vm);

		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoEditar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarprojetogeografico, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public ActionResult Editar(int id, int empreendimento, int tipo, bool isCadastrarCaracterizacao = true, bool mostrarModalDependencias = true)
		{
			if (!_validar.Dependencias(empreendimento, tipo))
			{
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
			}

			ProjetoGeograficoVM vm = new ProjetoGeograficoVM();
			vm.isCadastrarCaracterizacao = isCadastrarCaracterizacao;
			vm.IsCredenciado = false;
            vm.UrlVoltar = Url.Action("../Caracterizacao/Index", new { id = empreendimento });

			vm.Projeto = _bus.ObterProjeto(id);

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

			//Carregar os dados do projeto geográfico
			return Carregar(vm, mostrarModalDependencias);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarprojetogeografico, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public ActionResult Visualizar(int id, int empreendimento, int tipo, bool isCadastrarCaracterizacao = true, bool mostrarModalDependencias = true)
		{
			ProjetoGeograficoVM vm = new ProjetoGeograficoVM();
			vm.isCadastrarCaracterizacao = isCadastrarCaracterizacao;
			vm.Projeto = _bus.ObterProjeto(id);

			vm.IsVisualizar = true;
			vm.Desenhador.IsVisualizar = true;
			vm.Sobreposicoes.MostarVerificar = false;
			vm.BaseReferencia.IsVisualizar = true;
			vm.EnviarProjeto.IsVisualizar = true;
			vm.IsCredenciado = false;
            vm.UrlVoltar = Url.Action("../Caracterizacao/Index", new { id = empreendimento });

			Boolean podeCriarEditar = new PermissaoValidar().ValidarAny(new[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar }, false);
			if (!String.IsNullOrWhiteSpace(vm.TextoMerge) && !podeCriarEditar)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.NaoPodeVisualizarComDependenciasAlteradas(vm.Projeto.CaracterizacaoTexto));
				return RedirectToAction("", "Caracterizacao", new { id = vm.Projeto.EmpreendimentoId, Msg = Validacao.QueryParam() });
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

			//Carregar os dados do projeto geográfico
			return Carregar(vm, mostrarModalDependencias, isVisualizar: true);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoVisualizar })]
		public ActionResult VisualizarProjetoGeoCredenciado(int projetoDigitalId, int dependenciaTipo, int protocoloId, int requerimentoId)
		{
			ProjetoGeograficoVM vm = new ProjetoGeograficoVM() { ProtocoloId = protocoloId, RequerimentoId = requerimentoId, IsCredenciado = true };

			Cred.ModuloProjetoGeografico.Bussiness.ProjetoGeograficoBus projetoGeoCredBus = new Cred.ModuloProjetoGeografico.Bussiness.ProjetoGeograficoBus();
			Cred.ModuloDominialidade.Business.DominialidadeBus dominialidadeCredBus = new Cred.ModuloDominialidade.Business.DominialidadeBus();
			Cred.ModuloCaracterizacao.Bussiness.CaracterizacaoBus caracterizacaoCredBus = new Cred.ModuloCaracterizacao.Bussiness.CaracterizacaoBus();
			ProjetoDigitalCredenciadoBus busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();


			List<Dependencia> lstDependencias = busProjetoDigitalCredenciado.ObterDependencias(projetoDigitalId);
			Dependencia dependencia = lstDependencias.SingleOrDefault(x => x.DependenciaCaracterizacao == dependenciaTipo && x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico) ?? new Dependencia();

			vm.Projeto = projetoGeoCredBus.ObterHistorico(dependencia.DependenciaId, dependencia.DependenciaTid);
			vm.IsVisualizar = true;
			vm.IsVisualizarCredenciado = true;
			vm.Desenhador.Mostrar = false;
			vm.Sobreposicoes.MostarVerificar = false;
			vm.BaseReferencia.IsVisualizar = true;
			vm.EnviarProjeto.IsVisualizar = true;
			vm.IsProcessado = projetoGeoCredBus.IsProcessado(vm.Projeto.Id, (eCaracterizacao)dependenciaTipo);

			Dominialidade dominialidade = dominialidadeCredBus.ObterDadosGeo(vm.Projeto.EmpreendimentoId);

			vm.PossuiAPPNaoCaracterizada = dominialidade.AreaAPPNaoCaracterizada.MaiorToleranciaM2();
			vm.PossuiARLNaoCaracterizada = dominialidade.ARLNaoCaracterizada.MaiorToleranciaM2();

			//Carregar os dados do projeto geográfico
			eCaracterizacao tipo = (eCaracterizacao)vm.Projeto.CaracterizacaoId;

			vm.ArquivoEnviadoTipo = (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado;
			vm.ArquivoEnviadoFilaTipo = (tipo == eCaracterizacao.Dominialidade) ? (int)eFilaTipoGeo.Dominialidade : (int)eFilaTipoGeo.Atividade;

			vm.NiveisPrecisao = ViewModelHelper.CriarSelectList(_bus.ObterNiveisPrecisao());
			vm.SistemaCoordenada = ViewModelHelper.CriarSelectList(_bus.ObterSistemaCoordenada());

			Empreendimento emp = new EmpreendimentoCredenciadoBus().Obter(vm.Projeto.EmpreendimentoId);
			vm.Projeto.EmpreendimentoEasting = emp.Coordenada.EastingUtm.Value;
			vm.Projeto.EmpreendimentoNorthing = emp.Coordenada.NorthingUtm.Value;

			vm.Projeto.CaracterizacaoTexto = (_listaBus.Caracterizacoes.SingleOrDefault(x => x.Id == vm.Projeto.CaracterizacaoId) ?? new CaracterizacaoLst()).Texto;
			vm.Projeto.SistemaCoordenada = ConcatenarSistemaCoordenada(emp);

			vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;

			vm.UrlBaixarOrtofoto = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/DownloadArquivoOrtoFoto";
			vm.UrlValidarOrtofoto = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/ValidarChaveArquivoOrtoFoto";
			vm.UrlsArquivo = ViewModelHelper.Json(ObterUrlsArquivo());
            vm.UrlVoltar = Url.Action("Analisar", "AnaliseItens", new { protocoloId = protocoloId, requerimentoId = requerimentoId });
			#region Verificar o Redirecionamento

            vm.UrlAvancar = Url.Action("VisualizarCredenciado", "Dominialidade", new { projetoDigitalId = projetoDigitalId, protocoloId = protocoloId });

			List<DependenciaLst> dependencias = _caracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);

			#endregion

			if (vm.Projeto.Dependencias == null || vm.Projeto.Dependencias.Count == 0)
			{
				vm.Projeto.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(vm.Projeto.EmpreendimentoId, tipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
			}

			if (vm.Projeto.Id > 0)
			{
				vm.TextoMerge = _caracterizacaoValidar.DependenciasAlteradas(
						vm.Projeto.EmpreendimentoId,
						vm.Projeto.CaracterizacaoId,
						eCaracterizacaoDependenciaTipo.ProjetoGeografico,
						vm.Projeto.Dependencias, true);
			}

			vm.Projeto.Dependencias = caracterizacaoCredBus.ObterDependenciasAtual(vm.Projeto.EmpreendimentoId, tipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);

			//Busca as dependencias desse projeto geográfico
			projetoGeoCredBus.ObterDependencias(vm.Projeto, true);

			if (vm.Projeto.Id > 0)
			{
				vm.CarregarVMs();
			}

			return View("ProjetoGeografico", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ObterMerge(int id)
		{
			ProjetoGeografico projetoGeo = _bus.ObterProjeto(id);
			//Busca as dependencias desse projeto geográfico
			_bus.ObterDependencias(projetoGeo);
			return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido, @Projeto = projetoGeo });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar })]
		public ActionResult CriarParcial(ProjetoGeografico projeto)
		{
			_bus.Salvar(projeto);

			return Json(new { @Id = projeto.Id, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		public ActionResult ObterSituacao(List<ArquivoProcessamentoVM> arquivos, int projetoId = 0, int arquivoEnviadoTipo = 0, int arquivoEnviadoFilaTipo = 0)
		{
			foreach (ArquivoProcessamentoVM arquivoVetorial in arquivos)
			{
				_bus.ObterSituacaoFila(arquivoVetorial.ArquivoProcessamento);

				arquivoVetorial.RegraBotoesGridVetoriais();
			}

			List<ArquivoProcessamentoVM> arquivosProcessados = new List<ArquivoProcessamentoVM>();

			if (projetoId > 0)
			{
				List<ArquivoProjeto> listas = _bus.ObterArquivos(projetoId);

				foreach (ArquivoProjeto item in listas.Where(x =>
					x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosIDAF &&
					x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES &&
					x.Tipo != (int)eProjetoGeograficoArquivoTipo.CroquiFinal))
				{
					arquivosProcessados.Add(new ArquivoProcessamentoVM(item, arquivoEnviadoTipo, arquivoEnviadoFilaTipo));
				}
			}

			return Json(new { @lista = arquivos, @arquivosProcessados = arquivosProcessados, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Importar(ProjetoGeografico projeto)
		{
			ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM();

			arquivo.ArquivoProcessamento = _bus.EnviarArquivo(projeto);

			arquivo.RegraBotoesGridVetoriais();

			if (!Validacao.EhValido)
			{
				arquivo = null;
			}

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, @Arquivo = arquivo });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Refazer(ProjetoGeografico projeto, bool isCadastrarCaracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Index", "../Empreendimento", Validacao.QueryParamSerializer()) });
			}

			_bus.Refazer(projeto);

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				Url = Url.Action("Editar", "ProjetoGeografico", Validacao.QueryParamSerializer(
					new { id = projeto.Id, empreendimento = projeto.EmpreendimentoId, tipo = projeto.CaracterizacaoId, isCadastrarCaracterizacao = isCadastrarCaracterizacao, mostrarModalDependencias = false }))
			});
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Atualizar(ProjetoGeografico projeto, bool isCadastrarCaracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(projeto.EmpreendimentoId))
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Index", "../Empreendimento", Validacao.QueryParamSerializer()) });

			_bus.Atualizar(projeto);

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				Url = Url.Action("Editar", "ProjetoGeografico", Validacao.QueryParamSerializer(
					new { id = projeto.Id, empreendimento = projeto.EmpreendimentoId, tipo = projeto.CaracterizacaoId, isCadastrarCaracterizacao = isCadastrarCaracterizacao, mostrarModalDependencias = false }))
			});
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Recarregar(ProjetoGeografico projeto, bool isCadastrarCaracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Index", "../Empreendimento", Validacao.QueryParamSerializer()) });
			}

			_bus.Recarregar(projeto);

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				Url = Url.Action("Editar", "ProjetoGeografico", Validacao.QueryParamSerializer(
					new { id = projeto.Id, empreendimento = projeto.EmpreendimentoId, tipo = projeto.CaracterizacaoId, isCadastrarCaracterizacao = isCadastrarCaracterizacao }))
			});
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ExcluirRascunho(ProjetoGeografico projeto, bool isCadastrarCaracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(projeto.EmpreendimentoId))
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Index", "../Empreendimento", Validacao.QueryParamSerializer()) });

			if (projeto.CaracterizacaoId == (int)eCaracterizacao.ExploracaoFlorestal)
			{
				_exploracaoFlorestalBus.Excluir(projeto.EmpreendimentoId);
				if (Validacao.EhValido) Validacao.Erros.Clear();
			}
			_bus.ExcluirRascunho(projeto);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, Url = Url.Action("Index", "Caracterizacao", Validacao.QueryParamSerializer(new { id = projeto.EmpreendimentoId })) });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Finalizar(ProjetoGeografico projeto, string url)
		{
			if (!_caracterizacaoValidar.Basicas(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Index", "../Empreendimento", Validacao.QueryParamSerializer()) });
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

			_bus.Finalizar(projeto);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, Url = url });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult Salvar(ProjetoGeografico projeto, string url)
		{
			if (!_caracterizacaoValidar.Basicas(projeto.EmpreendimentoId))
			{
				return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRedirect = Url.Action("Index", "../Empreendimento", Validacao.QueryParamSerializer()) });
			}

			var urlCriar = CaracterizacaoVM.GerarUrl(projeto.EmpreendimentoId, true, eCaracterizacao.ExploracaoFlorestal);
			var urlEditar = CaracterizacaoVM.GerarUrl(projeto.EmpreendimentoId, false, eCaracterizacao.ExploracaoFlorestal);
			if (url.Contains(urlCriar) || url.Contains(urlEditar)) {
				if(!_validar.Finalizar(projeto))
					return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, Url = url });
				if (_exploracaoFlorestalBus.ExisteExploracaoGeoNaoCadastrada(projeto.Id))
					url = urlCriar;
			}

			_bus.Salvar(projeto);
			if (projeto.CaracterizacaoId == (int)eCaracterizacao.ExploracaoFlorestal)
				_caracterizacaoBus.AtualizarDependentes(projeto.Id, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao, projeto.Tid);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, Url = url });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult AlterarBaseAbrangencia(ProjetoGeografico projeto)
		{
			_bus.AlterarAreaAbrangencia(projeto);

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult SalvarOrtofoto(ProjetoGeografico projeto)
		{
			_bus.SalvarOrtofoto(projeto);

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
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

		private string ConcatenarSistemaCoordenada(Empreendimento emp)
		{
			return emp.Coordenada.Datum.Texto + " - " + emp.Coordenada.Tipo.Texto + " - Fuso " + _listaBus.Fusos[0].Texto;
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoEditar })]
		public ActionResult VerificarAreaNaoCaracterizada(int empreendimento)
		{
			Dominialidade dominialidade = new DominialidadeBus().ObterDadosGeoTMP(empreendimento);
			return Json(new
			{
				Msg = Validacao.Erros,
				PossuiAPPNaoCaracterizada = dominialidade.AreaAPPNaoCaracterizada.MaiorToleranciaM2(),
				PossuiARLNaoCaracterizada = dominialidade.ARLNaoCaracterizada.MaiorToleranciaM2()
			});
		}

		#endregion

		#region Desenhador

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ReprocessarDesenhador()
		{
			return Json(new { EhValido = Validacao.EhValido });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ProcessarDesenhador(ProjetoGeografico projeto)
		{
			if (projeto.CaracterizacaoId == (int)eCaracterizacao.ExploracaoFlorestal)
			{
				_exploracaoFlorestalBus.Excluir(projeto.EmpreendimentoId);
				if (Validacao.EhValido) Validacao.Erros.Clear();
			}

			ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM();

			arquivo.ArquivoProcessamento = _bus.ProcessarDesenhador(projeto);

			arquivo.RegraBotoesGridVetoriais();

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, @Arquivo = arquivo });
		}

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ObterSituacaoDesenhador(int projetoId, int arquivoId, int arquivoEnviadoTipo, int arquivoEnviadoFilaTipo)
		{
			ArquivoProjeto arq = new ArquivoProjeto() { IdRelacionamento = arquivoId };

			_bus.ObterSituacaoFila(arq);

			ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM(arq);

			List<ArquivoProcessamentoVM> arquivosProcessados = new List<ArquivoProcessamentoVM>();

			if (projetoId > 0)
			{
				List<ArquivoProjeto> listas = _bus.ObterArquivos(projetoId);

				foreach (ArquivoProjeto item in listas.Where(x =>
					x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosIDAF &&
					x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES &&
					x.Tipo != (int)eProjetoGeograficoArquivoTipo.CroquiFinal))
				{
					arquivosProcessados.Add(new ArquivoProcessamentoVM(item, arquivoEnviadoTipo, arquivoEnviadoFilaTipo));
				}
			}

			return Json(new { @Arquivo = arquivo, @arquivosProcessados = arquivosProcessados, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Importador

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult CancelarProcessamento(ArquivoProcessamentoVM arquivo)
		{
			_bus.CancelarProcessamento(arquivo.ArquivoProcessamento);

			return Json(new { @Arquivo = arquivo.ArquivoProcessamento, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult ReenviarArquivoImportador()
		{
			return Json(new { EhValido = Validacao.EhValido });
		}

		#endregion

		#region Sobreposicao

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult VerificarSobreposicao(int projetoId, int tipo)
		{
			Sobreposicoes sobreposicoes = _bus.ObterGeoSobreposiacao(projetoId, (eCaracterizacao)tipo);
			_bus.SalvarSobreposicoes(new ProjetoGeografico() { Id = projetoId, Sobreposicoes = sobreposicoes });

			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, Sobreposicoes = sobreposicoes });
		}

		#endregion

		#region Arquivo Vetorial

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
		public ActionResult GerarArquivoVetorial(ArquivoProcessamentoVM arquivoVetorial)
		{
			_bus.ReprocessarBaseReferencia(arquivoVetorial.ArquivoProcessamento);

			arquivoVetorial.RegraBotoesGridVetoriais();

			return Json(new { @arquivo = arquivoVetorial, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
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

		#endregion

		#region Auxiliares

		public List<dynamic> ObterUrlsArquivo() 
		{
			List<dynamic> list = new List<dynamic>();
			String[] nomes = Enum.GetNames(typeof(eProjetoGeograficoArquivoTipo));

			for (int i = 0; i < nomes.Length; i++)
			{
				list.Add(new { Tipo = (Int32)(Enum.Parse(typeof(eProjetoGeograficoArquivoTipo), nomes[i])), Url = Url.Action("Baixar" + nomes[i], "ProjetoGeografico", new { Area = "Caracterizacoes" }) });
			}

			return list;
		}

		#endregion

		#region Download Arquivos

		#region Croqui

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarcroquipdf, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public FileResult BaixarCroqui(int id, int tipo)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo, dataHoraControleAcesso: true);
		}

		#endregion

		#region BaseReferenciaInterna

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarbasereferencainterna, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public FileResult BaixarBaseReferenciaInterna(int id, int tipo)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region BaseReferenciaGeoBases

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarbasereferenciageobases, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public FileResult BaixarBaseReferenciaGeoBases(int id, int tipo)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region ArquivoEnviado

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizararquivoenviado, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public FileResult BaixarArquivoEnviado(int id, int tipo)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region RelatorioImportacao

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarrelatorioimportacao, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public FileResult BaixarRelatorioImportacao(int id, int tipo)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo, dataHoraControleAcesso: true);
		}

		#endregion

		#region ArquivoProcessadoGIS

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizararquivoprocessadogis, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public FileResult BaixarArquivoProcessadoGIS(int id, int tipo)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region ArquivoProcessadoTRACKMAKER

		[Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar, ePermissao.ProjetoGeograficoVisualizar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizararquivoprocessadotrackmaker, Artefato = (int)eHistoricoArtefatoCaracterizacao.projetogeografico)]
		public FileResult BaixarArquivoProcessadoTRACKMAKER(int id, int tipo)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Interno).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#region Credenciado

		public FileResult BaixarArquivoCredenciado(int id)
		{
			Arquivo arquivo = new ArquivoBus(eExecutorTipo.Credenciado).Obter(id);

			return ViewModelHelper.GerarArquivo(arquivo);
		}

		#endregion

		#endregion
	}
}