namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital
{
	public enum eProjetoDigitalSituacao
	{
		EmElaboracao = 1,
		AguardandoImportacao,
		AguardandoCorrecao,
		AguardandoProtocolo,
		AguardandoAnalise,
		EmCorrecao,
		ComPendencia,
		Indeferido,
		Deferido,
		Importado,//so para os passivos(antes de Projeto Digital)
        Finalizado
	}
}