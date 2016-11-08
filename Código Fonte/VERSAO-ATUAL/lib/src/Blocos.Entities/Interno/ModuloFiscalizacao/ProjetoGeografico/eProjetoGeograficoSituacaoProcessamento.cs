
namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public enum eProjetoGeograficoSituacaoProcessamento
	{
		AguardandoValidacao = 1,
		ExecutantoValidacao,
		ErroValidacao,
		Reprovado,
		Cancelada,
		AguardandoProcessamento,
		Processando,
		ErroProcessamento,
		Processado,
		Cancelado,
		AguardandoGeracaoPDF,
		GerandoPDF,
		ErroGerarPDF
	}
}