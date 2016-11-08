using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao
{
	public class CertidaoDebitoVM
	{
		public bool IsVisualizar { get; set; }

		public String TipoCertidao { get; set; }

		private CertidaoDebito _certidao = new CertidaoDebito();
		public CertidaoDebito Certidao
		{
			get { return _certidao; }
			set { _certidao = value; }
		}

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

		public CertidaoDebitoVM()
		{

		}

		public CertidaoDebitoVM(CertidaoDebito certidaoDebito, List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, String tipoCertidao, string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Certidao = certidaoDebito;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, certidaoDebito.Destinatario.ToString());
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
			Atividades.MostrarBotoes = false;
			TipoCertidao = tipoCertidao;
		}
	}
}