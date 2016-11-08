using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class QueimaControladaQueimaVM
	{
		public bool IsVisualizar { get; set; }

		private QueimaControladaQueima _caracterizacao = new QueimaControladaQueima();
		public QueimaControladaQueima Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<SelectListItem> _tipoCultivo = new List<SelectListItem>();
		public List<SelectListItem> TipoCultivo
		{
			get { return _tipoCultivo; }
			set { _tipoCultivo = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@FinalidadeNomeObrigatorio = Mensagem.QueimaControlada.FinalidadeNomeObrigatorio,
					@TipoCultivoObrigatorio = Mensagem.QueimaControlada.TipoCultivoObrigatorio,
					@AreaQueimaObrigatoria = Mensagem.QueimaControlada.AreaQueimaObrigatoria,
					@AreaQueimaInvalida = Mensagem.QueimaControlada.AreaQueimaInvalida,
					@AreaQueimaMaiorZero = Mensagem.QueimaControlada.AreaQueimaMaiorZero,
					@TipoCultivoQueimaDuplicado = Mensagem.QueimaControlada.TipoCultivoQueimaDuplicado,
					@FinalidadeNomeQueimaDuplicada = Mensagem.QueimaControlada.FinalidadeNomeQueimaDuplicada
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@OutraFinalidade = eCultivoTipo.OutraFinalidade
				});
			}
		}

		public QueimaControladaQueimaVM(QueimaControladaQueima caracterizacao, List<Lista> tipoCultivo, bool isVisualizar = false)
		{

			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;
			TipoCultivo = ViewModelHelper.CriarSelectList(tipoCultivo, true, true);
		}
	}
}