

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
	}
}