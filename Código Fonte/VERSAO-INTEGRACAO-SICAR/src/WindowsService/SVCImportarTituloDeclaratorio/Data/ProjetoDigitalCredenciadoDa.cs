using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class ProjetoDigitalCredenciadoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

        Historico _historico = new Historico();

        public Historico Historico { get { return _historico; } }

        private string EsquemaBanco { get { return _configSys.UsuarioCredenciado; } }

        public ProjetoDigitalCredenciadoDa() { }

        public ProjetoDigital Obter(int id, BancoDeDados bancoCredenciado, string tid = null)
        {
            ProjetoDigital projeto = new ProjetoDigital();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Projeto Digital

                if (String.IsNullOrWhiteSpace(tid))
                {
                    projeto = Obter(idProjeto: id, bancoCredenciado: bancoDeDados);
                }
                else
                {
                    Comando comando = bancoDeDados.CriarComando(@"select count(r.id) existe from {0}tab_projeto_digital r where r.id = :id and r.tid = :tid", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

                    if (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando)))
                    {
                        projeto = Obter(idProjeto: id, bancoCredenciado: bancoDeDados);
                    }
                    else
                    {
                        projeto = ObterHistorico(id, tid, bancoDeDados);
                    }
                }

                #endregion
            }

            return projeto;
        }

        internal ProjetoDigital ObterHistorico(int id, string tid, BancoDeDados bancoCredenciado)
        {
            ProjetoDigital projeto = new ProjetoDigital();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();
                
                #region Projeto Digital

                Comando comando = bancoDeDados.CriarComando(@"select p.id, p.projeto_id, p.tid, p.etapa, p.situacao_id, p.requerimento_id, p.requerimento_tid, 
				p.empreendimento_id, p.empreendimento_tid, p.data_criacao, p.data_envio, p.credenciado_id, p.credenciado_tid, p.motivo_recusa 
				from {0}hst_projeto_digital p where p.projeto_id = :id and p.tid = :tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        projeto.Id = reader.GetValue<int>("projeto_id");
                        projeto.Tid = reader.GetValue<string>("tid");
                        projeto.Etapa = reader.GetValue<int>("etapa");
                        projeto.Situacao = reader.GetValue<int>("situacao_id");
                        projeto.RequerimentoId = reader.GetValue<int>("requerimento_id");
                        projeto.RequerimentoTid = reader.GetValue<string>("requerimento_tid");
                        projeto.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
                        projeto.EmpreendimentoTid = reader.GetValue<string>("empreendimento_tid");
                        projeto.DataCriacao.Data = reader.GetValue<DateTime>("data_criacao");
                        projeto.DataEnvio.Data = reader.GetValue<DateTime>("data_envio");
                        projeto.CredenciadoId = reader.GetValue<int>("credenciado_id");
                        projeto.CredenciadoTid = reader.GetValue<string>("credenciado_tid");
                        projeto.MotivoRecusa = reader.GetValue<string>("motivo_recusa");
                    }

                    reader.Close();
                }

                #endregion
            }

            return projeto;
        }

        internal ProjetoDigital Obter(int idProjeto = 0, int idRequerimento = 0, BancoDeDados bancoCredenciado = null)
        {
            ProjetoDigital projeto = new ProjetoDigital();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                #region Projeto Digital

                Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.etapa, p.situacao, l.texto situacao_texto, p.requerimento, p.empreendimento, 
				p.data_criacao, p.data_envio, p.credenciado, p.motivo_recusa from {0}tab_projeto_digital p, {0}lov_projeto_digital_situacao l where p.situacao = l.id", EsquemaBanco);

                if (idRequerimento > 0)
                {
                    comando.DbCommand.CommandText += comando.FiltroAnd("p.requerimento", "requerimento", idRequerimento);
                }
                else
                {
                    comando.DbCommand.CommandText += comando.FiltroAnd("p.id", "id", idProjeto);
                }

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        projeto.Id = reader.GetValue<int>("id");
                        projeto.Tid = reader.GetValue<string>("tid");
                        projeto.Situacao = reader.GetValue<int>("situacao");
                        projeto.SituacaoTexto = reader.GetValue<string>("situacao_texto");
                        projeto.Etapa = reader.GetValue<int>("etapa");
                        projeto.RequerimentoId = reader.GetValue<int>("requerimento");
                        projeto.EmpreendimentoId = reader.GetValue<int>("empreendimento");
                        projeto.DataCriacao.Data = reader.GetValue<DateTime>("data_criacao");
                        projeto.DataEnvio.Data = reader.GetValue<DateTime>("data_envio");
                        projeto.CredenciadoId = reader.GetValue<int>("credenciado");
                        projeto.MotivoRecusa = reader.GetValue<String>("motivo_recusa");
                    }

                    reader.Close();
                }

                #endregion

                #region Dependencias

                if (projeto.Id > 0)
                {
                    projeto.Dependencias = ObterDependencias(projeto.Id, bancoCredenciado);
                }

                #endregion
            }

            return projeto;
        }

        public List<Dependencia> ObterDependencias(int projetoDigitalID, BancoDeDados bancoCredenciado)
        {
            List<Dependencia> lista = new List<Dependencia>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select d.dependencia_tipo, d.dependencia_caracterizacao, lc.texto dependencia_carac_texto, d.dependencia_id, d.dependencia_tid 
				from {0}tab_proj_digital_dependencias d, {0}lov_caracterizacao_tipo lc where d.dependencia_caracterizacao = lc.id and d.projeto_digital_id = :projeto_digital_id", EsquemaBanco);

                comando.AdicionarParametroEntrada("projeto_digital_id", projetoDigitalID, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Dependencia dependencia = null;

                    while (reader.Read())
                    {
                        dependencia = new Dependencia();

                        dependencia.DependenciaTipo = reader.GetValue<int>("dependencia_tipo");
                        dependencia.DependenciaCaracterizacao = reader.GetValue<int>("dependencia_caracterizacao");
                        dependencia.DependenciaCaracterizacaoTexto = reader.GetValue<string>("dependencia_carac_texto");
                        dependencia.DependenciaId = reader.GetValue<int>("dependencia_id");
                        dependencia.DependenciaTid = reader.GetValue<string>("dependencia_tid");

                        lista.Add(dependencia);
                    }

                    reader.Close();
                }
            }

            return lista;
        }
        

        internal void AlterarSituacao(ProjetoDigital projeto, BancoDeDados bancoCredenciado)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.situacao = :situacao, r.tid = :tid where r.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("situacao", projeto.Situacao, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);
                //Historico
                Historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.alterarsituacao, bancoDeDados);

                bancoDeDados.Commit();
            }
        }
    }
}
