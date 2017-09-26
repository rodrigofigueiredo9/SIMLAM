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

        internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_multa t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

                var retorno = bancoDeDados.ExecutarScalar(comando);

                return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
            }
        }

		#endregion
	}
}