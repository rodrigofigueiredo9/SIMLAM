using System;

using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.DesenhadorWS.Models.Entities;
using System.Configuration;
using Tecnomapas.Blocos.Data;
using System.Data;

namespace Tecnomapas.DesenhadorWS.Models.DataAcess
{
	public class ProjetoDa
	{
		#region Obter
		internal int ObterSridBase()
		{
			Comando comandoGeo = null;

			try
			{
				string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
				BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				comandoGeo = bancoDeDadosGeo.GetComandoSql(@"select c.valor from tab_configuracao c where c.chave = 'SRID_BASE_REAL'");
				return Convert.ToInt32(bancoDeDadosGeo.ExecutarScalar(comandoGeo).ToString());
			}
			finally
			{
				if (comandoGeo != null)
				{
					comandoGeo.Dispose();
				}
			}
		}

		internal bool VerficiarProjetoFinalizado(int idProjeto, bool isFiscalizcao=false)
		{
			Comando comando = null;

			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"].ToUpper();
				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexao");

				comando = bancoDeDados.GetComandoSql(@"select count(*) from tmp_projeto_geo c where c.id = :projeto");
				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				if (Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0)
				{
					return false;
				}

				if (!isFiscalizcao)
				{
					comando = bancoDeDados.GetComandoSql(@"select count(*) from crt_projeto_geo c where c.id = :projeto");
					comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

					if (Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0)
					{
						return true;
					}
				}
				else
				{
					comando = bancoDeDados.GetComandoSql(@"select count(*) from tab_fisc_prj_geo c where c.id = :projeto");
					comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

					if (Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0)
					{
						return true;
					}
				}

			}
			finally
			{
				if (comando != null)
				{
					comando.Dispose();
				}
			}

			return false;
		}

		internal bool VerficiarProjetoCARFinalizado(int idProjeto)
		{
			Comando comando = null;

			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"].ToUpper();
				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexao");

				comando = bancoDeDados.GetComandoSql(@"select count(*) from tmp_cad_ambiental_rural c where c.projeto_geo_id = :projeto");
				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				if (Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0)
				{
					return false;
				}

				comando = bancoDeDados.GetComandoSql(@"select count(*) from crt_cad_ambiental_rural c where c.projeto_geo_id = :projeto");
				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				if (Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0)
				{
					return true;
				}
			}
			finally
			{
				if (comando != null)
				{
					comando.Dispose();
				}
			}

			return false;
		}

		internal List<ItemQuadroDeArea> ObterFeicaoDescricao()
		{
			List<ItemQuadroDeArea> lstDescricao = new List<ItemQuadroDeArea>();

			BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

			Comando comando = bancoDeDadosGeo.GetComandoSql(@"select distinct t.nome, t.descricao from TAB_FEICAO t");

			using (IDataReader reader = bancoDeDadosGeo.ExecutarReader(comando))
			{
				while (reader.Read())
				{
					lstDescricao.Add(new ItemQuadroDeArea()
					{
						Nome = reader["nome"].ToString(),
						Descricao = reader["descricao"].ToString()
					});
				}
				reader.Close();
			}

			return lstDescricao;
		}
		#endregion

		#region Buscar Dados Projeto
        internal Projeto BuscarDadosProjeto(int idProjeto, int idFilaTipo)
        {
            Comando comando = null;
            Comando comandoGeo = null;

            IDataReader reader = null;
            Projeto projeto = new Projeto();
            projeto.TipoNavegador = 1;
            projeto.Id = idProjeto;
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

                string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"].ToUpper();
                BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexao");

                if (idFilaTipo != 5 && idFilaTipo != 7 && idFilaTipo != 9)
                {
					comando = bancoDeDados.GetComandoSql(@"select (case when a.caracterizacao =1 then 1 else 2 end ) navegador, a.empreendimento from crt_projeto_geo a where a.id = :projeto
					union all
					select (case when a.caracterizacao =1 then 1 else 2 end ) navegador, a.empreendimento from tmp_projeto_geo a where a.id = :projeto ");
                    comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
                    reader = bancoDeDados.ExecutarReader(comando);
                }
                else
                {
                    comando = bancoDeDadosGeo.GetComandoSql(@"select id navegador, 0 empreendimento from tab_navegador a where a.fila_tipo = :filatipo ");
                    comando.AdicionarParametroEntrada("filatipo", idFilaTipo, DbType.Int32);
                    reader = bancoDeDadosGeo.ExecutarReader(comando);
                }
				try
				{
					if (reader.Read())
					{
						projeto.TipoNavegador = Convert.ToInt32(reader["navegador"]);
						projeto.Empreendimento = Convert.ToInt32(reader["empreendimento"]);
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

                #region Atualiza Ponto do empreendimento
                if (projeto.TipoNavegador == 1 && projeto.Empreendimento > 0)
                {
                    comandoGeo = bancoDeDadosGeo.GetComandoSql(@"update GEO_EMP_LOCALIZACAO set projeto = :projeto where empreendimento = :empreendimento ");
                    comandoGeo.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
                    comandoGeo.AdicionarParametroEntrada("empreendimento", projeto.Empreendimento, DbType.Int32);
                    bancoDeDadosGeo.ExecutarNonQuery(comandoGeo);
                }
                #endregion

                #region Insere o projeto a tabela de navegador

                comandoGeo = bancoDeDadosGeo.GetComandoSql(@"select count(*) existe from tab_navegador_projeto a where a.projeto = :projeto");
                comandoGeo.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comandoGeo);

					if (reader.Read())
					{
						if (Convert.ToInt32(reader["existe"]) == 0)
						{
							comandoGeo = bancoDeDadosGeo.GetComandoSql(@"insert into tab_navegador_projeto (projeto, is_valido_proces) values (:projeto, 0)");
							comandoGeo.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
							bancoDeDadosGeo.ExecutarNonQuery(comandoGeo);
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
					if (comandoGeo != null)
					{
						comandoGeo.Dispose();
						comandoGeo = null;
					}
				}

                #endregion
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
            return projeto;
        }
        #endregion

        #region Quadro de Áreas - Dominialidade
        public List<CategoriaQuadroDeArea> ListarQuadroAreasDominialidade(int idProjeto)
        {
            Comando comando = null;
            List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
            IDataReader reader = null;
            IDataReader readerQtd = null;
            try
            {
                string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
                BancoDeDados bancoGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
                string situacaoTexto = "Não processada";
               
                #region Busca situação do projeto
                comando = bancoGeo.GetComandoSql(string.Format(@"select (case when is_valido_proces != 1 or is_valido_proces is null then 'Inválido' else 'Válido' end) situacao_texto, is_valido_proces  
                  from {0}.tab_navegador_projeto where projeto = :projeto", schemaUsuarioGeo));
                /*comando = bancoGeo.GetComandoSql(string.Format(@"select  lf.id situacao, (case when  (lf.id = 9 or lf.id = 14) and np.is_valido_proces != 1 or np.is_valido_proces is null then 'Inválido'
                else lf.texto end) situacao_texto, np.is_valido_proces  from {0}.lov_crt_projeto_geo_sit_proce lf,  {1}.tab_fila f, {1}.tab_navegador_projeto np
                 where f.projeto = :projeto and np.projeto(+) = f.projeto and lf.situacao  = f.situacao and lf.etapa = f.etapa and f.mecanismo_elaboracao = 2 and f.tipo = 3 ", schemaUsuario, schemaUsuarioGeo));*/


                

                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				try
				{
					reader = bancoGeo.ExecutarReader(comando);
					if (reader.Read())
					{
						situacaoTexto = Convert.ToString(reader["situacao_texto"]);
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

                #region Total de Quadro de Áreas

                comando = bancoGeo.GetComandoSql(@"select * from (
            select 'ATP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  1 ORDEM from des_atp where projeto=:projeto
            union all select 'Área Construída' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  2 ORDEM from des_aconstruida where projeto=:projeto
            union all select 'AFD' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2, 0 is_processada, 0 is_subarea,  3 ORDEM from des_afd where projeto=:projeto
            union all select 'APMP'  feicao,  trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  4 ORDEM from des_apmp where projeto=:projeto
           union all select 'Rocha' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  5 ORDEM from des_rocha where projeto=:projeto
            union all select 'AVN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  6 ORDEM from des_avn where projeto=:projeto
            union all select ' -Estágio inicial' feicao,trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  7 ORDEM from des_avn where estagio='I' and projeto=:projeto 
            union all select ' -Estágio médio' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  8 ORDEM from des_avn where estagio='M' and projeto=:projeto 
            union all select ' -Estágio avançado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  9 ORDEM from des_avn where estagio='A' and projeto=:projeto 
            union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  10 ORDEM from des_avn where estagio='D' and projeto=:projeto 
            union all select 'AA' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  11 ORDEM from des_aa where projeto=:projeto
            union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  12 ORDEM from des_aa where tipo='REC' and projeto=:projeto
            union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  13 ORDEM from des_aa where tipo='USO' and projeto=:projeto 
            union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  14 ORDEM from des_aa where tipo='D' and projeto=:projeto
            union all select 'ARL' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  15 ORDEM from des_arl where projeto=:projeto
           "+/* union all select ' -Preservada' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  16 ORDEM from des_arl where situacao='PRESERV' and projeto=:projeto
            union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  17 ORDEM from des_arl where situacao='REC' and projeto=:projeto
            union all select ' -Em uso', trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  18 ORDEM from des_arl where situacao='USO' and projeto=:projeto
            union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  19 ORDEM from des_arl where situacao='D' and projeto=:projeto
            */@" union all select 'AFS' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  20 ORDEM from DES_afs where projeto=:projeto
            union all select 'RPPN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  21 ORDEM from des_rppn where projeto=:projeto
            union all select 'APP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  22 ORDEM from tmp_areas_calculadas where tipo='APP_APMP' and projeto=:projeto
            union all select ' -Preservada' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  23 ORDEM from tmp_areas_calculadas where tipo='APP_AVN' and projeto=:projeto
            union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  24 ORDEM from tmp_areas_calculadas where tipo='APP_AA_REC' and projeto=:projeto
            union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  25 ORDEM from tmp_areas_calculadas where tipo='APP_AA_USO' and projeto=:projeto
            union all select ' -Não caracterizado' feicao, trunc(nvl(apmp.area_m2-avn.area_m2-rec.area_m2-uso.area_m2,0),4) area_m2,  1 is_processada, 1 is_subarea,  27 ORDEM from 
                    (select  nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas apmp where tipo='APP_APMP' and projeto=:projeto) apmp, 
                    (select nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas where tipo='APP_AVN' and projeto=:projeto) avn,
                    (select nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas where tipo='APP_AA_REC' and projeto=:projeto) rec,
                    (select nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas where tipo='APP_AA_USO' and projeto=:projeto) uso
            union all select ' -Em reserva legal' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  26 ORDEM from tmp_areas_calculadas where tipo='APP_ARL' and projeto=:projeto
            union all select 'Massa dagua' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  28 ORDEM from tmp_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto=:projeto
           ) order by ordem");

                /*comando = bancoGeo.GetComandoSql(@"select * from (
                  select 'ATP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea,  1 ORDEM from vw_des_atp a where a.PROJETO = :projeto
                  union all select 'Área construída' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 2 ORDEM  from des_aconstruida a where a.PROJETO = :projeto
                  union all select 'AFD' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 3 ORDEM  from des_afd a where a.PROJETO = :projeto
                  union all select 'APMP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 5 ORDEM  from des_apmp a where a.PROJETO = :projeto
                  union all select 'Rocha' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 6 ORDEM  from des_rocha a where a.PROJETO = :projeto
                  union all select 'AVN' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 7 ORDEM  from des_avn a where a.PROJETO = :projeto
                  union all select '-Estágio inicial' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 8 ORDEM  from des_avn a where a.PROJETO = :projeto and a.estagio = 'I'
                  union all select '-Estágio médio' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 9 ORDEM  from des_avn a where a.PROJETO = :projeto and a.estagio = 'M'
                  union all select '-Estágio avançado' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 10 ORDEM  from des_avn a where a.PROJETO = :projeto and a.estagio = 'A'
                  union all select '-Desconhecido' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 11 ORDEM  from vw_des_avn a where a.PROJETO = :projeto and a.estagio = 'D'
                  union all select 'AA' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 12 ORDEM  from  vw_des_aa a where a.PROJETO = :projeto
                  union all select '-Cultivada' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 13 ORDEM  from  vw_des_aa a where a.PROJETO = :projeto and a.TIPO = 'C'
                  union all select '-Não cultivada' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 14 ORDEM  from  vw_des_aa a where a.PROJETO = :projeto and a.TIPO = 'NC'
                  union all select '-Desconhecida' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 1 is_subarea, 15 ORDEM  from  vw_des_aa a where a.PROJETO = :projeto and a.TIPO = 'D'
                  union all select 'AFS' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 16 ORDEM  from des_afs a where a.PROJETO = :projeto
                  union all select 'RPPN' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 17 ORDEM  from des_rppn a where a.PROJETO = :projeto
                  union all select 'ARL' feicao, nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 0 is_subarea, 18 ORDEM from des_arl where projeto = :projeto
                  union all select '-Preservada' feicao, nvl( sum(area_m2), 0 ) area_m2, 0 is_processada, 1 is_subarea, 19 ORDEM from des_arl where situacao='AVN' and projeto = :projeto
                  union all select '-Em Formação' feicao,  nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 1 is_subarea, 20 ORDEM from des_arl where situacao='AA' and projeto = :projeto
                  union all select '-Indefinida' feicao, nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 1 is_subarea, 21 ORDEM from des_arl where situacao='Não Informado' and projeto = :projeto
                  union all select 'APP' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 0 is_subarea, 22 ORDEM from tmp_areas_calculadas where tipo='APP_APMP' and projeto = :projeto
                  union all select '-Preservada' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 23 ORDEM from tmp_areas_calculadas where tipo='APP_AVN' and projeto = :projeto
                  union all select '-Em formação' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 24 ORDEM from tmp_areas_calculadas where tipo='APP_AA' and projeto = :projeto
                  union all select '-Em ARL' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 1 is_subarea, 25 ORDEM from tmp_areas_calculadas where tipo='APP_ARL' and projeto = :projeto
                  union all select 'Massa Dagua' feicao, nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 0 is_subarea, 26 ordem from tmp_areas_calculadas where tipo='MASSA_DAGUA' and projeto = :projeto
                  ) order by ordem ");*/


           
                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();

				try
				{
					reader = bancoGeo.ExecutarReader(comando);

				#endregion
					
					categoria.Nome = "Total";
					categoria.Itens = new List<ItemQuadroDeArea>();
					while (reader.Read())
					{
						ItemQuadroDeArea item = new ItemQuadroDeArea();
						item.Nome = Convert.ToString(reader["feicao"]);

						item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
						item.IsProcessada = Convert.ToString(reader["is_processada"]) == "1";
						item.IsSubArea = Convert.ToString(reader["is_subarea"]) == "1";
						if (item.IsProcessada)
							item.Nome = item.Nome + " {" + situacaoTexto + "}";

						categoria.Itens.Add(item);
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
                lista.Add(categoria);

                comando = bancoGeo.GetComandoSql(@"select
                  (case when a.tipo = 'M' then 'Matrícula: 'else 'Posse: 'end) ||upper(a.nome) matricula_nome,
                  a.area_m2 matricula_area,
                  nvl( (select sum(b.area_m2) from des_rocha b where b.cod_apmp = a.id), 0) rocha_area,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id), 0) avn_area,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='I'), 0) avn_area_inicial,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='M'), 0) avn_area_medio,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='A'), 0) avn_area_avancado,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='D'), 0) avn_area_desconhecido,
                   trunc(nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id), 0),4) aa_area,
                  trunc( nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id and b.tipo='REC'), 0), 4) aa_rec_area,
                  trunc( nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id and b.tipo='USO'), 0), 4) aa_uso_area,
                  trunc( nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id and b.tipo='D'), 0), 4) aa_d_area,
                   nvl( (select sum(b.area_m2) from des_afs b where b.cod_apmp = a.id), 0) afs_area,
                  nvl( (select sum(b.area_m2) from des_rppn b where b.cod_apmp = a.id), 0) rppn_area,
                  nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id ), 0) arl_area,
                  trunc( nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id and b.situacao='PRESERV'), 0), 4) arl_area_preservada,
                  trunc( nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id and b.situacao='REC'), 0), 4) arl_area_rec,
                  trunc( nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id and b.situacao='USO'), 0), 4) arl_area_uso,
                  trunc( nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id and b.situacao='D'), 0), 4) arl_area_desconhecido,
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_area_apmp,
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_area_preservada,                  
                  trunc( nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0), 4) app_aa_rec_area,    
                  trunc( nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0), 4) app_aa_uso_area,    
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_area_arl,
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area
                from des_apmp a where a.projeto=:projeto and ( a.tipo = 'M' or  a.tipo = 'P')  order by matricula_nome");



/*@"select
               (case when a.tipo = 'M' then 'Matrícula: 'else 'Posse: 'end) ||upper(a.nome) matricula_nome,
                  a.area_m2 matricula_area,
                  nvl( (select sum(b.area_m2) from des_rocha b where b.cod_apmp = a.id), 0) rocha_area,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id), 0) avn_area,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='I'), 0) avn_area_inicial,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='M'), 0) avn_area_medio,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='A'), 0) avn_area_avancado,
                  nvl( (select sum(b.area_m2) from des_avn b where b.cod_apmp = a.id and b.estagio='D'), 0) avn_area_desconhecido,
                  nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id), 0) aa_area,
                  nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id and b.tipo='C'), 0) aa_area_cultivada,
                  nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id and b.tipo='NC'), 0) aa_area_nao_cultiv,
                  nvl( (select sum(b.area_m2) from des_aa b where b.cod_apmp = a.id and b.tipo='D'), 0) aa_area_desconhec,  
                  nvl( (select sum(b.area_m2) from des_afs b where b.cod_apmp = a.id), 0) afs_area,
                  nvl( (select sum(b.area_m2) from des_rppn b where b.cod_apmp = a.id), 0) rppn_area,
                  nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id ), 0) arl_area,
                  nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id and b.situacao='AVN'), 0) arl_area_preservada,
                  nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id and b.situacao='AA'), 0) arl_area_emformacao,
                  nvl( (select sum(b.area_m2) from des_arl b where b.cod_apmp = a.id and b.situacao is null), 0) arl_area_desconhecido,

                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_area_apmp,
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_area_preservada,
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA'), 0) app_area_emformacao,
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_area_arl,
                  nvl( (select sum(b.area_m2) from tmp_areas_calculadas b where b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area
                from des_apmp a where a.projeto=:projeto and ( a.tipo = 'M' or  a.tipo = 'P')  order by matricula_nome ");*/

                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

                reader = bancoGeo.ExecutarReader(comando);

                while (reader.Read())
                {
                    ItemQuadroDeArea item = null;
                    categoria = new CategoriaQuadroDeArea();
                    categoria.Nome = Convert.ToString(reader["matricula_nome"]);
                    categoria.Itens = new List<ItemQuadroDeArea>();

                    item = new ItemQuadroDeArea();
                    item.Nome = "APMP";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["matricula_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "Rocha";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rocha_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "AVN";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Estágio inicial";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_inicial"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Estágio médio";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_medio"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Estágio avançado";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_avancado"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_desconhecido"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "AA";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em recuperação";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_rec_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em uso";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_uso_area"]));
                    categoria.Itens.Add(item);

                   /* item = new ItemQuadroDeArea();
                    item.Nome = "-Cultivada";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area_cultivada"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não cultivada";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area_nao_cultiv"]));
                    categoria.Itens.Add(item);
                    */
                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_d_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "AFS";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["afs_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "RPPN";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rppn_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "ARL";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area"]));
                    categoria.Itens.Add(item);

                   /* item = new ItemQuadroDeArea();
                    item.Nome = "ARL " + "{" + situacaoTexto + "}";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_processada"]));
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);
                    */
                   /* item = new ItemQuadroDeArea();
                    item.Nome = "-Preservada " ;
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_preservada"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em recuperação ";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_rec"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em uso " ;
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_uso"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);
                  
                    //item = new ItemQuadroDeArea();
                    //item.Nome = "-Em formação " + "{" + situacaoTexto + "}";
                    //item.IsSubArea = true;
                    //item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_emformacao"]));
                    //item.IsProcessada = false;
                    //categoria.Itens.Add(item);
                   
                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado " ;
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_desconhecido"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);
                    */
                    double appNaoCaracterizada = 0;
                    item = new ItemQuadroDeArea();
                    item.Nome = "APP " + "{" + situacaoTexto + "}";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_apmp"]));
                    appNaoCaracterizada = Convert.ToDouble(reader["app_area_apmp"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    
                    item = new ItemQuadroDeArea();
                    item.Nome = "-Preservada " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_preservada"]));
                    appNaoCaracterizada -= Convert.ToDouble(reader["app_area_preservada"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);
                    /*
                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em formação " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_emformacao"]));
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);*/

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em recuperação " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_rec_area"]));
                    appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_rec_area"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em uso " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_uso_area"]));
                    appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_uso_area"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", appNaoCaracterizada);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em reserva legal " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_arl"]));
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "Massa Dagua " + "{" + situacaoTexto + "}";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["massa_dagua_area"]));
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    lista.Add(categoria);
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

		public List<CategoriaQuadroDeArea> ListarQuadroAreasDominialidadeFinalizado(int idProjeto)
		{
			Comando comando = null;
			List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
			IDataReader reader = null;
			IDataReader readerQtd = null;
			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
				string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
				BancoDeDados bancoGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
				string situacaoTexto = "Não processada";

				#region Busca situação do projeto
				comando = bancoGeo.GetComandoSql(string.Format(@"select (case when is_valido_proces != 1 or is_valido_proces is null then 'Inválido' else 'Válido' end) situacao_texto, is_valido_proces  
                  from {0}.tab_navegador_projeto where projeto = :projeto", schemaUsuarioGeo));
				
				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				try
				{
					reader = bancoGeo.ExecutarReader(comando);
					if (reader.Read())
					{
						situacaoTexto = Convert.ToString(reader["situacao_texto"]);
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

				#region Total de Quadro de Áreas

				comando = bancoGeo.GetComandoSql(@"select * from (
					select 'ATP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  1 ORDEM from geo_atp where projeto=:projeto
					union all select 'Área Construída' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  2 ORDEM from geo_aconstruida where projeto=:projeto
					union all select 'AFD' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2, 0 is_processada, 0 is_subarea,  3 ORDEM from geo_afd where projeto=:projeto
					union all select 'APMP'  feicao,  trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  4 ORDEM from geo_apmp where projeto=:projeto
					union all select 'Rocha' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  5 ORDEM from geo_rocha where projeto=:projeto
					union all select 'AVN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  6 ORDEM from geo_avn where projeto=:projeto
					union all select ' -Estágio inicial' feicao,trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  7 ORDEM from geo_avn where estagio='I' and projeto=:projeto 
					union all select ' -Estágio médio' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  8 ORDEM from geo_avn where estagio='M' and projeto=:projeto 
					union all select ' -Estágio avançado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  9 ORDEM from geo_avn where estagio='A' and projeto=:projeto 
					union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  10 ORDEM from geo_avn where estagio='D' and projeto=:projeto 
					union all select 'AA' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  11 ORDEM from geo_aa where projeto=:projeto
					union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  12 ORDEM from geo_aa where tipo='REC' and projeto=:projeto
					union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  13 ORDEM from geo_aa where tipo='USO' and projeto=:projeto 
					union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  14 ORDEM from geo_aa where tipo='D' and projeto=:projeto
					union all select 'ARL' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  15 ORDEM from geo_arl where projeto=:projeto
					union all select 'AFS' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  20 ORDEM from geo_afs where projeto=:projeto
					union all select 'RPPN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  21 ORDEM from geo_rppn where projeto=:projeto
					union all select 'APP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  22 ORDEM from geo_areas_calculadas where tipo='APP_APMP' and projeto=:projeto
					union all select ' -Preservada' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  23 ORDEM from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projeto
					union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  24 ORDEM from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projeto
					union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  25 ORDEM from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projeto
					union all select ' -Não caracterizado' feicao, trunc(nvl(apmp.area_m2-avn.area_m2-rec.area_m2-uso.area_m2,0),4) area_m2,  1 is_processada, 1 is_subarea,  27 ORDEM from 
						(select  nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas apmp where tipo='APP_APMP' and projeto=:projeto) apmp, 
						(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projeto) avn,
						(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projeto) rec,
						(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projeto) uso
					union all select ' -Em reserva legal' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  26 ORDEM from geo_areas_calculadas where tipo='APP_ARL' and projeto=:projeto
					union all select 'Massa dagua' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  28 ORDEM from geo_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto=:projeto
					) order by ordem");

				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();

				#endregion

				try
				{
					reader = bancoGeo.ExecutarReader(comando);

					categoria.Nome = "Total";
					categoria.Itens = new List<ItemQuadroDeArea>();
					while (reader.Read())
					{
						ItemQuadroDeArea item = new ItemQuadroDeArea();
						item.Nome = Convert.ToString(reader["feicao"]);

						item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
						item.IsProcessada = Convert.ToString(reader["is_processada"]) == "1";
						item.IsSubArea = Convert.ToString(reader["is_subarea"]) == "1";
						if (item.IsProcessada)
							item.Nome = item.Nome + " {" + situacaoTexto + "}";

						categoria.Itens.Add(item);
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
				lista.Add(categoria);

				comando = bancoGeo.GetComandoSql(@"select
					(case when a.tipo = 'M' then 'Matrícula: 'else 'Posse: 'end) ||upper(a.nome) matricula_nome,
					a.area_m2 matricula_area,
					nvl( (select sum(b.area_m2) from geo_rocha b where b.cod_apmp = a.id), 0) rocha_area,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id), 0) avn_area,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='I'), 0) avn_area_inicial,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='M'), 0) avn_area_medio,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='A'), 0) avn_area_avancado,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='D'), 0) avn_area_desconhecido,
					trunc(nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id), 0),4) aa_area,
					trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='REC'), 0), 4) aa_rec_area,
					trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='USO'), 0), 4) aa_uso_area,
					trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='D'), 0), 4) aa_d_area,
					nvl( (select sum(b.area_m2) from geo_afs b where b.cod_apmp = a.id), 0) afs_area,
					nvl( (select sum(b.area_m2) from geo_rppn b where b.cod_apmp = a.id), 0) rppn_area,
					nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id ), 0) arl_area,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='PRESERV'), 0), 4) arl_area_preservada,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='REC'), 0), 4) arl_area_rec,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='USO'), 0), 4) arl_area_uso,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='D'), 0), 4) arl_area_desconhecido,
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_area_apmp,
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_area_preservada,                  
					trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0), 4) app_aa_rec_area,    
					trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0), 4) app_aa_uso_area,    
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_area_arl,
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area
					from geo_apmp a where a.projeto=:projeto and ( a.tipo = 'M' or  a.tipo = 'P')  order by matricula_nome");

				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				reader = bancoGeo.ExecutarReader(comando);

				while (reader.Read())
				{
					ItemQuadroDeArea item = null;
					categoria = new CategoriaQuadroDeArea();
					categoria.Nome = Convert.ToString(reader["matricula_nome"]);
					categoria.Itens = new List<ItemQuadroDeArea>();

					item = new ItemQuadroDeArea();
					item.Nome = "APMP";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["matricula_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "Rocha";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rocha_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AVN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio inicial";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_inicial"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio médio";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_medio"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio avançado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_avancado"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_desconhecido"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AA";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em recuperação";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_rec_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em uso";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_uso_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_d_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AFS";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["afs_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "RPPN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rppn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "ARL";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area"]));
					categoria.Itens.Add(item);

					double appNaoCaracterizada = 0;
					item = new ItemQuadroDeArea();
					item.Nome = "APP " + "{" + situacaoTexto + "}";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_apmp"]));
					appNaoCaracterizada = Convert.ToDouble(reader["app_area_apmp"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);


					item = new ItemQuadroDeArea();
					item.Nome = "-Preservada " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_preservada"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_area_preservada"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em recuperação " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_rec_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_rec_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em uso " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_uso_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_uso_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", appNaoCaracterizada);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em reserva legal " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_arl"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "Massa Dagua " + "{" + situacaoTexto + "}";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["massa_dagua_area"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					lista.Add(categoria);
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

        #region Quadro de Áreas - Atividade
        public List<CategoriaQuadroDeArea> ListarQuadroAreasAtividade(int idProjeto)
        {
            Comando comando = null;
            List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
            IDataReader reader = null;
            IDataReader readerQtd = null;
            int idProjetoDominialidade = 0;
            try
            {
                string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

                comando = bancoDeDadosGeo.GetComandoSql(string.Format(@"select id projeto_dominialidade from {0}.crt_projeto_geo a where  a.empreendimento = (select empreendimento from {0}.tmp_projeto_geo pg where pg.id = :projeto) and a.caracterizacao = 1 ",schemaUsuario));
                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);

					if (reader.Read())
					{
						if (!string.IsNullOrEmpty(Convert.ToString(reader["projeto_dominialidade"])))
						{
							idProjetoDominialidade = Convert.ToInt32(reader["projeto_dominialidade"]);
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
                
                string situacaoTexto = "Não processada";
                #region Busca situação do projeto
                comando = bancoDeDadosGeo.GetComandoSql(string.Format(@"select (case when is_valido_proces != 1 or is_valido_proces is null then 'Inválido' else 'Válido' end) situacao_texto, is_valido_proces  
                  from {0}.tab_navegador_projeto where projeto = :projeto", schemaUsuarioGeo));
                /*comando = bancoDeDadosGeo.GetComandoSql(string.Format(@"select lf.id situacao, lf.texto situacao_texto from {0}.lov_crt_projeto_geo_sit_proce lf,  {1}.tab_fila f
                 where f.projeto = :projeto and  lf.situacao  = f.situacao and lf.etapa = f.etapa and f.mecanismo_elaboracao = 2 and f.tipo = 4 ", schemaUsuario, schemaUsuarioGeo));
                */
                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);
					if (reader.Read())
					{
						situacaoTexto = Convert.ToString(reader["situacao_texto"]);
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

                #region Total de Quadro de Áreas
                comando = bancoDeDadosGeo.GetComandoSql(@"select * from ( 
                  select 'Área de influência da atividade' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 1 ORDEM  from des_AIATIV a where a.PROJETO = :projeto
                  union all select 'Área da atividade' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 2 ORDEM  from des_AATIV a where a.PROJETO = :projeto
                  union all select 'ATP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea,  3 ORDEM from geo_atp a where a.PROJETO = :projeto_dominialidade
                  union all select 'Área construída' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 4 ORDEM  from geo_aconstruida a where a.PROJETO = :projeto_dominialidade
                  union all select 'AFD' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 5 ORDEM  from geo_afd a where a.PROJETO = :projeto_dominialidade
                  union all select 'APMP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 7 ORDEM  from geo_apmp a where a.PROJETO = :projeto_dominialidade
                  union all select 'Rocha' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 8 ORDEM  from geo_rocha a where a.PROJETO = :projeto_dominialidade
                  union all select 'AVN' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 9 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade
                  union all select '-Estágio inicial' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 10 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'I' 
                  union all select '-Estágio médio' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 11 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'M' 
                  union all select '-Estágio avançado' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 12 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'A' 
                  union all select '-Não caracterizado' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 13 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'D' 
                  union all select 'AA' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 14 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade
                  union all select '-Em recuperação' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 15 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'REC'
                  union all select '-Em uso' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 16 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'USO' " +// group by vegetacao
                  @"union all select '-Não caracterizado' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 1 is_subarea, 17 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'D'
                  union all select 'AFS' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 18 ORDEM  from geo_afs a where a.PROJETO = :projeto_dominialidade
                  union all select 'RPPN' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 19 ORDEM  from geo_rppn a where a.PROJETO = :projeto_dominialidade
                  union all select 'ARL' feicao, nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 0 is_subarea, 20 ORDEM from geo_arl where projeto = :projeto_dominialidade
                  "+ /*union all select '-Preservada' feicao, nvl( sum(area_m2), 0 ) area_m2, 0 is_processada, 1 is_subarea, 21 ORDEM from geo_arl where situacao='PRESERV' and projeto = :projeto_dominialidade
                  union all select '-Em recuperação' feicao,  nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 1 is_subarea, 22 ORDEM from geo_arl where situacao='REC' and projeto = :projeto_dominialidade
                  union all select '-Em  uso' feicao,  nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 1 is_subarea, 23 ORDEM from geo_arl where situacao='USO' and projeto = :projeto_dominialidade
                  union all select '-Não caracterizado' feicao, nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 1 is_subarea, 24 ORDEM from geo_arl where situacao='D' and projeto = :projeto_dominialidade
                 */
                  @" union all select 'APP' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 0 is_subarea, 25 ORDEM from geo_areas_calculadas where tipo='APP_APMP' and projeto = :projeto_dominialidade
                  union all select '-Preservada' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 26 ORDEM from geo_areas_calculadas where tipo='APP_AVN' and projeto = :projeto_dominialidade
                  union all select '-Em recuperação' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 27 ORDEM from geo_areas_calculadas where tipo='APP_AA_REC' and projeto = :projeto_dominialidade
                  union all select '-Em uso' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 28 ORDEM from geo_areas_calculadas where tipo='APP_AA_USO' and projeto = :projeto_dominialidade
                  union all select '-Não caracterizado' feicao, trunc(nvl(apmp.area_m2-avn.area_m2-rec.area_m2-uso.area_m2,0),4) area_m2,  1 is_processada, 1 is_subarea,  29 ORDEM from 
                    (select  nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas apmp where tipo='APP_APMP' and projeto=:projeto_dominialidade) apmp, 
                    (select nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas where tipo='APP_AVN' and projeto=:projeto_dominialidade) avn,
                    (select nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas where tipo='APP_AA_REC' and projeto=:projeto_dominialidade) rec,
                    (select nvl( sum(area_m2), 0 ) area_m2 from tmp_areas_calculadas where tipo='APP_AA_USO' and projeto=:projeto_dominialidade) uso
                  union all select '-Em reserva legal' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 1 is_subarea, 30 ORDEM from geo_areas_calculadas where tipo='APP_ARL' and projeto = :projeto_dominialidade
                  union all select 'Massa Dagua' feicao, nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 0 is_subarea, 31 ordem from geo_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto = :projeto_dominialidade
                  ) order by ordem  ");

                /*
@"select * from ( 
                  select 'Área de influência da atividade' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 1 ORDEM  from des_AIATIV a where a.PROJETO = :projeto
                  union all select 'Área da atividade' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 2 ORDEM  from des_AATIV a where a.PROJETO = :projeto
                  union all select 'ATP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea,  3 ORDEM from geo_atp a where a.PROJETO = :projeto_dominialidade
                  union all select 'Área construída' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 4 ORDEM  from geo_aconstruida a where a.PROJETO = :projeto_dominialidade
                  union all select 'AFD' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 5 ORDEM  from geo_afd a where a.PROJETO = :projeto_dominialidade
                  union all select 'APMP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 7 ORDEM  from geo_apmp a where a.PROJETO = :projeto_dominialidade
                  union all select 'Rocha' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 8 ORDEM  from geo_rocha a where a.PROJETO = :projeto_dominialidade
                  union all select 'AVN' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 9 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade
                  union all select '-Estágio inicial' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 10 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'I'
                  union all select '-Estágio médio' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 11 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'M'
                  union all select '-Estágio avançado' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 12 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'A'
                  union all select '-Desconhecido' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 13 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'D'
                  union all select 'AA' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 14 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade
                  union all select '-Cultivada' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 15 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'C'
                  union all select '-Não cultivada' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 16 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'NC'
                  union all select '-Desconhecida' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 1 is_subarea, 17 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'D'
                  union all select 'AFS' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 18 ORDEM  from geo_afs a where a.PROJETO = :projeto_dominialidade
                  union all select 'RPPN' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 19 ORDEM  from geo_rppn a where a.PROJETO = :projeto_dominialidade
                  union all select 'ARL' feicao, nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 0 is_subarea, 20 ORDEM from geo_arl where projeto = :projeto_dominialidade
                  union all select '-Preservada' feicao, nvl( sum(area_m2), 0 ) area_m2, 0 is_processada, 1 is_subarea, 21 ORDEM from geo_arl where situacao='AVN' and projeto = :projeto_dominialidade
                  union all select '-Em Formação' feicao,  nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 1 is_subarea, 22 ORDEM from geo_arl where situacao='AA' and projeto = :projeto_dominialidade
                  union all select '-Indefinida' feicao, nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 1 is_subarea, 23 ORDEM from geo_arl where situacao='Não Informado' and projeto = :projeto_dominialidade
                  union all select 'APP' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 0 is_subarea, 24 ORDEM from geo_areas_calculadas where tipo='APP_APMP' and projeto = :projeto_dominialidade
                  union all select '-Preservada' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 25 ORDEM from geo_areas_calculadas where tipo='APP_AVN' and projeto = :projeto_dominialidade
                  union all select '-Em formação' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 26 ORDEM from geo_areas_calculadas where tipo='APP_AA' and projeto = :projeto_dominialidade
                  union all select '-Em ARL' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 1 is_subarea, 27 ORDEM from geo_areas_calculadas where tipo='APP_ARL' and projeto = :projeto_dominialidade
                  union all select 'Massa Dagua' feicao, nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 0 is_subarea, 28 ordem from geo_areas_calculadas where tipo='MASSA_DAGUA' and projeto = :projeto_dominialidade
                  ) order by ordem ");*/

                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
                comando.AdicionarParametroEntrada("projeto_dominialidade", idProjetoDominialidade, DbType.Int32);

				CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();
				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);

				#endregion

					
					categoria.Nome = "Total";
					categoria.Itens = new List<ItemQuadroDeArea>();
					while (reader.Read())
					{
						ItemQuadroDeArea item = new ItemQuadroDeArea();
						item.Nome = Convert.ToString(reader["feicao"]);

						item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
						item.IsProcessada = Convert.ToString(reader["is_processada"]) == "1";
						item.IsSubArea = Convert.ToString(reader["is_subarea"]) == "1";
						if (item.IsProcessada)
							item.Nome = item.Nome + " {" + situacaoTexto + "}";

						categoria.Itens.Add(item);
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
                lista.Add(categoria);

                comando = bancoDeDadosGeo.GetComandoSql(@"select
                (case when a.tipo = 'M' then 'Matrícula: 'else 'Posse: 'end) ||upper(a.nome) matricula_nome,
                a.area_m2 matricula_area,
                nvl( (select sum(b.area_m2) from des_AIATIV b where b.cod_apmp = a.id and a.projeto = :projeto ), 0) area_influencia_area,
                nvl( (select sum(b.area_m2) from des_AATIV b where b.cod_apmp = a.id and a.projeto = :projeto ), 0) atividade_area,

                nvl( (select sum(b.area_m2) from geo_rocha b where b.cod_apmp = a.id), 0) rocha_area,

                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id), 0) avn_area,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='I'), 0) avn_area_inicial,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='M'), 0) avn_area_medio,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='A'), 0) avn_area_avancado,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='D'), 0) avn_area_desconhecido,
                nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id), 0) aa_area,
                trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='REC'), 0), 4) aa_rec_area,
                trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='USO'), 0), 4) aa_uso_area,
                nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='D'), 0) aa_area_desconhec,  
                nvl( (select sum(b.area_m2) from geo_afs b where b.cod_apmp = a.id), 0) afs_area,
                nvl( (select sum(b.area_m2) from geo_rppn b where b.cod_apmp = a.id), 0) rppn_area,
                nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id ), 0) arl_area,

                nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='AVN'), 0) arl_area_preservada,

                trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='REC'), 0), 4) arl_area_rec,
                trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='USO'), 0), 4) arl_area_uso,

            
                nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao is null), 0) arl_area_desconhecido,

                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_area_apmp,
                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_area_preservada,

                trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0), 4) app_aa_rec_area,    
                 trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0), 4) app_aa_uso_area,    
                
                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_area_arl,
                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area
                from geo_apmp a where a.projeto=:projeto_dominialidade and ( a.tipo = 'M' or  a.tipo = 'P')  order by matricula_nome");

                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
                comando.AdicionarParametroEntrada("projeto_dominialidade", idProjetoDominialidade, DbType.Int32);
                reader = bancoDeDadosGeo.ExecutarReader(comando);

                while (reader.Read())
                {
                    ItemQuadroDeArea item = null;
                    categoria = new CategoriaQuadroDeArea();
                    categoria.Nome = Convert.ToString(reader["matricula_nome"]);
                    categoria.Itens = new List<ItemQuadroDeArea>();

                    item = new ItemQuadroDeArea();
                    item.Nome = "Área de influência da atividade";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_influencia_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "Área da atividade";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["atividade_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "APMP";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["matricula_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "Rocha";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rocha_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "AVN";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Estágio inicial";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_inicial"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Estágio médio";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_medio"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Estágio avançado";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_avancado"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_desconhecido"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "AA";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area"]));
                    categoria.Itens.Add(item);

                   /* item = new ItemQuadroDeArea();
                    item.Nome = "-Cultivada";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area_cultivada"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não cultivada";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area_nao_cultiv"]));
                    categoria.Itens.Add(item);
                    */

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em recuperação";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_rec_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em uso";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_uso_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area_desconhec"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "AFS";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["afs_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "RPPN";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rppn_area"]));
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "ARL";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area"]));
                    categoria.Itens.Add(item);

                     //item = new ItemQuadroDeArea();
                     //item.Nome = "ARL " + "{" + situacaoTexto + "}";
                     //item.IsSubArea = false;
                     //item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_processada"]));
                     //item.IsProcessada = true;
                     //categoria.Itens.Add(item);
                    /* 
                    item = new ItemQuadroDeArea();
                    item.Nome = "-Preservada";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_preservada"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);
                    
                    //item = new ItemQuadroDeArea();
                    //item.Nome = "-Em formação " + "{" + situacaoTexto + "}";
                    //item.IsSubArea = true;
                    //item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_emformacao"]));
                    //item.IsProcessada = false;
                    //categoria.Itens.Add(item);
                    

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em recuperação";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_rec"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em uso ";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_uso"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado ";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area_desconhecido"]));
                    item.IsProcessada = false;
                    categoria.Itens.Add(item);
                    */
                    item = new ItemQuadroDeArea();
                    item.Nome = "APP " + "{" + situacaoTexto + "}";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_apmp"]));
                    double appNaoCaracterizada = Convert.ToDouble(reader["app_area_apmp"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Preservada " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_preservada"]));
                    appNaoCaracterizada -= Convert.ToDouble(reader["app_area_preservada"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                   /* item = new ItemQuadroDeArea();
                    item.Nome = "-Em formação " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_emformacao"]));
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);
                    */

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em recuperação " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_rec_area"]));
                    appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_rec_area"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em uso " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_uso_area"]));
                    appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_uso_area"]);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Não caracterizado " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", appNaoCaracterizada);
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "-Em reserva legal " + "{" + situacaoTexto + "}";
                    item.IsSubArea = true;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_arl"]));
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    item = new ItemQuadroDeArea();
                    item.Nome = "Massa Dagua " + "{" + situacaoTexto + "}";
                    item.IsSubArea = false;
                    item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["massa_dagua_area"]));
                    item.IsProcessada = true;
                    categoria.Itens.Add(item);

                    lista.Add(categoria);
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

		public List<CategoriaQuadroDeArea> ListarQuadroAreasAtividadeFinalizado(int idProjeto)
		{
			Comando comando = null;
			List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
			IDataReader reader = null;
			IDataReader readerQtd = null;
			int idProjetoDominialidade = 0;
			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
				string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
				BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				comando = bancoDeDadosGeo.GetComandoSql(string.Format(@"select id projeto_dominialidade from {0}.crt_projeto_geo a where  a.empreendimento = (select empreendimento from {0}.crt_projeto_geo pg where pg.id = :projeto) and a.caracterizacao = 1 ", schemaUsuario));
				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);

					if (reader.Read())
					{
						if (!string.IsNullOrEmpty(Convert.ToString(reader["projeto_dominialidade"])))
						{
							idProjetoDominialidade = Convert.ToInt32(reader["projeto_dominialidade"]);
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

				string situacaoTexto = "Não processada";
				#region Busca situação do projeto
				comando = bancoDeDadosGeo.GetComandoSql(string.Format(@"select (case when is_valido_proces != 1 or is_valido_proces is null then 'Inválido' else 'Válido' end) situacao_texto, is_valido_proces  
                  from {0}.tab_navegador_projeto where projeto = :projeto", schemaUsuarioGeo));

				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);
					if (reader.Read())
					{
						situacaoTexto = Convert.ToString(reader["situacao_texto"]);
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

				#region Total de Quadro de Áreas
				comando = bancoDeDadosGeo.GetComandoSql(@"select * from ( 
					select 'Área de influência da atividade' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 1 ORDEM  from des_AIATIV a where a.PROJETO = :projeto
					union all select 'Área da atividade' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 2 ORDEM  from des_AATIV a where a.PROJETO = :projeto
					union all select 'ATP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea,  3 ORDEM from geo_atp a where a.PROJETO = :projeto_dominialidade
					union all select 'Área construída' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 4 ORDEM  from geo_aconstruida a where a.PROJETO = :projeto_dominialidade
					union all select 'AFD' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 5 ORDEM  from geo_afd a where a.PROJETO = :projeto_dominialidade
					union all select 'APMP' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 7 ORDEM  from geo_apmp a where a.PROJETO = :projeto_dominialidade
					union all select 'Rocha' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 8 ORDEM  from geo_rocha a where a.PROJETO = :projeto_dominialidade
					union all select 'AVN' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 0 is_subarea, 9 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade
					union all select '-Estágio inicial' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 10 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'I' 
					union all select '-Estágio médio' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 11 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'M' 
					union all select '-Estágio avançado' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 12 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'A' 
					union all select '-Não caracterizado' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 13 ORDEM  from geo_avn a where a.PROJETO = :projeto_dominialidade and a.estagio = 'D' 
					union all select 'AA' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 14 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade
					union all select '-Em recuperação' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 15 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'REC'
					union all select '-Em uso' feicao, nvl(sum(a.AREA_M2),0) area_m2,0 is_processada, 1 is_subarea, 16 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'USO' " +// group by vegetacao
					@"union all select '-Não caracterizado' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 1 is_subarea, 17 ORDEM  from  geo_aa a where a.PROJETO = :projeto_dominialidade and a.TIPO = 'D'
					union all select 'AFS' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 18 ORDEM  from geo_afs a where a.PROJETO = :projeto_dominialidade
					union all select 'RPPN' feicao, nvl(sum(a.AREA_M2),0) area_m2, 0 is_processada, 0 is_subarea, 19 ORDEM  from geo_rppn a where a.PROJETO = :projeto_dominialidade
					union all select 'ARL' feicao, nvl( sum(area_m2), 0 ) area_m2,0 is_processada, 0 is_subarea, 20 ORDEM from geo_arl where projeto = :projeto_dominialidade
					union all select 'APP' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 0 is_subarea, 25 ORDEM from geo_areas_calculadas where tipo='APP_APMP' and projeto = :projeto_dominialidade
					union all select '-Preservada' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 26 ORDEM from geo_areas_calculadas where tipo='APP_AVN' and projeto = :projeto_dominialidade
					union all select '-Em recuperação' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 27 ORDEM from geo_areas_calculadas where tipo='APP_AA_REC' and projeto = :projeto_dominialidade
					union all select '-Em uso' feicao,  nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 1 is_subarea, 28 ORDEM from geo_areas_calculadas where tipo='APP_AA_USO' and projeto = :projeto_dominialidade
					union all select '-Não caracterizado' feicao, trunc(nvl(apmp.area_m2-avn.area_m2-rec.area_m2-uso.area_m2,0),4) area_m2,  1 is_processada, 1 is_subarea,  29 ORDEM from 
					(select  nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas apmp where tipo='APP_APMP' and projeto=:projeto_dominialidade) apmp, 
					(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projeto_dominialidade) avn,
					(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projeto_dominialidade) rec,
					(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projeto_dominialidade) uso
					union all select '-Em reserva legal' feicao,  nvl( sum(area_m2), 0 ) area_m2, 1 is_processada, 1 is_subarea, 30 ORDEM from geo_areas_calculadas where tipo='APP_ARL' and projeto = :projeto_dominialidade
					union all select 'Massa Dagua' feicao, nvl( sum(area_m2), 0 ) area_m2,1 is_processada, 0 is_subarea, 31 ordem from geo_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto = :projeto_dominialidade
					) order by ordem  ");

				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_dominialidade", idProjetoDominialidade, DbType.Int32);

				#endregion

				CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();
				
				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);

					categoria.Nome = "Total";
					categoria.Itens = new List<ItemQuadroDeArea>();
					while (reader.Read())
					{
						ItemQuadroDeArea item = new ItemQuadroDeArea();
						item.Nome = Convert.ToString(reader["feicao"]);

						item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
						item.IsProcessada = Convert.ToString(reader["is_processada"]) == "1";
						item.IsSubArea = Convert.ToString(reader["is_subarea"]) == "1";
						if (item.IsProcessada)
							item.Nome = item.Nome + " {" + situacaoTexto + "}";

						categoria.Itens.Add(item);
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
				lista.Add(categoria);

				comando = bancoDeDadosGeo.GetComandoSql(@"select
                (case when a.tipo = 'M' then 'Matrícula: 'else 'Posse: 'end) ||upper(a.nome) matricula_nome,
                a.area_m2 matricula_area,
                nvl( (select sum(b.area_m2) from des_AIATIV b where b.cod_apmp = a.id and a.projeto = :projeto ), 0) area_influencia_area,
                nvl( (select sum(b.area_m2) from des_AATIV b where b.cod_apmp = a.id and a.projeto = :projeto ), 0) atividade_area,

                nvl( (select sum(b.area_m2) from geo_rocha b where b.cod_apmp = a.id), 0) rocha_area,

                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id), 0) avn_area,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='I'), 0) avn_area_inicial,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='M'), 0) avn_area_medio,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='A'), 0) avn_area_avancado,
                nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='D'), 0) avn_area_desconhecido,
                nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id), 0) aa_area,
                trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='REC'), 0), 4) aa_rec_area,
                trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='USO'), 0), 4) aa_uso_area,
                nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='D'), 0) aa_area_desconhec,  
                nvl( (select sum(b.area_m2) from geo_afs b where b.cod_apmp = a.id), 0) afs_area,
                nvl( (select sum(b.area_m2) from geo_rppn b where b.cod_apmp = a.id), 0) rppn_area,
                nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id ), 0) arl_area,

                nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='AVN'), 0) arl_area_preservada,

                trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='REC'), 0), 4) arl_area_rec,
                trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='USO'), 0), 4) arl_area_uso,

            
                nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao is null), 0) arl_area_desconhecido,

                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_area_apmp,
                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_area_preservada,

                trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0), 4) app_aa_rec_area,    
                 trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0), 4) app_aa_uso_area,    
                
                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_area_arl,
                nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area
                from geo_apmp a where a.projeto=:projeto_dominialidade and ( a.tipo = 'M' or  a.tipo = 'P')  order by matricula_nome");

				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_dominialidade", idProjetoDominialidade, DbType.Int32);
				reader = bancoDeDadosGeo.ExecutarReader(comando);

				while (reader.Read())
				{
					ItemQuadroDeArea item = null;
					categoria = new CategoriaQuadroDeArea();
					categoria.Nome = Convert.ToString(reader["matricula_nome"]);
					categoria.Itens = new List<ItemQuadroDeArea>();

					item = new ItemQuadroDeArea();
					item.Nome = "Área de influência da atividade";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_influencia_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "Área da atividade";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["atividade_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "APMP";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["matricula_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "Rocha";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rocha_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AVN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio inicial";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_inicial"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio médio";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_medio"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio avançado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_avancado"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_desconhecido"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AA";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em recuperação";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_rec_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em uso";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_uso_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area_desconhec"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AFS";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["afs_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "RPPN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rppn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "ARL";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "APP " + "{" + situacaoTexto + "}";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_apmp"]));
					double appNaoCaracterizada = Convert.ToDouble(reader["app_area_apmp"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Preservada " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_preservada"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_area_preservada"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em recuperação " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_rec_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_rec_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em uso " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_uso_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_uso_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", appNaoCaracterizada);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em reserva legal " + "{" + situacaoTexto + "}";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_arl"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "Massa Dagua " + "{" + situacaoTexto + "}";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["massa_dagua_area"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					lista.Add(categoria);
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

        #region Quadro de Áreas - Fiscalização
        public List<CategoriaQuadroDeArea> ListarQuadroAreasFiscalizacao(int idProjeto)
        {
            Comando comando = null;
            List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
            IDataReader reader = null;
           
            try
            {
                string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

                #region Total de Quadro de Áreas
                comando = bancoDeDadosGeo.GetComandoSql(@"select 'Área' feicao, nvl(sum(a.AREA_M2),0) area_m2 from des_fiscal_area a where a.PROJETO = :projeto ");

                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

                CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();
                try
                {
                    reader = bancoDeDadosGeo.ExecutarReader(comando);

                #endregion

                    categoria.Nome = "Total";
                    categoria.Itens = new List<ItemQuadroDeArea>();
                    while (reader.Read())
                    {
                        ItemQuadroDeArea item = new ItemQuadroDeArea();
                        item.Nome = Convert.ToString(reader["feicao"]);
                        item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
                        item.IsProcessada = false;
                        item.IsSubArea = false;
                      
                        categoria.Itens.Add(item);
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
                lista.Add(categoria);

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
            return lista;
        }

		public List<CategoriaQuadroDeArea> ListarQuadroAreasFiscalizacaoFinalizado(int idProjeto)
		{
			Comando comando = null;
			List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
			IDataReader reader = null;

			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
				string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
				BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				#region Total de Quadro de Áreas
				comando = bancoDeDadosGeo.GetComandoSql(@"select 'Área' feicao, nvl(sum(a.AREA_M2),0) area_m2 from geo_fiscal_area a where a.PROJETO = :projeto ");

				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);

				CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();
				try
				{
					reader = bancoDeDadosGeo.ExecutarReader(comando);

				#endregion

					categoria.Nome = "Total";
					categoria.Itens = new List<ItemQuadroDeArea>();
					while (reader.Read())
					{
						ItemQuadroDeArea item = new ItemQuadroDeArea();
						item.Nome = Convert.ToString(reader["feicao"]);
						item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
						item.IsProcessada = false;
						item.IsSubArea = false;

						categoria.Itens.Add(item);
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
				lista.Add(categoria);

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
			return lista;
		}
        #endregion

		#region Quadro de Áreas - CAR
		public List<CategoriaQuadroDeArea> ListarQuadroAreasCAR(int projetoCarId)
		{
			Comando comando = null;
			List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
			IDataReader reader = null;
			IDataReader readerQtd = null;
			int projetoDomId = 0;

			string SituacaoTextoDom = "Não processada";
			string SituacaoTextoCAR = "Finalizada";
			
			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
				string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
				BancoDeDados bancoGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				#region Obter Projeto Dominialidade Id

				comando = bancoGeo.GetComandoSql(String.Format(@"select g.id from {0}.crt_projeto_geo g where g.caracterizacao = 1 and g.empreendimento in (
					select cc.empreendimento 
					 from {0}.crt_cad_ambiental_rural cc 
					 where cc.projeto_geo_id = :projetoCarId
					union 
					select tc.empreendimento 
					from {0}.tmp_cad_ambiental_rural tc 
					where tc.projeto_geo_id = :projetoCarId )", schemaUsuario));
				comando.AdicionarParametroEntrada("projetoCarId", projetoCarId, DbType.Int32);

				projetoDomId = Convert.ToInt32(bancoGeo.ExecutarScalar(comando));
				
				#endregion

				#region Busca situação do projeto
				comando = bancoGeo.GetComandoSql(string.Format(@"select (case when is_valido_proces != 1 or is_valido_proces is null then 'Inválido' else 'Válido' end) situacao_texto, is_valido_proces  
                  from {0}.tab_navegador_projeto where projeto = :projetoDomId", schemaUsuarioGeo));
				
				comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

				try
				{
					reader = bancoGeo.ExecutarReader(comando);
					if (reader.Read())
					{
						SituacaoTextoDom = Convert.ToString(reader["situacao_texto"]);
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

				#region Total de Quadro de Áreas

				comando = bancoGeo.GetComandoSql(@"select * from (
            select 'ATP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  1 ORDEM from geo_atp where projeto=:projetoDomId
            union all select 'Área Construída' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  2 ORDEM from geo_aconstruida where projeto=:projetoDomId
            union all select 'AFD' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2, 0 is_processada, 0 is_subarea,  3 ORDEM from geo_afd where projeto=:projetoDomId
            union all select 'APMP'  feicao,  trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  4 ORDEM from geo_apmp where projeto=:projetoDomId
           union all select 'Rocha' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  5 ORDEM from geo_rocha where projeto=:projetoDomId
            union all select 'AVN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  6 ORDEM from geo_avn where projeto=:projetoDomId
            union all select ' -Estágio inicial' feicao,trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  7 ORDEM from geo_avn where estagio='I' and projeto=:projetoDomId 
            union all select ' -Estágio médio' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  8 ORDEM from geo_avn where estagio='M' and projeto=:projetoDomId 
            union all select ' -Estágio avançado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  9 ORDEM from geo_avn where estagio='A' and projeto=:projetoDomId 
            union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  10 ORDEM from geo_avn where estagio='D' and projeto=:projetoDomId 
            union all select 'AA' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  11 ORDEM from geo_aa where projeto=:projetoDomId
            union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  12 ORDEM from geo_aa where tipo='REC' and projeto=:projetoDomId
            union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  13 ORDEM from geo_aa where tipo='USO' and projeto=:projetoDomId 
            union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  14 ORDEM from geo_aa where tipo='D' and projeto=:projetoDomId
            union all select 'ARL' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  15 ORDEM from geo_arl where projeto=:projetoDomId
            union all select 'AFS' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  20 ORDEM from geo_afs where projeto=:projetoDomId
            union all select 'RPPN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  21 ORDEM from geo_rppn where projeto=:projetoDomId
            union all select 'APP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  22 ORDEM from geo_areas_calculadas where tipo='APP_APMP' and projeto=:projetoDomId
            union all select ' -Preservada' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  23 ORDEM from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projetoDomId
            union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  24 ORDEM from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projetoDomId
			union all select ' -A recuperar' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  25 ORDEM from tmp_car_areas_calculadas where tipo='CAR_APP_AA_USO' and projeto=:projetoCarId
            union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  26 ORDEM from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projetoDomId
            union all select ' -Não caracterizado' feicao, trunc(nvl(apmp.area_m2-avn.area_m2-rec.area_m2-uso.area_m2,0),4) area_m2,  1 is_processada, 1 is_subarea,  28 ORDEM from 
                    (select  nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas apmp where tipo='APP_APMP' and projeto=:projetoDomId) apmp, 
                    (select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projetoDomId) avn,
                    (select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projetoDomId) rec,
                    (select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projetoDomId) uso
            union all select ' -Em reserva legal' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  27 ORDEM from geo_areas_calculadas where tipo='APP_ARL' and projeto=:projetoDomId
            union all select 'Massa dagua' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  29 ORDEM from geo_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto=:projetoDomId
           ) order by ordem");


				comando.AdicionarParametroEntrada("projetoCarId", projetoCarId, DbType.Int32);
				comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);
				

				CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();

				try
				{
					reader = bancoGeo.ExecutarReader(comando);

					categoria.Nome = "Total";
					categoria.Itens = new List<ItemQuadroDeArea>();
					while (reader.Read())
					{
						ItemQuadroDeArea item = new ItemQuadroDeArea();
						item.Nome = Convert.ToString(reader["feicao"]);

						item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
						item.IsProcessada = Convert.ToString(reader["is_processada"]) == "1";
						item.IsSubArea = Convert.ToString(reader["is_subarea"]) == "1";

						if (item.IsProcessada && item.Nome != " -A recuperar")
						{
							item.Nome = String.Format("{0} {{{1}}}", item.Nome, SituacaoTextoDom);
						}
						else if (item.Nome == " -A recuperar")
						{
							item.Nome = String.Format("{0} {{{1}}}", item.Nome, SituacaoTextoCAR);
						}

						categoria.Itens.Add(item);
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
				lista.Add(categoria);


				#endregion

				#region Areas por APMP

				comando = bancoGeo.GetComandoSql(@"select
                  (case when a.tipo = 'M' then 'Matrícula: 'else 'Posse: 'end) ||upper(a.nome) matricula_nome,
                  a.area_m2 matricula_area,
                  nvl( (select sum(b.area_m2) from geo_rocha b where b.cod_apmp = a.id and b.projeto = a.projeto), 0) rocha_area,
                  nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.projeto = a.projeto), 0) avn_area,
                  nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.projeto = a.projeto and b.estagio='I'), 0) avn_area_inicial,
                  nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.projeto = a.projeto and b.estagio='M'), 0) avn_area_medio,
                  nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.projeto = a.projeto and b.estagio='A'), 0) avn_area_avancado,
                  nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.projeto = a.projeto and b.estagio='D'), 0) avn_area_desconhecido,
                  trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.projeto = a.projeto), 0),4) aa_area,
                  trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='REC'), 0), 4) aa_rec_area,
                  trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='USO'), 0), 4) aa_uso_area,
                  trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='D'), 0), 4) aa_d_area,
                  nvl( (select sum(b.area_m2) from geo_afs b where  b.cod_apmp = a.id and b.projeto = a.projeto), 0) afs_area,
                  nvl( (select sum(b.area_m2) from geo_rppn b where b.cod_apmp = a.id and b.projeto = a.projeto), 0) rppn_area,
                  nvl( (select sum(b.area_m2) from geo_arl b where  b.cod_apmp = a.id and b.projeto = a.projeto), 0) arl_area,
                  trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.projeto = a.projeto and b.situacao='PRESERV'), 0), 4) arl_area_preservada,
                  trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.projeto = a.projeto and b.situacao='REC'), 0), 4) arl_area_rec,
                  trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.projeto = a.projeto and b.situacao='USO'), 0), 4) arl_area_uso,
                  trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.projeto = a.projeto and b.situacao='D'), 0), 4) arl_area_desconhecido,
                  nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='APP_APMP'), 0) app_area_apmp,
                  nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='APP_AVN'), 0) app_area_preservada,                  
                  trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='APP_AA_REC'), 0), 4) app_aa_rec_area,    
                  trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='APP_AA_USO'), 0), 4) app_aa_uso_area,    
                  nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='APP_ARL'), 0) app_area_arl,
                  nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.projeto = a.projeto and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area,
				  nvl( (select sum(b.area_m2) from tmp_car_areas_calculadas b where b.cod_apmp = a.id and b.projeto = :projetoCarId and b.tipo='CAR_APP_AA_USO'), 0) car_app_aa_uso
                from geo_apmp a where a.projeto=:projetoDomId and ( a.tipo = 'M' or  a.tipo = 'P')  order by matricula_nome");

				comando.AdicionarParametroEntrada("projetoCarId", projetoCarId, DbType.Int32);
				comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

				reader = bancoGeo.ExecutarReader(comando);

				while (reader.Read())
				{
					#region Adicionar Areas
					ItemQuadroDeArea item = null;
					categoria = new CategoriaQuadroDeArea();
					categoria.Nome = Convert.ToString(reader["matricula_nome"]);
					categoria.Itens = new List<ItemQuadroDeArea>();

					item = new ItemQuadroDeArea();
					item.Nome = "APMP";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["matricula_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "Rocha";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rocha_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AVN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio inicial";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_inicial"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio médio";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_medio"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio avançado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_avancado"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_desconhecido"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AA";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em recuperação";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_rec_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em uso";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_uso_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_d_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AFS";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["afs_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "RPPN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rppn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "ARL";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area"]));
					categoria.Itens.Add(item);

					double appNaoCaracterizada = 0;
					item = new ItemQuadroDeArea();
					item.Nome = String.Format("APP {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_apmp"]));
					appNaoCaracterizada = Convert.ToDouble(reader["app_area_apmp"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);


					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Preservada {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_preservada"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_area_preservada"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Em recuperação {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_rec_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_rec_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-A recuperar {{{0}}}", SituacaoTextoCAR);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["car_app_aa_uso"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["car_app_aa_uso"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Em uso {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_uso_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_uso_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Não caracterizado {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", appNaoCaracterizada);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Em reserva legal {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_arl"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Massa Dagua {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["massa_dagua_area"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item);
					#endregion

					lista.Add(categoria);
				} 
				#endregion
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

		public List<CategoriaQuadroDeArea> ListarQuadroAreasCARFinalizado(int projetoCarId)
		{
			Comando comando = null;
			List<CategoriaQuadroDeArea> lista = new List<CategoriaQuadroDeArea>();
			IDataReader reader = null;
			IDataReader readerQtd = null;
			
			string SituacaoTextoDom = "Não processada";
			string SituacaoTextoCAR = "Finalizada";
			int projetoDomId = 0;

			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuario"];
				string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];
				BancoDeDados bancoGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				#region Obter Projeto Dominialidade Id

				comando = bancoGeo.GetComandoSql(String.Format(@"select g.id from {0}.crt_projeto_geo g where g.caracterizacao = 1 and g.empreendimento in (
					select cc.empreendimento 
					 from {0}.crt_cad_ambiental_rural cc 
					 where cc.projeto_geo_id = :projetoCarId
					union 
					select tc.empreendimento 
					from {0}.tmp_cad_ambiental_rural tc 
					where tc.projeto_geo_id = :projetoCarId )", schemaUsuario));
				comando.AdicionarParametroEntrada("projetoCarId", projetoCarId, DbType.Int32);

				projetoDomId = Convert.ToInt32(bancoGeo.ExecutarScalar(comando));

				#endregion

				#region Busca situação do projeto
				comando = bancoGeo.GetComandoSql(string.Format(@"select (case when is_valido_proces != 1 or is_valido_proces is null then 'Inválido' else 'Válido' end) situacao_texto, is_valido_proces  
                  from {0}.tab_navegador_projeto where projeto = :projetoDomId", schemaUsuarioGeo));

				comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

				try
				{
					reader = bancoGeo.ExecutarReader(comando);
					if (reader.Read())
					{
						SituacaoTextoDom = Convert.ToString(reader["situacao_texto"]);
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

				#region Total de Quadro de Áreas

				comando = bancoGeo.GetComandoSql(@"select * from (
					select 'ATP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  1 ORDEM from geo_atp where projeto=:projetoDomId
					union all select 'Área Construída' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  2 ORDEM from geo_aconstruida where projeto=:projetoDomId
					union all select 'AFD' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2, 0 is_processada, 0 is_subarea,  3 ORDEM from geo_afd where projeto=:projetoDomId
					union all select 'APMP'  feicao,  trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  4 ORDEM from geo_apmp where projeto=:projetoDomId
					union all select 'Rocha' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  5 ORDEM from geo_rocha where projeto=:projetoDomId
					union all select 'AVN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  6 ORDEM from geo_avn where projeto=:projetoDomId
					union all select ' -Estágio inicial' feicao,trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  7 ORDEM from geo_avn where estagio='I' and projeto=:projetoDomId 
					union all select ' -Estágio médio' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  8 ORDEM from geo_avn where estagio='M' and projeto=:projetoDomId 
					union all select ' -Estágio avançado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  9 ORDEM from geo_avn where estagio='A' and projeto=:projetoDomId 
					union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  10 ORDEM from geo_avn where estagio='D' and projeto=:projetoDomId 
					union all select 'AA' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  11 ORDEM from geo_aa where projeto=:projetoDomId
					union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  12 ORDEM from geo_aa where tipo='REC' and projeto=:projetoDomId
					union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  13 ORDEM from geo_aa where tipo='USO' and projeto=:projetoDomId 
					union all select ' -Não caracterizado' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 1 is_subarea,  14 ORDEM from geo_aa where tipo='D' and projeto=:projetoDomId
					union all select 'ARL' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  15 ORDEM from geo_arl where projeto=:projetoDomId
					union all select 'AFS' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  20 ORDEM from geo_afs where projeto=:projetoDomId
					union all select 'RPPN' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  0 is_processada, 0 is_subarea,  21 ORDEM from geo_rppn where projeto=:projetoDomId
					union all select 'APP' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  22 ORDEM from geo_areas_calculadas where tipo='APP_APMP' and projeto=:projetoDomId
					union all select ' -Preservada' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  23 ORDEM from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projetoDomId
					union all select ' -Em recuperação' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  24 ORDEM from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projetoDomId
					union all select ' -A recuperar' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  25 ORDEM from geo_car_areas_calculadas where tipo='CAR_APP_AA_USO' and projeto=:projetoCarId
					union all select ' -Em uso' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  26 ORDEM from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projetoDomId
					union all select ' -Não caracterizado' feicao, trunc(nvl(apmp.area_m2-avn.area_m2-rec.area_m2-uso.area_m2,0),4) area_m2,  1 is_processada, 1 is_subarea,  28 ORDEM from 
						(select  nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas apmp where tipo='APP_APMP' and projeto=:projetoDomId) apmp, 
						(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AVN' and projeto=:projetoDomId) avn,
						(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_REC' and projeto=:projetoDomId) rec,
						(select nvl( sum(area_m2), 0 ) area_m2 from geo_areas_calculadas where tipo='APP_AA_USO' and projeto=:projetoDomId) uso
					union all select ' -Em reserva legal' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 1 is_subarea,  27 ORDEM from geo_areas_calculadas where tipo='APP_ARL' and projeto=:projetoDomId
					union all select 'Massa dagua' feicao, trunc(nvl( sum(area_m2), 0 ),4) area_m2,  1 is_processada, 0 is_subarea,  29 ORDEM from geo_areas_calculadas where tipo='MASSA_DAGUA_APMP' and projeto=:projetoDomId
					) order by ordem");

				comando.AdicionarParametroEntrada("projetoCarId", projetoCarId, DbType.Int32);
				comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

				CategoriaQuadroDeArea categoria = new CategoriaQuadroDeArea();

				try
				{
					reader = bancoGeo.ExecutarReader(comando);

					categoria.Nome = "Total";
					categoria.Itens = new List<ItemQuadroDeArea>();
					while (reader.Read())
					{
						ItemQuadroDeArea item = new ItemQuadroDeArea();
						item.Nome = Convert.ToString(reader["feicao"]);

						item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["area_m2"]));
						item.IsProcessada = Convert.ToString(reader["is_processada"]) == "1";
						item.IsSubArea = Convert.ToString(reader["is_subarea"]) == "1";

						if (item.IsProcessada && item.Nome != " -A recuperar")
						{
							item.Nome = String.Format("{0} {{{1}}}", item.Nome, SituacaoTextoDom);
						}
						else if (item.Nome == " -A recuperar")
						{
							item.Nome = String.Format("{0} {{{1}}}", item.Nome, SituacaoTextoCAR);
						}

						categoria.Itens.Add(item);
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
				lista.Add(categoria);

				#endregion

				#region Areas por APMP

				comando = bancoGeo.GetComandoSql(@"select
					(case when a.tipo = 'M' then 'Matrícula: 'else 'Posse: 'end) ||upper(a.nome) matricula_nome,
					a.area_m2 matricula_area,
					nvl( (select sum(b.area_m2) from geo_rocha b where b.cod_apmp = a.id), 0) rocha_area,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id), 0) avn_area,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='I'), 0) avn_area_inicial,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='M'), 0) avn_area_medio,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='A'), 0) avn_area_avancado,
					nvl( (select sum(b.area_m2) from geo_avn b where b.cod_apmp = a.id and b.estagio='D'), 0) avn_area_desconhecido,
					trunc(nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id), 0),4) aa_area,
					trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='REC'), 0), 4) aa_rec_area,
					trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='USO'), 0), 4) aa_uso_area,
					trunc( nvl( (select sum(b.area_m2) from geo_aa b where b.cod_apmp = a.id and b.tipo='D'), 0), 4) aa_d_area,
					nvl( (select sum(b.area_m2) from geo_afs b where b.cod_apmp = a.id), 0) afs_area,
					nvl( (select sum(b.area_m2) from geo_rppn b where b.cod_apmp = a.id), 0) rppn_area,
					nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id ), 0) arl_area,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='PRESERV'), 0), 4) arl_area_preservada,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='REC'), 0), 4) arl_area_rec,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='USO'), 0), 4) arl_area_uso,
					trunc( nvl( (select sum(b.area_m2) from geo_arl b where b.cod_apmp = a.id and b.situacao='D'), 0), 4) arl_area_desconhecido,
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_APMP'), 0) app_area_apmp,
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AVN'), 0) app_area_preservada,                  
					trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_REC'), 0), 4) app_aa_rec_area,    
					trunc( nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_AA_USO'), 0), 4) app_aa_uso_area,    
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='APP_ARL'), 0) app_area_arl,
					nvl( (select sum(b.area_m2) from geo_areas_calculadas b where b.cod_apmp = a.id and b.tipo='MASSA_DAGUA_APMP'), 0) massa_dagua_area,
					nvl( (select sum(b.area_m2) from geo_car_areas_calculadas b where b.cod_apmp = a.id and b.projeto = :projetoCarId and b.tipo='CAR_APP_AA_USO'), 0) car_app_aa_uso
					from geo_apmp a where a.projeto=:projetoDomId and ( a.tipo = 'M' or  a.tipo = 'P')  order by matricula_nome");

				comando.AdicionarParametroEntrada("projetoCarId", projetoCarId, DbType.Int32);
				comando.AdicionarParametroEntrada("projetoDomId", projetoDomId, DbType.Int32);

				reader = bancoGeo.ExecutarReader(comando);

				while (reader.Read())
				{
					#region Adicionar Areas
					ItemQuadroDeArea item = null;
					categoria = new CategoriaQuadroDeArea();
					categoria.Nome = Convert.ToString(reader["matricula_nome"]);
					categoria.Itens = new List<ItemQuadroDeArea>();

					item = new ItemQuadroDeArea();
					item.Nome = "APMP";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["matricula_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "Rocha";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rocha_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AVN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio inicial";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_inicial"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio médio";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_medio"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Estágio avançado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_avancado"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["avn_area_desconhecido"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AA";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em recuperação";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_rec_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Em uso";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_uso_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "-Não caracterizado";
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["aa_d_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "AFS";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["afs_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "RPPN";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["rppn_area"]));
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = "ARL";
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["arl_area"]));
					categoria.Itens.Add(item);

					double appNaoCaracterizada = 0;
					item = new ItemQuadroDeArea();
					item.Nome = String.Format("APP {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_apmp"]));
					appNaoCaracterizada = Convert.ToDouble(reader["app_area_apmp"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);


					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Preservada {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_preservada"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_area_preservada"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Em recuperação {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_rec_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_rec_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-A recuperar {{{0}}}", SituacaoTextoCAR);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["car_app_aa_uso"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["car_app_aa_uso"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Em uso {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_aa_uso_area"]));
					appNaoCaracterizada -= Convert.ToDouble(reader["app_aa_uso_area"]);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Não caracterizado {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", appNaoCaracterizada);
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Em reserva legal {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = true;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["app_area_arl"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item);

					item = new ItemQuadroDeArea();
					item.Nome = String.Format("-Massa Dagua {{{0}}}", SituacaoTextoDom);
					item.IsSubArea = false;
					item.Area = String.Format(@"{0:N2}", Convert.ToDouble(reader["massa_dagua_area"]));
					item.IsProcessada = true;
					categoria.Itens.Add(item); 
					#endregion

					lista.Add(categoria);
				}

				#endregion
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
                
        #region Área de Abrangencia
        internal bool SalvarAreaAbrangencia(FeicaoAreaAbrangencia feicao)
        {

            IDataReader reader = null;
            Comando comando = null;
            
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

                if (feicao !=null && feicao.IdProjeto > 0)
                {
                    comando = bancoDeDadosGeo.GetComandoSql(@"select id from " + schemaUsuarioGeo + @".DES_AREA_ABRANGENCIA a where projeto = :projeto ");

                    comando.AdicionarParametroEntrada("projeto", feicao.IdProjeto, DbType.Int32);

                    reader = bancoDeDadosGeo.ExecutarReader(comando);

                    if (reader.Read())
                    {
                        feicao.ObjectId = Convert.ToInt32(reader["id"]);
                    }

                    if (feicao.ObjectId > 0)
                    {

						comando = bancoDeDadosGeo.GetComandoSql(@"update " + schemaUsuarioGeo + @".des_area_abrangencia set geometry = mdsys.sdo_geometry(2003,:srid, null, mdsys.sdo_elem_info_array(1,1003,1), mdsys.sdo_ordinate_array(:minx, :miny, :maxx, :miny, :maxx, :maxy, :minx, :maxy, :minx, :miny)) where id = :objectid");
                        
                        comando.AdicionarParametroEntrada("objectid", feicao.ObjectId, DbType.Int32);
						comando.AdicionarParametroEntrada("srid", ObterSridBase(), DbType.Int32);
                        comando.AdicionarParametroEntrada("minx", feicao.MinX, DbType.Double);
                        comando.AdicionarParametroEntrada("miny", feicao.MinY, DbType.Double);
                        comando.AdicionarParametroEntrada("maxx", feicao.MaxX, DbType.Double);
                        comando.AdicionarParametroEntrada("maxy", feicao.MaxY, DbType.Double);

                    }
                    else
                    {
                        comando = bancoDeDadosGeo.GetComandoSql(@"insert into " + schemaUsuarioGeo + @".des_area_abrangencia (id, projeto, geometry) values (seq_des_area_abrang.nextval, :projeto, 
                            mdsys.sdo_geometry(2003,:srid, null, mdsys.sdo_elem_info_array(1,1003,1), mdsys.sdo_ordinate_array(:minx, :miny, :maxx, :miny, :maxx, :maxy, :minx, :maxy, :minx, :miny))) "); 
                 
                        comando.AdicionarParametroEntrada("projeto", feicao.IdProjeto, DbType.Int32);
						comando.AdicionarParametroEntrada("srid", ObterSridBase(), DbType.Int32);
                        comando.AdicionarParametroEntrada("minx", feicao.MinX, DbType.Double);
                        comando.AdicionarParametroEntrada("miny", feicao.MinY, DbType.Double);
                        comando.AdicionarParametroEntrada("maxx", feicao.MaxX, DbType.Double);
                        comando.AdicionarParametroEntrada("maxy", feicao.MaxY, DbType.Double);

                    }

                    bancoDeDadosGeo.ExecutarNonQuery(comando);
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
            return true;

        }
        #endregion

        #region Invalida o Processamento
        internal static void InvalidarProcessamento(int idProjeto)
        {
            Comando comando = null;
            IDataReader reader = null;
          
            try
            {
                string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
                BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
                comando = bancoDeDadosGeo.GetComandoSql(@"update tab_navegador_projeto set is_valido_proces = 0 where projeto = :projeto");
                comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
                bancoDeDadosGeo.ExecutarNonQuery(comando);
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
        }
        #endregion
    }
}