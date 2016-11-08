namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class CampoVM
	{
		private int? _id;
		public int? Id { get { return _id ?? 0; } set { _id = value; } }

		private object _valor;
		public object Valor { get { return _valor; } set { _valor = value; } }
	} 
}