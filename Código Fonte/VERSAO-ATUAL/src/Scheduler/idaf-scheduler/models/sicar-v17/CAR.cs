using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class CAR
	{
		public Cadastrante cadastrante { get; set; }
		public Imovel imovel { get; set; }
		public List<ProprietariosPosseirosConcessionario> proprietariosPosseirosConcessionarios { get; set; }
		public List<Documento> documentos { get; set; }
		public List<Resposta> informacoes { get; set; }
		public Origem origem { get; set; }
		public string versao { get; set; }
		public List<Geo> geo { get; set; }

		public CAR()
		{
			versao = "1.9";
			origem = new Origem();
			cadastrante = new Cadastrante();
			imovel = new Imovel();
			proprietariosPosseirosConcessionarios = new List<ProprietariosPosseirosConcessionario>();
			documentos = new List<Documento>();

			informacoes = new List<Resposta>
			{
				new Resposta() {codigo = "DESEJA_ADERIR_PRA", respostas = new List<object>() {"Sim"}}, //{"Não Informado"}},
				new Resposta() {codigo = "POSSUI_DEFICIT_RL", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "POSSUI_DEFICIT_RL_REGULARIZAR", respostas = new List<object>()},
				new Resposta() {codigo = "POSSUI_DEFICIT_RL_COMPENSAR", respostas = new List<object>()},
				new Resposta() {codigo = "EXISTE_TAC", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "EXISTE_PRAD", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "EXISTE_INFRACAO", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "POSSUI_EXCEDENTE_VEGETACAO_NATIVA", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "POSSUI_EXCEDENTE_VEGETACAO_NATIVA_FAZER", respostas = new List<object>()},
				new Resposta() {codigo = "EXISTE_RPPN", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "EXISTE_RPPN_AREA", respostas = new List<object>() },
				new Resposta() {codigo = "EXISTE_RPPN_DATA_PUBLICACAO", respostas = new List<object>() },
				new Resposta() {codigo = "EXISTE_RPPN_NUMERO", respostas = new List<object>() },
				new Resposta() {codigo = "POSSUI_CRF", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "RL_TEMPORALIDADE", respostas = new List<object>() {"PERIODO_15_09_1965_A_17_07_1989"}},
				new Resposta() {codigo = "TAMANHO_ALTERADO_APOS_2008", respostas = new List<object>() {"Não Informado"}},
				new Resposta() {codigo = "TAMANHO_ALTERADO_APOS_2008_AREA", respostas = new List<object>() }
			};

			geo = new List<Geo>();
		}
	}
}
