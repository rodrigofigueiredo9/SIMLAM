using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico;
using FiscalizacaoDa = Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data.FiscalizacaoDa;
using RelFiscalizacaoLib = Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using RelFiscalizacao = Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
    public class FiscalizacaoController : DefaultController
    {
        #region Propriedades

        PdfFiscalizacao _pdf = new PdfFiscalizacao();
        FiscalizacaoBus _bus = new FiscalizacaoBus();
        LocalInfracaoBus _busLocalInfracao = new LocalInfracaoBus();
        ProjetoGeograficoBus _busProjGeo = new ProjetoGeograficoBus();
        ComplementacaoDadosBus _busComplementacaoDados = new ComplementacaoDadosBus();
        InfracaoBus _busInfracao = new InfracaoBus();
        ConfigFiscalizacaoBus _busConfiguracao = new ConfigFiscalizacaoBus();
        EnquadramentoBus _busEnquadramento = new EnquadramentoBus();
        ObjetoInfracaoBus _busObjetoInfracao = new ObjetoInfracaoBus();
        MaterialApreendidoBus _busMaterialApreendido = new MaterialApreendidoBus();
        ConsideracaoFinalBus _busConsideracaoFinal = new ConsideracaoFinalBus();
        MultaBus _busMulta = new MultaBus();
        OutrasPenalidadesBus _busOutrasPenalidades = new OutrasPenalidadesBus();

        ListaBus _busLista = new ListaBus();
        FuncionarioBus _busFuncionario = new FuncionarioBus();
        GerenciadorConfiguracao<ConfiguracaoSistema> _configSistema = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
        PessoaBus _busPessoa = new PessoaBus();
        EmpreendimentoBus _busEmpreendimento = new EmpreendimentoBus();
        SetorLocalizacaoBus _busSetorLocalizacao = new SetorLocalizacaoBus(new SetorLocalizacaoValidar());
        AcompanhamentoBus _busAcompanhamento = new AcompanhamentoBus();

        ConfigFiscalizacaoValidar _validarConfigFisc = new ConfigFiscalizacaoValidar();
        AcompanhamentoValidar _validarAcompanhamento = new AcompanhamentoValidar();

        public static EtramitePrincipal Usuario
        {
            get { return (System.Web.HttpContext.Current.User as EtramitePrincipal); }
        }

        #endregion

        #region Filtrar

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoListar })]
        public ActionResult Index()
        {
            ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.Setores, _bus.ObterTipoInfracao(), _bus.ObterItemInfracao(), _busLista.FiscalizacaoSituacao.Where(x => x.Id != "4"/*Cancelar Conclusão*/).ToList());
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            return PartialView(vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoListar })]
        public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
        {
            if (!String.IsNullOrEmpty(vm.UltimaBusca))
            {
                vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
            }

            vm.Paginacao = paginacao;
            vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

            Resultados<Fiscalizacao> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
            if (resultados == null)
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
            }

            if (!vm.PodeAssociar)
            {
                vm.PodeEditar = User.IsInRole(ePermissao.FiscalizacaoEditar.ToString());
                vm.PodeExcluir = User.IsInRole(ePermissao.FiscalizacaoExcluir.ToString());
                vm.PodeVisualizar = User.IsInRole(ePermissao.FiscalizacaoVisualizar.ToString());
                vm.PodeAlterarSituacao = User.IsInRole(ePermissao.FiscalizacaoAlterarSituacao.ToString());
                vm.PodeVisualizarAcompanhamentos = User.IsInRole(ePermissao.AcompanhamentoListar.ToString());
            }

            vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
            vm.Paginacao.EfetuarPaginacao();
            vm.Resultados = resultados.Itens;

            return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Criar / Editar / Salvar / Visualizar

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult Criar()
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            LocalInfracao local = new LocalInfracao();

            vm.LocalInfracaoVM = new LocalInfracaoVM(local, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busFuncionario.ObterSetoresFuncionario(), new Pessoa(), new List<PessoaLst>());
            vm.ComplementacaoDadosVM = new ComplementacaoDadosVM(new ComplementacaoDados(), _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosRendaMensal, _busLista.FiscalizacaoComplementoDadosNivelEscolaridade, _busLista.TiposResponsavel, _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosReservaLegalTipo);
            vm.Fiscalizacao.SituacaoId = 1;
            return View("Salvar", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar })]
        public ActionResult Editar(int? id)
        {
            Fiscalizacao fiscalizacao = _bus.Obter(id.GetValueOrDefault());

            if (fiscalizacao.Situacao != eFiscalizacaoSituacao.EmAndamento)
            {
                String situacaoTexto = _busLista.FiscalizacaoSituacao.FirstOrDefault(x => x.Id == fiscalizacao.SituacaoId.ToString()).Texto;

                Validacao.Add(Mensagem.Fiscalizacao.SituacaoNaoPodeEditar(situacaoTexto));
                return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            if (fiscalizacao.Autuante.Id != FiscalizacaoBus.User.EtramiteIdentity.FuncionarioId)
            {
                Validacao.Add(Mensagem.Fiscalizacao.AgenteFiscalInvalido("editar"));
                return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            FiscalizacaoVM vm = new FiscalizacaoVM();
            vm.Id = fiscalizacao.Id;
            vm.Fiscalizacao.SituacaoId = fiscalizacao.SituacaoId == 0 ? 1 : fiscalizacao.SituacaoId;
            List<PessoaLst> lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveis(fiscalizacao.LocalInfracao.EmpreendimentoId.Value) : new List<PessoaLst>();
            vm.LocalInfracaoVM = new LocalInfracaoVM(fiscalizacao.LocalInfracao, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.Setores, _busPessoa.Obter(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault()), lstResponsaveis);
            vm.LocalInfracaoVM.IsVisualizar = true;
            vm.ComplementacaoDadosVM = new ComplementacaoDadosVM(new ComplementacaoDados(), _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosRendaMensal, _busLista.FiscalizacaoComplementoDadosNivelEscolaridade, _busLista.TiposResponsavel, _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosReservaLegalTipo);
            vm.InfracaoVM = new InfracaoVM();
            vm.InfracaoVM.Infracao = _busInfracao.Obter(fiscalizacao.Id, false);

            if (vm.InfracaoVM.Infracao.IdsOutrasPenalidades.Count() > 0) vm.InfracaoVM.Infracao.PossuiAdvertencia = true;   //apenas para carregar a aba

            return View("Salvar", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult Salvar(Fiscalizacao fiscalizacao)
        {
            if (fiscalizacao.LocalInfracao.AreaAbrangencia == null) fiscalizacao.LocalInfracao.AreaAbrangencia = "0";
            _bus.Salvar(fiscalizacao);

            if (Validacao.EhValido)
            {
                return Json(new { @EhValido = Validacao.EhValido, @id = fiscalizacao.Id, @Msg = Validacao.Erros });
            }
            else
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult LocalInfracao(int? id)
        {
            Fiscalizacao fiscalizacao = id.GetValueOrDefault() > 0 ? _bus.Obter(id.GetValueOrDefault()) : new Fiscalizacao();
            FiscalizacaoVM vm = new FiscalizacaoVM();
            List<PessoaLst> lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveis(fiscalizacao.LocalInfracao.EmpreendimentoId.Value) : new List<PessoaLst>();

            vm.LocalInfracaoVM = new LocalInfracaoVM(fiscalizacao.LocalInfracao, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busFuncionario.ObterSetoresFuncionario(), _busPessoa.Obter(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault()), lstResponsaveis);

            if (Request.IsAjaxRequest())
            {
                return PartialView(vm.LocalInfracaoVM);
            }
            else
            {
                vm.PartialInicial = "Local da Infração";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult LocalInfracaoVisualizar(int id)
        {
            Fiscalizacao fiscalizacao = _bus.Obter(id);
            FiscalizacaoVM vm = new FiscalizacaoVM();

            List<PessoaLst> lstResponsaveis = new List<PessoaLst>();
            Pessoa pessoa = new Pessoa();

            if (fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
            {
                fiscalizacao.LocalInfracao = new LocalInfracaoDa().Obter(id);
                lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveis(fiscalizacao.LocalInfracao.EmpreendimentoId.Value) : new List<PessoaLst>();
                pessoa = _busPessoa.Obter(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0));
            }
            else
            {
                fiscalizacao.LocalInfracao = new LocalInfracaoDa().ObterHistorico(id);
                lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveisHistorico(fiscalizacao.LocalInfracao.EmpreendimentoId.Value, fiscalizacao.LocalInfracao.EmpreendimentoTid) : new List<PessoaLst>();
                pessoa = _busLocalInfracao.ObterPessoaSimplificadaPorHistorico(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0), fiscalizacao.LocalInfracao.PessoaTid);
            }

            vm.LocalInfracaoVM = new LocalInfracaoVM(fiscalizacao.LocalInfracao, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.Setores, pessoa, lstResponsaveis);
            vm.LocalInfracaoVM.IsVisualizar = fiscalizacao.LocalInfracao.Id > 0;

            if (Request.IsAjaxRequest())
            {
                return PartialView("LocalInfracao", vm.LocalInfracaoVM);
            }
            else
            {
                vm.PartialInicial = "LocalInfracao";
                return View("Visualizar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult ObterResponsaveis(int empreendimentoId)
        {
            List<PessoaLst> lstResponsaveis = _busLocalInfracao.ObterResponsaveis(empreendimentoId);
            List<object> lstResponsaveisSelect = lstResponsaveis.Select(x => new { Id = x.Id, Texto = x.Texto }).ToList<object>();
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Responsaveis = lstResponsaveisSelect });
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar })]
        public ActionResult Visualizar(int id)
        {
            Fiscalizacao fiscalizacao = _bus.Obter(id);
            FiscalizacaoVM vm = new FiscalizacaoVM();
            vm.Id = fiscalizacao.Id;
            vm.Fiscalizacao.SituacaoId = fiscalizacao.SituacaoId == 0 ? 1 : fiscalizacao.SituacaoId;

            List<PessoaLst> lstResponsaveis = new List<PessoaLst>();
            Pessoa pessoa = new Pessoa();

            if (fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
            {
                fiscalizacao.LocalInfracao = _busLocalInfracao.Obter(id);
                lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveis(fiscalizacao.LocalInfracao.EmpreendimentoId.Value) : new List<PessoaLst>();
                pessoa = _busPessoa.Obter(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0));
            }
            else
            {
                //fiscalizacao.LocalInfracao = _busLocalInfracao.ObterHistorico(id);
                fiscalizacao.LocalInfracao = _busLocalInfracao.Obter(id);   //temporário
                lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveisHistorico(fiscalizacao.LocalInfracao.EmpreendimentoId.Value, fiscalizacao.LocalInfracao.EmpreendimentoTid) : new List<PessoaLst>();
                pessoa = _busLocalInfracao.ObterPessoaSimplificadaPorHistorico(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0), fiscalizacao.LocalInfracao.PessoaTid);
            }

            vm.LocalInfracaoVM = new LocalInfracaoVM(fiscalizacao.LocalInfracao, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.Setores, pessoa, lstResponsaveis);
            vm.LocalInfracaoVM.IsVisualizar = true;

            vm.ComplementacaoDadosVM = new ComplementacaoDadosVM(new ComplementacaoDados(), _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosRendaMensal, _busLista.FiscalizacaoComplementoDadosNivelEscolaridade, _busLista.TiposResponsavel, _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosReservaLegalTipo);
            vm.IsVisualizar = true;

            vm.InfracaoVM = new InfracaoVM();
            vm.InfracaoVM.Infracao = _busInfracao.Obter(fiscalizacao.Id, false);

            if (vm.InfracaoVM.Infracao.IdsOutrasPenalidades.Count() > 0) vm.InfracaoVM.Infracao.PossuiAdvertencia = true;   //apenas para carregar a aba

            return View("Visualizar", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar })]
        public ActionResult VisualizarFiscalizacaoModal(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            Fiscalizacao fiscalizacao = _bus.Obter(id);

            vm.Fiscalizacao = fiscalizacao;
            vm.Id = fiscalizacao.Id;

            if (fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
            {
                List<PessoaLst> lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveis(fiscalizacao.LocalInfracao.EmpreendimentoId.Value) : new List<PessoaLst>();
                vm.LocalInfracaoVM = new LocalInfracaoVM(fiscalizacao.LocalInfracao, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busFuncionario.ObterSetoresFuncionario(), fiscalizacao.AutuadoPessoa, lstResponsaveis);
            }
            else
            {
                fiscalizacao.LocalInfracao = _busLocalInfracao.ObterHistorico(id);
                fiscalizacao.AutuadoEmpreendimento = (fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault(0) > 0) ? _busEmpreendimento.ObterHistorico(fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault(0), fiscalizacao.LocalInfracao.EmpreendimentoTid) : new Empreendimento();
                fiscalizacao.Autuante = _bus.ObterAutuanteHistorico(fiscalizacao.Id);

                List<PessoaLst> lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveisHistorico(fiscalizacao.LocalInfracao.EmpreendimentoId.Value, fiscalizacao.LocalInfracao.EmpreendimentoTid) : new List<PessoaLst>();
                vm.LocalInfracaoVM = new LocalInfracaoVM(fiscalizacao.LocalInfracao, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busFuncionario.ObterSetoresFuncionario(), _busLocalInfracao.ObterPessoaSimplificadaPorHistorico(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0), fiscalizacao.LocalInfracao.PessoaTid), lstResponsaveis);
            }

            vm.ComplementacaoDadosVM.Entidade = fiscalizacao.ComplementacaoDados;
            vm.EnquadramentoVM.Entidade = fiscalizacao.Enquadramento;
            vm.InfracaoVM.Infracao = fiscalizacao.Infracao;
            vm.InfracaoVM.Classificacoes = ViewModelHelper.CriarSelectList(_busLista.InfracaoClassificacao, true, selecionado: fiscalizacao.Infracao.ClassificacaoId.ToString());
            vm.InfracaoVM.Tipos = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterTipos(fiscalizacao.Infracao.ClassificacaoId), true, selecionado: fiscalizacao.Infracao.TipoId.ToString());
            vm.InfracaoVM.Itens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterItens(fiscalizacao.Infracao.ClassificacaoId, fiscalizacao.Infracao.TipoId), true, selecionado: fiscalizacao.Infracao.ItemId.ToString());
            vm.InfracaoVM.Subitens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterSubitens(fiscalizacao.Infracao.ClassificacaoId, fiscalizacao.Infracao.TipoId, fiscalizacao.Infracao.ItemId), true, selecionado: fiscalizacao.Infracao.SubitemId.GetValueOrDefault().ToString());
            vm.InfracaoVM.Series = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterSeries(fiscalizacao.Infracao.IsGeradaSistema.GetValueOrDefault()), true, selecionado: fiscalizacao.Infracao.SerieId.GetValueOrDefault().ToString());
            vm.InfracaoVM.CodigoReceitas = ViewModelHelper.CriarSelectList(_busLista.InfracaoCodigoReceita, true, selecionado: fiscalizacao.Infracao.CodigoReceitaId.GetValueOrDefault().ToString());
            vm.InfracaoVM.Campos = fiscalizacao.Infracao.Campos;
            vm.InfracaoVM.Perguntas = fiscalizacao.Infracao.Perguntas;
            vm.ObjetoInfracaoVM.Entidade = fiscalizacao.ObjetoInfracao;
            vm.MaterialApreendidoVM.MaterialApreendido = fiscalizacao.MaterialApreendido;
            vm.ConsideracaoFinalVM.ConsideracaoFinal = fiscalizacao.ConsideracaoFinal;
            vm.IsVisualizar = true;

            return PartialView("FiscalizacaoModal", vm);
        }

        #endregion

        #region Projeto Geografico

        /* Wizard ------------------------------------------------------------------------------------------------------------------------------- */
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ProjetoGeografico(int id = 0)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();

            vm.ProjetoGeoVM.ArquivoEnviadoTipo = (int)eFilaTipoGeo.Fiscalizacao;
            vm.ProjetoGeoVM.NiveisPrecisao = ViewModelHelper.CriarSelectList(_busProjGeo.ObterNiveisPrecisao());
            vm.ProjetoGeoVM.SistemaCoordenada = ViewModelHelper.CriarSelectList(_busProjGeo.ObterSistemaCoordenada());

            vm.ProjetoGeoVM.UrlBaixarOrtofoto = _configSistema.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/DownloadArquivoOrtoFoto";
            vm.ProjetoGeoVM.UrlValidarOrtofoto = _configSistema.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/ValidarChaveArquivoOrtoFoto";

            vm.Id = id;
            vm.ProjetoGeoVM.Projeto = _busProjGeo.ObterProjetoGeograficoPorFiscalizacao(id);

            Fiscalizacao fiscalizacao = _bus.Obter(id);
            if (fiscalizacao != null)
            {
                vm.ProjetoGeoVM.Projeto.FiscalizacaoEasting = fiscalizacao.LocalInfracao.LonEastingToDecimal;
                vm.ProjetoGeoVM.Projeto.FiscalizacaoNorthing = fiscalizacao.LocalInfracao.LatNorthingToDecimal;

                vm.LocalInfracaoVM.LocalInfracao = _busLocalInfracao.Obter(id);
            }

            if (vm.ProjetoGeoVM.Projeto.Id > 0)
            {
                vm.ProjetoGeoVM.CarregarVMs();
            }
            else
            {
                vm.ProjetoGeoVM.Projeto.MecanismoElaboracaoId = -1;
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView(vm);
            }
            else
            {
                vm.PartialInicial = "ProjetoGeografico";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult ProjetoGeograficoVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();

            vm.ProjetoGeoVM.ArquivoEnviadoTipo = (int)eFilaTipoGeo.Fiscalizacao;
            vm.ProjetoGeoVM.NiveisPrecisao = ViewModelHelper.CriarSelectList(_busProjGeo.ObterNiveisPrecisao());
            vm.ProjetoGeoVM.SistemaCoordenada = ViewModelHelper.CriarSelectList(_busProjGeo.ObterSistemaCoordenada());

            vm.ProjetoGeoVM.UrlBaixarOrtofoto = _configSistema.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/DownloadArquivoOrtoFoto";
            vm.ProjetoGeoVM.UrlValidarOrtofoto = _configSistema.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices) + "/Arquivo/ValidarChaveArquivoOrtoFoto";

            vm.Id = id;
            vm.ProjetoGeoVM.Projeto = _busProjGeo.ObterProjetoGeograficoPorFiscalizacao(id);

            Fiscalizacao fiscalizacao = _bus.Obter(id);
            if (fiscalizacao != null)
            {
                vm.ProjetoGeoVM.Projeto.FiscalizacaoEasting = fiscalizacao.LocalInfracao.LonEastingToDecimal;
                vm.ProjetoGeoVM.Projeto.FiscalizacaoNorthing = fiscalizacao.LocalInfracao.LatNorthingToDecimal;
            }

            vm.ProjetoGeoVM.CarregarVMs();

            vm.ProjetoGeoVM.IsVisualizar = vm.ProjetoGeoVM.Projeto.Id > 0 || vm.ProjetoGeoVM.Projeto.PossuiProjetoGeo != null;
            vm.ProjetoGeoVM.BaseReferencia.IsVisualizar = vm.ProjetoGeoVM.IsVisualizar;
            vm.ProjetoGeoVM.EnviarProjeto.IsVisualizar = vm.ProjetoGeoVM.IsVisualizar;
            vm.ProjetoGeoVM.Desenhador.IsVisualizar = vm.ProjetoGeoVM.IsVisualizar;

            vm.LocalInfracaoVM.LocalInfracao = _busLocalInfracao.Obter(id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("ProjetoGeografico", vm);
            }
            else
            {
                vm.PartialInicial = "ProjetoGeografico";
                return View("Visualizar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ProjetoGeograficoSalvar(ProjetoGeografico projeto)
        {
            _busProjGeo.Salvar(projeto);
            return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
        }
        /* ------------------------------------------------------------------------------------------------------------------------------- */

        /* Tela ------------------------------------------------------------------------------------------------------------------------------- */
        #region Projeto Greográfico

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult Carregar(ProjetoGeograficoVM vm, bool mostrarModalDependencias = true)
        {
            vm.ArquivoEnviadoTipo = (int)eFilaTipoGeo.Fiscalizacao;

            if (vm.Projeto.Id > 0)
            {
                vm.CarregarVMs();
            }

            return View("Criar", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterMerge(int id)
        {
            ProjetoGeografico projetoGeo = _busProjGeo.ObterProjeto(id);
            return Json(new { @Msg = Validacao.Erros, @EhValido = Validacao.EhValido, @Projeto = projetoGeo });
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult CriarParcial(ProjetoGeografico projeto)
        {
            _busProjGeo.Salvar(projeto);

            return Json(new { @Id = projeto.Id, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterSituacao(List<ArquivoProcessamentoVM> arquivos, int projetoId = 0, int arquivoEnviadoTipo = 0)
        {
            foreach (ArquivoProcessamentoVM arquivoVetorial in arquivos)
            {
                _busProjGeo.ObterSituacaoFila(arquivoVetorial.ArquivoProcessamento);

                arquivoVetorial.RegraBotoesGridVetoriais();
            }

            List<ArquivoProcessamentoVM> arquivosProcessados = new List<ArquivoProcessamentoVM>();

            if (projetoId > 0)
            {
                List<ArquivoProjeto> listas = _busProjGeo.ObterArquivos(projetoId);

                foreach (ArquivoProjeto item in listas.Where(x =>
                    x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosIDAF &&
                    x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES &&
                    x.Tipo != (int)eProjetoGeograficoArquivoTipo.CroquiFinal))
                {
                    arquivosProcessados.Add(new ArquivoProcessamentoVM(item, arquivoEnviadoTipo));
                }
            }

            return Json(new { @lista = arquivos, @arquivosProcessados = arquivosProcessados, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult Importar(ProjetoGeografico projeto)
        {
            ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM();

            arquivo.ArquivoProcessamento = _busProjGeo.EnviarArquivo(projeto);

            arquivo.RegraBotoesGridVetoriais();

            if (Validacao.EhValido)
                return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, @Arquivo = arquivo });
            else
                return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult AlterarBaseAbrangencia(ProjetoGeografico projeto)
        {
            _busProjGeo.AlterarAreaAbrangencia(projeto);

            return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ProjetoGeograficoCriar, ePermissao.ProjetoGeograficoEditar })]
        public ActionResult SalvarOrtofoto(ProjetoGeografico projeto)
        {
            _busProjGeo.SalvarOrtofoto(projeto);

            return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public FileResult BaixarArquivoOrtofoto(int id, int situacao)
        {
            try
            {
                ProjetoGeograficoBus _bus = new ProjetoGeograficoBus();
                bool finalizado = situacao == (int)eProjetoGeograficoSituacao.Finalizado;

                Arquivo arquivo = _busProjGeo.ArquivoOrtofoto(id, finalizado, false);
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

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
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

        #region Desenhador

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ReprocessarDesenhador()
        {
            return Json(new { EhValido = Validacao.EhValido });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ProcessarDesenhador(ProjetoGeografico projeto)
        {
            ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM();

            arquivo.ArquivoProcessamento = _busProjGeo.ProcessarDesenhador(projeto);

            arquivo.RegraBotoesGridVetoriais();

            return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros, @Arquivo = arquivo });
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterSituacaoDesenhador(int projetoId, int arquivoId, int arquivoEnviadoTipo)
        {
            ArquivoProjeto arq = new ArquivoProjeto() { IdRelacionamento = arquivoId };

            _busProjGeo.ObterSituacaoFila(arq);

            ArquivoProcessamentoVM arquivo = new ArquivoProcessamentoVM(arq);

            List<ArquivoProcessamentoVM> arquivosProcessados = new List<ArquivoProcessamentoVM>();

            if (projetoId > 0)
            {
                List<ArquivoProjeto> listas = _busProjGeo.ObterArquivos(projetoId);

                foreach (ArquivoProjeto item in listas.Where(x =>
                    x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosIDAF &&
                    x.Tipo != (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES &&
                    x.Tipo != (int)eProjetoGeograficoArquivoTipo.CroquiFinal))
                {
                    arquivosProcessados.Add(new ArquivoProcessamentoVM(item, arquivoEnviadoTipo));
                }
            }

            return Json(new { @Arquivo = arquivo, @arquivosProcessados = arquivosProcessados, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Importador

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult CancelarProcessamento(ArquivoProcessamentoVM arquivo)
        {
            _busProjGeo.CancelarProcessamento(arquivo.ArquivoProcessamento);

            return Json(new { @Arquivo = arquivo.ArquivoProcessamento, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ReenviarArquivoImportador()
        {
            return Json(new { EhValido = Validacao.EhValido });
        }

        #endregion

        #region Arquivo Vetorial

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult GerarArquivoVetorial(ArquivoProcessamentoVM arquivoVetorial)
        {
            _busProjGeo.ReprocessarBaseReferencia(arquivoVetorial.ArquivoProcessamento);

            arquivoVetorial.RegraBotoesGridVetoriais();

            return Json(new { @arquivo = arquivoVetorial, @EhValido = Validacao.EhValido });
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterOrtoFotoMosaico(int projetoId)
        {
            List<ArquivoProcessamentoVM> listaVM = new List<ArquivoProcessamentoVM>();

            foreach (ArquivoProjeto item in _busProjGeo.ObterOrtofotos(projetoId))
            {
                listaVM.Add(new ArquivoProcessamentoVM(item));
            }

            return Json(new { @Lista = listaVM, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        /* ------------------------------------------------------------------------------------------------------------------------------- */
        #endregion

        #region Complementacao de dados do autuado

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult ComplementacaoDados(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            ComplementacaoDados entidade = _busComplementacaoDados.Obter(id);

            vm.ComplementacaoDadosVM = new ComplementacaoDadosVM(entidade, _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosRendaMensal, _busLista.FiscalizacaoComplementoDadosNivelEscolaridade, _busLista.TiposResponsavel, _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosReservaLegalTipo);

            if (Request.IsAjaxRequest())
            {
                return PartialView(vm.ComplementacaoDadosVM);
            }
            else
            {
                vm.PartialInicial = "ComplementacaoDados";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult ComplementacaoDadosVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            ComplementacaoDados entidade = _busComplementacaoDados.Obter(id);

            vm.ComplementacaoDadosVM = new ComplementacaoDadosVM(entidade, _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosRendaMensal, _busLista.FiscalizacaoComplementoDadosNivelEscolaridade, _busLista.TiposResponsavel, _busLista.FiscalizacaoComplementoDadosRespostas, _busLista.FiscalizacaoComplementoDadosReservaLegalTipo);
            vm.ComplementacaoDadosVM.IsVisualizar = entidade.Id > 0;

            if (Request.IsAjaxRequest())
            {
                return PartialView("ComplementacaoDados", vm.ComplementacaoDadosVM);
            }
            else
            {
                vm.PartialInicial = "ComplementacaoDados";
                return View("Visualizar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult CriarComplementacaoDados(ComplementacaoDados entidade)
        {
            _busComplementacaoDados.Salvar(entidade);

            return Json(new { id = entidade.Id, Msg = Validacao.Erros });
        }

        #region Auxiliares

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult ObterVinculoPropriedade(int autuadoId, int empreendimentoId)
        {
            string[] vinculo = _busComplementacaoDados.ObterVinculoPropriedade(autuadoId, empreendimentoId).Split('@');

            return Json(new
            {
                @vinculo = vinculo[0],
                @especificar = vinculo[1]
            }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #endregion

        #region Enquadramento

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult Enquadramento(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            vm.EnquadramentoVM = new EnquadramentoVM(_busEnquadramento.Obter(id));

            if (Request.IsAjaxRequest())
            {
                return PartialView(vm.EnquadramentoVM);
            }
            else
            {
                vm.PartialInicial = "Enquadramento";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult EnquadramentoVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            Enquadramento entidade = _busEnquadramento.Obter(id);
            vm.EnquadramentoVM = new EnquadramentoVM(entidade);
            vm.EnquadramentoVM.IsVisualizar = entidade.Id > 0;

            if (Request.IsAjaxRequest())
            {
                return PartialView("Enquadramento", vm.EnquadramentoVM);
            }
            else
            {
                vm.PartialInicial = "Enquadramento";
                return View("Salvar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult CriarEnquadramento(Enquadramento entidade)
        {
            _busEnquadramento.Salvar(entidade);

            return Json(new { id = entidade.Id, Msg = Validacao.Erros });
        }

        #endregion

        #region Interdição/Embargo

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult ObjetoInfracao(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            ObjetoInfracao entidade = new ObjetoInfracao();

            if (id != 0)
            {
                entidade = _busObjetoInfracao.Obter(id);
            }

            vm.ObjetoInfracaoVM = new ObjetoInfracaoVM(entidade, _busLista.FiscalizacaoSerie);

            if (vm.ObjetoInfracaoVM.Entidade.Arquivo == null)
            {
                vm.ObjetoInfracaoVM.Entidade.Arquivo = new Arquivo();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("InterdicaoEmbargo", vm.ObjetoInfracaoVM);
            }
            else
            {
                vm.PartialInicial = "InterdicaoEmbargo";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult ObjetoInfracaoVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();

            ObjetoInfracao entidade = _busObjetoInfracao.Obter(id);
            
            vm.ObjetoInfracaoVM = new ObjetoInfracaoVM(entidade, _busLista.FiscalizacaoSerie);
            vm.ObjetoInfracaoVM.IsVisualizar = entidade.Id > 0;
            vm.ObjetoInfracaoVM.DataConclusaoFiscalizacao = _bus.ObterDataConclusao(id);


            if (Request.IsAjaxRequest())
            {
                return PartialView("InterdicaoEmbargo", vm.ObjetoInfracaoVM);
            }
            else
            {
                vm.PartialInicial = "InterdicaoEmbargo";
                return View("Visualizar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar })]
        public ActionResult CriarObjetoInfracao(ObjetoInfracao entidade)
        {
            _busObjetoInfracao.Salvar(entidade);

            return Json(new { id = entidade.Id, Msg = Validacao.Erros });
        }

        #endregion

        #region Considerações Finais

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult ConsideracaoFinal(int? id)
        {
            Fiscalizacao fiscalizacao = new Fiscalizacao();
            fiscalizacao.ConsideracaoFinal = id.GetValueOrDefault() > 0 ? _busConsideracaoFinal.Obter(id.GetValueOrDefault()) : new ConsideracaoFinal();

            FiscalizacaoVM vm = new FiscalizacaoVM();
            List<PessoaLst> funcionarios = _busConsideracaoFinal.ObterFuncionarios();
            List<Lista> funcionarioIDAF = new List<Lista> 
			{
				new Lista { Texto = "Sim",  Id = "1" }, 
				new Lista { Texto = "Não", Id = "2" } 
			};

            vm.ConsideracaoFinalVM = new ConsideracaoFinalVM();
            vm.ConsideracaoFinalVM.ConsideracaoFinal = fiscalizacao.ConsideracaoFinal;
            vm.ConsideracaoFinalVM.ArquivoVM.Anexos = fiscalizacao.ConsideracaoFinal.Anexos;
            vm.ConsideracaoFinalVM.ArquivoIUFVM.Anexos = fiscalizacao.ConsideracaoFinal.AnexosIUF;

            if (fiscalizacao.ConsideracaoFinal.Testemunhas.Count == 0)
            {
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM.ForEach(x =>
                {
                    x.FuncionarioIDAF = ViewModelHelper.CriarSelectList(funcionarioIDAF, false, true);
                    x.Funcionarios = ViewModelHelper.CriarSelectList(funcionarios, true, true);
                    x.Setores = ViewModelHelper.CriarSelectList(_busFuncionario.ObterSetoresFuncionario(), true, true);
                });
            }

            for (int i = 0; i < fiscalizacao.ConsideracaoFinal.Testemunhas.Count; i++)
            {
                var testIDAF = fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaIDAF;
                var strValue = testIDAF.HasValue ? (testIDAF.Value ? "1" : "2") : "0";
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].FuncionarioIDAF = ViewModelHelper.CriarSelectList(funcionarioIDAF, false, true, strValue);
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Funcionarios = ViewModelHelper.CriarSelectList(funcionarios, true, true, fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaId.GetValueOrDefault().ToString());
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Setores = ViewModelHelper.CriarSelectList(_busFuncionario.ObterSetoresFuncionario(fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaId.GetValueOrDefault()), true, true, fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaSetorId.GetValueOrDefault().ToString());

                fiscalizacao.ConsideracaoFinal.Testemunhas[i].Colocacao = vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Testemunha.Colocacao;
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Testemunha = fiscalizacao.ConsideracaoFinal.Testemunhas[i];
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].IsVisualizar = vm.ConsideracaoFinalVM.IsVisualizar;
            }

            vm.ConsideracaoFinalVM.AssinantesVM.IsVisualizar = vm.ConsideracaoFinalVM.IsVisualizar;
            vm.ConsideracaoFinalVM.AssinantesVM.Setores = ViewModelHelper.CriarSelectList(_busLista.Setores);
            vm.ConsideracaoFinalVM.AssinantesVM.Assinantes = fiscalizacao.ConsideracaoFinal.Assinantes;

            if (fiscalizacao.ConsideracaoFinal.Arquivo.Id.GetValueOrDefault() > 0)
            {
                vm.ConsideracaoFinalVM.ArquivoJSon = vm.JSON(fiscalizacao.ConsideracaoFinal.Arquivo);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView(vm.ConsideracaoFinalVM);
            }
            else
            {
                vm.PartialInicial = "ConsideracaoFinal";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult ConsideracaoFinalVisualizar(int id)
        {
            Fiscalizacao fiscalizacao = new Fiscalizacao();
            fiscalizacao.ConsideracaoFinal = _busConsideracaoFinal.Obter(id);

            FiscalizacaoVM vm = new FiscalizacaoVM();
            List<PessoaLst> funcionarios = _busConsideracaoFinal.ObterFuncionarios();
            vm.ConsideracaoFinalVM = new ConsideracaoFinalVM();
            List<Lista> funcionarioIDAF = new List<Lista> 
			{
				new Lista { Texto = "Sim",  Id = "1" }, 
				new Lista { Texto = "Não", Id = "2" } 
			};

            vm.ConsideracaoFinalVM.IsVisualizar = fiscalizacao.ConsideracaoFinal.Id > 0;
            vm.ConsideracaoFinalVM.ArquivoVM.IsVisualizar = vm.ConsideracaoFinalVM.IsVisualizar;
            vm.ConsideracaoFinalVM.ArquivoIUFVM.IsVisualizar = vm.ConsideracaoFinalVM.IsVisualizar;
            vm.ConsideracaoFinalVM.ConsideracaoFinal = fiscalizacao.ConsideracaoFinal;
            vm.ConsideracaoFinalVM.ArquivoVM.Anexos = fiscalizacao.ConsideracaoFinal.Anexos;
            vm.ConsideracaoFinalVM.ArquivoIUFVM.Anexos = fiscalizacao.ConsideracaoFinal.AnexosIUF;

            if (fiscalizacao.ConsideracaoFinal.Testemunhas.Count == 0)
            {
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM.ForEach(x =>
                {
                    x.FuncionarioIDAF = ViewModelHelper.CriarSelectList(funcionarioIDAF, false, true);
                    x.Funcionarios = ViewModelHelper.CriarSelectList(funcionarios, true, true);
                    x.Setores = ViewModelHelper.CriarSelectList(_busFuncionario.ObterSetoresFuncionario(), true, true);
                });
            }

            for (int i = 0; i < fiscalizacao.ConsideracaoFinal.Testemunhas.Count; i++)
            {
                var testIDAF = fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaIDAF;
                var strValue = testIDAF.HasValue ? (testIDAF.Value ? "1" : "2") : "0";
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].FuncionarioIDAF = ViewModelHelper.CriarSelectList(funcionarioIDAF, false, true, strValue);
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Funcionarios = ViewModelHelper.CriarSelectList(funcionarios, true, true, fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaId.GetValueOrDefault().ToString());
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Setores = ViewModelHelper.CriarSelectList(_busFuncionario.ObterSetoresFuncionario(fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaId.GetValueOrDefault()), true, true, fiscalizacao.ConsideracaoFinal.Testemunhas[i].TestemunhaSetorId.GetValueOrDefault().ToString());

                fiscalizacao.ConsideracaoFinal.Testemunhas[i].Colocacao = vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Testemunha.Colocacao;
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].Testemunha = fiscalizacao.ConsideracaoFinal.Testemunhas[i];
                vm.ConsideracaoFinalVM.ConsideracaoFinalTestemunhaVM[i].IsVisualizar = vm.ConsideracaoFinalVM.IsVisualizar;
            }

            vm.ConsideracaoFinalVM.AssinantesVM.IsVisualizar = vm.ConsideracaoFinalVM.IsVisualizar;
            vm.ConsideracaoFinalVM.AssinantesVM.Setores = ViewModelHelper.CriarSelectList(_busLista.Setores);
            vm.ConsideracaoFinalVM.AssinantesVM.Assinantes = fiscalizacao.ConsideracaoFinal.Assinantes;

            if (fiscalizacao.ConsideracaoFinal.Arquivo.Id.GetValueOrDefault() > 0)
            {
                vm.ConsideracaoFinalVM.ArquivoJSon = vm.JSON(fiscalizacao.ConsideracaoFinal.Arquivo);
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("ConsideracaoFinal", vm.ConsideracaoFinalVM);
            }
            else
            {
                vm.PartialInicial = "ConsideracaoFinal";
                return View("Salvar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult SalvarConsideracaoFinal(ConsideracaoFinal consideracaoFinal)
        {
            _busConsideracaoFinal.Salvar(consideracaoFinal);

            if (Validacao.EhValido)
            {
                return Json(new { @EhValido = Validacao.EhValido, @id = consideracaoFinal.Id, @Msg = Validacao.Erros });
            }
            else
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros });
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult ObterSetores(int funcionarioId)
        {
            var lstSetores = _busFuncionario.ObterSetoresFuncionario(funcionarioId);
            List<object> lstSetoresSelect = lstSetores.Select(x => new { Id = x.Id, Texto = x.Texto }).ToList<object>();
            string strEndereco = string.Empty;

            if (lstSetores.Count == 1)
            {
                var setorLocalizacao = _busSetorLocalizacao.Obter(lstSetores[0].Id);
                strEndereco = setorLocalizacao.FormatarEndereco();
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Setores = lstSetoresSelect, @Endereco = strEndereco });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult ObterEnderecoSetor(int setorId)
        {
            var setorLocalizacao = _busSetorLocalizacao.Obter(setorId);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Endereco = setorLocalizacao.FormatarEndereco() });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult ObterCPF(int funcionarioId)
        {
            var cpf = _busFuncionario.Obter(funcionarioId).Cpf;
            
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @CPF = cpf });
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar, ePermissao.AcompanhamentoCriar, ePermissao.AcompanhamentoEditar })]
        public ActionResult ObterAssinanteCargos(int setorId)
        {
            var lista = _bus.ObterAssinanteCargos(setorId);

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar, ePermissao.AcompanhamentoCriar, ePermissao.AcompanhamentoEditar })]
        public ActionResult ObterAssinanteFuncionarios(int setorId, int cargoId)
        {
            var lista = _bus.ObterAssinanteFuncionarios(setorId, cargoId);

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Infração/Fiscalização

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult Infracao(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            Infracao infracao = new Infracao();
            List<Lista> codigoReceitas = new List<Lista>();
            List<Item> tipos = new List<Item>();
            List<Lista> itens = new List<Lista>();
            List<Lista> subitens = new List<Lista>();
            List<Lista> series = new List<Lista>();
            List<Lista> penalidades = new List<Lista>();

            if (id != 0)
            {
                infracao = _busInfracao.Obter(id, true);

                if (infracao.Id != 0)
                {
                    tipos = _busConfiguracao.ObterTipos(infracao.ClassificacaoId);
                    itens = _busConfiguracao.ObterItens(infracao.ClassificacaoId, infracao.TipoId);
                    subitens = _busConfiguracao.ObterSubitens(infracao.ClassificacaoId, infracao.TipoId, infracao.ItemId);
                    series = _busLista.FiscalizacaoSerie;
                    penalidades = _busConfiguracao.ObterPenalidadesLista();
                }
                else
                {
                    infracao = new Infracao();
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (infracao.IdsOutrasPenalidades.Count <= i)
                {
                    infracao.IdsOutrasPenalidades.Add(0);
                }
            }

            vm.InfracaoVM = new InfracaoVM
            {
                Infracao = infracao,
                Classificacoes = ViewModelHelper.CriarSelectList(_busLista.InfracaoClassificacao, true, selecionado: infracao.ClassificacaoId.ToString()),
                Tipos = ViewModelHelper.CriarSelectList(tipos, true, selecionado: infracao.TipoId.ToString()),
                Itens = ViewModelHelper.CriarSelectList(itens, true, selecionado: infracao.ItemId.ToString()),
                Subitens = ViewModelHelper.CriarSelectList(subitens, true, selecionado: infracao.SubitemId.GetValueOrDefault().ToString()),
                Series = ViewModelHelper.CriarSelectList(series, true, selecionado: infracao.SerieId.GetValueOrDefault().ToString()),
                CodigoReceitas = ViewModelHelper.CriarSelectList(_busLista.InfracaoCodigoReceita, true, selecionado: infracao.CodigoReceitaId.GetValueOrDefault().ToString()),
                Penalidades = penalidades,
                ListaPenalidades01 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[0].ToString()),
                ListaPenalidades02 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[1].ToString()),
                ListaPenalidades03 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[2].ToString()),
                ListaPenalidades04 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[3].ToString())
            };

            vm.InfracaoVM.Campos = infracao.Campos;
            vm.InfracaoVM.Perguntas = infracao.Perguntas;

            if (vm.InfracaoVM.Infracao.Arquivo == null)
            {
                vm.InfracaoVM.Infracao.Arquivo = new Arquivo();
            }

            vm.InfracaoVM.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(vm.InfracaoVM.Infracao.Arquivo);

            if (Request.IsAjaxRequest())
            {
                return PartialView(vm.InfracaoVM);
            }
            else
            {
                vm.PartialInicial = "Infracao";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoVisualizar })]
        public ActionResult InfracaoVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            Infracao infracao = new Infracao();
            List<Lista> codigoReceitas = new List<Lista>();
            List<Item> tipos = new List<Item>();
            List<Lista> itens = new List<Lista>();
            List<Lista> subitens = new List<Lista>();
            List<Lista> series = new List<Lista>();
            List<Lista> penalidades = new List<Lista>();

            //infracao = _busInfracao.ObterHistoricoPorFiscalizacao(id);
            infracao = _busInfracao.Obter(id, true);

            for (int i = 0; i < 4; i++)
            {
                if (infracao.IdsOutrasPenalidades.Count <= i)
                {
                    infracao.IdsOutrasPenalidades.Add(0);
                }
            }

                tipos = _busConfiguracao.ObterTipos(infracao.ClassificacaoId);
            itens = _busConfiguracao.ObterItens(infracao.ClassificacaoId, infracao.TipoId);
            subitens = _busConfiguracao.ObterSubitens(infracao.ClassificacaoId, infracao.TipoId, infracao.ItemId);
            series = _busLista.FiscalizacaoSerie;
            penalidades = _busConfiguracao.ObterPenalidadesLista();

            if (_busInfracao.ConfigAlterada(infracao.ConfiguracaoId, infracao.ConfiguracaoTid))
            {
                if (tipos.FindIndex(x => x.Id == infracao.TipoId.ToString()) == -1)
                {
                    tipos.Add(new Item { Id = infracao.TipoId.ToString(), Texto = infracao.TipoTexto });
                }

                if (itens.FindIndex(x => x.Id == infracao.ItemId.ToString()) == -1)
                {
                    itens.Add(new Item { Id = infracao.ItemId.ToString(), Texto = infracao.ItemTexto });
                }

                if (subitens.FindIndex(x => x.Id == infracao.SubitemId.ToString()) == -1)
                {
                    subitens.Add(new Item { Id = infracao.SubitemId.ToString(), Texto = infracao.SubitemTexto });
                }
            }

            vm.InfracaoVM = new InfracaoVM
            {
                IsVisualizar = infracao.Id > 0,
                Infracao = infracao,
                Classificacoes = ViewModelHelper.CriarSelectList(_busLista.InfracaoClassificacao, true, selecionado: infracao.ClassificacaoId.ToString()),
                Tipos = ViewModelHelper.CriarSelectList(tipos, true, selecionado: infracao.TipoId.ToString()),
                Itens = ViewModelHelper.CriarSelectList(itens, true, selecionado: infracao.ItemId.ToString()),
                Subitens = ViewModelHelper.CriarSelectList(subitens, true, selecionado: infracao.SubitemId.GetValueOrDefault().ToString()),
                Series = ViewModelHelper.CriarSelectList(series, true, selecionado: infracao.SerieId.GetValueOrDefault().ToString()),
                CodigoReceitas = ViewModelHelper.CriarSelectList(_busLista.InfracaoCodigoReceita, true, selecionado: infracao.CodigoReceitaId.GetValueOrDefault().ToString()),
                Penalidades = penalidades,
                ListaPenalidades01 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[0].ToString()),
                ListaPenalidades02 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[1].ToString()),
                ListaPenalidades03 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[2].ToString()),
                ListaPenalidades04 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: infracao.IdsOutrasPenalidades[3].ToString())
            };

            vm.InfracaoVM.Campos = infracao.Campos;
            vm.InfracaoVM.Perguntas = infracao.Perguntas;

            vm.InfracaoVM.DataConclusaoFiscalizacao = _bus.ObterDataConclusao(id);

            if (vm.InfracaoVM.Infracao.Arquivo == null)
            {
                vm.InfracaoVM.Infracao.Arquivo = new Arquivo();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Infracao", vm.InfracaoVM);
            }
            else
            {
                vm.PartialInicial = "Infracao";
                return View("Salvar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult CriarInfracao(Infracao entidade)
        {
            _busInfracao.Salvar(entidade);

            return Json(new { id = entidade.Id, Msg = Validacao.Erros });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterInfracaoTipos(int classificacaoId)
        {
            List<Item> lista = _busConfiguracao.ObterTipos(classificacaoId);

            return Json(new
            {
                Msg = Validacao.Erros,
                EhValido = Validacao.EhValido,
                Tipos = lista
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterInfracaoItens(int classificacaoId, int tipoId)
        {
            List<Lista> lista = _busConfiguracao.ObterItens(classificacaoId, tipoId);

            return Json(new
            {
                Msg = Validacao.Erros,
                EhValido = Validacao.EhValido,
                Itens = lista
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterConfiguracao(int classificacaoId, int tipoId, int itemId)
        {
            InfracaoVM vm = new InfracaoVM();

            var configuracao = _busConfiguracao.Obter(classificacaoId, tipoId, itemId);

            if (configuracao.GetValue<int>("Id") == 0)
            {
                Validacao.Add(Mensagem.Fiscalizacao.ConfiguracaoObrigatoria);
            }

            vm.Infracao.ConfiguracaoId = configuracao.GetValue<int>("Id");
            vm.Infracao.ConfiguracaoTid = configuracao.GetValue<string>("Tid");

            List<Lista> lista = _busConfiguracao.ObterSubitens(classificacaoId, tipoId, itemId);

            vm.Campos = _busConfiguracao.ObterCampos(classificacaoId, tipoId, itemId);
            vm.Perguntas = _busConfiguracao.ObterQuestionarios(classificacaoId, tipoId, itemId);

            return Json(new
            {
                Msg = Validacao.Erros,
                EhValido = Validacao.EhValido,
                Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoCamposPerguntas", vm),
                Subitens = lista
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult ObterInfracaoSeries(bool isSim)
        {
            List<Lista> lista = _busConfiguracao.ObterSeries(isSim);

            return Json(new
            {
                Msg = Validacao.Erros,
                EhValido = Validacao.EhValido,
                Series = lista
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Apreensão

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult MaterialApreendido(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            MaterialApreendido materialApreendido = new MaterialApreendido();
            List<ProdutoApreendidoLst> produtosApreendidos = new List<ProdutoApreendidoLst>();

            if (id != 0)
            {
                materialApreendido = _busMaterialApreendido.Obter(id);
            }

            produtosApreendidos = _busMaterialApreendido.ObterProdutosApreendidosLst();

            vm.MaterialApreendidoVM = new MaterialApreendidoVM
            {
                MaterialApreendido = materialApreendido,
                Tipos = ViewModelHelper.CriarSelectList(_busLista.MaterialApreendidoTipo, true),
                produtosUnidades = produtosApreendidos,
                ListaProdutosApreendidos = ViewModelHelper.CriarSelectList(produtosApreendidos, true),
                ListaDestinos = ViewModelHelper.CriarSelectList(_busMaterialApreendido.ObterDestinosLst()),
                Ufs = ViewModelHelper.CriarSelectList(_busLista.Estados, true, selecionado: materialApreendido.Depositario.Estado.GetValueOrDefault().ToString()),
                Municipios = new List<SelectListItem>(),
                Series = ViewModelHelper.CriarSelectList(_busLista.FiscalizacaoSerie, true, true, selecionado: materialApreendido.SerieId.ToString())
            };

            if (id != 0)
            {
                if (materialApreendido.IsTadGeradoSistema.HasValue)
                {
                    vm.MaterialApreendidoVM.Series = ViewModelHelper.CriarSelectList(_busLista.FiscalizacaoSerie, true, selecionado: materialApreendido.SerieId.GetValueOrDefault().ToString());
                }

                vm.MaterialApreendidoVM.Municipios = ViewModelHelper.CriarSelectList(_busLista.Municipios(materialApreendido.Depositario.Estado.GetValueOrDefault()), true, selecionado: materialApreendido.Depositario.Municipio.GetValueOrDefault().ToString());
            }

            if (vm.MaterialApreendidoVM.MaterialApreendido.Arquivo == null)
            {
                vm.MaterialApreendidoVM.MaterialApreendido.Arquivo = new Arquivo();
            }

            vm.MaterialApreendidoVM.ArquivoJSon = ViewModelHelper.JsSerializer.Serialize(vm.MaterialApreendidoVM.MaterialApreendido.Arquivo);

            if (Request.IsAjaxRequest())
            {
                return PartialView(vm.MaterialApreendidoVM);
            }
            else
            {
                vm.PartialInicial = "MaterialApreendido";
                return View("Salvar", vm);
            }
        }

        //Carrega a sessão, em uma fiscalização já salva
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar, ePermissao.FiscalizacaoEditar })]
        public ActionResult MaterialApreendidoVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            MaterialApreendido materialApreendido = new MaterialApreendido();
            List<ProdutoApreendidoLst> produtosApreendidos = new List<ProdutoApreendidoLst>();

            materialApreendido = _busMaterialApreendido.Obter(id);
            produtosApreendidos = _busMaterialApreendido.ObterProdutosApreendidosLst();

            vm.MaterialApreendidoVM = new MaterialApreendidoVM
            {
                IsVisualizar = materialApreendido.Id > 0,
                MaterialApreendido = materialApreendido,
                Tipos = ViewModelHelper.CriarSelectList(_busLista.MaterialApreendidoTipo, true),
                produtosUnidades = produtosApreendidos,
                ListaProdutosApreendidos = ViewModelHelper.CriarSelectList(produtosApreendidos, true),
                ListaDestinos = ViewModelHelper.CriarSelectList(_busMaterialApreendido.ObterDestinosLst()),
                Ufs = ViewModelHelper.CriarSelectList(_busLista.Estados, true, selecionado: materialApreendido.Depositario.Estado.GetValueOrDefault().ToString()),
                Municipios = new List<SelectListItem>(),
                Series = ViewModelHelper.CriarSelectList(_busLista.FiscalizacaoSerie, true, true, selecionado: materialApreendido.SerieId.ToString())
            };

            vm.MaterialApreendidoVM.DataConclusaoFiscalizacao = _bus.ObterDataConclusao(id);

            if (materialApreendido.IsTadGeradoSistema.HasValue)
            {
                vm.MaterialApreendidoVM.Municipios = ViewModelHelper.CriarSelectList(_busLista.Municipios(materialApreendido.Depositario.Estado.GetValueOrDefault()), true, selecionado: materialApreendido.Depositario.Municipio.GetValueOrDefault().ToString());
            }

            vm.MaterialApreendidoVM.Municipios = ViewModelHelper.CriarSelectList(_busLista.Municipios(materialApreendido.Depositario.Estado.GetValueOrDefault()), true, selecionado: materialApreendido.Depositario.Municipio.GetValueOrDefault().ToString());

            if (vm.MaterialApreendidoVM.MaterialApreendido.Arquivo == null)
            {
                vm.MaterialApreendidoVM.MaterialApreendido.Arquivo = new Arquivo();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("MaterialApreendido", vm.MaterialApreendidoVM);
            }
            else
            {
                vm.PartialInicial = "MaterialApreendido";
                return View("Salvar", vm);
            }
        }

        //Salva a sessão
        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult CriarMaterialApreendido(MaterialApreendido entidade)
        {
            _busMaterialApreendido.Salvar(entidade);

            return Json(new { id = entidade.Id, Msg = Validacao.Erros });
        }

        #endregion

        #region Multa

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult Multa(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            Multa multa = new Multa();

            if (id != 0)
            {
                multa = _busMulta.Obter(id);
            }
            vm.MultaVM = new MultaVM
            {
                Multa = multa,
                Series = ViewModelHelper.CriarSelectList(_busLista.FiscalizacaoSerie, true, true, selecionado: multa.SerieId.ToString()),
                //CodigosReceita = ViewModelHelper.CriarSelectList(_busLista.InfracaoCodigoReceita, true, selecionado: multa.CodigoReceitaId.GetValueOrDefault().ToString())
                CodigosReceita = ViewModelHelper.CriarSelectList(_busMulta.obterCodigoReceita(id), true, selecionado: multa.CodigoReceitaId.GetValueOrDefault().ToString())
            };

            vm.MultaVM.DataConclusaoFiscalizacao = _bus.ObterDataConclusao(id);

            if (vm.MultaVM.Multa.Arquivo == null)
            {
                vm.MultaVM.Multa.Arquivo = new Arquivo();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Multa", vm.MultaVM);
            }
            else
            {
                vm.PartialInicial = "Multa";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult MultaVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            Multa multa = new Multa();

            multa = _busMulta.Obter(id);
            
            vm.MultaVM = new MultaVM
            {
                IsVisualizar = multa.Id > 0,
                Multa = multa,
                Series = ViewModelHelper.CriarSelectList(_busLista.FiscalizacaoSerie, true, true, selecionado: multa.SerieId.ToString()),
                //CodigosReceita = ViewModelHelper.CriarSelectList(_busLista.InfracaoCodigoReceita, true, selecionado: multa.CodigoReceitaId.GetValueOrDefault().ToString())
                CodigosReceita = ViewModelHelper.CriarSelectList(_busMulta.obterCodigoReceita(id), true, selecionado: multa.CodigoReceitaId.GetValueOrDefault().ToString())
            };

            vm.MultaVM.DataConclusaoFiscalizacao = _bus.ObterDataConclusao(id);

            if (vm.MultaVM.Multa.Arquivo == null)
            {
                vm.MultaVM.Multa.Arquivo = new Arquivo();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("Multa", vm.MultaVM);
            }
            else
            {
                vm.PartialInicial = "Multa";
                return View("Salvar", vm);
            }
        }

        //Salva a sessão
        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult CriarMulta(Multa entidade)
        {
            _busMulta.Salvar(entidade);

            return Json(new { id = entidade.Id, Msg = Validacao.Erros });
        }

        #endregion Multa

        #region Outras Penalidades

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult OutrasPenalidades(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            OutrasPenalidades outrasPenalidades = new OutrasPenalidades();

            if (id != 0)
            {
                outrasPenalidades = _busOutrasPenalidades.Obter(id);
            }

            vm.OutrasPenalidadesVM = new OutrasPenalidadesVM
            {
                OutrasPenalidades = outrasPenalidades,
                Series = ViewModelHelper.CriarSelectList(_busLista.FiscalizacaoSerie, true, true, selecionado: outrasPenalidades.SerieId.ToString())
            };

            vm.OutrasPenalidadesVM.DataConclusaoFiscalizacao = _bus.ObterDataConclusao(id);

            if (vm.OutrasPenalidadesVM.OutrasPenalidades.Arquivo == null)
            {
                vm.OutrasPenalidadesVM.OutrasPenalidades.Arquivo = new Arquivo();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("OutrasPenalidades", vm.OutrasPenalidadesVM);
            }
            else
            {
                vm.PartialInicial = "OutrasPenalidades";
                return View("Salvar", vm);
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult OutrasPenalidadesVisualizar(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            OutrasPenalidades outrasPenalidades = new OutrasPenalidades();

            outrasPenalidades = _busOutrasPenalidades.Obter(id);

            vm.OutrasPenalidadesVM = new OutrasPenalidadesVM
            {
                IsVisualizar = outrasPenalidades.Id > 0,
                OutrasPenalidades = outrasPenalidades,
                Series = ViewModelHelper.CriarSelectList(_busLista.FiscalizacaoSerie, true, true, selecionado: outrasPenalidades.SerieId.ToString())
            };

            vm.OutrasPenalidadesVM.DataConclusaoFiscalizacao = _bus.ObterDataConclusao(id);

            if (vm.OutrasPenalidadesVM.OutrasPenalidades.Arquivo == null)
            {
                vm.OutrasPenalidadesVM.OutrasPenalidades.Arquivo = new Arquivo();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("OutrasPenalidades", vm.OutrasPenalidadesVM);
            }
            else
            {
                vm.PartialInicial = "OutrasPenalidades";
                return View("Salvar", vm);
            }
        }

        //Salva a sessão
        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoCriar, ePermissao.FiscalizacaoEditar })]
        public ActionResult CriarOutrasPenalidades(OutrasPenalidades entidade)
        {
            _busOutrasPenalidades.Salvar(entidade);

            return Json(new { id = entidade.Id, Msg = Validacao.Erros });
        }

        #endregion Outras Penalidades

        #region Finalizar

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar, ePermissao.FiscalizacaoEditar })]
        public ActionResult Finalizar(int? id)
        {
            Fiscalizacao fiscalizacao = id.GetValueOrDefault() > 0 ? _bus.Obter(id.Value) : new Fiscalizacao();
            FiscalizacaoVM vm = new FiscalizacaoVM();

            vm.Fiscalizacao = fiscalizacao;


            List<PessoaLst> lstResponsaveis = new List<PessoaLst>();
            Pessoa pessoa = new Pessoa();

            if (fiscalizacao.SituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
            {
                fiscalizacao.LocalInfracao = _busLocalInfracao.Obter(fiscalizacao.Id);

                fiscalizacao.AutuadoPessoa = fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault() > 0 ? _busPessoa.Obter(fiscalizacao.LocalInfracao.PessoaId.Value) : new Pessoa();
                fiscalizacao.AutuadoEmpreendimento = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busEmpreendimento.Obter(fiscalizacao.LocalInfracao.EmpreendimentoId.Value) : new Empreendimento();

                lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveis(fiscalizacao.LocalInfracao.EmpreendimentoId.Value) : new List<PessoaLst>();
                pessoa = _busPessoa.Obter(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0));
            }
            else
            {
                fiscalizacao.LocalInfracao = _busLocalInfracao.ObterHistorico(fiscalizacao.Id);

                fiscalizacao.Autuante = _bus.ObterAutuanteHistorico(fiscalizacao.Id);
                fiscalizacao.AutuadoEmpreendimento = (fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault(0) > 0) ? _busEmpreendimento.ObterHistorico(fiscalizacao.LocalInfracao.EmpreendimentoId.Value, fiscalizacao.LocalInfracao.EmpreendimentoTid) : new Empreendimento();
                fiscalizacao.AutuadoPessoa = (fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0) > 0) ? _busLocalInfracao.ObterPessoaSimplificadaPorHistorico(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0), fiscalizacao.LocalInfracao.PessoaTid) : new Pessoa();

                lstResponsaveis = fiscalizacao.LocalInfracao.EmpreendimentoId.GetValueOrDefault() > 0 ? _busLocalInfracao.ObterResponsaveisHistorico(fiscalizacao.LocalInfracao.EmpreendimentoId.Value, fiscalizacao.LocalInfracao.EmpreendimentoTid) : new List<PessoaLst>();
                pessoa = _busLocalInfracao.ObterPessoaSimplificadaPorHistorico(fiscalizacao.LocalInfracao.PessoaId.GetValueOrDefault(0), fiscalizacao.LocalInfracao.PessoaTid);
            }

            vm.LocalInfracaoVM = new LocalInfracaoVM(fiscalizacao.LocalInfracao, _busLista.Estados, _busLista.Municipios(_busLista.EstadoDefault), _busLista.Segmentos, _busLista.TiposCoordenada, _busLista.Datuns, _busLista.Fusos, _busLista.Hemisferios, _busLista.Setores, pessoa, lstResponsaveis);
            vm.ComplementacaoDadosVM.Entidade = fiscalizacao.ComplementacaoDados;
            vm.EnquadramentoVM.Entidade = fiscalizacao.Enquadramento;

            vm.InfracaoVM.Infracao = fiscalizacao.Infracao;

            List<Lista> penalidades = _busConfiguracao.ObterPenalidadesLista();
            vm.InfracaoVM.Penalidades = penalidades;
            vm.InfracaoVM.ListaPenalidades01 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: vm.InfracaoVM.Infracao.IdsOutrasPenalidades[0].ToString());
            vm.InfracaoVM.ListaPenalidades02 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: vm.InfracaoVM.Infracao.IdsOutrasPenalidades[1].ToString());
            vm.InfracaoVM.ListaPenalidades03 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: vm.InfracaoVM.Infracao.IdsOutrasPenalidades[2].ToString());
            vm.InfracaoVM.ListaPenalidades04 = ViewModelHelper.CriarSelectList(penalidades, true, selecionado: vm.InfracaoVM.Infracao.IdsOutrasPenalidades[3].ToString());

            vm.InfracaoVM.Classificacoes = ViewModelHelper.CriarSelectList(_busLista.InfracaoClassificacao, true, selecionado: fiscalizacao.Infracao.ClassificacaoId.ToString());
            vm.InfracaoVM.Tipos = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterTipos(fiscalizacao.Infracao.ClassificacaoId), true, selecionado: fiscalizacao.Infracao.TipoId.ToString());
            vm.InfracaoVM.Itens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterItens(fiscalizacao.Infracao.ClassificacaoId, fiscalizacao.Infracao.TipoId), true, selecionado: fiscalizacao.Infracao.ItemId.ToString());
            vm.InfracaoVM.Subitens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterSubitens(fiscalizacao.Infracao.ClassificacaoId, fiscalizacao.Infracao.TipoId, fiscalizacao.Infracao.ItemId), true, selecionado: fiscalizacao.Infracao.SubitemId.GetValueOrDefault().ToString());
            vm.InfracaoVM.Series = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterSeries(fiscalizacao.Infracao.IsGeradaSistema.GetValueOrDefault()), true, selecionado: fiscalizacao.Infracao.SerieId.GetValueOrDefault().ToString());
            vm.InfracaoVM.CodigoReceitas = ViewModelHelper.CriarSelectList(_busLista.InfracaoCodigoReceita, true, selecionado: fiscalizacao.Infracao.CodigoReceitaId.GetValueOrDefault().ToString());
            vm.InfracaoVM.Campos = fiscalizacao.Infracao.Campos;
            vm.InfracaoVM.Perguntas = fiscalizacao.Infracao.Perguntas;

            vm.ObjetoInfracaoVM.Entidade = fiscalizacao.ObjetoInfracao;
            vm.MaterialApreendidoVM.MaterialApreendido = fiscalizacao.MaterialApreendido;
            vm.ConsideracaoFinalVM.ConsideracaoFinal = fiscalizacao.ConsideracaoFinal;
            vm.ProjetoGeoVM.Projeto = _busProjGeo.ObterProjetoGeograficoPorFiscalizacao(fiscalizacao.Id);
            vm.MultaVM.Multa = fiscalizacao.Multa;
            vm.OutrasPenalidadesVM.OutrasPenalidades = fiscalizacao.OutrasPenalidades;

            if (Request.IsAjaxRequest())
            {
                vm.PartialInicial = "Concluir Cadastro";
                return View("ConcluirCadastro", vm);
            }
            else
            {
                vm.PartialInicial = "Concluir Cadastro";
                return View("Salvar", vm);
            }
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult FinalizarFiscalizacao(int id)
        {
            _bus.ConcluirCadastro(id);

            String urlRedireciona = Url.Action("Index", "Fiscalizacao", Validacao.QueryParamSerializer()) + "&acaoId=" + id.ToString();
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Redirect = urlRedireciona });
        }

        #endregion

        #region Alterar Situacao

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoAlterarSituacao })]
        public ActionResult AlterarSituacao(int id)
        {
            Fiscalizacao fiscalizacao = _bus.Obter(id);
            if (_bus.PodeAlterarSituacao(fiscalizacao))
            {
                AlterarSituacaoVM vm = new AlterarSituacaoVM(fiscalizacao, _busLista.Setores, _busLista.Segmentos, _busLista.FiscalizacaoSituacao);
                return View(vm);
            }

            return RedirectToAction("Index", "Fiscalizacao", new { Msg = Validacao.QueryParam() });
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoAlterarSituacao })]
        public ActionResult AlterarSituacao(Fiscalizacao fiscalizacao)
        {
            string urlRedireciona = string.Empty;

            if (_bus.AlterarSituacao(fiscalizacao))
            {
                Validacao.Add(Mensagem.Fiscalizacao.SituacaoSalvar);
                urlRedireciona = Url.Action("Index", "Fiscalizacao", new { id = fiscalizacao.Id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @id = fiscalizacao.Id, @UrlRedirecionar = urlRedireciona, @Msg = Validacao.Erros, @EhValido = Validacao.EhValido });
        }

        #endregion

        #region Associar

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoListar })]
        public ActionResult Associar()
        {
            ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.Setores, _bus.ObterTipoInfracao(), _bus.ObterItemInfracao(), _busLista.FiscalizacaoSituacao.Where(x => x.Id != "4"/*Cancelar Conclusão*/).ToList());
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            return PartialView("ListarFiltros", vm);
        }

        #endregion

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoExcluir })]
        public ActionResult ExcluirConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            vm.Id = id;
            vm.Titulo = "Excluir Fiscalização";
            vm.Mensagem = Mensagem.Fiscalizacao.ExcluirConfirmacao(id);
            return View("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoExcluir })]
        public ActionResult Excluir(int id)
        {
            _bus.Excluir(id);
            return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
        }

        #endregion

        #region Download

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult DownloadDocumentosGerados(int tipo)
        {
            return PartialView();
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar })]
        public ActionResult DocumentosGeradosPartial(int id)
        {
            FiscalizacaoVM vm = new FiscalizacaoVM();
            Fiscalizacao fiscalizacao = _bus.Obter(id);

            vm.Fiscalizacao = fiscalizacao;
            vm.Id = fiscalizacao.Id;
            vm.InfracaoVM.Infracao = fiscalizacao.Infracao;
            vm.ObjetoInfracaoVM.Entidade = fiscalizacao.ObjetoInfracao;
            vm.ProjetoGeoVM.Projeto = _busProjGeo.ObterProjetoGeograficoPorFiscalizacao(id);
            vm.MaterialApreendidoVM.MaterialApreendido = fiscalizacao.MaterialApreendido;
            vm.ConsideracaoFinalVM.ConsideracaoFinal = fiscalizacao.ConsideracaoFinal;

            vm.DocumentosCancelados = _bus.ObterHistoricoDocumentosCancelados(id);

            AcompanhamentoBus acompanhamentoBus = new AcompanhamentoBus();
            vm.Acompanhamentos = acompanhamentoBus.ObterAcompanhamentos(id).Itens;

            return PartialView("DocumentosGeradosModal", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult AutoTermoFiscalizacaoPdf(int id, int arquivo = 0, int historico = 0)
        {
            try
            {
                Stream strArquivo = _bus.AutoTermoFiscalizacaoPdf(id, arquivo, historico);

                if (!Validacao.EhValido)
                {
                    return RedirectToAction("Index", Validacao.QueryParamSerializer());
                }
                return ViewModelHelper.GerarArquivo("Auto Termo de Fiscalizacao.pdf", strArquivo, "application/pdf");
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
                return RedirectToAction("Index", Validacao.QueryParamSerializer());
            }
        }

        public ActionResult InstrumentoUnicoFiscalizacaoPdf(int id, int arquivo = 0, int historico = 0)
        {
            try
            {
                Stream strArquivo = _bus.InstrumentoUnicoFiscalizacaoPdf(id, arquivo, historico);

                if (!Validacao.EhValido)
                {
                    return RedirectToAction("Index", Validacao.QueryParamSerializer());
                }
                return ViewModelHelper.GerarArquivo("Instrumento Unico de Fiscalizacao.pdf", strArquivo, "application/pdf");
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
                return RedirectToAction("Index", Validacao.QueryParamSerializer());
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult LaudoFiscalizacaoPdf(int id, int arquivo = 0, int historico = 0)
        {
            try
            {
                Stream strArquivo = _bus.LaudoFiscalizacaoPdfNovo(id, arquivo, historico);

                if (!Validacao.EhValido)
                {
                    return RedirectToAction("Index", Validacao.QueryParamSerializer());
                }
                return ViewModelHelper.GerarArquivo("Laudo de Fiscalizacao.pdf", strArquivo, "application/pdf");
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
                return RedirectToAction("Index", Validacao.QueryParamSerializer());
            }
        }

        [Permite(RoleArray = new Object[] { ePermissao.FiscalizacaoVisualizar, ePermissao.FiscalizacaoEditar, ePermissao.FiscalizacaoCriar })]
        public ActionResult Baixar(int id, int historico = 0)
        {
            try
            {
                return ViewModelHelper.GerarArquivo(_bus.BaixarArquivo(id, historico));
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
                return RedirectToAction("Index", Validacao.QueryParamSerializer());
            }
        }

        #endregion

        #region Configuracoes

        #region Index

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarFiscListar })]
        public ActionResult ConfiguracaoIndex()
        {
            ConfiguracaoListarVM vm = new ConfiguracaoListarVM(_busLista.QuantPaginacao, _busLista.InfracaoClassificacao, _busConfiguracao.ObterTiposConfig(null), _busConfiguracao.ObterItensConfig(null));
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            return View(vm);
        }

        #endregion

        #region Configurar

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarFiscListar })]
        public ActionResult ConfiguracaoFiltrar(ConfiguracaoListarVM vm, Paginacao paginacao)
        {
            if (!String.IsNullOrEmpty(vm.UltimaBusca))
            {
                vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ConfiguracaoListarVM>(vm.UltimaBusca).Filtros;
            }

            vm.Paginacao = paginacao;
            vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

            Resultados<ConfigFiscalizacao> resultados = _busConfiguracao.Filtrar(vm.Filtros, vm.Paginacao);

            if (resultados == null)
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
            }

            if (!vm.PodeAssociar)
            {
                vm.PodeEditar = User.IsInRole(ePermissao.ConfigurarFiscEditar.ToString());
                vm.PodeExcluir = User.IsInRole(ePermissao.ConfigurarFiscExcluir.ToString());
                vm.PodeVisualizar = User.IsInRole(ePermissao.ConfigurarFiscVisualizar.ToString());
            }

            vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
            vm.Paginacao.EfetuarPaginacao();
            vm.Resultados = resultados.Itens;

            return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ConfiguracaoListarResultados", vm) }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarFiscCriar, ePermissao.ConfigurarFiscEditar })]
        public ActionResult ConfiguracaoSalvar(int? id)
        {
            ConfiguracaoSalvarVM vm = new ConfiguracaoSalvarVM();

            if (id.GetValueOrDefault() > 0)
            {
                vm.Configuracao = _busConfiguracao.Obter(id.Value);
            }

            vm.Classificacoes = ViewModelHelper.CriarSelectList(_busLista.InfracaoClassificacao, true, selecionado: vm.Configuracao.ClassificacaoId.ToString());
            vm.Tipos = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterTiposConfig(true), true, selecionado: vm.Configuracao.TipoId.ToString());
            vm.Itens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterItensConfig(true), true, selecionado: vm.Configuracao.ItemId.ToString());
            vm.Subitens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterSubitensConfig(true));
            vm.Perguntas = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterPerguntasConfig());
            vm.Campos = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterCamposConfig(true));

            return View(vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarFiscVisualizar })]
        public ActionResult ConfiguracaoVisualizar(int? id)
        {
            ConfiguracaoSalvarVM vm = new ConfiguracaoSalvarVM();
            vm.IsVisualizar = true;
            if (id.GetValueOrDefault() > 0)
            {
                vm.Configuracao = _busConfiguracao.Obter(id.Value);
            }

            vm.Classificacoes = ViewModelHelper.CriarSelectList(_busLista.InfracaoClassificacao, true, selecionado: vm.Configuracao.ClassificacaoId.ToString());
            vm.Tipos = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterTiposConfig(true), true, selecionado: vm.Configuracao.TipoId.ToString());
            vm.Itens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterItensConfig(true), true, selecionado: vm.Configuracao.ItemId.ToString());
            vm.Subitens = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterSubitensConfig(true));
            vm.Perguntas = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterPerguntasConfig());
            vm.Campos = ViewModelHelper.CriarSelectList(_busConfiguracao.ObterCamposConfig(true));

            return View("ConfiguracaoVisualizar", vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarFiscCriar, ePermissao.ConfigurarFiscEditar })]
        public ActionResult ConfiguracaoCriar(ConfigFiscalizacao configuracao)
        {
            _busConfiguracao.Salvar(configuracao);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Redirect = Url.Action("ConfiguracaoIndex", "Fiscalizacao", Validacao.QueryParamSerializer()) });
        }

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarFiscExcluir })]
        public ActionResult ConfiguracaoExcluirConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            vm.Id = id;
            vm.Titulo = "Excluir Configuração";
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirConfirmacao(id);
            return View("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarFiscExcluir })]
        public ActionResult ConfiguracaoExcluir(int id)
        {
            _busConfiguracao.Excluir(id);
            return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
        }

        #endregion

        #region Tipo Infracao

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarTipoInfracao })]
        public ActionResult ConfigurarTipoInfracao()
        {
            TipoInfracaoVM vm = new TipoInfracaoVM(_busConfiguracao.ObterTipoInfracao());
            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarTipoInfracao })]
        public ActionResult ConfigurarTipoInfracao(Item entidade)
        {
            _busConfiguracao.SalvarTipoInfracao(entidade);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new TipoInfracaoVM(_busConfiguracao.ObterTipoInfracao()));

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Html = html,
                @UrlRedirecionar = Url.Action("ConfiguracaoIndex", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarTipoInfracao })]
        public ActionResult ExcluirTipoInfracaoConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            List<Item> itens = _busConfiguracao.ObterTipoInfracao();

            String nome = itens.FirstOrDefault(x => x.Id == id.ToString()).Texto;

            vm.Id = id;
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirTipoInfracaoMensagem(nome);
            vm.Titulo = "Excluir Tipo de infração";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarTipoInfracao })]
        public ActionResult ExcluirTipoInfracao(int id)
        {
            string urlRedireciona = string.Empty;

            if (_busConfiguracao.ExcluirTipoInfracao(id))
            {
                urlRedireciona = Url.Action("ConfigurarTipoInfracao", "Fiscalizacao", new { id = id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Penalidade



        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPenalidade })]
        public ActionResult AlterarSituacaoPenalidade(int tipoId, int situacaoNova)
        {
            _busConfiguracao.AlterarSituacaoPenalidade(tipoId, situacaoNova);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "FiscalizacaoPenalidade", new PenalidadeVM(_busConfiguracao.ObterPenalidades(), "", "", ""));
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = html }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPenalidade })]
        public ActionResult ExcluirPenalidadeConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            List<Penalidade> itens = _busConfiguracao.ObterPenalidades();

            String nome = itens.FirstOrDefault(x => x.Id == id.ToString()).Artigo;

            vm.Id = id;
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirPenaliadadeMensagem(nome);
            vm.Titulo = "Excluir Penalidade";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPenalidade })]
        public ActionResult ExcluirPenalidade(int id)
        {
            string urlRedireciona = string.Empty;

            if (_busConfiguracao.ExcluirPenalidade(id))
            {
                urlRedireciona = Url.Action("ConfigurarPenalidade", "Fiscalizacao", new { id = id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPenalidade })]
        public ActionResult ConfigurarPenalidade()
        {
            PenalidadeVM vm = new PenalidadeVM(_busConfiguracao.ObterPenalidades(), "","","");
            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPenalidade })]
        public ActionResult ConfigurarPenalidade(List<Penalidade> entidade)
        {
            _busConfiguracao.SalvarPenalidade(entidade);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "FiscalizacaoPenalidade", new PenalidadeVM(_busConfiguracao.ObterPenalidades(), "", "", ""));

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Html = html,
                @UrlRedirecionar = Url.Action("ConfiguracaoIndex", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion  

        #region Item

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarItem })]
		public ActionResult ConfigurarItem()
		{
			ItemInfracaoVM vm = new ItemInfracaoVM(_busConfiguracao.ObterItemInfracao());
			return View(vm);
		}

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarItem })]
        public ActionResult ConfigurarItem(Item entidade)
        {
            _busConfiguracao.SalvarItemInfracao(entidade);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new ItemInfracaoVM(_busConfiguracao.ObterItemInfracao()));

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Html = html,
                @UrlRedirecionar = Url.Action("ConfiguracaoIndex", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarItem })]
        public ActionResult ExcluirItemInfracaoConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            List<Item> itens = _busConfiguracao.ObterItemInfracao();

            String nome = itens.FirstOrDefault(x => x.Id == id.ToString()).Texto;

            vm.Id = id;
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirItemInfracaoMensagem(nome);
            vm.Titulo = "Excluir Item";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarItem })]
        public ActionResult ExcluirItemInfracao(int id)
        {
            string urlRedireciona = string.Empty;

            if (_busConfiguracao.ExcluirItemInfracao(id))
            {
                urlRedireciona = Url.Action("ConfigurarItem", "Fiscalizacao", new { id = id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion

        #region SubItem

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarSubitem })]
        public ActionResult ConfigurarSubItem()
        {
            SubItemInfracaoVM vm = new SubItemInfracaoVM(_busConfiguracao.ObterSubItemInfracao());
            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarSubitem })]
        public ActionResult ConfigurarSubItem(Item entidade)
        {
            _busConfiguracao.SalvarSubItemInfracao(entidade);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new SubItemInfracaoVM(_busConfiguracao.ObterSubItemInfracao()));

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Html = html,
                @UrlRedirecionar = Url.Action("ConfiguracaoIndex", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarSubitem })]
        public ActionResult ExcluirSubItemInfracaoConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            List<Item> itens = _busConfiguracao.ObterSubItemInfracao();

            String nome = itens.FirstOrDefault(x => x.Id == id.ToString()).Texto;

            vm.Id = id;
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirSubItemInfracaoMensagem(nome);
            vm.Titulo = "Excluir SubItem de infração";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarSubitem })]
        public ActionResult ExcluirSubItemInfracao(int id)
        {
            string urlRedireciona = string.Empty;

            if (_busConfiguracao.ExcluirSubItemInfracao(id))
            {
                urlRedireciona = Url.Action("ConfigurarSubItem", "Fiscalizacao", new { id = id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion

        #region Campo

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarCampo })]
        public ActionResult ConfigurarCampo()
        {
            CampoInfracaoVM vm = new CampoInfracaoVM(_busConfiguracao.ObterCampoInfracao(), _busLista.ConfiguracaoFiscalizacaoCamposTipo, _busLista.ConfiguracaoFiscalizacaoCamposUnidade);
            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarCampo })]
        public ActionResult ConfigurarCampo(Item entidade)
        {
            _busConfiguracao.SalvarCampoInfracao(entidade);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "FiscalizacaoCampos", new CampoInfracaoVM(_busConfiguracao.ObterCampoInfracao(), new List<Lista>(), new List<Lista>()));

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Html = html,
                @UrlRedirecionar = Url.Action("ConfiguracaoIndex", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);

        }

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarCampo })]
        public ActionResult ExcluirCampoInfracaoConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            List<Item> itens = _busConfiguracao.ObterCampoInfracao();

            String nome = itens.FirstOrDefault(x => x.Id == id.ToString()).Texto;

            vm.Id = id;
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirCampoInfracaoMensagem(nome);
            vm.Titulo = "Excluir Campo";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarCampo })]
        public ActionResult ExcluirCampoInfracao(int id)
        {
            string urlRedireciona = string.Empty;

            if (_busConfiguracao.ExcluirCampoInfracao(id))
            {
                urlRedireciona = Url.Action("ConfigurarCampo", "Fiscalizacao", new { id = id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Resposta

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarResposta })]
        public ActionResult ConfigurarResposta()
        {
            RespostaInfracaoVM vm = new RespostaInfracaoVM(_busConfiguracao.ObterRespostaInfracao());
            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarResposta })]
        public ActionResult ConfigurarResposta(Item entidade)
        {
            _busConfiguracao.SalvarRespostaInfracao(entidade);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new RespostaInfracaoVM(_busConfiguracao.ObterRespostaInfracao()));

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Html = html,
                @UrlRedirecionar = Url.Action("ConfiguracaoIndex", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);
        }

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarResposta })]
        public ActionResult ExcluirRespostaInfracaoConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            List<Item> itens = _busConfiguracao.ObterRespostaInfracao();

            String nome = itens.FirstOrDefault(x => x.Id == id.ToString()).Texto;

            vm.Id = id;
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirRespostaInfracaoMensagem(nome);
            vm.Titulo = "Excluir Resposta de infração";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarResposta })]
        public ActionResult ExcluirRespostaInfracao(int id)
        {
            string urlRedireciona = string.Empty;

            if (_busConfiguracao.ExcluirRespostaInfracao(id))
            {
                urlRedireciona = Url.Action("ConfigurarResposta", "Fiscalizacao", new { id = id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Pergunta

        #region Filtrar

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult ConfigurarPerguntasListar()
        {
            PerguntaListarVM vm = new PerguntaListarVM(_busLista.QuantPaginacao, _busConfiguracao.ObterPerguntaInfracao(), _busConfiguracao.ObterRespostaInfracao());
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            return PartialView(vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult ConfigurarPerguntasFiltrar(PerguntaListarVM vm, Paginacao paginacao)
        {
            if (!String.IsNullOrEmpty(vm.UltimaBusca))
            {
                vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PerguntaListarVM>(vm.UltimaBusca).Filtros;
            }

            vm.Paginacao = paginacao;
            vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
            vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
            vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

            Resultados<PerguntaInfracaoListarResultado> resultados = _busConfiguracao.PerguntasFiltrar(vm.Filtros, vm.Paginacao);
            if (resultados == null)
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
            }

            vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
            vm.Paginacao.EfetuarPaginacao();
            vm.Resultados = resultados.Itens;

            return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ConfigurarPerguntasListarResultados", vm) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult ConfigurarPergunta(int? id)
        {
            PerguntaInfracao entidade = _busConfiguracao.ObterPerguntaRespostasInfracao(id.GetValueOrDefault(0)) ?? new PerguntaInfracao();

            if (entidade.Id > 0 && entidade.SituacaoId == 0)
            {
                Validacao.Add(Mensagem.FiscalizacaoConfiguracao.EditarPerguntaInfracaoDesativado);
                return RedirectToAction("ConfigurarPerguntasListar", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            PerguntaInfracaoVM vm = new PerguntaInfracaoVM(entidade, _busConfiguracao.ObterRespostaInfracao());

            return View(vm);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult ConfigurarPerguntaVisualizar(int id)
        {
            PerguntaInfracaoVM vm = new PerguntaInfracaoVM(_busConfiguracao.ObterPerguntaRespostasInfracao(id), _busConfiguracao.ObterRespostaInfracao(), true);
            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult ConfigurarPergunta(PerguntaInfracao entidade)
        {
            _busConfiguracao.SalvarPerguntaInfracao(entidade);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @UrlRedirecionar = Url.Action("ConfigurarPerguntasListar", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);

        }

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult ExcluirPerguntaInfracaoConfirm(int id)
        {
            ExcluirVM vm = new ExcluirVM();
            List<Item> itens = _busConfiguracao.ObterPerguntaInfracao();

            String nome = itens.FirstOrDefault(x => x.Id == id.ToString()).Texto;

            vm.Id = id;
            vm.Mensagem = Mensagem.FiscalizacaoConfiguracao.ExcluirPerguntaInfracaoMensagem("\"" + nome + "\" ");
            vm.Titulo = "Excluir Pergunta";

            return PartialView("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult ExcluirPerguntaInfracao(int id)
        {
            string urlRedireciona = string.Empty;

            if (_busConfiguracao.ExcluirPerguntaInfracao(id))
            {
                urlRedireciona = Url.Action("ConfigurarPerguntas", "Fiscalizacao", new { id = id, Msg = Validacao.QueryParam() });
            }

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, urlRedireciona = urlRedireciona }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Produtos Apreendidos/Destinação

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarProdutosDestinacao })]
        public ActionResult ConfigurarProdutosDestinacao()
        {
            ProdutoDestinacaoVM vm = new ProdutoDestinacaoVM();
            vm.ListaProdutos = _busConfiguracao.ObterProdutosApreendidos();
            vm.ListaDestinos = _busConfiguracao.ObterDestinacao();

            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarProdutosDestinacao })]
        public ActionResult ConfigurarProdutosDestinacao(List<ProdutoApreendido> listaProdutos, List<DestinacaoProduto> listaDestinos)
        {
            if (listaProdutos == null)
            {
                listaProdutos = new List<ProdutoApreendido>();
            }

            if (listaDestinos == null)
            {
                listaDestinos = new List<DestinacaoProduto>();
            }

            _busConfiguracao.SalvarProdutosDestinacao(listaProdutos, listaDestinos);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Url = Url.Action("ConfigurarProdutosDestinacao", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion Produtos Apreendidos/Destinação

        #region Códigos da Receita

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarCodigoReceita })]
        public ActionResult ConfigurarCodigosReceita()
        {
            CodigosReceitaVM vm = new CodigosReceitaVM();
            vm.ListaCodigosReceita = _busConfiguracao.ObterCodigosReceita();

            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarProdutosDestinacao })]
        public ActionResult ConfigurarCodigosReceita(List<CodigoReceita> listaCodigosReceita)
        {
            if (listaCodigosReceita == null)
            {
                listaCodigosReceita = new List<CodigoReceita>();
            }

            _busConfiguracao.SalvarCodigosReceita(listaCodigosReceita);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Url = Url.Action("ConfigurarCodigosReceita", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarProdutosDestinacao })]
        public ActionResult ExcluirCodigoReceita(CodigoReceita codigoExcluido)
        {
            if (codigoExcluido != null && codigoExcluido.Id != 0)
            {
                _busConfiguracao.PermiteExcluirCodigo(codigoExcluido);
            }

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @Url = Url.Action("ConfigurarCodigosReceita", "Fiscalizacao", new { Msg = Validacao.QueryParam() })
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion Códigos da Receita

        #region Auxiliares

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarTipoInfracao })]
        public ActionResult AlterarSituacaoTipoInfracao(int tipoId, int situacaoNova)
        {
            _busConfiguracao.AlterarSituacaoTipoInfracao(tipoId, situacaoNova);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new TipoInfracaoVM(_busConfiguracao.ObterTipoInfracao()));
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = html }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarItem })]
        public ActionResult AlterarSituacaoItemInfracao(int tipoId, int situacaoNova)
        {
            _busConfiguracao.AlterarSituacaoItemInfracao(tipoId, situacaoNova);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new TipoInfracaoVM(_busConfiguracao.ObterItemInfracao()));
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = html }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarSubitem })]
        public ActionResult AlterarSituacaoSubItemInfracao(int tipoId, int situacaoNova)
        {
            _busConfiguracao.AlterarSituacaoSubItemInfracao(tipoId, situacaoNova);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new TipoInfracaoVM(_busConfiguracao.ObterSubItemInfracao()));
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = html }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarCampo })]
        public ActionResult AlterarSituacaoCampoInfracao(int tipoId, int situacaoNova)
        {
            _busConfiguracao.AlterarSituacaoCampoInfracao(tipoId, situacaoNova);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "FiscalizacaoCampos", new CampoInfracaoVM(_busConfiguracao.ObterCampoInfracao(), new List<Lista>(), new List<Lista>()));
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = html }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarResposta })]
        public ActionResult AlterarSituacaoRespostaInfracao(int tipoId, int situacaoNova)
        {
            _busConfiguracao.AlterarSituacaoRespostaInfracao(tipoId, situacaoNova);
            String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "InfracaoItens", new RespostaInfracaoVM(_busConfiguracao.ObterRespostaInfracao()));
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Html = html }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarPergunta })]
        public ActionResult AlterarSituacaoPerguntaInfracao(int tipoId, int situacaoNova)
        {
            _busConfiguracao.AlterarSituacaoPerguntaInfracao(tipoId, situacaoNova);

            return Json(new
                {
                    @EhValido = Validacao.EhValido,
                    @Msg = Validacao.Erros,
                }, JsonRequestBehavior.AllowGet);
        }


        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarTipoInfracao })]
        public ActionResult PodeEditarTipoInfracao(int id)
        {
            _busConfiguracao.PodeEditarTipoInfracao(id);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarItem })]
        public ActionResult PodeEditarItemInfracao(int id)
        {
            _busConfiguracao.PodeEditarItemInfracao(id);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarSubitem })]
        public ActionResult PodeEditarSubItemInfracao(int id)
        {
            _busConfiguracao.PodeEditarSubItemInfracao(id);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarCampo })]
        public ActionResult PodeEditarCampoInfracao(int id)
        {
            _busConfiguracao.PodeEditarCampoInfracao(id);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        [Permite(RoleArray = new Object[] { ePermissao.ConfigurarResposta })]
        public ActionResult PodeEditarRespostaInfracao(int id)
        {
            _busConfiguracao.PodeEditarRespostaInfracao(id);
            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Passivo de Pdfs da Fiscalizacao

        public FileStreamResult GerarPassivoConcluido()
        {
            PdfFiscalizacao pdf = new PdfFiscalizacao();
            FileStreamResult result = new FileStreamResult(pdf.GerarPassivoZip(), "application / pdf");

            return result;
        }

        public FileStreamResult GerarPassivoTarja()
        {
            PdfFiscalizacao pdf = new PdfFiscalizacao();
            FileStreamResult result = new FileStreamResult(pdf.GerarPassivoTarjaZip(), "application / pdf");

            return result;
        }

        public string AtualizarPassivoConcluido()
        {

            PdfFiscalizacao pdf = new PdfFiscalizacao();
            pdf.GerarPassivo();

            return "Gerar com sucesso";
        }

        public class FiscComparer<T> : IEqualityComparer<T>
        {
            Func<T, T, bool> _equalFunc = null;

            public FiscComparer(Func<T, T, bool> EqualsFunc)
            {
                _equalFunc = EqualsFunc;
            }

            public bool Equals(T x, T y)
            {
                return _equalFunc(x, y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        public ActionResult ObterPassivoConcluidoIds()
        {
            FiscalizacaoDa da = new FiscalizacaoDa();
            List<RelFiscalizacaoLib.FiscalizacaoRelatorio> lstFiscalizacao = da.ObterHistoricoConcluidos();

            var lst = lstFiscalizacao.Select(x => new { Id = x.Id, HistoricoId = x.HistoricoId }).ToList();

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Acompanhamentos

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoListar })]
        public ActionResult Acompanhamentos(int id)
        {
            Fiscalizacao fiscalizacao = _bus.Obter(id, true);

            if (!_validarAcompanhamento.Acompanhamentos(fiscalizacao))
            {
                return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            AcompanhamentosVM vm = new AcompanhamentosVM();

            vm.fiscalizacaoId = id;
            vm.Resultados = _busAcompanhamento.ObterAcompanhamentos(id).Itens;
            vm.PodeVisualizar = User.IsInRole(ePermissao.AcompanhamentoVisualizar.ToString());
            vm.PodeCriar = User.IsInRole(ePermissao.AcompanhamentoCriar.ToString());
            vm.PodeEditar = User.IsInRole(ePermissao.AcompanhamentoEditar.ToString());
            vm.PodeExcluir = User.IsInRole(ePermissao.AcompanhamentoExcluir.ToString());
            vm.PodeAlterarSituacao = User.IsInRole(ePermissao.AcompanhamentoAlterarSituacao.ToString());

            return View(vm);
        }

        #region Criar

        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoCriar })]
        public ActionResult AcompanhamentoCriar(int fiscalizacaoId)
        {
            if (!_validarAcompanhamento.ValidarAcesso(_bus.Obter(fiscalizacaoId, true)))
            {
                return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            Acompanhamento acompanhamento = new Acompanhamento(_bus.Obter(fiscalizacaoId));
            acompanhamento.AgenteId = Usuario.EtramiteIdentity.FuncionarioId;
            acompanhamento.AgenteNome = Usuario.EtramiteIdentity.Name;

            AcompanhamentoVM vm = new AcompanhamentoVM(acompanhamento, _busFuncionario.ObterSetoresFuncionario(), _busLista.AcompanhamentoFiscalizacaoReservaLegalTipo, _busLista.AcompanhamentoFiscalizacaoCaracteristicaSolo);

            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoCriar })]
        public ActionResult AcompanhamentoCriar(Acompanhamento acompanhamento)
        {
            if (!_validarAcompanhamento.ValidarAcesso(_bus.Obter(acompanhamento.FiscalizacaoId, true)))
            {
                return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            _busAcompanhamento.Salvar(acompanhamento);

            string urlRedirecionar = Url.Action("AcompanhamentoCriar", "Fiscalizacao", new { fiscalizacaoId = acompanhamento.FiscalizacaoId });
            urlRedirecionar += "&Msg=" + Validacao.QueryParam() + "&acaoId=" + acompanhamento.Id.ToString();

            return Json(new { @EhValido = Validacao.EhValido, @AcompanhamentoId = acompanhamento.Id, @Msg = Validacao.Erros, @UrlRedirecionar = urlRedirecionar }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Editar

        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoEditar })]
        public ActionResult AcompanhamentoEditar(int id)
        {
            Acompanhamento acompanhamento = _busAcompanhamento.Obter(id);

            if (!_validarAcompanhamento.ValidarAcesso(_bus.Obter(acompanhamento.FiscalizacaoId, true)))
            {
                return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            if (!_validarAcompanhamento.PodeEditar(acompanhamento))
            {
                return RedirectToAction("Acompanhamentos", "Fiscalizacao", Validacao.QueryParamSerializer(new { id = acompanhamento.FiscalizacaoId }));
            }

            AcompanhamentoVM vm = new AcompanhamentoVM(acompanhamento, _busFuncionario.ObterSetoresFuncionario(), _busLista.AcompanhamentoFiscalizacaoReservaLegalTipo, _busLista.AcompanhamentoFiscalizacaoCaracteristicaSolo);
            return View(vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoEditar })]
        public ActionResult AcompanhamentoEditar(Acompanhamento acompanhamento)
        {
            if (!_validarAcompanhamento.ValidarAcesso(_bus.Obter(acompanhamento.FiscalizacaoId, true)))
            {
                return RedirectToAction("Index", "Fiscalizacao", Validacao.QueryParamSerializer());
            }

            _busAcompanhamento.Salvar(acompanhamento);

            string urlRedirecionar = Url.Action("Acompanhamentos", "Fiscalizacao", new { id = acompanhamento.FiscalizacaoId });
            urlRedirecionar += "?Msg=" + Validacao.QueryParam() + "&acaoId=" + acompanhamento.Id.ToString() + "&editar=1";

            return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlRedirecionar = urlRedirecionar }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Visualizar

        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoVisualizar })]
        public ActionResult AcompanhamentoVisualizar(int id)
        {
            AcompanhamentoVM vm = new AcompanhamentoVM(_busAcompanhamento.Obter(id), _busFuncionario.ObterSetoresFuncionario(), _busLista.AcompanhamentoFiscalizacaoReservaLegalTipo, _busLista.AcompanhamentoFiscalizacaoCaracteristicaSolo, isVisualizar: true);
            return View(vm);
        }

        #endregion

        #region Excluir

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoExcluir })]
        public ActionResult AcompanhamentoExcluirConfirm(int id)
        {
            Acompanhamento acompanhamento = _busAcompanhamento.Obter(id, true);

            ExcluirVM vm = new ExcluirVM();
            vm.Id = id;
            vm.Titulo = "Excluir Acompanhamento da Fiscalização";
            vm.Mensagem = Mensagem.Acompanhamento.ExcluirConfirmacao(acompanhamento.Numero);
            return View("Excluir", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoExcluir })]
        public ActionResult AcompanhamentoExcluir(int id)
        {
            AcompanhamentoBus acompanhamentoBus = new AcompanhamentoBus();
            acompanhamentoBus.Excluir(id);
            return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
        }

        #endregion

        #region Alterar Situação

        [HttpGet]
        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoAlterarSituacao })]
        public ActionResult AcompanhamentoAlterarSituacao(int id)
        {
            Acompanhamento acompanhamento = _busAcompanhamento.Obter(id, true);
            AcompanhamentoAlterarSituacaoVM vm = new AcompanhamentoAlterarSituacaoVM(
                _busAcompanhamento.ObterSituacoes((eAcompanhamentoSituacao)acompanhamento.SituacaoId, _busLista.AcompanhamentoFiscalizacaoSituacao));

            AcompanhamentoValidar acompanhamentoValidar = new AcompanhamentoValidar();
            if (!acompanhamentoValidar.PodeAlterarSituacao(acompanhamento, _bus.Obter(acompanhamento.FiscalizacaoId)))
            {
                return RedirectToAction("Acompanhamentos", "Fiscalizacao", new { id = acompanhamento.FiscalizacaoId, Msg = Validacao.QueryParam() });
            }

            vm.Acompanhamento = acompanhamento;
            return View("AcompanhamentoAlterarSituacao", vm);
        }

        [HttpPost]
        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoAlterarSituacao })]
        public ActionResult AcompanhamentoAlterarSituacao(Acompanhamento acompanhamento)
        {
            _busAcompanhamento.AlterarSituacao(acompanhamento);

            return Json(new
            {
                @EhValido = Validacao.EhValido,
                @Msg = Validacao.Erros,
                @UrlRedirecionar = Url.Action("Acompanhamentos", "Fiscalizacao", new { id = acompanhamento.FiscalizacaoId, Msg = Validacao.QueryParam(), acaoId = acompanhamento.Id.ToString() })
            });
        }

        #endregion Alterar Situação

        #region PDF

        [Permite(RoleArray = new Object[] { ePermissao.AcompanhamentoVisualizar, ePermissao.AcompanhamentoEditar, ePermissao.AcompanhamentoCriar })]
        public ActionResult LaudoAcompanhamentoFiscalizacaoPdf(int id)
        {
            try
            {
                Stream strArquivo = _busAcompanhamento.LaudoAcompanhamentoFiscalizacaoPdf(id);

                if (!Validacao.EhValido)
                {
                    return RedirectToAction("Index", Validacao.QueryParamSerializer());
                }
                return ViewModelHelper.GerarArquivo("Laudo de Acompanhamento de Fiscalizacao.pdf", strArquivo, ContentType.PDF);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
                return RedirectToAction("Index", Validacao.QueryParamSerializer());
            }
        }

        #endregion

        #endregion Acompanhamentos
    }
}
