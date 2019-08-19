using System.ComponentModel;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
	public enum eCARSolicitacaoSituacao
	{
		Nulo = 0,
		[Description("Em cadastro")]
		EmCadastro = 1,
		[Description("Válido")]
		Valido = 2,
		[Description("Inválido")]
		Invalido = 3,
		[Description("Suspenso")]
		Suspenso = 4,
		[Description("Substituído pelo título CAR")]
		SubstituidoPeloTituloCAR = 5,
		[Description("Pendente")]
		Pendente = 6
	}
}