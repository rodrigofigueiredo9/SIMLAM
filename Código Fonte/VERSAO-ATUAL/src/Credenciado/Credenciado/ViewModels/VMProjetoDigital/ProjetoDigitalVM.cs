using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital
{
	public class ProjetoDigitalVM
	{
		public string UrlRequerimento { get; set; }
		public string UrlRequerimentoVisualizar { get; set; }
		public string UrlCaracterizacao { get; set; }
		public string UrlCaracterizacaoVisualizar { get; set; }
		public string UrlEnviar { get; set; }
		public string UrlImprimirDocumentos { get; set; }
		public string UrlEnviarVisualizar { get; set; }
		public bool ModoVisualizar { get; set; }
		public bool PossuiAtividadeCAR { get; set; }
		public bool? PossuiAtividadeBarragem { get; set; }
		public bool DesativarPasso4 { get; set; }

		private ProjetoDigital _projetoDigital = new ProjetoDigital();
		public ProjetoDigital ProjetoDigital
		{
			get { return _projetoDigital; }
			set { _projetoDigital = value; }
		}
	}
}