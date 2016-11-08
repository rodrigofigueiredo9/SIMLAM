using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class SilviculturaVM
	{
		public bool IsVisualizar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }
		public bool TemARL { get; set; }
		public bool TemARLDesconhecida { get; set; }

		Silvicultura _caracterizacao = new Silvicultura();
		public Silvicultura Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public List<CulturaFlorestal> CulturasGroup
		{
			get
			{
				List<CulturaFlorestal> culturasSelecionadas = new List<CulturaFlorestal>();
				List<CulturaFlorestal> culturas = new List<CulturaFlorestal>();

				Caracterizacao.Silviculturas.ForEach(x => { x.Culturas.ForEach(y => { culturas.Add(y); }); });

				culturas.ForEach(cultura =>
				{
					if (!culturasSelecionadas.Exists(y => y.CulturaTipoTexto.ToLower() == cultura.CulturaTipoTexto.ToLower()))
					{
						cultura.AreaCulturaHa = culturas
							.Where(x => x.CulturaTipoTexto.ToLower() == cultura.CulturaTipoTexto.ToLower())
							.Sum(x => x.AreaCulturaHa);
						culturasSelecionadas.Add(cultura);
					}
				});

				return culturasSelecionadas;

			}
		}

		private List<SilviculturaSilvicultVM> _silviculturaSilvicultVM = new List<SilviculturaSilvicultVM>();
		public List<SilviculturaSilvicultVM> SilviculturaSilvicultVM
		{
			get { return _silviculturaSilvicultVM; }
			set { _silviculturaSilvicultVM = value; }
		}

		public Decimal TotalAreaCroqui 
		{
			get 
			{
				if (Caracterizacao.Silviculturas != null && Caracterizacao.Silviculturas.Count > 0) 
				{
					return Caracterizacao.Silviculturas.Sum(x => x.AreaCroquiHa);
				}

				return 0;
			}
		}

		public Decimal TotalFloresta
		{
			get
			{
				if (Caracterizacao.Areas != null && Caracterizacao.Areas.Count > 0)
				{
					return Caracterizacao.Areas.Where(x => (x.Tipo == (int)eSilviculturaArea.AVN || x.Tipo == (int)eSilviculturaArea.AA_FLORESTA_PLANTADA)).Sum(x => x.Valor);
				}

				return 0;
			}
		}

		public SilviculturaArea ObterArea(eSilviculturaArea tipo)
		{
			return this.Caracterizacao.Areas.LastOrDefault(x => x.Tipo == (int)tipo) ?? new SilviculturaArea() { Tipo = (int)tipo };
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SemARLConfirm = Mensagem.Silvicultura.SemARLConfirm,
					@ARLDesconhecidaConfirm = Mensagem.Silvicultura.ARLDesconhecidaConfirm
				});
			}
		}

		public SilviculturaVM(Silvicultura caracterizacao, List<Lista> tipoCultivo,List<Lista> tipoGeometria,  bool isVisualizar = false) 
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;

			foreach (SilviculturaSilvicult silvicultura in caracterizacao.Silviculturas)
			{
				SilviculturaSilvicultVM silviculturaVM = new SilviculturaSilvicultVM(silvicultura, tipoCultivo, tipoGeometria, isVisualizar);
				SilviculturaSilvicultVM.Add(silviculturaVM);
			}
		}
	}
}