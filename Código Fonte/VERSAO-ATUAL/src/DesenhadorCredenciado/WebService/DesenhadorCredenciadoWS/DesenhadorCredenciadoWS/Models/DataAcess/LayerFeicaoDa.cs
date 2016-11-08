using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections;
using Tecnomapas.Blocos.Data;
using System.Data;
using Tecnomapas.DesenhadorWS.Models.Entities;

namespace Tecnomapas.DesenhadorWS.Models.DataAcess
{
    public class LayerFeicaoDa
    {
        #region Lista categorias de layers feição
        public List<CategoriaLayerFeicao> ListarCategoria(int idNavegador, int idProjeto)
        {
            Comando comando = null;
            IDataReader reader = null;
            IDbConnection connection = null;
            List<CategoriaLayerFeicao> lista = new List<CategoriaLayerFeicao>();
            try
            {
                string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
					 BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
                CategoriaLayerFeicao categoria = null;
                LayerFeicao layerFeicao = null;
                IDataReader readerQtde = null;
                int? idCategoria = null;
					 comando = bancoDeDadosGeo.GetComandoSql(@"select id_categoria, nome_categoria, id, descricao, feicao, tabela, esquema, sequencia, tipo, coluna_pk, is_principal, servico, url, id_layer, nome_layer, is_visivel, is_editavel,
                nvl(is_finalizada,0) is_finalizada, ordem_categoria, ordem_feicao from (
                select c.id id_categoria, c.nome nome_categoria, f.id, f.nome feicao, f.descricao, f.tabela, f.esquema, f.sequencia, f.tipo, f.coluna_pk, n.is_principal, serv.id servico, serv.url,
                fei.id_layer, fei.nome_layer, fei.is_visivel,  fei.is_editavel, fei.is_finalizada, c.ordem ordem_Categoria, fei.ordem ordem_feicao
                from tab_categoria_feicao c, tab_feicao f, tab_navegador_servico n, tab_servico serv,
                tab_servico_feicao fei, tab_navegador nav
                where c.id = f.categoria and n.servico = fei.servico and fei.feicao = f.id and n.is_principal = 1 and serv.id = n.servico 
                and n.navegador = nav.id and nav.id = :navegador   
                union
                select  c.id id_categoria, c.nome nome_categoria, f.id, f.nome feicao, f.descricao, f.tabela, f.esquema, f.sequencia, f.tipo, f.coluna_pk, 0 is_principal, serv.id servico, serv.url,
                fei.id_layer, fei.nome_layer, fei.is_visivel,  fei.is_editavel, fei.is_finalizada, c.ordem ordem_Categoria, fei.ordem ordem_feicao
                from tab_categoria_feicao c, tab_feicao f, tab_navegador_camada nav_cam, tab_servico serv,
                tab_servico_feicao fei
                where c.id = f.categoria and nav_cam.servico = fei.servico and fei.feicao = f.id and serv.id = nav_cam.servico 
                 and nav_cam.navegador = :navegador ) 
                order by   ordem_Categoria, ordem_feicao ");

                comando.AdicionarParametroEntrada("navegador", idNavegador, DbType.Int32);

					 reader = bancoDeDadosGeo.ExecutarReader(comando);
                if (reader == null) return null;
                while (reader.Read())
                {

                    if (reader["id_categoria"] is DBNull)
                    {
                        idCategoria = null;
                        if (categoria == null)
                        {
                            categoria = new CategoriaLayerFeicao();
                            categoria.Id = -1;
                            categoria.Nome = string.Empty;
                        }
                    }
                    if (!(reader["id_categoria"] is DBNull) && idCategoria != Convert.ToInt32(reader["id_categoria"]))
                    {
                        idCategoria = Convert.ToInt32(reader["id_categoria"]);
                        if (categoria != null)
                            lista.Add(categoria);
                        categoria = new CategoriaLayerFeicao();
                        categoria.Id = Convert.ToInt32(reader["id_categoria"]);
                        categoria.Nome = Convert.ToString(reader["nome_categoria"]);
                    }
                    if (!categoria.LayersFeicoes.Exists(delegate(LayerFeicao f) { return f.Id == Convert.ToInt32(reader["id"]); }))
                    {
                        layerFeicao = new LayerFeicao();
                        layerFeicao.Categoria = categoria.Id;
                        layerFeicao.Id = Convert.ToInt32(reader["id"]);
                        layerFeicao.Nome = Convert.ToString(reader["feicao"]);
						layerFeicao.Descricao = Convert.ToString(reader["descricao"]);
                        layerFeicao.Tabela = Convert.ToString(reader["tabela"]);
                        layerFeicao.Schema = Convert.ToString(reader["esquema"]);
                        if (reader["tipo"] is DBNull)
                        {
                            layerFeicao.TipoGeometria = TipoGeometriaFeicao.NaoDefinido;
                        }
                        else
                        {
                            layerFeicao.TipoGeometria = (TipoGeometriaFeicao)Convert.ToInt32(reader["tipo"]);
                        }
                        layerFeicao.Sequencia = Convert.ToString(reader["sequencia"]);
                        layerFeicao.ServicoId = Convert.ToInt32(reader["servico"]);
                        layerFeicao.ServicoUrlMxd = Convert.ToString(reader["url"]);
                        layerFeicao.ServicoIsPrincipal = Convert.ToString(reader["is_principal"]) != "0";
                        layerFeicao.Selecionavel = Convert.ToString(reader["is_editavel"]) != "0"; ;

                        if (reader["id_layer"] is DBNull)
                            layerFeicao.IdLayer = -1;
                        else
                            layerFeicao.IdLayer = Convert.ToInt32(reader["id_layer"]);

                        layerFeicao.NomeLayer = Convert.ToString(reader["nome_layer"]);

                        layerFeicao.Visivel = Convert.ToString(reader["is_visivel"]) == "1";

                        layerFeicao.IsFinalizada = Convert.ToString(reader["is_finalizada"]) == "1";

                        layerFeicao.ColunaPK = Convert.ToString(reader["coluna_pk"]);

                        if (!string.IsNullOrEmpty(layerFeicao.Tabela.Trim()))
                        {
									comando = bancoDeDadosGeo.GetComandoSql(@"select count(*) quantidade from " + layerFeicao.Tabela + " where projeto = :projeto");
                            comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

							try
							{
								readerQtde = bancoDeDadosGeo.ExecutarReader(comando);
								if (readerQtde.Read())
								{
									layerFeicao.Quantidade = Convert.ToInt32(readerQtde["quantidade"]);
								}
							}
							finally
							{
								if (readerQtde != null)
								{
									readerQtde.Close();
									readerQtde.Dispose();
									readerQtde = null;
								}
								if (comando != null)
								{
									comando.Dispose();
									comando = null;
								}
							}
                        }

                        categoria.LayersFeicoes.Add(layerFeicao);
                    }
                }

                if (categoria != null)
                    lista.Add(categoria);
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                }
				if (comando != null)
				{
					comando.Dispose();
				}

				comando = bancoDeDadosGeo.GetComandoSql(@"select f.feicao id_feicao, f.coluna, f.tipo, f.tamanho, f.tabela_referenciada, f.coluna_referenciada, f.alias, 
                f.is_obrigatorio, f.is_visivel, f.is_editavel, tfco.operacao, tfco.valor, tfco.coluna_obrigada from " + schemaUsuario + @".tab_feicao_colunas f, " 
                + schemaUsuario + @".tab_feicao_col_obrigator tfco where tfco.feicao(+) = f.feicao and tfco.coluna(+) = f.coluna and f.feicao = :id ");

                comando.AdicionarParametroEntrada("id", DbType.Int32);
                ColunaLayerFeicao coluna = null;
                foreach (CategoriaLayerFeicao c in lista)
                {
                    foreach (LayerFeicao f in c.LayersFeicoes)
                    {
                        comando.SetarValorDoParametro("id", f.Id);
								reader = bancoDeDadosGeo.ExecutarReader(comando);
                        if (reader == null) continue;
                        List<ColunaLayerFeicao> colunas = new List<ColunaLayerFeicao>();
                        while (reader.Read())
                        {
                            coluna = new ColunaLayerFeicao();
                            coluna.Coluna = Convert.ToString(reader["coluna"]);
                            coluna.Alias = Convert.ToString(reader["alias"]);
                            if (reader["tipo"] != null && !(reader["tipo"] is DBNull))
                                coluna.Tipo = Convert.ToInt32(reader["tipo"]);
                            else
                                coluna.Tipo = 0;
                            if (reader["tamanho"] != null && !(reader["tamanho"] is DBNull))
                                coluna.Tamanho = Convert.ToDouble(reader["tamanho"]);
                            else
                                coluna.Tamanho = 0;
                            coluna.Tabela_Referencia = Convert.ToString(reader["tabela_referenciada"]);
                            coluna.Coluna_Referencia = Convert.ToString(reader["coluna_referenciada"]);
                            coluna.IsObrigatorio = Convert.ToString(reader["is_obrigatorio"]) == "1";
                            coluna.IsVisivel = Convert.ToString(reader["is_visivel"]) == "1";
                            coluna.IsEditavel = Convert.ToString(reader["is_editavel"]) == "1";
                            if (reader["operacao"] is DBNull)
                            {
                                coluna.Operacao = TipoOperacao.NaoDefinido;
                            }
                            else
                            {
                                coluna.Operacao = (TipoOperacao)Convert.ToInt32(reader["operacao"]);
                            }
                            coluna.ValorCondicao = Convert.ToString(reader["valor"]);
                            coluna.ColunaObrigada = Convert.ToString(reader["coluna_obrigada"]);
                            colunas.Add(coluna);
                        }
                        f.Colunas = colunas;
                       
						reader.Close();
                        reader.Dispose();
                    }
                }
            }
            finally
            {                
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
				if (comando != null)
				{
					comando.Dispose();
				}
            }
            return lista;
        }
        #endregion

        #region Busca layer feição
        public LayerFeicao Buscar(Hashtable filtros)
        {
            LayerFeicao layerFeicao = null;
            BancoDeDados banco = null;

            string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
            banco = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
            Comando comando = banco.GetComandoSql(@"select t.id, t.nome, t.categoria, t.esquema schema, t.tabela,  t.sequencia,  t.tipo, t.coluna_pk from tab_feicao t where rownum > 0 ");

            if (filtros != null && filtros.Count > 0)
            {
                if (filtros.ContainsKey("feicao"))
                {
                    comando.DBCommand.CommandText += " and upper(t.nome) = upper(:feicao) ";
                    comando.AdicionarParametroEntrada("feicao", DbType.String, 4000, filtros["feicao"]);
                }
                if (filtros.ContainsKey("id"))
                {
                    comando.DBCommand.CommandText += " and id = :id";
                    comando.AdicionarParametroEntrada("id", DbType.Int32, 10, filtros["id"]);
                }
                if (filtros.ContainsKey("tabela"))
                {
                    comando.DBCommand.CommandText += " and upper(t.tabela) = upper(:tabela) ";
                    comando.AdicionarParametroEntrada("tabela", DbType.String, 4000, filtros["tabela"]);
                }

            }

			IDataReader reader = null;
			try
			{
				reader = banco.ExecutarReader(comando);
				if (reader == null) return null;
				if (reader.Read())
				{
					layerFeicao = new LayerFeicao();
					layerFeicao.Nome = Convert.ToString(reader["nome"]);
					layerFeicao.Id = Convert.ToInt32(reader["id"]);
					layerFeicao.Tabela = Convert.ToString(reader["tabela"]);
					layerFeicao.Schema = Convert.ToString(reader["schema"]);
					layerFeicao.Sequencia = Convert.ToString(reader["sequencia"]);
					layerFeicao.TipoGeometria = (TipoGeometriaFeicao)Convert.ToInt32(reader["tipo"]);
					layerFeicao.ColunaPK = Convert.ToString(reader["coluna_pk"]);
				}
			}
			finally
			{
				if (reader != null)
				{
					reader.Close();
					reader.Dispose();
                    reader = null;
				}
				if (comando != null)
				{
					comando.Dispose();
				}
			}

            if (layerFeicao == null) return null;

            if (filtros == null || !filtros.ContainsKey("SEM_COLUNAS"))
            {

                comando = banco.GetComandoSql(@"select f.feicao, f.coluna, f.tipo, f.tamanho, f.tabela_referenciada, f.coluna_referenciada, f.alias, 
                f.is_obrigatorio, f.is_visivel, f.is_editavel from " + schemaUsuario + @".tab_feicao_colunas f where f.is_visivel = 1 and f.feicao = :id ");

                comando.AdicionarParametroEntrada("id", DbType.Int32, 10, layerFeicao.Id);

				try
				{


					reader = banco.ExecutarReader(comando);

					ColunaLayerFeicao coluna = null;
					while (reader.Read())
					{
						coluna = new ColunaLayerFeicao();
						coluna.Coluna = Convert.ToString(reader["coluna"]);
						coluna.Alias = Convert.ToString(reader["alias"]);
						if (reader["tipo"] != null && !(reader["tipo"] is DBNull))
							coluna.Tipo = Convert.ToInt32(reader["tipo"]);
						else
							coluna.Tipo = 0;
						if (reader["tamanho"] != null && !(reader["tamanho"] is DBNull))
							coluna.Tamanho = Convert.ToDouble(reader["tamanho"]);
						else
							coluna.Tamanho = 0;
						coluna.Tabela_Referencia = Convert.ToString(reader["tabela_referenciada"]);
						coluna.Coluna_Referencia = Convert.ToString(reader["coluna_referenciada"]);
						coluna.IsObrigatorio = Convert.ToString(reader["is_obrigatorio"]) == "1";
						coluna.IsVisivel = Convert.ToString(reader["is_visivel"]) == "1";
						coluna.IsEditavel = Convert.ToString(reader["is_editavel"]) == "1";
						layerFeicao.Colunas.Add(coluna);
					}
				}
				finally
				{
					if (reader != null)
					{
						reader.Close();
						reader.Dispose();
                        reader = null;
					}
					if (comando != null)
					{
						comando.Dispose();
					}
				}
            }
            return layerFeicao;
        }
        #endregion

        #region Lista quantidade feição
        public List<LayerFeicaoQuantidade> ListarQuantidadeFeicoes(int idNavegador, int idProjeto)
        {
            Comando comando = null;
            List<LayerFeicaoQuantidade> lista = new List<LayerFeicaoQuantidade>();
            IDataReader reader = null;
            IDataReader readerQtd = null;
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

                string projetos_associados = string.Empty;

                if (idNavegador ==2)
                {
					#region Atividade
						 comando = bancoDeDadosGeo.GetComandoSql(@"select id projeto_associado from crt_projeto_geo a where a.empreendimento = (select empreendimento from tmp_projeto_geo pg where pg.id = :projeto union select empreendimento from crt_projeto_geo pg where pg.id = :projeto )");
					comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

					try
					{
						reader = bancoDeDadosGeo.ExecutarReader(comando);

						while (reader.Read())
						{
							if (!string.IsNullOrEmpty(Convert.ToString(reader["projeto_associado"])))
							{
								projetos_associados += " or PROJETO = " + Convert.ToString(reader["projeto_associado"]);
							}
						}
					}
					finally
					{
						if (reader != null)
						{
							reader.Close();
							reader.Dispose();
							reader = null;
						}
						if (comando != null)
						{
							comando.Dispose();
							comando = null;
						}
					} 
					#endregion
                }

				/*if (idNavegador == 4)
				{
					#region CAR
					comando = bancoDeDadosGeo.GetComandoSql(@"select id projeto_associado from crt_projeto_geo a where a.empreendimento = (select empreendimento from tmp_projeto_geo pg where pg.id = :projeto union select empreendimento from crt_cad_ambiental_rural pg where pg.projeto_geo_id = :projeto )");
					comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

					try
					{
						reader = bancoDeDadosGeo.ExecutarReader(comando);

						while (reader.Read())
						{
							if (!string.IsNullOrEmpty(Convert.ToString(reader["projeto_associado"])))
							{
								projetos_associados += " or PROJETO = " + Convert.ToString(reader["projeto_associado"]);
							}
						}
					}
					finally
					{
						if (reader != null)
						{
							reader.Close();
							reader.Dispose();
							reader = null;
						}
						if (comando != null)
						{
							comando.Dispose();
							comando = null;
						}
					}
					#endregion
				}*/

                comando = bancoDeDadosGeo.GetComandoSql(@"select sqlquery  from (
                select ' select count('||f.coluna_pk||') quantidade, '||f.id ||'  layer, '||f.categoria||' categoria from '||
                f.tabela ||' where '||(case when sf.is_editavel = 1 then ' PROJETO = " + idProjeto.ToString() + "' else ' ( PROJETO = " + idProjeto.ToString() + projetos_associados + @")' end)
                ||(case when sf.filtro is not null then ' and '||sf.filtro else ''end) ||'  union' sqlQuery, f.categoria, f.id from tab_servico s, tab_servico_feicao sf, 
                tab_feicao f, tab_navegador_servico ns where ns.servico = s.id and sf.feicao = f.id and sf.servico = s.id and ns.is_principal = 1
                and  ns.navegador = :navegador
                union 
                select ' select count('||f.coluna_pk||') quantidade, '||f.id ||'  layer, '||f.categoria||' categoria from '||
                  f.tabela 
                  ||(case when sf.filtro is not null then ' where '||sf.filtro else ''end) ||'  union' sqlQuery, f.categoria, f.id  from tab_servico s, tab_servico_feicao sf, 
                  tab_feicao f, tab_navegador_camada ns where ns.servico = s.id and sf.feicao = f.id and sf.servico = s.id
                  and  ns.navegador = :navegador and ns.servico = s.id 
                  ) order by categoria, id  ");
                
                comando.AdicionarParametroEntrada("navegador", idNavegador, DbType.Int32);
				string sqlQuery = string.Empty;
				LayerFeicaoQuantidade layer = null;

				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);

					if (reader == null) return null;

					while (reader.Read())
					{
						sqlQuery += Convert.ToString(reader["sqlQuery"]);
					}
				}
				finally
				{
					if (reader != null)
					{
						reader.Close();
						reader.Dispose();
						reader = null;
					}
					if (comando != null)
					{
						comando.Dispose();
						comando = null;
					}
				}

                if (sqlQuery.Length > 7)
                {
                    sqlQuery = sqlQuery.Substring(0, sqlQuery.Length - 6);

                    if (!string.IsNullOrEmpty(sqlQuery.Trim()))
                    {
                        sqlQuery += " order by categoria, layer";
                        comando = bancoDeDadosGeo.GetComandoSql(sqlQuery);

                        readerQtd = bancoDeDadosGeo.ExecutarReader(comando);

                        while (readerQtd.Read())
                        {
                            layer = new LayerFeicaoQuantidade();
                            layer.Categoria = Convert.ToInt32(readerQtd["categoria"]);
                            layer.LayerFeicao = Convert.ToInt32(readerQtd["layer"]);
                            layer.Quantidade = Convert.ToInt32(readerQtd["quantidade"]);

                            lista.Add(layer);
                        }
                    }
                }
            }
            finally
            {
                if (comando != null)
                {
                    comando.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (readerQtd != null)
                {
                    readerQtd.Close();
                    readerQtd.Dispose();
                }


            }
            return lista;
        }
        #endregion

        #region Busca lista de valores
        public List<Item> BuscarListaDeValores(string tabela_referencia, string coluna_referencia)
        {
            string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];

            BancoDeDados bancoGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
            IDataReader reader = null;
            List<Item> itens = new List<Item>();
            Comando comando = null;
            try
            {
	            comando =
		            bancoGeo.GetComandoSql(
			            @"select upper(t.table_name) tabela_esquema from user_tables t where upper(t.table_name) = :tabela 
								union
								select upper(s.table_owner) tabela_esquema from user_synonyms s where upper(s.table_name) = :tabela ");

                comando.AdicionarParametroEntrada("tabela", tabela_referencia, DbType.String);

				try
				{
					reader = bancoGeo.ExecutarReader(comando);

					if (reader == null) return null;

					if (!reader.Read()) return itens;

					if (reader["tabela_esquema"].ToString() != tabela_referencia.ToUpper())
					{
						schemaUsuarioGeo = reader["tabela_esquema"].ToString();
					}
				}
				finally
				{
					if (reader != null)
					{
						reader.Close();
						reader.Dispose();
						reader = null;
					}
					if (comando != null)
					{
						comando.Dispose();
						comando = null;
					}
				}

                comando = bancoGeo.GetComandoSql(@"select t.chave, t.texto  from " + schemaUsuarioGeo + "." + tabela_referencia + @" t  order by texto ");
                reader = bancoGeo.ExecutarReader(comando);

                if (reader == null) return null;

                Item item = null;
                item = new Item();
                item.Chave = "";
                item.Texto = "**Selecione**";
                itens.Add(item);

                while (reader.Read())
                {
                    item = new Item();
                    item.Chave = Convert.ToString(reader["chave"]);
                    item.Texto = Convert.ToString(reader["texto"]);
                    itens.Add(item);
                }
            }
            finally 
            {
                if (comando != null)
                {
                    comando.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
    
            return itens;
        }
        #endregion
    }
}


    