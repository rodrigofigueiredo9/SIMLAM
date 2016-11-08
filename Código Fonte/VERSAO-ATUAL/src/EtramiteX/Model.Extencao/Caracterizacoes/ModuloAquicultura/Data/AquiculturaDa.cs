using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAquicultura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloAquicultura.Data
{
	public class AquiculturaDa
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

		public AquiculturaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Aquicultura caracterizacao, BancoDeDados banco)
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

		internal int? Criar(Aquicultura caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Aquicultura

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}crt_aquicultura c(id, empreendimento, tid) values 
				(seq_crt_aquicultura.nextval, :empreendimento, :tid ) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Aquiculturas

				foreach (AquiculturaAquicult item in caracterizacao.AquiculturasAquicult)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_aquicultura_aquic c
					(id, atividade, caracterizacao, area_inundada, num_viveiros_escavados, area_cultivo, num_unid_cultivo, 
					geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
					values(seq_crt_aquicultura_aquic.nextval, :atividade, :caracterizacao, :area_inundada, :num_viveiros_escavados, :area_cultivo, :num_unid_cultivo, 
					:geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) returning c.id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.Int32);
					comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("area_inundada", (String.IsNullOrWhiteSpace(item.AreaInundadaTotal)) ? (object)DBNull.Value : item.AreaInundadaTotal, DbType.Decimal);
					comando.AdicionarParametroEntrada("num_viveiros_escavados", (String.IsNullOrWhiteSpace(item.NumViveiros)) ? (object)DBNull.Value : item.NumViveiros, DbType.Decimal);
					comando.AdicionarParametroEntrada("area_cultivo", (String.IsNullOrWhiteSpace(item.AreaCultivo)) ? (object)DBNull.Value : item.AreaCultivo, DbType.Decimal);
					comando.AdicionarParametroEntrada("num_unid_cultivo", (String.IsNullOrWhiteSpace(item.NumUnidadeCultivos)) ? (object)DBNull.Value : item.NumUnidadeCultivos, DbType.Decimal);
					comando.AdicionarParametroEntrada("geometria_id", item.CoordenadaAtividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_tipo", item.CoordenadaAtividade.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_coord_atv_x", item.CoordenadaAtividade.CoordX, DbType.Decimal);
					comando.AdicionarParametroEntrada("geometria_coord_atv_y", item.CoordenadaAtividade.CoordY, DbType.Decimal);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					#region Cultivos

					foreach (Cultivo cultivo in item.Cultivos)
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_aquicultura_cultivos c 
						(id, caracterizacao, aquicultura, identificador, volume, tid) values 
						(seq_crt_aquicultura_cultivos.nextval, :caracterizacao, :aquicultura, :identificador, :volume, :tid) returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("aquicultura", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificador", cultivo.Identificador, DbType.Int32);
						comando.AdicionarParametroEntrada("volume", cultivo.Volume, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);
						bancoDeDados.ExecutarNonQuery(comando);

						cultivo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}

					#endregion
				}

				#endregion

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.aquicultura, eHistoricoAcao.criar, bancoDeDados, null);

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(Aquicultura caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Aquicultura

				Comando comando = bancoDeDados.CriarComando(@"
				update crt_aquicultura c set c.empreendimento = :empreendimento, c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Cultivos
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_aquicultura_cultivos c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.AquiculturasAquicult.SelectMany(x => x.Cultivos).Select(y => y.Id).ToList());
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Aquiculturas
				comando = bancoDeDados.CriarComando("delete from {0}crt_aquicultura_aquic c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.AquiculturasAquicult.Select(x => x.Id).ToList());
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Aquiculturas

				foreach (AquiculturaAquicult item in caracterizacao.AquiculturasAquicult)
				{

					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update {0}crt_aquicultura_aquic c set c.atividade = :atividade, c.caracterizacao = :caracterizacao, c.area_inundada = :area_inundada, 
						c.num_viveiros_escavados = :num_viveiros_escavados, c.area_cultivo = :area_cultivo, c.num_unid_cultivo = :num_unid_cultivo, 
						c.geometria_coord_atv_x = :geometria_coord_atv_x, c.geometria_coord_atv_y = :geometria_coord_atv_y,  c.geometria_id = :geometria_id, 
						c.geometria_tipo = :geometria_tipo, c.tid = :tid where c.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_aquicultura_aquic c 
						(id, atividade, caracterizacao, area_inundada, num_viveiros_escavados, area_cultivo, num_unid_cultivo, 
						geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
						values (seq_crt_aquicultura_aquic.nextval, :atividade, :caracterizacao, :area_inundada, :num_viveiros_escavados, :area_cultivo, :num_unid_cultivo, 
						:geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroSaida("id", DbType.Int32);
					}

					comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.Int32);
					comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("area_inundada", (String.IsNullOrWhiteSpace(item.AreaInundadaTotal)) ? (object)DBNull.Value : item.AreaInundadaTotal, DbType.Decimal);
					comando.AdicionarParametroEntrada("num_viveiros_escavados", (String.IsNullOrWhiteSpace(item.NumViveiros)) ? (object)DBNull.Value : item.NumViveiros, DbType.Decimal);
					comando.AdicionarParametroEntrada("area_cultivo", (String.IsNullOrWhiteSpace(item.AreaCultivo)) ? (object)DBNull.Value : item.AreaCultivo, DbType.Decimal);
					comando.AdicionarParametroEntrada("num_unid_cultivo", (String.IsNullOrWhiteSpace(item.NumUnidadeCultivos)) ? (object)DBNull.Value : item.NumUnidadeCultivos, DbType.Decimal);
					comando.AdicionarParametroEntrada("geometria_id", item.CoordenadaAtividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_tipo", item.CoordenadaAtividade.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_coord_atv_x", item.CoordenadaAtividade.CoordX, DbType.Decimal);
					comando.AdicionarParametroEntrada("geometria_coord_atv_y", item.CoordenadaAtividade.CoordY, DbType.Decimal);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);

					if (item.Id <= 0)
					{
						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}

					#region Cultivos

					foreach (Cultivo cultivo in item.Cultivos)
					{
						if (cultivo.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"
							update {0}crt_aquicultura_cultivos c set c.caracterizacao = :caracterizacao, c.aquicultura = :aquicultura,
							c.identificador = :identificador, c.volume = :volume, c.tid = :tid where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", cultivo.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_aquicultura_cultivos c 
							(id, caracterizacao, aquicultura, identificador, volume, tid) values 
							(seq_crt_aquicultura_cultivos.nextval, :caracterizacao, :aquicultura, :identificador, :volume, :tid ) 
							returning c.id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("aquicultura", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificador", cultivo.Identificador, DbType.Int32);
						comando.AdicionarParametroEntrada("volume", cultivo.Volume, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (cultivo.Id <= 0)
						{
							cultivo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}

					#endregion
				}

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.aquicultura, eHistoricoAcao.atualizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_aquicultura c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_aquicultura c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.aquicultura, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_aquicultura_cultivos c where c.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_aquicultura_aquic a where a.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_aquicultura e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.Aquicultura, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal Aquicultura ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Aquicultura caracterizacao = new Aquicultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_aquicultura s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal Aquicultura Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Aquicultura caracterizacao = new Aquicultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_aquicultura s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal Aquicultura Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Aquicultura caracterizacao = new Aquicultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.tid  from {0}crt_aquicultura c where c.id = :id", EsquemaBanco);

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

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Aquiculturas

				comando = bancoDeDados.CriarComando(@"select c.id, c.atividade, c.area_inundada,  c.num_viveiros_escavados, c.num_unid_cultivo, c.geometria_coord_atv_x, c.geometria_coord_atv_y, c.area_cultivo,
													c.geometria_id, c.geometria_tipo, c.tid  from {0}crt_aquicultura_aquic c where c.caracterizacao = :caracterizacao order by c.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				AquiculturaAquicult aquicultura = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						aquicultura = new AquiculturaAquicult();
						aquicultura.Id = Convert.ToInt32(reader["id"]);
						aquicultura.Atividade = Convert.ToInt32(reader["atividade"]);
						aquicultura.AreaInundadaTotal = reader["area_inundada"].ToString();
						aquicultura.NumViveiros = reader["num_viveiros_escavados"].ToString();
						aquicultura.NumUnidadeCultivos = reader["num_unid_cultivo"].ToString();
						aquicultura.AreaCultivo = reader["area_cultivo"].ToString();
						aquicultura.CoordenadaAtividade.Id = Convert.ToInt32(reader["geometria_id"]);
						aquicultura.CoordenadaAtividade.Tipo = Convert.ToInt32(reader["geometria_tipo"]);

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							aquicultura.CoordenadaAtividade.CoordX = Convert.ToDecimal(reader["geometria_coord_atv_x"]);
						}

						if (reader["geometria_coord_atv_y"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_y"]))
						{
							aquicultura.CoordenadaAtividade.CoordY = Convert.ToDecimal(reader["geometria_coord_atv_y"]);
						}

						#region Cultivos

						comando = bancoDeDados.CriarComando(@"select c.id, c.identificador, c.volume
															from {0}crt_aquicultura_cultivos c 
															where c.aquicultura = :aquicultura order by c.identificador", EsquemaBanco);

						comando.AdicionarParametroEntrada("aquicultura", aquicultura.Id, DbType.Int32);

						Cultivo cultivo = null;
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{

							while (readerAux.Read())
							{
								cultivo = new Cultivo();
								cultivo.Id = Convert.ToInt32(readerAux["id"]);
								cultivo.Identificador = readerAux["identificador"].ToString();
								cultivo.Volume = readerAux["volume"].ToString();

								aquicultura.Cultivos.Add(cultivo);

							}

							readerAux.Close();
						}
						#endregion

						caracterizacao.AquiculturasAquicult.Add(aquicultura);
					}

					reader.Close();
				}

				#endregion

			}

			return caracterizacao;
		}

		private Aquicultura ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Aquicultura caracterizacao = new Aquicultura();

			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento, c.tid  from {0}hst_crt_aquicultura c where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Aquiculturas

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					comando = bancoDeDados.CriarComando(@"select c.aquicultura_id id, c.atividade_id, c.area_inundada, c.num_viveiros_escavados, c.num_unid_cultivo, c.area_cultivo, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
														c.geometria_id, c.geometria_tipo, c.tid  from {0}hst_crt_aquicultura_aquic c where c.id_hst = :id_hst", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

					AquiculturaAquicult aquicultura = null;

					while (reader.Read())
					{
						aquicultura = new AquiculturaAquicult();
						aquicultura.Id = Convert.ToInt32(reader["id"]);
						aquicultura.Atividade = Convert.ToInt32(reader["atividade_id"]);

						aquicultura.AreaInundadaTotal = reader["area_inundada"].ToString();
						aquicultura.NumViveiros = reader["num_viveiros_escavados"].ToString();
						aquicultura.NumUnidadeCultivos = reader["num_unid_cultivo"].ToString();
						aquicultura.AreaCultivo = reader["area_cultivo"].ToString();

						aquicultura.CoordenadaAtividade.Id = Convert.ToInt32(reader["geometria_id"]);
						aquicultura.CoordenadaAtividade.Tipo = Convert.ToInt32(reader["geometria_tipo"]);

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							aquicultura.CoordenadaAtividade.CoordX = Convert.ToDecimal(reader["geometria_coord_atv_x"]);
						}

						if (reader["geometria_coord_atv_y"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_y"]))
						{
							aquicultura.CoordenadaAtividade.CoordY = Convert.ToDecimal(reader["geometria_coord_atv_y"]);
						}

						#region Cultivos

						comando = bancoDeDados.CriarComando(@"select c.cultivo id, c.identificador, c.volume, c.unidade, c.quantidade, m.tid 
															from {0}hst_crt_aquicultura_cultivos c where c.id_hst = :id order by c.identificador", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						Cultivo cultivo = null;
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{

							while (readerAux.Read())
							{
								cultivo = new Cultivo();
								cultivo.Id = Convert.ToInt32(readerAux["id"]);
								cultivo.Identificador = readerAux["identificador"].ToString();
								cultivo.Volume = readerAux["volume"].ToString();

								aquicultura.Cultivos.Add(cultivo);

							}

							readerAux.Close();

						}

						#endregion

						caracterizacao.AquiculturasAquicult.Add(aquicultura);
					}

					reader.Close();

				}

				#endregion

			}

			return caracterizacao;

		}

		internal object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco)
		{
			CaracterizacaoPDF caract = new CaracterizacaoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select ca.geometria_coord_atv_x, ca.geometria_coord_atv_y, ca.area_inundada, ca.num_viveiros_escavados, ca.num_unid_cultivo,
				(select sum(cc.volume) from crt_aquicultura_cultivos cc where cc.aquicultura = ca.id) volume, ca.area_cultivo
				from crt_aquicultura c, crt_aquicultura_aquic ca where ca.caracterizacao = c.id and c.empreendimento = :empreendimento and ca.atividade = :atividade", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["area_inundada"] != null && !Convert.IsDBNull(reader["area_inundada"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Área total inundada (ha)", Valor = reader.GetValue<string>("area_inundada") });
						}

						if (reader["num_viveiros_escavados"] != null && !Convert.IsDBNull(reader["num_viveiros_escavados"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Nº de viveiros escavados", Valor = reader.GetValue<string>("num_viveiros_escavados") });
						}

						if (reader["num_unid_cultivo"] != null && !Convert.IsDBNull(reader["num_unid_cultivo"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Nº de unidade de cultivo", Valor = reader.GetValue<string>("num_unid_cultivo") });
						}

						if (reader["volume"] != null && !Convert.IsDBNull(reader["volume"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Volume total de cultivo (m³)", Valor = reader.GetValue<string>("volume") });
						}

						if (reader["area_cultivo"] != null && !Convert.IsDBNull(reader["area_cultivo"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Área de cultivo (m²)", Valor = reader.GetValue<string>("area_cultivo") });
						}

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caract.EastingLongitude = reader.GetValue<string>("geometria_coord_atv_x");
							caract.NorthingLatitude = reader.GetValue<string>("geometria_coord_atv_y");
						}
					}

					reader.Close();
				}
			}

			return caract;
		}

		internal List<int> ObterAtividadesCaracterizacao(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select b.atividade, a.empreendimento from crt_aquicultura a, crt_aquicultura_aquic b 
															where a.empreendimento = :empreendimento and b.caracterizacao = a.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarList<Int32>(comando);
			}
		}

		#endregion
	}
}