using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data
{	
	public class AtividadeProtocoloDa
	{
		#region Propriedades

		Historico _historico = new Historico();

		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public AtividadeProtocoloDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Atividade atividade, BancoDeDados banco)
		{
			if (atividade == null)
			{
				throw new Exception("A Atividade é nula.");
			}

			if (atividade.IdRelacionamento <= 0)
			{
				Criar(atividade, banco);
			}
			else
			{
				Editar(atividade, banco);
			}
		}

		internal int? Criar(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Atividades de Processo

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_atividades t (id, protocolo, requerimento, atividade, situacao, tid)
				values ({0}seq_protocolo_atividades.nextval, :protocolo, :requerimento, :atividade, 1, :tid) returning t.id into :id", EsquemaBanco);

				comando.AdicionarParametroSaida("id", DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", atividade.Protocolo.Requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				atividade.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Finalidades da Atividade

				if (atividade.Finalidades.Count > 0)
				{
					foreach (Finalidade itemAux in atividade.Finalidades)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_ativ_finalida (id, protocolo_ativ, modelo,
						titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
						({0}seq_protocolo_ativ_fin.nextval, :protocolo_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
						:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("protocolo_ativ", atividade.IdRelacionamento, DbType.Int32);
						comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
						comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
						comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
						comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
						comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);						
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				#endregion

				#endregion

				#region Histórico

				Historico.Gerar(atividade.IdRelacionamento.Value, eHistoricoArtefato.protocoloatividade, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return atividade.Id;
			}
		}

		internal void Editar(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Atividades de Processo

				bancoDeDados.IniciarTransacao();

				Comando comando;

				comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_atividades e set e.protocolo = :protocolo, e.requerimento = :requerimento,
				e.atividade = :atividade, e.tid =:tid where e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", atividade.IdRelacionamento, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", atividade.Protocolo.Requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#region Finalidades da Atividade

				if (atividade.Finalidades != null)
				{
					foreach (Finalidade itemAux in atividade.Finalidades)
					{
						if (itemAux.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_ativ_finalida a set protocolo_ativ = :protocolo_ativ,
							modelo = :modelo, titulo_anterior_tipo = :titulo_anterior_tipo, titulo_anterior_id = :titulo_anterior_id, titulo_anterior_numero = :titulo_anterior_numero, 
							modelo_anterior_id = :modelo_anterior_id, modelo_anterior_nome = :modelo_anterior_nome, modelo_anterior_sigla = :modelo_anterior_sigla, orgao_expedidor = :orgao_expedidor, 
							finalidade = :finalidade, tid = :tid where id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", itemAux.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_protocolo_ativ_finalida (id, protocolo_ativ, modelo,
							titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
							({0}seq_protocolo_ativ_fin.nextval, :protocolo_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
							:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("protocolo_ativ", atividade.IdRelacionamento, DbType.Int32);
						comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
						comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
						comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
						comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
						comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);						
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				#endregion						

				#endregion

				#region Histórico

				Historico.Gerar(atividade.IdRelacionamento.Value, eHistoricoArtefato.protocoloatividade, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion
	}
}
