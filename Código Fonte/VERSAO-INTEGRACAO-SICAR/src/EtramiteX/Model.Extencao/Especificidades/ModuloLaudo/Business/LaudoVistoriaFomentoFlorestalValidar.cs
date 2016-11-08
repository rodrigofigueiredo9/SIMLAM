using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoVistoriaFomentoFlorestalValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		LaudoVistoriaFomentoFlorestalDa _da = new LaudoVistoriaFomentoFlorestalDa();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		GerenciadorConfiguracao<ConfiguracaoProcesso> _atividadeConfig = new GerenciadorConfiguracao<ConfiguracaoProcesso>(new ConfiguracaoProcesso());

		public bool Salvar(IEspecificidade especificidade)
		{

			LaudoVistoriaFomentoFlorestal esp = especificidade as LaudoVistoriaFomentoFlorestal;
			RequerimentoAtividade(esp, false, true);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Laudo_Destinatario");

			ValidacoesGenericasBus.DataMensagem(esp.DataVistoria, "Laudo_DataVistoria_DataTexto", "vistoria");

			if (String.IsNullOrWhiteSpace(esp.Objetivo))
			{
				Validacao.Add(Mensagem.LaudoVistoriaFomentoFlorestal.ObjetivoObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.Consideracoes))
			{
				Validacao.Add(Mensagem.LaudoVistoriaFomentoFlorestal.ConsideracoesObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.DescricaoParecer))
			{
				Validacao.Add(Mensagem.LaudoVistoriaFomentoFlorestal.ParecerTecnicoObrigatorio);
			}

			if (esp.ConclusaoTipo <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaFomentoFlorestal.ConclusaoObrigatoria);
			}

			#region Caracterizacao

			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			int caracterizacao = caracterizacaoBus.Existe(esp.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.SilviculturaATV);

			if (caracterizacao <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaFomentoFlorestal.CaracterizacaoCadastrada);
			}
			else
			{
				CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
				List<Dependencia> dependencias = caracterizacaoBus.ObterDependencias(caracterizacao, eCaracterizacao.SilviculturaATV, eCaracterizacaoDependenciaTipo.Caracterizacao);

				string retorno = caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(),
					(int)eCaracterizacao.SilviculturaATV,
					eCaracterizacaoDependenciaTipo.Caracterizacao,
					dependencias);

				if (!string.IsNullOrEmpty(retorno))
				{
					List<CaracterizacaoLst> caracterizacoes = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
					Validacao.Add(Mensagem.LaudoVistoriaFomentoFlorestal.CaracterizacaoInvalida(caracterizacoes.SingleOrDefault(x => x.Id == (int)eCaracterizacao.SilviculturaATV).Texto));
				}
			}

			#endregion

			#region Atividade

			foreach (var atividade in esp.Atividades)
			{
				if (atividade.Id != 0 && atividade.Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ImplantacaoAtividadeSilviculturaReferenteAoFomentoFlorestal))
				{
					List<ProcessoAtividadeItem> atividades = _atividadeConfig.Obter<List<ProcessoAtividadeItem>>(ConfiguracaoProcesso.KeyAtividadesProcesso);
					Validacao.Add(Mensagem.LaudoVistoriaFomentoFlorestal.AtividadeInvalida(atividades.SingleOrDefault(x => x.Id == atividade.Id).Texto));
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}