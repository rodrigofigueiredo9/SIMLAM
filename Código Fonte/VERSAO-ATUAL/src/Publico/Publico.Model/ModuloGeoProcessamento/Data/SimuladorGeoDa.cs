using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Data
{
	class SimuladorGeoDa
	{
		#region Propriedades

		private ConfiguracaoSistema _configuracaoSistema = new ConfiguracaoSistema();

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		public String EsquemaBancoPublicoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioPublicoGeo); }
		}

		private string EsquemaBanco { get; set; }

		#endregion

		public SimuladorGeoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(SimuladorGeo simulador, BancoDeDados banco = null)
		{
			if (simulador == null)
			{
				throw new Exception("O Projeto geográfico é nulo.");
			}

			if (simulador.Id <= 0)
			{
				Criar(simulador, banco);
			}
			else
			{
				Editar(simulador, banco);
			}
		}

		internal int? Criar(SimuladorGeo simulador, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo p (id, situacao, mecanismo_elaboracao, cpf,
				menor_x, menor_y, maior_x, maior_y, tid) values ({0}seq_tmp_projeto_geo.nextval, 1, :mecanismo_elaboracao, :cpf,
				:menor_x, :menor_y, :maior_x, :maior_y, :tid) returning p.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("mecanismo_elaboracao", simulador.MecanismoElaboracaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 15, simulador.Cpf);
				comando.AdicionarParametroEntrada("menor_x", simulador.MenorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("menor_y", simulador.MenorY, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_x", simulador.MaiorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_y", simulador.MaiorY, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				simulador.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				bancoDeDados.Commit();

				return simulador.Id;
			}
		}

		internal void Editar(SimuladorGeo simulador, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo p set p.situacao = :situacao,
				mecanismo_elaboracao = :mecanismo_elaboracao, p.easting = :easting, p.norhting = :northing, 
				p.menor_x = :menor_x, p.menor_y = :menor_y, p.maior_x = :maior_x, p.maior_y = :maior_y where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", simulador.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", simulador.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo_elaboracao", simulador.MecanismoElaboracaoId, DbType.Int32);

				comando.AdicionarParametroEntrada("easting", simulador.EastingDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("northing", simulador.NorthingDecimal, DbType.Decimal);

				comando.AdicionarParametroEntrada("menor_x", simulador.MenorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("menor_y", simulador.MenorY, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_x", simulador.MaiorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_y", simulador.MaiorY, DbType.Decimal);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AtualizarArquivosImportar(SimuladorGeoArquivo arquivoEnviado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				update {0}tmp_projeto_geo_arquivos a set a.valido = 0, a.tid = :tid 
				where a.tipo > 3 and a.projeto = :projeto", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", arquivoEnviado.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"
				update {0}tmp_projeto_geo_arquivos a set a.valido = 1, a.arquivo = :arquivo, a.tid = :tid 
				where a.projeto = :projeto and a.tipo = :tipo", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", arquivoEnviado.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivoEnviado.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", arquivoEnviado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				if (comando.LinhasAfetadas == 0 && arquivoEnviado.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) values
					({0}seq_tmp_projeto_geo_arquivos.nextval, :projeto, 3, :arquivo, 1, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("projeto", arquivoEnviado.ProjetoId, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", arquivoEnviado.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}
			}
		}

		#endregion

		#region Ações de DML da base GEO

		internal void InserirFila(SimuladorGeoArquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {1}tab_fila f (id, projeto, tipo, mecanismo_elaboracao, etapa, situacao, data_fila)
				(select {1}seq_fila.nextval, t.id, :tipo, :mecanismo_elaboracao, :etapa, :situacao, sysdate from {0}tmp_projeto_geo t where t.id = :projeto)",
					EsquemaBanco, EsquemaBancoPublicoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo_elaboracao", arquivo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select f.id from {0}tab_fila f where f.projeto = :projeto and f.tipo = :tipo", EsquemaBancoPublicoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.Tipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					arquivo.IdRelacionamento = Convert.ToInt32(valor);
				}

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacaoFila(SimuladorGeoArquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("begin update {1}tmp_projeto_geo tt set tt.mecanismo_elaboracao = :mecanismo where tt.id = :projeto;" +
				"update {0}tab_fila t set t.etapa = :etapa, t.situacao = :situacao, t.data_fila = sysdate, t.data_inicio = null, t.data_fim = null, t.mecanismo_elaboracao = :mecanismo " +
				"where t.projeto = :projeto and t.tipo = :tipo returning t.id into :id; end;", EsquemaBancoPublicoGeo, EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo", arquivo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				arquivo.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal SimuladorGeo Obter(int id, BancoDeDados banco = null, bool simplificado = false, bool finalizado = false)
		{
			SimuladorGeo simulador = new SimuladorGeo();

			string tabela = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, ls.id situacao_id, ls.texto situacao_texto,
					lm.id mecanismo_elaboracao_id, lm.texto mecanismo_elaboracao_texto, g.easting, g.norhting northing,
					g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid 
					from {0}tmp_projeto_geo g, {0}lov_crt_projeto_geo_situacao ls, {0}lov_crt_projeto_geo_nivel ln, {0}lov_crt_projeto_geo_mecanismo lm         
					where g.situacao = ls.id 
					and g.nivel_precisao = ln.id(+) 
					and g.mecanismo_elaboracao = lm.id(+) 
					and g.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						simulador = new SimuladorGeo();
						simulador.Id = id;
						simulador.Tid = reader["tid"].ToString();

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							simulador.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							simulador.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["mecanismo_elaboracao_id"] != null && !Convert.IsDBNull(reader["mecanismo_elaboracao_id"]))
						{
							simulador.MecanismoElaboracaoId = Convert.ToInt32(reader["mecanismo_elaboracao_id"]);
							simulador.MecanismoElaboracaoTexto = reader["mecanismo_elaboracao_texto"].ToString();
						}

						if (reader["easting"] != null && !Convert.IsDBNull(reader["easting"]))
						{
							simulador.Easting = Convert.ToDecimal(reader["easting"]).ToString("N4");
						}

						if (reader["northing"] != null && !Convert.IsDBNull(reader["northing"]))
						{
							simulador.Northing = Convert.ToDecimal(reader["northing"]).ToString("N4");
						}

						if (reader["menor_x"] != null && !Convert.IsDBNull(reader["menor_x"]))
						{
							simulador.MenorX = Convert.ToDecimal(reader["menor_x"]);
						}

						if (reader["menor_y"] != null && !Convert.IsDBNull(reader["menor_y"]))
						{
							simulador.MenorY = Convert.ToDecimal(reader["menor_y"]);
						}

						if (reader["maior_x"] != null && !Convert.IsDBNull(reader["maior_x"]))
						{
							simulador.MaiorX = Convert.ToDecimal(reader["maior_x"]);
						}

						if (reader["maior_y"] != null && !Convert.IsDBNull(reader["maior_y"]))
						{
							simulador.MaiorY = Convert.ToDecimal(reader["maior_y"]);
						}
					}

					reader.Close();
				}

				if (simulador.Id <= 0 || simplificado)
				{
					return simulador;
				}

				#endregion
			}

			return simulador;
		}

		internal SimuladorGeo Obter(string cpf, BancoDeDados banco = null, bool simplificado = false, bool finalizado = false)
		{
			SimuladorGeo projeto = new SimuladorGeo();

			string tabela = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, ls.id situacao_id, ls.texto situacao_texto,
					lm.id mecanismo_elaboracao_id, lm.texto mecanismo_elaboracao_texto, g.easting, g.norhting northing,
					g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid 
					from {0}tmp_projeto_geo g, {0}lov_crt_projeto_geo_situacao ls, {0}lov_crt_projeto_geo_mecanismo lm 
					where g.situacao = ls.id 
					and g.mecanismo_elaboracao = lm.id(+) 
					and g.cpf = :cpf", EsquemaBanco);

				comando.AdicionarParametroEntrada("cpf", DbType.String, 20, cpf);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projeto = new SimuladorGeo();
						projeto.Id = Convert.ToInt32(reader["id"]);
						projeto.Tid = reader["tid"].ToString();
						projeto.Cpf = cpf;

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							projeto.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							projeto.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["mecanismo_elaboracao_id"] != null && !Convert.IsDBNull(reader["mecanismo_elaboracao_id"]))
						{
							projeto.MecanismoElaboracaoId = Convert.ToInt32(reader["mecanismo_elaboracao_id"]);
							projeto.MecanismoElaboracaoTexto = reader["mecanismo_elaboracao_texto"].ToString();
						}

						if (reader["easting"] != null && !Convert.IsDBNull(reader["easting"]))
						{
							projeto.Easting = Convert.ToDecimal(reader["easting"]).ToString("F4");
						}

						if (reader["northing"] != null && !Convert.IsDBNull(reader["northing"]))
						{
							projeto.Northing = Convert.ToDecimal(reader["northing"]).ToString("F4");
						}

						if (reader["menor_x"] != null && !Convert.IsDBNull(reader["menor_x"]))
						{
							projeto.MenorX = Convert.ToDecimal(reader["menor_x"]);
						}

						if (reader["menor_y"] != null && !Convert.IsDBNull(reader["menor_y"]))
						{
							projeto.MenorY = Convert.ToDecimal(reader["menor_y"]);
						}

						if (reader["maior_x"] != null && !Convert.IsDBNull(reader["maior_x"]))
						{
							projeto.MaiorX = Convert.ToDecimal(reader["maior_x"]);
						}

						if (reader["maior_y"] != null && !Convert.IsDBNull(reader["maior_y"]))
						{
							projeto.MaiorY = Convert.ToDecimal(reader["maior_y"]);
						}
					}

					reader.Close();
				}

				if (projeto.Id <= 0 || simplificado)
				{
					return projeto;
				}

				//Busca os arquivos
				if (projeto.Id > 0)
				{
					projeto.Arquivos = ObterArquivos(projeto.Id, banco: bancoDeDados, finalizado: finalizado);
				}

				#endregion
			}

			return projeto;
		}

		public List<SimuladorGeoArquivo> ObterArquivos(int projetoId, BancoDeDados banco = null, bool finalizado = false)
		{
			List<SimuladorGeoArquivo> arquivos = new List<SimuladorGeoArquivo>();

			string tabela = finalizado ? "crt_projeto_geo_arquivos" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos

				Comando comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, t.arquivo, t.valido, 
				lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabela + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
				{0}lov_crt_projeto_geo_sit_proce lcp where t.tipo = lc.id and t.projeto = tf.projeto(+) and t.tipo = tf.tipo(+) 
				and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.projeto = :projeto and t.valido = 1 and t.tipo <> 5 order by lc.id", EsquemaBanco, EsquemaBancoPublicoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						SimuladorGeoArquivo arq = new SimuladorGeoArquivo();

						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arq.IdRelacionamento = Convert.ToInt32(reader["id"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							arq.Nome = reader["tipo_texto"].ToString();
							arq.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							arq.Id = Convert.ToInt32(reader["arquivo"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							arq.Situacao = Convert.ToInt32(reader["situacao_id"]);
							arq.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["valido"] != null && !Convert.IsDBNull(reader["valido"]))
						{
							arq.isValido = Convert.ToBoolean(reader["valido"]);
						}

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}
			return arquivos;
		}

		internal void ObterSituacaoFila(SimuladorGeoArquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select lc.id, lc.texto, cpg.arquivo, t.tipo from {1}tab_fila t, {0}lov_crt_projeto_geo_sit_proce lc, {0}tmp_projeto_geo_arquivos cpg
				where t.situacao = lc.situacao(+) and t.etapa = lc.etapa(+) and t.projeto = cpg.projeto(+) and t.tipo = cpg.tipo(+) and t.id = :arquivo_id", EsquemaBanco, EsquemaBancoPublicoGeo);

				comando.AdicionarParametroEntrada("arquivo_id", arquivo.IdRelacionamento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arquivo.Situacao = Convert.ToInt32(reader["id"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							arquivo.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["texto"] != null && !Convert.IsDBNull(reader["texto"]))
						{
							arquivo.SituacaoTexto = reader["texto"].ToString();
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							arquivo.Id = Convert.ToInt32(reader["arquivo"]);
						}
					}
					reader.Close();
				}
			}
		}

		internal void ObterSituacao(SimuladorGeoArquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto from {0}lov_crt_projeto_geo_sit_proce t where t.etapa = :etapa and t.situacao = :situacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arquivo.Situacao = Convert.ToInt32(reader["id"]);
						}

						if (reader["texto"] != null && !Convert.IsDBNull(reader["texto"]))
						{
							arquivo.SituacaoTexto = reader["texto"].ToString();
						}
					}
					reader.Close();
				}
			}
		}

		#endregion

		#region Validações

		internal int ExisteArquivoFila(SimuladorGeoArquivo arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_fila t where t.projeto = :projeto and t.tipo = :tipo", EsquemaBancoPublicoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.Tipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal bool PontoForaMBR(string easting, string northing, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare
					resposta number(1) := 0;
					ordenadas sdo_ordinate_array;
				begin
					ordenadas := sdo_ordinate_array(:easting, :northing);

					if (ordenadas(1)<-3720618.6798069715 
					or ordenadas(2)<5916693.2686790377 
					or ordenadas(1)>1670563.45910363365
					or ordenadas(2)>10704234.897805285) 
					then
						resposta := 1;
					end if;

					select resposta into :saida from dual;
				end;", EsquemaBancoPublicoGeo);

				comando.AdicionarParametroEntrada("easting", easting, DbType.Double);
				comando.AdicionarParametroEntrada("northing", northing, DbType.Double);
				comando.AdicionarParametroSaida("saida", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				return Convert.ToInt32(comando.ObterValorParametro("saida")) > 0;
			}
		}

		#endregion

		internal void AtualizarGeoEmp(SimuladorGeo projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(
					@"declare " +
					  "v_srid number:=0; " +
					  "v_count number:=0; " +
					  "v_geo mdsys.sdo_geometry; " +
					"begin " +
					  "select to_number(c.valor) into v_srid from cnf_configuracao c where c.chave = 'SRID_BASE'; " +
					  "v_geo := mdsys.sdo_geometry(2001, v_srid, mdsys.sdo_point_type(:x, :y, null), null, null); " +
					  "select count(*) into v_count from geo_emp_localizacao l where l.projeto = :projeto; " +
					  "if v_count = 0 then  " +
						 "insert into {0}geo_emp_localizacao(id,projeto,geometry,tid) values ({0}seq_geo_emp_localizacao.nextval, :projeto, v_geo, :tid); " +
					  "else  " +
						 "update {0}geo_emp_localizacao g set g.geometry =v_geo, g.tid=:tid where g.projeto = :projeto; " +
					  "end if; " +
					"end;", EsquemaBancoPublicoGeo);

				comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("x", projeto.EastingDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("y", projeto.NorthingDecimal, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}
	}
}