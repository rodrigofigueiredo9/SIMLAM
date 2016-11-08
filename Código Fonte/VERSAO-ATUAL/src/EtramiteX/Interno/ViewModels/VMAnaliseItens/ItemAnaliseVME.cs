using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens
{
	public class ItemAnaliseVME
	{
		public Item _item = new Item();

		public int Id { get { return _item.Id; } set { _item.Id = value; } }
		public int IdRelacionamento { get { return _item.IdRelacionamento; } set { _item.IdRelacionamento = value; } }
		public int Tipo { get { return _item.Tipo; } set { _item.Tipo = value; } }
		public string Nome { get { return _item.Nome; } set { _item.Nome = value; } }
		public int Situacao { get { return _item.Situacao; } set { _item.Situacao = value; } }
		public string SituacaoTexto { get { return _item.SituacaoTexto; } set { _item.SituacaoTexto = value; } }
		public string ProcedimentoAnalise { get { return _item.ProcedimentoAnalise; } set { _item.ProcedimentoAnalise = value; } }

		public string Motivo { get { return _item.Motivo; } set { _item.Motivo = value; } }

		public string Descricao { get { return _item.Descricao; } set { _item.Descricao = value; } }

		public string DataAnalise { get { return _item.DataAnalise; } set { _item.DataAnalise = value; } }
		public int ChecagemId { set; get; }

		private List<SelectListItem> _listaSituacaoItem = new List<SelectListItem>();
		public List<SelectListItem> ListaSituacaoItem
		{
			get { return _listaSituacaoItem.FindAll(x => x.Value != "1"); }
			set { _listaSituacaoItem = value; }
		}

		public Item getItem { get { return _item; } }
		public Item setItem { set { _item = value; } }

		public ItemAnaliseVME(){ }

		public void SetLista(List<Situacao> listaSituacaoItem)
		{
			if (Tipo == (int)eRoteiroItemTipo.ProjetoDigital)
			{
				listaSituacaoItem = listaSituacaoItem.Where(x => x.Id == (int)eAnaliseItemSituacao.AprovadoImportacao).ToList();
			}

			if (Tipo == (int)eRoteiroItemTipo.Tecnico || Tipo == (int)eRoteiroItemTipo.Administrativo) 
			{
				listaSituacaoItem = listaSituacaoItem.Where(x => x.Id == (int)eAnaliseItemSituacao.Aprovado ||
																 x.Id == (int)eAnaliseItemSituacao.Dispensado ||
																 x.Id == (int)eAnaliseItemSituacao.Pendente ||
																 x.Id == (int)eAnaliseItemSituacao.Recebido ||
																 x.Id == (int)eAnaliseItemSituacao.Reprovado).ToList();
			}

			ListaSituacaoItem = ViewModelHelper.CriarSelectList(listaSituacaoItem, true, true);
		}
	}
}