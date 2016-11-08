using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class EnquadramentoVM
	{
		public Boolean IsVisualizar { get; set; }

		private Enquadramento _entidade = new Enquadramento();
		public Enquadramento Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}


		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ListaArtigosObrigatoria = Mensagem.Enquadramento.ListaArtigosObrigatoria,
					@Salvar = Mensagem.Enquadramento.Salvar
				});
			}
		}

		public EnquadramentoVM()
		{

		}

		public EnquadramentoVM(Enquadramento entidade)
		{
			Entidade = entidade;
		}
	}
}