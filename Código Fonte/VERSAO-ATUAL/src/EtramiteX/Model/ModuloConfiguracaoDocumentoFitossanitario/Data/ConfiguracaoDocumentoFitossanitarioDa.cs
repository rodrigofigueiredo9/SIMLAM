using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Data
{
	class ConfiguracaoDocumentoFitossanitarioDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		#region DMLs

		internal void Salvar(ConfiguracaoDocumentoFitossanitario configuracao, BancoDeDados banco = null)
		{
			if (configuracao == null)
			{
				throw new Exception("Configuração é nula.");
			}

			if (configuracao.ID <= 0)
			{
				Criar(configuracao, banco);
			}
			else
			{
				Editar(configuracao, banco);
			}
		}

        internal void SalvarEdicaoIntervalo(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado, BancoDeDados banco = null)
        {
            if (configuracao == null)
            {
                throw new Exception("Configuração é nula.");
            }

            EditarIntervalo(configuracao, idEditado, banco);
        }

		internal void Criar(ConfiguracaoDocumentoFitossanitario configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into cnf_doc_fitossanitario (id, tid) values (seq_cnf_doc_fitossanitario.nextval, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				configuracao.ID = comando.ObterValorParametro<int>("id");

				foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into cnf_doc_fito_intervalo (id, tid, configuracao, tipo_documento, tipo, numero_inicial, numero_final) 
					values(seq_cnf_doc_fito_intervalo.nextval, :tid, :configuracao, :tipo_documento, :tipo, :numero_inicial, :numero_final)");

					comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_documento", item.TipoDocumentoID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_inicial", item.NumeroInicial, DbType.Int64);
					comando.AdicionarParametroEntrada("numero_final", item.NumeroFinal, DbType.Int64);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Editar(ConfiguracaoDocumentoFitossanitario configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update cnf_doc_fitossanitario t set t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", configuracao.ID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando("delete from cnf_doc_fito_intervalo where configuracao = :configuracao ");
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "id", DbType.Int32, configuracao.DocumentoFitossanitarioIntervalos.Select(x => x.ID).ToList());
				comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
				{
					if (item.ID > 0)
					{
						continue;
					}

					comando = bancoDeDados.CriarComando(@"
					insert into cnf_doc_fito_intervalo (id, tid, configuracao, tipo_documento, tipo, numero_inicial, numero_final) 
					values (seq_cnf_doc_fito_intervalo.nextval, :tid, :configuracao, :tipo_documento, :tipo, :numero_inicial, :numero_final)");

					comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_documento", item.TipoDocumentoID, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_inicial", item.NumeroInicial, DbType.Int64);
					comando.AdicionarParametroEntrada("numero_final", item.NumeroFinal, DbType.Int64);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

        internal void EditarIntervalo(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update cnf_doc_fitossanitario t set t.tid = :tid where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", configuracao.ID, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                comando = bancoDeDados.CriarComando("delete from cnf_doc_fito_intervalo where configuracao = :configuracao ");
                comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "id", DbType.Int32, configuracao.DocumentoFitossanitarioIntervalos.Select(x => x.ID).ToList());
                comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
                {
                    if (item.ID != idEditado)
                    {
                        continue;
                    }

                    comando = bancoDeDados.CriarComando(@"
					update cnf_doc_fito_intervalo set numero_inicial=" + item.NumeroInicial + ", numero_final=" + item.NumeroFinal + " where id = " + item.ID);

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.atualizar, bancoDeDados);

                bancoDeDados.Commit();
            }
        }

        internal void Excluir(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"update cnf_doc_fitossanitario t set t.tid = :tid where t.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("id", configuracao.ID, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                comando = bancoDeDados.CriarComando("delete from cnf_doc_fito_intervalo where configuracao = :configuracao ");
                comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "id", DbType.Int32, configuracao.DocumentoFitossanitarioIntervalos.Select(x => x.ID).ToList());
                comando.AdicionarParametroEntrada("configuracao", configuracao.ID, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in configuracao.DocumentoFitossanitarioIntervalos)
                {
                    if (item.ID != idEditado)
                    {
                        continue;
                    }

                    comando = bancoDeDados.CriarComando(@"
					delete from cnf_doc_fito_intervalo where id = " + item.ID);

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                Historico.Gerar(configuracao.ID, eHistoricoArtefato.configdocumentofitossanitario, eHistoricoAcao.atualizar, bancoDeDados);

                bancoDeDados.Commit();
            }
        }

		#endregion

		#region Obter

		internal ConfiguracaoDocumentoFitossanitario Obter(bool simplificado = false, BancoDeDados banco = null)
		{
			ConfiguracaoDocumentoFitossanitario retorno = new ConfiguracaoDocumentoFitossanitario();

			using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDedados.CriarComando(@"select id, tid from cnf_doc_fitossanitario", EsquemaBanco);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.ID = reader.GetValue<int>("id");
						retorno.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				if (retorno == null || retorno.ID < 1 || simplificado)
				{
					return retorno;
				}

				#region Intervalos

				comando = bancoDedados.CriarComando(@"select i.id, i.tid, i.tipo_documento, lt.texto tipo_documento_texto, i.tipo, i.numero_inicial, i.numero_final 
				from cnf_doc_fito_intervalo i, lov_doc_fitossanitarios_tipo lt where lt.id = i.tipo_documento and i.configuracao = :configuracao", EsquemaBanco);

				comando.AdicionarParametroEntrada("configuracao", retorno.ID, DbType.Int32);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					DocumentoFitossanitario item = null;

					while (reader.Read())
					{
						item = new DocumentoFitossanitario();
						item.ID = reader.GetValue<int>("id");
						item.TID = reader.GetValue<string>("tid");
						item.TipoDocumentoID = reader.GetValue<int>("tipo_documento");
						item.TipoDocumentoTexto = reader.GetValue<string>("tipo_documento_texto");
						item.Tipo = reader.GetValue<int>("tipo");
						item.NumeroInicial = reader.GetValue<long>("numero_inicial");
						item.NumeroFinal = reader.GetValue<long>("numero_final");
						retorno.DocumentoFitossanitarioIntervalos.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

        internal List<long> LiberadosIntervalo(int tipo, long inicio, long fim, BancoDeDados banco = null)
        {
            List<long> retorno = new List<long>();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando;

                if (tipo == (int)eDocumentoFitossanitarioTipo.CFO || tipo == (int)eDocumentoFitossanitarioTipo.CFOC)
                {
                    comando = bancoDeDados.CriarComando(@"
				              select t.*
                              from tab_numero_cfo_cfoc t, hst_liberacao_cfo_cfoc h 
				              where h.liberacao_id = t.liberacao
                                    and t.numero >= " + inicio +
                                    " and t.numero <= " + fim +
                                    " and t.tipo_documento = " + tipo);

                    comando.DbCommand.CommandText += " order by t.numero";
                }
                else //PTV
                {
                    comando = bancoDeDados.CriarComando(@"
				              select t.NUMERO
                              from tab_ptv t
                              where t.NUMERO >= " + inicio +
                                    " and t.NUMERO <= " + fim +
                              " order by t.NUMERO");
                }

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    long numero;
                    while (reader.Read())
                    {
                        numero = reader.GetValue<long>("numero");
                        retorno.Add(numero);
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

		#endregion

		#region Validaçoes

		internal bool ContidoIntervalo(DocumentoFitossanitario intervalo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from cnf_doc_fito_intervalo t 
				where ((:numero_inicial between t.numero_inicial and t.numero_final) or (t.numero_inicial between :numero_inicial and :numero_final))
				and tipo = :tipo and tipo_documento = :tipo_documento and id != :intervalo", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero_inicial", intervalo.NumeroInicial, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_final", intervalo.NumeroFinal, DbType.Int64);
				comando.AdicionarParametroEntrada("tipo", intervalo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_documento", intervalo.TipoDocumentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("intervalo", intervalo.ID, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion
	}
}