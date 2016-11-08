using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Data
{
	public class ProtocoloDa
	{
		private string EsquemaBanco { get; set; }

		public ProtocoloDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal int ExisteProtocoloAssociado(int protocoloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_protocolo_associado x where x.protocolo = :protocolo", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal ProtocoloRelatorio Obter(int protocoloId)
		{
			ProtocoloRelatorio protocolo = new ProtocoloRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Protocolo

				Comando comando = bancoDeDados.CriarComando(@"select lpt.texto tipo_texto, t.numero||'/'||t.ano numero, t.setor_criacao 
				from {0}tab_protocolo t, {0}lov_protocolo_tipo lpt where t.tipo = lpt.id and t.id = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						protocolo.Numero = reader["numero"].ToString();
						protocolo.TipoTexto = reader["tipo_texto"].ToString();
						protocolo.SetorCriacaoId = Convert.ToInt32(reader["setor_criacao"]);
					}

					reader.Close();
				}

				#endregion

				if (!String.IsNullOrWhiteSpace(protocolo.Numero))
				{
					#region Protocolos Associados

					comando = bancoDeDados.CriarComando(@"select (select h.executor_nome from {0}hst_protocolo_associado h where h.associado_id = pp.associado and h.tid = pp.tid) executor_nome,
					(select h.data_execucao from {0}hst_protocolo_associado h where h.associado_id = pp.associado and h.tid = pp.tid) data_execucao, tp.numero || '/' || tp.ano numero, 
					(select s.nome from {0}hst_protocolo h, {0}hst_setor s where h.tid in (select p.tid from {0}tab_protocolo_associado p where p.protocolo = :protocolo) 
					and h.setor_id = s.id and h.setor_tid = s.tid and h.id_protocolo = :protocolo and rownum = 1) setor_texto, 
					tp.protocolo protocolo_tipo, (select lp.texto from {0}lov_protocolo_tipo lp where lp.id = tp.tipo) tipo_texto 
					from {0}tab_protocolo_associado pp, {0}tab_protocolo tp where pp.associado = tp.id and pp.protocolo = :protocolo order by data_execucao", EsquemaBanco);

					comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							ProtocoloRelatorio proc = new ProtocoloRelatorio();

							proc.ProtocoloTipo = Convert.ToInt32(reader["protocolo_tipo"]);
							proc.Numero = reader["numero"].ToString();
							proc.TipoTexto = reader["tipo_texto"].ToString();
							proc.Data = reader["data_execucao"].ToString();
							proc.Setor = reader["setor_texto"].ToString();
							proc.Executor = reader["executor_nome"].ToString();

							protocolo.ProtocolosAssociados.Add(proc);
						}

						reader.Close();
					}

					#endregion
				}
			}

			return protocolo;
		}
	}
}