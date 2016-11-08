using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado
{
	public class AlterarSituacaoVM
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public String CpfCnpj { get; set; }
		public String Situacao { get; set; }
		public Int32 SituacaoId { get; set; }
		public Int32 NovaSituacaoId { get; set; }
		public String Motivo { get; set; }

		

		public List<SelectListItem> Situacoes { get; private set; }

		public AlterarSituacaoVM(List<Situacao> situacoes)
		{
			Situacoes = ViewModelHelper.CriarSelectList(situacoes, true, false);
		}

		
	}
}