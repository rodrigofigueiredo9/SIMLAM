using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.TecnoGeo.Geografico;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Data
{
	public class OperacoesGeoDa
	{
		internal const int FILA_SITUACAO_AGUARDANDO = 1;
		internal const int FILA_SITUACAO_EXECUTANTO = 2;
		internal const int FILA_SITUACAO_ERRO = 3;
		internal const int FILA_SITUACAO_CONCLUIDO = 4;
		internal const int FILA_SITUACAO_CANCELADO = 5;

		internal const int VALIDACAO_ERRO_ESPACIAL = 1;
		internal const int VALIDACAO_OBRIGATORIEDADE_NAO_ATENDIDA = 2;
		internal const int VALIDACAO_ATRIBUTO_INVALIDO = 3;
		internal const int VALIDACAO_CONTABILIZACAO_DE_GEOMETRIAS = 4;

		internal const int OPERACAO_BASEREF_INTERNA = 1;
		internal const int OPERACAO_BASEREF_GEOBASES = 2;
		internal const int OPERACAO_CADASTRO_PROPRIEDADE = 3;
		internal const int OPERACAO_ATIVIDADE = 4;
		internal const int OPERACAO_FISCALIZACAO = 5;
		internal const int OPERACAO_BASEREF_FISCAL = 6;
		internal const int OPERACAO_CAR = 7;
		internal const int OPERACAO_ATIVIDADE_TITULO = 8;
		internal const int OPERACAO_REGULARIZACAO = 9;

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

			string strSQL = @"select
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_NAMES') GEOBASES_FEATURE_NAMES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_TYPES') GEOBASES_FEATURE_TYPES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_FEATURE_ALIASES') GEOBASES_FEATURE_ALIASES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_GEOBASES_-_CONNECTION_KEY') GEOBASES_CONNECTION_KEY,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_NAMES') INTERNO_FEATURE_NAMES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_TYPES') INTERNO_FEATURE_TYPES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_FEATURE_ALIASES') INTERNO_FEATURE_ALIASES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_INTERNO_-_CONNECTION_KEY') INTERNO_CONNECTION_KEY,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_FISCAL_-_FEATURE_NAMES') FISCAL_FEATURE_NAMES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_FISCAL_-_FEATURE_TYPES') FISCAL_FEATURE_TYPES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_FISCAL_-_FEATURE_ALIASES') FISCAL_FEATURE_ALIASES,
                                    (select t.valor from cnf_sistema_geo t where t.campo='SERVICO_GEO_-_BASEREF_FISCAL_-_CONNECTION_KEY') FISCAL_CONNECTION_KEY,
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

						result["FISCAL_FEATURE_NAMES"] = reader["FISCAL_FEATURE_NAMES"];
						result["FISCAL_FEATURE_TYPES"] = reader["FISCAL_FEATURE_TYPES"];
						result["FISCAL_FEATURE_ALIASES"] = reader["FISCAL_FEATURE_ALIASES"];
						result["FISCAL_CONNECTION_KEY"] = reader["FISCAL_CONNECTION_KEY"];

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

		internal void SetComErroParaAguardandoEtapaNaFila()
		{
			string strSQL = $"begin update tab_fila t set t.situacao={FILA_SITUACAO_AGUARDANDO}, t.data_inicio=null, t.data_fim=null where t.situacao = {FILA_SITUACAO_ERRO} and t.titulo > 0; end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal void SetSemRegistroParaCanceladoNaFila()
		{
			string strSQL = $"begin update tab_fila f set f.situacao={ FILA_SITUACAO_CANCELADO}, f.data_fim=sysdate where f.titulo > 0 and f.situacao in ({FILA_SITUACAO_AGUARDANDO}, {FILA_SITUACAO_ERRO}, {FILA_SITUACAO_EXECUTANTO}) and not exists (select 1 from tab_titulo t where t.id = f.titulo); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
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

		internal void SetConcluidoNaFila(int ticketID, int ticketType, int ticketStep)
		{
			string strSQL = "begin update tab_fila t set t.situacao=" + FILA_SITUACAO_CONCLUIDO + ", t.data_fim=sysdate where t.projeto=:projeto and t.tipo=:tipo and t.etapa=:etapa; end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", ticketStep, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}

		}

		internal bool VerificarCancelado(int ticketID, int ticketType)
		{
			string strSQL = @"select count(*) from tab_fila t where t.projeto=:projeto and t.tipo=:tipo and t.situacao=:situacaoCancelado";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);
				comando.AdicionarParametroEntrada("situacaoCancelado", FILA_SITUACAO_CANCELADO, DbType.Int32);

				return Convert.ToBoolean(banco.ExecutarScalar(comando));
			}
		}

		internal void SetArquivoDisponivel(int arquivoId, int ticketID, int ticketType, int fileType)
		{
			string strSQL = String.Format(@"begin 
                       for i in (select count(*) qtd from {0}tmp_projeto_geo_arquivos t where t.projeto=:projeto and t.tipo=:tipo) loop
                          if (i.qtd>0) then
                             update {0}tmp_projeto_geo_arquivos t set t.valido = 1, t.tid = :tid where t.projeto=:projeto and t.tipo=:tipo;
                          else
                             insert into {0}tmp_projeto_geo_arquivos(id, projeto, tipo, arquivo, arquivo_fila_tipo, valido, tid) values ({0}seq_tmp_projeto_geo_arquivos.nextval, :projeto, :tipo, :arquivo, :arquivo_fila_tipo, 1, :tid);
                          end if;
                       end loop;
                    end;", this.EsquemaOficialComPonto);

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", fileType, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", arquivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo_fila_tipo", ticketType, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal void SetArquivoTitulo(int arquivoId, int? titulo, int ticketType, int fileType)
		{
			string strSQL = String.Format(@"begin 
                       for i in (select count(*) qtd from {0}tab_titulo_arquivo t where t.titulo=:titulo and t.ordem=:ordem) loop
                          if (i.qtd>0) then
                             update {0}tab_titulo_arquivo t set t.tid = :tid, t.croqui = 1 where t.titulo=:titulo and t.ordem=:ordem;
                          else
                             insert into {0}tab_titulo_arquivo(id, titulo, arquivo, ordem, descricao, tid, croqui) values ({0}seq_titulo_arquivo.nextval, :titulo, :arquivo, :ordem, :descricao, :tid, 1);
                          end if;
                       end loop;
                    end;", this.EsquemaOficialComPonto);

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("ordem", fileType, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", arquivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("descricao", "Croqui da Atividade", DbType.String);
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

		internal void ApagarGeometriasTemporariasTrackmaker(int ticketID, int ticketType)
		{
			string strSQL = "begin OperacoesProcessamentoGeo.ApagarGeometriasTrackmaker(:projeto, :tipo); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", ticketType, DbType.Int32);

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

		private List<Hashtable> BuscarValidacoes(int ticketID, int ticketType, int tipoValidacao)
		{
			string strSQL = "select * from tab_validacao_geo t where t.projeto=:projeto and t.tipo=:tipo ";

			List<Hashtable> result = new List<Hashtable>();

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", ticketID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", tipoValidacao, DbType.Int32);

				result = this.banco.ExecutarHashtable(comando);

				return result;
			}
		}

		internal List<Hashtable> ValidarErrosEspaciais(int ticketID, int ticketType)
		{
			return BuscarValidacoes(ticketID, ticketType, VALIDACAO_ERRO_ESPACIAL);
		}

		internal List<Hashtable> ValidarObrigatoriedades(int ticketID, int ticketType)
		{
			return BuscarValidacoes(ticketID, ticketType, VALIDACAO_OBRIGATORIEDADE_NAO_ATENDIDA);
		}

		internal List<Hashtable> ValidarAtributos(int ticketID, int ticketType)
		{
			return BuscarValidacoes(ticketID, ticketType, VALIDACAO_ATRIBUTO_INVALIDO);
		}

		internal List<Hashtable> ContabilizarGeometrias(int ticketID, int ticketType)
		{
			return BuscarValidacoes(ticketID, ticketType, VALIDACAO_CONTABILIZACAO_DE_GEOMETRIAS);
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

		internal void LimparErrosGeometricos(int projectID, int projectType)
		{
			string strSQL = "begin OperacoesProcessamentoGeo.ApagarGeometriasDESInvalidas(:projeto, :tipo); end;";

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", projectID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", projectType, DbType.Int32);

				this.banco.ExecutarNonQuery(comando);
			}
		}

		internal int ObterProjetoIdDominialidade(int projetoId)
		{
			int projetoIdDominialidade = 0;

			string strSQL = String.Format(@"select c.id 
				from {0}crt_projeto_geo c, tab_fila f 
				where c.empreendimento = f.empreendimento 
				and c.caracterizacao = 1
				and f.tipo = 7
				and f.projeto = :projeto", this.EsquemaOficialComPonto);

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				projetoIdDominialidade = this.banco.ExecutarScalar<int>(comando);
			}

			return projetoIdDominialidade;
		}

		internal ClasseFeicao ObterClasseFeicao(string classeFeicao)
		{
			ClasseFeicao classeFeicao1 = new ClasseFeicao(classeFeicao);

			try
			{
				int length = classeFeicao1.Nome.IndexOf(".");
				string str1;
				string str2;
				if (length < 0)
				{
					str1 = "IDAFGEO";
					str2 = classeFeicao1.Nome;
				}
				else
				{
					str1 = classeFeicao1.Nome.Substring(0, length);
					str2 = classeFeicao1.Nome.Substring(length + 1);
				}

				string sql = "select count(*) from (select m.owner||'.'|| m.table_name tabela from " +
							"all_sdo_geom_metadata m) x where x.tabela = upper(:tabela)";
				string result;

				using (Comando comando = this.banco.CriarComando(sql))
				{
					comando.AdicionarParametroEntrada("tabela", (str1 + "." + str2), DbType.String);

					result = this.banco.ExecutarScalar(comando).ToString();
				}

				if (result == "")
				{
					throw new Exception("Tabela " + classeFeicao1.Nome + " não encontrada");
				}

				sql = "select a.column_name, a.data_type, a.char_length, a.data_precision, a.data_scale " +
						"from all_tab_cols a where a.owner=UPPER(:owner) and a.table_name=UPPER(:tablename) " +
						 "and a.hidden_column='NO' order by a.column_id";

				Hashtable list = new Hashtable();
				ClasseFeicao classeFeicao2 = new ClasseFeicao(classeFeicao);
				using (Comando comando = this.banco.CriarComando(sql))
				{
					comando.AdicionarParametroEntrada("owner", str1, DbType.String);
					comando.AdicionarParametroEntrada("tablename", str2, DbType.String);

					using (IDataReader reader = banco.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							if (reader["data_type"].ToString() == "SDO_GEOMETRY")
							{
								classeFeicao2.CampoGeometrico = reader["column_name"].ToString();
							}
							else
							{
								Atributo campo = new Atributo();
								campo.Nome = reader["column_name"].ToString();
								switch (reader["data_type"].ToString())
								{
									case "DATE":
										campo.Tipo = DbType.DateTime;
										break;
									case "NUMBER":
										campo.Tipo = DbType.Decimal;
										if (reader["data_precision"] is DBNull || reader["data_precision"] == null)
										{
											if (reader["data_scale"] is DBNull || reader["data_scale"] == null)
											{
												campo.Tamanho = -1;
												campo.CasasDecimais = -1;
												break;
											}
											campo.Tamanho = 38;
											campo.CasasDecimais = int.Parse(reader["data_scale"].ToString());
											break;
										}
										campo.Tamanho = int.Parse(reader["data_precision"].ToString());
										int num = reader["data_scale"] is DBNull || reader["data_scale"] == null ? 0 : (Convert.ToInt32(reader["data_scale"]) >= 0 ? 1 : 0);
										campo.CasasDecimais = num != 0 ? int.Parse(reader["data_scale"].ToString()) : 0;
										break;
									case "REAL":
									case "FLOAT":
										campo.Tipo = DbType.Decimal;
										campo.Tamanho = -1;
										campo.CasasDecimais = -1;
										break;
									case "INT":
									case "SMALLINT":
									case "DECIMAL":
									case "DEC":
									case "NUMERIC":
									case "INTEGER":
										campo.Tipo = DbType.Decimal;
										campo.Tamanho = 38;
										campo.CasasDecimais = 0;
										break;
									case "CHAR":
									case "NCHAR":
									case "NVARCHAR2":
									case "STRING":
									case "VARCHAR2":
										campo.Tipo = DbType.String;
										campo.Tamanho = int.Parse(reader["char_length"].ToString());
										break;
									case "LONG":
										campo.Tipo = DbType.String;
										campo.Tamanho = 32760;
										break;
									case "BLOB":
										break;
									default:
										throw new Exception("Tipo de campo não tratado \"" + reader["data_type"].ToString() + "\"");
								}
								classeFeicao2.Atributos.Adicionar(campo);
							}
						}
						return classeFeicao2;
					}
				}
			}
			catch
			{
				return null;
			}
		}
	}
}