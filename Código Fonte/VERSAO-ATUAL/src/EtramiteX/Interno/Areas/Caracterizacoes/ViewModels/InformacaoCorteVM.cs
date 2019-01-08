using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class InformacaoCorteVM
	{
		#region Constructores

		public InformacaoCorteVM() { }

		public InformacaoCorteVM(EmpreendimentoCaracterizacao empreendimento, List<Lista> destinacao, List<Lista> produto, List<Lista> tipoCorte,
			List<Lista> especie, InformacaoCorte caracterizacao = null)
		{
			if (caracterizacao != null)
			{
				this.Id = caracterizacao.Id;
				this.DataInformacao = caracterizacao.DataInformacao;
				this.AreaPlantada = caracterizacao.AreaFlorestaPlantada;
				this.InformacaoCorteLicencaList = caracterizacao.InformacaoCorteLicenca;
				foreach (var item in caracterizacao.InformacaoCorteTipo)
				{
					var linhas = item.InformacaoCorteDestinacao.Count;
					foreach (var dest in item.InformacaoCorteDestinacao)
					{
						var resultado = new InformacaoCorteResultadosVM();
						resultado.Id = item.Id;
						resultado.IdadePlantio = item.IdadePlantio;
						resultado.AreaCorte = item.AreaCorte;
						resultado.TipoCorte = item.TipoCorte;
						resultado.TipoCorteTexto = item.TipoCorteTexto;
						resultado.Especie = item.EspecieInformada;
						resultado.EspecieTexto = item.EspecieInformadaTexto;
						resultado.DestinacaoId = dest.Id;
						resultado.Quantidade = dest.Quantidade;
						resultado.CodigoSefazId = dest.CodigoSefazId;
						resultado.DestinacaoMaterial = dest.DestinacaoMaterial;
						resultado.DestinacaoMaterialTexto = dest.DestinacaoMaterialTexto;
						resultado.Produto = dest.Produto;
						resultado.ProdutoTexto = dest.ProdutoTexto;
						resultado.Linhas = linhas;
						this.InformacaoCorteResultados.Add(resultado);
						linhas = 0;
					}
				}
			}

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

			this.InformacaoCorteDestinacao = new InformacaoCorteDestinacaoVM()
			{
				DestinacaoMaterial = ViewModelHelper.CriarSelectList(destinacao),
				Produto = ViewModelHelper.CriarSelectList(produto)
			};

			this.InformacaoCorteTipo = new InformacaoCorteTipoVM()
			{
				TipoCorte = ViewModelHelper.CriarSelectList(tipoCorte),
				Especie = ViewModelHelper.CriarSelectList(especie)
			};
		}

		#endregion

		#region Properties

		public int? Id { get; set; }
		public DateTecno DataInformacao { get; set; } = new DateTecno { Data = DateTime.Now };
		public Decimal AreaPlantada { get; set; }

		public bool IsVisualizar { get; set; }
		public bool IsPodeExcluir { get; set; }

		public InformacaoCorteDestinacaoVM InformacaoCorteDestinacao { get; set; }
		public InformacaoCorteTipoVM InformacaoCorteTipo { get; set; }
		public EmpreendimentoCaracterizacaoVM Empreendimento { get; set; }

		public List<InformacaoCorteLicenca> InformacaoCorteLicencaList { get; set; } = new List<InformacaoCorteLicenca>();
		public List<InformacaoCorteResultadosVM> InformacaoCorteResultados { get; set; } = new List<InformacaoCorteResultadosVM>();

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@TipoCorteObrigatorio = Mensagem.InformacaoCorte.TipoCorteObrigatorio,
					@EspecieObrigatoria = Mensagem.InformacaoCorte.EspecieObrigatoria,
					@AreaCorteObrigatoria = Mensagem.InformacaoCorte.AreaCorteObrigatoria,
					@IdadePlantioObrigatoria = Mensagem.InformacaoCorte.IdadePlantioObrigatoria,
					@DestinacaoMaterialObrigatoria = Mensagem.InformacaoCorte.DestinacaoMaterialObrigatoria,
					@ProdutoObrigatorio = Mensagem.InformacaoCorte.ProdutoObrigatorio,
					@QuantidadeObrigatoria = Mensagem.InformacaoCorte.QuantidadeObrigatoria,
					@Declaracao1Obrigatoria = Mensagem.InformacaoCorte.Declaracao1Obrigatoria,
					@Declaracao2Obrigatoria = Mensagem.InformacaoCorte.Declaracao2Obrigatoria,
					@NumeroLicencaObrigatoria = Mensagem.InformacaoCorte.NumeroLicencaObrigatoria,
					@TipoLicencaObrigatoria = Mensagem.InformacaoCorte.TipoLicencaObrigatoria,
					@AtividadeObrigatoria = Mensagem.InformacaoCorte.AtividadeObrigatoria,
					@AreaLicencaObrigatoria = Mensagem.InformacaoCorte.AreaLicencaObrigatoria,
					@DataVencimentoObrigatoria = Mensagem.InformacaoCorte.DataVencimentoObrigatoria,
					@AreaPlantadaObrigatoria = Mensagem.InformacaoCorte.AreaPlantadaObrigatoria
				});
			}
		}

		#endregion
	}
}