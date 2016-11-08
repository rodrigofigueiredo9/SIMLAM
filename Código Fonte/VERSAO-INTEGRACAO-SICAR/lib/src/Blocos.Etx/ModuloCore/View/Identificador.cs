namespace Tecnomapas.Blocos.Etx.ModuloCore.View
{
	public class Identificador
	{
		public string Valor { get; set; }
		public string Classe { get; set; }

		public Identificador(object valor, string classe)
		{
			Valor =  valor == null ? "" : valor.ToString();
			Classe = classe;
		}
	}
}
