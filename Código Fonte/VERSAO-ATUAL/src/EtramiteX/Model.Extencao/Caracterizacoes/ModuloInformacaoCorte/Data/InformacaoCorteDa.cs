using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data
{
	public class InformacaoCorteDa
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

		public InformacaoCorteDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal int Salvar(InformacaoCorte caracterizacao, BancoDeDados banco)
		{

			if (caracterizacao == null)
			{
				throw new Exception("A Caracterização é nula.");
			}

			if (caracterizacao.Id <= 0)
			{
				int? id = Criar(caracterizacao, banco);
				return id.GetValueOrDefault(0);
			}
			else
			{
				Editar(caracterizacao.InformacaoCorteInformacao, banco);
				return caracterizacao.Id;
			}
		}

		internal int? Criar(InformacaoCorte caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informacao de Corte

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_informacao_corte(id, empreendimento, tid) 
															values({0}seq_crt_informacao_corte.nextval, :empreendimento, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Informacao de Corte Informacao

				comando = bancoDeDados.CriarComando(@"insert into {0}crt_inf_corte_inf(id, caracterizacao, arvores_isoladas_restante, 
													area_corte_restante, data_informacao, tid) values({0}seq_crt_inf_corte_inf.nextval,
													:caracterizacao, :arvores_isoladas_restante, :area_corte_restante,  
													:data_informacao, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arvores_isoladas_restante", caracterizacao.InformacaoCorteInformacao.ArvoresIsoladasRestantes, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_corte_restante", caracterizacao.InformacaoCorteInformacao.AreaCorteRestante, DbType.Decimal);
				comando.AdicionarParametroEntrada("data_informacao", caracterizacao.InformacaoCorteInformacao.DataInformacao.DataTexto, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.InformacaoCorteInformacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Especies

				if (caracterizacao.InformacaoCorteInformacao.Especies != null && caracterizacao.InformacaoCorteInformacao.Especies.Count > 0)
				{
					foreach (Especie especie in caracterizacao.InformacaoCorteInformacao.Especies)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_inf_corte_inf_especie(id, inf_corte_inf, especie, 
															especie_especificar_texto, arvores_isoladas, area_corte, idade_plantio, tid)
															values({0}seq_crt_inf_corte_inf_especie.nextval, :inf_corte_inf, 
															:especie, :especie_especificar_texto, :arvores_isoladas, :area_corte, :idade_plantio, 
															:tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("inf_corte_inf", caracterizacao.InformacaoCorteInformacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("especie", especie.EspecieTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("especie_especificar_texto", String.IsNullOrWhiteSpace(especie.EspecieEspecificarTexto) ? (Object)DBNull.Value: especie.EspecieEspecificarTexto, DbType.String);
						comando.AdicionarParametroEntrada("arvores_isoladas", especie.ArvoresIsoladas, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_corte", especie.AreaCorte, DbType.Decimal);
						comando.AdicionarParametroEntrada("idade_plantio", especie.IdadePlantio, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Produtos

				if (caracterizacao.InformacaoCorteInformacao.Produtos != null && caracterizacao.InformacaoCorteInformacao.Produtos.Count > 0)
				{
					foreach (Produto produto in caracterizacao.InformacaoCorteInformacao.Produtos)
					{
						comando = bancoDeDados.CriarComando(@"insert into crt_inf_corte_inf_produto(id, inf_corte_inf, produto,
															destinacao_material, quantidade, tid) values({0}seq_crt_inf_corte_inf_produto.nextval, 
															:inf_corte_inf, :produto, :destinacao_material, :quantidade, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("inf_corte_inf", caracterizacao.InformacaoCorteInformacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("produto", produto.ProdutoTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("destinacao_material", produto.DestinacaoTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("quantidade", produto.Quantidade, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#endregion

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(InformacaoCorteInformacao informacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informacao de Corte

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_informacao_corte i set i.tid = :tid where i.id = :caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", informacao.CaracterizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				#region Especies

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_inf_corte_inf_especie e
													where e.inf_corte_inf = :inf_corte_inf", EsquemaBanco);

				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "e.id", DbType.Int32, informacao.Especies.Select(x => x.Id).ToList()));

				comando.AdicionarParametroEntrada("inf_corte_inf", informacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Produtos

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_inf_corte_inf_produto p
													where p.inf_corte_inf = :inf_corte_inf", EsquemaBanco);

				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "p.id", DbType.Int32, informacao.Produtos.Select(x => x.Id).ToList()));

				comando.AdicionarParametroEntrada("inf_corte_inf", informacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#endregion

				#region Informacao de Corte Informacao

				if (informacao.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"update {0}crt_inf_corte_inf i set i.caracterizacao = :caracterizacao, 
														i.arvores_isoladas_restante = :arvores_isoladas_restante, 
														i.area_corte_restante = :area_corte_restante, 
														i.data_informacao = :data_informacao, i.tid = :tid 
														where i.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", informacao.Id, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}crt_inf_corte_inf(id, caracterizacao, arvores_isoladas_restante, 
														area_corte_restante, data_informacao, tid) values({0}seq_crt_inf_corte_inf.nextval,
														:caracterizacao, :arvores_isoladas_restante, :area_corte_restante, 
														:data_informacao, :tid) returning id into :id", EsquemaBanco);

					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("caracterizacao", informacao.CaracterizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("arvores_isoladas_restante", informacao.ArvoresIsoladasRestantes, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_corte_restante", informacao.AreaCorteRestante, DbType.Decimal);
				comando.AdicionarParametroEntrada("data_informacao", informacao.DataInformacao.DataTexto, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (informacao.Id <= 0)
				{
					informacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#region Especies

				if (informacao.Especies != null && informacao.Especies.Count > 0)
				{
					foreach (Especie especie in informacao.Especies)
					{
						if (especie.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_inf_corte_inf_especie e set e.especie = :especie, 
																e.especie_especificar_texto = :especie_especificar_texto, 
																e.arvores_isoladas = :arvores_isoladas, e.area_corte = :area_corte, e.idade_plantio = :idade_plantio, 
																e.tid = :tid where e.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", especie.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_inf_corte_inf_especie(id, inf_corte_inf, especie, 
																especie_especificar_texto, arvores_isoladas, area_corte, idade_plantio, tid)
																values({0}seq_crt_inf_corte_inf_especie.nextval, :inf_corte_inf, 
																:especie, :especie_especificar_texto, :arvores_isoladas, :area_corte, :idade_plantio, 
																:tid)", EsquemaBanco);

							comando.AdicionarParametroEntrada("inf_corte_inf", informacao.Id, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("especie", especie.EspecieTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("especie_especificar_texto", String.IsNullOrWhiteSpace(especie.EspecieEspecificarTexto) ? (Object)DBNull.Value : especie.EspecieEspecificarTexto, DbType.String);
						comando.AdicionarParametroEntrada("arvores_isoladas", especie.ArvoresIsoladas, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_corte", especie.AreaCorte, DbType.Decimal);
						comando.AdicionarParametroEntrada("idade_plantio", especie.IdadePlantio, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Produtos

				if (informacao.Produtos != null && informacao.Produtos.Count > 0)
				{
					foreach (Produto produto in informacao.Produtos)
					{
						if (produto.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0} crt_inf_corte_inf_produto p set p.produto = :produto, 
																p.destinacao_material = :destinacao_material, p.quantidade = :quantidade, 
																p.tid = :tid where p.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", produto.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into crt_inf_corte_inf_produto(id, inf_corte_inf, produto,
																destinacao_material, quantidade, tid) values({0}seq_crt_inf_corte_inf_produto.nextval, 
																:inf_corte_inf, :produto, :destinacao_material, :quantidade, :tid)", EsquemaBanco);

							comando.AdicionarParametroEntrada("inf_corte_inf", informacao.Id, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("produto", produto.ProdutoTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("destinacao_material", produto.DestinacaoTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("quantidade", produto.Quantidade, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#endregion

				#region Histórico

				Historico.Gerar(informacao.CaracterizacaoId, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void ExcluirInformacao(int informacaoItemId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.caracterizacao from {0}crt_inf_corte_inf c where c.id = :inf_corte_inf", EsquemaBanco);
				comando.AdicionarParametroEntrada("inf_corte_inf", informacaoItemId, DbType.Int32);

				int id = 0;
				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					id = Convert.ToInt32(retorno);
				}

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}crt_informacao_corte c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
													@"delete from {0}crt_inf_corte_inf_especie e where e.inf_corte_inf = :inf_corte_inf;" +
													@"delete from {0}crt_inf_corte_inf_produto p where p.inf_corte_inf = :inf_corte_inf;" +
													@"delete from {0}crt_inf_corte_inf c where c.id = :inf_corte_inf;" +
													@"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("inf_corte_inf", informacaoItemId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_informacao_corte c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_informacao_corte c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_inf_corte_inf_especie e where e.inf_corte_inf in (select d.id from {0}crt_inf_corte_inf d where d.caracterizacao = :caracterizacao);" +
					@"delete from {0}crt_inf_corte_inf_produto p where p.inf_corte_inf in (select d.id from {0}crt_inf_corte_inf d where d.caracterizacao = :caracterizacao);" +
					@"delete from {0}crt_inf_corte_inf i where i.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_informacao_corte c where c.id = :caracterizacao;" +
					@"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.InformacaoCorte, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal InformacaoCorte ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_informacao_corte s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal InformacaoCorte Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_informacao_corte s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal InformacaoCorte Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informacao de Corte

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.tid 
															from {0}crt_informacao_corte c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Informacoes

				comando = bancoDeDados.CriarComando(@"select i.id, i.arvores_isoladas_restante, i.area_corte_restante, i.data_informacao, i.tid
													from crt_inf_corte_inf i where i.caracterizacao = :caracterizacao order by i.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					InformacaoCorteInformacao informacaoCorte = null;

					while (reader.Read())
					{
						informacaoCorte = new InformacaoCorteInformacao();
						informacaoCorte.Id = Convert.ToInt32(reader["id"]);
						informacaoCorte.CaracterizacaoId = caracterizacao.Id;
						informacaoCorte.Tid = reader["tid"].ToString();

						if (reader["arvores_isoladas_restante"] != null && !Convert.IsDBNull(reader["arvores_isoladas_restante"]))
						{
							informacaoCorte.ArvoresIsoladasRestantes = Convert.ToDecimal(reader["arvores_isoladas_restante"]).ToString("N0");
						}

						if (reader["data_informacao"] != null && !Convert.IsDBNull(reader["data_informacao"]))
						{
							informacaoCorte.DataInformacao.DataTexto = Convert.ToDateTime(reader["data_informacao"]).ToShortDateString();
						}

						if (reader["area_corte_restante"] != null && !Convert.IsDBNull(reader["area_corte_restante"]))
						{
							informacaoCorte.AreaCorteRestante= Convert.ToDecimal(reader["area_corte_restante"]).ToString("N4");
						}

						#region Especies

						comando = bancoDeDados.CriarComando(@"select e.id, e.especie, le.texto especie_texto, e.especie_especificar_texto, 
															e.arvores_isoladas, e.area_corte, e.idade_plantio, e.tid from crt_inf_corte_inf_especie e, 
															lov_crt_silvicultura_cult_fl le where le.id = e.especie 
															and e.inf_corte_inf = :inf_corte_inf order by e.id", EsquemaBanco);

						comando.AdicionarParametroEntrada("inf_corte_inf", informacaoCorte.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Especie especie = null;

							while (readerAux.Read())
							{
								especie = new Especie();
								especie.Id = Convert.ToInt32(readerAux["id"]);
								especie.EspecieTipo = Convert.ToInt32(readerAux["especie"]);
								especie.EspecieTipoTexto = readerAux["especie_texto"].ToString();
								especie.ArvoresIsoladas = readerAux["arvores_isoladas"].ToString();
								especie.Tid = readerAux["tid"].ToString();

								if (readerAux["especie_especificar_texto"] != null && !Convert.IsDBNull(readerAux["especie_especificar_texto"]))
								{
									especie.EspecieEspecificarTexto = readerAux["especie_especificar_texto"].ToString();
									especie.EspecieTipoTexto = especie.EspecieEspecificarTexto;
								}

								if (readerAux["area_corte"] != null && !Convert.IsDBNull(readerAux["area_corte"]))
								{
									especie.AreaCorte = Convert.ToDecimal(readerAux["area_corte"]).ToString("N4");
								}

								if (readerAux["idade_plantio"] != null && !Convert.IsDBNull(readerAux["idade_plantio"]))
								{
									especie.IdadePlantio = Convert.ToDecimal(readerAux["idade_plantio"]).ToString("N0");
								}

								informacaoCorte.Especies.Add(especie);
							}

							readerAux.Close();
						}

						#endregion

						#region Produtos

						comando = bancoDeDados.CriarComando(@"select p.id, p.produto, lp.texto produto_texto, p.destinacao_material, 
															lm.texto destinacao_material_texto, p.quantidade, p.tid 
															from {0}crt_inf_corte_inf_produto p, {0}lov_crt_produto lp, 
															{0}lov_crt_inf_corte_inf_dest_mat lm where lp.id = p.produto 
															and lm.id = p.destinacao_material and p.inf_corte_inf = :inf_corte_inf order by p.id", EsquemaBanco);

						comando.AdicionarParametroEntrada("inf_corte_inf", informacaoCorte.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Produto produto = null;

							while (readerAux.Read())
							{
								produto = new Produto();
								produto.Id = Convert.ToInt32(readerAux["id"]);
								produto.ProdutoTipo = Convert.ToInt32(readerAux["produto"]);
								produto.ProdutoTipoTexto = readerAux["produto_texto"].ToString();
								produto.DestinacaoTipo = Convert.ToInt32(readerAux["destinacao_material"]);
								produto.DestinacaoTipoTexto = readerAux["destinacao_material_texto"].ToString();
								produto.Tid = readerAux["tid"].ToString();

								if (readerAux["quantidade"] != null && !Convert.IsDBNull(readerAux["quantidade"]))
								{
									produto.Quantidade = Convert.ToDecimal(readerAux["quantidade"]).ToString("N2");
								}

								informacaoCorte.Produtos.Add(produto);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.InformacoesCortes.Add(informacaoCorte);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal InformacaoCorteInformacao ObterInformacaoItem(int id, BancoDeDados banco = null)
		{
			InformacaoCorteInformacao informacaoCorte = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Informacao de Corte Informacao

				Comando comando = bancoDeDados.CriarComando(@"select i.arvores_isoladas_restante, i.area_corte_restante, i.data_informacao, i.tid
															from crt_inf_corte_inf i where i.id = :id order by i.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					if (reader.Read())
					{
						informacaoCorte = new InformacaoCorteInformacao();
						informacaoCorte.Id = id;
						informacaoCorte.Tid = reader["tid"].ToString();

						if (reader["data_informacao"] != null && !Convert.IsDBNull(reader["data_informacao"]))
						{
							informacaoCorte.DataInformacao.DataTexto = Convert.ToDateTime(reader["data_informacao"]).ToShortDateString();
						}

						if (reader["arvores_isoladas_restante"] != null && !Convert.IsDBNull(reader["arvores_isoladas_restante"]))
						{
							informacaoCorte.ArvoresIsoladasRestantes = Convert.ToDecimal(reader["arvores_isoladas_restante"]).ToString("N0");
						}

						if (reader["area_corte_restante"] != null && !Convert.IsDBNull(reader["area_corte_restante"]))
						{
							informacaoCorte.AreaCorteRestante = Convert.ToDecimal(reader["area_corte_restante"]).ToString("N4");
						}

						#region Especies

						comando = bancoDeDados.CriarComando(@"select e.id, e.especie, le.texto especie_texto, e.especie_especificar_texto, 
															e.arvores_isoladas, e.area_corte, e.idade_plantio, e.tid from crt_inf_corte_inf_especie e, 
															lov_crt_silvicultura_cult_fl le where le.id = e.especie 
															and e.inf_corte_inf = :inf_corte_inf order by e.id", EsquemaBanco);

						comando.AdicionarParametroEntrada("inf_corte_inf", informacaoCorte.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Especie especie = null;

							while (readerAux.Read())
							{
								especie = new Especie();
								especie.Id = Convert.ToInt32(readerAux["id"]);
								especie.EspecieTipo = Convert.ToInt32(readerAux["especie"]);
								especie.EspecieTipoTexto = readerAux["especie_texto"].ToString();

								especie.Tid = readerAux["tid"].ToString();

								if (readerAux["arvores_isoladas"] != null && !Convert.IsDBNull(readerAux["arvores_isoladas"]))
								{
									especie.ArvoresIsoladas = Convert.ToDecimal(readerAux["arvores_isoladas"]).ToString("N0");
								}

								if (readerAux["area_corte"] != null && !Convert.IsDBNull(readerAux["area_corte"]))
								{
									especie.AreaCorte = Convert.ToDecimal(readerAux["area_corte"]).ToString("N4");
								}

								if (readerAux["idade_plantio"] != null && !Convert.IsDBNull(readerAux["idade_plantio"]))
								{
									especie.IdadePlantio = Convert.ToDecimal(readerAux["idade_plantio"]).ToString("N0");
								}

								if (readerAux["especie_especificar_texto"] != null && !Convert.IsDBNull(readerAux["especie_especificar_texto"]))
								{
									especie.EspecieEspecificarTexto = readerAux["especie_especificar_texto"].ToString();
									especie.EspecieTipoTexto = especie.EspecieEspecificarTexto;
								}

								informacaoCorte.Especies.Add(especie);
							}

							readerAux.Close();
						}

						#endregion

						#region Produtos

						comando = bancoDeDados.CriarComando(@"select p.id, p.produto, lp.texto produto_texto, p.destinacao_material, 
															lm.texto destinacao_material_texto, p.quantidade, p.tid 
															from {0}crt_inf_corte_inf_produto p, {0}lov_crt_produto lp, 
															{0}lov_crt_inf_corte_inf_dest_mat lm where lp.id = p.produto 
															and lm.id = p.destinacao_material and p.inf_corte_inf = :inf_corte_inf order by p.id", EsquemaBanco);

						comando.AdicionarParametroEntrada("inf_corte_inf", informacaoCorte.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Produto produto = null;

							while (readerAux.Read())
							{
								produto = new Produto();
								produto.Id = Convert.ToInt32(readerAux["id"]);
								produto.ProdutoTipo = Convert.ToInt32(readerAux["produto"]);
								produto.ProdutoTipoTexto = readerAux["produto_texto"].ToString();
								produto.DestinacaoTipo = Convert.ToInt32(readerAux["destinacao_material"]);
								produto.DestinacaoTipoTexto = readerAux["destinacao_material_texto"].ToString();

								if (readerAux["quantidade"] != null && !Convert.IsDBNull(readerAux["quantidade"]))
								{
									produto.Quantidade = Convert.ToDecimal(readerAux["quantidade"]).ToString("N2");
								}

								produto.Tid = readerAux["tid"].ToString();

								informacaoCorte.Produtos.Add(produto);
							}

							readerAux.Close();
						}

						#endregion
					}

					reader.Close();
				}

				#endregion
			}

			return informacaoCorte;
		}

		private InformacaoCorte ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();
			int hst = 0;
			int hst_inf_corte_inf = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informacao de Corte

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.tid 
															from {0}hst_crt_informacao_corte c where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento_id"]);
						caracterizacao.Tid = reader["tid"].ToString();
						hst = Convert.ToInt32(reader["id"]);
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Informacoes

				comando = bancoDeDados.CriarComando(@"select i.id, i.inf_corte_inf, i.arvores_isoladas_restante, i.area_corte_restante, i.data_informacao, i.tid
													from hst_crt_inf_corte_inf i where i.caracterizacao = :caracterizacao and i.id_hst = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					InformacaoCorteInformacao informacaoCorte = null;

					while (reader.Read())
					{
						informacaoCorte = new InformacaoCorteInformacao();
						informacaoCorte.Id = Convert.ToInt32(reader["inf_corte_inf"]);
						informacaoCorte.ArvoresIsoladasRestantes = reader["arvores_isoladas_restante"].ToString();
						informacaoCorte.AreaCorteRestante = reader["area_corte_restante"].ToString();
						informacaoCorte.DataInformacao.DataTexto = Convert.ToDateTime(reader["data_informacao"]).ToShortDateString();
						informacaoCorte.Tid = reader["tid"].ToString();
						hst_inf_corte_inf = Convert.ToInt32(reader["id"]);

						#region Especies

						comando = bancoDeDados.CriarComando(@"select e.especie_entidade_id, e.especie_id, e.especie_texto, e.especie_especificar_texto, 
															e.arvores_isoladas, e.area_corte, e.idade_plantio, e.tid from hst_crt_inf_corte_inf_especie e
															where e.id_hst = :hst_inf_corte_inf", EsquemaBanco);

						comando.AdicionarParametroEntrada("inf_corte_inf", informacaoCorte.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("hst_inf_corte_inf", hst_inf_corte_inf, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Especie especie = null;

							while (readerAux.Read())
							{
								especie = new Especie();
								especie.Id = Convert.ToInt32(readerAux["especie_entidade_id"]);
								especie.EspecieTipo = Convert.ToInt32(readerAux["especie_id"]);
								especie.EspecieTipoTexto = readerAux["especie_texto"].ToString();
								especie.ArvoresIsoladas = readerAux["arvores_isoladas"].ToString();
								especie.AreaCorte = readerAux["area_corte"].ToString();
								especie.Tid = readerAux["tid"].ToString();

								if (readerAux["especie_especificar_texto"] != null && !Convert.IsDBNull(readerAux["especie_especificar_texto"]))
								{
									especie.EspecieEspecificarTexto = readerAux["especie_especificar_texto"].ToString();
								}

								if (readerAux["idade_plantio"] != null && !Convert.IsDBNull(readerAux["idade_plantio"]))
								{
									especie.IdadePlantio = Convert.ToDecimal(readerAux["idade_plantio"]).ToString("N0");
								}

								informacaoCorte.Especies.Add(especie);
							}

							readerAux.Close();
						}

						#endregion

						#region Produtos

						comando = bancoDeDados.CriarComando(@"select p.id, p.produto_entidade_id, p.produto_id, p.produto_texto, p.destinacao_material_id, 
															p.destinacao_material_texto, p.quantidade, p.tid 
															from {0}hst_crt_inf_corte_inf_produto p
															where p.id_hst = :hst_inf_corte_inf", EsquemaBanco);

						comando.AdicionarParametroEntrada("hst_inf_corte_inf", hst_inf_corte_inf, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Produto produto = null;

							while (readerAux.Read())
							{
								produto = new Produto();
								produto.Id = Convert.ToInt32(readerAux["produto_entidade_id"]);
								produto.ProdutoTipo = Convert.ToInt32(readerAux["produto_id"]);
								produto.ProdutoTipoTexto = readerAux["produto_texto"].ToString();
								produto.DestinacaoTipo = Convert.ToInt32(readerAux["destinacao_material_id"]);
								produto.DestinacaoTipoTexto = readerAux["destinacao_material_texto"].ToString();
								produto.Quantidade = readerAux["quantidade"].ToString();
								produto.Tid = readerAux["tid"].ToString();

								informacaoCorte.Produtos.Add(produto);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.InformacoesCortes.Add(informacaoCorte);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		#endregion
	}
}