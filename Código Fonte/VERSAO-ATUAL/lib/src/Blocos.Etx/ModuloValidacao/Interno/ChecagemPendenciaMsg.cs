using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ChecagemPendenciaMsg _checagemPendenciaMsg = new ChecagemPendenciaMsg();

		public static ChecagemPendenciaMsg ChecagemPendencia
		{
			get { return _checagemPendenciaMsg; }
		}
	}

	public class ChecagemPendenciaMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Checagem de pendência excluída com sucesso." }; } }
		public Mensagem SalvarChecagemItemNaoConferido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível salvar checagem com item não conferido." }; } }

		public Mensagem TituloJaEstaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O título já está associado a outra checagem de pendência." }; } }
		public Mensagem TituloDeveEstarConcluidoProrrogado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O título deve estar concluído ou prorrogado para ser associado." }; } }
		public Mensagem TituloNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Títlo inválido ou inexistente." }; } }

		public Mensagem NaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Checagem de pendência inválida ou inexistente." }; } }
		public Mensagem NaoPodeSerAlteradaPoisEstaFinalizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Checagem de pendência não pode ser editada pois está finalizada." }; } }

		public Mensagem TituloDeveSerDoTipoConfigurado(List<string> nomeDoModeloDoTitulo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo título associado deve ser do tipo \"{0}\".", Mensagem.Concatenar(nomeDoModeloDoTitulo)) };
		}

		public Mensagem ConfirmarFinalizarChecagem { get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "A checagem será finalizada não podendo ser alterada posteriormente. Deseja finalizar a checagem?" }; } }

		public Mensagem MensagemExcluir(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir a checagem de pendência número {0}?", numero) };
		}

		public Mensagem ExcluirNaoPoisAssociadoDocumento(string numeroDocumento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A checagem de pendêncianão pode ser excluída, pois está associada ao documento {0}.", numeroDocumento) };
		}

		public Mensagem Salvar(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Checagem de pendência salva com sucesso. Número da checagem {0}.", numero) };
		}

		public Mensagem TituloSemItem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O título deve conter ao menos um item." }; } }

		public Mensagem ItemNaoEhDoTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Existem itens não provenientes do título." }; } }

		public Mensagem ItemNaoEhDaChecagem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Existem itens não provenientes da checagem de pendência." }; } }

		public Mensagem FinalizarMensagem { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "A checagem será finalizada não podendo ser alterada posteriormente. Deseja finalizar a checagem?" }; } }

		public Mensagem FinalizarTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Finalizar checagem de pendência?" }; } }
	}
}