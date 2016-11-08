using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness
{
	public class ProjetoGeograficoBus
	{
		public delegate void DesassociarDependencias(ProjetoDigital projetoDigital, BancoDeDados banco);

		#region Propriedades

		ProjetoGeograficoDa _da;
		ProjetoGeograficoValidar _validar;
		CaracterizacaoBus _caracterizacaoBus;
		ConfiguracaoSistema _config;
		GerenciadorConfiguracao<ConfiguracaoProjetoGeo> _configPGeo;
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig;
		GerenciadorArquivo _gerenciador;

		public string EsquemaBancoInterno
		{
			get { return new ConfiguracaoSistema().Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		private string EsquemaBancoCredenciado
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public GerenciadorConfiguracao<ConfiguracaoCaracterizacao> CaracterizacaoConfig
		{
			get { return _caracterizacaoConfig; }
		}

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public List<SobreposicaoTipo> SobreposicaoTipo
		{
			get { return _configPGeo.Obter<List<SobreposicaoTipo>>(ConfiguracaoProjetoGeo.KeyTipoSobreposicao); }
		}

		public List<NivelPrecisao> ObterNiveisPrecisao()
		{
			return _configPGeo.Obter<List<NivelPrecisao>>(ConfiguracaoProjetoGeo.KeyObterNiveisPrecisao);
		}

		public List<SistemaCoordenada> ObterSistemaCoordenada()
		{
			return _configPGeo.Obter<List<SistemaCoordenada>>(ConfiguracaoProjetoGeo.KeyObterSistemaCoordenada);
		}

		#endregion

		public ProjetoGeograficoBus() : this(new ProjetoGeograficoValidar()) { }

		public ProjetoGeograficoBus(ProjetoGeograficoValidar validar)
		{
			_da = new ProjetoGeograficoDa();
			_validar = new ProjetoGeograficoValidar();
			_caracterizacaoBus = new CaracterizacaoBus();
			_config = new ConfiguracaoSistema();
			_configPGeo = new GerenciadorConfiguracao<ConfiguracaoProjetoGeo>(new ConfiguracaoProjetoGeo());
			_caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
			_gerenciador = new GerenciadorArquivo(_config.DiretorioOrtoFotoMosaico, EsquemaBancoInterno);
		}

		#region Comandos DML

		public void Salvar(ProjetoGeografico projetoGeo)
		{
			try
			{
				if (!String.IsNullOrEmpty(projetoGeo.Sobreposicoes.DataVerificacao))
				{
					projetoGeo.Sobreposicoes.DataVerificacaoBanco = new DateTecno()
					{
						Data = DateTime.ParseExact(projetoGeo.Sobreposicoes.DataVerificacao, "dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat)
					};
				}

				if (_validar.Salvar(projetoGeo))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(projetoGeo, bancoDeDados);

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.ProjetoGeografico.SalvoSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Processar(ProcessamentoGeo processamentoGeo)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
				{
					processamentoGeo.Etapa = (processamentoGeo.Etapa <= 0) ? (int)eFilaEtapaGeo.Validacao : processamentoGeo.Etapa;

					processamentoGeo.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;

					processamentoGeo.Id = _da.ExisteItemFila(processamentoGeo);

					_da.ExcluirArquivos(processamentoGeo.ProjetoId, bancoDeDados);

					if (processamentoGeo.Id == 0)
					{
						_da.InserirFila(processamentoGeo, bancoDeDados);
					}
					else
					{
						_da.AlterarSituacaoFila(processamentoGeo, bancoDeDados);
					}

					ObterSituacao(processamentoGeo);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public ArquivoProjeto EnviarArquivo(ArquivoProjeto arquivo)
		{
			try
			{
				if (_validar.EnviarArquivo(arquivo))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
						ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);

						_busArquivo.Copiar(arquivo);

						_busArquivo.ObterTemporario(arquivo);

						arquivo.Processamento.Id = _da.ExisteItemFila(arquivo.Processamento);

						_da.ExcluirArquivoDuplicados(arquivo.ProjetoId, bancoDeDados);

						ArquivoProjeto arq = _da.ObterArquivos(arquivo.ProjetoId, bancoDeDados).SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado) ?? new ArquivoProjeto();
						if (arq.Id.GetValueOrDefault() > 0 && !_da.ArquivoAssociadoProjetoDigital(arq.Id.GetValueOrDefault()))
						{
							Arquivo arqAnterior = null;

							try
							{
								arqAnterior = _busArquivo.Obter(arq.Id.GetValueOrDefault());
								if (arqAnterior != null)
								{
									arquivo.Id = arqAnterior.Id.Value;
									_busArquivo.Deletar(arqAnterior.Caminho);
								}
							}
							catch
							{
								ArquivoDa arqDa = new ArquivoDa();
								if (arqAnterior == null && (arq ?? new ArquivoProjeto()).Id.GetValueOrDefault() > 0)
								{
									arqAnterior = _busArquivo.ObterDados((arq ?? new ArquivoProjeto()).Id.GetValueOrDefault());
								}
								arqDa.MarcarDeletado(arqAnterior.Id.Value, arqAnterior.Caminho, bancoDeDados);
							}
						}

						ArquivoDa arquivoDa = new ArquivoDa();
						arquivoDa.Salvar(arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);

						//Atualiza a lista de arquivos do projeto
						_da.AtualizarArquivosEnviado(arquivo, bancoDeDados);

						arquivo.Buffer.Close();
						arquivo.Buffer.Dispose();
						arquivo.Buffer = null;
						arquivo.Processamento.Etapa = (int)eFilaEtapaGeo.Validacao;
						arquivo.Processamento.Situacao = (int)eFilaSituacaoGeo.Aguardando;

						if (arquivo.Processamento.Id <= 0)
						{
							_da.InserirFila(arquivo.Processamento, bancoDeDados);
						}
						else
						{
							_da.AlterarSituacaoFila(arquivo.Processamento, bancoDeDados);
						}

						bancoDeDados.Commit();

						ObterSituacao(arquivo.Processamento);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return arquivo;

		}

		public void AlterarAreaAbrangencia(ProjetoGeografico projetoGeo)
		{
			try
			{
				if (!_validar.Salvar(projetoGeo))
				{
					return;
				}

				#region Obter Arquivos Ortofoto

				projetoGeo.ArquivosOrtofotos = ObterArquivosOrtofotoWebService(projetoGeo);

				if (!Validacao.EhValido)
				{
					return;
				}

				#endregion

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(projetoGeo, bancoDeDados);

					_da.InvalidarArquivoProcessados(projetoGeo.Id, new List<int>(), bancoDeDados);

					_da.InvalidarFila(
						projetoGeo.Id,
						new List<int>() { (int)eFilaTipoGeo.BaseReferenciaInterna, (int)eFilaTipoGeo.BaseReferenciaGEOBASES },
						bancoDeDados);

					bancoDeDados.Commit();
				}

				Validacao.Add(Mensagem.ProjetoGeografico.SalvoSucesso);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ReprocessarBaseReferencia(ProcessamentoGeo processamentoGeo)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
				{
					processamentoGeo.Etapa = (int)eFilaEtapaGeo.Processamento;

					processamentoGeo.Situacao = (int)eFilaSituacaoGeo.Aguardando;

					processamentoGeo.Id = _da.ExisteItemFila(processamentoGeo);

					if (processamentoGeo.Id == 0)
					{
						_da.InserirFila(processamentoGeo, bancoDeDados);
					}
					else
					{
						_da.AlterarSituacaoFila(processamentoGeo, bancoDeDados);
					}

					bancoDeDados.Commit();

					_da.ObterSituacao(processamentoGeo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void CancelarProcessamento(ProcessamentoGeo processamentoGeo)
		{
			try
			{
				processamentoGeo.Etapa = (int)eFilaEtapaGeo.Validacao;//1;
				processamentoGeo.Situacao = (int)eFilaSituacaoGeo.Cancelado;//5;

				_da.AlterarSituacaoFila(processamentoGeo);

				ObterSituacao(processamentoGeo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Reprocessar(ProcessamentoGeo processamentoGeo)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					processamentoGeo.Etapa = (int)eFilaEtapaGeo.Validacao;//1;
					processamentoGeo.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;

					_da.AlterarSituacaoFila(processamentoGeo);

					_da.ObterSituacao(processamentoGeo);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Refazer(ProjetoGeografico projetoGeo)
		{
			try
			{
				if (_validar.Refazer(projetoGeo))
				{
					_da.Refazer(projetoGeo.Id);
					Validacao.Add(Mensagem.ProjetoGeografico.RefeitoSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Recarregar(ProjetoGeografico projetoGeo)
		{
			try
			{
				if (_validar.Recarregar(projetoGeo))
				{
					_da.Refazer(projetoGeo.Id);
					Validacao.Add(Mensagem.ProjetoGeografico.RecarregadoSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void SalvarSobreposicoes(ProjetoGeografico projetoGeo)
		{
			try
			{
				if (!Validacao.EhValido)
				{
					return;
				}

				if (projetoGeo.Sobreposicoes != null && !String.IsNullOrEmpty(projetoGeo.Sobreposicoes.DataVerificacao))
				{
					projetoGeo.Sobreposicoes.DataVerificacaoBanco = new DateTecno()
					{
						Data = DateTime.ParseExact(projetoGeo.Sobreposicoes.DataVerificacao, "dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat)
					};
				}

				GerenciadorTransacao.ObterIDAtual();

				_da.AlterarSobreposicoes(projetoGeo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void SalvarOrtofoto(ProjetoGeografico projetoGeo)
		{
			try
			{
				projetoGeo.ArquivosOrtofotos = ObterArquivosOrtofotoWebService(projetoGeo);
				_da.AlterarArquivosOrtofoto(projetoGeo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Excluir(int empreendimentoID, eCaracterizacao caracterizacaoTipo, BancoDeDados banco = null)
		{
			try
			{
				_da.Excluir(empreendimentoID, (int)caracterizacaoTipo, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ExcluirRascunho(ProjetoGeografico projetoGeo)
		{
			try
			{
				_da.ExcluirRascunho(projetoGeo.Id);
				Validacao.Add(Mensagem.ProjetoGeografico.RascunhoExcluidoSucesso);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Finalizar(ProjetoGeografico projetoGeo, int projetoDigitalID, DesassociarDependencias desassociarDependencias)
		{
			try
			{
				if (_validar.Finalizar(projetoGeo))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Finalizar(projetoGeo);

						//Gerencia as dependências da caracterização
						_caracterizacaoBus.Dependencias(new Caracterizacao()
						{
							Id = projetoGeo.Id,
							Tipo = (eCaracterizacao)projetoGeo.CaracterizacaoId,
							DependenteTipo = eCaracterizacaoDependenciaTipo.ProjetoGeografico,
							Dependencias = projetoGeo.Dependencias
						}, bancoDeDados);

						#region Alterar Projeto Digital

						ProjetoDigital projetoDigital = new ProjetoDigital();
						projetoDigital.Id = projetoDigitalID;
						projetoDigital.EmpreendimentoId = projetoGeo.EmpreendimentoId;
						projetoDigital.Dependencias.Add(new Dependencia() { DependenciaCaracterizacao = (int)projetoGeo.CaracterizacaoId });
						desassociarDependencias(projetoDigital, bancoDeDados);

						#endregion

						if (!Validacao.EhValido)
						{
							bancoDeDados.Rollback();
						}

						Validacao.Add(Mensagem.ProjetoGeografico.FinalizadoSucesso);
						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void CopiarDadosInstitucional(int empreendimentoID, int empreendimentoInternoID, eCaracterizacao caracterizacaoTipo, BancoDeDados banco)
		{
			if (banco == null)
			{
				return;
			}

			#region Configurar Projeto

			ProjetoGeograficoInternoBus projetoGeograficoInternoBus = new ProjetoGeograficoInternoBus();
			ProjetoGeografico projetoGeo = new ProjetoGeografico();

			//Obter do Institucional
			projetoGeo.InternoID = projetoGeograficoInternoBus.ExisteProjetoGeografico(empreendimentoInternoID, (int)caracterizacaoTipo);
			if (projetoGeo.InternoID > 0)
			{
				projetoGeo = projetoGeograficoInternoBus.ObterProjeto(projetoGeo.InternoID);
			}

			projetoGeo.EmpreendimentoId = empreendimentoID;
			projetoGeo.InternoID = projetoGeo.Id;
			projetoGeo.InternoTID = projetoGeo.Tid;

			#endregion

			if (_validar.CopiarDadosInstitucional(projetoGeo))
			{
				projetoGeo.Id = 0;//Só é apagado aqui por causa da validação de soprepor ponto do empreendimento

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.CopiarDadosInstitucional(projetoGeo);

					#region Arquivo

					ArquivoBus _busArquivoInterno = new ArquivoBus(eExecutorTipo.Interno);
					ArquivoBus _busArquivoCredenciado = new ArquivoBus(eExecutorTipo.Credenciado);

					foreach (var item in projetoGeo.Arquivos)
					{
						Arquivo aux = _busArquivoInterno.Obter(item.Id.Value);//Obtém o arquivo completo do diretorio do interno(nome, buffer, etc)

						aux.Id = 0;//Zera o ID
						aux = _busArquivoCredenciado.SalvarTemp(aux);//salva no diretório temporário
						aux = _busArquivoCredenciado.Copiar(aux);//Copia para o diretório oficial

						//Salvar na Oficial
						ArquivoDa arquivoDa = new ArquivoDa(EsquemaBancoCredenciado);
						arquivoDa.Salvar(aux, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Credenciado, User.FuncionarioTid, bancoDeDados);

						item.Id = aux.Id;
					}

					_da.SalvarArquivosInstitucional(projetoGeo, bancoDeDados);

					#endregion

					#region Histórico

					Historico historico = new Historico();
					historico.Gerar(projetoGeo.Id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.copiar, bancoDeDados, null);
					historico.GerarGeo(projetoGeo.Id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.copiar, bancoDeDados, null);

					#endregion

					#region Dependencias

					//Gerencia as dependências
					projetoGeo.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(empreendimentoID, caracterizacaoTipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
					_caracterizacaoBus.Dependencias(new Caracterizacao()
					{
						Id = projetoGeo.Id,
						Tipo = caracterizacaoTipo,
						DependenteTipo = eCaracterizacaoDependenciaTipo.ProjetoGeografico,
						Dependencias = projetoGeo.Dependencias
					}, bancoDeDados);

					#endregion

					bancoDeDados.Commit();
				}
			}
		}

		#endregion

		#region Obter

		public ProjetoGeografico ObterProjeto(int projetoGeoID, int projetoDigitalID = 0, bool simplificado = false)
		{
			ProjetoGeografico projeto = null;
			try
			{
				projeto = _da.Obter(projetoGeoID, null, simplificado, (!_da.ValidarProjetoGeograficoTemporario(projetoGeoID)));

				if (projeto.Id > 0 && !simplificado)
				{
					projeto.Dependencias = _caracterizacaoBus.ObterDependencias(projetoGeoID, (eCaracterizacao)projeto.CaracterizacaoId, eCaracterizacaoDependenciaTipo.ProjetoGeografico);

					ObterOrtofotos(projeto);

					#region Sobreposicoes

					if (projeto.Sobreposicoes.Itens != null && projeto.Sobreposicoes.Itens.Count > 0)
					{
						foreach (var item in SobreposicaoTipo)
						{
							if (projeto.Sobreposicoes.Itens.Exists(x => x.Tipo == item.Id))
							{
								projeto.Sobreposicoes.Itens.First(x => x.Tipo == item.Id).TipoTexto = item.Texto;
							}
							else
							{
								projeto.Sobreposicoes.Itens.Add(new Sobreposicao()
								{
									Tipo = item.Id,
									TipoTexto = item.Texto,
									Base = (int)((item.Id == (int)eSobreposicaoTipo.OutrosEmpreendimento) ? eSobreposicaoBase.IDAF : eSobreposicaoBase.GeoBase),
									Identificacao = " - "
								});
							}
						}
					}

					#endregion
				}

				return projeto;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return projeto;
		}

		public ProjetoGeografico ObterHistorico(int projetoGeoID, string projetoGeoTID, bool simplificado = false)
		{
			ProjetoGeografico projeto = null;
            
			try
			{
				projeto = _da.ObterHistorico(projetoGeoID, projetoGeoTID, simplificado: simplificado);

				if (projeto.Id > 0 && !simplificado)
				{
					ObterOrtofotos(projeto);

					#region Sobreposicoes

					if (projeto.Sobreposicoes.Itens != null && projeto.Sobreposicoes.Itens.Count > 0)
					{
						foreach (var item in SobreposicaoTipo)
						{
							if (projeto.Sobreposicoes.Itens.Exists(x => x.Tipo == item.Id))
							{
								projeto.Sobreposicoes.Itens.First(x => x.Tipo == item.Id).TipoTexto = item.Texto;
							}
							else
							{
								projeto.Sobreposicoes.Itens.Add(new Sobreposicao()
								{
									Tipo = item.Id,
									TipoTexto = item.Texto,
									Base = (int)((item.Id == (int)eSobreposicaoTipo.OutrosEmpreendimento) ? eSobreposicaoBase.IDAF : eSobreposicaoBase.GeoBase),
									Identificacao = " - "
								});
							}
						}
					}

					#endregion
				}

				return projeto;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return projeto;
		}

		public List<ArquivoProjeto> ObterOrtofotos(int projetoGeoID)
		{
			List<ArquivoProjeto> lstOrtofoto = new List<ArquivoProjeto>();
			try
			{
				lstOrtofoto = _da.ObterOrtofotos(projetoGeoID);

				if (lstOrtofoto == null || lstOrtofoto.Count == 0)
				{
					ProjetoGeografico projeto = ObterProjeto(projetoGeoID, simplificado: true);
					lstOrtofoto = ObterArquivosOrtofotoWebService(projeto);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return lstOrtofoto;
		}

		public void ObterOrtofotos(ProjetoGeografico projetoGeo)
		{
			try
			{
				if (projetoGeo.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador)
				{
					return;
				}

				if (projetoGeo.ArquivosOrtofotos != null && projetoGeo.ArquivosOrtofotos.Count > 0)
				{
					var arquivoOrtofoto = projetoGeo.ArquivosOrtofotos.First();
					if (arquivoOrtofoto.ChaveData != DateTime.MinValue &&
						arquivoOrtofoto.ChaveData.AddDays(2) >= DateTime.Now)
					{
						return;
					}
				}

				projetoGeo.ArquivosOrtofotos = ObterArquivosOrtofotoWebService(projetoGeo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public List<ArquivoProjeto> ObterArquivos(int projetoGeoID, bool finalizado = false)
		{
			try
			{
				return _da.ObterArquivos(projetoGeoID, finalizado: finalizado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<ArquivoProjeto> ObterArquivos(int empreendimentoID, eCaracterizacao caracterizacaoTipo, bool finalizado = false)
		{
			try
			{
				return _da.ObterArquivos(empreendimentoID, caracterizacaoTipo, finalizado: finalizado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<ArquivoProjeto> ObterArquivosHistorico(int projetoGeoID, string projetoTid)
		{
			try
			{
				return _da.ObterArquivosHistorico(projetoGeoID, projetoTid);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public void ObterDependencias(ProjetoGeografico projetoGeo, bool atual = false)
		{
			try
			{
				if (projetoGeo.Dependencias == null || projetoGeo.Dependencias.Count == 0 || atual)
				{
					projetoGeo.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(projetoGeo.EmpreendimentoId, (eCaracterizacao)projetoGeo.CaracterizacaoId, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
				}

				Dependencia dependencia = projetoGeo.Dependencias.SingleOrDefault(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico
					&& x.DependenciaCaracterizacao == (int)eCaracterizacao.Dominialidade);

				if (dependencia != null)
				{
					ProjetoGeografico dominio = _da.Obter(dependencia.DependenciaId, null, false, true);

					projetoGeo.MenorX = dominio.MenorX;
					projetoGeo.MenorY = dominio.MenorY;
					projetoGeo.MaiorX = dominio.MaiorX;
					projetoGeo.MaiorY = dominio.MaiorY;

					projetoGeo.ArquivosDominio = dominio.Arquivos.Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoProcessadoSoftwareGIS
						|| x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoProcessadoTrackMaker
						|| x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui).ToList();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ObterSituacao(ProcessamentoGeo processamentoGeo)
		{
			try
			{
				_da.ObterSituacao(processamentoGeo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ObterSituacaoFila(ProcessamentoGeo processamentoGeo)
		{
			try
			{
				_da.ObterSituacaoFila(processamentoGeo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public int ObterSitacaoProjetoGeografico(int projetoGeoID)
		{
			try
			{
				return _da.ObterSituacaoProjetoGeografico(projetoGeoID);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return 1;
		}

		public List<ArquivoProjeto> ObterArquivosOrtofotoWebService(ProjetoGeografico projetoGeo)
		{
			List<ArquivoProjeto> arquivosOrtofotos = new List<ArquivoProjeto>();
			try
			{
				RequestJson requestJson = new RequestJson();

				GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				string urlGeoBasesWebService = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices);
				string geoBasesChave = _config.Obter<string>(ConfiguracaoSistema.KeyGeoBasesWebServicesAutencicacaoChave);

				requestJson.Executar(urlGeoBasesWebService + "/Autenticacao/LogOn", RequestJson.GET, new { chaveAutenticacao = geoBasesChave });

				string mbrWkt = String.Format("POLYGON (({0} {1}, {2} {1}, {2} {3}, {0} {3}, {0} {1}))",
					projetoGeo.MenorX.ToString(NumberFormatInfo.InvariantInfo),
					projetoGeo.MenorY.ToString(NumberFormatInfo.InvariantInfo),
					projetoGeo.MaiorX.ToString(NumberFormatInfo.InvariantInfo),
					projetoGeo.MaiorY.ToString(NumberFormatInfo.InvariantInfo));

				ResponseJsonData<dynamic> resp = requestJson.Executar<dynamic>(urlGeoBasesWebService + "/Ortofoto/ObterOrtofoto", RequestJson.POST, new { wkt = mbrWkt });

				if (resp.Erros != null && resp.Erros.Count > 0)
				{
					Validacao.Erros.AddRange(resp.Erros);
				}

				foreach (var item in resp.Data)
				{
					arquivosOrtofotos.Add(new ArquivoProjeto() { Nome = Path.GetFileName(item["ArquivoNome"]), Caminho = item["ArquivoNome"], Chave = item["ArquivoChave"], ChaveData = Convert.ToDateTime(item["ArquivoChaveData"]) });
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return arquivosOrtofotos;
		}

		public Arquivo ArquivoOrtofoto(int projetoGeoID, bool finalizado, bool todos)
		{
			List<ArquivoProjeto> arquivos = _da.ObterOrtofotos(projetoGeoID, finalizado: finalizado, todos: todos);

			if (arquivos != null && arquivos.Count > 0)
			{
				return _gerenciador.Obter(arquivos.SingleOrDefault());
			}

			return null;
		}

		public Sobreposicoes ObterGeoSobreposiacao(int projetoGeoID, eCaracterizacao caracterizacaoTipo)
		{
			Sobreposicoes sobreposicoes = new Sobreposicoes();

			try
			{
				sobreposicoes.DataVerificacao = DateTime.Now.ToString("dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
				#region Empreendimento
				Sobreposicao sobreposicaoEmp = _da.ObterGeoSobreposicaoIdaf(projetoGeoID, caracterizacaoTipo);

				if (sobreposicaoEmp == null)
				{
					sobreposicaoEmp = CriarObjSobreposicao(string.Empty);
					sobreposicaoEmp.Identificacao = " - ";
				}
				else
				{
					sobreposicaoEmp.TipoTexto = SobreposicaoTipo.Single(y => y.Id == (int)sobreposicaoEmp.Tipo).Texto;
				}

				sobreposicoes.Itens.Add(sobreposicaoEmp);
				#endregion

				#region Feicoes Geobases
				string ATPWkt = _da.ObterWktATP(projetoGeoID, caracterizacaoTipo);
				string urlGeoBasesWebService = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices);
				urlGeoBasesWebService = urlGeoBasesWebService + "/Topologia/Relacao";

				if (String.IsNullOrEmpty(ATPWkt))
				{
					Validacao.Add(Mensagem.ProjetoGeografico.ATPNaoEncontrada);
					return null;
				}

				//teste
				//urlGeoBasesWebService = "http://localhost:33716/Topologia/Relacao";

				List<string> feicoes = new List<string>(){			
					"HID_BACIA_HIDROGRAFICA","LIM_TERRA_INDIGENA","LIM_UNIDADE_PROTECAO_INTEGRAL","LIM_UNIDADE_CONSERV_NAO_SNUC","LIM_OUTRAS_UNID_PROTEGIDAS","LIM_UNIDADE_USO_SUSTENTAVEL"
				};

				RequestJson request = new RequestJson();
				ResponseJsonData<List<FeicaoJson>> responseData = request.Executar<List<FeicaoJson>>(urlGeoBasesWebService, RequestJson.POST, new { feicoes = feicoes, wkt = ATPWkt });

				if (responseData.Erros != null && responseData.Erros.Count > 0)
				{
					responseData.Erros.Insert(0, new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Erro no WebService GeoBases" });
					Validacao.Erros.AddRange(responseData.Erros);
					return null;
				}

				Sobreposicao sobreposicao = null;

				foreach (var key in feicoes)
				{
					sobreposicao = CriarObjSobreposicao(key);

					if (!responseData.Data.Exists(x => x.Nome == key))
					{
						sobreposicao.Identificacao = " - ";
						sobreposicoes.Itens.Add(sobreposicao);
						continue;
					}

					FeicaoJson feicaoJson = responseData.Data.First(x => x.Nome == key);
					List<string> lst = null;

					if (!feicaoJson.Geometrias.SelectMany(x => x.Atributos).Any(x => x.Nome == "ADMINISTRACAO"))
					{
						sobreposicao.Identificacao = String.Join("; ", feicaoJson.Geometrias.SelectMany(x => x.Atributos).Select(x => x.Valor).ToArray());
					}
					else
					{
						lst = feicaoJson.Geometrias
										.Select(x => String.Format("{0} - {1}",
											x.Atributos.Single(y => y.Nome == "Nome").Valor,
											x.Atributos.Single(y => y.Nome == "ADMINISTRACAO").Valor)).ToList();
						sobreposicao.Identificacao = String.Join("; ", lst.ToArray());
					}

					sobreposicoes.Itens.Add(sobreposicao);
				}
				#endregion
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return sobreposicoes;
		}

		#endregion

		#region Auxiliares

		private Sobreposicao CriarObjSobreposicao(string key)
		{
			Sobreposicao sobreposicao = new Sobreposicao();

			switch (key)
			{
				case "HID_BACIA_HIDROGRAFICA":
					sobreposicao.Base = (int)eSobreposicaoBase.GeoBase;
					sobreposicao.Tipo = (int)eSobreposicaoTipo.BaciasHidrograficas;
					break;

				case "LIM_TERRA_INDIGENA":
					sobreposicao.Base = (int)eSobreposicaoBase.GeoBase;
					sobreposicao.Tipo = (int)eSobreposicaoTipo.TerrasIndigenas;
					break;

				case "LIM_OUTRAS_UNID_PROTEGIDAS":
					sobreposicao.Base = (int)eSobreposicaoBase.GeoBase;
					sobreposicao.Tipo = (int)eSobreposicaoTipo.UnidadesProtegidas;
					break;

				case "LIM_UNIDADE_PROTECAO_INTEGRAL":
					sobreposicao.Base = (int)eSobreposicaoBase.GeoBase;
					sobreposicao.Tipo = (int)eSobreposicaoTipo.UnidadesProtecaoIntegral;
					break;

				case "LIM_UNIDADE_CONSERV_NAO_SNUC":
					sobreposicao.Base = (int)eSobreposicaoBase.GeoBase;
					sobreposicao.Tipo = (int)eSobreposicaoTipo.UnidadesConservacaoNaoSNUC;
					break;

				case "LIM_UNIDADE_USO_SUSTENTAVEL":
					sobreposicao.Base = (int)eSobreposicaoBase.GeoBase;
					sobreposicao.Tipo = (int)eSobreposicaoTipo.UnidadesUsoSustentavel;
					break;

				default:
					sobreposicao.Tipo = (int)eSobreposicaoTipo.OutrosEmpreendimento;
					sobreposicao.Base = (int)eSobreposicaoBase.IDAF;
					break;
			}

			sobreposicao.TipoTexto = SobreposicaoTipo.Single(y => y.Id == (int)sobreposicao.Tipo).Texto;

			return sobreposicao;
		}

		public int ExisteProjetoGeografico(int empreedimentoID, int caracterizacaoTipo)
		{
			try
			{
				return _da.ExisteProjetoGeografico(empreedimentoID, caracterizacaoTipo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public bool Desatualizado(int projetoID, string projetoTID)
		{
			try
			{
				ProjetoGeografico projetoGeo = _da.ObterProjetoGeografico(projetoID);

				return projetoTID != projetoGeo.Tid;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool IsProcessado(int projetoID, eCaracterizacao caracterizacao) 
		{
			try
			{
				return _da.VerificarProjetoGeograficoProcessado(projetoID, caracterizacao);
			}
			catch (Exception e)
			{

				Validacao.AddErro(e);
			}

			return false;
		}

		public void AtualizarInternoIdTid(int empreendimentoId, int projetoId, eCaracterizacao caracterizacaoTipo, int internoId, string tid, BancoDeDados banco = null)
		{	
			_da.AtualizarInternoIdTid(projetoId, internoId, tid, banco);

			List<Dependencia> dependencias = _caracterizacaoBus.ObterDependenciasAtual(empreendimentoId, caracterizacaoTipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico, banco);

			_caracterizacaoBus.Dependencias(new Caracterizacao()
			{
				Id = projetoId,
				Tipo = caracterizacaoTipo,
				DependenteTipo = eCaracterizacaoDependenciaTipo.ProjetoGeografico,
				Dependencias = dependencias
			}, banco);
		}

		#endregion
	}
}