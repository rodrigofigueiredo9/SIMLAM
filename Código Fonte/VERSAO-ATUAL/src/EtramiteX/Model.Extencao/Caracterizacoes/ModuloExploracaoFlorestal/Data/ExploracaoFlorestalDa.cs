using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Data
{
	class ExploracaoFlorestalDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		private enum eTabelaRelacionamento
		{
			tmp_pativ = 1,
			tmp_aativ = 2
		}

		internal Historico Historico { get { return _historico; } }
		private String EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public ExploracaoFlorestalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(ExploracaoFlorestal caracterizacao, BancoDeDados banco)
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

		internal int? Criar(ExploracaoFlorestal caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Exploração Florestal

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_exploracao_florestal c (id, empreendimento, tid, codigo_exploracao, tipo_exploracao, data_cadastro, localizador) 
				values ({0}seq_crt_exploracao_florestal.nextval, :empreendimento, :tid, :codigo_exploracao, :tipo_exploracao, :data_cadastro, :localizador) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo_exploracao", caracterizacao.CodigoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_exploracao", caracterizacao.TipoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("data_cadastro", caracterizacao.DataCadastro.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("localizador", caracterizacao.Localizador, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Explorações

				if (caracterizacao.Exploracoes != null && caracterizacao.Exploracoes.Count > 0)
				{
					foreach (ExploracaoFlorestalExploracao item in caracterizacao.Exploracoes)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_exp_florestal_exploracao c (id, exploracao_florestal, identificacao, geometria, area_croqui, 
						classificacao_vegetacao, area_requerida, arvores_requeridas, quantidade_arvores, tid, finalidade, finalidade_outros, parecer_favoravel) values ({0}seq_crt_exp_flores_exploracao.nextval, :exploracao_florestal, :identificacao, 
						:geometria, :area_croqui, :classificacao_vegetacao, :area_requerida, :arvores_requeridas, :quantidade_arvores, :tid, :finalidade, :finalidade_outros, :parecer_favoravel) returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("exploracao_florestal", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("geometria", item.GeometriaTipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("classificacao_vegetacao", item.ClassificacaoVegetacaoId, DbType.Int32);
						comando.AdicionarParametroEntrada("area_requerida", item.AreaRequerida, DbType.Decimal);
						comando.AdicionarParametroEntrada("arvores_requeridas", item.ArvoresRequeridas, DbType.Decimal);
						comando.AdicionarParametroEntrada("quantidade_arvores", item.QuantidadeArvores, DbType.Int32);
						comando.AdicionarParametroEntrada("finalidade", item.FinalidadeExploracao, DbType.Decimal);
						comando.AdicionarParametroEntrada("finalidade_outros", DbType.String, 80, item.FinalidadeEspecificar);
						comando.AdicionarParametroEntrada("parecer_favoravel", item.ParecerFavoravel, DbType.Boolean);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Exploracao Geo

						if (item.ExploracaoFlorestalGeo != null)
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_exp_florestal_geo c (id, exp_florestal_exploracao, geo_pativ_id,
							geo_aativ_id, tmp_pativ_id, tmp_aativ_id)
							values ({0}seq_exp_florestal_geo.nextval, :exp_florestal_exploracao, :geo_pativ_id, :geo_aativ_id,
							:tmp_pativ_id, :tmp_aativ_id)", EsquemaBanco);

							comando.AdicionarParametroEntrada("exp_florestal_exploracao", item.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("geo_pativ_id", item.ExploracaoFlorestalGeo.GeoPativId, DbType.Int32);
							comando.AdicionarParametroEntrada("geo_aativ_id", item.ExploracaoFlorestalGeo.GeoAativId, DbType.Int32);
							comando.AdicionarParametroEntrada("tmp_pativ_id", item.ExploracaoFlorestalGeo.TmpPativId, DbType.Int32);
							comando.AdicionarParametroEntrada("tmp_aativ_id", item.ExploracaoFlorestalGeo.TmpAativId, DbType.Int32);
							bancoDeDados.ExecutarNonQuery(comando);
						}

						#endregion

						#region Produtos

						if (item.Produtos != null && item.Produtos.Count > 0)
						{
							foreach (ExploracaoFlorestalProduto itemAux in item.Produtos)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_exp_florestal_produto c (id, exp_florestal_exploracao, produto, quantidade, especie_popular_id, destinacao_material_id, tid)
								values ({0}seq_crt_exp_florestal_produto.nextval, :exp_florestal_exploracao, :produto, :quantidade, :especie_popular_id, :destinacao_material_id, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("exp_florestal_exploracao", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("produto", itemAux.ProdutoId, DbType.Int32);
								comando.AdicionarParametroEntrada("quantidade", itemAux.Quantidade, DbType.Decimal);
								comando.AdicionarParametroEntrada("especie_popular_id", itemAux.EspeciePopularId, DbType.Int32);
								comando.AdicionarParametroEntrada("destinacao_material_id", itemAux.DestinacaoMaterialId > 0 ? itemAux.DestinacaoMaterialId : null, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.exploracaoflorestal, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(ExploracaoFlorestal caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Exploração Florestal

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_exploracao_florestal c set 
				c.tid = :tid, c.codigo_exploracao = :codigo_exploracao, c.tipo_exploracao = :tipo_exploracao,
				c.data_cadastro = :data_cadastro, c.localizador = :localizador where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo_exploracao", caracterizacao.CodigoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_exploracao", caracterizacao.TipoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("data_cadastro", caracterizacao.DataCadastro.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("localizador", caracterizacao.Localizador, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				foreach (ExploracaoFlorestalExploracao item in caracterizacao.Exploracoes)
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}crt_exp_florestal_produto c 
					where c.exp_florestal_exploracao in (select a.id from {0}crt_exp_florestal_exploracao a where a.exploracao_florestal = :exploracao_florestal and a.id = :exp_florestal_exploracao)", EsquemaBanco);
					comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.Produtos.Select(x => x.Id).ToList()));

					comando.AdicionarParametroEntrada("exp_florestal_exploracao", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("exploracao_florestal", caracterizacao.Id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				//Explorações
				comando = bancoDeDados.CriarComando("delete from {0}crt_exp_florestal_exploracao c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.exploracao_florestal = :exploracao_florestal{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Exploracoes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("exploracao_florestal", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Explorações

				if (caracterizacao.Exploracoes != null && caracterizacao.Exploracoes.Count > 0)
				{
					foreach (ExploracaoFlorestalExploracao item in caracterizacao.Exploracoes)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_exp_florestal_exploracao c set c.identificacao = :identificacao, c.geometria = :geometria, 
							c.area_croqui = :area_croqui, c.classificacao_vegetacao = :classificacao_vegetacao, c.area_requerida = :area_requerida, c.arvores_requeridas = :arvores_requeridas,
							c.quantidade_arvores = :quantidade_arvores, c.tid = :tid, c.finalidade = :finalidade, c.finalidade_outros = :finalidade_outros, c.parecer_favoravel = :parecer_favoravel where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_exp_florestal_exploracao c (id, exploracao_florestal, identificacao, geometria, area_croqui, 
							classificacao_vegetacao, area_requerida, arvores_requeridas, quantidade_arvores, finalidade, finalidade_outros, parecer_favoravel, tid) values ({0}seq_crt_exp_flores_exploracao.nextval, :exploracao_florestal, :identificacao, 
							:geometria, :area_croqui, :classificacao_vegetacao, :area_requerida, :arvores_requeridas,  :quantidade_arvores, :finalidade, :finalidade_outros, :parecer_favoravel, :tid) returning c.id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("exploracao_florestal", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("finalidade", item.FinalidadeExploracao, DbType.Decimal);
						comando.AdicionarParametroEntrada("finalidade_outros", DbType.String, 80, item.FinalidadeEspecificar);
						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("geometria", item.GeometriaTipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("classificacao_vegetacao", item.ClassificacaoVegetacaoId, DbType.Int32);
						comando.AdicionarParametroEntrada("area_requerida", item.AreaRequerida, DbType.Decimal);
						comando.AdicionarParametroEntrada("arvores_requeridas", item.ArvoresRequeridas, DbType.Decimal);
						comando.AdicionarParametroEntrada("quantidade_arvores", item.QuantidadeArvores, DbType.Int32);
						comando.AdicionarParametroEntrada("parecer_favoravel", item.ParecerFavoravel, DbType.Boolean);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#region Produtos

						if (item.Produtos != null && item.Produtos.Count > 0)
						{
							foreach (ExploracaoFlorestalProduto itemAux in item.Produtos)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_exp_florestal_produto c set c.produto = :produto, c.quantidade = :quantidade,
									c.especie_popular_id = :especie_popular_id, c.destinacao_material_id = :destinacao_material_id, c.tid = :tid where c.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_exp_florestal_produto c (id, exp_florestal_exploracao, produto, quantidade, especie_popular_id, destinacao_material_id, tid)
									values ({0}seq_crt_exp_florestal_produto.nextval, :exp_florestal_exploracao, :produto, :quantidade, :especie_popular_id, :destinacao_material_id, :tid)", EsquemaBanco);

									comando.AdicionarParametroEntrada("exp_florestal_exploracao", item.Id, DbType.Int32);
								}

								comando.AdicionarParametroEntrada("produto", itemAux.ProdutoId, DbType.Int32);
								comando.AdicionarParametroEntrada("quantidade", itemAux.Quantidade, DbType.Decimal);
								comando.AdicionarParametroEntrada("especie_popular_id", itemAux.EspeciePopularId, DbType.Int32);
								comando.AdicionarParametroEntrada("destinacao_material_id", itemAux.DestinacaoMaterialId > 0 ? itemAux.DestinacaoMaterialId : null, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.exploracaoflorestal, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int exploracaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_exploracao_florestal c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", exploracaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(exploracaoId, eHistoricoArtefatoCaracterizacao.exploracaoflorestal, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					"delete from {0}crt_exp_florestal_produto r where r.exp_florestal_exploracao in (select d.id from {0}crt_exp_florestal_exploracao d where d.exploracao_florestal = :caracterizacao);" +
					"delete from {0}crt_exp_florestal_geo r where r.exp_florestal_exploracao in (select d.id from {0}crt_exp_florestal_exploracao d where d.exploracao_florestal = :caracterizacao);" +
					"delete from {0}crt_exp_florestal_exploracao b where b.exploracao_florestal = :caracterizacao;" +
					"delete from {0}crt_exploracao_florestal e where e.id = :caracterizacao;" +
				"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", exploracaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.ExploracaoFlorestal, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion
			}
		}

		internal void FinalizarExploracao(int empreendimento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@" begin
				update {0}crt_exploracao_florestal e
					set e.data_conclusao = sysdate
					where e.empreendimento = :empreendimento
					and e.data_conclusao is null
					and exists
					(select 1 from tab_titulo_exp_florestal t
					where t.exploracao_florestal = e.id);
				insert into crt_exp_florestal_geo (id, exp_florestal_exploracao, geo_pativ_id)
					select seq_exp_florestal_geo.nextval, cp.id, g.id from idafgeo.geo_pativ g 
					inner join crt_exp_florestal_exploracao cp
					on(cp.identificacao = g.codigo)
					where cp.parecer_favoravel = 1
					and exists
					(
						select 1 from crt_exploracao_florestal c
						where cp.exploracao_florestal = c.id
						and c.empreendimento = :empreendimento
						and exists
						(
							select 1 from crt_projeto_geo p
							where p.id = g.projeto
							and p.empreendimento = c.empreendimento
						)
						and exists
						(select 1 from tab_titulo_exp_florestal t
						where t.exploracao_florestal = c.id)
					)
					and not exists
					(select 1 from crt_exp_florestal_geo gp 
					where gp.geo_pativ_id = g.id);
				insert into crt_exp_florestal_geo (id, exp_florestal_exploracao, geo_aativ_id)
					select seq_exp_florestal_geo.nextval, cp.id, g.id from idafgeo.geo_aativ g 
					inner join crt_exp_florestal_exploracao cp
					on(cp.identificacao = g.codigo)
					where cp.parecer_favoravel = 1
					and exists
					(
						select 1 from crt_exploracao_florestal c
						where cp.exploracao_florestal = c.id
						and c.empreendimento = :empreendimento
						and exists
						(
							select * from crt_projeto_geo p
							where p.id = g.projeto
							and p.empreendimento = c.empreendimento
						)
						and exists
						(select 1 from tab_titulo_exp_florestal t
						where t.exploracao_florestal = c.id)
					)
					and not exists
					(select 1 from crt_exp_florestal_geo gp 
					where gp.geo_aativ_id = g.id);
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		internal void ReabrirExploracao(int empreendimento, int titulo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@" begin
				update {0}crt_exploracao_florestal e
					set e.data_conclusao = null
					where e.empreendimento = :empreendimento
					and exists
					(select 1 from tab_titulo_exp_florestal t
					where t.exploracao_florestal = e.id
					and t.titulo = :titulo);
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		#endregion

		#region Obter / Filtrar

		internal ExploracaoFlorestal ObterPorEmpreendimento(int empreendimento, bool simplificado = false, int tipoExploracao = 0, BancoDeDados banco = null)
		{
			ExploracaoFlorestal caracterizacao = new ExploracaoFlorestal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_exploracao_florestal s
						where s.empreendimento = :empreendimento and s.data_conclusao is null" +
						(tipoExploracao > 0 ? " and s.tipo_exploracao = :tipo_exploracao" : "") +
						" and rownum = 1", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				if (tipoExploracao > 0)
					comando.AdicionarParametroEntrada("tipo_exploracao", tipoExploracao, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
			}

			return caracterizacao;
		}

		internal IEnumerable<ExploracaoFlorestal> ObterPorEmpreendimentoList(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			var exploracaoFlorestalList = new List<ExploracaoFlorestal>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_exploracao_florestal s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						int valor = Convert.ToInt32(reader["id"]);
						var exploracao = Obter(valor, bancoDeDados, simplificado);
						exploracaoFlorestalList.Add(exploracao);
					}
				}
			}

			return exploracaoFlorestalList;
		}

		internal ExploracaoFlorestal Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			ExploracaoFlorestal caracterizacao = new ExploracaoFlorestal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_exploracao_florestal s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal ExploracaoFlorestal Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			ExploracaoFlorestal caracterizacao = new ExploracaoFlorestal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Exploração Florestal

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.codigo_exploracao, c.tipo_exploracao,
						c.data_cadastro, c.data_conclusao, c.tid, lv.texto tipo_exploracao_texto,
						nvl(c.localizador, concat(concat(concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')), '-'), to_char(c.data_cadastro, 'ddMMyyyy'))) localizador
						from {0}crt_exploracao_florestal c
						left join idafgeo.lov_tipo_exploracao lv on (c.tipo_exploracao = lv.tipo_atividade)
						where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						if (!Convert.IsDBNull(reader["codigo_exploracao"]))
							caracterizacao.CodigoExploracao = Convert.ToInt32(reader["codigo_exploracao"]);
						if (!Convert.IsDBNull(reader["tipo_exploracao"]))
							caracterizacao.TipoExploracao = Convert.ToInt32(reader["tipo_exploracao"]);
						if (!Convert.IsDBNull(reader["data_cadastro"]))
							caracterizacao.DataCadastro = new DateTecno() { Data = Convert.ToDateTime(reader["data_cadastro"]) };
						if (!Convert.IsDBNull(reader["data_conclusao"]))
							caracterizacao.DataConclusao = new DateTecno() { Data = Convert.ToDateTime(reader["data_conclusao"]) };
						else
							caracterizacao.DataConclusao = new DateTecno();
						if (!Convert.IsDBNull(reader["tipo_exploracao_texto"]))
							caracterizacao.CodigoExploracaoTexto = reader["tipo_exploracao_texto"].ToString().Substring(0, 3) + caracterizacao.CodigoExploracao.ToString().PadLeft(3, '0');
						if (reader["localizador"] != null && !Convert.IsDBNull(reader["localizador"]))
							caracterizacao.Localizador = reader["localizador"].ToString();
						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
					return caracterizacao;

				#region Explorações

				comando = bancoDeDados.CriarComando(@"select c.id, c.identificacao, c.geometria, lg.texto geometria_texto, c.area_croqui, c.area_requerida,
				c.arvores_requeridas, c.classificacao_vegetacao, lc.texto classificacao_vegetacao_texto,
				c.quantidade_arvores, c.tid, c.finalidade, c.finalidade_outros, c.parecer_favoravel
				from {0}crt_exp_florestal_exploracao c, {0}lov_crt_geometria_tipo lg, {0}lov_crt_exp_flores_classif lc
				where c.geometria = lg.id and c.classificacao_vegetacao = lc.id and c.exploracao_florestal = :id order by c.identificacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ExploracaoFlorestalExploracao exploracao = null;

					while (reader.Read())
					{
						exploracao = new ExploracaoFlorestalExploracao();
						exploracao.Id = Convert.ToInt32(reader["id"]);
						if (!Convert.IsDBNull(reader["parecer_favoravel"]))
							exploracao.ParecerFavoravel = Convert.ToBoolean(reader["parecer_favoravel"]);
						exploracao.FinalidadeExploracao = Convert.IsDBNull(reader["finalidade"]) ? 0 : Convert.ToInt32(reader["finalidade"]);
						exploracao.FinalidadeEspecificar = reader["finalidade_outros"].ToString();
						exploracao.Tid = reader["tid"].ToString();
						exploracao.Identificacao = reader["identificacao"].ToString();
						exploracao.ArvoresRequeridas = reader["arvores_requeridas"].ToString();
						exploracao.QuantidadeArvores = reader["quantidade_arvores"].ToString();

						exploracao.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						exploracao.AreaRequerida = reader.GetValue<decimal>("area_requerida");


						if (reader["geometria"] != null && !Convert.IsDBNull(reader["geometria"]))
						{
							exploracao.GeometriaTipoId = Convert.ToInt32(reader["geometria"]);
							exploracao.GeometriaTipoTexto = reader["geometria_texto"].ToString();
						}

						if (reader["classificacao_vegetacao"] != null && !Convert.IsDBNull(reader["classificacao_vegetacao"]))
						{
							exploracao.ClassificacaoVegetacaoId = Convert.ToInt32(reader["classificacao_vegetacao"]);
							exploracao.ClassificacaoVegetacaoTexto = reader["classificacao_vegetacao_texto"].ToString();
						}

						#region Produtos

						comando = bancoDeDados.CriarComando(@"select c.id, c.produto, lp.texto produto_texto, c.quantidade,
						c.especie_popular_id, e.id especie_cientifico_id, concat(concat(e.nome_cientifico, '/'), ep.nome_popular) especie_popular_texto,
						c.destinacao_material_id, lv.texto destinacao_material_texto, c.tid 
						from {0}crt_exp_florestal_produto c, {0}lov_crt_produto lp, {0}tab_especie_popular ep, {0}tab_especie e, {0}lov_dest_material_lenhoso lv
						where c.produto = lp.id and c.especie_popular_id = ep.id(+) and ep.especie = e.id(+) and c.destinacao_material_id = lv.id(+)
						and c.exp_florestal_exploracao = :exploracao", EsquemaBanco);

						comando.AdicionarParametroEntrada("exploracao", exploracao.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ExploracaoFlorestalProduto produto = null;

							while (readerAux.Read())
							{
								produto = new ExploracaoFlorestalProduto();
								produto.Id = Convert.ToInt32(readerAux["id"]);
								produto.Tid = readerAux["tid"].ToString();
								produto.Quantidade = readerAux["quantidade"].ToString();

								if (readerAux["produto"] != null && !Convert.IsDBNull(readerAux["produto"]))
								{
									produto.ProdutoId = Convert.ToInt32(readerAux["produto"]);
									produto.ProdutoTexto = readerAux["produto_texto"].ToString();
								}

								if (readerAux["especie_popular_id"] != null && !Convert.IsDBNull(readerAux["especie_popular_id"]))
								{
									produto.EspeciePopularId = Convert.ToInt32(readerAux["especie_popular_id"]);
									produto.EspeciePopularTexto = readerAux["especie_popular_texto"].ToString();
									produto.EspecieCientificoId = Convert.ToInt32(readerAux["especie_cientifico_id"]);
								}

								if (readerAux["destinacao_material_id"] != null && !Convert.IsDBNull(readerAux["destinacao_material_id"]))
								{
									produto.DestinacaoMaterialId = Convert.ToInt32(readerAux["destinacao_material_id"]);
									produto.DestinacaoMaterialTexto = readerAux["destinacao_material_texto"].ToString();
								}

								exploracao.Produtos.Add(produto);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.Exploracoes.Add(exploracao);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		private ExploracaoFlorestal ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			ExploracaoFlorestal caracterizacao = new ExploracaoFlorestal();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Exploração Florestal

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.tid 
				from {0}hst_crt_exploracao_florestal c where c.exploracao_florestal_id = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento_id"]);
						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Explorações

				comando = bancoDeDados.CriarComando(@"select c.id, c.exp_florestal_exploracao_id, c.identificacao, c.geometria_id, c.geometria_texto, c.area_croqui, c.area_requerida, 
				c.arvores_requeridas, c.classificacao_vegetacao_id, c.classificacao_vegetacao_texto, c.quantidade_arvores, c.tid 
				from {0}hst_crt_exp_florest_exploracao c where c.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ExploracaoFlorestalExploracao exploracao = null;

					while (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						exploracao = new ExploracaoFlorestalExploracao();
						exploracao.Id = Convert.ToInt32(reader["exp_florestal_exploracao_id"]);
						exploracao.Tid = reader["tid"].ToString();
						exploracao.Identificacao = reader["identificacao"].ToString();
						exploracao.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						exploracao.AreaRequerida = reader.GetValue<decimal>("area_requerida");
						exploracao.QuantidadeArvores = reader["quantidade_arvores"].ToString();
						exploracao.ArvoresRequeridas = reader["arvores_requeridas"].ToString();

						if (reader["geometria_id"] != null && !Convert.IsDBNull(reader["geometria_id"]))
						{
							exploracao.GeometriaTipoId = Convert.ToInt32(reader["geometria_id"]);
							exploracao.GeometriaTipoTexto = reader["geometria_texto"].ToString();
						}

						if (reader["classificacao_vegetacao_id"] != null && !Convert.IsDBNull(reader["classificacao_vegetacao_id"]))
						{
							exploracao.ClassificacaoVegetacaoId = Convert.ToInt32(reader["classificacao_vegetacao_id"]);
							exploracao.ClassificacaoVegetacaoTexto = reader["classificacao_vegetacao_texto"].ToString();
						}

						#region Produtos

						comando = bancoDeDados.CriarComando(@"select c.exp_florestal_produto_id, c.produto_id, c.produto_texto, c.quantidade, c.especie_popular_id, c.especie_popular_texto, c.destinacao_material_id, c.destinacao_material_texto, c.tid 
						from {0}hst_crt_exp_florestal_produto c where c.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ExploracaoFlorestalProduto produto = null;

							while (readerAux.Read())
							{
								produto = new ExploracaoFlorestalProduto();
								produto.Id = Convert.ToInt32(readerAux["exp_florestal_produto_id"]);
								produto.Tid = readerAux["tid"].ToString();
								produto.Quantidade = readerAux["quantidade"].ToString();

								if (reader["produto_id"] != null && !Convert.IsDBNull(readerAux["produto_id"]))
								{
									produto.ProdutoId = Convert.ToInt32(readerAux["produto_id"]);
									produto.ProdutoTexto = readerAux["produto_texto"].ToString();
								}

								if (readerAux["especie_popular_id"] != null && !Convert.IsDBNull(readerAux["especie_popular_id"]))
								{
									produto.EspeciePopularId = Convert.ToInt32(readerAux["especie_popular_id"]);
									produto.EspeciePopularTexto = readerAux["especie_popular_texto"].ToString();
								}

								if (readerAux["destinacao_material_id"] != null && !Convert.IsDBNull(readerAux["destinacao_material_id"]))
								{
									produto.DestinacaoMaterialId = Convert.ToInt32(readerAux["destinacao_material_id"]);
									produto.DestinacaoMaterialTexto = readerAux["destinacao_material_texto"].ToString();
								}

								exploracao.Produtos.Add(produto);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.Exploracoes.Add(exploracao);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal IEnumerable<ExploracaoFlorestal> ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{
			var exploracaoFlorestalList = new List<ExploracaoFlorestal>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados Geo
				/*Verificar classificação com analista*/
				Comando comando = bancoDeDados.CriarComando(@"
					select tab.*,
						   (case
								when substr(tab.avn,1,1) = 'I' then 2/*Floresta Nativa - Estágio inicial*/
								when substr(tab.avn,1,1) = 'M' then 3/*Floresta Nativa - Estágio médio*/
								when substr(tab.avn,1,1) = 'A' then 4/*Floresta Nativa - Estágio avançado*/
								when substr(tab.avn,1,1) = 'N' and Instr(tab.aa, 'FLORESTA-PLANTADA') > 0	then 1/*aa - Floresta Plantada*/
								when substr(tab.avn,1,1) = 'N' and Instr(tab.aa, 'CULTURAS-PERENES') > 0	then 6/*aa - CULTURAS-PERENES*/
								when substr(tab.avn,1,1) = 'N' and Instr(tab.aa, 'CULTURAS-ANUAIS') > 0		then 9/*aa - CULTURAS-ANUAIS*/
								when substr(tab.avn,1,1) = 'N' and Instr(tab.aa, 'PASTAGEM') > 0			then 7/*aa - PASTAGEM*/								
								when substr(tab.avn,1,1) = 'N' and Instr(tab.aa, 'OUTRO') > 0				then 8/*aa - OUTRO*/
								when tab.avn = '[x]' then 5 /*Arvores isoladas->Linha*/
						   end) class_vegetal
					  from (select a.id, a.atividade,
								   a.codigo             identificacao,
								   3					 geometria_tipo,
								   a.area_m2            area_croqui,
								   a.avn,
								   a.aa,
								   lv.tipo_atividade tipo_exploracao,
								   lv.chave tipo_exploracao_texto,
								   sysdate data, " +
								   (int)eTabelaRelacionamento.tmp_aativ + @" tabela
							  from {1}tmp_aativ       a,
								   {0}tmp_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc,
								   {1}lov_tipo_exploracao lv
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							   and lv.chave (+)= a.tipo_exploracao
								and not exists(select 1 from crt_exp_florestal_geo cp
									where cp.geo_aativ_id = a.id )
							union all
							select a.id, a.atividade,
								   a.codigo             identificacao,
								   1 geometria_tipo,
								   null                 area_croqui,
								   '[x]' avn,
								   a.aa,
								   lv.tipo_atividade tipo_exploracao,
								   lv.chave tipo_exploracao_texto,
								   sysdate data, " +
								   (int)eTabelaRelacionamento.tmp_pativ + @" tabela
							  from {1}tmp_pativ       a,
								   {0}tmp_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc,
								   {1}lov_tipo_exploracao lv
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							   and lv.chave (+)= a.tipo_exploracao
								and not exists(select 1 from crt_exp_florestal_geo cp
									where cp.geo_pativ_id = a.id )

							) tab
							order by tab.tipo_exploracao, tab.geometria_tipo, tab.identificacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.ExploracaoFlorestal, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ExploracaoFlorestal exploracao = new ExploracaoFlorestal();
					ExploracaoFlorestal exploracaoAnterior = new ExploracaoFlorestal();
					ExploracaoFlorestalExploracao detalhe = null;
					ExploracaoFlorestalExploracao detalheAnterior = null;
					while (reader.Read())
					{
						detalhe = new ExploracaoFlorestalExploracao();
						detalhe.Identificacao = reader["identificacao"].ToString();
						detalhe.ClassificacaoVegetacaoId = reader.GetValue<int>("class_vegetal");
						detalhe.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						detalhe.GeometriaTipoId = Convert.ToInt32(reader["geometria_tipo"]);
						detalhe.GeometriaTipoTexto = _caracterizacaoConfig.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyCaracterizacaoGeometriaTipo).
									SingleOrDefault(x => x.Id == (detalhe.GeometriaTipoId).ToString()).Texto;

						if (detalheAnterior == null) detalheAnterior = detalhe;

						if (exploracaoAnterior.TipoExploracao != Convert.ToInt32(reader["tipo_exploracao"]) || detalheAnterior.GeometriaTipoId != detalhe.GeometriaTipoId)
						{
							exploracao = new ExploracaoFlorestal();
							exploracao.EmpreendimentoId = empreendimento;
							if (exploracaoAnterior.CodigoExploracao > 0 && exploracaoAnterior.TipoExploracao == Convert.ToInt32(reader["tipo_exploracao"]))
								exploracao.CodigoExploracao = exploracaoAnterior.CodigoExploracao + 1;
							else
								exploracao.CodigoExploracao = this.ObterCodigoExploracao(Convert.ToInt32(reader["tipo_exploracao"]), empreendimento, bancoDeDados);
							if (!Convert.IsDBNull(reader["tipo_exploracao"]))
								exploracao.TipoExploracao = Convert.ToInt32(reader["tipo_exploracao"]);
							if (!Convert.IsDBNull(reader["tipo_exploracao_texto"]))
								exploracao.CodigoExploracaoTexto = reader["tipo_exploracao_texto"].ToString().Substring(0, 3) + exploracao.CodigoExploracao.ToString().PadLeft(3, '0');
							if (!Convert.IsDBNull(reader["data"]))
								exploracao.DataCadastro = new DateTecno() { Data = Convert.ToDateTime(reader["data"]) };
						}

						detalhe.ExploracaoFlorestalGeo = new ExploracaoFlorestalGeo();						
						switch (Convert.ToInt32(reader["tabela"]))
						{
							case (int)eTabelaRelacionamento.tmp_aativ:
								detalhe.ExploracaoFlorestalGeo.TmpAativId = Convert.ToInt32(reader["id"]);
								break;
							case (int)eTabelaRelacionamento.tmp_pativ:
								detalhe.ExploracaoFlorestalGeo.TmpPativId = Convert.ToInt32(reader["id"]);
								break;
						}

						if (exploracaoFlorestalList.Contains(exploracao))
							exploracaoFlorestalList.Remove(exploracao);
						exploracao.Exploracoes.Add(detalhe);
						exploracaoFlorestalList.Add(exploracao);
						exploracaoAnterior = exploracao;
						detalheAnterior = detalhe;
					}

					reader.Close();
				}

				#endregion
			}

			return exploracaoFlorestalList;
		}

		internal int ObterCodigoExploracao(int tipoExploracao, int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
								select max(c.codigo_exploracao) from {0}crt_exploracao_florestal c
							   where c.tipo_exploracao = :tipo_exploracao and c.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("tipo_exploracao", tipoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				var codigo = bancoDeDados.ExecutarScalar<int>(comando);
				return codigo + 1;
			}
		}

		public Resultados<ExploracaoFlorestal> Filtrar(Filtro<ListarExploracaoFlorestalFiltro> filtros, BancoDeDados banco = null)
		{
			var lista = new Resultados<ExploracaoFlorestal>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if (filtros.Dados.EmpreendimentoId > 0)
					comandtxt += comando.FiltroAnd("c.empreendimento", "empreendimento", filtros.Dados.EmpreendimentoId);

				if (filtros.Dados.TipoExploracao > 0)
					comandtxt += comando.FiltroAnd("c.tipo_exploracao", "tipo_exploracao", filtros.Dados.TipoExploracao);

				if (filtros.Dados.CodigoExploracao != null)
					comandtxt += comando.FiltroAnd("c.codigo_exploracao", "codigo_exploracao", filtros.Dados.CodigoExploracao);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.DataExploracao))
					comandtxt += comando.FiltroAnd("to_char(c.data_cadastro, 'dd/MM/yyyy')", "data_cadastro", filtros.Dados.DataExploracao);

				if (!filtros.Dados.IsVisualizar)
					comandtxt += " and c.data_conclusao is null";

				#endregion

				#region Ordenação
				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "tipo_exploracao_texto", "codigo_exploracao", "data_cadastro" };

				if (filtros.OdenarPor > 0)
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				else
					ordenar.Add("tipo_exploracao_texto");
				#endregion Ordenação

				comando.DbCommand.CommandText = String.Format(@"select count(*) from (select c.id, c.empreendimento, c.codigo_exploracao, c.tipo_exploracao,
						c.data_cadastro, c.data_conclusao, c.tid, lv.chave tipo_exploracao_texto,
						concat(concat(concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')), '-'), to_char(c.data_cadastro, 'ddMMyyyy')) localizador,
						concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')) codigo_exploracao_texto
						from crt_exploracao_florestal c
						left join idafgeo.lov_tipo_exploracao lv on (c.tipo_exploracao = lv.tipo_atividade) where 1=1 " + comandtxt + ") consulta", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				lista.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				#region QUERY

				comandtxt = String.Format(@"select c.id, c.empreendimento, c.codigo_exploracao, c.tipo_exploracao, c.data_cadastro, c.data_conclusao,
						c.tid, lv.chave tipo_exploracao_texto,
						concat(concat(concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')), '-'), to_char(c.data_cadastro, 'ddMMyyyy')) localizador,
						concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')) codigo_exploracao_texto,
						(select lg.texto from lov_crt_geometria_tipo lg
							where lg.id = (select cp.geometria from crt_exp_florestal_exploracao cp
							where cp.exploracao_florestal = c.id
							and rownum = 1)) geometria_texto,
						(select sum(case when cp.area_requerida > 0 then cp.area_requerida else cp.arvores_requeridas end) from crt_exp_florestal_exploracao cp
							where cp.exploracao_florestal = c.id) quantidade
						from crt_exploracao_florestal c
						left join idafgeo.lov_tipo_exploracao lv on (c.tipo_exploracao = lv.tipo_atividade) where 1=1 " + comandtxt +
						Blocos.Etx.ModuloCore.Data.DaHelper.Ordenar(colunas, ordenar, filtros.OdenarPor == 0), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var caracterizacao = new ExploracaoFlorestal
						{
							Id = reader.GetValue<int>("id"),
							EmpreendimentoId = reader.GetValue<int>("empreendimento"),
							CodigoExploracaoTexto = reader.GetValue<string>("codigo_exploracao_texto"),
							TipoExploracaoTexto = reader.GetValue<string>("tipo_exploracao_texto"),
							Localizador = reader.GetValue<string>("localizador"),
							GeometriaPredominanteTexto = reader.GetValue<string>("geometria_texto"),
							Quantidade = (reader.GetValue<decimal>("quantidade")).ToString("N2")
						};

						if (!Convert.IsDBNull(reader["codigo_exploracao"]))
							caracterizacao.CodigoExploracao = Convert.ToInt32(reader["codigo_exploracao"]);
						if (!Convert.IsDBNull(reader["tipo_exploracao"]))
							caracterizacao.TipoExploracao = Convert.ToInt32(reader["tipo_exploracao"]);
						if (!Convert.IsDBNull(reader["data_cadastro"]))
							caracterizacao.DataCadastro = new DateTecno() { Data = Convert.ToDateTime(reader["data_cadastro"]) };
						if (!Convert.IsDBNull(reader["data_conclusao"]))
							caracterizacao.DataConclusao = new DateTecno() { Data = Convert.ToDateTime(reader["data_conclusao"]) };

						lista.Itens.Add(caracterizacao);
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal bool ExisteExploracaoDesfavoravel(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
							select count(*) from crt_exploracao_florestal c where empreendimento = :empreendimento
							and exists
							(select 1 from crt_exp_florestal_exploracao cp
							where cp.exploracao_florestal = c.id
							and cp.parecer_favoravel = :parecer_favoravel 
							and exists
							(select 1 from crt_exp_florestal_geo cg
								where cg.exp_florestal_exploracao = cp.id))", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("parecer_favoravel", false, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool ExisteExploracaoEmAndamento(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
							select count(*) from crt_exploracao_florestal c where empreendimento = :empreendimento
							and c.data_conclusao is null", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				return bancoDeDados.ExecutarScalar<bool>(comando);
			}
		}

		internal bool ExisteExploracaoGeoNaoCadastrada(int projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
								select sum(soma) from 
								(
									select count(*) soma from idafgeo.tmp_pativ g where g.projeto = :projeto
									and not exists (select 1 from crt_exp_florestal_geo eg where eg.tmp_pativ_id = g.id )
									union all 
									select count(*) soma from idafgeo.tmp_aativ g where g.projeto = :projeto
									and not exists (select 1 from crt_exp_florestal_geo eg where eg.tmp_aativ_id = g.id )
								)", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projeto, DbType.Int32);
				var soma = bancoDeDados.ExecutarScalar<int>(comando);
				return (soma > 0);
			}
		}

		internal IEnumerable<ExploracaoFlorestal> ObterExploracoes(int tituloId, int modelo, BancoDeDados banco = null)
		{
			var exploracoes = new List<ExploracaoFlorestal>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Exploração Florestal

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento, c.codigo_exploracao, c.tipo_exploracao,
						c.data_cadastro, c.data_conclusao, c.tid, lv.texto tipo_exploracao_texto, lv.chave chave_tipo_exploracao,
						nvl(c.localizador, concat(concat(concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')), '-'), to_char(c.data_cadastro, 'ddMMyyyy'))) localizador
						from {0}crt_exploracao_florestal c
						left join idafgeo.lov_tipo_exploracao lv on (c.tipo_exploracao = lv.tipo_atividade)
						where exists
						(select 1 from tab_titulo_exp_florestal t
							where t.exploracao_florestal = c.id
							and t.titulo = :titulo_id)
						order by c.tipo_exploracao, c.codigo_exploracao", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo_id", tituloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var exploracao = new ExploracaoFlorestal();
						exploracao.Id = Convert.ToInt32(reader["id"]);
						exploracao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						if (!Convert.IsDBNull(reader["codigo_exploracao"]))
							exploracao.CodigoExploracao = Convert.ToInt32(reader["codigo_exploracao"]);
						if (!Convert.IsDBNull(reader["tipo_exploracao"]))
							exploracao.TipoExploracao = Convert.ToInt32(reader["tipo_exploracao"]);
						if (!Convert.IsDBNull(reader["data_cadastro"]))
							exploracao.DataCadastro = new DateTecno() { Data = Convert.ToDateTime(reader["data_cadastro"]) };
						if (!Convert.IsDBNull(reader["data_conclusao"]))
							exploracao.DataConclusao = new DateTecno() { Data = Convert.ToDateTime(reader["data_conclusao"]) };
						else
							exploracao.DataConclusao = new DateTecno();
						if (!Convert.IsDBNull(reader["chave_tipo_exploracao"]))
							exploracao.CodigoExploracaoTexto = reader["chave_tipo_exploracao"].ToString() + exploracao.CodigoExploracao.ToString().PadLeft(3, '0');
						if (reader["localizador"] != null && !Convert.IsDBNull(reader["localizador"]))
							exploracao.Localizador = reader["localizador"].ToString();
						exploracao.TipoExploracaoTexto = reader["tipo_exploracao_texto"].ToString();
						exploracao.Tid = reader["tid"].ToString();

						#region Explorações

						comando = bancoDeDados.CriarComando(@"select c.id, c.identificacao, c.geometria, lg.texto geometria_texto, c.area_croqui, c.area_requerida,
						c.arvores_requeridas, c.classificacao_vegetacao, lc.texto classificacao_vegetacao_texto,
						c.quantidade_arvores, c.tid, c.finalidade, c.finalidade_outros, c.parecer_favoravel, lf.texto finalidade_texto
						from {0}crt_exp_florestal_exploracao c, {0}lov_crt_geometria_tipo lg, {0}lov_crt_exp_flores_classif lc, {0}lov_crt_exp_flores_finalidade lf
						where c.geometria = lg.id and c.classificacao_vegetacao = lc.id and c.finalidade = lf.id(+)
						and c.exploracao_florestal = :exploracao_florestal " +
						(modelo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal ? @"
						and exists(select 1 from tab_titulo_exp_flor_exp t
							where t.exp_florestal_exploracao = c.id
							and exists (select 1 from tab_titulo_exp_florestal tt
								where tt.titulo = :titulo_id
								and tt.id = t.titulo_exploracao_florestal))" : "") + @"
						order by c.identificacao", EsquemaBanco);

						comando.AdicionarParametroEntrada("exploracao_florestal", exploracao.Id, DbType.Int32);
						if(modelo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal)
							comando.AdicionarParametroEntrada("titulo_id", tituloId, DbType.Int32);

						using (IDataReader readerChild = bancoDeDados.ExecutarReader(comando))
						{
							ExploracaoFlorestalExploracao exploracaoChild = null;

							while (readerChild.Read())
							{
								exploracaoChild = new ExploracaoFlorestalExploracao();
								exploracaoChild.Id = Convert.ToInt32(readerChild["id"]);
								if (!Convert.IsDBNull(readerChild["parecer_favoravel"]))
									exploracaoChild.ParecerFavoravel = Convert.ToBoolean(readerChild["parecer_favoravel"]);
								exploracaoChild.FinalidadeExploracao = Convert.IsDBNull(readerChild["finalidade"]) ? 0 : Convert.ToInt32(readerChild["finalidade"]);
								exploracaoChild.FinalidadeEspecificar = readerChild["finalidade_outros"].ToString();
								exploracaoChild.FinalidadeExploracaoTexto = readerChild["finalidade_texto"].ToString();
								exploracaoChild.Tid = readerChild["tid"].ToString();
								exploracaoChild.Identificacao = readerChild["identificacao"].ToString();
								exploracaoChild.ArvoresRequeridas = readerChild["arvores_requeridas"].ToString();
								exploracaoChild.QuantidadeArvores = readerChild["quantidade_arvores"].ToString();

								exploracaoChild.AreaCroqui = readerChild.GetValue<decimal>("area_croqui");
								exploracaoChild.AreaRequerida = readerChild.GetValue<decimal>("area_requerida");


								if (readerChild["geometria"] != null && !Convert.IsDBNull(readerChild["geometria"]))
								{
									exploracaoChild.GeometriaTipoId = Convert.ToInt32(readerChild["geometria"]);
									exploracaoChild.GeometriaTipoTexto = readerChild["geometria_texto"].ToString();
								}

								if (readerChild["classificacao_vegetacao"] != null && !Convert.IsDBNull(readerChild["classificacao_vegetacao"]))
								{
									exploracaoChild.ClassificacaoVegetacaoId = Convert.ToInt32(readerChild["classificacao_vegetacao"]);
									exploracaoChild.ClassificacaoVegetacaoTexto = readerChild["classificacao_vegetacao_texto"].ToString();
								}

								#region Produtos

								comando = bancoDeDados.CriarComando(@"select c.id, c.produto, lp.texto produto_texto, c.quantidade,
								c.especie_popular_id, e.id especie_cientifico_id, concat(concat(e.nome_cientifico, '/'), ep.nome_popular) especie_popular_texto,
								c.destinacao_material_id, lv.texto destinacao_material_texto, c.tid 
								from {0}crt_exp_florestal_produto c, {0}lov_crt_produto lp, {0}tab_especie_popular ep, {0}tab_especie e, {0}lov_dest_material_lenhoso lv
								where c.produto = lp.id and c.especie_popular_id = ep.id(+) and ep.especie = e.id(+) and c.destinacao_material_id = lv.id(+)
								and c.exp_florestal_exploracao = :exploracao", EsquemaBanco);

								comando.AdicionarParametroEntrada("exploracao", exploracaoChild.Id, DbType.Int32);

								using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
								{
									ExploracaoFlorestalProduto produto = null;

									while (readerAux.Read())
									{
										produto = new ExploracaoFlorestalProduto();
										produto.Id = Convert.ToInt32(readerAux["id"]);
										produto.Tid = readerAux["tid"].ToString();
										produto.Quantidade = readerAux["quantidade"].ToString();

										if (readerAux["produto"] != null && !Convert.IsDBNull(readerAux["produto"]))
										{
											produto.ProdutoId = Convert.ToInt32(readerAux["produto"]);
											produto.ProdutoTexto = readerAux["produto_texto"].ToString();
										}

										if (readerAux["especie_popular_id"] != null && !Convert.IsDBNull(readerAux["especie_popular_id"]))
										{
											produto.EspeciePopularId = Convert.ToInt32(readerAux["especie_popular_id"]);
											produto.EspeciePopularTexto = readerAux["especie_popular_texto"].ToString();
											produto.EspecieCientificoId = Convert.ToInt32(readerAux["especie_cientifico_id"]);
										}

										if (readerAux["destinacao_material_id"] != null && !Convert.IsDBNull(readerAux["destinacao_material_id"]))
										{
											produto.DestinacaoMaterialId = Convert.ToInt32(readerAux["destinacao_material_id"]);
											produto.DestinacaoMaterialTexto = readerAux["destinacao_material_texto"].ToString();
										}

										exploracaoChild.Produtos.Add(produto);
									}

									readerAux.Close();
								}

								exploracao.Exploracoes.Add(exploracaoChild);

								#endregion
							}

							readerChild.Close();
						}

						#endregion

						exploracoes.Add(exploracao);
					}

					reader.Close();
				}

				#endregion
			}

			return exploracoes;
		}

		#endregion

		#region Validações

		public bool EmPosse(int empreendimento)
		{
			return _caracterizacaoDa.EmPosse(empreendimento);
		}

		public bool NaoPossuiAVNOuAA(int empreendimento, BancoDeDados banco = null)
		{
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select 'select 
				(select count(*) from {1}geo_aativ a where a.projeto = '||g.id||' and a.atividade = '''||lc.texto||''' and a.avn = ''N'' and a.aa = ''N'') +
				(select count(*) from {1}geo_pativ a where a.projeto = '||g.id||' and a.atividade = '''||lc.texto||''' and a.avn = ''N'' and a.aa = ''N'') +
				(select count(*) from {1}geo_lativ a where a.projeto = '||g.id||' and a.atividade = '''||lc.texto||''' and a.avn = ''N'' and a.aa = ''N'') 				
				retorno from dual' retorno from {0}crt_projeto_geo g, {0}lov_caracterizacao_tipo lc
				where g.caracterizacao = lc.id and g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.ExploracaoFlorestal, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader["retorno"].ToString();
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(select);
					return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
				}

				return false;
			}
		}

		public bool PossuiAVNNaoCaracterizada(int empreendimento, BancoDeDados banco = null)
		{
			String select = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select 'select 
				(select count(*) from {1}geo_aativ a where a.projeto = '||g.id||' and a.atividade = '''||lc.texto||''' and instr(a.avn, ''Não caracterizado'' ) > 0)+
				(select count(*) from {1}geo_pativ a where a.projeto = '||g.id||' and a.atividade = '''||lc.texto||''' and instr(a.avn, ''Não caracterizado'' ) > 0)+
				(select count(*) from {1}geo_lativ a where a.projeto = '||g.id||' and a.atividade = '''||lc.texto||''' and instr(a.avn, ''Não caracterizado'' ) > 0)
				retorno from dual' retorno from {0}crt_projeto_geo g, {0}lov_caracterizacao_tipo lc
				where g.caracterizacao = lc.id and g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.ExploracaoFlorestal, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						select += reader["retorno"].ToString();
					}

					reader.Close();
				}

				if (!string.IsNullOrEmpty(select))
				{
					comando = bancoDeDados.CriarComando(select);
					return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
				}

				return false;
			}
		}

		#endregion
	}
}