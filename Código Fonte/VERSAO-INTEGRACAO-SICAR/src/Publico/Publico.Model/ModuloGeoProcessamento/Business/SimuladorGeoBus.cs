using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Data;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business
{
	public class SimuladorGeoBus
	{
		#region Propriedades

		SimuladorGeoValidar _validar = null;
		SimuladorGeoDa _da = null;
		GerenciadorConfiguracao<ConfiguracaoProjetoGeo> _configPGeo = new GerenciadorConfiguracao<ConfiguracaoProjetoGeo>(new ConfiguracaoProjetoGeo());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		public GerenciadorConfiguracao<ConfiguracaoCaracterizacao> CaracterizacaoConfig
		{
			get { return _caracterizacaoConfig; }
		}

		public List<SobreposicaoTipo> SobreposicaoTipo
		{
			get
			{
				return _configPGeo.Obter<List<SobreposicaoTipo>>(ConfiguracaoProjetoGeo.KeyTipoSobreposicao);
			}
		}

		ConfiguracaoSistema _config = new ConfiguracaoSistema();

		public String UsuarioPublicoGeo
		{
			get { return _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioPublicoGeo); }

		}

		#endregion

		public SimuladorGeoBus()
		{
			_da = new SimuladorGeoDa(UsuarioPublicoGeo);
			_validar = new SimuladorGeoValidar();
		}

		#region Comandos DML

		public SimuladorGeoArquivo EnviarArquivo(SimuladorGeo projeto)
		{
			SimuladorGeoArquivo arquivoEnviado = new SimuladorGeoArquivo();

			try
			{
				if (!_validar.EnviarArquivo(projeto))
				{
					return new SimuladorGeoArquivo();
				}

				arquivoEnviado = projeto.ArquivoEnviado;
				arquivoEnviado.Tipo = (int)eSimuladorGeoFilaTipo.Dominialidade;
				arquivoEnviado.Etapa = (int)eSimuladorGeoFilaEtapa.Validacao;
				arquivoEnviado.Situacao = (int)eSimuladorGeoFilaSituacao.Aguardando;
				arquivoEnviado.Mecanismo = (int)eSimuladorGeoMecanismo.Simulador;

				GerenciadorTransacao.ObterIDAtual();
				SimuladorGeoArquivoBus busArquivo = new SimuladorGeoArquivoBus();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
				{
					bancoDeDados.IniciarTransacao();

					busArquivo.Copiar(arquivoEnviado);

					busArquivo.ObterTemporario(arquivoEnviado);

					arquivoEnviado.IdRelacionamento = _da.ExisteArquivoFila(arquivoEnviado, bancoDeDados);

					List<SimuladorGeoArquivo> arquivosSalvos = _da.ObterArquivos(arquivoEnviado.ProjetoId, bancoDeDados)
						.Where(x => x.Tipo == (int)eSimuladorGeoArquivoTipo.ArquivoEnviado).ToList();

					Arquivo arqAnterior = null;

					if (arquivosSalvos.Count > 0)
					{
						SimuladorGeoArquivo arq = arquivosSalvos.FirstOrDefault(x => x.Tipo == (int)eSimuladorGeoArquivoTipo.ArquivoEnviado);
						try
						{
							arqAnterior = busArquivo.Obter((arq ?? new SimuladorGeoArquivo()).Id.GetValueOrDefault());
							if (arqAnterior != null)
							{
								arquivoEnviado.Id = arqAnterior.Id.Value;
								busArquivo.Deletar(arqAnterior.Caminho);
							}
						}
						catch
						{
							ArquivoDa arqDa = new ArquivoDa();
							if (arqAnterior == null && (arq ?? new SimuladorGeoArquivo()).Id.GetValueOrDefault() > 0)
							{
								arqAnterior = busArquivo.ObterDados((arq ?? new SimuladorGeoArquivo()).Id.GetValueOrDefault());
							}
							arqDa.MarcarDeletado(arqAnterior.Id.Value, arqAnterior.Caminho, bancoDeDados);
						}
					}

					ArquivoDa _arquivoDa = new ArquivoDa();
					_arquivoDa.Salvar(arquivoEnviado, null, "PublicoGeo", "PublicoGeo", (int)eExecutorTipo.Publico, null, bancoDeDados);

					arquivoEnviado.Buffer.Close();
					arquivoEnviado.Buffer.Dispose();
					arquivoEnviado.Buffer = null;

					projeto.MenorX = (projeto.EastingDecimal > 0) ? projeto.EastingDecimal - 5000 : Decimal.Zero;
					projeto.MenorY = (projeto.NorthingDecimal > 0) ? projeto.NorthingDecimal - 5000 : Decimal.Zero;
					projeto.MaiorX = (projeto.EastingDecimal > 0) ? projeto.EastingDecimal + 5000 : Decimal.Zero;
					projeto.MaiorY = (projeto.NorthingDecimal > 0) ? projeto.NorthingDecimal + 5000 : Decimal.Zero;

					_da.Salvar(projeto, bancoDeDados);

					//Atualiza a lista de arquivos do projeto
					_da.AtualizarArquivosImportar(arquivoEnviado, bancoDeDados);

					if (arquivoEnviado.IdRelacionamento <= 0)
					{
						_da.InserirFila(arquivoEnviado, bancoDeDados);
					}
					else
					{
						_da.AlterarSituacaoFila(arquivoEnviado, bancoDeDados);
					}

					_da.AtualizarGeoEmp(projeto, bancoDeDados);

					bancoDeDados.Commit();
				}

				ObterSituacao(arquivoEnviado);

			}
			catch (Exception exc)
			{
				if (arquivoEnviado != null && arquivoEnviado.Buffer != null)
				{
					arquivoEnviado.Buffer.Close();
					arquivoEnviado.Buffer.Dispose();
					arquivoEnviado.Buffer = null;
				}

				Validacao.AddErro(exc);
			}
			return arquivoEnviado;
		}

		public void ReprocessarBaseReferencia(SimuladorGeoArquivo arquivo)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
				{
					arquivo.Mecanismo = (int)eSimuladorGeoMecanismo.Simulador;
					arquivo.Etapa = (int)eSimuladorGeoFilaEtapa.Processamento;
					arquivo.Situacao = (int)eSimuladorGeoFilaSituacao.Aguardando;

					arquivo.IdRelacionamento = _da.ExisteArquivoFila(arquivo, bancoDeDados);

					if (arquivo.IdRelacionamento == 0)
					{
						_da.InserirFila(arquivo, bancoDeDados);
					}
					else
					{
						_da.AlterarSituacaoFila(arquivo, bancoDeDados);
					}

					bancoDeDados.Commit();

					_da.ObterSituacao(arquivo, bancoDeDados);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void CancelarProcessamento(SimuladorGeoArquivo arquivo)
		{
			try
			{
				arquivo.Etapa = (int)eSimuladorGeoFilaEtapa.Validacao;
				arquivo.Situacao = (int)eSimuladorGeoFilaSituacao.Cancelado;
				arquivo.Tipo = (int)eSimuladorGeoFilaTipo.Dominialidade;
				arquivo.Mecanismo = (int)eSimuladorGeoMecanismo.Simulador;

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
				{
					_da.AlterarSituacaoFila(arquivo, bancoDeDados);
				}

				ObterSituacao(arquivo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter

		public List<SimuladorGeoArquivo> ObterArquivos(int projetoId, bool finalizado = false)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
				{
					return _da.ObterArquivos(projetoId, bancoDeDados, finalizado: finalizado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public void ObterSituacao(SimuladorGeoArquivo arquivo)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
				{
					_da.ObterSituacao(arquivo, bancoDeDados);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ObterSituacaoFila(SimuladorGeoArquivo arquivo)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
				{
					_da.ObterSituacaoFila(arquivo, bancoDeDados);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		public SimuladorGeo VerificarCpf(string cpf)
		{
			SimuladorGeo simualdor = new SimuladorGeo();
			
			try
			{
				if (!_validar.VerificarCpf(cpf))
				{
					return simualdor;
				}

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioPublicoGeo))
				{
					bancoDeDados.IniciarTransacao();

					simualdor = _da.Obter(cpf, bancoDeDados);

					if (simualdor == null || simualdor.Id == 0)
					{
						simualdor.Cpf = cpf;
						simualdor.MecanismoElaboracaoId = (int)eSimuladorGeoMecanismo.Simulador;
						simualdor.SituacaoId = (int)eSimuladorGeoSituacao.EmElaboracao;
						_da.Salvar(simualdor, bancoDeDados);
					}

					bancoDeDados.Commit();
				}

				simualdor.ArquivoEnviado = simualdor.Arquivos.FirstOrDefault(x => x.Tipo == (int)eSimuladorGeoArquivoTipo.ArquivoEnviado) ?? new SimuladorGeoArquivo();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return simualdor;
		}
	}
}