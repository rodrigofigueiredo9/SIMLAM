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
	public class MaterialApreendidoVM
	{
		public Boolean IsVisualizar { get; set; }
		public MaterialApreendido MaterialApreendido { get; set; }
		public List<SelectListItem> Series { get; set; }
		public List<SelectListItem> Tipos { get; set; } //vai deixar de existir
        public List<ProdutoApreendidoLst> produtosUnidades { get; set; }
        public List<SelectListItem> ListaProdutosApreendidos { get; set; }
		public List<SelectListItem> Ufs { get; set; }
		public List<SelectListItem> Municipios { get; set; }
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
					@TipoObrigatorio = Mensagem.MaterialApreendidoMsg.TipoObrigatorio,
					@EspecificacaoObrigatorio = Mensagem.MaterialApreendidoMsg.EspecificacaoObrigatorio,
					@MaterialJaAdicionado = Mensagem.MaterialApreendidoMsg.MaterialJaAdicionado,
					@Salvar = Mensagem.MaterialApreendidoMsg.Salvar,

					@ArquivoObrigatorio = Mensagem.MaterialApreendidoMsg.ArquivoObrigatorio,
					@ArquivoNaoEhPdf = Mensagem.MaterialApreendidoMsg.ArquivoNaoEhPdf
				});
			}
		}

		public String TiposArquivoValido = ViewModelHelper.Json(new ArrayList { ".pdf" });

		public MaterialApreendidoVM()
		{
			this.MaterialApreendido = new MaterialApreendido();
			this.Series = new List<SelectListItem>();
			this.Tipos = new List<SelectListItem>();
			this.Ufs = new List<SelectListItem>();
			this.Municipios = new List<SelectListItem>();
            this.ListaProdutosApreendidos = new List<SelectListItem>();
		}
	}
}