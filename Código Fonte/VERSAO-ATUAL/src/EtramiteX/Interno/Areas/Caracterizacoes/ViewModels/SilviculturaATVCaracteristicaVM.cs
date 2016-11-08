using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class SilviculturaATVCaracteristicaVM
	{
		public bool IsVisualizar { get; set; }

		private SilviculturaCaracteristicaATV _caracterizacao = new SilviculturaCaracteristicaATV();
		public SilviculturaCaracteristicaATV Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<SelectListItem> _tipoCobertura = new List<SelectListItem>();
		public List<SelectListItem> TipoCobertura
		{
			get { return _tipoCobertura; }
			set { _tipoCobertura = value; }
		}

		private List<SelectListItem> _geometriaTipo = new List<SelectListItem>();
		public List<SelectListItem> GeometriaTipo
		{
			get { return _geometriaTipo; }
			set { _geometriaTipo = value; }
		}

		private List<SelectListItem> _fomentoTipo = new List<SelectListItem>();
		public List<SelectListItem> FomentoTipo
		{
			get { return _fomentoTipo; }
			set { _fomentoTipo = value; }
		}


		public String Mensagens
		{
			get
			{

				return ViewModelHelper.Json(new
				{
					@TipoCulturaObrigatorio = Mensagem.SilviculturaAtvMsg.TipoCulturaObrigatorio,
					@TipoCulturaJaAdicionado = Mensagem.SilviculturaAtvMsg.TipoCulturaJaAdicionado,
					@AreaCulturaObrigatoria = Mensagem.SilviculturaAtvMsg.AreaCulturaObrigatoria,
					@AreaCulturaInvalida = Mensagem.SilviculturaAtvMsg.AreaCulturaInvalida,
					@AreaCulturaMaiorZero = Mensagem.SilviculturaAtvMsg.AreaCulturaMaiorZero					
				});
			}
		}

		internal Mensagem FormarMsg(Mensagem msg) { return new Mensagem { Campo = msg.Campo, Texto = msg.Texto, Tipo = msg.Tipo }; }

		public SilviculturaATVCaracteristicaVM(SilviculturaCaracteristicaATV caracterizacao, List<Lista> tipoCobertura, List<Lista> tipoGeometria, List<Lista> tipoFomento, bool isVisualizar = false)
		{

			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;
			TipoCobertura = ViewModelHelper.CriarSelectList(tipoCobertura, true, true);
			GeometriaTipo = ViewModelHelper.CriarSelectList(tipoGeometria, true, true, caracterizacao.GeometriaTipo.ToString());
			FomentoTipo = ViewModelHelper.CriarSelectList(tipoFomento, true, true, Convert.ToInt32(caracterizacao.Fomento).ToString());
			FomentoTipo = FomentoTipo.Where(x => x.Value != "0").ToList();
		}
	}
}