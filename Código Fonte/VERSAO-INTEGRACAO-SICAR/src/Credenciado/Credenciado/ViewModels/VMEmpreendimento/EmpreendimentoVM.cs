using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMEmpreendimento
{
	public class EmpreendimentoVM
	{
		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());

		public String UrlObterMunicipioCoordenada { get { return _configCoordenada.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada); } }
		public SalvarVM SalvarVM { get; set; }
		public LocalizarVM LocalizarVM { get; set; }
		public string EmpreendimentoHtmlCopiar { get; set; }
		
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					CnpjJaExistente = Mensagem.Empreendimento.CnpjJaExistente,
					CnpjDisponivel = Mensagem.Empreendimento.CnpjDisponivel,
					ResponsavelExistente = Mensagem.Empreendimento.ResponsavelExistente,
					ResponsavelBloqueado = Mensagem.Empreendimento.ResponsavelBloqueado,
					Posse = Mensagem.Empreendimento.Posse,
					CnpjNaoCadastrado = Mensagem.Empreendimento.CnpjNaoCadastrado,
					@ResponsavelObrigatorio = Mensagem.Empreendimento.ResponsavelSemPreencher
				});
			}
		}

		public EmpreendimentoVM()
		{
			SalvarVM = new SalvarVM();
			LocalizarVM = new LocalizarVM();
		}

		public EmpreendimentoVM(List<Estado> lstEstados, List<Municipio> lstMunicipio, List<Segmento> lstSegmentos, List<CoordenadaTipo> lstTiposCoordenada,
			List<Datum> lstDatuns, List<Fuso> lstFusos, List<CoordenadaHemisferio> lstHemisferios, List<TipoResponsavel> lstTiposResponsavel)
		{
			LocalizarVM = new LocalizarVM(lstEstados, lstMunicipio, lstSegmentos, lstTiposCoordenada, lstDatuns, lstFusos, lstHemisferios);
			SalvarVM = new SalvarVM();

			SalvarVM.SetarDenominadores(lstSegmentos);
		}
	}
}