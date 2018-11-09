using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Model.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business
{
	public class RequerimentoCredenciadoValidar
	{
		#region Propriedades

		RequerimentoMsg Msg = new RequerimentoMsg();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		RequerimentoCredenciadoDa _da;

		PessoaCredenciadoBus _busPessoa;
		AtividadeConfiguracaoInternoBus _atividadeConfigurada;
		RoteiroInternoBus _roteiroBus;
		AtividadeInternoBus _atividadeBus;
		TituloModeloInternoBus _tituloModeloBus;
		ChecagemRoteiroInternoBus _checkListRoteiroBus;
		EmpreendimentoCredenciadoBus _busEmpreendimento;

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		#endregion

		public RequerimentoCredenciadoValidar()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new RequerimentoCredenciadoDa();

			_busPessoa = new PessoaCredenciadoBus();
			_atividadeConfigurada = new AtividadeConfiguracaoInternoBus();
			_roteiroBus = new RoteiroInternoBus();
			_atividadeBus = new AtividadeInternoBus();
			_tituloModeloBus = new TituloModeloInternoBus();
			_checkListRoteiroBus = new ChecagemRoteiroInternoBus();
			_busEmpreendimento = new EmpreendimentoCredenciadoBus();
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

			if (!_busEmpreendimento.ExisteEmpreendimentoResponsavel(requerimento.Interessado.Id)) 
				Validacao.Add(Msg.NaoExisteEmpreendimentoAssociadoResponsavel);

			return Validacao.EhValido;
		}

		public bool ObjetivoPedidoValidar(Requerimento requerimento)
		{
			RequerimentoCredenciadoBus _bus = new RequerimentoCredenciadoBus();

			if (requerimento.DataCadastro.ToString() == string.Empty)
			{
				Validacao.Add(Msg.DataCriacaobrigatorio);
			}

			if (requerimento.AgendamentoVistoria <= 0)
			{
				Validacao.Add(Msg.AgendamentObriagatorio);
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

			if(requerimento.Atividades.Count() > 1 && requerimento.Atividades.Any(item => item.Id == 209))/*Informação de Corte*/
				Validacao.Add(Msg.AtividadeInformacaoCorte);

			ValidarAtividade(requerimento.Atividades);

			return Validacao.EhValido;
		}

		public bool ValidarAtividade(List<Atividade> atividades, string local = null)
		{
			if (atividades.Count <= 0)
			{
				Validacao.Add(Msg.AtividadeObrigatorio);
				return Validacao.EhValido;
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

					if (modelo.TipoDocumentoEnum == eTituloModeloTipoDocumento.Titulo)
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
									if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(item.TituloAnteriorNumero))
									{
										Validacao.Add(Msg.TituloAnteriorNumeroInvalido);
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
											_da.ValidarNumeroSemAnoExistente(item.TituloAnteriorNumero, item.TituloModeloAnteriorId.GetValueOrDefault()))
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

		public bool AssociarEmpreendimento(Requerimento requerimento)
		{
			if (requerimento.Empreendimento.Id > 0 && requerimento.CredenciadoId != requerimento.Empreendimento.CredenciadoId)
			{
				Validacao.Add(Msg.EmpreendimentoAssociadoInvalido);
				return Validacao.EhValido;
			}

			List<String> atividades = _da.ObterAtividadesEmpreendimentoObrigatorio(requerimento.Id);

			if (atividades != null && atividades.Count > 0)
			{
				if (requerimento.Empreendimento.Id <= 0)
				{
					Validacao.Add(Msg.EmpreendimentoObrigatorioPorAtividade(Mensagem.Concatenar(atividades).ToLower()));
				}
			}

			return Validacao.EhValido;
		}

		public bool Finalizar(Requerimento requerimento)
		{
			ObjetivoPedidoValidar(requerimento);

			InteressadoValidar(requerimento);

			ResponsavelTecnicoValidar(requerimento.Responsaveis, requerimento.Atividades);

			AssociarEmpreendimento(requerimento);

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
					Validacao.Add(Msg.TituloAnteriorNumeroInvalido);
				}
			}

			if (string.IsNullOrWhiteSpace(item.OrgaoExpedidor))
			{
				Validacao.Add(Msg.OrgaoExpedidorObrigatorio);
			}
		}

		public bool ValidarModeloAnteriorNumero(int tituloAnteriorId, int tituloAnteriorTipo)
		{
			if (_da.ValidarModeloAnteriorNumero(tituloAnteriorId, tituloAnteriorTipo))
			{
				Validacao.Add(Msg.TituloAnteriorUtilizado(""));
			}
			return Validacao.EhValido;
		}

		public bool ValidarNumero(string numero)
		{
			if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
			{
				Validacao.Add(Msg.TituloAnteriorNumeroInvalido);
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			Requerimento req = _da.Obter(id, simplificado: true);

			if (req != null && req.SituacaoId != (int)eRequerimentoSituacao.EmAndamento)
			{
				Validacao.Add(Mensagem.Requerimento.Excluir(id));
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

			RequerimentoCredenciadoBus bus = new RequerimentoCredenciadoBus();
			List<Roteiro> roteirosAtuais = bus.ObterRequerimentoRoteiros(requerimentoId, requerimentoSituacao);

			Roteiro roteiro = null;
			checagem.Roteiros.ForEach(atual =>
			{
				roteiro = roteirosAtuais.SingleOrDefault(rot => rot.Tid == atual.Tid);
				if (roteiro == null)
				{
					Validacao.Add(Mensagem.Processo.ChecagemRoteirosDifentesRoteiroAtuais);
					return;
				}
			});
		}

		public bool Posse(int id)
		{
			Requerimento requerimento = _da.Obter(id, simplificado: true);

			if (requerimento.CredenciadoId != User.EtramiteIdentity.FuncionarioId)
			{
				Validacao.Add(Mensagem.Requerimento.PosseCredenciado);
			}

			return Validacao.EhValido;
		}

		public bool PosseInterno(int id)
		{
			//CredenciadoBus credenciadoBus = new CredenciadoBus();
			//CredenciadoPessoa credenciado = credenciadoBus.Obter(User.EtramiteIdentity.FuncionarioId, true);
			//int pessoa = credenciado.Pessoa.InternoId.GetValueOrDefault();

			//if (true)
			//{
			//	Validacao.Add(Mensagem.Requerimento.PosseCredenciado);
			//}

			return Validacao.EhValido;
		}
	}
}