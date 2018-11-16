using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento
{
	public class EmpreendimentoVM
	{
		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());

		public String UrlObterMunicipioCoordenada { get { return _configCoordenada.Obter<String>(ConfiguracaoCoordenada.KeyUrlObterMunicipioCoordenada); } }
		public SalvarVM SalvarVM { get; set; }
		public LocalizarVM LocalizarVM { get; set; }
		public ListarVM ListarVM { get; set; }
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
					@ResponsavelObrigatorio = Mensagem.ResponsavelTecnico.ResponsavelObrigatorio
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EnderecoTipoEmpreendimento = (int)eEnderecoTipo.Empreendimento,
					@EnderecoTipoInteressado = (int)eEnderecoTipo.Interessado,
					@EnderecoTipoRepresentante = (int)eEnderecoTipo.Representante,
					@EnderecoTipoResponsavelTecnico = (int)eEnderecoTipo.ResponsavelTecnico
				});
			}
		}

		public EmpreendimentoVM()
		{
			SalvarVM = new SalvarVM();
			LocalizarVM = new LocalizarVM();
			ListarVM = new ListarVM();
		}

		public EmpreendimentoVM(List<Estado> lstEstados, List<Municipio> lstMunicipio, List<Segmento> lstSegmentos, List<CoordenadaTipo> lstTiposCoordenada,
			List<Datum> lstDatuns, List<Fuso> lstFusos, List<CoordenadaHemisferio> lstHemisferios, List<TipoResponsavel> lstTiposResponsavel)
		{
			LocalizarVM = new LocalizarVM(lstEstados, lstMunicipio, lstSegmentos, lstTiposCoordenada, lstDatuns, lstFusos, lstHemisferios);
			SalvarVM = new SalvarVM();
			ListarVM = new ListarVM();
			SalvarVM.SetarDenominadores(lstSegmentos);
		}
	}
}