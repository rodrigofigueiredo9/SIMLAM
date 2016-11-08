using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro
{
	public class SalvarCheckListRoteiroVM
	{
		public Int32? IdPdf { get; set; }
		public String Interessado { get; set; }
		public ChecagemRoteiro ChecagemRoteiro { get; set; }
		public List<string> ItensJson { get; set; }
		public List<string> RoteirosJson { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ItemJaAdicionado = Mensagem.ChecagemRoteiro.ItemJaAdicionado,
					MotivoObrigatorio = Mensagem.ChecagemRoteiro.ItemMotivoObrigatorio,
					RoteiroJaAdicionado = Mensagem.ChecagemRoteiro.RoteiroJaAdicionado,
					RemoverItemAssociadoRoteiro = Mensagem.ChecagemRoteiro.RemoverItemAssociadoRoteiro,
					EditarItemAssociadoRoteiro = Mensagem.ChecagemRoteiro.EditarItemAssociadoRoteiro,
					RemoverItemSituacaoInvalida = Mensagem.ChecagemRoteiro.RemoverItemSituacaoInvalida,
					EditarItemSituacaoInvalida = Mensagem.ChecagemRoteiro.EditarItemSituacaoInvalida,
					ConferirItemSituacaoIvalida = Mensagem.ChecagemRoteiro.ConferirItemSituacaoIvalida,
					DispensarItemSituacaoIvalida = Mensagem.ChecagemRoteiro.DispensarItemSituacaoIvalida,
					CancelarDispRecebItemSituacaoIvalida = Mensagem.ChecagemRoteiro.CancelarDispRecebItemSituacaoIvalida,

					ItemNomeObrigatorio = Mensagem.ChecagemRoteiro.ItemNomeObrigatorio,
					SituacaoItemRoteiro = Mensagem.ChecagemRoteiro.SituacaoItemRoteiro,
					ItemAdicionado = Mensagem.Item.ItemAdicionado,
					ItemEditado = Mensagem.Item.Editar,
					ItemEditadoSucesso = Mensagem.Item.ItemEditado,
					RoteiroDesativo = Mensagem.Roteiro.RoteiroDesativo
				});
			}
		}

		public SalvarCheckListRoteiroVM()
		{
			ChecagemRoteiro = new ChecagemRoteiro();
			ItensJson = new List<string>();
			RoteirosJson = new List<string>();
		}

		public SalvarCheckListRoteiroVM(ChecagemRoteiro checkListRoteiro)
		{
			this.ChecagemRoteiro = checkListRoteiro;
			SerelizarItens();
		}

		private void SerelizarItens()
		{
			ItensJson = new List<string>();
			RoteirosJson = new List<string>();

			if (ChecagemRoteiro != null && ChecagemRoteiro.Roteiros != null)
			{
				for (int i = 0; i < ChecagemRoteiro.Roteiros.Count; i++)
				{
					RoteirosJson.Add(ViewModelHelper.Json(ChecagemRoteiro.Roteiros[i]));
				}
			}

			if (ChecagemRoteiro != null && ChecagemRoteiro.Itens != null)
			{
				for (int i = 0; i < ChecagemRoteiro.Itens.Count; i++)
				{
					ItensJson.Add(ViewModelHelper.Json(ChecagemRoteiro.Itens[i]));
				}
			}
		}
	}
}