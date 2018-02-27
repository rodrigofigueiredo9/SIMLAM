using System;
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
	public class OutrasPenalidadesDa
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

        public OutrasPenalidadesDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

        public OutrasPenalidades Salvar(OutrasPenalidades outrasPenalidades, BancoDeDados banco = null)
        {
            if (outrasPenalidades == null)
            {
                throw new Exception("Outras penalidades é nulo.");
            }

            if (outrasPenalidades.Id <= 0)
            {
                outrasPenalidades = Criar(outrasPenalidades, banco);
            }
            else
            {
                outrasPenalidades = Editar(outrasPenalidades, banco);
            }

            return outrasPenalidades;
        }

        public OutrasPenalidades Criar(OutrasPenalidades outrasPenalidades, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    insert into {0}tab_fisc_outras_penalidades (id,
                                                                               fiscalizacao,
                                                                               iuf_digital,
                                                                               iuf_numero,
                                                                               iuf_data,
                                                                               serie,
                                                                               descricao,
                                                                               arquivo,
                                                                               tid)
                                    values ({0}seq_fisc_outras_penalidades.nextval,
                                            :fiscalizacao,
                                            :iuf_digital,
                                            :iuf_numero,
                                            :iuf_data,
                                            :serie,
                                            :descricao,
                                            :arquivo,
                                            :tid)
                                    returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", outrasPenalidades.FiscalizacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("iuf_digital", outrasPenalidades.IsDigital, DbType.Boolean);
                comando.AdicionarParametroEntrada("iuf_numero", outrasPenalidades.NumeroIUF, DbType.String);
                comando.AdicionarParametroEntrada("iuf_data", outrasPenalidades.DataLavratura.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("serie", outrasPenalidades.SerieId, DbType.Int32);
                comando.AdicionarParametroEntrada("descricao", outrasPenalidades.Descricao, DbType.String);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroSaida("id", DbType.Int32);

                if (outrasPenalidades.Arquivo == null)
                {
                    comando.AdicionarParametroEntrada("arquivo", DBNull.Value, DbType.Int32);
                }
                else
                {
                    comando.AdicionarParametroEntrada("arquivo", outrasPenalidades.Arquivo.Id, DbType.Int32);
                }

                bancoDeDados.ExecutarNonQuery(comando);

                outrasPenalidades.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

                Historico.Gerar(outrasPenalidades.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(outrasPenalidades.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

                bancoDeDados.Commit();
            }
            return outrasPenalidades;
        }

        public OutrasPenalidades Editar(OutrasPenalidades outrasPenalidades, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fisc_outras_penalidades t
                                    set t.fiscalizacao = :fiscalizacao,
                                        t.iuf_digital = :iuf_digital,
                                        t.iuf_numero = :iuf_numero,
                                        t.iuf_data = :iuf_data,
                                        t.serie = :serie,
                                        t.descricao = :descricao,
                                        t.arquivo = :arquivo,
                                        t.tid = :tid
                                    where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", outrasPenalidades.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("fiscalizacao", outrasPenalidades.FiscalizacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("iuf_digital", outrasPenalidades.IsDigital, DbType.Boolean);
                comando.AdicionarParametroEntrada("iuf_numero", outrasPenalidades.NumeroIUF, DbType.String);
                comando.AdicionarParametroEntrada("iuf_data", outrasPenalidades.DataLavratura.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("serie", outrasPenalidades.SerieId, DbType.Int32);
                comando.AdicionarParametroEntrada("descricao", outrasPenalidades.Descricao, DbType.String);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                if (outrasPenalidades.Arquivo == null)
                {
                    comando.AdicionarParametroEntrada("arquivo", DBNull.Value, DbType.Int32);
                }
                else
                {
                    comando.AdicionarParametroEntrada("arquivo", outrasPenalidades.Arquivo.Id, DbType.Int32);
                }

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(outrasPenalidades.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                Consulta.Gerar(outrasPenalidades.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

                bancoDeDados.Commit();
            }
            return outrasPenalidades;
        }

        public void Excluir(int idFiscalizacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    select nvl(tfop.arquivo, 0) arquivo
                                    from {0}tab_fisc_outras_penalidades tfop
                                    where tfop.fiscalizacao = :idFiscalizacao", EsquemaBanco);
                comando.AdicionarParametroEntrada("idFiscalizacao", idFiscalizacao, DbType.Int32);
                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        int idArquivo = reader.GetValue<int>("arquivo");

                        if (idArquivo > 0)
                        {
                            Comando comandoAux = bancoDeDados.CriarComando(@"
                                                    delete from {0}tab_arquivo ta
                                                    where id = :idArquivo", EsquemaBanco);
                            comando.AdicionarParametroEntrada("idArquivo", idArquivo, DbType.Int32);
                            bancoDeDados.ExecutarNonQuery(comando);
                        }
                    }
                    reader.Close();
                }

                comando = bancoDeDados.CriarComando(@"
                                    delete from {0}tab_fisc_outras_penalidades tfop
                                    where tfop.fiscalizacao = :idFiscalizacao", EsquemaBanco);
                comando.AdicionarParametroEntrada("idFiscalizacao", idFiscalizacao, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                Historico.Gerar(idFiscalizacao, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.excluir, bancoDeDados);

                Consulta.Gerar(idFiscalizacao, eHistoricoArtefato.fiscalizacao, bancoDeDados);

                bancoDeDados.Commit();
            }
        }

		#endregion

		#region Obter / Filtrar

        public OutrasPenalidades Obter(int fiscalizacaoId, BancoDeDados banco = null)
        {
            OutrasPenalidades outrasPenalidades = new OutrasPenalidades();
            
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
                                    select tfop.id,
                                           f.situacao situacao_id,
                                           tfop.iuf_digital,
                                           tfop.iuf_numero,
                                           tfop.iuf_data,
                                           tfop.serie,
                                           lfs.texto serie_texto,
                                           tfop.descricao,
                                           tfop.arquivo,
                                           a.nome arquivo_nome
                                    from {0}tab_fisc_outras_penalidades tfop,
                                         {0}tab_fiscalizacao f,
                                         {0}lov_fiscalizacao_serie lfs,
                                         {0}tab_arquivo a
                                    where tfop.arquivo = a.id(+)
                                          and tfop.fiscalizacao = :fiscalizacao
                                          and (tfop.serie is null or tfop.serie = lfs.id)
                                          and f.id = tfop.fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        outrasPenalidades = new OutrasPenalidades
                        {
                            Id = reader.GetValue<int>("id"),
                            IsDigital = reader.GetValue<bool>("iuf_digital"),
                            NumeroIUF = reader.GetValue<string>("iuf_numero"),
                            SerieId = reader.GetValue<int>("serie"),
                            SerieTexto = reader.GetValue<string>("serie_texto"),
                            Descricao = reader.GetValue<string>("descricao"),
                            FiscalizacaoSituacaoId = reader.GetValue<int>("situacao_id")
                        };

                        outrasPenalidades.Arquivo = new Arquivo
                        {
                            Id = reader.GetValue<int>("arquivo"),
                            Nome = reader.GetValue<string>("arquivo_nome")
                        };

                        if (outrasPenalidades.IsDigital == true && outrasPenalidades.FiscalizacaoSituacaoId == (int)eFiscalizacaoSituacao.EmAndamento)
                        {
                            outrasPenalidades.NumeroIUF = null;
                            outrasPenalidades.DataLavratura.Data = DateTime.MinValue;
                        }
                        else
                        {
                            outrasPenalidades.DataLavratura.Data = reader.GetValue<DateTime>("iuf_data");
                        }
                    }
                    reader.Close();
                }
            }

            return outrasPenalidades;
        }

        internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_outras_penalidades t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

                var retorno = bancoDeDados.ExecutarScalar(comando);

                return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
            }
        }

		#endregion
	}
}