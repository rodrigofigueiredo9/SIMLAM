

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	public partial class Mensagem
	{
		private static CertidaoDebitoMsg _certidaoDebito = new CertidaoDebitoMsg();
		public static CertidaoDebitoMsg CertidaoDebito
		{
			get { return _certidaoDebito; }
			set { _certidaoDebito = value; }
		}
	}

	public class CertidaoDebitoMsg
	{
		public Mensagem EspecificidadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A especificidade do título deve ser atualizada." }; } }

		public Mensagem AtividadeInvalida(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo de título Certidão de débito não pode ser utilizado para atividade {0}.", atividade) };
		}

	}
}
