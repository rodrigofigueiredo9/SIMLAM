using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens
{
	public class ResultadoMergeItemVM
	{
		private MergeItens _mergeItens = new MergeItens();
		public MergeItens MergeItens
		{
			get { return _mergeItens; }
			set { _mergeItens = value; }
		}

		public List<object> ObterRoteirosSimplificado()
		{
			List<object> retorno = new List<object>();
			foreach (var item in MergeItens.Roteiros)
			{
				retorno.Add(new { Id = item.Id, IdRelacionamento = item.IdRelacionamento, Tid = item.Tid });
			}
			return retorno;
		}

		public ResultadoMergeItemVM() { }
	}
}