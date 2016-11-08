using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public interface IEspecificiadeValidar
	{
		bool Salvar(IEspecificidade especificidade);
		bool Emitir(IEspecificidade especificidade);
	}
}