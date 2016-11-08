using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCadastro;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Cadastro;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCadastro.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CadastroController : DefaultController
	{
		#region Propriedades

		TituloBus _busTitulo = new TituloBus();
		CadastroAmbientalRuralTituloBus _busCAR = new CadastroAmbientalRuralTituloBus();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		EspecificidadeValidarBase _busEspecificidade = new EspecificidadeValidarBase();

		#endregion

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult CadastroAmbientalRuralTitulo(EspecificidadeVME especificidade)
		{
			CadastroAmbientalRuralTituloBus _cadastroBus = new CadastroAmbientalRuralTituloBus();
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(especificidade.ProtocoloId);
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();
			List<PessoaLst> destinatarios = new List<PessoaLst>();

			TituloModelo modelo = _tituloModeloBus.Obter(especificidade.ModeloId ?? 0);
			Titulo titulo = new Titulo();
			CadastroAmbientalRuralTitulo cadastro = new CadastroAmbientalRuralTitulo();
			string htmlEspecificidade = string.Empty;

			if (especificidade.TituloId > 0)
			{
				titulo = _busTitulo.ObterSimplificado(especificidade.TituloId);
				titulo.Atividades = _busTitulo.ObterAtividades(especificidade.TituloId);
				titulo.Condicionantes = _busTitulo.ObterCondicionantes(especificidade.TituloId);

				cadastro = _cadastroBus.Obter(especificidade.TituloId) as CadastroAmbientalRuralTitulo;

				if (cadastro != null)
				{
					especificidade.AtividadeProcDocReq = cadastro.ProtocoloReq;
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
					EmpreendimentoBus empreendimentoBus = new EmpreendimentoBus();
					destinatarios = empreendimentoBus.ObterResponsaveis(especificidade.EmpreendimentoId);
				}
				else
				{
					destinatarios.Add(new PessoaLst() { Id = cadastro.Destinatario, Texto = cadastro.DestinatarioNomeRazao, IsAtivo = true });
				}

				if (!especificidade.IsVisualizar)
				{
					_busEspecificidade.PossuiAtividadeEmAndamento(especificidade.ProtocoloId);
				}
			}

			CadastroAmbientalRuralTituloVM vm = new CadastroAmbientalRuralTituloVM(
				cadastro,
				lstProcessosDocumentos,
				lstAtividades,
				destinatarios,
				titulo.Condicionantes,
				especificidade.AtividadeProcDocReqKey,
				especificidade.IsVisualizar);

			if (especificidade.TituloId > 0)
			{
				vm.Atividades.Atividades = titulo.Atividades;
			}

			vm.IsCondicionantes = modelo.Regra(eRegra.Condicionantes) || (titulo.Condicionantes != null && titulo.Condicionantes.Count > 0);

			htmlEspecificidade = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/Cadastro/CadastroAmbientalRuralTitulo.ascx", vm);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido, @Html = htmlEspecificidade }, JsonRequestBehavior.AllowGet);

		}

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult ObterDadosCadastro(Protocolo protocolo)
		{
			EmpreendimentoBus empreendimentoBus = new EmpreendimentoBus();

			return Json(new
			{
				@Destinatarios = empreendimentoBus.ObterResponsaveis(protocolo.Empreendimento.Id),
				@Matriculas = _busCAR.ObterMatriculasStragg(protocolo.Id.GetValueOrDefault())
			}, JsonRequestBehavior.AllowGet);
		}

		#endregion
	}
}