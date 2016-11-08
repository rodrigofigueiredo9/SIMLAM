namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CertificadoRegistroAtividadeFlorestalMsg _certificadoRegistroAtividadeFlorestalMsg =
			new CertificadoRegistroAtividadeFlorestalMsg();

		public static CertificadoRegistroAtividadeFlorestalMsg CertificadoRegistroAtividadeFlorestalMsg
		{
			get { return _certificadoRegistroAtividadeFlorestalMsg; }
			set { _certificadoRegistroAtividadeFlorestalMsg = value; }
		}
	}

	public class CertificadoRegistroAtividadeFlorestalMsg
	{
		public Mensagem ViaObrigatorio
		{
			get { return new Mensagem() {Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_Vias", Texto = "Vias é obrigatório."}; }
		}

		public Mensagem OutrasViaObrigatorio
		{
			get
			{
				return new Mensagem()
					       {Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_ViasOutra", Texto = "Vias é obrigatório."};
			}
		}

		public Mensagem AnoExercicioObrigatorio
		{
			get
			{
				return new Mensagem()
					       {Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_AnoExercicio", Texto = "Exercício é obrigatório."};
			}
		}

		public Mensagem DataVistoriaObrigatoria
		{
			get
			{
				return new Mensagem()
					       {
						       Tipo = eTipoMensagem.Advertencia,
						       Campo = "Certificado_DataVistoria",
						       Texto = "Data de vistoria é obrigatória."
					       };
			}
		}

		public Mensagem ObjetivoObrigatorio
		{
			get
			{
				return new Mensagem()
					       {Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_Objetivo", Texto = "Objetivo é obrigatório."};
			}
		}

		public Mensagem ConstatacaoObrigatoria
		{
			get
			{
				return new Mensagem()
					       {Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_Constatacao", Texto = "Constatação é obrigatória."};
			}
		}

		public Mensagem RegistroAtividadeFlorestalInexistente
		{
			get
			{
				return new Mensagem()
					       {
						       Tipo = eTipoMensagem.Advertencia,
						       Texto = "A caracterizacao Registro Atividade Florestal deve estar cadastrada."
					       };
			}
		}

		public Mensagem CaracterizacaoAtividadeInexistente
		{
			get
			{
				return new Mensagem()
					       {
						       Tipo = eTipoMensagem.Advertencia,
						       Texto =
							       "A atividade selecionada no título deve ser a mesma selecionada na caracterização de Registro Atividade Florestal."
					       };
			}
		}
	}
}