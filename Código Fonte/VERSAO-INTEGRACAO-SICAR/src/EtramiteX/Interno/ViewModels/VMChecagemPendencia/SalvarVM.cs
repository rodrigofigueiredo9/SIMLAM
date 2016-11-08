using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemPendencia
{
	public class SalvarVM
	{
		private ChecagemPendencia _checagemPendencia = new ChecagemPendencia();
		public ChecagemPendencia ChecagemPendencia { get { return _checagemPendencia; } set { _checagemPendencia = value; } }

		public String ModelosListarTitulo { get; set; }

		public Boolean IsVisualizar { get; set; }
		public Boolean IsEditar { get { return ChecagemPendencia.Id > 0; } }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ItemJaAdicionado = Mensagem.ChecagemRoteiro.ItemJaAdicionado
				});
			}
		}

		public SalvarVM() { }

		public SalvarVM(ChecagemPendencia checagemPendencia)
		{
			this.ChecagemPendencia = checagemPendencia;
			SerelizarItens();
		}

		private void SerelizarItens()
		{
			if (ChecagemPendencia != null && ChecagemPendencia.Itens != null)
			{
				for (int i = 0; i < ChecagemPendencia.Itens.Count; i++)
				{
					//ItensJson.Add(ViewModelHelper.Json(ChecagemPendencia.Itens[i]));
				}
			}
		}
	}
}