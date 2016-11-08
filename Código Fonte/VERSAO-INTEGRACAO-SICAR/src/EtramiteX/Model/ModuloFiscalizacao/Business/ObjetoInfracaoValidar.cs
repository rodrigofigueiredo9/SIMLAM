using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ObjetoInfracaoValidar
	{
		public bool Salvar(ObjetoInfracao objetoInfracao)
		{

			#region Área embargada e/ou atividade interditada

			if (objetoInfracao.AreaEmbargadaAtvIntermed.HasValue)
			{
				if (objetoInfracao.AreaEmbargadaAtvIntermed.Value == (int)Resposta.Sim)
				{

					#region TEI gerado pelo sistema

					if (objetoInfracao.TeiGeradoPeloSistema.HasValue)
					{
						if (objetoInfracao.TeiGeradoPeloSistema.Value == (int)Resposta.Nao)
						{

							if (objetoInfracao.TeiGeradoPeloSistemaSerieTipo == (int)eSerie.C)
							{
								#region Nº do TEI

								if (!String.IsNullOrEmpty(objetoInfracao.NumTeiBloco))
								{
									Int32 aux = 0;
									if (Int32.TryParse(objetoInfracao.NumTeiBloco, out aux))
									{
										if (aux <= 0)
										{
											Validacao.Add(Mensagem.ObjetoInfracao.NumTeiMaiorZero);
										}
									}
									else
									{
										Validacao.Add(Mensagem.ObjetoInfracao.NumTeiInvalido);
									}
								}
								else
								{
									Validacao.Add(Mensagem.ObjetoInfracao.NumTeiObrigatorio);
								}

								#endregion
							}
							else
							{
								#region Nº do TEI - bloco

								if (!String.IsNullOrEmpty(objetoInfracao.NumTeiBloco))
								{
									Decimal aux = 0;
									if (Decimal.TryParse(objetoInfracao.NumTeiBloco, out aux))
									{
										if (aux <= 0)
										{
											Validacao.Add(Mensagem.ObjetoInfracao.NumTeiBlocoMaiorZero);
										}
									}
									else
									{
										Validacao.Add(Mensagem.ObjetoInfracao.NumTeiBlocoInvalido);
									}
								}
								else
								{
									Validacao.Add(Mensagem.ObjetoInfracao.NumTeiBlocoObrigatorio);
								}

								#endregion
							}

							#region Data da lavratura

							ValidacoesGenericasBus.DataMensagem(objetoInfracao.DataLavraturaTermo, "ObjetoInfracao_DataLavraturaTermo_DataTexto", "lavratura do termo");

							#endregion

						}
					}
					else
					{
						Validacao.Add(Mensagem.ObjetoInfracao.TeiGeradoPeloSistemaObrigatorio);
					}

					if (objetoInfracao.TeiGeradoPeloSistemaSerieTipo <= 0)
					{
						Validacao.Add(Mensagem.ObjetoInfracao.TeiGeradoPeloSistemaSerieTipoObrigatorio);
					}

					#endregion

					if (String.IsNullOrWhiteSpace(objetoInfracao.DescricaoTermoEmbargo))
					{
						Validacao.Add(Mensagem.ObjetoInfracao.DescricaoTermoEmbargoObrigatorio);
					}

				}
			}
			else
			{
				Validacao.Add(Mensagem.ObjetoInfracao.AreaEmbarcadaAtvIntermedObrigatorio);
			}

			#endregion

			#region Esta sendo desenvolvida alguma atividade na area degradada

			if (!objetoInfracao.ExisteAtvAreaDegrad.HasValue)
			{
				Validacao.Add(Mensagem.ObjetoInfracao.ExisteAtvAreaDegradObrigatorio);
			}
			else
			{
				if (objetoInfracao.ExisteAtvAreaDegrad.Value == (int)Resposta.Sim)
				{
					if (String.IsNullOrWhiteSpace(objetoInfracao.ExisteAtvAreaDegradEspecificarTexto))
					{
						Validacao.Add(Mensagem.ObjetoInfracao.ExisteAtvAreaDegradEspecificarTextoObrigatorio);
					}
				}
			}

			#endregion

			#region Fundamentos que caracterizam a infração

			if (String.IsNullOrWhiteSpace(objetoInfracao.FundamentoInfracao))
			{
				Validacao.Add(Mensagem.ObjetoInfracao.FundamentoInfracaoObrigatorio);
			}

			#endregion

			#region Declividade média da área

			if (!String.IsNullOrWhiteSpace(objetoInfracao.AreaDeclividadeMedia))
			{
				Decimal aux = 0;
				if (Decimal.TryParse(objetoInfracao.AreaDeclividadeMedia, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.ObjetoInfracao.AreaDeclividadeMediaMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.ObjetoInfracao.AreaDeclividadeMediaInvalida);
				}
			}

			#endregion

			if (objetoInfracao.InfracaoResultouErosaoTipo == (int)Resposta.Sim && 
				String.IsNullOrWhiteSpace(objetoInfracao.InfracaoResultouErosaoTipoTexto))
			{
				Validacao.Add(Mensagem.ObjetoInfracao.EspecificarObrigatorio);
			}

			return Validacao.EhValido;
		}

		internal enum Resposta
		{
			Nao = 0,
			Sim = 1
		}
	}
}
