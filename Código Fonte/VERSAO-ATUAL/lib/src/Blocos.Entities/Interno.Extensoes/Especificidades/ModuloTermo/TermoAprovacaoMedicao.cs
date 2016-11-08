using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo
{
	public class TermoAprovacaoMedicao : Especificidade
	{
		public Int32 Id { set; get; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public DateTecno DataMedicao { get; set; }
		public Int32? Funcionario { get; set; }
		public Int32? ResponsavelMedicao { get; set; }
		public Int32? SetorCadastro { get; set; }
		public Int32 TipoResponsavel { get; set; }
		public String Tid { set; get; }

		public TermoAprovacaoMedicao()
		{
			this.Id = 0;
			this.Destinatario = 0;
			this.DataMedicao = new DateTecno();
			this.TipoResponsavel = 0;
			this.Tid = String.Empty;
		}
	}
}