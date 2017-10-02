using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class ObjetoInfracaoVM
	{
		public Boolean IsVisualizar { get; set; }
		public String ArquivoJSon { get; set; }

		private DateTecno _dataConclusaoFiscalizacao = new DateTecno();
		public DateTecno DataConclusaoFiscalizacao
		{
			get { return _dataConclusaoFiscalizacao; }
			set { _dataConclusaoFiscalizacao = value; }
		}

		private ObjetoInfracao _entidade = new ObjetoInfracao();
		public ObjetoInfracao Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}

		private List<SelectListItem> _series = new List<SelectListItem>();
		public List<SelectListItem> Series
		{
			get { return _series; }
			set { _series = value; }
		}

		private List<SelectListItem> _resultouErosao = new List<SelectListItem>();
		public List<SelectListItem> ResultouErosao
		{
			get { return _resultouErosao; }
			set { _resultouErosao = value; }
		}

		private List<CaracteristicaSoloAreaDanificada> _caracteristicasSolo = new List<CaracteristicaSoloAreaDanificada>();
		public List<CaracteristicaSoloAreaDanificada> CaracteristicasSolo
		{
			get { return _caracteristicasSolo; }
			set { _caracteristicasSolo = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Salvar = Mensagem.ObjetoInfracao.Salvar,
					@ArquivoObrigatorio = Mensagem.ObjetoInfracao.ArquivoObrigatorio,
					@ArquivoNaoEhPdf = Mensagem.ObjetoInfracao.ArquivoNaoEhPdf
				});
			}
		}

		public String TiposArquivoValido = ViewModelHelper.Json(new ArrayList { ".pdf" });

		public ObjetoInfracaoVM(){}

		public ObjetoInfracaoVM(ObjetoInfracao entidade, List<Lista> series, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Entidade = entidade;

			Series = ViewModelHelper.CriarSelectList(series, true, true, selecionado: entidade.SerieId.ToString());
		}
	}
}