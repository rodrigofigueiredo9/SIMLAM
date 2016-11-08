using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class TituloAdicionarVM
	{
		public string Indice { get; set; }
		public Boolean IsVisualizar { get; set; }
		public String SiglaOrgao { get; set; }
		public FinalidadeCaracterizacao Finalidade { get; set; }
		public List<SelectListItem> Titulos { get; set; }

		public TituloAdicionarVM() : this(new List<TituloModeloLst>(), new FinalidadeCaracterizacao(), null) { }

		public TituloAdicionarVM(List<TituloModeloLst> modelos, FinalidadeCaracterizacao finalidade, string indice, bool isVisualizar = false)
		{
			Indice = indice;
			IsVisualizar = isVisualizar;
			SiglaOrgao = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema()).Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla);

			Finalidade = finalidade;
			Titulos = ViewModelHelper.CriarSelectList(modelos, true, true, Finalidade.TituloModelo.ToString());
		}
	}
}