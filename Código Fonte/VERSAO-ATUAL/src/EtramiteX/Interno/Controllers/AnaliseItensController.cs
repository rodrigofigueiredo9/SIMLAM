using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.AnaliseItens.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade;
using ProjetoDigitalCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business.ProjetoDigitalCredenciadoBus;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AnaliseItensController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		RequerimentoBus _busRequerimento = new RequerimentoBus(); 
		AnaliseItensBus _bus = new AnaliseItensBus(new AnaliseItensValidar());
		AnaliseItensValidar _validar = new AnaliseItensValidar();
		EmpreendimentoBus _busEmpreendimento = new EmpreendimentoBus();
		ProtocoloBus _protocoloBus = new ProtocoloBus();
		FuncionarioBus _busFuncionario = new FuncionarioBus();
		PecaTecnicaValidar _validarPecaTenica = new PecaTecnicaValidar();
		PecaTecnicaBus _busPecaTecnica = new PecaTecnicaBus();
		ProjetoDigitalCredenciadoBus _busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();

		#endregion

		#region Index

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult Index()
		{
			return RedirectToAction("Analisar");
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult Analisar(int protocoloId = 0, int requerimentoId = 0)
		{
			AnaliseItemVM vm = new AnaliseItemVM();

			if (protocoloId == 0 || requerimentoId == 0)
			{
				return View(vm);
			}

			IProtocolo protocolo = _protocoloBus.Obter(protocoloId);
			AnaliseItem analise = _bus.VerificarProtocolo(protocolo.Numero);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Analisar", "AnaliseItens", Validacao.QueryParamSerializer());
			}
			
			vm.Requerimentos = analise.Requerimentos;
			vm.RequerimentoSelecionado = requerimentoId;
			vm.ProjetoDigitalId = _busProjetoDigitalCredenciado.ObterProjetoDigitalId(requerimentoId);
			vm.ProtocoloId = protocoloId;
			vm.ProtocoloNumero = protocolo.Numero;

			Requerimento req = vm.Requerimentos.FirstOrDefault(x => x.Id == requerimentoId);

			AnaliseItem analiseAux = _bus.ObterPorChecagem(req.Checagem);
			if (analiseAux != null && analiseAux.Id > 0)
			{
				analise.Id = analiseAux.Id;
				analise.Situacao = analiseAux.Situacao;
			}

			if (_validar.ValidarProtocoloAnalisar(req.Checagem, req.Id, protocolo.IsProcesso))
			{

				if (_validar.Analise(analise))
				{
					// Caso não exista a analise na entrada no metodo abaixo a mesma vai ser criada
					analise = _bus.ObterAnaliseProtocolo(req, protocolo.Id.GetValueOrDefault(), false);
					vm.AnaliseId = analise.Id;

					vm.CheckListId = req.Checagem;
					vm.Atualizado = false;
					vm.Roteiros = analise.Roteiros;

					analise.Itens.ForEach(x => x.Analista = string.Empty);
					vm.ListarItens = analise.Itens;

				}
			}

			vm.Situacao = analise.Situacao;
			vm.UrlsCaracterizacoes = ObterUrlsCaracterizacoes(_busCaracterizacao.ObterCaracterizacoes(vm.ProjetoDigitalId));

			return View(vm);
		}

		#endregion

		#region Criar/Salvar

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult Salvar(AnaliseVM vm, bool atualizarRoteiro = false)
		{
			AnaliseItem analise = new AnaliseItem();

			analise.Id = vm.Id;

			analise.Checagem.Id = vm.ChecagemId;

			if (vm.ProtocoloId > 0)
			{
				analise.Protocolo.Id = vm.ProtocoloId;
			}

			analise.Itens = vm.Itens;
			analise.Roteiros = vm.Roteiros;
			analise.ProtocoloPai = vm.ProtocoloPai;

			_bus.Salvar(analise, atualizarRoteiro);

			analise.Itens.ForEach(x => x.Analista = string.Empty);

			return Json(new { @Itens = analise.Itens, @Msg = Validacao.Erros, AnaliseId = analise.Id }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult CriarModalAtualizarRoteiro(int checagemId)
		{
			ResultadoMergeItemVM merge = new ResultadoMergeItemVM();
			merge.MergeItens = _bus.MergeItens(checagemId);

			return PartialView("ModalAtualizarRoteiro", merge);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult CriarItemAnalise(ItemAnaliseVME item)
		{
			return PartialView("AdicionarItemAnalise", item);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult AtividadesSolicitadas(RequerimentoAnaliseVME requerimento)
		{
			IProtocolo protocolo = _protocoloBus.ObterAtividadesProtocolo(requerimento.ProtocoloId) as IProtocolo;
			ListarAtividadesSolicitadasVM vm = new ListarAtividadesSolicitadasVM(_busLista.TiposProcesso, _busLista.TiposDocumento, protocolo, protocolo.Tipo.Id);

			vm.Protocolo = protocolo;
			vm.IsEncerrar = false;

			return PartialView("AtividadesSolicitadaAnalise", vm);
		}

		#endregion

		#region Importar Dados

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult ImportarProjetoDigital(int analiseId, int requerimentoId, int protocoloId)
		{
			int projetoDigitalId = _busProjetoDigitalCredenciado.ObterProjetoDigitalId(requerimentoId);

			bool isImportado = _bus.ImportarProjetoDigital(analiseId, projetoDigitalId);
			
			string urlRedireciona = Url.Action("Analisar", "AnaliseItens", new { @protocoloId = protocoloId, @requerimentoId = requerimentoId }) + "&Msg=" + Validacao.QueryParam() + "&acaoId=" + analiseId.ToString();

			return Json(new { @Msg = Validacao.Erros, @IsImportado = true.ToString().ToLower(), @Url = urlRedireciona }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region  PDF's

		public ActionResult GerarPdf(int id)
		{
			try
			{
				AnaliseItem analise = _bus.Obter(id);
				if (!_validar.Salvar(analise))
				{
					return RedirectToAction("Analisar", "AnaliseItens", Validacao.QueryParamSerializer());
				}

				return ViewModelHelper.GerarArquivoPdf(new PdfAnalise().GerarPDFAnalise(id), "Analise de Itens");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarPdfAnaliseProcesso(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfAnalise().GerarPDFAnaliseProtocolo(id, 1), "Analise de Itens");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarPdfAnaliseDocumeto(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfAnalise().GerarPDFAnaliseProtocolo(id, 2), "Analise de Itens");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult ObterRequerimentos(string numero)
		{
			AnaliseItemVM vm = new AnaliseItemVM();

			AnaliseItem analise = _bus.VerificarProtocolo(numero);

			vm.Requerimentos = analise.Requerimentos;

			return Json(new
			{
				@IsProcesso = analise.IsProcesso,
				@ProtocoloId = analise.Protocolo.Id,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "Requerimentos", vm),
				Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult ObterVersaoRoteiros(RequerimentoAnaliseVME objeto)
		{
			_validar.ValidarVersaoRoteiro(objeto.ChecagemId);

			return Json(new { @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult ObterAnalisePartial(RequerimentoAnaliseVME objeto)
		{
			AnaliseItemVM vm = new AnaliseItemVM();
			AnaliseItem analise = new AnaliseItem();

			if (_validar.ValidarProtocoloAnalisar(objeto.ChecagemId, objeto.NumeroRequerimento, objeto.IsProcesso))
			{
				analise = _bus.ObterPorChecagem(objeto.ChecagemId) ?? new AnaliseItem();
				analise.Protocolo.Id = objeto.ProtocoloId;

				if (_validar.Analise(analise))
				{
					Requerimento req = _busRequerimento.Obter(objeto.NumeroRequerimento);
					req.Checagem = objeto.ChecagemId;
					vm.RequerimentoSelecionado = req.Id;
					vm.Requerimentos.Add(req);

					// Caso não exista a analise na entrada no metodo abaixo a mesma vai ser criada
					analise = _bus.ObterAnaliseProtocolo(req, objeto.ProtocoloId, objeto.Atualizar);
					analise.Requerimentos = vm.Requerimentos;

					vm.CheckListId = objeto.ChecagemId;
					vm.Atualizado = objeto.Atualizar;
					vm.Roteiros = analise.Roteiros;

					analise.Itens.ForEach(x => x.Analista = string.Empty);

					vm.ListarItens = analise.Itens;
					vm.Situacao = analise.Situacao;
					vm.ProjetoDigitalId = _busProjetoDigitalCredenciado.ObterProjetoDigitalId(req.Id);

					vm.ProtocoloId = objeto.ProtocoloId;
				}
				else
				{
					vm.Situacao = analise.Situacao;
				}
			}

			if (vm.ProjetoDigitalId > 0)
			{
				vm.UrlsCaracterizacoes = ObterUrlsCaracterizacoes(_busCaracterizacao.ObterCaracterizacoes(vm.ProjetoDigitalId));
			}

			List<Mensagem> info = Validacao.Erros.FindAll(x => x.Tipo == eTipoMensagem.Informacao);
			List<Mensagem> erro = Validacao.Erros.FindAll(x => x.Tipo != eTipoMensagem.Informacao);

			return Json(new { @Msg = erro, @MsgInfo = info, @Html = ((Validacao.EhValido) ? ViewModelHelper.RenderPartialViewToString(ControllerContext, "AnalisarPartial", vm) : ""), analiseId = analise.Id, projetoDigitalId = vm.ProjetoDigitalId }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult ObterHistoricoAnaliseItem(ItemAnaliseVME vm)
		{
			HistoricoAnalise hst = new HistoricoAnalise();
			hst.NomeHist = vm.Nome;
			hst.TipoHist = vm.Tipo;
			hst.Analises = _bus.ObterHistoricoAnalise(vm.Id, vm.ChecagemId);

			HistoricoAnaliseVME vme = new HistoricoAnaliseVME(hst);

			return PartialView("HistoricoAnaliseItem", vme);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public ActionResult ObterAnaliseItem(int id)
		{
			ItemAnaliseVME vm = new ItemAnaliseVME() { Tipo = id, DataAnalise = DateTime.Today.ToShortDateString() };
			vm.SetLista(_busLista.SituacoesItemAnalise);

			return PartialView("AnalisarItem", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.AnaliseItensCriar })]
		public String ObterUrlsCaracterizacoes(List<CaracterizacaoLst> caracterizacoes) 
		{
			List<dynamic> list = new List<dynamic>();
			caracterizacoes.ForEach(x => { list.Add(new { Tipo = x.Id, Url = Url.Action("VisualizarCredenciado", ((eCaracterizacao)x.Id).ToString(), new { Area = "Caracterizacoes" }) }); });

			return ViewModelHelper.Json(list);

		}

		#endregion

		#region Peça Tecnica

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.PecaTecnicaGerar })]
		public ActionResult GerarPecaTecnica()
		{
			PecaTecnicaVM vm = new PecaTecnicaVM(new PecaTecnica());

			return View(vm);

		}

		[Permite(RoleArray = new Object[] { ePermissao.PecaTecnicaGerar })]
		public ActionResult SalvarPecaTecnica(PecaTecnica pecaTecnica)
		{
			int pecaTecnicaId = _busPecaTecnica.Salvar(pecaTecnica);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Id = pecaTecnicaId });
		}

		#endregion

		#region PDF

		[Permite(RoleArray = new Object[] { ePermissao.PecaTecnicaGerar })]
		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarcroquipdf, Artefato = (int)eHistoricoArtefato.pecatecnica)]
		public ActionResult GerarPdfPecaTecnica(int id, int empreendimentoId)
		{
			try
			{
				Arquivo arquivo = null;
				ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);
				PdfPecaTecnica analisePdf = new PdfPecaTecnica();
				List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();
				ProjetoGeograficoBus projetoBus = new ProjetoGeograficoBus();

				arquivos = projetoBus.ObterArquivos(empreendimentoId, eCaracterizacao.Dominialidade, true);

				ArquivoProjeto ArquivoAux = arquivos.SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.CroquiFinal);
				int idArquivo = (ArquivoAux != null) ? ArquivoAux.Id.GetValueOrDefault() : 0;

				arquivo = arquivoBus.Obter(idArquivo);
				MemoryStream resultado = analisePdf.GerarPdf(arquivo, id);

				return ViewModelHelper.GerarArquivo("Pdf Peca Tecnica.pdf", resultado, "application/pdf", dataHoraControleAcesso: true);

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.PecaTecnicaGerar })]
		public ActionResult ObterPecaTecnicaRequerimentos(string numero)
		{
			PecaTecnica pecaTecnica = _busPecaTecnica.VerificarProtocolo(numero);

			pecaTecnica.ProtocoloPai = pecaTecnica.Protocolo.Id;

			PecaTecnicaVM vm = new PecaTecnicaVM(pecaTecnica);

			vm.Requerimentos = _protocoloBus.ObterProtocoloRequerimentos(pecaTecnica.Protocolo.Id.GetValueOrDefault());

			return Json(new
			{
				@IsProcesso = pecaTecnica.Protocolo.IsProcesso,
				@ProtocoloId = pecaTecnica.Protocolo.Id,
				@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "PecaTecnicaRequerimentos", vm),
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido
			}, JsonRequestBehavior.AllowGet);


		}

		[Permite(RoleArray = new Object[] { ePermissao.PecaTecnicaGerar })]
		public ActionResult ObterPecaTecnicaAtividades(int id)
		{

			PecaTecnicaVM vm = new PecaTecnicaVM(new PecaTecnica());

			vm.PecaTecnica.Protocolo = _protocoloBus.ObterAtividades(id);

			if (vm.PecaTecnica.Protocolo.Empreendimento.Id <= 0)
			{
				Validacao.Add(Mensagem.AnaliseItem.PecaTecnicaNaoPossuiEmpreendimento);
				return Json(new { EhValido = false, Msg = Validacao.Erros });
			}


			string html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "PecaTecnicaAtividades", vm);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, html = html });

		}

		[Permite(RoleArray = new Object[] { ePermissao.PecaTecnicaGerar })]
		public ActionResult ObterPecaTecnicaConteudo(int id, int protocoloId)
		{
			int pecaTecnicaId = _busPecaTecnica.ExistePecaTecnica(id, protocoloId);

			PecaTecnica pecaTecnica = pecaTecnicaId > 0 ? _busPecaTecnica.Obter(pecaTecnicaId) : new PecaTecnica();

			PecaTecnicaVM vm = new PecaTecnicaVM(pecaTecnica);

			List<int> tiposDestinatario = new List<int>(){
				(int)eEmpreendimentoResponsavelTipo.Proprietario,
				(int)eEmpreendimentoResponsavelTipo.Socio,
				(int)eEmpreendimentoResponsavelTipo.Herdeiro,
				(int)eEmpreendimentoResponsavelTipo.Posseiro
			};

			vm.PecaTecnica.Protocolo = _protocoloBus.ObterSimplificado(protocoloId);

			if (pecaTecnicaId > 0)
			{
				List<PessoaLst> responsaveisTecnicos;
				if (vm.PecaTecnica.ElaboradorTipoEnum == eElaboradorTipo.TecnicoIdaf)
				{
					vm.Setores = ViewModelHelper.CriarSelectList(_busFuncionario.ObterSetoresFuncionario());

					EtramiteIdentity func = User.Identity as EtramiteIdentity ?? new EtramiteIdentity("", "", "", null, "", 0, 0, "", "", 0, 0);
					responsaveisTecnicos = new List<PessoaLst>();
					responsaveisTecnicos.Add(new PessoaLst() { Texto = func.Name, Id = func.UsuarioId });
				}
				else
				{
					responsaveisTecnicos = _protocoloBus.ObterResponsaveisTecnicos(vm.PecaTecnica.Protocolo.Id.GetValueOrDefault());
				}

				vm.Elaboradores = ViewModelHelper.CriarSelectList(responsaveisTecnicos, itemTextoPadrao: responsaveisTecnicos.Count != 1);
			}

			List<PessoaLst> responsaveis = _busEmpreendimento.ObterResponsaveis(vm.PecaTecnica.Protocolo.Empreendimento.Id).Where(x => tiposDestinatario.Contains(x.VinculoTipo)).ToList();

			vm.RespEmpreendimento = ViewModelHelper.CriarSelectList<PessoaLst>(responsaveis, itemTextoPadrao: responsaveis.Count != 1);

			string html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "PecaTecnicaConteudo", vm);
			return Json(new { EhValido = true, Msg = Validacao.Erros, html = html });
		}

		[Permite(RoleArray = new Object[] { ePermissao.PecaTecnicaGerar })]
		public ActionResult ObterPecaTecnicaElaboradores(int registro, bool? tecIdaf, int setor)
		{
			List<PessoaLst> responsaveis = null;

			if (tecIdaf.GetValueOrDefault())
			{
				EtramiteIdentity func = User.Identity as EtramiteIdentity ?? new EtramiteIdentity("", "", "", null, "", 0, 0, "", "", 0, 0);
				responsaveis = new List<PessoaLst>();
				responsaveis = _busFuncionario.ObterFuncionariosPorSetorFuncao(setor, (int)eCargoCodigo.TecnicoDesenvolvimentoAgropecuarioTopografo);
			}
			else
			{
				responsaveis = _protocoloBus.ObterResponsaveisTecnicos(registro);
			}
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, lista = responsaveis, Setores = _busLista.Setores });
		}

		#endregion

		#endregion
	}
}