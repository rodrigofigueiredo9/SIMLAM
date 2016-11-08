using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCadastro
{
	public class CadastroAmbientalRuralTitulo : Especificidade
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public String Matricula { get; set; }
	}
}