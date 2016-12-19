

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LiberacaoNumeroCFOCFOCMsg _liberacaoNumeroCFOCFOC = new LiberacaoNumeroCFOCFOCMsg();
		public static LiberacaoNumeroCFOCFOCMsg LiberacaoNumeroCFOCFOC
		{
			get { return _liberacaoNumeroCFOCFOC; }
		}
	}

	public class LiberacaoNumeroCFOCFOCMsg
	{
		public Mensagem CPFNaoAssociadoACredenciado { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "O CPF não está associado a nenhum credenciado.", Tipo= eTipoMensagem.Advertencia }; } }

		public Mensagem CPFNaoPossuiRegistroOrgaoClasse { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "A pessoa deve possuir profissão e registro no órgão de classe informados.", Tipo=eTipoMensagem.Advertencia }; } }

		public Mensagem CPFNaoPossuiHabilitacaoCFOCFOC { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "O responsável técnico deve estar habilitado para emissão de CFO e CFOC.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem HabilitacaoCFOCCFOCInativa { get { return new Mensagem() { Campo = "Pessoa_Fisica_CPF", Texto = "A situação da habilitação para emissão de CFO e CFOC do responsável técnico deve ser ativo ou advertido.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CPFObrigatorio { get { return new Mensagem() { Campo = "Pessoa_Fisica_Cpf", Texto = "CPF é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CpfInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_Cpf", Texto = "CPF inválido." }; } }

		public Mensagem NomeObrigatorio { get { return new Mensagem() { Campo = "Liberacao_Nome", Texto = "Nome é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroInicialCFOObrigatorio { get { return new Mensagem() { Campo = "Liberacao_NumeroInicialBlocoCFO", Texto = "Nº inicial do bloco CFO é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroFinalCFOObrigatorio { get { return new Mensagem() { Campo = "Liberacao_NumeroFinalBlocoCFO", Texto = "Nº final do bloco CFO é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroInicialCFOCObrigatorio { get { return new Mensagem() { Campo = "Liberacao_NumeroInicialBlocoCFOC", Texto = "Nº inicial do bloco CFOC é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroFinalCFOCObrigatorio { get { return new Mensagem() { Campo = "Liberacao_NumeroFinalBlocoCFOC", Texto = "Nº final do bloco CFOC é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

        public Mensagem AnoCFOInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Número de Bloco Inválido. O código informado não é do ano atual." }; } }
        public Mensagem AnoCFOCInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Número de Bloco Inválido. O código informado não é do ano atual." }; } }

        public Mensagem QuantidadeDigitalCFOCObrigatorio { get { return new Mensagem() { Campo = "Liberacao_QuantidadeNumeroDigitalCFOC", Texto = "Quantidade de Nº CFOC é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem QuantidadeDigitalCFOObrigatorio { get { return new Mensagem() { Campo = "Liberacao_QuantidadeNumeroCFO", Texto = "Quantidade de Nº CFO é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem IntervaloCFONaoConfigurado { get { return new Mensagem() { Campo = "", Texto = "Existe nº de bloco no intervalo do CFO que não está configurado no sistema.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem IntervaloCFOCNaoConfigurado { get { return new Mensagem() { Campo = "", Texto = "Existe nº de bloco no intervalo do CFOC que não está configurado no sistema.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem IntervaloCFOJaExiste { get { return new Mensagem() { Texto="Existe nº de bloco no intervalo do CFO que já foi atribuído a um responsável técnico", Tipo= eTipoMensagem.Advertencia}; } }
		public Mensagem IntervaloCFOCJaExiste { get { return new Mensagem() { Texto = "Existe nº de bloco no intervalo do CFOC que já foi atribuído a um responsável técnico", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeCFODeveSerIgual25 { get { return new Mensagem() { Texto = "A quantidade de nº de bloco no intervalo do CFO deve ser igual a 25.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeCFOCDeveSerIgual25 { get { return new Mensagem() { Texto="A quantidade de nº de bloco no intervalo do CFOC deve ser igual a 25.", Tipo= eTipoMensagem.Advertencia}; } }

		public Mensagem QuantidadeNumeroDigitalCFOCAcimaDaConfigurada { get { return new Mensagem() { Texto = "A quantidade do nº digital do CFOC somado a outras liberações ultrapassa a quantidade configurada no sistema", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeNumeroDigitalCFOAcimaDaConfigurada { get { return new Mensagem() { Texto = "A quantidade do nº digital do CFO somado a outras liberações ultrapassa a quantidade configurada no sistema", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeNumDigitalCFOUltrapassaConfiguradaSistema { get { return new Mensagem() { Texto = "A quantidade do nº digital do CFO somado a outras liberações ultrapassa a quantidade configurada no sistema.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem QuantidadeNumDigitalCFOCUltrapassaConfiguradaSistema { get { return new Mensagem() { Texto = "A quantidade do nº digital do CFOC somado a outras liberações ultrapassa a quantidade configurada no sistema.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem SalvoSucesso { get { return new Mensagem() { Texto = "Liberação de CFO/CFOC salvo com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem CPFNaoAssociadoAUmaLiberacao { get { return new Mensagem() { Campo = "Pessoa_Fisica_Cpf", Texto = "O responsável técnico não possui número de CFO/CFOC liberados.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroFinalNaoPodeSerMaiorInicialCFO { get { return new Mensagem() { Campo = "Liberacao_NumeroFinalBlocoCFO", Texto = "Numero final do bloco de CFO não pode ser menor que o número inicial.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroFinalNaoPodeSerMaiorInicialCFOC { get { return new Mensagem() { Campo = "Liberacao_NumeroFinalBlocoCFOC", Texto = "Numero final do bloco de CFOC não pode ser menor que o número inicial.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeCFOCadastradaNaoPodeUltrapassar25 { get { return new Mensagem() {  Texto = "A quantidade de nº de bloco do CFO em todas as liberações não deve ultrapassar 25.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem QuantidadeCFOCCadastradaNaoPodeUltrapassar25 { get { return new Mensagem() { Texto = "A quantidade de nº de bloco do CFOC em todas as liberações não deve ultrapassar 25.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeNumeroDigitalCFOCNaoPodeSerMenorIgualZero { get { return new Mensagem() { Texto = "A quantidade de número digital CFOC não pode ser menor ou igual a 0.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeNumeroDigitalCFONaoPodeSerMenorIgualZero { get { return new Mensagem() { Texto = "A quantidade de número digital CFO não pode ser menor ou igual a 0.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem QuantidadeNumerosDigitaisCFONaoPodeExcederAConfiguradaNoSistema { get { return new Mensagem() { Texto = "A quantidade do nº digital do CFO somado a outras liberações ultrapassa a quantidade configurada no sistema.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem QuantidadeNumerosDigitaisCFOCNaoPodeExcederAConfiguradaNoSistema { get { return new Mensagem() { Texto = "A quantidade do nº digital do CFOC somado a outras liberações ultrapassa a quantidade configurada no sistema.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeMaximaNumeroDigitalCFOCadastradosUltrapassa50 { get { return new Mensagem() { Texto = "A quantidade de nº digital do CFO em todas as liberações não deve ultrapassar 50.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeMaximaNumeroDigitalCFOCCadastradosUltrapassa50 { get { return new Mensagem() { Texto = "A quantidade de nº digital do CFOC em todas as liberações não deve ultrapassar 50.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroBlocoCFO_CFOCObrigatorio { get { return new Mensagem() {Texto = "Número do Bloco é obgrigatório.", Tipo = eTipoMensagem.Advertencia } ;} }

		public Mensagem MarqueUmTipoNumero { get { return new Mensagem() { Texto = "Marque o tipo de número a ser liberado.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroCanceladoSucesso(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Número {0} cancelado com sucesso.", numero) };
		}

		public Mensagem DataInvalida(string tipoData, string campo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("{0} inválida.", tipoData) };
		}

		public Mensagem InicialQuantidadeInvalida(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Liberacao_NumeroInicialBloco" + tipo, Texto = String.Format("O número inicial do bloco {0} deve possuir o tamanho igual a 8 caracteres.", tipo) };
		}

		public Mensagem FinalQuantidadeInvalida(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Liberacao_NumeroFinalBloco" + tipo, Texto = String.Format("O número final do bloco {0} deve possuir o tamanho igual a 8 caracteres.", tipo) };
		}

		public Mensagem NumeroNaoEncontrado { get { return new Mensagem() { Texto = "Número não encontrado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem MotivoCancelamentoObrigatorio { get { return new Mensagem() { Texto = "Motivo do cancelamento do número é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CancelarSituacaoInvalida(string tipo, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("{0} {1} já foi cancelado. Visualize o motivo do cancelamento.", tipo, numero) };
		}

		public Mensagem DocumentoJaAssociado(string origemTipo, string origemNumero, string associadoTipo, string associadoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("{0} Nº {1} já utilizado e associado a {2} Nº {3}.", origemTipo, origemNumero, associadoTipo, associadoNumero) };
		}

		public Mensagem DocumentoJaAssociadoEOutros(string origemTipo, string origemNumero, string associadoTipo, string associadoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("{0} Nº {1} já utilizado e associado a {2} Nº {3} e outros.", origemTipo, origemNumero, associadoTipo, associadoNumero) };
		}
	}
}