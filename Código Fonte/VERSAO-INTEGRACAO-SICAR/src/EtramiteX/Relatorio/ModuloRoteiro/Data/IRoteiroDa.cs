

using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRoteiro.Data
{
	public interface IRoteiroDa
	{
		RoteiroRelatorio Obter(int id);
		RoteiroRelatorio Obter(int id, string tid);
	}
}
