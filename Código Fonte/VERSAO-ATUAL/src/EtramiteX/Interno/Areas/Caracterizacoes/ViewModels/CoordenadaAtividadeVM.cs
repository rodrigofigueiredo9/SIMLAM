using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class CoordenadaAtividadeVM
	{
		public bool IsVisualizar { get; set; }

		private CoordenadaAtividade _coordenadaAtividade = new CoordenadaAtividade();
		public CoordenadaAtividade CoordenadaAtividade
		{
			get { return _coordenadaAtividade; }
			set { _coordenadaAtividade = value; }
		}

		private List<SelectListItem> _tipoGeometrico = new List<SelectListItem>();
		public List<SelectListItem> TipoGeometrico
		{
			get { return _tipoGeometrico; }
			set { _tipoGeometrico = value; }
		}

		private List<SelectListItem> _coordenadasAtividade = new List<SelectListItem>();
		public List<SelectListItem> CoordenadasAtividade
		{
			get { return _coordenadasAtividade; }
			set { _coordenadasAtividade = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@GeometriaTipoObrigatorio = Mensagem.CoordenadaAtividade.GeometriaTipoObrigatorio,
					@CoordenadaAtividadeObrigatoria = Mensagem.CoordenadaAtividade.CoordenadaAtividadeObrigatoria
				});
			}
		}

		public CoordenadaAtividadeVM(CoordenadaAtividade coordenada, List<Lista> coordenadaslst, List<Lista> tipoGeometrico, bool isVisualizar = false)
		{

			IsVisualizar = isVisualizar;
			CoordenadaAtividade = coordenada;
			String coordenadaSelecionadaValue = CoordenadaAtividade.Id + "|" + CoordenadaAtividade.CoordX + "|" + CoordenadaAtividade.CoordY;

			TipoGeometrico = ViewModelHelper.CriarSelectList(tipoGeometrico, true, true, selecionado: CoordenadaAtividade.Tipo.ToString());
			CoordenadasAtividade = ViewModelHelper.CriarSelectList(coordenadaslst, true, true, selecionado: coordenadaSelecionadaValue);

		}

		public CoordenadaAtividadeVM() { }		
	}
}