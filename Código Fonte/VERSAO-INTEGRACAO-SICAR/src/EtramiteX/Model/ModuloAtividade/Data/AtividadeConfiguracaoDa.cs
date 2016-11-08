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

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data
{
	public class AtividadeConfiguracaoDa
	{
		#region Propriedade

		Historico _historico = new Historico();

		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public AtividadeConfiguracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(AtividadeConfiguracao atividade, BancoDeDados banco = null)
		{
			if (atividade == null)
			{
				throw new Exception("A Configuração da Atividade é nula.");
			}

			if (atividade.Id <= 0)
			{
				Criar(atividade, banco);
			}
			else
			{
				Editar(atividade, banco);
			}
		}

		internal int? Criar(AtividadeConfiguracao atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Configuração de Atividade

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}cnf_atividade e (id, nome, tid)
				values ({0}seq_cnf_atividade.nextval, :nome, :tid) returning e.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", DbType.String, 100, atividade.NomeGrupo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				atividade.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Modelos de Títulos

				if (atividade.Modelos != null && atividade.Modelos.Count > 0)
				{
					foreach (Modelo item in atividade.Modelos)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}cnf_atividade_modelos(id, configuracao, modelo, tid)
						values ({0}seq_cnf_atividade_modelos.nextval, :configuracao, :modelo, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("configuracao", atividade.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("modelo", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Atividades

				if (atividade.Atividades != null && atividade.Atividades.Count > 0)
				{
					foreach (AtividadeSolicitada item in atividade.Atividades)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}cnf_atividade_atividades(id, configuracao, atividade, tid)
						values ({0}seq_cnf_atividade_atividades.nextval, :configuracao, :atividade, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("configuracao", atividade.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(atividade.Id, eHistoricoArtefato.cnfatividades, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return atividade.Id;
			}
		}

		internal void Editar(AtividadeConfiguracao configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Configuração de Atividade

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_atividade t set t.nome = :nome, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", configuracao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 100, configuracao.NomeGrupo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Modelos de Títulos
				comando = bancoDeDados.CriarComando(@"delete {0}cnf_atividade_modelos s ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where s.configuracao = :configuracao{0}",
				comando.AdicionarNotIn("and", "s.id", DbType.Int32, configuracao.Modelos.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Atividades
				comando = bancoDeDados.CriarComando(@"delete {0}cnf_atividade_atividades s ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where s.configuracao = :configuracao{0}",
				comando.AdicionarNotIn("and", "s.id", DbType.Int32, configuracao.Atividades.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Modelos de Títulos

				if (configuracao.Modelos != null && configuracao.Modelos.Count > 0)
				{
					foreach (Modelo item in configuracao.Modelos)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}cnf_atividade_modelos e set e.configuracao = :configuracao, e.modelo = :modelo, 
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}cnf_atividade_modelos s (id, configuracao, modelo, tid)
							values ({0}seq_cnf_atividade_modelos.nextval, :configuracao, :modelo, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("modelo", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Atividades

				if (configuracao.Atividades != null && configuracao.Atividades.Count > 0)
				{
					foreach (AtividadeSolicitada item in configuracao.Atividades)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}cnf_atividade_atividades e set e.configuracao = :configuracao, e.atividade = :atividade, 
							e.tid = :tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}cnf_atividade_atividades s (id, configuracao, atividade, tid)
							values ({0}seq_cnf_atividade_atividades.nextval, :configuracao, :atividade, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(configuracao.Id, eHistoricoArtefato.cnfatividades, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_atividade c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.cnfatividades, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da titulo

				List<String> lista = new List<string>();
				lista.Add("delete from {0}cnf_atividade_modelos e where e.configuracao = :configuracao;");
				lista.Add("delete from {0}cnf_atividade_atividades e where e.configuracao = :configuracao;");
				lista.Add("delete from {0}cnf_atividade e where e.id = :configuracao;");

				comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("configuracao", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Resultados<AtividadeConfiguracao> Filtrar(Filtro<ListarConfigurarcaoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<AtividadeConfiguracao> retorno = new Resultados<AtividadeConfiguracao>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("a.nome", "nome", filtros.Dados.NomeGrupo + "%", true);

				comandtxt += comando.FiltroIn("a.id", String.Format(@"select m.configuracao from {0}cnf_atividade_modelos m where m.modelo = :modelo",
				(string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "modelo", filtros.Dados.Modelo);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.AtividadeSolicitada))
				{
					comandtxt += comando.FiltroIn("a.id", String.Format(@"select m.configuracao from {0}cnf_atividade_atividades m, {0}tab_atividade a where m.atividade = a.id and lower(trim(a.atividade)) like '%'||:atividade||'%'",
					(string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "atividade", filtros.Dados.AtividadeSolicitada.ToLower().Trim());
				}

				if (filtros.Dados.SetorId > 0)
				{
					comandtxt += " and a.id in (select t.configuracao from cnf_atividade_atividades t, tab_atividade ta where t.atividade = ta.id and ta.setor = :setor)";
					comando.AdicionarParametroEntrada("setor", filtros.Dados.SetorId, DbType.Int32);
				}

				if (filtros.Dados.AgrupadorId > 0)
				{
					comandtxt += " and a.id in (select t.configuracao from cnf_atividade_atividades t, tab_atividade ta where t.atividade = ta.id and ta.agrupador = :agrupador)";
					comando.AdicionarParametroEntrada("agrupador", filtros.Dados.AgrupadorId, DbType.Int32);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome");
				}
				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}cnf_atividade a where a.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select a.id, a.nome, a.tid from {0}cnf_atividade a where 1=1 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					AtividadeConfiguracao atividade;
					while (reader.Read())
					{
						atividade = new AtividadeConfiguracao();
						atividade.Id = Convert.ToInt32(reader["id"]);
						atividade.NomeGrupo = reader["nome"].ToString();

						retorno.Itens.Add(atividade);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal AtividadeConfiguracao Obter(int id, BancoDeDados banco = null, bool obterHistorico = false)
		{
			AtividadeConfiguracao atividade = new AtividadeConfiguracao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Configuração de Atividade

				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.nome, a.tid from {0}cnf_atividade a where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						atividade.Id = id;
						atividade.Tid = reader["tid"].ToString();
						atividade.NomeGrupo = reader["nome"].ToString();
					}
					reader.Close();
				}

				#endregion

                if (obterHistorico)
                {
                    return ObterHistorico(atividade, banco);
                }

				#region Modelos de Títulos

				comando = bancoDeDados.CriarComando(@"select m.id, m.configuracao, m.modelo, tm.nome modelo_nome, m.tid from {0}cnf_atividade_modelos m, {0}tab_titulo_modelo tm 
				where m.modelo = tm.id and m.configuracao = :configuracao", EsquemaBanco);

				comando.AdicionarParametroEntrada("configuracao", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Modelo modelo;

					while (reader.Read())
					{
						modelo = new Modelo();
						modelo.IdRelacionamento = Convert.ToInt32(reader["id"]);
						modelo.Id = Convert.ToInt32(reader["modelo"]);
						modelo.Texto = reader["modelo_nome"].ToString();
						atividade.Modelos.Add(modelo);
					}
					reader.Close();
				}

				#endregion

				#region atividades

				comando = bancoDeDados.CriarComando(@"select a.id, a.configuracao, a.atividade, ta.atividade atividade_nome, a.tid, ta.setor from {0}cnf_atividade_atividades a, 
				{0}tab_atividade ta where a.atividade = ta.id and a.configuracao = :configuracao", EsquemaBanco);

				comando.AdicionarParametroEntrada("configuracao", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AtividadeSolicitada ativ;

					while (reader.Read())
					{
						ativ = new AtividadeSolicitada();
						ativ.IdRelacionamento = Convert.ToInt32(reader["id"]);
						ativ.Id = Convert.ToInt32(reader["atividade"]);
						ativ.Texto = reader["atividade_nome"].ToString();
						ativ.SetorId = Convert.ToInt32(reader["setor"]);

						atividade.Atividades.Add(ativ);
					}
					reader.Close();
				}

				#endregion

			}
			return atividade;
		}

        private AtividadeConfiguracao ObterHistorico(AtividadeConfiguracao atividade, BancoDeDados banco = null) 
        { 
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {

                #region Modelos de Títulos

                Comando comando = bancoDeDados.CriarComando(@"select hm.configuracao_id id, hm.modelo_id modelo, htm.nome modelo_nome, hm.tid from {0}hst_cnf_atividade_modelos hm, 
                {0}hst_titulo_modelo htm where hm.modelo_id = htm.modelo_id and hm.modelo_tid = htm.tid and hm.configuracao_id = :config_id and hm.configuracao_tid = 
                :config_tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("config_id", atividade.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("config_tid", atividade.Tid, DbType.String);


                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Modelo modelo;

                    while (reader.Read())
                    {
                        modelo = new Modelo();
                        modelo.Id = Convert.ToInt32(reader["modelo"]);
                        modelo.Texto = reader["modelo_nome"].ToString();
                        atividade.Modelos.Add(modelo);
                    }
                    reader.Close();
                }

                #endregion

                #region atividades

                comando = bancoDeDados.CriarComando(@"select ha.atividade_ativ_id, ha.configuracao_id, hta.atividade atividade_nome, ha.tid, hta.setor_id from hst_cnf_atividade_atividades 
                ha, hst_atividade hta where ha.atividade_id = hta.atividade_id and ha.atividade_tid = hta.tid and ha.configuracao_id = :config_id and 
                ha.configuracao_tid = :config_tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("config_id", atividade.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("config_tid", atividade.Tid, DbType.String);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    AtividadeSolicitada ativ;

                    while (reader.Read())
                    {
                        ativ = new AtividadeSolicitada();
                        ativ.Id = Convert.ToInt32(reader["atividade_ativ_id"]);
                        ativ.Texto = reader["atividade_nome"].ToString();
                        ativ.SetorId = Convert.ToInt32(reader["setor_id"]);

                        atividade.Atividades.Add(ativ);
                    }
                    reader.Close();
                }

                #endregion
            }

            return atividade;
        }


		internal List<TituloModeloLst> ObterModelosSolicitadoExterno()
		{
			List<TituloModeloLst> modelos = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Modelos de Títulos

				Comando comando = bancoDeDados.CriarComando(@"select tm.id, tm.nome from {0}tab_titulo_modelo tm, {0}tab_titulo_modelo_regras r where tm.id = r.modelo
				and r.regra = 8 and tm.situacao = 1 order by tm.nome", EsquemaBanco); //8 = É solicitado pelo público externo? Situaçao Ativo

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloModeloLst modelo;

					while (reader.Read())
					{
						modelo = new TituloModeloLst();
						modelo.Id = Convert.ToInt32(reader["id"]);
						modelo.Texto = reader["nome"].ToString();
						modelo.IsAtivo = true;

						modelos.Add(modelo);
					}
					reader.Close();
				}

				#endregion
			}

			return modelos;
		}

		internal List<TituloModeloLst> ObterModelosAtividades(List<AtividadeSolicitada> atividades, bool renovacao = false, BancoDeDados banco = null)
		{
			List<TituloModeloLst> modelos = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiros

				Comando comando = bancoDeDados.CriarComando(@"select tm.id, tm.nome from cnf_atividade_atividades a, cnf_atividade_modelos m, tab_titulo_modelo tm
				where a.configuracao = m.configuracao and tm.id = m.modelo ");

				comando.DbCommand.CommandText += comando.AdicionarIn("and", "a.atividade", DbType.Int32, atividades.Select(x => x.Id).ToList());

				if (renovacao)
				{
					comando.DbCommand.CommandText += String.Format("and tm.id in (select r.modelo from {0}tab_titulo_modelo_regras r where r.regra = 6)", EsquemaBanco);
				}

				comando.DbCommand.CommandText += " group by tm.id, tm.nome";

				bancoDeDados.ExecutarNonQuery(comando);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloModeloLst modelo;
					while (reader.Read())
					{
						modelo = new TituloModeloLst();
						modelo.Id = Convert.ToInt32(reader["id"]);
						modelo.Texto = reader["nome"].ToString();

						modelos.Add(modelo);
					}
					reader.Close();
				}

				#endregion
			}
			return modelos;
		}

		#endregion

		#region Validações

		public AtividadeConfiguracao AtividadeConfigurada(int id)
		{
			AtividadeConfiguracao atividadeConfiguracao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select ca.nome from cnf_atividade_atividades a, cnf_atividade ca where a.configuracao = ca.id and a.atividade = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						atividadeConfiguracao = new AtividadeConfiguracao();
						atividadeConfiguracao.NomeGrupo = reader["nome"].ToString();
					}

					reader.Close();
				}
			}
			return atividadeConfiguracao;
		}

		internal bool AtividadeConfigurada(int atividadeId, int modeloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from cnf_atividade ca, cnf_atividade_atividades caa,
					cnf_atividade_modelos    cam where ca.id = caa.configuracao and ca.id = cam.configuracao and cam.modelo = :modelo
					and caa.atividade = :atividade", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividade", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}