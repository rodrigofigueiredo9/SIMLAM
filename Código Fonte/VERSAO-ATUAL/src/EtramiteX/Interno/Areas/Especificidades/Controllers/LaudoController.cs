using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class LaudoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		TituloBus _busTitulo = new TituloBus();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();
		ProtocoloBus _protocoloBus = new ProtocoloBus();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoConstatacao(EspecificidadeVME especificidade)
		{
			LaudoConstatacaoBus _busLaudo = new LaudoConstatacaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			LaudoConstatacao laudo = new LaudoConstatacao();

			LaudoConstatacaoVM vm = null;
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoConstatacao;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
					laudo.Anexos = titulo.Anexos;
				}
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
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			vm = new LaudoConstatacaoVM(
				lstProcessosDocumentos, 
				lstAtividades, 
				especificidade.AtividadeProcDocReqKey, 
				especificidade.IsVisualizar);

			vm.Laudo = laudo;
			vm.Destinatarios = ViewModelHelper.CriarSelectList<PessoaLst>(destinatarios, selecionado: laudo.Destinatario.ToString());

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoConstatacao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoVistoriaQueimaControlada(EspecificidadeVME especificidade)
		{
			LaudoVistoriaFlorestalBus _busLaudo = new LaudoVistoriaFlorestalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			List<PessoaLst> destinatarios = new List<PessoaLst>();
			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LaudoVistoriaFlorestal laudo = new LaudoVistoriaFlorestal();

			LaudoVistoriaFlorestalVM vm = null;
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoVistoriaFlorestal;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
					laudo.Anexos = titulo.Anexos;
				}
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
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
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

			vm = new LaudoVistoriaFlorestalVM(
				modelo.Codigo,
				laudo,
				lstProcessosDocumentos,
				lstAtividades,
				_busLaudo.ObterCaracterizacoes(especificidade.EmpreendimentoId),
				destinatarios,
				_protocoloBus.ObterResponsaveisTecnicos(especificidade.ProtocoloId),
				_busLista.ObterEspecificidadeConclusoes,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoVistoriaQueimaControlada.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoVistoriaFlorestal(EspecificidadeVME especificidade)
		{
			LaudoVistoriaFlorestalBus _busLaudo = new LaudoVistoriaFlorestalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			List<PessoaLst> destinatarios = new List<PessoaLst>();
			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LaudoVistoriaFlorestal laudo = new LaudoVistoriaFlorestal();

			LaudoVistoriaFlorestalVM vm = null;
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);
				titulo.Exploracoes = _busTitulo.ObterExploracoes(especificidade.TituloId);

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoVistoriaFlorestal;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
					laudo.Anexos = titulo.Anexos;
				}
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
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
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

			var busExploracao = new ExploracaoFlorestalBus();
			var exploracoesLst = busExploracao.ObterPorEmpreendimentoList(especificidade.EmpreendimentoId);
			var caracterizacaoLst = new List<CaracterizacaoLst>();
			if (exploracoesLst.Count() > 0)
			{
				caracterizacaoLst = exploracoesLst.Select(x => new CaracterizacaoLst
				{
					Id = x.Id,
					Texto = x.CodigoExploracaoTexto ?? "",
					ParecerFavoravel = String.Join(", ", x.Exploracoes.Where(w => w.ParecerFavoravel == true).Select(y => y.Identificacao)?.ToList()),
					ParecerDesfavoravel = String.Join(", ", x.Exploracoes.Where(w => w.ParecerFavoravel == false).Select(y => y.Identificacao)?.ToList()),
					IsAtivo = true
				})?.ToList();
			}

			vm = new LaudoVistoriaFlorestalVM(
				modelo.Codigo,
				laudo,
				lstProcessosDocumentos,
				lstAtividades,
				caracterizacaoLst,
				destinatarios,
				_protocoloBus.ObterResponsaveisTecnicosPorRequerimento(especificidade.AtividadeProcDocReq.RequerimentoId),
				_busLista.ObterEspecificidadeConclusoes,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
				vm.Exploracoes = titulo.Exploracoes;

				var parecerFavoravel = new ArrayList();
				var parecerDesfavoravel = new ArrayList();
				foreach (var exploracao in exploracoesLst)
				{
					if (exploracao.Exploracoes.Where(x => x.ParecerFavoravel == true)?.ToList().Count > 0)
						parecerFavoravel.Add(String.Concat(exploracao.CodigoExploracaoTexto, " (", String.Join(", ", exploracao.Exploracoes.Where(x => x.ParecerFavoravel == true).Select(x => x.Identificacao)?.ToList()), ")"));
					if (exploracao.Exploracoes.Where(x => x.ParecerFavoravel == false)?.ToList().Count > 0)
						parecerDesfavoravel.Add(String.Concat(exploracao.CodigoExploracaoTexto, " (", String.Join(", ", exploracao.Exploracoes.Where(x => x.ParecerFavoravel == false).Select(x => x.Identificacao)?.ToList()), ")"));
				}
				vm.ParecerFavoravelLabel = parecerFavoravel.Count > 0 ? String.Join(", ", parecerFavoravel?.ToArray()) : "";
				vm.ParecerDesfavoravelLabel = parecerDesfavoravel.Count > 0 ? String.Join(", ", parecerDesfavoravel?.ToArray()) : "";
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoVistoriaFlorestal.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoVistoriaFundiaria(EspecificidadeVME especificidade)
		{
			LaudoVistoriaFundiariaBus _busLaudo = new LaudoVistoriaFundiariaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			int atividadeSelecionada = 0;

			LaudoVistoriaFundiaria laudo = new LaudoVistoriaFundiaria();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);

				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoVistoriaFundiaria;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
				}

				_busLaudo.EhHistorico = especificidade.IsVisualizar;

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
						destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
					}
					else
					{
						destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
					}

					if (!especificidade.IsVisualizar)
					{
						_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
					}
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			LaudoVistoriaFundiariaVM vm = new LaudoVistoriaFundiariaVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				_busLaudo.ObterPossesRegularizacao(especificidade.EmpreendimentoId),
				especificidade.AtividadeProcDocReqKey,
				atividadeSelecionada,
				especificidade.IsVisualizar);

			vm.Laudo = laudo;

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoVistoriaFundiaria.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoVistoriaLicenciamento(EspecificidadeVME especificidade)
		{
			LaudoVistoriaLicenciamentoBus _busLaudo = new LaudoVistoriaLicenciamentoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> lstResponsaveisTecnicos = new List<PessoaLst>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LaudoVistoriaLicenciamento laudo = new LaudoVistoriaLicenciamento();

			LaudoVistoriaLicenciamentoVM vm = null;
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoVistoriaLicenciamento;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
					laudo.Anexos = titulo.Anexos;
				}
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
					lstResponsaveisTecnicos = _protocoloBus.ObterResponsaveisTecnicos(especificidade.AtividadeProcDocReq.Id);
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
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

			vm = new LaudoVistoriaLicenciamentoVM(
				laudo,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				lstResponsaveisTecnicos,
				_busLista.ObterEspecificidadeConclusoes,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoVistoriaLicenciamento.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoFundiarioSimplificado(EspecificidadeVME especificidade)
		{
			LaudoFundiarioSimplificadoBus _busLaudo = new LaudoFundiarioSimplificadoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LaudoFundiarioSimplificado laudo = new LaudoFundiarioSimplificado();

			LaudoFundiarioSimplificadoVM vm = null;
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoFundiarioSimplificado;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
					laudo.Anexos = titulo.Anexos;
				}
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
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
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

			vm = new LaudoFundiarioSimplificadoVM(
				laudo,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoFundiarioSimplificado.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoAuditoriaFomentoFlorestal(EspecificidadeVME especificidade)
		{
			LaudoAuditoriaFomentoFlorestalBus _busLaudo = new LaudoAuditoriaFomentoFlorestalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LaudoAuditoriaFomentoFlorestal laudo = new LaudoAuditoriaFomentoFlorestal();

			LaudoAuditoriaFomentoFlorestalVM vm = null;
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoAuditoriaFomentoFlorestal;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
					laudo.Anexos = titulo.Anexos;
				}
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
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
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

			vm = new LaudoAuditoriaFomentoFlorestalVM(
				laudo,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				_protocoloBus.ObterResponsaveisTecnicos(especificidade.ProtocoloId),
				_busLista.ObterEspecificidadeResultados,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoAuditoriaFomentoFlorestal.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LaudoVistoriaFomentoFlorestal(EspecificidadeVME especificidade)
		{
			LaudoVistoriaFomentoFlorestalBus _busLaudo = new LaudoVistoriaFomentoFlorestalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LaudoVistoriaFomentoFlorestal laudo = new LaudoVistoriaFomentoFlorestal();

			LaudoVistoriaFomentoFlorestalVM vm = null;
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				laudo = _busLaudo.Obter(especificidade.TituloId) as LaudoVistoriaFomentoFlorestal;

				if (laudo != null)
				{
					especificidade.AtividadeProcDocReq = laudo.ProtocoloReq;
					laudo.Anexos = titulo.Anexos;
				}
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
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = laudo.Destinatario, Texto = laudo.DestinatarioNomeRazao, IsAtivo = true });
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

			vm = new LaudoVistoriaFomentoFlorestalVM(
				laudo,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				_protocoloBus.ObterResponsaveisTecnicos(especificidade.ProtocoloId),
				_busLista.ObterEspecificidadeConclusoes,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Laudo/LaudoVistoriaFomentoFlorestal.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoConstatacao(int id)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(id)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoVistoriaQueimaControlada(int id, int empreendimento)
		{
			LaudoVistoriaFlorestalBus laudoBus = new LaudoVistoriaFlorestalBus();

			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(id),
				@ResponsaveisTecnico = _protocoloBus.ObterResponsaveisTecnicos(id),
				@Caracterizacoes = laudoBus.ObterCaracterizacoes(empreendimento)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoVistoriaFlorestal(int id, int empreendimento)
		{
			var laudoBus = new LaudoVistoriaFlorestalBus();
			var busExploracao = new ExploracaoFlorestalBus();
			var exploracoesLst = busExploracao.ObterPorEmpreendimentoList(empreendimento)?.Where(x => x.DataConclusao.IsEmpty);
			var caracterizacaoLst = exploracoesLst?.Select(x => new CaracterizacaoLst
			{
				Id = x.Id,
				Texto = x.CodigoExploracaoTexto ?? "",
				ParecerFavoravel = String.Join(", ", x.Exploracoes?.Where(w => w.ParecerFavoravel == true)?.Select(y => y.Identificacao)?.ToList()),
				ParecerDesfavoravel = String.Join(", ", x.Exploracoes?.Where(w => w.ParecerFavoravel == false)?.Select(y => y.Identificacao)?.ToList()),
				IsAtivo = true
			});

			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(id),
				@ResponsaveisTecnico = _protocoloBus.ObterResponsaveisTecnicos(id),
				@Caracterizacoes = caracterizacaoLst
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoVistoriaFundiaria(EspecificidadeVME especificidade)
		{
			LaudoVistoriaFundiariaBus bus = new LaudoVistoriaFundiariaBus();
			RegularizacaoFundiaria regularizacao = new RegularizacaoFundiariaBus().ObterPorEmpreendimento(especificidade.EmpreendimentoId);

			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId),
				@Posses = bus.ObterPossesRegularizacao(especificidade.EmpreendimentoId),
				@RegularizacaoFundiariaId = regularizacao.Id,
				@RegularizacaoFundiariaTid = regularizacao.Tid
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoVistoriaLicenciamento(int id, int empreendimento)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(id),
				@ResponsaveisTecnico = _protocoloBus.ObterResponsaveisTecnicos(id)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoFundiarioSimplificado(int id, int empreendimento)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(id)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoAuditoriaFomentoFlorestal(int id, int empreendimento)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(id)
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosLaudoVistoriaFomentoFlorestal(int id, int empreendimento)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(id)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}