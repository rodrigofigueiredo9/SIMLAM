using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Data
{
	class FatoDa
	{
		private string EsquemaBanco { get; set; }

		public FatoDa(string strBancoDeDados)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public Fato Obter(int id, bool simplificado = false)
		{
			Fato retorno = new Fato();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				#region Fato

				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.nome, f.tabela, f.tid from {0}tab_fato f where f.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.Id = reader.GetValue<int>("id");
						retorno.Nome = reader.GetValue<string>("nome");
						retorno.Tabela = reader.GetValue<string>("tabela");
						retorno.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

                

				#endregion

				if (simplificado || retorno.Id == 0)
				{
					return retorno;
				}

				#region Dimensoes

				comando = bancoDeDados.CriarComando(@"select d.id, d.fato, d.nome, d.tabela from {0}tab_dimensao d where d.fato = :id order by d.nome", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Dimensao dimensao = new Dimensao();
						dimensao.Id = reader.GetValue<int>("id");
						dimensao.Nome = reader.GetValue<string>("nome");
						dimensao.Tabela = reader.GetValue<string>("tabela");

						retorno.Dimensoes.Add(dimensao);
					}

					reader.Close();
				}

                

				#endregion

				#region Campos

				comando = bancoDeDados.CriarComando(@"select c.id, c.codigo, c.fato, c.dimensao, c.campo, c.alias, c.tipo_dado, c.campo_exibicao, c.campo_filtro, 
				c.campo_ordenacao, c.consulta, c.campo_consulta, c.sistema_consulta from {0}tab_campo c where c.fato = :id order by c.alias", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Campo campo = new Campo();
						campo.Id = reader.GetValue<int>("id");
						campo.Codigo = reader.GetValue<int>("codigo");
						campo.Nome = reader.GetValue<string>("campo");
						campo.Alias = reader.GetValue<string>("alias");
						campo.TipoDados = reader.GetValue<int>("tipo_dado");
						campo.CampoExibicao = reader.GetValue<bool>("campo_exibicao");
						campo.CampoFiltro = reader.GetValue<bool>("campo_filtro");
						campo.CampoOrdenacao = reader.GetValue<bool>("campo_ordenacao");
						campo.Consulta = reader.GetValue<string>("consulta");
						campo.CampoConsulta = reader.GetValue<string>("campo_consulta");
						campo.SistemaConsulta = reader.GetValue<int>("sistema_consulta");
						campo.DimensaoNome = retorno.Nome;
						campo.Tabela = retorno.Tabela;

						int dimensaoId = reader.GetValue<int>("dimensao");
						if (dimensaoId > 0)
						{
							var dimensao = retorno.Dimensoes.Single(d => d.Id == dimensaoId);
							dimensao.Campos.Add(campo);
							campo.DimensaoNome = dimensao.Nome;
							campo.Tabela = dimensao.Tabela;
						}

						retorno.Campos.Add(campo);
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

		public Fato ObterCamposDinamicos(Fato fato)
		{
			Fato retorno = fato;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				if (retorno.Id == 0)
				{
					return retorno;
				}

				#region Campos

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.codigo, c.fato, c.dimensao, c.campo, c.alias, c.tipo_dado, c.campo_exibicao, c.campo_filtro, 
				c.campo_ordenacao, c.consulta, c.campo_consulta, c.sistema_consulta from {0}tab_campo c where c.fato = :id and c.codigo is null order by c.alias", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", retorno.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Campo campo = new Campo();
						campo.Id = reader.GetValue<int>("id");
						campo.Codigo = reader.GetValue<int>("codigo");
						campo.Nome = reader.GetValue<string>("campo");
						campo.Alias = reader.GetValue<string>("alias");
						campo.TipoDados = reader.GetValue<int>("tipo_dado");
						campo.CampoExibicao = reader.GetValue<bool>("campo_exibicao");
						campo.CampoFiltro = reader.GetValue<bool>("campo_filtro");
						campo.CampoOrdenacao = reader.GetValue<bool>("campo_ordenacao");
						campo.Consulta = reader.GetValue<string>("consulta");
						campo.CampoConsulta = reader.GetValue<string>("campo_consulta");
						campo.SistemaConsulta = reader.GetValue<int>("sistema_consulta");
						campo.DimensaoNome = retorno.Nome;
						campo.Tabela = retorno.Tabela;

						int dimensaoId = reader.GetValue<int>("dimensao");
						if (dimensaoId > 0)
						{
							var dimensao = retorno.Dimensoes.Single(d => d.Id == dimensaoId);
							dimensao.Campos.Add(campo);
							campo.DimensaoNome = dimensao.Nome;
							campo.Tabela = dimensao.Tabela;
						}

						retorno.Campos.Add(campo);
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

		public void ObterCamposListas(Campo campo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(campo.Consulta, banco.Conexao);
				campo.Lista = new List<Lista>();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Lista item = new Lista();
						item.Id = reader.GetValue<string>("id");
						item.Texto = reader.GetValue<string>("texto");

						if (reader.ContainsColumn("codigo"))
						{
							item.Codigo = reader.GetValue<string>("codigo");
						}

						campo.Lista.Add(item);
					}

					campo.Lista.RemoveAll(x => string.IsNullOrEmpty(x.Id));
					reader.Close();
				}
			}
		}

		internal List<Lista> ObterFonteDados()
		{
			List<Lista> retorno = new List<Lista>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.nome from {0}tab_fato t order by t.nome", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista objeto = null;

					while (reader.Read())
					{
						objeto = new Lista();
						objeto.Id = reader["id"].ToString();
						objeto.Texto = reader["nome"].ToString();
						objeto.IsAtivo = true;
						retorno.Add(objeto);
					}

					reader.Close();
				}
			}

			return retorno;
		}
	}
}