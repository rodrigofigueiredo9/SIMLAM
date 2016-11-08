using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF.Data
{
	public class SilviculturaPPFFDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }
		private String EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public SilviculturaPPFFDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(SilviculturaPPFF caracterizacao, BancoDeDados banco)
		{
			if (caracterizacao == null)
			{
				throw new Exception("A Caracterização é nula.");
			}

			if (caracterizacao.Id <= 0)
			{
				Criar(caracterizacao, banco);
			}
			else
			{
				Editar(caracterizacao, banco);
			}
		}

		internal int? Criar(SilviculturaPPFF caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@" insert into {0}crt_silvicultura_ppff c(id, empreendimento, atividade, fomento_tipo, area_total, tid) values(seq_crt_sil_ppf_municipio.nextval, 
                    :empreendimento, :atividade, :fomento_tipo, :area_total, :tid ) returning c.id into :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("fomento_tipo", (int)caracterizacao.FomentoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("area_total", caracterizacao.AreaTotal, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Municipios

				if (caracterizacao.Itens != null && caracterizacao.Itens.Count > 0)
				{
					foreach (SilviculturaPPFFItem item in caracterizacao.Itens)
					{
						comando = bancoDeDados.CriarComando(@" insert into {0}crt_sil_ppf_municipio c(id, caracterizacao, municipio, tid) values(seq_crt_sil_ppf_municipio.nextval, 
                            :caracterizacao, :municipio, :tid) returning c.id into :id ", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("municipio", item.Municipio.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.silviculturappff, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(SilviculturaPPFF caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura_ppff c set c.empreendimento = :empreendimento, c.atividade = :atividade, c.fomento_tipo = :fomento_tipo, c.area_total = :area_total, c.tid = :tid
                    where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("fomento_tipo", caracterizacao.FomentoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("area_total", caracterizacao.AreaTotal, DbType.Decimal);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_sil_ppf_municipio c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Itens.Select(x => x.Id).ToList()));

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Municípios

				if (caracterizacao.Itens != null && caracterizacao.Itens.Count > 0)
				{
					foreach (SilviculturaPPFFItem item in caracterizacao.Itens)
					{
						if (item.Id <= 0)
						{
							comando = bancoDeDados.CriarComando(@" insert into {0}crt_sil_ppf_municipio c(id, caracterizacao, municipio, tid) values(seq_crt_sil_ppf_municipio.nextval, 
                                :caracterizacao, :municipio, :tid) returning c.id into :id ", EsquemaBanco);

							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("municipio", item.Municipio.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							comando.AdicionarParametroSaida("id", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						}
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.silviculturappff, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_silvicultura_ppff c where c.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				int id = 0;
				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					id = Convert.ToInt32(retorno);
				}

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura_ppff c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.silviculturappff, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_sil_ppf_municipio e where e.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_silvicultura_ppff r where r.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal SilviculturaPPFF ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			SilviculturaPPFF caracterizacao = new SilviculturaPPFF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_silvicultura_ppff s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal SilviculturaPPFF Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			SilviculturaPPFF caracterizacao = new SilviculturaPPFF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_silvicultura_ppff s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal SilviculturaPPFF Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			SilviculturaPPFF caracterizacao = new SilviculturaPPFF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.atividade, c.fomento_tipo, c.area_total, c.tid from {0}crt_silvicultura_ppff c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);

						caracterizacao.Atividade = Convert.ToInt32(reader["atividade"]);
						caracterizacao.AreaTotal = reader["area_total"].ToString();
						caracterizacao.FomentoTipo = (eFomentoTipo)Enum.Parse(typeof(eFomentoTipo), reader["fomento_tipo"].ToString());

						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Municípios

				comando = bancoDeDados.CriarComando(@"select cspm.id, cspm.tid, lm.id municipio_id, lm.texto municipio_texto from {0}crt_sil_ppf_municipio cspm, {0}lov_municipio lm 
                    where cspm.caracterizacao = :caracterizacao and lm.id = cspm.municipio order by lm.texto", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					SilviculturaPPFFItem item = null;

					while (reader.Read())
					{
						item = new SilviculturaPPFFItem();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.Municipio.Id = Convert.ToInt32(reader["municipio_id"]);
						item.Municipio.Texto = reader["municipio_texto"].ToString();

						caracterizacao.Itens.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		private SilviculturaPPFF ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			SilviculturaPPFF caracterizacao = new SilviculturaPPFF();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.atividade_id, c.fomento_tipo_id, c.area_total, c.tid from {0}hst_crt_silvicultura_ppff c 
                    where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento_id"]);
						caracterizacao.Atividade = Convert.ToInt32(reader["atividade_id"]);
						caracterizacao.AreaTotal = reader["area_total"].ToString();
						caracterizacao.FomentoTipo = (eFomentoTipo)Enum.Parse(typeof(eFomentoTipo), reader["fomento_tipo_id"].ToString());
						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Municpios

				comando = bancoDeDados.CriarComando(@"select hcspm.id, hcspm.tid, lm.id municipio_id, lm.texto municipio_texto from {0}hst_crt_sil_ppf_municipio hcspm, {0}lov_municipio lm where 
                    hcspm.id_hst = :id_hst and lm.id = s.municipio_id order by lm.texto", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					SilviculturaPPFFItem item = null;

					while (reader.Read())
					{
						item = new SilviculturaPPFFItem();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.Municipio.Id = Convert.ToInt32(reader["municipio_id"]);
						item.Municipio.Texto = reader["municipio_texto"].ToString();

						caracterizacao.Itens.Add(item);
					}

					reader.Close();
				}

				#endregion

			}

			return caracterizacao;

		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			CaracterizacaoPDF caract = new CaracterizacaoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@" select l.texto, c.area_total from {0}crt_silvicultura_ppff c, {0}lov_crt_sil_ppf_fomento l where c.empreendimento = :empreendimento 
                    and c.atividade = :atividade ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Tipo de Fomento", Valor = reader["texto"].ToString() });
						caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Área Total", Valor = reader["area_total"].ToString() });
					}

					reader.Close();
				}
			}

			return caract;
		}

		#endregion

	}
}
