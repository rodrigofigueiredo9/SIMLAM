using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ArquivoMsg _arquivoMsg = new ArquivoMsg();

		public static ArquivoMsg Arquivo
		{
			get { return _arquivoMsg; }
		}
	}

	public class ArquivoMsg
	{
		public Mensagem ArquivoTempSucesso(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Arquivo {0} enviado com sucesso.", nome) };
		}

		public Mensagem ArquivoTipoInvalido(String nome, List<String> tiposArquivos)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = String.Format("Arquivo {0} não é do tipo: {1}", nome, String.Join(", ", tiposArquivos)) };
		}

		public Mensagem DescricaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Descrição do arquivo é obrigatório." }; } }
		public Mensagem ArquivoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Arquivo inválido." }; } }
		public Mensagem TransferenciaAndamento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Existe uma Transferencia em Andamento." }; } }
		public Mensagem ArquivoExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este arquivo já está associado." }; } }

		public Mensagem NenhumArquivoSelecionadoParaEnviar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Nenhum arquivo foi selecionado para envio." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivo_Nome", Texto = "Selecione o arquivo." }; } }
	}
}
