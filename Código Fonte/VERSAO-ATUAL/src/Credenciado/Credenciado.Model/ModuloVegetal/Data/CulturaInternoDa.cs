﻿using System.Data;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using System.Linq;
using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using System.Text;
using System.Collections;
using Tecnomapas.EtramiteX.Configuracao;


namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Data
{
	public class CulturaInternoDa
	{
		#region Propriedades

        GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private string EsquemaBanco { get; set; }

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
				lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto, ld.texto_formatado as DeclaracaoAdicionalTextoHtml
				from {0}tab_cultivar_configuracao t, {0}tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld
				where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and ld.outro_estado = '0' and t.cultivar = :id", EsquemaBanco);

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
								DeclaracaoAdicionalTexto = reader.GetValue<string>("DeclaracaoAdicionalTexto"),
								DeclaracaoAdicionalTextoHtml = reader.GetValue<string>("DeclaracaoAdicionalTextoHtml"),
							});
						}

						reader.Close();
					}
				});

				#endregion
			}

			return cultura;
		}

		public List<Cultivar> ObterCultivares(int culturaId)
		{
			List<Cultivar> retorno = new List<Cultivar>();
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select id, cultivar from tab_cultura_cultivar where cultura = :cultura_id", EsquemaBanco);
				comando.AdicionarParametroEntrada("cultura_id", culturaId, DbType.Int32);

				using (IDataReader reader = banco.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Cultivar()
								{
									Id = reader.GetValue<int>("id"),
									Nome = reader.GetValue<string>("cultivar"),
									CulturaId = culturaId
								});
					}

					reader.Close();
				}
			}

			return retorno;
		}

        private string EsquemaCredenciado
        {
            get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
        }

        private Dictionary<int, int> BuscaTipoDocOrigemCFOC(int origem, int Cultivar, BancoDeDados bancoDeDados = null)
        {
            int OrigemTipo = 0;
            Dictionary<int, int> lstTipoDocOrigem = new Dictionary<int, int>();

            using (BancoDeDados dbCon = BancoDeDados.ObterInstancia(EsquemaCredenciado))
            {

                do
                {
                    string cmdSql = @"select distinct origem_tipo, origem
                                      from tab_cfoc cfoc, tab_cfoc_produto cp, tab_lote_item hli
                                       where cfoc.id = cp.cfoc and cp.lote = hli.lote and cfoc.id = {0} and hli.cultivar = {1} ";

                    cmdSql = string.Format(cmdSql, origem, Cultivar);

                    Comando comandoCred = dbCon.CriarComando(cmdSql);


                    using (IDataReader reader = dbCon.ExecutarReader(comandoCred))
                    {
                        while (reader.Read())
                        {
                            OrigemTipo = reader.GetValue<int>("origem_tipo");
                            origem = reader.GetValue<int>("origem");
                            if (!lstTipoDocOrigem.ContainsKey(OrigemTipo))
                                lstTipoDocOrigem.Add(OrigemTipo, origem); 
                        }

                        reader.Close();
                    }

                } while (OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC);

            }

            return lstTipoDocOrigem;

        }

        internal string ObterDeclaracaoAdicionalPTVOutroEstado(int numero)
        {
            string ret = " ";
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select replace(declaracao_adicional,'|','')
																from {0}tab_ptv_outrouf tt
															  where tt.id = :id 
																 ", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", numero, DbType.Int32);

                ret = bancoDeDados.ExecutarScalar<string>(comando);
            }

            return string.IsNullOrEmpty(ret) ? " " : ret;

        }

		internal List<Cultivar> ObterCultivares(List<int> culturas, List<int> lotes= null, int OutroEstado = 0,  BancoDeDados banco = null)
		{
			List<Cultivar> lstCultivar = new List<Cultivar>();
            LoteDa _loteDa = new LoteDa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Cultivar

				Comando comando = bancoDeDados.CriarComando(@"select cc.id, cc.cultivar, cc.cultura from tab_cultura_cultivar cc ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarIn("where", "cultura", DbType.Int32, culturas);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Cultivar lista = null;

					while (reader.Read())
					{
						lista = new Cultivar();
						lista.Id = reader.GetValue<int>("id");
						lista.Nome = reader.GetValue<string>("cultivar");
						lista.CulturaId = reader.GetValue<int>("cultura");

						lstCultivar.Add(lista);
					}

					reader.Close();
				}

				#endregion

				#region Cultivar_Configurações
                StringBuilder sbDecAdd = new StringBuilder();
                List<LoteItem> lstLotesPtvOutro = new List<LoteItem>();
                List<LoteItem> lstLotesCfo = new List<LoteItem>();
                List<LoteItem> lstLotesCfoc = new List<LoteItem>();
                string cmdSql = "";
               // string strOutroEstado = "";
                if (lotes != null && lotes.Count > 0 && lotes[0] != 0)
                {
                    // strOutroEstado = "('0')";
                    foreach (int idLot in lotes)
                    {
                        Lote lot = _loteDa.Obter(idLot);
                        // totaloutro = lot.Lotes.Where(z => z.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado).ToList().Count;
                        lstLotesPtvOutro.AddRange(lot.Lotes.Where(z => z.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado).ToList());

                        lstLotesCfo.AddRange(lot.Lotes.Where(z => z.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO).ToList());

                        lstLotesCfoc.AddRange(lot.Lotes.Where(z => z.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC).ToList());
                        //foreach (LoteItem item in lstLotes)
                        //{
                        //    sbDecAdd.Append(ObterDeclaracaoAdicionalPTVOutroEstado(item.Origem));
                        //}

                        //if (lstLotes.Count > 0 && lstLotes.Count == lot.Lotes.Count)
                        //{
                        //    strOutroEstado = "('1')";
                        //}
                        //else if (lstLotes.Count > 0 && lstLotes.Count != lot.Lotes.Count)
                        //    strOutroEstado = "('1','0')";
                        //else
                        //    strOutroEstado = "('0')";
                    }
                }
                else
                {
                    cmdSql = string.Format(@" select t.id, t.tid, t.cultivar, t.praga PragaId, p.nome_cientifico || nvl2(p.nome_comum,' - '||p.nome_comum,'') as PragaTexto, t.tipo_producao TipoProducaoId,
				                                lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto, ld.texto_formatado as DeclaracaoAdicionalTextoHtml
				                                from {0}tab_cultivar_configuracao t, {0}tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld
				                                where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and t.cultivar = :id and
                                                ld.outro_estado = '0'", EsquemaBanco);
                }


               

                //else if (OutroEstado == 1)
                //    strOutroEstado = "('1')";
                //else
                //    strOutroEstado = "('0')";


//                string cmdSql = string.Format(@"select t.id, t.tid, t.cultivar, t.praga PragaId, p.nome_cientifico || nvl2(p.nome_comum,' - '||p.nome_comum,'') as PragaTexto, t.tipo_producao TipoProducaoId,
//				            lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto, ld.texto_formatado as DeclaracaoAdicionalTextoHtml
//				            from {0}tab_cultivar_configuracao t, {0}tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld, tab_ptv_outrouf_declaracao ot
//				            where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and t.cultivar = :id and ld.id = ot.declaracao_adicional(+)", EsquemaBanco, strOutroEstado);



                
                string strLotes = "";
                if ( (lstLotesCfo != null && lstLotesCfo.Count > 0) ||
                    (lstLotesCfoc != null && lstLotesCfoc.Count > 0))
                {
                    string strCultivares = "";
                    if (lstLotesCfo != null && lstLotesCfo.Count > 0)
                    {
                        strCultivares = string.Join(",", lstLotesCfo
                                         .Select(x => string.Format("'{0}'", x.Cultivar.ToString())));

                        cmdSql = string.Format(@" select t.id, t.tid, t.cultivar, t.praga PragaId, p.nome_cientifico || nvl2(p.nome_comum,' - '||p.nome_comum,'') as PragaTexto, t.tipo_producao TipoProducaoId,
				                              lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto, ld.texto_formatado as DeclaracaoAdicionalTextoHtml
				                              from {0}tab_cultivar_configuracao t, {0}tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld
				                              where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and t.cultivar in ({1}) and
                                              ld.outro_estado = '0'", EsquemaBanco, strCultivares);
                    
                    }
                    else if (lstLotesCfoc != null && lstLotesCfoc.Count > 0)
                    {
                        strCultivares = string.Join(",", lstLotesCfoc
                                         .Select(x => string.Format("'{0}'", x.Cultivar.ToString())));

                       
                        foreach (LoteItem ltItem in lstLotesCfoc)
                        {
                            Dictionary<int,int> lstTipoDocOrigem = BuscaTipoDocOrigemCFOC(ltItem.Origem, ltItem.Cultivar, bancoDeDados);


                            foreach (KeyValuePair<int, int> kv in lstTipoDocOrigem)
                            {
                                if (kv.Key == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)
                                {
                                    if (!string.IsNullOrEmpty(cmdSql))
                                        cmdSql += " union all ";

                                    cmdSql += string.Format(@" select t.id, t.tid, t.cultivar, t.praga PragaId, p.nome_cientifico || nvl2(p.nome_comum,' - '||p.nome_comum,'') as PragaTexto, t.tipo_producao TipoProducaoId,
				                                        lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto, ld.texto_formatado as DeclaracaoAdicionalTextoHtml
				                                        from tab_cultivar_configuracao t, tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld, tab_ptv_outrouf_declaracao ot
				                                        where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and t.cultivar = :id and ld.id = ot.declaracao_adicional
                                                        and ot.ptv = {1} ", EsquemaBanco, kv.Value );
                                }

                                if (kv.Key == (int)eDocumentoFitossanitarioTipo.CFO)
                                {
                                    if (!string.IsNullOrEmpty(cmdSql))
                                        cmdSql += " union all ";

                                    cmdSql += string.Format(@" select t.id, t.tid, t.cultivar, t.praga PragaId, p.nome_cientifico || nvl2(p.nome_comum,' - '||p.nome_comum,'') as PragaTexto, t.tipo_producao TipoProducaoId,
				                              lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto, ld.texto_formatado as DeclaracaoAdicionalTextoHtml
				                              from {0}tab_cultivar_configuracao t, {0}tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld
				                              where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and t.cultivar = :id and
                                              ld.outro_estado = '0'", EsquemaBanco);
                                }
                            }

                        }

                    }

                    

                }

                if (lstLotesPtvOutro != null && lstLotesPtvOutro.Count > 0)
                {

                    strLotes = string.Join(",", lstLotesPtvOutro
                                            .Select(x => string.Format("'{0}'", x.Origem.ToString())));

                    if (!string.IsNullOrEmpty(cmdSql))
                        cmdSql += " union all ";

                    cmdSql += string.Format(@" select t.id, t.tid, t.cultivar, t.praga PragaId, p.nome_cientifico || nvl2(p.nome_comum,' - '||p.nome_comum,'') as PragaTexto, t.tipo_producao TipoProducaoId,
				                                        lt.texto as TipoProducaoTexto, t.declaracao_adicional DeclaracaoAdicionalId, ld.texto as DeclaracaoAdicionalTexto, ld.texto_formatado as DeclaracaoAdicionalTextoHtml
				                                        from tab_cultivar_configuracao t, tab_praga p, lov_cultivar_tipo_producao lt, lov_cultivar_declara_adicional ld, tab_ptv_outrouf_declaracao ot
				                                        where p.id = t.praga and lt.id = t.tipo_producao and ld.id = t.declaracao_adicional and t.cultivar = :id and ld.id = ot.declaracao_adicional
                                                and ot.ptv in ({1})", EsquemaBanco, strLotes);

                }

                //ld.outro_estado in {1}

                if (string.IsNullOrEmpty(cmdSql))
                    return lstCultivar;

				comando = bancoDeDados.CriarComando(cmdSql, EsquemaBanco);

				lstCultivar.ForEach(x =>
				{

                    if (cmdSql.Contains(":")) 
					    comando.AdicionarParametroEntrada("id", x.Id, DbType.Int32);

                
                   
					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
                            //string Declaracao = "";

                            //if (lstLotes != null && lstLotes.Any(z => z.Cultivar == reader.GetValue<int>("cultivar")))
                            //    Declaracao = sbDecAdd.ToString();
                            //else
                            //    Declaracao = reader.GetValue<string>("DeclaracaoAdicionalTexto");

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
                                DeclaracaoAdicionalTexto = reader.GetValue<string>("DeclaracaoAdicionalTexto"),
                                DeclaracaoAdicionalTextoHtml = reader.GetValue<string>("DeclaracaoAdicionalTextoHtml")
							});
						}

						reader.Close();
					}
				});

				#endregion
			}

			return lstCultivar;
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
	}
}