using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class ItemInfracaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private ItemInfracao _entidade = new ItemInfracao();
		public ItemInfracao Entidade
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
					@NomeItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.ItemNomeObrigatorio,
					@ItemDuplicado = Mensagem.FiscalizacaoConfiguracao.ItemJaAdicionado,
					@EditarItemDesativado = Mensagem.FiscalizacaoConfiguracao.EditarItemInfracaoDesativado
				});
			}
		}

		public String Nome
		{
			get
			{
				return "item";
			}
		}

		public String TituloGrid
		{
			get
			{
				return "Nome do " + this.Nome;
			}
		}

		public ItemInfracaoVM(List<Item> itens)
		{
			Entidade.Itens = itens;
		}

		public ItemInfracaoVM()
		{

		}
	}
}