using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class DestinatarioEmailVM
	{
		private List<DestinatarioEmail> _destinatarios = new List<DestinatarioEmail>();
		public List<DestinatarioEmail> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		public bool IsVisualizar { get; set; }

		public DestinatarioEmailVM() { }

		internal void MergeDestinatario(List<DestinatarioEmail> tituloDestinatarios)
		{
			//Seleciona os assinates			
			foreach (var destinatario in this.Destinatarios)
			{
				// tenta encontrar funcionário do título dentro da lista de funcionários
				var destEncontrado = tituloDestinatarios.FirstOrDefault(x => x.PessoaId == destinatario.PessoaId);
				if (destEncontrado != null)
				{
					destinatario.Selecionado = true;
				}
			}

			foreach (var destinatario in tituloDestinatarios)
			{
				if (!this.Destinatarios.Exists(x => x.PessoaId == destinatario.PessoaId))
				{
					destinatario.Selecionado = true;
					this.Destinatarios.Add(destinatario);
				}
			}
		}
	}
}