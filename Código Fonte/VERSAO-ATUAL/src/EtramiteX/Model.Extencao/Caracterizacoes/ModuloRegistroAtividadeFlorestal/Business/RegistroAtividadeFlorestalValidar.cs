using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal.Business
{
	public class RegistroAtividadeFlorestalValidar
	{
		RegistroAtividadeFlorestalDa _da = new RegistroAtividadeFlorestalDa();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		ConfiguracaoCaracterizacao _config = new ConfiguracaoCaracterizacao();

		internal bool Salvar(RegistroAtividadeFlorestal caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new RegistroAtividadeFlorestal()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			#region Numero

			if (!caracterizacao.PossuiNumero.HasValue)
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.PossuiNumeroObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(caracterizacao.NumeroRegistro) && caracterizacao.PossuiNumero.GetValueOrDefault())
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroRegistroObrigatorio);
			}
			else if (!ValidacoesGenericasBus.ValidarNumero(caracterizacao.NumeroRegistro, 9) && caracterizacao.PossuiNumero.GetValueOrDefault())
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroRegistroInvalido);
			}
			else if (_da.IsNumeroUtilizado(caracterizacao.NumeroRegistro, caracterizacao.EmpreendimentoId))
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroUtilizado);
			}
			else if (caracterizacao.PossuiNumero.GetValueOrDefault())
			{
				int numeroMaximo = _config.RegAtvFlorestalNumeroMaximo;
				if (Convert.ToInt32(caracterizacao.NumeroRegistro) > numeroMaximo)
				{
					Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroRegistroSuperiorMax(numeroMaximo.ToString()));
				}
			}

			#endregion

			if (caracterizacao.Consumos.Count <= 0)
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.ConsumoObrigatorio);
			}

			#region Consumos

			var count = 0;
			caracterizacao.Consumos.ForEach(r =>
			{
				if (caracterizacao.Consumos.Any(x => caracterizacao.Consumos.Count(y => y.Atividade == x.Atividade) > 1))
				{
					Validacao.Add(Mensagem.RegistroAtividadeFlorestal.CategoriaDuplicada);
					return;
				}

				AtividadeSolicitada atividadeSolicitada = _da.ObterAtividadeSolicitada(r.Atividade);
				if (atividadeSolicitada != null && !atividadeSolicitada.Situacao)
				{
					Validacao.Add(Mensagem.RegistroAtividadeFlorestal.AtividadeDesabilitada(atividadeSolicitada.Texto, count));
				}
				else
				{
					if (r.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.FabricanteMotosserra) ||
						r.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ComercianteMotosserra))
					{
						r.FontesEnergia.Clear();
					}
					else
					{
						if (r.Atividade == 0)
						{
							Validacao.Add(Mensagem.RegistroAtividadeFlorestal.CategoriaObrigatoria(count));
							return;
						}

						if (r.FontesEnergia.Count == 0)
						{
							Validacao.Add(Mensagem.RegistroAtividadeFlorestal.FonteEnergiaObrigatoria(count));
							return;
						}
					}

					if (!r.PossuiLicencaAmbiental.HasValue)
					{
						Validacao.Add(Mensagem.RegistroAtividadeFlorestal.PossuiLicencaAmbiental(count));
					}
					else
					{
						switch (r.PossuiLicencaAmbiental.GetValueOrDefault())
						{
							case ConfiguracaoSistema.NAO:
								if (_caracterizacaoValidar.AtividadeLicencaObrigatoria(r.Atividade))
								{
									Validacao.Add(Mensagem.RegistroAtividadeFlorestal.LicencaAmbientalObrigatoria(count, atividadeSolicitada.Categoria + " - " + atividadeSolicitada.Texto));
								}
								break;

							case ConfiguracaoSistema.SIM:
								if (!r.Licenca.EmitidoPorInterno.HasValue)
								{
									Validacao.Add(Mensagem.RegistroAtividadeFlorestal.EmitidoIDAFOuExterno(count));
								}
								else
								{
									if (r.Licenca.TituloModelo == 0)
									{
										Validacao.Add(Mensagem.RegistroAtividadeFlorestal.ModeloLicencaObrigatorio(count));
									}

									if (string.IsNullOrWhiteSpace(r.Licenca.TituloModeloTexto))
									{
										Validacao.Add(Mensagem.RegistroAtividadeFlorestal.ModeloLicencaObrigatorio(count));
									}

									if (string.IsNullOrWhiteSpace(r.Licenca.TituloNumero))
									{
										Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroLicencaObrigatorio(count));
									}
									else
									{
										if (r.Licenca.EmitidoPorInterno.Value && !ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(r.Licenca.TituloNumero))
										{
											Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroLicencaInvalido(count));
										}
									}

									ValidacoesGenericasBus.DataMensagem(r.Licenca.TituloValidadeData, "TituloValidadeData_DataTexto" + count, "validade", false);

									if (string.IsNullOrWhiteSpace(r.Licenca.ProtocoloNumero))
									{
										Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroProtocoloLicencaObrigatorio(count));
									}

									if (r.Licenca.TituloValidadeData.Data.GetValueOrDefault() < DateTime.Today || !r.Licenca.ProtocoloRenovacaoData.IsEmpty)
									{
										ValidacoesGenericasBus.DataMensagem(r.Licenca.ProtocoloRenovacaoData, "ProtocoloRenovacaoData_DataTexto" + count, "renovação");
									}

									if (!r.Licenca.ProtocoloRenovacaoData.IsEmpty && r.Licenca.TituloValidadeData.Data.GetValueOrDefault() < r.Licenca.ProtocoloRenovacaoData.Data)
									{
										Validacao.Add(Mensagem.RegistroAtividadeFlorestal.RenovacaoDataMaiorValidade(count));
									}

									if (r.Licenca.TituloValidadeData.Data.GetValueOrDefault() < DateTime.Today && string.IsNullOrWhiteSpace(r.Licenca.ProtocoloRenovacaoNumero))
									{
										Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroProtocoloRenovacaoObrigatorio(count));
									}

									if (!r.Licenca.EmitidoPorInterno.Value && string.IsNullOrWhiteSpace(r.Licenca.OrgaoExpedidor))
									{
										Validacao.Add(Mensagem.RegistroAtividadeFlorestal.OrgaoExpedidoObrigatorio(count));
									}
								}
								break;

							case ConfiguracaoSistema.Dispensado:
								if (!r.Licenca.EmitidoPorInterno.HasValue)
								{
									Validacao.Add(Mensagem.RegistroAtividadeFlorestal.GrupoDispensadoEmitidoIDAFOuExterno(count));
								}

								if (string.IsNullOrWhiteSpace(r.Licenca.TituloModeloTexto))
								{
									Validacao.Add(Mensagem.RegistroAtividadeFlorestal.DocumentoObrigatorio(count));
								}

								if (string.IsNullOrWhiteSpace(r.Licenca.ProtocoloNumero))
								{
									Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroProtocoloLicencaObrigatorio(count));
								}

								if (string.IsNullOrWhiteSpace(r.Licenca.OrgaoExpedidor))
								{
									Validacao.Add(Mensagem.RegistroAtividadeFlorestal.OrgaoEmissorObrigatorio(count));
								}
								break;

							default:
								break;
						}
					}

					if (string.IsNullOrWhiteSpace(r.DUANumero))
					{
						Validacao.Add(Mensagem.RegistroAtividadeFlorestal.DUANumeroObrigatorio(count));
					}

					if (Convert.ToDecimal(r.DUAValor) <= 0)
					{
						Validacao.Add(Mensagem.RegistroAtividadeFlorestal.DUAValorObrigatorio(count));
					}

					ValidacoesGenericasBus.DataMensagem(r.DUADataPagamento, "DUADataPagamento" + count, "pagamento do DUA");
				}

				count++;
			});

			#endregion

			return Validacao.EhValido;
		}

		public void Consumo(List<Consumo> consumoLista)
		{
			var count = 0;
			consumoLista.ForEach(r =>
			{
				if (r.Atividade != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.FabricanteMotosserra) &&
					r.Atividade != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ComercianteMotosserra))
				{
					if (r.Atividade == 0 || r.FontesEnergia.Count == 0)
					{
						Validacao.Add(Mensagem.RegistroAtividadeFlorestal.CamposObrigatorio(count));
					}
				}

				count++;
			});
		}

		public void Fonte(List<FonteEnergia> fonteLista, FonteEnergia fonte, int index)
		{
			fonteLista = fonteLista ?? new List<FonteEnergia>();

			if(fonte.TipoId < 1)
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.FonteTipoObrigatorio(index));
			}

			if (fonte.UnidadeId < 1)
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.UnidadeObrigatoria(index));
			}

			if (string.IsNullOrEmpty(fonte.Qde))
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.QdeObrigatoria(index));
			}

			decimal qdeFlorestaPlantada = Convert.ToDecimal(fonte.QdeFlorestaPlantada);
			decimal qdeFlorestaNativa = Convert.ToDecimal(fonte.QdeFlorestaNativa);
			if ((qdeFlorestaPlantada > 0 || qdeFlorestaNativa > 0) && (qdeFlorestaPlantada + qdeFlorestaNativa) != Convert.ToDecimal(fonte.Qde))
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.QdePlantadaNativaMaiorAno(index));
			}

			decimal qdeOutroEstado = Convert.ToDecimal(fonte.QdeOutroEstado);
			if (qdeOutroEstado > 0 && (qdeOutroEstado > Convert.ToDecimal(fonte.Qde)))
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.QdeOutroEstadoMaiorAno(index));
			}

			if (qdeFlorestaPlantada <= 0 && qdeFlorestaNativa <= 0 && qdeOutroEstado <= 0)
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.PeloMenosUmaQdeObrigatoria(index));
			}

			if (fonteLista.Count(x => x.TipoId == fonte.TipoId) > 0)
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.FonteDuplicada(index));
			}
		}

		public bool ObterTitulo(TituloFiltro filtros, int indice)
		{
			filtros.SituacoesFiltrar.Add((int)eTituloSituacao.Concluido);
			filtros.SituacoesFiltrar.Add((int)eTituloSituacao.Prorrogado);

			if (filtros.Modelo < 1)
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.ModeloLicencaObrigatorio(indice));
			}

			if (String.IsNullOrEmpty(filtros.Numero))
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.NumeroLicencaObrigatorio(indice));
			}

			return Validacao.EhValido;
		}

		public void AposObterTitulo(TituloFiltro filtros, List<Titulo> resultados)
		{
			if (resultados == null || (resultados != null && resultados.Count < 1))
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.LicencaAmbientalNaoEncontrada(filtros.ModeloTexto, filtros.Numero));
			}
			else
			{
				Validacao.Add(Mensagem.RegistroAtividadeFlorestal.LicencaAmbientalEncontrada(filtros.ModeloTexto, filtros.Numero));
			}
		}
	}
}