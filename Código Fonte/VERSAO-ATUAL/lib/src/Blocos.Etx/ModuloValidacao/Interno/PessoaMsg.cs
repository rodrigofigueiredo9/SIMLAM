

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static PessoaMsg _pessoaMsg = new PessoaMsg();

		public static PessoaMsg Pessoa
		{
			get { return _pessoaMsg; }
		}
	}

	public class PessoaMsg : IPessoaMsg
	{
		public Mensagem ObrigatorioRg { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_RG", Texto = "RG é obrigatório." }; } }
		public Mensagem ObrigatorioEstadoCivil { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_EstadoCivil", Texto = "Estado civíl é obrigatório." }; } }
		public Mensagem ObrigatorioEstadoCivilConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Estado civíl é obrigatório." }; } }
		
		public Mensagem ObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Cônjuge é obrigatório." }; } }
		public Mensagem ObrigatorioSexo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Sexo", Texto = "Sexo é obrigatório." }; } }
		public Mensagem ObrigatorioSexoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Sexo do cônjuge é obrigatório." }; } }
		
		public Mensagem ObrigatorioNacionalidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Nacionalidade", Texto = "Nacionalidade é obrigatório." }; } }
		public Mensagem ObrigatorioNacionalidadeConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Nacionalidade do cônjuge é obrigatório." }; } }
		public Mensagem ObrigatorioNaturalidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Naturalidade", Texto = "Naturalidade é obrigatório." }; } }
		public Mensagem ObrigatorioNaturalidadeConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Naturalidade do cônjuge é obrigatório." }; } }
		
		public Mensagem ObrigatorioNome { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem ObrigatorioNomeConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Nome do cônjuge é obrigatório." }; } }
		public Mensagem CpfInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Cpf", Texto = "CPF inválido." }; } }
		public Mensagem CpfObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Cpf", Texto = "CPF é obrigatório." }; } }
		public Mensagem CpfObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeCPF", Texto = "CPF do cônjuge é obrigatório." }; } }
		public Mensagem DataNascimentoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataNascimento", Texto = "Data de nascimento é obrigatório." }; } }
		public Mensagem DataNascimentoObrigatoriaConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Data de nascimento do cônjuge é obrigatório." }; } }
		
		public Mensagem DataNascimentoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataNascimento", Texto = "Data de nascimento inválida." }; } }
		public Mensagem DataNascimentoInvalidaConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Data de nascimento do cônjuge inválida." }; } }
		
		
		public Mensagem DataNascimentoFuturo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataNascimento", Texto = "Data de nascimento deve ser menor que a data atual." }; } }
		public Mensagem DataNascimentoFuturoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Data de nascimento do cônjuge deve ser menor que a data atual." }; } }

		public Mensagem ObrigatorioRazaoSocial { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_RazaoSocial", Texto = "Razão social é obrigatório." }; } }
		public Mensagem CnpjInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_Cnpj", Texto = "CNPJ inválido." }; } }
		public Mensagem CnpjObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_Cnpj", Texto = "CNPJ é obrigatório." }; } }
		public Mensagem CnpjExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Juridica_Cnpj", Texto = "CNPJ já está cadastrado." }; } }
		public Mensagem CpfExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Cpf", Texto = "CPF já está cadastrado." }; } }
		public Mensagem DataFundacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataFundacao", Texto = "Data da fundação inválida." }; } }
		public Mensagem DataFundacaoFuturo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataFundacao", Texto = "Data da fundação está no futuro." }; } }

		public Mensagem RepresentanteExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este representante já está associado." }; } }
		public Mensagem RepresentanteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Representante_Container", Texto = "Representante é obrigatório." }; } }

		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }

		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Pessoa salva com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Pessoa editada com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Pessoa excluída com sucesso." }; } }

		public Mensagem CpfCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O CPF já está associado a uma pessoa." }; } }
		public Mensagem CnpjCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O CNPJ já está associado a uma pessoa." }; } }
		public Mensagem CpfNaoCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O CPF não está associado a uma pessoa." }; } }
		public Mensagem CnpjNaoCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O CNPJ não está associado a uma pessoa." }; } }

		public Mensagem EnderecoCepInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Cep", Texto = "CEP é inválido." }; } }
		public Mensagem EnderecoCepInvalidoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "CEP do cônjuge é inválido." }; } }

		public Mensagem ResponsavelTecnicoSemProfissao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Não é possível associar um responsável técnico sem profissão." }; } }

		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pessoa inexistente" }; } }

		public Mensagem MensagemExcluir(string nome)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir a pessoa {0}?", nome) };
		}

		public Mensagem NaoPodeSalvarResponsavelTecnicoSemProfissao(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Profissao_ProfissaoTexto", Texto = String.Format("{0} é um responsável técnico, portanto profissão é obrigatória.", nome) };
		}

		public Mensagem ExcluirNaoPermitidoPoisRepresenta(String pessoaCnpj)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois é um representante de {0}", pessoaCnpj) };
		}

		public Mensagem AssociadoEmpreendimento(String emp)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associada ao empreendimento: {0}", emp) };
		}

		public Mensagem AssociadoRequerimento(String req)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pessoa não pode ser excluída, pois está associada ao requerimento: {0}", req) };
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

		public Mensagem Posse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não tem permissão para acessar os dados dessa pessoa." }; } }

		#region Endereco

		public Mensagem EnderecoMunicipioInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_MunicipioId", Texto = "Município é inválido." }; } }
		public Mensagem EnderecoMunicipioInvalidoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Município do cônjuge é inválido." }; } }
				
		public Mensagem EnderecoMunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_MunicipioId", Texto = "Município é obrigatório." }; } }
		public Mensagem EnderecoMunicipioObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Município do cônjuge é obrigatório." }; } }
						
		public Mensagem EnderecoMunicipioOutroEstado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_MunicipioId", Texto = "O estado do município selecionado é diferente do estado selecionado." }; } }
		public Mensagem EnderecoMunicipioOutroEstadoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "O estado do município selecionado do cônjuge é diferente do estado selecionado." }; } }

		public Mensagem EnderecoEstadoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_EstadoId", Texto = "Estado é inválido." }; } }
		public Mensagem EnderecoEstadoInvalidoConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Estado do cônjuge é inválido." }; } }
		
		public Mensagem EnderecoEstadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_EstadoId", Texto = "Estado é obrigatório." }; } }
		public Mensagem EnderecoEstadoObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Estado do cônjuge é obrigatório." }; } }

		public Mensagem LocalidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_DistritoLocalizacao", Texto = "Distrito/Localidade é obrigatório." }; } }
		public Mensagem LocalidadeObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Distrito/Localidade do cônjuge é obrigatório." }; } }
		
		public Mensagem BairroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Bairro", Texto = "Bairro/Gleba é obrigatório." }; } }
		public Mensagem BairroObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Bairro/Gleba do cônjuge é obrigatório." }; } }
		
		public Mensagem LogradouroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Endereco_Logradouro", Texto = "Logradouro é obrigatório." }; } }
		public Mensagem LogradouroObrigatorioConjuge { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Logradouro do cônjuge é obrigatório." }; } }

		#endregion

		#region Cônjuge

		public Mensagem ConjugeExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "Esta pessoa não pode ser excluída, pois possui um cônjuge. Altere a situação do estado civil." }; } }
		public Mensagem ConjugeJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "O cônjuge selecionado já está associado à outra pessoa." }; } }
		public Mensagem PessoaConjugeSaoIguais { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "O cônjuge selecionado é mesmo." }; } }
		public Mensagem ConjugeNaoExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_ConjugeNome", Texto = "O cônjuge selecionado não existe no sistema." }; } }

		#endregion

		public Mensagem ObrigatorioNomeMsg(bool isConjuge = false)
		{
			return isConjuge ? ObrigatorioNomeConjuge : ObrigatorioNome;
		}

		public Mensagem ObrigatorioCPFMsg(bool isConjuge = false)
		{
			return isConjuge ? CpfObrigatorioConjuge : CpfObrigatorio;
		}

		public Mensagem ObrigatorioNacionalidadeMsg(bool isConjuge = false)
		{
			return isConjuge ? ObrigatorioNacionalidadeConjuge : ObrigatorioNacionalidade;
		}

		public Mensagem ObrigatorioNaturalidadeMsg(bool isConjuge)
		{
			return isConjuge ? ObrigatorioNaturalidadeConjuge : ObrigatorioNaturalidade;
		}

		public Mensagem ObrigatorioSexoMsg(bool isConjuge)
		{
			return isConjuge ? ObrigatorioSexoConjuge : ObrigatorioSexo;
		}

		public Mensagem ObrigatorioEstadoCivilMsg(bool isConjuge)
		{
			return isConjuge ? ObrigatorioEstadoCivilConjuge : ObrigatorioEstadoCivil;
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
			return isConjuge ? DataNascimentoFuturoConjuge: DataNascimentoFuturo;
		}

		public Mensagem EnderecoCepInvalidoMsg(bool isConjuge) 
		{
			return isConjuge ? EnderecoCepInvalidoConjuge : EnderecoCepInvalido;
		}

		public Mensagem LogradouroObrigatorioMsg(bool isConjuge)
		{
			return isConjuge ? LogradouroObrigatorioConjuge : LogradouroObrigatorio;
		}

		public Mensagem BairroObrigatorioMsg(bool isConjuge)
		{
			return isConjuge ? BairroObrigatorioConjuge : BairroObrigatorio;
		}

		public Mensagem EnderecoEstadoObrigatorioMsg(bool isConjuge) 
		{
			return isConjuge ? EnderecoEstadoObrigatorioConjuge : EnderecoEstadoObrigatorio;
		}

		public Mensagem EnderecoEstadoInvalidoMsg(bool isConjuge) 
		{
			return isConjuge ? EnderecoEstadoInvalidoConjuge : EnderecoEstadoInvalido;
		}
		
		public Mensagem EnderecoMunicipioObrigatorioMsg(bool isConjuge)
		{
			return isConjuge ? EnderecoMunicipioObrigatorioConjuge : EnderecoMunicipioObrigatorio;
		}

		public Mensagem EnderecoMunicipioInvalidoMsg(bool isConjuge)
		{
			return isConjuge ? EnderecoMunicipioInvalidoConjuge : EnderecoMunicipioInvalido;
		}

		public Mensagem EnderecoMunicipioOutroEstadoMsg(bool isConjuge) 
		{
			return isConjuge ? EnderecoMunicipioOutroEstadoConjuge : EnderecoMunicipioOutroEstado;
		}

		public Mensagem LocalidadeObrigatorioMsg(bool isConjuge)
		{
			return isConjuge ? LocalidadeObrigatorioConjuge : LocalidadeObrigatorio;
		}

		public Mensagem DadosRepresentanteIncompleto(string nome) 
		{
			return new Mensagem() { Texto = String.Format("O representante {0} apresenta dados de obrigatoriedade não preenchidos. Verifique o cadastro do representante.", nome), Tipo = eTipoMensagem.Advertencia };
		}

	}
}