using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class DominialidadeVM
	{
		public Boolean IsVisualizar { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }
		public List<SelectListItem> BooleanLista { get; set; }
		public int ProjetoDigitalId { get; set; }
		public int ProtocoloId { get; set; }
		public int RequerimentoId { get; set; }
		public String UrlRetorno { get; set; }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@TipoMatriculaId = eDominioTipo.Matricula
				});
			}
		}

		private Dominialidade _caracterizacao = new Dominialidade();
		public Dominialidade Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public DominialidadeVM(Dominialidade caracterizacao, List<Lista> booleanLista, bool isVisualizar = false)
		{
			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;

			int selecionado = -1;

			if (caracterizacao.PossuiAreaExcedenteMatricula.HasValue)
			{
				selecionado = caracterizacao.PossuiAreaExcedenteMatricula.GetValueOrDefault();
			}

			BooleanLista = ViewModelHelper.CriarSelectList(booleanLista, itemTextoPadrao: false, selecionado: selecionado.ToString());
		}

		public DominialidadeArea ObterArea(eDominialidadeArea tipo)
		{
			DominialidadeArea area = this.Caracterizacao.Areas.LastOrDefault(x => x.Tipo == (int)tipo) ?? new DominialidadeArea() { Tipo = (int)tipo };

			return area;
		}
	}
}