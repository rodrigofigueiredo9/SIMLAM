using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Gerencial.ViewModels;

namespace Tecnomapas.EtramiteX.Gerencial.Areas.Relatorios.ViewModels
{
	public class PersonalizadoListarVM
	{
		public bool PodeExecutar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeExportar { get; set; }
		public bool PodeAtribuirExecutor { get; set; }
		public string UltimaBusca { get; set; }
		public string QuantidadeItensTexto
		{
			get
			{
				if (Resultados != null)
				{
					if (Resultados.Count > 1)
					{
						return "itens encontrados.";
					}

					return "item encontrado.";
				}

				return string.Empty;
			}
		}

		public Relatorio Filtros { get; set; }
		public List<Relatorio> Resultados { get; set; }
		public List<SelectListItem> FonteDadosLst { get; set; }

		public PersonalizadoListarVM() : this(new List<Lista>()) { }

		public PersonalizadoListarVM(List<Lista> fontesDados)
		{
			Filtros = new Relatorio();
			Resultados = new List<Relatorio>();
			FonteDadosLst = ViewModelHelper.CriarSelectList(fontesDados, true);
		}
	}
}