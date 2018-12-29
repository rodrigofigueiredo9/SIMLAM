using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using System.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data
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

		private string EsquemaBancoCredenciado
		{
			get {
				return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
			}
		}

		private string EsquemaBancoCredenciadoGeo
		{
			get {
				return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciadoGeo);
			}
		}

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

						Historico.Gerar(item.Id, eHistoricoArtefatoCaracterizacao.dependencia, eHistoricoAcao.atualizar, bancoDeDados, executor: null);
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void ExcluirTemporario(List<ProjetoDigital> projetos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete tmp_projeto_digital a ");

				comando.DbCommand.CommandText += comando.AdicionarIn("where", "a.id", DbType.Int32, projetos.Select(x => x.Id).ToList());

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void CancelarEnvio(ProjetoDigital projeto, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.situacao = :situacao, r.etapa = :etapa, 
				r.data_envio = null, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", projeto.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", projeto.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Tecnomapas.Blocos.Etx.ModuloCore.Data.Historico historico = new Tecnomapas.Blocos.Etx.ModuloCore.Data.Historico();
				historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.cancelarenvio, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Editar(ProjetoDigital projeto, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_projeto_digital r set r.etapa = :etapa, r.situacao = :situacao, 
				r.data_envio = null, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", projeto.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", projeto.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Tecnomapas.Blocos.Etx.ModuloCore.Data.Historico historico = new Tecnomapas.Blocos.Etx.ModuloCore.Data.Historico();
				historico.Gerar(projeto.Id, eHistoricoArtefato.projetodigital, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void DesassociarDependencias(List<ProjetoDigital> projetos, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete from {0}tab_proj_digital_dependencias d ", EsquemaBancoCredenciado);

				comando.DbCommand.CommandText += comando.AdicionarIn("where", "d.projeto_digital_id", DbType.Int32, projetos.Select(x => x.Id).ToList());

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public EmpreendimentoCaracterizacao ObterEmpreendimentoSimplificado(int id, BancoDeDados banco = null)
		{
			EmpreendimentoCaracterizacao empreendimento = new EmpreendimentoCaracterizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"
				select e.id, e.codigo, e.interno, ls.texto segmento_texto, ls.denominador, e.cnpj, e.denominador denominador_nome,
				e.tid, ee.zona, ee.municipio  municipio_id, (select m.texto from {0}lov_municipio m where m.id = ee.municipio) municipio,
				(select m.ibge from {0}lov_municipio m where m.id = ee.municipio) municipio_ibge,
				cm.id modulo_id, cm.modulo_ha, (select es.sigla from {0}lov_estado es where es.id = ee.estado) estado,
				case when ee.zona = 1 then 'Urbana' else 'Rural' end zona_texto, e.interno_tid from {0}tab_empreendimento e,
				{0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento  ls, {0}tab_empreendimento_endereco ee,
				{0}cnf_municipio_mod_fiscal cm where e.atividade = a.id(+) and e.segmento = ls.id and ee.correspondencia = 0
				and ee.empreendimento = e.id and ee.municipio = cm.municipio(+) and e.id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = id;
						empreendimento.Codigo = reader.GetValue<int?>("codigo");
						empreendimento.InternoID = reader.GetValue<int>("interno");
						empreendimento.Tid = reader.GetValue<string>("tid");
						empreendimento.DenominadorTipo = reader.GetValue<string>("denominador");
						empreendimento.Denominador = reader.GetValue<string>("denominador_nome");
						empreendimento.Municipio = reader.GetValue<string>("municipio");
						empreendimento.MunicipioId = reader.GetValue<int>("municipio_id");
						empreendimento.MunicipioIBGE = reader.GetValue<int>("municipio_ibge");
						empreendimento.ModuloFiscalId = reader.GetValue<int>("modulo_id");
						empreendimento.ModuloFiscalHA = reader.GetValue<decimal>("modulo_ha");
						empreendimento.Uf = reader.GetValue<string>("estado");
						empreendimento.ZonaLocalizacao = (eZonaLocalizacao)reader.GetValue<int>("zona");
						empreendimento.ZonaLocalizacaoTexto = reader.GetValue<string>("zona_texto");
						empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						empreendimento.InternoTID = reader.GetValue<string>("interno_tid");
					}

					reader.Close();
				}

				#endregion
			}

			return empreendimento;
		}

		public List<Caracterizacao> ObterCaracterizacoes(int empreendimentoId, int projetoDigitalId, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Lista das caracterizações

				List<CaracterizacaoLst> caracterizacoesPermitidas = ObterCaracterizacoesPorProjetoDigital(projetoDigitalId, banco: banco)
					.Where(x => x.IsExibirCredenciado || x.Id == (int)eCaracterizacao.UnidadeProducao || x.Id == (int)eCaracterizacao.UnidadeConsolidacao || x.Id == (int)eCaracterizacao.BarragemDispensaLicenca).ToList();
				if (caracterizacoesPermitidas.Count <= 0)
				{
					return null;
				}

				Comando comando = bancoDeDados.CriarComando(@"select 'select ' || t.id || ' tipo, a.id caracterizacao_id, a.tid caracterizacao_tid from {0}' ||
				t.tabela || ' a where a.empreendimento =:empreendimento and rownum = 1 union all ' campo from {0}lov_caracterizacao_tipo t ", EsquemaBancoCredenciado);

				comando.DbCommand.CommandText += comando.AdicionarIn("where", "t.id", DbType.Int32, caracterizacoesPermitidas.Select(x => x.Id).ToList());

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
					comando = bancoDeDados.CriarComando(@"
						select lc.id                tipo,
							   lc.texto             tipo_texto,
							   pg_rascunho.id       projeto_rascunho_id,
							   c.caracterizacao_id,
							   c.caracterizacao_tid,
							   pg.id                projeto_id,
							   pg.tid               projeto_tid
						  from {0}lov_caracterizacao_tipo lc,
							   (" + select.Substring(0, select.Length - 10) + @") c,
							   (select p.id, p.tid, p.empreendimento, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento) pg,
							   (select p.id, p.tid, p.empreendimento, p.caracterizacao from {0}tmp_projeto_geo p where p.empreendimento = :empreendimento) pg_rascunho
						 where lc.id = pg.caracterizacao(+)
						   and lc.id = pg_rascunho.caracterizacao(+)
						   and lc.id = c.tipo(+) ", EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "lc.id", DbType.Int32, caracterizacoesPermitidas.Select(x => x.Id).ToList());
					comando.DbCommand.CommandText += "order by lc.id";

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
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
				 where t.id = c.dependencia", EsquemaBancoCredenciado);

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Lista das caracterizações do empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select d.dependencia_tipo, d.dependencia_caracterizacao, d.dependencia_id, d.dependencia_tid 
				from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_caracterizacao = :dependente_caracterizacao and d.dependente_id = :dependente_id", EsquemaBancoCredenciado);

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

		internal List<Dependencia> ObterDependenciasHistorico(int dependenteId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, string tid, BancoDeDados banco)
		{
			List<Dependencia> lista = new List<Dependencia>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Lista das caracterizações do empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select h.dependencia_tipo, h.dependencia_caracterizacao, h.dependencia_id, h.dependencia_tid from {0}hst_crt_dependencia h where 
                h.dependente_caracterizacao = :dependente_caracterizacao and h.dependente_tipo = :dependente_tipo and h.dependente_id = :dependente_id and h.tid = :tid ", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("dependente_tipo", (int)tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)caracterizacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_id", dependenteId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento", EsquemaBancoCredenciado);
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

		internal List<ProjetoGeografico> ObterProjetosPorProjetoDigital(int projetoId, int empreendimentoId, BancoDeDados banco = null)
		{
			List<ProjetoGeografico> projetos = new List<ProjetoGeografico>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("empreendimento", projetoId, DbType.Int32);

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
				parametro + @") t group by dependente_tipo, dependente_caracterizacao' retorno from lov_caracterizacao_tipo lc where lc.id = :caracterizacao", EsquemaBancoCredenciado);

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
				Comando comando = bancoDeDados.CriarComando(@"select a.id, ord.column_value from {1}geo_" + pref + @"ativ a, table(a.geometry.SDO_ORDINATES) ord, {0}crt_projeto_geo g where a.projeto = g.id and g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

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
															and l.id = 3", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);
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
				Comando comando = bancoDeDados.CriarComando(@"select c.atividade from {0}" + caractTabela + " c where c.empreendimento = :empreendimento", EsquemaBancoCredenciado);//5-Encerrado

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarList<Int32>(comando);
			}
		}

		internal List<CaracterizacaoLst> ObterCaracterizacoesPorProjetoDigital(int projetoDigitalID, BancoDeDados banco = null)
		{
			List<CaracterizacaoLst> caracterizacoes = new List<CaracterizacaoLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select distinct c.id, c.texto, c.tabela, cna.permissao, 
				(select sum(cc.exibir_credenciado) from  cnf_caracterizacao cc where cc.caracterizacao = c.id) exibir_credenciado
				from lov_caracterizacao_tipo c, cnf_atividade_crt_ator cna
				 where c.id = cna.caracterizacao
				   and cna.atividade in (select ra.atividade
						  from tab_requerimento_atividade ra, tab_atividade ta
						 where ra.requerimento =
							   (select pd.requerimento
								  from tab_projeto_digital pd
								 where pd.id = :projeto_digital)
						   and ta.exibir_credenciado = 1
						   and ta.id = ra.atividade)
				   and cna.ator_tipo = :autor_tipo ", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto_digital", projetoDigitalID, DbType.Int32);
				comando.AdicionarParametroEntrada("autor_tipo", User.FuncionarioTipo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacoes.Add(new CaracterizacaoLst()
						{
							Id = Convert.ToInt32(reader.GetValue<String>("id")),
							Texto = reader.GetValue<String>("texto"),
							Tabela = reader.GetValue<String>("tabela"),
							Permissao = (ePermissaoTipo)reader.GetValue<Int32>("permissao"),
							IsExibirCredenciado = reader.GetValue<Boolean>("exibir_credenciado")
						});
					}
				}
			}

			if (caracterizacoes.Count > 0)
			{
				caracterizacoes = ObterCaracterizacoesDependentes(caracterizacoes, projetoDigitalID, banco);
			}

			return caracterizacoes;
		}

		internal List<CaracterizacaoLst> ObterCaracterizacoesDependentes(List<CaracterizacaoLst> caracterizacoes, int projetoDigitalID, BancoDeDados banco = null)
		{
			List<CaracterizacaoLst> retorno = new List<CaracterizacaoLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct lc.id, lc.texto, lc.tabela, c.caracterizacao, (select sum(cc.exibir_credenciado) from cnf_caracterizacao cc where cc.caracterizacao = c.dependencia) exibir_credenciado 
				from lov_caracterizacao_tipo lc, cnf_caracterizacao c where lc.id = c.dependencia and c.tipo_detentor = 2 ", EsquemaBancoCredenciado);

				comando.DbCommand.CommandText += comando.AdicionarIn("and", "c.caracterizacao", DbType.Int32, caracterizacoes.Select(x => x.Id).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						bool skip = false;
						int caracterizacaoId = reader.GetValue<int>("id");
						int caracterizacaoPaiId = reader.GetValue<int>("caracterizacao");

						caracterizacoes.ForEach(x =>
						{
							if (x.Id == caracterizacaoId)
							{
								x.IsExibirCredenciado = Convert.ToBoolean(reader.GetValue<Int32>("exibir_credenciado"));
								skip = true;
							}
						});

						if (skip)
						{
							continue;
						}

						CaracterizacaoLst caracterizacao = new CaracterizacaoLst()
						{
							Id = caracterizacaoId,
							Texto = reader.GetValue<String>("texto"),
							Tabela = reader.GetValue<String>("tabela"),
							IsExibirCredenciado = Convert.ToBoolean(reader.GetValue<Int32>("exibir_credenciado")),
							Permissao = caracterizacoes.Single(x => x.Id == caracterizacaoPaiId).Permissao,
							IsAtivo = true
						};

						retorno.Add(caracterizacao);
						caracterizacoes.Add(caracterizacao);
					}
				}

				if (retorno.Count > 0)
				{
					retorno.AddRange(ObterCaracterizacoesDependentes(retorno, projetoDigitalID, bancoDeDados));
				}
			}

			return caracterizacoes;
		}

		internal CaracterizacaoLst ObterCaracterizacaoDependente(int caracterizacaoId, BancoDeDados banco = null)
		{
			CaracterizacaoLst caracterizacao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.texto, c.tabela from lov_caracterizacao_tipo c where c.id = (select cnc.dependencia 
															from cnf_caracterizacao cnc where cnc.tipo = :tipo and cnc.caracterizacao = :caracterizacao 
															and cnc.dependencia = :dependencia)", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{

						caracterizacao = new CaracterizacaoLst()
						{
							Id = Convert.ToInt32(reader.GetValue<String>("id")),
							Texto = reader.GetValue<String>("texto"),
							Tabela = reader.GetValue<String>("tabela"),
							IsAtivo = true
						};
					}
				}
			}

			return caracterizacao;
		}

		internal List<Caracterizacao> ObterCaracterizacoesAssociadasProjetoDigital(int projetoDigitalID, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Lista das caracterizações

				Comando comando = bancoDeDados.CriarComando(@"
				select lc.id tipo, lc.texto tipo_texto, dc.dependencia_id caracterizacao_id, 
				dc.dependencia_tid caracterizacao_tid, dp.dependencia_id projeto_id, dp.dependencia_tid projeto_tid
				from tab_proj_digital_dependencias dc, tab_proj_digital_dependencias dp, lov_caracterizacao_tipo lc 
				where dc.dependencia_caracterizacao = dp.dependencia_caracterizacao(+) and dc.projeto_digital_id = dp.projeto_digital_id(+)
				and dc.dependencia_caracterizacao = lc.id and dp.dependencia_tipo(+) = 1 and dc.dependencia_tipo = 2 
				and dc.projeto_digital_id = :projeto_digital_id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto_digital_id", projetoDigitalID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Caracterizacao caracterizacao = null;

					while (reader.Read())
					{
						caracterizacao = new Caracterizacao();

						caracterizacao.Tipo = (eCaracterizacao)reader.GetValue<int>("tipo");
						caracterizacao.Nome = reader.GetValue<string>("tipo_texto");
						caracterizacao.Id = reader.GetValue<int>("caracterizacao_id");
						caracterizacao.Tid = reader.GetValue<string>("caracterizacao_tid");
						caracterizacao.ProjetoId = reader.GetValue<int>("projeto_id");
						caracterizacao.ProjetoTid = reader.GetValue<string>("projeto_tid");

						lista.Add(caracterizacao);
					}

					reader.Close();
				}

				#endregion
			}

			return lista;
		}

		public List<Caracterizacao> ObterCaracterizacoesAtuais(int empreendimento, List<Caracterizacao> caracterizacoesCadastradas, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Lista das caracterizações

				Comando comando = bancoDeDados.CriarComando(@"select 'select ' || t.id || ' tipo, c.id caracterizacao_id, c.tid caracterizacao_tid from {0}' ||
				t.tabela || ' c where c.empreendimento = :empreendimento union all ' resultado from lov_caracterizacao_tipo t ", EsquemaBancoCredenciado);

				comando.DbCommand.CommandText += comando.AdicionarIn("where", "t.id", DbType.Int32, caracterizacoesCadastradas.Select(x => (int)x.Tipo).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader.GetValue<string>("resultado");
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(@"
					select lc.id tipo, lc.texto tipo_texto, c.caracterizacao_id, c.caracterizacao_tid, pg.id projeto_id, pg.tid projeto_tid
					from {0}lov_caracterizacao_tipo lc, (" + select.Substring(0, select.Length - 10) + @") c,
						(select p.id, p.tid, p.empreendimento, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento) pg
					where lc.id = pg.caracterizacao(+) and lc.id = c.tipo order by lc.id", EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Caracterizacao caracterizacao = null;

						while (reader.Read())
						{
							caracterizacao = new Caracterizacao();

							caracterizacao.Tipo = (eCaracterizacao)reader.GetValue<int>("tipo");
							caracterizacao.Nome = reader.GetValue<string>("tipo_texto");
							caracterizacao.Id = reader.GetValue<int>("caracterizacao_id");
							caracterizacao.Tid = reader.GetValue<string>("caracterizacao_tid");
							caracterizacao.ProjetoId = reader.GetValue<int>("projeto_id");
							caracterizacao.ProjetoTid = reader.GetValue<string>("projeto_tid");

							lista.Add(caracterizacao);
						}

						reader.Close();
					}
				}

				#endregion
			}

			return lista;
		}

		public List<Caracterizacao> ObterCaracterizacoesInternoAtuais(int empreendimento, List<Caracterizacao> caracterizacoesCadastradas, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Lista das caracterizações

				Comando comando = bancoDeDados.CriarComando(@"select 'select ' || t.id || ' tipo, c.interno_id caracterizacao_id, c.interno_tid caracterizacao_tid from {0}' ||
				t.tabela || ' c where c.empreendimento = :empreendimento union all ' resultado from lov_caracterizacao_tipo t ", EsquemaBancoCredenciado);

				comando.DbCommand.CommandText += comando.AdicionarIn("where", "t.id", DbType.Int32, caracterizacoesCadastradas.Select(x => (int)x.Tipo).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader.GetValue<string>("resultado");
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(@"
					select lc.id tipo, lc.texto tipo_texto, c.caracterizacao_id, c.caracterizacao_tid, pg.interno_id projeto_id, pg.interno_tid projeto_tid
					from {0}lov_caracterizacao_tipo lc, (" + select.Substring(0, select.Length - 10) + @") c,
						(select p.interno_id, p.interno_tid, p.empreendimento, p.caracterizacao from {0}crt_projeto_geo p where p.empreendimento = :empreendimento) pg
					where lc.id = pg.caracterizacao(+) and lc.id = c.tipo order by lc.id", EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Caracterizacao caracterizacao = null;

						while (reader.Read())
						{
							caracterizacao = new Caracterizacao();

							caracterizacao.Tipo = (eCaracterizacao)reader.GetValue<int>("tipo");
							caracterizacao.Nome = reader.GetValue<string>("tipo_texto");
							caracterizacao.Id = reader.GetValue<int>("caracterizacao_id");
							caracterizacao.Tid = reader.GetValue<string>("caracterizacao_tid");
							caracterizacao.ProjetoId = reader.GetValue<int>("projeto_id");
							caracterizacao.ProjetoTid = reader.GetValue<string>("projeto_tid");

							lista.Add(caracterizacao);
						}

						reader.Close();
					}
				}

				#endregion
			}

			return lista;
		}

		internal List<int> ObterPossivelCopiar(List<Caracterizacao> cadastradas, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"");
				string query = string.Empty;

				foreach (var item in cadastradas)
				{
					switch (item.Tipo)
					{
						case eCaracterizacao.Dominialidade:
							query += @"
							select :tipo caracterizacao from hst_crt_projeto_geo p where p.projeto_geo_id = :projeto_geo_domi and p.acao_executada not in (36, 44) and p.caracterizacao_id = :tipo 
							and p.data_execucao = (select max(h.data_execucao) from hst_crt_projeto_geo h where h.projeto_geo_id = :projeto_geo_domi) 
							union all 
							select :tipo caracterizacao from hst_crt_dominialidade c where c.dominialidade_id = :dominialidade_id and c.acao_executada not in (37, 45)
							and c.data_execucao = (select max(h.data_execucao) from hst_crt_dominialidade h where h.dominialidade_id = :dominialidade_id)";

							comando.AdicionarParametroEntrada("projeto_geo_domi", item.ProjetoId, DbType.Int32);
							comando.AdicionarParametroEntrada("dominialidade_id", item.Id, DbType.Int32);
							break;

						case eCaracterizacao.UnidadeProducao:
							query += @"
								select :tipo caracterizacao from hst_crt_unidade_producao c where c.unidade_producao_id = :unidade_producao_id and c.acao_executada not in (49, 50)
								and c.data_execucao = (select max(h.data_execucao) from hst_crt_unidade_producao h where h.unidade_producao_id = c.unidade_producao_id)";

							comando.AdicionarParametroEntrada("unidade_producao_id", item.Id, DbType.Int32);

							break;

						case eCaracterizacao.UnidadeConsolidacao:
							query += @"
								select :tipo caracterizacao from hst_crt_unidade_consolidacao u where u.unidade_consolidacao = :unidade_consolidacao and u.acao_executada not in (54, 55)
								and u.data_execucao = (select max(c.data_execucao) from hst_crt_unidade_consolidacao c where c.unidade_consolidacao = :unidade_consolidacao)";

							comando.AdicionarParametroEntrada("unidade_consolidacao", item.Id, DbType.Int32);

							break;
						case eCaracterizacao.BarragemDispensaLicenca:
							query += @"
								select :tipo caracterizacao from hst_crt_barragem_dispe_lic u where u.caracterizacao_id = :barragem_id and u.acao_executada not in (75, 76)
                                and u.data_execucao = (select max(c.data_execucao) from hst_crt_barragem_dispe_lic c where c.caracterizacao_id = :barragem_id)";

							comando.AdicionarParametroEntrada("barragem_id", item.Id, DbType.Int32);

							break;
						default:
							break;
					}

					query += " union all ";

					comando.AdicionarParametroEntrada("tipo", (int)item.Tipo, DbType.Int32);
				}

				comando.DbCommand.CommandText = query.Remove(query.Length - 11);

				return bancoDeDados.ExecutarList<Int32>(comando);
			}
		}

		internal CredenciadoPessoa ObterCredenciado(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			CredenciadoPessoa credenciado = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Credenciado

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.chave, c.usuario, p.id pessoa, p.interno, p.tipo pessoa_tipo, nvl(p.nome, p.razao_social) nome_razao, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.interno, c.situacao, c.tipo, lt.texto tipo_texto, (ts.sigla || ' - ' || ts.nome_local) unidade_sigla_nome ,c.orgao_parc, 
				c.orgao_parc_unid, c.tid, u.login, (case when trunc(sysdate) > trunc(u.senha_data+(prazo.dias)) then 1 else 0 end) senha_vencida, (select valor from tab_pessoa_meio_contato 
				where meio_contato = 5 and pessoa = p.id) email from {0}tab_credenciado c, {0}tab_orgao_parc_conv_sigla_unid ts, {0}tab_usuario u, {0}tab_pessoa p, 
				{0}lov_credenciado_tipo lt, (select to_number(c.valor) dias from {0}cnf_sistema c where c.campo = 'validadesenha') prazo where c.usuario = u.id(+) and 
				c.pessoa = p.id and c.tipo = lt.id and c.orgao_parc_unid = ts.id(+) and c.id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						credenciado = new CredenciadoPessoa();
						credenciado.Id = id;
						credenciado.Tid = reader.GetValue<string>("tid");
						credenciado.Tipo = reader.GetValue<int>("tipo");
						credenciado.TipoTexto = reader.GetValue<string>("tipo_texto");
						credenciado.Situacao = reader.GetValue<int>("situacao");
						credenciado.OrgaoParceiroId = reader.GetValue<int>("orgao_parc");
						credenciado.OrgaoParceiroUnidadeId = reader.GetValue<int>("orgao_parc_unid");
						credenciado.Chave = reader.GetValue<string>("chave");
						credenciado.OrgaoParceiroUnidadeSiglaNome = reader.GetValue<string>("unidade_sigla_nome");

						if (reader["pessoa"] != null && !Convert.IsDBNull(reader["pessoa"]))
						{
							credenciado.Pessoa.Id = Convert.ToInt32(reader["pessoa"]);
							credenciado.Pessoa.Tipo = Convert.ToInt32(reader["pessoa_tipo"]);
							credenciado.Pessoa.MeiosContatos.Add(new Contato { Valor = reader.GetValue<string>("email"), TipoContato = eTipoContato.Email });
							if (credenciado.Pessoa.IsFisica)
							{
								credenciado.Pessoa.Fisica.Nome = reader["nome_razao"].ToString();
								credenciado.Pessoa.Fisica.CPF = reader["cpf_cnpj"].ToString();
							}
							else
							{
								credenciado.Pessoa.Juridica.RazaoSocial = reader["nome_razao"].ToString();
								credenciado.Pessoa.Juridica.CNPJ = reader["cpf_cnpj"].ToString();
							}
						}

						if (reader["interno"] != null && !Convert.IsDBNull(reader["interno"]))
						{
							credenciado.Pessoa.InternoId = Convert.ToInt32(reader["interno"]);
						}

						if (reader["usuario"] != null && !Convert.IsDBNull(reader["usuario"]))
						{
							credenciado.Usuario.Id = Convert.ToInt32(reader["usuario"]);
							credenciado.Usuario.Login = reader["login"].ToString();
							credenciado.AlterarSenha = (reader["senha_vencida"].ToString() == "1");
						}
					}

					reader.Close();
				}

				#endregion
			}

			return credenciado;
		}

		public List<ProjetoDigital> ObterProjetosDigitaisExcluirCaracterizacao(int empreendimento, BancoDeDados banco)
		{
			List<ProjetoDigital> lista = new List<ProjetoDigital>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.situacao, t.etapa, t.requerimento from tab_projeto_digital t 
				where t.situacao in (1, 2, 3, 6) and t.empreendimento = :empreendimento", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ProjetoDigital item = null;

					while (reader.Read())
					{
						item = new ProjetoDigital();

						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.Situacao = reader.GetValue<int>("situacao");
						item.Etapa = reader.GetValue<int>("etapa");
						item.RequerimentoId = reader.GetValue<int>("requerimento");

						lista.Add(item);
					}

					reader.Close();
				}
			}

			return lista;
		}

		#endregion

		#region Validações

		public bool EmPosse(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(e.id) retorno from {0}tab_empreendimento e where e.id = :id and e.credenciado = :credenciado", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ExisteEmpreendimento(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(id) from {0}tab_empreendimento where id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		#region Auxiliares

		internal HabilitarEmissaoCFOCFOC ObterPorCredenciado(int credenciadoId, bool simplificado = false, BancoDeDados banco = null)
		{
			HabilitarEmissaoCFOCFOC retorno = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select h.id from tab_hab_emi_cfo_cfoc h where h.responsavel = :credenciado_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("credenciado_id", credenciadoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = new HabilitarEmissaoCFOCFOC();
						retorno.Id = reader.GetValue<int>("id");

					}

					reader.Close();
				}

				if (retorno != null)
				{
					retorno = ObterHabilitacao(retorno.Id, simplificado);
				}
			}

			return retorno;
		}

		internal HabilitarEmissaoCFOCFOC ObterHabilitacao(int id, bool simplificado = false, string _schemaBanco = null, bool isCredenciado = false)
		{
			HabilitarEmissaoCFOCFOC habilitar = null;
			PragaHabilitarEmissao praga = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Habilitar Emissão

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.responsavel, t.responsavel_arquivo, aa.nome responsavel_arquivo_nome, cc.cpf responsavel_cpf, cc.nome responsavel_nome, 
				pc.registro registro_crea, t.numero_habilitacao, trunc(t.validade_registro) validade_registro, trunc(t.situacao_data) situacao_data, t.situacao, ls.texto situacao_texto, t.motivo,
				lm.texto motivo_texto, t.observacao, t.numero_dua, t.extensao_habilitacao, t.numero_habilitacao_ori, t.uf, t.numero_visto_crea, t.tid from tab_hab_emi_cfo_cfoc t, tab_credenciado tc, 
				cre_pessoa cc, cre_pessoa_profissao pc, lov_hab_emissao_cfo_situacao ls, lov_hab_emissao_cfo_motivo lm, tab_arquivo aa where t.situacao = ls.id and t.motivo = lm.id(+) 
				and t.responsavel_arquivo = aa.id(+) and tc.id = t.responsavel and tc.pessoa = cc.id and cc.id = pc.pessoa(+) and t.id = :id", EsquemaBancoCredenciado);

				if (isCredenciado)
				{
					comando.DbCommand.CommandText = comando.DbCommand.CommandText.Replace("and t.id = :id", "and t.responsavel = :id");
				}

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						habilitar = new HabilitarEmissaoCFOCFOC();
						habilitar.Id = reader.GetValue<Int32>("id");
						habilitar.Tid = reader.GetValue<String>("tid");
						habilitar.Responsavel.Id = reader.GetValue<Int32>("responsavel");
						habilitar.Arquivo.Id = reader.GetValue<Int32>("responsavel_arquivo");
						habilitar.Arquivo.Nome = reader.GetValue<String>("responsavel_arquivo_nome");
						habilitar.Responsavel.Pessoa.NomeRazaoSocial = reader.GetValue<String>("responsavel_nome");
						habilitar.Responsavel.Pessoa.CPFCNPJ = reader.GetValue<String>("responsavel_cpf");
						habilitar.NumeroHabilitacao = reader.GetValue<String>("numero_habilitacao");
						habilitar.ValidadeRegistro = reader.GetValue<DateTime>("validade_registro").ToShortDateString();
						habilitar.SituacaoData = reader.GetValue<DateTime>("situacao_data").ToShortDateString();
						habilitar.Situacao = reader.GetValue<Int32>("situacao");
						habilitar.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						habilitar.Motivo = reader.GetValue<Int32>("motivo");
						habilitar.MotivoTexto = reader.GetValue<String>("motivo_texto");
						habilitar.Observacao = reader.GetValue<String>("observacao");
						habilitar.NumeroDua = reader.GetValue<String>("numero_dua");
						habilitar.ExtensaoHabilitacao = reader.GetValue<Int32>("extensao_habilitacao");
						habilitar.NumeroHabilitacaoOrigem = reader.GetValue<String>("numero_habilitacao_ori");
						habilitar.RegistroCrea = reader.GetValue<String>("registro_crea");
						habilitar.UF = reader.GetValue<Int32>("uf");
						habilitar.NumeroVistoCrea = reader.GetValue<String>("numero_visto_crea");
					}
					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return habilitar;
				}

				#region Pragas

				if (habilitar != null)
				{
					comando = bancoDeDados.CriarComando(@"
					select hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, trunc(hp.data_habilitacao_inicial) data_habilitacao_inicial, 
					trunc(hp.data_habilitacao_final) data_habilitacao_final, hp.tid, stragg(c.texto) cultura 
					from tab_hab_emi_cfo_cfoc_praga hp, tab_praga pa, tab_praga_cultura pc, tab_cultura c 
					where hp.praga = pa.id and hp.praga = pc.praga(+) and pc.cultura = c.id(+) and hp.habilitar_emi_id = :id 
					group by hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, hp.data_habilitacao_inicial, hp.data_habilitacao_final, hp.tid", EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("id", habilitar.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							praga = new PragaHabilitarEmissao();
							praga.Id = reader.GetValue<Int32>("id");
							praga.Praga.Id = reader.GetValue<Int32>("praga");
							praga.Praga.NomeCientifico = reader.GetValue<String>("nome_cientifico");
							praga.Praga.NomeComum = reader.GetValue<String>("nome_comum");
							praga.DataInicialHabilitacao = reader.GetValue<DateTime>("data_habilitacao_inicial").ToShortDateString(); ;
							praga.DataFinalHabilitacao = reader.GetValue<DateTime>("data_habilitacao_final").ToShortDateString(); ;
							praga.Tid = reader.GetValue<String>("tid");
							praga.Cultura = reader.GetValue<String>("cultura");
							habilitar.Pragas.Add(praga);
						}
						reader.Close();
					}
				}

				#endregion
			}

			return habilitar;
		}

		internal List<Cultura> ObterCulturas(int pragaId, BancoDeDados banco = null)
		{
			List<Cultura> retorno = new List<Cultura>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.texto nome from tab_praga p, tab_praga_cultura pc, tab_cultura c where pc.cultura = c.id 
				and pc.praga = p.id and p.id = :id");
				comando.AdicionarParametroEntrada("id", pragaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Cultura item;

					while (reader.Read())
					{
						item = new Cultura();
						item = ObterCultura(reader.GetValue<int>("id"));
						retorno.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal Cultura ObterCultura(int id)
		{
			Cultura cultura = new Cultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Cultura

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_cultura where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						cultura.Id = id;
						cultura.Nome = reader.GetValue<string>("texto");
						cultura.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				#region Cultivar

				comando.DbCommand.CommandText = "select id, cultivar nome, tid from tab_cultura_cultivar where cultura = :id";
				cultura.LstCultivar = bancoDeDados.ObterEntityList<Cultivar>(comando);

				#endregion
			}

			return cultura;
		}

		#endregion Auxiliares
	}
}