using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data
{
	public class AtividadeInternoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		#region Ações de DML

		public void AlterarSituacao(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = null;

				if ((atividade.IdRelacionamento ?? 0) <= 0)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_atividades a set a.situacao = :situacao, a.motivo = :motivo, a.tid = :tid 
						where a.protocolo = :protocolo and a.atividade = :atividade returning a.id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_atividades a set a.situacao = :situacao, a.motivo = :motivo, a.tid = :tid where a.id = :atividade_id_relacionamento", EsquemaBanco);
					comando.AdicionarParametroEntrada("atividade_id_relacionamento", atividade.IdRelacionamento, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("situacao", atividade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", atividade.Motivo, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if ((atividade.IdRelacionamento ?? 0) <= 0)
				{
					atividade.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				Historico.Gerar(atividade.IdRelacionamento.Value, eHistoricoArtefato.protocoloatividade, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public AtividadeInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public int ObterAtividadeProtocoloSituacao(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				if (atividade.IdRelacionamento > 0)
				{
					comando = bancoDeDados.CriarComando(@"select a.situacao atividade_situacao_id from {0}tab_protocolo_atividades a where a.id = :atividadeIdRelacionamento", EsquemaBanco);
					comando.AdicionarParametroEntrada("atividadeIdRelacionamento", atividade.IdRelacionamento, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select a.situacao atividade_situacao_id from {0}tab_protocolo_atividades a where a.atividade = :atividade and a.protocolo = :protocolo", EsquemaBanco);
					comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
				}

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		public Resultados<AtividadeListarFiltro> Filtrar(Filtro<AtividadeListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<AtividadeListarFiltro> retorno = new Resultados<AtividadeListarFiltro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if (!string.IsNullOrWhiteSpace(filtros.Dados.AtividadeNome))
				{
					comandtxt += " and lower(trim(ta.atividade)) like '%'||:atividade||'%' ";
					comando.AdicionarParametroEntrada("atividade", filtros.Dados.AtividadeNome.ToLower().Trim(), DbType.String);
				}

				if (filtros.Dados.SetorId > 0)
				{
					comandtxt += " and ts.id = :setor ";
					comando.AdicionarParametroEntrada("setor", filtros.Dados.SetorId, DbType.Int32);
				}

				if (filtros.Dados.AgrupadorId > 0)
				{
					comandtxt += " and ta.agrupador = :agrupador ";
					comando.AdicionarParametroEntrada("agrupador", filtros.Dados.AgrupadorId, DbType.Int32);
				}

				if (filtros.Dados.ExibirCredenciado.HasValue)
				{
					comandtxt += " and ta.exibir_credenciado = :exibir_credenciado";

					comando.AdicionarParametroEntrada("exibir_credenciado", Convert.ToInt32(filtros.Dados.ExibirCredenciado.Value), DbType.Int32);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "setor_texto", "atividade", "agrupador_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("setor_texto");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_atividade ta, {0}tab_setor ts where ta.situacao=1 and ta.setor = ts.id " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));


				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Processo.NaoEncontrouRegistros);
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select ta.id, ta.atividade, ta.setor setor_id, ts.sigla setor_sigla, ts.nome setor_texto, taa.nome agrupador_texto 
				from {2}tab_atividade ta, {2}tab_setor ts, {2}tab_atividade_agrupador taa where ta.situacao=1 and ta.agrupador = taa.id and ta.setor = ts.id {0} {1}",
				comandtxt, DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AtividadeListarFiltro atividade;
					while (reader.Read())
					{
						atividade = new AtividadeListarFiltro();
						atividade.Id = Convert.ToInt32(reader["id"]);
						atividade.AtividadeNome = reader["atividade"].ToString();
						atividade.SetorId = Convert.ToInt32(reader["setor_id"]);
						atividade.SetorTexto = reader["setor_texto"].ToString();
						atividade.SetorSigla = reader["setor_sigla"].ToString();
						atividade.AgrupadorTexto = reader["agrupador_texto"].ToString();

						retorno.Itens.Add(atividade);
					}
					reader.Close();
				}
			}

			return retorno;
		}

		internal bool VerificarAtividadeAssociadaTitulo(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(t.id) from {0}tab_titulo_atividades tt, {0}tab_titulo t where t.id = tt.titulo
				and tt.atividade = :atividade and t.modelo = :modelo and ((t.situacao = 5 and t.situacao_motivo not in(1, 4)) or (t.situacao not in (1,5))) and tt.protocolo = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modelo, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarAtividadeComTituloOuEncerrada(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				//6 - Encerrada
				Comando comando = bancoDeDados.CriarComando(@"select (select count(d.id) from {0}tab_protocolo_atividades d where d.protocolo = :protocolo 
				and d.atividade = :atividade and d.situacao = 6) + (select count(t.id) from {0}tab_titulo_atividades ta, {0}tab_titulo t 
				where t.id = ta.titulo and t.modelo = :modelo and ta.atividade = :atividade and ta.protocolo = :protocolo
				and ((t.situacao = 5 and t.situacao_motivo not in(1, 4)) or (t.situacao not in (1,5)))) qtd from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modelo, DbType.Int32);

				return bancoDeDados.ExecutarScalar<bool>(comando);
			}
		}

		internal bool AtividadeAtiva(int atividadeId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_atividade t where t.situacao = 1 and t.id = :atividade_id", EsquemaBanco);
				comando.AdicionarParametroEntrada("atividade_id", atividadeId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal Atividade ObterAtividadePorCodigo(int codigo)
		{
			Atividade retorno = new Atividade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.atividade from {0}tab_atividade t where t.codigo = :codigo", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo", codigo);
				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					retorno = new Atividade()
					{
						Id = Convert.ToInt32(item["id"]),
						NomeAtividade = item["atividade"].ToString()
					};
				}
			}

			return retorno;
		}

		internal List<AtividadeSolicitada> ObterAtividadesListaReq(int requerimentoId)
		{
			var lst = new List<AtividadeSolicitada>();

			var sql = @"
			select ta.id Id, 
					ta.atividade Texto, 
					ta.situacao IsAtivo
				from tab_requerimento_atividade tr, 
					tab_atividade ta
				where tr.atividade = ta.id
				and tr.requerimento = :requerimentoId";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				using (var comando = bancoDeDados.CriarComando(sql, EsquemaBanco))
				{
					comando.AdicionarParametroEntrada("requerimentoId", requerimentoId);
					lst = bancoDeDados.ObterEntityList<AtividadeSolicitada>(comando);
				}
			}

			return lst;
		}

		#endregion
	}
}