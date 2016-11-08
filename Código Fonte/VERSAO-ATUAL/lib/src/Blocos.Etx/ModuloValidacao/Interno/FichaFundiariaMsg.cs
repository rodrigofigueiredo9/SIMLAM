namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static FichaFundiariaMsg _fichaFundiaria = new FichaFundiariaMsg();
		public static FichaFundiariaMsg FichaFundiaria
		{
			get { return _fichaFundiaria; }
		}
	}

	public class FichaFundiariaMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Ficha fundiária salva com sucesso." }; } }

		public Mensagem RequerenteNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "FichaFundiaria_Requerente_Nome", Texto = "Nome do requerente é obrigatório." }; } }

		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Ficha Fundiária excluída com sucesso." }; } }
		public Mensagem ExcluirConfirmacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Tem certeza que deseja excluir essa Ficha Fundiária? " }; } }
	}
}
