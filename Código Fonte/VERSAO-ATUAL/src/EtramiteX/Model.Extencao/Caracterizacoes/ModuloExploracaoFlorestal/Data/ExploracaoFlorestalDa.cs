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

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_exploracao_florestal c (id, empreendimento, tid, codigo_exploracao, tipo_exploracao, data_cadastro) 
				values ({0}seq_crt_exploracao_florestal.nextval, :empreendimento, :tid, :codigo_exploracao, :tipo_exploracao, :data_cadastro) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo_exploracao", caracterizacao.CodigoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_exploracao", caracterizacao.TipoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("data_cadastro", caracterizacao.DataCadastro.Data, DbType.DateTime);
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

						#region Produtos

						if (item.Produtos != null && item.Produtos.Count > 0)
						{
							foreach (ExploracaoFlorestalProduto itemAux in item.Produtos)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_exp_florestal_produto c (id, exp_florestal_exploracao, produto, quantidade, especie_id, especie_nome, tid)
								values ({0}seq_crt_exp_florestal_produto.nextval, :exp_florestal_exploracao, :produto, :quantidade, :especie_popular_id, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("exp_florestal_exploracao", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("produto", itemAux.ProdutoId, DbType.Int32);
								comando.AdicionarParametroEntrada("quantidade", itemAux.Quantidade, DbType.Decimal);
								comando.AdicionarParametroEntrada("especie_popular_id", itemAux.EspeciePopularId, DbType.Int32);
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
				c.data_cadastro = :data_cadastro where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo_exploracao", caracterizacao.CodigoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_exploracao", caracterizacao.TipoExploracao, DbType.Int32);
				comando.AdicionarParametroEntrada("data_cadastro", caracterizacao.DataCadastro.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_exp_florestal_produto c where c.exp_florestal_exploracao in 
				(select a.id from {0}crt_exp_florestal_exploracao a where a.exploracao_florestal = :exploracao_florestal ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.id", DbType.Int32, caracterizacao.Exploracoes.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("exploracao_florestal", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

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
									c.especie_id = :especie_id, c.especie_nome = :especie_nome, c.tid = :tid where c.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.ProdutoId, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_exp_florestal_produto c (id, exp_florestal_exploracao, produto, quantidade, especie_id, especie_nome, tid)
									values ({0}seq_crt_exp_florestal_produto.nextval, :exp_florestal_exploracao, :produto, :quantidade, :especie_popular_id, :tid)", EsquemaBanco);

									comando.AdicionarParametroEntrada("exp_florestal_exploracao", item.Id, DbType.Int32);
								}

								comando.AdicionarParametroEntrada("produto", itemAux.ProdutoId, DbType.Int32);
								comando.AdicionarParametroEntrada("quantidade", itemAux.Quantidade, DbType.Decimal);
								comando.AdicionarParametroEntrada("especie_popular_id", itemAux.EspeciePopularId, DbType.Int32);
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

		#endregion

		#region Obter / Filtrar

		internal ExploracaoFlorestal ObterPorEmpreendimento(int empreendimento, bool simplificado = false, int tipoExploracao = 0, BancoDeDados banco = null)
		{
			ExploracaoFlorestal caracterizacao = new ExploracaoFlorestal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_exploracao_florestal s
						where s.empreendimento = :empreendimento " + (tipoExploracao > 0 ? " and s.tipo_exploracao = :tipo_exploracao" : "") +
						" and rownum = 1", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				if(tipoExploracao > 0)
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

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.codigo_exploracao, c.tipo_exploracao, c.data_cadastro, c.tid, lv.texto tipo_exploracao_texto from {0}crt_exploracao_florestal c
						left join idafgeo.lov_tipo_exploracao lv on (c.tipo_exploracao = lv.tipo_atividade)
						where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						if(!Convert.IsDBNull(reader["codigo_exploracao"]))
							caracterizacao.CodigoExploracao = Convert.ToInt32(reader["codigo_exploracao"]);
						if(!Convert.IsDBNull(reader["tipo_exploracao"]))
							caracterizacao.TipoExploracao = Convert.ToInt32(reader["tipo_exploracao"]);
						if(!Convert.IsDBNull(reader["data_cadastro"]))
							caracterizacao.DataCadastro = new DateTecno() { Data = Convert.ToDateTime(reader["data_cadastro"]) };
						if (!Convert.IsDBNull(reader["tipo_exploracao_texto"]))
							caracterizacao.CodigoExploracaoTexto = reader["tipo_exploracao_texto"].ToString().Substring(0, 3) + caracterizacao.CodigoExploracao.ToString().PadLeft(3, '0');
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
						if(!Convert.IsDBNull(reader["parecer_favoravel"]))
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
						c.especie_popular_id, concat(concat(e.nome_cientifico, '/'), ep.nome_popular) especie_popular_texto, c.tid 
						from {0}crt_exp_florestal_produto c, {0}lov_crt_produto lp, {0}tab_especie_popular ep, {0}tab_especie e
						where c.produto = lp.id and c.especie_popular_id = ep.id and ep.especie = e.id and c.exp_florestal_exploracao = :exploracao", EsquemaBanco);

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

						comando = bancoDeDados.CriarComando(@"select c.exp_florestal_produto_id, c.produto_id, c.produto_texto, c.quantidade, c.especie_popular_id, c.especie_popular_texto, c.tid 
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
									produto.ProdutoTexto = readerAux["especie_popular_texto"].ToString();
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
								when tab.avn = '[x]' then 5 /*Arvores isoladas->Ponto e Linha*/
						   end) class_vegetal
					  from (select a.atividade,
								   a.codigo             identificacao,
								   3					 geometria_tipo,
								   a.area_m2            area_croqui,
								   a.avn,
								   a.aa,
								   lv.tipo_atividade tipo_exploracao,
								   lv.chave tipo_exploracao_texto,
								   (select max(c.codigo_exploracao)+1 from crt_exploracao_florestal c
								   where c.tipo_exploracao = lv.tipo_atividade) codigo_exploracao,
								   a.data
							  from {1}geo_aativ       a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc,
								   {1}lov_tipo_exploracao lv
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							   and lv.chave (+)= a.tipo_exploracao
							union all
							select a.atividade,
								   a.codigo             identificacao,
								   2 geometria_tipo,
								   null                 area_croqui,
								   '[x]' avn,
								   a.aa,
								   lv.tipo_atividade tipo_exploracao,
								   lv.chave tipo_exploracao_texto,
								   (select max(c.codigo_exploracao)+1 from crt_exploracao_florestal c
								   where c.tipo_exploracao = lv.tipo_atividade) codigo_exploracao,
								   a.data
							  from {1}geo_lativ       a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc,
								   {1}lov_tipo_exploracao lv
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							   and lv.chave (+)= a.tipo_exploracao
							union all
							select a.atividade,
								   a.codigo             identificacao,
								   1 geometria_tipo,
								   null                 area_croqui,
								   '[x]' avn,
								   a.aa,
								   lv.tipo_atividade tipo_exploracao,
								   lv.chave tipo_exploracao_texto,
								   (select max(c.codigo_exploracao)+1 from crt_exploracao_florestal c
								   where c.tipo_exploracao = lv.tipo_atividade) codigo_exploracao,
								   a.data
							  from {1}geo_pativ       a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc,
								   {1}lov_tipo_exploracao lv
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							   and lv.chave (+)= a.tipo_exploracao
							union all
							select a.atividade,
								   a.codigo             identificacao,
								   3 geometria_tipo,
								   a.area_m2            area_croqui,
								   a.avn,
								   a.aa,
								   lv.tipo_atividade tipo_exploracao,
								   lv.chave tipo_exploracao_texto,
								   (select max(c.codigo_exploracao)+1 from crt_exploracao_florestal c
								   where c.tipo_exploracao = lv.tipo_atividade) codigo_exploracao,
								   a.data
							  from {1}geo_aiativ      a,
								   {0}crt_projeto_geo         g,
								   {0}lov_caracterizacao_tipo lc,
								   {1}lov_tipo_exploracao lv
							 where a.atividade = lc.texto
							   and a.projeto = g.id
							   and lc.id = :caracterizacao
							   and g.empreendimento = :empreendimento
							   and g.caracterizacao = :caracterizacao
							   and lv.chave (+)= a.tipo_exploracao) tab
							order by tab.tipo_exploracao, tab.identificacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.ExploracaoFlorestal, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ExploracaoFlorestal exploracao = new ExploracaoFlorestal();
					ExploracaoFlorestal exploracaoAnterior = new ExploracaoFlorestal();
					ExploracaoFlorestalExploracao detalhe = null;
					while (reader.Read())
					{
						detalhe = new ExploracaoFlorestalExploracao();
						detalhe.Identificacao = reader["identificacao"].ToString();
						detalhe.GeometriaTipoId = Convert.ToInt32(reader["geometria_tipo"]);

						detalhe.ClassificacaoVegetacaoId = reader.GetValue<int>("class_vegetal");

						detalhe.AreaCroqui = reader.GetValue<decimal>("area_croqui");

						detalhe.GeometriaTipoTexto = _caracterizacaoConfig.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyCaracterizacaoGeometriaTipo).
									SingleOrDefault(x => x.Id == (detalhe.GeometriaTipoId).ToString()).Texto;

						if (exploracaoAnterior.TipoExploracao != Convert.ToInt32(reader["tipo_exploracao"]))
						{
							exploracao = new ExploracaoFlorestal();
							exploracao.EmpreendimentoId = empreendimento;
							if (!Convert.IsDBNull(reader["codigo_exploracao"]))
								exploracao.CodigoExploracao = Convert.ToInt32(reader["codigo_exploracao"]);
							else
								exploracao.CodigoExploracao = 1;
							if (!Convert.IsDBNull(reader["tipo_exploracao"]))
								exploracao.TipoExploracao = Convert.ToInt32(reader["tipo_exploracao"]);
							if (!Convert.IsDBNull(reader["tipo_exploracao_texto"]))
								exploracao.CodigoExploracaoTexto = reader["tipo_exploracao_texto"].ToString().Substring(0, 3) + exploracao.CodigoExploracao.ToString().PadLeft(3, '0');

							if (!Convert.IsDBNull(reader["data"]))
								exploracao.DataCadastro = new DateTecno() { Data = Convert.ToDateTime(reader["data"]) };
						}
						if (exploracaoFlorestalList.Contains(exploracao))
							exploracaoFlorestalList.Remove(exploracao);
						exploracao.Exploracoes.Add(detalhe);
						exploracaoFlorestalList.Add(exploracao);
						exploracaoAnterior = exploracao;
					}

					reader.Close();
				}

				#endregion
			}

			return exploracaoFlorestalList;
		}

		internal int ObterCodigoExploracao(int tipoExploracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
								select max(c.codigo_exploracao) from crt_exploracao_florestal c
							   where c.tipo_exploracao = :tipo_exploracao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("tipo_exploracao", tipoExploracao, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando);
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

				if (filtros.Dados.TipoExploracao != null)
					comandtxt += comando.FiltroAndLike("lv.chave", "tipo_exploracao_texto", filtros.Dados.TipoExploracao, true);

				if (filtros.Dados.CodigoExploracao != null)
					comandtxt += comando.FiltroAnd("c.codigo_exploracao", "codigo_exploracao", filtros.Dados.CodigoExploracao);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.DataExploracao))
					comandtxt += comando.FiltroAnd("to_char(c.data_cadastro, 'dd/MM/yyyy')", "data_cadastro", filtros.Dados.DataExploracao);

				#endregion

				#region Ordenação
				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "tipo_exploracao_texto", "codigo_exploracao", "data_cadastro" };

				if (filtros.OdenarPor > 0)
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				else
					ordenar.Add("tipo_exploracao_texto");
				#endregion Ordenação

				comando.DbCommand.CommandText = String.Format(@"select count(*) from (select c.id, c.empreendimento, c.codigo_exploracao, c.tipo_exploracao, c.data_cadastro, c.tid, lv.chave tipo_exploracao_texto,
						concat(concat(concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')), '-'), to_char(c.data_cadastro, 'ddMMyyyy')) localizador,
						concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')) codigo_exploracao_texto
						from crt_exploracao_florestal c
						left join idafgeo.lov_tipo_exploracao lv on (c.tipo_exploracao = lv.tipo_atividade) where 1=1 " + comandtxt + ") consulta", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				lista.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				#region QUERY

				comandtxt = String.Format(@"select c.id, c.empreendimento, c.codigo_exploracao, c.tipo_exploracao, c.data_cadastro, c.tid, lv.chave tipo_exploracao_texto,
						concat(concat(concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')), '-'), to_char(c.data_cadastro, 'ddMMyyyy')) localizador,
						concat(lv.chave, lpad(to_char(c.codigo_exploracao), 3, '0')) codigo_exploracao_texto
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
							Localizador = reader.GetValue<string>("localizador")							
						};

						if (!Convert.IsDBNull(reader["codigo_exploracao"]))
							caracterizacao.CodigoExploracao = Convert.ToInt32(reader["codigo_exploracao"]);
						if (!Convert.IsDBNull(reader["tipo_exploracao"]))
							caracterizacao.TipoExploracao = Convert.ToInt32(reader["tipo_exploracao"]);
						if (!Convert.IsDBNull(reader["data_cadastro"]))
							caracterizacao.DataCadastro = new DateTecno() { Data = Convert.ToDateTime(reader["data_cadastro"]) };

						lista.Itens.Add(caracterizacao);
					}

					reader.Close();
				}
			}

			return lista;
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