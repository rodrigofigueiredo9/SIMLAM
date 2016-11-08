using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Relatorios.ViewModels
{
	public class PersonalizadoExecutarVM
	{
		public List<SelectListItem> SetorLst { get; set; }
		public Relatorio Relatorio { get; set; }
		public List<Termo> TermosExecucao
		{
			get
			{
				if (Relatorio != null && Relatorio.ConfiguracaoRelatorio != null && Relatorio.ConfiguracaoRelatorio.Termos != null)
				{
					return Relatorio.ConfiguracaoRelatorio.Termos.Where(x => x.DefinirExecucao).ToList();
				}

				return new List<Termo>();
			}
		}

		public PersonalizadoExecutarVM() : this(new List<Setor>()) { }

		public PersonalizadoExecutarVM(List<Setor> setores, int setor = 0)
		{
			if(setor != 0 && setores != null && setores.Count == 1)
			{
				setor = setores.First().Id;
			}

			Relatorio = new Relatorio();
			SetorLst = ViewModelHelper.CriarSelectList(setores, true, (setor <= 0), setor.ToString());
		}

		public string ObterMascara(Campo campo)
		{
			string mascara = string.Empty;

			switch (campo.TipoDadosEnum)
			{
				case eTipoDados.Inteiro:
					mascara = "maskNumInt";
					break;

				case eTipoDados.Real:
					mascara = "maskDecimal";
					break;

				case eTipoDados.Data:
					mascara = "maskData";
					break;
			}

			return mascara;
		}

		public string ObterCampo(Termo termo)
		{
			return ViewModelHelper.Json(new { Valor = termo.Valor, Ordem = termo.Ordem, Campo = new { Id = termo.Campo.Id, Alias = termo.Campo.Alias, TipoDados = termo.Campo.TipoDados } });
		}
	}
}