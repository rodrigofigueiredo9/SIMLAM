using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business
{
	public class AtividadeInternoValidar
	{
		AtividadeInternoDa _da = new AtividadeInternoDa();

		public bool AlterarSituacao(Atividade atividade)
		{
			eAtividadeSituacao atividadeSituacaoAtual = (eAtividadeSituacao)_da.ObterAtividadeProtocoloSituacao(atividade);

			if ((eAtividadeSituacao)atividade.SituacaoId == atividadeSituacaoAtual)
			{
				return false;
			}

			//Situações fim--------------
			if ((eAtividadeSituacao)atividade.SituacaoId != eAtividadeSituacao.EmAndamento && 
				atividadeSituacaoAtual == eAtividadeSituacao.Indeferida) //id da situação (lov_protocolo_atividade_sit)
			{
				Validacao.Add(Mensagem.Atividade.Indefirida);
				return false;
			}

			if (atividadeSituacaoAtual == eAtividadeSituacao.Encerrada) //id da situação (lov_protocolo_atividade_sit)
			{
				Validacao.Add(Mensagem.Atividade.Encerrada);
				return false;
			}
			return Validacao.EhValido;
		}
	}
}