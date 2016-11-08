using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class EnquadramentoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }

		#endregion

		public EnquadramentoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public Enquadramento Salvar(Enquadramento enquadramento, BancoDeDados banco = null)
		{
			if (enquadramento == null)
			{
				throw new Exception("Enquadramento é nulo.");
			}

			if (enquadramento.Id <= 0) 
			{
				enquadramento = Criar(enquadramento, banco);
			}
			else
			{
				enquadramento = Editar(enquadramento, banco);
			}

			return enquadramento;
		}

		internal Enquadramento Criar(Enquadramento enquadramento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Enquadramento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_fisc_enquadramento e (id, fiscalizacao, tid) 
															values({0}seq_tab_fisc_enquadramento.nextval, :fiscalizacao, :tid) 
															returning e.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", enquadramento.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				enquadramento.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Artigos

				if (enquadramento.Artigos != null && enquadramento.Artigos.Count > 0)
				{
					foreach (Artigo item in enquadramento.Artigos)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_fisc_enquadr_artig e
															(id, enquadramento_id, artigo, artigo_paragrafo, combinado_artigo, combinado_artigo_paragrafo, da_do, tid)
															values({0}seq_tab_fisc_enquadr_artig.nextval, :enquadramento_id, :artigo, :artigo_paragrafo, :combinado_artigo, 
															:combinado_artigo_paragrafo, :da_do, :tid) returning e.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("enquadramento_id", enquadramento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("artigo", item.ArtigoTexto, DbType.String);
						comando.AdicionarParametroEntrada("artigo_paragrafo", item.ArtigoParagrafo, DbType.String);
						comando.AdicionarParametroEntrada("combinado_artigo", item.CombinadoArtigo, DbType.String);
						comando.AdicionarParametroEntrada("combinado_artigo_paragrafo", item.CombinadoArtigoParagrafo, DbType.String);
						comando.AdicionarParametroEntrada("da_do", item.DaDo, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}
				}

				#endregion

				#endregion

				Historico.Gerar(enquadramento.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(enquadramento.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();

			}

			return enquadramento;
		}

		internal Enquadramento Editar(Enquadramento enquadramento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Enquadramento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fisc_enquadramento e set e.fiscalizacao = :fiscalizacao, e.tid = :tid
															where e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", enquadramento.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("id", enquadramento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Artigos
				comando = bancoDeDados.CriarComando("delete from {0}tab_fisc_enquadr_artig c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.enquadramento_id = :enquadramento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, enquadramento.Artigos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("enquadramento", enquadramento.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Artigos

				if (enquadramento.Artigos != null && enquadramento.Artigos.Count > 0)
				{
					foreach (Artigo item in enquadramento.Artigos)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_fisc_enquadr_artig e set e.enquadramento_id = :enquadramento_id, 
																e.artigo = :artigo, e.artigo_paragrafo = :artigo_paragrafo, e.combinado_artigo = :combinado_artigo,
																e.combinado_artigo_paragrafo = :combinado_artigo_paragrafo, e.da_do = :da_do, e.tid = :tid 
																where e.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);

						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_fisc_enquadr_artig a
																(id, enquadramento_id, artigo, artigo_paragrafo, combinado_artigo, combinado_artigo_paragrafo, da_do, tid)
																values({0}seq_tab_fisc_enquadr_artig.nextval, :enquadramento_id, :artigo, :artigo_paragrafo, :combinado_artigo, 
																:combinado_artigo_paragrafo, :da_do, :tid) returning a.id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("enquadramento_id", enquadramento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("artigo", item.ArtigoTexto, DbType.String);
						comando.AdicionarParametroEntrada("artigo_paragrafo", item.ArtigoParagrafo, DbType.String);
						comando.AdicionarParametroEntrada("combinado_artigo", item.CombinadoArtigo, DbType.String);
						comando.AdicionarParametroEntrada("combinado_artigo_paragrafo", item.CombinadoArtigoParagrafo, DbType.String);
						comando.AdicionarParametroEntrada("da_do", item.DaDo, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}
				}

				#endregion

				Historico.Gerar(enquadramento.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(enquadramento.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}

			return enquadramento;
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComandoPlSql(@"
					begin
					  delete {0}tab_fisc_enquadr_artig t where t.enquadramento_id = (select id from {0}tab_fisc_enquadramento where fiscalizacao = :fiscalizacao);
					  delete {0}tab_fisc_enquadramento t where t.fiscalizacao = :fiscalizacao;
					end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Enquadramento Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			Enquadramento enquadramento = new Enquadramento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Enquadramento

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid from tab_fisc_enquadramento e
															where e.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						enquadramento.Id = Convert.ToInt32(reader["id"]);
						enquadramento.FiscalizacaoId = fiscalizacaoId;
						enquadramento.Tid = reader["tid"].ToString();

						#region Artigos

						comando = bancoDeDados.CriarComando(@"select e.id, e.artigo, e.artigo_paragrafo, e.combinado_artigo,
															e.combinado_artigo_paragrafo, e.da_do, e.tid from {0}tab_fisc_enquadr_artig e
															where e.enquadramento_id = :enquadramento order by e.id", EsquemaBanco);

						comando.AdicionarParametroEntrada("enquadramento", enquadramento.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								Artigo artigo = new Artigo();
								artigo.Id = Convert.ToInt32(readerAux["id"]);
								artigo.Identificador = Guid.NewGuid().ToString(); //Identificador p/ Tela
								artigo.ArtigoTexto = readerAux["artigo"].ToString();
								artigo.ArtigoParagrafo = readerAux["artigo_paragrafo"].ToString();
								artigo.CombinadoArtigo = readerAux["combinado_artigo"].ToString();
								artigo.CombinadoArtigoParagrafo = readerAux["combinado_artigo_paragrafo"].ToString();
								artigo.DaDo = readerAux["da_do"].ToString();
								artigo.Tid = readerAux["tid"].ToString();

								enquadramento.Artigos.Add(artigo);
							}

							readerAux.Close();
						}

						#endregion
					}

					reader.Close();
				}

				#endregion

			}
			return enquadramento;
		}

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_enquadramento t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

		#endregion
	}
}