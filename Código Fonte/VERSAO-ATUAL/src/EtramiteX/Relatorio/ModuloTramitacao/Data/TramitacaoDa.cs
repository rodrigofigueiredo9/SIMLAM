using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloTramitacao.Data
{
	public class TramitacaoDa
	{
		#region Propriedade

		private string EsquemaBanco { get; set; }

		#endregion

		public TramitacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal TramitacaoRelatorioPDF Obter(int id, BancoDeDados banco = null)
		{
			TramitacaoRelatorioPDF tramitacao = new TramitacaoRelatorioPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Tramitacao

				Comando comando = bancoDeDados.CriarComando(@"select t.tramitacao_hst_id, t.tramitacao_id id, t.protocolo, t.protocolo_id, t.protocolo_tid, 
				t.protocolo_numero, t.protocolo_ano, t.protocolo_numero_completo, t.protocolo_numero_autuacao, t.protocolo_tipo_id, t.protocolo_tipo_texto, 
				t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, 
				t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, 
				t.objetivo_texto, t.situacao_id, t.situacao_texto, t.data_execucao data_recebimento, t.data_envio, t.data_execucao, t.arquivo_nome, t.estante_nome, 
				t.prateleira_nome, t.despacho, t.remetente_municipio_nome,t.remetente_estado_sigla,t.destinatario_municipio_nome,t.destinatario_estado_sigla,
				t.orgao_externo_nome, t.tid from {0}tab_tramitacao a, {0}lst_hst_tramitacao t where a.tid = t.tid and a.id = t.tramitacao_id and a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						tramitacao.Id = id;
						tramitacao.Tid = reader["tid"].ToString();
						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);
						tramitacao.Protocolo.Tipo = Convert.ToInt32(reader["protocolo_tipo_id"]);
						tramitacao.Protocolo.IsProcesso = reader["protocolo"].ToString() == "1";
						tramitacao.Protocolo.Numero = reader["protocolo_numero_completo"].ToString();
						tramitacao.Protocolo.TipoTexto = reader["protocolo_tipo_texto"].ToString();

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
						}

						if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
						{
							tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);
						}

						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}

						if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
						{
							tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
						}

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}

						if (reader["arquivo_nome"] != null && !Convert.IsDBNull(reader["arquivo_nome"]))
						{
							tramitacao.Arquivo.Nome = reader["arquivo_nome"].ToString();
						}

						if (reader["estante_nome"] != null && !Convert.IsDBNull(reader["estante_nome"]))
						{
							tramitacao.Arquivo.EstanteNome = reader["estante_nome"].ToString();
						}

						if (reader["prateleira_nome"] != null && !Convert.IsDBNull(reader["prateleira_nome"]))
						{
							tramitacao.Arquivo.PrateleiraNome = reader["prateleira_nome"].ToString();
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							tramitacao.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						}

						tramitacao.Objetivo.Texto = reader["objetivo_texto"].ToString();
						tramitacao.SituacaoTexto = reader["situacao_texto"].ToString();
						tramitacao.Despacho = reader["despacho"].ToString();
						tramitacao.DestinatarioSetor.Nome = reader["destinatario_setor_nome"].ToString();
						tramitacao.RemetenteSetor.Nome = reader["remetente_setor_nome"].ToString();
						tramitacao.RemetenteSetor.Endereco.MunicipioTexto = reader["remetente_municipio_nome"].ToString();
						tramitacao.RemetenteSetor.Endereco.EstadoTexto = reader["remetente_estado_sigla"].ToString();
						tramitacao.DestinatarioSetor.Endereco.MunicipioTexto = reader["destinatario_municipio_nome"].ToString();
						tramitacao.DestinatarioSetor.Endereco.EstadoTexto = reader["destinatario_estado_sigla"].ToString();
						tramitacao.Destinatario.Nome = reader["destinatario_nome"].ToString();
						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();
						tramitacao.OrgaoExterno.Texto = reader["orgao_externo_nome"].ToString();
					}

					reader.Close();
				}
				#endregion		
			}

			return tramitacao;
		}

		internal TramitacaoRelatorioPDF ObterHistorico(int id, BancoDeDados banco = null)
		{
			TramitacaoRelatorioPDF tramitacao = new TramitacaoRelatorioPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Tramitacao

				Comando comando = bancoDeDados.CriarComando(@"select t.tramitacao_hst_id, t.tramitacao_id id, t.protocolo, t.protocolo_id, t.protocolo_tid, 
				t.protocolo_numero, t.protocolo_ano, t.protocolo_numero_completo, t.protocolo_numero_autuacao, t.protocolo_tipo_id, t.protocolo_tipo_texto, 
				t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, 
				t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, 
				t.objetivo_texto, t.situacao_id, t.situacao_texto, t.data_execucao data_recebimento, t.data_envio, t.data_execucao, t.arquivo_nome, 
				t.prateleira_nome, t.estante_nome, t.despacho, t.remetente_municipio_nome,t.remetente_estado_sigla,t.destinatario_municipio_nome,t.destinatario_estado_sigla,
				t.orgao_externo_nome, t.acao_executada, t.acao_executada_texto, t.tid from {0}lst_hst_tramitacao t where t.tramitacao_hst_id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						tramitacao.Id = Convert.ToInt32(reader["id"]);
						tramitacao.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						tramitacao.SituacaoTexto = reader["situacao_texto"].ToString();
						tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);
						tramitacao.Tid = reader["tid"].ToString();
						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);

						tramitacao.Arquivo.Nome = reader["arquivo_nome"].ToString();
						tramitacao.Arquivo.PrateleiraNome = reader["prateleira_nome"].ToString();
						tramitacao.Arquivo.EstanteNome = reader["estante_nome"].ToString();

						tramitacao.Despacho = reader["despacho"].ToString();
						tramitacao.Protocolo.Numero = reader["protocolo_numero_completo"].ToString();
						tramitacao.Protocolo.IsProcesso = reader["protocolo"].ToString() == "1";
						tramitacao.Protocolo.TipoTexto = reader["protocolo_tipo_texto"].ToString();
						tramitacao.Protocolo.Tipo = Convert.ToInt32(reader["protocolo_tipo_id"]);

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
						}

						tramitacao.Objetivo.Texto = reader["objetivo_texto"].ToString();
						
						if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
						{
							tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);
						}

						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();
						
						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}

						if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
						{
							tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
						}

						tramitacao.RemetenteSetor.Nome = reader["remetente_setor_nome"].ToString();
						
						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}

												
						tramitacao.RemetenteSetor.Endereco.MunicipioTexto = reader["remetente_municipio_nome"].ToString();						
						tramitacao.RemetenteSetor.Endereco.EstadoTexto = reader["remetente_estado_sigla"].ToString();
						tramitacao.DestinatarioSetor.Endereco.MunicipioTexto = reader["destinatario_municipio_nome"].ToString();
						tramitacao.DestinatarioSetor.Nome = reader["destinatario_setor_nome"].ToString();
						tramitacao.DestinatarioSetor.Endereco.EstadoTexto = reader["destinatario_estado_sigla"].ToString();
						tramitacao.Destinatario.Nome = reader["destinatario_nome"].ToString();
						tramitacao.OrgaoExterno.Texto = reader["orgao_externo_nome"].ToString();
						tramitacao.AcaoExecutadaId = Convert.ToInt32(reader["acao_executada"]);
						tramitacao.AcaoExecutada = reader["acao_executada_texto"].ToString();
					}
					reader.Close();
				}
				#endregion
			}
			return tramitacao;
		}

		internal TramitacaoHistoricoRelatorioPDF ObterTramitacaoHistorico(int protocolo, BancoDeDados banco = null)
		{
			TramitacaoHistoricoRelatorioPDF historico = new TramitacaoHistoricoRelatorioPDF();
			List<TramitacaoRelatorio> tramitacoes = new List<TramitacaoRelatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.tipo protocolo_tipo_id, pt.texto protocolo_tipo_texto, t.numero ||'/'|| t.ano protocolo_numero_completo 
				from {0}tab_protocolo t, {0}lov_protocolo_tipo pt where t.tipo = pt.id and t.id = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						historico.Protocolo.Tipo = reader.GetValue<Int32>("protocolo_tipo_id");
						historico.Protocolo.TipoTexto = reader.GetValue<String>("protocolo_tipo_texto");
						historico.Protocolo.Numero = reader.GetValue<String>("protocolo_numero_completo");
					}

					reader.Close();
				}

				#region Tramitações(Enviar/Receber/Cancelar/Arquivar/Desarquivar)

				comando = bancoDeDados.CriarComando(@"
				select t.tramitacao_hst_id, t.tramitacao_id id, t.protocolo, t.protocolo_id, t.protocolo_tid, t.protocolo_numero, t.protocolo_ano, t.protocolo_numero_completo, t.protocolo_numero_autuacao, 
				t.protocolo_tipo_id, t.protocolo_tipo_texto, t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, 
				t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.arquivo_nome, t.estante_nome, t.prateleira_nome,t.objetivo_id, 
				t.objetivo_texto, t.situacao_id, t.situacao_texto, t.data_execucao, t.data_envio, t.despacho, t.remetente_municipio_nome,t.remetente_estado_sigla,
				t.destinatario_municipio_nome, t.destinatario_estado_sigla, t.orgao_externo_nome, la.id acao_id, t.acao_executada, t.acao_executada_texto, t.tid 
				from {0}lst_hst_tramitacao t, lov_historico_acao la, lov_historico_artefatos_acoes laa where t.tramitacao_hst_id in 
				(select tramitacao_hst_id from {0}lst_hst_tramitacao where protocolo_id = :protocolo) and la.id = laa.acao and laa.id = t.acao_executada order by t.id desc", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TramitacaoRelatorio tramitacao;

					while (reader.Read())
					{
						tramitacao = new TramitacaoRelatorio();
						tramitacao.Id = reader.GetValue<int>("id");
						tramitacao.SituacaoId = reader.GetValue<int>("situacao_id");
						tramitacao.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						tramitacao.HistoricoId = reader.GetValue<int>("tramitacao_hst_id");
						tramitacao.Tid = reader.GetValue<string>("tid");
						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : reader.GetValue<DateTime?>("data_envio");
						tramitacao.Objetivo.Id = reader.GetValue<int>("objetivo_id");
						tramitacao.Objetivo.Texto = reader.GetValue<string>("objetivo_texto");
						tramitacao.Despacho = reader.GetValue<string>("despacho");

						tramitacao.Protocolo.Numero = reader.GetValue<string>("protocolo_numero_completo");
						tramitacao.Protocolo.IsProcesso = reader.GetValue<string>("protocolo") == "1";
						tramitacao.Protocolo.TipoTexto = reader.GetValue<string>("protocolo_tipo_texto");
						tramitacao.Protocolo.Tipo = reader.GetValue<int>("protocolo_tipo_id");

						tramitacao.Remetente.Id = reader.GetValue<int>("remetente_id");
						tramitacao.Remetente.Nome = reader.GetValue<string>("remetente_nome");
						tramitacao.RemetenteSetor.Id = reader.GetValue<int>("remetente_setor_id");
						tramitacao.RemetenteSetor.Nome = reader.GetValue<string>("remetente_setor_nome");
						tramitacao.RemetenteSetor.Endereco.MunicipioTexto = reader.GetValue<string>("remetente_municipio_nome");
						tramitacao.RemetenteSetor.Endereco.EstadoTexto = reader.GetValue<string>("remetente_estado_sigla");

						tramitacao.Destinatario.Id = reader.GetValue<int>("destinatario_id");
						tramitacao.Destinatario.Nome = reader.GetValue<string>("destinatario_nome");
						tramitacao.DestinatarioSetor.Id = reader.GetValue<int>("destinatario_setor_id");
						tramitacao.DestinatarioSetor.Nome = reader.GetValue<string>("destinatario_setor_nome");
						tramitacao.DestinatarioSetor.Endereco.MunicipioTexto = reader.GetValue<string>("destinatario_municipio_nome");
						tramitacao.DestinatarioSetor.Endereco.EstadoTexto = reader.GetValue<string>("destinatario_estado_sigla");

						tramitacao.DataExecucao.Data = reader.GetValue<DateTime>("data_execucao");

						tramitacao.Arquivo.Nome = reader.GetValue<string>("arquivo_nome");
						tramitacao.Arquivo.PrateleiraNome = reader.GetValue<string>("prateleira_nome");
						tramitacao.Arquivo.EstanteNome = reader.GetValue<string>("estante_nome");

						tramitacao.OrgaoExterno.Texto = reader.GetValue<string>("orgao_externo_nome");
						tramitacao.AcaoExecutadaId = reader.GetValue<int>("acao_executada");
						tramitacao.Acao = (eHistoricoAcao)reader.GetValue<int>("acao_id");
						tramitacao.AcaoExecutada = reader.GetValue<string>("acao_executada_texto");

						if (tramitacao.Acao == eHistoricoAcao.desarquivar)
						{
							comando = bancoDeDados.CriarComando(@"
							select t.remetente_id, t.remetente_nome, t.remetente_tid from lst_hst_tramitacao t where tramitacao_hst_id = 
							(select max(t.tramitacao_hst_id) from lst_hst_tramitacao t where t.acao_executada = 53)/*Arquivar*/", EsquemaBanco);

							using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
							{
								if (readerAux.Read())
								{
									tramitacao.RemetenteArquivar.Id = readerAux.GetValue<Int32>("remetente_id");
									tramitacao.RemetenteArquivar.Nome = readerAux.GetValue<String>("remetente_nome");
								}

								readerAux.Close();
							}
						}

						tramitacoes.Add(tramitacao);
					}

					reader.Close();
				}

				#endregion

				#region Tramitações(Converter/Apensar/Juntar/Desapensar/Desentranhar)

				comando = bancoDeDados.CriarComando(@"
				select a.id, a.associado_id, s.id destinatario_setor_id, s.nome destinatario_setor_nome, le.texto destinatario_estado_sigla, lm.texto destinatario_municipio_nome, 
				a.executor_id, a.executor_nome executor_nome, to_char(a.data_execucao, 'dd/mm/yyyy hh24:mi:ss') data_execucao, la.id acao_id, a.acao_executada, la.texto acao_executada_texto, 
				pp.numero || '/' || pp.ano numero_pai from hst_protocolo_associado a, tab_protocolo pp, tab_setor s, tab_setor_endereco se, lov_estado le, lov_municipio lm, 
				lov_historico_artefatos_acoes l, lov_historico_acao la where a.protocolo_id = pp.id and pp.setor = s.id(+) and s.id = se.setor(+) and se.estado = le.id(+) 
				and se.municipio = lm.id(+) and l.acao = la.id and l.id = a.acao_executada and a.associado_id = :protocolo 
				union all 
				select a.id, a.id_protocolo associado_id, s.id destinatario_setor_id, s.nome destinatario_setor_nome, le.texto destinatario_estado_sigla, lm.texto destinatario_municipio_nome, 
				a.executor_id, a.executor_nome executor_nome, to_char(a.data_execucao, 'dd/mm/yyyy hh24:mi:ss') data_execucao, la.id acao_id, a.acao_executada, la.texto acao_executada_texto, 
				null numero_pai from hst_protocolo a, tab_setor s, tab_setor_endereco se, lov_estado le, lov_municipio lm, lov_historico_artefatos_acoes l, lov_historico_acao la 
				where a.setor_id = s.id(+) and s.id = se.setor(+) and se.estado = le.id(+) and se.municipio = lm.id(+) and a.acao_executada = l.id(+) and l.acao = la.id(+) and l.acao = 33 
				and a.id_protocolo = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TramitacaoRelatorio tramitacao;

					while (reader.Read())
					{
						tramitacao = new TramitacaoRelatorio();
						tramitacao.Id = reader.GetValue<int>("id");

						tramitacao.Destinatario.Id = reader.GetValue<int>("executor_id");
						tramitacao.Destinatario.Nome = reader.GetValue<string>("executor_nome");
						tramitacao.DataExecucao.Data = reader.GetValue<DateTime>("data_execucao");

						tramitacao.DestinatarioSetor.Id = reader.GetValue<int>("destinatario_setor_id");
						tramitacao.DestinatarioSetor.Nome = reader.GetValue<string>("destinatario_setor_nome");
						tramitacao.DestinatarioSetor.Endereco.MunicipioTexto = reader.GetValue<string>("destinatario_municipio_nome");
						tramitacao.DestinatarioSetor.Endereco.EstadoTexto = reader.GetValue<string>("destinatario_estado_sigla");

						tramitacao.Acao = (eHistoricoAcao)reader.GetValue<int>("acao_id");
						tramitacao.AcaoExecutadaId = reader.GetValue<int>("acao_executada");
						tramitacao.AcaoExecutada = reader.GetValue<string>("acao_executada_texto");

						tramitacao.ProtocoloPai.Numero = reader.GetValue<string>("numero_pai");

						tramitacoes.Add(tramitacao);
					}

					reader.Close();
				}

				#endregion
			}

			foreach (var item in tramitacoes.OrderByDescending(x => x.DataExecucao.Data.Value))
			{
				historico.Tramitacoes.Add(new TramitacaoHistoricoRelatorio(item));
			}

			return historico;
		}

		internal int ExisteProcDocArquivado(int protocolo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;

				comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_tramitacao t where t.situacao = 2 and t.protocolo = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno) ? Convert.ToInt32(retorno) : 0);
			}
		}
	}
}