using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class LicencaController : DefaultController
	{
		#region propriedades

		TituloBus _busTitulo = new TituloBus();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaPorteUsoMotosserra(EspecificidadeVME especificidade)
		{
			LicencaPorteUsoMotosserraBus bus = new LicencaPorteUsoMotosserraBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaPorteUsoMotosserra licenca = new LicencaPorteUsoMotosserra();

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				licenca = bus.Obter(especificidade.TituloId) as LicencaPorteUsoMotosserra;

				if (!especificidade.IsVisualizar)
				{
					licenca.Motosserra = bus.ObterMotosserra(licenca.Motosserra.Id);
				}

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = licenca.Destinatario, Texto = licenca.DestinatarioNomeRazao, IsAtivo = true });
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

			LicencaPorteUsoMotosserraVM vm = new LicencaPorteUsoMotosserraVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				licenca,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaPorteUsoMotosserra.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaOperacao(EspecificidadeVME especificidade)
		{
			LicencaOperacaoBus bus = new LicencaOperacaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaOperacao licenca = new LicencaOperacao();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				licenca = bus.Obter(especificidade.TituloId) as LicencaOperacao;

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = licenca.Destinatario, Texto = licenca.DestinatarioNomeRazao, IsAtivo = true });
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

			LicencaOperacaoVM vm = new LicencaOperacaoVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				licenca,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaOperacao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaOperacaoFomento(EspecificidadeVME especificidade)
		{
			LicencaOperacaoFomentoBus bus = new LicencaOperacaoFomentoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaOperacaoFomento licenca = new LicencaOperacaoFomento();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				licenca = bus.Obter(especificidade.TituloId) as LicencaOperacaoFomento;

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!Validacao.EhValido)
			{
				return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = string.Empty }, JsonRequestBehavior.AllowGet);
			}

			LicencaOperacaoFomentoVM vm = new LicencaOperacaoFomentoVM(
				lstProcessosDocumentos,
				lstAtividades,
				licenca,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaOperacaoFomento.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaInstalacao(EspecificidadeVME especificidade)
		{
			LicencaInstalacaoBus bus = new LicencaInstalacaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaInstalacao licenca = new LicencaInstalacao();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				licenca = bus.Obter(especificidade.TituloId) as LicencaInstalacao;

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = licenca.Destinatario, Texto = licenca.DestinatarioNomeRazao, IsAtivo = true });
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

			LicencaInstalacaoVM vm = new LicencaInstalacaoVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				licenca,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaInstalacao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaPrevia(EspecificidadeVME especificidade)
		{
			LicencaPreviaBus bus = new LicencaPreviaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaPrevia licenca = new LicencaPrevia();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				licenca = bus.Obter(especificidade.TituloId) as LicencaPrevia;

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = licenca.Destinatario, Texto = licenca.DestinatarioNomeRazao, IsAtivo = true });
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

			LicencaPreviaVM vm = new LicencaPreviaVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				licenca,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaPrevia.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaAmbientalRegularizacao(EspecificidadeVME especificidade)
		{
			LicencaAmbientalRegularizacaoBus bus = new LicencaAmbientalRegularizacaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaAmbientalRegularizacao licenca = new LicencaAmbientalRegularizacao();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				licenca = bus.Obter(especificidade.TituloId) as LicencaAmbientalRegularizacao;

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = licenca.Destinatario, Texto = licenca.DestinatarioNomeRazao, IsAtivo = true });
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

			LicencaAmbientalRegularizacaoVM vm = new LicencaAmbientalRegularizacaoVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				licenca,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaAmbientalRegularizacao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaAmbientalUnica(EspecificidadeVME especificidade)
		{
			LicencaAmbientalUnicaBus bus = new LicencaAmbientalUnicaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaAmbientalUnica licenca = new LicencaAmbientalUnica();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				licenca = bus.Obter(especificidade.TituloId) as LicencaAmbientalUnica;

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = licenca.Destinatario, Texto = licenca.DestinatarioNomeRazao, IsAtivo = true });
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

			LicencaAmbientalUnicaVM vm = new LicencaAmbientalUnicaVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				licenca,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaAmbientalUnica.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult LicencaSimplificada(EspecificidadeVME especificidade)
		{
			LicencaSimplificadaBus bus = new LicencaSimplificadaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			LicencaSimplificada licenca = new LicencaSimplificada();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				licenca = bus.Obter(especificidade.TituloId) as LicencaSimplificada;

				if (licenca != null)
				{
					especificidade.AtividadeProcDocReq = licenca.ProtocoloReq;
					licenca.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = licenca.Destinatario, Texto = licenca.DestinatarioNomeRazao, IsAtivo = true });
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

			LicencaSimplificadaVM vm = new LicencaSimplificadaVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				licenca,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Licenca/LicencaSimplificada.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliar

		public ActionResult ObterDadosLicencaPorteUsoMotosserra(Protocolo protocolo)
		{
			return Json(new { @Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault()) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar })]
		public ActionResult ObterMotosserra(int motosserraId, int destinatarioId, int tituloId)
		{
			LicencaPorteUsoMotosserraBus _bus = new LicencaPorteUsoMotosserraBus();

			if (!_bus.LicencaValidar.AssociarMotosserra(motosserraId, destinatarioId, tituloId))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			Motosserra motosserra = _bus.ObterMotosserra(motosserraId);

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @Motosserra = motosserra}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}