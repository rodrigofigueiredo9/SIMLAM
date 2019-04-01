using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using Tecnomapas.DesenhadorWS.Models.Entities;
using Tecnomapas.TecnoGeo.Geometria;
using Tecnomapas.TecnoGeo.Acessadores.OracleSpatial;
using Tecnomapas.TecnoGeo.Geografico;
using Tecnomapas.TecnoGeo.Ferramentas;
using Tecnomapas.Blocos.Data;
using Tecnomapas.TecnoGeo.Acessadores;
using Oracle.DataAccess.Client;

namespace Tecnomapas.DesenhadorWS.Models.DataAcess
{
	public class FeicaoDa
	{
		#region gera Id para o objectId da feição
		internal int GerarId(string sequencia)
		{
			Comando comando = null;
			int idGerado = 0;
			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				comando = bancoDeDados.GetComandoSql("select " + sequencia + ".nextval from dual");
				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno == null || !int.TryParse(retorno.ToString(), out idGerado))
					return -1;
			}
			finally
			{
				if (comando != null)
				{
					comando.Dispose();
				}
			}
			return idGerado;
		}
		#endregion

		#region Valida a geometria da tabela rascunho
		internal bool ValidarGeometria(int idGeometria, string tabelaRascunho, string primaryKey, int idLayerFeicao, int idProjeto, out string mensagem)
		{
			Comando comando = null;

			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				comando = bancoDeDados.GetComandoSql(@"select x.COLUMN_NAME from all_sdo_geom_metadata x where x.TABLE_NAME = :tabela and x.owner = :schema");
				comando.AdicionarParametroEntrada("tabela", DbType.String, 50, tabelaRascunho);
				comando.AdicionarParametroEntrada("schema", DbType.String, 50, schemaUsuario);
				object retorno = bancoDeDados.ExecutarScalar(comando);
				if (retorno == null)
					throw new Exception("Feição não encontrada");

				bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
				
				comando = bancoDeDados.GetComandoPlSql(String.Format(
				@"declare
					v_geo_val mdsys.sdo_geometry;
					v_val varchar2(4000);
					v_projetoId number:= :idProjeto;
					v_idLayerFeicao number:= :idLayerFeicao;
				begin 
					:resposta := 'TRUE';

					for i in (select t.{0} geometry from {1}.{2} t where t.{3} = :id and t.projeto = v_projetoId and t.feicao = v_idLayerFeicao) loop
						
						v_val :=  DesenhadorWS.ValidarGeometria(i.geometry, v_projetoId);
						
						if v_val <> 'TRUE' then
							if :resposta = 'TRUE' then
								:resposta := '';
							end if;
							:resposta := :resposta||' '||v_val;
						end if;
				
					end loop;
					
				end;", retorno, schemaUsuario, tabelaRascunho, primaryKey));

				comando.AdicionarParametroEntrada("id", idGeometria, DbType.Int32);
				comando.AdicionarParametroEntrada("idProjeto", idProjeto, DbType.Int32);
				comando.AdicionarParametroEntrada("idLayerFeicao", idLayerFeicao, DbType.Int32);
				
				
				comando.AdicionarParametroSaida("resposta", DbType.String, 4000);
				bancoDeDados.ExecutarNonQuery(comando);
				retorno = comando.ObterValorDoParametro("resposta");
				if (retorno == null)
					throw new Exception("Geometria não encontrada");
				mensagem = retorno.ToString();
			}
			finally
			{
				if (comando != null)
				{
					comando.Dispose();
				}
			}
			return mensagem == "TRUE";
		}
		#endregion

		#region Cadastra a geometria na tabela rascunho
		internal bool Cadastrar(FeicaoGeometria geoFeicao, string tabelaRascunho, int idLayerFeicao)
		{
			BancoDeDados bancoDeDados = null;
			if (geoFeicao == null)
				throw new ApplicationException("Referência nula do objeto");

			string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
			bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
			FonteFeicaoOracleSpatial destino = GetConexao(bancoDeDados);
			OperadorFeicaoOracleSpatial operador = null;
			try
			{
				destino.Abrir();

				ClasseFeicao classeDestino = destino.ObterClasseFeicao(tabelaRascunho);
				FeicaoAdapter adpt = new FeicaoAdapter(classeDestino);

				operador = (OperadorFeicaoOracleSpatial)destino.ObterOperadorFeicao(schemaUsuario + "." + tabelaRascunho);
				Tecnomapas.TecnoGeo.Geografico.Feicao feicao = classeDestino.CriarFeicao();

				feicao.Geometria = geoFeicao.RetornarGeometria();

				if (feicao.Geometria == null)
					throw new ApplicationException("Referência nula da geometria");

				foreach (AtributoFeicao a in geoFeicao.Atributos)
				{
					if (feicao.Atributos.IndiceDe(a.Nome.ToUpper()) < 0) continue;
					switch (a.Tipo)
					{
						case AtributoFeicao.TipoAtributo.Manual:
							feicao.Atributos[a.Nome.ToUpper()].Valor = a.Valor;
							break;
						case AtributoFeicao.TipoAtributo.Sequencia:
							adpt.Adaptadores[a.Nome.ToUpper()].Origem = TipoOrigem.Sequencia;
							adpt.Adaptadores[a.Nome.ToUpper()].Valor = a.Valor.ToString();
							break;
					}
				}
				feicao.Atributos["FEICAO"].Valor = idLayerFeicao;

				decimal srid = GetSrid();
				OperacaoEspacial operacao = new OperacaoEspacialTransformacao(new CampoGeometrico(), srid, srid);

				operador.Inserir(adpt.Transformar(feicao), operacao);
				
				operador.Fechar();
			}
			finally
			{				
				destino.Fechar();
			}
			return true;
		}
		#endregion

		#region Transferir a geometria da tabela rascunho para tabela
		internal void Transferir(string schema, string tabela, string rascunho, int objectid)
		{
			Comando comando = null;
			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				comando = bancoDeDados.GetComandoSql(@"begin " +
				" DesenhadorWS.InserirFeicao(:schema, :tabela, :objectid, :rascunho);" +
				" end;"); ;
				comando.AdicionarParametroEntrada("schema", schema, DbType.String);
				comando.AdicionarParametroEntrada("tabela", tabela, DbType.String);
				comando.AdicionarParametroEntrada("objectid", objectid, DbType.Int32);
				comando.AdicionarParametroEntrada("rascunho", rascunho, DbType.String);
				bancoDeDados.ExecutarNonQuery(comando);
			}
			finally
			{
				if (comando != null)
				{
					comando.Dispose();
				}
			}
		}
		#endregion

		#region Exclui a geometria e atributos
		internal bool Excluir(int objectId, string tabela, string primaryKey)
		{
			Comando comando = null;
			try
			{
				if(tabela.ToUpper().Contains("DES_PATIV") || tabela.ToUpper().Contains("DES_AATIV"))
				{
					BancoDeDados banco = BancoDeDadosFactory.CriarBancoDeDados("StringConexao");
					comando = banco.GetComandoSql($@"select c.id from crt_exp_florestal_exploracao c where exists (select 1 from crt_exp_florestal_geo g
						where g.exp_florestal_exploracao = c.id
						and exists (select 1 from idafgeo.{(tabela.ToUpper().Contains("DES_PATIV") ? "des_pativ" : "des_aativ")} d where d.codigo = c.identificacao
						and d.id = :objectid and exists(select 1 from idafgeo.{(tabela.ToUpper().Contains("DES_PATIV") ? "tmp_pativ" : "tmp_aativ")} t where t.projeto = d.projeto
						and id = g.{(tabela.ToUpper().Contains("DES_PATIV") ? "tmp_pativ_id" : "tmp_aativ_id")})))");

					comando.AdicionarParametroEntrada("objectId", objectId, DbType.Int32);
					var id = banco.ExecutarScalar(comando);

					comando = banco.GetComandoPlSql($@"begin
					delete from crt_exp_florestal_geo g where g.exp_florestal_exploracao = :id;
					delete from crt_exp_florestal_produto p where p.exp_florestal_exploracao = :id;
					delete from crt_exp_florestal_exploracao c where c.id = :id;

					delete from tab_titulo_exp_florestal t
						where exists (select 1 from crt_exploracao_florestal c
							where not exists (select 1 from crt_exp_florestal_exploracao ce where
							ce.exploracao_florestal = c.id)
						and c.id = t.exploracao_florestal);

					delete from crt_exploracao_florestal c
						where not exists (select 1 from crt_exp_florestal_exploracao ce where
						ce.exploracao_florestal = c.id);

					end;");
					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					banco.ExecutarNonQuery(comando);
				}

				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
				comando = bancoDeDados.GetComandoSql("delete from " + schemaUsuario + "." + tabela + " t where t." + primaryKey + " = :objectId");
				comando.AdicionarParametroEntrada("objectId", objectId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
			}
			finally
			{
				if (comando != null)
				{
					comando.Dispose();
				}
			}
			return true;
		}
		#endregion

		#region Atualiza a geometria na tabela rascunho
		internal bool AtualizarRascunho(FeicaoGeometria geoFeicao, int objectid, string tabelaRascunho, string primaryKey, int idLayerFeicao)
		{
			OracleConnection connection = null;
			OracleTransaction transaction = null;
			OracleCommand comando = null;
			FonteFeicaoOracleSpatial destino = null;

			try
			{
				if (geoFeicao == null)
				{
					throw new ApplicationException("Referência nula do objeto");
				}
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();

				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				destino = GetConexao(bancoDeDados);
				destino.Abrir();

				connection = destino.Conexao;
				if (connection != null)
					transaction = connection.BeginTransaction();

				comando = new OracleCommand("delete from " + tabelaRascunho + " t where t." + primaryKey + " = :objectid and t.feicao = :feicao ", connection);
				comando.Parameters.Add("objectid", OracleDbType.Int32);
				comando.Parameters["objectid"].Value = objectid;
				comando.Parameters.Add("feicao", OracleDbType.Int32);
				comando.Parameters["feicao"].Value = idLayerFeicao;
				comando.ExecuteNonQuery();

				ClasseFeicao classeDestino = destino.ObterClasseFeicao(tabelaRascunho);
				FeicaoAdapter adpt = new FeicaoAdapter(classeDestino);
				OperadorFeicaoOracleSpatial operador = (OperadorFeicaoOracleSpatial)destino.ObterOperadorFeicao(tabelaRascunho);
				TecnoGeo.Geografico.Feicao feicao = classeDestino.CriarFeicao();
				decimal srid = GetSrid();
				OperacaoEspacial operacao = new OperacaoEspacialTransformacao(new CampoGeometrico(), srid, srid);

				feicao.Geometria = geoFeicao.RetornarGeometria();

				if (feicao.Geometria == null)
				{
					throw new ApplicationException("Referência nula da geometria");
				}

				foreach (AtributoFeicao a in geoFeicao.Atributos)
				{
					if (feicao.Atributos.IndiceDe(a.Nome.ToUpper()) < 0) continue;
					switch (a.Tipo)
					{
						case AtributoFeicao.TipoAtributo.Manual:
							feicao.Atributos[a.Nome.ToUpper()].Valor = a.Valor;
							break;
						case AtributoFeicao.TipoAtributo.Sequencia:
							adpt.Adaptadores[a.Nome.ToUpper()].Origem = TipoOrigem.Sequencia;
							adpt.Adaptadores[a.Nome.ToUpper()].Valor = a.Valor.ToString();
							break;
					}
				}

				feicao.Atributos["FEICAO"].Valor = idLayerFeicao;

				operador.Inserir(adpt.Transformar(feicao), operacao);
				transaction.Commit();
			}
			catch
			{
				if (transaction != null)
					transaction.Rollback();
				throw;
			}
			finally
			{
				if (transaction != null)
					transaction.Dispose();
				if (destino != null)
					destino.Fechar();
				if (connection != null)
				{
					connection.Close();
					connection.Dispose();
				}
			}
			return true;
		}
		#endregion

		#region Transferir a geometria da tabela rascunho para tabela
		internal void TransferirAtualizarGeometria(string tabela, string rascunho, int objectid)
		{
			Comando comando = null;
			IDbConnection connection = null;

			try
			{
				string schemaUsuario = ConfigurationManager.AppSettings["SchemaUsuarioGeo"];

				BancoDeDados bancoDeDados = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				connection = bancoDeDados.GetConexao();
				connection.Open();

				comando = bancoDeDados.GetComandoSql(@"begin " +
				" DesenhadorWS.AtualizarGeometriaFeicao(:schema, :tabela, :objectid, :rascunho);" +
				" end;");

				comando.AdicionarParametroEntrada("schema", schemaUsuario.ToUpper(), DbType.String);
				comando.AdicionarParametroEntrada("tabela", tabela.ToUpper(), DbType.String);
				comando.AdicionarParametroEntrada("objectid", objectid, DbType.Int32);
				comando.AdicionarParametroEntrada("rascunho", rascunho.ToUpper(), DbType.String);
				bancoDeDados.ExecutarNonQuery(comando);

			}
			finally
			{
				if (connection != null)
				{
					connection.Close();
					connection.Dispose();
				}
			}
		}
		#endregion

		#region Atualiza os atributos da tabela
		internal void AtualizarAtributos(List<AtributoFeicao> atributos, string schema, string tabela, int id, string colunaPK)
		{
			Comando comando = null;
			IDataReader reader = null;
			try
			{
				BancoDeDados banco = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

				string sqlQuery = @"select t.column_name coluna_nome,  (case t.char_length when 0 then t.data_type else t.data_type end) tipo, nvl(t.char_length,0) tamanho from 
            all_tab_cols t where t.owner=upper('" + schema.ToUpper() + @"') and t.table_name = upper('" + tabela + @"') and t.column_name not like '%$%' and t.data_type<>'BLOB'
            and t.column_name not like upper('geometry') and t.column_name not like upper('" + colunaPK + @"')";

				comando = banco.GetComandoSql(sqlQuery);
				List<ColunaLayerFeicao> colunas = new List<ColunaLayerFeicao>();

				reader = banco.ExecutarReader(comando);

				while (reader.Read())
				{
					ColunaLayerFeicao coluna = new ColunaLayerFeicao();
					coluna.Coluna = Convert.ToString(reader["coluna_nome"]);
					coluna.Tamanho = Convert.ToInt32(reader["tamanho"]);
					coluna.Alias = Convert.ToString(reader["tipo"]);
					colunas.Add(coluna);
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
				int qtdColunas = colunas.Count;
				string valores = string.Empty;
				comando = banco.GetComandoSql(" ");
				for (int j = 0; j < qtdColunas; j++)
				{
					for (int i = 0; i < atributos.Count; i++)
					{
						if (Convert.ToString(atributos[i].Nome).ToUpper() == colunas[j].Coluna.ToUpper() && Convert.ToString(atributos[i].Valor) != null)
						{
							valores += colunas[j].Coluna + " = :coluna" + j.ToString()+" , ";
							if (colunas[j].Alias.ToUpper() == "NUMBER")
							{
								comando.AdicionarParametroEntrada("coluna" + j.ToString(), atributos[i].Valor, DbType.Decimal);
							}
							else
							{
								comando.AdicionarParametroEntrada("coluna" + j.ToString(), atributos[i].Valor, DbType.String);
							}
						}
					}
				}

				if (valores != string.Empty)
				{
					valores = valores.Substring(0, (valores.Length - 2));
					comando.DBCommand.CommandText += "update " + tabela + " set " + valores + " where " + colunaPK + " = :id";
					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					banco.ExecutarNonQuery(comando);
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
		}
		#endregion

		#region Importar Feicoes do Projeto Processado ou Finalizado
		internal Retorno ImportarFeicoes(int idNavegador, int idProjeto, bool isFinalizadas)
		{
			Comando comando = null;

			IDataReader reader = null;
			Projeto projeto = new Projeto();
			projeto.TipoNavegador = 1;
			projeto.Id = idProjeto;
			try
			{
				string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
				BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");
				int fila_tipo = 0;
				switch (idNavegador)
				{
					case 1: //Dominialidade
						if (isFinalizadas)
						{
							comando = bancoDeDadosGeo.GetComandoSql(@"select count(*) quantidade from geo_atp where projeto = :projeto ");
						}
						else
						{
							comando = bancoDeDadosGeo.GetComandoSql(@"select count(*) quantidade from tmp_atp where projeto = :projeto ");
						}
						break;
					case 2: //Atividade
						if (isFinalizadas)
						{

							comando = bancoDeDadosGeo.GetComandoSql(@"select sum(quantidade) quantidade from (
                                select count(*) quantidade from geo_pativ where projeto = :projeto
                                union all select count(*) quantidade from geo_aativ where projeto = :projeto
                                union all select count(*) quantidade from geo_aiativ where projeto = :projeto 
                                union all select count(*) quantidade from geo_lativ where projeto = :projeto )  ");
						}
						else
						{
							comando = bancoDeDadosGeo.GetComandoSql(@"select sum(quantidade) quantidade from (
                                select count(*) quantidade from tmp_pativ where projeto = :projeto
                                union all select count(*) quantidade from tmp_aativ where projeto = :projeto
                                union all select count(*) quantidade from tmp_aiativ where projeto = :projeto 
                                union all select count(*) quantidade from tmp_lativ where projeto = :projeto )  ");
						}
						break;
					case 3: //Fiscalização
						comando = bancoDeDadosGeo.GetComandoSql(@"select sum(quantidade) quantidade from (
                                select count(*) quantidade from tmp_fiscal_area where projeto = :projeto
                                union all select count(*) quantidade from tmp_fiscal_ponto where projeto = :projeto 
                                union all select count(*) quantidade from tmp_fiscal_linha where projeto = :projeto )");
						break;
				}

				comando.AdicionarParametroEntrada("projeto", idProjeto, DbType.Int32);
				reader = bancoDeDadosGeo.ExecutarReader(comando);
				int quantidade = 0;
				if (reader.Read())
				{
					quantidade = Convert.ToInt32(reader["quantidade"]);
				}
				if (quantidade == 0)
				{
					return new Retorno(false, "Nenhum projeto geográfico encontrado.");
				}

				if (reader != null)
				{
					reader.Close();
					reader.Dispose();
				}

				comando = bancoDeDadosGeo.GetComandoSql(@"select fila_tipo from tab_navegador a where a.id = :navegador ");
				comando.AdicionarParametroEntrada("navegador", idNavegador, DbType.Int32);
				reader = bancoDeDadosGeo.ExecutarReader(comando);
				if (reader.Read())
				{
					fila_tipo = Convert.ToInt32(reader["fila_tipo"]);
				}

				if (reader != null)
				{
					reader.Close();
					reader.Dispose();
				}

				#region Importa Feições
				if (fila_tipo > 0)
				{
					if (isFinalizadas)
					{
						comando = bancoDeDadosGeo.GetComandoSql(@"begin " +
							" Operacoesprocessamentogeo.ImportarParaDesenhFinalizada(" + idProjeto + "," + fila_tipo + " ); " +
							" end;");

					}
					else
					{
						comando = bancoDeDadosGeo.GetComandoSql(@"begin " +
						   " Operacoesprocessamentogeo.ImportarParaDesenhProcessada(" + idProjeto + "," + fila_tipo + " ); " +
						   " end;");
					}

					bancoDeDadosGeo.ExecutarNonQuery(comando);
				}
				#endregion
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
			return new Retorno(true);
		}
		#endregion

		#region Conexao e SRID
		private static FonteFeicaoOracleSpatial GetConexao(BancoDeDados bancoDeDados)
		{
			FonteFeicaoOracleSpatial fonte = new FonteFeicaoOracleSpatial();
			bool[] parameters = new bool[] { false, false, false };
			IDbConnection con = null;

			try
			{
				con = bancoDeDados.GetConexao();

				string strCon = con.ConnectionString;

				string[] conn_str = strCon.Split(';');
				for (int i = 0; i < conn_str.Length; i++)
				{
					string[] param = conn_str[i].Split('=');
					param[0] = param[0].ToLower();

					if (param[0].IndexOf("source") >= 0)
					{
						fonte.Fonte = param[1];
						parameters[0] = true;
					}
					else if (param[0].IndexOf("user") >= 0)
					{
						fonte.Usuario = param[1];
						parameters[1] = true;
					}
					else if (param[0].IndexOf("password") >= 0)
					{
						fonte.Senha = param[1];
						parameters[2] = true;
					}
				}
				if (!(parameters[0] && parameters[1] && parameters[2]))
					throw new Exception("Conexão não encontrada.");
			}
			finally
			{
				if (con != null)
				{
					con.Dispose();
				}
			}
			return fonte;
		}
		public static decimal GetSrid()
		{
			BancoDeDados bd = null;
			bd = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

			Comando comando = null;

			comando = bd.GetComandoSql("select c.valor SRID from tab_configuracao c where c.chave = 'SRID_BASE_REAL'");

			object retorno = bd.ExecutarScalar(comando);
			if (retorno == null)
				return 0;
			return Convert.ToDecimal(retorno);
		}
		#endregion

		public Dictionary<string, string> ObterParameters()
		{
			string schemaUsuarioGeo = ConfigurationManager.AppSettings["SchemaUsuarioGeo"].ToUpper();
			BancoDeDados bancoDeDadosGeo = BancoDeDadosFactory.CriarBancoDeDados("StringConexaoGeo");

			Comando com = bancoDeDadosGeo.GetComandoSql("select * from v$nls_parameters");

			IDataReader reader = bancoDeDadosGeo.ExecutarReader(com);

			Dictionary<string, string> dic = new Dictionary<string, string>();

			while (reader.Read())
			{
				dic.Add(reader[0].ToString(), reader[1].ToString());
			}

			reader.Close();

			return dic;
		}
	}

}