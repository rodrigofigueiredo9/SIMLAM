using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business
{
	public class RequerimentoValidar
	{
		#region Propriedades

		AtividadeConfiguracaoBus _atividadeConfigurada = new AtividadeConfiguracaoBus();
		RequerimentoMsg Msg = new RequerimentoMsg();
		PessoaBus _busPessoa = new PessoaBus(string.Empty);
		RoteiroBus _roteiroBus = new RoteiroBus();
		AtividadeBus _atividadeBus = new AtividadeBus();
		RequerimentoDa _requerimentoDa = new RequerimentoDa();
		TituloModeloBus _tituloModeloBus = new TituloModeloBus(new TituloModeloValidacao());
		ChecagemRoteiroBus _checkListRoteiroBus = new ChecagemRoteiroBus();
		EmpreendimentoBus _busEmpreendimento = new EmpreendimentoBus();
		#endregion

		public bool ObjetivoPedidoValidar(Requerimento requerimento)
		{
			RequerimentoBus _bus = new RequerimentoBus();

		    //Atividade Barragem dispensada de licenciamento ambiental -> só pode ser feito no Credenciado
			if (requerimento.Atividades.Count(x => x.Id == 327) > 0)
			{
				Validacao.Add(Msg.RequerimentoBarragemInstitucional);
			}

			if (requerimento.DataCadastro.ToString() == string.Empty)
			{
				Validacao.Add(Msg.DataCriacaobrigatorio);
			}

			if (requerimento.AgendamentoVistoria <= 0)
			{
				Validacao.Add(Msg.AgendamentObriagatorio);
			}

			if (!requerimento.IsCredenciado && requerimento.SetorId <= 0)
			{
				Validacao.Add(Msg.SetorObrigatorio);
			}

			_bus.ValidarRoteiroRemovido(requerimento, true);

			if (requerimento.Roteiros.Count <= 0)
			{
				Validacao.Add(Msg.RoteiroObrigatorio);
			}

			foreach (Roteiro item in requerimento.Roteiros)
			{
				if (_roteiroBus.ObterSituacao(item.Id) == 2)
				{
					Validacao.Add(Msg.RoteiroDesativoAoCadastrar(item.Numero));
				}
			}
			//informação de corte
			if(requerimento.Atividades.Count() > 1 && requerimento.Atividades.Any(item => item.Id == 209))
			{
				Validacao.Add(Msg.AtividadeInformacaoCorte);
			}

			ValidarAtividade(requerimento.Atividades);

			return Validacao.EhValido;
		}

		public bool InteressadoValidar(Requerimento requerimento)
		{
			if (requerimento.Interessado.Id <= 0)
			{
				Validacao.Add(Msg.InteressadoObrigatorio);
				return Validacao.EhValido;
			}

			if (!_busPessoa.ExisteEndereco(requerimento.Interessado.Id))
			{
				Validacao.Add(Msg.InteressadoSemEndereco);
				return Validacao.EhValido;
			}
			// empreedimento responsavel
			if (!_busEmpreendimento.ExisteEmpreendimentoResponsavel(requerimento.Interessado.Id))
				Validacao.Add(Msg.NaoExisteEmpreedimentoAssociadoResponsavelInstitucional);

			return Validacao.EhValido;
		}

		public bool ValidarAtividade(List<Atividade> atividades, string local = null)
		{
			if (atividades.Count <= 0)
			{
				Validacao.Add(Msg.AtividadeObrigatorio);
				return Validacao.EhValido;
			}

			foreach (Atividade atividade in atividades)
			{
				if (!_atividadeBus.ExisteAtividadeNoSetor(atividade.Id, atividade.SetorId))
				{
					Validacao.Add(Msg.AtividadeNaoEstaNoSetorInformado(atividade.NomeAtividade));
				}
			}

			int SetorId = atividades[0].SetorId;
			int titulos = 0;
			int titulosDeclaratorio = 0;
			string tituloDeclaratorioModelo = string.Empty;

			foreach (Atividade atividade in atividades)
			{
				#region Atividades

				if (SetorId != atividade.SetorId)
				{
					Validacao.Add(Msg.AtividadesSetoresDiferentes);
					return Validacao.EhValido;
				}

				if (atividade.Finalidades.Count < 1)
				{
					Validacao.Add(Msg.TituloObrigatorio(atividade.NomeAtividade));
					return Validacao.EhValido;
				}

				if (!_atividadeBus.AtividadeAtiva(atividade.Id))
				{
					Validacao.Add(Msg.AtividadeDesativada(atividade.NomeAtividade, local));
				}

				foreach (var item in atividade.Finalidades)
				{
					#region Finalidades

					if (item.TituloModelo == 0)
					{
						Validacao.Add(Msg.TituloObrigatorio(atividade.NomeAtividade));
					}

					//Verifica se a situação da atividade é encerrada ou já tem título emitido
					if (atividade.Protocolo.Id > 0 && _atividadeBus.ValidarAtividadeComTituloOuEncerrada(atividade.Protocolo.Id.Value, atividade.Protocolo.IsProcesso, atividade.Id, item.TituloModelo))
					{
						continue;
					}

					if (!_atividadeConfigurada.AtividadeConfigurada(atividade.Id, item.TituloModelo))
					{
						Validacao.Add(Msg.TituloNaoConfiguradoAtividade(item.TituloModeloTexto, atividade.NomeAtividade, local));
						continue;
					}

					TituloModelo modelo = _tituloModeloBus.Obter(item.TituloModelo);

					if(modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.Titulo)
					{
						++titulos;
					}
					else
					{
						++titulosDeclaratorio;
						tituloDeclaratorioModelo = modelo.Nome;
					}

					if (titulos > 0 && titulosDeclaratorio > 0)
					{
						Validacao.Add(Msg.TituloDeclaratorioOutroRequerimento(tituloDeclaratorioModelo));
					}

					if (item.Id == 3)//Renovação
					{
						#region Renovacao

						if (!modelo.Regra(eRegra.Renovacao))
						{
							Validacao.Add(Msg.TituloNaoEhRenovacao(item.TituloModeloTexto, atividade.NomeAtividade, local));
						}

						if (modelo.Regra(eRegra.Renovacao))
						{
							if (item.EmitidoPorInterno)
							{
								if (item.TituloModeloAnteriorId == 0 || item.TituloModeloAnteriorId == null)
								{
									Validacao.Add(Msg.TituloAnteriorObrigatorio(item.TituloModeloTexto, atividade.NomeAtividade));
								}

								if (string.IsNullOrWhiteSpace(item.TituloAnteriorNumero))
								{
									Validacao.Add(Msg.NumeroAnteriorObrigatorio(item.TituloModeloTexto, atividade.NomeAtividade));
								}
								else
								{
									if (!(ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(item.TituloAnteriorNumero) ||
										ValidacoesGenericasBus.ValidarNumero(item.TituloAnteriorNumero, 12)))
									{
										Validacao.Add(Msg.TituloAnteriorNumeroInvalido);
									}
									else
									{
										if (ValidacoesGenericasBus.ValidarNumero(item.TituloAnteriorNumero, 12) &&
											_requerimentoDa.ValidarNumeroSemAnoExistente(item.TituloAnteriorNumero, item.TituloModeloAnteriorId.GetValueOrDefault()))
										{
											Validacao.Add(Msg.TituloNumeroSemAnoEncontrado);
										}
									}
								}

								CarregarTituloAnteriorSigla(item);
							}
							else
							{
								ValidarTituloNumeroOrgaoTexto(item, atividade);
							}
						}

						#endregion
					}
					else
					{
						#region Fase Anterior

						if (!modelo.Regra(eRegra.FaseAnterior) && (item.TituloModeloAnteriorId != null && item.TituloModeloAnteriorId != 0))
						{
							Validacao.Add(Msg.TituloNaoTemTituloAnterior(item.TituloModeloAnteriorTexto, item.TituloModeloTexto, local));
							continue;
						}

						Boolean adicionouTituloAnterior = false;
						Boolean validarTituloAnterior = Convert.ToBoolean((modelo.Resposta(eRegra.FaseAnterior, eResposta.TituloAnteriorObrigatorio) ?? new TituloModeloResposta()).Valor);

						if (item.EmitidoPorInterno)
						{
							adicionouTituloAnterior = (item.TituloAnteriorId.GetValueOrDefault(0) > 0) || (!String.IsNullOrWhiteSpace(item.TituloAnteriorNumero));
						}
						else
						{
							adicionouTituloAnterior = (!String.IsNullOrWhiteSpace(item.TituloModeloAnteriorTexto)) || (!String.IsNullOrWhiteSpace(item.TituloAnteriorNumero)) || (!String.IsNullOrWhiteSpace(item.OrgaoExpedidor));
						}

						validarTituloAnterior = validarTituloAnterior || adicionouTituloAnterior;

						if (modelo.Regra(eRegra.FaseAnterior) && validarTituloAnterior)
						{
							List<TituloModeloResposta> respostas = modelo.Respostas(eRegra.FaseAnterior, eResposta.Modelo);

							if (respostas == null || respostas.Count < 1)
							{
								Validacao.Add(Msg.TituloNaoEhFaseAnterior(item.TituloModeloTexto, local));
							}

							if (item.EmitidoPorInterno)
							{
								if (item.TituloModeloAnteriorId != null && item.TituloModeloAnteriorId != 0)
								{
									if (respostas.SingleOrDefault(x => x.Valor.ToString() == item.TituloModeloAnteriorId.ToString()) == null)
									{
										Validacao.Add(Msg.TituloNaoTemTituloAnterior(item.TituloModeloTexto, item.TituloModeloAnteriorTexto, local));
									}
								}

								if (string.IsNullOrWhiteSpace(item.TituloAnteriorNumero))
								{
									Validacao.Add(Msg.NumeroAnteriorObrigatorio(item.TituloModeloTexto, atividade.NomeAtividade));
								}
								else
								{
									if (!(ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(item.TituloAnteriorNumero) ||
										ValidacoesGenericasBus.ValidarNumero(item.TituloAnteriorNumero, 12)))
									{
										Validacao.Add(Msg.TituloAnteriorNumeroInvalido);
									}
									else
									{
										if (ValidacoesGenericasBus.ValidarNumero(item.TituloAnteriorNumero, 12) &&
											_requerimentoDa.ValidarNumeroSemAnoExistente(item.TituloAnteriorNumero, item.TituloModeloAnteriorId.GetValueOrDefault()))
										{
											Validacao.Add(Msg.TituloNumeroSemAnoEncontrado);
										}
									}
								}

								CarregarTituloAnteriorSigla(item);
							}
							else
							{
								ValidarTituloNumeroOrgaoTexto(item, atividade);
							}
						}

						#endregion
					}
					#endregion
				}

				#endregion
			}

			return Validacao.EhValido;
		}

		public void CarregarTituloAnteriorSigla(Finalidade item)
		{
			if (Validacao.EhValido && String.IsNullOrWhiteSpace(item.TituloModeloAnteriorSigla))
			{
				TituloModelo tituloModeloAnterior = _tituloModeloBus.ObterSimplificado(item.TituloModeloAnteriorId.Value);

				item.TituloModeloAnteriorSigla = tituloModeloAnterior.Sigla;
			}
		}

		public bool ResponsavelTecnicoValidar(List<ResponsavelTecnico> responsaveisTecnicos, List<Atividade> atividades)
		{
			Atividade atividade = _atividadeBus.ObterAtividadePorCodigo((int)eAtividadeCodigo.BarragemDeAte1HaLâminaDaguaAte10000M3DeVolumeArmazenado);

			if (atividades.Any(x => x.Id == atividade.Id) && (responsaveisTecnicos == null || responsaveisTecnicos.Count <= 0))
			{
				Validacao.Add(Mensagem.Requerimento.AtividadeResponsavelTecnicoObrigatorio(atividade.NomeAtividade));
				return false;
			}

			for (int i = 0; i < responsaveisTecnicos.Count; i++)
			{
				ResponsavelTecnico responsavel = responsaveisTecnicos[i];

				if (string.IsNullOrEmpty(responsavel.NomeRazao))
				{
					Validacao.Add(Msg.ResponsavelNomeRazaoObrigatorio(i));
				}

				if (string.IsNullOrEmpty(responsavel.CpfCnpj))
				{
					Validacao.Add(Msg.ResponsavelCpfCnpjObrigatorio(i));
				}

				if (responsavel.Funcao <= 0)
				{
					Validacao.Add(Msg.ResponsavelFuncaoObrigatorio(i));
				}

				if (string.IsNullOrEmpty(responsavel.NumeroArt))
				{
					Validacao.Add(Msg.ResponsavelARTObrigatorio(i));
				}

				if (!_busPessoa.ExisteProfissao(responsavel.Id))
				{
					Validacao.Add(Mensagem.ResponsavelTecnico.ResponsavelNaoPossuiProfissao(responsavel.NomeRazao));
				}
			}

			return Validacao.EhValido;
		}

		public void ValidarTituloNumeroOrgaoTexto(Finalidade item, Atividade atividade)
		{
			if (string.IsNullOrWhiteSpace(item.TituloModeloAnteriorTexto))
			{
				Validacao.Add(Msg.TituloAnteriorObrigatorio(item.TituloModeloTexto, atividade.NomeAtividade));
			}

			if (string.IsNullOrWhiteSpace(item.TituloAnteriorNumero))
			{
				Validacao.Add(Msg.NumeroAnteriorObrigatorio(item.TituloModeloTexto, atividade.NomeAtividade));
			}
			else
			{
				if (!(ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(item.TituloAnteriorNumero) || ValidacoesGenericasBus.ValidarNumero(item.TituloAnteriorNumero, 12)))
				{
					Validacao.Add(Mensagem.Requerimento.TituloAnteriorNumeroInvalido);
				}
			}

			if (string.IsNullOrWhiteSpace(item.OrgaoExpedidor))
			{
				Validacao.Add(Msg.OrgaoExpedidorObrigatorio);
			}
		}

		public bool ValidarModeloAnteriorNumero(int tituloAnteriorId, int tituloAnteriorTipo)
		{
			if (_requerimentoDa.ValidarModeloAnteriorNumero(tituloAnteriorId, tituloAnteriorTipo))
			{
				Validacao.Add(Msg.TituloAnteriorUtilizado(""));
			}
			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			string numeroProtocolo = _requerimentoDa.AssociadoProcesso(id);

			if (numeroProtocolo != string.Empty)
			{
				Validacao.Add(Mensagem.Requerimento.ExcluirRequerimentoProcesso(numeroProtocolo));
			}

			numeroProtocolo = _requerimentoDa.AssociadoDocumento(id);

			if (numeroProtocolo != string.Empty)
			{
				Validacao.Add(Mensagem.Requerimento.ExcluirRequerimentoDocumento(numeroProtocolo));
			}

			string situacao = _requerimentoDa.AssociadoTitulo(id);

			if (situacao != string.Empty)
			{
				Validacao.Add(Mensagem.Requerimento.ExcluirRequerimentoTitulo(situacao));
			}

			return Validacao.EhValido;
		}

		internal bool ObterNumerosTitulos(string numero, int modeloId)
		{
			if (modeloId < 1)
			{
				Validacao.Add(Mensagem.Requerimento.TituloAnteriorObrigatorioModal);
			}

			if (string.IsNullOrWhiteSpace(numero))
			{
				Validacao.Add(Mensagem.Requerimento.NumeroAnteriorObrigatorioModal);
			}

			if (!(ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero) || ValidacoesGenericasBus.ValidarNumero(numero, 12)))
			{
				Validacao.Add(Mensagem.Requerimento.TituloAnteriorNumeroInvalido);
			}

			return Validacao.EhValido;
		}

		public void RoteirosChecagemRequerimento(int checagemId, int requerimentoId, int requerimentoSituacao)
		{
			ChecagemRoteiro checagem = _checkListRoteiroBus.Obter(checagemId);

			RequerimentoBus bus = new RequerimentoBus();
			List<Roteiro> roteirosAtuais = bus.ObterRequerimentoRoteiros(requerimentoId, requerimentoSituacao);

			Roteiro roteiro = null;
			checagem.Roteiros.ForEach(atual =>
			{
				roteiro = roteirosAtuais.FirstOrDefault(rot => rot.Tid == atual.Tid);
				if (roteiro == null)
				{
					Validacao.Add(Mensagem.Processo.ChecagemRoteirosDifentesRoteiroAtuais);
					return;
				}
			});
		}

		internal bool Existe(int requerimentoId)
		{
			if (!_requerimentoDa.Existe(requerimentoId))
			{
				Validacao.Add(Mensagem.Requerimento.Inexistente);
			}
			return Validacao.EhValido;
		}

		internal bool Finalizar(Requerimento requerimento)
		{
			ObjetivoPedidoValidar(requerimento);

			InteressadoValidar(requerimento);

			ResponsavelTecnicoValidar(requerimento.Responsaveis, requerimento.Atividades);

			PodeEditar(requerimento);

			if (requerimento.Atividades.Any(x => x.Id == 209)) /* Informação de Corte */
				ValidacoesEmpreendimentoAtividadeCorte(requerimento);

			return Validacao.EhValido;
		}

		public void ValidacoesEmpreendimentoAtividadeCorte(Requerimento requerimento)
		{

			//if (!_busEmpreendimento.EmpreendimentoPossuiCodigoSicar(requerimento.Empreendimento.Codigo ?? 0))
			//	Validacao.Add(Msg.EmpreendimentoNaoIntegradoAoSicar);

			if (!_busEmpreendimento.EmpreendimentoAssociadoResponsavel(requerimento.Interessado.Id, requerimento.Empreendimento.Id))
				Validacao.Add(Msg.EmpreendimentoNaoAssociadoAoResponsavel);
		}

		public bool RequerimentoDeclaratorio(int requerimentoId)
		{
			return _requerimentoDa.RequerimentoDeclaratorio(requerimentoId);
		}

		private void PodeEditar(Requerimento requerimento)
		{
			if (Validacao.EhValido)
			{
				string situacao = _requerimentoDa.PossuiTituloDeclaratorio(requerimento.Id);
				if (!string.IsNullOrWhiteSpace(situacao))
				{
					Validacao.Add(Mensagem.Requerimento.PossuiTituloDeclaratorio(situacao));
				}
			}
		}
	}
}