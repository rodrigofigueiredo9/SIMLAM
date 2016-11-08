using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public class EspecificidadeValidarDefault : IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade objEspecificidade)
		{
			return true;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return true;
		}
	}
}