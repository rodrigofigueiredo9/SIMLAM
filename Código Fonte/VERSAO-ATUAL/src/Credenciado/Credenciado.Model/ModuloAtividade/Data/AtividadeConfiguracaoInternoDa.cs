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

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data
{
	public class AtividadeConfiguracaoInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public AtividadeConfiguracaoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal bool AtividadeConfigurada(int atividadeId, int modeloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from cnf_atividade ca, cnf_atividade_atividades caa,
					cnf_atividade_modelos    cam where ca.id = caa.configuracao and ca.id = cam.configuracao and cam.modelo = :modelo
					and caa.atividade = :atividade", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividade", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar<int>(comando));
			}
		}

		internal List<TituloModeloLst> ObterModelosAtividades(List<AtividadeSolicitada> atividades, bool renovacao = false, BancoDeDados banco = null)
		{
			List<TituloModeloLst> modelos = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiros

				Comando comando = bancoDeDados.CriarComando(@"select tm.id, tm.nome from cnf_atividade_atividades a, cnf_atividade_modelos m, tab_titulo_modelo tm
				where a.configuracao = m.configuracao and tm.id = m.modelo and tm.situacao != 2");

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

	}
}