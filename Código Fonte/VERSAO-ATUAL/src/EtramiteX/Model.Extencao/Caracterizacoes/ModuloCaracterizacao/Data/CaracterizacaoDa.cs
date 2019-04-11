using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data
{
	public class CaracterizacaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();

		internal Historico Historico { get { return _historico; } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Caracterização

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		private string EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public CaracterizacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Dependencias(Caracterizacao caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Dependências da caracterização

				Comando comando = bancoDeDados.CriarComando("delete from {0}crt_dependencia c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.dependente_tipo = :dependente_tipo and c.dependente_caracterizacao = :dependente_caracterizacao and c.dependente_id = :dependente_id {0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Dependencias.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("dependente_tipo", (int)caracterizacao.DependenteTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)caracterizacao.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_id", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				if (caracterizacao.Dependencias != null && caracterizacao.Dependencias.Count > 0)
				{
					foreach (Dependencia item in caracterizacao.Dependencias)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_dependencia d set d.dependencia_tipo = :dependencia_tipo, d.dependente_caracterizacao = :dependente_caracterizacao,
							d.dependencia_id = :dependencia_id, d.dependencia_tid = :dependencia_tid, d.tid = :tid where d.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_dependencia d (id, dependente_tipo, dependente_caracterizacao, dependente_id, dependencia_tipo,
							dependencia_caracterizacao, dependencia_id, dependencia_tid, tid) values ({0}seq_crt_dependencia.nextval, :dependente_tipo, :dependente_caracterizacao,
							:dependente_id, :dependencia_tipo, :dependencia_caracterizacao, :dependencia_id, :dependencia_tid, :tid) returning id into :id_dep", EsquemaBanco);

							comando.AdicionarParametroEntrada("dependente_tipo", (int)caracterizacao.DependenteTipo, DbType.Int32);
							comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)caracterizacao.Tipo, DbType.Int32);
							comando.AdicionarParametroEntrada("dependente_id", caracterizacao.Id, DbType.Int32);

							comando.AdicionarParametroSaida("id_dep", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("dependencia_tipo", item.DependenciaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_caracterizacao", item.DependenciaCaracterizacao, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_id", item.DependenciaId, DbType.Int32);
						comando.AdicionarParametroEntrada("dependencia_tid", DbType.String, 36, item.DependenciaTid);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id_dep"));
						}

						Historico.Gerar(item.Id, eHistoricoArtefatoCaracterizacao.dependencia, eHistoricoAcao.atualizar, bancoDeDados);
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AtualizarDependentes(int dependenciaID, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo eCaracterizacaoDependenciaTipo, string dependenciaTID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_dependencia c set c.dependencia_tid = :dependencia_tid, c.tid = :tid
				where c.dependencia_id = :dependencia_id and c.dependencia_tipo = :dependencia_tipo and c.dependencia_caracterizacao = :dependencia_caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("dependencia_caracterizacao", (int)caracterizacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_tipo", (int)eCaracterizacaoDependenciaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_id", dependenciaID, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_tid", DbType.String, 36, dependenciaTID);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		#endregion

		#region Obter / Filtrar

		public EmpreendimentoCaracterizacao ObterEmpreendimentoSimplificado(int id, BancoDeDados banco = null)
		{
			EmpreendimentoCaracterizacao empreendimento = new EmpreendimentoCaracterizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@" select
				e.id, e.codigo, ls.texto segmento_texto, ls.denominador, e.cnpj, e.denominador denominador_nome, e.tid, ee.zona, ee.municipio municipio_id,
				(select m.texto from {0}lov_municipio m where m.id = ee.municipio) municipio, (select m.ibge from {0}lov_municipio m where m.id = ee.municipio) municipio_ibge, 
				cm.id modulo_id, cm.modulo_ha, (select es.sigla from {0}lov_estado es where es.id = ee.estado) estado, 
				case when ee.zona = 1 then 'Urbana' else 'Rural' end zona_texto,
				(select sum(dd.area_croqui) from crt_dominialidade_dominio dd
					where exists
					(select 1 from crt_dominialidade d
					where d.id = dd.dominialidade
					and d.empreendimento = e.id)) area_croqui
				from {0}tab_empreendimento e, {0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento ls, {0}tab_empreendimento_endereco ee, {0}cnf_municipio_mod_fiscal cm 
				where e.atividade = a.id(+) and e.segmento = ls.id and ee.correspondencia = 0 and ee.empreendimento = e.id and ee.municipio = cm.municipio(+) and e.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = id;
						empreendimento.Tid = reader.GetValue<string>("tid");
						empreendimento.DenominadorTipo = reader.GetValue<string>("denominador");
						empreendimento.Denominador = reader.GetValue<string>("denominador_nome");
						empreendimento.MunicipioId = reader.GetValue<int>("municipio_id");
						empreendimento.MunicipioIBGE = reader.GetValue<int>("municipio_ibge");
						empreendimento.Municipio = reader.GetValue<string>("municipio");
						empreendimento.ModuloFiscalId = reader.GetValue<int>("modulo_id");
						empreendimento.ModuloFiscalHA = reader.GetValue<decimal>("modulo_ha");
						empreendimento.Uf = reader.GetValue<string>("estado");
						empreendimento.ZonaLocalizacao = (eZonaLocalizacao)reader.GetValue<int>("zona");
						empreendimento.ZonaLocalizacaoTexto = reader.GetValue<string>("zona_texto");
						empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						empreendimento.Codigo = reader.GetValue<int>("codigo");
						empreendimento.AreaImovelHA = reader.GetValue<decimal>("area_croqui");
						if (empreendimento.AreaImovelHA > 0)
							empreendimento.AreaImovelHA = empreendimento.AreaImovelHA / 10000;
					}

					reader.Close();
				}

				#endregion
			}

			return empreendimento;
		}

		public List<Caracterizacao> ObterCaracterizacoes(int empreendimentoId, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Lista das caracterizações do empreendimento

				Comando comando = bancoDeDados.CriarComando(@" select 'select '||t.id||' tipo,
				a.id caracterizacao_id, a.tid caracterizacao_tid from {0}'||t.tabela||' a where a.empreendimento =:empreendimento and rownum = 1 union all ' campo 
				from {0}lov_caracterizacao_tipo t where t.id != :caracterizacao ", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.CadastroAmbientalRural, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader["campo"].ToString();
					}

					reader.Close();
				}

				#region CAR
				
                select += @" ( select TO_NUMBER(:caracterizacao) tipo, t.caracterizacao_id,  
                      nvl((select b.tid from tmp_cad_ambiental_rural b where b.id = t.caracterizacao_id), 
                          (select b.tid from crt_cad_ambiental_rural b where b.id = t.caracterizacao_id)) caracterizacao_tid
                from (select nvl((select a.id from crt_cad_ambiental_rural a where a.empreendimento = :empreendimento), 
                      (select a.id from tmp_cad_ambiental_rural a where a.empreendimento = :empreendimento)) caracterizacao_id from dual) t ) union all ";

                 

                #endregion

                if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(@" 
						select lc.id                tipo,
							   lc.texto             tipo_texto,
							   pg_rascunho.id       projeto_rascunho_id,
							   c.caracterizacao_id,
							   c.caracterizacao_tid,
							   pg.id                projeto_id,
							   pg.tid               projeto_tid,
							   dscLicAtividade.id  dscLicAtividade_id,
							   dscLicAtividade.tid dscLicAtividade_tid
						  from {0}lov_caracterizacao_tipo lc,
							   (" + select.Substring(0, select.Length - 10) + @" ) c,
							   (select p.id, p.tid, p.empreendimento, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento) pg,
							   (select p.id, p.tid, p.empreendimento, p.caracterizacao from {0}tmp_projeto_geo p where p.empreendimento = :empreendimento) pg_rascunho,
							   (select p.id, p.tid, p.empreendimento, p.caracterizacao from {0}crt_dsc_lc_atividade p where p.empreendimento = :empreendimento) dscLicAtividade
						 where lc.id = pg.caracterizacao(+)
						   and lc.id = pg_rascunho.caracterizacao(+)
						   and lc.id = c.tipo(+)
						   and lc.id = dscLicAtividade.caracterizacao(+)
						 order by lc.id ", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
					comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.CadastroAmbientalRural, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Caracterizacao caracterizacao = null;

						while (reader.Read())
						{
							caracterizacao = new Caracterizacao();

							if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
							{
								caracterizacao.Tipo = (eCaracterizacao)Convert.ToInt32(reader["tipo"]);
								caracterizacao.Nome = reader["tipo_texto"].ToString();
							}

							if (reader["caracterizacao_id"] != null && !Convert.IsDBNull(reader["caracterizacao_id"]))
							{
								caracterizacao.Id = Convert.ToInt32(reader["caracterizacao_id"]);
								caracterizacao.Tid = reader["caracterizacao_tid"].ToString();
							}

							if (reader["projeto_id"] != null && !Convert.IsDBNull(reader["projeto_id"]))
							{
								caracterizacao.ProjetoId = Convert.ToInt32(reader["projeto_id"]);
								caracterizacao.ProjetoTid = reader["projeto_tid"].ToString();
							}

							if (reader["projeto_rascunho_id"] != null && !Convert.IsDBNull(reader["projeto_rascunho_id"]))
							{
								caracterizacao.ProjetoRascunhoId = Convert.ToInt32(reader["projeto_rascunho_id"]);
							}

							caracterizacao.DscLicAtividadeId = reader.GetValue<int>("dscLicAtividade_id");
							caracterizacao.DscLicAtividadeTid = reader.GetValue<string>("dscLicAtividade_tid");

							lista.Add(caracterizacao);
						}

						reader.Close();
					}
				}

				#endregion
			}

			return lista;
		}

        public List<Caracterizacao> ObterCaracterizacoesCAR(Int64 empreendimentoCod, BancoDeDados banco = null)
        {
            List<Caracterizacao> lista = new List<Caracterizacao>();
            String select = String.Empty;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
						SELECT 22 tipo, 'Cadastro Ambiental Rural' tipo_texto, CR.ID caracterizacao_id, CR.TID caracterizacao_tid  
                            FROM TAB_EMPREENDIMENTO E
                                INNER JOIN CRT_CAD_AMBIENTAL_RURAL CR ON CR.EMPREENDIMENTO = E.ID 
                            WHERE E.CODIGO = :empreendimento");

                comando.AdicionarParametroEntrada("empreendimento", empreendimentoCod, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Caracterizacao caracterizacao = null;

                    while (reader.Read())
                    {
                        caracterizacao = new Caracterizacao();

                        if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
                        {
                            caracterizacao.Tipo = (eCaracterizacao)Convert.ToInt32(reader["tipo"]);
                            caracterizacao.Nome = reader["tipo_texto"].ToString();
                        }

                        if (reader["caracterizacao_id"] != null && !Convert.IsDBNull(reader["caracterizacao_id"]))
                        {
                            caracterizacao.Id = Convert.ToInt32(reader["caracterizacao_id"]);
                            caracterizacao.Tid = reader["caracterizacao_tid"].ToString();
                        }

                        lista.Add(caracterizacao);
                    }

                    reader.Close();
                }
            }

            return lista;
        }

		public List<Dependencia> ObterDependenciasAtual(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, BancoDeDados banco = null)
		{
			List<Dependencia> lista = new List<Dependencia>();
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Lista das caracterizações do empreendimento

				Comando comando = bancoDeDados.CriarComando(@"
				select case
						 when c.tipo = 1 then /*Projeto Geográfico*/
						  'select ' || t.id || ' caracterizacao, ' || c.tipo ||
						  ' tipo,
						a.id dependencia_id, a.tid dependencia_tid from {0}crt_projeto_geo a where a.empreendimento = :empreendimento and a.caracterizacao = ' || t.id ||
						  ' union '
						 when c.tipo = 2 then /*Caracterização*/
						  'select ' || t.id || ' caracterizacao, ' || c.tipo ||
						  ' tipo, a.id dependencia_id,
						a.tid dependencia_tid from {0}' || t.tabela ||
						  ' a where a.empreendimento = :empreendimento union '
						 when c.tipo = 3 then /*Descrição de Licenciamento de Atividade*/
						  'select ' || t.id || ' caracterizacao, ' || c.tipo ||
						  ' tipo,
						a.id dependencia_id, a.tid dependencia_tid from {0}crt_dsc_lc_atividade a where a.empreendimento = :empreendimento and a.caracterizacao = ' || t.id ||
						  ' union '
					   end campo
				  from {0}lov_caracterizacao_tipo t,
					   (select d.dependencia, d.tipo
						  from {0}cnf_caracterizacao d
						 where d.caracterizacao = :caracterizacao
						   and d.tipo_detentor = :tipo_detentor) c
				 where t.id = c.dependencia", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_detentor", tipo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader["campo"].ToString();
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(select.Substring(0, select.Length - 6), EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Dependencia dependencia = null;

						while (reader.Read())
						{
							dependencia = new Dependencia();

							dependencia.DependenciaTipo = Convert.ToInt32(reader["tipo"]);
							dependencia.DependenciaId = Convert.ToInt32(reader["dependencia_id"]);
							dependencia.DependenciaTid = reader["dependencia_tid"].ToString();
							dependencia.DependenciaCaracterizacao = Convert.ToInt32(reader["caracterizacao"]);

							lista.Add(dependencia);
						}

						reader.Close();
					}
				}

				#endregion
			}
			return lista;
		}

		public List<Dependencia> ObterDependencias(int dependenteId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, BancoDeDados banco = null)
		{

			List<Dependencia> lista = new List<Dependencia>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Lista das caracterizações do empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select d.dependencia_tipo, d.dependencia_caracterizacao, d.dependencia_id, d.dependencia_tid
				from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_caracterizacao = :dependente_caracterizacao and d.dependente_id = :dependente_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("dependente_tipo", (int)tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)caracterizacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_id", dependenteId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dependencia dependencia = null;

					while (reader.Read())
					{
						dependencia = new Dependencia();

						dependencia.DependenciaTipo = Convert.ToInt32(reader["dependencia_tipo"]);
						dependencia.DependenciaCaracterizacao = Convert.ToInt32(reader["dependencia_caracterizacao"]);
						dependencia.DependenciaId = Convert.ToInt32(reader["dependencia_id"]);
						dependencia.DependenciaTid = reader["dependencia_tid"].ToString();

						lista.Add(dependencia);
					}

					reader.Close();
				}

				#endregion
			}

			return lista;
		}

		internal List<ProjetoGeografico> ObterProjetosEmpreendimento(int id, BancoDeDados banco = null)
		{
			List<ProjetoGeografico> projetos = new List<ProjetoGeografico>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ProjetoGeografico projeto = null;

					while (reader.Read())
					{
						projeto = new ProjetoGeografico();
						projeto.Id = Convert.ToInt32(reader["id"]);
						projeto.Tid = reader["tid"].ToString();
						projeto.CaracterizacaoId = Convert.ToInt32(reader["caracterizacao"]);

						projetos.Add(projeto);
					}

					reader.Close();
				}
			}

			return projetos;
		}

		internal List<Dependencia> ObterDependentes(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo? dependenciaTipo = null, BancoDeDados banco = null)
		{
			List<Dependencia> lista = new List<Dependencia>();
			String select = String.Empty;
			String parametro = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("");

				#region Lista das caracterizações do empreendimento

				if (dependenciaTipo != null)
				{
					parametro = comando.FiltroAnd("d.dependencia_tipo", "dependencia_tipo", (int)dependenciaTipo);
				}
				else
				{
					parametro = comando.FiltroAnd("d.dependente_caracterizacao", "dependente_caracterizacao", (int)caracterizacaoTipo, isDiferente: true);
				}

				comando = bancoDeDados.CriarComando(@"select 'select * from (select d.dependente_tipo, d.dependente_caracterizacao from {0}crt_dependencia d,
				{0}crt_projeto_geo g where d.dependencia_caracterizacao = 1 and d.dependencia_id = g.id and d.dependencia_tipo = :caracterizacao and g.caracterizacao = :caracterizacao
				and g.empreendimento = :empreendimento " + parametro + @" union all select d.dependente_tipo, d.dependente_caracterizacao from {0}crt_dependencia d, {0}'|| lc.tabela ||' c
				where d.dependencia_id = c.id and d.dependencia_caracterizacao = :caracterizacao and d.dependencia_tipo = :empreendimento and c.empreendimento = :empreendimento " +
				parametro + @") t group by dependente_tipo, dependente_caracterizacao' retorno from lov_caracterizacao_tipo lc where lc.id = :caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", (int)caracterizacaoTipo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						select = reader["retorno"].ToString();
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(select, EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
					comando.AdicionarParametroEntrada("caracterizacao", (int)caracterizacaoTipo, DbType.Int32);

					if (dependenciaTipo != null)
					{
						comando.AdicionarParametroEntrada("dependencia_tipo", (int)dependenciaTipo, DbType.Int32);
					}
					else
					{
						comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)caracterizacaoTipo, DbType.Int32);
					}

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Dependencia dependencia = null;

						while (reader.Read())
						{
							dependencia = new Dependencia();

							dependencia.DependenciaTipo = Convert.ToInt32(reader["dependente_tipo"]);
							dependencia.DependenciaCaracterizacao = Convert.ToInt32(reader["dependente_caracterizacao"]);

							lista.Add(dependencia);
						}

						reader.Close();
					}
				}
				#endregion
			}

			return lista;
		}

		internal List<CoordenadaAtividade> ObterCoordenadas(int empreendimento, eCaracterizacao caracterizacaoTipo, eTipoGeometria tipoGeometria, BancoDeDados banco = null)
		{
			List<CoordenadaAtividade> lstCoordenadas = new List<CoordenadaAtividade>();

			string pref = "p";
			switch (tipoGeometria)
			{
				case eTipoGeometria.Ponto:
					pref = "p";
					break;
				case eTipoGeometria.Linha:
					pref = "l";
					break;
				case eTipoGeometria.Poligono:
					pref = "a";
					break;
				default:
					return lstCoordenadas;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id, ord.column_value from {1}geo_" + pref + @"ativ a, table(a.geometry.SDO_ORDINATES) ord, {0}crt_projeto_geo g where a.projeto = g.id and g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)caracterizacaoTipo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					CoordenadaAtividade coordenada = null;

					int idx = 0;

					while (reader.Read())
					{
						idx++;

						if ((idx % 2) != 0)
						{
							coordenada = new CoordenadaAtividade();
							coordenada.Id = Convert.ToInt32(reader["id"]);
							coordenada.Tipo = (int)tipoGeometria;
							coordenada.CoordX = Convert.ToDecimal(reader["column_value"]);
						}
						else
						{
							coordenada.Tipo = (int)tipoGeometria;
							coordenada.CoordY = Convert.ToDecimal(reader["column_value"]);
							lstCoordenadas.Add(coordenada);
						}
					}

					reader.Close();
				}
			}

			return lstCoordenadas;
		}

		internal List<Lista> ObterTipoDeGeometriaAtividade(int empreendimento, int caracterizacaoTipo, BancoDeDados banco = null)
		{
			List<Lista> lstTipoGeometria = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select 1 tipo, l.texto from {1}geo_pativ atv, {0}lov_crt_geometria_tipo l
															where atv.projeto = (select g.id from {0}crt_projeto_geo g where g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao)
															and l.id = 1 union all
															select 2  tipo, l.texto from {1}geo_lativ atv, {0}lov_crt_geometria_tipo l
															where atv.projeto = (select g.id from {0}crt_projeto_geo g where g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao)
															and l.id = 2 union all
															select 3  tipo, l.texto from {1}geo_aativ atv, {0}lov_crt_geometria_tipo l
															where atv.projeto = (select g.id from {0}crt_projeto_geo g where g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao)
															and l.id = 3", EsquemaBanco, EsquemaBancoGeo);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacaoTipo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstTipoGeometria.Add(new Lista() { Id = reader["tipo"].ToString(), Texto = reader["texto"].ToString(), IsAtivo = true });
					}

					reader.Close();
				}
			}

			return lstTipoGeometria;
		}

		internal List<int> ObterAtividadesCaracterizacao(string caractTabela, int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.atividade from {0}" + caractTabela + " c where c.empreendimento = :empreendimento", EsquemaBanco);//5-Encerrado

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarList<Int32>(comando);
			}
		}

		internal int ObterPessoaID(String cpfCnpj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id from {0}tab_pessoa p where p.cpf = :cpf_cnpj or p.cnpj = :cpf_cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 50, cpfCnpj);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		#endregion

		#region Validações

		public bool EmPosse(int empreendimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select (select(select count(*) from {0}tab_protocolo p where p.empreendimento = :empreendimento
				and p.emposse = :funcionario and not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.id))
				+ (select count(*) from {0}tab_protocolo_associado pa, {0}tab_protocolo p, {0}tab_protocolo a where pa.protocolo = p.id and
				p.emposse = :funcionario and pa.associado = a.id and a.empreendimento = :empreendimento and not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.id)
				) valor from dual) emposse,(select count(*) from {0}tab_protocolo t where t.empreendimento = :empreendimento) existe from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", User.FuncionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						return (Convert.ToBoolean(reader["existe"]) && Convert.ToBoolean(reader["emposse"])) || (!Convert.ToBoolean(reader["existe"]));
					}

					reader.Close();
				}
			}

			return false;
		}

		internal bool ExisteEmpreendimento(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(id) from {0}tab_empreendimento where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool AtividadeLicencaObrigatoria(int atividade)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from tab_atividade a where a.id = :atividade and a.licenca_obrigatoria = 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}