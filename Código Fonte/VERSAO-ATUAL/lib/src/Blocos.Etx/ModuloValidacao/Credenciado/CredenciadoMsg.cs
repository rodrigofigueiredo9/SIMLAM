

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CredenciadoMsg _credenciadoMsg = new CredenciadoMsg();
		public static CredenciadoMsg Credenciado
		{
			get { return _credenciadoMsg; }
		}
	}

	public class CredenciadoMsg : IPessoaMsg
	{
		public Mensagem RegistroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Profissao_Registro", Texto = "Para o perfil Responsável Técnico, é obrigatório que informe seu Registro no CREA." }; } }
		public Mensagem OrgaoCreaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Profissao_OrgaoClasseId", Texto = "Para o perfil Responsável técnico, é obrigatório que o órgão de classe seja o CREA." }; } }
		public Mensagem TipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Credenciado_Tipo", Texto = "Perfil do usuário é obrigatório." }; } }
		public Mensagem ObrigatorioNome { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem CpfInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Cpf", Texto = "CPF inválido." }; } }
		public Mensagem CpfObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Cpf", Texto = "CPF é obrigatório." }; } }
		public Mensagem DataNascimentoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataNascimento", Texto = "Data de nascimento é obrigatório." }; } }
		public Mensagem DataNascimentoObrigatoriaConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Data de nascimento do cônjuge é obrigatório." }; } }
		
		public Mensagem DataNascimentoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataNascimento", Texto = "Data de nascimento inválida." }; } }
		public Mensagem DataNascimentoInvalidaConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Data de nascimento do cônjuge inválida." }; } }
		
		public Mensagem DataNascimentoFuturo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataNascimento", Texto = "Data de nascimento está no futuro." }; } }
		public Mensagem DataNascimentoFuturoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Data de nascimento do cônjuge deve ser menor que a data atual." }; } }

		public Mensagem EmailObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Contato_Email", Texto = "E-mail é obrigatório." }; } }
		public Mensagem EmailInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Contato_Email", Texto = "E-mail é inválido." }; } }
		public Mensagem ObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Cônjuge é obrigatório." }; } }
		public Mensagem ObrigatorioMae { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_NomeMae", Texto = "Nome da mãe é obrigatório." }; } }
		public Mensagem ObrigatorioPai { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_NomePai", Texto = "Nome do pai é obrigatório." }; } }

		public Mensagem EnderecoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Numero", Texto = "Número é obrigatório." }; } }
		public Mensagem EnderecoBairroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Bairro", Texto = "Bairro é obrigatório." }; } }
		public Mensagem EnderecoLogradouroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Logradouro", Texto = "Logradouro é obrigatório." }; } }

		public Mensagem EnderecoEstadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_EstadoId", Texto = "Estado é obrigatório." }; } }
		public Mensagem EnderecoEstadoObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Estado do cônjuge é obrigatório." }; } }

		public Mensagem EnderecoMunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_MunicipioId", Texto = "Município é obrigatório." }; } }
		public Mensagem EnderecoCepObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Cep", Texto = "CEP é obrigatório." }; } }

		public Mensagem ObrigatorioRazaoSocial { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_RazaoSocial", Texto = "Razão social é obrigatório." }; } }
		public Mensagem CnpjInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_Cnpj", Texto = "CNPJ inválido." }; } }
		public Mensagem CnpjObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_Cnpj", Texto = "CNPJ é obrigatório." }; } }
		public Mensagem DataFundacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataFundacao", Texto = "Data da fundação inválida." }; } }
		public Mensagem DataFundacaoFuturo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataFundacao", Texto = "Data da fundação está no futuro." }; } }

		public Mensagem RepresentanteExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este representante já está associado." }; } }
		public Mensagem RepresentanteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Representante_Container", Texto = "Representante é obrigatório." }; } }
		public Mensagem ObrigatorioQualquerTelefone { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Contato_Container", Texto = "É obrigatório informar pelo menos um telefone." }; } }

		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }

		public Mensagem CnpjExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_Cnpj", Texto = "CNPJ já está cadastrado." }; } }
		public Mensagem CpfExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Cpf", Texto = "CPF já está cadastrado." }; } }

		public Mensagem SalvarPublico(string email)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("Foi enviado ao e-mail {0} a confirmação de cadastro e o número da chave de acesso para acessar o Módulo Credenciado. Esse cadastro tem prazo de validade de 5 dias. Após essa data, caso não tenha ativado o acesso ao módulo credenciado, seu cadastro será excluído", email) };
		}

		public Mensagem ReenviarChaveSucesso(string email)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("Foi enviado ao e-mail {0} a nova da chave de acesso para acessar o Módulo Credenciado. Essa chave tem prazo de validade de 5 dias. Após essa data, caso não tenha ativado o acesso ao módulo credenciado, seu cadastro será excluído.", email) };
		}

		public Mensagem ReenviarChaveSucessoOrgaoParceiro
		{
			get
			{
				return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "E-mail atualizado com sucesso. Aguarde a chave de ativação gerada pelo IDAF, a qual será enviada para o e-mail informado." };
			}
		}

		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Credenciado salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Credenciado editado com sucesso." }; } }
		public Mensagem RegerarChave { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Chave regerada com sucesso. Um e-mail com a nova chave foi enviado para o credenciado." }; } }

		public Mensagem AlterarSituacao(string situacao, string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Situação do credenciado \"{0}\" foi alterada para \"{1}\" com sucesso.", nome, situacao) };
		}

		public Mensagem CpfCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O CPF já está associado a um credenciado." }; } }
		public Mensagem CnpjCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O CNPJ já está associado a um credenciado." }; } }

		public Mensagem EnderecoCepInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Cep", Texto = "CEP é inválido." }; } }
		public Mensagem EnderecoCepInvalidoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "CEP do cônjuge é inválido." }; } }

		public Mensagem EnderecoMunicipioInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_MunicipioId", Texto = "Município é inválido." }; } }
		public Mensagem EnderecoMunicipioInvalidoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Município do cônjuge é inválido." }; } }
		
		public Mensagem EnderecoEstadoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_EstadoId", Texto = "Estado é inválido." }; } }
		public Mensagem EnderecoEstadoInvalidoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Estado do cônjuge é inválido." }; } }
		public Mensagem EnderecoMunicipioOutroEstadoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "O estado do município selecionado do cônjuge é diferente do estado selecionado." }; } }

		public Mensagem EnderecoMunicipioOutroEstado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_MunicipioId", Texto = "O estado do município selecionado é diferente do estado selecionado." }; } }

		public Mensagem ExcluirNaoPermitidoPoisRepresenta(string pessoaCnpj)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O credenciado não pode ser excluído, pois é um representante de {0}", pessoaCnpj) };
		}

		public Mensagem PessoaExistenteInterno(string tipoDocumento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("{0} localizado. Continue o cadastro.", tipoDocumento) };
		}

		public Mensagem ReenviarChave(string tipoDocumento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} já está associado a um solicitante de credenciamento. Se desejar, altere o e-mail para reenvio da chave de acesso.", tipoDocumento) };
		}

		public Mensagem ExistenteAtivado(string tipoDocumento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} já está associado a um credenciado cadastrado.", tipoDocumento) };
		}

		public Mensagem ExistenteBloqueado
		{
			get
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CPF já está associado a um credenciado com situação igual a \"Bloqueado\". Por favor, entre em contato com o IDAF para mais informações." };
			}
		}

		public Mensagem CredenciadoBloqueado(string tipoDocumento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} já está associado a um credenciado com situação igual a 'Bloqueado'. Por favor, entre em contato com o IDAF para mais informações.", tipoDocumento) };
		}

		public Mensagem AtualizarEmail(string tipoDocumento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} já está associado a um solicitante de credenciamento com perfil 'Parceiro/ Conveniado'. Se desejar, altere o e-mail para atualização dos dados. A chave de acesso será gerada pelo IDAF, a qual será enviada para o e- mail informado.", tipoDocumento) };
		}

		public Mensagem SituacaoMotivoObrigatorio(string motivo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Motivo para situação {0} é obrigatório.", motivo.ToLower()) };
		}

		public Mensagem AssociadoEmpreendimento(String emp)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associado ao(s) empreendimento(s): {0}", emp) };
		}

		public Mensagem AssociadoRequerimento(String req)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associado ao requerimento: {0}", req) };
		}

		public Mensagem AssociadoDocumento(String doc)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associada ao documento: {0}", doc) };
		}

		public Mensagem AssociadoProcesso(String pro)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associada ao processo: {0}", pro) };
		}

		public Mensagem AssociadoTitulo(String tit)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associada ao título: {0}", tit) };
		}

		public Mensagem AssociadoTituloEntrega(String tit)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associada a entrega do título: {0}", tit) };
		}

		public Mensagem AssociadoCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A pessoa não pode ser excluída, pois ela é um credenciado." }; } }
		public Mensagem ObrigatorioRg { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_RG", Texto = "RG é obrigatório" }; } }
		public Mensagem ObrigatorioEstadoCivil { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_EstadoCivil", Texto = "Estado civíl é obrigatório." }; } }
		public Mensagem ObrigatorioSexo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Sexo", Texto = "Sexo é obrigatório." }; } }
		public Mensagem ObrigatorioNacionalidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Nacionalidade", Texto = "Nacionalidade é obrigatório." }; } }
		public Mensagem ObrigatorioNaturalidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Naturalidade", Texto = "Naturalidade é obrigatório." }; } }
		public Mensagem ObrigatorioProfissao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Profissao_ProfissaoTexto", Texto = "Profissão é obrigatório." }; } }
		public Mensagem ObrigatorioNomeFantasia { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_NomeFantasia", Texto = "Nome fantasia é obrigatório." }; } }

		public Mensagem ResponsavelTecnicoSemProfissao
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsável técnico sem profissão." }; }
		}

		public Mensagem ChaveObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Chave", Texto = "Chave de acesso é obrigatória." }; } }
		public Mensagem ChaveInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Chave", Texto = "Chave de acesso inválida." }; } }
		public Mensagem CredenciadoChaveJaAtiva { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Chave de acesso já utilizada." }; } }
		public Mensagem CredenciadoAtivado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "O Credenciado foi ativado com sucesso" }; } }
		public Mensagem CredenciadoReativado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "O Credenciado foi reativado com sucesso" }; } }
		public Mensagem LoginExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "O login já está sendo utilizado por um outro usuário" }; } }
		public Mensagem LoginObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "Login é obrigatório" }; } }
		public Mensagem SenhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = "Senha é obrigatório" }; } }
		public Mensagem ConfirmarSenhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConfirmarSenha", Texto = "Confirmar senha é obrigatório" }; } }
		public Mensagem SenhaDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConfirmarSenha", Texto = "A confirmação de senha está diferente da senha" }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Credenciado inexistente" }; } }
		public Mensagem FormatoLogin { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "Formato do login é inválido. Use apenas letras e números, iniciando sempre com uma letra." }; } }
		public Mensagem AlterarSenhaFuncionario { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "A senha foi alterada com sucesso." }; } }

		public Mensagem SenhaTamanho(int configuracao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = String.Format("A senha precisa ter no mínimo {0} caracteres", configuracao) };
		}

		public Mensagem LoginTamanho(int configuracao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = String.Format("O login precisa ter no mínimo {0} caracteres", configuracao) };
		}
		public Mensagem ExcluirPessoaCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível excluir a pessoa vinculada ao usuário logado." }; } }

		public Mensagem EmailObrigatorioRegerar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Contato_Email", Texto = "Para regerar a chave, é preciso que este credenciado cadastre um e-mail de contato." }; } }
		public Mensagem EmailInvalidoRegerar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Contato_Email", Texto = "Para regerar a chave, é preciso que este credenciado possua um e-mail válido." }; } }

		public Mensagem PosseCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Você não tem permissão para acessar os dados desse Requerimento." }; } }

		public Mensagem RegerarChaveAguardandoAtivacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Não é possível regerar chave de acesso, pois o credenciado está Aguardando ativação." }; } }

		public Mensagem OrgaoParceiroPessoaTipoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo pessoa jurídica inválido para cadastro de órgão parceiro/ conveniado." }; } }
		public Mensagem OrgaoParceiroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Credenciado_OrgaoParceiroId", Texto = "Órgão parceiro/ conveniado é obrigatório." }; } }
		public Mensagem OrgaoParceiroUnidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Credenciado_OrgaoParceiroUnidadeId", Texto = "Unidade do órgão parceiro/ conveniado é obrigatória." }; } }

		public Mensagem OrgaoParceiroInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Credenciado_OrgaoParceiroId", Texto = "Órgão parceiro/ conveniado não existe mais." }; } }
		public Mensagem OrgaoParceiroUnidadeInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Credenciado_OrgaoParceiroUnidadeId", Texto = "Unidade do órgão parceiro/ conveniado não existe mais." }; } }

		public Mensagem OrgaoParceiroSituacaoInvalida(string orgaoSiglaNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Credenciado_OrgaoParceiroId", Texto = String.Format("O órgao parceiro/ conveniado {0} não está mais ativo junto ao IDAF.", orgaoSiglaNome) };
		}

		public Mensagem SalvarOrgaoParceiroConveniado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Usuário Credenciado salvo com sucesso. Aguarde a chave de ativação gerada pelo IDAF, a qual será enviada para o e-mail informado." }; } }

		public Mensagem SalvoSucesso(string email)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("E-mail atualizado com sucesso. Aguarde a chave de ativação gerada pelo IDAF, a qual será enviada para o e-mail informado.", email) };
		}

		public Mensagem DataNascimentoObrigatoriaMsg(bool isConjuge)
		{
			return isConjuge ? DataNascimentoObrigatoriaConjuge : DataNascimentoObrigatoria;
		}

		public Mensagem DataNascimentoInvalidaMsg(bool isConjuge)
		{
			return isConjuge ? DataNascimentoInvalidaConjuge : DataNascimentoInvalida;
		}

		public Mensagem DataNascimentoFuturoMsg(bool isConjuge)
		{
			return isConjuge ? DataNascimentoFuturoConjuge : DataNascimentoFuturo;
		}

		public Mensagem EnderecoCepInvalidoMsg(bool isConjuge)
		{
			return isConjuge ? EnderecoCepInvalidoConjuge : EnderecoCepInvalido;
		}

		public Mensagem EnderecoEstadoInvalidoMsg(bool isConjuge)
		{
			return isConjuge ? EnderecoEstadoInvalidoConjuge : EnderecoEstadoInvalido;
		}

		public Mensagem EnderecoMunicipioInvalidoMsg(bool isConjuge)
		{
			return isConjuge ? EnderecoMunicipioInvalidoConjuge : EnderecoMunicipioInvalido;
		}

		public Mensagem EnderecoMunicipioOutroEstadoMsg(bool isConjuge)
		{
			return isConjuge ? EnderecoMunicipioOutroEstadoConjuge : EnderecoMunicipioOutroEstado;
		}
		
		public Mensagem CPFNaoAssociadoACredenciado { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "O CPF não está associado a nenhum credenciado.", Tipo= eTipoMensagem.Advertencia }; } }

		public Mensagem CPFNaoPossuiRegistroOrgaoClasse { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "A pessoa deve possuir profissão e registro no órgão de classe informados." }; } }

		public Mensagem CPFNaoPossuiHabilitacaoCFOCFOC { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "O responsável técnico deve estar habilitado para emissão de CFO e CFOC.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem HabilitacaoCFOCCFOCInativa { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "A situação da habilitação para emissão de CFO e CFOC do responsável técnico deve ser ativo ou advertido.", Tipo = eTipoMensagem.Advertencia }; } }
	}
}