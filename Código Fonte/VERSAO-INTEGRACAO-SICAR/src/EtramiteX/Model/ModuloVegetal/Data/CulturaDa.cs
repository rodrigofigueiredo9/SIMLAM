using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data
{
	public class CulturaDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		#region Ações DML

		internal void Salvar(Cultura cultura, BancoDeDados banco = null)
		{
			if (cultura == null)
			{
				throw new Exception("Objeto é nulo.");
			}

			if (cultura.Id <= 0)
			{
				Criar(cultura, banco);
			}
			else
			{
				Editar(cultura, banco);
			}
		}

		private void Criar(Cultura cultura, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cultura

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_cultura (id, texto, tid) values (seq_cultura.nextval, :cultura, :tid) returning id into :id", EsquemaBanco);
				
				comando.AdicionarParametroEntrada("cultura", cultura.Nome, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				cultura.Id = comando.ObterValorParametro<int>("id");

				if (cultura.LstCultivar.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_cultura_cultivar(id, cultivar, cultura, tid) 
													      values({0}seq_cultura_cultivar.nextval, :cultivar, :cultura, :tid) returning id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("cultura", cultura.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("cultivar", DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					//Insert Configurações Adicional
					Comando cmdCultivarConfiguracao = bancoDeDados.CriarComando(@"
					insert into {0}tab_cultivar_configuracao(id, tid, cultivar, praga, tipo_producao, declaracao_adicional)
					values({0}seq_cultura_cultivar_config.nextval, :tid, :cultivar, :praga, :tipo_producao, :declaracao_adicional)", EsquemaBanco);

					foreach (Cultivar item in cultura.LstCultivar)
					{
						comando.SetarValorParametro("cultivar", item.Nome);

						bancoDeDados.ExecutarNonQuery(comando);

						cultura.Cultivar.Id = comando.ObterValorParametro<int>("id");

						item.LsCultivarConfiguracao.ForEach(x => 
						{
							cmdCultivarConfiguracao.AdicionarParametroEntrada("cultivar", cultura.Cultivar.Id, DbType.Int32);
							cmdCultivarConfiguracao.AdicionarParametroEntrada("praga", x.PragaId, DbType.Int32);
							cmdCultivarConfiguracao.AdicionarParametroEntrada("tipo_producao", x.TipoProducaoId, DbType.Int32);
							cmdCultivarConfiguracao.AdicionarParametroEntrada("declaracao_adicional", x.DeclaracaoAdicionalId, DbType.Int32);
							cmdCultivarConfiguracao.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

							bancoDeDados.ExecutarNonQuery(cmdCultivarConfiguracao);
						});
					}
				}				

				#endregion

				Historico.Gerar(cultura.Id, eHistoricoArtefato.cultura, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		private void Editar(Cultura cultura, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cultura

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_cultura set texto = :cultura, tid = :tid where id = :id", EsquemaBanco);
				Comando cmdConfiguracao = null;

				comando.AdicionarParametroEntrada("cultura", cultura.Nome, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", cultura.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

                //Cultivar
                comando = bancoDeDados.CriarComando("delete from {0}tab_cultura_cultivar c ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where c.cultura = :cultura{0}",
                comando.AdicionarNotIn("and", "c.id", DbType.Int32, cultura.LstCultivar.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("cultura", cultura.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);	

				if (cultura.LstCultivar != null && cultura.LstCultivar.Count > 0)
				{
					int idCultivar = 0;
					foreach (Cultivar item in cultura.LstCultivar)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_cultura_cultivar c set c.cultivar = :cultivar, c.tid = :tid  where c.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_cultura_cultivar(id, cultivar, cultura, tid) 
							values ({0}seq_cultura_cultivar.nextval, :cultivar, :cultura, :tid) returning id into :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("cultura", cultura.Id, DbType.Int32);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroEntrada("cultivar", item.Nome, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);

						if(item.Id < 0)
						{
							idCultivar = comando.ObterValorParametro<int>("id");
						}

						//Cultura_Cultivar_Configuracões
						comando = bancoDeDados.CriarComando(@"delete from {0}tab_cultivar_configuracao t where t.cultivar = :cultivar", EsquemaBanco);
						comando.AdicionarParametroEntrada("cultivar", item.Id, DbType.Int32);
						comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "t.id", DbType.Int32,  item.LsCultivarConfiguracao.Select(xx => xx.Id).ToList());
						bancoDeDados.ExecutarNonQuery(comando);

						cmdConfiguracao = null;
						if (item.LsCultivarConfiguracao != null )
						{
							item.LsCultivarConfiguracao.ForEach(x =>
							{
								if (x.Id > 0)
								{
									cmdConfiguracao = bancoDeDados.CriarComando(@"update tab_cultivar_configuracao t set t.cultivar = :cultivar, t.praga = :praga, 
								    t.tipo_producao = :tipo_producao, t.declaracao_adicional =:declaracao_adicional, t.tid = :tid where t.id = :id", EsquemaBanco);
									cmdConfiguracao.AdicionarParametroEntrada("cultivar", x.Cultivar, DbType.Int32);
									cmdConfiguracao.AdicionarParametroEntrada("id", x.Id, DbType.Int32);
								}
								else
								{
									cmdConfiguracao = bancoDeDados.CriarComando(@"
									insert into {0}tab_cultivar_configuracao(id, tid, cultivar, praga, tipo_producao, declaracao_adicional)
									values({0}seq_cultura_cultivar_config.nextval, :tid, :cultivar, :praga, :tipo_producao, :declaracao_adicional)", EsquemaBanco);

									if (item.Id < 0)
									{
										cmdConfiguracao.AdicionarParametroEntrada("cultivar", idCultivar, DbType.Int32);
									}
									else
									{
										cmdConfiguracao.AdicionarParametroEntrada("cultivar", x.Cultivar, DbType.Int32);
									}
								}

								cmdConfiguracao.AdicionarParametroEntrada("praga", x.PragaId, DbType.Int32);
								cmdConfiguracao.AdicionarParametroEntrada("tipo_producao", x.TipoProducaoId, DbType.Int32);
								cmdConfiguracao.AdicionarParametroEntrada("declaracao_adicional", x.DeclaracaoAdicionalId, DbType.Int32);
								cmdConfiguracao.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(cmdConfiguracao);
							});
						}
					}
				}

				#endregion

				Historico.Gerar(cultura.Id, eHistoricoArtefato.cultura, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Listar

		internal Cultura Obter(int id)
		{
			Cultura cultura = new Cultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Cultura

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.tid from {0}tab_cultura t where t.id = :id", EsquemaBanco);

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

				comando.DbCommand.CommandText = @" select cc.id, cc.cultivar nome, cc.tid from tab_cultura_cultivar cc where cc.cultura = :id";
				cultura.LstCultivar = bancoDeDados.ObterEntityList<Cultivar>(comando);

				#endregion

				#region Cultivar_Configurações

				comando = bancoDeDados.CriarComando(@"
				select t.id, t.tid, t.cultivar, t.praga PragaId, p.nome_cientifico || nvl2(p.nome_comum,' - '||p.nome_comum,'') as PragaTexto, t.tipo_producao TipoProducaoId,
				lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto
				from {0}tab_cultivar_configuracao t, {0}tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld
				where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and t.cultivar = :id", EsquemaBanco);

				cultura.LstCultivar.ForEach(x =>
				{
					comando.AdicionarParametroEntrada("id", x.Id, DbType.Int32);
					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							x.LsCultivarConfiguracao.Add(new CultivarConfiguracao()
							{
								Id = reader.GetValue<int>("id"),
								Tid = reader.GetValue<string>("tid"),
								Cultivar = reader.GetValue<int>("cultivar"),
								PragaId = reader.GetValue<int>("PragaId"),
								PragaTexto = reader.GetValue<string>("PragaTexto"),
								TipoProducaoId = reader.GetValue<int>("TipoProducaoId"),
								TipoProducaoTexto = reader.GetValue<string>("TipoProducaoTexto"),
								DeclaracaoAdicionalId = reader.GetValue<int>("DeclaracaoAdicionalId"),
								DeclaracaoAdicionalTexto = reader.GetValue<string>("DeclaracaoAdicionalTexto")
							});
						}

						reader.Close();
					}
				});

				#endregion
			}

			return cultura;
		}

		internal Resultados<CulturaListarResultado> Filtrar(Filtro<CulturaListarFiltro> filtro)
		{
			Resultados<CulturaListarResultado> retorno = new Resultados<CulturaListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");
				List<int> culturas = new List<int>();

				#region Adicionando Filtros

				if (!string.IsNullOrEmpty(filtro.Dados.Cultivar))
				{
					if (filtro.Dados.StraggCultivar)
					{
						comandtxt = comando.FiltroAndLike("cc.cultivar", "cultivar", filtro.Dados.Cultivar, true, true);

						comando.DbCommand.CommandText = String.Format(@"select distinct cc.cultura from tab_cultura_cultivar cc where cc.id > 0" + comandtxt, esquemaBanco);
						culturas = bancoDeDados.ExecutarList<int>(comando);

						comando = bancoDeDados.CriarComando("");
						comandtxt = string.Empty;
					}
					else
					{
						comandtxt = comando.FiltroAndLike("cc.cultivar", "cultivar", filtro.Dados.Cultivar, true, true);
					}
				}

				comandtxt += comando.FiltroAndLike("c.texto", "cultura", filtro.Dados.Cultura, true, true);

				comandtxt += comando.AdicionarIn("and", "c.id", DbType.Int32, culturas);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "cultura", "cultivar" };

				if (filtro.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("cultura");
				}

				#endregion

				#region Quantidade de registro do resultado

				if (filtro.Dados.StraggCultivar)
				{
					comando.DbCommand.CommandText = String.Format(@"select count(*) from tab_cultura c where c.id > 0" + comandtxt, esquemaBanco);
				}
				else
				{
					comando.DbCommand.CommandText = String.Format(@"select count(*) from(select c.id, c.texto cultura, cc.cultivar 
					from {0}tab_cultura c, {0}tab_cultura_cultivar cc where cc.cultura = c.id " + comandtxt + ")", esquemaBanco);
				}

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				if (filtro.Dados.StraggCultivar)
				{
					comandtxt = String.Format(@"select c.id, c.texto cultura, stragg(cc.cultivar) cultivar from {0}tab_cultura c, {0}tab_cultura_cultivar cc
					where cc.cultura(+) = c.id " + comandtxt + " group by c.id, c.texto " + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);
				}
				else
				{
					comandtxt = String.Format(@"select c.id, c.texto cultura, cc.cultivar, cc.id cultivar_id from {0}tab_cultura c, {0}tab_cultura_cultivar cc 
					where cc.cultura = c.id " + comandtxt + " group by c.id, c.texto, cc.cultivar, cc.id" + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);
				}

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					CulturaListarResultado item;

					while (reader.Read())
					{
						item = new CulturaListarResultado();
						item.Id = reader.GetValue<int>("id");
						item.Cultura = reader.GetValue<string>("cultura");
						item.Cultivar = reader.GetValue<string>("cultivar");

						if (!filtro.Dados.StraggCultivar)
						{
							item.CultivarId = reader.GetValue<string>("cultivar_id");
						}

						retorno.Itens.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		public bool Existe(Cultura cultura, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_cultura where lower(texto) = :cultura", EsquemaBanco);
				comando.AdicionarParametroEntrada("cultura", DbType.String, 100, cultura.Nome.ToLower());

				int culturaId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return culturaId > 0 && culturaId != cultura.Id;
			}
		}

		#endregion

		internal List<Lista> ObterLstCultivar(int culturaId, BancoDeDados banco = null)
		{
			List<Lista> lstCultivar = new List<Lista>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id, cultivar from tab_cultura_cultivar where cultura = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", culturaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista lista = null;

					while (reader.Read())
					{
						lista = new Lista();
						lista.Id = reader.GetValue<string>("id");
						lista.Texto = reader.GetValue<string>("cultivar");
						lstCultivar.Add(lista);
					}
				}
			}

			return lstCultivar;
		}
	}
}