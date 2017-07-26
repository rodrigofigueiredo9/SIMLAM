using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data
{
	class LoteDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		public string EsquemaCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public LoteDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#endregion

		#region Ações de DML

		public void Salvar(Lote lote, BancoDeDados banco = null)
		{
			if (lote == null)
			{
				throw new Exception("Lote é nulo.");
			}

			if (lote.Id == 0)
			{
				Criar(lote, banco);
			}
			else
			{
				Editar(lote, banco);
			}
		}

		internal void Criar(Lote lote, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}tab_lote t (id, tid, situacao, empreendimento, data_criacao, codigo_uc, ano, numero, credenciado)
				values (seq_tab_lote.nextval, :tid, :situacao, :empreendimento, :data_criacao, :codigo_uc, :ano, :numero, :credenciado) returning t.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("situacao", (int)eLoteSituacao.NaoUtilizado, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", lote.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("data_criacao", lote.DataCriacao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("codigo_uc", lote.CodigoUC, DbType.Int64);
				comando.AdicionarParametroEntrada("ano", lote.Ano, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", lote.Numero, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				lote.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Lotes

				comando = bancoDeDados.CriarComando(@"
				insert into tab_lote_item (id, tid, lote, origem_tipo, origem, origem_numero, cultura, cultivar, quantidade, unidade_medida, exibe_kilos)
				values (seq_tab_lote_item.nextval, :tid, :lote, :origem_tipo, :origem, :origem_numero, :cultura, :cultivar, :quantidade, :unidade_medida, :exibe_kilos)");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("lote", lote.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("origem_tipo", DbType.Int32);
				comando.AdicionarParametroEntrada("origem", DbType.Int32);
				comando.AdicionarParametroEntrada("origem_numero", DbType.Int64);
				comando.AdicionarParametroEntrada("cultura", DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
				comando.AdicionarParametroEntrada("unidade_medida", DbType.Int32);
				comando.AdicionarParametroEntrada("quantidade", DbType.Decimal);
                comando.AdicionarParametroEntrada("exibe_kilos", DbType.String, 1);

				lote.Lotes.ForEach(item =>
				{
					comando.SetarValorParametro("origem_tipo", item.OrigemTipo);
					comando.SetarValorParametro("origem", item.Origem);
					comando.SetarValorParametro("origem_numero", item.OrigemNumero);
					comando.SetarValorParametro("cultura", item.Cultura);
					comando.SetarValorParametro("cultivar", item.Cultivar);
					comando.SetarValorParametro("unidade_medida", item.UnidadeMedida);
					comando.SetarValorParametro("quantidade", item.Quantidade);
                    comando.SetarValorParametro("exibe_kilos", item.ExibeKg ? "1" : "0" );
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion Lotes

				Historico.Gerar(lote.Id, eHistoricoArtefato.lote, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Editar(Lote lote, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update tab_lote t set t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", lote.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Lotes

				//Limpar
				comando = bancoDeDados.CriarComando(@"delete from tab_lote_item ", EsquemaCredenciado);
				comando.DbCommand.CommandText += String.Format("where lote = :lote {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, lote.Lotes.Select(p => p.Id).ToList()));
				comando.AdicionarParametroEntrada("lote", lote.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				lote.Lotes.ForEach(item =>
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_lote_item  set tid =:tid, origem_tipo = :origem_tipo, origem = :origem, origem_numero = :origem_numero,
						cultura = :cultura, cultivar = :cultivar, unidade_medida = :unidade_medida, quantidade = :quantidade, exibe_kilos = :exibe_kilos where id = :id");

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into tab_lote_item (id, tid, lote, origem_tipo, origem, origem_numero, cultura, cultivar, quantidade, unidade_medida, exibo_kilos)
						values (seq_tab_lote_item.nextval, :tid, :lote, :origem_tipo, :origem, :origem_numero, :cultura, :cultivar, :quantidade, :unidade_medida, :exibe_kilos)");

						comando.AdicionarParametroEntrada("lote", lote.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("origem_tipo", item.OrigemTipo, DbType.Int32);
					comando.AdicionarParametroEntrada("origem", item.Origem, DbType.Int32);
					comando.AdicionarParametroEntrada("origem_numero", item.OrigemNumero, DbType.Int64);
					comando.AdicionarParametroEntrada("cultura", item.Cultura, DbType.Int32);
					comando.AdicionarParametroEntrada("cultivar", item.Cultivar, DbType.Int32);
					comando.AdicionarParametroEntrada("unidade_medida", item.UnidadeMedida, DbType.Int32);
					comando.AdicionarParametroEntrada("quantidade", item.Quantidade, DbType.Decimal);
                    comando.AdicionarParametroEntrada("exibe_kilos", DbType.String , 1, item.ExibeKg ? "1" : "0");
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion Lotes

				Historico.Gerar(lote.Id, eHistoricoArtefato.lote, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_lote c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.lote, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					delete {0}tab_lote_item where lote = :id;
					delete {0}tab_lote a where a.id = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacaoLote(int id, eLoteSituacao situacao, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_lote l set l.situacao = :situacao, l.tid =:tid where l.id = :lote_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("lote_id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.lote, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Lote Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			Lote retorno = new Lote();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				#region Dados

				Comando comando = bancoDeDados.CriarComando(@"select tid, situacao, empreendimento, data_criacao, codigo_uc, ano, numero from tab_lote t where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.Id = id;
						retorno.Tid = reader.GetValue<string>("tid");
						retorno.SituacaoId = reader.GetValue<int>("situacao");
						retorno.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						retorno.DataCriacao.DataTexto = reader.GetValue<String>("data_criacao");
						retorno.CodigoUC = reader.GetValue<long>("codigo_uc");
						retorno.Ano = reader.GetValue<int>("ano");
						retorno.Numero = reader.GetValue<string>("numero");
					}

					reader.Close();
				}

				#endregion

				#region Lotes

				comando = bancoDeDados.CriarComando(@"
				select t.id,
					t.origem_tipo,
					o.texto origem_texto,
					t.origem,
					t.cultura,
					c.texto cultura_texto,
					t.cultivar,
					cc.cultivar cultivar_texto,
					t.quantidade,
                    t.exibe_kilos,
					(case 
					when t.origem_tipo = 1 then (select cf.numero from tab_cfo cf where cf.id = t.origem)
					when t.origem_tipo = 2 then (select cf.numero from tab_cfoc cf where cf.id = t.origem)
					when t.origem_tipo = 4 then (select cf.numero from tab_ptv_outrouf cf where cf.id = t.origem)
					else t.origem_numero end) origem_numero, 
					t.unidade_medida,
					(select l.texto from lov_crt_uni_prod_uni_medida l where l.id = t.unidade_medida) unidade_medida_texto 
				from tab_lote_item                t,
					lov_doc_fitossanitarios_tipo  o,
					tab_cultura                   c,
					tab_cultura_cultivar          cc
				where t.origem_tipo = o.id
				and t.cultura = c.id
				and t.cultivar = cc.id
				and t.lote = :lote", EsquemaBanco);

				comando.AdicionarParametroEntrada("lote", retorno.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Lotes.Add(new LoteItem()
						{
							Id = reader.GetValue<int>("id"),
							OrigemTipo = reader.GetValue<int>("origem_tipo"),
							OrigemTipoTexto = reader.GetValue<string>("origem_texto"),
							Origem = reader.GetValue<int>("origem"),
							OrigemNumero = reader.GetValue<string>("origem_numero"),
							Cultura = reader.GetValue<int>("cultura"),
							CulturaTexto = reader.GetValue<string>("cultura_texto"),
							Cultivar = reader.GetValue<int>("cultivar"),
							CultivarTexto = reader.GetValue<string>("cultivar_texto"),
							UnidadeMedida = reader.GetValue<int>("unidade_medida"),
							UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
                            ExibeKg = reader.GetValue<string>("exibe_kilos") == "1" ? true : false
						});
					}

					reader.Close();
				}

				#endregion Lotes
			}

			return retorno;
		}

		public Resultados<Lote> Filtrar(Filtro<Lote> filtros)
		{
			Resultados<Lote> retorno = new Resultados<Lote>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("d.credenciado", "credenciado", User.FuncionarioId);

				comandtxt += comando.FiltroAnd("d.empreendimento", "empreendimento", filtros.Dados.EmpreendimentoId);

				comandtxt += comando.FiltroAnd("d.numero_completo", "numero_completo", filtros.Dados.Numero);

				// += comando.FiltroAnd("d.situacao", "situacao", filtros.Dados.SituacaoId);

				comandtxt += comando.FiltroAnd("d.data_criacao", "data_criacao", filtros.Dados.DataCriacao.DataTexto);

				comandtxt += comando.FiltroAndLike("d.cultura_cultivar", "cultura_cultivar", filtros.Dados.CulturaCultivar, upper: true, likeInicio: true);

                comandtxt += comando.FiltroAndLike("d.denominador", "denominador", filtros.Dados.NomeEmpreendimento, upper: true, likeInicio: true);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "data_criacao", "cultivar" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

                comando.DbCommand.CommandText = String.Format(@"select COUNT(*) from 
                (select l.id, l.tid, l.codigo_uc, l.ano, lpad(l.numero, 4, '0') numero,
					    l.codigo_uc || l.ano || lpad(l.numero, 4, '0') numero_completo, l.data_criacao, l.situacao, ls.texto situacao_texto, l.empreendimento, 
					    l.credenciado, c.cultura_id, c.cultura, c.cultivar_id, c.cultivar, c.cultura || '/' || c.cultivar cultura_cultivar, c.quantidade - (
                            select nvl(sum(lote.quantidade),0) from tab_cfoc_produto cfoc inner join tab_lote_item lote on cfoc.lote = lote.lote where lote.lote = l.id ) as quantidade, c.unidade_medida, 
                c.unidade_medida_texto, emp.denominador
				    from tab_lote l, lov_lote_situacao ls, IDAF.tab_empreendimento emp,
					    (select i.lote, c.id cultura_id, c.texto cultura, cc.id cultivar_id, cc.cultivar, sum(i.quantidade) quantidade,
						    i.unidade_medida, i.exibe_kilos, (
                select l.texto from lov_crt_uni_prod_uni_medida l where l.id = i.unidade_medida) unidade_medida_texto
						    from tab_lote_item i, tab_cultura c, tab_cultura_cultivar cc
						    where c.id = i.cultura and cc.id = i.cultivar 
                group by i.lote, i.unidade_medida, i.exibe_kilos, c.id, c.texto, cc.id, cc.cultivar, cc.tipo_producao) c
				    where ls.id = l.situacao and c.lote = l.id and emp.id = l.empreendimento) d where d.quantidade > 0 and d.id > 0" + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

                comandtxt = String.Format(@"select * from 
                    (select l.id, l.tid, l.codigo_uc, l.ano, lpad(l.numero, 4, '0') numero,
					        l.codigo_uc || l.ano || lpad(l.numero, 4, '0') numero_completo, l.data_criacao, l.situacao, ls.texto situacao_texto, l.empreendimento, 
					        l.credenciado, c.cultura_id, c.cultura, c.cultivar_id, c.cultivar, c.cultura || '/' || c.cultivar cultura_cultivar, c.quantidade - (
                             select nvl(sum(cfoc.quantidade),0) from tab_cfoc_produto cfoc 
                                                               inner join tab_cfoc tc on tc.id = cfoc.cfoc
                                                               where tc.situacao = 2 and cfoc.lote = l.id ) as quantidade, c.unidade_medida, c.exibe_kilos,
                  c.unidade_medida_texto, emp.denominador
				        from tab_lote l, lov_lote_situacao ls, IDAF.tab_empreendimento emp,
					        (select i.lote, c.id cultura_id, c.texto cultura, cc.id cultivar_id, cc.cultivar, sum(i.quantidade) quantidade,
						        i.unidade_medida, i.exibe_kilos, (
                    select l.texto from lov_crt_uni_prod_uni_medida l where l.id = i.unidade_medida) unidade_medida_texto
						        from tab_lote_item i, tab_cultura c, tab_cultura_cultivar cc
						        where c.id = i.cultura and cc.id = i.cultivar 
                    group by i.lote, i.unidade_medida, i.exibe_kilos, c.id, c.texto, cc.id, cc.cultivar, cc.tipo_producao) c
				        where ls.id = l.situacao and c.lote = l.id and emp.id = l.empreendimento) d where d.quantidade > 0 and d.id > 0 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Lote item;

					while (reader.Read())
					{
						item = new Lote();
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.CodigoUC = reader.GetValue<long>("codigo_uc");
						item.Ano = reader.GetValue<int>("ano");
						item.Numero = reader.GetValue<string>("numero");
						item.DataCriacao.Data = reader.GetValue<DateTime>("data_criacao");
						item.SituacaoId = reader.GetValue<int>("situacao");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						item.Item.Cultura = reader.GetValue<int>("cultura_id");
						item.Item.CulturaTexto = reader.GetValue<string>("cultura");
						item.Item.Cultivar = reader.GetValue<int>("cultivar_id");
						item.Item.CultivarTexto = reader.GetValue<string>("cultivar");
						item.Item.UnidadeMedida = reader.GetValue<int>("unidade_medida");
						item.Item.UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto");
						item.Item.Quantidade = reader.GetValue<decimal>("quantidade");
                        item.Item.ExibeKg = reader.GetValue<string>("exibe_kilos") == "1" ? true : false;
                        

						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Lista ObterCodigoUC(int id)
		{
			Lista retorno = new Lista();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select uc.id, uc.codigo_uc from crt_unidade_consolidacao uc where uc.empreendimento =:id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.Id = reader.GetValue<string>("id");
						retorno.Texto = reader.GetValue<string>("codigo_uc");
					}

					reader.Close();
				}

				return retorno;
			}
		}

		internal List<Lista> ObterCultivar(int origemTipo, int origemID, int culturaID)
		{
			if ((eDocumentoFitossanitarioTipo)origemTipo == eDocumentoFitossanitarioTipo.PTVOutroEstado)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					Comando comando = bancoDeDados.CriarComando(@"
					select distinct cc.id, cc.cultivar
						from tab_ptv_outrouf_produto pp,
						tab_cultura_cultivar cc
					where cc.id = pp.cultivar 
						and pp.cultura = :culturaID 
						and pp.ptv = :origemID", EsquemaBanco);

					comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
					comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);

					List<Lista> lstLoteItens = new List<Lista>();
					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							lstLoteItens.Add(new Lista() { Id = reader.GetValue<int>("id").ToString(), Texto = reader.GetValue<string>("cultivar") });
						}

						reader.Close();
					}

					return lstLoteItens;
				}
			}

			if ((eDocumentoFitossanitarioTipo)origemTipo == eDocumentoFitossanitarioTipo.PTV)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					Comando comando = bancoDeDados.CriarComando(@"
					select distinct cc.id, cc.cultivar
					  from TAB_PTV_PRODUTO pp, tab_cultura_cultivar cc
					 where cc.id = pp.cultivar
					   and pp.cultura = :culturaID
					   and pp.ptv = :origemID", EsquemaBanco);

					comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
					comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);

					List<Lista> lstLoteItens = new List<Lista>();
					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							lstLoteItens.Add(new Lista() { Id = reader.GetValue<int>("id").ToString(), Texto = reader.GetValue<string>("cultivar") });
						}

						reader.Close();
					}

					return lstLoteItens;
				}
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = null;
				switch ((eDocumentoFitossanitarioTipo)origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
						comando = bancoDeDados.CriarComando(@"
						select distinct cc.id, cc.cultivar
						from {0}tab_cfo_produto           cp,
							ins_crt_unidade_prod_unidade  i,
							{0}tab_cultura                c,
							{0}tab_cultura_cultivar       cc 
						where i.id = cp.unidade_producao
							and c.id = i.cultura
							and cc.id = i.cultivar
							and i.cultura = :culturaID
							and cp.cfo = :origemID", EsquemaBanco);
						break;

					case eDocumentoFitossanitarioTipo.CFOC:
						comando = bancoDeDados.CriarComando(@"
						select distinct cc.id, cc.cultivar
							from tab_cfoc_produto     cp,
								tab_lote              l,
								tab_lote_item         li,
								tab_cultura           c,
								tab_cultura_cultivar  cc
							where l.id = cp.lote
							and li.lote = l.id
							and c.id = li.cultura
							and cc.id = li.cultivar
							and li.cultura = :culturaID
							and cp.cfoc = :origemID", EsquemaBanco);
						break;
				}

				comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
				comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);

				List<Lista> lstLoteItens = new List<Lista>();
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstLoteItens.Add(new Lista() { Id = reader.GetValue<int>("id").ToString(), Texto = reader.GetValue<string>("cultivar") });
					}

					reader.Close();
				}

				return lstLoteItens;
			}
		}

		internal List<Lista> ObterUnidadeMedida(int origemTipo, int origemID, int culturaID, int cultivarID, out decimal Quantidade)
		{

            Quantidade = 0;

			if ((eDocumentoFitossanitarioTipo)origemTipo == eDocumentoFitossanitarioTipo.PTV)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{

                    Comando comandoQtd = bancoDeDados.CriarComando(@"
					select sum(pp.quantidade)
						from tab_ptv_produto pp,
						lov_crt_uni_prod_uni_medida lu
		            where lu.id = pp.unidade_medida 
                        and pp.cultura = :culturaID 
                        and pp.cultivar = :cultivarID
                        and pp.ptv = :origemID", EsquemaBanco);


                    comandoQtd.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
                    comandoQtd.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
                    comandoQtd.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

                    Quantidade = bancoDeDados.ExecutarScalar<decimal>(comandoQtd);

				    Comando comando = bancoDeDados.CriarComando(@"
					select distinct lu.id, lu.texto
						from tab_ptv_produto pp,
						lov_crt_uni_prod_uni_medida lu
		            where lu.id = pp.unidade_medida 
                        and pp.cultura = :culturaID 
                        and pp.cultivar = :cultivarID
                        and pp.ptv = :origemID", EsquemaBanco);

					comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
					comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
					comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

					List<Lista> retorno = new List<Lista>();
					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							retorno.Add(new Lista() { Id = reader.GetValue<int>("id").ToString(), Texto = reader.GetValue<string>("texto") });
						}

						reader.Close();
					}

					return retorno;
				}
			}

			if ((eDocumentoFitossanitarioTipo)origemTipo == eDocumentoFitossanitarioTipo.PTVOutroEstado)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{

                    Comando comandoQtd = bancoDeDados.CriarComando(@"
					select sum(pp.quantidade)
						from tab_ptv_outrouf_produto pp,
						lov_crt_uni_prod_uni_medida lu
					where lu.id = pp.unidade_medida 
						and pp.cultura = :culturaID 
						and pp.cultivar = :cultivarID
						and pp.ptv = :origemID", EsquemaBanco);


                    comandoQtd.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
                    comandoQtd.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
                    comandoQtd.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

                    Quantidade = bancoDeDados.ExecutarScalar<decimal>(comandoQtd);


					Comando comando = bancoDeDados.CriarComando(@"
					select distinct lu.id, lu.texto
						from tab_ptv_outrouf_produto pp,
						lov_crt_uni_prod_uni_medida lu
					where lu.id = pp.unidade_medida 
						and pp.cultura = :culturaID 
						and pp.cultivar = :cultivarID
						and pp.ptv = :origemID", EsquemaBanco);

					comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
					comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
					comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

					List<Lista> retorno = new List<Lista>();
					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							retorno.Add(new Lista() { Id = reader.GetValue<int>("id").ToString(), Texto = reader.GetValue<string>("texto") });
						}

						reader.Close();
					}

					return retorno;
				}
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando("");

				switch ((eDocumentoFitossanitarioTipo)origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:

                         Comando comandoQtd = bancoDeDados.CriarComando(@"
					    select sum(cp.quantidade)
						from tab_cfo_produto cp, ins_crt_unidade_prod_unidade i
							where i.id = cp.unidade_producao
							and i.cultivar = :cultivarID
							and i.cultura = :culturaID
							and cp.cfo = :origemID", EsquemaBanco);


                        comandoQtd.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
                        comandoQtd.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
                        comandoQtd.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

                        Quantidade = bancoDeDados.ExecutarScalar<decimal>(comandoQtd);


						comando = bancoDeDados.CriarComando(@"
						select (case when i.tipo_producao = 1 then (select lu.id from lov_crt_uni_prod_uni_medida lu where lu.id = 2)
									when i.tipo_producao = 2 then (select lu.id from lov_crt_uni_prod_uni_medida lu where lu.id = 1)
									else (select lu.id from lov_crt_uni_prod_uni_medida lu where lu.id = 3) end) id,
							(case when i.tipo_producao = 1 then (select lu.texto from lov_crt_uni_prod_uni_medida lu where lu.id = 2)
									when i.tipo_producao = 2 then (select lu.texto from lov_crt_uni_prod_uni_medida lu where lu.id = 1)
									else (select lu.texto from lov_crt_uni_prod_uni_medida lu where lu.id = 3) end) texto
						from tab_cfo_produto cp, ins_crt_unidade_prod_unidade i
							where i.id = cp.unidade_producao
							and i.cultivar = :cultivarID
							and i.cultura = :culturaID
							and cp.cfo = :origemID", EsquemaBanco);
						break;

					case eDocumentoFitossanitarioTipo.CFOC:

                        comandoQtd = bancoDeDados.CriarComando(@"
					    select sum(cp.quantidade)
						from tab_cfoc_produto            cp,
							tab_lote                     l,
							tab_lote_item                li,
							lov_crt_uni_prod_uni_medida lu
						where l.id = cp.lote
							and li.lote = l.id
							and li.unidade_medida = lu.id
							and li.cultivar = :cultivarID
							and li.cultura = :culturaID
							and cp.cfoc = :origemID", EsquemaBanco);


                        comandoQtd.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
                        comandoQtd.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
                        comandoQtd.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

                        Quantidade = bancoDeDados.ExecutarScalar<decimal>(comandoQtd);



						comando = bancoDeDados.CriarComando(@"
						select lu.id, lu.texto
						from tab_cfoc_produto            cp,
							tab_lote                     l,
							tab_lote_item                li,
							lov_crt_uni_prod_uni_medida lu
						where l.id = cp.lote
							and li.lote = l.id
							and li.unidade_medida = lu.id
							and li.cultivar = :cultivarID
							and li.cultura = :culturaID
							and cp.cfoc = :origemID", EsquemaBanco);
						break;

					case eDocumentoFitossanitarioTipo.CFCFR:
					case eDocumentoFitossanitarioTipo.TF:
						comando = bancoDeDados.CriarComando(@"
						", EsquemaBanco);
						break;
				}

				comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
				comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
				comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

				List<Lista> retorno = new List<Lista>();
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<int>("id").ToString(), Texto = reader.GetValue<string>("texto") });
					}

					reader.Close();
				}

				return retorno;
			}
		}

		internal string ObterNumero(int empreendimentoID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select max(t.numero) from hst_lote t where t.ano = :ano and t.empreendimento_id = :empreendimentoID), 0) + 1 from dual", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("empreendimentoID", (int)empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("ano", DateTime.Today.Year.ToString().Substring(2), DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal decimal ObterOrigemQuantidade(eDocumentoFitossanitarioTipo origemTipo, int origemID, string origemNumero, int cultivarID, int unidadeMedida, int anoEmissao, int lote)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				string query = string.Empty;

				switch (origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
					case eDocumentoFitossanitarioTipo.CFOC:
					case eDocumentoFitossanitarioTipo.PTV:
						query = @"
						select (
						/*LOTE*/
						nvl((select sum(i.quantidade) quantidade
						from tab_lote t, tab_lote_item i
						where i.lote = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem = :origem
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and extract (year from t.data_criacao) = :anoEmissao
						and t.id != :lote), 0)
						+
						/*EPTV*/
						nvl((select sum(i.quantidade) quantidade
						from tab_ptv t, tab_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem = :origem
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and extract (year from t.data_emissao) = :anoEmissao
						and t.situacao != 3), 0)
						+
						/*PTV*/
						nvl((select sum(i.quantidade) quantidade
						from ins_ptv t, ins_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem = :origem
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and extract (year from t.data_emissao) = :anoEmissao
						and t.situacao != 3), 0)) saldo_utilizado from dual";
						break;
					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						query = @"
						select (
						/*LOTE*/
						nvl((select sum(i.quantidade) quantidade
						from tab_lote t, tab_lote_item i
						where i.lote = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem_numero = :origem_numero
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and t.id != :lote), 0)
						+
						/*EPTV*/
						nvl((select sum(i.quantidade) quantidade
						from tab_ptv t, tab_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.numero_origem = :origem_numero
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and t.situacao != 3), 0)
						+
						/*PTV*/
						nvl((select sum(i.quantidade) quantidade
						from ins_ptv t, ins_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.numero_origem = :origem_numero
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and t.situacao != 3), 0)) saldo_utilizado from dual";
						break;
				}

				Comando comando = bancoDeDados.CriarComando(query, EsquemaCredenciado);

				comando.AdicionarParametroEntrada("lote", lote, DbType.Int32);
				comando.AdicionarParametroEntrada("origem_tipo", (int)origemTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", cultivarID, DbType.Int32);
				comando.AdicionarParametroEntrada("unidade_medida", unidadeMedida, DbType.Int32);

				switch (origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
					case eDocumentoFitossanitarioTipo.CFOC:
					case eDocumentoFitossanitarioTipo.PTV:
						comando.AdicionarParametroEntrada("origem", origemID, DbType.Int32);
						comando.AdicionarParametroEntrada("anoEmissao", anoEmissao, DbType.Int32);
						break;
					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						comando.AdicionarParametroEntrada("origem_numero", origemNumero, DbType.String);
						break;
				}

				return Convert.ToDecimal(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		#region Validações

		internal bool UCPossuiCultivar(int empreendimentoID, int cultivarID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from crt_unidade_consolidacao c, crt_unidade_cons_cultivar cc 
				where cc.unidade_consolidacao = c.id and c.empreendimento = :empreendimentoID and cc.cultivar = :cultivarID", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal string LoteSituacao(int id, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.numero from {0}tab_cfoc_produto t, {0}tab_cfoc c where c.id = t.cfoc and c.situacao = 2 and t.lote = :id", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal string LoteAssociado(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.numero from tab_cfoc_produto p, tab_cfoc c where p.cfoc = c.id and p.lote = :lote", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("lote", id, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal string CFOCFOCJaAssociado(int origemTipo, int origemID, int empreendimentoID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select e.denominador from tab_lote l, tab_lote_item li, ins_empreendimento e 
				where li.lote = l.id and e.id = l.empreendimento
				and li.origem_tipo = :origemTipo and li.origem = :origemID and l.empreendimento != :empreendimentoID", EsquemaBanco);

				comando.AdicionarParametroEntrada("origemTipo", origemTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal string PTVOutroEstadoJaAssociado(int origemID, int empreendimentoID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select d.nome from tab_ptv_outrouf p, tab_destinatario_ptv d 
				where d.id = p.destinatario and p.id = :origemID and d.empreendimento_id != :empreendimentoID", EsquemaBanco);

				comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}
		public bool VerificarSeDocumentoUtilizadoPorMesmaUC(int origemID, int empreendimentoID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_lote tl, tab_lote_item tli where tl.id = tli.lote and tli.origem_tipo = 1
					and tli.origem = :origemID and tl.codigo_uc = (select uc.codigo_uc from ins_crt_unidade_consolidacao uc where uc.empreendimento=:empreendimentoID )", EsquemaBanco);

				comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		#endregion


		public bool VerificarSeCfoJaAssociadaALote(int origemID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_lote tl, tab_lote_item tli where tl.id = tli.lote and tli.origem_tipo = 1 and tli.origem = :origemID", EsquemaBanco);

				comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		public bool VerificarSeDocumentoJaAssociadaALote(int origemID, int origemTipo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_lote tl, tab_lote_item tli where tl.id = tli.lote and tli.origem_tipo = :origemTipo and tli.origem = :origemID", EsquemaBanco);

				comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
				comando.AdicionarParametroEntrada("origemTipo", origemTipo, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}
	}
}