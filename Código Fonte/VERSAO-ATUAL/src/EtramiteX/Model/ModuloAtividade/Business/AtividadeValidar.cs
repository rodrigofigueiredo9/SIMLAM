using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business
{
	public class AtividadeValidar
	{
		AtividadeDa _da = new AtividadeDa();

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

		public bool EncerrarAtividadeManual(Atividade atividade)
		{
			eAtividadeSituacao atividadeSituacaoAtual = (eAtividadeSituacao)_da.ObterAtividadeProtocoloSituacao(atividade);

			if ((atividadeSituacaoAtual != eAtividadeSituacao.EmAndamento && atividadeSituacaoAtual != eAtividadeSituacao.ComPendencia)) 
			{
				Validacao.Add(Mensagem.Atividade.EncerrarSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		public bool ValidarEncerarAtividade(int id, bool isProcesso)
		{
			try
			{
				if (isProcesso)
				{
					ProcessoBus busProcesso = new ProcessoBus();
					string numero = busProcesso.VerificarProcessoApensadoNumero(id);
					if (!string.IsNullOrEmpty(numero))
					{
						Validacao.Add(Mensagem.Processo.ProcessoApensado(numero));
						return false;
					}

					if (!busProcesso.EmPosse(id))
					{
						Validacao.Add(Mensagem.Processo.PosseProcessoNecessariaAtividade);
					}
				}
				else
				{
					DocumentoBus busDocumento = new DocumentoBus();
					string numero = busDocumento.VerificarDocumentoJuntadoNumero(id);
					if (!string.IsNullOrEmpty(numero))
					{
						Validacao.Add(Mensagem.Documento.DocumentoJuntado(numero));
						return false;
					}

					if (!busDocumento.EmPosse(id))
					{
						Validacao.Add(Mensagem.Documento.PosseDocumentoNecessariaAtividade);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool EncerrarAtividade(int id, bool isProcesso)
		{
			try
			{
				ValidarEncerarAtividade(id, isProcesso);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool EncerrarAtividadeMotivo(string motivo, int id, bool isProcesso)
		{
			try
			{
				if (ValidarEncerarAtividade(id, isProcesso))
				{
					if (string.IsNullOrEmpty(motivo))
					{
						Validacao.Add(Mensagem.Processo.MotivoObrigatorio);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}
	}
}