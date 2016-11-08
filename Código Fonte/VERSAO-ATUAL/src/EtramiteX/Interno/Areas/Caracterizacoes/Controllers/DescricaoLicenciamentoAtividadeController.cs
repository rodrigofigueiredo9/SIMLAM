using System;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class DescricaoLicenciamentoAtividadeController : DefaultController
	{
		#region Propriedades

		ListaBus _listaBus = new ListaBus();
		DescricaoLicenciamentoAtividadeBus _bus = new DescricaoLicenciamentoAtividadeBus();
		DescricaoLicenciamentoAtividadeValidar _validar = new DescricaoLicenciamentoAtividadeValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();

		#endregion

		public ActionResult Index(int id, int empreendimento, int tipo, bool isCadastrarCaracterizacao)
		{
			PermissaoValidar permissaoValidar = new PermissaoValidar();

			if (isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermissao.DescricaoLicenciamentoAtividadeCriar }, false))
			{
				return Criar(empreendimento, tipo, isCadastrarCaracterizacao);
			}

			if (!isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermissao.DescricaoLicenciamentoAtividadeEditar }, false))
			{
				return Editar(id, empreendimento, tipo, isCadastrarCaracterizacao);
			}

			if (permissaoValidar.ValidarAny(new[] { ePermissao.DescricaoLicenciamentoAtividadeVisualizar }, false))
			{
				return Visualizar(empreendimento, tipo, isCadastrarCaracterizacao);
			}

			permissaoValidar.ValidarAny(new[] { 
				ePermissao.DescricaoLicenciamentoAtividadeCriar, 
				ePermissao.DescricaoLicenciamentoAtividadeEditar, 
				ePermissao.DescricaoLicenciamentoAtividadeVisualizar });

			return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
		}

		[Permite(RoleArray = new Object[] { ePermissao.DescricaoLicenciamentoAtividadeCriar })]
		public ActionResult Criar(int empreendimento, int tipo, bool isCadastrarCaracterizacao)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimento, tipo))
			{
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
			}

			DescricaoLicenciamentoAtividadeVM vm = new DescricaoLicenciamentoAtividadeVM();

			DescricaoLicenciamentoAtividade dscLicAtividade = _bus.ObterPorEmpreendimento(empreendimento, (eCaracterizacao)tipo);

			dscLicAtividade = _bus.ObterDadosGeo(empreendimento, (eCaracterizacao)tipo, dscLicAtividade);
			if (dscLicAtividade.Id == 0)
			{
				dscLicAtividade.Tipo = tipo;	
			}

			vm.CaracterizacaoTipoTexto = _caracterizacaoBus.ObterCaracterizacoesEmpreendimento(empreendimento).SingleOrDefault(x => (int)x.Tipo == tipo).Nome;			
			dscLicAtividade.EmpreendimentoId = empreendimento;
			vm.DscLicAtividade = dscLicAtividade;
			vm.IsCadastrarCaracterizacao = isCadastrarCaracterizacao;

			vm.ResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(_bus.ObterResponsaveis(empreendimento));
			vm.FontesAbastecimentoAguaTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvFontesAbastecimentoAguaTipo);
			vm.PontosLancamentoEfluenteTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvPontosLancamentoEfluenteTipo);
			vm.OutorgaAguaTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvOutorgaAguaTipo);
			vm.FontesGeracaoTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvFontesGeracaoTipo);
			vm.UnidadeTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvUnidadeTipo);
			vm.CombustivelTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvCombustivelTipo);

			vm.VegetacaoAreaUtil = _listaBus.DscLicAtvVegetacaoAreaUtil;
			vm.Acondicionamento = _listaBus.DscLicAtvAcondicionamento;
			vm.Estocagem = _listaBus.DscLicAtvEstocagem;
			vm.Tratamento = _listaBus.DscLicAtvTratamento;
			vm.DestinoFinal = _listaBus.DscLicAtvDestinoFinal;

			if (vm.DscLicAtividade.Id > 0)
			{
				vm.DscLicAtividade.Dependencias = _caracterizacaoBus.ObterDependencias(vm.DscLicAtividade.Id, vm.DscLicAtividade.GetTipo, eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade);

				vm.TextoMerge = _caracterizacaoValidar.DependenciasAlteradas(
					vm.DscLicAtividade.EmpreendimentoId,
					vm.DscLicAtividade.Tipo,
					eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,
					vm.DscLicAtividade.Dependencias);
				vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			}

			if (vm.ResponsaveisEmpreendimento != null && vm.ResponsaveisEmpreendimento.Count == 2)
			{
				vm.DscLicAtividade.RespAtividade = Int32.Parse(vm.ResponsaveisEmpreendimento.First(x => int.Parse(x.Value) > 0).Value);
			}

			vm.UrlAvancar = CaracterizacaoVM.GerarUrl(empreendimento, isCadastrarCaracterizacao, (eCaracterizacao)tipo);
			
			return View("Criar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DescricaoLicenciamentoAtividadeEditar })]
		public ActionResult Editar(int id, int empreendimento, int tipo, bool isCadastrarCaracterizacao)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimento, tipo))
			{
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
			}

			DescricaoLicenciamentoAtividadeVM vm = new DescricaoLicenciamentoAtividadeVM();

			DescricaoLicenciamentoAtividade dscLicAtividade = _bus.Obter(id);

			dscLicAtividade = _bus.ObterDadosGeo(empreendimento, (eCaracterizacao)tipo, dscLicAtividade);

			vm.CaracterizacaoTipoTexto = _caracterizacaoBus.ObterCaracterizacoesEmpreendimento(empreendimento).SingleOrDefault(x => (int)x.Tipo == tipo).Nome;
			dscLicAtividade.EmpreendimentoId = empreendimento;
			vm.DscLicAtividade = dscLicAtividade;
			vm.IsCadastrarCaracterizacao = isCadastrarCaracterizacao;

			vm.ResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(_bus.ObterResponsaveis(empreendimento));
			vm.FontesAbastecimentoAguaTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvFontesAbastecimentoAguaTipo);
			vm.PontosLancamentoEfluenteTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvPontosLancamentoEfluenteTipo);
			vm.OutorgaAguaTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvOutorgaAguaTipo);
			vm.FontesGeracaoTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvFontesGeracaoTipo);
			vm.UnidadeTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvUnidadeTipo);
			vm.CombustivelTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvCombustivelTipo);

			vm.VegetacaoAreaUtil = _listaBus.DscLicAtvVegetacaoAreaUtil;
			vm.Acondicionamento = _listaBus.DscLicAtvAcondicionamento;
			vm.Estocagem = _listaBus.DscLicAtvEstocagem;
			vm.Tratamento = _listaBus.DscLicAtvTratamento;
			vm.DestinoFinal = _listaBus.DscLicAtvDestinoFinal;

			if (vm.DscLicAtividade.Id > 0)
			{
				vm.DscLicAtividade.Dependencias = _caracterizacaoBus.ObterDependencias(vm.DscLicAtividade.Id, vm.DscLicAtividade.GetTipo, eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade);

				vm.TextoMerge = _caracterizacaoValidar.DependenciasAlteradas(
					vm.DscLicAtividade.EmpreendimentoId,
					vm.DscLicAtividade.Tipo,
					eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,
					vm.DscLicAtividade.Dependencias);
				vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			}

			vm.UrlAvancar = CaracterizacaoVM.GerarUrl(empreendimento, isCadastrarCaracterizacao, (eCaracterizacao)tipo);

			return View("Criar", vm);
		}
		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.DescricaoLicenciamentoAtividadeCriar, ePermissao.DescricaoLicenciamentoAtividadeEditar })]
		public ActionResult Salvar(DescricaoLicenciamentoAtividade dscLicAtividade, bool isCadastrarCaracterizacao)
		{
			_bus.Salvar(dscLicAtividade);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@UrlRedirecionar = CaracterizacaoVM.GerarUrl(dscLicAtividade.EmpreendimentoId, isCadastrarCaracterizacao, dscLicAtividade.GetTipo),
				@AtividadeId = dscLicAtividade.Id
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.DescricaoLicenciamentoAtividadeVisualizar })]
		public ActionResult Visualizar(int empreendimento, int tipo, bool isCadastrarCaracterizacao)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimento, tipo))
			{
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
			}

			DescricaoLicenciamentoAtividadeVM vm = new DescricaoLicenciamentoAtividadeVM();

			DescricaoLicenciamentoAtividade dscLicAtividade = _bus.ObterPorEmpreendimento(empreendimento, (eCaracterizacao)tipo);

			dscLicAtividade = _bus.ObterDadosGeo(empreendimento, (eCaracterizacao)tipo, dscLicAtividade);

			vm.CaracterizacaoTipoTexto = _caracterizacaoBus.ObterCaracterizacoesEmpreendimento(empreendimento).SingleOrDefault(x => (int)x.Tipo == tipo).Nome;
			dscLicAtividade.EmpreendimentoId = empreendimento;
			vm.DscLicAtividade = dscLicAtividade;
			vm.IsCadastrarCaracterizacao = isCadastrarCaracterizacao;

			vm.ResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(_bus.ObterResponsaveis(empreendimento));
			vm.FontesAbastecimentoAguaTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvFontesAbastecimentoAguaTipo);
			vm.PontosLancamentoEfluenteTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvPontosLancamentoEfluenteTipo);
			vm.OutorgaAguaTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvOutorgaAguaTipo);
			vm.FontesGeracaoTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvFontesGeracaoTipo);
			vm.UnidadeTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvUnidadeTipo);
			vm.CombustivelTipo = ViewModelHelper.CriarSelectList(_listaBus.DscLicAtvCombustivelTipo);

			vm.VegetacaoAreaUtil = _listaBus.DscLicAtvVegetacaoAreaUtil;
			vm.Acondicionamento = _listaBus.DscLicAtvAcondicionamento;
			vm.Estocagem = _listaBus.DscLicAtvEstocagem;
			vm.Tratamento = _listaBus.DscLicAtvTratamento;
			vm.DestinoFinal = _listaBus.DscLicAtvDestinoFinal;

			if (vm.DscLicAtividade.Id > 0)
			{
				vm.DscLicAtividade.Dependencias = _caracterizacaoBus.ObterDependencias(vm.DscLicAtividade.Id, vm.DscLicAtividade.GetTipo, eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade);

				vm.TextoMerge = _caracterizacaoValidar.DependenciasAlteradas(
					vm.DscLicAtividade.EmpreendimentoId,
					vm.DscLicAtividade.Tipo,
					eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,
					vm.DscLicAtividade.Dependencias, true);
				vm.AtualizarDependenciasModalTitulo = Mensagem.Caracterizacao.AtualizarDependenciasModalTitulo.Texto;
			}

			Boolean podeCriarEditar = new PermissaoValidar().ValidarAny(new[] { ePermissao.DescricaoLicenciamentoAtividadeCriar, ePermissao.DescricaoLicenciamentoAtividadeEditar }, false);

			if (!String.IsNullOrWhiteSpace(vm.TextoMerge) && !podeCriarEditar)
			{
				Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.NaoPodeVisualizarComDependenciasAlteradas(vm.CaracterizacaoTipoTexto));
				return RedirectToAction("", "Caracterizacao", new { id = empreendimento, Msg = Validacao.QueryParam() });
			}

			vm.UrlAvancar = CaracterizacaoVM.GerarUrl(empreendimento, isCadastrarCaracterizacao, (eCaracterizacao)tipo);
			vm.IsVisualizar = true;

			return View("Criar", vm);
		}		
	}
}
