using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros
{
	public class OutrosLegitimacaoTerraDevolutaVM
	{
		public bool IsVisualizar { set; get; }

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private List<SelectListItem> _municipiosGleba = new List<SelectListItem>();
		public List<SelectListItem> MunicipiosGleba
		{
			get { return _municipiosGleba; }
			set { _municipiosGleba = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<SelectListItem> _dominios = new List<SelectListItem>();
		public List<SelectListItem> Dominios
		{
			get { return _dominios; }
			set { _dominios = value; }
		}

		private OutrosLegitimacaoTerraDevoluta _outros = new OutrosLegitimacaoTerraDevoluta();
		public OutrosLegitimacaoTerraDevoluta Outros
		{
			get { return _outros; }
			set { _outros = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@DestinatarioObrigatorio = Mensagem.OutrosLegitimacaoTerraDevolutaMsg.DestinatarioObrigatorio,
					@DestinatarioJaAdicionado = Mensagem.OutrosLegitimacaoTerraDevolutaMsg.DestinatarioJaAdicionado
				});
			}
		}

		public OutrosLegitimacaoTerraDevolutaVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios,
		List<ListaValor> dominios, OutrosLegitimacaoTerraDevoluta outros, List<Municipio> lstMunicipiosGleba, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			this.IsVisualizar = isVisualizar;
			this.Outros = outros;

			this.Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true);
			this.Dominios = ViewModelHelper.CriarSelectList(dominios, true, true, outros.Dominio.ToString());
			this.MunicipiosGleba = ViewModelHelper.CriarSelectList(lstMunicipiosGleba, true, true, outros.MunicipioGlebaId.ToString());

			this.Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			this.Atividades.MostrarBotoes = false;
		}

		public OutrosLegitimacaoTerraDevolutaVM() { }
	}
}