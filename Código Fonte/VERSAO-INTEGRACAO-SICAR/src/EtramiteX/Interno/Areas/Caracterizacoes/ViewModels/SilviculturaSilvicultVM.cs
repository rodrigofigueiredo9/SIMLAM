using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class SilviculturaSilvicultVM
	{
		public bool IsVisualizar { get; set; }

		private SilviculturaSilvicult _caracterizacao = new SilviculturaSilvicult();
		public SilviculturaSilvicult Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<SelectListItem> _tipoCultura = new List<SelectListItem>();
		public List<SelectListItem> TipoCultura
		{
			get { return _tipoCultura; }
			set { _tipoCultura = value; }
		}

		private List<SelectListItem> _geometriaTipo = new List<SelectListItem>();
		public List<SelectListItem> GeometriaTipo
		{
			get { return _geometriaTipo; }
			set { _geometriaTipo = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@TipoCulturaObrigatorio = Mensagem.Silvicultura.TipoCulturaObrigatorio,
					@TipoCulturaJaAdicionado = Mensagem.Silvicultura.TipoCulturaJaAdicionado,

					@AreaCulturaObrigatoria = Mensagem.Silvicultura.AreaCulturaObrigatoria,
					@AreaCulturaInvalida = Mensagem.Silvicultura.AreaCulturaInvalida,
					@AreaCulturaMaiorZero = Mensagem.Silvicultura.AreaCulturaMaiorZero,

					@EspecificarTipoCulturaObrigatorio = Mensagem.Silvicultura.EspecificarTipoCulturaObrigatorio,
					@EspecificarTipoCulturaJaAdicionado = Mensagem.Silvicultura.EspecificarTipoCulturaJaAdicionado
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Outros = eCulturaFlorestal.Outros
				});
			}
		}

		public SilviculturaSilvicultVM(SilviculturaSilvicult caracterizacao, List<Lista> tipoCultivo, List<Lista> tipoGeometria, bool isVisualizar = false)
		{

			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;
			TipoCultura = ViewModelHelper.CriarSelectList(tipoCultivo, true, true);
			GeometriaTipo = ViewModelHelper.CriarSelectList(tipoGeometria, true, true, caracterizacao.GeometriaTipo.ToString());
		}
	}
}