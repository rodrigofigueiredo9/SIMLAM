using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class TituloCondicionanteVM
	{
		private List<TituloCondicionante> _condicionantes = new List<TituloCondicionante>();
		public List<TituloCondicionante> Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}

		// Somente deve mostrar botões quando situação é "cadastrado"
		public Boolean MostrarBotoes { get; set; }

		public TituloCondicionanteVM() { }
	}
}