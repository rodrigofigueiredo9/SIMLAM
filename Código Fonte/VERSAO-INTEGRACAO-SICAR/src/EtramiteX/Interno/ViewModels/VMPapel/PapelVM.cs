using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPapel
{
	public class PapelVM
	{
		public int ID { get; set; }
		public String Nome { get; set; }
		public List<PermissaoGrupo> GrupoColecao { get; set; }
	}
}