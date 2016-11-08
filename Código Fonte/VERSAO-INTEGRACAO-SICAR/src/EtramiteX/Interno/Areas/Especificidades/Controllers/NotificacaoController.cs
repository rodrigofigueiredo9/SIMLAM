using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloNotificacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Notificacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloNotificacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class NotificacaoController : DefaultController
	{
		#region Propriedades

		TituloBus _busTitulo = new TituloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult NotificacaoEmbargo(EspecificidadeVME especificidade)
		{
			NotificacaoEmbargoBus bus = new NotificacaoEmbargoBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<AtividadeSolicitada> lstAtividadesEmbargo = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			NotificacaoEmbargo notificacao = new NotificacaoEmbargo();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					notificacao = bus.Obter(especificidade.TituloId) as NotificacaoEmbargo;
				}
				else
				{
					notificacao = bus.ObterHistorico(especificidade.TituloId, 0) as NotificacaoEmbargo;
				}

				if (notificacao != null)
				{
					especificidade.AtividadeProcDocReq = notificacao.ProtocoloReq;
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
					notificacao.Destinatarios.ForEach(x =>
					{
						destinatarios.Add(new PessoaLst() { Id = x.IdRelacionamento, Texto = x.Nome, IsAtivo = true });
					});
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

			NotificacaoEmbargoVM vm = new NotificacaoEmbargoVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				notificacao,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
				vm.AtividadesEmbargo = ViewModelHelper.CriarSelectList(_busAtividade.ObterAtividadesLista(new Protocolo { Id = titulo.Protocolo.Id }, true), true);
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Notificacao/NotificacaoEmbargo.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliar

		public ActionResult ObterDadosNotificacao(Protocolo protocolo)
		{
			return Json(new
			{
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault()),
				@Atividades = _busAtividade.ObterAtividadesLista(protocolo, true)
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}