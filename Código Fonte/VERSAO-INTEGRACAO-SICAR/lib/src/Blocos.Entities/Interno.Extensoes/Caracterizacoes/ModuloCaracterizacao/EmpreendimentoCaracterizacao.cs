using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao
{
	public class EmpreendimentoCaracterizacao
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int? Codigo { get; set; }
		public int InternoID { get; set; }
		public string InternoTID { get; set; }
		public string DenominadorTipo { get; set; }
		public string CNPJ { get; set; }
		public string Denominador { get; set; }
		public eZonaLocalizacao ZonaLocalizacao { get; set; }
		public string ZonaLocalizacaoTexto { get; set; }
		public string Uf { get; set; }
		public int MunicipioId { get; set; }
		public int MunicipioIBGE { get; set; }
		public string Municipio { get; set; }
		public int ModuloFiscalId { get; set; }
		public decimal ModuloFiscalHA { get; set; }
	}
}