using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Oficio;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class OficioController : DefaultController
	{
		#region Propriedades
		TituloBus _busTitulo = new TituloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();
		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult OficioNotificacao(EspecificidadeVME especificidade)
		{
			OficioNotificacaoBus _busPendencia = new OficioNotificacaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			OficioNotificacao oficio = new OficioNotificacao();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);

				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				oficio = _busPendencia.Obter(especificidade.TituloId) as OficioNotificacao;

				if (oficio != null)
				{
					especificidade.AtividadeProcDocReq = oficio.ProtocoloReq;
				}
			}

			if (especificidade.ProtocoloId>0)
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

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado || titulo.Situacao.Id == (int)eTituloSituacao.Nulo)
				{
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = oficio.Destinatario.GetValueOrDefault(), Texto = oficio.DestinatarioNomeRazao ?? "", IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			OficioNotificacaoVM vm = new OficioNotificacaoVM(lstProcessosDocumentos, lstAtividades, destinatarios, 
				especificidade.AtividadeProcDocReqKey, especificidade.IsVisualizar);

			vm.Oficio = oficio;

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Oficio/OficioNotificacao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult OficioUsucapiao(EspecificidadeVME especificidade)
		{
			OficioUsucapiaoBus _busUsucapiao = new OficioUsucapiaoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			Titulo titulo = new Titulo();
			OficioUsucapiao oficio = new OficioUsucapiao();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Anexos = _busTitulo.ObterAnexos(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				oficio = _busUsucapiao.Obter(especificidade.TituloId) as OficioUsucapiao;

				if (oficio != null)
				{
					especificidade.AtividadeProcDocReq = oficio.ProtocoloReq;
					oficio.Anexos = titulo.Anexos;
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

			OficioUsucapiaoVM vm = new OficioUsucapiaoVM(lstProcessosDocumentos, lstAtividades, especificidade.AtividadeProcDocReqKey, especificidade.IsVisualizar);

			vm.Oficio = oficio;

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Oficio/OficioUsucapiao.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosOficioNotificacao(Protocolo protocolo)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosOficioUsucapiao(Protocolo protocolo)
		{
			OficioUsucapiaoBus _busUsucapiao = new OficioUsucapiaoBus();

			return Json(new
			{
				@ZonaLocalizacao = _busUsucapiao.ObterZonaLocalizacaoEmpreendimento(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}
	}
}