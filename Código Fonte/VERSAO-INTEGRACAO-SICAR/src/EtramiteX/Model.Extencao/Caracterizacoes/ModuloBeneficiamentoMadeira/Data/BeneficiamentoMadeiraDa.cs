using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira.Data
{
	public class BeneficiamentoMadeiraDa
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

		public BeneficiamentoMadeiraDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(BeneficiamentoMadeira caracterizacao, BancoDeDados banco)
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

		internal int? Criar(BeneficiamentoMadeira caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Beneficiamento e tratamento de madeira

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_benefic_madeira c(id, empreendimento, tid)
															values(seq_crt_benefic_madeira.nextval, :empreendimento, :tid ) 
															returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Beneficiamentos

				foreach (BeneficiamentoMadeiraBeneficiamento item in caracterizacao.Beneficiamentos)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}crt_benefic_madeira_benef c(id, atividade, caracterizacao, volume_madeira_serrar, volume_madeira_processar, equip_contr_poluicao_sonora, geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
															values(seq_crt_benefic_madeira_benef.nextval, :atividade, :caracterizacao, :volume_madeira_serrar, :volume_madeira_processar, :equip_contr_poluicao_sonora, :geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) 
															returning c.id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.Int32);
					comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("volume_madeira_serrar", (String.IsNullOrWhiteSpace(item.VolumeMadeiraSerrar)) ? (object)DBNull.Value : item.VolumeMadeiraSerrar, DbType.Decimal);
					comando.AdicionarParametroEntrada("volume_madeira_processar", (String.IsNullOrWhiteSpace(item.VolumeMadeiraProcessar)) ? (object)DBNull.Value : item.VolumeMadeiraProcessar, DbType.Decimal);
					comando.AdicionarParametroEntrada("equip_contr_poluicao_sonora", item.EquipControlePoluicaoSonora, DbType.String);
					comando.AdicionarParametroEntrada("geometria_id", item.CoordenadaAtividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_tipo", item.CoordenadaAtividade.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_coord_atv_x", item.CoordenadaAtividade.CoordX, DbType.Decimal);
					comando.AdicionarParametroEntrada("geometria_coord_atv_y", item.CoordenadaAtividade.CoordY, DbType.Decimal);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));


					#region Materias-Prima Florestal Consumida

					if (item.MateriasPrimasFlorestais != null && item.MateriasPrimasFlorestais.Count > 0)
					{
						foreach (MateriaPrima materia in item.MateriasPrimasFlorestais)
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_benef_madeira_mat_fl_c m (id, beneficiamento_id, caracterizacao, materia_prima_tipo, unidade, quantidade, especificar, tid)
															values(seq_crt_benef_madeira_mat_fl_c.nextval, :beneficiamento_id, :caracterizacao, :materia_prima_tipo, :unidade, :quantidade, :especificar, :tid)
															returning m.id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("beneficiamento_id", item.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("materia_prima_tipo", materia.MateriaPrimaConsumida, DbType.Int32);
							comando.AdicionarParametroEntrada("unidade", materia.Unidade, DbType.Int32);
							comando.AdicionarParametroEntrada("quantidade", materia.Quantidade, DbType.Decimal);
							comando.AdicionarParametroEntrada("especificar", (materia.MateriaPrimaConsumida == (int)eMateriaPrima.Outros) ? materia.EspecificarTexto : (object)DBNull.Value, DbType.String);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							comando.AdicionarParametroSaida("id", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							materia.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						}
					}

					#endregion
				}

				#endregion

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.beneficiamentomadeira, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(BeneficiamentoMadeira caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Beneficiamento e tratamento de madeira

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_benefic_madeira c set c.empreendimento = :empreendimento, c.tid = :tid
															where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco
				
				//Materias-Prima Florestal Consumida
				List<int> listIds = new List<int>();

				caracterizacao.Beneficiamentos.ForEach(x =>
				{
					listIds.AddRange(x.MateriasPrimasFlorestais.Select(z => z.Id).ToList());
				});

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_benef_madeira_mat_fl_c c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, listIds));
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Beneficiamentos
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_benefic_madeira_benef c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Beneficiamentos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Beneficiamentos

				foreach (BeneficiamentoMadeiraBeneficiamento item in caracterizacao.Beneficiamentos)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}crt_benefic_madeira_benef c set c.atividade = :atividade, c.caracterizacao = :caracterizacao, 
															c.volume_madeira_serrar = :volume_madeira_serrar, c.volume_madeira_processar = :volume_madeira_processar, 
															c.equip_contr_poluicao_sonora = :equip_contr_poluicao_sonora, c.geometria_coord_atv_x = :geometria_coord_atv_x, 
															c.geometria_coord_atv_y = :geometria_coord_atv_y,  c.geometria_id = :geometria_id, c.geometria_tipo = :geometria_tipo, 
															c.tid = :tid where c.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_benefic_madeira_benef c(id, atividade, caracterizacao, volume_madeira_serrar, volume_madeira_processar, equip_contr_poluicao_sonora, geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
															values(seq_crt_benefic_madeira_benef.nextval, :atividade, :caracterizacao, :volume_madeira_serrar, :volume_madeira_processar, :equip_contr_poluicao_sonora, :geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) 
																returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroSaida("id", DbType.Int32);
					}

					comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.Int32);
					comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("volume_madeira_serrar", (String.IsNullOrWhiteSpace(item.VolumeMadeiraSerrar)) ? (object)DBNull.Value : item.VolumeMadeiraSerrar, DbType.Decimal);
					comando.AdicionarParametroEntrada("volume_madeira_processar", (String.IsNullOrWhiteSpace(item.VolumeMadeiraProcessar)) ? (object)DBNull.Value : item.VolumeMadeiraProcessar, DbType.Decimal);
					comando.AdicionarParametroEntrada("equip_contr_poluicao_sonora", item.EquipControlePoluicaoSonora, DbType.String);
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

					#region Materias-Prima Florestal Consumida

					if (item.MateriasPrimasFlorestais != null && item.MateriasPrimasFlorestais.Count > 0)
					{
						foreach (MateriaPrima materia in item.MateriasPrimasFlorestais)
						{
							if (materia.Id > 0)
							{
								comando = bancoDeDados.CriarComando(@"update {0}crt_benef_madeira_mat_fl_c c set c.beneficiamento_id = :beneficiamento_id,
																	c.caracterizacao = :caracterizacao, c.materia_prima_tipo = :materia_prima_tipo, c.unidade = :unidade, 
																	c.quantidade =  :quantidade, c.especificar = :especificar, c.tid = :tid where c.id = :id", EsquemaBanco);

								comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
							}
							else
							{
								comando = bancoDeDados.CriarComando(@" insert into {0}crt_benef_madeira_mat_fl_c c (id, beneficiamento_id, caracterizacao, materia_prima_tipo, unidade, quantidade, especificar, tid)
																values (seq_crt_benef_madeira_mat_fl_c.nextval, :beneficiamento_id, :caracterizacao, :materia_prima_tipo, :unidade, :quantidade, :especificar, :tid )
																returning c.id into :id", EsquemaBanco);

								comando.AdicionarParametroSaida("id", DbType.Int32);
							}

							comando.AdicionarParametroEntrada("beneficiamento_id", item.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("materia_prima_tipo", materia.MateriaPrimaConsumida, DbType.Int32);
							comando.AdicionarParametroEntrada("unidade", materia.Unidade, DbType.Int32);
							comando.AdicionarParametroEntrada("quantidade", materia.Quantidade, DbType.Decimal);
							comando.AdicionarParametroEntrada("especificar", (materia.MateriaPrimaConsumida == (int)eMateriaPrima.Outros) ? materia.EspecificarTexto : (object)DBNull.Value, DbType.String);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

							bancoDeDados.ExecutarNonQuery(comando);

							if (materia.Id <= 0)
							{
								materia.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
							}
						}
					}

					#endregion
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.beneficiamentomadeira, eHistoricoAcao.atualizar, bancoDeDados, null);

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

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_benefic_madeira c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_benefic_madeira c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.beneficiamentomadeira, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_benef_madeira_mat_fl_c m where m.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_benefic_madeira_benef m where m.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_benefic_madeira e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.BeneficiamentoMadeira, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal BeneficiamentoMadeira ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			BeneficiamentoMadeira caracterizacao = new BeneficiamentoMadeira();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_benefic_madeira s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal BeneficiamentoMadeira Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			BeneficiamentoMadeira caracterizacao = new BeneficiamentoMadeira();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_benefic_madeira s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal BeneficiamentoMadeira Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			BeneficiamentoMadeira caracterizacao = new BeneficiamentoMadeira();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.tid  from {0}crt_benefic_madeira c where c.id = :id", EsquemaBanco);

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

				#region Beneficiamentos

				comando = bancoDeDados.CriarComando(@"select c.id, c.atividade, c.volume_madeira_serrar,  c.volume_madeira_processar, c.geometria_coord_atv_x, c.geometria_coord_atv_y, c.equip_contr_poluicao_sonora,
															c.geometria_id, c.geometria_tipo, c.tid  from {0}crt_benefic_madeira_benef c where c.caracterizacao = :caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				BeneficiamentoMadeiraBeneficiamento beneficiamento = null;
				using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
				{

					while (readerAux.Read())
					{
						beneficiamento = new BeneficiamentoMadeiraBeneficiamento();
						beneficiamento.Id = Convert.ToInt32(readerAux["id"]);
						beneficiamento.Atividade = Convert.ToInt32(readerAux["atividade"]);
						beneficiamento.VolumeMadeiraSerrar = readerAux["volume_madeira_serrar"].ToString();
						beneficiamento.VolumeMadeiraProcessar = readerAux["volume_madeira_processar"].ToString();
						beneficiamento.EquipControlePoluicaoSonora = readerAux["equip_contr_poluicao_sonora"].ToString();
						beneficiamento.CoordenadaAtividade.Id = Convert.ToInt32(readerAux["geometria_id"]);
						beneficiamento.CoordenadaAtividade.Tipo = Convert.ToInt32(readerAux["geometria_tipo"]);

						if (readerAux["geometria_coord_atv_x"] != null && !Convert.IsDBNull(readerAux["geometria_coord_atv_x"]))
						{
							beneficiamento.CoordenadaAtividade.CoordX = Convert.ToDecimal(readerAux["geometria_coord_atv_x"]);
						}

						if (readerAux["geometria_coord_atv_y"] != null && !Convert.IsDBNull(readerAux["geometria_coord_atv_y"]))
						{
							beneficiamento.CoordenadaAtividade.CoordY = Convert.ToDecimal(readerAux["geometria_coord_atv_y"]);
						}

						#region Materias-Prima Florestal Consumida

						comando = bancoDeDados.CriarComando(@"select m.id, m.materia_prima_tipo, lm.texto materia_prima_tipo_texto, m.unidade, lu.texto unidade_texto, m.quantidade, m.especificar, m.tid 
															from {0}crt_benef_madeira_mat_fl_c m, {0}lov_crt_benef_madeira_mat_pr_c lm, {0}lov_crt_unidade_medida lu
															where m.beneficiamento_id = :beneficiamento_id and lm.id = m.materia_prima_tipo and lu.id = m.unidade order by m.id", EsquemaBanco);

						comando.AdicionarParametroEntrada("beneficiamento_id", beneficiamento.Id, DbType.Int32);

						using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
						{
							MateriaPrima materia = null;

							while (readerAux2.Read())
							{
								materia = new MateriaPrima();
								materia.Id = Convert.ToInt32(readerAux2["id"]);
								materia.MateriaPrimaConsumida = Convert.ToInt32(readerAux2["materia_prima_tipo"]);
								materia.MateriaPrimaConsumidaTexto = (materia.MateriaPrimaConsumida == (int)eMateriaPrima.Outros) ? readerAux2["especificar"].ToString() : readerAux2["materia_prima_tipo_texto"].ToString();
								materia.Unidade = Convert.ToInt32(readerAux2["unidade"]);
								materia.UnidadeTexto = readerAux2["unidade_texto"].ToString();
								materia.Quantidade = readerAux2["quantidade"].ToString();
								materia.EspecificarTexto = readerAux2["especificar"].ToString();
								materia.Tid = readerAux2["tid"].ToString();

								beneficiamento.MateriasPrimasFlorestais.Add(materia);
							}

							readerAux2.Close();
						}

						#endregion

						caracterizacao.Beneficiamentos.Add(beneficiamento);
					}

					readerAux.Close();
				}

				#endregion

			}

			return caracterizacao;
		}

		private BeneficiamentoMadeira ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			BeneficiamentoMadeira caracterizacao = new BeneficiamentoMadeira();

			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento, c.tid  from {0}hst_crt_benefic_madeira c where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

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

				#region Beneficiamentos

				using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
				{

					comando = bancoDeDados.CriarComando(@"select c.id, c.atividade_id, c.beneficiamento_mad_benf_id, c.volume_madeira_serrar, c.volume_madeira_processar, c.geometria_coord_atv_x, c.geometria_coord_atv_y, c.equip_contr_poluicao_sonora,
														c.geometria_id, c.geometria_tipo, c.tid  from {0}hst_crt_benefic_madeira_benef c where c.id_hst = :id_hst", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

					BeneficiamentoMadeiraBeneficiamento beneficiamento = null;

					while (readerAux.Read())
					{
						beneficiamento = new BeneficiamentoMadeiraBeneficiamento();
						beneficiamento.Id = Convert.ToInt32(readerAux["beneficiamento_mad_benf_id"]);
						beneficiamento.Atividade = Convert.ToInt32(readerAux["atividade_id"]);
						beneficiamento.VolumeMadeiraSerrar = readerAux["volume_madeira_serrar"].ToString();
						beneficiamento.VolumeMadeiraProcessar = readerAux["volume_madeira_processar"].ToString();
						beneficiamento.EquipControlePoluicaoSonora = readerAux["equip_contr_poluicao_sonora"].ToString();
						beneficiamento.CoordenadaAtividade.Id = Convert.ToInt32(readerAux["geometria_id"]);
						beneficiamento.CoordenadaAtividade.Tipo = Convert.ToInt32(readerAux["geometria_tipo"]);

						if (readerAux["geometria_coord_atv_x"] != null && !Convert.IsDBNull(readerAux["geometria_coord_atv_x"]))
						{
							beneficiamento.CoordenadaAtividade.CoordX = Convert.ToDecimal(readerAux["geometria_coord_atv_x"]);
						}

						if (readerAux["geometria_coord_atv_y"] != null && !Convert.IsDBNull(readerAux["geometria_coord_atv_y"]))
						{
							beneficiamento.CoordenadaAtividade.CoordY = Convert.ToDecimal(readerAux["geometria_coord_atv_y"]);
						}
					}

					readerAux.Close();

					#region Materias-Prima Florestal Consumida

					comando = bancoDeDados.CriarComando(@"select m.materia_prima id, m.materia_prima_tipo, lm.texto materia_prima_tipo_texto, 
													m.unidade, lu.texto unidade_texto, m.quantidade, m.especificar, m.tid 
														from {0}crt_benef_madeira_mat_fl_c m, {0}lov_crt_benefic_madeira_mat_pr_c lm, {0}lov_crt_unidade_medida lu
														where lm.id = m.materia_prima_tipo and lu.id = m.unidade  and c.id_hst = :id order by m.id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

					using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
					{
						MateriaPrima materia = null;

						while (readerAux2.Read())
						{
							materia = new MateriaPrima();
							materia.Id = Convert.ToInt32(readerAux2["id"]);
							materia.MateriaPrimaConsumida = Convert.ToInt32(readerAux2["materia_prima_tipo"]);
							materia.MateriaPrimaConsumidaTexto = (materia.MateriaPrimaConsumida == (int)eMateriaPrima.Outros) ? readerAux2["especificar"].ToString() : readerAux2["materia_prima_tipo_texto"].ToString();
							materia.Unidade = Convert.ToInt32(readerAux2["unidade"]);
							materia.UnidadeTexto = readerAux2["unidade_texto"].ToString();
							materia.Quantidade = readerAux2["quantidade"].ToString();
							materia.EspecificarTexto = readerAux2["especificar"].ToString();
							materia.Tid = readerAux2["tid"].ToString();

							beneficiamento.MateriasPrimasFlorestais.Add(materia);
						}

						readerAux2.Close();
					}

					#endregion
				}

				#endregion

			}

			return caracterizacao;

		}

		internal object ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			CaracterizacaoPDF caract = new CaracterizacaoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select c.geometria_coord_atv_x, c.geometria_coord_atv_y , c.volume_madeira_serrar, c.volume_madeira_processar
                              from {0}crt_benefic_madeira_benef c, {0}crt_benefic_madeira m 
                              where m.empreendimento = :empreendimento and c.atividade = :atividade and c.caracterizacao = m.id", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["volume_madeira_serrar"] != null && !Convert.IsDBNull(reader["volume_madeira_serrar"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Volume de madeira a ser serrada (m³/mês)", Valor = Convert.ToDecimal(reader["volume_madeira_serrar"]).ToString("N2") });
						}

						if (reader["volume_madeira_processar"] != null && !Convert.IsDBNull(reader["volume_madeira_processar"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Volume de madeira a ser processada (m³/mês)", Valor = Convert.ToDecimal(reader["volume_madeira_processar"]).ToString("N2") });
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

		internal List<int> ObterAtividadesCaracterizacao(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select b.atividade from crt_benefic_madeira_benef b, crt_benefic_madeira m where m.empreendimento = :empreendimento and m.id = b.caracterizacao", EsquemaBanco);//5-Encerrado

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarList<Int32>(comando);
			}
		}

		#endregion

	}
}