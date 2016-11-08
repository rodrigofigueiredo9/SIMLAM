using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class ComplementacaoDadosDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }

		#endregion

		public ComplementacaoDadosDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public ComplementacaoDados Salvar(ComplementacaoDados complementacao, BancoDeDados banco = null)
		{
			if (complementacao == null)
			{
				throw new Exception("Complemento de Dados do autuado é nulo.");
			}

			if (complementacao.Id <= 0) 
			{
				complementacao = Criar(complementacao, banco);
			}
			else
			{
				complementacao = Editar(complementacao, banco);
			}

			return complementacao;
		}

		internal ComplementacaoDados Criar(ComplementacaoDados complementacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Complementacao de Dados do responsavel

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_fisc_compl_dados_aut f (id, fiscalizacao, responsavel, empreendimento, reside_propriedade, 
															renda_mensal_familiar, nivel_escolaridade, vinc_prop, vinc_prop_especif_text, conhec_legisl, 
															conhec_legisl_justif_text, prop_area_total,  prop_area_cobert_flores_nativ, prop_area_reserv_legal, tid) 
															values ({0}seq_tab_fisc_compl_dados_aut.nextval, :fiscalizacao, :responsavel, :empreendimento, :reside_propriedade, :renda_mensal_familiar, 
															:nivel_escolaridade, :vinc_prop, :vinc_prop_especif_text, :conhec_legisl, :conhec_legisl_justif_text, :prop_area_total, 
															:prop_area_cobert_flores_nativ, :prop_area_reserv_legal, :tid) returning f.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", complementacao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel", complementacao.AutuadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", complementacao.EmpreendimentoId == 0 ? (object)DBNull.Value : complementacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("reside_propriedade", complementacao.ResidePropriedadeTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("renda_mensal_familiar", complementacao.RendaMensalFamiliarTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("nivel_escolaridade", complementacao.NivelEscolaridadeTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("vinc_prop", complementacao.VinculoComPropriedadeTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("vinc_prop_especif_text", String.IsNullOrWhiteSpace(complementacao.VinculoComPropriedadeEspecificarTexto) ? (object)DBNull.Value : complementacao.VinculoComPropriedadeEspecificarTexto, DbType.String);
				comando.AdicionarParametroEntrada("conhec_legisl", complementacao.ConhecimentoLegislacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("conhec_legisl_justif_text", complementacao.Justificativa, DbType.String);
				comando.AdicionarParametroEntrada("prop_area_total", complementacao.AreaTotalInformada, DbType.Decimal);
				comando.AdicionarParametroEntrada("prop_area_cobert_flores_nativ", complementacao.AreaCoberturaFlorestalNativa, DbType.Decimal);
				comando.AdicionarParametroEntrada("prop_area_reserv_legal", complementacao.ReservalegalTipo, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				complementacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				Historico.Gerar(complementacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(complementacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);
				
				bancoDeDados.Commit();
			}

			return complementacao;
		}

		internal ComplementacaoDados Editar(ComplementacaoDados complementacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Complementacao de Dados do responsavel

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fisc_compl_dados_aut f set f.fiscalizacao = :fiscalizacao, f.responsavel = :responsavel, f.empreendimento = :empreendimento,
															f.reside_propriedade = :reside_propriedade, f.renda_mensal_familiar = :renda_mensal_familiar, f.nivel_escolaridade = :nivel_escolaridade, 
															f.vinc_prop = :vinc_prop, f.vinc_prop_especif_text = :vinc_prop_especif_text, f.conhec_legisl = :conhec_legisl, 
															f.conhec_legisl_justif_text = :conhec_legisl_justif_text, f.prop_area_total = :prop_area_total, 
															f.prop_area_cobert_flores_nativ = :prop_area_cobert_flores_nativ, f.prop_area_reserv_legal = :prop_area_reserv_legal,
															f.tid = :tid where f.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", complementacao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel", complementacao.AutuadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", complementacao.EmpreendimentoId == 0 ? (object)DBNull.Value : complementacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("reside_propriedade", complementacao.ResidePropriedadeTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("renda_mensal_familiar", complementacao.RendaMensalFamiliarTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("nivel_escolaridade", complementacao.NivelEscolaridadeTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("vinc_prop", complementacao.VinculoComPropriedadeTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("vinc_prop_especif_text", String.IsNullOrWhiteSpace(complementacao.VinculoComPropriedadeEspecificarTexto) ? (object)DBNull.Value : complementacao.VinculoComPropriedadeEspecificarTexto, DbType.String);
				comando.AdicionarParametroEntrada("conhec_legisl", complementacao.ConhecimentoLegislacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("conhec_legisl_justif_text", complementacao.Justificativa, DbType.String);
				comando.AdicionarParametroEntrada("prop_area_total", complementacao.AreaTotalInformada, DbType.Decimal);
				comando.AdicionarParametroEntrada("prop_area_cobert_flores_nativ", complementacao.AreaCoberturaFlorestalNativa, DbType.Decimal);
				comando.AdicionarParametroEntrada("prop_area_reserv_legal", complementacao.ReservalegalTipo, DbType.Decimal);
				comando.AdicionarParametroEntrada("id", complementacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(complementacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(complementacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}

			return complementacao;
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete {0}tab_fisc_compl_dados_aut t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal ComplementacaoDados Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			ComplementacaoDados complementacao = new ComplementacaoDados();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Complementacao de Dados do responsavel

				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.fiscalizacao, f.responsavel, f.empreendimento, f.reside_propriedade, la.texto reside_propriedade_texto, f.renda_mensal_familiar, 
															lrm.texto renda_mensal_familiar_texto, f.nivel_escolaridade, lne.texto nivel_escolaridade_texto, f.vinc_prop, 
															lvp.texto vinc_prop_texto, f.vinc_prop_especif_text, f.conhec_legisl, lb.texto conhec_legisl_texto, 
															f.conhec_legisl_justif_text, f.prop_area_total, f.prop_area_cobert_flores_nativ, f.prop_area_reserv_legal, 
															f.tid from {0}tab_fisc_compl_dados_aut f, lov_fisc_compl_dad_resp_padrao la, lov_fisc_compl_dad_resp_padrao lb, 
															lov_fisc_compl_dad_rend_mensal lrm, lov_fisc_compl_dad_nivel_escol lne, lov_empreendimento_tipo_resp lvp 
															where la.id = f.reside_propriedade and lb.id = f.conhec_legisl and lrm.id = f.renda_mensal_familiar 
															and lne.id = f.nivel_escolaridade and lvp.id = f.vinc_prop and f.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						complementacao.Id = Convert.ToInt32(reader["id"]);
						complementacao.AutuadoId = Convert.ToInt32(reader["responsavel"]);
						complementacao.ResidePropriedadeTipo = Convert.ToInt32(reader["reside_propriedade"]);
						complementacao.ResidePropriedadeTipoTexto = reader["reside_propriedade_texto"].ToString();
						complementacao.RendaMensalFamiliarTipo = Convert.ToInt32(reader["renda_mensal_familiar"]);
						complementacao.RendaMensalFamiliarTipoTexto = reader["renda_mensal_familiar_texto"].ToString();
						complementacao.NivelEscolaridadeTipo = Convert.ToInt32(reader["nivel_escolaridade"]);
						complementacao.NivelEscolaridadeTipoTexto = reader["nivel_escolaridade_texto"].ToString();
						complementacao.VinculoComPropriedadeTipo = Convert.ToInt32(reader["vinc_prop"]);
						complementacao.VinculoComPropriedadeEspecificarTexto = reader["vinc_prop_especif_text"].ToString();
						complementacao.VinculoComPropriedadeTipoTexto = reader["vinc_prop_texto"].ToString();
						complementacao.ConhecimentoLegislacaoTipo = Convert.ToInt32(reader["conhec_legisl"]);
						complementacao.ConhecimentoLegislacaoTipoTexto = reader["conhec_legisl_texto"].ToString();
						complementacao.Justificativa = reader["conhec_legisl_justif_text"].ToString();
						complementacao.AreaTotalInformada = reader["prop_area_total"].ToString();
						complementacao.AreaCoberturaFlorestalNativa = reader["prop_area_cobert_flores_nativ"].ToString();

						if (reader["prop_area_reserv_legal"] != null && !Convert.IsDBNull(reader["prop_area_reserv_legal"]))
						{
							complementacao.ReservalegalTipo = Convert.ToInt32(reader["prop_area_reserv_legal"]);
						}

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							complementacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						}
						else 
						{
							complementacao.EmpreendimentoId = 0;
						}


						complementacao.Tid = reader["tid"].ToString();

					}
					reader.Close();
				}

				#endregion

				#region Dados da Propriedade

				comando = bancoDeDados.CriarComando(@"select count(*) from tab_fisc_local_infracao i where i.fiscalizacao = :fiscalizacao and i.pessoa IS NOT NULL", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				complementacao.AutuadoTipo = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando)) ? (int)eTipoAutuado.Pessoa : (int)eTipoAutuado.Empreendimento;
				int autuadoId = ObterAutuadoId(fiscalizacaoId);

				//Se autuado for do Tipo Empreendimento, entao busca o responsavel, senao mantem o id da pessoa
				if (complementacao.AutuadoTipo == (int)eTipoAutuado.Empreendimento)
				{
					int empreendimento = autuadoId;
					comando = bancoDeDados.CriarComando(@"select e.responsavel from tab_fisc_local_infracao e where e.fiscalizacao = :fiscalizacao", EsquemaBanco);
					comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);
					complementacao.AutuadoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

					if (complementacao.EmpreendimentoId != empreendimento) 
					{
						ComplementacaoDados aux = ObterDadosPropriedade(empreendimento);
						complementacao.EmpreendimentoId = empreendimento;
						complementacao.AreaTotalInformada = aux.AreaTotalInformada;
						complementacao.AreaCoberturaFlorestalNativa = aux.AreaCoberturaFlorestalNativa;
						complementacao.ReservalegalTipo = aux.ReservalegalTipo;
					}
				}

				if (complementacao.AutuadoTipo == (int)eTipoAutuado.Pessoa) 
				{
					if (complementacao.AutuadoId != autuadoId)
					{
						complementacao.VinculoComPropriedadeTipo = 0;
						complementacao.AutuadoId = autuadoId;
					}
				}
				
				#endregion

			}
			return complementacao;
		}

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_compl_dados_aut t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

		#endregion

		#region Auxiliares

		internal ComplementacaoDados ObterDadosPropriedade(int empreendimentoId)
		{
			ComplementacaoDados dados = new ComplementacaoDados();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Reservas legais

				Comando comando = bancoDeDados.CriarComando(@"select distinct r.situacao from crt_dominialidade_reserva r,crt_dominialidade d, crt_dominialidade_dominio dd, 
															tab_empreendimento e, lov_crt_domin_reserva_situacao lr where r.dominio = dd.id  and lr.id = r.situacao
															and dd.dominialidade = d.id and d.empreendimento = e.id and e.id = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				List<eReservaLegal> situacoes = new List<eReservaLegal>();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["situacao"] != null && !Convert.IsDBNull(reader["situacao"]))
						{
							int valor = Convert.ToInt32(reader["situacao"]);

							switch (valor)
							{
								case 1:
									situacoes.Add(eReservaLegal.NaoInformado);
									break;
								case 2:
									situacoes.Add(eReservaLegal.Proposta);
									break;
								case 3:
									situacoes.Add(eReservaLegal.Averbada);
									break;
								default:
									situacoes.Add(eReservaLegal.NaoPossui);
									break;
							}

						}
					}

					dados.ReservalegalTipo = 0;

					bool possuiAverbada = false;
					bool possuiProposta = false;
					bool possuiNaoInformado = false;
					bool NaoPossui = false;

					foreach (eReservaLegal item in situacoes)
					{
						switch (item)
						{
							case eReservaLegal.Averbada:
								possuiAverbada = true;
								break;
							case eReservaLegal.Proposta:
								possuiProposta = true;
								break;
							case eReservaLegal.NaoInformado:
								possuiNaoInformado = true;
								break;
							case eReservaLegal.NaoPossui:
								NaoPossui = true;
								break;
							default:
								break;
						}
					}

					if (possuiNaoInformado) 
					{
						possuiNaoInformado = !(possuiAverbada || possuiProposta);
					}

					if (NaoPossui)
					{
						dados.ReservalegalTipo += 8;
					}
					else 
					{
						if (possuiAverbada) 
						{
							dados.ReservalegalTipo += 1;
						}

						if (possuiProposta)
						{
							dados.ReservalegalTipo += 2;
						}

						if (possuiNaoInformado)
						{
							dados.ReservalegalTipo += 4;
						}
					}

					reader.Close();
				}

				#endregion

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select (select sum(dd.area_croqui) from crt_dominialidade d, crt_dominialidade_dominio dd
													where dd.dominialidade = d.id and d.empreendimento = :empreendimento) area_total,
													(select sum(a.valor) from crt_dominialidade_areas a, lov_crt_dominialidade_area la,
													crt_dominialidade d where a.tipo = la.id and a.dominialidade = d.id 
													and (a.tipo = 1 /*AVN_I*/ or a.tipo = 2 /*AVN_M*/ or a.tipo = 3 /*AVN_A*/ or a.tipo = 4 /*AVN_D*/)
													and d.empreendimento = :empreendimento) area_cobertura from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
				{
					if (readerAux.Read())
					{
						Decimal areaTotalInformada = 0;
						Decimal areaCoberturaFlorestalNativa = 0;

						if (readerAux["area_total"] != null && !Convert.IsDBNull(readerAux["area_total"]))
						{
							areaTotalInformada = Convert.ToDecimal(readerAux["area_total"]);
						}

						if (readerAux["area_cobertura"] != null && !Convert.IsDBNull(readerAux["area_cobertura"]))
						{
							areaCoberturaFlorestalNativa = Convert.ToDecimal(readerAux["area_cobertura"]);
						}

						dados.AreaTotalInformada = (areaTotalInformada > 0) ? areaTotalInformada.Convert(eMetrica.M2ToHa).ToStringTrunc(4) : String.Empty;
						dados.AreaCoberturaFlorestalNativa = (areaCoberturaFlorestalNativa > 0) ? areaCoberturaFlorestalNativa.Convert(eMetrica.M2ToHa).ToStringTrunc(4) : String.Empty;
					}

					readerAux.Close();
				}

				#endregion

				return dados;
			}
		}

		private int ObterAutuadoId(int fiscalizacaoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				int autuadoId = 0;

				Comando comando = bancoDeDados.CriarComando(@"select nvl(i.pessoa, i.empreendimento) autuado_id from {0}tab_fisc_local_infracao i 
														where i.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["autuado_id"] != null && !Convert.IsDBNull(reader["autuado_id"]))
						{
							autuadoId = Convert.ToInt32(reader["autuado_id"]);

						}
					}

					reader.Close();
				}

				return autuadoId;

			}

		}

		internal String ObterVinculoPropriedade(int responsavelId, int empreendimentoId) 
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string vinculo = string.Empty;

				Comando comando = bancoDeDados.CriarComando(@"select e.tipo vinculo_id, e.especificar from {0}tab_empreendimento_responsavel e 
															where e.responsavel = :responsavel and e.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("responsavel", responsavelId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["vinculo_id"] != null && !Convert.IsDBNull(reader["vinculo_id"]))
						{
							vinculo = reader["vinculo_id"].ToString() + "@" + reader["especificar"].ToString();
						}
					}

					reader.Close();
				}

				return vinculo;
			}
			
		}

		#endregion
	}
}