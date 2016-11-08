using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro
{
	public class ItemRoteiroVM
	{
		public Item ItemRoteiro { set; get; }
		public int ModoTela { set; get; }
		public bool IsModal { set; get; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ItemExistente = Mensagem.Roteiro.ItemExistente,
					ItemObrigatorio = Mensagem.Roteiro.ItemObrigatorio,
					ItemAdicionado = Mensagem.Item.ItemAdicionado,
					ItemEditado = Mensagem.Item.Editar,
					ItemEditadoSucesso = Mensagem.Item.ItemEditado
				});
			}
		}

		public ItemRoteiroVM()
		{
			ModoTela = 1;
			ItemRoteiro = new Item();
		}

		public ItemRoteiroVM(Item item)
		{
			ModoTela = 2;
			ItemRoteiro = item;
		}
	}
}