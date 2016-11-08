
namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico
{
	public enum eProjetoGeograficoSituacaoProcessamento
	{
		AguardandoValidacao = 1,
		ExecutantoValidacao = 2,
		ErroValidacao = 3,
		Reprovado = 4,
		Cancelada = 5,
		AguardandoProcessamento = 6,
		Processando = 7,
		ErroProcessamento = 8,
		Processado = 9,
		Cancelado = 10,
		AguardandoGeracaoPDF = 11,
		GerandoPDF = 12,
		ErroGerarPDF = 13,
		ProcessadoPDF = 14,
		CanceladaPDF = 15
	}
}