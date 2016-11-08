using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using System.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data
{
    public class BarragemDispensaLicencaInternoDa
    {
        #region Propriedades

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		private String EsquemaBanco { get; set; }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		#endregion

        public BarragemDispensaLicencaInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter/Filtrar

		internal BarragemDispensaLicenca ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
            BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
                Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_barragem_dispensa_lic t where t.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

        internal BarragemDispensaLicenca Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
            BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.empreendimento, c.atividade, c.tipo_barragem, lt.texto tipo_barragem_texto, c.finalidade_atividade, c.curso_hidrico, 
				c.vazao_enchente, c.area_bacia_contribuicao, c.precipitacao, c.periodo_retorno, c.coeficiente_escoamento, c.tempo_concentracao, c.equacao_calculo, c.area_alagada, c.volume_armazenado, 
				c.fase, c.possui_monge, c.tipo_monge, c.especificacao_monge, c.possui_vertedouro, c.tipo_vertedouro, c.especificacao_vertedouro, c.possui_estrutura_hidrau, c.adequacoes_realizada, 
				c.data_inicio_obra, c.data_previsao_obra, c.easting, c.northing, c.formacao_resp_tec, c.especificacao_rt, c.autorizacao, c.numero_art_elaboracao, c.numero_art_execucao 
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt where lt.id = c.tipo_barragem and c.id = :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        caracterizacao.Id = reader.GetValue<int>("id");
                        caracterizacao.Tid = reader.GetValue<string>("tid");
                        caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento");
                        caracterizacao.AtividadeID = reader.GetValue<int>("atividade");
                        caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem");
                        caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
                        caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
                        caracterizacao.CursoHidrico = reader.GetValue<string>("curso_hidrico");
                        caracterizacao.VazaoEnchente = reader.GetValue<decimal?>("vazao_enchente");
                        caracterizacao.AreaBaciaContribuicao = reader.GetValue<decimal?>("area_bacia_contribuicao");
                        caracterizacao.Precipitacao = reader.GetValue<decimal?>("precipitacao");
                        caracterizacao.PeriodoRetorno = reader.GetValue<int?>("periodo_retorno");
                        caracterizacao.CoeficienteEscoamento = reader.GetValue<string>("coeficiente_escoamento");
                        caracterizacao.TempoConcentracao = reader.GetValue<string>("tempo_concentracao");
                        caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
                        caracterizacao.AreaAlagada = reader.GetValue<decimal?>("area_alagada");
                        caracterizacao.VolumeArmazanado = reader.GetValue<decimal?>("volume_armazenado");
                        caracterizacao.Fase = reader.GetValue<int?>("fase");
                        caracterizacao.PossuiMonge = reader.GetValue<int?>("possui_monge");
                        caracterizacao.MongeTipo = reader.GetValue<int>("tipo_monge");
                        caracterizacao.EspecificacaoMonge = reader.GetValue<string>("especificacao_monge");
                        caracterizacao.PossuiVertedouro = reader.GetValue<int?>("possui_vertedouro");
                        caracterizacao.VertedouroTipo = reader.GetValue<int>("tipo_vertedouro");
                        caracterizacao.EspecificacaoVertedouro = reader.GetValue<string>("especificacao_vertedouro");
                        caracterizacao.PossuiEstruturaHidraulica = reader.GetValue<int?>("possui_estrutura_hidrau");
                        caracterizacao.AdequacoesRealizada = reader.GetValue<string>("adequacoes_realizada");
                        caracterizacao.DataInicioObra = reader.GetValue<string>("data_inicio_obra");
                        caracterizacao.DataPrevisaoTerminoObra = reader.GetValue<string>("data_previsao_obra");
                        caracterizacao.Coordenada.EastingUtmTexto = reader.GetValue<string>("easting");
                        caracterizacao.Coordenada.NorthingUtmTexto = reader.GetValue<string>("northing");
                        caracterizacao.FormacaoRT = reader.GetValue<int>("formacao_resp_tec");
                        caracterizacao.EspecificacaoRT = reader.GetValue<string>("especificacao_rt");
                        caracterizacao.Autorizacao.Id = reader.GetValue<int>("autorizacao");
                        caracterizacao.NumeroARTElaboracao = reader.GetValue<string>("numero_art_elaboracao");
                        caracterizacao.NumeroARTExecucao = reader.GetValue<string>("numero_art_execucao");
                    }

                    reader.Close();
                }
            }

            return caracterizacao;
		}

		#endregion
    }
}
