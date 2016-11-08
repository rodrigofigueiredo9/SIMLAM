using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CertidaoController : DefaultController
	{
		#region Propriedades

		TituloBus _busTitulo = new TituloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertidaoAnuencia(EspecificidadeVME especificidade)
		{
			CertidaoAnuenciaBus _certifidaoAnuenciaBus = new CertidaoAnuenciaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CertidaoAnuencia certidao = new CertidaoAnuencia();

			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					certidao = _certifidaoAnuenciaBus.Obter(especificidade.TituloId) as CertidaoAnuencia;
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					certidao = _certifidaoAnuenciaBus.ObterHistorico(especificidade.TituloId, 0) as CertidaoAnuencia;
				}

				if (certidao != null)
				{
					especificidade.AtividadeProcDocReq = certidao.ProtocoloReq;
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

			CertidaoAnuenciaVM vm = new CertidaoAnuenciaVM(
				certidao,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certidao/CertidaoAnuencia.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertidaoCartaAnuencia(EspecificidadeVME especificidade)
		{
			CertidaoCartaAnuenciaBus _busCertidaoCartaAnuencia = new CertidaoCartaAnuenciaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CertidaoCartaAnuencia certidao = new CertidaoCartaAnuencia();

			string htmlEspecificidade = string.Empty;
			int atividadeId = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				atividadeId = titulo.Atividades.FirstOrDefault().Id;

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					certidao = _busCertidaoCartaAnuencia.Obter(especificidade.TituloId) as CertidaoCartaAnuencia;
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					certidao = _busCertidaoCartaAnuencia.ObterHistorico(especificidade.TituloId, 0) as CertidaoCartaAnuencia;
				}

				if (certidao != null)
				{
					especificidade.AtividadeProcDocReq = certidao.ProtocoloReq;
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

			CertidaoCartaAnuenciaVM vm = new CertidaoCartaAnuenciaVM(
				certidao,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				_busCertidaoCartaAnuencia.ObterDominios(especificidade.ProtocoloId),
				especificidade.AtividadeProcDocReqKey,
				atividadeId,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certidao/CertidaoCartaAnuencia.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertidaoDebito(EspecificidadeVME especificidade)
		{
			CertidaoDebitoBus _busCertidaoDebito = new CertidaoDebitoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			CertidaoDebito certidao = new CertidaoDebito();

			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				certidao = _busCertidaoDebito.Obter(especificidade.TituloId) as CertidaoDebito;

				if (certidao != null)
				{
					especificidade.AtividadeProcDocReq = certidao.ProtocoloReq;
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
					destinatarios.Add(new PessoaLst() { Id = certidao.Destinatario, Texto = certidao.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			if (!especificidade.IsVisualizar && certidao.Destinatario > 0)
			{
				certidao.Fiscalizacoes = _busCertidaoDebito.ObterFiscalizacoesPorAutuado(certidao.Destinatario).Where(x => x.InfracaoAutuada).ToList();
			}

			eCertidaoTipo tipoCertidao = _busCertidaoDebito.GerarCertidao(certidao.Fiscalizacoes);
			String tipoCertidaoTexto = tipoCertidao == eCertidaoTipo.PositivaComEfeitoNegativa ? "Positiva com efeito de negativa" : tipoCertidao.ToString();

			CertidaoDebitoVM vm = new CertidaoDebitoVM(
				certidao,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				tipoCertidaoTexto,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certidao/CertidaoDebito.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CertidaoDispensaLicenciamentoAmbiental(EspecificidadeVME especificidade)
		{
			var _busLista = new ListaBus();
			var _busCertidao = new CertidaoDispensaLicenciamentoAmbientalBus();
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			var titulo = new Titulo();
			var certidao = new CertidaoDispensaLicenciamentoAmbiental();

			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				certidao = _busCertidao.Obter(especificidade.TituloId) as CertidaoDispensaLicenciamentoAmbiental;

				if (certidao != null)
				{
					especificidade.AtividadeProcDocReq = certidao.ProtocoloReq;
				}

				lstAtividades = _busAtividade.ObterAtividadesListaReq(titulo.RequerimetoId.GetValueOrDefault());
			}

			CertidaoDispensaLicenciamentoAmbientalVM vm = new CertidaoDispensaLicenciamentoAmbientalVM(certidao, lstAtividades, _busLista.ObterVinculoPropriedade, especificidade.IsVisualizar);
			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certidao/CertidaoDispensaLicenciamentoAmbiental.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosCertidaoDestinatarios(Protocolo protocolo)
		{
			CertidaoCartaAnuenciaBus _busCertidaoCartaAnuencia = new CertidaoCartaAnuenciaBus();

			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault()),
				@Dominios = _busCertidaoCartaAnuencia.ObterDominios(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDestinatarios(Protocolo protocolo)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterFiscalizacoesPorAutuado(int autuadoId)
		{
			List<Fiscalizacao> fiscalizacoes = new List<Fiscalizacao>();

			CertidaoDebitoVM vm = new CertidaoDebitoVM();
			CertidaoDebitoBus _bus = new CertidaoDebitoBus();

			fiscalizacoes = _bus.ObterFiscalizacoesPorAutuado(autuadoId);
			vm.Certidao.Fiscalizacoes = fiscalizacoes.Where(x => x.InfracaoAutuada).ToList();

			eCertidaoTipo tipoCertidao = _bus.GerarCertidao(fiscalizacoes);

			String html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Certidao/CertidaoDebitoFiscalizacoesPartial.ascx", vm);
			String resultado = tipoCertidao == eCertidaoTipo.PositivaComEfeitoNegativa ? "Positiva com efeito de negativa" : tipoCertidao.ToString();

			return Json(new
			{
				@Html = html,
				@StrResultado = resultado
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}