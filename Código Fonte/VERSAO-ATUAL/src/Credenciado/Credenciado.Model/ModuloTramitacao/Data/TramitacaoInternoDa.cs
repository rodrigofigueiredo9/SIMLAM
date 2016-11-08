using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Data
{
	public class TramitacaoInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public TramitacaoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal Tramitacao Obter(int id, BancoDeDados banco = null)
		{
			Tramitacao tramitacao = new Tramitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Tramitacao

				Comando comando = bancoDeDados.CriarComando(@"select t.tramitacao_hst_id, t.tramitacao_id id, t.protocolo, t.protocolo_id, t.protocolo_tid, 
				t.protocolo_numero, t.protocolo_ano, t.protocolo_numero_completo, t.protocolo_numero_autuacao, t.protocolo_tipo_id, t.protocolo_tipo_texto, 
				t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, 
				t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, 
				t.objetivo_texto, t.situacao_id, t.situacao_texto, t.data_execucao data_recebimento, t.data_envio, t.data_execucao, t.despacho, t.remetente_municipio_nome,
				t.remetente_estado_sigla, t.destinatario_municipio_nome,t.destinatario_estado_sigla, t.orgao_externo_id, t.orgao_externo_nome, a.executor, t.tid 
				from {0}tab_tramitacao a, {0}lst_hst_tramitacao t where a.tid = t.tid and a.id = t.tramitacao_id and a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						tramitacao.Id = id;

						if (reader["tramitacao_hst_id"] != null && !Convert.IsDBNull(reader["tramitacao_hst_id"]))
						{
							tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);
						}

						tramitacao.Tid = reader["tid"].ToString();
						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);
						tramitacao.Despacho = reader["despacho"].ToString();
						tramitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]);
						tramitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);
						tramitacao.Protocolo.Tipo.Texto = reader["protocolo_tipo_texto"].ToString();

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
						{
							tramitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
							tramitacao.Protocolo.IsProcesso = reader["protocolo"].ToString() == "1";
						}

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
							tramitacao.Objetivo.Texto = reader["objetivo_texto"].ToString();
						}

						if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
						{
							tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);
						}
						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();

						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}
						tramitacao.Destinatario.Nome = reader["destinatario_nome"].ToString();

						if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
						{
							tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
						}
						tramitacao.RemetenteSetor.Nome = reader["remetente_setor_nome"].ToString();

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}
						tramitacao.DestinatarioSetor.Nome = reader["destinatario_setor_nome"].ToString();

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							tramitacao.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						}

						if (reader["orgao_externo_id"] != null && !Convert.IsDBNull(reader["orgao_externo_id"]))
						{
							tramitacao.OrgaoExterno.Id = Convert.ToInt32(reader["orgao_externo_id"]);
						}
						tramitacao.OrgaoExterno.Texto = reader["orgao_externo_nome"].ToString();

						if (reader["executor"] != null && !Convert.IsDBNull(reader["executor"]))
						{
							tramitacao.Executor.Id = Convert.ToInt32(reader["executor"]);
						}
					}

					reader.Close();
				}

				#endregion
			}
			return tramitacao;
		}

		internal int ExisteTramitacao(int protocolo, BancoDeDados banco = null)
		{
			int retorno = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id from {0}tab_tramitacao a where a.protocolo = :protocolo", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					retorno = Convert.ToInt32(obj);
				}
			}
			return retorno;
		}

		internal TramitacaoPosse ObterProtocoloPosse(int protocoloId, BancoDeDados banco = null)
		{
			TramitacaoPosse posse = new TramitacaoPosse();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id funcionario_id, f.nome funcionario_nome, s.id setor_id, s.nome setor_nome
				from {0}tab_protocolo p, {0}tab_funcionario f, {0}tab_setor s where f.id(+) = p.emposse and s.id = p.setor and p.id = :protocolo_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo_id", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						posse.FuncionarioId = Convert.ToInt32(reader["funcionario_id"]);
						posse.FuncionarioNome = reader["funcionario_nome"].ToString();

						posse.SetorId = Convert.ToInt32(reader["setor_id"]);
						posse.SetorNome = reader["setor_nome"].ToString();
					}

					reader.Close();
				}
			}
			return posse;
		}
	}
}
