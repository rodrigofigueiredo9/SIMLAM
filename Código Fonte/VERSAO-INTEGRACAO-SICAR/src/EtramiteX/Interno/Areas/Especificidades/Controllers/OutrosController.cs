using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class OutrosController : DefaultController
	{
		#region propriedades

		TituloBus _busTitulo = new TituloBus();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult OutrosReciboEntregaCopia(EspecificidadeVME especificidade)
		{
			OutrosReciboEntregaCopiaBus bus = new OutrosReciboEntregaCopiaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			OutrosReciboEntregaCopia outros = new OutrosReciboEntregaCopia();

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

				outros = bus.Obter(especificidade.TituloId) as OutrosReciboEntregaCopia;

				if (outros != null)
				{
					especificidade.AtividadeProcDocReq = outros.ProtocoloReq;
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
					destinatarios.Add(new PessoaLst() { Id = outros.Destinatario, Texto = outros.DestinatarioNomeRazao, IsAtivo = true });
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

			OutrosReciboEntregaCopiaVM vm = new OutrosReciboEntregaCopiaVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				outros,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Outros/OutrosReciboEntregaCopia.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult OutrosConclusaoTransferenciaDominio(EspecificidadeVME especificidade)
		{
			OutrosConclusaoTransferenciaDominioBus bus = new OutrosConclusaoTransferenciaDominioBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			OutrosConclusaoTransferenciaDominio outros = new OutrosConclusaoTransferenciaDominio();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				outros = especificidade.IsVisualizar ? bus.ObterHistorico(especificidade.TituloId, titulo.Tid) as OutrosConclusaoTransferenciaDominio : bus.Obter(especificidade.TituloId) as OutrosConclusaoTransferenciaDominio;

				if (outros != null)
				{
					especificidade.AtividadeProcDocReq = outros.ProtocoloReq;
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

			OutrosConclusaoTransferenciaDominioVM vm = new OutrosConclusaoTransferenciaDominioVM(
				lstProcessosDocumentos,
				lstAtividades,
				outros,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			List<PessoaLst> lstDestinatarios = new List<PessoaLst>();
			List<PessoaLst> lstResponsaveis = new List<PessoaLst>();

			if (especificidade.ProtocoloId > 0)
			{
				IProtocolo iProtocolo = new ProtocoloBus().ObterSimplificado(especificidade.ProtocoloId);

				lstDestinatarios.Add(new PessoaLst() { Texto = iProtocolo.Interessado.NomeRazaoSocial, Id = iProtocolo.Interessado.Id });

				List<Pessoa> responsaveis = new EmpreendimentoBus().ObterEmpreendimentoResponsaveis(iProtocolo.Empreendimento.Id);

				foreach (Pessoa pessoa in responsaveis)
				{
					lstResponsaveis.Add(new PessoaLst() { Texto = pessoa.NomeRazaoSocial, Id = pessoa.Id });
					if (pessoa.Id == iProtocolo.Interessado.Id)
					{
						continue;
					}
					lstDestinatarios.Add(new PessoaLst() { Texto = pessoa.NomeRazaoSocial, Id = pessoa.Id });
				}
			}

			vm.DestinatariosLst = ViewModelHelper.CriarSelectList(lstDestinatarios);
			vm.ResponsaveisLst = ViewModelHelper.CriarSelectList(lstResponsaveis);
			vm.InteressadosLst = ViewModelHelper.CriarSelectList(lstDestinatarios);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Outros/OutrosConclusaoTransferenciaDominio.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult OutrosLegitimacaoTerraDevoluta(EspecificidadeVME especificidade)
		{
			OutrosLegitimacaoTerraDevolutaBus bus = new OutrosLegitimacaoTerraDevolutaBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			OutrosLegitimacaoTerraDevoluta outros = new OutrosLegitimacaoTerraDevoluta();

			int atividadeSelecionada = 0;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.Obter(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);

				if (titulo.Atividades.Count > 0)
				{
					atividadeSelecionada = titulo.Atividades[0].Id;
				}

				especificidade.AtividadeProcDocReq = _busTitulo.ObterProcDocReqEspecificidade(especificidade.TituloId);

				if (titulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
				{
					outros = bus.Obter(especificidade.TituloId) as OutrosLegitimacaoTerraDevoluta;
					destinatarios = _busTitulo.ObterDestinatarios(especificidade.ProtocoloId);
				}
				else
				{
					outros = bus.ObterHistorico(especificidade.TituloId, 0) as OutrosLegitimacaoTerraDevoluta;
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

			ListaBus listaBus = new ListaBus();
			OutrosLegitimacaoTerraDevolutaVM vm = new OutrosLegitimacaoTerraDevolutaVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				bus.ObterDominios(especificidade.ProtocoloId),
				outros,
				listaBus.Municipios(ViewModelHelper.EstadoDefaultId()),
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Outros/OutrosLegitimacaoTerraDevoluta.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult OutrosInformacaoCorte(EspecificidadeVME especificidade)
		{
			OutrosInformacaoCorteBus bus = new OutrosInformacaoCorteBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			Titulo titulo = new Titulo();
			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			OutrosInformacaoCorte outros = new OutrosInformacaoCorte();

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

				outros = bus.Obter(especificidade.TituloId) as OutrosInformacaoCorte;
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
					destinatarios.Add(new PessoaLst() { Id = outros.Destinatario, Texto = outros.DestinatarioNomeRazao, IsAtivo = true });
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

			OutrosInformacaoCorteVM vm = new OutrosInformacaoCorteVM(
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				bus.ObterInformacaoCortes(especificidade.ProtocoloId),
				outros,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar,
				atividadeSelecionada);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			string htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Outros/OutrosInformacaoCorte.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);
		}

		#region Auxiliar

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosOutros(Protocolo protocolo)
		{
			return Json(new { @Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.GetValueOrDefault()) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CarregarListasPessoas(int protocoloId)
		{
			IProtocolo iProtocolo = new ProtocoloBus().Obter(protocoloId);

			List<PessoaLst> lstDestinatarios = new List<PessoaLst>();
			List<PessoaLst> lstResponsaveis = new List<PessoaLst>();

			lstDestinatarios.Add(new PessoaLst() { Texto = iProtocolo.Interessado.NomeRazaoSocial, Id = iProtocolo.Interessado.Id });

			List<Pessoa> responsaveis = new EmpreendimentoBus().ObterEmpreendimentoResponsaveis(iProtocolo.Empreendimento.Id);

			foreach (Pessoa pessoa in responsaveis)
			{
				lstResponsaveis.Add(new PessoaLst() { Texto = pessoa.NomeRazaoSocial, Id = pessoa.Id });
				if (pessoa.Id == iProtocolo.Interessado.Id)
				{
					continue;
				}
				lstDestinatarios.Add(new PessoaLst() { Texto = pessoa.NomeRazaoSocial, Id = pessoa.Id });
			}

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				Destinatarios = lstDestinatarios,
				Responsaveis = lstResponsaveis,
				Interessados = lstDestinatarios
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosOutrosLegitimacaoTerraDevoluta(Protocolo protocolo)
		{
			OutrosLegitimacaoTerraDevolutaBus bus = new OutrosLegitimacaoTerraDevolutaBus();
			EmpreendimentoBus empreendimentoBus = new EmpreendimentoBus();

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.Value),
				@Dominios = bus.ObterDominios(protocolo.Id.GetValueOrDefault()),
				@MunicipioEmp = empreendimentoBus.ObterEndereco(protocolo.Empreendimento.Id).MunicipioId
			}, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosOutrosInformacaoCorte(Protocolo protocolo)
		{
			OutrosInformacaoCorteBus bus = new OutrosInformacaoCorteBus();

			return Json(new
			{
				Msg = Validacao.Erros,
				EhValido = Validacao.EhValido,
				@Destinatarios = _busTitulo.ObterDestinatarios(protocolo.Id.Value),
				@InformacaoCortes = bus.ObterInformacaoCortes(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}