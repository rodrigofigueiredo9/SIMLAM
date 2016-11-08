using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia
{
	public class ChecagemPendenciaItem
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public String Tid { get; set; }		
		public String Nome { get; set; }
		public int SituacaoId { get; set; } // Chave estrangeira para lov_checagem_pend_item_sit
		public string SituacaoTexto { get; set; }
		public int ChecagemId { get; set; }

		public static ChecagemPendenciaItem FromAnaliseItemEsp(AnaliseItemEsp analiseItemEsp)
		{
			return new ChecagemPendenciaItem()
			{
				IdRelacionamento = 0,
				Id = analiseItemEsp.Id,
				Nome = analiseItemEsp.Nome,
				Tid = analiseItemEsp.Tid
			};
		}
		
		public ChecagemPendenciaItem() { }
	}
}
