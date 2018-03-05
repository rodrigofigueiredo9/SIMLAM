using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business
{
	public class CARSolicitacaoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		CARSolicitacaoValidar _validar = null;
		ProjetoDigitalCredenciadoBus _busProjetoDigital = null;
		RequerimentoCredenciadoBus _busRequerimento = null;
		CARSolicitacaoDa _da = null;
		CARSolicitacaoInternoDa _daInterno = null;
		AtividadeCredenciadoBus _busAtividade = null;
		ConsultaCredenciado _consultaCredenciado = null;

		public CARSolicitacaoValidar Validar { get { return _validar; } }
		internal ConsultaCredenciado ConsultaCredenciado { get { return _consultaCredenciado; } }

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public CARSolicitacaoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new CARSolicitacaoDa();
			_daInterno = new CARSolicitacaoInternoDa();
			_busProjetoDigital = new ProjetoDigitalCredenciadoBus();
			_busRequerimento = new RequerimentoCredenciadoBus();
			_busAtividade = new AtividadeCredenciadoBus();
			_validar = new CARSolicitacaoValidar();
			_consultaCredenciado = new ConsultaCredenciado();
		}

		#region Comandos DML

		public bool Salvar(CARSolicitacao carSolicitacao)
		{
			try
			{
                if(_validar.RetificacaoValidar(carSolicitacao, 1))
                {
                    if (_validar.Salvar(carSolicitacao))
                    {
                        GerenciadorTransacao.ObterIDAtual();
                        carSolicitacao.Id = _daInterno.ObterNovoID();

                        //carSolicitacao.Declarante.Id = User.FuncionarioId;                  

                        using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
                        {
                            bancoDeDados.IniciarTransacao();

                            _da.Salvar(carSolicitacao, bancoDeDados);

                            ConsultaCredenciado.Gerar(carSolicitacao.Id, eHistoricoArtefato.carsolicitacao, bancoDeDados);

                            bancoDeDados.Commit();

                            Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoSalvarTopico1(carSolicitacao.Id.ToString()));
                            Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoSalvarTopico2);
                            Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoSalvarTopico3);
                        }

                        #region Carga das tabelas APP Caculada e APP Escadinha
                        var qtdModuloFiscal = 0.0;
                        using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
                        {
                            #region Select QTD Modulo Fiscal
                            //Comando comando = bancoDeDados.CriarComando(@"SELECT ATP_QTD_MODULO_FISCAL FROM CRT_CAD_AMBIENTAL_RURAL WHERE EMPREENDIMENTO = :empreendimentoID");
                            Comando comando = bancoDeDados.CriarComando(@"
                        select  
                           round((SELECT (t.croqui_area/10000) FROM idafcredenciado.CRT_DOMINIALIDADE t WHERE t.empreendimento = car_sol.EMPREENDIMENTO) /
                              (SELECT m.modulo_ha FROM idaf.CNF_MUNICIPIO_MOD_FISCAL m WHERE m.municipio = (SELECT e.municipio FROM
                              idafcredenciado.TAB_EMPREENDIMENTO_ENDERECO e WHERE e.empreendimento = car_sol.EMPREENDIMENTO AND e.correspondencia = 0)),2) as ATP_QTD_MODULO_FISCAL    
                        from idafcredenciado.TAB_CAR_SOLICITACAO car_sol, idafcredenciado.CRT_PROJETO_GEO g
                        where car_sol.SITUACAO NOT IN (3) --Em Cadastro (1) e Inválidos (3) );
                        and car_sol.EMPREENDIMENTO = g.EMPREENDIMENTO
                        and g.empreendimento = :empreendimentoID
                        and g.caracterizacao = 1");
                            comando.AdicionarParametroEntrada("empreendimentoID", carSolicitacao.Empreendimento.Id, DbType.Int32);

                            using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                            {
                                while (reader.Read())
                                {
                                    qtdModuloFiscal = Convert.ToDouble(reader["ATP_QTD_MODULO_FISCAL"]);
                                }

                                reader.Close();
                            }
                            #endregion
                        }
                        using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia("idafcredenciadogeo"))
                        {
                            #region Chamada Procedure
                            bancoDeDados.IniciarTransacao();
                            Comando command = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularAppClassificadaCAR(:emp); end;");

                            command.AdicionarParametroEntrada("emp", carSolicitacao.Empreendimento.Id, System.Data.DbType.Int32);

                            bancoDeDados.ExecutarNonQuery(command);

                            bancoDeDados.Commit();

                            bancoDeDados.IniciarTransacao();
                            Comando com = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularEscadinhaCAR(:emp, :moduloFiscal); end;");

                            com.AdicionarParametroEntrada("emp", carSolicitacao.Empreendimento.Id, System.Data.DbType.Int32);
                            com.AdicionarParametroEntrada("moduloFiscal", qtdModuloFiscal, System.Data.DbType.Double);

                            bancoDeDados.ExecutarNonQuery(com);

                            bancoDeDados.Commit();

                            bancoDeDados.IniciarTransacao();
                            command = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularArlTotalCAR(:emp); end;");

                            command.AdicionarParametroEntrada("emp", carSolicitacao.Empreendimento.Id, System.Data.DbType.Int32);

                            bancoDeDados.ExecutarNonQuery(command);

                            bancoDeDados.Commit();
                            #endregion

                        }
                        #endregion
                    }
                }
				
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public void AlterarSituacao(CARSolicitacao filtro, CARSolicitacao carSolicitacao, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.AlterarSituacao(carSolicitacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						List<CARSolicitacao> resultados = _da.ObterCARSolicitacoes(filtro, bancoDeDados);
						
						if (resultados != null && resultados.Count > 0)
						{
							foreach (var item in resultados.Where(x => x.SituacaoId == (int)eCARSolicitacaoSituacao.Valido || x.SituacaoId == (int)eCARSolicitacaoSituacao.Suspenso))
							{
								if (item.SituacaoId == (int)carSolicitacao.SituacaoId)
								{
									continue;
								}

								_da.AlterarSituacao(new CARSolicitacao() { Id = item.Id, SituacaoId = (int)carSolicitacao.SituacaoId, Motivo = carSolicitacao.Motivo }, bancoDeDados);
								ConsultaCredenciado.Gerar(item.Id, eHistoricoArtefato.carsolicitacao, bancoDeDados);
							}
						}

						bancoDeDados.Commit();
					}
				}				
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarSituacao___QueFazVirarPassivo(CARSolicitacao filtro, CARSolicitacao carSolicitacao, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.AlterarSituacao(carSolicitacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						List<CARSolicitacao> resultados = _da.ObterCARSolicitacoes(filtro, bancoDeDados);

						if (resultados != null && resultados.Count > 0)
						{
                            foreach (var item in resultados.Where(x => x.SituacaoId == (int)eCARSolicitacaoSituacao.Valido || x.SituacaoId == (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR  || x.SituacaoId == (int)eCARSolicitacaoSituacao.Suspenso))
							{
								if (item.SituacaoId == (int)carSolicitacao.SituacaoId)
								{
									continue;
								}

								_da.AlterarSituacao(new CARSolicitacao() { Id = item.Id, SituacaoId = (int)carSolicitacao.SituacaoId, Motivo = carSolicitacao.Motivo }, bancoDeDados);
								_da.FazerVirarPassivo(item.Id, bancoDeDados);
								ConsultaCredenciado.Gerar(item.Id, eHistoricoArtefato.carsolicitacao, bancoDeDados);
							}
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}


		public void AssociarProtocolo(CARSolicitacao carSolicitacao, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					carSolicitacao.SituacaoId = (int)eCARSolicitacaoSituacao.Suspenso;
					List<CARSolicitacao> resultados = _da.ObterCARSolicitacoes(carSolicitacao, bancoDeDados);

					if (resultados != null && resultados.Count > 0)
					{
						foreach (var item in resultados)
						{
							if (!_da.AlteradoSituacaoInterno(item.Id))
							{
								_da.AlterarSituacao(new CARSolicitacao() { Id = item.Id, SituacaoId = (int)eCARSolicitacaoSituacao.Valido }, bancoDeDados, eHistoricoAcao.associar);

								ConsultaCredenciado.Gerar(item.Id, eHistoricoArtefato.carsolicitacao, bancoDeDados);
							}
						}
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void DesassociarProtocolo(CARSolicitacao carSolicitacao, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					carSolicitacao.SituacaoId = (int)eCARSolicitacaoSituacao.Valido;
					List<CARSolicitacao> resultados = _da.ObterCARSolicitacoes(carSolicitacao, bancoDeDados);

					if (resultados != null && resultados.Count > 0)
					{
						foreach (var item in resultados)
						{
							CARSolicitacao carSolicitacaoHistorico = _da.ObterUltimoHistoricoSituacao(item.Id, eCARSolicitacaoSituacao.Suspenso, bancoDeDados);

							if (carSolicitacaoHistorico.Id < 1)
							{
								continue;
							}

							if (!_da.AlteradoSituacaoInterno(item.Id))
							{
								_da.AlterarSituacao(
								new CARSolicitacao()
								{
									Id = item.Id,
									SituacaoId = carSolicitacaoHistorico.SituacaoId,
									Motivo = carSolicitacaoHistorico.Motivo
								},
								bancoDeDados, eHistoricoAcao.desassociar);

								ConsultaCredenciado.Gerar(item.Id, eHistoricoArtefato.carsolicitacao, bancoDeDados);
							}
						}
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}
        
        public void EnviarReenviarArquivoSICAR(CARSolicitacao solicitacao, bool isEnviar, BancoDeDados banco = null)
        {
            try
            {
                GerenciadorTransacao.ObterIDAtual();

                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
                {
                    bancoDeDados.IniciarTransacao();

                    _da.InserirFilaArquivoCarSicar(solicitacao, eCARSolicitacaoOrigem.Credenciado, bancoDeDados);

                    bancoDeDados.Commit();

                    Validacao.Add(Mensagem.CARSolicitacao.SucessoEnviarReenviarArquivoSICAR(isEnviar));
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
        }

		#endregion

		#region Obter

		public CARSolicitacao ObterPorRequerimento(int requerimentoID, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterPorRequerimento(requerimentoID, simplificado, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterDeclarantesLst(int requerimentoId)
		{
			List<PessoaLst> pessoas = new List<PessoaLst>();
			try
			{
				pessoas = _da.ObterDeclarantes(requerimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return pessoas;
		}

		public List<Lista> ObterAtividadesLista(int requerimentoId)
		{
			List<Lista> atividades = new List<Lista>();
			try
			{
				atividades = _busAtividade.ObterAtividadesLista(requerimentoId).Where(x => Convert.ToInt32(x.Codigo) == (int)eAtividadeCodigo.CadastroAmbientalRural).ToList();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return atividades;
		}

		public Resultados<SolicitacaoListarResultados> Filtrar(SolicitacaoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				filtrosListar.AutorCPFCNPJ = new CredenciadoBus().Obter(User.FuncionarioId, simplificado: true).Pessoa.CPFCNPJ;
				Filtro<SolicitacaoListarFiltro> filtro = new Filtro<SolicitacaoListarFiltro>(filtrosListar, paginacao);

				Resultados<SolicitacaoListarResultados> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public CARSolicitacao Obter(int id, BancoDeDados banco = null)
		{
            //Chamada tambem do botão enviar para SICAR
			try
			{
                //Consulta no credenciado, se vier nulo, consulta no institucional
                var onCred = _da.Obter(id, banco);
                if(onCred.AutorId== null || onCred.Id == null || onCred.AutorId == 0 || onCred.Id == 0)
                {
                    return _da.ObterOnInstitucional(id, banco);
                }
                return onCred;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;

		}

		public CARSolicitacao ObterHistoricoPrimeiraSituacao(int id, eCARSolicitacaoSituacao situacao, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterHistoricoPrimeiraSituacao(id, situacao, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public CARSolicitacao ObterPorProjetoDigital(int projetoDigitalId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterPorProjetoDigital(projetoDigitalId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Validacao

		public string EmpreendimentoPossuiSolicitacao(int empreendimento, BancoDeDados banco = null)
		{
			try
			{
				return _da.EmpreendimentoPossuiSolicitacao(empreendimento, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public string EmpreendimentoPossuiSolicitacao(string cnpj, BancoDeDados banco = null)
		{
			try
			{
				return _da.EmpreendimentoPossuiSolicitacao(cnpj, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		public void FazerVirarPassivo(int solicitacaoID, BancoDeDados banco = null)
		{
			_da.FazerVirarPassivo(solicitacaoID, banco);

			
		}
	}
}