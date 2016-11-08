using System.Collections.Generic;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;

namespace Tecnomapas.EtramiteX.Gerencial.Areas.Relatorios.ViewModels
{
	public class PersonalizadoExecutarVME
	{
		public int Id { get; set; }
		public int Tipo { get; set; }
		public int Setor { get; set; }
		public List<Termo> Termos { get; set; }

		public PersonalizadoExecutarVME()
		{
			Termos = new List<Termo>();
		}
	}
}