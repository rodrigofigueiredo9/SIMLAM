using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Data
{
	class ConfiguracaoDocumentoFitossanitarioDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		#region DMLs

		internal void Salvar(ConfiguracaoDocumentoFitossanitario configuracao, BancoDeDados banco = null)
		{
			if (configuracao == null)
			{
				throw new Exception("Configuração é nula.");
			}

			if (configuracao.ID <= 0)
			{
				Criar(configuracao, banco);
			}
			else
			{
				Editar(configuracao, banco);
			}
		}

        internal void SalvarEdicaoIntervalo(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado, BancoDeDados banco = null)
        {
            if (configuracao == null)
            {
                throw new Exception("Configuração é nula.");
            }

            EditarIntervalo(configuracao, idEditado, banco);
        }

		internal void Criar(ConfiguracaoDocumentoFitossanitario configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into cnf_doc_fitossanitario (id, tid) values (seq_cnf_doc_fitossanitario.nextval, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				configuracao.ID = comando.ObterValorParametro<int>("id");

				foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into cnf_doc_fito_intervalo (id, tid, configuracao, tipo_documento, tipo, numero_inicial, numero_final) 
					values(seq_cnf_doc_fito_intervalo.nextval, :tid, :configuracao, :tipo_documento, :tipo, :numero_inicial, :numero_final)");

					comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_documento", item.TipoDocumentoID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_inicial", item.NumeroInicial, DbType.Int64);
					comando.AdicionarParametroEntrada("numero_final", item.NumeroFinal, DbType.Int64);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Editar(ConfiguracaoDocumentoFitossanitario configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update cnf_doc_fitossanitario t set t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", configuracao.ID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

                //Isso estava excluindo os intervalos que não estavam na lista. Como agora só são exibidos os intervalos do ano corrente, não queremos que os outros sejam apagados.
                //comando = bancoDeDados.CriarComando("delete from cnf_doc_fito_intervalo where configuracao = :configuracao ");
                //comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "id", DbType.Int32, configuracao.DocumentoFitossanitarioIntervalos.Select(x => x.ID).ToList());
                //comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
                //bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
				{
					if (item.ID > 0)
					{
						continue;
					}

					comando = bancoDeDados.CriarComando(@"
					insert into cnf_doc_fito_intervalo (id, tid, configuracao, tipo_documento, tipo, numero_inicial, numero_final) 
					values (seq_cnf_doc_fito_intervalo.nextval, :tid, :configuracao, :tipo_documento, :tipo, :numero_inicial, :numero_final)");

					comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_documento", item.TipoDocumentoID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_inicial", item.NumeroInicial, DbType.Int64);
					comando.AdicionarParametroEntrada("numero_final", item.NumeroFinal, DbType.Int64);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

        internal void EditarIntervalo(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update cnf_doc_fitossanitario t set t.tid = :tid where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", configuracao.ID, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                comando = bancoDeDados.CriarComando("delete from cnf_doc_fito_intervalo where configuracao = :configuracao ");
                comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "id", DbType.Int32, configuracao.DocumentoFitossanitarioIntervalos.Select(x => x.ID).ToList());
                comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
                {
                    if (item.ID != idEditado)
                    {
                        continue;
                    }

                    comando = bancoDeDados.CriarComando(@"
					update cnf_doc_fito_intervalo set numero_inicial=" + item.NumeroInicial + ", numero_final=" + item.NumeroFinal + " where id = " + item.ID);

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.atualizar, bancoDeDados);

                bancoDeDados.Commit();
            }
        }

        internal void Excluir(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update cnf_doc_fitossanitario t set t.tid = :tid where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", configuracao.ID, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                comando = bancoDeDados.CriarComando("delete from cnf_doc_fito_intervalo where configuracao = :configuracao ");
                comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "id", DbType.Int32, configuracao.DocumentoFitossanitarioIntervalos.Select(x => x.ID).ToList());
                comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
                {
                    if (item.ID != idEditado)
                    {
                        continue;
                    }

                    comando = bancoDeDados.CriarComando(@"
					delete from cnf_doc_fito_intervalo where id = " + item.ID);

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.atualizar, bancoDeDados);

                bancoDeDados.Commit();
            }
        }

		#endregion

		#region Obter/Filtrar

		internal ConfiguracaoDocumentoFitossanitario Obter(bool simplificado = false, BancoDeDados banco = null)
		{
			ConfiguracaoDocumentoFitossanitario retorno = new ConfiguracaoDocumentoFitossanitario();

			using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDedados.CriarComando(@"select id, tid from cnf_doc_fitossanitario", EsquemaBanco);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.ID = reader.GetValue<int>("id");
						retorno.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				if (retorno == null || retorno.ID < 1 || simplificado)
				{
					return retorno;
				}

				#region Intervalos

				comando = bancoDedados.CriarComando(@"select i.id, i.tid, i.tipo_documento, lt.texto tipo_documento_texto, i.tipo, i.numero_inicial, i.numero_final 
				from cnf_doc_fito_intervalo i, lov_doc_fitossanitarios_tipo lt where lt.id = i.tipo_documento and i.configuracao = :configuracao", EsquemaBanco);

				comando.AdicionarParametroEntrada("configuracao", retorno.ID, DbType.Int32);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					DocumentoFitossanitario item = null;

					while (reader.Read())
					{
						item = new DocumentoFitossanitario();
						item.ID = reader.GetValue<int>("id");
						item.TID = reader.GetValue<string>("tid");
						item.TipoDocumentoID = reader.GetValue<int>("tipo_documento");
						item.TipoDocumentoTexto = reader.GetValue<string>("tipo_documento_texto");
						item.Tipo = reader.GetValue<int>("tipo");
						item.NumeroInicial = reader.GetValue<long>("numero_inicial");
						item.NumeroFinal = reader.GetValue<long>("numero_final");
						retorno.DocumentoFitossanitarioIntervalos.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

        internal ConfiguracaoDocumentoFitossanitario ObterPorAno(int ano, bool simplificado = false, BancoDeDados banco = null)
        {
            ConfiguracaoDocumentoFitossanitario retorno = new ConfiguracaoDocumentoFitossanitario();

            using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDedados.CriarComando(@"select id, tid from cnf_doc_fitossanitario", EsquemaBanco);

                using (IDataReader reader = bancoDedados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        retorno.ID = reader.GetValue<int>("id");
                        retorno.Tid = reader.GetValue<string>("tid");
                    }

                    reader.Close();
                }

                if (retorno == null || retorno.ID < 1 || simplificado)
                {
                    return retorno;
                }

                #region Intervalos

                string anoStr = ano.ToString().Substring(2, 2);

                comando = bancoDedados.CriarComando(@"select i.id, i.tid, i.tipo_documento, lt.texto tipo_documento_texto, i.tipo, i.numero_inicial, i.numero_final 
				                                      from cnf_doc_fito_intervalo i, lov_doc_fitossanitarios_tipo lt
                                                      where lt.id = i.tipo_documento
                                                            and i.configuracao = :configuracao
                                                            and substr(i.NUMERO_INICIAL, 3, 2) = " + anoStr +    //to_char(sysdate, 'YY')
                                                      " order by i.tipo_documento, i.numero_inicial", EsquemaBanco);

                comando.AdicionarParametroEntrada("configuracao", retorno.ID, DbType.Int32);

                using (IDataReader reader = bancoDedados.ExecutarReader(comando))
                {
                    DocumentoFitossanitario item = null;

                    while (reader.Read())
                    {
                        item = new DocumentoFitossanitario();
                        item.ID = reader.GetValue<int>("id");
                        item.TID = reader.GetValue<string>("tid");
                        item.TipoDocumentoID = reader.GetValue<int>("tipo_documento");
                        item.TipoDocumentoTexto = reader.GetValue<string>("tipo_documento_texto");
                        item.Tipo = reader.GetValue<int>("tipo");
                        item.NumeroInicial = reader.GetValue<long>("numero_inicial");
                        item.NumeroFinal = reader.GetValue<long>("numero_final");
                        retorno.DocumentoFitossanitarioIntervalos.Add(item);
                    }

                    reader.Close();
                }

                #endregion
            }

            return retorno;
        }

        internal List<long> LiberadosIntervalo(int tipo, long inicio, long fim, BancoDeDados banco = null)
        {
            List<long> retorno = new List<long>();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando;

                if (tipo == (int)eDocumentoFitossanitarioTipo.CFO || tipo == (int)eDocumentoFitossanitarioTipo.CFOC)
                {
                    comando = bancoDeDados.CriarComando(@"
				              select t.*
                              from tab_numero_cfo_cfoc t, hst_liberacao_cfo_cfoc h 
				              where h.liberacao_id = t.liberacao
                                    and t.numero >= " + inicio +
                                    " and t.numero <= " + fim +
                                    " and t.tipo_documento = " + tipo);

                    comando.DbCommand.CommandText += " order by t.numero";
                }
                else //PTV
                {
                    comando = bancoDeDados.CriarComando(@"
				              select t.NUMERO
                              from tab_ptv t
                              where t.NUMERO >= " + inicio +
                                    " and t.NUMERO <= " + fim +
                              " order by t.NUMERO");
                }

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    long numero;
                    while (reader.Read())
                    {
                        numero = reader.GetValue<long>("numero");
                        retorno.Add(numero);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

        internal Resultados<DocumentoFitossanitario> Filtrar(Filtro<DocumentoFitossanitarioListarFiltros> filtros, BancoDeDados banco = null)
        {
            Resultados<DocumentoFitossanitario> retorno = new Resultados<DocumentoFitossanitario>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                string comandtxt = string.Empty;
                Comando comando = bancoDeDados.CriarComando("");

                #region Adicionando Filtros

                //comandtxt += comando.FiltroAndLike("p.texto", "texto", filtros.Dados.Texto, true, true);
                //comandtxt += comando.FiltroAndLike("i.TIPO_DOCUMENTO", "TipoDocumento", filtros.Dados.TipoDocumentoID, true, true);
                //comandtxt += comando.FiltroAndLike("i.TIPO", "TipoNumeracao", filtros.Dados.TipoNumeracaoID, true, true);
                //comandtxt += comando.FiltroAndLike("i.NUMERO_INICIAL", "Numero_Inicial", filtros.Dados.Ano, true, true);

//                comando = bancoDedados.CriarComando(@"select i.id, i.tid, i.tipo_documento, lt.texto tipo_documento_texto, i.tipo, i.numero_inicial, i.numero_final 
//				                                      from cnf_doc_fito_intervalo i, lov_doc_fitossanitarios_tipo lt
//                                                      where lt.id = i.tipo_documento
//                                                            and i.configuracao = :configuracao
//                                                            and substr(i.NUMERO_INICIAL, 3, 2) = " + anoStr +    //to_char(sysdate, 'YY')
//                                                      " order by i.tipo_documento, i.numero_inicial", EsquemaBanco);

//                comando.AdicionarParametroEntrada("configuracao", retorno.ID, DbType.Int32);


                List<String> ordenar = new List<String>() { "TipoDocumento", "Numero_Inicial" };
                List<String> colunas = new List<String>() { "TipoDocumento", "Numero_Inicial" };

                #endregion

                if (!string.IsNullOrEmpty(filtros.Dados.TipoNumeracaoID)
                    && !string.IsNullOrEmpty(filtros.Dados.TipoDocumentoID)
                    && !string.IsNullOrEmpty(filtros.Dados.Ano))
                {
//                    comandtxt += @" union all select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, max(trunc(metaphone.jaro_winkler(:filtro_fonetico,p.texto),5)) 
//								similaridade from tab_profissao p, lov_profissao_origem o where p.origem = o.id and p.texto_fonema like upper('%' || upper(metaphone.gerarCodigo(:filtro_fonetico)) || '%') 
//								and metaphone.jaro_winkler(:filtro_fonetico,p.texto) >= to_number(:limite_similaridade) group by p.id, p.texto, p.codigo, p.tid, p.origem, o.texto";
                    comandtxt += @"select td.texto TipoDocumento, tn.texto TipoNumeracao, i.NUMERO_INICIAL, i.NUMERO_FINAL
                                   from CNF_DOC_FITO_INTERVALO i, lov_doc_fitossanitarios_tipo td, LOV_DOC_FITOSSANI_TIPO_NUMERO tn
                                   where i.TIPO_DOCUMENTO = " + Convert.ToInt32(filtros.Dados.TipoDocumentoID)
                                         + " and i.TIPO = " + Convert.ToInt32(filtros.Dados.TipoNumeracaoID)
                                         + " and substr(i.NUMERO_INICIAL, 3, 2) = " + filtros.Dados.Ano.Substring(2, 2)
                                         + " and i.TIPO_DOCUMENTO = td.ID and i.TIPO = tn.ID";

                    //comando.AdicionarParametroEntrada("filtro_fonetico", filtros.Dados.Texto, DbType.String);
                    //comando.AdicionarParametroEntrada("limite_similaridade", ConfiguracaoSistema.LimiteSimilaridade, DbType.String);
                    colunas[0] = "TipoDocumento";
                    ordenar[0] = "TipoDocumento";
                }

                #region Executa a pesquisa nas tabelas
                //comando.DbCommand.CommandText = "select count(*) from (select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, 0 similaridade from tab_profissao p, lov_profissao_origem o where p.origem = o.id " + comandtxt + ")";
                comando.DbCommand.CommandText = "select count(*) from (" + comandtxt + ")";

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                comando.AdicionarParametroEntrada("menor", filtros.Menor);
                comando.AdicionarParametroEntrada("maior", filtros.Maior);

                //comandtxt = String.Format(@"select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, 1 similaridade 
                //from tab_profissao p, lov_profissao_origem o where p.origem = o.id {0} {1}", comandtxt, DaHelper.Ordenar(colunas, ordenar, !string.IsNullOrEmpty(filtros.Dados.Texto)));

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    DocumentoFitossanitario doc;

                    while (reader.Read())
                    {
                        doc = new DocumentoFitossanitario();
                        
                        doc.TipoDocumentoTexto = reader["TipoDocumento"].ToString();
                        doc.NumeroInicial = Convert.ToInt64(reader["NUMERO_INICIAL"]);
                        doc.NumeroFinal = Convert.ToInt64(reader["NUMERO_FINAL"]);

                        retorno.Itens.Add(doc);
                        //profissao.Id = Convert.ToInt32(reader["id"]);

                        //if (retorno.Itens.Exists(x => x.Id == profissao.Id))
                        //{
                        //    continue;
                        //}

                        //profissao.Tid = reader["tid"].ToString();
                        //profissao.Texto = reader["texto"].ToString();
                        //profissao.Codigo = reader["codigo"].ToString();
                        //profissao.OrigemId = Convert.ToInt32(reader["origem"]);

                        //retorno.Itens.Add(profissao);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

        internal Resultados<DocumentoFitossanitarioConsolidado> FiltrarConsolidado(Filtro<DocumentoFitossanitarioListarFiltros> filtros, BancoDeDados banco = null)
        {
            Resultados<DocumentoFitossanitarioConsolidado> retorno = new Resultados<DocumentoFitossanitarioConsolidado>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                string comandtxt = string.Empty;
                Comando comando = bancoDeDados.CriarComando("");

                if (!string.IsNullOrEmpty(filtros.Dados.AnoConsolidado))
                {
                    #region SQL

                    #region inicio
                    comandtxt += @"select 
                                   --Não Liberados: Bloco CFO, Bloco CFOC, Digital CFO, Digital CFOC
                                   (cad_B_CFO - lib_B_CFO) nlib_B_CFO,
                                   (cad_B_CFOC - lib_B_CFOC) nlib_B_CFOC,
                                   (cad_D_CFO - lib_D_CFO) nlib_D_CFO,
                                   (cad_D_CFOC - lib_D_CFOC) nlib_D_CFOC,
       
                                   --Liberados: Bloco CFO, Bloco CFOC, Digital CFO, Digital CFOC
                                   lib_B_CFO,
                                   lib_B_CFOC,
                                   lib_D_CFO,
                                   lib_D_CFOC,
       
                                   --Em elaboração: Bloco CFO, Bloco CFOC, Bloco PTV, Digital CFO, Digital CFOC, Digital PTV
                                   elab_B_CFO,
                                   elab_B_CFOC,
                                   elab_B_PTV,
                                   elab_D_CFO,
                                   elab_D_CFOC,
                                   elab_D_PTV,
       
       
                                   --Quantidade de números Utilizados: Bloco CFO, Bloco CFOC, Bloco PTV, Digital CFO, Digital CFOC, Digital PTV
                                   uti_B_CFO,
                                   uti_B_CFOC,
                                   uti_B_PTV,
                                   uti_D_CFO,
                                   uti_D_CFOC,
                                   uti_D_PTV,
       
                                   --Quantidade de números Cancelados: Bloco CFO, Bloco CFOC, Bloco PTV, Digital CFO, Digital CFOC, Digital PTV
                                   canc_B_CFO,
                                   canc_B_CFOC,
                                   canc_B_PTV,
                                   canc_D_CFO,
                                   canc_D_CFOC,
                                   canc_D_PTV,
       
                                   --Último número liberado: Digital CFO, Digital CFOC, Digital PTV
                                   ultnum_D_CFO,
                                   ultnum_D_CFOC,
                                   ultnum_D_PTV
                            from ";

                    #endregion inicio

                    #region IDAFCREDENCIADO.TAB_CFO

                    comandtxt += @"(
                        select sum( (case when c.tipo_numero = 1 and c.situacao = 1 then 1 else 0 end) ) elab_B_CFO,
                               sum( (case when c.tipo_numero = 2 and c.situacao = 1 then 1 else 0 end) ) elab_D_CFO,
                               sum( (case when c.tipo_numero = 1 and (c.situacao = 2 or c.situacao = 3) then 1 else 0 end) ) uti_B_CFO,
                               sum( (case when c.tipo_numero = 2 and (c.situacao = 2 or c.situacao = 3) then 1 else 0 end) ) uti_D_CFO,
                               sum( (case when c.tipo_numero = 1 and c.situacao = 4 then 1 else 0 end) ) canc_B_CFO,
                               sum( (case when c.tipo_numero = 2 and c.situacao = 4 then 1 else 0 end) ) canc_D_CFO
                        from IDAFCREDENCIADO.TAB_CFO c
                        where substr(c.NUMERO, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"),";

                    #endregion IDAFCREDENCIADO.TAB_CFO

                    #region IDAFCREDENCIADO.TAB_CFOC

                    comandtxt += @"(
                         select sum( (case when c.tipo_numero = 1 and c.situacao = 1 then 1 else 0 end) ) elab_B_CFOC,
                                sum( (case when c.tipo_numero = 2 and c.situacao = 1 then 1 else 0 end) ) elab_D_CFOC,
                                sum( (case when c.tipo_numero = 1 and (c.situacao = 2 or c.situacao = 3) then 1 else 0 end) ) uti_B_CFOC,
                                sum( (case when c.tipo_numero = 2 and (c.situacao = 2 or c.situacao = 3) then 1 else 0 end) ) uti_D_CFOC,
                                sum( (case when c.tipo_numero = 1 and c.situacao = 4 then 1 else 0 end) ) canc_B_CFOC,
                                sum( (case when c.tipo_numero = 2 and c.situacao = 4 then 1 else 0 end) ) canc_D_CFOC
                        from idafcredenciado.tab_cfoc c
                        where substr(c.numero, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"),";

                    #endregion IDAFCREDENCIADO.TAB_CFOC

                    #region TAB_PTV

                    comandtxt += @"(
                        select sum( (case when p.tipo_numero = 1 and p.situacao = 1 then 1 else 0 end) ) elab_B_PTV,
                               sum( (case when p.tipo_numero = 2 and p.situacao = 1 then 1 else 0 end) ) elab_D_PTV,
                               sum( (case when p.tipo_numero = 1 and (p.situacao = 2 or p.situacao = 4) then 1 else 0 end) ) uti_B_PTV,
                               sum( (case when p.tipo_numero = 2 and (p.situacao = 2 or p.situacao = 4) then 1 else 0 end) ) uti_D_PTV,
                               sum( (case when p.tipo_numero = 1 and p.situacao = 3 then 1 else 0 end) ) canc_B_PTV,
                               sum( (case when p.tipo_numero = 2 and p.situacao = 3 then 1 else 0 end) ) canc_D_PTV
                        from TAB_PTV p
                        where (p.numero is null and to_char(p.DATA_EMISSAO, 'YYYY') = " + filtros.Dados.AnoConsolidado + @")
                              or substr(p.NUMERO, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"),";

                    #endregion TAB_PTV

                    #region CNF_DOC_FITO_INTERVALO

                    comandtxt += @"(
                        select sum( (case when i.tipo = 1 and i.tipo_documento = 1 then (i.numero_final - i.numero_inicial + 1) else 0 end) ) cad_B_CFO,
                               sum( (case when i.tipo = 1 and i.tipo_documento = 2 then (i.numero_final - i.numero_inicial + 1) else 0 end) ) cad_B_CFOC,
                               sum( (case when i.tipo = 2 and i.tipo_documento = 1 then (i.numero_final - i.numero_inicial + 1) else 0 end) ) cad_D_CFO,
                               sum( (case when i.tipo = 2 and i.tipo_documento = 2 then (i.numero_final - i.numero_inicial + 1) else 0 end) ) cad_D_CFOC
                        from CNF_DOC_FITO_INTERVALO i
                        where substr(i.NUMERO_INICIAL, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"),";

                    #endregion CNF_DOC_FITO_INTERVALO

                    #region OUTRAS
                    comandtxt += @"
                              /*LIBERADOS, ÚLTIMOS*/
                              --Bloco de CFO      
                              (select count (*) lib_B_CFO,
                                      max(l.numero) ultnum_B_CFO
                              from TAB_NUMERO_CFO_CFOC l
                              where substr(l.NUMERO, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"
                                    and l.TIPO_DOCUMENTO = 1  --CFO
                                    and l.TIPO_NUMERO = 1 --BLOCO
                              ),
                              --Bloco de CFOC
                              (select count (*) lib_B_CFOC,
                                      max(l.numero) ultnum_B_CFOC
                              from TAB_NUMERO_CFO_CFOC l
                              where substr(l.NUMERO, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"
                                    and l.TIPO_DOCUMENTO = 2  --CFOC
                                    and l.TIPO_NUMERO = 1 --BLOCO
                              ),
                              --Digital de CFO
                              (select count (*) lib_D_CFO,
                                      max(l.numero) ultnum_D_CFO
                              from TAB_NUMERO_CFO_CFOC l
                              where substr(l.NUMERO, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"
                                    and l.TIPO_DOCUMENTO = 1  --CFO
                                    and l.TIPO_NUMERO = 2 --DIGITAL
                              ),
                              --Digital de CFOC
                              (select count (*) lib_D_CFOC,
                                      max(l.numero) ultnum_D_CFOC
                              from TAB_NUMERO_CFO_CFOC l
                              where substr(l.NUMERO, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"
                                    and l.TIPO_DOCUMENTO = 2  --CFOC
                                    and l.TIPO_NUMERO = 2 --DIGITAL
                              ),
                              --Digital de PTV
                              (select max(t.numero) ultnum_D_PTV
                              from tab_ptv t
                              where substr(t.numero, 3, 2) = " + filtros.Dados.AnoConsolidado.Substring(2, 2) + @"
                                   and t.tipo_numero = 2 --DIGITAL
                              )";
                    #endregion OUTRAS

                    #endregion SQL

                    #region Executa a pesquisa nas tabelas
                    //comando.DbCommand.CommandText = "select count(*) from (" + comandtxt + ")";

                    //retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                    //comando.AdicionarParametroEntrada("menor", filtros.Menor);
                    //comando.AdicionarParametroEntrada("maior", filtros.Maior);

                    //comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";
                    comando.DbCommand.CommandText = comandtxt;

                    #endregion

                    #region Preenche o resultado
                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        DocumentoFitossanitarioConsolidado doc;

                        reader.Read();

                        #region Não Liberados

                        doc = new DocumentoFitossanitarioConsolidado();

                        doc.Texto = "Não Liberados";
                        doc.QtdBlocoCFO = reader["nlib_B_CFO"].ToString();
                        doc.QtdBlocoCFOC = reader["nlib_B_CFOC"].ToString();
                        doc.QtdDigitalCFO = reader["nlib_D_CFO"].ToString();
                        doc.QtdDigitalCFOC = reader["nlib_D_CFOC"].ToString();

                        retorno.Itens.Add(doc);

                        #endregion

                        #region Liberados

                        doc = new DocumentoFitossanitarioConsolidado();

                        doc.Texto = "Liberados";
                        doc.QtdBlocoCFO = reader["lib_B_CFO"].ToString();
                        doc.QtdBlocoCFOC = reader["lib_B_CFOC"].ToString();
                        doc.QtdDigitalCFO = reader["lib_D_CFO"].ToString();
                        doc.QtdDigitalCFOC = reader["lib_D_CFOC"].ToString();

                        retorno.Itens.Add(doc);

                        #endregion

                        #region Em Elaboração

                        doc = new DocumentoFitossanitarioConsolidado();

                        doc.Texto = "Em Elaboração";
                        doc.QtdBlocoCFO = reader["elab_B_CFO"].ToString();
                        doc.QtdBlocoCFOC = reader["elab_B_CFOC"].ToString();
                        doc.QtdBlocoPTV = reader["elab_B_PTV"].ToString();
                        doc.QtdDigitalCFO = reader["elab_D_CFO"].ToString();
                        doc.QtdDigitalCFOC = reader["elab_D_CFOC"].ToString();
                        doc.QtdDigitalPTV = reader["elab_D_PTV"].ToString();

                        retorno.Itens.Add(doc);

                        #endregion

                        #region Utilizados

                        doc = new DocumentoFitossanitarioConsolidado();

                        doc.Texto = "Utilizados";
                        doc.QtdBlocoCFO = reader["uti_B_CFO"].ToString();
                        doc.QtdBlocoCFOC = reader["uti_B_CFOC"].ToString();
                        doc.QtdBlocoPTV = reader["uti_B_PTV"].ToString();
                        doc.QtdDigitalCFO = reader["uti_D_CFO"].ToString();
                        doc.QtdDigitalCFOC = reader["uti_D_CFOC"].ToString();
                        doc.QtdDigitalPTV = reader["uti_D_PTV"].ToString();

                        retorno.Itens.Add(doc);

                        #endregion

                        #region Cancelados

                        doc = new DocumentoFitossanitarioConsolidado();

                        doc.Texto = "Cancelados";
                        doc.QtdBlocoCFO = reader["canc_B_CFO"].ToString();
                        doc.QtdBlocoCFOC = reader["canc_B_CFOC"].ToString();
                        doc.QtdBlocoPTV = reader["canc_B_PTV"].ToString();
                        doc.QtdDigitalCFO = reader["canc_D_CFO"].ToString();
                        doc.QtdDigitalCFOC = reader["canc_D_CFOC"].ToString();
                        doc.QtdDigitalPTV = reader["canc_D_PTV"].ToString();

                        retorno.Itens.Add(doc);

                        #endregion

                        #region Ultimo Liberado

                        doc = new DocumentoFitossanitarioConsolidado();

                        doc.Texto = "Último nº liberado";
                        doc.QtdDigitalCFO = reader["ultnum_D_CFO"].ToString();
                        doc.QtdDigitalCFOC = reader["ultnum_D_CFOC"].ToString();
                        doc.QtdDigitalPTV = reader["ultnum_D_PTV"].ToString();

                        retorno.Itens.Add(doc);

                        #endregion

                        reader.Close();
                    }
                    #endregion Preenche o resultado
                }
            }

            return retorno;
        }

		#endregion

		#region Validaçoes

		internal bool ContidoIntervalo(DocumentoFitossanitario intervalo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from cnf_doc_fito_intervalo t 
				where ((:numero_inicial between t.numero_inicial and t.numero_final) or (t.numero_inicial between :numero_inicial and :numero_final))
				and tipo = :tipo and tipo_documento = :tipo_documento and id != :intervalo", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero_inicial", intervalo.NumeroInicial, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_final", intervalo.NumeroFinal, DbType.Int64);
				comando.AdicionarParametroEntrada("tipo", intervalo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_documento", intervalo.TipoDocumentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("intervalo", intervalo.ID, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion
	}
}