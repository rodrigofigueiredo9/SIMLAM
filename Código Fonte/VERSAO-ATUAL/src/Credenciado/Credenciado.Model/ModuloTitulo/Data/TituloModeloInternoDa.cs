using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data
{
	public class TituloModeloInternoDa
	{
		private string EsquemaBanco { get; set; }

		public TituloModeloInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal TituloModelo Obter(int id, BancoDeDados banco = null)
		{
			TituloModelo modelo = new TituloModelo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título Modelo

				Comando comando = bancoDeDados.CriarComando(@"select m.id,
					m.tid, m.codigo, m.tipo, m.subtipo, m.data_criacao, m.nome,
					m.sigla, m.tipo_protocolo, m.arquivo, ta.nome nome_arquivo, m.situacao,  m.documento,
                    l.texto documento_texto from {0}tab_titulo_modelo m, {0}tab_arquivo ta, {0}lov_titulo_modelo_tipo_doc l
					where m.arquivo = ta.id(+) and m.documento = l.id(+) and m.id = :id", EsquemaBanco);

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

				Comando comando = bancoDeDados.CriarComando(@"select m.id, m.tid, m.codigo, m.tipo, m.subtipo, 
				m.data_criacao, m.nome, m.sigla, m.tipo_protocolo, m.arquivo from {0}tab_titulo_modelo m where m.id = :id", EsquemaBanco);

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
	}
}