using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloPTV.Data
{
    public class PTVDa
    {
        #region Propriedades

        GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

        public string UsuarioCredenciado
        {
            get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
        }

        public string UsuarioInterno
        {
            get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
        }

        private String EsquemaBanco { get; set; }

        #endregion

		internal Resultados<PTVListarResultado> Filtrar(Filtro<PTVListarFiltro> filtro)
		{
			Resultados<PTVListarResultado> retorno = new Resultados<PTVListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("pt.numero", "numero", filtro.Dados.Numero);

				if (!String.IsNullOrEmpty(filtro.Dados.Empreendimento))
				{
					comandtxt += comando.FiltroAndLike("em.denominador", "nome_fantasia", filtro.Dados.Empreendimento, true, true);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.Destinatario))
				{
					comandtxt += comando.FiltroAndLike("d.nome", "nome", filtro.Dados.Destinatario, true, true);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.CulturaCultivar))
				{
					comandtxt += comando.FiltroAndLike("c.texto||'/'||cc.cultivar", "cultura_cultivar", filtro.Dados.CulturaCultivar, true, true);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "empreendimento", "cultura_cultivar", "tipo", "situacao" };

				if (filtro.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText =
				"select count(*) from (" + String.Format(@"(select pt.id
															   from ins_ptv              pt,
																	ins_ptv_produto      pr,
																	ins_empreendimento   em,
																	lov_ptv_situacao     st,
																	tab_cultura          c,
																	tab_cultura_cultivar cc,
																	ins_destinatario_ptv d
															  where pt.id(+) = pr.ptv
																and em.id = pt.empreendimento
																and st.id = pt.situacao
																and c.id = pr.cultura
																and cc.id = pr.cultivar
																and st.id != 1
																and d.id = pt.destinatario " + comandtxt + @"
															  group by pt.id,
																	   pt.numero,
																	   pt.tipo_numero,
																	   em.denominador,
																	   pt.situacao,
																	   st.texto,
																	   pt.responsavel_tecnico) union all
															(select pt.id
															   from tab_ptv                      pt,
																	tab_ptv_produto              pr,
																	ins_empreendimento           em,
																	lov_solicitacao_ptv_situacao sts,
																	tab_cultura                  c,
																	tab_cultura_cultivar         cc,
																	tab_destinatario_ptv         d
															  where pt.id(+) = pr.ptv
																and em.id = pt.empreendimento
																and sts.id = pt.situacao
																and c.id = pr.cultura
																and cc.id = pr.cultivar
																and sts.id not in (1, 3)
																and d.id = pt.destinatario " + comandtxt + @"
															  group by pt.id,
																	   pt.numero,
																	   pt.tipo_numero,
																	   em.denominador,
																	   pt.situacao,
																	   sts.texto,
																	   pt.responsavel_tecnico)) a ", UsuarioCredenciado);

//				comando.DbCommand.CommandText =
//				"select count(*) from (" + String.Format(@"select pt.id
//														from {0}tab_ptv pt,{0}tab_ptv_produto pr,{0}tab_empreendimento em,{0}lov_ptv_situacao st,{0}tab_cultura c,{0}tab_cultura_cultivar cc,{0}tab_destinatario_ptv d
//														where pt.id(+) = pr.ptv
//															and em.id = pt.empreendimento 
//															and st.id = pt.situacao 
//															and c.id = pr.cultura
//															and cc.id = pr.cultivar 
//															and d.id = pt.destinatario " + comandtxt + " group by pt.id) a ", UsuarioCredenciado);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				comandtxt = String.Format(@"select id,
												   numero,
												   tipo_numero,
												   empreendimento,
												   situacao,
												   situacao_texto,
												   tipo,
												   cultura_cultivar
											  from ((select pt.id,
															pt.numero,
															pt.tipo_numero,
															em.denominador as empreendimento,
															pt.situacao,
															st.texto as situacao_texto,
															'PTV' as tipo,
															stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar
													   from ins_ptv              pt,
															ins_ptv_produto      pr,
															ins_empreendimento   em,
															lov_ptv_situacao     st,
															tab_cultura          c,
															tab_cultura_cultivar cc,
															ins_destinatario_ptv d
													  where pt.id(+) = pr.ptv
														and em.id = pt.empreendimento
														and st.id = pt.situacao
														and c.id = pr.cultura
														and cc.id = pr.cultivar
														and st.id != 1
														and d.id = pt.destinatario " + comandtxt + @"
													  group by pt.id,
															   pt.numero,
															   pt.tipo_numero,
															   em.denominador,
															   pt.situacao,
															   st.texto,
															   pt.responsavel_tecnico) union all
													(select pt.id,
															pt.numero,
															pt.tipo_numero,
															em.denominador as empreendimento,
															pt.situacao,
															sts.texto as situacao_texto,
															'EPTV' as tipo,
															stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar
													   from tab_ptv                      pt,
															tab_ptv_produto              pr,
															ins_empreendimento           em,
															lov_solicitacao_ptv_situacao sts,
															tab_cultura                  c,
															tab_cultura_cultivar         cc,
															tab_destinatario_ptv         d
													  where pt.id(+) = pr.ptv
														and em.id = pt.empreendimento
														and sts.id = pt.situacao
														and c.id = pr.cultura
														and cc.id = pr.cultivar
														and sts.id not in (1, 3)
														and d.id = pt.destinatario " + comandtxt + @"
													  group by pt.id,
															   pt.numero,
															   pt.tipo_numero,
															   em.denominador,
															   pt.situacao,
															   sts.texto,
															   pt.responsavel_tecnico)) " + DaHelper.Ordenar(colunas, ordenar), UsuarioCredenciado);

//				comandtxt = String.Format(@"select 
//												pt.id,
//												pt.numero,
//												pt.tipo_numero,
//												em.denominador as empreendimento,
//											    pt.situacao,
//												st.texto as situacao_texto,
//												pt.responsavel_tecnico,
//												stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar
//											from {0}tab_ptv pt, {0}tab_ptv_produto pr, {0}tab_empreendimento em, {0}lov_ptv_situacao st, {0}tab_cultura c, {0}tab_cultura_cultivar cc,{0}tab_destinatario_ptv d
//											where pt.id(+) = pr.ptv
//											  and em.id = pt.empreendimento
//											  and st.id = pt.situacao
//											  and c.id = pr.cultura
//											  and cc.id = pr.cultivar 
//										      and d.id = pt.destinatario " + comandtxt + " group by pt.id, pt.numero, pt.tipo_numero, em.denominador, pt.situacao, st.texto, pt.responsavel_tecnico " + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);
				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PTVListarResultado item;
					while (reader.Read())
					{
						item = new PTVListarResultado();
						item.ID = reader.GetValue<int>("id");
						item.Numero = reader.GetValue<string>("numero");
						item.NumeroTipo = reader.GetValue<int>("tipo_numero");
						item.Empreendimento = reader.GetValue<string>("empreendimento");
						item.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						item.Situacao = reader.GetValue<int>("situacao");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.Tipo = reader.GetValue<string>("tipo");

						retorno.Itens.Add(item);
					}
					reader.Close();
				}
			}
			return retorno;
		}

    }
}
