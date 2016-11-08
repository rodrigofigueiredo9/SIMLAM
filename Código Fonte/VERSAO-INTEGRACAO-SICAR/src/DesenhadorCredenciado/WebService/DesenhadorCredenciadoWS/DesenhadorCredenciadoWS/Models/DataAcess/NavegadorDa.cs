using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Tecnomapas.Blocos.Data;
using System.Data;
using Tecnomapas.DesenhadorWS.Models.Entities;

namespace Tecnomapas.DesenhadorWS.Models.DataAcess
{
    public class NavegadorDa
    {
        #region Buscar Navegador
        internal Navegador Buscar(int idNavegador, int idProjeto)
        {
            Comando comando = null;
            Comando comandoGeo = null;
            Navegador navegador = null;
            IDataReader reader = null;
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
                               
                comandoGeo = bancoDeDadosGeo.GetComandoSql(@"select a.nome from " + schemaUsuarioGeo + @".tab_navegador a where a.id = :id ");
                comandoGeo.AdicionarParametroEntrada("id", idNavegador, DbType.Int32);
                reader = bancoDeDadosGeo.ExecutarReader(comandoGeo);

                if (reader.Read())
                {
                    navegador = new Navegador();
                    navegador.Id = idNavegador;
                    navegador.Nome = Convert.ToString(reader["nome"]);

                }

                if (navegador != null)
                {
                    List<string> projetosAssociados;
                    navegador.Servicos = BuscarServicos(idNavegador);
                    navegador.Cenarios = BuscarCenarios(idNavegador);
                    navegador.Filtros = BuscarFiltrosLayerFeicao(idNavegador, idProjeto, out projetosAssociados);
                    if (projetosAssociados != null)
                        navegador.ProjetosAssociados = projetosAssociados.ToArray();
                }
            }
            finally
            {
                if (comando != null)
                {
                    comando.Dispose();
                }
                if (comandoGeo != null)
                {
                    comandoGeo.Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
            return navegador;
        }
        #endregion

        #region Serviços
        internal ServicoArcGis[] BuscarServicos(int idNavegador)
        {
            string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
				BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
            IDataReader reader = null;
            List<ServicoArcGis> lista = null;

				Comando comando = bancoDeDadosGeo.GetComandoSql(@"select b.id, b.nome, b.url, b.is_cacheado, a.is_principal, a.identificar, a.gera_legenda, nvl((select max(sf.id_layer)
            from " + schemaUsuarioGeo + @".tab_servico_feicao sf where sf.servico(+) = b.id),0) ultimo_id_layer from " + schemaUsuarioGeo + @".tab_navegador_servico a, 
            " + schemaUsuarioGeo + @".tab_servico b where a.servico = b.id and a.navegador = :navegador order by a.ordem_exibicao ");
            
            comando.AdicionarParametroEntrada("navegador", idNavegador, DbType.Int32);

			try
			{
				reader = bancoDeDadosGeo.ExecutarReader(comando);

				lista = new List<ServicoArcGis>();
				ServicoArcGis servico = null;
				while (reader.Read())
				{
					servico = new ServicoArcGis();

					servico.Id = Convert.ToInt32(reader["id"]);
					servico.Nome = Convert.ToString(reader["nome"]);
					servico.Url = Convert.ToString(reader["url"]);
					servico.IsCacheado = Convert.ToString(reader["is_cacheado"]) == "1";
					servico.IsPrincipal = Convert.ToString(reader["is_principal"]) == "1";
					servico.Identificar = Convert.ToString(reader["identificar"]) == "1";
                    servico.GeraLegenda = Convert.ToString(reader["gera_legenda"]) == "1";
					servico.UltimoIdLayer = Convert.ToInt32(reader["ultimo_id_layer"]);
					lista.Add(servico);
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
            return lista.ToArray();
        }
        #endregion

        #region Buscar filtros layers feição
        internal string[] BuscarFiltrosLayerFeicao(int idNavegador, int idProjeto, out List<string> projetosAssociados)
        {
             
            Comando comando = null;
            IDataReader reader = null;
            List<string> lista = new List<string>();
            projetosAssociados = new List<string>();
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
            
                 string projetos_associados = string.Empty;

                if (idNavegador == 2)
                {
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
								projetosAssociados.Add(Convert.ToString(reader["projeto_associado"]));
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
                }
					 /*else if (idNavegador == 4)
					 {
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
									 projetosAssociados.Add(Convert.ToString(reader["projeto_associado"]));
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
					 }*/

					 comando = bancoDeDadosGeo.GetComandoSql(@"select (case when a.filtro is not null then a.filtro||' and 'else ''end)||(case when a.is_editavel = 1 then ' PROJETO = " + idProjeto.ToString() +"' else ' ( PROJETO = " + idProjeto.ToString() + projetos_associados+ @")' end) filtro
                from tab_servico_feicao a, tab_navegador_servico ns, tab_feicao f where f.id = a.feicao and ns.navegador = :navegador and ns.servico = a.servico and ns.is_principal = 1 order by a.id_layer ");

                comando.AdicionarParametroEntrada("navegador", idNavegador, DbType.Int32);

                reader = bancoDeDadosGeo.ExecutarReader(comando);
                              
                while (reader.Read())
                {
                    string filtro = Convert.ToString(reader["filtro"]);
                    lista.Add(filtro);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (comando != null)
                {
                    comando.Dispose();
                }
 
            }
            return lista.ToArray();
        }
        #endregion

        #region Cenários
        internal CenarioServicoArcGis[] BuscarCenarios(int idNavegador)
        {
            IDataReader reader = null;
            List<CenarioServicoArcGis> lista = new List<CenarioServicoArcGis>();
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
					 BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");


					 Comando comando = bancoDeDadosGeo.GetComandoSql(@"select a.id, a.nome, a.is_ativo isprincipal from " + schemaUsuarioGeo + @".tab_cenario_navegador a where a.navegador = :navegador order by a.ordem_exibicao");

                comando.AdicionarParametroEntrada("navegador", idNavegador, DbType.Int32);

					 reader = bancoDeDadosGeo.ExecutarReader(comando);

                CenarioServicoArcGis cenario = new CenarioServicoArcGis();
                cenario.Id = 0;
                cenario.Nome = "Branco";
                cenario.IsPrincipal = false;
                cenario.ExibirLogotipo = false;
                lista.Add(cenario);

                while (reader.Read())
                {
                    cenario = new CenarioServicoArcGis();
                    cenario.Id = Convert.ToInt32(reader["id"]);
                    cenario.Nome = Convert.ToString(reader["nome"]);
                    cenario.IsPrincipal = Convert.ToString(reader["isprincipal"]) == "1";
                    cenario.ExibirLogotipo = true;
                    lista.Add(cenario);
                }
                
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

                List<string> camadas = new List<string>();

					 comando = bancoDeDadosGeo.GetComandoSql("select servico from " + schemaUsuarioGeo + @".tab_navegador_camada nc  where nc.navegador = :navegador");
                comando.AdicionarParametroEntrada("navegador", idNavegador, DbType.Int32);

					 reader = bancoDeDadosGeo.ExecutarReader(comando);

                while (reader.Read())
                {
                    camadas.Add(Convert.ToString(reader["servico"]));
                }

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

                if (lista != null)
                {
						 comando = bancoDeDadosGeo.GetComandoSql("select a.servico id from " + schemaUsuarioGeo + @".tab_cenario_servico a where a.cenario_navegador = :cenario");

                    comando.AdicionarParametroEntrada("cenario", DbType.Int32);
                    for (int n = 0; n < lista.Count; n++)
                    {
                        comando.SetarValorDoParametro("cenario", lista[n].Id);

								reader = bancoDeDadosGeo.ExecutarReader(comando);

                        List<string> listaServicos = new List<string>();
                        string servico;
                        while (reader.Read())
                        {
                            servico = Convert.ToString(reader["id"]);
                            listaServicos.Add(servico);
                        }

                        if (camadas != null)
                        {
                            foreach (string camada in camadas)
                            {
                                listaServicos.Add(camada);
                            }
                        }

                        lista[n].Servicos = listaServicos.ToArray();
                        if (reader != null)
                        {
                            reader.Close();
                            reader.Dispose();
                            reader = null;
                        }                 
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
            }
            return lista.ToArray();
        }
        #endregion
    }
}