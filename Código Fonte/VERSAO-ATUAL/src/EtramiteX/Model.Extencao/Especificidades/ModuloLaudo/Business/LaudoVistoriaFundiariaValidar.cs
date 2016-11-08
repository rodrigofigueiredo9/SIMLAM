using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoVistoriaFundiariaValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			LaudoVistoriaFundiaria esp = especificidade as LaudoVistoriaFundiaria;
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			LaudoVistoriaFundiariaBus busLaudo = new LaudoVistoriaFundiariaBus();
			List<Dependencia> dependencias = new List<Dependencia>();

			//Genéricas
			RequerimentoAtividade(esp, false, true);

			if (esp.Destinatario <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaFundiariaMsg.DestinatarioObrigatorio);
			}

			if (esp.RegularizacaoDominios.Count <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaFundiariaMsg.RegularizacaoDominioObrigatorio);
			}

			int idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.RegularizacaoFundiaria);
			if (idCaracterizacao > 0)
			{
				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.LaudoVistoriaFundiariaMsg.RegularizacaoFundiariaInvalida);
				}
			}
			else
			{
				Validacao.Add(Mensagem.LaudoVistoriaFundiariaMsg.RegularizacaoFundiariaObrigatorio);
			}

			#region Regularizacao fundiaria

			List<Int32> idsPossesRegularizacao = busLaudo.ObterPossesRegularizacao(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()).Select(x => Convert.ToInt32(x.Id)).ToList();
			esp.RegularizacaoDominios.ForEach(x =>
			{
				if (!idsPossesRegularizacao.Contains(x.DominioId)) 
				{
					Validacao.Add(Mensagem.LaudoVistoriaFundiariaMsg.RegularizacaoDominioInexistente);
				}
			});

			#endregion

			ValidacoesGenericasBus.DataMensagem(esp.DataVistoria, "Laudo_DataVistoria_DataTexto", "vistoria");

			if (busLaudo.ExistePecaTecnica(esp.Atividades.First().Id, esp.ProtocoloReq.Id) <= 0)
			{
				Validacao.Add(Mensagem.LaudoVistoriaFundiariaMsg.PecaTecnicaObrigatorio(esp.Atividades.First().NomeAtividade));
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			if (new LaudoVistoriaFundiariaBus().ValidarCaracterizacaoModificada(especificidade.Titulo.Id))
			{
				Validacao.Add(Mensagem.LaudoVistoriaFundiariaMsg.CaracterizacaoAlterada);
			}

			return Validacao.EhValido;
		}
	}
}