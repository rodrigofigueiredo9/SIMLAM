namespace Tecnomapas.Blocos.Entities.Interno.Security
{
	public enum ePermissao
	{
		FuncionarioCriar,
		FuncionarioEditar,
		FuncionarioListar,
		FuncionarioVisualizar,
		FuncionarioAlterarSituacao,
		PessoaAssociar,
		PessoaCriar,
		PessoaEditar,
		PessoaListar,
		PessoaExcluir,
		PessoaVisualizar,
		AdministradorCriar,
		AdministradorEditar,
		AdministradorListar,
		AdministradorAlterarSituacao,
		AdministradorVisualizar,
		RoteiroCriar,
		RoteiroEditar,
		RoteiroDesativar,
		RoteiroVisualizar,
		RoteiroListar,
		ChecagemItemRoteiroCriar,
		ChecagemItemRoteiroExcluir,
		ChecagemItemRoteiroListar,
		ChecagemItemRoteiroVisualizar,
		ChecagemItemRoteiroCriarPendente,
		EmpreendimentoCriar,
		EmpreendimentoEditar,
		EmpreendimentoExcluir,
		EmpreendimentoListar,
		EmpreendimentoVisualizar,
		EmpreendimentoEditarSemPosse,
		RequerimentoCriar,
		RequerimentoEditar,
		RequerimentoVisualizar,
		RequerimentoListar,
		RequerimentoExcluir,
		ProcessoCriar,
		ProcessoEditar,
		ProcessoVisualizar,
		ProcessoListar,
		ProcessoExcluir,
		ProcessoAutuar,
		EditarRequerimento,
		AtividadeEncerrar,
		DocumentoCriar,
		DocumentoEditar,
		DocumentoVisualizar,
		DocumentoListar,
		DocumentoExcluir,
		DocumentoAutuar,
		DocumentoEncerrarOficioPendencia,
		ItemRoteiroCriar,
		ItemRoteiroEditar,
		ItemRoteiroVisualizar,
		ItemRoteiroListar,
		ItemRoteiroExcluir,
		ItemRoteiroAssociadoEditar,
		AnaliseItensCriar,
		PecaTecnicaGerar,
		AnaliseGeograficaCriar,
		AnaliseGeograficaMapa,
		TramitacaoEnviar,
		TramitacaoReceber,
		TramitacaoCancelar,
		TramitacaoApensarJuntar,
		TramitacaoEnviarOrgaoExterno,
		TramitacaoRetirarOrgaoExterno,
		TramitacaoArquivar,
		TramitacaoDesarquivar,
		TramitacaoConfigurar,
		TramitacaoEnviarRegistro,
		TramitacaoReceberRegistro,
		TramitacaoMotivoConfigurar,
		TramitacaoConverterDocumento,
		ArquivoTramitacaoCriar,
		ArquivoTramitacaoEditar,
		ArquivoTramitacaoVisualizar,
		ArquivoTramitacaoListar,
		ArquivoTramitacaoExcluir,
		PapelCriar,
		PapelListar,
		PapelEditar,
		PapelVisualizar,
		PapelExcluir,
		TituloModeloEditar,
		TituloModeloVisualizar,
		TituloModeloListar,
		TituloModeloAlterarSituacao,

		TituloCriar,
		TituloEditar,
		TituloVisualizar,
		TituloExcluir,
		TituloListar,

		TituloEntregar,
		TituloEmitir,
		TituloCancelarEmissao,
		TituloAssinar,
		TituloProrrogar,
		TituloEncerrar,

		TituloDeclaratorioCriar,
		TituloDeclaratorioEditar,
		TituloDeclaratorioVisualizar,
		TituloDeclaratorioExcluir,
		TituloDeclaratorioListar,
		TituloDeclaratorioAlterarSituacao,

		CondicionanteDescricaoListar,
		CondicionanteDescricaoCriar,
		CondicionanteDescricaoEditar,
		CondicionanteDescricaoExcluir,
		CondicionanteAlterarSituacao,
		AtividadeConfiguracaoCriar,
		AtividadeConfiguracaoEditar,
		AtividadeConfiguracaoVisualizar,
		AtividadeConfiguracaoExcluir,
		AtividadeConfiguracaoListar,
		ChecagemPendenciaCriar,
		ChecagemPendenciaVisualizar,
		ChecagemPendenciaExcluir,
		ChecagemPendenciaListar,
		TituloRelatorioIndicadoresTitulos,
		TituloRelatorioIndicadoresTitulosCondicionantes,
		CredenciadoListar,
		CredenciadoVisualizar,
		CredenciadoRegerarChave,
		CredenciadoAlterarSituacao,
		//Caracterização
		ProjetoGeograficoCriar,
		ProjetoGeograficoEditar,
		ProjetoGeograficoExcluir,
		ProjetoGeograficoVisualizar,
		DominialidadeCriar,
		DominialidadeEditar,
		DominialidadeVisualizar,
		DominialidadeExcluir,
		RegularizacaoFundiariaVisualizar,
		RegularizacaoFundiariaExcluir,
		RegularizacaoFundiariaEditar,
		RegularizacaoFundiariaCriar,
		ExploracaoFlorestalVisualizar,
		ExploracaoFlorestalExcluir,
		ExploracaoFlorestalEditar,
		ExploracaoFlorestalCriar,
		QueimaControladaVisualizar,
		QueimaControladaExcluir,
		QueimaControladaEditar,
		QueimaControladaCriar,
		SetorEditar,
		SetorListar,
		SetorVisualizar,
		SecagemMecanicaGraosVisualizar,
		SecagemMecanicaGraosExcluir,
		SecagemMecanicaGraosEditar,
		SecagemMecanicaGraosCriar,
		DescricaoLicenciamentoAtividadeCriar,
		DescricaoLicenciamentoAtividadeEditar,
		DescricaoLicenciamentoAtividadeExcluir,
		DescricaoLicenciamentoAtividadeVisualizar,
		ProducaoCarvaoVegetalVisualizar,
		ProducaoCarvaoVegetalExcluir,
		ProducaoCarvaoVegetalEditar,
		ProducaoCarvaoVegetalCriar,
		DespolpamentoCafeVisualizar,
		DespolpamentoCafeExcluir,
		DespolpamentoCafeEditar,
		DespolpamentoCafeCriar,
		AviculturaVisualizar,
		AviculturaExcluir,
		AviculturaEditar,
		AviculturaCriar,
		SuinoculturaVisualizar,
		SuinoculturaExcluir,
		SuinoculturaEditar,
		SuinoculturaCriar,
		BeneficiamentoMadeiraVisualizar,
		BeneficiamentoMadeiraExcluir,
		BeneficiamentoMadeiraEditar,
		BeneficiamentoMadeiraCriar,
		TerraplanagemVisualizar,
		TerraplanagemExcluir,
		TerraplanagemEditar,
		TerraplanagemCriar,
		ProjetoDigitalImportar,
		ProjetoDigitalVisualizar,
		ProjetoDigitalListar,
		SilviculturaPPFFVisualizar,
		SilviculturaPPFFExcluir,
		SilviculturaPPFFEditar,
		SilviculturaPPFFCriar,
		BarragemVisualizar,
		BarragemExcluir,
		BarragemEditar,
		BarragemCriar,

		BarragemDispensaLicencaCriar,
		BarragemDispensaLicencaEditar,
		BarragemDispensaLicencaExcluir,
		BarragemDispensaLicencaVisualizar,

		RegistroAtividadeFlorestalVisualizar,
		RegistroAtividadeFlorestalExcluir,
		RegistroAtividadeFlorestalEditar,
		RegistroAtividadeFlorestalCriar,
		AquiculturaVisualizar,
		AquiculturaExcluir,
		AquiculturaEditar,
		AquiculturaCriar,
		SilviculturaVisualizar,
		SilviculturaExcluir,
		SilviculturaEditar,
		SilviculturaCriar,
		SilviculturaATVCriar,
		SilviculturaATVEditar,
		SilviculturaATVExcluir,
		SilviculturaATVVisualizar,
		RelatorioPersonalizadoCriar,
		RelatorioPersonalizadoEditar,
		RelatorioPersonalizadoExcluir,
		RelatorioPersonalizadoExecutar,
		RelatorioPersonalizadoExportar,
		RelatorioPersonalizadoImportar,
		RelatorioPersonalizadoListar,
		FiscalizacaoCriar,
		FiscalizacaoEditar,
		FiscalizacaoVisualizar,
		FiscalizacaoListar,
		FiscalizacaoExcluir,
		FiscalizacaoAlterarSituacao,
		FiscalizacaoSemPosse,
		ConfigurarTipoInfracao,
		ConfigurarItem,
		ConfigurarSubitem,
		ConfigurarCampo,
		ConfigurarPergunta,
		ConfigurarResposta,
		ConfigurarFiscCriar,
		ConfigurarFiscEditar,
		ConfigurarFiscVisualizar,
		ConfigurarFiscListar,
		ConfigurarFiscExcluir,
		GerencialAcessar,
		RelatorioPersonalizadoAtribuirExecutor,

		InformacaoCorteVisualizar,
		InformacaoCorteExcluir,
		InformacaoCorteEditar,
		InformacaoCorteCriar,
		ConfigurarCampos,

		MotosserraCriar,
		MotosserraListar,
		MotosserraEditar,
		MotosserraVisualizar,
		MotosserraExcluir,
		MotosserraAlterarSituacao,

		FichaFundiariaCriar,
		FichaFundiariaExcluir,
		FichaFundiariaListar,
		FichaFundiariaVisualizar,
		FichaFundiariaEditar,

		PulverizacaoProdutoCriar,
		PulverizacaoProdutoExcluir,
		PulverizacaoProdutoListar,
		PulverizacaoProdutoVisualizar,
		PulverizacaoProdutoEditar,

		PatioLavagemCriar,
		PatioLavagemExcluir,
		PatioLavagemListar,
		PatioLavagemVisualizar,
		PatioLavagemEditar,

		AcompanhamentoCriar,
		AcompanhamentoListar,
		AcompanhamentoVisualizar,
		AcompanhamentoEditar,
		AcompanhamentoExcluir,
		AcompanhamentoAlterarSituacao,

		CadastroAmbientalRuralSolicitacaoCriar,
		CadastroAmbientalRuralSolicitacaoListar,
		CadastroAmbientalRuralSolicitacaoVisualizar,
		CadastroAmbientalRuralSolicitacaoEditar,
		CadastroAmbientalRuralSolicitacaoExcluir,
		CadastroAmbientalRuralSolicitacaoAlterarSituacao,

		CadastroAmbientalRuralVisualizar,
		CadastroAmbientalRuralExcluir,
		CadastroAmbientalRuralEditar,
		CadastroAmbientalRuralCriar,

		OrgaoParceiroConveniadoVisualizar,
		OrgaoParceiroConveniadoExcluir,
		OrgaoParceiroConveniadoEditar,
		OrgaoParceiroConveniadoCriar,
		OrgaoParceiroConveniadoAlterarSituacao,
		OrgaoParceiroConveniadoListar,
		OrgaoParceiroConveniadoGerenciar,

		ProfissaoListar,
		ProfissaoCriar,
		ProfissaoEditar,

		GrupoQuimico,
		ClasseUso,
		PericulosidadeAmbiental,
		ClassificacaoToxicologica,
		ModalidadeAplicacao,
		FormaApresentacao,
		IngredienteAtivo,
		Cultura,
		Praga,
		PragaAssociarCultura,

		UnidadeProducaoCriar,
		UnidadeProducaoEditar,
		UnidadeProducaoExcluir,
		UnidadeProducaoVisualizar,

		UnidadeConsolidacaoCriar,
		UnidadeConsolidacaoEditar,
		UnidadeConsolidacaoExcluir,
		UnidadeConsolidacaoVisualizar,

		HabilitarEmissaoCFOCFOCCriar,
		HabilitarEmissaoCFOCFOCEditar,
		HabilitarEmissaoCFOCFOCListar,
		HabilitarEmissaoCFOCFOCVisualizar,
		HabilitarEmissaoCFOCFOCAlterarSituacao,

		AgrotoxicoCriar,
		AgrotoxicoListar,
		AgrotoxicoVisualizar,
		AgrotoxicoEditar,
		AgrotoxicoExcluir,

		ConfigDocumentoFitossanitario,
		LiberacaoNumeroCFOCFOCCriar,
		LiberacaoNumeroCFOCFOCListar,
		NumeroCFOCFOCConsultar,

		DestinatarioPTVCriar,
		DestinatarioPTVListar,
		DestinatarioPTVEditar,
		DestinatarioPTVVisualizar,
		DestinatarioPTVExcluir,

		HabilitacaoEmissaoPTVListar,
		HabilitacaoEmissaoPTVCriar,
		HabilitacaoEmissaoPTVEditar,

		PTVCriar,
		PTVEditar,
		PTVExcluir,
		PTVListar,
		PTVVisualizar,
		PTVAtivar,
		PTVCancelar,
		PTVComunicador,

		CFOListar,
		CFOVisualizar,

		CFOCListar,
		CFOCVisualizar,

		PTVOutroCriar,
		PTVOutroListar,
		PTVOutroVisualizar,
		PTVOutroCancelar,
        PTVOutroEditar,

		LocalVistoriaOperar,
		LocalVistoriaListar,
		LocalVistoriaVisualizar,

		RelatorioMapa,

        DeclaracaoAdicional,

        ConfigurarCodigoReceita,
        ConfigurarPenalidade,
        ConfigurarProdutosDestinacao,
	}
}