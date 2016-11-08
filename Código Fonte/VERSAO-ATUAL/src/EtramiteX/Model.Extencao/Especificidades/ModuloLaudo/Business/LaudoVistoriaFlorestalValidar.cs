using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoVistoriaFlorestalValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		LaudoConstatacaoDa _da = new LaudoConstatacaoDa();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		public bool Salvar(IEspecificidade especificidade)
		{
			LaudoVistoriaFlorestal esp = especificidade as LaudoVistoriaFlorestal;
			RequerimentoAtividade(esp, apenasObrigatoriedade: true);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Laudo_Destinatario");

			ValidacoesGenericasBus.DataMensagem(esp.DataVistoria, "Laudo_DataVistoria_DataTexto", "vistoria");

			if (String.IsNullOrWhiteSpace(esp.Objetivo))
			{
				Validacao.Add(Mensagem.LaudoVistoriaFlorestalMsg.ObjetivoObrigatorio);
			}

			if (esp.Caracterizacao <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaFlorestalMsg.CaracterizacaoObrigatoria);
			}
			else
			{
				CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
				int caracterizacao = caracterizacaoBus.Existe(esp.Titulo.EmpreendimentoId.GetValueOrDefault(), (eCaracterizacao)esp.Caracterizacao);

				if (caracterizacao <= 0)
				{
					Validacao.Add(Mensagem.LaudoVistoriaFlorestalMsg.CaracterizacaoCadastrada);
				}
				else
				{
					CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
					List<Dependencia> dependencias = caracterizacaoBus.ObterDependencias(caracterizacao, 
						(eCaracterizacao)esp.Caracterizacao, 
						eCaracterizacaoDependenciaTipo.Caracterizacao);

					string retorno = caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), 
						esp.Caracterizacao, 
						eCaracterizacaoDependenciaTipo.Caracterizacao, 
						dependencias);

					if (!string.IsNullOrEmpty(retorno))
					{
						List<CaracterizacaoLst> caracterizacoes = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
						Validacao.Add(Mensagem.LaudoVistoriaFlorestalMsg.CaracterizacaoInvalida(caracterizacoes.SingleOrDefault(x => x.Id == esp.Caracterizacao).Texto));
					}
				}
			}

			if (String.IsNullOrWhiteSpace(esp.Consideracao))
			{
				Validacao.Add(Mensagem.LaudoVistoriaFlorestalMsg.ConsideracoesObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.ParecerDescricao))
			{
				Validacao.Add(Mensagem.LaudoVistoriaFlorestalMsg.ParecerTecnicoDescricaoObrigatorio);
			}

			if (esp.Conclusao <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaFlorestalMsg.ConclusaoObrigatoria);
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}