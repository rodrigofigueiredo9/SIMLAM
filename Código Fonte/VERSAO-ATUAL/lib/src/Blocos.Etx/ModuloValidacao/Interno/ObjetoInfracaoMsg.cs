namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ObjetoInfracaoMsg _objetoInfracaoMsg = new ObjetoInfracaoMsg();
		public static ObjetoInfracaoMsg ObjetoInfracao
		{
			get { return _objetoInfracaoMsg; }
			set { _objetoInfracaoMsg = value; }
		}
	}

	public class ObjetoInfracaoMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Objeto da infração salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Objeto da infração editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Objeto da infração excluído com sucesso." }; } }

		public Mensagem AreaEmbarcadaAtvIntermedObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_AreaEmbarcadaAtvIntermed, #SpanAreaEmbargadaAtvIntermed", Texto = "Área embargada e/ou atividade interditada é obrigatório." }; } }
		public Mensagem TeiGeradoPeloSistemaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_TeiGeradoPeloSistema, #SpanTeiGeradoPeloSistema", Texto = "Gerar Nº do TEI é obrigatório." }; } }
		public Mensagem TeiGeradoPeloSistemaSerieTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_TeiGeradoPeloSistemaSerieTipo", Texto = "Série é obrigatório." }; } }

		public Mensagem NumTeiBlocoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_NumTeiBloco", Texto = "Nº do TEI - bloco é obrigatório." }; } }
		public Mensagem NumTeiBlocoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_NumTeiBloco", Texto = "Nº do TEI - bloco é inválido." }; } }
		public Mensagem NumTeiBlocoMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_NumTeiBloco", Texto = "Nº do TEI - bloco deve ser maior do que zero." }; } }

		public Mensagem NumTeiObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_NumTeiBloco", Texto = "Nº do TEI é obrigatório." }; } }
		public Mensagem NumTeiInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_NumTeiBloco", Texto = "Nº do TEI é inválido." }; } }
		public Mensagem NumTeiMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_NumTeiBloco", Texto = "Nº do TEI deve ser maior do que zero." }; } }

		public Mensagem DescricaoTermoEmbargoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_DescricaoTermoEmbargo", Texto = "Descrição do termo de interdição/embargo é obrigatório." }; } }
        public Mensagem OpinarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_OpniaoAreaDanificada", Texto = "Opinar quanto a interdição/embargo é obrigatório." }; } }

        public Mensagem IUFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_IsDigital", Texto = "IUF para interdição/embargo é obrigatório." }; } }
        public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_NumeroIUF", Texto = "Número do IUF é obrigatório." }; } }
        public Mensagem SerieObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_Serie", Texto = "Série é obrigatório." }; } }
        public Mensagem DataLavraturaIUFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_DataLavratura", Texto = "Data da Lavratura é obrigatório." }; } }

        public Mensagem InterditadoEmbargadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_Interditado", Texto = "Interditado/Embargado é obrigatório." }; } }

		public Mensagem ExisteAtvAreaDegradObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_ExisteAtvAreaDegrad, #SpanExisteAtvAreaDegrad", Texto = "Está sendo desenvolvida alguma atividade na área interditada/embargada é obrigatório." }; } }
		public Mensagem ExisteAtvAreaDegradEspecificarTextoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_ExisteAtvAreaDegradEspecificarTexto", Texto = "Especificar é obrigatório." }; } }

		public Mensagem FundamentoInfracaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_FundamentoInfracao", Texto = "Fundamentos que caracterizam a infração é obrigatório." }; } }

		public Mensagem CaracteristicaSoloAreaDanificadaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_CaracteristicaSoloAreaDanificada", Texto = "Característica do solo da área danificada é obrigatório." }; } }

		public Mensagem AreaDeclividadeMediaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_ExisteAtvAreaDegrad", Texto = "Declividade média da área é obrigatória." }; } }
		public Mensagem AreaDeclividadeMediaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_ExisteAtvAreaDegrad", Texto = "Declividade média da área deve ser maior do que zero." }; } }
		public Mensagem AreaDeclividadeMediaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_ExisteAtvAreaDegrad", Texto = "Declividade média da área é inválida." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem ArquivoNaoEhPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo não é do tipo pdf" }; } }

		public Mensagem EspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ObjetoInfracao_InfracaoResultouErosaoTipoTexto", Texto = "Especificar é obrigatório." }; } }		
	}

}