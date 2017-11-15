using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class MaterialApreendidoDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }

		#endregion

		public MaterialApreendidoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		public MaterialApreendidoRelatorio Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			MaterialApreendidoRelatorio objeto = new MaterialApreendidoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select m.id Id, m.tad_gerado IsGeradoSistema, (case m.houve_material when 1 then 'Sim' when 0 then 'Não' end) IsApreendido, nvl(m.tad_numero, f.autos) NumeroTAD, ls.texto SerieTexto,
					TO_CHAR(m.tad_data, 'DD/MM/YYYY') DataLavraturaTAD, m.descricao DescreverApreensao, m.opiniao OpinarDestino from {0}tab_fisc_material_apreendido m, lov_fiscalizacao_serie ls,
					tab_fiscalizacao f where m.fiscalizacao = f.id and ls.id = m.serie(+) and f.id = :fiscalizacaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<MaterialApreendidoRelatorio>(comando);
				objeto.Materiais = ObterMateriais(objeto.Id);
				objeto.SerieTexto = String.IsNullOrWhiteSpace(objeto.NumeroTAD) ? String.Empty : objeto.SerieTexto;

				if (objeto.Materiais == null || objeto.Materiais.Count == 0)
				{
					objeto.Materiais = new List<MaterialApreendidoMaterialRelatorio>();
					objeto.Materiais.Add(new MaterialApreendidoMaterialRelatorio());
				}

				if (objeto.DataLavraturaTAD == null)
				{
					objeto.DataLavraturaTAD = new FiscalizacaoDa().ObterDataConclusao(fiscalizacaoId, bancoDeDados).DataTexto;
				}

				if (!objeto.IsGeradoSistema.HasValue)
				{
					objeto.NumeroTAD = null;
					objeto.DataLavraturaTAD = null;
				}

				if (String.IsNullOrEmpty(objeto.IsApreendido))
				{
					objeto.IsApreendido = "Não";
				}
			}

			return objeto;
		}

        public MaterialApreendidoRelatorioNovo ObterNovo(int fiscalizacaoId, BancoDeDados banco = null)
        {
            MaterialApreendidoRelatorioNovo objeto = new MaterialApreendidoRelatorioNovo();
            Comando comando = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                comando = bancoDeDados.CriarComando(@"select tfa.id Id,
                                                             Tfa.Iuf_Numero NumeroIUF,
                                                             Lfs.Texto SerieTexto,
                                                             to_char(tfa.iuf_data, 'DD/MM/YYYY') DataLavraturaIUF,
                                                             Tfa.Descricao DescreverApreensao,
                                                             Tfa.Opiniao OpinarDestino
                                                      from {0}Tab_Fisc_Apreensao tfa,
                                                           {0}lov_fiscalizacao_serie lfs
                                                      where (tfa.serie is null or lfs.id = tfa.serie)
                                                            and tfa.Fiscalizacao = :fiscalizacaoId", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

                objeto = bancoDeDados.ObterEntity<MaterialApreendidoRelatorioNovo>(comando);
                objeto.ProdutosDestinacoes = ObterProdutosDestinacao(objeto.Id);
                objeto.SerieTexto = String.IsNullOrWhiteSpace(objeto.NumeroIUF) ? String.Empty : objeto.SerieTexto;              
            }

            return objeto;
        }

		public MaterialApreendidoRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
		{
			MaterialApreendidoRelatorio objeto = new MaterialApreendidoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"
					select m.id HistoricoId, m.material_apreendido_id Id, m.tad_gerado IsGeradoSistema, 
					(case m.houve_material when 1 then 'Sim' when 0 then 'Não' end) IsApreendido, 
							   nvl(m.tad_numero, f.autos) NumeroTAD, ls.texto SerieTexto, 
							   TO_CHAR(nvl(m.tad_data, f.situacao_data), 'DD/MM/YYYY') DataLavraturaTAD, 
							   m.descricao DescreverApreensao, m.opiniao OpinarDestino           
							from {0}hst_fisc_material_apreendido m, 
							{0}hst_fiscalizacao f,
							{0}lov_fiscalizacao_serie ls
							where m.fiscalizacao_id_hst = f.id 
							and ls.id = m.serie_id
							and f.id = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<MaterialApreendidoRelatorio>(comando);
				objeto.Materiais = ObterMateriaisHistorico(objeto.HistoricoId);
				objeto.SerieTexto = String.IsNullOrWhiteSpace(objeto.NumeroTAD) ? String.Empty : objeto.SerieTexto;

				if (objeto.Materiais == null || objeto.Materiais.Count == 0)
				{
					objeto.Materiais = new List<MaterialApreendidoMaterialRelatorio>();
					objeto.Materiais.Add(new MaterialApreendidoMaterialRelatorio());
				}

				if (!objeto.IsGeradoSistema.HasValue)
				{
					objeto.NumeroTAD = null;
					objeto.DataLavraturaTAD = null;
				}

				if (String.IsNullOrEmpty(objeto.IsApreendido))
				{
					objeto.IsApreendido = "Não";
				}
			}

			return objeto;
		}

		private List<MaterialApreendidoMaterialRelatorio> ObterMateriais(int materialApreendidoId, BancoDeDados banco = null)
		{
			List<MaterialApreendidoMaterialRelatorio> colecao = new List<MaterialApreendidoMaterialRelatorio>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select t.texto Tipo, m.especificacao Especificacao from {0}tab_fisc_mater_apree_material m, {0}lov_fisc_mate_apreendido_tipo t
					where m.tipo = t.id and m.material_apreendido = :materialApreendidoId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("materialApreendidoId", materialApreendidoId, DbType.Int32);

				colecao = bancoDeDados.ObterEntityList<MaterialApreendidoMaterialRelatorio>(comando);
			}

			return colecao;
		}

        private List<ApreensaoProdutoDestinacaoRelatorio> ObterProdutosDestinacao(int materialApreendidoId, BancoDeDados banco = null)
        {
            List<ApreensaoProdutoDestinacaoRelatorio> colecao = new List<ApreensaoProdutoDestinacaoRelatorio>();
            Comando comando = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                comando = bancoDeDados.CriarComando(@"select Cfip.Item Produto,
                                                             Cfip.Unidade Unidade,
                                                             Tfap.Quantidade,
                                                             Cfid.Destino Destino
                                                      from {0}Tab_Fisc_Apreensao_Produto tfap,
                                                           {0}Cnf_Fisc_Infracao_Produto cfip,
                                                           {0}Cnf_Fisc_Infr_Destinacao cfid
                                                      where Tfap.Produto = cfip.id
                                                            and Tfap.Destinacao = cfid.id
                                                            and Tfap.Apreensao = :apreensaoId", EsquemaBanco);

                comando.AdicionarParametroEntrada("apreensaoId", materialApreendidoId, DbType.Int32);

                colecao = bancoDeDados.ObterEntityList<ApreensaoProdutoDestinacaoRelatorio>(comando);

                for (int i = 0; i < colecao.Count; i++)
                {
                    colecao[i].Item = (i + 1).ToString();
                }
            }

            return colecao;
        }

		private List<MaterialApreendidoMaterialRelatorio> ObterMateriaisHistorico(int historicoMaterialApreendidoId, BancoDeDados banco = null)
		{
			List<MaterialApreendidoMaterialRelatorio> colecao = new List<MaterialApreendidoMaterialRelatorio>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select m.tipo_texto Tipo, m.especificacao Especificacao 
					from hst_fisc_mater_apree_material m
					where m.material_apreendido_id_hst = :historicoMaterialApreendidoId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoMaterialApreendidoId", historicoMaterialApreendidoId, DbType.Int32);

				colecao = bancoDeDados.ObterEntityList<MaterialApreendidoMaterialRelatorio>(comando);
			}

			return colecao;
		}

		#endregion
	}
}
