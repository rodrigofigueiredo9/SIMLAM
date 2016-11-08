

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public interface IPessoaMsg
	{
		Mensagem ObrigatorioRg { get; }
		Mensagem ObrigatorioEstadoCivil { get; }
		Mensagem ObrigatorioSexo { get; }
		Mensagem ObrigatorioNacionalidade { get; }
		Mensagem ObrigatorioNaturalidade { get; }
		Mensagem CnpjCadastrado { get; }
		Mensagem CnpjInvalido { get; }
		Mensagem CnpjObrigatorio { get; }
		Mensagem CpfCadastrado { get; }
		Mensagem CpfInvalido { get; }
		Mensagem CpfObrigatorio { get; }
		Mensagem DataFundacaoFuturo { get; }
		Mensagem DataFundacaoInvalida { get; }
		Mensagem DataNascimentoFuturo { get; }
		Mensagem DataNascimentoObrigatoria { get; }
		Mensagem DataNascimentoInvalida { get; }
		Mensagem Editar { get; }
		Mensagem EnderecoCepInvalido { get; }
		Mensagem EnderecoEstadoInvalido { get; }
		Mensagem EnderecoMunicipioInvalido { get; }
		Mensagem EnderecoMunicipioOutroEstado { get; }
		Mensagem NaoEncontrouRegistros { get; }
		Mensagem ObrigatorioNome { get; }
		Mensagem ObrigatorioRazaoSocial { get; }
		Mensagem ExcluirNaoPermitidoPoisRepresenta(string pessoaCnpj);
		Mensagem RepresentanteExistente { get; }
		Mensagem RepresentanteObrigatorio { get; }
		Mensagem Salvar { get; }
		Mensagem CpfExistente { get; }
		Mensagem CnpjExistente { get; }
		Mensagem AssociadoEmpreendimento(String emp);
		Mensagem AssociadoRequerimento(String req);
		Mensagem AssociadoDocumento(String doc);
		Mensagem AssociadoProcesso(String pro);
		Mensagem AssociadoTitulo(String tit);
		Mensagem AssociadoTituloEntrega(String tit);
		Mensagem AssociadoCredenciado { get; }
		Mensagem ResponsavelTecnicoSemProfissao { get; }

		Mensagem DataNascimentoInvalidaMsg(bool isConjge);
		Mensagem DataNascimentoFuturoMsg(bool isConjuge);
		Mensagem EnderecoCepInvalidoMsg(bool isConjuge);

		Mensagem EnderecoEstadoInvalidoMsg(bool isConjuge);
		Mensagem EnderecoMunicipioInvalidoMsg(bool isConjuge);
		Mensagem EnderecoMunicipioOutroEstadoMsg(bool isConjuge);
	}
}