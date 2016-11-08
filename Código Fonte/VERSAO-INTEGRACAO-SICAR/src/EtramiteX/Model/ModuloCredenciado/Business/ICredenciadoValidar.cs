namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business
{
	public interface ICredenciadoIntValidar
	{
		bool AlterarSituacao(int id, int novaSituacao, string motivo);
	}
}

