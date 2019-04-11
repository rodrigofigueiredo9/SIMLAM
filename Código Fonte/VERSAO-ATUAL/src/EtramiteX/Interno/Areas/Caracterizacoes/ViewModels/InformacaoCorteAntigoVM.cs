using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Antigo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class InformacaoCorteAntigoVM
	{
		public bool IsVisualizar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		private InformacaoCorteAntigo _caracterizacao = new InformacaoCorteAntigo();
		public InformacaoCorteAntigo Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public InformacaoCorteAntigoVM(InformacaoCorteAntigo caracterizacao, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
		}
		
	}
}