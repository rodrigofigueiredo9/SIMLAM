using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros
{
	public class OutrosConclusaoTransferenciaDominioVM
	{
		public bool IsVisualizar { set; get; }

		private List<SelectListItem> _destinatariosLst = new List<SelectListItem>();
		public List<SelectListItem> DestinatariosLst
		{
			get { return _destinatariosLst; }
			set { _destinatariosLst = value; }
		}

		private List<SelectListItem> _interessadosLst = new List<SelectListItem>();
		public List<SelectListItem> InteressadosLst
		{
			get { return _interessadosLst; }
			set { _interessadosLst = value; }
		}

		private List<SelectListItem> _responsaveisLst = new List<SelectListItem>();
		public List<SelectListItem> ResponsaveisLst
		{
			get { return _responsaveisLst; }
			set { _responsaveisLst = value; }
		}

		public String MensagensDestinatarios
		{
			get
			{
				List<Mensagem> msgs = new List<Mensagem>();
				msgs.Add(Mensagem.Especificidade.DestinatarioObrigatorio("ddlDestinatarios"));
				msgs.Add(Mensagem.Especificidade.DestinatarioJaAdicionado);

				return ViewModelHelper.Json(new
				{
					msgs
				});
			}
		}
        
		public String MensagensResponsaveisEmpreendimento
		{
			get
			{
				List<Mensagem> msgs = new List<Mensagem>();
				msgs.Add(Mensagem.Especificidade.ResponsavelObrigatorio("ddlResponsaveis")); 
				msgs.Add(Mensagem.Especificidade.ResponsavelJaAdicionado);
                msgs.Add(Mensagem.Especificidade.ResponsavelIgualInteressado);
				return ViewModelHelper.Json(new
				{
					msgs
				});
			}
		}

		public String MensagensInteressados
		{
			get
			{

				List<Mensagem> msgs = new List<Mensagem>();
				msgs.Add(Mensagem.Especificidade.InteressadoObrigatorio("ddlInteressados"));
				msgs.Add(Mensagem.Especificidade.InteressadoJaAdicionado);
				msgs.Add(Mensagem.Especificidade.InteressadoIgualResponsavel);
				return ViewModelHelper.Json(new
				{
					msgs
				});
			}
		}
				
		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private OutrosConclusaoTransferenciaDominio _outros = new OutrosConclusaoTransferenciaDominio();
		public OutrosConclusaoTransferenciaDominio Outros
		{
			get { return _outros; }
			set { _outros = value; }
		}
		
		public OutrosConclusaoTransferenciaDominioVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, OutrosConclusaoTransferenciaDominio outros, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

			Outros = outros;
			if (Outros.Responsaveis == null && Outros.Destinatarios == null && Outros.Interessados == null)
			{
				Outros.Destinatarios = new List<PessoaEspecificidade>();
				Outros.Responsaveis = new List<PessoaEspecificidade>();
				Outros.Interessados = new List<PessoaEspecificidade>();
			}
		}

		public OutrosConclusaoTransferenciaDominioVM() { }
	}
}