using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class RespostaInfracaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private RespostaInfracao _entidade = new RespostaInfracao();
		public RespostaInfracao Entidade
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
					@NomeItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.RespostaNomeObrigatorio,
					@ItemDuplicado = Mensagem.FiscalizacaoConfiguracao.RespostaJaAdicionada,
					@EditarItemDesativado = Mensagem.FiscalizacaoConfiguracao.EditarRespostaInfracaoDesativado
				});
			}
		}

		public String Nome
		{
			get
			{
				return "resposta";
			}
		}

		public String TituloGrid
		{
			get
			{
				return "Nome da " + this.Nome;
			}
		}

		public RespostaInfracaoVM(List<Item> itens)
		{
			Entidade.Itens = itens;
		}

		public RespostaInfracaoVM()
		{

		}
	}
}