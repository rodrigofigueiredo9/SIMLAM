using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels
{
	public class InformacaoCorteVM
	{
		#region Constructores

		public InformacaoCorteVM() { }

		public InformacaoCorteVM(EmpreendimentoCaracterizacao empreendimento) {
			var municipio = new ListaValor() { Id = 1, Texto = empreendimento.Municipio, IsAtivo = true };
			var uf = new ListaValor() { Id = 1, Texto = empreendimento.Uf, IsAtivo = true };
			var zonalocalizacao = new ListaValor() { Id = 1, Texto = empreendimento.ZonaLocalizacaoTexto, IsAtivo = true };

			this.Empreendimento = new EmpreendimentoCaracterizacaoVM
			{
				EmpreendimentoId = empreendimento.Id,
				EmpreendimentoCodigo = empreendimento.Codigo,
				EmpreendimentoCNPJ = empreendimento.CNPJ ?? "-",
				DenominadorValor = empreendimento.Denominador,
				DenominadorTexto = empreendimento.DenominadorTipo,
				AreaImovel = empreendimento.AreaImovelHA,
				EmpreendimentoMunicipio = ViewModelHelper.CriarSelectList(new List<ListaValor>() { municipio }, true, selecionado: "1"),
				EmpreendimentoUf = ViewModelHelper.CriarSelectList(new List<ListaValor>() { uf }, true, selecionado: "1"),
				EmpreendimentoZonaLocalizacao = ViewModelHelper.CriarSelectList(new List<ListaValor>() { zonalocalizacao }, true, selecionado: "1")
			};
		}

		public InformacaoCorteVM(EmpreendimentoCaracterizacao empreendimento, List<Lista> destinacao, List<Lista> produto, List<Lista> tipoCorte, List<Lista> especie)
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

			this.InformacaoCorteDestinacao = new InformacaoCorteDestinacaoVM()
			{
				DestinacaoMaterial = ViewModelHelper.CriarSelectList(destinacao),
				Produto = ViewModelHelper.CriarSelectList(produto)
			};

			this.InformacaoCorteTipo = new InformacaoCorteTipoVM() {
				TipoCorte = ViewModelHelper.CriarSelectList(tipoCorte),
				Especie = ViewModelHelper.CriarSelectList(especie)
			};
		}

		#endregion

		#region Properties

		public int? Id { get; set; }
		public DateTecno DataInformacao { get; set; } = new DateTecno { Data = DateTime.Now };
		public Decimal AreaPlantada { get; set; }

		public long ProjetoDigitalId { get; set; }
		public bool IsVisualizar { get; set; }
		public bool IsPodeExcluir { get; set; }

		public InformacaoCorteDestinacaoVM InformacaoCorteDestinacao { get; set; }
		public InformacaoCorteTipoVM InformacaoCorteTipo { get; set; }
		public EmpreendimentoCaracterizacaoVM Empreendimento { get; set; }

		public List<InformacaoCorte> InformacoesCortes { get; set; } = new List<InformacaoCorte>();
		public List<InformacaoCorteLicenca> InformacaoCorteLicencaList { get; set; } = new List<InformacaoCorteLicenca>();
		public List<InformacaoCorteDestinacao> InformacaoCorteDestinacaoList { get; set; } = new List<InformacaoCorteDestinacao>();
		public List<InformacaoCorteResultadosVM> InformacaoCorteResultados { get; set; } = new List<InformacaoCorteResultadosVM>();

		public bool DeclaracaoVerdadeira { get; set; }
		public bool ResponsavelPelasDeclaracoes { get; set; }

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
					@QuantidadeObrigatoria = Mensagem.InformacaoCorte.QuantidadeObrigatoria
				});
			}
		}

		#endregion
	}
}