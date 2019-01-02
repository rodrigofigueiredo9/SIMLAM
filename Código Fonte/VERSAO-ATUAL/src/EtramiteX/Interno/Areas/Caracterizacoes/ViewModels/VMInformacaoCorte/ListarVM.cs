using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte
{
	public class ListarVM
	{
		public ListarVM() { }

		public ListarVM(EmpreendimentoCaracterizacao empreendimento)
		{
			var municipio = new ListaValor() { Id = 1, Texto = empreendimento.Municipio, IsAtivo = true };
			var uf = new ListaValor() { Id = 1, Texto = empreendimento.Uf, IsAtivo = true };
			var zonalocalizacao = new ListaValor() { Id = 1, Texto = empreendimento.ZonaLocalizacaoTexto, IsAtivo = true };

			this.Empreendimento = new EmpreendimentoCaracterizacaoVM
			{
				EmpreendimentoId = empreendimento.Id,
				EmpreendimentoCodigo = empreendimento.Codigo,
				EmpreendimentoCNPJ = empreendimento.CNPJ,
				DenominadorValor = empreendimento.Denominador,
				DenominadorTexto = empreendimento.DenominadorTipo,
				AreaImovel = empreendimento.AreaImovelHA,
				EmpreendimentoMunicipio = ViewModelHelper.CriarSelectList(new List<ListaValor>() { municipio }, true, selecionado: "1"),
				EmpreendimentoUf = ViewModelHelper.CriarSelectList(new List<ListaValor>() { uf }, true, selecionado: "1"),
				EmpreendimentoZonaLocalizacao = ViewModelHelper.CriarSelectList(new List<ListaValor>() { zonalocalizacao }, true, selecionado: "1")
			};
		}

		public EmpreendimentoCaracterizacaoVM Empreendimento { get; set; }
		public Decimal AreaPlantada { get; set; }

		private List<InformacaoCorte> _resultados = new List<InformacaoCorte>();
		public List<InformacaoCorte> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public bool PodeVisualizar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeCriar { get; set; }
	}
}