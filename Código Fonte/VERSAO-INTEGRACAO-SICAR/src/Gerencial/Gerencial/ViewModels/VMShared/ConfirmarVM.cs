using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Gerencial.ViewModels
{
	public class ConfirmarVM
	{
		public int Id { get; set; }
		public string Titulo { get; set; }
		public Mensagem Mensagem { get; set; }

		public ConfirmarVM() { }

		public ConfirmarVM(string titulo, string msg)
		{
			Titulo = titulo;
			Mensagem = new Mensagem();
			Mensagem.Texto = msg;
		}
	}
}