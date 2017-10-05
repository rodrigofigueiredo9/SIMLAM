using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Scheduler.misc;
using Tecnomapas.EtramiteX.Scheduler.misc.WKT;
using Tecnomapas.EtramiteX.Scheduler.models;
using Tecnomapas.EtramiteX.Scheduler.models.misc;
using Tecnomapas.EtramiteX.Scheduler.models.simlam;
using Tecnomapas.EtramiteX.Scheduler.jobs.Class;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Oracle.ManagedDataAccess.Client;

using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
    public class CARSolicitacaoFunc
    {
        private ActionResult Json(object p, JsonRequestBehavior jsonRequestBehavior)
        {
            throw new NotImplementedException();
        }
        private String EsquemaBanco { get; set; }
        #region Enviar/Reenviar
        GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
        public string UsuarioCredenciado
        {
            get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
        }

        public ActionResult EnviarReenviarArquivoSICAR(int solicitacaoId, int origem, bool isEnviar, OracleConnection conn)
        {
            CARSolicita solicitacao = null;

            if (origem == 1)  //OBTEM AS INFORMAÇÕES DO CAR INSTITUCIONAL
                solicitacao = ObterInst(solicitacaoId, conn);
            else              //OBTEM AS INFORMAÇÕES DO CAR CREDENCIADO  
                solicitacao = ObterCred(solicitacaoId, conn);

            if (!AcessoEnviarReenviarArquivoSICAR(solicitacao, origem)) //VERIFICAR SE O ARQUIVO .CAR "PASSIVO" É INVALIDO OU EM CADASTRO
            {
                return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
            }

            if (origem == (int)eCARSolicitacaoOrigem.Credenciado)   // Com dados do credenciado: INSERE NA TABELA TAB_SCHEDULER_FILA
            {
                EnviarReenviarArquivoSICARCred(solicitacaoId, isEnviar, conn);
            }
            else  // Com dados do Institucional: INSERE NA TABELA TAB_SCHEDULER_FILA
            {
                EnviarReenviarArquivoSICARInst(solicitacaoId, isEnviar, conn);
            }
            return null;
            //return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
        }



        #endregion Enviar/Reenviar



        public CARSolicita ObterInst(int id, OracleConnection conn, bool simplificado = false, string tid = null)//, BancoDeDados banco = null)
        {
            try
            {
                if (tid == null)
                {
                    return daObterInst(id, conn, simplificado);
                }
                else
                {
                    return ObterHistoricoInst(id, tid, conn, simplificado);
                }
            }
            catch (Exception exc)
            {
                string str = exc.Message;
                //Validacao.AddErro(exc);
            }

            return null;
        }

        //
        internal CARSolicita daObterInst(int id, OracleConnection conn, bool simplificado = false)//BancoDeDados banco = bancoInstitucional)
        {
            CARSolicita solicitacao = new CARSolicita();

            //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            //{
            using (OracleCommand command = new OracleCommand(
            #region Solicitação

                //Comando comando = bancoDeDados.CriarComando(@"
                "select s.tid," +
                    "s.numero," +
                    "s.data_emissao," +
                    "s.situacao_data," +
                    "l.id situacao," +
                    "l.texto situacao_texto," +
                    "s.situacao_anterior," +
                    "la.texto situacao_anterior_texto," +
                    "s.situacao_anterior_data," +
                    "p.id protocolo_id," +
                    "p.protocolo," +
                    "p.numero protocolo_numero," +
                    "p.ano protocolo_ano," +
                    "nvl(pes.nome, pes.razao_social) declarante_nome_razao," +
                    "ps.id protocolo_selecionado_id," +
                    "ps.protocolo protocolo_selecionado," +
                    "ps.numero protocolo_selecionado_numero," +
                    "ps.ano protocolo_selecionado_ano," +
                    "s.requerimento," +
                    "s.atividade," +
                    "e.id empreendimento_id," +
                    "e.denominador empreendimento_nome," +
                    "e.codigo empreendimento_codigo," +
                    "s.declarante," +
                    "f.funcionario_id autor_id," +
                    "f.nome autor_nome," +
                    "(select stragg_barra(sigla) from hst_setor where " +
                    "setor_id in (select fs.setor_id from hst_funcionario_setor fs where fs.id_hst = f.id)" +
                    "and tid in (select fs.setor_tid from hst_funcionario_setor fs where fs.id_hst = f.id )) autor_setor," +
                    "'Institucional' autor_modulo," +
                    "s.autor," +
                    "s.motivo," +
                    "tr.data_criacao requerimento_data_cadastro," +
                    "pg.id projeto_geo_id" +
                " from tab_car_solicitacao         s," +
                    "lov_car_solicitacao_situacao l," +
                    "lov_car_solicitacao_situacao la," +
                    "tab_protocolo                p," +
                    "tab_protocolo                ps," +
                    "tab_empreendimento           e," +
                    "crt_projeto_geo              pg," +
                    "tab_pessoa                   pes," +
                    "tab_requerimento             tr," +
                    "hst_funcionario              f " +
                " where s.situacao = l.id" +
                " and s.situacao_anterior = la.id(+)" +
                " and s.protocolo = p.id" +
                " and s.protocolo_selecionado = ps.id(+)" +
                " and s.empreendimento = e.id" +
                " and s.empreendimento = pg.empreendimento" +
                " and s.declarante = pes.id" +
                " and s.requerimento = tr.id" +
                " and pg.caracterizacao = 1" +
                " and f.funcionario_id = s.autor" +
                " and f.tid = (select autor_tid from hst_car_solicitacao where acao_executada = 342 and solicitacao_id = s.id)" +
                " and s.id = :id", conn))
            {
                //comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                command.Parameters.Add(new OracleParameter("id", id));
                //using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                //{
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        solicitacao.Id = id; //Convert.ToInt32(dr["pessoa_id"]);
                        solicitacao.Tid = Convert.ToString(reader["tid"]); //reader.GetValue<String>("tid");
                        solicitacao.Numero = Convert.ToString(reader["numero"]);//reader.GetValue<String>("numero");
                        solicitacao.DataEmissao.DataTexto = Convert.ToString(reader["data_emissao"]); //reader.GetValue<String>("data_emissao");
                        solicitacao.SituacaoId = Convert.ToInt32(reader["situacao"]); //reader.GetValue<Int32>("situacao");
                        solicitacao.SituacaoTexto = Convert.ToString(reader["situacao_texto"]); //reader.GetValue<String>("situacao_texto");
                        solicitacao.DataSituacao.DataTexto = Convert.ToString(reader["situacao_data"]); //reader.GetValue<String>("situacao_data");
                        solicitacao.SituacaoAnteriorId = Convert.ToInt32(reader["situacao_anterior"]); //reader.GetValue<Int32>("situacao_anterior");
                        solicitacao.SituacaoAnteriorTexto = Convert.ToString(reader["situacao_anterior_texto"]); //reader.GetValue<String>("situacao_anterior_texto");
                        solicitacao.DataSituacaoAnterior.DataTexto = Convert.ToString(reader["situacao_anterior_data"]); //reader.GetValue<String>("situacao_anterior_data");

                        solicitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]); //reader.GetValue<Int32>("protocolo_id");
                        solicitacao.Protocolo.IsProcesso = Convert.ToBoolean(reader["protocolo"]); //reader.GetValue<Int32>("protocolo") == 1;
                        solicitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]); //reader.GetValue<Int32?>("protocolo_numero");
                        solicitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]); //reader.GetValue<Int32>("protocolo_ano");

                        solicitacao.ProtocoloSelecionado.Id = Convert.ToInt32(reader["protocolo_selecionado_id"]); //reader.GetValue<Int32>("protocolo_selecionado_id");
                        solicitacao.ProtocoloSelecionado.IsProcesso = Convert.ToBoolean(reader["protocolo_selecionado"]); //reader.GetValue<Int32>("protocolo_selecionado") == 1;
                        solicitacao.ProtocoloSelecionado.NumeroProtocolo = Convert.ToInt32(reader["protocolo_selecionado_numero"]); //reader.GetValue<Int32?>("protocolo_selecionado_numero");
                        solicitacao.ProtocoloSelecionado.Ano = Convert.ToInt32(reader["protocolo_selecionado_ano"]); //reader.GetValue<Int32>("protocolo_selecionado_ano");
                        solicitacao.Requerimento.Id = Convert.ToInt32(reader["requerimento"]); //reader.GetValue<Int32>("requerimento");
                        solicitacao.Requerimento.DataCadastro = Convert.ToDateTime(reader["requerimento_data_cadastro"]); //reader.GetValue<DateTime>("requerimento_data_cadastro");
                        solicitacao.Atividade.Id = Convert.ToInt32(reader["atividade"]); //reader.GetValue<Int32>("atividade");
                        solicitacao.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]); //reader.GetValue<Int32>("empreendimento_id");
                        solicitacao.Empreendimento.NomeRazao = Convert.ToString(reader["empreendimento_nome"]); //reader.GetValue<String>("empreendimento_nome");
                        solicitacao.Empreendimento.Codigo = Convert.ToInt64(reader["empreendimento_codigo"]); //reader.GetValue<Int64?>("empreendimento_codigo");
                        solicitacao.Declarante.Id = Convert.ToInt32(reader["declarante"]); //reader.GetValue<Int32>("declarante");
                        solicitacao.Declarante.NomeRazaoSocial = Convert.ToString(reader["declarante_nome_razao"]); //reader.GetValue<String>("declarante_nome_razao");

                        solicitacao.AutorId = Convert.ToInt32(reader["autor_id"]); //reader.GetValue<Int32>("autor_id");
                        solicitacao.AutorNome = Convert.ToString(reader["autor_nome"]); //reader.GetValue<String>("autor_nome");
                        solicitacao.AutorSetorTexto = Convert.ToString(reader["autor_setor"]); //reader.GetValue<String>("autor_setor");
                        solicitacao.AutorModuloTexto = Convert.ToString(reader["autor_modulo"]); //reader.GetValue<String>("autor_modulo");

                        solicitacao.Motivo = Convert.ToString(reader["motivo"]); //reader.GetValue<String>("motivo");
                        solicitacao.ProjetoId = Convert.ToInt32(reader["projeto_geo_id"]); //reader.GetValue<Int32>("projeto_geo_id");
                    }

                    reader.Close();
                }
            }

            return solicitacao;
            #endregion
        }

        //

        public CARSolicita ObterCred(int id, OracleConnection conn)//BancoDeDados banco = null)
        {
            try
            {
                return daObterCred(id, conn);
            }
            catch (Exception exc)
            {
                //Validacao.AddErro(exc);
                string str = exc.Message;
            }
            return null;
        }
        //

        internal CARSolicita ObterHistoricoInst(int id, string tid, OracleConnection conn, bool simplificado = false) //BancoDeDados banco = null)
        {
            #region Solicitação
            CARSolicita solicitacao = new CARSolicita();

            //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            //{
            using (OracleCommand command = new OracleCommand(
           

                //Comando comando = bancoDeDados.CriarComando(@"
                "select s.tid," +
                   "s.numero," +
                   "s.data_emissao," +
                   "s.situacao_id," +
                   "s.situacao_texto," +
                   "s.situacao_data," +
                   "s.situacao_anterior_id," +
                   "s.situacao_anterior_texto," +
                   "s.situacao_anterior_data," +
                   "p.id_protocolo," +
                   "p.protocolo_id," +
                   "p.numero protocolo_numero," +
                   "p.ano protocolo_ano," +
                   "ps.id_protocolo id_protocolo_selecionado," +
                   "ps.protocolo_id protocolo_selecionado_id," +
                   "ps.numero protocolo_selecionado_numero," +
                   "ps.ano protocolo_selecionado_ano," +
                   "r.requerimento_id," +
                   "r.data_criacao requerimento_data_criacao," +
                   "a.id atividade_id," +
                   "a.atividade atividade_texto," +
                   "e.empreendimento_id," +
                   "e.denominador empreendimento_nome," +
                   "e.codigo empreendimento_codigo," +
                   "s.declarante_id," +

                   "f.funcionario_id autor_id," +
                   "f.tid autor_tid," +
                   "f.nome autor_nome," +
                   "(select stragg_barra(sigla) from hst_setor where " +
                   "setor_id in (select fs.setor_id from hst_funcionario_setor fs where fs.id_hst = f.id)" +
                   "and tid in (select fs.setor_tid from hst_funcionario_setor fs where fs.id_hst = f.id )) autor_setor," +
                   "'Institucional' autor_modulo," +

                   "nvl(d.nome, d.razao_social) declarante_nomerazao," +
                   "s.motivo," +
                   "pg.projeto_geo_id" +
              "from hst_car_solicitacao s," +
                   "hst_protocolo       p," +
                   "hst_protocolo       ps," +
                   "hst_requerimento    r," +
                   "hst_empreendimento  e," +
                   "hst_pessoa          d," +
                   "tab_atividade       a," +
                   "hst_crt_projeto_geo pg," +
                   "hst_funcionario     f" +
             "where s.protocolo_id = p.id_protocolo" +
               "and s.protocolo_tid = p.tid" +
               "and s.protocolo_selecionado_id = ps.id_protocolo" +
               "and s.protocolo_selecionado_tid = ps.tid" +
               "and ps.requerimento_id = r.requerimento_id" +
               "and ps.requerimento_tid = r.tid" +
               "and s.empreendimento_id = e.empreendimento_id" +
               "and s.empreendimento_tid = e.tid" +
               "and s.declarante_id = d.pessoa_id" +
               "and s.declarante_tid = d.tid" +
               "and s.atividade_id = a.id" +
               "and s.projeto_geo_id = pg.projeto_geo_id" +
               "and s.projeto_geo_tid = pg.tid" +
               "and f.funcionario_id = s.autor_id" +
               "and f.tid = s.autor_tid" +
               "and s.solicitacao_id = :id" +
               "and s.tid = :tid", conn))
            {

                //comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                //comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

                //using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                //{
                command.Parameters.Add(new OracleParameter("id", id));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        solicitacao.Id = id;
                        solicitacao.Tid = Convert.ToString(reader["tid"]); //reader.GetValue<String>("tid");
                        solicitacao.Numero = Convert.ToString(reader["numero"]); //reader.GetValue<String>("numero");
                        solicitacao.DataEmissao.DataTexto = Convert.ToString(reader["data_emissao"]); //reader.GetValue<String>("data_emissao");
                        solicitacao.SituacaoId = Convert.ToInt32(reader["situacao_id"]); //reader.GetValue<Int32>("situacao_id");
                        solicitacao.SituacaoTexto = Convert.ToString(reader["situacao_texto"]); //reader.GetValue<String>("situacao_texto");
                        solicitacao.DataSituacao.DataTexto = Convert.ToString(reader["situacao_data"]); //reader.GetValue<String>("situacao_data");
                        solicitacao.SituacaoAnteriorId = Convert.ToInt32(reader["situacao_anterior_id"]);  //reader.GetValue<Int32>("situacao_anterior_id");
                        solicitacao.SituacaoAnteriorTexto = Convert.ToString(reader["situacao_anterior_texto"]); //reader.GetValue<String>("situacao_anterior_texto");
                        solicitacao.DataSituacaoAnterior.DataTexto = Convert.ToString(reader["situacao_anterior_data"]);  //reader.GetValue<String>("situacao_anterior_data");
                        solicitacao.Protocolo.Id = Convert.ToInt32(reader["id_protocolo"]);  //reader.GetValue<Int32>("id_protocolo");
                        solicitacao.Protocolo.IsProcesso = Convert.ToBoolean(reader["protocolo_id"]); //reader.GetValue<Int32>("protocolo_id") == 1;
                        solicitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]); //reader.GetValue<Int32?>("protocolo_numero");
                        solicitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);  //reader.GetValue<Int32>("protocolo_ano");
                        solicitacao.ProtocoloSelecionado.Id = Convert.ToInt32(reader["id_protocolo_selecionado"]);  //reader.GetValue<Int32>("id_protocolo_selecionado");
                        solicitacao.ProtocoloSelecionado.IsProcesso = Convert.ToBoolean(reader["protocolo_selecionado_id"]);  //reader.GetValue<Int32>("protocolo_selecionado_id") == 1;
                        solicitacao.ProtocoloSelecionado.NumeroProtocolo = Convert.ToInt32(reader["protocolo_selecionado_numero"]);  //reader.GetValue<Int32?>("protocolo_selecionado_numero");
                        solicitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_selecionado_ano"]);  //reader.GetValue<Int32>("protocolo_selecionado_ano");
                        solicitacao.Requerimento.Id = Convert.ToInt32(reader["requerimento_id"]);  //reader.GetValue<Int32>("requerimento_id");
                        solicitacao.Requerimento.DataCadastro = Convert.ToDateTime(reader["requerimento_data_criacao"]);  //reader.GetValue<DateTime>("requerimento_data_criacao");
                        solicitacao.Atividade.Id = Convert.ToInt32(reader["atividade_id"]);  //reader.GetValue<Int32>("atividade_id");
                        solicitacao.Atividade.NomeAtividade = Convert.ToString(reader["atividade_texto"]); //reader.GetValue<String>("atividade_texto");
                        solicitacao.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]);  //reader.GetValue<Int32>("empreendimento_id");
                        solicitacao.Empreendimento.NomeRazao = Convert.ToString(reader["empreendimento_nome"]);  //reader.GetValue<String>("empreendimento_nome");
                        solicitacao.Empreendimento.Codigo = Convert.ToInt64(reader["empreendimento_codigo"]); //reader.GetValue<Int64?>("empreendimento_codigo");
                        solicitacao.Declarante.Id = Convert.ToInt32(reader["declarante_id"]); //reader.GetValue<Int32>("declarante_id");
                        solicitacao.Declarante.NomeRazaoSocial = Convert.ToString(reader["declarante_nomerazao"]); //reader.GetValue<String>("declarante_nomerazao");

                        solicitacao.AutorId = Convert.ToInt32(reader["autor_id"]); //reader.GetValue<Int32>("autor_id");
                        solicitacao.AutorNome = Convert.ToString(reader["autor_nome"]);  //reader.GetValue<String>("autor_nome");
                        solicitacao.AutorSetorTexto = Convert.ToString(reader["autor_setor"]);  //reader.GetValue<String>("autor_setor");
                        solicitacao.AutorModuloTexto = Convert.ToString(reader["autor_modulo"]);  //reader.GetValue<String>("autor_modulo");

                        solicitacao.Motivo = Convert.ToString(reader["motivo"]); //reader.GetValue<String>("motivo");
                        solicitacao.ProjetoId = Convert.ToInt32(reader["projeto_geo_id"]);  //reader.GetValue<Int32>("projeto_geo_id");
                    }

                    reader.Close();
                }
            }
            #endregion
            return solicitacao;
        }



        //

        internal CARSolicita daObterCred(int id, OracleConnection conn)//BancoDeDados banco = null)
        {
            CARSolicita solicitacao = new CARSolicita();

            //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            //{
            #region Solicitação

            //Comando comando = bancoDeDados.CriarComando(@"
            using (OracleCommand command = new OracleCommand(

            "select s.tid, s.numero, s.data_emissao, s.situacao_data, l.id situacao, l.texto situacao_texto, s.situacao_anterior, la.texto situacao_anterior_texto, s.situacao_anterior_data, nvl(pes.nome, pes.razao_social) declarante_nome_razao," +
                  "s.requerimento, s.atividade, " +
                  "e.id empreendimento_id," +
                  "e.denominador empreendimento_nome," +
                  "e.codigo empreendimento_codigo,}" +
                  "s.declarante," +

                  "p.id protocolo_id," +
                  "p.protocolo," +
                  "p.numero protocolo_numero," +
                  "p.ano protocolo_ano," +

                  "s.credenciado autor_id," +
                  "nvl(f.nome, f.razao_social) autor_nome," +
                  "lct.texto  autor_tipo," +
                  "'Credenciado' autor_modulo," +

                  "s.motivo," +
                  "tr.data_criacao requerimento_data_cadastro," +
                  "s.projeto_digital" +

                  "from tab_car_solicitacao          s," +
                  "lov_car_solicitacao_situacao l," +
                  "lov_car_solicitacao_situacao la," +
                  "tab_empreendimento           e," +
                  "tab_pessoa                   pes," +
                  "tab_requerimento             tr," +
                  "tab_credenciado              tc," +
                  "tab_pessoa                   f," +
                  "lov_credenciado_tipo         lct," +
                  "ins_protocolo                p" +
                "where s.situacao = l.id" +
                "and s.situacao_anterior = la.id(+)" +
                "and s.empreendimento = e.id" +
                "and s.declarante = pes.id" +
                "and s.requerimento = tr.id" +
                "and s.empreendimento = e.id" +
                "and tc.id = s.credenciado" +
                "and f.id = tc.pessoa" +
                "and lct.id = tc.tipo" +
                "and s.requerimento=p.requerimento(+)" +
                "and s.id = :id", conn))//UsuarioCredenciado)
            {
                //comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                //using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                //{
                command.Parameters.Add(new OracleParameter("id", id));
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        solicitacao.Id = id;
                        solicitacao.Tid = Convert.ToString(reader["tid"]);//reader.GetValue<String>("tid");
                        solicitacao.Numero = Convert.ToString(reader["numero"]);  //reader.GetValue<String>("numero");
                        solicitacao.DataEmissao.DataTexto = Convert.ToString(reader["data_emissao"]); //reader.GetValue<String>("data_emissao");
                        solicitacao.SituacaoId = Convert.ToInt32(reader["situacao"]); //reader.GetValue<Int32>("situacao");
                        solicitacao.SituacaoTexto = Convert.ToString(reader["tid"]); //reader.GetValue<String>("situacao_texto");
                        solicitacao.DataSituacao.DataTexto = Convert.ToString(reader["tid"]);  //reader.GetValue<String>("situacao_data");
                        solicitacao.SituacaoAnteriorId = Convert.ToInt32(reader["situacao_anterior"]); //reader.GetValue<Int32>("situacao_anterior");
                        solicitacao.SituacaoAnteriorTexto = Convert.ToString(reader["situacao_anterior_texto"]);  //reader.GetValue<String>("situacao_anterior_texto");
                        solicitacao.DataSituacaoAnterior.DataTexto = Convert.ToString(reader["situacao_anterior_data"]);  //reader.GetValue<String>("situacao_anterior_data");
                        solicitacao.Requerimento.Id = Convert.ToInt32(reader["requerimento"]); //reader.GetValue<Int32>("requerimento");
                        solicitacao.Requerimento.DataCadastro = Convert.ToDateTime(reader["requerimento_data_cadastro"]); //reader.GetValue<DateTime>("requerimento_data_cadastro");
                        solicitacao.Atividade.Id = Convert.ToInt32(reader["atividade"]); //reader.GetValue<Int32>("atividade");
                        solicitacao.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]); //reader.GetValue<Int32>("empreendimento_id");
                        solicitacao.Empreendimento.NomeRazao = Convert.ToString(reader["empreendimento_nome"]);  //reader.GetValue<String>("empreendimento_nome");
                        solicitacao.Empreendimento.Codigo = Convert.ToInt64(reader["empreendimento_codigo"]);  //reader.GetValue<Int64?>("empreendimento_codigo");
                        solicitacao.Declarante.Id = Convert.ToInt32(reader["declarante"]);  //reader.GetValue<Int32>("declarante");
                        solicitacao.Declarante.NomeRazaoSocial = Convert.ToString(reader["declarante_nome_razao"]); // reader.GetValue<String>("declarante_nome_razao");

                        solicitacao.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);  //reader.GetValue<Int32>("protocolo_id");
                        solicitacao.Protocolo.IsProcesso = Convert.ToBoolean(reader["protocolo"]);  //reader.GetValue<Int32>("protocolo") == 1;
                        solicitacao.Protocolo.NumeroProtocolo = Convert.ToInt32(reader["protocolo_numero"]);  //reader.GetValue<Int32?>("protocolo_numero");
                        solicitacao.Protocolo.Ano = Convert.ToInt32(reader["protocolo_ano"]);  //reader.GetValue<Int32>("protocolo_ano");

                        solicitacao.AutorId = Convert.ToInt32(reader["autor_id"]);  //reader.GetValue<Int32>("autor_id");
                        solicitacao.AutorNome = Convert.ToString(reader["autor_nome"]);  //reader.GetValue<String>("autor_nome");
                        solicitacao.AutorTipoTexto = Convert.ToString(reader["autor_tipo"]);  //reader.GetValue<String>("autor_tipo");
                        solicitacao.AutorModuloTexto = Convert.ToString(reader["autor_modulo"]);  //reader.GetValue<String>("autor_modulo");

                        solicitacao.Motivo = Convert.ToString(reader["motivo"]);  //reader.GetValue<String>("motivo");
                        solicitacao.ProjetoId = Convert.ToInt32(reader["projeto_digital"]);  //reader.GetValue<Int32>("projeto_digital");
                    }

                    reader.Close();
                }
            }


            #endregion
            return solicitacao;
        }

        //
        public bool AcessoEnviarReenviarArquivoSICAR(CARSolicita entidade, int origem)
        {
            if (entidade.Id <= 0)
            {
                Validacao.Add(Mensagem.Padrao.Inexistente);
                return false;
            }

            //var situacaoArquivo = BuscaSituacaoAtualArquivoSICAR(entidade.Id, origem);

            /*if (situacaoArquivo.Item1 == eStatusArquivoSICAR.ArquivoEntregue)
            {
                //Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoJaEnviada);
                return false;
            }*/

            /*if (entidade.Protocolo.Id.GetValueOrDefault() > 0)
            {
                if (!_protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault()))
                {
                    //Validacao.Add(Mensagem.CARSolicitacao.ProtocoloPosse);
                    return false;
                }
            }
            else
            {
                if (origem == (int)eCARSolicitacaoOrigem.Credenciado)
                {
                    //Validacao.Add(Mensagem.CARSolicitacao.NaoPodeEnviarCredenciado);
                    return false;
                }
            }*/

            if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Invalido || entidade.SituacaoId == (int)eCARSolicitacaoSituacao.EmCadastro)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaEnviarSituacaoSICARInvalida(entidade.SituacaoTexto));
                return false;
            }

            /*if (situacaoArquivo.Item1 != eStatusArquivoSICAR.Nulo && situacaoArquivo.Item1 != eStatusArquivoSICAR.ArquivoReprovado)
            {
                //Validacao.Add(Mensagem.CARSolicitacao.SolicitacaEnviarSituacaoArquivoSICARInvalida(situacaoArquivo.Item2));
            }*/

            return true;
        }
        //
        public Tuple<eStatusArquivoSICAR, string> BuscaSituacaoAtualArquivoSICAR(int solicitacaoId, int origem, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select e.situacao_envio, (select s.texto from {0}lov_situacao_envio_sicar s where s.id = e.situacao_envio) situacao_envio_texto from {0}tab_controle_sicar e where e.solicitacao_car = :solicitacaoId and e.solicitacao_car_esquema = :origem", EsquemaBanco);

                comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("origem", origem, DbType.Int32);
                using (var reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (!reader.Read())
                        return new Tuple<eStatusArquivoSICAR, string>(eStatusArquivoSICAR.Nulo, "vazio");

                    var SituacaoArquivoid = Convert.ToInt32(reader[0]);
                    var SituacaoArquivoTexto = Convert.ToString(reader[1]);
                    return new Tuple<eStatusArquivoSICAR, string>((eStatusArquivoSICAR)SituacaoArquivoid, SituacaoArquivoTexto);
                }
            }
        }
        //
        public void EnviarReenviarArquivoSICARCred(int solicitacaoId, bool isEnviar, OracleConnection conn)//, BancoDeDados banco = null)
        {/*
            try
            {
                GerenciadorTransacao.ObterIDAtual();

                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
                {
                    bancoDeDados.IniciarTransacao();
                */
                    InserirFilaArquivoCarSicarCred(solicitacaoId, eCARSolicitacaoOrigem.Credenciado, conn);//, bancoDeDados);

                    //bancoDeDados.Commit();
            /*
                    Validacao.Add(Mensagem.CARSolicitacao.SucessoEnviarReenviarArquivoSICAR(isEnviar));
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }*/
        }
        //
        internal void InserirFilaArquivoCarSicarCred(int solicitacaoId, eCARSolicitacaoOrigem solicitacaoOrigem, OracleConnection conn)//, BancoDeDados banco = null)
        {
            string requisicao_fila = string.Empty;

            //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            //{
            //bancoDeDados.IniciarTransacao();

            //Comando comando = bancoDeDados.CriarComando(@
            using (OracleCommand command = new OracleCommand("select s.solicitacao_id solic_id, s.tid solic_tid, s.empreendimento_id emp_id, s.empreendimento_tid emp_tid from hst_car_solicitacao s where s.solicitacao_id = :idSolicitacao" +
                    "and s.tid = (select ss.tid from tab_car_solicitacao ss where ss.id= :idSolicitacao) order by id desc", conn))
            {
                //comando.AdicionarParametroEntrada("idSolicitacao", solicitacaoId, DbType.Int32);

                //using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                //{
                command.Parameters.Add(new OracleParameter("idSolicitacao", solicitacaoId));
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requisicao_fila = "{"
                                            + "\"origem\": \"" + solicitacaoOrigem.ToString().ToLower() + "\", "
                                            + "\"empreendimento\":" + Convert.ToInt32(reader["emp_id"]) + ", "              //reader.GetValue<int>("emp_id") + ", "
                                            + "\"empreendimento_tid\": \"" + Convert.ToString(reader["emp_tid"]) + "\","    //reader.GetValue<string>("emp_tid") + "\","
                                            + "\"solicitacao_car\": " + Convert.ToString(reader["emp_tid"]) + ","           //reader.GetValue<int>("solic_id") + ","
                                            + "\"solicitacao_car_tid\": \"" + Convert.ToString(reader["emp_tid"]) + "\"" +  //reader.GetValue<string>("solic_tid") + "\"" +
                                          "}";
                    }

                    reader.Close();
                }

                if (requisicao_fila != string.Empty)
                {
                    //comando = bancoDeDados.CriarComando(@"
                    using (OracleCommand commando = new OracleCommand("insert into tab_scheduler_fila (id, tipo, requisitante, requisicao, empreendimento, data_criacao, data_conclusao, resultado, sucesso) " +
                   " values (seq_tab_scheduler_fila.nextval, 'gerar-car', 0, :requisicao, 0, NULL, NULL, '', '')", conn))
                    {
                        //comando.AdicionarParametroEntrada("requisicao", requisicao_fila, DbType.String);
                        commando.Parameters.Add(new OracleParameter("requisicao", requisicao_fila));
                        //bancoDeDados.ExecutarNonQuery(comando);
                        commando.ExecuteNonQuery();
                        SalvarControleArquivoCarSicarCred(solicitacaoId, eStatusArquivoSICAR.AguardandoEnvio, solicitacaoOrigem, conn);

                        //bancoDeDados.Commit();
                    }
                }
            }
        }
             
        internal void SalvarControleArquivoCarSicarCred(int solicitacaoId, eStatusArquivoSICAR statusArquivoSICAR, eCARSolicitacaoOrigem solicitacaoOrigem, OracleConnection conn) //BancoDeDados banco = null)
        {
            ControleArquivoSICAR controleArquivoSICAR = new ControleArquivoSICAR();
            controleArquivoSICAR.SolicitacaoCarId = solicitacaoId;

            #region Coleta de dados

            using (OracleCommand command = new OracleCommand("select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid, DECODE( tcrls.id, null, 0, tcrls.id) controle_id " +
                    " from tab_car_solicitacao tcs, tab_empreendimento te, (select tcsicar.id, tcsicar.solicitacao_car from tab_controle_sicar tcsicar  " +
                    " where tcsicar.solicitacao_car_esquema = :esquema) tcrls where tcs.empreendimento = te.id and tcs.id = tcrls.solicitacao_car(+) " +
                    " and tcs.id = :idSolicitacao", conn))
              {
                
                command.Parameters.Add(new OracleParameter("esquema", (int)solicitacaoOrigem));
                command.Parameters.Add(new OracleParameter("idSolicitacao", controleArquivoSICAR.SolicitacaoCarId));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        controleArquivoSICAR.SolicitacaoCarTid = Convert.ToString(reader["solic_tid"]); //reader.GetValue<String>("solic_tid");
                        controleArquivoSICAR.EmpreendimentoId = Convert.ToInt32(reader["emp_id"]); //reader.GetValue<Int32>("emp_id");
                        controleArquivoSICAR.EmpreendimentoTid = Convert.ToString(reader["emp_tid"]); //reader.GetValue<String>("emp_tid");
                        controleArquivoSICAR.Id = Convert.ToInt32(Convert.ToString(reader["controle_id"])); //(reader.GetValue<String>("controle_id"));
                        //controleArquivoSICAR.Id = Convert.ToInt32(Convert.ToString(reader["emp_id"])); // Wilson: NÃO SEI PQ USARAM EMP_ID??
                    }
                    reader.Close();
                }

              }                            

               #endregion

                if (controleArquivoSICAR.Id == 0) //Wilson: Verificar se todos estão retornando 0, fiz alteração na consulta (DECODE)
                {
                    #region Criar controle arquivo SICAR
                    using (OracleCommand comand = new OracleCommand(
                        " insert into tab_controle_sicar (id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio, solicitacao_car_esquema) " +
                        "  values " +
                        "  (seq_tab_controle_sicar.nextval, :tid, :empreendimento, :empreendimento_tid, :solicitacao_car, :solicitacao_car_tid, :situacao_envio, :solicitacao_car_esquema) " +
                        "  returning id into :id", conn)) 
                     {

                         comand.Parameters.Add(new OracleParameter("tid", GerenciadorTransacao.ObterIDAtual()));
                        comand.Parameters.Add(new OracleParameter("empreendimento", Convert.ToInt32(controleArquivoSICAR.EmpreendimentoId)));
                        comand.Parameters.Add(new OracleParameter("empreendimento_tid", Convert.ToString(controleArquivoSICAR.EmpreendimentoTid)));
                        comand.Parameters.Add(new OracleParameter("solicitacao_car", Convert.ToInt32(controleArquivoSICAR.SolicitacaoCarId)));
                        comand.Parameters.Add(new OracleParameter("solicitacao_car_tid", Convert.ToString(controleArquivoSICAR.SolicitacaoCarTid)));
                        comand.Parameters.Add(new OracleParameter("situacao_envio", Convert.ToInt32(statusArquivoSICAR))); //(int)statusArquivoSICAR);
                        comand.Parameters.Add(new OracleParameter("solicitacao_car_esquema", Convert.ToInt32(solicitacaoOrigem))); //(int)solicitacaoOrigem);
                        comand.Parameters.Add(new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output)); //DbType.Int32
                                                
                        comand.ExecuteNonQuery();

                        controleArquivoSICAR.Id = int.Parse(comand.Parameters["id"].Value.ToString()); 

                    }
                    
                    #endregion
                }
                else
                {
                    #region Editar controle arquivo SICAR
                    using (OracleCommand comand = new OracleCommand(
                       " update tab_controle_sicar r set r.empreendimento_tid = :empreendimento_tid, r.solicitacao_car_tid = :solicitacao_car_tid, r.situacao_envio = :situacao_envio, " +
                       " r.tid = :tid, r.arquivo = null where r.id = :id", conn))
                    {
                        comand.Parameters.Add("empreendimento_tid", controleArquivoSICAR.EmpreendimentoTid);
                        comand.Parameters.Add("solicitacao_car_tid", controleArquivoSICAR.SolicitacaoCarTid);
                        comand.Parameters.Add("situacao_envio", (int)statusArquivoSICAR);
                        comand.Parameters.Add("tid", GerenciadorTransacao.ObterIDAtual());
                        comand.Parameters.Add("id", controleArquivoSICAR.Id);

                        comand.ExecuteNonQuery();
                    }
                    
                    #endregion
                }
                
                GerarHistoricoControleArquivoCarSicarCred(controleArquivoSICAR.Id, conn);             
        }
        
      
        internal void GerarHistoricoControleArquivoCarSicarCred(int controleArquivoId, OracleConnection conn) //BancoDeDados banco = null)
        {
            #region Histórico do controle de arquivo SICAR
            if (controleArquivoId > 0)
            {
                using (OracleCommand comando2 = new OracleCommand(
                "begin " +
                "  for j in (select tcs.id, tcs.tid, tcs.empreendimento, tcs.empreendimento_tid, tcs.solicitacao_car, tcs.solicitacao_car_tid, " +
                "           tcs.situacao_envio, tcs.chave_protocolo, tcs.data_gerado, tcs.data_envio, tcs.arquivo, tcs.pendencias, " +
                "           tcs.codigo_imovel, tcs.url_recibo, tcs.status_sicar, tcs.condicao, tcs.solicitacao_car_esquema  " +
                "           from tab_controle_sicar tcs " +
                "           where tcs.id = :id) loop   " +
                "    INSERT INTO HST_CONTROLE_SICAR " +
                "      (id, controle_sicar_id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio, " +
                "       chave_protocolo, data_gerado, data_envio, arquivo, pendencias, codigo_imovel, url_recibo, status_sicar, condicao, " +
                "       solicitacao_car_esquema, data_execucao) " +
                "    values  " +
                "      (SEQ_HST_CONTROLE_SICAR.nextval, j.id, j.tid, j.empreendimento, j.empreendimento_tid, j.solicitacao_car, j.solicitacao_car_tid, " +
                "       j.situacao_envio, j.chave_protocolo, j.data_gerado, j.data_envio, j.arquivo, j.pendencias, j.codigo_imovel, j.url_recibo, " +
                "       j.status_sicar, j.condicao, j.solicitacao_car_esquema, CURRENT_TIMESTAMP); " +
                "  end loop; " +
                " end;", conn))
                {

                    comando2.Parameters.Add("id", controleArquivoId);
                    comando2.ExecuteNonQuery();

                }                   
//                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
//                {
//                    #region Histórico do controle de arquivo SICAR

//                    bancoDeDados.IniciarTransacao();

//                    Comando comando = bancoDeDados.CriarComandoPlSql(@"
//					begin
//						for j in (select tcs.id, tcs.tid, tcs.empreendimento, tcs.empreendimento_tid, tcs.solicitacao_car, tcs.solicitacao_car_tid,
//										 tcs.situacao_envio, tcs.chave_protocolo, tcs.data_gerado, tcs.data_envio, tcs.arquivo, tcs.pendencias,
//										 tcs.codigo_imovel, tcs.url_recibo, tcs.status_sicar, tcs.condicao, tcs.solicitacao_car_esquema
//								  from tab_controle_sicar tcs
//								  where tcs.id = :id) loop  
//						   INSERT INTO HST_CONTROLE_SICAR
//							 (id, controle_sicar_id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio,
//							  chave_protocolo, data_gerado, data_envio, arquivo, pendencias, codigo_imovel, url_recibo, status_sicar, condicao,
//							  solicitacao_car_esquema, data_execucao)
//						   values
//							 (SEQ_HST_CONTROLE_SICAR.nextval, j.id, j.tid, j.empreendimento, j.empreendimento_tid, j.solicitacao_car, j.solicitacao_car_tid,
//							  j.situacao_envio, j.chave_protocolo, j.data_gerado, j.data_envio, j.arquivo, j.pendencias, j.codigo_imovel, j.url_recibo,
//							  j.status_sicar, j.condicao, j.solicitacao_car_esquema, CURRENT_TIMESTAMP);
//						end loop;
//					end;", UsuarioCredenciado);

//                    comando.AdicionarParametroEntrada("id", controleArquivoId, DbType.Int32);

//                    bancoDeDados.ExecutarNonQuery(comando);
//                    bancoDeDados.Commit();

//                    #endregion
                //                }            
            }
            #endregion
        }
        //
        public void EnviarReenviarArquivoSICARInst(int solicitacaoId, bool isEnviar, OracleConnection conn)//, BancoDeDados banco = null)
        {
            try
            {
                GerenciadorTransacao.ObterIDAtual();

                //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
                //{
                //bancoDeDados.IniciarTransacao();

                InserirFilaArquivoCarSicarInst(solicitacaoId, eCARSolicitacaoOrigem.Institucional, conn);

                //bancoDeDados.Commit();

                Validacao.Add(Mensagem.CARSolicitacao.SucessoEnviarReenviarArquivoSICAR(isEnviar));
                //}
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
        }
        //
        internal void InserirFilaArquivoCarSicarInst(int solicitacaoId, eCARSolicitacaoOrigem solicitacaoOrigem, OracleConnection conn)//, BancoDeDados banco = null)
        {
            string requisicao_fila = string.Empty;

            //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            //{
                //bancoDeDados.IniciarTransacao();

               // Comando comando = bancoDeDados.CriarComando(@
            using (OracleCommand command = new OracleCommand("select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid from tab_car_solicitacao tcs, tab_empreendimento te "+
                    " where tcs.empreendimento = te.id and tcs.id = :idSolicitacao", conn))
            {

                //comando.AdicionarParametroEntrada("idSolicitacao", solicitacaoId, DbType.Int32);

                //using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                //{
                command.Parameters.Add(new OracleParameter("idSolicitacao", solicitacaoId));
                using (var reader = command.ExecuteReader())
                {
                        while (reader.Read())
                        {
                            requisicao_fila = "{ \"origem\": \"" + solicitacaoOrigem.ToString().ToLower() + "\"" +
                                                ", \"empreendimento\":" + Convert.ToInt32(reader["emp_id"]) +                       //reader.GetValue<int>("emp_id") +
                                                ", \"empreendimento_tid\": \"" + Convert.ToString(reader["emp_tid"]) + "\"" +       //reader.GetValue<string>("emp_tid") + "\"" +
                                                ", \"solicitacao_car\":" + Convert.ToInt32(reader["solic_id"]) +                    //reader.GetValue<int>("solic_id") +
                                                ", \"solicitacao_car_tid\": \"" + Convert.ToString(reader["solic_tid"]) + "\"" +    // reader.GetValue<string>("solic_tid") + "\"" +
                                                 "}";
                        }

                        reader.Close();
               }

                    if (requisicao_fila != string.Empty)
                    {
                        //comando = bancoDeDados.CriarComando(@"
				        using (OracleCommand commando = new OracleCommand("insert into tab_scheduler_fila (id, tipo, requisitante, requisicao, empreendimento, data_criacao, data_conclusao, resultado, sucesso) "+
                        "values (seq_tab_scheduler_fila.nextval, 'gerar-car', 0, :requisicao, 0, NULL, NULL, '', '')", conn))
                        {
                            //comando.AdicionarParametroEntrada("requisicao", requisicao_fila, DbType.String);
                            commando.Parameters.Add(new OracleParameter("requisicao", requisicao_fila));
                            //bancoDeDados.ExecutarNonQuery(comando);
                            commando.ExecuteNonQuery();

                            SalvarControleArquivoCarSicarInst(solicitacaoId, eStatusArquivoSICAR.AguardandoEnvio, solicitacaoOrigem, conn);

                            //bancoDeDados.Commit();
                        }
                    }
             }
         }
        
        
        internal void SalvarControleArquivoCarSicarInst(int solicitacaoId, eStatusArquivoSICAR statusArquivoSICAR, eCARSolicitacaoOrigem solicitacaoOrigem, OracleConnection conn)//, BancoDeDados banco = null)
        {
            ControleArquivoSICAR controleArquivoSICAR = new ControleArquivoSICAR();
            controleArquivoSICAR.SolicitacaoCarId = solicitacaoId;
            
            #region Coleta de dados
            //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            using (OracleCommand comando = new OracleCommand("select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid, DECODE( tcrls.id, null, 0, tcrls.id) controle_id " +
                    " from tab_car_solicitacao tcs, tab_empreendimento te, (select tcsicar.id, tcsicar.solicitacao_car from tab_controle_sicar tcsicar  "+
                    " where tcsicar.solicitacao_car_esquema = :esquema) tcrls where tcs.empreendimento = te.id and tcs.id = tcrls.solicitacao_car(+) "+
                    " and tcs.id = :idSolicitacao", conn))
            {
                //bancoDeDados.IniciarTransacao();            

                comando.Parameters.Add(new OracleParameter("esquema", (int)solicitacaoOrigem));
                comando.Parameters.Add(new OracleParameter("idSolicitacao", controleArquivoSICAR.SolicitacaoCarId));

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        controleArquivoSICAR.SolicitacaoCarTid = Convert.ToString(reader["solic_tid"]); //reader.GetValue<String>("solic_tid");
                        controleArquivoSICAR.EmpreendimentoId = Convert.ToInt32(reader["emp_id"]); //reader.GetValue<Int32>("emp_id");
                        controleArquivoSICAR.EmpreendimentoTid = Convert.ToString(reader["emp_tid"]); //reader.GetValue<String>("emp_tid");
                        controleArquivoSICAR.Id = Convert.ToInt32(Convert.ToString(reader["controle_id"])); //(reader.GetValue<String>("controle_id"));
                    }
                    reader.Close();
                }
                
                #endregion

                if (controleArquivoSICAR.Id == 0) //Wilson: Verificar se todos estão retornando 0, fiz alteração na consulta (DECODE)
                {
                    
                    int Emp = Convert.ToInt32(controleArquivoSICAR.EmpreendimentoId);

                    if (Emp == 13417)
                    {
                        Emp = 13417;

                    }
                    #region Criar controle arquivo SICAR
                    using (OracleCommand comand = new OracleCommand(
                        " insert into tab_controle_sicar (id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio, solicitacao_car_esquema) " +
                        "  values " +
                        "  (seq_tab_controle_sicar.nextval, :tid, :empreendimento, :empreendimento_tid, :solicitacao_car, :solicitacao_car_tid, :situacao_envio, :solicitacao_car_esquema) " + //, conn))
                        "  returning id into :id", conn)) 

                    {

                        comand.Parameters.Add(new OracleParameter("tid", GerenciadorTransacao.ObterIDAtual()));
                        comand.Parameters.Add(new OracleParameter("empreendimento", Convert.ToInt32(controleArquivoSICAR.EmpreendimentoId)));
                        comand.Parameters.Add(new OracleParameter("empreendimento_tid", Convert.ToString(controleArquivoSICAR.EmpreendimentoTid)));
                        comand.Parameters.Add(new OracleParameter("solicitacao_car", Convert.ToInt32(controleArquivoSICAR.SolicitacaoCarId)));
                        comand.Parameters.Add(new OracleParameter("solicitacao_car_tid", Convert.ToString(controleArquivoSICAR.SolicitacaoCarTid)));
                        comand.Parameters.Add(new OracleParameter("situacao_envio", Convert.ToInt32(statusArquivoSICAR))); //(int)statusArquivoSICAR);
                        comand.Parameters.Add(new OracleParameter("solicitacao_car_esquema", Convert.ToInt32(solicitacaoOrigem))); //(int)solicitacaoOrigem);
                        comand.Parameters.Add(new OracleParameter("id", OracleDbType.Int32, ParameterDirection.Output)); //DbType.Int32
                                                                         
                        comand.ExecuteNonQuery();

                        //int.Parse(cmd.Parameters["successCount"].Value.ToString());
                        controleArquivoSICAR.Id = int.Parse(comand.Parameters["id"].Value.ToString()); 

                    }

                    #endregion
                }
                else
                {
                    #region Editar controle arquivo SICAR

                    using (OracleCommand comand = new OracleCommand(
                        " update tab_controle_sicar r set r.empreendimento_tid = :empreendimento_tid, r.solicitacao_car_tid = :solicitacao_car_tid, r.situacao_envio = :situacao_envio, " +
                        " r.tid = :tid, r.arquivo = null where r.id = :id", conn))
                    {
                        comand.Parameters.Add("empreendimento_tid", controleArquivoSICAR.EmpreendimentoTid);
                        comand.Parameters.Add("solicitacao_car_tid", controleArquivoSICAR.SolicitacaoCarTid);
                        comand.Parameters.Add("situacao_envio", (int)statusArquivoSICAR);
                        comand.Parameters.Add("tid", Blocos.Data.GerenciadorTransacao.ObterIDAtual());
                        comand.Parameters.Add("id", controleArquivoSICAR.Id);

                        comand.ExecuteNonQuery();
                    }
                    #endregion
                }

                GerarHistoricoControleArquivoCarSicarInst(controleArquivoSICAR.Id, conn);

                //bancoDeDados.Commit();
            }
            
        }
        
        internal void GerarHistoricoControleArquivoCarSicarInst(int controleArquivoId, OracleConnection conn) //BancoDeDados banco = null)
        {
            //using (OracleCommand comando = new OracleCommand("select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid, tcrls.id controle_id "+
            //    " from tab_car_solicitacao tcs, tab_empreendimento te, (select tcsicar.id, tcsicar.solicitacao_car from tab_controle_sicar tcsicar  "+
            //    " where tcsicar.solicitacao_car_esquema = :esquema) tcrls where tcs.empreendimento = te.id and tcs.id = tcrls.solicitacao_car(+) "+
            //    " and tcs.id = :idSolicitacao", conn))
            //{
            //    //bancoDeDados.IniciarTransacao();            

            //    comando.Parameters.Add(new OracleParameter("esquema", 1)); //1: Institucional
            //    comando.Parameters.Add(new OracleParameter("idSolicitacao", controleArquivoId));

                #region Histórico do controle de arquivo SICAR
                if (controleArquivoId > 0)
                {
                    //using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
                    using (OracleCommand comando2 = new OracleCommand(
                        "begin " +
                          "  for j in (select tcs.id, tcs.tid, tcs.empreendimento, tcs.empreendimento_tid, tcs.solicitacao_car, tcs.solicitacao_car_tid, " +
                          "           tcs.situacao_envio, tcs.chave_protocolo, tcs.data_gerado, tcs.data_envio, tcs.arquivo, tcs.pendencias, " +
                          "           tcs.codigo_imovel, tcs.url_recibo, tcs.status_sicar, tcs.condicao, tcs.solicitacao_car_esquema  " +
                          "           from tab_controle_sicar tcs " +
                          "           where tcs.id = :id) loop   " +
                          "    INSERT INTO HST_CONTROLE_SICAR " +
                          "      (id, controle_sicar_id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio, " +
                          "       chave_protocolo, data_gerado, data_envio, arquivo, pendencias, codigo_imovel, url_recibo, status_sicar, condicao, " +
                          "       solicitacao_car_esquema, data_execucao) " +
                          "    values  " +
                          "      (SEQ_HST_CONTROLE_SICAR.nextval, j.id, j.tid, j.empreendimento, j.empreendimento_tid, j.solicitacao_car, j.solicitacao_car_tid, " +
                          "       j.situacao_envio, j.chave_protocolo, j.data_gerado, j.data_envio, j.arquivo, j.pendencias, j.codigo_imovel, j.url_recibo, " +
                          "       j.status_sicar, j.condicao, j.solicitacao_car_esquema, CURRENT_TIMESTAMP); " +
                          "  end loop; " +
                        " end;", conn))
                    {

                        comando2.Parameters.Add("id", controleArquivoId);
                        comando2.ExecuteNonQuery();
                    
                    }                   

                        #endregion
                //}
            }
        }
        
    }
}