using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Data
{
	public class RelatorioMapaDa
	{

		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		Consulta _consulta = new Consulta();
		internal Consulta Consulta { get { return _consulta; } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public RelatorioMapaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal RelatorioMapaFiltroeResultado RelatorioCFOCFOC(RelatorioMapaFiltroeResultado filtro)
		{
			RelatorioMapaFiltroeResultado relatorio = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{

				Comando comando = bancoDeDados.CriarComando(@"select id, data_emissao, data_, cultura_cultivar, tipo, num_CFO, num_CFOC, quantidade, unidade, unidade_origem, municipio
																from (select cfo.id,to_char(cfo.data_emissao,'DD/MM/YYYY') data_emissao, cfo.data_emissao data_,
																		     stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar, 'CFO' tipo, 
																			 cfo.numero num_CFO, null num_CFOC, cfopr.quantidade, um.texto unidade, up.codigo_up unidade_origem, mun.texto municipio
																		from {0}tab_cfo cfo, {0}tab_cfo_produto cfopr, {0}crt_unidade_producao_unidade up, 
																			 {0}tab_cultura_cultivar cc, {0}tab_cultura c, {0}lov_crt_uni_prod_uni_medida um, {0}lov_municipio mun
																		where cfopr.cfo = cfo.id and up.id = cfopr.unidade_producao and cc.id = up.cultivar and 
																			  c.id = cc.cultura and um.id(+) = up.estimativa_unid_medida and mun.id = cfo.municipio_emissao and 
																			  cfo.situacao in (2, 3) and data_emissao >= :datainicial and data_emissao <= :datafinal
																		group by cfo.id, cfo.numero, cfo.data_emissao, cfopr.quantidade, um.texto, up.codigo_up, mun.texto
																		order by cfo.data_emissao)
															union all (select cfoc.id, to_char(cfoc.data_emissao,'DD/MM/YYYY') data_emissao, cfoc.data_emissao data_,
																			  stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar, 'CFOC' tipo,
																		     null num_CFO, cfoc.numero num_CFOC, li.quantidade, um.texto unidade, l.codigo_uc unidade_origem, mun.texto municipio
																			from {0}tab_cfoc cfoc, {0}tab_cfoc_produto cfocpr, {0}tab_lote l, {0}tab_lote_item li, 
																				 {0}tab_cultura_cultivar cc, {0}tab_cultura c, {0}lov_crt_uni_prod_uni_medida um, {0}lov_municipio mun
																		where cfocpr.cfoc = cfoc.id and l.id = cfocpr.lote and li.lote = l.id and cc.id = li.cultivar and 
																			  c.id = cc.cultura and um.id(+) = li.unidade_medida and mun.id = cfoc.municipio_emissao and 
																			  cfoc.situacao in (2, 3) and data_emissao >= :datainicial and data_emissao <= :datafinal
																		group by cfoc.id, cfoc.data_emissao, cfoc.numero, li.quantidade, um.texto, l.codigo_uc, mun.texto)
																order by data_", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("datainicial", filtro.DataInicial, DbType.String);
				comando.AdicionarParametroEntrada("datafinal", filtro.DataFinal, DbType.String);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					relatorio = new RelatorioMapaFiltroeResultado();
					ItemRelatorioMapaCFOCFOC item;
					while (reader.Read())
					{
						item = new ItemRelatorioMapaCFOCFOC();	
						item.DataEmissao = reader.GetValue<string>("data_emissao");
						item.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						item.Tipo = reader.GetValue<string>("tipo");
						item.NumeroCFO = reader.GetValue<string>("num_CFO");
						item.NumeroCFOC = reader.GetValue<string>("num_CFOC");
						item.Quantidade = reader.GetValue<decimal>("quantidade");
						item.UnidadeMedida = reader.GetValue<string>("unidade");
						item.UnidadeOrigem = reader.GetValue<string>("unidade_origem");
						item.Municipio = reader.GetValue<string>("municipio");
						
						relatorio.ItensRelatorioMapaCFOCFOC.Add(item);

					}
					reader.Close();
				}
			}


			return relatorio;
		}

		internal RelatorioMapaFiltroeResultado RelatorioPTV(RelatorioMapaFiltroeResultado filtro)
		{
			RelatorioMapaFiltroeResultado relatorio = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{

				Comando comando = bancoDeDados.CriarComando(@"select pt.id ptv_id, to_char(pt.data_emissao,'DD/MM/YYYY') data_emissao, pt.numero, pt.situacao, stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar, 
																	pr.quantidade, um.texto unidade, des.nome destinatario, est.sigla uf_estado, mun.texto municipio
																from {0}tab_ptv pt, {0}tab_ptv_produto pr, {0}tab_cultura c, {0}tab_cultura_cultivar cc, {0}lov_crt_uni_prod_uni_medida um, 
																   {0}tab_destinatario_ptv des, {0}lov_estado est, {0}lov_municipio mun
																where pt.id (+) = pr.ptv and c.id = pr.cultura and cc.id = pr.cultivar and um.id = pr.unidade_medida
																	and est.id=des.uf and mun.id=des.municipio and des.id = pt.destinatario and est.id=des.uf and mun.id=des.municipio
																	and pt.situacao in (2,3,4) 
																	and pt.data_emissao >= :datainicial and pt.data_emissao <= :datafinal
																group by pt.id, pt.data_emissao, pt.numero, pt.situacao, pr.quantidade, um.texto, des.nome, est.sigla, 
																	 mun.texto order by pt.data_emissao", EsquemaBanco);
				comando.AdicionarParametroEntrada("datainicial", filtro.DataInicial, DbType.String);
				comando.AdicionarParametroEntrada("datafinal", filtro.DataFinal, DbType.String);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					relatorio = new RelatorioMapaFiltroeResultado();
					ItemRelatorioMapaPTV item;
					while (reader.Read())
					{
						item = new ItemRelatorioMapaPTV();
						item.DataEmissao = reader.GetValue<string>("data_emissao");
						item.NumeroPTV = reader.GetValue<string>("numero");
						item.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						item.Quantidade = reader.GetValue<decimal>("quantidade");
						item.UnidadeMedida = reader.GetValue<string>("unidade");
						item.DestinatarioNome = reader.GetValue<string>("destinatario");
						item.DestinatarioEstado = reader.GetValue<string>("uf_estado");
						item.DestinatarioMunicipio = reader.GetValue<string>("municipio");

						relatorio.ItensRelatorioMapaPTV.Add(item);

					}
					reader.Close();
				}
			}


			return relatorio;
		}

// PTV
//select pt.id ptv_id, pt.data_emissao, pt.numero, pt.situacao, stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar, pr.quantidade, um.texto
//from ins_ptv pt, ins_ptv_produto pr, tab_cultura c, tab_cultura_cultivar cc, lov_crt_uni_prod_uni_medida um
//where pt.id (+) = pr.ptv and c.id = pr.cultura and cc.id = pr.cultivar and um.id = pr.unidade_medida and pt.situacao != 1 
//and pt.data_emissao >= '01/01/2015' and pt.data_emissao <= '31/12/2015'
//group by pt.id, pt.data_emissao, pt.numero, pt.situacao, pr.quantidade, um.texto


//select id, data_emissao, cultura_cultivar, tipo, num_CFO, num_CFOC, quantidade, unidade, unidade_origem, municipio
// from
//  (select cfo.id, cfo.data_emissao, stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar, 'CFO' tipo, cfo.numero num_CFO, null num_CFOC, 
//cfopr.quantidade, um.texto unidade, up.codigo_up unidade_origem, mun.texto municipio
//	   from tab_cfo cfo, tab_cfo_produto cfopr, crt_unidade_producao_unidade up, tab_cultura_cultivar cc, tab_cultura c, 
//	   lov_crt_uni_prod_uni_medida um,  lov_municipio mun
//where cfopr.cfo = cfo.id and up.id = cfopr.unidade_producao and cc.id = up.cultivar and c.id = cc.cultura and um.id (+) = up.estimativa_unid_medida 
//	  and mun.id = cfo.municipio_emissao and cfo.situacao in (2,3) and data_emissao >= '01/11/2015' and data_emissao <= '31/12/2015'
//group by cfo.id, cfo.numero, cfo.data_emissao, cfopr.quantidade, um.texto, up.codigo_up, mun.texto order by cfo.data_emissao)
//union all
//(select cfoc.id, cfoc.data_emissao, stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar, 'CFOC' tipo, null num_CFO, 
//cfoc.numero num_CFOC,  li.quantidade, um.texto unidade, l.codigo_uc unidade_origem, mun.texto municipio
//from tab_cfoc cfoc, tab_cfoc_produto cfocpr, tab_lote l, tab_lote_item li, tab_cultura_cultivar cc, tab_cultura c, 
//lov_crt_uni_prod_uni_medida um,  lov_municipio mun
//where cfocpr.cfoc = cfoc.id and l.id = cfocpr.lote and li.lote = l.id and cc.id = li.cultivar and c.id = cc.cultura and um.id (+) = li.unidade_medida
//and mun.id = cfoc.municipio_emissao and cfoc.situacao in (2,3) and data_emissao >= '01/11/2015' and data_emissao <= '31/12/2015'
//group by cfoc.id, cfoc.data_emissao, cfoc.numero, li.quantidade, um.texto, l.codigo_uc, mun.texto) 
//order by data_emissao


	}
}
