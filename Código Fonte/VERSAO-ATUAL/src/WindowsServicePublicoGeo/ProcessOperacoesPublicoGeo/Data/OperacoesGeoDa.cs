using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.Data
{
	public class OperacoesGeoDa
	{
		internal const int FILA_SITUACAO_AGUARDANDO = 1;
		internal const int FILA_SITUACAO_EXECUTANTO = 2;
		internal const int FILA_SITUACAO_ERRO = 3;
		internal const int FILA_SITUACAO_CONCLUIDO = 4;

		internal const int VALIDACAO_ERRO_ESPACIAL = 1;
		internal const int VALIDACAO_OBRIGATORIEDADE_NAO_ATENDIDA = 2;
		internal const int VALIDACAO_ATRIBUTO_INVALIDO = 3;
		internal const int VALIDACAO_CONTABILIZACAO_DE_GEOMETRIAS = 4;

		internal const int OPERACAO_BASEREF_INTERNA = 1;
		internal const int OPERACAO_BASEREF_GEOBASES = 2;
		internal const int OPERACAO_CADASTRO_PROPRIEDADE = 3;
		internal const int OPERACAO_ATIVIDADE = 4;

		private string _esquemaOficial = "";
		private Dictionary<Int32, String> _lstDiretorio;

		public BancoDeDados banco { get; set; }

		public OperacoesGeoDa(BancoDeDados banco)
		{
			this.banco = banco;
		}

		internal string EsquemaOficial
		{
			get
			{
				if (String.IsNullOrEmpty(_esquemaOficial))
				{
					string strSQL = @"begin select to_char(c.valor) into :ESQUEMA_OFICIAL from cnf_sistema_geo c where c.campo = 'ESQUEMA_OFICIAL'; end;";
					using (Comando comando = this.banco.CriarComando(strSQL))
					{
						comando.AdicionarParametroSaida("ESQUEMA_OFICIAL", DbType.String, 100);
						this.banco.ExecutarNonQuery(comando);

						string valor = comando.ObterValorParametro("ESQUEMA_OFICIAL").ToString();
						_esquemaOficial = String.IsNullOrWhiteSpace(valor) ? " " : valor;
					}
				}

				return _esquemaOficial;
			}
		}

		private string EsquemaOficialComPonto
		{
			get { return (EsquemaOficial != " ") ? EsquemaOficial + "." : ""; }
		}

		internal Dictionary<Int32, String> DiretorioArquivo
		{
			get
			{
				if (_lstDiretorio == null)
				{
					_lstDiretorio = new Dictionary<int, string>();

					using (Comando comando = this.banco.CriarComando(String.Format(@"select c.id, c.raiz from {0}cnf_arquivo c where c.ativo = 1 and tipo = 2", this.EsquemaOficialComPonto)))
					{
						using (IDataReader reader = banco.ExecutarReader(comando))
						{
							while (reader.Read())
							{
								_lstDiretorio.Add(Convert.ToInt32(reader["id"]), reader["raiz"].ToString());
							}

							reader.Close();
						}
					}
				}

				return _lstDiretorio;
			}
		}

		internal List<int> BuscarIdsArquivo(int ticketID, int fileType)
		{
			string strSQL = String.Format(@"select t.id, t.arquivo from {0}tmp_projeto_geo_arquivos t where t.projeto=:projeto and t.tipo=:tipo", this.EsquemaOficialComPonto);
			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", fileType, DbType.Int32);

				List<int> result = new List<int>();

				using (IDataReader reader = banco.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						result.Add(Convert.ToInt32(reader["id"]));
						result.Add(Convert.ToInt32(reader["arquivo"]));
					}

					reader.Close();
				}

				return result;
			}
		}

		internal Hashtable ObterConfiguracoesBaseRef()
		{
			Hashtable result = null;

			string strSQL = @"
			select 
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_NAMES') GEOBASES_FEATURE_NAMES,
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_TYPES') GEOBASES_FEATURE_TYPES,
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_ALIASES') GEOBASES_FEATURE_ALIASES,
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_CONNECTION_KEY') GEOBASES_CONNECTION_KEY,
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_NAMES') INTERNO_FEATURE_NAMES,
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_TYPES') INTERNO_FEATURE_TYPES,
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_ALIASES') INTERNO_FEATURE_ALIASES,
				(select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_CONNECTION_KEY') INTERNO_CONNECTION_KEY,									
				(select c.valor from cnf_configuracao c where c.chave = 'SRID_BASE' ) SRID_BASE
			from dual";

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				using (IDataReader reader = banco.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						result = new Hashtable();
						result["GEOBASES_FEATURE_NAMES"] = reader["GEOBASES_FEATURE_NAMES"];
						result["GEOBASES_FEATURE_TYPES"] = reader["GEOBASES_FEATURE_TYPES"];
						result["GEOBASES_FEATURE_ALIASES"] = reader["GEOBASES_FEATURE_ALIASES"];
						result["GEOBASES_CONNECTION_KEY"] = reader["GEOBASES_CONNECTION_KEY"];

						result["INTERNO_FEATURE_NAMES"] = reader["INTERNO_FEATURE_NAMES"];
						result["INTERNO_FEATURE_TYPES"] = reader["INTERNO_FEATURE_TYPES"];
						result["INTERNO_FEATURE_ALIASES"] = reader["INTERNO_FEATURE_ALIASES"];
						result["INTERNO_CONNECTION_KEY"] = reader["INTERNO_CONNECTION_KEY"];
						result["SRID_BASE"] = reader["SRID_BASE"];
					}

					reader.Close();
				}

			}

			return result;
		}

		internal void SetAguardandoEtapaNaFila(int ticketID, int ticketType, int etapa)
		{
			string strSQL = "begin update tab_fila t set t.etapa=:etapa, t.situacao=" + FILA_SITUACAO_AGUARDANDO + ", t.data_inicio=null, t.data_fim=null where t.projeto=:projeto and t.tipo=:tipo; end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", etapa, DbType.Int32);
				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal void SetFalhaNaFila(int ticketID, int ticketType)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string strSQL = "begin update tab_fila t set t.situacao=" + FILA_SITUACAO_ERRO + ", t.data_fim=sysdate where t.projeto=:projeto and t.tipo=:tipo; end;";

				using (Comando comando = this.banco.CriarComando(strSQL))
				{
					comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}
			}
		}

		internal void SetConcluidoNaFila(int ticketID, int ticketType)
		{
			string strSQL = "begin update tab_fila t set t.situacao=" + FILA_SITUACAO_CONCLUIDO + ", t.data_fim=sysdate where t.projeto=:projeto and t.tipo=:tipo; end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal void SetProcessado(int ticketID)
		{
			/*
			string strSQL = "begin update tab_projeto_geo t set t.situacao_geo=" +  + " where t.id=:projeto; end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
			*/
		}

		internal void SetArquivoDisponivel(int arquivoId, int ticketID, int fileType)
		{
			string strSQL = String.Format(@"
			begin 
				for i in (select count(*) qtd from {0}tmp_projeto_geo_arquivos t where t.projeto=:projeto and t.tipo=:tipo) loop
					if (i.qtd>0) then
						update {0}tmp_projeto_geo_arquivos t set t.valido = 1, t.tid = :tid where t.projeto=:projeto and t.tipo=:tipo;
					else
						insert into {0}tmp_projeto_geo_arquivos(id, projeto, tipo, arquivo, valido, tid) values ({0}seq_tmp_projeto_geo_arquivos.nextval, :projeto, :tipo, :arquivo, 1, :tid);
					end if;
				end loop;
			end;", this.EsquemaOficialComPonto);

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", fileType, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", arquivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal int BuscarMecanismoDeEnvio(int ticketID, int ticketType)
		{
			int mecanismo = 0;

			using (Comando comando = this.banco.CriarComando(@"begin select t.mecanismo_elaboracao into :mecanismo from tab_fila t where t.projeto=:projeto and t.tipo = :tipo; end;"))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);
				comando.AdicionarParametroSaida("mecanismo", DbType.Int32);

				this.banco.ExecutarNonQuery(comando);

				mecanismo = (comando.ObterValorParametro("mecanismo") is DBNull) ? 0 : Convert.ToInt32(comando.ObterValorParametro("mecanismo"));
			}

			return mecanismo;
		}

		internal List<int> BuscarEnvelope(int ticketID)
		{
			string strSQL = String.Format(@"select t.menor_x, t.menor_y, t.maior_x, t.maior_y from {0}tmp_projeto_geo t where t.id=:projeto", this.EsquemaOficialComPonto);
			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

				List<int> result = new List<int>();

				using (IDataReader reader = banco.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						result.Add(Convert.ToInt32(reader["menor_x"]));
						result.Add(Convert.ToInt32(reader["menor_y"]));
						result.Add(Convert.ToInt32(reader["maior_x"]));
						result.Add(Convert.ToInt32(reader["maior_y"]));
					}

					reader.Close();
				}

				return result;
			}
		}

		internal void ApagarGeometriasTemporariasTrackmaker(int ticketID)
		{
			string strSQL = "begin OperacoesProcessamentoGeo.ApagarGeometriasTrackmaker(:projeto); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal void FinalizarImportacaoTrackmaker(int ticketID, int ticketType)
		{
			string strSQL = "begin OperacoesProcessamentoGeo.ImportarDoTrackmaker(:projeto, :tipo); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal void FinalizarImportacaoDesenhador(int ticketID, int ticketType)
		{
			string strSQL = "begin OperacoesProcessamentoGeo.ImportarDoDesenhador(:projeto, :tipo); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
		}

		#region Validações

		private List<Hashtable> BuscarValidacoes(int ticketID, int tipoValidacao)
		{
			string strSQL = "select * from tab_validacao_geo t where t.projeto=:projeto and t.tipo=:tipo";

			List<Hashtable> result = new List<Hashtable>();

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", tipoValidacao, DbType.Int32);

				result = this.banco.ExecutarHashtable(comando);

				return result;
			}
		}

		internal List<Hashtable> ValidarErrosEspaciais(int ticketID)
		{
			return BuscarValidacoes(ticketID, VALIDACAO_ERRO_ESPACIAL);
		}

		internal List<Hashtable> ValidarObrigatoriedades(int ticketID)
		{
			return BuscarValidacoes(ticketID, VALIDACAO_OBRIGATORIEDADE_NAO_ATENDIDA);
		}

		internal List<Hashtable> ValidarAtributos(int ticketID)
		{
			return BuscarValidacoes(ticketID, VALIDACAO_ATRIBUTO_INVALIDO);
		}

		internal List<Hashtable> ContabilizarGeometrias(int ticketID)
		{
			return BuscarValidacoes(ticketID, VALIDACAO_CONTABILIZACAO_DE_GEOMETRIAS);
		}

		#endregion

		internal void SalvarLogOperacoes(int ticketID, int ticketType, int ticketStep, List<Hashtable> listLogProcessamento)
		{
			/*
			string strSQL = @"insert into tab_log_operacoes (id, projeto, mapa, etapa, nome_recurso, tempo_execucao, data_evento) 
				values (seq_log_operacoes.nextval, :projeto, :mapa, :etapa, :nome_recurso, :tempo_execucao, sysdate)";

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			foreach (Hashtable htExecucao in listLogProcessamento)
			{
				using (this.Conexao.Comando = this.Conexao.BancoDeDados.CriarComando(strSQL))
				{
					this.Conexao.Comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
					this.Conexao.Comando.AdicionarParametroEntrada("mapa", ticketType, DbType.String);
					this.Conexao.Comando.AdicionarParametroEntrada("etapa", ticketStep, DbType.String);
					this.Conexao.Comando.AdicionarParametroEntrada("nome_recurso", htExecucao["NOME_RECURSO"], DbType.String);
					this.Conexao.Comando.AdicionarParametroEntrada("tempo_execucao", htExecucao["TEMPO"].ToString(), DbType.String);

					this.Conexao.BancoDeDados.ExecutarNonQuery(this.Conexao.Comando, this.Conexao.Transaction);
				}
			}
			*/
		}

		internal void ProcessarGeometrias(int ticketID, int ticketType)
		{
			string strSQL = "begin OperacoesProcessamentoGeo.Processar(:projeto, :tipo); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal void ProcessarValidacao(int ticketID, int ticketType)
		{
			string strSQL = "begin OperacoesProcessamentoGeo.ProcessarValidacao(:projeto, :tipo); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
		}
	}
}