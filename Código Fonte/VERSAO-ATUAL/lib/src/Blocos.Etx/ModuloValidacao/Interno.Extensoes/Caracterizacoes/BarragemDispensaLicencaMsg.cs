namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static BarragemDispensaLicencaMsg _barragemDispensaLicencaMsg = new BarragemDispensaLicencaMsg();
		public static BarragemDispensaLicencaMsg BarragemDispensaLicenca
		{
			get { return _barragemDispensaLicencaMsg; }
			set { _barragemDispensaLicencaMsg = value; }
		}
	}

    public class BarragemDispensaLicencaMsg
    {
        public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Barragem para Dispensa de Licença Ambiental salva com sucesso" }; } }
        public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Barragem para Dispensa de Licença Ambiental excluída com sucesso" }; } }
        public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de Barragem para Dispensa de Licença Ambiental deste empreendimento?" }; } }
        public Mensagem FormacaoRTOutros { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O IDAF (Instituto de Defesa Agropecuária e Florestal) informa que por orientação do CREA/ES, só são aptos a serem responsáveis técnicos de barragens de terra, os engenheiros agrônomos, engenheiros agrícolas e engenheiros civis, e apenas engenheiros civis no caso de barragens de concreto ou mista. Demais profissionais só serão aceitos com a apresentação de uma autorização específica do conselho de classe." }; } }

        public Mensagem SelecioneAtividade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Atividade", Texto = "Selecione a atividade." }; } }
        public Mensagem SelecioneTipoBarragem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TipoBarragem", Texto = "Selecione o tipo de barragem." }; } }
        public Mensagem SelecioneFinalidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "FinalidadeAtividade", Texto = "Defina a finalidade da barragem." }; } }
        public Mensagem InformeCursoHidrico { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CursoHidrico", Texto = "Informe o nome do curso hídrico." }; } }
        public Mensagem InformeVazaoInchenteZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "VazaoEnchente", Texto = "Informe a vazão de enchente(m³/s) maior que zero." }; } }
        public Mensagem InformeAreaBaciaContribuicaoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaBaciaContribuicao", Texto = "Informe a área da bacia de contribuição (ha) maior que zero." }; } }
        public Mensagem InformePrecipitacaoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Precipitacao", Texto = "Informe a intensidade máxima média de precipitação (im) (mm/h) maior que zero." }; } }
        public Mensagem InformePeriodoRetornoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PeriodoRetorno", Texto = "Informe o período de retorno (T) (anos) maior que zero." }; } }
        public Mensagem InformeCoeficienteEscoamentoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CoeficienteEscoamento", Texto = "Informe o coeficiente de escoamento (C) maior que zero." }; } }
        public Mensagem InformeTempoConcentracao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TempoConcentracao", Texto = "Informe o tempo de concentração (tc) (min)." }; } }
        public Mensagem InformeEquacaoCalculo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EquacaoCalculo", Texto = "Informe a equação utilizada no cálculo do tc." }; } }
        public Mensagem InformeAreaAlagadaZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaAlagada", Texto = "Informe a área alagada (ha) maior que zero." }; } }
        public Mensagem InformeVolumeArmazenadoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "VolumeArmazanado", Texto = "Informe o volume armazenado (m³) maior que zero." }; } }
        public Mensagem InformeFase { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Fase", Texto = "Informe a fase da barragem a ser feita a dispensa de licença." }; } }
        public Mensagem InformePossuiMonge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiMonge", Texto = "Informe se a barragem possui Monge." }; } }
        public Mensagem InformeTipoMonge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MongeTipo", Texto = "Informe o tipo de Monge da barragem." }; } }
        public Mensagem InformeEspecificacaoMonge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EspecificacaoMonge", Texto = "Informe a especificação do tipo de Monge da barragem." }; } }
        public Mensagem InformePossuiVertedouro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiVertedouro", Texto = "Informe se a barragem possui Vertedouro." }; } }
        public Mensagem InformeTipoVertedouro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "VertedouroTipo", Texto = "Informe o tipo de Vertedouro da barragem." }; } }
        public Mensagem InformeEspecificacaoVertedouro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EspecificacaoVertedouro", Texto = "Informe a especificação do tipo de Vertedouro da barragem." }; } }
        public Mensagem InformePossuiEstruturaHidraulica { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiEstruturaHidraulica", Texto = "Informe se as estruturas (monge e vertedouro) e o corpo do barramento estão funcionando de acordo com as normas de segurança." }; } }
        public Mensagem InformeAdequacoesRealizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AdequacoesRealizada", Texto = "Informe quais adequações serão realizadas para a barragem." }; } }

        public Mensagem InformeDataInicioObra { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataInicioObra", Texto = "Informe a data de início da obra (mês/ano)." }; } }
        public Mensagem InformeDataInicioObraFormatoValido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataInicioObra", Texto = "Informe a data de início da obra (mês/ano) em um formato correto." }; } }
        public Mensagem InformeDataPrevisaoTerminoObra { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataPrevisaoTerminoObra", Texto = "Informe a data de previsão de término da obra (mês/ano)." }; } }
        public Mensagem InformeDataPrevisaoTerminoObraFormatoValido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataPrevisaoTerminoObra", Texto = "Informe a data de previsão de término da obra (mês/ano) em um formato correto." }; } }
        public Mensagem PeriodoObraValido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataInicioObra, #DataPrevisaoTerminoObra", Texto = "A data de início da obra deve ser menor ou igual a data de previsão de término da obra." }; } }

        public Mensagem InformeCoordEasting { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EastingUtmTexto", Texto = "Informe a coordenada Easting da atividade." }; } }
        public Mensagem InformeCoordNorthing { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NorthingUtmTexto", Texto = "Informe a coordenada Northing da atividade." }; } }

        public Mensagem InformeFormacaoRT { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "FormacaoRT", Texto = "Informe a formação do responsável técnico." }; } }
        public Mensagem InformativoFormacaoRTOutro { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "EspecificacaoRT", Texto = "O IDAF (Instituto de Defesa Agropecuária e Florestal) informa que por orientação do CREA/ES, só são aptos a serem responsáveis técnicos de barragens de terra, os engenheiros agrônomos, engenheiros agrícolas e engenheiros civis, e apenas engenheiros civis no caso de barragens de concreto ou mista. Demais profissionais só serão aceitos com a apresentação de uma autorização específica do conselho de classe." }; } }
        public Mensagem InformeEspecificacaoRT { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EspecificacaoRT", Texto = "Informe a outra formação do responsável técnico." }; } }

        public Mensagem InformeArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivo_Nome, #ArquivoId, #Arquivo.Nome, #file", Texto = "Envie o arquivo de autorização do conselho de classe." }; } }
        public Mensagem InformeNumeroARTElaboracao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroARTElaboracao", Texto = "Informe o número da ART de elaboração do responsável técnico." }; } }

        public Mensagem CopiarCaractizacaoCadastrada { get { return new Mensagem() { Texto = "IDAF não possui Barragem para Dispensa de Licença Ambiental cadastrada.", Tipo = eTipoMensagem.Advertencia }; } }
    }
}