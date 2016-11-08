using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class MateriaPrimaFlorestalConsumidaVM
	{
		public bool IsVisualizar { get; set; }

		private List<MateriaPrima> _materiasPrimasFlorestais = new List<MateriaPrima>();
		public List<MateriaPrima> MateriasPrimasFlorestais
		{
			get { return _materiasPrimasFlorestais; }
			set { _materiasPrimasFlorestais = value; }
		}

		private List<SelectListItem> _materiaPrimaFlorestalConsumidaLst = new List<SelectListItem>();
		public List<SelectListItem> MateriaPrimaFlorestalConsumidaLst
		{
			get { return _materiaPrimaFlorestalConsumidaLst; }
			set { _materiaPrimaFlorestalConsumidaLst = value; }
		}

		private List<SelectListItem> _unidade = new List<SelectListItem>();
		public List<SelectListItem> Unidade
		{
			get { return _unidade; }
			set { _unidade = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@MateriaPrimaFlorestalConsumidaObrigatoria = Mensagem.MateriaPrimaFlorestalConsumida.MateriaPrimaFlorestalConsumidaObrigatoria,
					@MateriaPrimaFlorestalConsumidaDuplicada = Mensagem.MateriaPrimaFlorestalConsumida.MateriaPrimaFlorestalConsumidaDuplicada,
					@UnidadeMateriaPrimaObrigatoria = Mensagem.MateriaPrimaFlorestalConsumida.UnidadeMateriaPrimaObrigatoria,

					@QuantidadeMateriaPrimaObrigatoria = Mensagem.MateriaPrimaFlorestalConsumida.QuantidadeMateriaPrimaObrigatoria,
					@QuantidadeMateriaPrimaInvalida = Mensagem.MateriaPrimaFlorestalConsumida.QuantidadeMateriaPrimaInvalida,
					@QuantidadeMateriaPrimaMairZero = Mensagem.MateriaPrimaFlorestalConsumida.QuantidadeMateriaPrimaMaiorZero,

					@EspecificarMateriaPrimaObrigatorio = Mensagem.MateriaPrimaFlorestalConsumida.EspecificarMateriaPrimaObrigatorio
				});
			}
		}


		public MateriaPrimaFlorestalConsumidaVM(List<MateriaPrima> materiasPrima, List<Lista> materiasPrimaConsumidasLst, List<Lista> unidadeMedida, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			MateriaPrimaFlorestalConsumidaLst = ViewModelHelper.CriarSelectList(materiasPrimaConsumidasLst, true, true);
			Unidade = ViewModelHelper.CriarSelectList(unidadeMedida, true, true, "1");
			MateriasPrimasFlorestais = materiasPrima;
		}
	}
}