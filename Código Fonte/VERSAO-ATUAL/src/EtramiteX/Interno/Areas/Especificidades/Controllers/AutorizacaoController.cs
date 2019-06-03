using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloAutorizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Autorizacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloAutorizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AutorizacaoController : DefaultController
	{
		#region propriedades

		TituloBus _busTitulo = new TituloBus();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		#region Validações

		[HttpPost]
		public ActionResult ValidarAssociarLaudoVistoria(int tituloAssociadoId)
		{
			try
			{
				Titulo tituloAssociado = _busTitulo.ObterSimplificado(tituloAssociadoId);

				TituloValidar tituloValidar = new TituloValidar();

				if (!(tituloValidar.VerificarEhModeloCodigo(tituloAssociadoId, (int)eEspecificidade.LaudoVistoriaFlorestal) ||
					tituloValidar.VerificarEhModeloCodigo(tituloAssociadoId, (int)eEspecificidade.LaudoVistoriaQueimaControlada)))
				{
					Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.LaudoVistoriaModelo);
				}

				if (tituloAssociado.Situacao.Id != (int) eTituloSituacao.Concluido && tituloAssociado.Situacao.Id != (int)eTituloSituacao.Prorrogado)
				{
					Validacao.Add(Mensagem.AutorizacaoExploracaoFlorestal.LaudoVistoriaDeveEstarConcluidoOuProrrogado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult AutorizacaoExploracaoFlorestal(EspecificidadeVME especificidade)
		{
			AutorizacaoExploracaoFlorestalBus bus = new AutorizacaoExploracaoFlorestalBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			AutorizacaoExploracaoFlorestal autorizacao = new AutorizacaoExploracaoFlorestal();

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				autorizacao = bus.Obter(especificidade.TituloId) as AutorizacaoExploracaoFlorestal;

				if (autorizacao != null)
				{
					especificidade.AtividadeProcDocReq = autorizacao.ProtocoloReq;
					autorizacao.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = autorizacao.Destinatario, Texto = autorizacao.DestinatarioNomeRazao, IsAtivo = true });
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

			AutorizacaoExploracaoFlorestalVM vm = new AutorizacaoExploracaoFlorestalVM(
				lstProcessosDocumentos, 
				lstAtividades, 
				destinatarios,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey, especificidade.IsVisualizar);

			vm.Autorizacao = autorizacao;
			vm.TituloAssociado = autorizacao.TitulosAssociado.FirstOrDefault() ?? new TituloAssociadoEsp();
			vm.ArquivoVM.Anexos = titulo.Anexos;

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
				vm.Atividades.Especificidade = eEspecificidade.AutorizacaoExploracaoFlorestal;
				vm.Atividades.Destinatarios = vm.Destinatarios;
				var select = vm.Atividades.Destinatarios.FirstOrDefault(x => x.Value == autorizacao.Destinatario.ToString());
				select.Selected = true;

				var exploracoes = _busTitulo.ObterExploracoes(titulo.Id);
				var exploracao = exploracoes?.FirstOrDefault();
				if(exploracao != null)
					vm.TituloExploracaoDetalhes = exploracao.TituloExploracaoFlorestalExploracaoList;
				vm.Exploracoes = ViewModelHelper.CriarSelectList(exploracoes?.Select(x => new Lista()
				{
					Id = x.ExploracaoFlorestalId.ToString(),
					Codigo = x.Id.ToString(),
					Texto = x.ExploracaoFlorestalTexto
				}).ToList(), selecionado: exploracao?.ExploracaoFlorestalId.ToString());
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Autorizacao/AutorizacaoExploracaoFlorestal.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult AutorizacaoQueimaControlada(EspecificidadeVME especificidade)
		{
			AutorizacaoQueimaControladaBus bus = new AutorizacaoQueimaControladaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			AutorizacaoQueimaControlada autorizacao = new AutorizacaoQueimaControlada();

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				autorizacao = bus.Obter(especificidade.TituloId) as AutorizacaoQueimaControlada;

				if (autorizacao != null)
				{
					especificidade.AtividadeProcDocReq = autorizacao.ProtocoloReq;
					autorizacao.TitulosAssociado = (titulo.ToEspecificidade() ?? new Especificidade()).TitulosAssociado;
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
					destinatarios.Add(new PessoaLst() { Id = autorizacao.Destinatario, Texto = autorizacao.DestinatarioNomeRazao, IsAtivo = true });
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

			AutorizacaoQueimaControladaVM vm = new AutorizacaoQueimaControladaVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey, especificidade.IsVisualizar);

			vm.Autorizacao = autorizacao;
			vm.TituloAssociado = autorizacao.TitulosAssociado.FirstOrDefault() ?? new TituloAssociadoEsp();
			vm.ArquivoVM.Anexos = titulo.Anexos;


			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Autorizacao/AutorizacaoQueimaControlada.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliar

		public ActionResult ObterDadosAutorizacaoDestinatarios(Protocolo protocolo) =>
			Json(new { @Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault()) }, JsonRequestBehavior.AllowGet);

		public ActionResult ObterDadosExploracao(int tituloAssociadoId) =>
			Json(new { @Exploracoes = _busTitulo.ObterExploracoesTituloAssociado(tituloAssociadoId) }, JsonRequestBehavior.AllowGet);

		#endregion
	}
}