﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

using System.Web.Mvc;


namespace Tecnomapas.EtramiteX.Interno.Model.ModuloLocalVistoria.Data
{
    class LocalVistoriaDa
    {
        #region Propriedades

        private Historico _historico = new Historico();
        private Historico Historico { get { return _historico; } }
        private string EsquemaBanco { get; set; }

        GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
        public String UsuarioCredenciado
        {
            get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
        }


        #endregion

        #region Ações DML

        internal void Salvar(LocalVistoria local, BancoDeDados banco) 
		{			
			if (local == null)
			{
				throw new Exception("Local de Vistoria é nulo.");
			}

            ExcluirTodosNaoAssociados(local, banco);

            foreach (DiaHoraVistoria dia in local.DiasHorasVistoria)
            {
                if (dia.Id <= 0)
                {
                    Criar(dia, local.SetorID, banco);
                }
                else
                {
                    if (string.IsNullOrEmpty(dia.Tid))
                    {
                        Editar(dia, local.SetorID, banco);
                    }
                }
            }
		}


        internal void Criar(DiaHoraVistoria local, int idsetor, BancoDeDados banco) 
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"insert into {0}CNF_LOCAL_VISTORIA (ID, SETOR, DIA_SEMANA, HORA_INICIO, HORA_FIM, SITUACAO, TID) 
                                                                    values ({0}SEQ_CNF_LOCAL_VISTORIA.nextval, :setor, :dia_semana, :hora_inicio, :hora_fim, :situacao, :tid) 
                                                              returning id into :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("setor", idsetor, DbType.String);
                comando.AdicionarParametroEntrada("dia_semana", local.DiaSemanaId, DbType.String);
                comando.AdicionarParametroEntrada("hora_inicio", local.HoraInicio, DbType.String);
                comando.AdicionarParametroEntrada("hora_fim", local.HoraFim, DbType.String);
                comando.AdicionarParametroEntrada("situacao", local.Situacao, DbType.String);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);
                local.Id = comando.ObterValorParametro<int>("id");
                local.Tid = GerenciadorTransacao.ObterIDAtual();

                Historico.Gerar(local.Id, eHistoricoArtefato.localvistoria, eHistoricoAcao.criar, bancoDeDados);

                bancoDeDados.Commit();
            }
        }

        internal void Editar(DiaHoraVistoria local, int idsetor, BancoDeDados banco) 
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = comando = bancoDeDados.CriarComando(@"update {0}CNF_LOCAL_VISTORIA set DIA_SEMANA = :dia_semana, HORA_INICIO = :hora_inicio , HORA_FIM = :hora_fim,
				SITUACAO = :situacao, tid = :tid where id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("dia_semana", local.DiaSemanaId, DbType.String);
                comando.AdicionarParametroEntrada("hora_inicio", local.HoraInicio, DbType.String);
                comando.AdicionarParametroEntrada("hora_fim", local.HoraFim, DbType.String);
                comando.AdicionarParametroEntrada("situacao", local.Situacao, DbType.String);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", local.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                local.Tid = GerenciadorTransacao.ObterIDAtual();

                Historico.Gerar(local.Id, eHistoricoArtefato.localvistoria, eHistoricoAcao.atualizar, bancoDeDados);

                bancoDeDados.Commit();
            }
        }



        internal void ExcluirTodosNaoAssociados(LocalVistoria local, BancoDeDados banco)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                List<int> listaIdExcluir = new List<int>();

                Comando cmdConsulta = bancoDeDados.CriarComando(@"select lv.id from {0}CNF_LOCAL_VISTORIA lv where lv.setor = :setorId", EsquemaBanco);
                cmdConsulta.DbCommand.CommandText += cmdConsulta.AdicionarNotIn("and", "lv.id", DbType.Int32, local.DiasHorasVistoria.Select(x => x.Id).ToList());
                cmdConsulta.AdicionarParametroEntrada("setorId", local.SetorID, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(cmdConsulta))
                {
                    while (reader.Read())
                    {
                        listaIdExcluir.Add(reader.GetValue<int>("id"));
                    }

                    reader.Close();
                }

                if (listaIdExcluir.Count > 0)
                {
                    Comando cmdUpdate = bancoDeDados.CriarComando(@"update {0}CNF_LOCAL_VISTORIA set tid = :tid where ", EsquemaBanco);
                    cmdUpdate.DbCommand.CommandText += cmdUpdate.AdicionarIn("", "id", DbType.Int32, listaIdExcluir);
                    cmdUpdate.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                    bancoDeDados.ExecutarNonQuery(cmdUpdate);

                    foreach (int idExcluido in listaIdExcluir)
                    {
                        // Gerando historico para todos os itens excluidos
                        Historico.Gerar(idExcluido, eHistoricoArtefato.localvistoria, eHistoricoAcao.excluir, bancoDeDados);
                    }

                    Comando comando = bancoDeDados.CriarComando("delete from {0}cnf_local_vistoria lv where lv.setor = :setorId ", EsquemaBanco);
                    comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "lv.id", DbType.Int32, local.DiasHorasVistoria.Select(x => x.Id).ToList());
                    comando.AdicionarParametroEntrada("setorId", local.SetorID, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);
                }


                bancoDeDados.Commit();

            }

        }

        internal List<DiaHoraVistoria> Listar()
        {
            List<DiaHoraVistoria> lista = new List<DiaHoraVistoria>();

            return lista;

        }


        internal LocalVistoria Obter(int idsetor, BancoDeDados banco)
        {
            LocalVistoria local = new LocalVistoria();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select lv.id, lv.setor, lv.dia_semana dia_semana_id, dia.texto dia_semana_texto, lv.hora_inicio, lv.hora_fim, lv.situacao, lv.tid 
                                                              from {0}cnf_local_vistoria lv, {0}lov_dia_semana dia where dia.id=lv.dia_semana and lv.setor = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", idsetor, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    DiaHoraVistoria diahora = null;

                    while (reader.Read())
                    {
                        diahora = new DiaHoraVistoria();

                        diahora.Id = reader.GetValue<int>("id");
                        diahora.DiaSemanaId = reader.GetValue<int>("dia_semana_id");
                        diahora.DiaSemanaTexto = reader.GetValue<string>("dia_semana_texto");
                        diahora.HoraInicio = reader.GetValue<string>("hora_inicio");
                        diahora.HoraFim = reader.GetValue<string>("hora_fim");
                        diahora.Situacao = reader.GetValue<int>("situacao");
                        diahora.Tid = reader.GetValue<string>("tid");
                        local.DiasHorasVistoria.Add(diahora);
                    }

                    reader.Close();
                }
               
            }

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select s.id, s.nome from {0}tab_setor s where s.id=:id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", idsetor, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        local.SetorID = reader.GetValue<int>("id");
                        local.SetorTexto = reader.GetValue<string>("nome");
                    }

                    reader.Close();
                }
            }

            return local;

        }


        internal int PossuiHorarioAssociado(int localId, BancoDeDados banco = null) 
        {

            int qtdAssociacao = 0;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
            {

                Comando comando = bancoDeDados.CriarComando(@"select count(l.id) qtdLocalVistoria from {0}tab_ptv l where l.local_vistoria = :LocalId", UsuarioCredenciado);
                comando.AdicionarParametroEntrada("LocalId", localId, DbType.Int32);

               qtdAssociacao =  bancoDeDados.ExecutarScalar<int>(comando);
            }
            return qtdAssociacao;

        }

        #endregion



        public List<Setor> ObterSetorAgrupadorTecnico()
        {
            List<Setor> lst = new List<Setor>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.nome, s.sigla, s.responsavel, s.unidade_convenio from tab_setor s, tab_setor_grupo g 
                                                                      where g.grupo=2 and s.id=g.setor order by s.nome");
            foreach (var item in daReader)
            {
                lst.Add(new Setor()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Nome = item["nome"].ToString(),
                    Sigla = item["sigla"].ToString(),
                    Responsavel = Convert.IsDBNull(item["responsavel"]) ? 0 : Convert.ToInt32(item["responsavel"]),
                    UnidadeConvenio = item["unidade_convenio"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }


        internal Resultados<LocalVistoriaListar> Filtrar(Filtro<LocalVistoriaListar> filtros)
        {
            Resultados<LocalVistoriaListar> retorno = new Resultados<LocalVistoriaListar>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                string comandtxt = string.Empty;
                string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
                Comando comando = bancoDeDados.CriarComando("");

                #region Adicionando Filtros

                comandtxt += comando.FiltroAndLike("s.nome", "nome_setor", filtros.Dados.SetorTexto, true, true);
                comandtxt += comando.FiltroAnd("lv.dia_semana", "dia_semana", filtros.Dados.DiaSemanaId);

                List<String> ordenar = new List<String>();
                List<String> colunas = new List<String>() { "nome_setor", "dia_semana" };

                if (filtros.OdenarPor > 0)
                {
                    ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
                }
                else
                {
                    ordenar.Add("nome_setor");
                }

                #endregion

                #region Quantidade de registro do resultado

                comando.DbCommand.CommandText = String.Format(@"select count(distinct(lv.setor)) from {0}cnf_local_vistoria lv, {0}tab_setor s where s.id=lv.setor and lv.id > 0  " + comandtxt, esquemaBanco);

                retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                comando.AdicionarParametroEntrada("menor", filtros.Menor);
                comando.AdicionarParametroEntrada("maior", filtros.Maior);


                comandtxt = String.Format(@"select distinct(lv.setor) setor_id, s.nome nome_setor 
                                            from {0}cnf_local_vistoria lv, {0}tab_setor s where s.id=lv.setor" + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

                comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

                #endregion

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    LocalVistoriaListar item;

                    while (reader.Read())
                    {
                        item = new LocalVistoriaListar();

                        item.SetorID = reader.GetValue<int>("setor_id");
                        item.SetorTexto = reader.GetValue<string>("nome_setor");
                        retorno.Itens.Add(item);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

    }
}
