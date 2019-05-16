using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	partial class Mensagem
	{
		private static RetificacaoNFCaixaMsg _emissaoRetificacaoNFCaixaMsg = new RetificacaoNFCaixaMsg();

		public static RetificacaoNFCaixaMsg RetificacaoNFCaixa
		{
			get { return _emissaoRetificacaoNFCaixaMsg; }
			set { _emissaoRetificacaoNFCaixaMsg = value; }
		}
	}

	public class RetificacaoNFCaixaMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Texto = "Nota fiscal de caixa salva com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem Alterado { get { return new Mensagem() { Texto = "PTV alterado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem Excluido { get { return new Mensagem() { Texto = "Nota fiscal de caixa excluído com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem NaoPodeExcluir { get { return new Mensagem() { Texto = "Não é possível excluir a nota fiscal de caixa. Existem PTVs associadas a ela.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem MensagemExcluirConfirm { get { return new Mensagem() { Texto = "Tem certeza que deseja excluir a nota fiscal de caixa? ", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem AtivadoSucesso(string numero)
		{
			return new Mensagem() { Texto = string.Format("PTV Nº {0} salvo com sucesso.", numero), Tipo = eTipoMensagem.Sucesso };
		}

		public Mensagem CNPJObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CNPJ é obrigatório.", Campo = "CPFCNPJ" }; } }

		public Mensagem CNPJInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CNPJ é inválido.", Campo = "CPFCNPJ" }; } }

		public Mensagem CPFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CPF é obrigatório.", Campo = "CPFCNPJ" }; } }

		public Mensagem CPFInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O CPF é inválido.", Campo = "CPFCNPJ" }; } }

	}
}