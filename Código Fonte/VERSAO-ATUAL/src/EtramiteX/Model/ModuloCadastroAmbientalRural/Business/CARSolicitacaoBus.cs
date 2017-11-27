using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using CARSolicitacaoCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business.CARSolicitacaoBus;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business
{
	public class CARSolicitacaoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		CARSolicitacaoValidar _validar = null;
		ProtocoloBus _busProtocolo = new ProtocoloBus();
		CARSolicitacaoCredenciadoBus _busCredenciado = new CARSolicitacaoCredenciadoBus();
		CARSolicitacaoDa _da = null;

		String UrlSICAR
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUrlSICAR); }
		}

		public CARSolicitacaoValidar Validar { get { return _validar; } }

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public CARSolicitacaoBus() : this(new CARSolicitacaoValidar()) { }

		public CARSolicitacaoBus(CARSolicitacaoValidar validar)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_validar = validar;
			_da = new CARSolicitacaoDa();
		}

		#region Comandos DML

		public bool Salvar(CARSolicitacao entidade)
		{
			try
			{
			    if (_validar.Salvar(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

                    //função não esta sendo usada, pois implementamos outra soluçao no scheduler; arquivo: gerarArquivoCarJob; função: obterDadosReservaLegal;
                    //var verificar = _da.VerificaSolicitacaoCedente(entidade.Empreendimento.Id);
                    var verificar = false;
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						entidade.AutorId = User.FuncionarioId;

						_da.Salvar(entidade, bancoDeDados, verificar);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoSalvarTopico1(entidade.Id.ToString()));
						Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoSalvarTopico2);
						Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoSalvarTopico3);
					}

                    #region Carga das tabelas APP Caculada e APP Escadinha
                    var qtdModuloFiscal = 0.0;
                    using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                    {
                        #region Select QTD Modulo Fiscal
                        Comando comando = bancoDeDados.CriarComando(@"SELECT ATP_QTD_MODULO_FISCAL FROM CRT_CAD_AMBIENTAL_RURAL WHERE EMPREENDIMENTO = :empreendimentoID");//, EsquemaBanco);

                        comando.AdicionarParametroEntrada("empreendimentoID", entidade.Empreendimento.Id, DbType.Int32);

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
                    using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia("idafgeo"))
                    {
                        #region Chamada Procedure
                        bancoDeDados.IniciarTransacao();
                        Comando command = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularAppClassificadaCAR(:emp); end;");

                        command.AdicionarParametroEntrada("emp", entidade.Empreendimento.Id, System.Data.DbType.Int32);

                        bancoDeDados.ExecutarNonQuery(command);
                        bancoDeDados.Commit();

                        bancoDeDados.IniciarTransacao();
                        Comando com = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularEscadinhaCAR(:emp, :moduloFiscal); end;");

                        com.AdicionarParametroEntrada("emp", entidade.Empreendimento.Id, System.Data.DbType.Int32);
                        com.AdicionarParametroEntrada("moduloFiscal", qtdModuloFiscal, System.Data.DbType.Double);

                        bancoDeDados.ExecutarNonQuery(com);
                        bancoDeDados.Commit();

                        bancoDeDados.IniciarTransacao();
                        command = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularArlTotalCAR(:emp); end;");

                        command.AdicionarParametroEntrada("emp", entidade.Empreendimento.Id, System.Data.DbType.Int32);

                        bancoDeDados.ExecutarNonQuery(command);

                        bancoDeDados.Commit();
                        #endregion

                    }
                    #endregion
            
				}
		    }
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(CARSolicitacao entidade, BancoDeDados banco = null, bool mostrarMsg = true)
		{
			try
			{
				bool IsCredenciado = false;
				CARSolicitacao solicitacaoAtual = Obter(entidade.Id) ?? new CARSolicitacao();

				if (solicitacaoAtual.Id == 0)
				{
					solicitacaoAtual = _busCredenciado.Obter(entidade.Id);
					IsCredenciado = true;
				}

				entidade.SituacaoAnteriorId = solicitacaoAtual.SituacaoId;
				entidade.DataSituacaoAnterior = solicitacaoAtual.DataSituacao;
				entidade.Protocolo = solicitacaoAtual.Protocolo;

				//passivo arrumado
				GerenciadorTransacao.ObterIDAtual();

				if (IsCredenciado)
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_busCredenciado.AlterarSituacao(new CARSolicitacao() { Id = entidade.Id }, entidade, bancoDeDados);
						_busCredenciado.FazerVirarPassivo(entidade.Id, bancoDeDados);
						bancoDeDados.Commit();
					}
				}
				else
				{
					if (_validar.AlterarSituacao(entidade))
					{
						using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
						{
							bancoDeDados.IniciarTransacao();

							_da.AlterarSituacao(entidade, bancoDeDados);
							_da.FazerVirarPassivo(entidade.Id, bancoDeDados);

							bancoDeDados.Commit();
						}
					}
				}

				if (mostrarMsg)
				{
					Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacao);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		internal void SubstituirPorTituloCARCredenciado(int empreendimentoInstitucionalID, BancoDeDados banco = null)
		{
			_da.SubstituirPorTituloCARCredenciado(empreendimentoInstitucionalID, banco);
			_da.FazerVirarPassivoCredenciado(empreendimentoInstitucionalID, banco);
			_da.AtualizarLstConsultaCredenciado(empreendimentoInstitucionalID, banco);
		}


		public bool Excluir(int id, BancoDeDados banco = null)
		{
			try
			{
				CARSolicitacao entidade = Obter(id, true);

				if (_validar.Excluir(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoExcluir);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void EnviarReenviarArquivoSICAR(int solicitacaoId, bool isEnviar, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.InserirFilaArquivoCarSicar(solicitacaoId, eCARSolicitacaoOrigem.Institucional, bancoDeDados);

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

		public CARSolicitacao Obter(int id, bool simplificado = false, string tid = null, BancoDeDados banco = null)
		{
			try
			{
				if (tid == null)
				{
					return _da.Obter(id, simplificado, banco);
				}
				else
				{
					return _da.ObterHistorico(id, tid, simplificado, banco);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public CARSolicitacao ObterHistorico(int id, bool simplificado = false, string tid = null, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterHistorico(id, tid, simplificado, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public CARSolicitacao ObterPorEmpreendimento(int empreendimentoId, List<int> situacoes = null, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterPorEmpreendimento(empreendimentoId, situacoes, simplificado, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public CARSolicitacao ObterHistoricoUltimaSituacao(int id, eCARSolicitacaoSituacao situacao, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterHistoricoUltimaSituacao(id, situacao, banco);
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

		public Resultados<SolicitacaoListarResultados> Filtrar(SolicitacaoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
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

		public List<Lista> ObterSituacoes(int situacao)
		{
			ListaBus listaBus = new ListaBus();

			switch ((eCARSolicitacaoSituacao)situacao)
			{
				case eCARSolicitacaoSituacao.Pendente:
					return listaBus.CadastroAmbientalRuralSolicitacaoSituacao
						.Where(x => int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Invalido).ToList();

				case eCARSolicitacaoSituacao.Valido:
					return listaBus.CadastroAmbientalRuralSolicitacaoSituacao
						.Where(x => int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Invalido).ToList();

				case eCARSolicitacaoSituacao.Suspenso:
					return listaBus.CadastroAmbientalRuralSolicitacaoSituacao
						.Where(x => int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Valido || int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Invalido).ToList();

				default:
					return new List<Lista>();
			}
		}

		#endregion

		#region Auxiliares

		public List<PessoaLst> ObterResponsaveis(int empreendimentoId)
		{
			return _da.ObterResponsaveis(empreendimentoId);
		}

		#endregion Auxiliares

		#region Validacoes

		public bool ValidarAssociar(IProtocolo protocolo, int funcionarioId)
		{
			if (!_busProtocolo.EmPosse(protocolo.Id.GetValueOrDefault(0), funcionarioId))
			{
				if (protocolo.IsProcesso)
				{
					Validacao.Add(Mensagem.Processo.PosseProcessoNecessaria);
				}
				else
				{
					Validacao.Add(Mensagem.Documento.PosseDocumentoNecessaria);
				}

				return Validacao.EhValido;
			}


			if (protocolo.Requerimento.Id <= 0)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoRequerimentoObrigatorio);
			}


			if (protocolo.Empreendimento.Id <= 0)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool ExisteProtocoloAssociado(int protocolo, int associado)
		{
			try
			{
				return _da.ExisteProtocoloAssociado(protocolo, associado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool ExisteCredenciado(int solicitacaoId)
		{
			try
			{
				return _da.ExisteCredenciado(solicitacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public String ObterSituacaoTituloCARExistente(int empreendimentoId)
		{
			try
			{
				return _da.ObterSituacaoTituloCARExistente(empreendimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return String.Empty;
		}

		#endregion

		public string ObterUrlRecibo(int solicitacaoId, int schemaSolicitacao)
		{
			var urlGerar = _da.ObterUrlGeracaoRecibo(solicitacaoId, schemaSolicitacao);

			RequestJson requestJson = new RequestJson();

			var strResposta = requestJson.Executar(urlGerar);

			var resposta = requestJson.Deserializar<dynamic>(strResposta);

			if (resposta["status"] != "s")
			{
				return string.Empty;
			}

			return UrlSICAR + resposta["dados"];
		}

		public object ObterIdAquivoSICAR(int id, int schemaSolicitacao)
		{
			throw new NotImplementedException();
		}

		internal bool VerificarSeEmpreendimentoPossuiSolicitacaoValidaEEnviada(int empreendimentoID)
		{
			return _da.VerificarSeEmpreendimentoPossuiSolicitacaoValidaEEnviada(empreendimentoID);

		}
	}
}