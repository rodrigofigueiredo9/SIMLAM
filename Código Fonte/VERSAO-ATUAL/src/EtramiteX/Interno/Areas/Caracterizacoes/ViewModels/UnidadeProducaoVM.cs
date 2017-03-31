using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class UnidadeProducaoVM
	{
		public bool IsVisualizar { get; set; }
		public string UrlRetorno { get; set; }
		public UnidadeProducao UnidadeProducao { get; set; }
        public string Situacao { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@CodigoPropriedadeObrigatorio = Mensagem.UnidadeProducao.CodigoPropriedadeObrigatorio,
					@CodigoUPJaExiste = Mensagem.UnidadeProducao.CodigoUPJaExiste,
					@LocalLivroDisponivelObrigatorio = Mensagem.UnidadeProducao.LocalLivroDisponivelObrigatorio,
					@UnidadeProducaoObrigatorio = Mensagem.UnidadeProducao.UnidadeProducaoObrigatorio,
				});
			}
		}

		public UnidadeProducaoVM()
		{
			UnidadeProducao = new UnidadeProducao();
		}
	}
}