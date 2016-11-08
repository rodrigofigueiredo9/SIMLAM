

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static OficioUsucapiaoMsg _oficioUsucapiaoMsg = new OficioUsucapiaoMsg();
		public static OficioUsucapiaoMsg OficioUsucapiao
		{
			get { return _oficioUsucapiaoMsg; }
			set { _oficioUsucapiaoMsg = value; }
		}
	}

	public class OficioUsucapiaoMsg
	{
		public Mensagem DimensaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Oficio_Dimensao", Texto = "Dimensão da área requerida é obrigatória." }; } }
		public Mensagem DimensaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Oficio_Dimensao", Texto = "Dimensão da área requerida é inválida." }; } }
		public Mensagem DimensaoMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Oficio_Dimensao", Texto = "Dimensão da área requerida deve ser maior do que zero." }; } }

		public Mensagem DestinatarioPGEObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Oficio_Destinatario", Texto = "Destinatário PGE é obrigatório." }; } }
		public Mensagem DescricaoOficioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Oficio_Descricao", Texto = "Descrição do ofício é obrigatório." }; } }
		public Mensagem EmpreedimentoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Oficio_Descricao", Texto = "Tipo de empreendimento é obrigatório." }; } }

		public Mensagem AtividadeInvalida(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo de título ofício de usucapião não pode ser utilizado para atividade {0}.", atividade) };
		}
	}
}
