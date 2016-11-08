using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class RequerimentoCredenciadoDa
    {
        ConfiguracaoSistema _configSys = new ConfiguracaoSistema();
        
        Historico _historico = new Historico();
        public Historico Historico { get { return _historico; } }

        public String UsuarioCredenciado
        {
            get { return _configSys.UsuarioCredenciado; }
        }

        public List<Pessoa> ObterPessoas(int requerimento, BancoDeDados bancoCredenciado)
        {
            List<Pessoa> retorno = new List<Pessoa>();
            List<int> ids = new List<int>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
            {

                bancoDeDados.IniciarTransacao();

                #region Pessoas relacionadas com o Projeto Digital

                Comando comando = bancoDeDados.CriarComando(@"select r.interessado id from {0}tab_requerimento r where r.id = :requerimento union all
				select er.responsavel id from {0}tab_requerimento r, {0}tab_empreendimento_responsavel er where r.id = :requerimento and r.empreendimento = er.empreendimento union all 
				select rr.responsavel id from {0}tab_requerimento_responsavel rr where rr.requerimento = :requerimento", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

                ids = bancoDeDados.ExecutarList<int>(comando);

                #region Responsáveis/Representantes

                ids = ids.Where(x => x > 0).ToList();
                List<string> conj = new List<string>();

                if (ids != null && ids.Count > 0)
                {
                    comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, p.interno InternoId, nvl(p.nome, p.razao_social) NomeRazaoSocial,
					nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
					from {0}tab_pessoa p ", UsuarioCredenciado);

                    comando.DbCommand.CommandText += comando.AdicionarIn("where", "p.id", DbType.Int32, ids);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        Pessoa pes;
                        while (reader.Read())
                        {
                            pes = new Pessoa();
                            pes.Id = reader.GetValue<int>("Id");
                            pes.Tipo = reader.GetValue<int>("Tipo");
                            pes.InternoId = reader.GetValue<int>("InternoId");
                            pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
                            pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
                            if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
                            {
                                conj = reader.GetValue<string>("conjuge").Split('@').ToList();
                                pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
                            }
                            retorno.Add(pes);
                        }

                        reader.Close();
                    }
                }

                if (retorno.Count > 0)
                {
                    if (retorno.Exists(x => x.IsJuridica))
                    {
                        //Representantes
                        comando = bancoDeDados.CriarComando(@"select pc.pessoa, p.tipo Tipo, p.id Id, p.interno InternoId, nvl(p.nome, p.razao_social) NomeRazaoSocial,
						nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
						from {0}tab_pessoa p, {0}tab_pessoa_representante pc where p.id = pc.representante", UsuarioCredenciado);

                        comando.DbCommand.CommandText += comando.AdicionarIn("and", "pc.pessoa", DbType.Int32, retorno.Where(x => x.IsJuridica).Select(x => x.Id).ToList());

                        using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                        {
                            Pessoa pes;
                            while (reader.Read())
                            {
                                pes = new Pessoa();
                                pes.Id = reader.GetValue<int>("Id");
                                pes.Tipo = reader.GetValue<int>("Tipo");
                                pes.InternoId = reader.GetValue<int>("InternoId");
                                pes.IdRelacionamento = reader.GetValue<int>("pessoa");
                                pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
                                pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
                                if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
                                {
                                    conj = reader.GetValue<string>("conjuge").Split('@').ToList();
                                    pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
                                }
                                retorno.Add(pes);
                            }

                            reader.Close();
                        }

                        retorno.Where(x => x.IsJuridica).ToList().ForEach(x =>
                        {
                            x.Juridica.Representantes = retorno.Where(y => y.IdRelacionamento == x.Id).ToList();
                        });
                    }

                    if (retorno.Exists(x => x.IsFisica))
                    {
                        //Conjuges
                        comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, p.interno InternoId, nvl(p.nome, p.razao_social) NomeRazaoSocial,
						nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
						from {0}tab_pessoa p, {0}tab_pessoa_conjuge pc where p.id = pc.conjuge", UsuarioCredenciado);

                        comando.DbCommand.CommandText += comando.AdicionarIn("and", "pc.pessoa", DbType.Int32, retorno.Where(x => x.IsFisica).Select(x => x.Id).ToList());

                        using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                        {
                            Pessoa pes;
                            while (reader.Read())
                            {
                                pes = new Pessoa();
                                pes.Id = reader.GetValue<int>("Id");
                                pes.Tipo = reader.GetValue<int>("Tipo");
                                pes.InternoId = reader.GetValue<int>("InternoId");
                                pes.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
                                pes.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
                                if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
                                {
                                    conj = reader.GetValue<string>("conjuge").Split('@').ToList();
                                    pes.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pes.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
                                }
                                retorno.Add(pes);
                            }

                            reader.Close();
                        }

                        //Obter CPF Conjuges
                        comando = bancoDeDados.CriarComando(@"select p.id, p.cpf from {0}tab_pessoa p ", UsuarioCredenciado);
                        comando.DbCommand.CommandText += comando.AdicionarIn("where", "p.id", DbType.Int32, retorno.Where(x => x.Fisica.ConjugeId > 0).Select(x => x.Fisica.ConjugeId).ToList());

                        using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                        {
                            Pessoa pes;

                            while (reader.Read())
                            {
                                pes = retorno.FirstOrDefault(x => x.Fisica.ConjugeId == reader.GetValue<int>("id"));

                                if (pes != null)
                                {
                                    pes.Fisica.ConjugeCPF = reader.GetValue<string>("cpf");
                                }
                            }

                            reader.Close();
                        }
                    }
                }

                #endregion

                #endregion
            }

            retorno = retorno.GroupBy(x => x.Id).Select(y => new Pessoa
            {
                Id = y.First().Id,
                Tipo = y.First().Tipo,
                InternoId = y.First().InternoId,
                NomeRazaoSocial = y.First().NomeRazaoSocial,
                CPFCNPJ = y.First().CPFCNPJ,
                Fisica = y.First().Fisica,
                Juridica = y.First().Juridica,
            }).ToList();

            return retorno;
        }

        internal Requerimento Obter(int id, BancoDeDados bancoCredenciado, bool simplificado = false)
        {
            Requerimento requerimento = new Requerimento();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
            {

                bancoDeDados.IniciarTransacao();

                #region Requerimento

                Comando comando = bancoDeDados.CriarComando(@"select r.id, r.numero, trunc(r.data_criacao) data_criacao, r.interessado, r.tid,
				nvl(p.nome, p.razao_social) interessado_nome, nvl(p.cpf, p.cnpj) interessado_cpf_cnpj, p.tipo interessado_tipo, r.empreendimento, e.codigo empreendimento_codigo, 
				e.cnpj empreendimento_cnpj, e.denominador, e.interno empreendimento_interno, r.situacao, r.agendamento, r.setor, r.informacoes, r.credenciado 
				from {0}tab_requerimento r, {0}tab_pessoa p, {0}tab_empreendimento e where r.interessado = p.id(+) and r.empreendimento = e.id(+) and r.id = :id", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        requerimento.Id = id;
                        requerimento.Tid = reader["tid"].ToString();
                        requerimento.DataCadastro = Convert.ToDateTime(reader["data_criacao"]);
                        requerimento.IsCredenciado = true;
                        requerimento.CredenciadoId = Convert.ToInt32(reader["credenciado"]);

                        if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
                        {
                            requerimento.SetorId = Convert.ToInt32(reader["setor"]);
                        }

                        if (reader["agendamento"] != null && !Convert.IsDBNull(reader["agendamento"]))
                        {
                            requerimento.AgendamentoVistoria = Convert.ToInt32(reader["agendamento"]);
                        }

                        if (reader["interessado"] != null && !Convert.IsDBNull(reader["interessado"]))
                        {
                            requerimento.Interessado.SelecaoTipo = (int)eExecutorTipo.Credenciado;
                            requerimento.Interessado.Id = Convert.ToInt32(reader["interessado"]);
                            requerimento.Interessado.Tipo = Convert.ToInt32(reader["interessado_tipo"]);

                            if (reader["interessado_tipo"].ToString() == "1")
                            {
                                requerimento.Interessado.Fisica.Nome = reader["interessado_nome"].ToString();
                                requerimento.Interessado.Fisica.CPF = reader["interessado_cpf_cnpj"].ToString();
                            }
                            else
                            {
                                requerimento.Interessado.Juridica.RazaoSocial = reader["interessado_nome"].ToString();
                                requerimento.Interessado.Juridica.CNPJ = reader["interessado_cpf_cnpj"].ToString();
                            }
                        }

                        if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
                        {
                            requerimento.Empreendimento.SelecaoTipo = (int)eExecutorTipo.Credenciado;
                            requerimento.Empreendimento.Id = Convert.ToInt32(reader["empreendimento"]);
                            requerimento.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
                            requerimento.Empreendimento.Denominador = reader["denominador"].ToString();
                            requerimento.Empreendimento.CNPJ = reader["empreendimento_cnpj"].ToString();
                            requerimento.Empreendimento.InternoId = reader.GetValue<int?>("empreendimento_interno");
                        }

                        if (reader["situacao"] != null && !Convert.IsDBNull(reader["situacao"]))
                        {
                            requerimento.SituacaoId = Convert.ToInt32(reader["situacao"]);
                        }

                        requerimento.Informacoes = reader["informacoes"].ToString();

                    }

                    reader.Close();
                }

                #endregion

                if (requerimento.Id <= 0 || simplificado)
                {
                    return requerimento;
                }

                #region Atividades

                comando = bancoDeDados.CriarComando(@"select a.id, a.atividade, ta.situacao, a.tid from {0}tab_requerimento_atividade a, tab_atividade ta where ta.id = a.atividade and a.requerimento = :id", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Atividade atividade;

                    while (reader.Read())
                    {
                        atividade = new Atividade();
                        atividade.Id = Convert.ToInt32(reader["atividade"]);
                        atividade.SituacaoId = Convert.ToInt32(reader["situacao"]);
                        atividade.IdRelacionamento = Convert.ToInt32(reader["id"]);
                        atividade.Tid = reader["tid"].ToString();

                        #region Atividades/Finalidades/Modelos
                        comando = bancoDeDados.CriarComando(@"select a.id, a.finalidade, a.modelo, a.titulo_anterior_tipo, a.titulo_anterior_id,
						a.titulo_anterior_numero, a.modelo_anterior_id, a.modelo_anterior_nome, a.modelo_anterior_sigla, a.orgao_expedidor 
						from {0}tab_requerimento_ativ_finalida a where a.requerimento_ativ = :id", UsuarioCredenciado);

                        comando.AdicionarParametroEntrada("id", atividade.IdRelacionamento, DbType.Int32);

                        using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
                        {
                            Finalidade fin;

                            while (readerAux.Read())
                            {
                                fin = new Finalidade();

                                fin.IdRelacionamento = Convert.ToInt32(readerAux["id"]);


                                fin.OrgaoExpedidor = readerAux["orgao_expedidor"].ToString();

                                if (readerAux["finalidade"] != DBNull.Value)
                                {
                                    fin.Id = Convert.ToInt32(readerAux["finalidade"]);
                                }

                                if (readerAux["modelo"] != DBNull.Value)
                                {
                                    fin.TituloModelo = Convert.ToInt32(readerAux["modelo"]);
                                }

                                if (readerAux["modelo_anterior_id"] != DBNull.Value)
                                {
                                    fin.TituloModeloAnteriorId = Convert.ToInt32(readerAux["modelo_anterior_id"]);
                                }

                                fin.TituloModeloAnteriorTexto = readerAux["modelo_anterior_nome"].ToString();
                                fin.TituloModeloAnteriorSigla = readerAux["modelo_anterior_sigla"].ToString();

                                if (readerAux["titulo_anterior_tipo"] != DBNull.Value)
                                {
                                    fin.TituloAnteriorTipo = Convert.ToInt32(readerAux["titulo_anterior_tipo"]);
                                }

                                if (readerAux["titulo_anterior_id"] != DBNull.Value)
                                {
                                    fin.TituloAnteriorId = Convert.ToInt32(readerAux["titulo_anterior_id"]);
                                }

                                fin.TituloAnteriorNumero = readerAux["titulo_anterior_numero"].ToString();
                                fin.EmitidoPorInterno = (fin.TituloAnteriorTipo != 3);
                                atividade.Finalidades.Add(fin);
                            }
                            readerAux.Close();
                        }
                        #endregion

                        requerimento.Atividades.Add(atividade);
                    }

                    reader.Close();
                }

                #endregion

                #region Responsáveis

                comando = bancoDeDados.CriarComando(@"select pr.id, pr.responsavel, pr.funcao, nvl(p.nome, p.razao_social) nome, pr.numero_art, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo from {0}tab_requerimento_responsavel pr, {0}tab_pessoa p where pr.responsavel = p.id and pr.requerimento = :requerimento", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    ResponsavelTecnico responsavel;
                    while (reader.Read())
                    {
                        responsavel = new ResponsavelTecnico();
                        responsavel.IdRelacionamento = reader.GetValue<int>("id");
                        responsavel.Id = reader.GetValue<int>("responsavel");
                        responsavel.Funcao = reader.GetValue<int>("funcao");
                        responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
                        responsavel.NomeRazao = reader.GetValue<string>("nome");
                        responsavel.NumeroArt = reader.GetValue<string>("numero_art");

                        if (reader.GetValue<int>("tipo") == PessoaTipo.JURIDICA)
                        {
                            comando = bancoDeDados.CriarComando(@"select pr.id, pr.pessoa, pr.representante, p.nome, p.cpf, p.tipo 
							from {0}tab_pessoa_representante pr, {0}tab_pessoa p where pr.representante = p.id and pr.pessoa = :pessoa", UsuarioCredenciado);

                            comando.AdicionarParametroEntrada("pessoa", responsavel.Id, DbType.Int32);

                            using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
                            {
                                Pessoa representante;
                                responsavel.Representantes = new List<Pessoa>();
                                while (readerAux.Read())
                                {
                                    representante = new Pessoa();
                                    representante.Id = readerAux.GetValue<int>("representante");
                                    representante.Tipo = readerAux.GetValue<int>("tipo");
                                    representante.Fisica.Nome = readerAux.GetValue<string>("nome");
                                    representante.Fisica.CPF = readerAux.GetValue<string>("cpf");
                                    responsavel.Representantes.Add(representante);
                                }
                                readerAux.Close();
                            }
                        }
                        requerimento.Responsaveis.Add(responsavel);
                    }
                    reader.Close();
                }

                #endregion
            }

            return requerimento;
        }

        internal List<Roteiro> ObterRequerimentoRoteirosHistorico(int requerimento, int situacao, BancoDeDados bancoCredenciado)
        {
            List<Roteiro> roteiros = new List<Roteiro>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando;

                comando =
                    bancoDeDados.CriarComando(@"select r.roteiro_id roteiro, r.nome roteiro_nome, r.versao, a.atividade_id atividade, ha.atividade atividade_nome, r.tid
				from {0}hst_roteiro r, {0}hst_roteiro_atividades a, {0}hst_roteiro_modelos m, {0}hst_atividade ha, (select lf.codigo finalidade, fa.modelo_id, a.atividade_id
				from {0}hst_requerimento_atividade a, {0}hst_requerimento_ativ_finalida fa, {0}lov_titulo_finalidade lf  where a.id = fa.id_hst and fa.finalidade = lf.id
				and a.id_hst = (select max(h.id) from {0}hst_requerimento h where h.requerimento_id = :requerimento)) x where r.id = a.id_hst and r.id = m.id_hst
				and a.atividade_id = x.atividade_id and bitand(r.finalidade, x.finalidade) > 0 and m.modelo_id = x.modelo_id and a.atividade_tid = ha.tid
				and a.atividade_id = ha.atividade_id and r.id in (select max(h.id) from {0}hst_roteiro h, {0}hst_roteiro_atividades aa, {0}hst_roteiro_modelos mm
				where h.id = aa.id_hst and h.id = mm.id_hst and aa.atividade_id = x.atividade_id and mm.modelo_id = x.modelo_id and bitand(h.finalidade, x.finalidade) > 0 
				and h.data_execucao <= (select max(hr.data_execucao) from {0}hst_requerimento hr where hr.requerimento_id = :requerimento)) order by r.roteiro_id, a.atividade_id",
                            UsuarioCredenciado);

                comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

                #region Buscar todas as atividades que estão configuradas com um roteiro
                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        roteiros.Add(new Roteiro()
                        {
                            Id = Convert.ToInt32(reader["roteiro"]),
                            VersaoAtual = Convert.ToInt32(reader["versao"]),
                            Nome = reader["roteiro_nome"].ToString(),
                            AtividadeId = Convert.ToInt32(reader["atividade"]),
                            AtividadeTexto = reader["atividade_nome"].ToString(),
                            Tid = reader["tid"].ToString()
                        });
                    }
                    reader.Close();
                }
                #endregion
                
                comando = bancoDeDados.CriarComando(@"
									select ta.id atividade_id, ta.atividade atividade_nome, r.roteiro_id roteiro, r.versao, r.nome roteiro_nome, r.roteiro_tid
									  from {0}tab_requerimento_atividade tra,
										   {0}tab_atividade              ta", UsuarioCredenciado);

                comando.DbCommand.CommandText += String.Format(
                            @", (select hr.roteiro_id, hr.tid, hr.versao, hr.numero, hr.nome, hr.tid roteiro_tid
									from {0}tab_checagem_roteiro t, {0}hst_roteiro hr
								where t.roteiro_tid = hr.tid
									and t.roteiro = hr.roteiro_id
									and t.checagem = (select t.checagem from {0}tab_protocolo t where t.requerimento = :requerimento) {1}) r 
									where tra.atividade = ta.id and tra.requerimento = :requerimento {2}",
                            UsuarioCredenciado, comando.AdicionarNotIn("and", "t.roteiro", DbType.Int32, roteiros.Select(x => x.Id).ToList()),
                            comando.AdicionarNotIn("and", "tra.atividade", DbType.Int32, roteiros.Select(x => x.AtividadeId).ToList()));

                comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        roteiros.Add(new Roteiro()
                        {
                            Id = Convert.ToInt32(reader["roteiro"]),
                            VersaoAtual = Convert.ToInt32(reader["versao"]),
                            Nome = reader["roteiro_nome"].ToString(),
                            AtividadeId = Convert.ToInt32(reader["atividade_id"]),
                            AtividadeTexto = reader["atividade_nome"].ToString(),
                            Tid = reader["roteiro_tid"].ToString()
                        });
                    }
                    reader.Close();
                }
            }
            return roteiros;
        }

        public bool ValidarVersaoAtual(int id, string tid, BancoDeDados bancoCredenciado)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
            {

                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_requerimento r where r.id = :id and r.tid= :tid", UsuarioCredenciado);
                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", tid, DbType.String);

                return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
            }
        }

        internal void Editar(Requerimento requerimento, BancoDeDados bancoCredenciado = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
            {
                bancoDeDados.IniciarTransacao();

                #region Requerimento

                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento r set r.interessado = :interessado, r.empreendimento = :empreendimento, 
				r.situacao = :situacao, r.tid = :tid, r.agendamento = :agendamento, r.informacoes = :informacoes where r.id = :id", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);

                if (requerimento.Interessado.Id > 0)
                {
                    comando.AdicionarParametroEntrada("interessado", requerimento.Interessado.Id, DbType.Int32);
                }
                else
                {
                    comando.AdicionarParametroEntrada("interessado", null, DbType.Int32);
                }

                if (requerimento.Empreendimento.Id > 0)
                {
                    comando.AdicionarParametroEntrada("empreendimento", requerimento.Empreendimento.Id, DbType.Int32);
                }
                else
                {
                    comando.AdicionarParametroEntrada("empreendimento", null, DbType.Int32);
                }

                comando.AdicionarParametroEntrada("agendamento", requerimento.AgendamentoVistoria, DbType.Int32);
                comando.AdicionarParametroEntrada("situacao", requerimento.SituacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("informacoes", DbType.String, 500, requerimento.Informacoes);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                #region Limpar os dados do banco

                #region Atividade/Modelos

                comando = bancoDeDados.CriarComando(@"delete from {0}tab_requerimento_ativ_finalida c 
				where c.requerimento_ativ in (select a.id from {0}tab_requerimento_atividade a where a.requerimento = :requerimento ", UsuarioCredenciado);

                comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.atividade", DbType.Int32, requerimento.Atividades.Select(x => x.Id).ToList());

                comando.DbCommand.CommandText += ")";

                comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
                {
                    foreach (Atividade item in requerimento.Atividades)
                    {
                        comando = bancoDeDados.CriarComando(@"delete from {0}tab_requerimento_ativ_finalida c 
						where c.requerimento_ativ in (select a.id from {0}tab_requerimento_atividade a where a.requerimento = :requerimento and a.atividade = :atividade)", UsuarioCredenciado);
                        comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Finalidades.Select(x => x.IdRelacionamento).ToList()));

                        comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                //Atividades
                comando = bancoDeDados.CriarComando("delete from {0}tab_requerimento_atividade c ", UsuarioCredenciado);

                comando.DbCommand.CommandText += String.Format("where c.requerimento = :requerimento{0}",
                comando.AdicionarNotIn("and", "c.id", DbType.Int32, requerimento.Atividades.Select(x => x.IdRelacionamento).ToList()));

                comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);
                #endregion

                //Responsáveis
                comando = bancoDeDados.CriarComando("delete from {0}tab_requerimento_responsavel c ", UsuarioCredenciado);
                comando.DbCommand.CommandText += String.Format("where c.requerimento = :requerimento{0}",
                comando.AdicionarNotIn("and", "c.id", DbType.Int32, requerimento.Responsaveis.Select(x => x.IdRelacionamento).ToList()));
                comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);
                #endregion

                #region Atividades
                if (requerimento.Atividades != null && requerimento.Atividades.Count > 0)
                {
                    foreach (Atividade item in requerimento.Atividades)
                    {
                        if (item.IdRelacionamento > 0)
                        {
                            comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_atividade e set e.requerimento = :requerimento, e.atividade = :atividade, e.tid =:tid where e.id = :id", UsuarioCredenciado);
                            comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
                        }
                        else
                        {
                            comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_atividade t (id, requerimento, atividade, tid)
							values ({0}seq_requerimento_atividade.nextval, :requerimento, :atividade, :tid) returning t.id into :id", UsuarioCredenciado);
                            comando.AdicionarParametroSaida("id", DbType.Int32);
                        }

                        comando.AdicionarParametroEntrada("requerimento", requerimento.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);

                        if (item.IdRelacionamento <= 0)
                        {
                            item.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
                        }

                        #region Modelo de Título

                        if (item.Finalidades != null)
                        {
                            foreach (Finalidade itemAux in item.Finalidades)
                            {
                                if (itemAux.IdRelacionamento > 0)
                                {
                                    comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_ativ_finalida a set requerimento_ativ = :requerimento_ativ,
									modelo = :modelo, titulo_anterior_tipo = :titulo_anterior_tipo, titulo_anterior_id = :titulo_anterior_id, titulo_anterior_numero = :titulo_anterior_numero, 
									modelo_anterior_id = :modelo_anterior_id, modelo_anterior_nome = :modelo_anterior_nome, modelo_anterior_sigla = :modelo_anterior_sigla, orgao_expedidor = :orgao_expedidor, finalidade = :finalidade, 
									tid = :tid where id = :id", UsuarioCredenciado);
                                    comando.AdicionarParametroEntrada("id", itemAux.IdRelacionamento, DbType.Int32);
                                }
                                else
                                {
                                    comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_ativ_finalida (id, requerimento_ativ, modelo,
									titulo_anterior_tipo, titulo_anterior_id, titulo_anterior_numero, modelo_anterior_id, modelo_anterior_nome, modelo_anterior_sigla, orgao_expedidor, finalidade, tid) values 
									({0}seq_requerimento_ativ_fin.nextval, :requerimento_ativ, :modelo, :titulo_anterior_tipo, :titulo_anterior_id, :titulo_anterior_numero, :modelo_anterior_id, 
									:modelo_anterior_nome, :modelo_anterior_sigla, :orgao_expedidor, :finalidade, :tid)", UsuarioCredenciado);
                                }

                                comando.AdicionarParametroEntrada("finalidade", itemAux.Id, DbType.Int32);
                                comando.AdicionarParametroEntrada("requerimento_ativ", item.IdRelacionamento, DbType.Int32);
                                comando.AdicionarParametroEntrada("modelo", itemAux.TituloModelo, DbType.Int32);
                                comando.AdicionarParametroEntrada("titulo_anterior_tipo", itemAux.TituloAnteriorTipo, DbType.Int32);
                                comando.AdicionarParametroEntrada("titulo_anterior_id", itemAux.TituloAnteriorId, DbType.Int32);
                                comando.AdicionarParametroEntrada("titulo_anterior_numero", DbType.String, 20, itemAux.TituloAnteriorNumero);
                                comando.AdicionarParametroEntrada("modelo_anterior_id", itemAux.TituloModeloAnteriorId, DbType.Int32);
                                comando.AdicionarParametroEntrada("modelo_anterior_nome", DbType.String, 100, itemAux.TituloModeloAnteriorTexto);
                                comando.AdicionarParametroEntrada("modelo_anterior_sigla", DbType.String, 100, itemAux.TituloModeloAnteriorSigla);
                                comando.AdicionarParametroEntrada("orgao_expedidor", DbType.String, 100, itemAux.OrgaoExpedidor);
                                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
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
                        if (responsavel.IdRelacionamento > 0)
                        {
                            comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento_responsavel r set r.requerimento = :requerimento, r.responsavel = :responsavel, 
							r.funcao = :funcao, r.numero_art = :numero_art, r.tid = :tid where r.id = :id", UsuarioCredenciado);
                            comando.AdicionarParametroEntrada("id", responsavel.IdRelacionamento, DbType.Int32);
                        }
                        else
                        {
                            comando = bancoDeDados.CriarComando(@"insert into {0}tab_requerimento_responsavel(id, requerimento, responsavel, funcao, numero_art, tid) values
							({0}seq_requerimento_responsavel.nextval, :requerimento, :responsavel, :funcao, :numero_art, :tid )", UsuarioCredenciado);
                        }

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
                    comando = bancoDeDados.CriarComando(@"delete {0}tab_requerimento_responsavel p where p.requerimento = :id", UsuarioCredenciado);
                    comando.AdicionarParametroEntrada("id", requerimento.Id, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);
                }
                #endregion

                #region Histórico
                //Historico
                Historico.Gerar(requerimento.Id, eHistoricoArtefato.requerimento, eHistoricoAcao.atualizar, bancoDeDados);

                #endregion

                bancoDeDados.Commit();
            }
        }
    }
}
