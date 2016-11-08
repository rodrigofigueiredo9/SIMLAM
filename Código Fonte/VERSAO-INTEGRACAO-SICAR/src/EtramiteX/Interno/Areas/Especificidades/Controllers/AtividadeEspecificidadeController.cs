using System;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.AtividadeEspecificidade;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class AtividadeEspecificidadeController : DefaultController
	{
		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.TituloCriar, ePermissao.TituloEditar, ePermissao.TituloVisualizar })]
		public ActionResult AtividadeCaracterizacao(EspecificidadeAtividadeCaracterizacaoVME especificidadeDados)
		{
			object retorno = new { };

			int atividadeCodigo = ConfiguracaoAtividade.ObterCodigo(especificidadeDados.AtividadeId);

			switch (atividadeCodigo)
			{
				case (int)eAtividadeCodigo.Barragem:
					#region Barragem

					BarragemBus _barragemBus = new BarragemBus();
					EspBarragemVM vm = new EspBarragemVM();
					vm = new EspBarragemVM();
					vm.BarragemId = especificidadeDados.DadoAuxiliarJSONDictionary.GetValue<int>("BarragemId");
					vm.IsVisualizar = especificidadeDados.DadoAuxiliarJSONDictionary.GetValue<bool>("IsVisualizar");

					if (string.IsNullOrEmpty(especificidadeDados.DadoAuxiliarJSONDictionary.GetValue<string>("CaracterizacaoTid")) || !vm.IsVisualizar)
					{
						vm.Barragens = ViewModelHelper.CriarSelectList(_barragemBus.ObterBarragens(especificidadeDados.EmpreendimentoId), true, true, selecionado: vm.BarragemId.ToString());
					}
					else
					{
						vm.Barragens = ViewModelHelper.CriarSelectList(_barragemBus.ObterBarragens(especificidadeDados.EmpreendimentoId, especificidadeDados.DadoAuxiliarJSONDictionary.GetValue<string>("CaracterizacaoTid"), vm.BarragemId.GetValueOrDefault()), true, true, selecionado: vm.BarragemId.ToString());
					}

					retorno = new
					{
						@Msg = Validacao.Erros,
						@EhValido = Validacao.EhValido,
						@Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "~/Areas/Especificidades/Views/AtividadeEspecificidade/EspBarragem.ascx", vm)
					};

					#endregion
					break;
			}

			return Json(retorno, JsonRequestBehavior.AllowGet);
		}

	}
}