namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital
{
	public enum eProjetoDigitalSituacao
	{
		EmElaboracao = 1,
		AguardandoImportacao = 2,
		AguardandoCorrecao = 3,
		AguardandoProtocolo = 4,
		AguardandoAnalise = 5,
		EmCorrecao = 6,
		ComPendencia = 7,
		Indeferido = 8,
		Deferido = 9,
		Importado = 10,//so para os passivos(antes de Projeto Digital)
        Finalizado = 11
	}
}