using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Data;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business
{
	public class ListasBus
	{
		ListasDa _da = new ListasDa();

		public Listas GetListas()
		{
			Listas result = new Listas();

			result.atividades = _da.GetAtividades();
			result.segmentos = _da.GetSegmentos();
			result.municipios = _da.GetMunicipios();

			return result;
		}
	}
}