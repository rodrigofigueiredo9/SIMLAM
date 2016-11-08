using System.Collections.Generic;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.AtividadeEspecificidade
{
	public class EspecificidadeAtividadeCaracterizacaoVME
	{
		public int AtividadeId { get; set; }
		public int EmpreendimentoId { get; set; }
		public string DadoAuxiliarJSON { get; set; }
		public Dictionary<string, object> _dadoAuxiliarJSONDictionary;
		public Dictionary<string, object> DadoAuxiliarJSONDictionary { get { return this._dadoAuxiliarJSONDictionary = _dadoAuxiliarJSONDictionary ?? ViewModelHelper.ParseJSON<Dictionary<string, object>>(string.IsNullOrEmpty(this.DadoAuxiliarJSON) ? "{}" : this.DadoAuxiliarJSON); } }
	}
}