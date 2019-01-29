﻿namespace Tecnomapas.Blocos.Etx.ModuloValidacao
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
        public Mensagem InformeVazaoInchenteZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "VazaoEnchente", Texto = "Informe a vazão máxima de cheia maior que zero." }; } }
        public Mensagem InformeAreaBaciaContribuicaoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaBaciaContribuicao", Texto = "Informe a área da bacia de contribuição (ha) maior que zero." }; } }
        public Mensagem InformePrecipitacaoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Precipitacao", Texto = "Informe a intensidade máxima média de precipitação (im) (mm/h) maior que zero." }; } }
        public Mensagem InformePrecipitacaoFonteDados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Precipitacao", Texto = "Informe a font de dados da intensidade máxima média de precipitação (im) (mm/h)." }; } }
        public Mensagem InformePeriodoRetornoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PeriodoRetorno", Texto = "Informe o período de retorno (T) (anos) maior que zero." }; } }
        public Mensagem InformeCoeficienteEscoamentoZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CoeficienteEscoamento", Texto = "Informe o coeficiente de escoamento (C) maior que zero." }; } }
        public Mensagem InformeCoeficienteEscoamentoZeroFonteDados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CoeficienteEscoamento", Texto = "Informe a fonte de dados do coeficiente de escoamento (C)." }; } }
        public Mensagem InformeTempoConcentracao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TempoConcentracao", Texto = "Informe o tempo de concentração (tc) (min)." }; } }
        public Mensagem InformeEquacaoCalculo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EquacaoCalculo", Texto = "Informe a equação utilizada no cálculo do tc." }; } }
        public Mensagem InformeAreaAlagadaZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaAlagada", Texto = "Informe a área alagada (ha) válida." }; } }
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

		public Mensagem InformeCoordEasting(string tipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Informe a coordenada easting do {0}.", tipo) }; }
		public Mensagem InformeCoordNorthing(string tipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Informe a coordenada northing do {0}.", tipo) }; }

		public Mensagem InformeFormacaoRT { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "FormacaoRT", Texto = "Informe a formação do responsável técnico." }; } }
        public Mensagem InformativoFormacaoRTOutro { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "EspecificacaoRT", Texto = "O IDAF (Instituto de Defesa Agropecuária e Florestal) informa que por orientação do CREA/ES, só são aptos a serem responsáveis técnicos de barragens de terra, os engenheiros agrônomos, engenheiros agrícolas e engenheiros civis, e apenas engenheiros civis no caso de barragens de concreto ou mista. Demais profissionais só serão aceitos com a apresentação de uma autorização específica do conselho de classe." }; } }
        public Mensagem InformeEspecificacaoRT { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EspecificacaoRT", Texto = "Informe a outra formação do responsável técnico." }; } }

        public Mensagem InformeArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivo_Nome, #ArquivoId, #Arquivo.Nome, #file", Texto = "Envie o arquivo de autorização do conselho de classe." }; } }
        public Mensagem InformeNumeroARTElaboracao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroARTElaboracao", Texto = "Informe o número da ART de elaboração do responsável técnico." }; } }

        public Mensagem CopiarCaractizacaoCadastrada { get { return new Mensagem() { Texto = "IDAF não possui Barragem para Dispensa de Licença Ambiental cadastrada.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem InformeAlturaBarramento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AlturaBarramento", Texto = "Informe a altura do barramento maior que zero." }; } }
		public Mensagem InformeComprimentoBarramento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ComprimentoBarramento", Texto = "Informe a comprimento do barramento maior que zero." }; } }
		public Mensagem InformeLarguraBaseBarramento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LarguraBaseBarramento", Texto = "Informe a largura base do barramento maior que zero." }; } }
		public Mensagem InformeLarguraCristaBarramento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LarguraCristaBarramento", Texto = "Informe a largura da crista do barramento maior que zero." }; } }
		public Mensagem InformeEquacaoTempo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TempoConcentracao", Texto = "Informe a equação utilizada no tempo de concentração." }; } }
		public Mensagem InformeLarguraDemarcada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "larguraDemarcada", Texto = "Informe a argura demarcada maior que zero." }; } }
		public Mensagem InformeFaixaCercada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "faixaCercada", Texto = "Informe se a faixa demarcada está cercada." }; } }
		public Mensagem InformeDescricaoApp { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "descricaoApp", Texto = "Informe a descrição do estágio de desenvolvimento." }; } }
		public Mensagem InformeBarramentoNormas { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "barramentoNormas", Texto = "Informe se o barramento está dentro das normas técnicas." }; } }
		public Mensagem InformeBarramentoAdequacoes { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "barramentoNormas", Texto = "Informe as adequações do barramento." }; } }
		public Mensagem InformeSupressaoApp { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "barramentoNormas", Texto = "Informe as adequações do barramento." }; } }
		public Mensagem InformeMesInicio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MesInicio", Texto = "Informe o mês de início." }; } }
		public Mensagem MesInicioInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MesInicio", Texto = "Mês de início inválido." }; } }
		public Mensagem InformeAnoInicio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MesInicio", Texto = "Informe o ano de início." }; } }
		public Mensagem AnoInicioInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MesInicio", Texto = "Ano de início inválido." }; } }

		public Mensagem InformeVazaoMinTipo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMinTipo", Texto = "Informe o tipo de dispositivo de vazão mínima." }; } }
		public Mensagem InformeVazaoMinDiametro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMinDiametro", Texto = "Informe o diâmetro do dispositivo de vazão mínima." }; } }
		public Mensagem InformeVazaoMinInstalado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMinInstalado", Texto = "Informe se o dispositivo de vazão mínima já está instalado." }; } }
		public Mensagem InformeVazaoMinNormas { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMinNormas", Texto = "Informe se o dispositivo de vazão mínima está dentro das normas." }; } }
		public Mensagem InformeVazaoMinAdequacoes { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMinAdequacoes", Texto = "Informe as adequações do dispositivo de vazão mínima." }; } }
		public Mensagem InformeVazaoMaxTipo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMaxTipo", Texto = "Informe o tipo de dispositivo de vazão máxima." }; } }
		public Mensagem InformeVazaoMaxDiametro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMaxDiametro", Texto = "Informe o diâmetro do dispositivo de vazão máxima." }; } }
		public Mensagem InformeVazaoMaxInstalado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMaxInstalado", Texto = "Informe se o dispositivo de vazão máxima já está instalado." }; } }
		public Mensagem InformeVazaoMaxNormas { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMaxNormas", Texto = "Informe se o dispositivo de vazão máxima está dentro das normas." }; } }
		public Mensagem InformeVazaoMaxAdequacoes { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vazaoMaxAdequacoes", Texto = "Informe as adequações do dispositivo de vazão máxima." }; } }

		public Mensagem InformeNomeRT(string tipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Informe o nome do responsável técnico {0}.", tipo) }; }
		public Mensagem InformeProfissapRT(string tipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Informe a profissão do responsável técnico {0}.", tipo) }; }
		public Mensagem InformeCREART(string tipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Informe o registro do CREA do responsável técnico {0}.", tipo) }; }
		public Mensagem InformeNumeroART(string tipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Informe o número do ART do responsável técnico {0}.", tipo) }; }
		public Mensagem InformeAutorizacaoCREA(string tipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("A autorização do CREA do responsável técnico {0} é obrigatória.", tipo) }; }

	}
}