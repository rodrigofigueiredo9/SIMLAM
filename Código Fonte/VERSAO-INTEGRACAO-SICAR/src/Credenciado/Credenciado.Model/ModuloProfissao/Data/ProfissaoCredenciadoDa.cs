using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProfissao.Data
{
	public class ProfissaoCredenciadoDa
	{
		internal Resultados<Profissao> Filtrar(Filtro<ProfissaoListarFiltros> filtros, BancoDeDados banco = null)
		{
			Resultados<Profissao> retorno = new Resultados<Profissao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("p.texto", "texto", filtros.Dados.Texto, true, true);

				List<String> ordenar = new List<String>() { "texto" };
				List<String> colunas = new List<String>() { "texto" };

				#endregion

				if (!string.IsNullOrEmpty(filtros.Dados.Texto))
				{
					comandtxt += @" union all select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, max(trunc(metaphone.jaro_winkler(:filtro_fonetico,p.texto),5)) 
								similaridade from tab_profissao p, lov_profissao_origem o where p.origem = o.id and p.texto_fonema like upper('%' || upper(metaphone.gerarCodigo(:filtro_fonetico)) || '%') 
								and metaphone.jaro_winkler(:filtro_fonetico,p.texto) >= to_number(:limite_similaridade) group by p.id, p.texto, p.codigo, p.tid, p.origem, o.texto";

					comando.AdicionarParametroEntrada("filtro_fonetico", filtros.Dados.Texto, DbType.String);
					comando.AdicionarParametroEntrada("limite_similaridade", ConfiguracaoSistema.LimiteSimilaridade, DbType.String);
					colunas[0] = "similaridade";
					ordenar[0] = "similaridade";
				}

				#region Executa a pesquisa nas tabelas
				comando.DbCommand.CommandText = "select count(*) from (select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, 0 similaridade from tab_profissao p, lov_profissao_origem o where p.origem = o.id " + comandtxt + ")";

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, 1 similaridade 
				from tab_profissao p, lov_profissao_origem o where p.origem = o.id {0} {1}", comandtxt, DaHelper.Ordenar(colunas, ordenar, !string.IsNullOrEmpty(filtros.Dados.Texto)));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Profissao profissao;

					while (reader.Read())
					{
						profissao = new Profissao();
						profissao.Id = Convert.ToInt32(reader["id"]);
						
						if (retorno.Itens.Exists(x => x.Id == profissao.Id))
						{
							continue;
						}

						profissao.Tid = reader["tid"].ToString();
						profissao.Texto = reader["texto"].ToString();
						profissao.Codigo = reader["codigo"].ToString();
						profissao.OrigemId = Convert.ToInt32(reader["origem"]);

						retorno.Itens.Add(profissao);
					}

					reader.Close();
				}
			}

			return retorno;
		}

	}
}
