using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class QueimaControladaVM
	{
		public bool IsVisualizar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		QueimaControlada _caracterizacao = new QueimaControlada();
		public QueimaControlada Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<QueimaControladaQueimaVM> _queimaControladaQueimaVM = new List<QueimaControladaQueimaVM>();
		public List<QueimaControladaQueimaVM> QueimaControladaQueimaVM
		{
			get { return _queimaControladaQueimaVM; }
			set { _queimaControladaQueimaVM = value; }
		}

		public QueimaControladaVM(QueimaControlada caracterizacao, List<Lista> tipoCultivo, bool isVisualizar = false) 
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;

			foreach (QueimaControladaQueima queima in caracterizacao.QueimasControladas)
			{
				QueimaControladaQueimaVM queimaVM = new QueimaControladaQueimaVM(queima, tipoCultivo, isVisualizar);
				QueimaControladaQueimaVM.Add(queimaVM);
			}
		}
	}
}