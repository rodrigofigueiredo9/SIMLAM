using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class SubItemInfracaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private SubItemInfracao _entidade = new SubItemInfracao();
		public SubItemInfracao Entidade
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
					@NomeItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.SubItemNomeObrigatorio,
					@ItemDuplicado = Mensagem.FiscalizacaoConfiguracao.SubItemJaAdicionado,
					@EditarItemDesativado = Mensagem.FiscalizacaoConfiguracao.EditarSubItemInfracaoDesativado
				});
			}
		}

		public String Nome 
		{
			get 
			{
				return "subitem";
			}
		}

		public String TituloGrid
		{
			get
			{
				return "Nome do "+this.Nome;
			}
		}

		public SubItemInfracaoVM(List<Item> itens)
		{
			Entidade.Itens = itens;
		}

		public SubItemInfracaoVM()
		{

		}
	}
}