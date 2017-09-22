﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class MultaDa
	{
		#region Propriedade e Atributos

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		ConfigFiscalizacaoDa _configuracaoDa = new ConfigFiscalizacaoDa();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public String EsquemaBancoGeo { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }

		#endregion

        public MultaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

        public Multa Salvar(Multa multa, BancoDeDados banco = null)
        {
            if (multa == null)
            {
                throw new Exception("Multa é nulo.");
            }

            if (multa.Id <= 0)
            {
                multa = Criar(multa, banco);
            }
            else
            {
                multa = Editar(multa, banco);
            }

            return multa;
        }

        public Multa Criar(Multa multa, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    insert into {0}tab_fisc_multa (id,
                                                                   fiscalizacao,
                                                                   valor_multa,
                                                                   arquivo,
                                                                   justificar,
                                                                   codigo_receita,
                                                                   tid)
                                    values ({0}seq_tab_fisc_multa.nextval,
                                            :fiscalizacao,
                                            :valor_multa,
                                            :arquivo,
                                            :justificar,
                                            :codigo_receita,
                                            :tid)
                                    returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", multa.FiscalizacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("valor_multa", multa.ValorMulta, DbType.Decimal);
                comando.AdicionarParametroEntrada("justificar", multa.Justificativa, DbType.String);
                comando.AdicionarParametroEntrada("codigo_receita", multa.CodigoReceitaId, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);

                if (multa.Arquivo == null)
                {
                    comando.AdicionarParametroEntrada("arquivo", DBNull.Value, DbType.Int32);
                }
                else
                {
                    comando.AdicionarParametroEntrada("arquivo", multa.Arquivo.Id, DbType.Int32);
                }

                bancoDeDados.ExecutarNonQuery(comando);

                multa.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

                Historico.Gerar(multa.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(multa.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

                bancoDeDados.Commit();
            }
            return multa;
        }

        public Multa Editar(Multa multa, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fisc_multa t
                                    set t.fiscalizacao = :fiscalizacao,
                                        t.valor_multa = :valor_multa,
                                        t.arquivo = :arquivo,
                                        t.justificar = :justificar,
                                        t.codigo_receita = :codigo_receita,
                                        t.tid = :tid
                                    where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", multa.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("fiscalizacao", multa.FiscalizacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("valor_multa", multa.ValorMulta, DbType.Decimal);
                comando.AdicionarParametroEntrada("justificar", multa.Justificativa, DbType.String);
                comando.AdicionarParametroEntrada("codigo_receita", multa.CodigoReceitaId, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                if (multa.Arquivo == null)
                {
                    comando.AdicionarParametroEntrada("arquivo", DBNull.Value, DbType.Int32);
                }
                else
                {
                    comando.AdicionarParametroEntrada("arquivo", multa.Arquivo.Id, DbType.Int32);
                }

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(multa.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(multa.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

                bancoDeDados.Commit();
            }
            return multa;
        }

//        public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
//        {
//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                bancoDeDados.IniciarTransacao();

//                Comando comando = bancoDeDados.CriarComando("begin "
//                                                            + "delete {0}tab_fisc_infracao_campo t where t.infracao = (select id from {0}tab_fisc_infracao where fiscalizacao = :fiscalizacao); "
//                                                            + "delete {0}tab_fisc_infracao_pergunta t where t.infracao = (select id from {0}tab_fisc_infracao where fiscalizacao = :fiscalizacao); "
//                                                            + "delete {0}tab_fisc_infracao t where t.fiscalizacao = :fiscalizacao; "
//                                                        + "end;", EsquemaBanco);
//                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

//                bancoDeDados.ExecutarNonQuery(comando);

//                bancoDeDados.Commit();
//            }
//        }

		#endregion

		#region Obter / Filtrar

        public Multa Obter(int fiscalizacaoId, BancoDeDados banco = null)
        {
            Multa multa = new Multa();
            
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select tfm.id,
                                           f.situacao situacao_id,
                                           tfm.valor_multa,
                                           tfm.codigo_receita,
                                           tfm.justificar,
                                           tfm.arquivo,
                                           a.nome arquivo_nome
                                    from {0}tab_fisc_multa tfm,
                                         {0}tab_fiscalizacao f,
                                         {0}tab_arquivo a
                                    where tfm.arquivo = a.id(+)
                                          and tfm.fiscalizacao = :fiscalizacao
                                          and f.id = tfm.fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        multa = new Multa
                        {
                            Id = reader.GetValue<int>("id"),
                            ValorMulta = reader.GetValue<decimal>("valor_multa"),
                            CodigoReceitaId = reader.GetValue<int>("codigo_receita"),
                            FiscalizacaoSituacaoId = reader.GetValue<int>("situacao_id"),
                            Justificativa = reader.GetValue<string>("justificar")
                        };

                        multa.Arquivo = new Arquivo
                        {
                            Id = reader.GetValue<int>("arquivo"),
                            Nome = reader.GetValue<string>("arquivo_nome")
                        };
                    }
                    else
                    {
                        multa = null;
                    }
                    reader.Close();
                }
            }

            return multa;
        }

        public Multa ObterAntigo(int fiscalizacaoId, BancoDeDados banco = null)
        {
            Multa multa = new Multa();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select f.situacao situacao_id,
                                           tfi.valor_multa,
                                           tfi.codigo_receita,
                                           tfcf.justificar,
                                           tfi.arquivo,
                                           a.nome arquivo_nome
                                    from {0}tab_fisc_infracao tfi,
                                         {0}tab_fisc_consid_final tfcf,
                                         {0}tab_fiscalizacao f,
                                         {0}tab_arquivo a
                                    where tfi.arquivo = a.id(+)
                                          and tfi.fiscalizacao = :fiscalizacao
                                          and tfcf.fiscalizacao = :fiscalizacao
                                          and f.id = tfi.fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        multa = new Multa
                        {
                            ValorMulta = reader.GetValue<decimal>("valor_multa"),
                            CodigoReceitaId = reader.GetValue<int>("codigo_receita"),
                            FiscalizacaoSituacaoId = reader.GetValue<int>("situacao_id"),
                            Justificativa = reader.GetValue<string>("justificar")
                        };

                        multa.Arquivo = new Arquivo
                        {
                            Id = reader.GetValue<int>("arquivo"),
                            Nome = reader.GetValue<string>("arquivo_nome")
                        };
                    }
                    reader.Close();
                }
            }

            return multa;
        }

//        public Infracao ObterHistoricoPorFiscalizacao(int fiscalizacaoId, BancoDeDados banco = null)
//        {
//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                Comando comando = bancoDeDados.CriarComando(@"select f.id from tab_fisc_infracao f where f.fiscalizacao = :fiscalizacaoId");
//                comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

//                int infracaoId = 0;
//                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//                {
//                    if (reader.Read())
//                    {
//                        infracaoId = reader.GetValue<int>("id");
//                    }
//                    reader.Close();
//                }

//                return ObterHistorico(infracaoId, bancoDeDados);
//            }
//        }

//        public Infracao ObterHistorico(int id, BancoDeDados banco = null)
//        {
//            Infracao infracao = new Infracao();
//            InfracaoPergunta questionario = new InfracaoPergunta();

//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                #region Infração

//                Comando comando = bancoDeDados.CriarComando(@"select tfi.infracao_id id, f.situacao_id, tfi.classificacao_id classificacao, tfi.classificacao_texto,
//															tfi.tipo_id tipo, lt.texto tipo_texto, tfi.item_id item, cfi.texto item_texto, tfi.subitem_id subitem,
//															cfs.texto subitem_texto, tfi.infracao_autuada, tfi.gerado_sistema, tfi.valor_multa, tfi.codigo_receita_id codigo_receita,
//															tfi.numero_auto_infracao_bloco, tfi.descricao_infracao, tfi.data_lavratura_auto, tfi.serie_id serie, tfi.configuracao_id configuracao,
//															tfi.arquivo_id arquivo, a.nome arquivo_nome, tfi.configuracao_tid from hst_fisc_infracao tfi, hst_fiscalizacao f,
//															tab_arquivo a, lov_cnf_fisc_infracao_classif lc, cnf_fisc_infracao_tipo lt, cnf_fisc_infracao_item cfi,
//															cnf_fisc_infracao_subitem cfs where tfi.arquivo_id = a.id(+) and tfi.classificacao_id = lc.id(+) and tfi.tipo_id = lt.id(+)
//															and tfi.item_id = cfi.id(+) and tfi.subitem_id = cfs.id(+) and tfi.fiscalizacao_id_hst = f.id 
//															and tfi.id = (select max(t.id) id from hst_fisc_infracao t where t.infracao_id = :id)", EsquemaBanco);

//                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

//                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//                {
//                    if (reader.Read())
//                    {
//                        infracao = new Infracao
//                        {
//                            Id = reader.GetValue<int>("id"),
//                            ClassificacaoId = reader.GetValue<int>("classificacao"),
//                            ClassificacaoTexto = reader.GetValue<string>("classificacao_texto"),
//                            TipoId = reader.GetValue<int>("tipo"),
//                            TipoTexto = reader.GetValue<string>("tipo_texto"),
//                            ItemId = reader.GetValue<int>("item"),
//                            ItemTexto = reader.GetValue<string>("item_texto"),
//                            SubitemId = reader.GetValue<int>("subitem"),
//                            SubitemTexto = reader.GetValue<string>("subitem_texto"),
//                            SerieId = reader.GetValue<int>("serie"),
//                            ConfiguracaoId = reader.GetValue<int>("configuracao"),
//                            IsAutuada = reader.GetValue<bool>("infracao_autuada"),
//                            IsGeradaSistema = reader.GetValue<bool?>("gerado_sistema"),
//                            ValorMulta = reader.GetValue<string>("valor_multa"),
//                            CodigoReceitaId = reader.GetValue<int>("codigo_receita"),
//                            NumeroAutoInfracaoBloco = reader.GetValue<string>("numero_auto_infracao_bloco"),
//                            DescricaoInfracao = reader.GetValue<string>("descricao_infracao"),
//                            ConfiguracaoTid = reader.GetValue<string>("configuracao_tid"),
//                            FiscalizacaoSituacaoId = reader.GetValue<int>("situacao_id")
//                        };

//                        infracao.Arquivo = new Arquivo
//                        {
//                            Id = reader.GetValue<int>("arquivo"),
//                            Nome = reader.GetValue<string>("arquivo_nome")
//                        };

//                        if (!string.IsNullOrWhiteSpace(reader.GetValue<string>("data_lavratura_auto")))
//                        {
//                            infracao.DataLavraturaAuto.DataTexto = reader.GetValue<string>("data_lavratura_auto");
//                        }
//                    }
//                    reader.Close();
//                }

//                #endregion

//                #region Campos

//                comando = bancoDeDados.CriarComando(@"
//					select tfic.infracao_campo_id Id,
//						   tfic.campo_id          CampoId,
//						   tfic.texto             Texto,
//						   cfic.texto             CampoIdentificacao,
//						   cfic.unidade           CampoUnidade,
//						   lu.texto               CampoUnidadeTexto,
//						   cfic.Tipo              CampoTipo,
//						   lt.texto               CampoTipoTexto
//					  from {0}hst_fisc_infracao_campo        tfic,
//						   {0}lov_cnf_fisc_infracao_camp_tip lt,
//						   {0}lov_cnf_fisc_infracao_camp_uni lu,
//						   {0}cnf_fisc_infracao_campo        cfic
//					 where tfic.campo_id = cfic.id
//					   and cfic.tipo = lt.id(+)
//					   and cfic.unidade = lu.id(+)
//					   and tfic.infracao_id_hst = (select max(t.id) id from {0}hst_fisc_infracao t where t.infracao_id = :id)", EsquemaBanco);

//                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

//                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//                {
//                    infracao.Campos = new List<InfracaoCampo>();
//                    while (reader.Read())
//                    {
//                        infracao.Campos.Add(new InfracaoCampo
//                        {
//                            Id = reader.GetValue<int>("Id"),
//                            CampoId = reader.GetValue<int>("CampoId"),
//                            Tipo = reader.GetValue<int>("CampoTipo"),
//                            Identificacao = reader.GetValue<string>("CampoIdentificacao"),
//                            TipoTexto = reader.GetValue<string>("CampoTipoTexto"),
//                            Unidade = reader.GetValue<int>("CampoUnidade"),
//                            UnidadeTexto = reader.GetValue<string>("CampoUnidadeTexto"),
//                            Texto = reader.GetValue<string>("Texto")
//                        });
//                    }
//                    reader.Close();
//                }

//                #endregion

//                #region Questionário

//                comando = bancoDeDados.CriarComando(@"
//					select tfiq.id            Id,
//						   tfiq.pergunta_id   PerguntaId,
//						   tfiq.pergunta_tid  PerguntaTid,
//						   hp.texto           PerguntaIdentificacao,
//						   tfiq.resposta_id   RespostaId,
//						   tfiq.especificacao
//					  from {0}hst_fisc_infracao_pergunta tfiq, 
//						   {0}cnf_fisc_infracao_pergunta cfip,
//						   {0}hst_cnf_fisc_infracao_pergunta hp
//					 where tfiq.pergunta_id = cfip.id
//					   and hp.tid = tfiq.pergunta_tid
//					   and hp.pergunta_id = cfip.id
//					   and tfiq.infracao_id_hst = (select max(t.id) id from {0}hst_fisc_infracao t where t.infracao_id = :id) order by tfiq.pergunta_id", EsquemaBanco);

//                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

//                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//                {
//                    infracao.Perguntas = new List<InfracaoPergunta>();
//                    while (reader.Read())
//                    {
//                        int perguntaId = reader.GetValue<int>("PerguntaId");
//                        string perguntaTid = reader.GetValue<string>("PerguntaTid");

//                        infracao.Perguntas.Add(new InfracaoPergunta
//                        {
//                            Id = reader.GetValue<int>("Id"),
//                            PerguntaId = reader.GetValue<int>("PerguntaId"),
//                            PerguntaTid = perguntaTid,
//                            RespostaId = reader.GetValue<int>("RespostaId"),
//                            Identificacao = reader.GetValue<string>("PerguntaIdentificacao"),
//                            Especificacao = reader.GetValue<string>("Especificacao"),
//                            Respostas = _configuracaoDa.ObterRespostasHistorico(perguntaId, perguntaTid)
//                        });
//                    }
//                    reader.Close();
//                }

//                #endregion
//            }

//            return infracao;
//        }

//        public Infracao ObterConfig(int configuracaoId, BancoDeDados banco = null)
//        {
//            Infracao infracao = new Infracao();

//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                #region Infração

//                Comando comando = bancoDeDados.CriarComando(@"
//					select t.id            ConfiguracaoId,
//						   t.classificacao ClassificacaoId,
//						   l.texto         ClassificacaoTexto,
//						   t.tipo          TipoId,
//						   cit.texto       TipoTexto,
//						   t.item          ItemId,
//						   cii.texto       ItemTexto,
//						   t.tid           ConfiguracaoTid
//					  from {0}cnf_fisc_infracao             t,
//						   {0}cnf_fisc_infracao_tipo        cit,
//						   {0}cnf_fisc_infracao_item        cii,
//						   {0}lov_cnf_fisc_infracao_classif l
//					 where t.tipo = cit.id(+)
//					   and t.item = cii.id(+)
//					   and t.classificacao = l.id(+)
//					   and t.id = :configuracaoId", EsquemaBanco);

//                comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);

//                infracao = bancoDeDados.ObterEntity<Infracao>(comando);

//                #endregion

//                #region Campos

//                comando = bancoDeDados.CriarComando(@"
//					select tfic.campo CampoId,
//						   cfic.texto Identificacao,
//						   lt.texto   TipoTexto,
//						   lt.id      Tipo,
//						   lu.id      Unidade,
//						   lu.texto   UnidadeTexto,
//						   ''         Texto
//					  from {0}cnf_fisc_infr_cnf_campo       tfic,
//						   {0}lov_cnf_fisc_infracao_camp_tip lt,
//						   {0}lov_cnf_fisc_infracao_camp_uni lu,
//						   {0}cnf_fisc_infracao_campo        cfic
//					 where tfic.campo = cfic.id
//					   and cfic.tipo = lt.id(+)
//					   and cfic.unidade = lu.id(+)
//					   and tfic.configuracao = :configuracaoId", EsquemaBanco);
//                comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);

//                infracao.Campos = bancoDeDados.ObterEntityList<InfracaoCampo>(comando);

//                #endregion

//                #region Questionário

//                comando = bancoDeDados.CriarComando(@"
//					 select tfiq.pergunta PerguntaId,
//							cfip.tid      PerguntaTid,
//							cfip.texto    Identificacao,
//							0             RespostaId,
//							''            Especificacao
//					   from {0}cnf_fisc_infr_cnf_pergunta tfiq,
//							{0}cnf_fisc_infracao_pergunta cfip
//					  where tfiq.pergunta = cfip.id
//						and tfiq.configuracao = :configuracaoId order by tfiq.pergunta", EsquemaBanco);

//                comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);

//                infracao.Perguntas = bancoDeDados.ObterEntityList<InfracaoPergunta>(comando, (IDataReader reader, InfracaoPergunta item) => 
//                { 
//                    item.Respostas = _configuracaoDa.ObterRespostas(item.PerguntaId);
//                });

//                #endregion
//            }

//            return infracao;
//        }

        internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_infracao t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

                var retorno = bancoDeDados.ExecutarScalar(comando);

                return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
            }
        }

		#endregion

		#region Validação

//        internal bool ConfigAlterada(int configuracaoId, string tid, BancoDeDados banco = null)
//        {
//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                Comando comando = bancoDeDados.CriarComando(@"select count(1) count from cnf_fisc_infracao t where t.id = :configuracaoId and t.tid <> :tid", EsquemaBanco);
//                comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);				
//                comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);
//                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
//            }
//        }

//        internal bool PerguntaRespostaAlterada(int infracaoId, BancoDeDados banco = null)
//        {
//            Boolean alterou = false;

//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                Comando comando = bancoDeDados.CriarComando(@"select i.pergunta, i.resposta, i.pergunta_tid, i.resposta_tid 
//															from tab_fisc_infracao_pergunta i where i.infracao = :infracao", EsquemaBanco);

//                comando.AdicionarParametroEntrada("infracao", infracaoId, DbType.Int32);

//                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//                {
//                    while (reader.Read())
//                    {
//                        Int32 perguntaId = reader.GetValue<Int32>("pergunta");
//                        String perguntaTid = reader.GetValue<String>("pergunta_tid");

//                        Int32 respostaId = reader.GetValue<Int32>("resposta");
//                        String respostaTid = reader.GetValue<String>("resposta_tid");


//                        #region Pergunta

//                        comando = bancoDeDados.CriarComando(@"select p.tid pergunta_tid 
//															from cnf_fisc_infracao_pergunta p 
//															where p.id = :pergunta", EsquemaBanco);

//                        comando.AdicionarParametroEntrada("pergunta", perguntaId, DbType.Int32);

//                        using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
//                        {
//                            while (readerAux.Read()) 
//                            {
//                                String perguntaNovaTid = readerAux.GetValue<String>("pergunta_tid");
//                                alterou = perguntaNovaTid != perguntaTid;
								
//                                if (alterou) 
//                                {
//                                    return true;
//                                }
//                            }

//                            readerAux.Close();
//                        }

//                        #endregion

//                        #region Resposta

//                        comando = bancoDeDados.CriarComando(@"select p.tid resposta_tid from cnf_fisc_infracao_resposta p 
//															where p.id = :resposta", EsquemaBanco);

//                        comando.AdicionarParametroEntrada("resposta", respostaId, DbType.Int32);

//                        using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
//                        {
//                            if (readerAux2.Read())
//                            {
//                                String respostaNovaTid = readerAux2.GetValue<String>("resposta_tid");
//                                alterou = respostaNovaTid != respostaTid;

//                                if (alterou)
//                                {
//                                    return true;
//                                }
//                            }

//                            readerAux2.Close();
//                        }

//                        #endregion
//                    }

//                    reader.Close();
//                }

//                return alterou;
//            }
//        }

//        internal bool PerguntaRespostaAlterada(Infracao infracao, BancoDeDados banco = null)
//        {
//            Boolean alterou = false;

//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                Comando comando = null;

//                if (infracao.Perguntas == null || infracao.Perguntas.Count == 0)
//                {
//                    return alterou;
//                }

//                foreach (var pergunta in infracao.Perguntas)
//                {
//                    #region Pergunta

//                    comando = bancoDeDados.CriarComando(@"select p.tid pergunta_tid 
//															from cnf_fisc_infracao_pergunta p 
//															where p.id = :pergunta", EsquemaBanco);

//                    comando.AdicionarParametroEntrada("pergunta", pergunta.PerguntaId, DbType.Int32);

//                    using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
//                    {
//                        while (readerAux.Read())
//                        {
//                            String perguntaNovaTid = readerAux.GetValue<String>("pergunta_tid");
//                            alterou = perguntaNovaTid != pergunta.PerguntaTid;

//                            if (alterou)
//                            {
//                                return true;
//                            }
//                        }

//                        readerAux.Close();
//                    }

//                    #endregion


//                    #region Resposta

//                    comando = bancoDeDados.CriarComando(@"select p.tid resposta_tid from cnf_fisc_infracao_resposta p 
//															where p.id = :resposta", EsquemaBanco);

//                    comando.AdicionarParametroEntrada("resposta", pergunta.RespostaId, DbType.Int32);

//                    using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
//                    {
//                        if (readerAux2.Read())
//                        {
//                            String respostaNovaTid = readerAux2.GetValue<String>("resposta_tid");
//                            alterou = respostaNovaTid != pergunta.RespostaTid;

//                            if (alterou)
//                            {
//                                return true;
//                            }
//                        }

//                        readerAux2.Close();
//                    }

//                    #endregion

//                }

//                return alterou;
//            }
//        }

		#endregion
	}
}