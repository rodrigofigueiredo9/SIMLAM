namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao
{
	public class LogonVM
	{
		public bool AlterarSenha { get; set; }
		public string AlterarSenhaMsg { get; set; }
		public bool IsAjaxRequest { get; set; }
        public bool? EsqueciSenha { get; set; }
	}
}