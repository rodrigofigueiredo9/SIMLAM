using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMRelatorioEspecifico
{
	public class RelatorioMapaVM
	{

		public DateTecno DataInicial { get; set; }

		public DateTecno DataFinal { get; set; }

		private List<SelectListItem> _tipoRelatorio = new List<SelectListItem>();
		public List<SelectListItem> TipoRelatorio
        {
			get { return _tipoRelatorio; }
			set { _tipoRelatorio = value; }
        }
        public string Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
					@EmitidoSucesso = Mensagem.Relatorio.EmitidoSucesso,
					@TipodoRelatorio = Mensagem.Relatorio.TipodoRelatorio, 
					@DataInicialObrigatorio = Mensagem.Relatorio.DataInicialObrigatorio,
					@DataFinalObrigatorio = Mensagem.Relatorio.DataFinalObrigatorio
                });

            }
        }
		public RelatorioMapaVM(List<ListaValor> tipodeRelatorio, int tipoSelecionado) 
        {
			TipoRelatorio = ViewModelHelper.CriarSelectList(tipodeRelatorio, true, true, tipoSelecionado.ToString());
        }


	}
}