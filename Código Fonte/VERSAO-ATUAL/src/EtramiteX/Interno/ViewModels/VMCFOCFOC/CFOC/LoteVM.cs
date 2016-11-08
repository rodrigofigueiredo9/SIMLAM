using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFOC
{
	public class LoteVM
	{
		public Lote Lote { get; set; }

		public List<SelectListItem> EmpreendimentoList { get; set; }

		public List<SelectListItem> OrigemList { get; set; }

		public bool IsVisualizar { get; set; }
		public string DataAtual { get { return DateTime.Today.ToShortDateString(); } }

		public string IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					TipoCFO = eDocumentoFitossanitarioTipo.CFO,
					TipoCFOC = eDocumentoFitossanitarioTipo.CFOC,
					TipoPTVOutroEstado = eDocumentoFitossanitarioTipo.PTVOutroEstado,
					TipoCFCFR = eDocumentoFitossanitarioTipo.CFCFR,
					TipoTF = eDocumentoFitossanitarioTipo.TF
				});
			}
		}

		public LoteVM() { }

		public LoteVM(List<ListaValor> lsEmpredimentos, List<Lista> lsOrigem, Lote lote)
		{
			this.Lote = lote ?? new Lote();

			if (lsEmpredimentos.Count == 1)
			{
				this.Lote.EmpreendimentoId = lsEmpredimentos.First().Id;
			}

			//Inicia lista de empreedimento
			EmpreendimentoList = ViewModelHelper.CriarSelectList(lsEmpredimentos, true, true, this.Lote.EmpreendimentoId.ToString());
			OrigemList = ViewModelHelper.CriarSelectList(lsOrigem, true, true);
		}
	}
}