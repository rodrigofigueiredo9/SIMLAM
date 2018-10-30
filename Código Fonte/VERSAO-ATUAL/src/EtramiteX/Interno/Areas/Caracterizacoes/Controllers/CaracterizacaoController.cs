using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CaracterizacaoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		CaracterizacaoBus _bus = new CaracterizacaoBus(new CaracterizacaoValidar());
		CaracterizacaoValidar _validar = new CaracterizacaoValidar();
		ExploracaoFlorestalBus _exploracaoFlorestalBus = new ExploracaoFlorestalBus();

		#endregion

		[Permite(Tipo=ePermiteTipo.Logado)]
		public ActionResult Index(int id)
		{
			EmpreendimentoCaracterizacao empreendimento = _bus.ObterEmpreendimentoSimplificado(id);

			if (empreendimento.Id == 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoNaoEncontrado);
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			CaracterizacaoVM vm = new CaracterizacaoVM(empreendimento);

			List<CaracterizacaoLst> caracterizacoes = _bus.CaracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			List<ProjetoGeografico> projetos = _bus.ObterProjetosEmpreendimento(id);

			if (!_bus.ValidarAcessarTela(caracterizacoes))
			{
				return RedirectToAction("Index", "../Empreendimento", Validacao.QueryParamSerializer());
			}

			List<Caracterizacao> cadastradas = _bus.ObterCaracterizacoesEmpreendimento(id) ?? new List<Caracterizacao>();

			vm.CaracterizacoesNaoCadastradas = caracterizacoes
							.Where(x => !cadastradas.Any(y => y.Id > 0 && (int)y.Tipo == x.Id))
							.Select(z => new CaracterizacaoVME() { Tipo = (eCaracterizacao)z.Id, Nome = z.Texto }).ToList();

			vm.CaracterizacoesCadastradas = caracterizacoes
							.Where(x => cadastradas.Any(y => y.Id > 0 && (int)y.Tipo == x.Id))
							.Select(z => new CaracterizacaoVME() { Tipo = (eCaracterizacao)z.Id, Nome = z.Texto }).ToList();

			List<DependenciaLst> dependencias = _bus.CaracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);

			vm.CaracterizacoesNaoCadastradas.ForEach(x =>
			{
				x.PodeCadastrar = User.IsInRole(String.Format("{0}Criar", x.Tipo.ToString()));
				x.ProjetoGeografico = User.IsInRole("ProjetoGeograficoCriar");
				x.DscLicAtividade = User.IsInRole("DescricaoLicenciamentoAtividadeCriar");
				x.ProjetoGeoObrigatorio = dependencias.Exists(y =>
					y.DependenteTipo == (int)x.Tipo &&
					y.DependenciaTipo == (int)x.Tipo &&
					y.TipoId == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico);

				x.DscLicAtividadeObrigatorio = dependencias.Exists(y =>
					y.DependenteTipo == (int)x.Tipo &&
					y.DependenciaTipo == (int)x.Tipo &&
					y.TipoId == (int)eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade);

				Caracterizacao cadastrada = cadastradas.SingleOrDefault(y => y.Tipo == x.Tipo) ?? new Caracterizacao();
				x.ProjetoGeograficoId = cadastrada.ProjetoRascunhoId;
				x.DscLicAtividadeId = cadastrada.DscLicAtividadeId;

				if (x.ProjetoGeograficoId <= 0)
				{
					x.ProjetoGeograficoId = cadastrada.ProjetoId;
				}

				x.UrlCriar = Url.Action("Criar", x.Tipo.ToString());
			});

			vm.CaracterizacoesCadastradas.ForEach(x =>
			{
				if (User.IsInRole("ProjetoGeograficoEditar") || User.IsInRole("ProjetoGeograficoVisualizar"))
				{
					x.ProjetoGeografico = true;
				}

				x.ProjetoGeograficoVisualizar = 
					User.IsInRole(ePermissao.ProjetoGeograficoVisualizar.ToString()) && 
					!User.IsInRole(ePermissao.ProjetoGeograficoCriar.ToString()) && 
					!User.IsInRole(ePermissao.ProjetoGeograficoEditar.ToString());

				if (User.IsInRole("DescricaoLicenciamentoAtividadeEditar") || User.IsInRole("DescricaoLicenciamentoAtividadeVisualizar"))
				{
					x.DscLicAtividade = true;
				}

				x.DscLicAtividadeVisualizar =
					User.IsInRole(ePermissao.DescricaoLicenciamentoAtividadeVisualizar.ToString()) &&
					!User.IsInRole(ePermissao.DescricaoLicenciamentoAtividadeCriar.ToString()) &&
					!User.IsInRole(ePermissao.DescricaoLicenciamentoAtividadeEditar.ToString());

				if (x.Tipo == eCaracterizacao.ExploracaoFlorestal)
				{
					var exploracao = _exploracaoFlorestalBus.ObterPorEmpreendimento(id, simplificado: true);
					if(exploracao.Id > 0)
					{
						x.PodeEditar = User.IsInRole(String.Format("{0}Editar", x.Tipo.ToString()));
						x.PodeExcluir = User.IsInRole(String.Format("{0}Excluir", x.Tipo.ToString()));
					}
				}
				else
				{
					x.PodeEditar = User.IsInRole(String.Format("{0}Editar", x.Tipo.ToString()));
					x.PodeExcluir = User.IsInRole(String.Format("{0}Excluir", x.Tipo.ToString()));
				}
				x.PodeVisualizar = User.IsInRole(String.Format("{0}Visualizar", x.Tipo.ToString()));

                // #2377: Alteração para resolver o problema de "sequence contains more than one matching element"
				//Caracterizacao cadastrada = cadastradas.SingleOrDefault(y => y.Tipo == x.Tipo) ?? new Caracterizacao();

                Caracterizacao cadastrada = cadastradas.FirstOrDefault(y => y.Tipo == x.Tipo) ?? new Caracterizacao();
				x.ProjetoGeograficoId = cadastrada.ProjetoId;
				if(cadastrada.ProjetoId == 0 && cadastrada.Tipo == eCaracterizacao.ExploracaoFlorestal)
					x.ProjetoGeograficoId = cadastrada.ProjetoRascunhoId;
				x.DscLicAtividadeId = cadastrada.DscLicAtividadeId;

				x.UrlEditar = Url.Action("Editar", x.Tipo.ToString());
				x.UrlVisualizar = Url.Action("Visualizar", x.Tipo.ToString());
				x.UrlExcluirConfirm = Url.Action("ExcluirConfirm", x.Tipo.ToString());
				x.UrlExcluir = Url.Action("Excluir", x.Tipo.ToString());
			});

			#region CAR

			vm.CaracterizacoesCadastradas.Where(x => x.Tipo == eCaracterizacao.CadastroAmbientalRural).ToList().ForEach(x =>
			{
				x.ProjetoGeografico = false;
				x.ProjetoGeograficoVisualizar = false;
				x.ProjetoGeograficoId = 0;

				x.DscLicAtividade = false;
				x.DscLicAtividadeVisualizar = false;
				x.DscLicAtividadeId = 0;
			});

			#endregion

			return View("Caracterizacao", vm);
		}
	}
}