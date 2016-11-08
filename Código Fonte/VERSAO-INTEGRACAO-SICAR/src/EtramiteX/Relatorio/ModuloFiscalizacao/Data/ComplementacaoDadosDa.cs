using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class ComplementacaoDadosDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }
		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		public ComplementacaoDadosDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		public ComplementacaoDadosRelatorio Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			ComplementacaoDadosRelatorio objeto = new ComplementacaoDadosRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select (case when v.id = 9/*Outro*/ then c.vinc_prop_especif_text else v.texto
													end) VinculoPropriedade, r.texto ResideResidencia, rm.texto RendaMensal, n.texto NivelEscolaridade,
													cl.texto ConhecimentoLegislacao, c.conhec_legisl_justif_text ConhecimentoLegislacaoJustifi,
													c.prop_area_total AreaInformada, c.prop_area_cobert_flores_nativ AreaFlorestaNativa,
													(select stragg(b.texto) from {0}lov_fisc_compl_dad_reserva_leg b where bitand(c.prop_area_reserv_legal,
													b.codigo) != 0) AreaReservaLegal from {0}tab_fisc_compl_dados_aut c, {0}lov_empreendimento_tipo_resp v,
													{0}lov_fisc_compl_dad_resp_padrao r, {0}lov_fisc_compl_dad_rend_mensal rm, {0}lov_fisc_compl_dad_nivel_escol n,
													{0}lov_fisc_compl_dad_resp_padrao cl, {0}lov_fisc_compl_dad_reserva_leg where c.vinc_prop = v.id(+)
													and c.reside_propriedade = r.id(+) and c.renda_mensal_familiar = rm.id(+) and c.nivel_escolaridade = n.id(+)
													and c.conhec_legisl = cl.id(+) and c.fiscalizacao = :fiscalizacaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<ComplementacaoDadosRelatorio>(comando);
			}

			return objeto;
		}

		public ComplementacaoDadosRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
		{
			ComplementacaoDadosRelatorio objeto = new ComplementacaoDadosRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select (case when v.id = 9/*Outro*/ then c.vinc_prop_especif_text else v.texto
				  end) VinculoPropriedade, r.texto ResideResidencia, rm.texto RendaMensal, n.texto NivelEscolaridade,
				  cl.texto ConhecimentoLegislacao, c.conhec_legisl_justif_text ConhecimentoLegislacaoJustifi,
				  c.prop_area_total AreaInformada, c.prop_area_cobert_flores_nativ AreaFlorestaNativa,
				  (select stragg(b.texto) from {0}lov_fisc_compl_dad_reserva_leg b where bitand(c.prop_area_reserv_legal,
				  b.codigo) != 0) AreaReservaLegal   
				  from {0}hst_fisc_compl_dados_aut c,   
				  {0}lov_empreendimento_tipo_resp v,
				  {0}lov_fisc_compl_dad_resp_padrao r, 
				  {0}lov_fisc_compl_dad_rend_mensal rm, 
				  {0}lov_fisc_compl_dad_nivel_escol n,
				  {0}lov_fisc_compl_dad_resp_padrao cl, 
				  {0}lov_fisc_compl_dad_reserva_leg   
				  where c.vinc_prop = v.id(+)
				  and c.reside_propriedade = r.id(+) 
				  and c.renda_mensal_familiar = rm.id(+) 
				  and c.nivel_escolaridade = n.id(+)
				  and c.conhec_legisl = cl.id(+) 
				  and c.fiscalizacao_id_hst = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<ComplementacaoDadosRelatorio>(comando);
			}

			return objeto;
		}

		#endregion
	}
}
