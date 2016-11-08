using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CertificadoController : DefaultController
	{
		#region Propriedades

		TituloBus _busTitulo = new TituloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertificadoRegistro(EspecificidadeVME especificidade)
		{
			CertificadoRegistroBus _certificadoRegistroBus = new CertificadoRegistroBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CertificadoRegistro certificado = new CertificadoRegistro();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				certificado = _certificadoRegistroBus.Obter(especificidade.TituloId) as CertificadoRegistro;

				if (certificado != null)
				{
					especificidade.AtividadeProcDocReq = certificado.ProtocoloReq;
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
					destinatarios.Add(new PessoaLst() { Id = certificado.Destinatario, Texto = certificado.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			CertificadoRegistroVM vm = new CertificadoRegistroVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			vm.Certificado = certificado;

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certificado/CertificadoRegistro.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CadComercProdutosAgrotoxicos(EspecificidadeVME especificidade)
		{
			CadComercProdutosAgrotoxicosBus _cadastroComercianteBus = new CadComercProdutosAgrotoxicosBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CadComercProdutosAgrotoxicos certificado = new CadComercProdutosAgrotoxicos();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				certificado = _cadastroComercianteBus.Obter(especificidade.TituloId) as CadComercProdutosAgrotoxicos;

				if (certificado != null)
				{
					especificidade.AtividadeProcDocReq = certificado.ProtocoloReq;
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
					destinatarios.Add(new PessoaLst() { Id = certificado.Destinatario, Texto = certificado.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			CadComercProdutosAgrotoxicosVM vm = new CadComercProdutosAgrotoxicosVM(
				certificado,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certificado/CadComercProdutosAgrotoxicos.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);

		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CadAplicProdutosAgrotoxicos(EspecificidadeVME especificidade)
		{
			CadAplicProdutosAgrotoxicosBus _cadastroComercianteBus = new CadAplicProdutosAgrotoxicosBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CadAplicProdutosAgrotoxicos certificado = new CadAplicProdutosAgrotoxicos();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				certificado = _cadastroComercianteBus.Obter(especificidade.TituloId) as CadAplicProdutosAgrotoxicos;

				if (certificado != null)
				{
					especificidade.AtividadeProcDocReq = certificado.ProtocoloReq;
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
					destinatarios.Add(new PessoaLst() { Id = certificado.Destinatario, Texto = certificado.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			CadAplicProdutosAgrotoxicosVM vm = new CadAplicProdutosAgrotoxicosVM(
				certificado,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certificado/CadAplicProdutosAgrotoxicos.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertificadoCadastroProdutoVegetal(EspecificidadeVME especificidade)
		{
			CertificadoCadastroProdutoVegetalBus _busCertificado = new CertificadoCadastroProdutoVegetalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CertificadoCadastroProdutoVegetal certificado = new CertificadoCadastroProdutoVegetal();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				certificado = _busCertificado.Obter(especificidade.TituloId) as CertificadoCadastroProdutoVegetal;

				if (certificado != null)
				{
					especificidade.AtividadeProcDocReq = certificado.ProtocoloReq;
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
					destinatarios.Add(new PessoaLst() { Id = certificado.Destinatario.GetValueOrDefault(), Texto = certificado.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			CertificadoCadastroProdutoVegetalVM vm = new CertificadoCadastroProdutoVegetalVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			vm.Certificado = certificado;

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certificado/CertificadoCadastroProdutoVegetal.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertificadoRegistroAtividadeFlorestal(EspecificidadeVME especificidade)
		{
			CertificadoRegistroAtividadeFlorestalBus bus = new CertificadoRegistroAtividadeFlorestalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CertificadoRegistroAtividadeFlorestal certificado = new CertificadoRegistroAtividadeFlorestal();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				certificado = bus.Obter(especificidade.TituloId) as CertificadoRegistroAtividadeFlorestal;

				if (certificado != null)
				{
					especificidade.AtividadeProcDocReq = certificado.ProtocoloReq;
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
					destinatarios.Add(new PessoaLst() { Id = certificado.Destinatario, Texto = certificado.DestinatarioNomeRazao, IsAtivo = true });
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

			CertificadoRegistroAtividadeFlorestalVM vm = new CertificadoRegistroAtividadeFlorestalVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				certificado,
				atividadeSelecionada,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certificado/CertificadoRegistroAtividadeFlorestal.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertificadoCadastroProdutoAgrotoxico(EspecificidadeVME especificidade)
		{
			CertificadoCadastroProdutoAgrotoxicoBus bus = new CertificadoCadastroProdutoAgrotoxicoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();
			List<Lista> unidadesProducoes = new List<Lista>();

			Titulo titulo = new Titulo();
			CertificadoCadastroProdutoAgrotoxico termo = new CertificadoCadastroProdutoAgrotoxico();

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
					termo = bus.Obter(especificidade.TituloId) as CertificadoCadastroProdutoAgrotoxico;
				}
				else
				{
					termo = bus.ObterHistorico(especificidade.TituloId, titulo.Situacao.Id) as CertificadoCadastroProdutoAgrotoxico;
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
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = termo.DestinatarioId, Texto = termo.DestinatarioNomeRazao, IsAtivo = true });
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

			CertificadoCadastroProdutoAgrotoxicoVM vm = new CertificadoCadastroProdutoAgrotoxicoVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				termo,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certificado/CertificadoCadastroProdutoAgrotoxico.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosCertificadoDestinatarios(Protocolo protocolo)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosCertificadoCadastroProdutoVegetal(Protocolo protocolo)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosCertificadoCadastroProdutoAgrotoxico(Protocolo protocolo)
		{
			CertificadoCadastroProdutoAgrotoxicoBus bus = new CertificadoCadastroProdutoAgrotoxicoBus();

			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault()),
				@Agrotoxico = bus.ObterAgrotoxico(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosAgrotoxico(Protocolo protocolo)
		{
			CertificadoCadastroProdutoAgrotoxicoBus bus = new CertificadoCadastroProdutoAgrotoxicoBus();
			Agrotoxico agrotoxico =  bus.ObterAgrotoxico(protocolo.Id.GetValueOrDefault());

			return Json(new
			{
				@Agrotoxico = agrotoxico,
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}