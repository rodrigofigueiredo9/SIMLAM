using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade.Business
{
	public class DescricaoLicenciamentoAtividadeValidar
	{
		DescricaoLicenciamentoAtividadeDa _da = new DescricaoLicenciamentoAtividadeDa();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		public bool Salvar(DescricaoLicenciamentoAtividade dscLicAtv)
		{
			if (dscLicAtv.ResidentesEntorno.HasValue && dscLicAtv.ResidentesEntorno.Value && dscLicAtv.ResidentesEnternoDistancia.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeDistancia);
			}

			if (dscLicAtv.AreaUtil.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeAreaUtil);
			}

			if (!dscLicAtv.FonteAbastecimentoAguaTipoId.HasValue)
			{
				if (dscLicAtv.FontesAbastecimentoAgua.Count == 0)
				{
					Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.AddFonteAbastecimento);
				}				
			}

			if (!dscLicAtv.PontoLancamentoTipoId.HasValue)
			{
				if (dscLicAtv.PontosLancamentoEfluente.Count == 0)
				{
					Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.AddPontoLancamento);
				}				
			}

			if (dscLicAtv.FonteAbastecimentoAguaTipoId != (int)eFontesAbastecimentoAgua.NaoPossui)
			{
				if (dscLicAtv.ConsumoAguaLs.GetValueOrDefault() <= 0 && dscLicAtv.ConsumoAguaMdia.GetValueOrDefault() <= 0 &&
					dscLicAtv.ConsumoAguaMh.GetValueOrDefault() <= 0 && dscLicAtv.ConsumoAguaMmes.GetValueOrDefault() <= 0)
				{
					Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.InformeConsumo);
				}

				if (dscLicAtv.EfluentesLiquido.Count == 0)
				{
					Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.AddFonteGeracao);
				}
			}

			if (dscLicAtv.ResiduosSolidosNaoInerte.Count == 0)
			{
				Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.AddResiduoSolidoNaoInerte);
			}

			if (dscLicAtv.EmissoesAtmosfericas.Count == 0)
			{
				Validacao.Add(Mensagem.DescricaoLicenciamentoAtividadeMsg.AddEmissoesAtm);
			}			
			
			return Validacao.EhValido;
		}
	}
}
