using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data
{
	public class TituloModeloDa
	{
		#region Propriedades

		Historico _historico = new Historico();

		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public TituloModeloDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TituloModelo modelo, BancoDeDados banco = null)
		{
			if (modelo == null)
			{
				throw new Exception("O Modelo de Título é nulo.");
			}

			if (modelo.Id == 0)
			{
				throw new Exception("O Modelo de Título não existe.");
			}

			Editar(modelo, banco);
		}

		internal int? Criar(TituloModelo modelo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo m (id, codigo, tipo, situacao, subtipo, data_criacao, nome, sigla, tipo_protocolo, arquivo, tid, documento)
				values({0}seq_titulo_modelo.nextval, (select max(c.codigo)+1 from {0}tab_titulo_modelo c), :tipo, :situacao, :subtipo, sysdate, :nome, :sigla, :tipo_protocolo, :arquivo, :tid, :documento)
				returning m.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tipo", modelo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", 1, DbType.Int32);
				comando.AdicionarParametroEntrada("subtipo", DbType.String, 100, modelo.SubTipo);
				comando.AdicionarParametroEntrada("nome", DbType.String, 200, modelo.Nome);
				comando.AdicionarParametroEntrada("sigla", DbType.String, 10, modelo.Sigla);
				comando.AdicionarParametroEntrada("arquivo", (modelo.Arquivo.Id.HasValue) ? modelo.Arquivo.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_protocolo", modelo.TipoProtocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("documento", modelo.TipoDocumento, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				modelo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Regras

				if (modelo.Regras != null && modelo.Regras.Count > 0)
				{
					foreach (TituloModeloRegra item in modelo.Regras)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_regras r (id, modelo, regra, tid) 
						values ({0}seq_titulo_modelo_regras.nextval, :modelo, :regra, :tid) returning r.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("regra", item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Respostas

						if (item.Respostas != null && item.Respostas.Count > 0)
						{
							foreach (TituloModeloResposta itemAux in item.Respostas)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_respostas e (id, modelo, regra, resposta, valor, tid)
								values ({0}seq_titulo_modelo_respostas.nextval, :modelo, :regra, :resposta, :valor, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("regra", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("resposta", itemAux.Tipo, DbType.Int32);
								comando.AdicionarParametroEntrada("valor", itemAux.Valor, DbType.String);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Setores

				if (modelo.Setores != null && modelo.Setores.Count > 0)
				{
					foreach (Setor item in modelo.Setores)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_setores s (id, modelo, setor, hierarquia, tid)
						values ({0}seq_titulo_modelo_setores.nextval, :modelo, :setor, :hierarquia, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("hierarquia", DbType.String, 100, item.HierarquiaCabecalho);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Assinantes

				if (modelo.Assinantes != null && modelo.Assinantes.Count > 0)
				{
					foreach (Assinante item in modelo.Assinantes)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_assinantes s (id, modelo, setor, tipo_assinante, tid)
						values ({0}seq_titulo_modelo_assinantes.nextval, :modelo, :setor, :tipo_assinante, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.SetorId, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_assinante", item.TipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(modelo.Id, eHistoricoArtefato.titulomodelo, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return modelo.Id;
			}
		}

		internal void Editar(TituloModelo modelo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_modelo m set m.subtipo = :subtipo, m.nome = :nome, m.sigla = :sigla, 
				m.tipo_protocolo = :tipo_protocolo, m.arquivo = :arquivo, m.tid = :tid, m.documento = :documento where m.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("subtipo", DbType.String, 100, modelo.SubTipo);
				comando.AdicionarParametroEntrada("nome", DbType.String, 200, modelo.Nome);
				comando.AdicionarParametroEntrada("sigla", DbType.String, 10, modelo.Sigla);
				comando.AdicionarParametroEntrada("tipo_protocolo", modelo.TipoProtocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", (modelo.Arquivo.Id.HasValue) ? modelo.Arquivo.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("documento", modelo.TipoDocumento, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", modelo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Respostas
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_modelo_respostas mr ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where mr.modelo = :modelo{0}",
				comando.AdicionarNotIn("and", "mr.id", DbType.Int32, modelo.Regras.SelectMany(x => x.Respostas).Select(y => y.Id).ToList()));
				comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Regras
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_modelo_regras r ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where r.modelo = :modelo{0}",
				comando.AdicionarNotIn("and", "r.id", DbType.Int32, modelo.Regras.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Setores
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_modelo_setores s ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where s.modelo = :modelo{0}",
				comando.AdicionarNotIn("and", "s.id", DbType.Int32, modelo.Setores.Select(x => x.IdRelacao).ToList()));
				comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Assinantes
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_modelo_assinantes a ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where a.modelo = :modelo{0}",
				comando.AdicionarNotIn("and", "a.id", DbType.Int32, modelo.Assinantes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Regras

				if (modelo.Regras != null && modelo.Regras.Count > 0)
				{
					foreach (TituloModeloRegra item in modelo.Regras)
					{
						if (item.Id == 0)
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_regras r (id, modelo, regra, tid) 
							values ({0}seq_titulo_modelo_regras.nextval, :modelo, :regra, :tid) returning r.id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("regra", item.Tipo, DbType.Int32);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							comando.AdicionarParametroSaida("id", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							if (item.Id == 0)
							{
								item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
							}
						}

						#region Respostas

						if (item.Respostas != null && item.Respostas.Count > 0)
						{
							foreach (TituloModeloResposta itemAux in item.Respostas)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_modelo_respostas e set e.modelo = :modelo, e.regra = :regra, e.resposta = :resposta, 
									e.valor = :valor, e.tid = :tid where e.id = :id", EsquemaBanco);
									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_respostas e (id, modelo, regra, resposta, valor, tid)
									values ({0}seq_titulo_modelo_respostas.nextval, :modelo, :regra, :resposta, :valor, :tid)", EsquemaBanco);
								}

								comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("regra", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("resposta", itemAux.Tipo, DbType.Int32);
								comando.AdicionarParametroEntrada("valor", itemAux.Valor, DbType.String);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion
					}
				}
				#endregion

				#region Setores

				if (modelo.Setores != null && modelo.Setores.Count > 0)
				{
					foreach (Setor item in modelo.Setores)
					{
						if (item.IdRelacao > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_modelo_setores e set e.modelo = :modelo, e.setor = :setor, e.hierarquia = :hierarquia, 
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_setores s (id, modelo, setor, hierarquia, tid)
							values ({0}seq_titulo_modelo_setores.nextval, :modelo, :setor, :hierarquia, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("hierarquia", DbType.String, 100, item.HierarquiaCabecalho);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				#endregion

				#region Assinantes

				if (modelo.Assinantes != null && modelo.Assinantes.Count > 0)
				{
					foreach (Assinante item in modelo.Assinantes)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_modelo_assinantes e set e.modelo = :modelo, e.setor = :setor, e.tipo_assinante = :tipo_assinante,
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_modelo_assinantes s (id, modelo, setor, tipo_assinante, tid)
							values ({0}seq_titulo_modelo_assinantes.nextval, :modelo, :setor, :tipo_assinante, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("modelo", modelo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.SetorId, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_assinante", item.TipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(modelo.Id, eHistoricoArtefato.titulomodelo, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(TituloModelo modelo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_modelo m set m.situacao = :situacao, m.tid = :tid where m.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", modelo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", modelo.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(modelo.Id, eHistoricoArtefato.titulomodelo, (modelo.SituacaoId == 1) ? eHistoricoAcao.ativar : eHistoricoAcao.desativar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public Resultados<TituloModelo> Filtrar(Filtro<TituloModeloListarFiltro> filtros)
		{
			Resultados<TituloModelo> retorno = new Resultados<TituloModelo>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("m.nome", "nome", filtros.Dados.Nome, true);
				comandtxt += comando.FiltroAnd("m.tipo", "tipo", filtros.Dados.Tipo);

				comandtxt += comando.FiltroAnd("m.situacao", "situacao", filtros.Dados.Situacao);

				comandtxt += comando.FiltroIn("m.id", "(select t.modelo from tab_titulo_modelo_setores t where t.setor = :setor)", "setor", filtros.Dados.Setor);

				comandtxt += comando.FiltroIn("m.id", String.Format("(select s.modelo from {0}tab_titulo_modelo_setores s where s.setor = :modelo_id)",
				(string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "modelo_id", filtros.Dados.Setor);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome", "tipo_texto", "situacao_texto" };

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

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_titulo_modelo m where m.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select m.id, m.codigo, m.tipo tipo_id, lt.texto tipo_texto, m.subtipo, m.data_criacao, m.nome, m.sigla, 
				lp.id tipo_protocolo_id, lp.texto tipo_protocolo_texto, m.tid, ltms.texto situacao_texto, ltms.id situacao_id 
				from {0}tab_titulo_modelo m, {0}lov_titulo_modelo_tipo lt, {0}lov_titulo_modelo_prot_tipo lp, {0}lov_titulo_modelo_situacao ltms 
				where m.tipo = lt.id and m.tipo_protocolo = lp.id and m.situacao = ltms.id" + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno
					TituloModelo modelo;
					while (reader.Read())
					{
						modelo = new TituloModelo();
						modelo.Id = Convert.ToInt32(reader["id"]);
						modelo.Tid = reader["tid"].ToString();
						modelo.Nome = reader["nome"].ToString();
						modelo.Sigla = reader["sigla"].ToString();
						modelo.SubTipo = reader["subtipo"].ToString();
						modelo.DataCriacao.Data = Convert.ToDateTime(reader["data_criacao"]);
						modelo.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						modelo.SituacaoTexto = reader["situacao_texto"].ToString();

						if (reader["tipo_protocolo_id"] != null && !Convert.IsDBNull(reader["tipo_protocolo_id"]))
						{
							modelo.TipoProtocolo = Convert.ToInt32(reader["tipo_protocolo_id"]);
						}

						if (reader["tipo_id"] != null && !Convert.IsDBNull(reader["tipo_id"]))
						{
							modelo.Tipo = Convert.ToInt32(reader["tipo_id"]);
							modelo.TipoTexto = reader["tipo_texto"].ToString();
						}

						retorno.Itens.Add(modelo);
					}

					reader.Close();
					reader.Dispose();

					#endregion
				}

				bancoDeDados.Dispose();
			}

			return retorno;
		}

		internal TituloModelo Obter(int id, BancoDeDados banco = null)
		{
			TituloModelo modelo = new TituloModelo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				Comando comando = bancoDeDados.CriarComando(@"
                    select m.id,
                           m.tid,
                           m.codigo,
                           m.tipo,
                           m.subtipo,
                           m.data_criacao,
                           m.nome,
                           m.sigla,
                           m.tipo_protocolo,
                           m.arquivo,
                           ta.nome nome_arquivo,
                           m.situacao,
                           m.documento,
                           l.texto documento_texto
                      from {0}tab_titulo_modelo m, 
                           {0}tab_arquivo ta,
                           {0}lov_titulo_modelo_tipo_doc l
                     where m.arquivo = ta.id(+)
                       and m.documento = l.id(+)
                       and m.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						modelo.Id = id;
						modelo.Tid = reader["tid"].ToString();
						modelo.SubTipo = reader["subtipo"].ToString();
						modelo.DataCriacao.Data = Convert.ToDateTime(reader["data_criacao"]);
						modelo.Nome = reader["nome"].ToString();
						modelo.Sigla = reader["sigla"].ToString();

						if (reader["situacao"] != null && !Convert.IsDBNull(reader["situacao"]))
						{
							modelo.SituacaoId = Convert.ToInt32(reader["situacao"]);
						}

						if (reader["codigo"] != null && !Convert.IsDBNull(reader["codigo"]))
						{
							modelo.Codigo = Convert.ToInt32(reader["codigo"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							modelo.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["tipo_protocolo"] != null && !Convert.IsDBNull(reader["tipo_protocolo"]))
						{
							modelo.TipoProtocolo = Convert.ToInt32(reader["tipo_protocolo"]);
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							modelo.Arquivo.Id = Convert.ToInt32(reader["arquivo"]);
							modelo.Arquivo.Nome = reader["nome_arquivo"].ToString();
						}

						modelo.TipoDocumento = reader.GetValue<int>("documento");
						modelo.TipoDocumentoTexto = reader.GetValue<string>("documento_texto");

					}
					reader.Close();
				}

				#endregion

				#region Regras

				comando = bancoDeDados.CriarComando(@"select r.id, r.modelo, r.regra, r.tid from {0}tab_titulo_modelo_regras r where r.modelo = :modelo", EsquemaBanco);
				comando.AdicionarParametroEntrada("modelo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloModeloRegra item;
					while (reader.Read())
					{
						item = new TituloModeloRegra();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.Valor = true;
						if (reader["regra"] != null && !Convert.IsDBNull(reader["regra"]))
						{
							item.Tipo = Convert.ToInt32(reader["regra"]);
						}

						#region Respostas

						comando = bancoDeDados.CriarComando(@"select mr.id, mr.modelo, mr.regra, mr.resposta, mr.valor, mr.tid 
						from {0}tab_titulo_modelo_respostas mr where mr.modelo = :modelo and mr.regra = :regra", EsquemaBanco);
						comando.AdicionarParametroEntrada("modelo", id, DbType.Int32);
						comando.AdicionarParametroEntrada("regra", item.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							TituloModeloResposta itemAux;
							while (readerAux.Read())
							{
								itemAux = new TituloModeloResposta();
								itemAux.Id = Convert.ToInt32(readerAux["id"]);
								itemAux.Tid = readerAux["tid"].ToString();
								if (readerAux["resposta"] != null && !Convert.IsDBNull(readerAux["resposta"]))
								{
									itemAux.Tipo = Convert.ToInt32(readerAux["resposta"]);
								}
								itemAux.Valor = readerAux["valor"];

								item.Respostas.Add(itemAux);
							}

							readerAux.Close();
						}

						#endregion

						modelo.Regras.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Setores

				comando = bancoDeDados.CriarComando(@"select mr.id, mr.modelo, mr.setor, mr.hierarquia, ts.nome, mr.tid from tab_titulo_modelo_setores mr, tab_setor ts where mr.setor = ts.id and mr.modelo = :modelo", EsquemaBanco);
				comando.AdicionarParametroEntrada("modelo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Setor item;
					while (reader.Read())
					{
						item = new Setor();
						item.IdRelacao = Convert.ToInt32(reader["id"]);
						item.Id = Convert.ToInt32(reader["setor"]);
						item.Nome = reader["nome"].ToString();
						item.HierarquiaCabecalho = reader["hierarquia"].ToString();
						modelo.Setores.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Assinantes

				comando = bancoDeDados.CriarComando(@"select mr.id, mr.modelo, mr.setor, s.nome setor_nome, mr.tipo_assinante, la.texto tipo_assinante_texto, mr.tid 
				from {0}tab_titulo_modelo_assinantes mr, {0}tab_setor s, {0}lov_titulo_modelo_assinante la
				where mr.tipo_assinante = la.id and mr.setor = s.id and mr.modelo = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("modelo", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Assinante item;
					while (reader.Read())
					{
						item = new Assinante();
						item.Id = Convert.ToInt32(reader["id"]);

						if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
						{
							item.SetorId = Convert.ToInt32(reader["setor"]);
							item.SetorTexto = reader["setor_nome"].ToString();
						}

						if (reader["tipo_assinante"] != null && !Convert.IsDBNull(reader["tipo_assinante"]))
						{
							item.TipoId = Convert.ToInt32(reader["tipo_assinante"]);
							item.TipoTexto = reader["tipo_assinante_texto"].ToString();
						}

						modelo.Assinantes.Add(item);
					}

					reader.Close();
				}

				#endregion
			}
			return modelo;
		}

		internal TituloModelo ObterSimplificado(int id, BancoDeDados banco = null)
		{
			TituloModelo modelo = new TituloModelo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				Comando comando = bancoDeDados.CriarComando(@"
                    select m.id,
                           m.tid,
                           m.codigo,
                           m.tipo,
                           m.subtipo,
                           m.data_criacao,
                           m.nome,
                           m.sigla,
                           m.tipo_protocolo,
                           m.arquivo,
                           m.documento,
                           l.texto documento_texto       
                      from {0}tab_titulo_modelo m,
                           {0}lov_titulo_modelo_tipo_doc l
                     where m.documento = l.id(+)
                       and m.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						modelo.Id = id;
						modelo.Tid = reader["tid"].ToString();
						modelo.SubTipo = reader["subtipo"].ToString();
						modelo.DataCriacao.Data = Convert.ToDateTime(reader["data_criacao"]);
						modelo.Nome = reader["nome"].ToString();
						modelo.Sigla = reader["sigla"].ToString();

						if (reader["codigo"] != null && !Convert.IsDBNull(reader["codigo"]))
						{
							modelo.Codigo = Convert.ToInt32(reader["codigo"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							modelo.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["tipo_protocolo"] != null && !Convert.IsDBNull(reader["tipo_protocolo"]))
						{
							modelo.TipoProtocolo = Convert.ToInt32(reader["tipo_protocolo"]);
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							modelo.Arquivo.Id = Convert.ToInt32(reader["arquivo"]);
						}

						modelo.TipoDocumento = reader.GetValue<int>("documento");
						modelo.TipoDocumentoTexto = reader.GetValue<string>("documento_texto");
					}
					reader.Close();
				}

				#endregion
			}
			return modelo;
		}

		internal TituloModelo ObterSimplificadoCodigo(int codigo, BancoDeDados banco = null)
		{
			TituloModelo modelo = new TituloModelo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				Comando comando = bancoDeDados.CriarComando(@"
                    select m.id,
                           m.tid,
                           m.codigo,
                           m.tipo,
                           m.subtipo,
                           m.data_criacao,
                           m.nome,
                           m.sigla,
                           m.tipo_protocolo,
                           m.arquivo,
                           m.documento,
                           l.texto documento_texto       
                      from {0}tab_titulo_modelo m,
                           {0}lov_titulo_modelo_tipo_doc l
                     where m.documento = l.id(+)
                       and m.codigo = :codigo", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo", codigo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						modelo.Id = Convert.ToInt32(reader["id"]);
						modelo.Tid = reader["tid"].ToString();
						modelo.SubTipo = reader["subtipo"].ToString();
						modelo.DataCriacao.Data = Convert.ToDateTime(reader["data_criacao"]);
						modelo.Nome = reader["nome"].ToString();
						modelo.Sigla = reader["sigla"].ToString();

						if (reader["codigo"] != null && !Convert.IsDBNull(reader["codigo"]))
						{
							modelo.Codigo = Convert.ToInt32(reader["codigo"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							modelo.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["tipo_protocolo"] != null && !Convert.IsDBNull(reader["tipo_protocolo"]))
						{
							modelo.TipoProtocolo = Convert.ToInt32(reader["tipo_protocolo"]);
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							modelo.Arquivo.Id = Convert.ToInt32(reader["arquivo"]);
						}

						modelo.TipoDocumento = reader.GetValue<int>("documento");
						modelo.TipoDocumentoTexto = reader.GetValue<string>("documento_texto");
					}
					reader.Close();
				}

				#endregion
			}
			return modelo;
		}

		public List<TituloModeloLst> ObterModelos(int exceto = 0, bool todos = false)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.codigo, t.nome from tab_titulo_modelo t");

				if (exceto > 0 || !todos)
				{

					comando.DbCommand.CommandText += " where ";

					if (exceto > 0)
					{
						comando.DbCommand.CommandText += "t.id <> :id";
						comando.AdicionarParametroEntrada("id", exceto, DbType.Int32);
					}

					if (!todos)
					{
						if (exceto > 0)
						{
							comando.DbCommand.CommandText += " and ";
						}

						comando.DbCommand.CommandText += "t.situacao = 1";
					}
				}
				comando.DbCommand.CommandText += " order by t.nome ";


				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloModeloLst()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						Codigo = Convert.IsDBNull(item["codigo"]) ? 0 : Convert.ToInt32(item["codigo"]),
						IsAtivo = true
					});
				}
			}
			return lst;
		}

		internal List<Situacao> ObterSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_situacao t order by texto");

			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ListaValor> ObterAssinanteFuncionarios(int modeloId, int setorId, int cargoId, BancoDeDados banco = null)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@" select distinct t.* from ( select tf.id, tf.nome from {0}tab_funcionario tf, {0}tab_setor ts, {0}tab_funcionario_cargo tfc, 
					{0}tab_titulo_modelo_assinantes ttma where ts.responsavel = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.id and ttma.tipo_assinante = 1 and ttma.modelo = :modeloId and ts.id 
					= :setorId and tfc.cargo = :cargoId union all select tf.id, tf.nome from {0}tab_funcionario tf, {0}tab_funcionario_setor ts, {0}tab_funcionario_cargo tfc, {0}tab_titulo_modelo_assinantes ttma
					where ts.funcionario = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.setor and ttma.tipo_assinante = 2 and ttma.modelo = :modeloId and ts.setor = :setorId and tfc.cargo = 
					:cargoId ) t order by t.nome ", EsquemaBanco);

				comando.AdicionarParametroEntrada("modeloId", modeloId, DbType.Int32);
				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);
				comando.AdicionarParametroEntrada("cargoId", cargoId, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new ListaValor()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<ListaValor> ObterAssinanteCargos(int modeloId, int setorId, BancoDeDados banco = null)
		{
			List<ListaValor> lst = new List<ListaValor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@" select distinct t.* from ( select  tc.id, tc.nome from {0}tab_funcionario tf, {0}tab_setor ts, {0}tab_funcionario_setor tse, 
					{0}tab_funcionario_cargo tfc, {0}tab_cargo tc, {0}tab_titulo_modelo_assinantes ttma where ts.responsavel = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.id and tfc.cargo = tc.id
					and ttma.tipo_assinante = 1 and ttma.modelo = :modeloId and ts.id = :setorId union all select tc.id, tc.nome from {0}tab_funcionario tf, {0}tab_funcionario_setor ts, 
					{0}tab_funcionario_cargo tfc, {0}tab_cargo tc, {0}tab_titulo_modelo_assinantes ttma where ts.funcionario = tf.id and tf.id = tfc.funcionario and ttma.setor = ts.setor and tfc.cargo = 
					tc.id and ttma.tipo_assinante = 2 and ttma.modelo = :modeloId and ts.setor = :setorId ) t order by t.nome ", EsquemaBanco);

				comando.AdicionarParametroEntrada("modeloId", modeloId, DbType.Int32);
				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new ListaValor()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<Setor> ObterSetoresModelo(int modelo, BancoDeDados banco = null)
		{
			List<Setor> lst = new List<Setor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ts.id, ts.nome from tab_titulo_modelo_setores t, tab_setor ts, tab_titulo_modelo tm 
				where t.setor = ts.id and t.modelo = tm.id and tm.id = :modelo order by ts.nome");

				comando.AdicionarParametroEntrada("modelo", modelo);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new Setor()
					{
						Id = Convert.ToInt32(item["id"]),
						Nome = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<Setor> ObterSetoresModeloPorTitulo(int titulo, BancoDeDados banco = null)
		{
			List<Setor> lst = new List<Setor>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ts.id, ts.nome from tab_titulo_modelo_setores t, tab_setor ts, tab_titulo_modelo tm, tab_titulo tt
				where t.setor = ts.id and t.modelo = tm.id and tt.modelo = tm.id and tt.id = :titulo order by ts.nome");

				comando.AdicionarParametroEntrada("titulo", titulo);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new Setor()
					{
						Id = Convert.ToInt32(item["id"]),
						Nome = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<TituloModeloLst> ObterModelosSetorFunc(int funcionarioId, int setor = 0, BancoDeDados banco = null)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				if (setor > 0)
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.nome from tab_titulo_modelo t where t.id in 
					(select ms.modelo from tab_titulo_modelo_setores ms, tab_funcionario_setor s where s.funcionario = :funcionario  
					and s.setor = ms.setor and ms.setor = :setor) and t.situacao = 1 order by t.nome");

					comando.AdicionarParametroEntrada("setor", setor);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.nome from tab_titulo_modelo t where t.id in 
					(select ms.modelo from tab_titulo_modelo_setores ms, tab_funcionario_setor s where s.funcionario = :funcionario  
					and s.setor = ms.setor ) and t.situacao = 1 order by t.nome");
				}

				comando.AdicionarParametroEntrada("funcionario", funcionarioId);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloModeloLst()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<TituloModeloLst> ObterModelosDeclaratorios()
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				var sql = @"
                    select t.id Id, 
                           t.nome Texto,
                           case when t.situacao = 1 then 1 else 0 end IsAtivo
                      from tab_titulo_modelo t
                     where t.documento = 2 /*Título Declaratório*/";

				using (var command = bancoDeDados.CriarComando(sql))
				{
					lst = bancoDeDados.ObterEntityList<TituloModeloLst>(command);
				}
			}

			return lst;
		}

		internal List<TituloModeloLst> ObterModelosLista(TituloModeloListarFiltro filtros, BancoDeDados banco = null)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				comandtxt += comando.FiltroAnd("t.tipo", "tipo", filtros.Tipo);

				comando.DbCommand.CommandText = @"select t.id, t.nome from tab_titulo_modelo t where t.situacao = 1 " + comandtxt + " order by t.nome ";

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloModeloLst()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<TituloModeloLst> ObterModelosAnteriores(int modelo, BancoDeDados banco = null)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ttm.id, ttm.nome, ttm.sigla
																from tab_titulo_modelo_respostas ttmr,
																	{0}tab_titulo_modelo           ttm,
																	{0}tab_titulo_modelo_regras    ttmreg
																where to_number(ttmr.valor) = ttm.id
																and ttmr.regra = ttmreg.id
																and ttmreg.regra = 10 /*Possui fase anterior?*/
																and ttmr.resposta = 7 /*Modelo de Título*/
																and ttmr.modelo = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("modelo", modelo, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloModeloLst()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						Sigla = item["sigla"].ToString(),
						IsAtivo = true
					});
				}
			}
			return lst;
		}

		internal string ObterUltimoNumeroGerado(int id, bool reiniciaPorAno = false, BancoDeDados banco = null)
		{
			string numero = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				Comando comando = bancoDeDados.CriarComando(@"select nvl(max(t.numero),1) numero from {0}tab_titulo t where t.modelo = :id", EsquemaBanco);
				if (reiniciaPorAno)
				{
					comando.DbCommand.CommandText = "and t.ano = to_char( sysdate,'yyyy')";
				}
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				numero = bancoDeDados.ExecutarScalar(comando).ToString();

				#endregion
			}

			return numero;
		}

		internal string VerificarPublicoExternoAtividade(int id)
		{
			string nomesGrupos = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Título Modelo

				Comando comando = bancoDeDados.CriarComando(@"select stragg(ca.nome) from cnf_atividade ca, cnf_atividade_modelos cam where ca.id = cam.configuracao and cam.modelo = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				nomesGrupos = bancoDeDados.ExecutarScalar(comando).ToString();

				#endregion
			}

			return nomesGrupos;
		}

		internal List<TituloModeloLst> ObterModelosRenovacao(int modelo, BancoDeDados banco = null)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
								select ttm.id, ttm.nome, ttm.sigla
									from tab_titulo_modelo_respostas ttmr,
										{0}tab_titulo_modelo           ttm,
										{0}tab_titulo_modelo_regras    ttmreg
									where to_number(ttmr.valor) = ttm.id
									and ttmr.regra = ttmreg.id
									and ttmreg.regra = 6 /*É passível de renovação?*/
									and ttmr.resposta = 7 /*Modelo de Título*/
									and ttmr.modelo = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("modelo", modelo, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloModeloLst()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						Sigla = item["sigla"].ToString(),
						IsAtivo = true
					});
				}
			}
			return lst;
		}

		internal bool PossuiConfiguracaoAtividade(TituloModelo modelo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from cnf_atividade_modelos t where t.modelo = :modeloId");

				comando.AdicionarParametroEntrada("modeloId", modelo.Id, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		#endregion
	}
}