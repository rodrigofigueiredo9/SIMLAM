using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Data
{
	public class QueimaControladaDa
	{

		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		internal Historico Historico { get { return _historico; } }
		private String EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public QueimaControladaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(QueimaControlada caracterizacao, BancoDeDados banco)
		{
			if (caracterizacao == null)
			{
				throw new Exception("A Caracterização é nula.");
			}

			if (caracterizacao.Id <= 0)
			{
				Criar(caracterizacao, banco);
			}
			else
			{
				Editar(caracterizacao, banco);
			}
		}

		internal int? Criar(QueimaControlada caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Queima Controlada

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_queima_contr c (id, empreendimento, tid) 
				values ({0}seq_crt_queima_contr.nextval, :empreendimento, :tid) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Queimas Controladas

				if (caracterizacao.QueimasControladas != null && caracterizacao.QueimasControladas.Count > 0)
				{
					foreach (QueimaControladaQueima item in caracterizacao.QueimasControladas)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_queima_contr_queima c (id, queima_controlada, identificacao, area_croqui,area_requerida, tid)
															values ({0}seq_crt_queima_contr_queima.nextval, :queima_controlada, :identificacao, :area_croqui, :area_requerida, :tid)
															returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("queima_controlada", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("area_croqui", DbType.Decimal, 100, item.AreaCroqui);
						comando.AdicionarParametroEntrada("area_requerida", DbType.String, 100, item.AreaRequerida);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Cultivos

						if (item.Cultivos != null && item.Cultivos.Count > 0)
						{
							foreach (Cultivo itemAux in item.Cultivos)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_queima_contr_tipo_cult c (id, queima_contr_queima, tipo_cultivo, nome_finalidade, area_queima, tid)
																	values ({0}seq_crt_queima_contr_tipo_cult.nextval, :queima_contr_queima, :tipo_cultivo, :nome_finalidade, :area_queima, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("queima_contr_queima", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo_cultivo", itemAux.CultivoTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("nome_finalidade", itemAux.FinalidadeNome, DbType.String);
								comando.AdicionarParametroEntrada("tipo_cultivo", itemAux.CultivoTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("area_queima", itemAux.AreaQueima, DbType.Decimal);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.queimacontrolada, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(QueimaControlada caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Queima Controlada

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_queima_contr c set c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_queima_contr_tipo_cult c where c.queima_contr_queima in 
				(select a.id from {0}crt_queima_contr_queima a where a.queima_controlada = :queima_controlada", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.id", DbType.Int32, caracterizacao.QueimasControladas.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("queima_controlada", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (QueimaControladaQueima item in caracterizacao.QueimasControladas)
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}crt_queima_contr_tipo_cult c where c.queima_contr_queima in (select a.id
														from {0}crt_queima_contr_queima a where a.queima_controlada = :queima_controlada
														and a.id = :queima_contr_queima)", EsquemaBanco);
					comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Cultivos.Select(x => x.Id).ToList()));

					comando.AdicionarParametroEntrada("queima_contr_queima", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("queima_controlada", caracterizacao.Id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				//Cultivos
				comando = bancoDeDados.CriarComando("delete from {0}crt_queima_contr_queima c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.queima_controlada = :queima_controlada{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.QueimasControladas.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("queima_controlada", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Queimas Controladas

				if (caracterizacao.QueimasControladas != null && caracterizacao.QueimasControladas.Count > 0)
				{
					foreach (QueimaControladaQueima item in caracterizacao.QueimasControladas)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_queima_contr_queima c set c.identificacao = :identificacao,
																c.queima_controlada = :queima_controlada, c.area_croqui = :area_croqui,
																c.area_requerida = :area_requerida, c.tid = :tid where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("queima_controlada", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_queima_contr_queima c (id, queima_controlada, identificacao, area_croqui,area_requerida, tid)
															values ({0}seq_crt_queima_contr_queima.nextval, :queima_controlada, :identificacao, :area_croqui, :area_requerida, :tid)
															returning c.id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("queima_controlada", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_requerida", item.AreaRequerida, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#region Cultivos

						if (item.Cultivos != null && item.Cultivos.Count > 0)
						{
							foreach (Cultivo itemAux in item.Cultivos)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_queima_contr_tipo_cult c set c.queima_contr_queima = :queima_contr_queima, c.tipo_cultivo = :tipo_cultivo, 
																		c.nome_finalidade = :nome_finalidade, c.area_queima = :area_queima, c.tid = :tid where c.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_queima_contr_tipo_cult c (id, queima_contr_queima, tipo_cultivo, nome_finalidade, area_queima, tid)
																		values ({0}seq_crt_queima_contr_tipo_cult.nextval, :queima_contr_queima, :tipo_cultivo, :nome_finalidade, :area_queima, :tid)", EsquemaBanco);
								}

								comando.AdicionarParametroEntrada("queima_contr_queima", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo_cultivo", itemAux.CultivoTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("nome_finalidade", itemAux.FinalidadeNome, DbType.String);
								comando.AdicionarParametroEntrada("area_queima", itemAux.AreaQueima, DbType.String);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.queimacontrolada, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_queima_contr c where c.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				int id = 0;
				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					id = Convert.ToInt32(retorno);
				}

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}crt_queima_contr c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.queimacontrolada, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_queima_contr_tipo_cult r where r.queima_contr_queima in (select d.id from {0}crt_queima_contr_queima d where d.queima_controlada = :caracterizacao);" +
					@"delete from {0}crt_queima_contr_queima b where b.queima_controlada = :caracterizacao;" +
					@"delete from {0}crt_queima_contr e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.QueimaControlada, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal QueimaControlada ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			QueimaControlada caracterizacao = new QueimaControlada();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_queima_contr s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal QueimaControlada Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			QueimaControlada caracterizacao = new QueimaControlada();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_queima_contr s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal QueimaControlada Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			QueimaControlada caracterizacao = new QueimaControlada();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Queima Controlada

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.tid from {0}crt_queima_contr c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Queimas Controladas

				comando = bancoDeDados.CriarComando(@"select c.id, c.queima_controlada, c.identificacao, c.area_croqui, c.area_requerida,
													c.tid from {0}crt_queima_contr_queima c where c.queima_controlada = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					QueimaControladaQueima queima = null;

					while (reader.Read())
					{
						queima = new QueimaControladaQueima();
						queima.Id = Convert.ToInt32(reader["id"]);
						queima.Tid = reader["tid"].ToString();
						queima.Identificacao = reader["identificacao"].ToString();

						if (reader["area_requerida"] != null && !Convert.IsDBNull(reader["area_requerida"]))
						{
							queima.AreaRequerida = Convert.ToDecimal(reader["area_requerida"]).ToString("N2");
						}

						queima.AreaCroqui = reader.GetValue<decimal>("area_croqui");

						#region Cultivos

						comando = bancoDeDados.CriarComando(@"select c.id, c.queima_contr_queima, c.tipo_cultivo, lc.texto texto_cultivo, c.nome_finalidade, c.area_queima, c.tid
															from {0}crt_queima_contr_tipo_cult c, {0}lov_crt_queima_c_cultivo_tipo lc where c.tipo_cultivo = lc.id
															and c.queima_contr_queima = :queima_contr_queima", EsquemaBanco);

						comando.AdicionarParametroEntrada("queima_contr_queima", queima.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Cultivo cultivo = null;

							while (readerAux.Read())
							{
								cultivo = new Cultivo();
								cultivo.Id = Convert.ToInt32(readerAux["id"]);
								cultivo.Tid = readerAux["tid"].ToString();
								cultivo.CultivoTipo = Convert.ToInt32(readerAux["tipo_cultivo"]);
								cultivo.CultivoTipoTexto = readerAux["texto_cultivo"].ToString();
								

								if (readerAux["area_queima"] != null && !Convert.IsDBNull(readerAux["area_queima"]))
								{
									cultivo.AreaQueima = readerAux["area_queima"].ToString();
									cultivo.AreaQueimaTexto = Convert.ToDecimal(cultivo.AreaQueima).ToString("N2");
								}

								if (readerAux["nome_finalidade"] != null && !Convert.IsDBNull(readerAux["nome_finalidade"]))
								{
									cultivo.FinalidadeNome = readerAux["nome_finalidade"].ToString();
									cultivo.CultivoTipoTexto = cultivo.FinalidadeNome;
								}

								queima.Cultivos.Add(cultivo);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.QueimasControladas.Add(queima);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		private QueimaControlada ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			QueimaControlada caracterizacao = new QueimaControlada();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Queima Controlada

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.tid 
				from {0}hst_crt_queima_contr c where c.queima_controlada_id = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento_id"]);
						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Queimas Controladas

				comando = bancoDeDados.CriarComando(@"select c.id, c.queima_contr_queima_id, c.identificacao, c.area_croqui,
													c.area_requerida, c.tid from {0}hst_crt_queima_contr_queima c where c.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					QueimaControladaQueima queima = null;

					while (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						queima = new QueimaControladaQueima();
						queima.Id = Convert.ToInt32(reader["queima_contr_queima_id"]);
						queima.Tid = reader["tid"].ToString();
						queima.Identificacao = reader["identificacao"].ToString();
						queima.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						queima.AreaRequerida = reader["area_requerida"].ToString();

						#region Cultivos

						comando = bancoDeDados.CriarComando(@"select c.queima_tipo_cultivo_id, c.tipo_cultivo_id, c.tipo_cultivo_texto, 
															c.nome_finalidade, c.tid from {0}hst_crt_queima_contr_tipo_cult c 
															where c.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Cultivo cultivo = null;

							while (readerAux.Read())
							{
								cultivo = new Cultivo();
								cultivo.Id = Convert.ToInt32(readerAux["queima_tipo_cultivo_id"]);
								cultivo.CultivoTipo = Convert.ToInt32(readerAux["tipo_cultivo_id"]);
								cultivo.CultivoTipoTexto = readerAux["tipo_cultivo_texto"].ToString();
								cultivo.Tid = readerAux["tid"].ToString();

								if (readerAux["nome_finalidade"] != null && !Convert.IsDBNull(readerAux["nome_finalidade"]))
								{
									cultivo.FinalidadeNome = readerAux["nome_finalidade"].ToString();
								}

								queima.Cultivos.Add(cultivo);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.QueimasControladas.Add(queima);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal QueimaControlada ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{

			QueimaControlada caracterizacao = new QueimaControlada();
			caracterizacao.EmpreendimentoId = empreendimento;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados Geo
				/*Verificar classificação com analista*/
				Comando comando = bancoDeDados.CriarComando(@"
					select a.atividade,
								   a.codigo             identificacao,
								   a.geometry.sdo_gtype geometria_tipo,
								   a.area_m2            area_croqui
							  from {1}geo_aativ       a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							union all
							select a.atividade,
								   a.codigo             identificacao,
								   a.geometry.sdo_gtype geometria_tipo,
								   null                 area_croqui
							  from {1}geo_lativ       a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							union all
							select a.atividade,
								   a.codigo             identificacao,
								   a.geometry.sdo_gtype geometria_tipo,
								   null                 area_croqui
							  from {1}geo_pativ       a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							union all
							select a.atividade,
								   a.codigo             identificacao,
								   a.geometry.sdo_gtype geometria_tipo,
								   a.area_m2            area_croqui
							  from {1}geo_aiativ      a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.QueimaControlada, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					QueimaControladaQueima queima = null;
					while (reader.Read())
					{
						queima = new QueimaControladaQueima();
						queima.Identificacao = reader["identificacao"].ToString();

						queima.AreaCroqui = reader.GetValue<decimal>("area_croqui");

						caracterizacao.QueimasControladas.Add(queima);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		#endregion
	}
}