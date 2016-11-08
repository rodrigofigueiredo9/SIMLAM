using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class RoteiroInternoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();
        public string EsquemaBanco
        {
            get { return _configSys.UsuarioInterno; }
        }

        public RoteiroInternoDa() { }

        public Atividade ObterAtividade(Atividade atividade, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select a.id Id, a.setor SetorId, a.atividade NomeAtividade from {0}tab_atividade a where a.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", atividade.Id, DbType.Int32);
                atividade = bancoDeDados.ObterEntity<Atividade>(comando);
                return atividade;
            }
        }

        internal int ObterFinalidadeCodigo(int finalidadeId, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select lr.codigo from {0}lov_titulo_finalidade lr where lr.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", finalidadeId, DbType.Int32);

                return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
            }
        }

        internal bool VerificarTidAtual(int id, string tid, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select r.tid from {0}tab_roteiro r where r.id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                string tidAtual = bancoDeDados.ExecutarScalar<string>(comando);

                return tidAtual.Equals(tid, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        internal Roteiro ObterSimplificado(int id, BancoDeDados bancoInterno)
        {
            Roteiro roteiro = new Roteiro();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Roteiro
                Comando comando = bancoDeDados.CriarComando(@"select a.numero, a.versao versao_atual, a.nome, a.setor setor_id, s.nome setor_texto, a.situacao, a.observacoes, a.finalidade,
				a.data_criacao, a.tid, (select r.versao from hst_roteiro r where r.tid = a.tid and r.roteiro_id = a.id) versao
				from {0}tab_roteiro a, {0}tab_setor s where a.setor = s.id and a.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        roteiro.Id = id;
                        roteiro.Tid = reader["tid"].ToString();
                        roteiro.Versao = Convert.ToInt32(reader["versao_atual"]);
                        roteiro.VersaoAtual = Convert.ToInt32(reader["versao"]);
                        roteiro.Nome = reader["nome"].ToString();
                        roteiro.Padrao = ListaCredenciadoBus.RoteiroPadrao.Exists(x => x.Id == roteiro.Id);

                        if (reader["finalidade"] != null && !Convert.IsDBNull(reader["finalidade"]))
                        {
                            roteiro.Finalidade = Convert.ToInt32(reader["finalidade"]);
                        }

                        if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
                        {
                            roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
                            roteiro.SetorNome = reader["setor_texto"].ToString();
                        }
                        roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
                        roteiro.Observacoes = reader["observacoes"].ToString();
                        roteiro.DataCriacao = Convert.ToDateTime(reader["data_criacao"]);
                    }
                    reader.Close();
                }
                #endregion

                #region Itens do roteiro
                comando = bancoDeDados.CriarComando(@"select tri.id, i.ordem, tri.nome, tri.condicionante, tri.procedimento, tri.tipo, lrip.texto tipo_texto, tri.tid
				from {0}tab_roteiro_itens i, {0}tab_roteiro_item tri, {0}lov_roteiro_item_tipo lrip where 
				i.item = tri.id and tri.tipo = lrip.id and i.roteiro = :roteiro order by i.ordem", EsquemaBanco);

                comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Item item;
                    while (reader.Read())
                    {
                        item = new Item();
                        item.Id = Convert.ToInt32(reader["id"]);
                        item.Nome = reader["nome"].ToString();
                        item.Condicionante = reader["condicionante"].ToString();
                        item.Ordem = Convert.ToInt32(reader["ordem"]);
                        item.ProcedimentoAnalise = reader["procedimento"].ToString();
                        if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
                        {
                            item.Tipo = Convert.ToInt32(reader["tipo"]);
                            item.TipoTexto = reader["tipo_texto"].ToString();
                        }
                        item.Tid = reader["tid"].ToString();
                        roteiro.Itens.Add(item);
                    }
                    reader.Close();
                }
                #endregion
            }
            return roteiro;
        }

        internal Roteiro ObterHistoricoSimplificado(int id, string tdi, BancoDeDados bancoInterno)
        {
            Roteiro roteiro = new Roteiro();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Roteiro
                Comando comando = bancoDeDados.CriarComando(@"select a.id, a.numero, a.versao, (select tr.versao from tab_roteiro tr where tr.id = a.roteiro_id) versao_atual, 
				a.nome, a.setor_id, s.nome setor_texto, a.situacao_id situacao, a.observacoes, a.finalidade, a.data_criacao, a.tid from {0}hst_roteiro a, {0}hst_setor s 
				where a.roteiro_id = :id and a.tid = :tidRoteiro and a.setor_id = s.setor_id and a.setor_tid = s.tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tdi);


                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        roteiro.Id = id;
                        roteiro.Tid = reader["tid"].ToString();
                        roteiro.IdRelacionamento = Convert.ToInt32(reader["id"]);
                        roteiro.Versao = Convert.ToInt32(reader["versao"]);
                        if (reader["versao_atual"] != null && !Convert.IsDBNull(reader["versao_atual"]))
                        {
                            roteiro.VersaoAtual = Convert.ToInt32(reader["versao_atual"]);
                        }

                        roteiro.Nome = reader["nome"].ToString();

                        if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
                        {
                            roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
                            roteiro.SetorNome = reader["setor_texto"].ToString();
                        }

                        if (reader["finalidade"] != null && !Convert.IsDBNull(reader["finalidade"]))
                        {
                            roteiro.Finalidade = Convert.ToInt32(reader["finalidade"]);
                        }

                        roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
                        roteiro.Observacoes = reader["observacoes"].ToString();
                        roteiro.DataCriacao = Convert.ToDateTime(reader["data_criacao"]);
                    }
                    reader.Close();
                }
                #endregion

                //Valida se houve retorno
                if (roteiro.Id > 0)
                {
                    #region Itens do roteiro
                    comando = bancoDeDados.CriarComando(@"select trhi.id_hst id, trhi.ordem, tri.nome, tri.condicionante, tri.procedimento, tri.tipo_id tipo, tri.tipo_texto, trhi.item_tid tid
					from {0}hst_roteiro_itens trhi, {0}hst_roteiro_item tri where trhi.roteiro_id = :roteiro and trhi.tid = :tidRoteiro and trhi.item_id = tri.item_id 
					and trhi.item_tid = tri.tid order by trhi.ordem", EsquemaBanco);

                    comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tdi);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        Item item;
                        while (reader.Read())
                        {
                            item = new Item();
                            item.Id = Convert.ToInt32(reader["id"]);
                            item.Nome = reader["nome"].ToString();
                            item.Condicionante = reader["condicionante"].ToString();
                            item.Ordem = Convert.ToInt32(reader["ordem"]);
                            item.ProcedimentoAnalise = reader["procedimento"].ToString();
                            item.Tipo = Convert.ToInt32(reader["tipo"]);
                            item.TipoTexto = reader["tipo_texto"].ToString();
                            item.Tid = reader["tid"].ToString();
                            roteiro.Itens.Add(item);
                        }
                        reader.Close();
                    }
                    #endregion
                }
            }
            return roteiro;
        }

        internal String ModeloTituloNaoAdicionadoRoteiro(Finalidade finalidade, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {

                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select tm.nome from cnf_atividade_atividades caa, cnf_atividade_modelos cam, 
															tab_titulo_modelo tm where caa.atividade = :atividade and cam.configuracao = caa.configuracao and 
															cam.modelo = :modelo and tm.id = cam.modelo and cam.modelo not in (select rm.modelo from 
															tab_roteiro_modelos rm, tab_roteiro_atividades ra where rm.roteiro = ra.roteiro and 
															ra.atividade = :atividade)", EsquemaBanco);

                comando.AdicionarParametroEntrada("atividade", finalidade.AtividadeId, DbType.Int32);
                comando.AdicionarParametroEntrada("modelo", finalidade.TituloModelo, DbType.Int32);

                return bancoDeDados.ExecutarScalar<String>(comando);
            }
        }

        internal Finalidade ObterFinalidade(Finalidade finalidade, BancoDeDados bancoInterno)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select lr.id Id, lr.codigo Codigo, lr.texto Texto, tm.nome TituloModeloTexto
				from {0}lov_titulo_finalidade lr, {0}tab_titulo_modelo tm where lr.id = :id and tm.id = :modelo", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", finalidade.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("modelo", finalidade.TituloModelo, DbType.Int32);

                finalidade = bancoDeDados.ObterEntity<Finalidade>(comando);
                return finalidade;
            }
        }

        internal Roteiro ObterRoteirosPorAtividades(Finalidade finalidade, BancoDeDados bancoInterno)
        {
            Roteiro roteiro = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
											select c.id, c.nome, c.versao, a.atividade atividade_nome, c.tid
												from {0}tab_roteiro            c,
													{0}tab_roteiro_atividades d,
													{0}tab_atividade          a,
													{0}tab_roteiro_modelos    g
												where c.id = d.roteiro
												and c.id = g.roteiro
												and d.atividade = a.id
												and c.situacao = 1
												and d.atividade = :atividade
												and bitand(c.finalidade, :finalidade) > 0
												and g.modelo = :modelo", EsquemaBanco);

                if (finalidade.AtividadeSetorId > 0)
                {
                    comando.DbCommand.CommandText += " and a.setor = :setor";
                    comando.AdicionarParametroEntrada("setor", finalidade.AtividadeSetorId, DbType.Int32);
                }

                comando.AdicionarParametroEntrada("atividade", finalidade.AtividadeId, DbType.Int32);
                comando.AdicionarParametroEntrada("finalidade", finalidade.Codigo, DbType.Int32);
                comando.AdicionarParametroEntrada("modelo", finalidade.TituloModelo, DbType.Int32);

                comando.DbCommand.CommandText += " order by c.id";

                #region busca todos os roteiros relacionados com a atividade/modelo/finalidade

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        roteiro = new Roteiro();
                        roteiro.Tid = reader["tid"].ToString();
                        roteiro.Id = Convert.ToInt32(reader["id"]);
                        roteiro.Nome = reader["nome"].ToString();
                        roteiro.VersaoAtual = Convert.ToInt32(reader["versao"]);
                        roteiro.AtividadeTexto = reader["atividade_nome"].ToString();
                    }
                    reader.Close();
                }
                #endregion
            }
            return roteiro;
        }
    }
}
