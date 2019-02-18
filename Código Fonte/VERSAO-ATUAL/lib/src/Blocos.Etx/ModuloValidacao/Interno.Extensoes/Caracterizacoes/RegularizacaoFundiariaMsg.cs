namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static RegularizacaoFundiariaMsg _regularizacaoFundiariaMsg = new RegularizacaoFundiariaMsg();
		public static RegularizacaoFundiariaMsg RegularizacaoFundiaria
		{
			get { return _regularizacaoFundiariaMsg; }
			set { _regularizacaoFundiariaMsg = value; }
		}
	}

	public class RegularizacaoFundiariaMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização Regularização fundiária excluida com sucesso." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização Regularização fundiária salva com sucesso." }; } }

		public Mensagem TempoDeOcupacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TempoOcupacao", Texto = @"O tempo de ocupação é obrigatório." }; } }
		public Mensagem TempoDeOcupacaoMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TempoOcupacao", Texto = @"O tempo de ocupação deve ser maior do que zero." }; } }
		public Mensagem PossesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"A caracterização de Dominialidade não possui domínio do tipo posse." }; } }
		public Mensagem TrasmitenteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NomeRazaoSocial, #CpfCpnj", Texto = @"Trasmitente da posse é obrigatório." }; } }
		public Mensagem TipoRegularizacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Caracterizacao_Posse_RegularizacaoTipo", Texto = @"Tipo da regularização é obrigatório." }; } }
		public Mensagem TipoEdificacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Tipo_Edificacao", Texto = @"Tipo da Edificação é obrigatório." }; } }
		public Mensagem QuantidadeEdificacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Quantidade_Edificacao", Texto = @"A quantidade da Edificação é obrigatório." }; } }
		public Mensagem CentroComercialObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CentroComercialPosse", Texto = @"Centro comercial é obrigatório." }; } }
		public Mensagem CentroComercialInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CentroComercialPosse", Texto = @"Centro comercial é inválido." }; } }
		public Mensagem BrPosseInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "BrPosse", Texto = @"BR a posse é inválido." }; } }
		public Mensagem EsPosseInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "EsPosse", Texto = @"ES a posse é inválido." }; } }
		public Mensagem BenfeitoriasEdificacoesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Benfeitorias", Texto = @"Benfeitorias/Edificações é obrigatório." }; } }
		public Mensagem UsoSoloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fsUsoSolo", Texto = @"É obrigatorio ter no minimo uma área de uso de solo." }; } }
		public Mensagem UsoSoloTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TipoUso", Texto = @"Tipo de Uso do solo é obrigatorio." }; } }
		public Mensagem UsoSoloTipoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TipoUso", Texto = @"Tipo de Uso já adicionado." }; } }
		public Mensagem UsoSoloAreaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "UsoSolo_Area", Texto = @"Porcengatem da area de Uso do solo é obrigatorio." }; } }
		public Mensagem UsoSoloAreaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "UsoSolo_Area", Texto = @"Porcengatem da area de Uso do solo deve ser maior do que zero." }; } }
		public Mensagem UsoSoloLimitePorcentagem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "UsoSolo_Area", Texto = @"A somatória das áreas não pode ser maior que 100%." }; } }
		public Mensagem TrasmitenteJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Trasmitente já foi adicionado." }; } }
		public Mensagem RelacaoTrabalhoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "RelacaoTrabalho", Texto = @"Relação de trabalho predominante é obrigatorio." }; } }
		public Mensagem CampoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Campo obrigatório." }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = @"Deseja realmente excluir a caracterização Regularização fundiária deste empreendimento?" }; } }
		public Mensagem AreaRequeridaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Caracterizacao_Posse_AreaRequerida_", Texto = @"Área requerida é obrigatória." }; } }
		public Mensagem AreaRequeridaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Caracterizacao_Posse_AreaRequerida_", Texto = @"Área requerida é inválida." }; } }
		public Mensagem AreaRequeridaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Caracterizacao_Posse_AreaRequerida_", Texto = @"Área requerida deve ser maior do que zero." }; } }
		public Mensagem TerrenoDevolutoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbTerrenoDevoluto", Texto = @"É presumidamente devoluto é obrigatório." }; } }
		public Mensagem EspecificarDominialidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TerrenoDevoluto_Outro", Texto = @"Especificar a Dominialidade é obrigatório." }; } }
		public Mensagem HomologacaoAprovadaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TerrenoDevoluto_Outro", Texto = @"Homologação aprovada é obrigatória." }; } }
		public Mensagem RequerenteResidePosseObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbRequerenteResideNaPosse", Texto = @"Requerente reside na posse é obrigatório." }; } }
		public Mensagem ExisteLitigioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbExisteLitigio", Texto = @"Existe Litígio é obrigatório." }; } }
		public Mensagem NomeLitigioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NomeLitigo", Texto = @"Nome do Litígio é obrigatório." }; } }
		public Mensagem SobrepoeFaixaDivisaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbSobrepoeSeDivisa", Texto = @"Sobrepõe-se a faixa de divisa é obrigatório." }; } }
		public Mensagem AQuemPertenceLimiteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SobrepoeSeDivisa_Outro", Texto = @"A quem pertence o limite é obrigatório." }; } }
		public Mensagem BanhadoRioCorregoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbBanhadoPorRioCorrego", Texto = @"Banhado por rios ou córregos é obrigatório." }; } }
		public Mensagem NomeRioCorregoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NomeDoRio", Texto = @"Nome do rio ou córrego é obrigatório." }; } }
		public Mensagem PossuiNascenteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbPossuiNascente", Texto = @"Possui nascente é obrigatório." }; } }
		public Mensagem RedeAguaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbRedeAgua", Texto = @"Rede de água é obrigatório." }; } }
		public Mensagem RedeEsgotoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbRedeEsgoto", Texto = @"Rede de Esgoto é obrigatório." }; } }
		public Mensagem LuzEletricaDomiciliarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbLuzEletrica", Texto = @"Luz elétrica domiciliar é obrigatório." }; } }
		public Mensagem IluminacaoViaPublicaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbIluminacaoPublica", Texto = @"Iluminação da via pública é obrigatório." }; } }
		public Mensagem RedeTelefonicaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbRedeTelefonica", Texto = @"Rede telefônica é obrigatório." }; } }
		public Mensagem CalcadaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbCalcada", Texto = @"Calçada é obrigatório." }; } }
		public Mensagem PavimentacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbPavimentacao", Texto = @"Pavimentação é obrigatório." }; } }
		public Mensagem TipoPavimentacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pavimentacao_Outro", Texto = @"Tipo de pavimentação é obrigatório." }; } }
		public Mensagem ComprovacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DominioComprovacaoId", Texto = @"Comprovação é obrigatório." }; } }
		public Mensagem AreaPosseDocumento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaPosseDocumento", Texto = @"Área de posse do Documento é de preenchimento obrigatório." }; } }
		public Mensagem ConfrontacoesNorte { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "txtConfrontacoesNorte", Texto = @"Confrontacoes do Norte é de preenchimento obrigatório." }; } }
		public Mensagem ConfrontacoesSul { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "txtConfrontacoesSul", Texto = @"Confrontacoes do Sul é de preenchimento obrigatório." }; } }
		public Mensagem ConfrontacoesOeste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "txtConfrontacoesOeste", Texto = @"Confrontacoes do Oeste é de preenchimento obrigatório." }; } }
		public Mensagem ConfrontacoesLeste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "txtConfrontacoesLeste", Texto = @"Confrontacoes do Leste é de preenchimento obrigatório." }; } }
		public Mensagem DescricaoComprovacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Descricao_Comprovacao", Texto = @"Descrição da comprovação é de preenchimento obrigatório." }; } }

		public Mensagem RegularizacaoInvalida(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fsRegularizacoesFundiarias", Texto = string.Format("A regularização fundiária de identificação {0} está inválida, favor atualizar os seus dados.", identificacao) };
		}

		#region Domínio Avulso

		public Mensagem MatriculaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Matricula", Texto = @"Matrícula Nº é obrigatório." }; } }
		public Mensagem FolhaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Folha", Texto = @"Folha Nº é obrigatório." }; } }
		public Mensagem LivroObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Livro", Texto = @"Livro Nº é obrigatório." }; } }
		public Mensagem AreaDocumentoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaDocumento", Texto = @"Área Documento (ha) é obrigatória." }; } }
		public Mensagem CartorioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cartorio", Texto = @"Cartório é obrigatório." }; } }
		public Mensagem MatriculaJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Matrícula já adicionada." }; } }

		public Mensagem PossuiDominioAvulsoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiDominioAvulso", Texto = @"Adicionar área titulada anexa à posse? é obrigatório." }; } }
		public Mensagem DominioAvulsoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fsAreaAnexaPosse", Texto = @"É necessário adicionar ao menos uma área titulada." }; } }

		#endregion
	}
}