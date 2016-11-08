using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels
{
	public class PersonalizadoVM
	{
		public int Id { get; set; }
		public string CaminhoImagem { get; set; }
		public ConfiguracaoRelatorio ConfiguracaoRelatorio { get; set; }
		public string ConfiguracaoRelatorioJSON { get { return ViewModelHelper.Json(ConfiguracaoRelatorio); } }
		public List<SelectListItem> FonteDadosLst { get; set; }
		public List<SelectListItem> DimensoesLst { get; set; }
		public List<SelectListItem> OperadoresLst { get; set; }
		public List<SelectListItem> CamposLst { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					NomeObrigatorio = Mensagem.RelatorioPersonalizado.NomeObrigatorio,
					DescricaoObrigatoria = Mensagem.RelatorioPersonalizado.DescricaoObrigatoria,
					RelatorioTipoObrigatorio = Mensagem.RelatorioPersonalizado.RelatorioTipoObrigatorio,
					CampoSelecionarObrigatorio = Mensagem.RelatorioPersonalizado.CampoSelecionarObrigatorio,
					CampoFiltroObrigatorio = Mensagem.RelatorioPersonalizado.CampoFiltroObrigatorio,
					OperadorObrigatorio = Mensagem.RelatorioPersonalizado.OperadorObrigatorio,
					FiltroObrigatorio = Mensagem.RelatorioPersonalizado.FiltroObrigatorio,
					SomaColunasInvalida = Mensagem.RelatorioPersonalizado.SomaColunasInvalida,
					SelecioneArquivo = Mensagem.RelatorioPersonalizado.SelecioneArquivo
				});
			}
		}

		public PersonalizadoVM() : this(new List<Lista>()) { }

		public PersonalizadoVM(List<Lista> fontesDados)
		{
			ConfiguracaoRelatorio = new ConfiguracaoRelatorio();
			FonteDadosLst = ViewModelHelper.CriarSelectList(fontesDados, true);
		}
	}
}