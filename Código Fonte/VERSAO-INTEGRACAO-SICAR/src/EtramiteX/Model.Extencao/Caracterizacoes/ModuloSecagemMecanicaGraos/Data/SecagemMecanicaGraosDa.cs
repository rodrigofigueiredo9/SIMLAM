using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos.Data
{
	public class SecagemMecanicaGraosDa
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

		public SecagemMecanicaGraosDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(SecagemMecanicaGraos caracterizacao, BancoDeDados banco)
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

		internal int? Criar(SecagemMecanicaGraos caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Secagem Mecanica de Graos

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_sec_mec_graos c(id, empreendimento, atividade, numero_secadores, geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
															values(seq_crt_sec_mec_graos.nextval, :empreendimento, :atividade, :numero_secadores, :geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) 
															returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_secadores", caracterizacao.NumeroSecadores, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_id", caracterizacao.CoordenadaAtividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_tipo", caracterizacao.CoordenadaAtividade.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_coord_atv_x", caracterizacao.CoordenadaAtividade.CoordX, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_coord_atv_y", caracterizacao.CoordenadaAtividade.CoordY, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Secadores

				if (caracterizacao.Secadores != null && caracterizacao.Secadores.Count > 0)
				{
					foreach (Secador item in caracterizacao.Secadores)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_sec_mec_graos_cap_sec c(id, caracterizacao, identificador, capacidade, tid)
															values(seq_crt_sec_mec_graos_cap_sec.nextval, :caracterizacao, :identificador, :capacidade, :tid)
															returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificador", item.Identificador, DbType.Int32);
						comando.AdicionarParametroEntrada("capacidade", item.Capacidade, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					}
				}

				#endregion

				#region Materias-Prima Florestal Consumida

				if (caracterizacao.MateriasPrimasFlorestais != null && caracterizacao.MateriasPrimasFlorestais.Count > 0)
				{
					foreach (MateriaPrima item in caracterizacao.MateriasPrimasFlorestais)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_sec_mec_graos_mat_fl_c m (id, caracterizacao, materia_prima_tipo, unidade, quantidade, tid)
															values(seq_crt_sec_mec_graos_mat_fl_c.nextval, :caracterizacao, :materia_prima_tipo, :unidade, :quantidade, :tid)
															returning m.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("materia_prima_tipo", item.MateriaPrimaConsumida, DbType.Int32);
						comando.AdicionarParametroEntrada("unidade", item.Unidade, DbType.Int32);
						comando.AdicionarParametroEntrada("quantidade", item.Quantidade, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.secagemmecanicagraos, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(SecagemMecanicaGraos caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Secagem Mecanica de Graos

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_sec_mec_graos c set c.empreendimento = :empreendimento, c.atividade = :atividade, c.numero_secadores = :numero_secadores,
															c.geometria_coord_atv_x = :geometria_coord_atv_x, c.geometria_coord_atv_y = :geometria_coord_atv_y, c.geometria_id = :geometria_id, c.geometria_tipo = :geometria_tipo, c.tid = :tid
															where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_secadores", caracterizacao.NumeroSecadores, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_id", caracterizacao.CoordenadaAtividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_tipo", caracterizacao.CoordenadaAtividade.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_coord_atv_x", caracterizacao.CoordenadaAtividade.CoordX, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_coord_atv_y", caracterizacao.CoordenadaAtividade.CoordY, DbType.Decimal);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Secadores
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_sec_mec_graos_cap_sec c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Secadores.Select(x => x.Id).ToList()));

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);


				//Materias-Prima Florestal Consumida
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_sec_mec_graos_mat_fl_c c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.MateriasPrimasFlorestais.Select(x => x.Id).ToList()));

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);


				#endregion

				#region Secadores

				if (caracterizacao.Secadores != null && caracterizacao.Secadores.Count > 0)
				{
					foreach (Secador item in caracterizacao.Secadores)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_sec_mec_graos_cap_sec c set c.caracterizacao = :caracterizacao,
																c.identificador = :identificador, c.capacidade = :capacidade, c.tid = :tid
																where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_sec_mec_graos_cap_sec c (id, caracterizacao, identificador, capacidade, tid)
																values (seq_crt_sec_mec_graos_cap_sec.nextval, :caracterizacao, :identificador, :capacidade, :tid )
																returning c.id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificador", item.Identificador, DbType.Int32);
						comando.AdicionarParametroEntrada("capacidade", item.Capacidade, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}
				}

				#endregion

				#region Materias-Prima Florestal Consumida

				if (caracterizacao.MateriasPrimasFlorestais != null && caracterizacao.MateriasPrimasFlorestais.Count > 0)
				{
					foreach (MateriaPrima item in caracterizacao.MateriasPrimasFlorestais)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_sec_mec_graos_mat_fl_c c set c.caracterizacao = :caracterizacao,
																c.materia_prima_tipo = :materia_prima_tipo, c.unidade = :unidade, 
																c.quantidade =  :quantidade, c.tid = :tid where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@" insert into {0}crt_sec_mec_graos_mat_fl_c c (id, caracterizacao, materia_prima_tipo, unidade, quantidade, tid)
																values (seq_crt_sec_mec_graos_mat_fl_c.nextval, :caracterizacao, :materia_prima_tipo, :unidade, :quantidade, :tid )
																returning c.id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("materia_prima_tipo", item.MateriaPrimaConsumida, DbType.Int32);
						comando.AdicionarParametroEntrada("unidade", item.Unidade, DbType.Int32);
						comando.AdicionarParametroEntrada("quantidade", item.Quantidade, DbType.Decimal);

						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.secagemmecanicagraos, eHistoricoAcao.atualizar, bancoDeDados, null);

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

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_sec_mec_graos c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_sec_mec_graos c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.secagemmecanicagraos, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_sec_mec_graos_cap_sec r where r.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_sec_mec_graos_mat_fl_c m where m.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_sec_mec_graos e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.SecagemMecanicaGraos, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal SecagemMecanicaGraos ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			SecagemMecanicaGraos caracterizacao = new SecagemMecanicaGraos();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_sec_mec_graos s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal SecagemMecanicaGraos Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			SecagemMecanicaGraos caracterizacao = new SecagemMecanicaGraos();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_sec_mec_graos s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal SecagemMecanicaGraos Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			SecagemMecanicaGraos caracterizacao = new SecagemMecanicaGraos();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Secagem Mecanica de Graos

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.atividade, c.numero_secadores, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															c.geometria_id, c.geometria_tipo, c.tid  from {0}crt_sec_mec_graos c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);

						caracterizacao.Atividade = Convert.ToInt32(reader["atividade"]);
						caracterizacao.NumeroSecadores = reader["numero_secadores"].ToString();
						caracterizacao.CoordenadaAtividade.Id = Convert.ToInt32(reader["geometria_id"]);
						caracterizacao.CoordenadaAtividade.Tipo = Convert.ToInt32(reader["geometria_tipo"]);

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caracterizacao.CoordenadaAtividade.CoordX = Convert.ToDecimal(reader["geometria_coord_atv_x"]);
						}

						if (reader["geometria_coord_atv_y"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_y"]))
						{
							caracterizacao.CoordenadaAtividade.CoordY = Convert.ToDecimal(reader["geometria_coord_atv_y"]);
						}

						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Secadores

				comando = bancoDeDados.CriarComando(@"select s.id, s.identificador, s.capacidade, s.tid from {0}crt_sec_mec_graos_cap_sec s 
													where s.caracterizacao = :caracterizacao order by s.identificador", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Secador secador = null;

					while (reader.Read())
					{
						secador = new Secador();
						secador.Id = Convert.ToInt32(reader["id"]);
						secador.Identificador = reader["identificador"].ToString();
						secador.Capacidade = reader["capacidade"].ToString();
						secador.Tid = reader["tid"].ToString();

						caracterizacao.Secadores.Add(secador);
					}

					reader.Close();
				}

				#endregion

				#region Materias-Prima Florestal Consumida

				comando = bancoDeDados.CriarComando(@"select m.id, m.materia_prima_tipo, lm.texto materia_prima_tipo_texto, m.unidade, lu.texto unidade_texto, m.quantidade, m.tid 
													from {0}crt_sec_mec_graos_mat_fl_c m, {0}lov_crt_sec_mec_graos_mat_pr_c lm, {0}lov_crt_unidade_medida lu
													where m.caracterizacao = :caracterizacao and lm.id = m.materia_prima_tipo and lu.id = m.unidade order by m.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					MateriaPrima materia = null;

					while (reader.Read())
					{
						materia = new MateriaPrima();
						materia.Id = Convert.ToInt32(reader["id"]);
						materia.MateriaPrimaConsumida = Convert.ToInt32(reader["materia_prima_tipo"]);
						materia.MateriaPrimaConsumidaTexto = reader["materia_prima_tipo_texto"].ToString();
						materia.Unidade = Convert.ToInt32(reader["unidade"]);
						materia.UnidadeTexto = reader["unidade_texto"].ToString();
						materia.Quantidade = reader["quantidade"].ToString();
						materia.Tid = reader["tid"].ToString();

						caracterizacao.MateriasPrimasFlorestais.Add(materia);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		private SecagemMecanicaGraos ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			SecagemMecanicaGraos caracterizacao = new SecagemMecanicaGraos();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Secagem Mecanica de Graos

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.atividade_id, c.numero_secadores, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															c.geometria_id, c.geometria_tipo, c.tid from {0}hst_crt_sec_mec_graos c 
															where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento_id"]);
						caracterizacao.Atividade = Convert.ToInt32(reader["atividade_id"]);
						caracterizacao.NumeroSecadores = reader["numero_secadores"].ToString();
						caracterizacao.CoordenadaAtividade.Id = Convert.ToInt32(reader["geometria_id"]);
						caracterizacao.CoordenadaAtividade.Tipo = Convert.ToInt32(reader["geometria_tipo"]);
						caracterizacao.Tid = reader["tid"].ToString();

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caracterizacao.CoordenadaAtividade.CoordX = Convert.ToDecimal(reader["geometria_coord_atv_x"]);
						}

						if (reader["geometria_coord_atv_y"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_y"]))
						{
							caracterizacao.CoordenadaAtividade.CoordY = Convert.ToDecimal(reader["geometria_coord_atv_y"]);
						}
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Secadores

				comando = bancoDeDados.CriarComando(@"select s.capacidade_secador id, s.identificador, s.capacidade, s.tid 
													from {0}hst_crt_sec_mec_graos_cap_sec s
													where s.id_hst = :id_hst order by s.identificador", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Secador secador = null;

					if (reader.Read())
					{
						secador = new Secador();
						secador.Id = Convert.ToInt32(reader["id"]);
						secador.Identificador = reader["identificador"].ToString();
						secador.Capacidade = reader["capacidade"].ToString();
						secador.Tid = reader["tid"].ToString();

						caracterizacao.Secadores.Add(secador);
					}

					reader.Close();
				}

				#endregion

				#region Materias-Prima Florestal Consumida

				comando = bancoDeDados.CriarComando(@"select m.materia_prima id, m.materia_prima_tipo, lm.texto materia_prima_tipo_texto, 
													m.unidade, lu.texto unidade_texto, m.quantidade, m.tid 
													from {0}hst_crt_sec_mec_graos_mat_fl_c m, {0}lov_crt_sec_mec_graos_mat_pr_c lm, {0}lov_crt_sec_mec_graos_unid_med lu
													where m.id_hst = :id_hst and lm.id = m.materia_prima_tipo and lu.id = m.unidade order by m.materia_prima", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					MateriaPrima materia = null;

					if (reader.Read())
					{
						materia = new MateriaPrima();
						materia.Id = Convert.ToInt32(reader["id"]);
						materia.MateriaPrimaConsumida = Convert.ToInt32(reader["materia_prima_tipo"]);
						materia.MateriaPrimaConsumidaTexto = reader["materia_prima_tipo_texto"].ToString();
						materia.Unidade = Convert.ToInt32(reader["unidade"]);
						materia.UnidadeTexto = reader["unidade_texto"].ToString();
						materia.Quantidade = reader["quantidade"].ToString();
						materia.Tid = reader["tid"].ToString();

						caracterizacao.MateriasPrimasFlorestais.Add(materia);
					}

					reader.Close();
				}

				#endregion

			}

			return caracterizacao;

		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			//Nº de secadores
			//Capacidade total instalada (L)
			//Coordenada da atividade
			CaracterizacaoPDF caract = new CaracterizacaoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select c.numero_secadores, (select sum(cap.capacidade) 
					from {0}crt_sec_mec_graos_cap_sec cap where cap.caracterizacao = c.id ) capacidade, c.geometria_coord_atv_x, 
					c.geometria_coord_atv_y from {0}crt_sec_mec_graos c where c.empreendimento = :empreendimento and c.atividade = :atividade", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["numero_secadores"] != null && !Convert.IsDBNull(reader["numero_secadores"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Nº de secadores", Valor = reader["numero_secadores"].ToString() });
						}

						if (reader["capacidade"] != null && !Convert.IsDBNull(reader["capacidade"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Capacidade total instalada (L)", Valor = reader["capacidade"].ToString() });
						}

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caract.EastingLongitude = Convert.ToDecimal(reader["geometria_coord_atv_x"]).ToString("F2");
							caract.NorthingLatitude = Convert.ToDecimal(reader["geometria_coord_atv_y"]).ToString("F2");
						}
					}

					reader.Close();
				}
			}

			return caract;
		}

		#endregion
	}
}