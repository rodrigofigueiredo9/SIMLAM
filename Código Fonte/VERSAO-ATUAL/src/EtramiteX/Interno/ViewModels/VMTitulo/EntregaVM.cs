using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class EntregaVM
	{
		private Entrega _entrega = new Entrega();
		public Entrega Entrega
		{
			get { return _entrega; }
			set { _entrega = value; }
		}

		public int Tipo { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ItemJaAssociado = Mensagem.Entrega.ItemJaAssociado,
					@ItemAssociadoSuccess = Mensagem.Entrega.ItemAssociado,
					@CPFObrigatorio = Mensagem.Entrega.CPFObrigatorio
					
				});
			}
		}

		public EntregaVM() { }
	}
}