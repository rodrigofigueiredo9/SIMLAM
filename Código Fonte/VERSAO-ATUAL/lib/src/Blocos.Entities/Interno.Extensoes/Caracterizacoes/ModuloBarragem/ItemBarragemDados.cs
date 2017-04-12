namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem
{
	public class BarragemDadosItem
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public int Identificador { get; set; }
        public string FinalidadeTexto { get; set; }
		public string LaminaAgua { get; set; }
		public decimal? LaminaAguaToDecimal { get { return this.ToDecimal(this.LaminaAgua); } }
		public string VolumeArmazenamento { get; set; }
		public decimal? VolumeArmazenamentoToDecimal { get { return this.ToDecimal(this.VolumeArmazenamento); } }
		public int? OutorgaId { get; set; }
		public string OutorgaTexto { get; set; }
		public string Numero { get; set; }
		public string Tid { get; set; }

		public BarragemDadosItem()
		{
			this.LaminaAgua =
			this.VolumeArmazenamento =
			this.Numero =
			this.Tid = string.Empty;		
		}

		internal decimal? ToDecimal(string strValor)
		{
			decimal decimalValor = 0;

			if (string.IsNullOrEmpty(strValor))
			{
				return null;
			}
			decimal.TryParse(strValor, out decimalValor);
			return decimalValor;
		}
	}
}
