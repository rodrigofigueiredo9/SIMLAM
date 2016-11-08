using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado
{
	public class CertificadoRegistroAtividadeFlorestalVM
	{
		public bool IsVisualizar { set; get; }

		private List<SelectListItem> _vias = new List<SelectListItem>();
		public List<SelectListItem> Vias
		{
			get { return _vias; }
			set { _vias = value; }
		}
		public string ViaSelecionada { get; set; }
		public string ViasOutra { get; set; }

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private CertificadoRegistroAtividadeFlorestal _certificado = new CertificadoRegistroAtividadeFlorestal();
		public CertificadoRegistroAtividadeFlorestal Certificado
		{
			get { return _certificado; }
			set { _certificado = value; }
		}

		public CertificadoRegistroAtividadeFlorestalVM() { }

		public CertificadoRegistroAtividadeFlorestalVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, CertificadoRegistroAtividadeFlorestal certificado, int atividadeSelecionada = 0, string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true);
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;
			Certificado = certificado;

			List<Lista> lstVia = new List<Lista>();

			lstVia.Add(new Lista { Id = "1", Texto = "1" });
			lstVia.Add(new Lista { Id = "2", Texto = "2" });
			lstVia.Add(new Lista { Id = "3", Texto = "3" });
			lstVia.Add(new Lista { Id = "4", Texto = "4" });
			lstVia.Add(new Lista { Id = "5", Texto = "5" });
			lstVia.Add(new Lista { Id = "6", Texto = "Outras" });

			if (Certificado != null && Certificado.Vias != null)
			{
				if (Convert.ToInt16(Certificado.Vias) > 5)
				{
					ViaSelecionada = "6";
					ViasOutra = Certificado.Vias.ToString();

				}
				else
				{
					ViaSelecionada = Certificado.Vias.ToString();
				}
			}

			Vias = ViewModelHelper.CriarSelectList(lstVia);
		}
	}
}