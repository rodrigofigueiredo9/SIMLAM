﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class NotificacaoDa
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

        public NotificacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

        public Notificacao Salvar(Notificacao Notificacao, BancoDeDados banco = null)
        {
            if (Notificacao == null)
            {
                throw new Exception("Notificacao é nulo.");
            }

            if (Notificacao.Id <= 0)
            {
                Notificacao = Criar(Notificacao, banco);
            }
            else
            {
                Notificacao = Editar(Notificacao, banco);
            }

            return Notificacao;
        }

        public Notificacao Criar(Notificacao notificacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    insert into {0}tab_fisc_notificacao (id,
                                                                   fiscalizacao,
                                                                   forma_iuf,
                                                                   forma_jiapi,
                                                                   forma_core,
                                                                   forma_iuf_data,
                                                                   forma_jiapi_data,
                                                                   forma_core_data,
                                                                   tid)
                                    values ({0}seq_tab_fisc_notificacao.nextval,
                                            :fiscalizacao,
                                            :forma_iuf,
                                            :forma_jiapi,
                                            :forma_core,
                                            :forma_iuf_data,
                                            :forma_jiapi_data,
                                            :forma_core_data,
                                            :tid)
                                    returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", notificacao.FiscalizacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("forma_iuf", notificacao.FormaIUF, DbType.Int32);
                comando.AdicionarParametroEntrada("forma_jiapi", notificacao.FormaJIAPI, DbType.Int32);
                comando.AdicionarParametroEntrada("forma_core", notificacao.FormaCORE, DbType.Int32);
				comando.AdicionarParametroEntrada("forma_iuf_data", notificacao.DataIUF.Data, DbType.Date);
				comando.AdicionarParametroEntrada("forma_jiapi_data", notificacao.DataJIAPI.Data, DbType.Date);
				comando.AdicionarParametroEntrada("forma_core_data", notificacao.DataCORE.Data, DbType.Date);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                notificacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Anexos

				foreach (var item in (notificacao.Anexos ?? new List<Anexo>()))
				{
					comando = bancoDeDados.CriarComando(@"
					 insert into {0}tab_fisc_notificacao_arq a
					   (id, 
						notificacao, 
						arquivo, 
						ordem, 
						descricao, 
						tid)
					 values
					   ({0}seq_tab_fisc_notificacao_arq.nextval,
						:notificacao,
						:arquivo,
						:ordem,
						:descricao,
						:tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("notificacao", notificacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(notificacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(notificacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

                bancoDeDados.Commit();
            }
            return notificacao;
        }

        public Notificacao Editar(Notificacao notificacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fisc_notificacao t
                                    set t.fiscalizacao = :fiscalizacao,
										t.forma_iuf = :forma_iuf,
                                        t.forma_jiapi = :forma_jiapi,
                                        t.forma_core = :forma_core,
                                        t.forma_iuf_data = :forma_iuf_data,
                                        t.forma_jiapi_data = :forma_jiapi_data,
                                        t.forma_core_data = :forma_core_data,
                                        t.tid = :tid
                                    where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", notificacao.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("fiscalizacao", notificacao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("forma_iuf", notificacao.FormaIUF, DbType.Int32);
				comando.AdicionarParametroEntrada("forma_jiapi", notificacao.FormaJIAPI, DbType.Int32);
				comando.AdicionarParametroEntrada("forma_core", notificacao.FormaCORE, DbType.Int32);
				comando.AdicionarParametroEntrada("forma_iuf_data", notificacao.DataIUF.Data, DbType.Date);
				comando.AdicionarParametroEntrada("forma_jiapi_data", notificacao.DataJIAPI.Data, DbType.Date);
				comando.AdicionarParametroEntrada("forma_core_data", notificacao.DataCORE.Data, DbType.Date);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

				#region Anexos

				comando = bancoDeDados.CriarComando("delete from {0}tab_fisc_notificacao_arq ra ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ra.notificacao = :notificacao{0}",
					comando.AdicionarNotIn("and", "ra.id", DbType.Int32, notificacao.Anexos?.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("notificacao", notificacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in (notificacao.Anexos ?? new List<Anexo>()))
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}tab_fisc_notificacao_arq t
							   set t.arquivo   = :arquivo,
								   t.ordem     = :ordem,
								   t.descricao = :descricao,
								   t.tid       = :tid
							 where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}tab_fisc_notificacao_arq a
							  (id, 
							   notificacao, 
							   arquivo, 
							   ordem, 
							   descricao, 
							   tid)
							values
							  ({0}seq_tab_fisc_notificacao_arq.nextval,
							   :notificacao,
							   :arquivo,
							   :ordem,
							   :descricao,
							   :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("notificacao", notificacao.Id, DbType.Int32);
					}
					
					comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(notificacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(notificacao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

                bancoDeDados.Commit();
            }
            return notificacao;
        }

		public bool Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("update {0}tab_fisc_notificacao t set t.tid = :tid where t.fiscalizacao = :fiscalizacao");
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				#region Histórico

				Historico.Gerar(fiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				comando = bancoDeDados.CriarComando(
					"begin " +
						"delete {0}tab_fisc_notificacao_arq a where exists (select 1 from tab_fisc_notificacao t where t.id = a.notificacao and t.fiscalizacao = :fiscalizacao); " +
						"delete {0}tab_fisc_notificacao t where t.fiscalizacao = :fiscalizacao; " +
					"end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Consulta

				Consulta.Deletar(fiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return true;
			}
		}

		#endregion

		#region Obter / Filtrar

		public Notificacao Obter(int fiscalizacaoId, BancoDeDados banco = null)
        {
            var notificacao = new Notificacao();
            
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select tfn.id,
											f.id as fiscalizacao,
											tfn.forma_iuf,
											tfn.forma_jiapi,
											tfn.forma_core,
											tfn.forma_iuf_data,
											tfn.forma_jiapi_data,
											tfn.forma_core_data,
											coalesce(cast(m.iuf_numero as varchar2(10)), tfi.numero_auto_infracao_bloco, cast(f.autos as varchar2(10))) iuf_numero,
											coalesce(i.pessoa, i.responsavel) pessoa
										from tab_fiscalizacao f 
										left join tab_fisc_notificacao tfn 
											on (tfn.fiscalizacao = f.id )
										left join tab_fisc_multa m
											on (m.fiscalizacao = f.id )
										left join tab_fisc_local_infracao i
											on (i.fiscalizacao = f.id)
										left join tab_fisc_infracao tfi
											on (tfi.fiscalizacao = f.id)
										where f.id = :fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        notificacao = new Notificacao
                        {
                            Id = reader.GetValue<int>("id"),
                            FiscalizacaoId = reader.GetValue<int>("fiscalizacao"),
                            NumeroIUF = reader.GetValue<string>("iuf_numero"),
							FormaIUF = reader.GetValue<int>("forma_iuf"),
                            FormaJIAPI = reader.GetValue<int>("forma_jiapi"),
                            FormaCORE = reader.GetValue<int>("forma_core"),
							AutuadoPessoaId = reader.GetValue<int>("pessoa")
						};

						notificacao.DataIUF.Data = reader.GetValue<DateTime>("forma_iuf_data");
						notificacao.DataJIAPI.Data = reader.GetValue<DateTime>("forma_jiapi_data");
						notificacao.DataCORE.Data = reader.GetValue<DateTime>("forma_core_data");
						if (notificacao.DataIUF.Data.HasValue && notificacao.DataIUF.Data.Value.Year == 1)
							notificacao.DataIUF = new DateTecno();
						if (notificacao.DataJIAPI.Data.HasValue && notificacao.DataJIAPI.Data.Value.Year == 1)
							notificacao.DataJIAPI = new DateTecno();
						if (notificacao.DataCORE.Data.HasValue && notificacao.DataCORE.Data.Value.Year == 1)
							notificacao.DataCORE = new DateTecno();
						notificacao.AutuadoPessoa = notificacao.AutuadoPessoaId > 0 ? new PessoaBus().Obter(notificacao.AutuadoPessoaId) : new Pessoa();
                    }
                    else
                    {
                        notificacao = null;
                    }
                    reader.Close();
                }

				#region Anexos

				if (notificacao?.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"
				select a.id Id,
					   a.ordem Ordem,
					   a.descricao Descricao,
					   b.nome,
					   b.extensao,
					   b.id arquivo_id,
					   b.caminho,
					   a.tid Tid
				  from {0}tab_fisc_notificacao_arq a, 
					   {0}tab_arquivo b
				 where a.arquivo = b.id
				   and a.notificacao = :notificacao
				 order by a.ordem", EsquemaBanco);

					comando.AdicionarParametroEntrada("notificacao", notificacao.Id, DbType.Int32);

					notificacao.Anexos = bancoDeDados.ObterEntityList<Anexo>(comando, (IDataReader reader, Anexo item) =>
					{
						item.Arquivo.Id = reader.GetValue<int>("arquivo_id");
						item.Arquivo.Caminho = reader.GetValue<string>("caminho");
						item.Arquivo.Nome = reader.GetValue<string>("nome");
						item.Arquivo.Extensao = reader.GetValue<string>("extensao");
					});
				}

				#endregion
			}

			return notificacao;
        }

        internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_notificacao t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

                var retorno = bancoDeDados.ExecutarScalar(comando);

                return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
            }
        }

		#endregion
	}
}