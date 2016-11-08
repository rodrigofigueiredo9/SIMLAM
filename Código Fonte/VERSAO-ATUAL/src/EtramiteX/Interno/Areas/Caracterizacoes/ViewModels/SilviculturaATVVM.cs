using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class SilviculturaATVVM
	{
		public bool IsVisualizar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }
		public bool TemARL { get; set; }
		public bool TemARLDesconhecida { get; set; }

		SilviculturaATV _caracterizacao = new SilviculturaATV();
		public SilviculturaATV Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<SilviculturaATVCaracteristicaVM> _silviculturaCaracteristicaVM = new List<SilviculturaATVCaracteristicaVM>();
		public List<SilviculturaATVCaracteristicaVM> SilviculturaCaracteristicaVM
		{
			get { return _silviculturaCaracteristicaVM; }
			set { _silviculturaCaracteristicaVM = value; }
		}

		public SilviculturaAreaATV ObterArea(eSilviculturaAreaATV tipo)
		{
			SilviculturaAreaATV area = this.Caracterizacao.Areas.LastOrDefault(x => x.Tipo == (int)tipo) ?? new SilviculturaAreaATV() { Tipo = (int)tipo };

			/*if (string.IsNullOrEmpty(area.ValorTexto))
			{
				area.ValorTexto = "0";
			}*/

			return area;
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SemARLConfirm = Mensagem.SilviculturaAtvMsg.SemARLConfirm,
					@ARLDesconhecidaConfirm = Mensagem.SilviculturaAtvMsg.ARLDesconhecidaConfirm
				});
			}
		}

		public SilviculturaATVVM(SilviculturaATV caracterizacao, List<Lista> tipoCobertura, List<Lista> tipoGeometria, List<Lista> tipoFomento, bool isVisualizar = false) 
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;

			foreach (var silvicultura in caracterizacao.Caracteristicas)
			{
				SilviculturaATVCaracteristicaVM queimaVM = new SilviculturaATVCaracteristicaVM(silvicultura, tipoCobertura, tipoGeometria, tipoFomento, isVisualizar);
				SilviculturaCaracteristicaVM.Add(queimaVM);
			}
		}
	}
}