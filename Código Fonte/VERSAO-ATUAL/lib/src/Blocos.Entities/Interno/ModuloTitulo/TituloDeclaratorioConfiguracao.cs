namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloDeclaratorioConfiguracao
	{
		public int Id { get; set; }
		public decimal MaximoAreaAlagada { get; set; }
		public decimal MaximoVolumeArmazenado { get; set; }
		private Arquivo.Arquivo _barragemSemAPP = new Arquivo.Arquivo();
		public Arquivo.Arquivo BarragemSemAPP
		{
			get { return _barragemSemAPP; }
			set { _barragemSemAPP = value; }
		}
		private Arquivo.Arquivo _barragemComAPP = new Arquivo.Arquivo();
		public Arquivo.Arquivo BarragemComAPP
		{
			get { return _barragemComAPP; }
			set { _barragemComAPP = value; }
		}
		public string Tid { get; set; }
	}
}
