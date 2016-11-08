namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade
{
	public class ResiduoSolidoNaoInerte
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }		
		public string ClasseResiduo { get; set; }
		public string Tipo { get; set; }
		public int AcondicionamentoCodigo { get; set; }
		public string AcondicionamentoTexto { get; set; }
		public int EstocagemCodigo { get; set; }
		public string EstocagemTexto { get; set; }
		
		public int TratamentoCodigo { get; set; }
		public string TratamentoTexto { get; set; }
		public string TratamentoDescricao { get; set; }
		public string TratamentoOutros { get { return string.IsNullOrEmpty(this.TratamentoDescricao) ? this.TratamentoTexto : this.TratamentoTexto + " - " + this.TratamentoDescricao + ";"; } }
		
		public int DestinoFinalCodigo { get; set; }
		public string DestinoFinalTexto { get; set; }
		public string DestinoFinalDescricao { get; set; }
		public string DestinoFinalOutros { get { return string.IsNullOrEmpty(this.DestinoFinalDescricao) ? this.DestinoFinalTexto : this.DestinoFinalTexto + " - " + this.DestinoFinalDescricao + ";"; } }

		public string Tid { get; set; }

		public ResiduoSolidoNaoInerte()
		{
			this.ClasseResiduo =
			this.Tipo =
			this.AcondicionamentoTexto =
			this.EstocagemTexto =
			this.TratamentoTexto =
			this.TratamentoDescricao =
			this.DestinoFinalTexto =
			this.DestinoFinalDescricao =
			this.Tid = "";
		}
	}
}
