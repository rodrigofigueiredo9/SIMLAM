using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Data
{
	public class TramitacaoCredenciadoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public TramitacaoCredenciadoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal Resultados<Tramitacao> FiltrarHistorico(Filtro<ListarTramitacaoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Tramitacao> retorno = new Resultados<Tramitacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				#region Filtros de protocolo

				comandtxt += comando.FiltroAnd("t.protocolo_id", "protocolo_id", filtros.Dados.Protocolo.Id);

				comandtxt += comando.FiltroAnd("t.protocolo", "protocolo", filtros.Dados.ProtocoloTipo);

				comandtxt += comando.FiltroAnd("t.protocolo_numero", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("t.protocolo_ano", "ano", filtros.Dados.Protocolo.Ano);

				#endregion

				comandtxt += comando.FiltroAnd("t.remetente_id", "remetente_id", filtros.Dados.RemetenteId);

				comandtxt += comando.FiltroAnd("t.remetente_setor_id", "remetente_setor_id", filtros.Dados.RemetenteSetorId);

				comandtxt += comando.FiltroAnd("t.destinatario_id", "destinatario_id", filtros.Dados.DestinatarioId);

				comandtxt += comando.FiltroAnd("t.destinatario_setor_id", "destinatario_setor_id", filtros.Dados.DestinatarioSetorId);

				comandtxt += comando.FiltroIn("t.destinatario_setor_id", String.Format("select s.setor from {0}tab_funcionario_setor s where s.funcionario = :funcionario_setor_destino", esquema),
				"funcionario_setor_destino", filtros.Dados.FuncionarioSetorDestinoId);

				comandtxt += comando.FiltroIn("t.destinatario_setor_id", String.Format("select s.setor from {0}tab_funcionario_setor s, {0}tab_tramitacao_setor_func fs where fs.funcionario = s.funcionario and s.funcionario = :registrador_setor", esquema),
				"registrador_setor", filtros.Dados.RegistradorDestinatarioSetorId);

				comandtxt += comando.FiltroIn("t.destinatario_setor_id", String.Format(@"select s.setor from {0}tab_funcionario_setor s, {0}tab_tramitacao_setor_func fs where fs.funcionario = s.funcionario and s.funcionario = :emposse_registrador", esquema),
				"emposse_registrador", filtros.Dados.RegistradorDestinatarioId, "or t.destinatario_id = :emposse_registrador");

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "protocolo_numero,protocolo_ano", "protocolo", "data_envio", "objetivo_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("protocolo_numero,protocolo_ano");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}lst_hst_tramitacao t where t.id > 0" + comandtxt, esquema);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.DbCommand.CommandText = String.Format(@"select t.id, t.tid, t.tramitacao_id, t.tramitacao_hst_id, t.protocolo, t.protocolo_id, t.protocolo_numero, t.protocolo_ano, 
				t.protocolo_numero_completo, t.protocolo_tipo_texto, t.remetente_id, t.remetente_tid, t.remetente_nome, t.remetente_setor_id, 
				t.remetente_setor_sigla, t.remetente_setor_nome, t.destinatario_id, t.destinatario_tid, t.destinatario_nome, t.destinatario_setor_id, 
				t.destinatario_setor_sigla, t.destinatario_setor_nome, t.objetivo_id, t.objetivo_texto, t.situacao_id, t.situacao_texto, 
				t.acao_executada, (select aa.texto from {0}lov_historico_artefatos_acoes a, {0}lov_historico_acao aa where a.acao = aa.id and a.id =  t.acao_executada) acao_executada_texto,
				t.data_envio, t.data_execucao data_recebimento from {0}lst_hst_tramitacao t where t.id > 0"
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), esquema);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Tramitacao tramitacao;

					while (reader.Read())
					{
						tramitacao = new Tramitacao();
						tramitacao.Id = Convert.ToInt32(reader["tramitacao_id"]);
						tramitacao.HistoricoId = Convert.ToInt32(reader["tramitacao_hst_id"]);

						tramitacao.DataEnvio.Data = Convert.IsDBNull(reader["data_envio"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_envio"]);
						tramitacao.DataRecebido.Data = Convert.IsDBNull(reader["data_recebimento"]) ? null : (DateTime?)Convert.ToDateTime(reader["data_recebimento"]);
						tramitacao.DataExecucao.Data = tramitacao.DataRecebido.Data;

						tramitacao.Tid = reader["tid"].ToString();

						if (reader["objetivo_id"] != null && !Convert.IsDBNull(reader["objetivo_id"]))
						{
							tramitacao.Objetivo.Id = Convert.ToInt32(reader["objetivo_id"]);
							tramitacao.Objetivo.Texto = Convert.ToString(reader["objetivo_texto"]);
						}

						if (reader["remetente_id"] != null && !Convert.IsDBNull(reader["remetente_id"]))
						{
							tramitacao.Remetente.Id = Convert.ToInt32(reader["remetente_id"]);
						}

						tramitacao.Remetente.Nome = reader["remetente_nome"].ToString();

						if (reader["remetente_setor_id"] != null && !Convert.IsDBNull(reader["remetente_setor_id"]))
						{
							tramitacao.RemetenteSetor.Id = Convert.ToInt32(reader["remetente_setor_id"]);
						}

						tramitacao.RemetenteSetor.Sigla = Convert.ToString(reader["remetente_setor_sigla"]);
						tramitacao.RemetenteSetor.Nome = Convert.ToString(reader["remetente_setor_nome"]);


						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							tramitacao.Destinatario.Id = Convert.ToInt32(reader["destinatario_id"]);
						}

						tramitacao.Destinatario.Nome = Convert.ToString(reader["destinatario_nome"]);

						if (reader["destinatario_setor_id"] != null && !Convert.IsDBNull(reader["destinatario_setor_id"]))
						{
							tramitacao.DestinatarioSetor.Id = Convert.ToInt32(reader["destinatario_setor_id"]);
						}

						tramitacao.DestinatarioSetor.Sigla = Convert.ToString(reader["destinatario_setor_sigla"]);
						tramitacao.DestinatarioSetor.Nome = Convert.ToString(reader["destinatario_setor_nome"]);

						tramitacao.IsExisteHistorico = (reader["tramitacao_hst_id"] != null && !Convert.IsDBNull(reader["tramitacao_hst_id"]));
						tramitacao.Protocolo.IsProcesso = (reader["protocolo"] != null && reader["protocolo"].ToString() == "1");
						tramitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						tramitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]);
						tramitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);
						tramitacao.Protocolo.Tipo.Texto = reader["protocolo_tipo_texto"].ToString();

						if (reader["acao_executada"] != null && !Convert.IsDBNull(reader["acao_executada"]))
						{
							tramitacao.AcaoId = Convert.ToInt32(reader["acao_executada"]);
							tramitacao.AcaoExecutada = reader["acao_executada_texto"].ToString();
						}

						retorno.Itens.Add(tramitacao);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
		}
	}
}