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
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ProjetoGeograficoBus
	{
		#region Propriedades

		ProjetoGeograficoValidar _validar = null;
		ProjetoGeograficoDa _da = new ProjetoGeograficoDa();
		GerenciadorConfiguracao<ConfiguracaoProjetoGeo> _configPGeo = new GerenciadorConfiguracao<ConfiguracaoProjetoGeo>(new ConfiguracaoProjetoGeo());
		//CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
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

		ConfiguracaoSistema _config = new ConfiguracaoSistema();
		GerenciadorArquivo _gerenciador;

		public string UsuarioInterno
		{
			get { return new ConfiguracaoSistema().Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public ProjetoGeograficoBus()
		{
			_validar = new ProjetoGeograficoValidar();
			_gerenciador = new GerenciadorArquivo(_config.DiretorioOrtoFotoMosaico, null);
		}

		public ProjetoGeograficoBus(ProjetoGeograficoValidar validar)
		{
			_validar = validar;
			_gerenciador = new GerenciadorArquivo(_config.DiretorioOrtoFotoMosaico, null);
		}

		#region Comandos DML

		public void Salvar(ProjetoGeografico entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					if (entidade.Id < 1)
					{
						entidade.Id = _da.ObterID(entidade.FiscalizacaoId);
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(entidade, bancoDeDados);

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
					arquivoEnviado.Tipo = 5;//#enum#(int)eProjetoGeograficoArquivoTipo.ArquivoEnviado;//5; falta verificar a tela para mudar de 5 para 3
					arquivoEnviado.FilaTipo = (int)eFilaTipoGeo.Fiscalizacao;//5;
					arquivoEnviado.Etapa = (int)eFilaEtapaGeo.Validacao;//1;
					arquivoEnviado.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;

					arquivoEnviado.IdRelacionamento = _da.ExisteArquivoFila(arquivoEnviado);

					//_da.LimparArquivoEnviadoShape(arquivoEnviado.ProjetoId, bancoDeDados);

					//Atualiza a lista de arquivos do projeto
					_da.AtualizarArquivosImportar(arquivoEnviado, bancoDeDados);

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

						arquivoEnviado.Tipo = (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado;//3;
						arquivoEnviado.FilaTipo = (int)eFilaTipoGeo.Fiscalizacao;//5;
						arquivoEnviado.IdRelacionamento = _da.ExisteArquivoFila(arquivoEnviado);

						List<ArquivoProjeto> arquivosSalvos = _da.ObterArquivos(arquivoEnviado.ProjetoId, bancoDeDados, valido: null).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado).ToList();

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
						_da.AtualizarArquivosImportar(arquivoEnviado, bancoDeDados);

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

		public void ReprocessarBaseReferencia(ArquivoProjeto arquivo)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					arquivo.Etapa = (int)eFilaEtapaGeo.Processamento;//2;
					arquivo.Situacao = (int)eFilaSituacaoGeo.Aguardando;//1;
					arquivo.Tipo = 6;//????
					arquivo.FilaTipo = (int)eFilaTipoGeo.BaseReferenciaFiscalizacao;//6;

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
					_da.InvalidarFila(projeto.Id, new List<int>() { (int)eProjetoGeograficoArquivoTipo.DadosIDAF, (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES }, bancoDeDados);

					bancoDeDados.Commit();
				}

				Validacao.Add(Mensagem.ProjetoGeografico.SalvoSucesso);
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
					arquivo.Tipo = 3;//????
					arquivo.FilaTipo = (int)eFilaTipoGeo.Fiscalizacao;//5;
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

		public void ConcluirCadastro(ProjetoGeografico projeto)
		{
			try
			{
				if (_validar.Finalizar(projeto))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.ConcluirCadastro(projeto.Id);

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

		#endregion

		#region Obter

		public ProjetoGeografico ObterProjeto(int id, bool simplificado = false)
		{
			ProjetoGeografico projeto = null;
			try
			{
				eProjetoGeograficoSituacao situacao = (eProjetoGeograficoSituacao)ObterSitacaoProjetoGeografico(id);

				projeto = _da.Obter(id, null, simplificado, situacao == eProjetoGeograficoSituacao.Finalizado);

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

		public ProjetoGeografico ObterProjetoGeograficoPorFiscalizacao(int fiscalizacaoId, bool simplificado = false, bool finalizado = false)
		{
			ProjetoGeografico projeto = _da.ObterProjetoGeograficoPorFiscalizacao(fiscalizacaoId, null, simplificado, finalizado);
			ObterOrtofotos(projeto);
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
					ProjetoGeografico projeto = ObterProjeto(projetoId, true);
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

		public int ExisteProjetoGeografico(int fiscalizacao)
		{
			try
			{
				return _da.ExisteProjetoGeografico(fiscalizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
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

		public bool Desatualizado(int id, string projetoTid)
		{
			try
			{
				ProjetoGeografico projetoGeo = _da.ObterProjetoGeografico(id);

				return projetoTid != projetoGeo.Tid;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
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
				return _da.ObterSitacaoProjetoGeografico(projetoId);
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

				requestJson.Executar(urlGeoBasesWebService + "/Autenticacao/LogOn", RequestJson.GET, new { chaveAutenticacao = geoBasesChave });

				string mbrWkt = String.Format("POLYGON (({0} {1}, {2} {1}, {2} {3}, {0} {3}, {0} {1}))",
					projeto.MenorX.ToString(NumberFormatInfo.InvariantInfo),
					projeto.MenorY.ToString(NumberFormatInfo.InvariantInfo),
					projeto.MaiorX.ToString(NumberFormatInfo.InvariantInfo),
					projeto.MaiorY.ToString(NumberFormatInfo.InvariantInfo));

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

		#endregion
	}
}