using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRequerimento.Data
{
	public interface IRelatorioRequerimentoDa
	{
		int ObterSituacao(int id);
		RequerimentoRelatorio Obter(int id, BancoDeDados banco = null);
		RequerimentoRelatorio ObterHistorico(int id, BancoDeDados banco = null);
		int RoteiroPadrao { get; }
	}
}