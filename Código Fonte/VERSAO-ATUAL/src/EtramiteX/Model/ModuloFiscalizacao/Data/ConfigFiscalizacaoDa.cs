using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
    public class ConfigFiscalizacaoDa
    {
        #region Propriedade e Atributos

        Historico _historico = new Historico();
        Consulta _consulta = new Consulta();
        internal Historico Historico { get { return _historico; } }
        internal Consulta Consulta { get { return _consulta; } }
        private string EsquemaBanco { get; set; }

        #endregion

        public ConfigFiscalizacaoDa(string strBancoDeDados = null)
        {
            EsquemaBanco = string.Empty;
            if (!string.IsNullOrEmpty(strBancoDeDados))
            {
                EsquemaBanco = strBancoDeDados;
            }
        }

        #region Ações de DML

        public int Salvar(ConfigFiscalizacao configuracao, BancoDeDados banco = null)
        {
            if (configuracao == null)
            {
                throw new Exception("Configuração é nulo.");
            }

            if (configuracao.Id <= 0)
            {
                configuracao.Id = Criar(configuracao, banco);
            }
            else
            {
                Editar(configuracao, banco);
            }

            return configuracao.Id;
        }

        public int Criar(ConfigFiscalizacao configuracao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}cnf_fisc_infracao
				  (id, 
				   classificacao, 
				   tipo, 
				   item, 
				   tid)
				values
				  ({0}seq_cnf_fisc_infracao.nextval, 
				   :classificacao, 
				   :tipo, 
				   :item, 
				   :tid)
				  returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("classificacao", configuracao.ClassificacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tipo", configuracao.TipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("item", configuracao.ItemId, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                configuracao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

                #region Subitens

                foreach (var item in configuracao.Subitens)
                {
                    comando = bancoDeDados.CriarComando(@"
						insert into {0}cnf_fisc_infr_cnf_subitem
						  (id, 
						   configuracao, 
						   subitem, 
						   tid)
						values
						  ({0}seq_cnf_fisc_infrcnfsubitem.nextval, 
						   :configuracao, 
						   :subitem, 
						   :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("subitem", item.SubItemId, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Perguntas

                foreach (var item in configuracao.Perguntas)
                {
                    comando = bancoDeDados.CriarComando(@"
						insert into {0}cnf_fisc_infr_cnf_pergunta
						  (id, 
						   configuracao, 
						   pergunta, 
						   tid)
						values
						  ({0}seq_cnf_fisc_infr_cnf_pergu.nextval, 
						   :configuracao, 
						   :pergunta, 
						   :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("pergunta", item.PerguntaId, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Campos

                foreach (var item in configuracao.Campos)
                {
                    comando = bancoDeDados.CriarComando(@"
						insert into {0}cnf_fisc_infr_cnf_campo
						  (id, 
						   configuracao, 
						   campo, 
						   tid)
						values
						  ({0}seq_cnf_fisc_infr_cnf_campo.nextval, 
						   :configuracao, 
						   :campo, 
						   :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("campo", item.CampoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Histórico

                Historico.Gerar(configuracao.Id, eHistoricoArtefato.configfiscalizacao, eHistoricoAcao.criar, bancoDeDados);

                #endregion

                #region Consulta

                Consulta.Gerar(configuracao.Id, eHistoricoArtefato.configfiscalizacao, bancoDeDados);

                #endregion

                bancoDeDados.Commit();
            }
            return configuracao.Id;
        }

        public void Editar(ConfigFiscalizacao configuracao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
					update {0}cnf_fisc_infracao t
					   set t.classificacao = :classificacao,
						   t.tipo          = :tipo,
						   t.item          = :item,
						   t.tid           = :tid
					 where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("classificacao", configuracao.ClassificacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tipo", configuracao.TipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("item", configuracao.ItemId, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", configuracao.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                #region Subitens

                comando = bancoDeDados.CriarComando("delete from {0}cnf_fisc_infr_cnf_subitem t ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where t.configuracao = :configuracao{0}",
                    comando.AdicionarNotIn("and", "t.id", DbType.Int32, configuracao.Subitens.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in configuracao.Subitens)
                {
                    if (item.Id > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
							update {0}cnf_fisc_infr_cnf_subitem t
							   set t.subitem = :subitem, 
								   t.tid     = :tid
							 where t.id = :id", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
							insert into {0}cnf_fisc_infr_cnf_subitem
							  (id, 
							   configuracao, 
							   subitem, 
							   tid)
							values
							  ({0}seq_cnf_fisc_infrcnfsubitem.nextval, 
							   :configuracao, 
							   :subitem, 
							   :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
                    }

                    comando.AdicionarParametroEntrada("subitem", item.SubItemId, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Perguntas

                comando = bancoDeDados.CriarComando("delete from {0}cnf_fisc_infr_cnf_pergunta t ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where t.configuracao = :configuracao{0}",
                    comando.AdicionarNotIn("and", "t.id", DbType.Int32, configuracao.Perguntas.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in configuracao.Perguntas)
                {
                    if (item.Id > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
						  update {0}cnf_fisc_infr_cnf_pergunta t
							 set t.pergunta	= :pergunta,
								 t.tid      = :tid
						   where t.id = :id", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
							insert into {0}cnf_fisc_infr_cnf_pergunta
							  (id, 
							   configuracao, 
							   pergunta, 
							   tid)
							values
							  ({0}seq_cnf_fisc_infr_cnf_pergu.nextval, 
							   :configuracao, 
							   :pergunta, 
							   :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
                    }

                    comando.AdicionarParametroEntrada("pergunta", item.PerguntaId, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Campos

                comando = bancoDeDados.CriarComando("delete from {0}cnf_fisc_infr_cnf_campo t ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where t.configuracao = :configuracao{0}",
                    comando.AdicionarNotIn("and", "t.id", DbType.Int32, configuracao.Campos.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in configuracao.Campos)
                {
                    if (item.Id > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
						  update {0}cnf_fisc_infr_cnf_campo t
							 set t.campo	= :campo,
								 t.tid      = :tid
						   where t.id = :id", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
							insert into {0}cnf_fisc_infr_cnf_campo
							  (id, 
							   configuracao, 
							   campo, 
							   tid)
							values
							  ({0}seq_cnf_fisc_infr_cnf_campo.nextval, 
							   :configuracao, 
							   :campo, 
							   :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("configuracao", configuracao.Id, DbType.Int32);
                    }

                    comando.AdicionarParametroEntrada("campo", item.CampoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Histórico

                Historico.Gerar(configuracao.Id, eHistoricoArtefato.configfiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                #endregion

                #region Consulta

                Consulta.Gerar(configuracao.Id, eHistoricoArtefato.configfiscalizacao, bancoDeDados);

                #endregion

                bancoDeDados.Commit();
            }
        }

        public void Excluir(int configfiscalizacaoid, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando("update {0}cnf_fisc_infracao t set t.tid = :tid where t.id = :id");
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", configfiscalizacaoid, DbType.Int32);

                #region Histórico

                Historico.Gerar(configfiscalizacaoid, eHistoricoArtefato.configfiscalizacao, eHistoricoAcao.excluir, bancoDeDados);

                #endregion

                comando = bancoDeDados.CriarComando(
                    "begin " +
                        "delete {0}cnf_fisc_infr_cnf_subitem t where t.configuracao = :configuracao; " +
                        "delete {0}cnf_fisc_infr_cnf_pergunta t where t.configuracao = :configuracao; " +
                        "delete {0}cnf_fisc_infr_cnf_campo t where t.configuracao = :configuracao; " +
                        "delete {0}cnf_fisc_infracao t where t.id = :configuracao; " +
                    "end;", EsquemaBanco);
                comando.AdicionarParametroEntrada("configuracao", configfiscalizacaoid, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                #region Consulta

                Consulta.Deletar(configfiscalizacaoid, eHistoricoArtefato.configfiscalizacao, bancoDeDados);

                #endregion

                bancoDeDados.Commit();
            }
        }

        #region Tipo de Infração

        internal int SalvarTipoInfracao(Item entidade, BancoDeDados banco = null)
        {
            if (entidade == null)
            {
                throw new Exception("Objeto item é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                int itemId = Convert.ToInt32(entidade.Id);
                eHistoricoAcao acao = eHistoricoAcao.criar;

                if (itemId <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_tipo (id, texto, ativo, tid) 
														values(seq_cnf_fisc_infracao_tipo.nextval, :texto, :ativo, :tid) 
														returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_tipo c set c.texto = :texto, c.ativo = :ativo, c.tid = :tid 
														where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", itemId, DbType.Int32);
                    acao = eHistoricoAcao.atualizar;
                }

                comando.AdicionarParametroEntrada("texto", entidade.Texto, DbType.String);
                comando.AdicionarParametroEntrada("ativo", entidade.IsAtivo, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


                bancoDeDados.ExecutarNonQuery(comando);

                if (itemId <= 0)
                {
                    entidade.Id = comando.ObterValorParametro("id").ToString();
                }

                #region Histórico

                Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.tipoinfracao, acao, bancoDeDados, null);

                #endregion

                bancoDeDados.Commit();

                return Convert.ToInt32(entidade.Id);
            }
        }

        public void ExcluirTipoInfracao(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;
                #region Histórico

                //Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_tipo t set t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.tipoinfracao, eHistoricoAcao.excluir, bancoDeDados, null);

                #endregion

                comando = bancoDeDados.CriarComando(@"delete {0}cnf_fisc_infracao_tipo t where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                bancoDeDados.Commit();
            }
        }

        public void GerarConsultaTipoInfracao(int id, BancoDeDados banco = null)
        {
            List<int> listConfig = new List<int>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select i.id configuracao from cnf_fisc_infracao i where i.tipo = :tipo", EsquemaBanco);

                comando.AdicionarParametroEntrada("tipo", id, DbType.Int32);

                listConfig = bancoDeDados.ObterEntityList<int>(comando);

                foreach (var tipo in listConfig)
                {
                    Consulta.Gerar(tipo, eHistoricoArtefato.configfiscalizacao, bancoDeDados);
                }
            }
        }

        #endregion

        #region Item Infracao

        internal int SalvarItemInfracao(Item entidade, BancoDeDados banco = null)
        {
            if (entidade == null)
            {
                throw new Exception("Objeto item é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                int itemId = Convert.ToInt32(entidade.Id);
                eHistoricoAcao acao = eHistoricoAcao.criar;

                if (itemId <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_item (id, texto, ativo, tid) 
															values(seq_cnf_fisc_infracao_item.nextval, :texto, :ativo, :tid) 
															returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_item c set c.texto = :texto, c.ativo = :ativo, c.tid = :tid 
														where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", itemId, DbType.Int32);
                    acao = eHistoricoAcao.atualizar;
                }

                comando.AdicionarParametroEntrada("texto", entidade.Texto, DbType.String);
                comando.AdicionarParametroEntrada("ativo", entidade.IsAtivo, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


                bancoDeDados.ExecutarNonQuery(comando);

                if (itemId <= 0)
                {
                    entidade.Id = comando.ObterValorParametro("id").ToString();
                }

                #region Histórico

                Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.iteminfracao, acao, bancoDeDados, null);

                #endregion

                bancoDeDados.Commit();

                return Convert.ToInt32(entidade.Id);
            }
        }

        public void ExcluirItemInfracao(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;

                #region Histórico

                //Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_item t set t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.iteminfracao, eHistoricoAcao.excluir, bancoDeDados, null);

                #endregion

                comando = bancoDeDados.CriarComando(@"delete {0}cnf_fisc_infracao_item t where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                bancoDeDados.Commit();
            }
        }

        public void GerarConsultaItem(int id, BancoDeDados banco = null)
        {
            List<int> listConfig = new List<int>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select i.id configuracao from {0}cnf_fisc_infracao i where i.item = :item", EsquemaBanco);

                comando.AdicionarParametroEntrada("item", id, DbType.Int32);

                listConfig = bancoDeDados.ObterEntityList<int>(comando);

                foreach (var item in listConfig)
                {
                    Consulta.Gerar(item, eHistoricoArtefato.configfiscalizacao, bancoDeDados);
                }
            }
        }

        #endregion

        #region SubItem Infracao

        internal int SalvarSubItemInfracao(Item entidade, BancoDeDados banco = null)
        {
            if (entidade == null)
            {
                throw new Exception("Objeto SubItem é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                int SubItemId = Convert.ToInt32(entidade.Id);
                eHistoricoAcao acao = eHistoricoAcao.criar;

                if (SubItemId <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_subitem (id, texto, ativo, tid) 
														values(seq_cnf_fisc_infracao_subitem.nextval, :texto, :ativo, :tid) 
														returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_subitem c set c.texto = :texto, c.ativo = :ativo, c.tid = :tid 
														where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", SubItemId, DbType.Int32);
                    acao = eHistoricoAcao.atualizar;
                }

                comando.AdicionarParametroEntrada("texto", entidade.Texto, DbType.String);
                comando.AdicionarParametroEntrada("ativo", entidade.IsAtivo, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


                bancoDeDados.ExecutarNonQuery(comando);

                if (SubItemId <= 0)
                {
                    entidade.Id = comando.ObterValorParametro("id").ToString();
                }

                #region Histórico

                Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.subiteminfracao, acao, bancoDeDados, null);

                #endregion

                bancoDeDados.Commit();

                return Convert.ToInt32(entidade.Id);
            }
        }

        public void ExcluirSubItemInfracao(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;

                #region Histórico

                //Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_subitem t set t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.subiteminfracao, eHistoricoAcao.excluir, bancoDeDados, null);

                #endregion

                comando = bancoDeDados.CriarComando(@"delete {0}cnf_fisc_infracao_subitem t where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                bancoDeDados.Commit();
            }
        }

        #endregion

        #region Penalidade

        public void ExcluirPenalidade(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;

                #region Histórico

                //Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_penalidade t set t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.penalidadeinfracao, eHistoricoAcao.excluir, bancoDeDados, null);

                #endregion

                comando = bancoDeDados.CriarComando(@"delete {0}cnf_fisc_infracao_penalidade t where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                bancoDeDados.Commit();
            }
        }


        public void ExcluirPenalidades(List<Penalidade> lstAtivadas, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;

                comando = bancoDeDados.CriarComando(@"delete {0}cnf_fisc_infracao_penalidade t ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where 0=0{0}",
                comando.AdicionarNotIn("and", "t.id", DbType.Int32, lstAtivadas.Select(x => x.Id).ToList()));
                bancoDeDados.ExecutarNonQuery(comando);


                bancoDeDados.Commit();
            }
        }

        internal int SalvarPenalidade(Penalidade entidade, BancoDeDados banco = null)
        {
            if (entidade == null)
            {
                throw new Exception("Objeto penalidade é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                int campoId = Convert.ToInt32(entidade.Id);
                eHistoricoAcao acao = eHistoricoAcao.criar;

                if (campoId <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_penalidade (id, artigo, item, descricao, tid, ativo) 
														values(seq_cnf_fisc_inf_penalidade.nextval, :artigo, :item, :descricao, :tid, :ativo) 
														returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_penalidade c set c.artigo = :artigo, c.item = :item,
														c.descricao = :descricao, c.tid = :tid, c.ativo = :ativo 
														where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", campoId, DbType.Int32);
                    acao = eHistoricoAcao.atualizar;
                }

                comando.AdicionarParametroEntrada("artigo", entidade.Artigo, DbType.String);
                comando.AdicionarParametroEntrada("item", entidade.Item, DbType.String);
                comando.AdicionarParametroEntrada("descricao", entidade.Descricao, DbType.String);
                comando.AdicionarParametroEntrada("ativo", entidade.IsAtivo, DbType.UInt32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


                bancoDeDados.ExecutarNonQuery(comando);

                if (campoId <= 0)
                {
                    entidade.Id = comando.ObterValorParametro("id").ToString();
                }

                #region Histórico

                Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.penalidadeinfracao, acao, bancoDeDados, null);

                #endregion

                bancoDeDados.Commit();

                return Convert.ToInt32(entidade.Id);
            }
        }

        #endregion

        #region Campo Infracao

        internal int SalvarCampoInfracao(Item entidade, BancoDeDados banco = null)
		{
			if (entidade == null)
			{
				throw new Exception("Objeto Campo é nulo.");
			}

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                int campoId = Convert.ToInt32(entidade.Id);
                eHistoricoAcao acao = eHistoricoAcao.criar;

                if (campoId <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_campo (id, texto, tipo, unidade, ativo, tid) 
														values(seq_cnf_fisc_infracao_campo.nextval, :texto, :tipo, :unidade, :ativo, :tid) 
														returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_campo c set c.texto = :texto, c.tipo = :tipo,
														c.unidade = :unidade, c.ativo = :ativo, c.tid = :tid 
														where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", campoId, DbType.Int32);
                    acao = eHistoricoAcao.atualizar;
                }

                comando.AdicionarParametroEntrada("texto", entidade.Texto, DbType.String);
                comando.AdicionarParametroEntrada("tipo", entidade.TipoCampo, DbType.Int32);
                comando.AdicionarParametroEntrada("unidade", entidade.UnidadeMedida > 0 ? entidade.UnidadeMedida : (Object)DBNull.Value, DbType.Int32);
                comando.AdicionarParametroEntrada("ativo", entidade.IsAtivo, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


                bancoDeDados.ExecutarNonQuery(comando);

                if (campoId <= 0)
                {
                    entidade.Id = comando.ObterValorParametro("id").ToString();
                }

                #region Histórico

                Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.campoinfracao, acao, bancoDeDados, null);

                #endregion

                bancoDeDados.Commit();

                return Convert.ToInt32(entidade.Id);
            }
        }

        public void ExcluirCampoInfracao(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;

                #region Histórico

                //Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_campo t set t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.campoinfracao, eHistoricoAcao.excluir, bancoDeDados, null);

                #endregion

                comando = bancoDeDados.CriarComando(@"delete {0}cnf_fisc_infracao_campo t where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                bancoDeDados.Commit();
            }
        }


        #endregion

        #region Pergunta Infracao

        internal int SalvarPerguntaInfracao(PerguntaInfracao entidade, BancoDeDados banco = null)
        {
            if (entidade == null)
            {
                throw new Exception("Objeto Pergunta é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                eHistoricoAcao acao = eHistoricoAcao.criar;

                if (entidade.Id <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_pergunta (id, texto, ativo, tid) 
														values(seq_cnf_fisc_infracao_pergunta.nextval, :texto, :ativo, :tid) 
														returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32);
                    entidade.SituacaoId = 1;
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_Pergunta c set c.texto = :texto, c.ativo = :ativo, c.tid = :tid 
														where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
                    acao = eHistoricoAcao.atualizar;
                }

                comando.AdicionarParametroEntrada("texto", entidade.Texto, DbType.String);
                comando.AdicionarParametroEntrada("ativo", entidade.SituacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


                bancoDeDados.ExecutarNonQuery(comando);

                if (entidade.Id <= 0)
                {
                    entidade.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
                }

                #region Pergunta/Respostas

                #region Limpar os dados do banco

                comando = bancoDeDados.CriarComando(@"delete from {0}cnf_fisc_infracao_pergu_respo c where c.pergunta = :pergunta", EsquemaBanco);

                comando.AdicionarParametroEntrada("pergunta", entidade.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                foreach (var resposta in entidade.Respostas)
                {
                    #region Obter Tid da Resposta

                    comando = bancoDeDados.CriarComando(@"select tid from {0}cnf_fisc_infracao_resposta c where c.id = :resposta", EsquemaBanco);

                    comando.AdicionarParametroEntrada("resposta", resposta.Id, DbType.Int32);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        if (reader.Read())
                        {
                            if (reader["tid"] != null && !Convert.IsDBNull(reader["tid"]))
                            {
                                resposta.Tid = reader["tid"].ToString();
                            }

                            reader.Close();
                            reader.Dispose();
                        }
                    }

                    #endregion

                    comando = bancoDeDados.CriarComando(@"insert into {0}cnf_fisc_infracao_pergu_respo c (id, pergunta, resposta, resposta_tid, especificar, tid)
														values({0}seq_cnf_fisc_infracao_pergu_re.nextval, :pergunta, :resposta, :resposta_tid, :especificar, :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("pergunta", entidade.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("resposta", resposta.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("resposta_tid", DbType.String, 36, resposta.Tid);
                    comando.AdicionarParametroEntrada("especificar", resposta.Especificar, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Histórico

                Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.perguntainfracao, acao, bancoDeDados, null);

                #endregion

                bancoDeDados.Commit();

                return entidade.Id;
            }
        }

        public void ExcluirPerguntaInfracao(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;

                #region Histórico

                //Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_Pergunta t set t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.perguntainfracao, eHistoricoAcao.excluir, bancoDeDados, null);

                #endregion

                comando = bancoDeDados.CriarComando(@"begin " +
                                                    @"delete from {0}cnf_fisc_infracao_pergu_respo pr where pr.pergunta = :perguntaId;" +
                                                    @"delete from {0}cnf_fisc_infracao_pergunta t where t.id = :perguntaId;" +
                                                    @" end;", EsquemaBanco);

                comando.AdicionarParametroEntrada("perguntaId", id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                bancoDeDados.Commit();
            }
        }

        #endregion

        #region Resposta Infracao

        internal int SalvarRespostaInfracao(Item entidade, BancoDeDados banco = null)
        {
            if (entidade == null)
            {
                throw new Exception("Objeto Resposta é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                int RespostaId = Convert.ToInt32(entidade.Id);
                eHistoricoAcao acao = eHistoricoAcao.criar;

                if (RespostaId <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_resposta (id, texto, ativo, tid) 
														values(seq_cnf_fisc_infracao_resposta.nextval, :texto, :ativo, :tid) 
														returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_resposta c set c.texto = :texto, c.ativo = :ativo, c.tid = :tid 
														where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", RespostaId, DbType.Int32);
                    acao = eHistoricoAcao.atualizar;
                }

                comando.AdicionarParametroEntrada("texto", entidade.Texto, DbType.String);
                comando.AdicionarParametroEntrada("ativo", entidade.IsAtivo, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());


                bancoDeDados.ExecutarNonQuery(comando);

                if (RespostaId <= 0)
                {
                    entidade.Id = comando.ObterValorParametro("id").ToString();
                }

                #region Histórico

                Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.respostainfracao, acao, bancoDeDados, null);

                #endregion

                bancoDeDados.Commit();

                return Convert.ToInt32(entidade.Id);
            }
        }

        public void ExcluirRespostaInfracao(int id, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = null;

                #region Histórico

                //Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_resposta t set t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(id, eHistoricoArtefato.respostainfracao, eHistoricoAcao.excluir, bancoDeDados, null);

                #endregion

                comando = bancoDeDados.CriarComando(@"delete {0}cnf_fisc_infracao_resposta t where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                bancoDeDados.Commit();
            }
        }

        #endregion

        #region Produtos Apreendidos/Destinação

        internal void SalvarProdutosApreendidos(List<ProdutoApreendido> listaProdutos, BancoDeDados banco = null)
        {
            if (listaProdutos == null)
            {
                throw new Exception("Objeto Produto Apreendido é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando;

                bancoDeDados.IniciarTransacao();

                eHistoricoAcao? acao;

                foreach (var produto in listaProdutos)
                {
                    acao = null;
                    comando = null;

                    if (produto.Id == 0)    //produto novo, incluir
                    {
                        acao = eHistoricoAcao.criar;

                        comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infracao_produto(id, item, unidade, ativo, tid)
                                                              values(seq_cnf_fisc_infracao_produto.nextval, :item, :unidade, :ativo, :tid)
                                                              returning id into :id", EsquemaBanco);

                        comando.AdicionarParametroSaida("id", DbType.Int32);
                        comando.AdicionarParametroEntrada("item", produto.Item, DbType.String);
                        comando.AdicionarParametroEntrada("unidade", produto.Unidade, DbType.String);
                        comando.AdicionarParametroEntrada("ativo", produto.Ativo, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        produto.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

                        #region Histórico

                        Historico.Gerar(produto.Id, eHistoricoArtefato.produtoapreendido, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion
                    }
                    else if (produto.Excluir == false && produto.Editado == true)  //produto existente, editar
                    {
                        acao = eHistoricoAcao.atualizar;

                        comando = bancoDeDados.CriarComando(@"update cnf_fisc_infracao_produto
                                                              set item = :item,
                                                                  unidade = :unidade,
                                                                  ativo = :ativo,
                                                                  tid = :tid
                                                              where id = :id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id", produto.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("item", produto.Item, DbType.String);
                        comando.AdicionarParametroEntrada("unidade", produto.Unidade, DbType.String);
                        comando.AdicionarParametroEntrada("ativo", produto.Ativo, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        #region Histórico

                        Historico.Gerar(produto.Id, eHistoricoArtefato.produtoapreendido, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion
                    }
                    else if (produto.Excluir == true)   //produto existente, excluir
                    {
                        acao = eHistoricoAcao.excluir;

                        comando = bancoDeDados.CriarComando(@"delete from cnf_fisc_infracao_produto
                                                              where id = :id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id", produto.Id, DbType.Int32);

                        #region Histórico

                        //No excluir, o histórico deve ser preenchido primeiro, para poder pegar o elemento antes que ele seja excluído
                        Historico.Gerar(produto.Id, eHistoricoArtefato.produtoapreendido, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                bancoDeDados.Commit();

                //return Convert.ToInt32(entidade.Id);
            }
        }

        internal void SalvarDestinacao(List<DestinacaoProduto> listaDestinacao, BancoDeDados banco = null)
        {
            if (listaDestinacao == null)
            {
                throw new Exception("Objeto Destinação é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando;

                bancoDeDados.IniciarTransacao();

                eHistoricoAcao? acao;

                foreach (var destinacao in listaDestinacao)
                {
                    acao = null;
                    comando = null;

                    if (destinacao.Id == 0)    //destinação nova, incluir
                    {
                        acao = eHistoricoAcao.criar;

                        comando = bancoDeDados.CriarComando(@"insert into cnf_fisc_infr_destinacao(id, destino, ativo, tid)
                                                              values(seq_cnf_fisc_infr_destinacao.nextval, :destino, :ativo, :tid)
                                                              returning id into :id", EsquemaBanco);

                        comando.AdicionarParametroSaida("id", DbType.Int32);
                        comando.AdicionarParametroEntrada("destino", destinacao.Destino, DbType.String);
                        comando.AdicionarParametroEntrada("ativo", destinacao.Ativo, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        destinacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

                        #region Histórico

                        Historico.Gerar(destinacao.Id, eHistoricoArtefato.fiscdestinacao, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion
                    }
                    else if (destinacao.Excluir == false && destinacao.Editado == true)  //destinação existente, editar
                    {
                        acao = eHistoricoAcao.atualizar;

                        comando = bancoDeDados.CriarComando(@"update cnf_fisc_infr_destinacao
                                                              set destino = :destino,
                                                                  ativo = :ativo,
                                                                  tid = :tid
                                                              where id = :id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id", destinacao.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("destino", destinacao.Destino, DbType.String);
                        comando.AdicionarParametroEntrada("ativo", destinacao.Ativo, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        #region Histórico

                        Historico.Gerar(destinacao.Id, eHistoricoArtefato.fiscdestinacao, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion
                    }
                    else if (destinacao.Excluir == true)   //destinação existente, excluir
                    {
                        acao = eHistoricoAcao.excluir;

                        comando = bancoDeDados.CriarComando(@"delete from cnf_fisc_infr_destinacao
                                                              where id = :id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id", destinacao.Id, DbType.Int32);

                        #region Histórico

                        //No excluir, o histórico deve ser preenchido primeiro, para poder pegar o elemento antes que ele seja excluído
                        Historico.Gerar(destinacao.Id, eHistoricoArtefato.fiscdestinacao, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                bancoDeDados.Commit();

                //return Convert.ToInt32(entidade.Id);
            }
        }

        #endregion Produtos Apreendidos/Destinação

        #region Códigos da Receita Infracao

        internal void SalvarCodigosReceita(List<CodigoReceita> listaCodigosReceita, BancoDeDados banco = null)
        {
            if (listaCodigosReceita == null)
            {
                throw new Exception("Objeto Códigos da Receita é nulo.");
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando;

                bancoDeDados.IniciarTransacao();

                eHistoricoAcao? acao;

                foreach (var codReceita in listaCodigosReceita)
                {
                    acao = null;
                    comando = null;

                    if (codReceita.Id == 0)    //código da receita novo, incluir  
                    {
                        acao = eHistoricoAcao.criar;

                        comando = bancoDeDados.CriarComando(@"insert into lov_fisc_infracao_codigo_rece(id, texto, descricao, ativo, tid)  
                                                              values(seq_lov_fisc_infr_cod_receita.nextval, :codigo, :descricao, :ativo, :tid) 
                                                              returning id into :id", EsquemaBanco);

                        comando.AdicionarParametroSaida("id", DbType.Int32);
                        comando.AdicionarParametroEntrada("codigo", codReceita.Codigo, DbType.String);
                        comando.AdicionarParametroEntrada("descricao", codReceita.Descricao, DbType.String);
                        comando.AdicionarParametroEntrada("ativo", codReceita.Ativo, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        codReceita.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

                        #region Histórico

                        Historico.Gerar(codReceita.Id, eHistoricoArtefato.codigoreceita, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion
                    }
                    else if (codReceita.Excluir == false && codReceita.Editado == true)  //produto existente, editar  
                    {
                        acao = eHistoricoAcao.atualizar;

                        comando = bancoDeDados.CriarComando(@"update lov_fisc_infracao_codigo_rece 
                                                              set texto = :codigo, 
                                                                  descricao = :descricao, 
                                                                  ativo = :ativo, 
                                                                  tid = :tid 
                                                              where id = :id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id", codReceita.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("codigo", codReceita.Codigo, DbType.String);
                        comando.AdicionarParametroEntrada("descricao", codReceita.Descricao, DbType.String);
                        comando.AdicionarParametroEntrada("ativo", codReceita.Ativo, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        #region Histórico

                        Historico.Gerar(codReceita.Id, eHistoricoArtefato.codigoreceita, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion
                    }
                    else if (codReceita.Excluir == true)   //produto existente, excluir  
                    {
                        acao = eHistoricoAcao.excluir;

                        comando = bancoDeDados.CriarComando(@"delete from lov_fisc_infracao_codigo_rece 
                                                              where id = :id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id", codReceita.Id, DbType.Int32);

                        #region Histórico

                        //No excluir, o histórico deve ser preenchido primeiro, para poder pegar o elemento antes que ele seja excluído  
                        Historico.Gerar(codReceita.Id, eHistoricoArtefato.codigoreceita, (eHistoricoAcao)acao, bancoDeDados, null);

                        #endregion

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                bancoDeDados.Commit();
            }
        }

        internal List<CodigoReceita> ObterCodigosReceita(BancoDeDados banco = null)
        {
            List<CodigoReceita> listaCodigosReceita = new List<CodigoReceita>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                comando = bancoDeDados.CriarComando(@"select id, texto, descricao, ativo, tid  
                                                      from lov_fisc_infracao_codigo_rece 
                                                      order by texto, descricao", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        CodigoReceita codReceita = new CodigoReceita();

                        codReceita.Id = reader.GetValue<int>("id");
                        codReceita.Codigo = reader.GetValue<string>("texto");
                        codReceita.Descricao = reader.GetValue<string>("descricao") ?? string.Empty;
                        codReceita.Ativo = reader.GetValue<bool>("ativo");
                        codReceita.Tid = reader.GetValue<string>("tid");

                        listaCodigosReceita.Add(codReceita);
                    }
                }
            }

            return listaCodigosReceita;
        }

        internal bool PermiteExcluirCodigo(CodigoReceita codigo, BancoDeDados banco = null)
        {
            bool podeExcluir = false;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                comando = bancoDeDados.CriarComando(@"select count(*) total 
                                                      from tab_fisc_infracao 
                                                      where codigo_receita = :idCodigo", EsquemaBanco);

                comando.AdicionarParametroEntrada("idCodigo", codigo.Id, DbType.Int32);

                var total = bancoDeDados.ExecutarScalar<int>(comando);

                podeExcluir = total > 0 ? false : true;
            }

            return podeExcluir;
        }


		#endregion Códigos da Receita Infracao

		#region VRTE

		internal void SalvarVrte(List<Vrte> listaVrte, BancoDeDados banco = null)
		{
			if (listaVrte == null)
			{
				throw new Exception("Objeto Códigos da Receita é nulo.");
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				bancoDeDados.IniciarTransacao();

				eHistoricoAcao? acao;

				foreach (var vrte in listaVrte)
				{
					acao = null;
					comando = null;

					if (vrte.Id == 0)    // novo, incluir  
					{
						acao = eHistoricoAcao.criar;

						comando = bancoDeDados.CriarComando(@"insert into tab_fisc_vrte(id, ano, vrte, tid)  
                                                              values(seq_fisc_vrte.nextval, :ano, :vrte,:tid) 
                                                              returning id into :id", EsquemaBanco);

						comando.AdicionarParametroSaida("id", DbType.Int32);
						comando.AdicionarParametroEntrada("ano", vrte.Ano, DbType.Int32);
						comando.AdicionarParametroEntrada("vrte", vrte.VrteEmReais, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						vrte.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Histórico

						Historico.Gerar(vrte.Id, eHistoricoArtefato.vrte, (eHistoricoAcao)acao, bancoDeDados, null);

						#endregion
					}
					else if (vrte.Excluir == false && vrte.Editado == true)  // existente, editar  
					{
						acao = eHistoricoAcao.atualizar;

						comando = bancoDeDados.CriarComando(@"update tab_fisc_vrte 
                                                              set ano = :ano, 
                                                                  vrte = :vrte, 
                                                                  tid = :tid 
                                                              where id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", vrte.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("ano", vrte.Ano, DbType.Int32);
						comando.AdicionarParametroEntrada("vrte", vrte.VrteEmReais, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						#region Histórico

						Historico.Gerar(vrte.Id, eHistoricoArtefato.vrte, (eHistoricoAcao)acao, bancoDeDados, null);

						#endregion
					}
					else if (vrte.Excluir)  
					{
						acao = eHistoricoAcao.excluir;

						comando = bancoDeDados.CriarComando(@"delete from tab_fisc_vrte 
                                                              where id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", vrte.Id, DbType.Int32);

						#region Histórico

						//No excluir, o histórico deve ser preenchido primeiro, para poder pegar o elemento antes que ele seja excluído  
						Historico.Gerar(vrte.Id, eHistoricoArtefato.vrte, (eHistoricoAcao)acao, bancoDeDados, null);

						#endregion

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				bancoDeDados.Commit();
			}
		}

		internal Vrte ObterVrte(int ano, BancoDeDados banco = null)
		{
			var vrte = new Vrte();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				bancoDeDados.IniciarTransacao();

				comando = bancoDeDados.CriarComando(@"select id, ano, vrte, tid  
                                                      from tab_fisc_vrte
													  where ano = :ano", EsquemaBanco);

                comando.AdicionarParametroEntrada("ano", ano, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						vrte = new Vrte()
						{
							Id = reader.GetValue<int>("id"),
							Ano = reader.GetValue<int>("ano"),
							VrteEmReais = reader.GetValue<decimal>("vrte"),
							Tid = reader.GetValue<string>("tid")
						};
					}
                    reader.Close();
				}
			}

			return vrte;
		}

		internal List<Vrte> ObterVrte(BancoDeDados banco = null)
		{
			List<Vrte> listaVrte = new List<Vrte>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				bancoDeDados.IniciarTransacao();

				comando = bancoDeDados.CriarComando(@"select id, ano, vrte, tid  
                                                      from tab_fisc_vrte 
                                                      order by ano, vrte", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Vrte codReceita = new Vrte();

						codReceita.Id = reader.GetValue<int>("id");
						codReceita.Ano = reader.GetValue<int>("ano");
						codReceita.VrteEmReais = reader.GetValue<decimal>("vrte");
						codReceita.Tid = reader.GetValue<string>("tid");

						listaVrte.Add(codReceita);
					}
				}
			}

			return listaVrte;
		}

		internal bool PermiteExcluirVrte(Vrte codigo, BancoDeDados banco = null) => true;

		#endregion VRTE

		#region Parametrizacao 

		internal int SalvarParametrizacao(Parametrizacao entidade, BancoDeDados banco = null)
		{
			if (entidade == null)
				throw new Exception("Objeto Parametrizacao é nulo.");

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				bancoDeDados.IniciarTransacao();

				eHistoricoAcao acao = eHistoricoAcao.criar;

				if (entidade.Id <= 0)
				{
					comando = bancoDeDados.CriarComando(@" insert into {0}tab_fisc_parametrizacao (id,
											codigoreceita, iniciovigencia, fimvigencia, valorminimo_pf, valorminimo_pj, multa_perc, juros_perc,
											desconto_perc, desconto_und, desconto_decor, tid)
											values ({0}seq_fisc_parametrizacao.nextval,
                                            :codigoreceita, :iniciovigencia, :fimvigencia, :valorminimo_pf, :valorminimo_pj,
											:multa_perc, :juros_perc, :desconto_perc, :desconto_und, :desconto_decor, :tid)
														returning id into :id", EsquemaBanco);

					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update tab_fisc_parametrizacao p set
														p.codigoreceita = :codigoreceita,
														p.iniciovigencia = :iniciovigencia,
														p.fimvigencia = :fimvigencia,
														p.valorminimo_pf = :valorminimo_pf,
														p.valorminimo_pj = :valorminimo_pj,
														p.multa_perc = :multa_perc,
														p.juros_perc = :juros_perc,
														p.desconto_perc = :desconto_perc,
														p.desconto_und = :desconto_und,
														p.desconto_decor = :desconto_decor,
														p.tid = :tid
														where p.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
					acao = eHistoricoAcao.atualizar;
				}

				comando.AdicionarParametroEntrada("codigoreceita", entidade.CodigoReceitaId, DbType.Int32);
				comando.AdicionarParametroEntrada("iniciovigencia", entidade.InicioVigencia.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("fimvigencia", entidade.FimVigencia.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("valorminimo_pf", entidade.ValorMinimoPF, DbType.Int32);
				comando.AdicionarParametroEntrada("valorminimo_pj", entidade.ValorMinimoPJ, DbType.Int32);
				comando.AdicionarParametroEntrada("multa_perc", entidade.MultaPercentual, DbType.Int32);
				comando.AdicionarParametroEntrada("juros_perc", entidade.JurosPercentual, DbType.Int32);
				comando.AdicionarParametroEntrada("desconto_perc", entidade.DescontoPercentual, DbType.Int32);
				comando.AdicionarParametroEntrada("desconto_und", entidade.PrazoDescontoUnidade, DbType.Int32);
				comando.AdicionarParametroEntrada("desconto_decor", entidade.PrazoDescontoDecorrencia, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (entidade.Id <= 0)
					entidade.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(Convert.ToInt32(entidade.Id), eHistoricoArtefato.parametrizacao, acao, bancoDeDados, null);

				bancoDeDados.Commit();

				return entidade.Id;
			}
		}

		internal void SalvarParametrizacaoDetalhe(List<ParametrizacaoDetalhe> listaDetalhe, BancoDeDados banco = null)
		{
			if (listaDetalhe == null)
			{
				throw new Exception("Objeto Parametrização é nulo.");
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				bancoDeDados.IniciarTransacao();

				foreach (var detalhe in listaDetalhe)
				{
					comando = null;

					if (detalhe.Id == 0)    // novo, incluir  
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_fisc_param_detalhe(id, parametrizacao, valorinicial, valorfinal, maximoparcelas, tid)  
                                                              values(seq_fisc_param_detalhe.nextval, :parametrizacao, :valorinicial, :valorfinal, :maximoparcelas, :tid) 
                                                              returning id into :id", EsquemaBanco);

						comando.AdicionarParametroSaida("id", DbType.Int32);
						comando.AdicionarParametroEntrada("parametrizacao", detalhe.ParametrizacaoId, DbType.Int32);
						comando.AdicionarParametroEntrada("valorinicial", detalhe.ValorInicial, DbType.Decimal);
						comando.AdicionarParametroEntrada("valorfinal", detalhe.ValorFinal, DbType.Decimal);
						comando.AdicionarParametroEntrada("maximoparcelas", detalhe.MaximoParcelas, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						detalhe.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}
					else if (detalhe.Excluir == false && detalhe.Editado == true)  // existente, editar  
					{
						comando = bancoDeDados.CriarComando(@"update tab_fisc_param_detalhe 
                                                              set parametrizacao = :parametrizacao,
																  valorinicial = :valorinicial,
																  valorfinal = :valorfinal,
																  maximoparcelas = :maximoparcelas,
                                                                  tid = :tid 
                                                              where id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", detalhe.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("parametrizacao", detalhe.ParametrizacaoId, DbType.Int32);
						comando.AdicionarParametroEntrada("valorinicial", detalhe.ValorInicial, DbType.Decimal);
						comando.AdicionarParametroEntrada("valorfinal", detalhe.ValorFinal, DbType.Decimal);
						comando.AdicionarParametroEntrada("maximoparcelas", detalhe.MaximoParcelas, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
					else if (detalhe.Excluir)  
					{
						comando = bancoDeDados.CriarComando(@"delete from tab_fisc_param_detalhe 
                                                              where id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", detalhe.Id, DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				bancoDeDados.Commit();
			}
		}

		public void ExcluirParametrizacao(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = null;

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}tab_fisc_parametrizacao t set t.tid = :tid where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.parametrizacao, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				comando = bancoDeDados.CriarComando(
                    "begin " +
						"delete from {0}tab_fisc_param_detalhe p where p.parametrizacao = :id;" +
						"delete from {0}tab_fisc_parametrizacao p where p.id = :id;" +
					"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#endregion

		#region Obter / Filtrar

		internal ConfigFiscalizacao Obter(int id, BancoDeDados banco = null)
        {
            ConfigFiscalizacao configFiscalizacao = new ConfigFiscalizacao();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
					select t.id            Id,
						   t.classificacao ClassificacaoId,
						   l.texto         ClassificacaoTexto,
						   t.tipo          TipoId,
						   cit.texto       TipoTexto,
						   t.item          ItemId,
						   cii.texto       ItemTexto,
						   t.tid           Tid
					  from {0}cnf_fisc_infracao             t,
						   {0}cnf_fisc_infracao_tipo        cit,
						   {0}cnf_fisc_infracao_item        cii,
						   {0}lov_cnf_fisc_infracao_classif l
					 where t.tipo = cit.id(+)
					   and t.item = cii.id(+)
					   and t.classificacao = l.id(+)
					   and t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                configFiscalizacao = bancoDeDados.ObterEntity<ConfigFiscalizacao>(comando);

                #region Subitens

                comando = bancoDeDados.CriarComando(@"
					select t.id     Id, 
						   t.subitem SubItemId, 
						   l.texto   SubItemTexto, 
						   t.tid     Tid
					  from {0}cnf_fisc_infr_cnf_subitem t, 
						   {0}cnf_fisc_infracao_subitem l
					 where t.subitem = l.id(+)
					   and t.configuracao = :config
						order by l.texto", EsquemaBanco);

                comando.AdicionarParametroEntrada("config", id, DbType.Int32);

                configFiscalizacao.Subitens = bancoDeDados.ObterEntityList<ConfigFiscalizacaoSubItem>(comando);

                #endregion

                #region Perguntas

                comando = bancoDeDados.CriarComando(@"
					select t.id       Id, 
						   t.pergunta PerguntaId, 
						   l.texto    PerguntaTexto, 
						   t.tid      Tid
					  from {0}cnf_fisc_infr_cnf_pergunta t, 
						   {0}cnf_fisc_infracao_pergunta l
					 where t.pergunta = l.id(+)
					   and t.configuracao = :config
						order by l.texto", EsquemaBanco);

                comando.AdicionarParametroEntrada("config", id, DbType.Int32);

                configFiscalizacao.Perguntas = bancoDeDados.ObterEntityList<ConfigFiscalizacaoPergunta>(comando);

                #endregion

                #region Campo

                comando = bancoDeDados.CriarComando(@"
					select t.id    Id, 
						   t.campo CampoId, 
						   l.texto||' - '||lu.texto||' - '||(select texto from lov_cnf_fisc_infracao_camp_tip where id = l.tipo) CampoTexto, 
						   t.tid   Tid
					  from {0}cnf_fisc_infr_cnf_campo t, 
						   {0}cnf_fisc_infracao_campo l,
						   {0}lov_cnf_fisc_infracao_camp_uni lu
					 where t.campo = l.id(+)
					   and t.configuracao = :config
					   and lu.id = l.unidade
						order by l.texto", EsquemaBanco);

                comando.AdicionarParametroEntrada("config", id, DbType.Int32);

                configFiscalizacao.Campos = bancoDeDados.ObterEntityList<ConfigFiscalizacaoCampo>(comando);

                #endregion
            }
            return configFiscalizacao;
        }

        internal Dictionary<string, object> Obter(int classificacaoId, int tipoId, int itemId, BancoDeDados banco = null)
        {
            Dictionary<string, object> config = new Dictionary<string, object>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"
					select c.id, 
						   c.tid
					  from {0}cnf_fisc_infracao c
					 where c.item = :itemId
					   and c.tipo = :tipoId
					   and c.classificacao = :classificacaoId", EsquemaBanco);

                comando.AdicionarParametroEntrada("classificacaoId", classificacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tipoId", tipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("itemId", itemId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        config["Id"] = reader.GetValue<int>("id");
                        config["Tid"] = reader.GetValue<string>("tid");
                    }

                    reader.Close();
                }
            }

            return config;
        }

        internal List<Item> ObterTipoInfracao()
        {
            List<Item> lista = new List<Item>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@" select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_tipo t order by t.texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Item
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = reader.GetValue<Boolean>("ativo")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Item> ObterItemInfracao()
        {
            List<Item> lista = new List<Item>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_item t order by t.texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Item
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = reader.GetValue<Boolean>("ativo")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Item> ObterSubItemInfracao()
        {
            List<Item> lista = new List<Item>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_subitem t order by t.texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Item
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = reader.GetValue<Boolean>("ativo")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Penalidade> ObterPenalidades()
        {
            List<Penalidade> lista = new List<Penalidade>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select id , artigo, item, descricao, tid, ativo
															from {0}cnf_fisc_infracao_penalidade t
															order by t.descricao", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        
                        lista.Add(new Penalidade
                        {
                            Id = reader.GetValue<string>("id"),
                            Artigo = reader.GetValue<string>("artigo"),
                            Item = reader.GetValue<string>("item"),
                            Descricao = reader.GetValue<string>("descricao"),
                            Tid = reader.GetValue<string>("tid"),
                            Texto = reader.GetValue<string>("artigo"),
                            IsAtivo = reader.GetValue<Boolean>("ativo")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

		internal List<Item> ObterCampoInfracao()
		{
			List<Item> lista = new List<Item>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo, t.tipo, lt.texto tipo_texto, 
															t.unidade, lu.texto unidade_texto
															from {0}cnf_fisc_infracao_campo t, lov_cnf_fisc_infracao_camp_uni lu,
															lov_cnf_fisc_infracao_camp_tip lt 
															where lu.id(+) = t.unidade and lt.id = t.tipo order by t.texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        int unidadeMedida = 0;
                        String unidadeMedidaTexto = String.Empty;

                        if (reader["unidade"] != null && !Convert.IsDBNull(reader["unidade"]))
                        {
                            unidadeMedida = reader.GetValue<Int32>("unidade");
                            unidadeMedidaTexto = reader.GetValue<string>("unidade_texto");
                        }

                        lista.Add(new Item
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            TipoCampo = reader.GetValue<Int32>("tipo"),
                            UnidadeMedida = unidadeMedida,
                            UnidadeMedidaTexto = unidadeMedidaTexto,
                            TipoCampoTexto = reader.GetValue<string>("tipo_texto"),
                            IsAtivo = reader.GetValue<Boolean>("ativo")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Item> ObterPerguntaInfracao()
        {
            List<Item> lista = new List<Item>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_Pergunta t order by t.texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Item
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = reader.GetValue<Boolean>("ativo")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Item> ObterRespostaInfracao()
        {
            List<Item> lista = new List<Item>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_resposta t order by t.texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Item
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = reader.GetValue<Boolean>("ativo")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal PerguntaInfracao ObterPerguntaRespostasInfracao(int id)
        {
            PerguntaInfracao pergunta = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.ativo from {0}cnf_fisc_infracao_pergunta t where t.id = :id order by t.texto", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        pergunta = new PerguntaInfracao();
                        pergunta.Id = reader.GetValue<Int32>("id");
                        pergunta.Texto = reader.GetValue<String>("texto");
                        pergunta.SituacaoId = reader.GetValue<Int32>("ativo");

                        comando = bancoDeDados.CriarComando(@"select r.id, r.texto, r.ativo, r.tid, (select especificar 
															from cnf_fisc_infracao_pergu_respo where resposta = r.id 
															and pergunta = :pergunta) especificar from cnf_fisc_infracao_resposta r,
															cnf_fisc_infracao_pergu_respo cpr where r.id in (select pr.resposta 
															from cnf_fisc_infracao_pergu_respo pr where pr.pergunta = :pergunta) 
															and cpr.resposta = r.id and cpr.pergunta = :pergunta order by cpr.id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("pergunta", pergunta.Id, DbType.Int32);

                        using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
                        {
                            while (readerAux.Read())
                            {
                                Resposta resposta = new Resposta();
                                resposta.Id = readerAux.GetValue<String>("id");
                                resposta.Texto = readerAux.GetValue<String>("texto");
                                resposta.IsAtivo = readerAux.GetValue<Int32>("ativo") == 1;
                                resposta.Especificar = readerAux.GetValue<Int32>("especificar") == 1;
                                pergunta.Respostas.Add(resposta);
                            }

                            readerAux.Close();
                        }
                    }

                    reader.Close();
                }
            }

            return pergunta;
        }

        internal List<Item> ObterTipos(int classificacaoId = 0)
        {
            List<Item> lista = new List<Item>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@" select distinct t.id, t.texto from {0}cnf_fisc_infracao_tipo t, {0}cnf_fisc_infracao c where t.id = c.tipo  ", EsquemaBanco);

                if (classificacaoId > 0)
                {
                    comando.DbCommand.CommandText += " and c.classificacao = :classificacao ";
                    comando.AdicionarParametroEntrada("classificacao", classificacaoId, DbType.Int32);
                }

                comando.DbCommand.CommandText += " order by t.texto ";

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Item
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = true
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Lista> ObterItens(int classificacaoId, int tipoId)
        {
            List<Lista> lista = new List<Lista>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@" select l.id, l.texto from {0}cnf_fisc_infracao_item l, {0}cnf_fisc_infracao c where c.item = l.id and c.tipo = :tipoId and c.classificacao 
					= :classificacaoId ", EsquemaBanco);

                comando.AdicionarParametroEntrada("tipoId", tipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("classificacaoId", classificacaoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Lista
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = true
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Lista> ObterSubitens(int classificacaoId, int tipoId, int itemId)
        {
            List<Lista> lista = new List<Lista>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@" select l.id, l.texto from {0}cnf_fisc_infracao_subitem l, {0}cnf_fisc_infracao c, {0}cnf_fisc_infr_cnf_subitem cs where cs.subitem = l.id 
					and cs.configuracao = c.id and c.item = :itemId and c.tipo = :tipoId and c.classificacao = :classificacaoId order by l.texto ", EsquemaBanco);

                comando.AdicionarParametroEntrada("classificacaoId", classificacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tipoId", tipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("itemId", itemId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Lista
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = true
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Lista> ObterPenalidadesLista()
        {
            List<Lista> lista = new List<Lista>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select id,
                                           concat(artigo, concat(' - ', item)) texto,
                                           descricao codigo,
                                           ativo
                                    from cnf_fisc_infracao_penalidade
                                    order by texto", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Lista
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            Codigo = reader.GetValue<string>("codigo"),
                            IsAtivo = reader.GetValue<bool>("ativo"),
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<Lista> ObterSeries(bool isSim)
        {
            List<Lista> lista = new List<Lista>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@" select l.id, l.texto from {0}lov_fiscalizacao_serie l where l.tipo = :tipo ", EsquemaBanco);

                comando.AdicionarParametroEntrada("tipo", (isSim ? 1 : 0), DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new Lista
                        {
                            Id = reader.GetValue<string>("id"),
                            Texto = reader.GetValue<string>("texto"),
                            IsAtivo = true
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<InfracaoCampo> ObterCampos(int classificacaoId, int tipoId, int itemId)
        {
            List<InfracaoCampo> lista = new List<InfracaoCampo>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select l.id, l.texto, l.tipo, lt.texto tipo_texto, l.unidade, lu.texto unidade_texto, c.id configuracao 
															from {0}cnf_fisc_infracao_campo l, {0}cnf_fisc_infracao c, {0}cnf_fisc_infr_cnf_campo cc, 
															lov_cnf_fisc_infracao_camp_uni lu, lov_cnf_fisc_infracao_camp_tip lt 
															where cc.campo = l.id and cc.configuracao = c.id and c.item = :itemId 
															and c.tipo = :tipoId and l.tipo = lt.id(+) and l.unidade = lu.id(+)
															and c.classificacao = :classificacaoId order by l.texto", EsquemaBanco);

                comando.AdicionarParametroEntrada("classificacaoId", classificacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tipoId", tipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("itemId", itemId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new InfracaoCampo
                        {
                            CampoId = reader.GetValue<int>("id"),
                            ConfiguracaoId = reader.GetValue<int>("configuracao"),
                            Identificacao = reader.GetValue<string>("texto"),
                            Tipo = reader.GetValue<Int32>("tipo"),
                            TipoTexto = reader.GetValue<string>("tipo_texto"),
                            Unidade = reader.GetValue<Int32>("unidade"),
                            UnidadeTexto = reader.GetValue<string>("unidade_texto")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<InfracaoPergunta> ObterPerguntas(int classificacaoId, int tipoId, int itemId)
        {
            List<InfracaoPergunta> lista = new List<InfracaoPergunta>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@" select p.id PerguntaId, p.Texto Identificacao , p.tid PerguntaTid, c.id configuracao from {0}cnf_fisc_infracao_pergunta p, {0}cnf_fisc_infracao c, {0}cnf_fisc_infr_cnf_pergunta cp
				where p.id = cp.pergunta and cp.configuracao = c.id and c.tipo = :tipoId and c.item = :itemId and c.classificacao = :classificacaoId order by p.texto ", EsquemaBanco);

                comando.AdicionarParametroEntrada("classificacaoId", classificacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tipoId", tipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("itemId", itemId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new InfracaoPergunta
                        {
                            PerguntaId = reader.GetValue<int>("PerguntaId"),
                            PerguntaTid = reader.GetValue<string>("PerguntaTid"),
                            ConfiguracaoId = reader.GetValue<int>("configuracao"),
                            Identificacao = reader.GetValue<string>("Identificacao"),
                            Respostas = ObterRespostas(reader.GetValue<int>("PerguntaId"))
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<InfracaoResposta> ObterRespostas(int perguntaId, BancoDeDados banco = null)
        {
            List<InfracaoResposta> lista = new List<InfracaoResposta>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select r.Id, r.Texto Identificacao, r.tid, l.Especificar IsEspecificar 
															from {0}cnf_fisc_infracao_resposta r, {0}cnf_fisc_infracao_pergu_respo l 
															where l.pergunta = :perguntaId and l.resposta = r.id order by l.id", EsquemaBanco);

                comando.AdicionarParametroEntrada("perguntaId", perguntaId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        lista.Add(new InfracaoResposta
                        {
                            Id = reader.GetValue<int>("id"),
                            Identificacao = reader.GetValue<string>("Identificacao"),
                            IsEspecificar = reader.GetValue<bool>("IsEspecificar"),
                            Tid = reader.GetValue<string>("tid")
                        });
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<InfracaoResposta> ObterRespostasHistorico(int perguntaId, string perguntaTid, BancoDeDados banco = null)
        {
            List<InfracaoResposta> lista = new List<InfracaoResposta>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select hr.resposta, hr.resposta_tid from {0}hst_cnf_fisc_infracao_pergu_re hr 
															where hr.pergunta = :pergunta and hr.tid = :pergunta_tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("pergunta", perguntaId, DbType.Int32);
                comando.AdicionarParametroEntrada("pergunta_tid", perguntaTid, DbType.String);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        int respostaId = reader.GetValue<int>("resposta");
                        string respostaTid = reader.GetValue<string>("resposta_tid");

                        comando = bancoDeDados.CriarComando(@"select r.resposta_id id, r.texto Identificacao, hpr.especificar IsEspecificar 
															from {0}hst_cnf_fisc_infracao_resposta r, {0}hst_cnf_fisc_infracao_pergu_re hpr
															where r.resposta_id = :resposta and r.tid = :resposta_tid and hpr.resposta = :resposta 
															and hpr.resposta_tid = :resposta_tid and hpr.pergunta = :pergunta 
															and hpr.tid = :pergunta_tid and r.resposta_id = :resposta order by hpr.pergu_respo_id", EsquemaBanco);

                        comando.AdicionarParametroEntrada("resposta", respostaId, DbType.Int32);
                        comando.AdicionarParametroEntrada("resposta_tid", respostaTid, DbType.String);
                        comando.AdicionarParametroEntrada("pergunta", perguntaId, DbType.Int32);
                        comando.AdicionarParametroEntrada("pergunta_tid", perguntaTid, DbType.String);

                        using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
                        {
                            while (readerAux.Read())
                            {
                                lista.Add(new InfracaoResposta
                                {
                                    Id = readerAux.GetValue<int>("id"),
                                    Identificacao = readerAux.GetValue<string>("Identificacao"),
                                    IsEspecificar = readerAux.GetValue<bool>("IsEspecificar"),
                                    Tid = respostaTid
                                });
                            }

                            readerAux.Close();
                        }
                    }

                    reader.Close();
                }
            }

            return lista;
        }

        internal List<ProdutoApreendido> ObterProdutosApreendidos(BancoDeDados banco = null)
        {
            List<ProdutoApreendido> listaProdutos = new List<ProdutoApreendido>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                comando = bancoDeDados.CriarComando(@"select id, item, unidade, ativo, tid
                                                      from cnf_fisc_infracao_produto
                                                      order by item, unidade", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        ProdutoApreendido produto = new ProdutoApreendido();

                        produto.Id = reader.GetValue<int>("id");
                        produto.Item = reader.GetValue<string>("item");
                        produto.Unidade = reader.GetValue<string>("unidade");
                        produto.Ativo = reader.GetValue<bool>("ativo");
                        produto.Tid = reader.GetValue<string>("tid");

                        listaProdutos.Add(produto);
                    }
                }
            }

            return listaProdutos;
        }

        internal List<DestinacaoProduto> ObterDestinacao(BancoDeDados banco = null)
        {
            List<DestinacaoProduto> listaDestinacao = new List<DestinacaoProduto>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                comando = bancoDeDados.CriarComando(@"select id, destino, ativo, tid
                                                      from cnf_fisc_infr_destinacao
                                                      order by destino", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        DestinacaoProduto destinacao = new DestinacaoProduto();

                        destinacao.Id = reader.GetValue<int>("id");
                        destinacao.Destino = reader.GetValue<string>("destino");
                        destinacao.Ativo = reader.GetValue<bool>("ativo");
                        destinacao.Tid = reader.GetValue<string>("tid");

                        listaDestinacao.Add(destinacao);
                    }
                }
            }

            return listaDestinacao;
        }

        internal List<Lista> ObterItensConfig(bool? isAtivo)
        {
            List<Lista> lista = new List<Lista>();
            string strSql = string.Empty;

            if (isAtivo.HasValue)
            {
                strSql = isAtivo.Value ? "l.ativo = 1" : "l.ativo = 0";
            }
            else
            {
                strSql = "1 = 1";
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                lista = bancoDeDados.ObterEntityList<Lista>(bancoDeDados.CriarComando(@"select l.id Id, l.texto Texto, l.ativo IsAtivo from {0}cnf_fisc_infracao_item l where {1} order by l.texto", EsquemaBanco, strSql));
            }

            return lista;
        }

        internal List<Lista> ObterTiposConfig(bool? isAtivo)
        {
            List<Lista> lista = new List<Lista>();
            string strSql = string.Empty;

            if (isAtivo.HasValue)
            {
                strSql = isAtivo.Value ? "t.ativo = 1" : "t.ativo = 0";
            }
            else
            {
                strSql = "1 = 1";
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                lista = bancoDeDados.ObterEntityList<Lista>(bancoDeDados.CriarComando(@"select t.id Id, t.texto Texto, t.ativo IsAtivo from {0}cnf_fisc_infracao_tipo t where {1} order by t.texto", EsquemaBanco, strSql));
            }

            return lista;
        }

        internal List<Lista> ObterSubitensConfig(bool? isAtivo)
        {
            List<Lista> lista = new List<Lista>();
            string strSql = string.Empty;

            if (isAtivo.HasValue)
            {
                strSql = isAtivo.Value ? "l.ativo = 1" : "l.ativo = 0";
            }
            else
            {
                strSql = "1 = 1";
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                lista = bancoDeDados.ObterEntityList<Lista>(bancoDeDados.CriarComando(@"select l.id Id, l.texto Texto, l.ativo IsAtivo from {0}cnf_fisc_infracao_subitem l where {1} order by l.texto", EsquemaBanco, strSql));
            }

            return lista;
        }

        internal List<Lista> ObterPerguntasConfig()
        {
            List<Lista> lista = new List<Lista>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                lista = bancoDeDados.ObterEntityList<Lista>(bancoDeDados.CriarComando(@"select l.id Id, l.texto Texto, 1 IsAtivo from {0}cnf_fisc_infracao_pergunta l where /*l.ativo*/ 1 = 1 order by l.texto", EsquemaBanco));
            }

            return lista;
        }

        internal List<Lista> ObterCamposConfig(bool? isAtivo)
        {
            List<Lista> lista = new List<Lista>();
            string strSql = string.Empty;

            if (isAtivo.HasValue)
            {
                strSql = isAtivo.Value ? "l.ativo = 1" : "l.ativo = 0";
            }
            else
            {
                strSql = "1 = 1";
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                lista = bancoDeDados.ObterEntityList<Lista>(bancoDeDados.CriarComando(@"select l.id Id, l.texto||' - '||(select texto from lov_cnf_fisc_infracao_camp_uni where id = l.unidade)||' - '||(select texto from lov_cnf_fisc_infracao_camp_tip where id = l.tipo) Texto, l.ativo IsAtivo from {0}cnf_fisc_infracao_campo l where {1} order by l.texto", EsquemaBanco, strSql));
            }

            return lista;
        }

        internal Resultados<PerguntaInfracaoListarResultado> PerguntasFiltrar(Filtro<PerguntaInfracaoListarFiltro> filtros, BancoDeDados banco = null)
        {
            Resultados<PerguntaInfracaoListarResultado> retorno = new Resultados<PerguntaInfracaoListarResultado>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                string comandtxt = string.Empty;
                Comando comando = bancoDeDados.CriarComando("");

                #region Adicionando Filtros

                comandtxt += comando.FiltroAnd("t.pergunta", "pergunta", filtros.Dados.PerguntaId);
                comandtxt += comando.FiltroAnd("t.resposta", "resposta", filtros.Dados.RespostaId);
                comandtxt += comando.FiltroAnd("t.pergunta", "numero_pergunta", filtros.Dados.CodigoPergunta);

                List<String> ordenar = new List<String>();
                List<String> colunas = new List<String>() { "numero_pergunta", "pergunta_texto", "resposta_texto", "situacao desc" };

                if (filtros.OdenarPor > 0)
                {
                    ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
                }
                else
                {
                    ordenar.Add("numero_pergunta");
                }

                #endregion

                #region Quantidade de registro do resultado

                comando.DbCommand.CommandText = String.Format(@"select count(*) from (select p.id from cnf_fisc_infracao_pergu_respo t, cnf_fisc_infracao_pergunta p, 
															  cnf_fisc_infracao_resposta r where p.id = t.pergunta and r.id = t.resposta " + comandtxt + @" group by p.id)", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                comando.AdicionarParametroEntrada("menor", filtros.Menor);
                comando.AdicionarParametroEntrada("maior", filtros.Maior);

                comandtxt = String.Format(@"select p.id numero_pergunta, p.texto pergunta_texto, 
											stragg(t.resposta) resposta_id, stragg(r.texto) resposta_texto, 
											p.ativo situacao from cnf_fisc_infracao_pergu_respo t, 
											cnf_fisc_infracao_pergunta p, cnf_fisc_infracao_resposta r 
											where p.id = t.pergunta and r.id = t.resposta " + comandtxt +
                                            @" group by p.id, p.texto, p.ativo"
                                    + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    #region Adicionando os dados na classe de retorno

                    PerguntaInfracaoListarResultado entidade;

                    while (reader.Read())
                    {
                        entidade = new PerguntaInfracaoListarResultado();
                        entidade.Id = Convert.ToInt32(reader["numero_pergunta"].ToString());
                        entidade.Texto = reader["pergunta_texto"].ToString();

                        entidade.Resposta = new Resposta()
                        {
                            Id = reader["resposta_id"].ToString(),
                            Texto = reader["resposta_texto"].ToString()
                        };

                        entidade.SituacaoTipoId = Convert.ToInt32(reader["situacao"].ToString());
                        entidade.SituacaoTipoTexto = (entidade.SituacaoTipoId == 1) ? "Ativado" : "Desativado";

                        retorno.Itens.Add(entidade);
                    }

                    reader.Close();

                    #endregion
                }
            }

            return retorno;
        }

        internal Resultados<ConfigFiscalizacao> Filtrar(Filtro<ConfigFiscalizacaoListarFiltro> filtros, BancoDeDados banco = null)
        {
            Resultados<ConfigFiscalizacao> retorno = new Resultados<ConfigFiscalizacao>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                string comandtxt = string.Empty;
                Comando comando = bancoDeDados.CriarComando("");

                #region Adicionando Filtros

                comandtxt += comando.FiltroAnd("t.configuracao_id", "configuracao_id", filtros.Dados.NumeroConfiguracao);
                comandtxt += comando.FiltroAnd("t.classificacao_id", "classificacao_id", filtros.Dados.ClassificacaoId);
                comandtxt += comando.FiltroAnd("t.tipo_id", "tipo_id", filtros.Dados.TipoId);
                comandtxt += comando.FiltroAnd("t.item_id", "item_id", filtros.Dados.ItemId);

                List<String> ordenar = new List<String>();
                List<String> colunas = new List<String>() { "configuracao_id", "classificacao_texto", "tipo_texto", "item_texto" };

                if (filtros.OdenarPor > 0)
                {
                    ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
                }
                else
                {
                    ordenar.Add("numero_fiscalizacao");
                }

                #endregion

                #region Quantidade de registro do resultado

                comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_configuracao_fisc t where 0 = 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                comando.AdicionarParametroEntrada("menor", filtros.Menor);
                comando.AdicionarParametroEntrada("maior", filtros.Maior);

                comandtxt = String.Format(@"
					select t.configuracao_id     Id,
						   t.classificacao_id    ClassificacaoId,
						   t.classificacao_texto ClassificacaoTexto,
						   t.tipo_id             TipoId,
						   t.tipo_texto          TipoTexto,
						   t.item_id             ItemId,
						   t.item_texto          ItemTexto
					  from {0}lst_configuracao_fisc t
					 where 0 = 0"
                + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                retorno.Itens = bancoDeDados.ObterEntityList<ConfigFiscalizacao>(comando);
            }

            return retorno;
        }

		#region Parametrizacao

		internal Parametrizacao ObterParametrizacao(int id)
		{
			Parametrizacao parametrizacao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select  p.id,
											p.codigoreceita,
											lov.texto,
											p.iniciovigencia,
											p.fimvigencia,
											p.valorminimo_pf,
											p.valorminimo_pj,
											p.multa_perc,
											p.juros_perc,
											p.desconto_perc,
											p.desconto_und,
											p.desconto_decor,
											p.tid
										from {0}tab_fisc_parametrizacao p 
										inner join lov_fisc_infracao_codigo_rece lov 
											on (lov.id = p.codigoreceita )
										where p.id = :id order by p.codigoreceita", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						parametrizacao = new Parametrizacao
						{
							Id = reader.GetValue<int>("id"),
							CodigoReceitaId = reader.GetValue<int>("codigoreceita"),
							CodigoReceitaTexto = reader.GetValue<string>("texto"),
							ValorMinimoPF = reader.GetValue<int>("valorminimo_pf"),
							ValorMinimoPJ = reader.GetValue<int>("valorminimo_pj"),
							MultaPercentual = reader.GetValue<int>("multa_perc"),
							JurosPercentual = reader.GetValue<int>("juros_perc"),
							DescontoPercentual = reader.GetValue<int>("desconto_perc"),
							PrazoDescontoUnidade = reader.GetValue<int>("desconto_und"),
							PrazoDescontoDecorrencia = reader.GetValue<int>("desconto_decor"),
						};

						parametrizacao.InicioVigencia.Data = reader.GetValue<DateTime>("iniciovigencia");
						parametrizacao.FimVigencia.Data = reader.GetValue<DateTime>("fimvigencia");
						if (parametrizacao.InicioVigencia.Data.HasValue && parametrizacao.InicioVigencia.Data.Value.Year == 1)
							parametrizacao.InicioVigencia = new DateTecno();
						if (parametrizacao.FimVigencia.Data.HasValue && parametrizacao.FimVigencia.Data.Value.Year == 1)
							parametrizacao.FimVigencia = new DateTecno();
						parametrizacao.ParametrizacaoDetalhes = this.ObterParametrizacaoDetalhe(parametrizacao.Id);
					}

					reader.Close();
				}
			}

			return parametrizacao;
		}

		internal Parametrizacao ObterParametrizacao(int codigoReceita, DateTime data)
		{
			Parametrizacao parametrizacao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select  p.id,
											p.codigoreceita,
											lov.texto,
											p.iniciovigencia,
											p.fimvigencia,
											p.valorminimo_pf,
											p.valorminimo_pj,
											p.multa_perc,
											p.juros_perc,
											p.desconto_perc,
											p.desconto_und,
											p.desconto_decor,
											p.tid
										from {0}tab_fisc_parametrizacao p 
										inner join lov_fisc_infracao_codigo_rece lov 
											on (lov.id = p.codigoreceita )
										where p.codigoreceita = :codigoreceita								
										and p.iniciovigencia <= :data
										and
										(
											p.fimvigencia > :data
											or p.fimvigencia is null
										)
										order by p.codigoreceita", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigoreceita", codigoReceita, DbType.Int32);
				comando.AdicionarParametroEntrada("data", data, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						parametrizacao = new Parametrizacao
						{
							Id = reader.GetValue<int>("id"),
							CodigoReceitaId = reader.GetValue<int>("codigoreceita"),
							CodigoReceitaTexto = reader.GetValue<string>("texto"),
							ValorMinimoPF = reader.GetValue<int>("valorminimo_pf"),
							ValorMinimoPJ = reader.GetValue<int>("valorminimo_pj"),
							MultaPercentual = reader.GetValue<int>("multa_perc"),
							JurosPercentual = reader.GetValue<int>("juros_perc"),
							DescontoPercentual = reader.GetValue<int>("desconto_perc"),
							PrazoDescontoUnidade = reader.GetValue<int>("desconto_und"),
							PrazoDescontoDecorrencia = reader.GetValue<int>("desconto_decor"),
						};

						parametrizacao.InicioVigencia.Data = reader.GetValue<DateTime>("iniciovigencia");
						parametrizacao.FimVigencia.Data = reader.GetValue<DateTime>("fimvigencia");
						if (parametrizacao.InicioVigencia.Data.HasValue && parametrizacao.InicioVigencia.Data.Value.Year == 1)
							parametrizacao.InicioVigencia = new DateTecno();
						if (parametrizacao.FimVigencia.Data.HasValue && parametrizacao.FimVigencia.Data.Value.Year == 1)
							parametrizacao.FimVigencia = new DateTecno();
						parametrizacao.ParametrizacaoDetalhes = this.ObterParametrizacaoDetalhe(parametrizacao.Id);
					}

					reader.Close();
				}
			}

			return parametrizacao;
		}

		internal List<Parametrizacao> ObterParametrizacao()
		{
			List<Parametrizacao> lista = new List<Parametrizacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select  p.id,
											p.codigoreceita,
											lov.texto,
											p.iniciovigencia,
											p.fimvigencia,
											p.valorminimo_pf,
											p.valorminimo_pj,
											p.multa_perc,
											p.juros_perc,
											p.desconto_perc,
											p.desconto_und,
											p.desconto_decor,
											p.tid
										from {0}tab_fisc_parametrizacao p 
										inner join lov_fisc_infracao_codigo_rece lov 
											on (lov.id = p.codigoreceita )
										order by p.codigoreceita", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var parametrizacao = new Parametrizacao
						{
							Id = reader.GetValue<int>("id"),
							CodigoReceitaId = reader.GetValue<int>("codigoreceita"),
							ValorMinimoPF = reader.GetValue<int>("valorminimo_pf"),
							ValorMinimoPJ = reader.GetValue<int>("valorminimo_pj"),
							MultaPercentual = reader.GetValue<int>("multa_perc"),
							JurosPercentual = reader.GetValue<int>("juros_perc"),
							DescontoPercentual = reader.GetValue<int>("desconto_perc"),
							PrazoDescontoUnidade = reader.GetValue<int>("desconto_und"),
							PrazoDescontoDecorrencia = reader.GetValue<int>("desconto_decor"),
						};

						parametrizacao.InicioVigencia.Data = reader.GetValue<DateTime>("iniciovigencia");
						parametrizacao.FimVigencia.Data = reader.GetValue<DateTime>("fimvigencia");
						if (parametrizacao.InicioVigencia.Data.HasValue && parametrizacao.InicioVigencia.Data.Value.Year == 1)
							parametrizacao.InicioVigencia = new DateTecno();
						if (parametrizacao.FimVigencia.Data.HasValue && parametrizacao.FimVigencia.Data.Value.Year == 1)
							parametrizacao.FimVigencia = new DateTecno();
						parametrizacao.ParametrizacaoDetalhes = this.ObterParametrizacaoDetalhe(parametrizacao.Id);

						lista.Add(parametrizacao);
					}

					reader.Close();
				}
			}

			return lista;
		}

		private List<ParametrizacaoDetalhe> ObterParametrizacaoDetalhe(int parametrizacao)
		{
			List<ParametrizacaoDetalhe> lista = new List<ParametrizacaoDetalhe>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select  p.id,
											p.parametrizacao,
											p.valorinicial,
											p.valorfinal,
											p.maximoparcelas,
											p.tid
										from {0}tab_fisc_param_detalhe p
										where p.parametrizacao = :parametrizacao
										order by p.valorinicial", EsquemaBanco);

				comando.AdicionarParametroEntrada("parametrizacao", parametrizacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var parametrizacaoDetalhe = new ParametrizacaoDetalhe
						{
							Id = reader.GetValue<int>("id"),
							ParametrizacaoId = reader.GetValue<int>("parametrizacao"),
							MaximoParcelas = reader.GetValue<int>("maximoparcelas"),
							ValorInicial = reader.GetValue<decimal>("valorinicial"),
							ValorFinal = reader.GetValue<decimal>("valorfinal")
						};

						lista.Add(parametrizacaoDetalhe);
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal Resultados<ParametrizacaoListarResultado> ParametrizacaoFiltrar(Filtro<ParametrizacaoListarFiltro> filtros, BancoDeDados banco = null)
		{
			var retorno = new Resultados<ParametrizacaoListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("p.codigoreceita", "codigoreceita", filtros.Dados.CodigoReceitaId);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "codigoreceita", "iniciovigencia", "fimvigencia", "multa_perc", "juros_perc", "desconto_perc", "desconto_und", "desconto_decor desc" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("codigoreceita");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from (select p.id from tab_fisc_parametrizacao p
															  where 1=1 " + comandtxt + ")", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select p.id,
											p.codigoreceita,
											lov.texto,
											p.iniciovigencia,
											p.fimvigencia,
											p.valorminimo_pf,
											p.valorminimo_pj,
											p.multa_perc,
											p.juros_perc,
											p.desconto_perc,
											p.desconto_und,
											p.desconto_decor,
											p.tid
											from tab_fisc_parametrizacao p
											inner join lov_fisc_infracao_codigo_rece lov 
											 on (lov.id = p.codigoreceita )
											where 1=1 " + comandtxt
									+ DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					ParametrizacaoListarResultado entidade;

					while (reader.Read())
					{
						entidade = new ParametrizacaoListarResultado();
						entidade.Id = Convert.ToInt32(reader["id"].ToString());
						entidade.CodigoReceitaId = Convert.ToInt32(reader["codigoreceita"].ToString());
						entidade.CodigoReceitaTexto = reader["texto"].ToString();
						entidade.ValorMinimoPF = Convert.ToInt32(reader["valorminimo_pf"].ToString());
						entidade.ValorMinimoPJ = Convert.ToInt32(reader["valorminimo_pj"].ToString());
						entidade.MultaPercentual = Convert.ToInt32(reader["multa_perc"]?.ToString());
						entidade.JurosPercentual = Convert.ToInt32(reader["juros_perc"]?.ToString());
						entidade.DescontoPercentual = Convert.ToInt32(reader["desconto_perc"]?.ToString());
						entidade.PrazoDescontoUnidade = Convert.ToInt32(reader["desconto_und"]?.ToString());
						entidade.PrazoDescontoDecorrencia = Convert.ToInt32(reader["desconto_decor"]?.ToString());
						entidade.InicioVigencia = Convert.ToDateTime(reader["iniciovigencia"]?.ToString());
						if(!string.IsNullOrWhiteSpace(reader["fimvigencia"].ToString()))
							entidade.FimVigencia = Convert.ToDateTime(reader["fimvigencia"]?.ToString());

						retorno.Itens.Add(entidade);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}
		#endregion Parametrizacao

		#endregion

		#region Validacoes

		internal bool TipoIsAtivo(int tipoId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}cnf_fisc_infracao_tipo t where t.id = :tipoId and t.ativo = 1", EsquemaBanco);
                comando.AdicionarParametroEntrada("tipoId", tipoId, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool ItemIsAtivo(int itemId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}cnf_fisc_infracao_item t where t.id = :itemId and t.ativo = 1", EsquemaBanco);
                comando.AdicionarParametroEntrada("itemId", itemId, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool SubitemIsAtivo(int subitemId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}cnf_fisc_infracao_subitem t where t.id = :subitemId and t.ativo = 1", EsquemaBanco);
                comando.AdicionarParametroEntrada("subitemId", subitemId, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool PerguntaIsAtivo(int perguntaId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}cnf_fisc_infracao_pergunta t where t.id = :perguntaId and t.ativo = 1", EsquemaBanco);
                comando.AdicionarParametroEntrada("perguntaId", perguntaId, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool RespostaIsAtivo(int respostaId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}cnf_fisc_infracao_resposta t where t.id = :respostaId and t.ativo = 1", EsquemaBanco);
                comando.AdicionarParametroEntrada("respostaId", respostaId, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool CampoIsAtivo(int campoId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}cnf_fisc_infracao_campo t where t.id = :campoId and t.ativo = 1", EsquemaBanco);
                comando.AdicionarParametroEntrada("campoId", campoId, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal List<string> ConfiguracaoEmUso(int configuracaoId)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.fiscalizacao from {0}tab_fisc_infracao t where t.configuracao = :configuracaoId", EsquemaBanco);
                comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);
                return bancoDeDados.ObterEntityList<string>(comando);
            }
        }

        #region Itens de Config. Associados

        internal bool TipoIsAssociadoFiscalizacao(int id)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}hst_fisc_infracao t where t.tipo_id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool ItemIsAssociadoFiscalizacao(int id)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}hst_fisc_infracao t where t.item_id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool SubItemIsAssociadoFiscalizacao(int id)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}hst_fisc_infracao t where t.subitem_id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool CampoIsAssociadoFiscalizacao(int id)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}hst_fisc_infracao_campo t where t.campo_id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool PerguntaIsAssociadoFiscalizacao(int id)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}hst_fisc_infracao_pergunta t where t.pergunta_id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        internal bool RespostaIsAssociadoFiscalizacao(int id)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}hst_fisc_infracao_pergunta t where t.resposta_id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                return bancoDeDados.ExecutarScalar<int>(comando) > 0;
            }
        }

        #endregion

        #endregion

        #region Auxiliares

        public void AlterarSituacaoTipoInfracao(int tipoId, int situacaoNova, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_tipo t set t.ativo = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("situacao", situacaoNova, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", tipoId, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(tipoId, eHistoricoArtefato.tipoinfracao, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        public void AlterarSituacaoItemInfracao(int itemId, int situacaoNova, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_item t set t.ativo = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("situacao", situacaoNova, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", itemId, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(itemId, eHistoricoArtefato.iteminfracao, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        public void AlterarSituacaoSubItemInfracao(int subItemId, int situacaoNova, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_subitem t set t.ativo = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("situacao", situacaoNova, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", subItemId, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(subItemId, eHistoricoArtefato.subiteminfracao, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        public void AlterarSituacaoPenalidade(int Id, int situacaoNova, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_penalidade t set t.ativo = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("situacao", situacaoNova, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(Id, eHistoricoArtefato.campoinfracao, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

		public void AlterarSituacaoCampoInfracao(int campoId, int situacaoNova, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_campo t set t.ativo = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("situacao", situacaoNova, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", campoId, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(campoId, eHistoricoArtefato.campoinfracao, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        public void AlterarSituacaoRespostaInfracao(int RespostaId, int situacaoNova, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_resposta t set t.ativo = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("situacao", situacaoNova, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", RespostaId, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(RespostaId, eHistoricoArtefato.respostainfracao, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        public void AlterarSituacaoPerguntaInfracao(int perguntaId, int situacaoNova, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}cnf_fisc_infracao_pergunta t set t.ativo = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("situacao", situacaoNova, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", perguntaId, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(perguntaId, eHistoricoArtefato.perguntainfracao, eHistoricoAcao.atualizar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        internal List<String> ObterIdsConfiguracoesAssociadasTipoInfracao(int tipo)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                List<String> ids = new List<String>();

                Comando comando = bancoDeDados.CriarComando(@"select i.id configuracao from cnf_fisc_infracao i where i.tipo = :tipo", EsquemaBanco);

                comando.AdicionarParametroEntrada("tipo", tipo, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        if (reader["configuracao"] != null && !Convert.IsDBNull(reader["configuracao"]))
                        {
                            ids.Add("Nº " + reader["configuracao"].ToString());

                        }
                    }

                    reader.Close();
                }

                return ids;
            }
        }

        internal List<String> ObterIdsConfiguracoesAssociadasItemInfracao(int item)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                List<String> ids = new List<String>();

                Comando comando = bancoDeDados.CriarComando(@"select i.id configuracao from cnf_fisc_infracao i where i.item = :item", EsquemaBanco);

                comando.AdicionarParametroEntrada("item", item, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        if (reader["configuracao"] != null && !Convert.IsDBNull(reader["configuracao"]))
                        {
                            ids.Add("Nº " + reader["configuracao"].ToString());

                        }
                    }

                    reader.Close();
                }

                return ids;
            }
        }

        internal List<String> ObterIdsConfiguracoesAssociadasSubItemInfracao(int subItem)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                List<String> ids = new List<String>();

                Comando comando = bancoDeDados.CriarComando(@"select s.configuracao from cnf_fisc_infr_cnf_subitem s where s.subitem = :subitem", EsquemaBanco);

                comando.AdicionarParametroEntrada("subitem", subItem, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        if (reader["configuracao"] != null && !Convert.IsDBNull(reader["configuracao"]))
                        {
                            ids.Add("Nº " + reader["configuracao"].ToString());

                        }
                    }

                    reader.Close();
                }

                return ids;
            }
        }

        internal List<String> ObterIdsConfiguracoesAssociadasCampoInfracao(int campo)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                List<String> ids = new List<String>();

                Comando comando = bancoDeDados.CriarComando(@"select c.configuracao from cnf_fisc_infr_cnf_campo c where c.campo = :campo", EsquemaBanco);

                comando.AdicionarParametroEntrada("campo", campo, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        if (reader["configuracao"] != null && !Convert.IsDBNull(reader["configuracao"]))
                        {
                            ids.Add("Nº " + reader["configuracao"].ToString());

                        }
                    }

                    reader.Close();
                }

                return ids;
            }
        }

        internal List<String> ObterIdsConfiguracoesAssociadasPerguntaInfracao(int pergunta)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                List<String> ids = new List<String>();

                Comando comando = bancoDeDados.CriarComando(@"select c.configuracao from cnf_fisc_infr_cnf_pergunta c where c.pergunta = :pergunta", EsquemaBanco);

                comando.AdicionarParametroEntrada("pergunta", pergunta, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        if (reader["configuracao"] != null && !Convert.IsDBNull(reader["configuracao"]))
                        {
                            ids.Add("Nº " + reader["configuracao"].ToString());

                        }
                    }

                    reader.Close();
                }

                return ids;
            }
        }

        internal List<String> ObterIdsPerguntasAssociadasRespostaInfracao(int resposta)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                List<String> ids = new List<String>();

                Comando comando = bancoDeDados.CriarComando(@"select s.pergunta from {0}cnf_fisc_infracao_pergu_respo s where s.resposta = :resposta", EsquemaBanco);

                comando.AdicionarParametroEntrada("resposta", resposta, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        if (reader["pergunta"] != null && !Convert.IsDBNull(reader["pergunta"]))
                        {
                            ids.Add("Nº " + reader["pergunta"].ToString());

                        }
                    }

                    reader.Close();
                }

                return ids;
            }
        }

        #endregion
    }
}
