using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	public class Executor
	{
		int contadorAux = 0;
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;

		public String UsuarioRelatorio
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioRelatorio); }
		}

		public Executor()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		}

		public DadosRelatorio Executar(ConfiguracaoRelatorio configuracao)
		{
			List<List<ValoresBanco>> dados = new List<List<ValoresBanco>>();
			DadosRelatorio retorno = null;
			using (BancoDeDados banco = BancoDeDados.ObterInstancia(UsuarioRelatorio))
			{
				// Montar o comando
				Comando comando = MontarSQL(configuracao, banco);

				// Executar
				using (IDataReader reader = banco.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader.FieldCount > 0)
						{
							List<ValoresBanco> lista = new List<ValoresBanco>();
							for (var i = 0; i < reader.FieldCount; i++)
							{
								lista.Add(new ValoresBanco()
								{
									Chave = (configuracao.Agrupamentos.FirstOrDefault(x => x.CampoId == Convert.ToInt32(reader.GetName(i))) ?? new ConfiguracaoAgrupamento()).Alias,
									Valor = reader.GetValue(i)
								});

								lista.Where(x => string.IsNullOrEmpty(x.Chave)).ToList().ForEach(r => {
									r.Chave = configuracao.CamposSelecionados.First(x => x.CampoId == Convert.ToInt32(reader.GetName(i))).Alias;
								});

								if (lista.Last().Valor is DateTime)
								{
									DateTime dataAux = Convert.ToDateTime(lista.Last().Valor);

									if(dataAux.ToLongTimeString() == DateTime.MinValue.ToLongTimeString())
									{
										lista.Last().Valor = dataAux.ToShortDateString();
									}
								}
							}
							dados.Add(lista);
						}
					}
					reader.Close();
				}
				
				// Agrupar
				retorno = CarregarDados(configuracao, dados);                

				//Data de execução
                comando = banco.CriarComando(@"select t.execucao_fim data from cnf_fato_etl t, tab_fato f where t.id = f.id_fato_etl and f.nome = :nome");
                comando.AdicionarParametroEntrada("nome", configuracao.FonteDados.Nome, DbType.String);

                retorno.Data = banco.ExecutarScalar(comando).ToString();

				// Sumarizar 
				Sumarizar(configuracao, retorno);
			}

			return retorno;
		}

		private DadosRelatorio CarregarDados(ConfiguracaoRelatorio configuracao, List<List<ValoresBanco>> dados)
		{
			Dictionary<int, string> colunas = new Dictionary<int, string>();
			configuracao.CamposSelecionados.OrderBy(x=>x.Posicao).ToList().ForEach(x => {
				colunas.Add(x.Posicao, x.Alias);
			});

			DadosRelatorio retorno = new DadosRelatorio(colunas);
			retorno.Nome = configuracao.Nome;
			retorno.Totalizar = configuracao.ContarRegistros;
			retorno.ComAgrupamento = configuracao.Agrupamentos.Count > 0;
			string grupo = null;
			GrupoDados grupoDados = null;
			int linha = 0;

			foreach (var item in dados)
			{
				int c = 0;
				foreach (var coluna in item)
				{
					if (!retorno.ComAgrupamento)
					{
						retorno.Dados[linha, c] = coluna.Valor.ToString();
					}
					else
					{
						if (c == 0)
						{
							string grupoAtual = coluna.Valor.ToString();
							if (grupo != grupoAtual)
							{
								grupo = grupoAtual;
								grupoDados = retorno.CriarGrupo();
								grupoDados.Campo = configuracao.Agrupamentos.Single(x => x.Alias == coluna.Chave).Campo.Alias;
								grupoDados.Valor = grupo;
								linha = 0;
							}
						}
						else
						{
							grupoDados.Dados[linha, (c - configuracao.Agrupamentos.Count)] = coluna.Valor.ToString();
						}
					}

					c++;
				}
				linha++;
			}
			
			foreach (var campo in configuracao.CamposSelecionados.OrderBy(x=>x.Posicao))
			{
				try
				{
					retorno.Campos.Add(new ValoresBanco() { Chave = campo.Alias, Valor = campo.Tamanho.ToString(), DimensaoColuna = campo.Campo.DimensaoNome + "@" + campo.Campo.Nome });
				}
				catch(Exception e)
				{
					throw new Exception(campo.CampoId + " - " + campo.Alias + " - " + campo.Posicao + " - " + campo.Tamanho, e);
				}
			}

			return retorno;
		}

		#region SQL

		private Comando MontarSQL(ConfiguracaoRelatorio configuracao, BancoDeDados banco)
		{
			#region Alias de tabelas

			List<ConfiguracaoCampo> campos = configuracao.CamposSelecionados.Where(c => c.Campo != null).ToList();

			List<string> tabelas = campos.Select(x => x.Campo.Tabela).Union(
			configuracao.Agrupamentos.Select(a => a.Campo.Tabela).Union(
			configuracao.Termos.Where(t => (eTipoTermo)t.Tipo == eTipoTermo.Filtro).Select(t => t.Campo.Tabela)).Union(
			configuracao.Ordenacoes.Select(o=>o.Campo.Tabela))
			).Distinct().ToList();

			if (!tabelas.Contains(configuracao.FonteDados.Tabela))
			{
				tabelas.Add(configuracao.FonteDados.Tabela);
			}

			Dictionary<string, string> aliasesTabelas = new Dictionary<string, string>();

			int i = 0;
			foreach (var tab in tabelas)
			{
				aliasesTabelas.Add(tab, string.Format("t{0}", i));
				i++;
			}

			#endregion

			#region SQL - SELECT

			string selectSql = "";

			foreach (var a in configuracao.Agrupamentos.OrderBy(x => x.Prioridade))
			{
				a.Alias = a.Campo.Nome;
				selectSql += string.Format(@"{0}.{1} ""{2}"", ", aliasesTabelas[a.Campo.Tabela], a.Campo.Nome, a.CampoId);
			}

			foreach (var a in campos.OrderBy(x => x.Posicao))
			{
				selectSql += string.Format(@"{0}.{1} ""{2}"", ", aliasesTabelas[a.Campo.Tabela], a.Campo.Nome, a.CampoId);
			}

			selectSql = selectSql.Substring(0, selectSql.Length - 2);

			#endregion

			#region SQL - FROM

			string tabelasSql = string.Join(", ", aliasesTabelas.Select(x => string.Format("{0} {1}", x.Key, x.Value)));

			#endregion

			#region SQL - WHERE (joins)

			string joinSql = "";
			string aliasFato = aliasesTabelas[configuracao.FonteDados.Tabela];

			foreach (var tab in aliasesTabelas)
			{
				if (configuracao.FonteDados.Tabela == tab.Key) continue;

				joinSql += string.Format("{0}.id = {1}.fato (+) and ", aliasesTabelas[configuracao.FonteDados.Tabela], tab.Value);
			}

			#endregion

			Comando comando = banco.CriarComando("");

			#region SQL - WHERE (filtros)

			string filtros = "";

			i = 0;
			foreach (var termo in configuracao.Termos.OrderBy(x => x.Ordem))
			{
				if ((eTipoTermo)termo.Tipo != eTipoTermo.Filtro)
				{
					filtros += TermoSimples(termo);
					continue;
				}

				filtros += TermoFiltro(termo, aliasesTabelas, comando, i);
				i++;
			}

			#endregion

			#region SQL - ORDERBY

			string orderSql = "";

			foreach (var campo in configuracao.Agrupamentos.OrderBy(c => c.Prioridade))
			{
				orderSql += string.Format("{0}.{1}, ", aliasesTabelas[campo.Campo.Tabela], campo.Campo.Nome);
			}

			foreach (var campo in configuracao.Ordenacoes.Where(y=>y.Campo.CampoOrdenacao).OrderBy(o => o.Prioridade))
			{
				orderSql += string.Format("{0}.{1} {2}, ", aliasesTabelas[campo.Campo.Tabela], campo.Campo.Nome, campo.Crescente ? "" : "desc");
			}

			if (!string.IsNullOrEmpty(orderSql))
			{
				orderSql = orderSql.Substring(0, orderSql.Length - 2);
			}

			#endregion

			#region SQL final

			bool possuiWhere = !string.IsNullOrEmpty(joinSql) || !string.IsNullOrEmpty(filtros);
			string sql = string.Format("select {0} from {1}", selectSql, tabelasSql);

			if (possuiWhere)
			{
				if (!string.IsNullOrEmpty(joinSql))
				{
					sql = string.Format("{0} where {1}", sql, joinSql);

					if (!string.IsNullOrEmpty(filtros))
					{
						sql = string.Format("{0} ({1})", sql, filtros);
					}
				}
				else if (!string.IsNullOrEmpty(filtros))
				{
					sql = string.Format("{0} where ({1})", sql, filtros);
				}
			}

			if (sql.Substring(sql.Length - 5, 4).Contains("and"))
			{
				sql = sql.Substring(0, sql.Length - 5);
			}

			if (!string.IsNullOrEmpty(orderSql))
			{
				sql = string.Format("{0} order by {1}", sql, orderSql);
			}

			comando.DbCommand.CommandText = string.Format(sql);

			#endregion

			return comando;
		}

		private string TermoFiltro(Termo termo, Dictionary<string, string> aliasesTabelas, Comando comando, int indice)
		{
			string coluna = string.Format("{0}.{1}", aliasesTabelas[termo.Campo.Tabela], string.IsNullOrEmpty(termo.Campo.CampoConsulta) ? termo.Campo.Nome : termo.Campo.CampoConsulta);
			string par = string.Format("p{0}", indice);
			string operando = "";

			if (termo.Campo.TipoDadosEnum == eTipoDados.Bitand)
			{
				switch ((eOperador)termo.Operador)
				{
					case eOperador.Diferente:
						operando = string.Format(" {2}.id not in (select b.id from {3} b where bitand({0}, :{1}) > 0) ", coluna, par, aliasesTabelas[termo.Campo.Tabela], termo.Campo.Tabela);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
					case eOperador.Igual:
						operando = string.Format(" bitand({0}, :{1}) > 0 ", coluna, par);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
				}
			}
			else
			{
				switch ((eOperador)termo.Operador)
				{
					case eOperador.Contem:
						contadorAux++;
						operando = string.Format(" contains({0}, (select :{1} from dual), " + contadorAux + ") > 0 ", coluna, par);
						comando.AdicionarParametroEntrada(par, DbType.String, 80, "%" + termo.Valor.ToString() + "%");
						break;
					case eOperador.NaoContem:
						contadorAux++;
						operando = string.Format(" contains({0}, (select :{1} from dual), " + contadorAux + ") = 0 ", coluna, par);
						comando.AdicionarParametroEntrada(par, DbType.String, 80, "%" + termo.Valor.ToString() + "%");
						break;
					case eOperador.Diferente:
						operando = string.Format(" {0} != :{1} ", coluna, par);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
					case eOperador.Igual:
						operando = string.Format(" {0} = :{1} ", coluna, par);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
					case eOperador.EhNulo:
						operando = string.Format(" {0} is null ", coluna);
						break;
					case eOperador.NaoEhNulo:
						operando = string.Format(" {0} is not null ", coluna);
						break;
					case eOperador.Maior:
						operando = string.Format(" {0} > :{1} ", coluna, par);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
					case eOperador.MaiorIgual:
						operando = string.Format(" {0} >= :{1} ", coluna, par);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
					case eOperador.Menor:
						operando = string.Format(" {0} < :{1} ", coluna, par);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
					case eOperador.MenorIgual:
						operando = string.Format(" {0} <= :{1} ", coluna, par);
						comando.AdicionarParametroEntrada(par, termo.Valor);
						break;
				}
			}

			return operando;
		}

		private string TermoSimples(Termo termo)
		{
			switch ((eTipoTermo)termo.Tipo)
			{
				case eTipoTermo.AbreParenteses:
					return "(";
				case eTipoTermo.FechaParenteses:
					return ")";
				case eTipoTermo.OperadorE:
					return " and ";
				case eTipoTermo.OperadorOu:
					return " or ";
				default:
					return string.Empty;
			}
		}

		#endregion

		#region Sumarios

		private void Sumarizar(ConfiguracaoRelatorio configuracao, DadosRelatorio dados)
		{
			if (configuracao.Sumarios.Count == 0 || dados.Colunas.Count == 0)
			{
				return;
			}

			List<int> campos = configuracao.CamposSelecionados.Where(x => configuracao.Sumarios.Exists(y => y.CampoId == x.CampoId)).Select(z => z.Posicao).ToList();
			Dictionary<int, string> colunas = new Dictionary<int, string>();

			dados.Sumarizacoes.Colunas.Where(x => campos.Exists(y => y == x.Key)).OrderBy(x => x.Key).ToList().ForEach(x =>
			{
				colunas.Add(x.Key, x.Value);
			});

			dados.Sumarizacoes.Colunas = colunas;

			//Ordenar as colunas do sumario
			configuracao.Sumarios = configuracao.Sumarios.OrderBy(x => configuracao.CamposSelecionados.Single(y => y.CampoId == x.CampoId).Posicao).ToList();

			PrepararSumarios(dados.Sumarizacoes, configuracao.Sumarios);

			foreach (var grupo in dados.Grupos)
			{
				PrepararSumarios(grupo.Sumarizacoes, configuracao.Sumarios);
				SumarizarGrupo(configuracao, grupo);
			}

			SumarizarTotal(configuracao, dados);
		}

		private void SumarizarTotal(ConfiguracaoRelatorio configuracao, DadosRelatorio dados)
		{
			foreach (var sumarioConfig in configuracao.Sumarios)
			{
				ConfiguracaoCampo campo = configuracao.CamposSelecionados.Single(x => x.CampoId == sumarioConfig.CampoId);
				var listaSumarios = sumarioConfig.ListarSumarios();

				foreach (var sumarioLinha in dados.Sumarizacoes.Linhas.OrderBy(x => x))
				{
					var itens = dados.Dados.Itens(campo.Posicao).Concat(dados.Grupos.SelectMany(g => g.Dados.Itens(campo.Posicao)));
					bool possuiItens = itens != null && itens.Count() > 0;

					if (itens == null || itens.Count() == 0)
					{
						dados.Sumarizacoes[sumarioLinha, campo.Posicao] = 0;
						continue;
					}

					if (listaSumarios.Keys.Any(y => y == (eTipoSumario)sumarioLinha))
					{
						switch ((eTipoSumario)sumarioLinha)
						{
							case eTipoSumario.Contar:
								dados.Sumarizacoes[sumarioLinha, campo.Posicao] = itens.Distinct().Count();
								break;

							case eTipoSumario.Somar:
								dados.Sumarizacoes[sumarioLinha, campo.Posicao] = possuiItens ? itens.Sum(x => Convert.ToDouble(((x != null && x.ToString() != string.Empty) ? x : 0))) : 0;
								break;

							case eTipoSumario.Media:
								dados.Sumarizacoes[sumarioLinha, campo.Posicao] = (possuiItens ? itens.Average(x => Convert.ToDouble(((x != null && x.ToString() != string.Empty) ? x : 0))) : 0).ToString("N2").Replace(".", string.Empty);
								break;

							case eTipoSumario.Maximo:
								dados.Sumarizacoes[sumarioLinha, campo.Posicao] = possuiItens ? itens.Max(x => Convert.ToDouble(((x != null && x.ToString() != string.Empty) ? x : 0))) : 0;
								break;

							case eTipoSumario.Minimo:
								dados.Sumarizacoes[sumarioLinha, campo.Posicao] = possuiItens ? itens.Min(x => Convert.ToDouble(((x != null && x.ToString() != string.Empty) ? x : 0))) : 0;
								break;
						}
					}
					else
					{
						dados.Sumarizacoes[sumarioLinha, campo.Posicao] = null;
					}
				}
			}
		}

		private void SumarizarGrupo(ConfiguracaoRelatorio configuracao, GrupoDados grupo)
		{
			ColecaoDados sumario = new ColecaoDados();
			foreach (var sumarioConfig in configuracao.Sumarios)
			{
				ConfiguracaoCampo campo = configuracao.CamposSelecionados.Single(x => x.CampoId == sumarioConfig.CampoId);

				sumario.Colunas.Add(campo.Posicao, campo.Alias);
				var listaSumarios = sumarioConfig.ListarSumarios();

				foreach (var sumarioLinha in grupo.Sumarizacoes.Linhas)
				{
					if (listaSumarios.Keys.Any(y => y == (eTipoSumario)sumarioLinha))
					{
						switch ((eTipoSumario)sumarioLinha)
						{
							case eTipoSumario.Contar:
								sumario[sumarioLinha, campo.Posicao] = grupo.Dados.ItensDistintos(campo.Posicao);
								break;

							case eTipoSumario.Somar:
								sumario[sumarioLinha, campo.Posicao] = grupo.Dados.Somar(campo.Posicao);
								break;

							case eTipoSumario.Media:
								sumario[sumarioLinha, campo.Posicao] = grupo.Dados.Media(campo.Posicao);
								break;

							case eTipoSumario.Maximo:
								sumario[sumarioLinha, campo.Posicao] = grupo.Dados.Maximo(campo.Posicao);
								break;

							case eTipoSumario.Minimo:
								sumario[sumarioLinha, campo.Posicao] = grupo.Dados.Minimo(campo.Posicao);
								break;
						}
					}
					else
					{
						sumario[sumarioLinha, campo.Posicao] = null;
					}
				}
				grupo.Sumarizacoes = sumario;
			}
		}

		private void PrepararSumarios(ColecaoDados dados, List<Sumario> sumarios)
		{
			foreach (var item in sumarios)
			{
				var listaSumarios = item.ListarSumarios();

				foreach (var sum in listaSumarios)
				{
					if (!dados.Linhas.Contains((int)sum.Key))
					{
						dados.Linhas.Add((int)sum.Key);
					}
				}
			}
		}

		private void PrepararSumarios(DadosRelatorio dados, ConfiguracaoRelatorio configuracao)
		{
			foreach (var item in configuracao.Sumarios)
			{
				var sumarios = item.ListarSumarios();

				foreach (var sum in sumarios)
				{
					if (!dados.Sumarizacoes.Linhas.Contains((int)sum.Key))
					{
						dados.Sumarizacoes.Linhas.Add((int)sum.Key);
					}
				}
			}
		}

		#endregion
	}
}