using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilvicultura.Data
{
	public class SilviculturaDa
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

		public SilviculturaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Silvicultura caracterizacao, BancoDeDados banco)
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

		internal int? Criar(Silvicultura caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_silvicultura c (id, empreendimento, tid) 
				values ({0}seq_crt_silvicultura.nextval, :empreendimento, :tid) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Silviculturas

				if (caracterizacao.Silviculturas != null && caracterizacao.Silviculturas.Count > 0)
				{
					foreach (SilviculturaSilvicult item in caracterizacao.Silviculturas)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_silvicultura_silv c (id, caracterizacao, identificacao, geometria, area_croqui, tid)
															values ({0}seq_crt_silvicultura_silv.nextval, :caracterizacao, :identificacao, :geometria, :area_croqui, :tid)
															returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", item.Identificacao, DbType.String);
						comando.AdicionarParametroEntrada("geometria", item.GeometriaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroquiHa.Convert(eMetrica.HaToM2), DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Culturas Florestais

						if (item.Culturas != null && item.Culturas.Count > 0)
						{
							foreach (CulturaFlorestal itemAux in item.Culturas)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_silvicultura_cult c (id, caracterizacao, silvicultura_id, cultura, area, especificar, tid)
																	values ({0}seq_crt_silvicultura_cult.nextval, :caracterizacao, :silvicultura_id, :cultura, :area, :especificar, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("silvicultura_id", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("cultura", itemAux.CulturaTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("area", itemAux.AreaCulturaHa.Convert(eMetrica.HaToM2), DbType.Decimal);
								comando.AdicionarParametroEntrada("especificar", (itemAux.CulturaTipo == (int)eCulturaFlorestal.Outros) ? itemAux.CulturaTipoTexto : (object)DBNull.Value, DbType.String);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (SilviculturaArea area in caracterizacao.Areas)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_silvicultura_areas(id, caracterizacao, tipo, valor, tid)
						values ({0}seq_crt_silvicultura_areas.nextval, :caracterizacao, :tipo, :valor, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", area.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", area.Valor, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.silvicultura, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(Silvicultura caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura c set c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_silvicultura_cult c where c.silvicultura_id in 
				(select a.id from {0}crt_silvicultura_silv a where a.caracterizacao = :caracterizacao", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.id", DbType.Int32, caracterizacao.Silviculturas.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (SilviculturaSilvicult item in caracterizacao.Silviculturas)
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}crt_silvicultura_cult c 
					where c.silvicultura_id in (select a.id from {0}crt_silvicultura_silv a where a.caracterizacao = :caracterizacao and a.id = :silvicultura)", EsquemaBanco);
					comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Culturas.Select(x => x.Id).ToList()));

					comando.AdicionarParametroEntrada("silvicultura", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_silvicultura_silv c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Silviculturas.Select(x => x.Id).ToList());

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				//Areas
				foreach (SilviculturaSilvicult silvicultura in caracterizacao.Silviculturas)
				{
					comando = bancoDeDados.CriarComando("delete from {0}crt_silvicultura_areas c ", EsquemaBanco);
					comando.DbCommand.CommandText += String.Format("where c.caracterizacao = :caracterizacao{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Areas.Select(x => x.Id).ToList()));
					comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Silviculturas

				if (caracterizacao.Silviculturas != null && caracterizacao.Silviculturas.Count > 0)
				{
					foreach (SilviculturaSilvicult item in caracterizacao.Silviculturas)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura_silv c set c.identificacao = :identificacao,
																c.caracterizacao = :caracterizacao, c.geometria = :geometria,
																c.area_croqui = :area_croqui, c.tid = :tid where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_silvicultura_silv c (id, caracterizacao, identificacao, geometria, area_croqui, tid)
																values ({0}seq_crt_silvicultura_silv.nextval, :caracterizacao, :identificacao, :geometria, :area_croqui, :tid)
																returning c.id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("identificacao", item.Identificacao, DbType.String);
						comando.AdicionarParametroEntrada("geometria", item.GeometriaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroquiHa.Convert(eMetrica.HaToM2), DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#region Culturas Florestais

						if (item.Culturas != null && item.Culturas.Count > 0)
						{
							foreach (CulturaFlorestal itemAux in item.Culturas)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura_cult c set c.caracterizacao = :caracterizacao, c.silvicultura_id = :silvicultura_id, 
																		c.cultura = :cultura, c.area = :area, c.especificar = :especificar, c.tid = :tid where c.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_silvicultura_cult c (id, caracterizacao, silvicultura_id, cultura, area, especificar, tid)
																		values ({0}seq_crt_silvicultura_cult.nextval, :caracterizacao, :silvicultura_id, :cultura, :area, :especificar, :tid) returning c.id into :id", EsquemaBanco);
									comando.AdicionarParametroSaida("id", DbType.Int32);
								}

								comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("silvicultura_id", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("cultura", itemAux.CulturaTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("area", itemAux.AreaCulturaHa.Convert(eMetrica.HaToM2), DbType.Decimal);
								comando.AdicionarParametroEntrada("especificar", (itemAux.CulturaTipo == (int)eCulturaFlorestal.Outros) ? itemAux.CulturaTipoTexto : (object)DBNull.Value, DbType.String);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);

								if (itemAux.Id <= 0)
								{
									itemAux.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
								}
							}
						}

						#endregion

					}
				}

				#endregion

				#region Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (SilviculturaArea area in caracterizacao.Areas)
					{
						if (area.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura_areas c set c.tipo = :tipo, c.valor = :valor, 
							c.tid = :tid where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", area.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_silvicultura_areas(id, caracterizacao, tipo, valor, tid)
							values ({0}seq_crt_silvicultura_areas.nextval, :caracterizacao, :tipo, :valor, :tid)", EsquemaBanco);

							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("tipo", area.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", area.Valor, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.silvicultura, eHistoricoAcao.atualizar, bancoDeDados, null);

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

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_silvicultura c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.silvicultura, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_silvicultura_cult m where m.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_silvicultura_areas b where b.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_silvicultura_silv m where m.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_silvicultura e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.Silvicultura, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal Silvicultura ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Silvicultura caracterizacao = new Silvicultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_silvicultura s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal Silvicultura Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Silvicultura caracterizacao = new Silvicultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_silvicultura s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal Silvicultura Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Silvicultura caracterizacao = new Silvicultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.tid from {0}crt_silvicultura c where c.id = :id", EsquemaBanco);

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

				#region Silviculturas

				comando = bancoDeDados.CriarComando(@"select c.id, c.identificacao, lv.id geometria, lv.texto geometria_texto, c.area_croqui, c.tid 
													from {0}crt_silvicultura_silv c, lov_crt_geometria_tipo lv 
													where c.caracterizacao = :id and c.geometria = lv.id order by c.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					SilviculturaSilvicult silvicultura = null;

					while (reader.Read())
					{
						silvicultura = new SilviculturaSilvicult();
						silvicultura.Id = Convert.ToInt32(reader["id"]);
						silvicultura.Tid = reader["tid"].ToString();
						silvicultura.Identificacao = reader["identificacao"].ToString();
						silvicultura.AreaCroqui = reader.GetValue<Decimal>("area_croqui");
						silvicultura.GeometriaTipo = Convert.ToInt32(reader["geometria"]);
						silvicultura.GeometriaTipoTexto = reader["geometria_texto"].ToString();
						silvicultura.AreaCroquiHa = silvicultura.AreaCroqui.Convert(eMetrica.M2ToHa);

						#region Culturas Florestais

						comando = bancoDeDados.CriarComando(@"select c.id, lc.id cultura, lc.texto cultura_texto, c.area, c.especificar, c.tid
															from {0}crt_silvicultura_cult c, {0}lov_crt_silvicultura_cult_fl lc where c.cultura = lc.id
															and c.silvicultura_id = :silvicultura", EsquemaBanco);

						comando.AdicionarParametroEntrada("silvicultura", silvicultura.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							CulturaFlorestal cultura = null;

							while (readerAux.Read())
							{
								cultura = new CulturaFlorestal();
								cultura.Id = Convert.ToInt32(readerAux["id"]);
								cultura.Tid = readerAux["tid"].ToString();
								cultura.CulturaTipo = Convert.ToInt32(readerAux["cultura"]);
								cultura.CulturaTipoTexto = readerAux["cultura_texto"].ToString();
								cultura.AreaCulturaHa = readerAux.GetValue<Decimal>("area").Convert(eMetrica.M2ToHa);
								cultura.AreaCulturaTexto = cultura.AreaCulturaHa.ToStringTrunc(4);

								if (readerAux["especificar"] != null && !Convert.IsDBNull(readerAux["especificar"]))
								{
									cultura.EspecificarTexto = readerAux["especificar"].ToString();
									cultura.CulturaTipoTexto = cultura.EspecificarTexto;
								}

								silvicultura.Culturas.Add(cultura);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.Silviculturas.Add(silvicultura);
					}

					reader.Close();
				}

				#endregion

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id, a.tipo, la.texto tipo_texto, a.valor, a.tid from {0}crt_silvicultura_areas a, {0}lov_crt_silvicultura_area la 
															where a.tipo = la.id and a.caracterizacao = :caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
				{
					SilviculturaArea item;
					while (readerAux2.Read())
					{
						item = new SilviculturaArea();
						item.Id = Convert.ToInt32(readerAux2["id"]);
						item.Tid = readerAux2["tid"].ToString();
						item.Tipo = Convert.ToInt32(readerAux2["tipo"]);
						item.TipoTexto = readerAux2["tipo_texto"].ToString();
						item.Valor = readerAux2.GetValue<Decimal>("valor");

						caracterizacao.Areas.Add(item);
					}

					readerAux2.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		private Silvicultura ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Silvicultura caracterizacao = new Silvicultura();
			int hst = 0;
			int hstCaracterizacao = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.tid 
				from {0}hst_crt_silvicultura c where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);
						hstCaracterizacao = hst;

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

				#region Silviculturas

				comando = bancoDeDados.CriarComando(@"select c.id, c.silvicultura_silv_id, c.identificacao, c.area_croqui, c.geometria_id, c.geometria_texto, c.tid 
													from {0}hst_crt_silvicultura_silv c where c.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					SilviculturaSilvicult silvicultura = null;

					while (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						silvicultura = new SilviculturaSilvicult();
						silvicultura.Id = Convert.ToInt32(reader["silvicultura_silv_id"]);
						silvicultura.Tid = reader["tid"].ToString();
						silvicultura.Identificacao = reader["identificacao"].ToString();
						silvicultura.AreaCroqui = reader.GetValue<Decimal>("area_croqui");
						silvicultura.GeometriaTipo = Convert.ToInt32(reader["geometria_id"]);
						silvicultura.GeometriaTipoTexto = reader["geometria_texto"].ToString();

						#region Culturas Florestais

						comando = bancoDeDados.CriarComando(@"select c.silvicultura_cult_id id, c.cultura_id, c.cultura_texto, 
															c.area, c.especificar, c.tid from {0}hst_crt_silvicultura_cult c 
															where c.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							CulturaFlorestal cultura = null;

							while (readerAux.Read())
							{
								cultura = new CulturaFlorestal();
								cultura.Id = Convert.ToInt32(readerAux["id"]);
								cultura.CulturaTipo = Convert.ToInt32(readerAux["cultura_id"]);
								cultura.CulturaTipoTexto = readerAux["cultura_texto"].ToString();
								cultura.Tid = readerAux["tid"].ToString();

								if (readerAux["especificar"] != null && !Convert.IsDBNull(readerAux["especificar"]))
								{
									cultura.EspecificarTexto = readerAux["especificar"].ToString();
									cultura.CulturaTipoTexto = cultura.EspecificarTexto;
								}

								silvicultura.Culturas.Add(cultura);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.Silviculturas.Add(silvicultura);
					}

					reader.Close();
				}

				#endregion

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.silvicultura_area_id, a.tipo_id, a.tipo_texto, a.valor, a.tid 
													from {0}hst_crt_silvicultura_areas a 
													where a.id_hst = :hst_caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst_caracterizacao", hstCaracterizacao, DbType.Int32);

				using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
				{
					SilviculturaArea item;
					while (readerAux2.Read())
					{
						item = new SilviculturaArea();
						item.Id = Convert.ToInt32(readerAux2["silvicultura_area_id"]);
						item.Tid = readerAux2["tid"].ToString();
						item.Tipo = Convert.ToInt32(readerAux2["tipo_id"]);
						item.TipoTexto = readerAux2["tipo_texto"].ToString();
						item.Valor = readerAux2.GetValue<Decimal>("valor");

						caracterizacao.Areas.Add(item);
					}

					readerAux2.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal Silvicultura ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{

			Silvicultura caracterizacao = new Silvicultura();
			caracterizacao.EmpreendimentoId = empreendimento;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados Geo

				Comando comando = bancoDeDados.CriarComando(@"
					select a.atividade,
								   a.codigo             identificacao,
								   3 geometria_tipo,
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
								   2 geometria_tipo,
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
								   1 geometria_tipo,
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
								   3 geometria_tipo,
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
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Silvicultura, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					SilviculturaSilvicult silvicultura = null;
					while (reader.Read())
					{
						silvicultura = new SilviculturaSilvicult();
						silvicultura.Identificacao = reader["identificacao"].ToString();
						silvicultura.GeometriaTipo = Convert.ToInt32(reader["geometria_tipo"]);
						silvicultura.AreaCroqui = reader.GetValue<Decimal>("area_croqui");
						silvicultura.AreaCroquiHa = silvicultura.AreaCroqui.Convert(eMetrica.M2ToHa);

						silvicultura.GeometriaTipoTexto = _caracterizacaoConfig.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyCaracterizacaoGeometriaTipo).
										SingleOrDefault(x => x.Id == (silvicultura.GeometriaTipo).ToString()).Texto;

						caracterizacao.Silviculturas.Add(silvicultura);
					}

					reader.Close();
				}

				#endregion

				#region Areas

				int projetoDomId = 0;

				comando = bancoDeDados.CriarComando(@"select g.id from {0}crt_projeto_geo g where g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao_dom", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao_dom", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				projetoDomId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));


				comando = bancoDeDados.CriarComando(@"(select sum(t.area_m2) area, 'ATP_CROQUI' area_tipo  from {1}geo_atp t where t.projeto = :projetoDominialidade)
					union all
					(select sum(t.area_m2) area, 'AVN' area_tipo from {1}geo_avn t where t.projeto = :projetoDominialidade) 
					union all
					(select sum(t.area_m2) area, 'AA_FLORESTA_PLANTADA' area_tipo from {1}geo_aa t where t.projeto = :projetoDominialidade and t.vegetacao = 'FLORESTA-PLANTADA' ) 
					union all
					(select sum(t.area_m2) area, t.tipo area_tipo from {1}geo_areas_calculadas t where t.projeto = :projetoDominialidade and t.tipo in ('APP_AVN', 'APP_AA_USO', 'APP_AA_REC' ) group by t.tipo)
					union all
					(select sum(t.area_m2) area, 'APP' area_tipo from {1}geo_areas_calculadas t where t.projeto = :projetoDominialidade and t.tipo = 'APP_APMP' group by t.tipo)
					union all 
					(select sum(t.area_m2) area, 'ARL_'||t.situacao area_tipo from {1}geo_arl t where t.projeto = :projetoDominialidade and t.situacao in ('PRESERV', 'USO', 'REC', 'D' ) group by t.situacao)", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projetoDominialidade", projetoDomId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.Areas.Add(new SilviculturaArea()
						{
							Tipo = (int)ObterEnumeSilviculturaArea(reader["area_tipo"].ToString()),
							Valor = reader.GetValue<Decimal>("area")
						});
					}

					reader.Close();
				}
			}

				#endregion

			return caracterizacao;
		}

		public eSilviculturaArea ObterEnumeSilviculturaArea(string tipoArea)
		{
			switch (tipoArea)
			{
				case "ATP_CROQUI":
					return eSilviculturaArea.ATP_CROQUI;
				case "AVN":
					return eSilviculturaArea.AVN;
				case "AA_FLORESTA_PLANTADA":
					return eSilviculturaArea.AA_FLORESTA_PLANTADA;
				case "APP":
					return eSilviculturaArea.APP;
				case "APP_AVN":
					return eSilviculturaArea.APP_AVN;
				case "APP_AA_USO":
					return eSilviculturaArea.APP_AA_USO;
				case "APP_AA_REC":
					return eSilviculturaArea.APP_AA_REC;
				case "APP_D":
					return eSilviculturaArea.APP_D;
				case "ARL_PRESERV":
					return eSilviculturaArea.ARL_AVN;
				case "ARL_USO":
					return eSilviculturaArea.ARL_USO;
				case "ARL_REC":
					return eSilviculturaArea.ARL_REC;
				case "ARL_D":
					return eSilviculturaArea.ARL_D;
			}

			throw new Exception("Tipo de area de eSilviculturaArea nao encontrada.");
		}

		#endregion

		#region Validações

		internal bool ExisteCaracterizacao(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(s.id) from {0}crt_silvicultura s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}
		
		internal bool TemARL(int empreendimentoId, BancoDeDados banco = null)
		{
			bool temARL = false;
			object valor = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_empreendimento_endereco t, {0}crt_projeto_geo cpg, {1}geo_arl ga
				where t.correspondencia = 0 and t.empreendimento = cpg.empreendimento and cpg.caracterizacao = 1 /*Dominialidade*/
				and cpg.id = ga.projeto and t.zona = 2 /*Rural*/ and t.empreendimento = :empreendimentoId", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				valor = bancoDeDados.ExecutarScalar(comando);

				temARL = valor != null && !Convert.IsDBNull(valor) && Convert.ToInt32(valor) > 0;
			}

			return temARL;
		}

		internal bool TemARLDesconhecida(int empreendimentoId, BancoDeDados banco = null)
		{
			bool temARLDesconhecida = false;
			object valor = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_empreendimento_endereco t, {0}crt_projeto_geo cpg, {1}geo_arl ga
				where t.correspondencia = 0 and t.empreendimento = cpg.empreendimento and cpg.caracterizacao = 1 /*Dominialidade*/
				and cpg.id = ga.projeto and ga.situacao = 'D'/*NÃO CARACTERIZADA*/ and t.zona = 2 /*Rural*/ and t.empreendimento = :empreendimentoId", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				valor = bancoDeDados.ExecutarScalar(comando);

				temARLDesconhecida = valor != null && !Convert.IsDBNull(valor) && Convert.ToInt32(valor) > 0;
			}

			return temARLDesconhecida;
		}

		#endregion
	}
}