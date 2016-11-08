using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class TermoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		TituloBus _busTitulo = new TituloBus();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();
		ProtocoloBus _protocoloBus = new ProtocoloBus();
		EtramiteIdentity _usuarioLogado { get { return (HttpContext.User.Identity as EtramiteIdentity); } }

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult TermoAprovacaoMedicao(EspecificidadeVME especificidade)
		{
			TermoAprovacaoMedicaoBus _busTermo = new TermoAprovacaoMedicaoBus();
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			Titulo titulo = new Titulo();
			List<PessoaLst> destinatarios = new List<PessoaLst>();
			TermoAprovacaoMedicao termo = new TermoAprovacaoMedicao();

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);

				termo = _busTermo.Obter(especificidade.TituloId) as TermoAprovacaoMedicao;
			}

			if (especificidade.ProtocoloId > 0)
			{
				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = termo.Destinatario, Texto = termo.DestinatarioNomeRazao, IsAtivo = true });
				}
			}

			TermoAprovacaoMedicaoVM vm = new TermoAprovacaoMedicaoVM
			{
				Termo = termo,
				Destinatarios = ViewModelHelper.CriarSelectList<PessoaLst>(destinatarios, selecionado: termo.Destinatario.ToString()),
				Funcionario = new List<SelectListItem> { new SelectListItem 
															{
																Value = _usuarioLogado.FuncionarioId.ToString(),
																Selected = true,
																Text = _usuarioLogado.Name
															}
														},
				Tecnicos = ViewModelHelper.CriarSelectList<PessoaLst>(_protocoloBus.ObterResponsaveisTecnicos(especificidade.ProtocoloId), selecionado: termo.ResponsavelMedicao.ToString()),
				Setores = ViewModelHelper.CriarSelectList<Setor>(_busTitulo.ObterFuncionarioSetores(), selecionado: termo.SetorCadastro.ToString()),
				IsVisualizar = especificidade.IsVisualizar
			};

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/TermoAprovacaoMedicao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult TermoCPFARL(EspecificidadeVME especificidade)
		{
			TermoCPFARLBus bus = new TermoCPFARLBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			TermoCPFARL termo = new TermoCPFARL();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					termo = bus.Obter(especificidade.TituloId) as TermoCPFARL;
				}
				else
				{
					termo = bus.ObterHistorico(especificidade.TituloId, 0) as TermoCPFARL;
				}

				if (termo != null)
				{
					termo.Anexos = titulo.Anexos;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);
			}

			if (especificidade.ProtocoloId > 0)
			{
				if (_busEspecificidade.ExisteProcDocFilhoQueFoiDesassociado(especificidade.TituloId))
				{
					lstAtividades = new List<AtividadeSolicitada>();
					titulo.Atividades = new List<Atividade>();
				}
				else
				{
					lstAtividades = _busAtividade.ObterAtividadesLista(especificidade.AtividadeProcDocReq.ToProtocolo());
				}

				termo.Destinatarios.ForEach(x =>
				{
					destinatarios.Add(new PessoaLst() { Id = x.Id, Texto = x.Nome, IsAtivo = true });
				});

				if (!especificidade.IsVisualizar)
				{
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			TermoCPFARLVM vm = new TermoCPFARLVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				titulo.Condicionantes,
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/TermoCPFARL.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult TermoCPFARLR(EspecificidadeVME especificidade)
		{
			TermoCPFARLRBus bus = new TermoCPFARLRBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			TermoCPFARLR termo = new TermoCPFARLR();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					termo = bus.Obter(especificidade.TituloId) as TermoCPFARLR;
				}
				else
				{
					termo = bus.ObterHistorico(especificidade.TituloId, 0) as TermoCPFARLR;
				}

				if (termo != null)
				{
					termo.Anexos = titulo.Anexos;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);
			}

			if (especificidade.ProtocoloId > 0)
			{
				if (_busEspecificidade.ExisteProcDocFilhoQueFoiDesassociado(especificidade.TituloId))
				{
					lstAtividades = new List<AtividadeSolicitada>();
					titulo.Atividades = new List<Atividade>();
				}
				else
				{
					lstAtividades = _busAtividade.ObterAtividadesLista(especificidade.AtividadeProcDocReq.ToProtocolo());
				}

				termo.Destinatarios.ForEach(x =>
				{
					destinatarios.Add(new PessoaLst() { Id = x.Id, Texto = x.Nome, IsAtivo = true });
				});

				if (!especificidade.IsVisualizar)
				{
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			TermoCPFARLRVM vm = new TermoCPFARLRVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				titulo.Condicionantes,
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
				vm.TituloAnterior = bus.ObterTituloAnterior(atividadeSelecionada, especificidade.ProtocoloId) ?? new TermoCPFARLRTituloAnterior();
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/TermoCPFARLR.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult TermoCPFARLC(EspecificidadeVME especificidade)
		{
			TermoCPFARLCBus bus = new TermoCPFARLCBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			TermoCPFARLC termo = new TermoCPFARLC();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades.First().Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					termo = bus.Obter(especificidade.TituloId) as TermoCPFARLC;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);
			}

			if (especificidade.ProtocoloId > 0)
			{
				if (_busEspecificidade.ExisteProcDocFilhoQueFoiDesassociado(especificidade.TituloId))
				{
					lstAtividades = new List<AtividadeSolicitada>();
					titulo.Atividades = new List<Atividade>();
				}
				else
				{
					lstAtividades = _busAtividade.ObterAtividadesLista(especificidade.AtividadeProcDocReq.ToProtocolo());
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			TermoCPFARLCVM vm = new TermoCPFARLCVM(
				lstProcessosDocumentos,
				lstAtividades,
				titulo.Condicionantes,
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				EmpreendimentoBus empreendimentoBus = new EmpreendimentoBus();
				DominialidadeBus dominialidadeBus = new DominialidadeBus();
				vm.Atividades.Atividades = titulo.Atividades;

				vm.CedenteDominios = ViewModelHelper.CriarSelectList(dominialidadeBus.ObterDominiosLista(especificidade.EmpreendimentoId), selecionado: termo.CedenteDominioID.ToString());
				vm.CedenteReservas = ViewModelHelper.CriarSelectList(dominialidadeBus.ObterARLCompensacaoDominio(termo.CedenteDominioID));
				vm.CedenteResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(empreendimentoBus.ObterResponsaveisComTipo(especificidade.EmpreendimentoId));

				vm.ReceptorEmpreendimentos = ViewModelHelper.CriarSelectList(
					new List<Lista>() { new Lista() { Id = termo.ReceptorEmpreendimentoID.ToString(), Texto = termo.ReceptorEmpreendimentoDenominador } },
					selecionado: termo.ReceptorEmpreendimentoID.ToString());
				vm.ReceptorDominios = ViewModelHelper.CriarSelectList(dominialidadeBus.ObterDominiosLista(termo.ReceptorEmpreendimentoID), selecionado: termo.ReceptorDominioID.ToString());
				vm.ReceptorResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(empreendimentoBus.ObterResponsaveisComTipo(termo.ReceptorEmpreendimentoID));
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/TermoCPFARLC.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult TermoCPFARLCR(EspecificidadeVME especificidade)
		{
			TermoCPFARLCRBus bus = new TermoCPFARLCRBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			TermoCPFARLCR termo = new TermoCPFARLCR();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades.First().Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					termo = bus.Obter(especificidade.TituloId) as TermoCPFARLCR;
				}
				else
				{
					//termo = bus.ObterHistorico(especificidade.TituloId, 0) as TermoCPFARLCR;
					termo = bus.Obter(especificidade.TituloId) as TermoCPFARLCR;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);
			}

			if (especificidade.ProtocoloId > 0)
			{
				if (_busEspecificidade.ExisteProcDocFilhoQueFoiDesassociado(especificidade.TituloId))
				{
					lstAtividades = new List<AtividadeSolicitada>();
					titulo.Atividades = new List<Atividade>();
				}
				else
				{
					lstAtividades = _busAtividade.ObterAtividadesLista(especificidade.AtividadeProcDocReq.ToProtocolo());
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			TermoCPFARLCRVM vm = new TermoCPFARLCRVM(
				lstProcessosDocumentos,
				lstAtividades,
				titulo.Condicionantes,
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				EmpreendimentoBus empreendimentoBus = new EmpreendimentoBus();
				DominialidadeBus dominialidadeBus = new DominialidadeBus();
				vm.Atividades.Atividades = titulo.Atividades;

				vm.CedenteDominios = ViewModelHelper.CriarSelectList(dominialidadeBus.ObterDominiosLista(especificidade.EmpreendimentoId), selecionado: termo.CedenteDominioID.ToString());
				vm.CedenteReservas = ViewModelHelper.CriarSelectList(dominialidadeBus.ObterARLCompensacaoDominio(termo.CedenteDominioID));
				vm.CedenteResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(empreendimentoBus.ObterResponsaveisComTipo(especificidade.EmpreendimentoId));

				vm.ReceptorEmpreendimentos = ViewModelHelper.CriarSelectList(
					new List<Lista>() { new Lista() { Id = termo.ReceptorEmpreendimentoID.ToString(), Texto = termo.ReceptorEmpreendimentoDenominador } },
					selecionado: termo.ReceptorEmpreendimentoID.ToString());
				vm.ReceptorDominios = ViewModelHelper.CriarSelectList(dominialidadeBus.ObterDominiosLista(termo.ReceptorEmpreendimentoID), selecionado: termo.ReceptorDominioID.ToString());
				vm.ReceptorResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(empreendimentoBus.ObterResponsaveisComTipo(termo.ReceptorEmpreendimentoID));
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/TermoCPFARLCR.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult TermoCompromissoAmbiental(EspecificidadeVME especificidade)
		{
			TermoCompromissoAmbientalBus bus = new TermoCompromissoAmbientalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TermoCompromissoAmbiental termo = new TermoCompromissoAmbiental();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					termo = bus.Obter(especificidade.TituloId) as TermoCompromissoAmbiental;
				}
				else
				{
					termo = bus.ObterHistorico(especificidade.TituloId, 0) as TermoCompromissoAmbiental;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);

			}

			if (especificidade.ProtocoloId > 0)
			{
				if (_busEspecificidade.ExisteProcDocFilhoQueFoiDesassociado(especificidade.TituloId))
				{
					lstAtividades = new List<AtividadeSolicitada>();
					titulo.Atividades = new List<Atividade>();
				}
				else
				{
					lstAtividades = _busAtividade.ObterAtividadesLista(especificidade.AtividadeProcDocReq.ToProtocolo());
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					destinatarios = _busTitulo.ObterDestinatarios(bus.ObterProtocolo(termo.Licenca));
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = termo.Destinatario, Texto = termo.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			TermoCompromissoAmbientalVM vm = new TermoCompromissoAmbientalVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				bus.ObterRepresentantes(termo.Destinatario, termo.DestinatarioTid),
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/TermoCompromissoAmbiental.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult AberturaLivroUnidadeConsolidacao(EspecificidadeVME especificidade)
		{
			AberturaLivroUnidadeConsolidacaoBus bus = new AberturaLivroUnidadeConsolidacaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<Lista> culturas = new List<Lista>();

			Titulo titulo = new Titulo();
			AberturaLivroUnidadeConsolidacao termo = new AberturaLivroUnidadeConsolidacao();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					termo = bus.Obter(especificidade.TituloId) as AberturaLivroUnidadeConsolidacao;
					culturas = bus.ObterCulturas(especificidade.ProtocoloId);
				}
				else
				{
					termo = bus.ObterHistorico(especificidade.TituloId, titulo.Situacao.Id) as AberturaLivroUnidadeConsolidacao;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);

			}

			if (especificidade.ProtocoloId > 0)
			{
				if (_busEspecificidade.ExisteProcDocFilhoQueFoiDesassociado(especificidade.TituloId))
				{
					lstAtividades = new List<AtividadeSolicitada>();
					titulo.Atividades = new List<Atividade>();
				}
				else
				{
					lstAtividades = _busAtividade.ObterAtividadesLista(especificidade.AtividadeProcDocReq.ToProtocolo());
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			AberturaLivroUnidadeConsolidacaoVM vm = new AberturaLivroUnidadeConsolidacaoVM(
				lstProcessosDocumentos,
				lstAtividades,
				culturas,
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/AberturaLivroUnidadeConsolidacao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult AberturaLivroUnidadeProducao(EspecificidadeVME especificidade)
		{
			AberturaLivroUnidadeProducaoBus bus = new AberturaLivroUnidadeProducaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<Lista> unidadesProducoes = new List<Lista>();

			Titulo titulo = new Titulo();
			AberturaLivroUnidadeProducao termo = new AberturaLivroUnidadeProducao();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					termo = bus.Obter(especificidade.TituloId) as AberturaLivroUnidadeProducao;
				}
				else
				{
					termo = bus.ObterHistorico(especificidade.TituloId, titulo.Situacao.Id) as AberturaLivroUnidadeProducao;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);
			}

			if (especificidade.ProtocoloId > 0)
			{
				if (_busEspecificidade.ExisteProcDocFilhoQueFoiDesassociado(especificidade.TituloId))
				{
					lstAtividades = new List<AtividadeSolicitada>();
					titulo.Atividades = new List<Atividade>();
				}
				else
				{
					lstAtividades = _busAtividade.ObterAtividadesLista(especificidade.AtividadeProcDocReq.ToProtocolo());
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					unidadesProducoes = bus.ObterUnidadesProducoes(especificidade.ProtocoloId);
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			AberturaLivroUnidadeProducaoVM vm = new AberturaLivroUnidadeProducaoVM(
				lstProcessosDocumentos,
				lstAtividades,
				unidadesProducoes,
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Termo/AberturaLivroUnidadeProducao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosTermoAprovacaoMedicao(Protocolo protocolo)
		{
			TermoAprovacaoMedicao termo = new TermoAprovacaoMedicao();

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@Termo = termo,
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.Value),
				@Funcionario = new List<PessoaLst> { new PessoaLst { Id = _usuarioLogado.FuncionarioId, Texto = _usuarioLogado.Name } },
				@Tecnicos = _protocoloBus.ObterResponsaveisTecnicos(protocolo.Id.Value),
				@Setores = _busTitulo.ObterFuncionarioSetores()
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosTermoCPFARL(Protocolo protocolo)
		{
			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.Value)
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosTermoCPFARLTituloAntigo(int atividadeId, int protocoloId)
		{
			TermoCPFARLRBus bus = new TermoCPFARLRBus();

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@TituloAnterior = bus.ObterTituloAnterior(atividadeId, protocoloId)
			}, JsonRequestBehavior.AllowGet);
		}

		#region Termo Compromisso Ambiental

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosTermoCompromissoAmbiental(Int32 titulo)
		{
			TermoCompromissoAmbientalBus _bus = new TermoCompromissoAmbientalBus();

			_bus.ValidarTitulo(titulo);

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Destinatarios = _busTitulo.ObterDestinatarios(_bus.ObterProtocolo(titulo))
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosTermoCompromissoAmbientalRepresentantes(Int32 destinatario)
		{
			TermoCompromissoAmbientalBus _bus = new TermoCompromissoAmbientalBus();

			Boolean MostrarRepresentantes = _bus.ObterDestinatarioTipo(destinatario) == PessoaTipo.JURIDICA;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@MostrarRepresentantes = MostrarRepresentantes,
				@Representantes = (MostrarRepresentantes) ? _bus.ObterRepresentantes(destinatario) : null
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosAberturaLivroUnidadeConsolidacao(Protocolo protocolo)
		{
			AberturaLivroUnidadeConsolidacaoBus bus = new AberturaLivroUnidadeConsolidacaoBus();

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@Culturas = bus.ObterCulturas(protocolo.Id.Value)
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosAberturaLivroUnidadeProducao(Protocolo protocolo)
		{
			AberturaLivroUnidadeProducaoBus bus = new AberturaLivroUnidadeProducaoBus();

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@UnidadesProducoes = bus.ObterUnidadesProducoes(protocolo.Id.Value)
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterUnidadeProducaoItem(int id)
		{
			UnidadeProducaoBus bus = new UnidadeProducaoBus();
			var item = bus.ObterUnidadeProducaoItem(id);

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Item = item
			}, JsonRequestBehavior.AllowGet);
		}

		#region TCPFARLC e TCPFARLCR

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosTermoCPFARLC(EspecificidadeVME especificidade)
		{
			DominialidadeBus dominialidadeBus = new DominialidadeBus();
			List<Lista> cedenteDominios = new DominialidadeBus().ObterDominiosLista(especificidade.EmpreendimentoId);
			List<Lista> cedenteARLCompensacao = new List<Lista>();

			if (cedenteDominios.Count == 1)
			{
				cedenteARLCompensacao = dominialidadeBus.ObterARLCompensacaoDominio(Convert.ToInt32(cedenteDominios.First().Id));
			}

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				Dominialidade = dominialidadeBus.ObterPorEmpreendimento(especificidade.EmpreendimentoId, true),
				CedenteDominios = cedenteDominios,
				CedenteARLCompensacao = cedenteARLCompensacao,
				CedenteResponsaveisEmpreendimento = new EmpreendimentoBus().ObterResponsaveisComTipo(especificidade.EmpreendimentoId)
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterArl(int dominio)
		{
			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				ARLCedente = new DominialidadeBus().ObterARLCompensacaoDominio(dominio)
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosTermoCPFARLCR(EspecificidadeVME especificidade)
		{
			DominialidadeBus dominialidadeBus = new DominialidadeBus();
			List<Lista> cedenteDominios = new DominialidadeBus().ObterDominiosLista(especificidade.EmpreendimentoId);
			List<Lista> cedenteARLCompensacao = new List<Lista>();

			if (cedenteDominios.Count == 1)
			{
				cedenteARLCompensacao = dominialidadeBus.ObterARLCompensacaoDominio(Convert.ToInt32(cedenteDominios.First().Id));
			}

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				Dominialidade = dominialidadeBus.ObterPorEmpreendimento(especificidade.EmpreendimentoId, true),
				CedenteDominios = cedenteDominios,
				CedenteARLCompensacao = cedenteARLCompensacao,
				CedenteResponsaveisEmpreendimento = new EmpreendimentoBus().ObterResponsaveisComTipo(especificidade.EmpreendimentoId)
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosEmpreendimentoReceptor(int reservaLegal)
		{
			DominialidadeBus dominialidadeBus = new DominialidadeBus();
			EmpreendimentoCaracterizacao empreendimentoReceptor = dominialidadeBus.ObterEmpreendimentoReceptor(reservaLegal);
			List<Lista> receptorEmpreendimento = new List<Lista>();

			if (empreendimentoReceptor.Id > 0)
			{
				receptorEmpreendimento.Add(new Lista() { Id = empreendimentoReceptor.Id.ToString(), Texto = empreendimentoReceptor.Denominador });
			}

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				ReceptorEmpreendimento = receptorEmpreendimento,
				ReceptorDominios = dominialidadeBus.ObterDominiosLista(empreendimentoReceptor.Id),
				ReceptorResponsaveisEmpreendimento = new EmpreendimentoBus().ObterResponsaveisComTipo(empreendimentoReceptor.Id)
			});
		}

		[HttpPost]
		[Permite(RoleArray = new object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ValidarAdicionarARL(int reservaLegal, int empreendimento)
		{
			DominialidadeBus dominialidadeBus = new DominialidadeBus();
			EmpreendimentoCaracterizacao empReservaLegal = dominialidadeBus.ObterEmpreendimentoReceptor(reservaLegal);

			TermoCPFARLCRValidar validar = new TermoCPFARLCRValidar();
			validar.AdicionarARL(empreendimento, empReservaLegal.Id);

			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
		}

		#endregion

		#endregion
	}
}