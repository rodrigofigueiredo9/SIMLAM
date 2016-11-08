using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class InfracaoValidar
	{
		InfracaoDa _da = new InfracaoDa();

		public bool Salvar(Infracao infracao)
		{
			#region Classificação da infração

			if (infracao.ClassificacaoId == 0)
			{
				Validacao.Add(Mensagem.InfracaoMsg.ClassificacaoObrigatorio);
			}

			if (infracao.TipoId == 0)
			{
				Validacao.Add(Mensagem.InfracaoMsg.TipoInfracaoObrigatorio);
			}

			if (infracao.ItemId == 0)
			{
				Validacao.Add(Mensagem.InfracaoMsg.ItemObrigatorio);
			}

			if (infracao.Campos.Count > 0 && infracao.Campos.Count(x => string.IsNullOrWhiteSpace(x.Texto)) > 0)
			{
				Validacao.Add(Mensagem.InfracaoMsg.CamposObrigatorioo);
			}

			if (infracao.Perguntas.Count > 0 && infracao.Perguntas.Count(x => x.RespostaId == 0 || (x.IsEspecificar && string.IsNullOrWhiteSpace(x.Especificacao))) > 0)
			{
				Validacao.Add(Mensagem.InfracaoMsg.QuestionariosObrigatorio);
			}

			#endregion

			#region Dados do auto de infração

			if (infracao.IsAutuada == null)
			{
				Validacao.Add(Mensagem.InfracaoMsg.InfracaoAutuadaObrigatorio);
			}
			else if (infracao.IsAutuada.Value)
			{
				if (infracao.IsGeradaSistema == null)
				{
					Validacao.Add(Mensagem.InfracaoMsg.AutoGeradoSistemaObrigatorio);
				}
				else if (!infracao.IsGeradaSistema.GetValueOrDefault())
				{
					if (string.IsNullOrWhiteSpace(infracao.NumeroAutoInfracaoBloco))
					{
						if (infracao.SerieId == (int)eSerie.C)
						{
							Validacao.Add(Mensagem.InfracaoMsg.NumeroAutoInfracaoObrigatorio);
						}
						else
						{
							Validacao.Add(Mensagem.InfracaoMsg.NumeroAutoInfracaoBlocoObrigatorio);
						}
					}

					ValidacoesGenericasBus.DataMensagem(infracao.DataLavraturaAuto, "Infracao_DataLavraturaAuto", "lavratura do auto");

				}

				if (infracao.SerieId == 0)
				{
					Validacao.Add(Mensagem.InfracaoMsg.SerieObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(infracao.DescricaoInfracao))
				{
					Validacao.Add(Mensagem.InfracaoMsg.DescricaoInfracaoObrigatorio);
				}

				if (infracao.CodigoReceitaId == 0)
				{
					Validacao.Add(Mensagem.InfracaoMsg.CodigoReceitaObrigatorio);
				}

				if (string.IsNullOrWhiteSpace(infracao.ValorMulta))
				{
					Validacao.Add(Mensagem.InfracaoMsg.ValorMultaObrigatorio);
				}
				else 
				{
					Decimal aux = 0;
					if (!Decimal.TryParse(infracao.ValorMulta, out aux))
					{
						Validacao.Add(Mensagem.InfracaoMsg.ValorMultaInvalido);
					}
				}
			}

			#endregion

			if (_da.ConfigAlterada(infracao.ConfiguracaoId, infracao.ConfiguracaoTid))
			{
				Validacao.Add(Mensagem.InfracaoMsg.ConfigAlteradaSemAtualizar);
			}

			if (_da.PerguntaRespostaAlterada(infracao))
			{
				Validacao.Add(Mensagem.InfracaoMsg.ConfigAlteradaSemAtualizar);
			}

			return Validacao.EhValido;
		}
	}
}
