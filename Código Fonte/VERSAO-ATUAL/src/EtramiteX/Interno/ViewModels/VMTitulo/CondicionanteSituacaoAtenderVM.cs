using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class CondicionanteSituacaoAtenderVM
	{
		private TituloCondicionante _condicionante = new TituloCondicionante();
		public TituloCondicionante Condicionante { get { return _condicionante; } set { _condicionante = value; } }
		public int PeriodicidadeId { get; set; }

		public CondicionanteSituacaoAtenderVM() 
		{

		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
				});
			}
		}
	}
}