using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloEmail;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloEmail.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;
using CARSolicitacaoCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business.CARSolicitacaoBus;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloSituacaoBus
	{
		#region Propriedades

		TituloSituacaoValidar _validar = null;
		ListaBus _busLista = new ListaBus();
		TituloBus _busTitulo = new TituloBus();
		TituloDa _da = new TituloDa();
		AnaliseItensBus _busAnalise = new AnaliseItensBus();
		TituloModeloBus _busModelo = new TituloModeloBus(new TituloModeloValidacao());
		CondicionanteDa _daCondicionante = new CondicionanteDa();
		CARSolicitacaoBus _busCARSolicitacao = new CARSolicitacaoBus();
		ExploracaoFlorestalBus _busExploracao = new ExploracaoFlorestalBus();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());

		public List<int> LstModeloCodigoPendencia
		{
			get { return _configModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia); }
		}

		public List<int> LstModeloCodigoIndeferido
		{
			get { return _configModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoIndeferido); }
		}

		public List<int> LstCadastroAmbientalRuralTituloCodigo
		{
			get { return _configModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyCadastroAmbientalRuralTituloCodigo); }
		}

		public EtramitePrincipal User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal);
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		#endregion

		public TituloSituacaoBus() { _validar = new TituloSituacaoValidar(); }

		public TituloSituacaoBus(TituloSituacaoValidar validar)
		{
			_validar = validar;
		}

		public void AlterarSituacao(Titulo titulo, int acao, bool gerouPdf = true)
		{
			try
			{
				AlterarSituacao(titulo, acao, null, gerouPdf);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarSituacao(Titulo titulo, int acao, BancoDeDados banco, bool gerouPdf = true)
		{
			Titulo atualTitulo = _da.ObterSimplificado(titulo.Id);

			if (titulo.Modelo.Regras == null || titulo.Modelo.Regras.Count == 0)
			{
				titulo.Modelo = _busModelo.Obter(titulo.Modelo.Id);
			}

			titulo.Situacao = ObterNovaSituacao(titulo, acao);

			bool isLimparPdfENumero = false;
			bool isGerarNumero = false;
			bool isGerarPdf = false;
			int novaSituacaoCondicionante = 0;

			if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.CertificadoRegistroAtividadeFlorestal)
			{
				if (titulo.Atividades.Any(x => x.Codigo == (int)eAtividadeCodigo.FabricanteMotosserra) || titulo.Atividades.Any(x => x.Codigo == (int)eAtividadeCodigo.ComercianteMotosserra))
				{
					titulo.Prazo = 365;
				}
				else
				{
					titulo.Prazo = new DateTime(DateTime.Now.Year, 12, 31).DayOfYear - titulo.DataEmissao.Data.Value.DayOfYear;
				}
			}

			if (!_validar.AlterarSituacao(titulo, acao, gerouPdf))
			{
				return;
			}

			#region Configurar Nova Situacao

			//Situação Nova
			switch ((eTituloSituacao)titulo.Situacao.Id)
			{
				#region 1 - Cadastrado

				case eTituloSituacao.Cadastrado:

					if (atualTitulo.Situacao.Id == (int)eTituloSituacao.Emitido)
					{
						if (titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
						{
							titulo.Numero.Inteiro = null;
							titulo.Numero.Ano = null;
						}
					}

					titulo.DataEmissao.Data = null;
					titulo.DataInicioPrazo.Data = null;
					titulo.DataAssinatura.Data = null;
					titulo.DataVencimento.Data = null;
					titulo.DataEncerramento.Data = null;
					titulo.Prazo = null;

					isLimparPdfENumero = true;
					break;

				#endregion

				#region 2 - Emitido

				case eTituloSituacao.Emitido:
					isGerarNumero = true;
					isGerarPdf = true;
					//titulo.DataEmissao info na tela

					if (titulo.Modelo.Regra(eRegra.Prazo))
					{
						int prazoId = Convert.ToInt32(titulo.Modelo.Resposta(eRegra.Prazo, eResposta.TipoPrazo).Valor);
						titulo.PrazoUnidade = _busLista.TituloModeloTiposPrazos.Single(x => x.Id == prazoId).Texto;
					}
					break;

				#endregion

				#region 3 - Concluído

				case eTituloSituacao.Concluido:
					if (titulo.Modelo.Regra(eRegra.Condicionantes))
					{
						novaSituacaoCondicionante = 2;
					}

					if (atualTitulo.Situacao.Id == (int)eTituloSituacao.Cadastrado)
					{
						isGerarNumero = true;
						isGerarPdf = true;
					}
					//titulo.DataAssinatura info na tela

					if (titulo.Modelo.Regra(eRegra.Prazo))
					{
						int prazoId = Convert.ToInt32(titulo.Modelo.Resposta(eRegra.Prazo, eResposta.TipoPrazo).Valor);

						titulo.PrazoUnidade = _busLista.TituloModeloTiposPrazos.Single(x => x.Id == prazoId).Texto;

						switch ((eAlterarSituacaoAcao)acao)
						{
							case eAlterarSituacaoAcao.EmitirParaAssinatura:
								titulo.DataInicioPrazo.Data = titulo.DataEmissao.Data.GetValueOrDefault();
								break;

							case eAlterarSituacaoAcao.Assinar:
								titulo.DataInicioPrazo.Data = titulo.DataAssinatura.Data.GetValueOrDefault();
								break;

							case eAlterarSituacaoAcao.Entregar:
							case eAlterarSituacaoAcao.Concluir:
								if (titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor.ToString() == "2")//data da assinatura
								{
									titulo.DataInicioPrazo.Data = titulo.DataAssinatura.Data.GetValueOrDefault();
								}
								else
								{
									titulo.DataInicioPrazo.Data = titulo.DataEmissao.Data.GetValueOrDefault();
								}
								break;

							default:
								titulo.DataInicioPrazo.Data = DateTime.Now;
								break;
						}

						if (prazoId == 1)//Dias
						{
							titulo.DataVencimento.Data = titulo.DataInicioPrazo.Data.Value.AddDays(titulo.Prazo.GetValueOrDefault());
						}
						else
						{
							titulo.DataVencimento.Data = titulo.DataInicioPrazo.Data.Value.AddYears(titulo.Prazo.GetValueOrDefault());
						}
					}
					break;

				#endregion

				#region 4 - Assinado

				case eTituloSituacao.Assinado:
					//titulo.DataAssinatura info na tela
					break;

				#endregion

				#region 5 - Encerrado

				case eTituloSituacao.Encerrado:
					if (titulo.Modelo.Regra(eRegra.Condicionantes))
					{
						novaSituacaoCondicionante = 5;
					}
					break;

				#endregion

				#region 6 - Prorrogado

				case eTituloSituacao.Prorrogado:
					if (titulo.Modelo.Regra(eRegra.Prazo))
					{
						titulo.DataVencimento.Data = titulo.DataVencimento.Data.Value.AddDays(titulo.DiasProrrogados.GetValueOrDefault());
					}
					break;

				#endregion

				#region 11 - Suspenso

				case eTituloSituacao.Suspenso:
					if (titulo.Modelo.Regra(eRegra.Condicionantes))
					{
						novaSituacaoCondicionante = 11;
					}
					break;

					#endregion
			}

			#endregion

			#region Numero de Titulo

			if (isGerarNumero)
			{
				titulo.Numero.ReiniciaPorAno = titulo.Modelo.Regra(eRegra.NumeracaoReiniciada);

				if (titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
				{
					titulo.Numero.Automatico = true;
					TituloModeloResposta iniciarEm = titulo.Modelo.Resposta(eRegra.NumeracaoAutomatica, eResposta.InicioNumeracao);
					titulo.Numero.IniciaEm = null;
					titulo.Numero.IniciaEmAno = null;

					if (iniciarEm != null)
					{
						if (iniciarEm.Valor == null || !ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(iniciarEm.Valor.ToString()))
						{
							Validacao.Add(Mensagem.Titulo.IniciarEmInvalido);
							return;
						}

						string[] iniciar = iniciarEm.Valor.ToString().Split('/');
						titulo.Numero.IniciaEm = Convert.ToInt32(iniciar[0]);
						titulo.Numero.IniciaEmAno = Convert.ToInt32(iniciar[1]);

						if (titulo.Numero.IniciaEmAno.GetValueOrDefault() != DateTime.Now.Year)
						{
							titulo.Numero.IniciaEm = null;
							titulo.Numero.IniciaEmAno = null;
						}
					}
				}

				if (titulo.DataEmissao.IsValido)
				{
					titulo.Numero.Ano = titulo.DataEmissao.Data.Value.Year;
				}
			}

			//Limpar numero 
			if (isLimparPdfENumero)
			{
				if (titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
				{
					titulo.Numero = null;
				}
			}

			#endregion

			if (isLimparPdfENumero)
			{
				titulo.ArquivoPdf.Id = null;
			}

			GerenciadorTransacao.ObterIDAtual();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				#region Condicionante

				if (novaSituacaoCondicionante > 0)
				{
					foreach (var item in titulo.Condicionantes)
					{
						TituloCondicionante condicionanteAtual = _daCondicionante.Obter(item.Id, bancoDeDados);

						//Filtro para não alterar as 4 - atendida
						if (novaSituacaoCondicionante == 5 && condicionanteAtual.Situacao.Id == 4)//5 - encerrada | 4 - atendida
						{
							continue;
						}

						//Ativa a condicionante
						if (novaSituacaoCondicionante == 2 && condicionanteAtual.Situacao.Id != 2)
						{
							if (condicionanteAtual.PossuiPrazo)
							{
								//Regras do Modelo Sem Prazo em Condicionantes com Prazo
								if (!titulo.Modelo.Regra(eRegra.Prazo))
								{
									condicionanteAtual.DataInicioPrazo.Data = titulo.DataEmissao.Data;
									condicionanteAtual.DataVencimento.Data = titulo.DataEmissao.Data.Value.AddDays(condicionanteAtual.Prazo.GetValueOrDefault());
								}
								else
								{
									condicionanteAtual.DataInicioPrazo = titulo.DataInicioPrazo;
									condicionanteAtual.DataVencimento.Data = titulo.DataInicioPrazo.Data.Value.AddDays(condicionanteAtual.Prazo.GetValueOrDefault());
								}
							}

							//Periodicidade
							if (!item.PossuiPeriodicidade && item.Periodicidades != null)
							{
								item.Periodicidades.Clear();
							}

							TituloCondicionantePeriodicidade periodicidade = null;

							if (item.PossuiPeriodicidade)
							{
								int diasTotais = item.PeriodicidadeValor.GetValueOrDefault();

								switch (item.PeriodicidadeTipo.Id)
								{
									case 1://dia
										break;

									case 2://Mes
										diasTotais *= 30;
										break;

									case 3://Ano
										diasTotais *= 365;
										break;

									default:
										break;
								}

								if (item.Prazo.GetValueOrDefault() > 0 && item.Prazo.GetValueOrDefault() <= diasTotais)
								{
									int qtdPeriodo = Math.Abs((diasTotais / (item.Prazo.GetValueOrDefault())));
									for (int i = 0; i < qtdPeriodo; i++)
									{
										periodicidade = new TituloCondicionantePeriodicidade();
										periodicidade.Situacao.Id = 2;//Ativa

										//Regras do Modelo Sem Prazo em Condicionantes com Prazo
										if (!titulo.Modelo.Regra(eRegra.Prazo))
										{
											periodicidade.DataInicioPrazo.Data = titulo.DataEmissao.Data.Value.AddDays(i * item.Prazo.GetValueOrDefault());
											periodicidade.DataVencimento.Data = titulo.DataEmissao.Data.Value.AddDays((i + 1) * item.Prazo.GetValueOrDefault());
										}
										else
										{
											periodicidade.DataInicioPrazo.Data = titulo.DataInicioPrazo.Data.Value.AddDays(i * item.Prazo.GetValueOrDefault());
											periodicidade.DataVencimento.Data = titulo.DataInicioPrazo.Data.Value.AddDays((i + 1) * item.Prazo.GetValueOrDefault());
										}

										item.Periodicidades.Add(periodicidade);
									}
								}
							}

							condicionanteAtual.Periodicidades = item.Periodicidades;
							condicionanteAtual.Situacao.Id = novaSituacaoCondicionante;
							_daCondicionante.Ativar(condicionanteAtual, bancoDeDados);
						}
						else
						{
							condicionanteAtual.Situacao.Id = novaSituacaoCondicionante;
							_daCondicionante.AlterarSituacao(condicionanteAtual, banco: bancoDeDados);
						}
					}
				}

				#endregion

				_da.AlterarSituacao(titulo, bancoDeDados);

				#region Atividades

				TituloBus tituloBus = new TituloBus();
				AtividadeBus atividadeBus = new AtividadeBus();
				List<Atividade> lstTituloAtividades = null;

				#region Título Concluido

				if (titulo.Situacao.Id == (int)eTituloSituacao.Concluido)//3 - Concluido 
				{
					lstTituloAtividades = tituloBus.ObterAtividades(titulo.Id);

					if (lstTituloAtividades != null && lstTituloAtividades.Count > 0)
					{
						int? situacao = null;
						if (EspecificiadadeBusFactory.Possui(titulo.Modelo.Codigo.GetValueOrDefault()))
						{
							IEspecificidadeBus busEsp = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.GetValueOrDefault());
							situacao = busEsp.ObterSituacaoAtividade(titulo.Id);
						}

						if (situacao != null
							|| LstModeloCodigoPendencia.Any(x => x == titulo.Modelo.Codigo.GetValueOrDefault())
							|| LstModeloCodigoIndeferido.Any(x => x == titulo.Modelo.Codigo.GetValueOrDefault()))
						{
							eAtividadeSituacao atividadeSituacao =
								(LstModeloCodigoPendencia.Any(x => x == titulo.Modelo.Codigo.GetValueOrDefault())) ? eAtividadeSituacao.ComPendencia : eAtividadeSituacao.Indeferida;

							if (situacao != null)
							{
								atividadeSituacao = (eAtividadeSituacao)situacao;
							}

							atividadeBus.AlterarSituacao(lstTituloAtividades, atividadeSituacao, bancoDeDados);
						}
						else
						{
							foreach (Atividade atividade in lstTituloAtividades)
							{
								if (atividadeBus.VerificarDeferir(atividade, bancoDeDados))
								{
									atividade.SituacaoId = (int)eAtividadeSituacao.Deferida;
									atividadeBus.AlterarSituacao(atividade, bancoDeDados);
								}
							}
						}
					}
				}

				#endregion

				#region Título Encerrado

				if (titulo.Situacao.Id == (int)eTituloSituacao.Encerrado)//Encerrado
				{
					lstTituloAtividades = tituloBus.ObterAtividades(titulo.Id);
					eAtividadeSituacao situacaoAtual = eAtividadeSituacao.EmAndamento;

					if (lstTituloAtividades != null && lstTituloAtividades.Count > 0)
					{
						foreach (Atividade atividade in lstTituloAtividades)
						{
							situacaoAtual = (eAtividadeSituacao)atividadeBus.ObterAtividadeSituacao(atividade, bancoDeDados).SituacaoId;

							if (situacaoAtual == eAtividadeSituacao.Indeferida || situacaoAtual == eAtividadeSituacao.ComPendencia)
							{
								atividade.SituacaoId = (int)eAtividadeSituacao.EmAndamento;
								atividadeBus.AlterarSituacao(atividade, bancoDeDados);
							}
							else
							{
								if (titulo.MotivoEncerramentoId == 1 || titulo.MotivoEncerramentoId == 4)
								{
									atividade.SituacaoId = (int)eAtividadeSituacao.EmAndamento;
									atividadeBus.AlterarSituacao(atividade, bancoDeDados);
								}
								else if (atividadeBus.VerificarEncerrar(atividade, bancoDeDados))
								{
									atividade.SituacaoId = (int)eAtividadeSituacao.Encerrada;
									atividadeBus.AlterarSituacao(atividade, bancoDeDados);
								}
							}
						}
					}
				}

				#endregion

				#region Título Prorrogado

				if (titulo.Situacao.Id == (int)eTituloSituacao.Prorrogado)//6 - Prorrogar
				{
					lstTituloAtividades = tituloBus.ObterAtividades(titulo.Id);

					if (lstTituloAtividades != null && lstTituloAtividades.Count > 0)
					{
						if (LstModeloCodigoPendencia.Any(x => x == titulo.Modelo.Codigo) ||
							LstModeloCodigoIndeferido.Any(x => x == titulo.Modelo.Codigo))
						{
							eAtividadeSituacao atividadeSituacao =
								(LstModeloCodigoPendencia.Any(x => x == titulo.Modelo.Codigo)) ? eAtividadeSituacao.ComPendencia : eAtividadeSituacao.Indeferida;

							atividadeBus.AlterarSituacao(lstTituloAtividades, atividadeSituacao, bancoDeDados);
						}
						else
						{
							int finalidade = _da.VerificarEhTituloAnterior(titulo);
							if (finalidade > 0)
							{
								eAtividadeSituacao atividadeSituacao = (finalidade == 1) ? eAtividadeSituacao.NovaFase : eAtividadeSituacao.EmRenovacao;
								atividadeBus.AlterarSituacao(lstTituloAtividades, atividadeSituacao, bancoDeDados);
							}
							else
							{
								foreach (Atividade atividade in lstTituloAtividades)
								{
									if (atividadeBus.VerificarDeferir(atividade, bancoDeDados))
									{
										atividade.SituacaoId = (int)eAtividadeSituacao.Deferida;
										atividadeBus.AlterarSituacao(atividade, bancoDeDados);
										continue;
									}

									//Voltar a situação default de andamento
									atividade.SituacaoId = (int)eAtividadeSituacao.EmAndamento;
									atividadeBus.AlterarSituacao(atividade, bancoDeDados);
								}
							}
						}
					}
				}

				#endregion

				#endregion

				#region Gerar Pdf de Titulo

				ArquivoBus arqBus = new ArquivoBus(eExecutorTipo.Interno);

				if (isGerarPdf && titulo.Modelo.Regra(eRegra.PdfGeradoSistema))
				{
					TituloBus bus = new TituloBus();

					titulo.ArquivoPdf.Nome = "Titulo.pdf";
					titulo.ArquivoPdf.Extensao = ".pdf";
					titulo.ArquivoPdf.ContentType = "application/pdf";
					titulo.ArquivoPdf.Buffer = bus.GerarPdf(titulo, bancoDeDados);

					if (titulo.ArquivoPdf.Buffer != null)
					{
						arqBus.Salvar(titulo.ArquivoPdf);

						ArquivoDa _arquivoDa = new ArquivoDa();
						_arquivoDa.Salvar(titulo.ArquivoPdf, User.EtramiteIdentity.FuncionarioId, User.EtramiteIdentity.Name,
							User.EtramiteIdentity.Login, (int)eExecutorTipo.Interno, User.EtramiteIdentity.FuncionarioTid, bancoDeDados);

						_da.SalvarPdfTitulo(titulo, bancoDeDados);
					}
				}

				#endregion

				#region Análise de Item

				//Trava a Análise de item de processo/documento caso o titulo que está sendo alterado seja um de pendência
				if (titulo.Atividades != null && titulo.Atividades.Count > 0)
				{
					Protocolo protocolo = titulo.Atividades.First().Protocolo;
					AnaliseItem analise = _busAnalise.Obter(protocolo);

					if (analise != null && analise.Id > 0)
					{
						if (_busTitulo.ExisteTituloPendencia(protocolo, bancoDeDados))
						{
							if (analise.Situacao == 1)// 1 - Em andamento
							{
								analise.Situacao = 2;// 2 - Com Pendência
								_busAnalise.AlterarSituacao(analise, bancoDeDados);
							}
						}
						else if (analise.Situacao == 2)
						{
							analise.Situacao = 1;
							_busAnalise.AlterarSituacao(analise, bancoDeDados);
						}
					}
				}

				#endregion

				#region Gerar/Enviar Email

				#region Gerar Email

				Email email = null;
				if (titulo.Situacao.Id == (int)eTituloSituacao.Concluido && titulo.Modelo.Regra(eRegra.EnviarEmail))//3 - Concluido 
				{
					if (titulo.Modelo.Resposta(eRegra.EnviarEmail, eResposta.TextoEmail).Valor != null)
					{
						string textoEmail = titulo.Modelo.Resposta(eRegra.EnviarEmail, eResposta.TextoEmail).Valor.ToString();

						if (!String.IsNullOrWhiteSpace(textoEmail))
						{
							Dictionary<String, String> emailKeys = new Dictionary<string, string>();

							emailKeys.Add("[orgão sigla]", _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla));
							emailKeys.Add("[data da conclusão]", titulo.Modelo.Regra(eRegra.Prazo) ? titulo.DataInicioPrazo.DataTexto : titulo.DataEmissao.DataTexto);
							emailKeys.Add("[nome do modelo]", titulo.Modelo.Nome);
							emailKeys.Add("[nome do subtipo]", titulo.Modelo.SubTipo);
							emailKeys.Add("[nº do título]", titulo.Numero.Texto);
							emailKeys.Add("[nº processo/documento do título]", titulo.Protocolo.Numero);
							emailKeys.Add("[nome do empreendimento]", titulo.EmpreendimentoTexto);

							foreach (string key in emailKeys.Keys)
							{
								textoEmail = textoEmail.Replace(key, emailKeys[key]);
							}

							email = new Email();
							email.Assunto = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla);
							email.Texto = textoEmail;
							email.Tipo = eEmailTipo.TituloConcluir;
							email.Codigo = titulo.Id;
						}
					}
				}

				#endregion

				if (email != null)
				{
					List<String> lstEmail = _da.ObterEmails(titulo.Id, bancoDeDados);

					if (lstEmail != null && lstEmail.Count > 0)
					{
						email.Destinatario = String.Join(", ", lstEmail.ToArray());

						if (titulo.Modelo.Regra(eRegra.AnexarPDFTitulo))
						{
							email.Anexos.Add(titulo.ArquivoPdf);
						}

						EmailBus emailBus = new EmailBus();
						emailBus.Enviar(email, bancoDeDados);
					}
				}

				#endregion

				#region [ Solicitação CAR ]

				if (LstCadastroAmbientalRuralTituloCodigo.Any(x => x == titulo.Modelo.Codigo))
				{
					if (titulo.Situacao.Id == (int)eTituloSituacao.Concluido)
					{
						//Interno
						List<int> situacoes = new List<int>() { (int)eCARSolicitacaoSituacao.Valido };
						CARSolicitacao solicitacao = _busCARSolicitacao.ObterPorEmpreendimento(titulo.EmpreendimentoId.GetValueOrDefault(0), situacoes, false, bancoDeDados) ?? new CARSolicitacao();

						if (solicitacao != null && solicitacao.Id > 0)
						{
							solicitacao.SituacaoId = (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR;
							solicitacao.DataSituacao.Data = DateTime.Now;

							_busCARSolicitacao.AlterarSituacao(solicitacao, bancoDeDados, mostrarMsg: false);
						}
						else
						{
							//Credenciado
							_busCARSolicitacao.SubstituirPorTituloCARCredenciado(titulo.EmpreendimentoId.GetValueOrDefault(0), bancoDeDados);
						}
					}

					if (titulo.Situacao.Id == (int)eTituloSituacao.Encerrado)
					{
						List<int> situacoes = new List<int>() { (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR };
						CARSolicitacao solicitacao = _busCARSolicitacao.ObterPorEmpreendimento(titulo.EmpreendimentoId.GetValueOrDefault(0), situacoes, false, bancoDeDados) ?? new CARSolicitacao();

						if (solicitacao != null && solicitacao.Id > 0)
						{
							solicitacao.SituacaoAnteriorId = solicitacao.SituacaoId;
							solicitacao.DataSituacaoAnterior = solicitacao.DataSituacao;

							solicitacao.SituacaoId = (int)eCARSolicitacaoSituacao.Valido;
							solicitacao.DataSituacao.Data = DateTime.Now;

							_busCARSolicitacao.AlterarSituacao(solicitacao, bancoDeDados, mostrarMsg: false);
						}
						else
						{
							//Credenciado
							Empreendimento empreendimento = new EmpreendimentoBus().ObterSimplificado(titulo.EmpreendimentoId.GetValueOrDefault(0));

							CARSolicitacaoCredenciadoBus carSolicitacaoCredBus = new CARSolicitacaoCredenciadoBus();
							CARSolicitacao carSolicitacaoCred = new CARSolicitacao();
							carSolicitacaoCred.Empreendimento.Codigo = empreendimento.Codigo;
							carSolicitacaoCredBus.AlterarSituacao___QueFazVirarPassivo(carSolicitacaoCred, new CARSolicitacao() { SituacaoId = (int)eCARSolicitacaoSituacao.Valido });
						}
					}

					//SE situacao == Encerrado
					//TODO:Mudar Situação para Válido
				}

				#endregion

				#region Explorações
				if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal)
				{
					if (titulo.Situacao.Id == (int)eTituloSituacao.Concluido)
						_busExploracao.FinalizarExploracao(titulo.EmpreendimentoId.GetValueOrDefault(0), titulo.Id, banco);
				}

				#endregion Explorações

				if (!Validacao.EhValido)
				{
					bancoDeDados.Rollback();
					return;
				}

				#region Salvar A especificidade

				if (EspecificiadadeBusFactory.Possui(titulo.Modelo.Codigo.GetValueOrDefault()))
				{
					IEspecificidadeBus busEsp = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.GetValueOrDefault());
					titulo.Especificidade = busEsp.Obter(titulo.Id) as Especificidade;
					titulo.Especificidade = titulo.ToEspecificidade();
					busEsp.Salvar(titulo.Especificidade, bancoDeDados);

					List<DependenciaLst> lstDependencias = busEsp.ObterDependencias(titulo.Especificidade);
					if (isGerarPdf && lstDependencias != null && lstDependencias.Count > 0)
					{
						if (!lstDependencias.Exists(x => x.TipoId == (int)eTituloDependenciaTipo.Caracterizacao && x.DependenciaTipo == (int)eCaracterizacao.Dominialidade))
						{
							lstDependencias.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
						}
						_da.Dependencias(titulo.Id, titulo.Modelo.Id, titulo.EmpreendimentoId.GetValueOrDefault(), lstDependencias);
					}
				}

				#endregion

				#region Histórico

				eHistoricoAcao eAcao;

				switch ((eAlterarSituacaoAcao)acao)
				{
					case eAlterarSituacaoAcao.EmitirParaAssinatura:
						eAcao = eHistoricoAcao.emitir;
						break;

					case eAlterarSituacaoAcao.CancelarEmissao:
						eAcao = eHistoricoAcao.cancelaremissao;
						break;

					case eAlterarSituacaoAcao.Assinar:
						eAcao = eHistoricoAcao.assinar;
						break;

					case eAlterarSituacaoAcao.Prorrogar:
						eAcao = eHistoricoAcao.prorrogar;
						break;

					case eAlterarSituacaoAcao.Encerrar:
						eAcao = eHistoricoAcao.encerrar;
						break;

					case eAlterarSituacaoAcao.Entregar:
						eAcao = eHistoricoAcao.entregar;
						break;

					default:
						eAcao = eHistoricoAcao.emitir;
						break;
				}

				_da.Historico.Gerar(titulo.Id, eHistoricoArtefato.titulo, eAcao, bancoDeDados);
				_da.Consulta.Gerar(titulo.Id, eHistoricoArtefato.titulo, bancoDeDados);

				#region Solicitacao CAR

				if (LstCadastroAmbientalRuralTituloCodigo.Any(x => x == titulo.Modelo.Codigo.GetValueOrDefault()))
				{
					if (titulo.Situacao.Id != (int)eTituloSituacao.Concluido)
					{
						_da.Consulta.Deletar(titulo.Id, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);
					}
					else
					{
						_da.Consulta.Gerar(titulo.Id, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);
					}
				}

				#endregion

				#endregion

				bancoDeDados.Commit();

				Validacao.Add(Mensagem.TituloAlterarSituacao.TituloAltSituacaoSucesso);
			}
		}

		public Situacao ObterNovaSituacao(Titulo titulo, int acao)
		{
			if (titulo.Modelo == null || titulo.Modelo.Regras == null || titulo.Modelo.Regras.Count == 0)
			{
				throw new Exception("Modelo deve ser definido para obter nova situação");
			}

			Situacao situacao = new Situacao();

			bool possuiPrazo = titulo.Modelo.Regra(eRegra.Prazo);
			TituloModeloResposta resposta = titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo);

			switch ((eAlterarSituacaoAcao)acao)
			{
				case eAlterarSituacaoAcao.EmitirParaAssinatura:
					if (!possuiPrazo || resposta == null || Convert.ToInt32(resposta.Valor) == 1)//data da emissão
					{
						situacao.Id = 3;//Concluido
					}
					else
					{
						situacao.Id = 2;//Emitido para assinatura
					}
					break;

				case eAlterarSituacaoAcao.CancelarEmissao:
					situacao.Id = 1;//Cadastrado
					break;

				case eAlterarSituacaoAcao.Assinar:
					if (resposta == null || Convert.ToInt32(resposta.Valor) == 2)//data da assinatura
					{
						situacao.Id = 3;//Concluido
					}
					else
					{
						situacao.Id = 4;//Assinado
					}
					break;

				case eAlterarSituacaoAcao.Prorrogar:
					situacao.Id = 6;//Prorrogado
					break;

				case eAlterarSituacaoAcao.Encerrar:
					situacao.Id = 5;//Encerrado
					break;

				case eAlterarSituacaoAcao.Entregar:
				case eAlterarSituacaoAcao.Concluir:
					situacao.Id = 3;//Concluido
					break;
				case eAlterarSituacaoAcao.Suspender:
					situacao.Id = 11; //Suspenso
					break;
			}

			situacao.Nome = _busLista.TituloSituacoes.SingleOrDefault(x => x.Id == situacao.Id).Texto;
			return situacao;
		}

		public List<Acao> SetarAcoesTela(List<Acao> acoes, Titulo titulo)
		{
			bool concluir = (!titulo.Modelo.Regra(eRegra.Prazo) && (new int[] { 2, 4 }).Contains(titulo.Situacao.Id)) ||
				((titulo.Modelo.Regra(eRegra.Prazo) && titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor.ToString() == "1" && titulo.Situacao.Id == 2) || // emitido para assinatura
				(titulo.Modelo.Regra(eRegra.Prazo) && (new string[] { "1", "2" }).Contains(titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor.ToString()) && titulo.Situacao.Id == 4)); // assinado

			bool permicaoAcaoConcluir = false;

			if (concluir)
			{
				if (titulo.Situacao.Id == 2)
				{
					permicaoAcaoConcluir = User.IsInRole(ePermissao.TituloEmitir.ToString());
				}
				else
				{
					permicaoAcaoConcluir = User.IsInRole(ePermissao.TituloAssinar.ToString());
				}
			}

			//Mostrar Radio
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.EmitirParaAssinatura).Mostrar = User.IsInRole(ePermissao.TituloEmitir.ToString());
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.CancelarEmissao).Mostrar = User.IsInRole(ePermissao.TituloCancelarEmissao.ToString());
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Assinar).Mostrar = User.IsInRole(ePermissao.TituloAssinar.ToString());
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Prorrogar).Mostrar = User.IsInRole(ePermissao.TituloProrrogar.ToString());
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Encerrar).Mostrar = User.IsInRole(ePermissao.TituloEncerrar.ToString());
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Suspender).Mostrar = User.IsInRole(ePermissao.TituloEncerrar.ToString()) &&
				(titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal || titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal);
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Concluir).Mostrar = permicaoAcaoConcluir;

			//Habilitar Radio
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.EmitirParaAssinatura).Habilitado = !concluir && (titulo.Situacao.Id == 1);
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.CancelarEmissao).Habilitado = (titulo.Situacao.Id == 2);
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Assinar).Habilitado = !concluir && (titulo.Situacao.Id == 2);
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Prorrogar).Habilitado = !concluir && (new int[] { 3, 6 }).Contains(titulo.Situacao.Id) && titulo.Modelo.Regra(eRegra.Prazo);
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Encerrar).Habilitado = !concluir && (new int[] { 3, 6 }).Contains(titulo.Situacao.Id);
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Suspender).Habilitado = !concluir && (new int[] { 3, 6 }).Contains(titulo.Situacao.Id) &&
				(titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal || titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal);
			acoes.SingleOrDefault(x => x.Id == (int)eAlterarSituacaoAcao.Concluir).Habilitado = concluir;

			return acoes;
		}
	}
}