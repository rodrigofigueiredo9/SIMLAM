using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class MultaVM
	{
		public Boolean IsVisualizar { get; set; }
		public Multa Multa { get; set; }
		public List<SelectListItem> Series { get; set; }
        public List<SelectListItem> CodigosReceita { get; set; }
		public String ArquivoJSon { get; set; }

		private DateTecno _dataConclusaoFiscalizacao = new DateTecno();
		public DateTecno DataConclusaoFiscalizacao
		{
			get { return _dataConclusaoFiscalizacao; }
			set { _dataConclusaoFiscalizacao = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
                    @Salvar = Mensagem.MultaMsg.Salvar,

					@ArquivoObrigatorio = Mensagem.MultaMsg.ArquivoObrigatorio,
					@ArquivoNaoEhPdf = Mensagem.MaterialApreendidoMsg.ArquivoNaoEhPdf
				});
			}
		}

		public String TiposArquivoValido = ViewModelHelper.Json(new ArrayList { ".pdf" });

		public MultaVM()
		{
			this.Multa = new Multa();
			this.Series = new List<SelectListItem>();
			this.CodigosReceita = new List<SelectListItem>();
		}
	}
}