using System;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class Resposta : Lista
	{
		public Boolean? Especificar { get; set; }
		public String EspecificarTexto 
		{ 
			get 
			{
				if (this.Especificar.HasValue) 
				{
					if (this.Especificar.Value) 
					{
						return "Sim";
					}
				}
				return "Não";
			} 
		}
	}
}
