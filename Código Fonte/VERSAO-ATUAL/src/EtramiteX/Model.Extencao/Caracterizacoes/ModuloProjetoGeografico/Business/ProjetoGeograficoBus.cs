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
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data;
using Cred = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes;
using HistCaract = Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business
{
	public class ProjetoGeograficoBus
	{
		#region Propriedades

		ProjetoGeograficoValidar _validar;
		ProjetoGeograficoDa _da;
		CaracterizacaoBus _caracterizacaoBus;
		ConfiguracaoSistema _config;
		GerenciadorConfiguracao<ConfiguracaoProjetoGeo> _configPGeo;
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig;
		GerenciadorArquivo _gerenciador;

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
			get
			{
				return _configPGeo.Obter<List<SobreposicaoTipo>>(ConfiguracaoProjetoGeo.KeyTipoSobreposicao);
			}
		}

		public string UsuarioInterno
		{
			get { return new ConfiguracaoSistema().Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
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
			_gerenciador = new GerenciadorArquivo(_config.DiretorioOrtoFotoMosaico, null);
		}

		#region Comandos DML

		public void Salvar(ProjetoGeografico projeto)
		{
			try
			{
				if (!String.IsNullOrEmpty(projeto.Sobreposicoes.DataVerificacao))
				{
					projeto.Sobreposicoes.DataVerificacaoBanco = new DateTecno()
					{
						Data = DateTime.ParseExact(projeto.Sobreposicoes.DataVerificacao, "dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat)
					};
				}

				if (_validar.Salvar(projeto))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(projeto, bancoDeDados);

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

		public ArquivoProjeto ProcessarDesenhador(ProjetoGeografico projeto)
		{
			ArquivoProjeto arquivoEnviado = projeto.Arquivos.FirstOrDefault() ?? new ArquivoProjeto();
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					arquivoEnviado.Etapa = (int)eFilaEtapaGeo.Validacao;//1;

					arquivoEnviado.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;

					arquivoEnviado.IdRelacionamento = _da.ExisteArquivoFila(arquivoEnviado);

					//Atualiza a lista de arquivos do projeto
					_da.AtualizarArquivosEnviado(arquivoEnviado, bancoDeDados);

					if (arquivoEnviado.IdRelacionamento == 0)
					{
						_da.InserirFila(arquivoEnviado, bancoDeDados);
					}
					else
					{
						_da.AlterarSituacaoFila(arquivoEnviado, bancoDeDados);
					}

					ObterSituacao(arquivoEnviado);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return arquivoEnviado;
		}

		public ArquivoProjeto EnviarArquivo(ProjetoGeografico projeto)
		{
			ArquivoProjeto arquivoEnviado;

			if (projeto.Arquivos == null || projeto.Arquivos.Count <= 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.ArquivoNaoEncontrado);
			}

			arquivoEnviado = projeto.Arquivos[0];

			try
			{
				if (_validar.EnviarArquivo(arquivoEnviado))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

						_busArquivo.Copiar(arquivoEnviado);

						_busArquivo.ObterTemporario(arquivoEnviado);

						arquivoEnviado.IdRelacionamento = _da.ExisteArquivoFila(arquivoEnviado);

						List<ArquivoProjeto> arquivosSalvos = _da.ObterArquivos(arquivoEnviado.ProjetoId).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado).ToList();

						#region Erro de Duplicaçao de arquivos enviados

						if (arquivosSalvos.Count > 1)
						{
							_da.ExcluirArquivoDuplicados(arquivoEnviado.ProjetoId, bancoDeDados);
							arquivosSalvos = _da.ObterArquivos(arquivoEnviado.ProjetoId,null, bancoDeDados).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado).ToList();
						}

						#endregion

						Arquivo arqAnterior = null;

						if (arquivosSalvos.Count > 0)
						{
							ArquivoProjeto arq = arquivosSalvos.SingleOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado);
							try
							{
								arqAnterior = _busArquivo.Obter((arq ?? new ArquivoProjeto()).Id.GetValueOrDefault());
								if (arqAnterior != null)
								{
									arquivoEnviado.Id = arqAnterior.Id.Value;
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
								arqDa.MarcarDeletado(arqAnterior.Id.Value, arqAnterior.Caminho);
							}
						}

						ArquivoDa _arquivoDa = new ArquivoDa();
						_arquivoDa.Salvar(arquivoEnviado, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);

						//Atualiza a lista de arquivos do projeto
						_da.AtualizarArquivosEnviado(arquivoEnviado, bancoDeDados);

						arquivoEnviado.Buffer.Close();
						arquivoEnviado.Buffer.Dispose();
						arquivoEnviado.Buffer = null;
						arquivoEnviado.Etapa = (int)eFilaEtapaGeo.Validacao;//1;
						arquivoEnviado.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;

						if (arquivoEnviado.IdRelacionamento <= 0)
						{
							_da.InserirFila(arquivoEnviado, bancoDeDados);
						}
						else
						{
							_da.AlterarSituacaoFila(arquivoEnviado, bancoDeDados);
						}

						bancoDeDados.Commit();

						ObterSituacao(arquivoEnviado);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return arquivoEnviado;
		}

		public ArquivoProjeto GerarCroquiTitulo(int projetoId, int tituloId, BancoDeDados banco = null)
		{
			var arquivoEnviado = new ArquivoProjeto()
			{
				ProjetoId = projetoId,
				FilaTipo = (int)eFilaTipoGeo.AtividadeTitulo,
				Mecanismo = (int)eProjetoGeograficoMecanismo.Desenhador,
				Etapa = (int)eFilaEtapaGeo.GeracaoDePDF,
				Situacao = (int)eFilaSituacaoGeo.Aguardando,
				TituloId = tituloId
			};

			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					arquivoEnviado.IdRelacionamento = _da.ExisteArquivoFila(arquivoEnviado);

					if (arquivoEnviado.IdRelacionamento == 0)
					{
						_da.InserirFila(arquivoEnviado, bancoDeDados);

						ObterSituacao(arquivoEnviado);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return arquivoEnviado;
		}

		public void ReprocessarBaseReferencia(ArquivoProjeto arquivo)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					arquivo.Etapa = (int)eFilaEtapaGeo.Processamento;//2;

					arquivo.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;

					arquivo.IdRelacionamento = _da.ExisteArquivoFila(arquivo);

					if (arquivo.IdRelacionamento == 0)
					{
						_da.InserirFila(arquivo, bancoDeDados);
					}
					else
					{
						_da.AlterarSituacaoFila(arquivo, bancoDeDados);
					}

					bancoDeDados.Commit();

					_da.ObterSituacao(arquivo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarAreaAbrangencia(ProjetoGeografico projeto)
		{
			try
			{
				if (!_validar.Salvar(projeto))
				{
					return;
				}

				#region Obter Arquivos Ortofoto
				projeto.ArquivosOrtofotos = ObterArquivosOrtofotoWebService(projeto);

				if (!Validacao.EhValido)
				{
					return;
				}
				#endregion

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(projeto, bancoDeDados);

					_da.InvalidarArquivoProcessados(projeto.Id, new List<int>(), bancoDeDados);
					_da.InvalidarFila(projeto.Id, new List<int>() { (int)eFilaTipoGeo.BaseReferenciaInterna, (int)eFilaTipoGeo.BaseReferenciaGEOBASES }, bancoDeDados);

					bancoDeDados.Commit();
				}

				Validacao.Add(Mensagem.ProjetoGeografico.SalvoSucesso);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void CancelarProcessamento(ArquivoProjeto arquivo)
		{
			try
			{
				arquivo.Etapa = (int)eFilaEtapaGeo.Validacao;//1;

				arquivo.Situacao = (int)eFilaSituacaoGeo.Cancelado;//5;

				_da.AlterarSituacaoFila(arquivo);

				ObterSituacao(arquivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Reprocessar(ArquivoProjeto arquivo)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					arquivo.Etapa = (int)eFilaEtapaGeo.Validacao;//1;
					arquivo.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;

					_da.AlterarSituacaoFila(arquivo);

					_da.ObterSituacao(arquivo);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Refazer(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.Refazer(projeto))
				{
					_da.Refazer(projeto.Id, banco);
					Validacao.Add(Mensagem.ProjetoGeografico.RefeitoSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Atualizar(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.Refazer(projeto))
				{
					_da.Atualizar(projeto.Id, banco);
					Validacao.Add(Mensagem.ProjetoGeografico.SalvoSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Recarregar(ProjetoGeografico projeto)
		{
			try
			{
				if (_validar.Recarregar(projeto))
				{
					#region Cancelar Processamento

					ArquivoProjeto arquivoProcessamento = new ArquivoProjeto();
					arquivoProcessamento.ProjetoId = projeto.Id;
					arquivoProcessamento.Mecanismo = projeto.MecanismoElaboracaoId;
					arquivoProcessamento.FilaTipo = projeto.CaracterizacaoId == (int)eCaracterizacao.Dominialidade ? (int)eFilaTipoGeo.Dominialidade : (int)eFilaTipoGeo.Atividade;

					CancelarProcessamento(arquivoProcessamento);

					#endregion

					_da.Refazer(projeto.Id);
					Validacao.Add(Mensagem.ProjetoGeografico.RecarregadoSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void SalvarSobreposicoes(ProjetoGeografico projeto)
		{
			try
			{
				if (!Validacao.EhValido)
				{
					return;
				}

				if (projeto.Sobreposicoes != null && !String.IsNullOrEmpty(projeto.Sobreposicoes.DataVerificacao))
				{
					projeto.Sobreposicoes.DataVerificacaoBanco = new DateTecno()
					{
						Data = DateTime.ParseExact(projeto.Sobreposicoes.DataVerificacao, "dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat)
					};
				}

				GerenciadorTransacao.ObterIDAtual();
				_da.AlterarSobreposicoes(projeto);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void SalvarOrtofoto(ProjetoGeografico projeto)
		{
			try
			{
				projeto.ArquivosOrtofotos = ObterArquivosOrtofotoWebService(projeto);
				_da.AlterarArquivosOrtofoto(projeto);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Excluir(int empreendimento, eCaracterizacao tipo, BancoDeDados banco = null)
		{
			try
			{
				_da.Excluir(empreendimento, (int)tipo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ExcluirRascunho(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			try
			{
				_da.ExcluirRascunho(projeto.Id, banco);
				Validacao.Add(Mensagem.ProjetoGeografico.RascunhoExcluidoSucesso);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Finalizar(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.Finalizar(projeto))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();

						_da.Finalizar(projeto.Id);

						//Gerencia as dependências da caracterização
						_caracterizacaoBus.Dependencias(new Caracterizacao()
						{
							Id = projeto.Id,
							Tipo = (eCaracterizacao)projeto.CaracterizacaoId,
							DependenteTipo = eCaracterizacaoDependenciaTipo.ProjetoGeografico,
							Dependencias = projeto.Dependencias
						}, bancoDeDados);

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

		public void CopiarDadosCredenciado(Dependencia dependencia, int empreendimentoInternoID, BancoDeDados banco, BancoDeDados bancoCredenciado)
		{
			if (banco == null) return;

			try
			{
				#region Configurar Projeto

				//Obter do Credenciado
				Cred.ModuloProjetoGeografico.Bussiness.ProjetoGeograficoBus projetoGeoCredBus = new Cred.ModuloProjetoGeografico.Bussiness.ProjetoGeograficoBus();
				ProjetoGeografico projetoGeo = projetoGeoCredBus.ObterHistorico(dependencia.DependenciaId, dependencia.DependenciaTid);
				eCaracterizacao caracterizacaoTipo = (eCaracterizacao)dependencia.DependenciaCaracterizacao;

				int projetoGeoCredenciadoId = projetoGeo.Id;
				int empreendimentoCredenciadoId = projetoGeo.EmpreendimentoId;

				bool atualizarDependencias = (!Desatualizado(projetoGeo.InternoID, projetoGeo.InternoTID) && !projetoGeo.AlteradoCopiar);

				#endregion

				if (_validar.CopiarDadosCredenciado(projetoGeo))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();

						_da.CopiarDadosCredenciado(projetoGeo, empreendimentoInternoID, bancoDeDados);

						projetoGeoCredBus.AtualizarInternoIdTid(
							empreendimentoCredenciadoId,
							projetoGeoCredenciadoId,
							(eCaracterizacao)projetoGeo.CaracterizacaoId,
							projetoGeo.Id,
							GerenciadorTransacao.ObterIDAtual(),
							bancoCredenciado);

						#region Arquivo

						ArquivoBus _busArquivoInterno = new ArquivoBus(eExecutorTipo.Interno);
						ArquivoBus _busArquivoCredenciado = new ArquivoBus(eExecutorTipo.Credenciado);

						foreach (var item in projetoGeo.Arquivos)
						{
							Arquivo aux = _busArquivoCredenciado.Obter(item.Id.Value);//Obtém o arquivo completo do diretorio do credenciado(nome, buffer, etc)

							aux.Id = 0;//Zera o ID
							aux = _busArquivoInterno.SalvarTemp(aux);//salva no diretório temporário
							aux = _busArquivoInterno.Copiar(aux);//Copia para o diretório oficial

							//Salvar na Oficial
							ArquivoDa arquivoDa = new ArquivoDa();
							arquivoDa.Salvar(aux, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);

							item.Id = aux.Id;
						}

						_da.SalvarArquivosCredenciado(projetoGeo, bancoDeDados);

						#endregion

						#region Histórico

						HistCaract.Historico historico = new HistCaract.Historico();
						historico.Gerar(projetoGeo.Id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.importar, bancoDeDados);
						historico.GerarGeo(projetoGeo.Id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.importar, bancoDeDados);

						#endregion

						#region Dependencias

						//Gerencia as dependências
						projetoGeo.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(empreendimentoInternoID, caracterizacaoTipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
						_caracterizacaoBus.Dependencias(new Caracterizacao()
						{
							Id = projetoGeo.Id,
							Tipo = caracterizacaoTipo,
							DependenteTipo = eCaracterizacaoDependenciaTipo.ProjetoGeografico,
							Dependencias = projetoGeo.Dependencias
						}, bancoDeDados);

						if (projetoGeo.InternoID > 0)
						{
							if (atualizarDependencias)
							{
								CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
								caracterizacaoBus.AtualizarDependentes(projetoGeo.InternoID, caracterizacaoTipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico, projetoGeo.Tid, bancoDeDados);
							}
						}

						#endregion

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AnexarCroqui(int titulo, int arquivo, BancoDeDados banco = null) => _da.AnexarCroqui(titulo, arquivo, banco);

		#endregion

		#region Obter

		public ProjetoGeografico ObterProjeto(int id,int? tipo = null, bool simplificado = false)
		{
			ProjetoGeografico projeto = null;
			try
			{
				projeto = _da.Obter(id, tipo, null, simplificado, (!_da.ValidarProjetoGeograficoTemporario(id)));

				if (projeto.Id > 0 && !simplificado)
				{
					projeto.Dependencias = _caracterizacaoBus.ObterDependencias(id, (eCaracterizacao)projeto.CaracterizacaoId, eCaracterizacaoDependenciaTipo.ProjetoGeografico);

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

		public List<ArquivoProjeto> ObterOrtofotos(int projetoId)
		{
			List<ArquivoProjeto> lstOrtofoto = new List<ArquivoProjeto>();
			try
			{
				lstOrtofoto = _da.ObterOrtofotos(projetoId);

				if (lstOrtofoto == null || lstOrtofoto.Count == 0)
				{
					ProjetoGeografico projeto = ObterProjeto(projetoId,null, true);
					lstOrtofoto = ObterArquivosOrtofotoWebService(projeto);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return lstOrtofoto;
		}

		public void ObterOrtofotos(ProjetoGeografico projeto)
		{
			try
			{
				if (projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador)
				{
					return;
				}

				if (projeto.ArquivosOrtofotos != null && projeto.ArquivosOrtofotos.Count > 0)
				{
					var arquivoOrtofoto = projeto.ArquivosOrtofotos.First();
					if (arquivoOrtofoto.ChaveData != DateTime.MinValue &&
						arquivoOrtofoto.ChaveData.AddDays(2) >= DateTime.Now)
					{
						return;
					}
				}

				projeto.ArquivosOrtofotos = ObterArquivosOrtofotoWebService(projeto);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public List<ArquivoProjeto> ObterArquivos(int projetoId, bool finalizado = false)
		{
			try
			{
				return _da.ObterArquivos(projetoId, finalizado: finalizado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<ArquivoProjeto> ObterArquivos(int empreendimento, eCaracterizacao caracterizacao, bool finalizado = false)
		{
			try
			{
				return _da.ObterArquivos(empreendimento, caracterizacao, finalizado: finalizado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<ArquivoProjeto> ObterArquivosHistorico(int projetoId, string projetoTid)
		{
			try
			{
				return _da.ObterArquivosHistorico(projetoId, projetoTid);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public int ExisteProjetoGeografico(int empreedimentoId, int caracterizacaoTipo, bool finalizado = false)
		{
			try
			{
				return _da.ExisteProjetoGeografico(empreedimentoId, caracterizacaoTipo, finalizado: finalizado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public void ObterDependencias(ProjetoGeografico projeto, bool atual = false)
		{
			try
			{
				if (projeto.Dependencias == null || projeto.Dependencias.Count == 0 || atual)
				{
					projeto.Dependencias = _caracterizacaoBus.ObterDependenciasAtual(projeto.EmpreendimentoId, (eCaracterizacao)projeto.CaracterizacaoId, eCaracterizacaoDependenciaTipo.ProjetoGeografico);
				}

				Dependencia dependencia = projeto.Dependencias.SingleOrDefault(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico
					&& x.DependenciaCaracterizacao == (int)eCaracterizacao.Dominialidade);

				if (dependencia != null)
				{
					ProjetoGeografico dominio = _da.Obter(dependencia.DependenciaId,null, banco:null, simplificado:false, finalizado:true);

					projeto.MenorX = dominio.MenorX;
					projeto.MenorY = dominio.MenorY;
					projeto.MaiorX = dominio.MaiorX;
					projeto.MaiorY = dominio.MaiorY;

					projeto.ArquivosDominio = dominio.Arquivos.Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoProcessadoSoftwareGIS
						|| x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoProcessadoTrackMaker
						|| x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui).ToList();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ObterSituacao(ArquivoProjeto arquivo)
		{
			try
			{
				_da.ObterSituacao(arquivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ObterSituacaoFila(ArquivoProjeto arquivo)
		{
			try
			{
				_da.ObterSituacaoFila(arquivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public List<NivelPrecisao> ObterNiveisPrecisao()
		{
			return _configPGeo.Obter<List<NivelPrecisao>>(ConfiguracaoProjetoGeo.KeyObterNiveisPrecisao);
		}

		public List<SistemaCoordenada> ObterSistemaCoordenada()
		{
			return _configPGeo.Obter<List<SistemaCoordenada>>(ConfiguracaoProjetoGeo.KeyObterSistemaCoordenada);
		}

		public int ObterSitacaoProjetoGeografico(int projetoId)
		{
			try
			{
				return _da.ObterSituacaoProjetoGeografico(projetoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return 1;
		}

		#region Arquivos Ortofoto

		public List<ArquivoProjeto> ObterArquivosOrtofotoWebService(ProjetoGeografico projeto)
		{
			List<ArquivoProjeto> arquivosOrtofotos = new List<ArquivoProjeto>();
			try
			{
				RequestJson requestJson = new RequestJson();

				GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				string urlGeoBasesWebService = _config.Obter<string>(ConfiguracaoSistema.KeyUrlGeoBasesWebServices);
				string geoBasesChave = _config.Obter<string>(ConfiguracaoSistema.KeyGeoBasesWebServicesAutencicacaoChave);

				ResponseJsonData<dynamic> resp = requestJson.Executar<dynamic>(urlGeoBasesWebService + "/Autenticacao/LogOn", RequestJson.GET, new { chaveAutenticacao = geoBasesChave });

				if (resp.Erros != null && resp.Erros.Count > 0)
				{
					Validacao.Erros.AddRange(resp.Erros);
					return null;
				}

				string mbrWkt = String.Format("POLYGON (({0} {1}, {2} {1}, {2} {3}, {0} {3}, {0} {1}))",
					projeto.MenorX.ToString(NumberFormatInfo.InvariantInfo),
					projeto.MenorY.ToString(NumberFormatInfo.InvariantInfo),
					projeto.MaiorX.ToString(NumberFormatInfo.InvariantInfo),
					projeto.MaiorY.ToString(NumberFormatInfo.InvariantInfo));

				resp = requestJson.Executar<dynamic>(urlGeoBasesWebService + "/Ortofoto/ObterOrtofoto", RequestJson.POST, new { wkt = mbrWkt });

				if (resp.Erros != null && resp.Erros.Count > 0)
				{
					Validacao.Erros.AddRange(resp.Erros);
					return null;
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

		public Arquivo ArquivoOrtofoto(int id, bool finalizado, bool todos)
		{
			List<ArquivoProjeto> arquivos = _da.ObterOrtofotos(id, finalizado: finalizado, todos: todos);

			if (arquivos != null && arquivos.Count > 0)
			{
				return _gerenciador.Obter(arquivos.SingleOrDefault());
			}

			return null;
		}

		#endregion

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

		public Sobreposicoes ObterGeoSobreposiacao(int id, eCaracterizacao tipo)
		{
			Sobreposicoes sobreposicoes = new Sobreposicoes();

			try
			{
				sobreposicoes.DataVerificacao = DateTime.Now.ToString("dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
				#region Empreendimento
				Sobreposicao sobreposicaoEmp = _da.ObterGeoSobreposicaoIdaf(id, tipo);

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
				string ATPWkt = _da.ObterWktATP(id, tipo);
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

		public bool Desatualizado(int id, string projetoGeoCredenciadoTID)
		{
			try
			{
				ProjetoGeografico projetoGeo = _da.ObterProjetoGeografico(id);

				return projetoGeoCredenciadoTID != projetoGeo.Tid;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}
	}
}