using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data
{
	class ArquivoDa
	{
		#region Propriedades
		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }
		#endregion

		public ArquivoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal TramitacaoArquivo Criar(TramitacaoArquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Tramitação Arquivo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_arquivo a (id, nome, setor, tipo, protocolo_ativ_situacao, tid)
				values({0}seq_tramitacao_arquivo.nextval, :nome, :setor, :tipo, :protocolo_ativ_situacao, :tid) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", DbType.String, 100, arquivo.Nome);
				comando.AdicionarParametroEntrada("setor", arquivo.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.TipoId, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo_ativ_situacao", arquivo.ProtocoloSituacao, DbType.Int32);				
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				arquivo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Estantes

				if (arquivo.Estantes != null && arquivo.Estantes.Count > 0)
				{
					foreach (Estante item in arquivo.Estantes)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_arq_estante a (id, arquivo, nome, tid) values ({0}seq_tramitacao_arq_esta.nextval, :arquivo, :nome, :tid) returning a.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("nome", DbType.String, 50, item.Texto);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Prateleiras

						if (item.Prateleiras != null && item.Prateleiras.Count > 0)
						{

							comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_arq_prateleira (id, arquivo, estante, modo, identificacao, tid) values ({0}seq_tramitacao_arq_prat.nextval, :arquivo, :estante, :modo, :identificacao, :tid)", EsquemaBanco);

							comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("estante", item.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("modo", DbType.Int32);
							comando.AdicionarParametroEntrada("identificacao", DbType.String, 50);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

							foreach (Prateleira itemAux in item.Prateleiras)
							{
								comando.SetarValorParametro("identificacao", itemAux.Texto);
								comando.SetarValorParametro("modo", itemAux.ModoId);
								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(arquivo.Id.Value, eHistoricoArtefato.tramitacaoarquivo, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
			return arquivo;
		}

		internal TramitacaoArquivo Editar(TramitacaoArquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Tramitacao Arquivo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao_arquivo a set a.nome = :nome, a.setor = :setor, a.tipo = :tipo, a.protocolo_ativ_situacao = :protocolo_ativ_situacao,
				a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", arquivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 100, arquivo.Nome);
				comando.AdicionarParametroEntrada("setor", arquivo.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.TipoId, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo_ativ_situacao", arquivo.ProtocoloSituacao, DbType.Int32);				
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				#region Estantes/Prateleira

				if (arquivo.Estantes != null && arquivo.Estantes.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}tab_tramitacao_arq_prateleira c where c.arquivo = :arquivo ", EsquemaBanco);
					comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, arquivo.Estantes.SelectMany(x => x.Prateleiras).Select(y => y.Id).ToList());

					comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				//Estante
				comando = bancoDeDados.CriarComando("delete from {0}tab_tramitacao_arq_estante ae ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ae.arquivo = :arquivo {0}",
					comando.AdicionarNotIn("and", "ae.id", DbType.Int32, arquivo.Estantes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#endregion

				#region Estantes

				if (arquivo.Estantes != null && arquivo.Estantes.Count > 0)
				{
					foreach (Estante item in arquivo.Estantes)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao_arq_estante a set a.arquivo = :arquivo, a.nome = :nome, a.tid = :tid where a.id = :id returning a.id into :id_out", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_arq_estante a (id, arquivo, nome, tid) 
								values ({0}seq_tramitacao_arq_esta.nextval, :arquivo, :nome, :tid) returning a.id into :id_out", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("nome", DbType.String, 50, item.Texto);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						comando.AdicionarParametroSaida("id_out", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id_out"));

						#region Prateleiras

						if (item.Prateleiras != null && item.Prateleiras.Count > 0)
						{
							foreach (Prateleira itemAux in item.Prateleiras)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao_arq_prateleira a set a.arquivo = :arquivo, a.estante = :estante, a.modo = :modo, 
									a.identificacao = :identificacao, a.tid = :tid where a.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}tab_tramitacao_arq_prateleira a (id, arquivo, estante, modo, identificacao, tid) 
								   values ({0}seq_tramitacao_arq_prat.nextval, :arquivo, :estante, :modo, :identificacao, :tid)", EsquemaBanco);
								}

								comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("estante", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("modo", itemAux.ModoId, DbType.Int32);
								comando.AdicionarParametroEntrada("identificacao", DbType.String, 50, itemAux.Texto);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(arquivo.Id.Value, eHistoricoArtefato.tramitacaoarquivo, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
			return arquivo;
		}

		public TramitacaoArquivo Salvar(TramitacaoArquivo arquivo, BancoDeDados banco = null)
		{
			if (arquivo == null)
			{
				throw new Exception("Tramitacao Arquivo é nulo.");
			}

			if (arquivo.Id.HasValue && arquivo.Id.Value > 0)
			{
				arquivo = Editar(arquivo, banco);
			}
			else
			{
				arquivo = Criar(arquivo, banco);
			}
			return arquivo;
		}

		internal void Excluir(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_tramitacao_arquivo c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.tramitacaoarquivo, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados do arquivo de tramitação

				List<String> lista = new List<string>();
				lista.Add("delete from {0}tab_tramitacao_arq_prateleira c where c.arquivo = :arquivo;");
				lista.Add("delete from {0}tab_tramitacao_arq_estante b where b.arquivo = :arquivo;");
				lista.Add("delete from {0}tab_tramitacao_arquivo e where e.id = :arquivo;");
				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("arquivo", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter / Filtrar

		internal TramitacaoArquivo Obter(int id, bool simplificado = false)
		{
			TramitacaoArquivo arquivo = new TramitacaoArquivo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Tramitacao Arquivo
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.nome, a.setor, a.tipo, a.protocolo_ativ_situacao, a.tid 
				from {0}tab_tramitacao_arquivo a where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						arquivo.Id = id;
						arquivo.Tid = reader["tid"].ToString();
						arquivo.Nome = reader["nome"].ToString();

						if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
						{
							arquivo.SetorId = Convert.ToInt32(reader["setor"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							arquivo.TipoId = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["protocolo_ativ_situacao"] != null && !Convert.IsDBNull(reader["protocolo_ativ_situacao"]))
						{
							arquivo.ProtocoloSituacao = Convert.ToInt32(reader["protocolo_ativ_situacao"]);
						}
					}
					reader.Close();
				}
				
				if (simplificado)
				{
					return arquivo;
				}
				#endregion

				#region Estantes
				comando = bancoDeDados.CriarComando(@"select e.id, e.arquivo, e.nome, e.tid from {0}tab_tramitacao_arq_estante e where e.arquivo = :arquivo", EsquemaBanco);
				comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Estante estante;
					while (reader.Read())
					{
						estante = new Estante();
						estante.Id = Convert.ToInt32(reader["id"]);
						estante.Arquivo = Convert.ToInt32(reader["arquivo"]);
						estante.Texto = reader["nome"].ToString();
						estante.Tid = reader["tid"].ToString();

						#region Prateleiras
						comando = bancoDeDados.CriarComando(@"
										select e.id,
												e.arquivo,
												e.estante,
												e.identificacao,
												e.modo,
												ltam.texto modo_texto,
												e.tid
											from {0}tab_tramitacao_arq_prateleira e, {0}lov_tramitacao_arq_modo ltam
											where e.modo = ltam.id
											and e.arquivo = :arquivo
											and e.estante = :estante", EsquemaBanco);

						comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("estante", estante.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Prateleira prateleira;
							while (readerAux.Read())
							{
								prateleira = new Prateleira();
								prateleira.Id = Convert.ToInt32(readerAux["id"]);
								prateleira.Arquivo = Convert.ToInt32(readerAux["arquivo"]);
								prateleira.Estante = Convert.ToInt32(readerAux["estante"]);
								prateleira.ModoId = Convert.ToInt32(readerAux["modo"]);
								prateleira.ModoTexto = readerAux["modo_texto"].ToString();
								prateleira.Texto = readerAux["identificacao"].ToString();
								prateleira.Tid = readerAux["tid"].ToString();
								estante.Prateleiras.Add(prateleira);
							}
							readerAux.Close();
						}
						#endregion

						arquivo.Estantes.Add(estante);
					}
					reader.Close();
				}
				#endregion
			}
			return arquivo;
		}

		internal Resultados<TramitacaoArquivo> Filtrar(Filtro<TramitacaoArquivoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<TramitacaoArquivo> retorno = new Resultados<TramitacaoArquivo>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("a.nome", "nome", filtros.Dados.Nome, true);

				comandtxt += comando.FiltroAnd("a.setor", "setor", filtros.Dados.Setor);

				if (filtros.Dados.Estante > 0)
				{
					comandtxt += String.Format(" and a.id in (select b.arquivo from {0}tab_tramitacao_arq_estante b where upper(b.nome) like upper(:estante))", esquema);
					comando.AdicionarParametroEntrada("estante", filtros.Dados.Estante + "%", ExtensaoComando.ToDbType(filtros.Dados.Estante));
				}

				if (filtros.Dados.Prateleira > 0)
				{
					comandtxt += String.Format(" and a.id in (select b.arquivo from {0}tab_tramitacao_arq_prateleira b where upper(b.nome) like upper(:prateleira))", esquema);
					comando.AdicionarParametroEntrada("prateleira", filtros.Dados.Prateleira + "%", ExtensaoComando.ToDbType(filtros.Dados.Prateleira));
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome", "setor", "tipo" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_tramitacao_arquivo a where a.id > 0" + comandtxt, esquema);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select a.id, a.nome, a.setor setor_id, s.nome setor_nome, a.tipo tipo_id, lp.texto tipo_texto, a.tid  
				from {0}tab_tramitacao_arquivo a, {0}tab_setor s, {0}lov_tramitacao_arq_tipo lp where a.setor = s.id and a.tipo = lp.id" +
				comandtxt + DaHelper.Ordenar(colunas, ordenar), esquema);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					TramitacaoArquivo arquivo;

					while (reader.Read())
					{
						arquivo = new TramitacaoArquivo();
						arquivo.Id = Convert.ToInt32(reader["id"]);
						arquivo.Nome = reader["nome"].ToString();

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							arquivo.SetorId = Convert.ToInt32(reader["setor_id"]);
							arquivo.SetorNome = reader["setor_nome"].ToString();
						}

						if (reader["tipo_id"] != null && !Convert.IsDBNull(reader["tipo_id"]))
						{
							arquivo.TipoId = Convert.ToInt32(reader["tipo_id"]);
							arquivo.TipoTexto = reader["tipo_texto"].ToString();
						}

						arquivo.Tid = reader["tid"].ToString();

						retorno.Itens.Add(arquivo);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		internal bool ExisteArquivo(int id)
		{
			bool existe = false;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Itens do arquivo
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_tramitacao_arquivo tri where tri.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				existe = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				#endregion
			}
			return existe;
		}

		internal bool EstantePossuiProtocolo(int estante)
		{
			bool possui = false;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_tramitacao_arquivar t where t.estante = :estante", EsquemaBanco);
				comando.AdicionarParametroEntrada("estante", estante, DbType.Int32);
				possui = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
			return possui;
		}

		internal bool PrateleiraPossuiProtocolo(int prateleira)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_tramitacao_arquivar t where t.prateleira = :prateleira", EsquemaBanco);
				comando.AdicionarParametroEntrada("prateleira", prateleira, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool PossuiProtocolo(int arquivo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_tramitacao_arquivar t where t.arquivo = :arquivo", EsquemaBanco);
				comando.AdicionarParametroEntrada("arquivo", arquivo, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		#endregion
	}
}