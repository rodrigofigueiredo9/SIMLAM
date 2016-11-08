using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso
{
	public class JuntarApensarVM
	{
		public Boolean IsProcessoValido {
			get { return Processo != null && Processo.Id != null && Processo.Id.HasValue; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					DocumentoJaListadoJuntar = Mensagem.Processo.DocumentoJaEstaNaListaParaSerJuntado,
					ProcessoJaListadoJuntar = Mensagem.Processo.ProcessoJaEstaNaListaParaSerJuntado
				});
			}
		}

		private Processo _processo = new Processo();
		public Processo Processo
		{
			get { return _processo; }
			set { _processo = value; }
		}

		public JuntarApensarVM() { }
	}
}