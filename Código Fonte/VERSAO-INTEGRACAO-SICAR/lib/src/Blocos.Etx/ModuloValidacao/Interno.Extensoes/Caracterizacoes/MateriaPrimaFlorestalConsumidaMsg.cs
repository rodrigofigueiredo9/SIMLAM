namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static MateriaPrimaFlorestalConsumidaMsg _materiaPrimaFlorestalConsumidaMsg = new MateriaPrimaFlorestalConsumidaMsg();
		public static MateriaPrimaFlorestalConsumidaMsg MateriaPrimaFlorestalConsumida
		{
			get { return _materiaPrimaFlorestalConsumidaMsg; }
			set { _materiaPrimaFlorestalConsumidaMsg = value; }
		}
	}

	public class MateriaPrimaFlorestalConsumidaMsg
	{
		public Mensagem MateriaPrimaFlorestalConsumidaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MateriaPrima_MateriaPrimaFlorestalConsumida", Texto = @"Tipo de matéria-prima florestal consumida é obrigatória." }; } }
		public Mensagem MateriaPrimaFlorestalConsumidaDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MateriaPrima_MateriaPrimaFlorestalConsumida", Texto = @"Tipo de matéria-prima florestal consumida ja adicionada." }; } }
		public Mensagem UnidadeMateriaPrimaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MateriaPrima_Unidade", Texto = @"Unidade da matéria-prima florestal consumida é obrigatório." }; } }
		public Mensagem MateriaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de matéria-prima não pode ser vazia." }; } }

		public Mensagem QuantidadeMateriaPrimaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MateriaPrima_Quantidade", Texto = @"Quantidade da matéria-prima florestal consumida é obrigatória." }; } }
		public Mensagem QuantidadeMateriaPrimaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MateriaPrima_Quantidade", Texto = @"Quantidade da matéria-prima florestal consumida é inválida." }; } }
		public Mensagem QuantidadeMateriaPrimaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MateriaPrima_Quantidade", Texto = @"Quantidade da matéria-prima florestal consumida deve ser maior do que zero" }; } }

		public Mensagem EspecificarMateriaPrimaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MateriaPrima_Especificar", Texto = @"Especificar da matéria-prima florestal consumida é obrigatório" }; } }
		
	}

}
