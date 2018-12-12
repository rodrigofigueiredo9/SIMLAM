namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
    public enum eStatusArquivoSICAR
    {
        Nulo = 0,
        AguardandoEnvio = 1,
        GerandoArquivo = 2,
        ArquivoGerado = 3,
        ArquivoReprovado = 4,
        Enviando = 5,
        ArquivoEntregue = 6,
		Erro = 7,
		ArquivoRetificado = 8,
		Cancelado = 9
    }
}
