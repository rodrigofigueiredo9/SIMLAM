using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Data
{
	public class OrgaoParceiroConveniadoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		public OrgaoParceiroConveniadoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações DML

		internal void Salvar(OrgaoParceiroConveniado entidade, BancoDeDados banco = null)
		{
			if (entidade == null)
			{
				throw new Exception("OrgaoParceiroConveniado é nulo.");
			}

			if (entidade.Id <= 0)
			{
				Criar(entidade, banco);
			}
			else
			{
				Editar(entidade, banco);
			}
		}

		private void Criar(OrgaoParceiroConveniado entidade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Orgao Parceiro/ Conveniado

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_orgao_parc_conv(id, orgao_sigla, orgao_nome, termo_numero_ano, diario_oficial_data, situacao,
															situacao_data, tid) values(seq_orgao_parc_conv.nextval, :orgao_sigla, :orgao_nome, :termo_numero_ano, 
															:diario_oficial_data, :situacao, :situacao_data, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("orgao_sigla", entidade.Sigla, DbType.String);
				comando.AdicionarParametroEntrada("orgao_nome", entidade.Nome, DbType.String);
				comando.AdicionarParametroEntrada("termo_numero_ano", entidade.TermoNumeroAno, DbType.String);
				comando.AdicionarParametroEntrada("diario_oficial_data", entidade.DiarioOficialData.Data, DbType.Date);
				comando.AdicionarParametroEntrada("situacao", (int)eOrgaoParceiroConveniadoSituacao.Ativo, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_data", DateTime.Now, DbType.Date);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				entidade.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Sigla/ Unidades

				if (entidade.Unidades != null && entidade.Unidades.Count > 0)
				{
					foreach (Unidade item in entidade.Unidades)
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_orgao_parc_conv_sigla_unid(id, orgao_parc_conv, sigla, nome_local,tid) 
															values(seq_orgao_parc_conv_sigla_unid.nextval, :orgao_parc_conv_id, :sigla, :nome_local, :tid) 
															returning id into :id", EsquemaBanco);


						comando.AdicionarParametroEntrada("orgao_parc_conv_id", entidade.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("sigla", item.Sigla, DbType.String);
						comando.AdicionarParametroEntrada("nome_local", item.Nome, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					}
				}

				#endregion

				#region Historico

				Historico.Gerar(entidade.Id, eHistoricoArtefato.orgaoparceiroconveniado, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(OrgaoParceiroConveniado entidade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Orgao Parceiro/ Conveniado

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_orgao_parc_conv t set t.orgao_sigla = :orgao_sigla, t.orgao_nome = :orgao_nome, 
																		t.termo_numero_ano = :termo_numero_ano, t.diario_oficial_data = :diario_oficial_data,
																		t.tid = :tid where t.id = :id");

				comando.AdicionarParametroEntrada("orgao_sigla", entidade.Sigla, DbType.String);
				comando.AdicionarParametroEntrada("orgao_nome", entidade.Nome, DbType.String);
				comando.AdicionarParametroEntrada("termo_numero_ano", entidade.TermoNumeroAno, DbType.String);
				comando.AdicionarParametroEntrada("diario_oficial_data", entidade.DiarioOficialData.Data, DbType.Date);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando("delete from {0}tab_orgao_parc_conv_sigla_unid c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.orgao_parc_conv = :orgao_parc_conv_id{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, entidade.Unidades.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("orgao_parc_conv_id", entidade.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Sigla/ Unidades

				if (entidade.Unidades != null && entidade.Unidades.Count > 0)
				{
					foreach (Unidade item in entidade.Unidades)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update tab_orgao_parc_conv_sigla_unid s set s.orgao_parc_conv = :orgao_parc_conv_id, 
																s.sigla = :sigla, s.nome_local = :nome_local, s.tid  = :tid where s.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into tab_orgao_parc_conv_sigla_unid(id, orgao_parc_conv, sigla, nome_local,tid) 
																values(seq_orgao_parc_conv_sigla_unid.nextval, :orgao_parc_conv_id, :sigla, :nome_local, :tid) 
																returning id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("orgao_parc_conv_id", entidade.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("sigla", item.Sigla, DbType.String);
						comando.AdicionarParametroEntrada("nome_local", item.Nome, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}
				}

				#endregion

				#region Historico

				Historico.Gerar(entidade.Id, eHistoricoArtefato.orgaoparceiroconveniado, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(OrgaoParceiroConveniado entidade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Orgao Parceiro/ Conveniado

				Comando comando = comando = bancoDeDados.CriarComando(@"update {0}tab_orgao_parc_conv t set t.situacao = :situacao, t.situacao_motivo = :situacao_motivo,
																		t.situacao_data = :situacao_data, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", entidade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_motivo", entidade.SituacaoMotivo, DbType.String);
				comando.AdicionarParametroEntrada("situacao_data", entidade.SituacaoData.Data, DbType.Date);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Historico

				Historico.Gerar(entidade.Id, eHistoricoArtefato.orgaoparceiroconveniado, eHistoricoAcao.alterarsituacao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_orgao_parc_conv t set t.tid = :tid where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.orgaoparceiroconveniado, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComando(@"begin" +
														"delete from {0}tab_orgao_parc_conv t where t.id = :id" +
														"delete from {0}tab_orgao_parc_conv_sigla_unid u where u.orgao_parc_conv = :id" +
													"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		internal OrgaoParceiroConveniado Obter(int id, BancoDeDados banco = null)
		{
			OrgaoParceiroConveniado entidade = new OrgaoParceiroConveniado();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Orgao Parceiro/ Conveniado

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.orgao_sigla, t.orgao_nome, t.termo_numero_ano, t.diario_oficial_data, 
															t.situacao, l.texto situacao_texto, t.situacao_motivo, t.situacao_data, t.tid 
															from tab_orgao_parc_conv t, lov_orgao_parc_conv_situacao l 
															where t.id = :id and l.id = t.situacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Sigla = reader.GetValue<String>("orgao_sigla");
						entidade.Nome = reader.GetValue<String>("orgao_nome");
						entidade.TermoNumeroAno = reader.GetValue<String>("termo_numero_ano");
						entidade.DiarioOficialData.DataTexto = reader.GetValue<String>("diario_oficial_data");
						entidade.SituacaoId = reader.GetValue<Int32>("situacao");
						entidade.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						entidade.SituacaoMotivo = reader.GetValue<String>("situacao_motivo");
						entidade.SituacaoData.DataTexto = reader.GetValue<String>("situacao_data");
						entidade.Tid = reader.GetValue<String>("tid");
					}

					reader.Close();

				}

				#endregion

				#region Sigla/ Unidades

				comando = bancoDeDados.CriarComando(@"select u.id, u.orgao_parc_conv, u.sigla, u.nome_local, u.tid from tab_orgao_parc_conv_sigla_unid u 
													where u.orgao_parc_conv = :orgao_parc_conv_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("orgao_parc_conv_id", entidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Unidade unidade = null;

					while (reader.Read())
					{
						unidade = new Unidade();
						unidade.Id = reader.GetValue<Int32>("id");
						unidade.Sigla = reader.GetValue<String>("sigla");
						unidade.Nome = reader.GetValue<String>("nome_local");
						unidade.Tid = reader.GetValue<String>("tid");

						entidade.Unidades.Add(unidade);

					}

					reader.Close();

				}

				#endregion
			}

			return entidade;
		}

		internal Resultados<OrgaoParceiroConveniadoListarResultados> Filtrar(Filtro<OrgaoParceiroConveniadoListarFiltros> filtros, BancoDeDados banco = null)
		{
			Resultados<OrgaoParceiroConveniadoListarResultados> retorno = new Resultados<OrgaoParceiroConveniadoListarResultados>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = string.IsNullOrEmpty(EsquemaBanco) ? "" : ".";
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("t.orgao_sigla", "sigla", filtros.Dados.Sigla, true);

				comandtxt += comando.FiltroAndLike("t.orgao_nome", "nome", filtros.Dados.Nome, true);

				comandtxt += comando.FiltroAnd("t.situacao", "situacao", filtros.Dados.SituacaoId);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "sigla", "nome", "situacao_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("sigla");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(1) from tab_orgao_parc_conv t where 1 = 1" + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select t.id, t.orgao_sigla sigla, t.orgao_nome nome, t.situacao ,l.texto situacao_texto from tab_orgao_parc_conv t , lov_orgao_parc_conv_situacao l where 
                t.situacao= l.id" + comandtxt + DaHelper.Ordenar(colunas, ordenar);

				comando.DbCommand.CommandText = String.Format(@"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", esquemaBanco);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando o retorno

					OrgaoParceiroConveniadoListarResultados item;

					while (reader.Read())
					{
						item = new OrgaoParceiroConveniadoListarResultados();
						item.Id = reader.GetValue<int>("id");
						item.Nome = reader.GetValue<string>("nome");
						item.Sigla = reader.GetValue<string>("sigla");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.SituacaoId = reader.GetValue<int>("situacao");
						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion Adicionando o retorno
				}
			}

			return retorno;
		}

		internal List<Lista> ObterUnidadesLst(int orgao, BancoDeDados banco = null)
		{
			List<Lista> unidades = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Unidades

				Comando comando = bancoDeDados.CriarComando(@"select u.id, u.sigla, u.nome_local from tab_orgao_parc_conv_sigla_unid u 
															where u.orgao_parc_conv = :orgao", EsquemaBanco);

				comando.AdicionarParametroEntrada("orgao", orgao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista unidade = null;

					while (reader.Read())
					{
						unidade = new Lista();
						unidade.Id = reader.GetValue<String>("id");
						unidade.Texto = reader.GetValue<String>("sigla") + " - " + reader.GetValue<String>("nome_local");
						unidade.IsAtivo = true;

						unidades.Add(unidade);

					}

					reader.Close();

				}

				#endregion
			}

			return unidades;
		}

		internal List<Lista> ObterOrgaosParceirosLst(BancoDeDados banco = null)
		{
			List<Lista> orgaosParceiros = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Orgaos Parceiros/ Conveniados

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.orgao_sigla, t.orgao_nome from tab_orgao_parc_conv t", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista orgao = null;

					while (reader.Read())
					{
						orgao = new Lista();
						orgao.Id = reader.GetValue<String>("id");
						orgao.Texto = reader.GetValue<String>("orgao_sigla") + " - " + reader.GetValue<String>("orgao_nome");
						orgao.IsAtivo = true;

						orgaosParceiros.Add(orgao);

					}

					reader.Close();

				}

				#endregion
			}

			return orgaosParceiros;
		}

		#endregion

		#region Validacoes

		internal bool PossuiCredenciadoAssociado(Unidade unidade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado t where t.orgao_parc_unid = :unidade", EsquemaBanco);

				comando.AdicionarParametroEntrada("unidade", unidade.Id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool VerificarSituacaoAlterada(OrgaoParceiroConveniado orgao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				comando = bancoDeDados.CriarComando(@"select situacao from tab_orgao_parc_conv t where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", orgao.Id, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) != orgao.SituacaoId;
			}
		}

		internal bool VerificarCredenciadoAssociadoOrgao(CredenciadoPessoa credenciado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				comando = bancoDeDados.CriarComando(@"select orgao_parc from tab_credenciado t where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) == credenciado.OrgaoParceiroId;
			}
		}

		internal bool VerificarSituacaoAtiva(int orgaoParceiroId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				comando = bancoDeDados.CriarComando(@"select situacao from tab_orgao_parc_conv where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", orgaoParceiroId, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) == (int)eOrgaoParceiroConveniadoSituacao.Ativo;
			}
		}

		internal bool Existe(OrgaoParceiroConveniado orgaoParceiro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;
				if (orgaoParceiro.Id < 1)
				{
					comando = bancoDeDados.CriarComando(@"select count(*) from tab_orgao_parc_conv where orgao_nome = :nome and orgao_sigla = :sigla", EsquemaBanco);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select count(*) from tab_orgao_parc_conv where orgao_nome = :nome and orgao_sigla = :sigla and id != :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", orgaoParceiro.Id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("nome", orgaoParceiro.Nome, DbType.String);
				comando.AdicionarParametroEntrada("sigla", orgaoParceiro.Sigla, DbType.String);


				return bancoDeDados.ExecutarScalar<int>(comando) < 1;
			}
		}

		#endregion
	}
}