using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAgrotoxico
{
	public class AgrotoxicoVM
	{
		public bool IsVisualizar { get; set; }

		private Agrotoxico _agrotoxico = new Agrotoxico();
		public Agrotoxico Agrotoxico
		{
			get { return _agrotoxico; }
			set { _agrotoxico = value; }
		}

		private List<ConfiguracaoVegetalItem> _classesUso = new List<ConfiguracaoVegetalItem>();
		public List<ConfiguracaoVegetalItem> ClassesUso
		{
			get { return _classesUso; }
			set { _classesUso = value; }
		}

		private List<SelectListItem> _formasApresentacao = new List<SelectListItem>();
		public List<SelectListItem> FormasApresentacao
		{
			get { return _formasApresentacao; }
			set { _formasApresentacao = value; }
		}

		private List<SelectListItem> _gruposQuimicos = new List<SelectListItem>();
		public List<SelectListItem> GruposQuimicos
		{
			get { return _gruposQuimicos; }
			set { _gruposQuimicos = value; }
		}

		private List<SelectListItem> _periculosidadesAmbientais = new List<SelectListItem>();
		public List<SelectListItem> PericulosidadesAmbientais
		{
			get { return _periculosidadesAmbientais; }
			set { _periculosidadesAmbientais = value; }
		}

		private List<SelectListItem> _classToxicologicas = new List<SelectListItem>();
		public List<SelectListItem> ClassToxicologicas
		{
			get { return _classToxicologicas; }
			set { _classToxicologicas = value; }
		}

		private List<SelectListItem> _ingredienteAtivoUnidadesMedida = new List<SelectListItem>();
		public List<SelectListItem> IngredienteAtivoUnidadeMedidaLst
		{
			get { return _ingredienteAtivoUnidadesMedida; }
			set { _ingredienteAtivoUnidadesMedida = value; }
		}

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@IngredienteAtivoAdicionado = Mensagem.Agrotoxico.IngredienteAtivoAdicionado,
					@IngredienteAtivoObrigatorio = Mensagem.Agrotoxico.IngredienteAtivoCampoObrigatorio,
					@ConcentracaoObrigatorio = Mensagem.Agrotoxico.ConcentracaoObrigatorio,
					@UnidadeMedidaObrigatoria = Mensagem.Agrotoxico.UnidadeMedidaObrigatoria,
					@UnidadeMedidaOutroObrigatorio = Mensagem.Agrotoxico.UnidadeMedidaOutroObrigatorio,
					@IngredienteAtivoDesativado = Mensagem.Agrotoxico.IngredienteAtivoDesativado,
					@SelecioneUmGrupoQuimico = Mensagem.Agrotoxico.SelecioneUmGrupoQuimico,
					@GrupoQuimicoAdicionado = Mensagem.Agrotoxico.GrupoQuimicoAdicionado,
					@ArquivoDeveSerPDF = Mensagem.Agrotoxico.ArquivoDeveSerPDF
				});
			}
		}

		public string IngredienteAtivoUnidadeMedida
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					Outros = eAgrotoxicoIngredienteAtivoUnidadeMedida.Outros
				});
			}
		}

		public string IdsTelaIngredienteAtivoSituacao
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Ativo = eIngredienteAtivoSituacao.Ativo,
					@Ativo_ANVISA_Inativo_Estado = eIngredienteAtivoSituacao.Ativo_ANVISA_Inativo_Estado,
					@Inativo = eIngredienteAtivoSituacao.Inativo
				});
			}
		}

		public String TiposArquivoValido { get { return ViewModelHelper.Json(new ArrayList { ".pdf" }); } }

		public AgrotoxicoVM(){}

		public AgrotoxicoVM(Agrotoxico agrotoxico, List<Lista> ingredienteAtivoUnidadesMedida, List<ConfiguracaoVegetalItem> classesUso, List<Lista> formasApresentacao, List<Lista> gruposQuimicos, List<Lista> periculosidadesAmbientais, List<Lista> classToxicologicas, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Agrotoxico = agrotoxico;
			ClassesUso = classesUso;

			IngredienteAtivoUnidadeMedidaLst = ViewModelHelper.CriarSelectList(ingredienteAtivoUnidadesMedida, true, true);
			FormasApresentacao = ViewModelHelper.CriarSelectList(formasApresentacao, true, true, agrotoxico.FormaApresentacao.Id.ToString());
			GruposQuimicos = ViewModelHelper.CriarSelectList(gruposQuimicos, true, true);
			PericulosidadesAmbientais = ViewModelHelper.CriarSelectList(periculosidadesAmbientais, true, true, agrotoxico.PericulosidadeAmbiental.Id.ToString());
			ClassToxicologicas = ViewModelHelper.CriarSelectList(classToxicologicas, true, true, agrotoxico.ClassificacaoToxicologica.Id.ToString());
		}
	}
}