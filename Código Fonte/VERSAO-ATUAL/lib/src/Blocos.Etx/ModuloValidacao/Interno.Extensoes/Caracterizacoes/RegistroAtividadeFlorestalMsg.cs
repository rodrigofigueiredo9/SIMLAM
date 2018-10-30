namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static RegistroAtividadeFlorestalMsg _registroAtividadeFlorestalMsg = new RegistroAtividadeFlorestalMsg();
		public static RegistroAtividadeFlorestalMsg RegistroAtividadeFlorestal
		{
			get { return _registroAtividadeFlorestalMsg; }
			set { _registroAtividadeFlorestalMsg = value; }
		}
	}
	public class RegistroAtividadeFlorestalMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Registro de atividade florestal excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Registro de atividade florestal salva com sucesso" }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de Registro de atividade florestal deste empreendimento?" }; } }

		public Mensagem PossuiNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiNumero", Texto = "Já possui número de registro é obrigatório." }; } }
		public Mensagem NumeroUtilizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroRegistro", Texto = @"Número de registro já foi utilizado por outro empreendimento." }; } }
		public Mensagem NumeroRegistroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroRegistro", Texto = @"Número do registro é obrigatório." }; } }
		public Mensagem NumeroRegistroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroRegistro", Texto = @"Número do registro inválido." }; } }
		public Mensagem NumeroRegistroSuperiorMax(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroRegistro", Texto = string.Format("Número do registro não pode ser superior a {0}.", strNumero) }; }

		public Mensagem ExcluirConsumo { get { return new Mensagem() { Texto = "Deseja realmente este consumo real?" }; } }
		public Mensagem ConsumoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Consumo real é obrigatório." }; } }
		public Mensagem CamposObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Consumo_Container" + indice, Texto = "Campos obrigatórios para adicionar um novo consumo real." };
		}

		public Mensagem CategoriaObrigatoria(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeId" + indice, Texto = "Categoria é obrigatória." };
		}

		public Mensagem FonteEnergiaObrigatoria(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Consumo_Fonte" + indice, Texto = "É necessário adicionar pelo menos um dado de fonte de energia." };
		}

		public Mensagem CategoriaDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "inexistente, .erroCategorias", Texto = @"Existem categorias duplicadas." }; } }
		public Mensagem AtividadeDesabilitada(string atividadeNome, int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeId" + indice, Texto = string.Format("A atividade {0} não pode mais ser caracterizada, pois está desativada. Favor selecionar nova atividade.", atividadeNome) };
		}

		public Mensagem FonteDuplicada(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "FonteTipos" + indice, Texto = @"Matéria-prima floresta / Fonte de energia já adicionada." };
		}

		public Mensagem FonteTipoObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "inexistente, .erroFonteTipos", Texto = @"Matéria-prima floresta / Fonte de energia é obrigatória." };
		}

		public Mensagem UnidadeObrigatoria(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "inexistente, .erroUnidades", Texto = @"Unidade é obrigatória." };
		}

		public Mensagem QdeObrigatoria(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "inexistente, .erroQde", Texto = @"Quantidade é obrigatória." };
		}

		public Mensagem QdePlantadaNativaMaiorAno(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "QdeFlorestaPlantada" + indice + ", #QdeFlorestaNativa" + indice, Texto = "A soma das quantidades de floresta exótica e floresta nativa deve ser igual a \"Quantidade(ano)\"." };
		}

		public Mensagem QdeOutroEstadoMaiorAno(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "QdeOutroEstado" + indice, Texto = "A quantidade oriunda de outro estado deve ser menor ou igual a \"Quantidade (ano)\"." };
		}

		public Mensagem PeloMenosUmaQdeObrigatoria(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "QdeOutroEstado" + indice + ", #QdeFlorestaPlantada" + indice + ", #QdeFlorestaNativa" + indice, Texto = "A quantidade floresta exótica e floresta nativa ou a quantidade oriunda de outro estado é obrigatória." };
		}


		public Mensagem PossuiLicencaAmbiental(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiLicencaAmbiental" + indice, Texto = @"Selecione uma resposta para a pergunta. Possui licença ambiental?" };
		}

		public Mensagem EmitidoIDAFOuExterno(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OrgaoEmissor" + indice, Texto = @"Selecione uma das opções. Emitido pelo IDAF ou Emitido por outro Orgão." };
		}

		public Mensagem GrupoDispensadoEmitidoIDAFOuExterno(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OrgaoEmissorDispensado" + indice, Texto = @"Selecione uma das opções. Emitido pelo IDAF ou Emitido por outro Orgão." };
		}

		public Mensagem LicencaAmbientalObrigatoria(int indice, string categoria)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PossuiLicencaAmbiental" + indice, Texto = string.Format("A categoria {0} está configurada para informar a Licença Ambiental ou Dispensa da Licença Ambiental.", categoria) };
		}

		public Mensagem LicencaAmbientalNaoEncontrada(string modelo, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = string.Format("{0} nº {1} não foi localizado no sistema, favor continuar o cadastro ou realizar nova busca.", modelo, numero) };
		}

		public Mensagem LicencaAmbientalEncontrada(string modelo, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = string.Format("{0} nº {1} localizado no sistema, favor continuar o cadastro.", modelo, numero) };
		}

		public Mensagem ModeloLicencaObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TituloModelo" + indice, Texto = "Licença é obrigatório." };
		}

		public Mensagem NumeroLicencaObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TituloNumero" + indice, Texto = "Número é obrigatório." };
		}

		public Mensagem NumeroLicencaInvalido(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TituloNumero" + indice, Texto = "Número do Título inválido. O formato correto é número/ano." };
		}

		public Mensagem NumeroProtocoloLicencaObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero" + indice, Texto = "Nº protocolo é obrigatório." };
		}

		public Mensagem NumeroProtocoloRenovacaoObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloRenovacaoNumero" + indice, Texto = "Nº protocolo renovação é obrigatório." };
		}

		public Mensagem RenovacaoDataMaiorValidade(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloRenovacaoData_DataTexto" + indice, Texto = @"A data do protocolo de renovação da licença ambiental deve ser no mínimo 120 dias antes da data de validade da licença anterior." };
		}

		public Mensagem OrgaoExpedidoObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OrgaoExpedidor" + indice, Texto = "Orgão expedidor é obrigatório." };
		}

		public Mensagem DocumentoObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TituloModelo" + indice, Texto = "Documento é obrigatório." };
		}

		public Mensagem OrgaoEmissorObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OrgaoExpedidor" + indice, Texto = "Orgão emissor é obrigatório." };
		}

		public Mensagem DUANumeroObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DUANumero" + indice, Texto = "Número do DUA é obrigatório." };
		}

		public Mensagem DUAValorObrigatorio(int indice)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DUAValor" + indice, Texto = "Valor do DUA é obrigatório." };
		}

	}
}