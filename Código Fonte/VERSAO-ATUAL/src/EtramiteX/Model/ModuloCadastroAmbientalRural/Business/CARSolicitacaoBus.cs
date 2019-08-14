using Interno.Model.WebService.ModuloWSSicar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
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
		ProtocoloDa _protocoloDa = new ProtocoloDa();

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
				if (_validar.RetificacaoValidar(entidade))
				{
					if (_validar.Salvar(entidade))
					{
						GerenciadorTransacao.ObterIDAtual();

						using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
						{
							bancoDeDados.IniciarTransacao();

							entidade.AutorId = User.FuncionarioId;

							_da.Salvar(entidade, bancoDeDados);

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
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(CARSolicitacao entidade, BancoDeDados banco = null, bool isTitulo = false, EtramiteIdentity funcionario = null)
		{
			try
			{
				bool IsCredenciado = false;
				ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
				ArquivoDa _arquivoDa = new ArquivoDa();

				entidade = obterDados(entidade);

				if (!isTitulo)
				{
					entidade.ArquivoCancelamento = _busArquivo.Copiar(entidade.ArquivoCancelamento);
					entidade.ArquivoCancelamento = _busArquivo.ObterTemporario(entidade.ArquivoCancelamento);
				}

				//passivo arrumado
				GerenciadorTransacao.ObterIDAtual();

				if (_validar.AlterarSituacao(entidade, funcionario.FuncionarioId, isTitulo))
				{
					AlterarSituacaoSicar(entidade);

					if (!Validacao.EhValido) return false;

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

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();
						if (!IsCredenciado)
						{
							_da.AlterarSituacao(entidade, bancoDeDados);
							_da.FazerVirarPassivo(entidade.Id, bancoDeDados);
						}

						if (!isTitulo && Validacao.EhValido)
						{
							int situacaoArquivo = ObterNovaSituacaoArquivo(entidade.SituacaoId, entidade.SituacaoAnteriorId);
							_da.AlterarSituacaoArquivoSicar(entidade, situacaoArquivo, bancoDeDados);

							_arquivoDa.Salvar(entidade.ArquivoCancelamento, funcionario.FuncionarioId, funcionario.Name, funcionario.Login, (int)eExecutorTipo.Interno, funcionario.FuncionarioTid);
							_da.InserirAlterarSituacaoLista(entidade, bancoDeDados);
							Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacao);
						}
						if(Validacao.EhValido)
							bancoDeDados.Commit();
						else
							bancoDeDados.Rollback();

					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;



			CARSolicitacao obterDados(CARSolicitacao solicitacaoCar)
			{
				CARSolicitacao solicitacaoAtual = Obter(solicitacaoCar.Id, simplificado: true) ?? new CARSolicitacao();

				if (solicitacaoAtual.Id == 0)
					solicitacaoAtual = _busCredenciado.Obter(solicitacaoCar.Id, simplificado: true);

				solicitacaoCar.SituacaoAnteriorId = solicitacaoAtual.SituacaoId;
				solicitacaoCar.DataSituacaoAnterior = solicitacaoAtual.DataSituacao;
				solicitacaoCar.Protocolo = solicitacaoAtual.Protocolo;
				solicitacaoCar.AutorCpf = solicitacaoAtual.AutorCpf;
				solicitacaoCar.SICAR.CodigoImovel = solicitacaoAtual.SICAR.CodigoImovel;
				solicitacaoCar.AutorCancelamento = new FuncionarioBus().Obter(funcionario?.FuncionarioId ?? 0);

				return solicitacaoCar;
			}

			void AlterarSituacaoSicar(CARSolicitacao solicitacao)
			{
				SicarAnaliseRetornoDTO resultado = new SicarAnaliseRetornoDTO();
				switch (solicitacao.SituacaoId)
				{
					case (int)eCARSolicitacaoSituacao.Valido:
						if (solicitacao.SituacaoAnteriorId == (int)eCARSolicitacaoSituacao.Suspenso)
							resultado = CarAnaliseService.ReverterSuspensao(solicitacao);
						else
						{
							solicitacao.Status = eStatusImovelSicar.Ativo;
							resultado = CarAnaliseService.AlterarSituacaoSicar(solicitacao);
						}
						break;

					case (int)eCARSolicitacaoSituacao.Invalido:
						solicitacao.Status = eStatusImovelSicar.Cancelado;
						resultado = CarAnaliseService.AlterarSituacaoSicar(solicitacao);
						break;

					case (int)eCARSolicitacaoSituacao.Suspenso:
						resultado = CarAnaliseService.SolicitarSuspensao(solicitacao);
						break;

					default:
						break;
				}

				if (resultado?.codigoResposta == 400)
				{
					if (resultado.mensagensResposta?.Count > 0)
						resultado.mensagensResposta.ForEach(mensagem =>
						{
							Validacao.Add(eTipoMensagem.Advertencia, mensagem);
						});
				}
			}
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

		public void EnviarReenviarArquivoSICAR(CARSolicitacao solicitacao, bool isEnviar, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.InserirFilaArquivoCarSicar(solicitacao, eCARSolicitacaoOrigem.Institucional, bancoDeDados);

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
				return (tid == null) ? _da.Obter(id, simplificado, banco) : _da.ObterHistorico(id, tid, simplificado, banco);
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
						.Where(x => int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Invalido
								|| int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Suspenso).ToList();

				case eCARSolicitacaoSituacao.Suspenso:
					return listaBus.CadastroAmbientalRuralSolicitacaoSituacao
						.Where(x => int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Valido
								|| int.Parse(x.Id) == (int)eCARSolicitacaoSituacao.Invalido).ToList();

				default:
					return new List<Lista>();
			}
		}

		public string ObterCodigoSicarPorEmpreendimento(int empreendimentoId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterCodigoSicarPorEmpreendimento(empreendimentoId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return "";
		}

		public List<CARCancelamento> ObterListaCancelamentoCar(int solicitacaoId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterListaCancelamentoCar(solicitacaoId, banco);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return new List<CARCancelamento>();
		}

		private int ObterNovaSituacaoArquivo(int situacaoNova, int situacaoAnterior)
		{
			if (situacaoAnterior == (int)eCARSolicitacaoSituacao.Valido && situacaoNova == (int)eCARSolicitacaoSituacao.Invalido)
				return  (int)eStatusArquivoSICAR.Cancelado;
			else if (situacaoAnterior == (int)eCARSolicitacaoSituacao.Valido && situacaoNova == (int)eCARSolicitacaoSituacao.Suspenso)
				return (int)eStatusArquivoSICAR.ArquivoEntregue;
			else
				return (int)eStatusArquivoSICAR.ArquivoReprovado;
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

		public bool ValidarVisualizarAlterarSituacao(CARSolicitacao entidade, int funcionarioId)
		{
			if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Invalido ||
				entidade.SituacaoId == (int)eCARSolicitacaoSituacao.EmCadastro ||
				entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Nulo ||
				entidade.SituacaoId == (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR ||
				entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Suspenso)
				return true;

			if (_validar.validarFuncionario(funcionarioId, (int)ePermissao.CadastroAmbientalRuralSolicitacaoInvalida))
				return false;
			else
			{
				if (!(entidade.Protocolo.Id.GetValueOrDefault() > 0 && _protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault())))
					return true;
				else
				{
					if (entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Pendente)
						return true;
				}
			}

			return false;
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
			if (urlGerar == null) return null;

			RequestJson requestJson = new RequestJson();

			var strResposta = requestJson.Executar(urlGerar);

			var resposta = requestJson.Deserializar<dynamic>(strResposta);

			if (resposta["status"] != "s")
			{
				return string.Empty;
			}
			//return UrlSICAR + resposta["dados"]; //PRODUCAO
			return "http://homolog-car.mma.gov.br" + resposta["dados"]; // HOMOLOG
		}

		public string ObterUrlDemonstrativo(int solicitacaoId, int schemaSolicitacao, bool isTitulo)
		{
			try
			{
				var urlGerar = _da.ObterUrlGeracaoDemonstrativo(solicitacaoId, schemaSolicitacao, isTitulo) ?? "";
				if (String.IsNullOrWhiteSpace(urlGerar))
					throw new Exception();

			RequestJson requestJson = new RequestJson();

            urlGerar = "http://www.car.gov.br/pdf/demonstrativo/" + urlGerar + "/gerar"; 
            //urlGerar = "http://homolog-car.mma.gov.br/pdf/demonstrativo/" + urlGerar + "/gerar";

            var strResposta = requestJson.Executar(urlGerar);

            var resposta = requestJson.Deserializar<dynamic>(strResposta);

            if (resposta["status"] != "s")
            {
                return string.Empty;
            }

				//return UrlSICAR + resposta["dados"];  // PRODUCAO 
				return "http://homolog-car.mma.gov.br" + resposta["dados"]; // HOMOLOG 
			}
			catch (Exception ex)
			{
				Validacao.Add(Mensagem.CARSolicitacao.ErroPdfDemonstrativo);
				return string.Empty;
			}
		}

		public object ObterIdAquivoSICAR(int id, int schemaSolicitacao)
		{
			throw new NotImplementedException();
		}
		internal bool VerificarSeEmpreendimentoPossuiSolicitacaoEmCadastro(int empreendimentoID)
		{
			return _da.VerificarSeEmpreendimentoPossuiSolicitacaoEmCadastro(empreendimentoID);

		}
		internal bool VerificarSeEmpreendimentoPossuiSolicitacaoValidaEEnviada(int empreendimentoID)
		{
			return _da.VerificarSeEmpreendimentoPossuiSolicitacaoValidaEEnviada(empreendimentoID);

		}

	}
}