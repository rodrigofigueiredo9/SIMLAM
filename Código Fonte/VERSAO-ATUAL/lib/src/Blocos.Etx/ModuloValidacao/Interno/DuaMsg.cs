

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static DuaMsg _DuaMsg = new DuaMsg();

		public static DuaMsg Dua
		{
			get { return _DuaMsg; }
		}
	}

	public class DuaMsg
	{
		public Mensagem Invalido(string numero)
		{
			return new Mensagem() { Texto = String.Format("O DUA {0} não é valido.", numero), Tipo = eTipoMensagem.Advertencia };
		}
		public Mensagem SemSaldo(string numero)
		{
			return new Mensagem() { Texto = String.Format("O DUA {0} não possui saldo.", numero), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem NaoEncontrado { get { return new Mensagem() { Campo = "", Texto = "Dua não encontrado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ErroSefaz(string xMotivo) { return new Mensagem() { Campo = "", Texto = "Sefaz informa: " + xMotivo, Tipo = eTipoMensagem.Advertencia }; }
		public Mensagem ErroAoConsultar { get { return new Mensagem() { Campo = "", Texto = "As tentativas de acesso ao serviço do DUA foram esgotadas, por favor entre em contato com o administrador do sistema.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroObrigatorio { get { return new Mensagem() { Campo = "NumeroDua", Texto = "Número do DUA é Obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CPFCNPJObrigatorio { get { return new Mensagem() { Campo = "CPFCNPJDUA, .txtCNPJDUA", Texto = "CPF/CNPJ é Obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CodigoInvalido { get { return new Mensagem() { Texto = "Código do DUA inválido.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem Sucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "DUAs emitidos com sucesso." }; } }
		public Mensagem Falha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Falha ao emitir DUAs." }; } }
		public Mensagem ExisteDuaTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível editar esse título, porque todos os DUAS para esse título já foram emitidos." }; } }
		public Mensagem SolicitacaoSituacaoNaoPodeEditar(string situacaoAtual)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Situacao", Texto = String.Format("Não é possível editar a Solicitação de inscrição na situação \"{0}\".", situacaoAtual) };
		}

	}
}