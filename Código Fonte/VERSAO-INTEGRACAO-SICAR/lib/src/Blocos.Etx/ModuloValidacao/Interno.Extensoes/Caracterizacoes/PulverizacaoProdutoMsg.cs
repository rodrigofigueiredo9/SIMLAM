namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static PulverizacaoProdutoMsg _pulverizacaoProdutoMsg = new PulverizacaoProdutoMsg();
		public static PulverizacaoProdutoMsg PulverizacaoProduto
		{
			get { return _pulverizacaoProdutoMsg; }
			set { _pulverizacaoProdutoMsg = value; }
		}
	}


	public class PulverizacaoProdutoMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de pulverização aérea de produtos agrotóxicos excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de pulverização aérea de produtos agrotóxicos salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProduto_Atividade", Texto = @"Atividade da pulverização aérea de produtos agrotóxicos é obrigatório." }; } }

		public Mensagem EmpresaPrestadoraObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProduto_EmpresaPrestadora", Texto = @"Empresa prestadora de serviço de aplicação aérea de agrotóxicos é obrigatória." }; } }
		public Mensagem CnpjObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProduto_CNPJ", Texto = @"CNPJ é obrigatório." }; } }
		public Mensagem CnpjInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProduto_CNPJ", Texto = "CNPJ inválido." }; } }

		public Mensagem CulturaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de culturas não pode ser vazia." }; } }
		public Mensagem CulturaTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProdutoCultura_Tipo", Texto = @"Tipo de cultura é obrigatório." }; } }
		public Mensagem CulturaTipoDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProdutoCultura_Tipo", Texto = @"Tipo de cultura já está adicionada." }; } }

		public Mensagem CulturaEspecificarTextoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProdutoCultura_EspecificarTexto", Texto = @"Especificar cultura é obrigatório." }; } }

		public Mensagem CulturaAreaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProdutoCultura_Area", Texto = @"Área de cultura é obrigatória." }; } }
		public Mensagem CulturaAreaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProdutoCultura_Area", Texto = @"Área de cultura é inválida." }; } }
		public Mensagem CulturaAreaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PulverizacaoProdutoCultura_Area", Texto = @"Área de cultura deve ser maior do que zero." }; } }

		public Mensagem SemARLConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade não possui Reserva Legal. Deseja realmente continuar?" }; } }
		public Mensagem ARLDesconhecidaConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade possui Reserva Legal sem situação vegetal definida. Deseja realmente continuar?" }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de pulverização aérea de produtos agrotóxicos deste empreendimento?" }; } }
	}
}
