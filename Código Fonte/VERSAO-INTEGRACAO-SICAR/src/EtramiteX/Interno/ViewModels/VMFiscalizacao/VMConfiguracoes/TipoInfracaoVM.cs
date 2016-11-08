using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class TipoInfracaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private TipoInfracao _entidade = new TipoInfracao();
		public TipoInfracao Entidade
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
					@NomeItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.TipoInfracaoNomeObrigatorio,
					@ItemDuplicado = Mensagem.FiscalizacaoConfiguracao.TipoInfracaoJaAdicionado,
					@EditarItemDesativado = Mensagem.FiscalizacaoConfiguracao.EditarTipoInfracaoDesativado
				});
			}
		}

		public String Nome
		{
			get
			{
				return "tipo de infração";
			}
		}

		public String TituloGrid
		{
			get
			{
				return "Nome do " + this.Nome;
			}
		}

		public TipoInfracaoVM(List<Item> itens)
		{
			Entidade.Itens = itens;
		}

	}
}