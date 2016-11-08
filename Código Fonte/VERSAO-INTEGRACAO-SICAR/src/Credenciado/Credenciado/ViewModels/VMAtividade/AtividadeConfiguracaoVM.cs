using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAtividade
{
	public class AtividadeConfiguracaoVM
	{
		public List<SelectListItem> Modelos { get; set; }

		public String Titulo { get; set; }

		private AtividadeConfiguracao _atividadeConfiguracao = new AtividadeConfiguracao();
		public AtividadeConfiguracao Configuracao
		{
			get { return _atividadeConfiguracao; }
			set { _atividadeConfiguracao = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@AtividadeJaAssociada = Mensagem.AtividadeConfiguracao.AtividadeJaAssociada,
					@AtividadeSetorDiferentes = Mensagem.AtividadeConfiguracao.AtividadeSetorDiferentes,
					@ModeloObrigatorio = Mensagem.AtividadeConfiguracao.ModeloObrigatorio,
					@ModeloJaAssociado = Mensagem.AtividadeConfiguracao.ModeloJaAssociado,
					@AtividadeAssociada = Mensagem.AtividadeConfiguracao.AtividadeAssociada
				});
			}
		}

		public AtividadeConfiguracaoVM()
		{
		}

		public AtividadeConfiguracaoVM(List<TituloModeloLst> modelos)
		{
			SetLista(modelos);
		}

		public void SetLista(List<TituloModeloLst> modelos)
		{
			Modelos = ViewModelHelper.CriarSelectList(modelos, true);
		}
	}
}