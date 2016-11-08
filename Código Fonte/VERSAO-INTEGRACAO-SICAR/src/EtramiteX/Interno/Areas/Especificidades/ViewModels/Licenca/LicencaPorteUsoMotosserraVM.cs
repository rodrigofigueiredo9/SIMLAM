using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Licenca
{
	public class LicencaPorteUsoMotosserraVM
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

		private LicencaPorteUsoMotosserra licenca = new LicencaPorteUsoMotosserra();
		public LicencaPorteUsoMotosserra Licenca
		{
			get { return licenca; }
			set { licenca = value; }
		}

		public LicencaPorteUsoMotosserraVM() { }

		public LicencaPorteUsoMotosserraVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, LicencaPorteUsoMotosserra licenca, string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true);
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
			Licenca = licenca;

			List<Lista> lstVia = new List<Lista>();

			lstVia.Add(new Lista { Id = "1", Texto = "1", IsAtivo = true });
			lstVia.Add(new Lista { Id = "2", Texto = "2", IsAtivo = true });
			lstVia.Add(new Lista { Id = "3", Texto = "3", IsAtivo = true });
			lstVia.Add(new Lista { Id = "4", Texto = "4", IsAtivo = true });
			lstVia.Add(new Lista { Id = "5", Texto = "5", IsAtivo = true });
			lstVia.Add(new Lista { Id = "6", Texto = "Outras", IsAtivo = true });

			if (Licenca != null && Licenca.Vias != null)
			{
				if (Licenca.Vias > 5)
				{
					ViaSelecionada = "6";
					ViasOutra = licenca.Vias.ToString();

				}
				else
				{
					ViaSelecionada = Licenca.Vias.ToString();
				}
			}

			Vias = ViewModelHelper.CriarSelectList(lstVia, true, true, ViaSelecionada);
		}
	}
}