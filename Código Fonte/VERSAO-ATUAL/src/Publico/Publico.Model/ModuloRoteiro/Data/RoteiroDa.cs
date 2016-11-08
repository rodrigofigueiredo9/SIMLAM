using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Entities;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Data
{
	public class RoteiroDa
	{
		private string EsquemaBanco { get; set; }

		public RoteiroDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal Resultados<Roteiro> Filtrar(Filtro<ListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Roteiro> retorno = new Resultados<Roteiro>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("r.numero", "numero", filtros.Dados.Numero);

				comandtxt += comando.FiltroAnd("r.setor", "setor", filtros.Dados.Setor);

				comandtxt += comando.FiltroAnd("r.situacao", "situacao", filtros.Dados.Situacao);

				comandtxt += comando.FiltroAndLike("r.nome", "nome", filtros.Dados.Nome, true);

				comandtxt += comando.FiltroIn("r.id", 
					String.Format("select t.roteiro from {0}tab_roteiro_atividades t, {0}tab_atividade a where t.atividade = a.id and upper(a.atividade) like upper('%'||:atividade||'%')", EsquemaBanco), 
					"atividade", filtros.Dados.Atividade);

				comandtxt += comando.FiltroIn("r.id", String.Format("select a.roteiro from {0}tab_roteiro_chave a where upper(a.chave) like upper(:palavra_chave)",
					EsquemaBanco), "palavra_chave", filtros.Dados.PalavaChave);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "versao", "nome", "situacao_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_roteiro r where r.id > 0 
				and r.id not in (select t.roteiro from {0}tab_roteiro_atividades t, tab_atividade a where t.atividade = a.id and a.situacao = 0)" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select r.id, r.numero, r.versao, r.tid, r.nome, s.id setor_id, s.nome setor_texto, r.situacao, lrs.texto situacao_texto 
				from {0}tab_roteiro r, {0}tab_setor s, {0}lov_roteiro_situacao lrs where r.setor = s.id and r.situacao = lrs.id 
				and r.id not in (select t.roteiro from {0}tab_roteiro_atividades t, tab_atividade a where t.atividade = a.id and a.situacao = 0) " + comandtxt + DaHelper.Ordenar(colunas, ordenar);

				comando.DbCommand.CommandText = String.Format(@"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Roteiro roteiro;

					while (reader.Read())
					{
						roteiro = new Roteiro();
						roteiro.Id = Convert.ToInt32(reader["id"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.Versao = Convert.ToInt32(reader["versao"]);
						roteiro.Tid = reader["tid"].ToString();
						roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
						roteiro.SetorNome = reader["setor_texto"].ToString();
						roteiro.SituacaoTexto = reader["situacao_texto"].ToString();
						roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
						retorno.Itens.Add(roteiro);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}
	}
}