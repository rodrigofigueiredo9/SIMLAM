using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;
using System.Web;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
	internal class ImportarTituloDeclaratorioBus : ServicoTimerBase
	{
		#region Propriedades

		private ImportarTituloDeclaratorioDa _da;
		private ConfiguracaoSistema _configSys;
		private PessoaBus _busPessoa;
		private EmpreendimentoBus _empBus;
		private RequerimentoDa _daREq;
		private ProjetoDigitalCredenciadoBus _busProjetoDigitalCredenciado;

		public String UsuarioCredenciado
		{
			get { return _configSys.UsuarioCredenciado; }
		}

		public ImportarTituloDeclaratorioBus()
		{
			_configSys = new ConfiguracaoSistema();
			_da = new ImportarTituloDeclaratorioDa(_configSys.UsuarioInterno);
			_busPessoa = new PessoaBus();
			_empBus = new EmpreendimentoBus();
			_daREq = new RequerimentoDa();
			_busProjetoDigitalCredenciado = new ProjetoDigitalCredenciadoBus();
		}

		#endregion

		public void Teste()
		{
			Executar();
		}

		protected override void Executar()
		{
			List<ConfiguracaoServico> configuracoes = _da.Configuracoes(eServico.ImportarTituloDeclaratorio);

			Executor.Current = new Executor()
			{
				Id = 1 /*sistema*/,
				Login = "SVCImportarTituloDeclaratorio",
				Nome = GetType().Assembly.ManifestModule.Name,
				Tid = GetType().Assembly.ManifestModule.ModuleVersionId.ToString(),
				Tipo = eExecutorTipo.Interno
			};

			_da.TID = GerenciadorTransacao.ObterIDAtual();
			try
			{
				using (BancoDeDados bancoDeDadosInteno = BancoDeDados.ObterInstancia())
				{
					bancoDeDadosInteno.IniciarTransacao();

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDadosCredenciado.IniciarTransacao();

						GerenciarImportarTituloDeclaratorio(configuracoes, bancoDeDadosInteno, bancoDeDadosCredenciado);

						bancoDeDadosCredenciado.Commit();
					}
					bancoDeDadosInteno.Commit();
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		public void GerenciarImportarTituloDeclaratorio(List<ConfiguracaoServico> configuracoes, BancoDeDados bancoInterno, BancoDeDados bancoCredenciado)
		{
			using (BancoDeDados bancoDeDadosInterno = BancoDeDados.ObterInstancia(bancoInterno))
			{
				bancoDeDadosInterno.IniciarTransacao();

				using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(bancoCredenciado))
				{
					bancoDeDadosCredenciado.IniciarTransacao();

					ConfiguracaoServico configuracao = configuracoes.SingleOrDefault(x => x.Id == (int)eServico.ImportarTituloDeclaratorio) ?? new ConfiguracaoServico();

					DateTime inicio = configuracao.DataInicioExecucao ?? DateTime.MinValue;

					if (configuracao == null || configuracao.Id <= 0 || configuracao.EmExecucao || (DateTime.Now - inicio) < configuracao.Intervalo)
					{
						return;
					}

					configuracao.EmExecucao = true;

					_da.EditarConfiguracao(configuracao, bancoDeDadosInterno);

					List<Requerimento> requerimentos = _da.ObterRequerimentosCredenciado();

					foreach (var requerimento in requerimentos)
					{
						Importar(requerimento, bancoDeDadosCredenciado, bancoInterno);

						ImportarCaracterizacao(requerimento, bancoDeDadosCredenciado, bancoInterno);
					}

					configuracao.EmExecucao = false;

					_da.EditarConfiguracao(configuracao, bancoDeDadosInterno);
				}
			}
		}

		public void ImportarCaracterizacao(Requerimento requerimento, BancoDeDados bancoCredenciado, BancoDeDados bancoInterno)
		{
			ImportarCaracterizacaoCDLA(requerimento, bancoCredenciado, bancoInterno);
		}

		public void ImportarCaracterizacaoCDLA(Requerimento requerimento, BancoDeDados bancoCredenciado, BancoDeDados bancoInterno)
		{
			BarragemDispensaLicenca caracterizacao = _da.ImportarCaracterizacaoCdla(requerimento.Id, bancoCredenciado);

			Dictionary<Int32, String> _diretorioCred = _da.ObterCredenciadoDiretorioArquivo(bancoCredenciado);
			Dictionary<Int32, String> _diretorioTempCred = _da.ObterCredenciadoDiretorioArquivoTemp(bancoCredenciado);

			ArquivoBus _busArquivoCredenciado = new ArquivoBus(eExecutorTipo.Credenciado, _diretorioCred, _diretorioTempCred, UsuarioCredenciado);
			ArquivoBus _busArquivoInterno = new ArquivoBus(eExecutorTipo.Interno, usuarioCredenciado: UsuarioCredenciado);

			Arquivo aux = _busArquivoCredenciado.Obter(caracterizacao.Autorizacao.Id.Value);//Obtém o arquivo completo do diretorio do Institucional(nome, buffer, etc)

			aux.Id = 0;//Zera o ID
			aux = _busArquivoInterno.SalvarTemp(aux);//salva no diretório temporário
			aux = _busArquivoInterno.Copiar(aux);//Copia para o diretório oficial

			Executor executor = Executor.Current;

			ArquivoDa arquivoDa = new ArquivoDa();

			arquivoDa.Salvar(aux, executor.Id, executor.Nome, executor.Login, (int)eExecutorTipo.Interno, executor.Tid, bancoInterno);

			caracterizacao.Autorizacao.Id = aux.Id;

			_da.CopiarDadosCredenciado(caracterizacao, bancoInterno);
		}

		public int Importar(Requerimento requerimento, BancoDeDados bancoCredenciado, BancoDeDados bancoInterno)
		{
			BancoDeDados bancoDeDadosInterno = null;
			BancoDeDados bancoDeDadosCredenciado = null;
			int empreedimentoIdInterno = 0;

			RequerimentoCredenciadoBus busRequerimentoCredenciado = new RequerimentoCredenciadoBus();

			var requerimentoCredenciado = busRequerimentoCredenciado.Obter(requerimento.Id, bancoCredenciado,
				bancoInterno, true);

			requerimentoCredenciado.SetorId = 20; //Departamento de Recursos Naturais Renováveis

			requerimentoCredenciado.Empreendimento.SelecaoTipo = (int)eExecutorTipo.Credenciado;
			requerimentoCredenciado.Pessoas.ForEach(p =>
			{
				p.SelecaoTipo = (int)eExecutorTipo.Credenciado;
			});

			List<Pessoa> pessoasRelacionadas = ObterPessoasRelacionadas(requerimentoCredenciado, bancoCredenciado);

			requerimentoCredenciado.IsCredenciado = true;

			GerenciadorTransacao.ObterIDAtual();

			using (bancoDeDadosInterno = BancoDeDados.ObterInstancia(bancoInterno))
			{
				bancoDeDadosInterno.IniciarTransacao();

				using (bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
				{
					bancoDeDadosCredenciado.IniciarTransacao();

					requerimentoCredenciado = ImportarPessoas(requerimentoCredenciado, pessoasRelacionadas,
						bancoDeDadosInterno);

					if (requerimentoCredenciado.Empreendimento.Id > 0)
					{
						requerimentoCredenciado = ImportarEmpreendimento(requerimentoCredenciado, bancoDeDadosInterno,
							bancoDeDadosCredenciado);
					}

					_daREq.Importar(requerimentoCredenciado, bancoDeDadosInterno);

					busRequerimentoCredenciado.AlterarSituacao(
						new Requerimento()
						{
							Id = requerimentoCredenciado.Id,
							SituacaoId = (int)eRequerimentoSituacao.Importado
						}, bancoDeDadosCredenciado, bancoDeDadosInterno);

					AlterarSituacao(requerimentoCredenciado.Id, eProjetoDigitalSituacao.Finalizado, bancoDeDadosCredenciado);

					bancoDeDadosCredenciado.Commit();
				}

				bancoDeDadosInterno.Commit();
				Validacao.Add(Mensagem.ProjetoDigital.RequerimentoImportado(requerimento.Numero.ToString()));
			}

			return empreedimentoIdInterno;
		}

		internal void AlterarSituacao(int requerimentoId, eProjetoDigitalSituacao eProjetoDigitalSituacao, BancoDeDados bancoCredenciado = null)
		{
			ProjetoDigital projetoDigital = _busProjetoDigitalCredenciado.Obter(idRequerimento: requerimentoId, bancoCredenciado: bancoCredenciado);

			projetoDigital.Situacao = (int)eProjetoDigitalSituacao;

			_busProjetoDigitalCredenciado.AlterarSituacao(projetoDigital, banco: bancoCredenciado);
		}

		public Requerimento ImportarEmpreendimento(Requerimento requerimento, BancoDeDados bancoInterno, BancoDeDados bancoCredenciado)
		{
			return _empBus.Importar(requerimento, bancoInterno, bancoCredenciado);
		}

		public Requerimento ImportarPessoas(Requerimento requerimento, List<Pessoa> pessoasRelacionadas, BancoDeDados bancoInterno)
		{
			#region Fisica

			Pessoa aux;

			List<Pessoa> pessoas =
				requerimento.Pessoas.Where(x => x.IsFisica && pessoasRelacionadas.Exists(z => z.CPFCNPJ == x.CPFCNPJ))
					.ToList();

			pessoas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Credenciado).ToList().ForEach(r =>
			{
				_busPessoa.AlterarConjuge(_busPessoa.ObterId(r.CPFCNPJ, bancoInterno), bancoInterno);
			});

			pessoas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Credenciado).ToList().ForEach(r =>
			{
				aux = _busPessoa.Importar(r, bancoInterno, true);
				r.InternoId = aux.InternoId;
			});

			pessoas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Interno).ToList().ForEach(r =>
			{
				r.InternoId = _busPessoa.ObterId(r.CPFCNPJ, bancoInterno);
			});

			#endregion

			#region Juridica

			List<Pessoa> juridicas =
				requerimento.Pessoas.Where(x => x.IsJuridica && pessoasRelacionadas.Exists(z => z.CPFCNPJ == x.CPFCNPJ))
					.ToList();

			juridicas.SelectMany(x => x.Juridica.Representantes).ToList().ForEach(r =>
			{
				aux = pessoas.FirstOrDefault(y => y.CPFCNPJ == r.CPFCNPJ);
				if (aux != null && aux.InternoId.HasValue)
				{
					r.Id = aux.InternoId.GetValueOrDefault();
				}
			});

			juridicas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Credenciado).ToList().ForEach(r =>
			{
				aux = _busPessoa.Importar(r, bancoInterno);
				r.InternoId = aux.InternoId;
			});

			juridicas.Where(x => x.SelecaoTipo == (int)eExecutorTipo.Interno).ToList().ForEach(r =>
			{
				r.InternoId = _busPessoa.ObterId(r.CPFCNPJ, bancoInterno);
			});

			#endregion

			pessoas.AddRange(juridicas);

			if (pessoas != null && pessoas.Count > 0)
			{
				pessoas.Where(x => x.IsFisica && x.Fisica.ConjugeId > 0).ToList().ForEach(r =>
				{
					aux = pessoas.FirstOrDefault(y => y.CPFCNPJ == r.Fisica.ConjugeCPF);

					if (aux != null)
					{
						r.Fisica.ConjugeId = aux.InternoId.GetValueOrDefault();
						_busPessoa.AlterarConjugeEstadoCivil(r.InternoId.GetValueOrDefault(),
							r.Fisica.ConjugeId.Value, bancoInterno);
					}
				});

				requerimento.Interessado.Id =
					pessoas.Where(y => y.Id == requerimento.Interessado.Id).FirstOrDefault().InternoId.GetValueOrDefault();

				requerimento.Responsaveis.Where(x => x.Representantes != null)
					.SelectMany(z => z.Representantes).ToList().ForEach(r =>
					{
						r.Id = pessoas.Where(y => y.Id == r.Id).FirstOrDefault().InternoId.GetValueOrDefault();
					});

				requerimento.Responsaveis.ForEach(r =>
				{
					aux = pessoas.FirstOrDefault(y => y.CPFCNPJ == r.CpfCnpj);
					if (aux != null && aux.InternoId.HasValue)
					{
						r.Id = aux.InternoId.GetValueOrDefault();
					}
				});
			}

			requerimento.Pessoas = pessoas;

			return requerimento;
		}

		public List<Pessoa> ObterPessoasRelacionadas(Requerimento requerimento, BancoDeDados bancoCredenciado)
		{
			List<Pessoa> pessoasRelacionadas = new List<Pessoa>();

			List<string> cpfCnpj =
				requerimento.Responsaveis.Where(x => x.Representantes != null)
					.SelectMany(x => x.Representantes)
					.ToList()
					.Select(x => x.CPFCNPJ)
					.ToList();

			pessoasRelacionadas.AddRange(
				requerimento.Pessoas.Where(x => cpfCnpj.Exists(y => x.IsFisica && y == x.CPFCNPJ)));

			pessoasRelacionadas.Add(
				requerimento.Pessoas.Where(x => x.CPFCNPJ == requerimento.Interessado.CPFCNPJ).FirstOrDefault());

			requerimento.Responsaveis.ForEach(r =>
			{
				pessoasRelacionadas.Add(requerimento.Pessoas.Where(x => x.CPFCNPJ == r.CpfCnpj).FirstOrDefault());
			});

			if (requerimento.Empreendimento.SelecaoTipo == (int)eExecutorTipo.Credenciado &&
				requerimento.Empreendimento.Id > 0)
			{
				EmpreendimentoCredenciadoBus empreendimentoCredenciadoBus = new EmpreendimentoCredenciadoBus();
				pessoasRelacionadas.AddRange(
					empreendimentoCredenciadoBus.ObterResponsaveis(requerimento.Empreendimento.Id, bancoCredenciado));
			}

			//Conjuge
			foreach (var item in requerimento.Pessoas.Where(x => x.Fisica.ConjugeId > 0).ToList())
			{
				if (item.SelecaoTipo == (int)eExecutorTipo.Credenciado)
				{
					pessoasRelacionadas.Add(requerimento.Pessoas.FirstOrDefault(x => x.CPFCNPJ == item.Fisica.ConjugeCPF));
				}
			}

			//Agrupando
			pessoasRelacionadas =
				pessoasRelacionadas.Where(x => x != null).GroupBy(x => x.CPFCNPJ).Select(y => new Pessoa
				{
					Id = y.First().Id,
					Tipo = y.First().Tipo,
					InternoId = y.First().InternoId,
					NomeRazaoSocial = y.First().NomeRazaoSocial,
					CPFCNPJ = y.First().CPFCNPJ,
					Fisica = y.First().Fisica,
				}).ToList();

			return pessoasRelacionadas;
		}
	}
}