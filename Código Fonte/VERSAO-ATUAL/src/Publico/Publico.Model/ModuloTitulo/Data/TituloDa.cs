using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloTitulo.Data
{
	public class TituloDa
	{
		private string EsquemaBanco { get; set; }

		public Resultados<Titulo> Filtrar(Filtro<TituloFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Titulo> retorno = new Resultados<Titulo>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.AdicionarIn("and", "t.situacao_id", DbType.Int32, new List<int>() { 3, 5, 6 });
				comandtxt += comando.FiltroIn("t.modelo_id", "select re.modelo from tab_titulo_modelo_regras re where re.regra = :regra", "regra", 8);
				
				comandtxt += comando.FiltroAnd("t.modelo_id", "modelo", filtros.Dados.Modelo);

				comandtxt += comando.FiltroAnd("t.setor_id", "setor", filtros.Dados.Setor);

				comandtxt += comando.FiltroAndLike("t.numero || '/' || t.ano", "numero", filtros.Dados.Numero, true);

				comandtxt += comando.FiltroAndLike("t.protocolo_numero", "protocolo_numero", filtros.Dados.Protocolo.NumeroTexto, true);

				comandtxt += comando.FiltroAndLike("t.empreendimento_denominador", "empreendimento", filtros.Dados.Empreendimento, true);

				if (filtros.Dados.Modelo <= 0 && filtros.Dados.ModeloFiltrar != null && filtros.Dados.ModeloFiltrar.Count > 0)
				{
					comandtxt += comando.AdicionarIn("and", "t.modelo_id", DbType.Int32, filtros.Dados.ModeloFiltrar.Select(x => x).ToList());
				}

				if (filtros.Dados.SituacoesFiltrar != null && filtros.Dados.SituacoesFiltrar.Count > 0)
				{
					comandtxt += comando.AdicionarIn("and", "t.situacao_id", DbType.Int32, filtros.Dados.SituacoesFiltrar.Select(x => x).ToList());
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "modelo_sigla", "protocolo_numero", "empreendimento", "situacao_texto", "data_vencimento" };

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

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}lst_titulo t where t.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select t.titulo_id id, t.titulo_tid, t.numero_completo, t.data_vencimento, t.autor_id, t.autor_nome, t.modelo_sigla, t.situacao_texto, t.modelo_id,
				t.modelo_nome, t.protocolo_id, t.protocolo protocolo_tipo, t.protocolo_numero, t.empreendimento_denominador empreendimento from 
				{0}lst_titulo t where 1 = 1 " + comandtxt + DaHelper.Ordenar(colunas, ordenar),
				(string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Titulo titulo;
					while (reader.Read())
					{
						titulo = new Titulo();
						titulo.Id = Convert.ToInt32(reader["id"]);
						titulo.Autor.Id = Convert.ToInt32(reader["autor_id"]);
						titulo.Autor.Nome = reader["autor_nome"].ToString();
						titulo.Tid = reader["titulo_tid"].ToString();

						if (reader["numero_completo"] != null && !Convert.IsDBNull(reader["numero_completo"]))
						{
							titulo.Numero.Texto = reader["numero_completo"].ToString();
						}

						if (reader["modelo_id"] != null && !Convert.IsDBNull(reader["modelo_id"]))
						{
							titulo.Modelo.Id = Convert.ToInt32(reader["modelo_id"]);
						}

						if (reader["modelo_sigla"] != null && !Convert.IsDBNull(reader["modelo_sigla"]))
						{
							titulo.Modelo.Sigla = reader["modelo_sigla"].ToString();
						}

						if (reader["modelo_nome"] != null && !Convert.IsDBNull(reader["modelo_nome"]))
						{
							titulo.Modelo.Nome = reader["modelo_nome"].ToString();
						}

						if (reader["situacao_texto"] != null && !Convert.IsDBNull(reader["situacao_texto"]))
						{
							titulo.Situacao.Nome = reader["situacao_texto"].ToString();
						}

						titulo.Protocolo.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
						{
							ProtocoloNumero prot = new ProtocoloNumero(reader["protocolo_numero"].ToString());
							titulo.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
							titulo.Protocolo.NumeroProtocolo = prot.Numero;
							titulo.Protocolo.Ano = prot.Ano;
						}

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							titulo.EmpreendimentoTexto = reader["empreendimento"].ToString();
						}

						if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
						{
							titulo.DataVencimento.Data = Convert.ToDateTime(reader["data_vencimento"]);
						}

						retorno.Itens.Add(titulo);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal List<Situacao> ObterSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_situacao t order by texto");

			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}
	}
}
