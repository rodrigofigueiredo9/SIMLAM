using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ConfiguracaoDocumentoFitossanitarioMsg _configuracaoDocumentoFitossanitarioMsg = new ConfiguracaoDocumentoFitossanitarioMsg();

		public static ConfiguracaoDocumentoFitossanitarioMsg ConfiguracaoDocumentoFitossanitario { get { return _configuracaoDocumentoFitossanitarioMsg; } }
	}

	public class ConfiguracaoDocumentoFitossanitarioMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração de numeração salvo com sucesso." }; } }
		public Mensagem NumeroInicialExiste(string tipoDocumento) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Existe número no intervalo do {0} que já está adicionado.", tipoDocumento) }; }

		public Mensagem TipoDocumentoDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Existe número no intervalo do #TIPO# que já está adicionado.", Campo = "NumeroInicial" }; } }

		public Mensagem ValidaBloco { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Número do Bloco é obrigatório.") }; } }

		public Mensagem ValidaDigital { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Número Digital é obrigatório.") }; } }


		public Mensagem TipoDocumentoObrigatorio(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O tipo de documento é obrigatório.", Campo = tipo + "TipoDocumento" };
		}

		public Mensagem NumeroInicialObrigatorio(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número inicial é obrigatório.", Campo = tipo + "NumeroInicial" };
		}

		public Mensagem NumeroInicialInvalido(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número inicial deve ser composto de 10 caracteres.", Campo = tipo + "NumeroInicial" };
		}

		public Mensagem NumeroFinalObrigatorio(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número final é obrigatório.", Campo = tipo + "NumeroFinal" };
		}

		public Mensagem NumeroFinalInvalido(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número final deve ser composto de 10 caracteres.", Campo = tipo + "NumeroFinal" };
		}

		public Mensagem NumeroFinalMaiorInicial(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O número final do {0} não deve ser menor ou igual que o número inicial", tipo), Campo = tipo + "NumeroFinal" };
		}

		public Mensagem NumeroFinalMultiplo25(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O número final do {0} deve ser múltiplo de 25.", tipo), Campo = tipo + "NumeroFinal" };
		}
	}
}