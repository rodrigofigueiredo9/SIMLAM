using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMUnidadeProducao
{
	public class UnidadeProducaoVM
	{
		public Boolean IsVisualizar { get; set; }
		public Boolean RetornarVisualizar { get; set; }
		public UnidadeProducao UnidadeProducao { get; set; }

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(
					new
					{
						@CodigoPropriedadeObrigatorio = Mensagem.UnidadeProducao.CodigoPropriedadeObrigatorio,
						@CodigoUPJaExiste = Mensagem.UnidadeProducao.CodigoUPJaExiste,
						@LocalLivroDisponivelObrigatorio = Mensagem.UnidadeProducao.LocalLivroDisponivelObrigatorio,
						@UnidadeProducaoObrigatorio = Mensagem.UnidadeProducao.UnidadeProducaoObrigatorio,
					});
			}
		}

		public UnidadeProducaoVM(UnidadeProducao unidade)
		{
			this.UnidadeProducao = unidade;
		}
	}
}