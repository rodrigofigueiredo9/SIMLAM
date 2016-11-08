using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class RequerimentoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        Consulta _consulta = new Consulta();
        public Consulta Consulta { get { return _consulta; } }
        
        Historico _historico = new Historico();

        public Historico Historico { get { return _historico; }}

        private string EsquemaBanco { get { return _configSys.UsuarioInterno; } }

        public RequerimentoDa() { }

        internal int? Importar(Requerimento requerimento, BancoDeDados bancoInterno) 
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
            {
                #region Requerimento

                Comando comando;

                bancoDeDados.IniciarTransacao();

                comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento e (id, numero, data_criacao, situacao, tid, agendamento, setor, interessado, empreendimento, informacoes, autor) 
				values (:requerimento, :requerimento, sysdate, :situacao, :tid, :agendamento, :setor, :interessado, :empreendimento, :informacoes, :autor)", EsquemaBanco);

                comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("situacao", (int)eRequerimentoSituacao.Finalizado, DbType.Int32); // 1 - Finalizado
                comando.AdicionarParametroEntrada("agendamento", requerimento.AgendamentoVistoria, DbType.Int32);
                comando.AdicionarParametroEntrada("setor", requerimento.SetorId, DbType.Int32);//Departamento de Recursos Naturais Renováveis
                comando.AdicionarParametroEntrada("interessado", requerimento.Interessado.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("empreendimento", (requerimento.Empreendimento.Id > 0) ? (object)requerimento.Empreendimento.Id : DBNull.Value, DbType.Int32);
                comando.AdicionarParametroEntrada("informacoes", DbType.String, 500, requerimento.Informacoes);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("autor", requerimento.CredenciadoId, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                #region Atividades

                if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
                {
                    foreach (Atividade item in requerimento.Atividades)
                    {
                        comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_atividade t (id, requerimento, atividade, tid)
						values ({0}seq_requerimento_atividade.nextval, :requerimento, :atividade, :tid) returning t.id into :id", EsquemaBanco);
                        comando.AdicionarParametroSaida("id", DbType.Int32);

                        comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));

                        #region Modelo de Título

                        if (item.Finalidades.Count > 0)
                        {
                            foreach (Finalidade itemAux in item.Finalidades)
                            {
                                comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_ativ_finalida (id, requerimento_ativ, modelo,
								titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
								({0}seq_requerimento_ativ_fin.nextval, :requerimento_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
								:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", EsquemaBanco);

                                comando.AdicionarParametroEntrada("requerimento_ativ", item.IdRelacionamento, DbType.Int32);
                                comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
                                comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
                                comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
                                comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
                                comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
                                comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
                                comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
                                comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);
                                comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
                                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                                comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);

                                bancoDeDados.ExecutarNonQuery(comando);
                            }
                        }

                        #endregion
                    }
                }

                #endregion

                #region Responsáveis

                if (requerimento.Responsaveis != null && requerimento.Responsaveis.Count > 0)
                {
                    foreach (ResponsavelTecnico responsavel in requerimento.Responsaveis)
                    {
                        comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_responsavel(id, requerimento, responsavel, funcao, numero_art, tid) values
						({0}seq_requerimento_responsavel.nextval, :requerimento, :responsavel, :funcao, :numero_art, :tid )", EsquemaBanco);

                        comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("responsavel", responsavel.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("funcao", responsavel.Funcao, DbType.Int32);
                        comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt, DbType.String);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"delete {0}tab_requerimento_responsavel p where p.requerimento = :id", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);
                }
                #endregion

                #region Histórico
                // Historico
                Historico.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, eHistoricoAcao.importar, bancoDeDados);
                Consulta.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, bancoDeDados);

                #endregion

                bancoDeDados.Commit();

                return requerimento.Id;
            }
        }
    }
}
