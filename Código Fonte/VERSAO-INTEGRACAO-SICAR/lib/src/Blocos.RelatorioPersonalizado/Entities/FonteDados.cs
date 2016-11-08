using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class Fato
	{
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Tid { get; set; }
		public string Tabela { get; set; }

		public List<Dimensao> Dimensoes { get; set; }
		public List<Campo> Campos { get; set; }
		public IEnumerable<Campo> CamposExibicao
		{
			get
			{
				return Campos.Where(c => c.CampoExibicao);
			}
		}
		public IEnumerable<Campo> CamposFato
		{
			get
			{
				return Campos.Where(c => c.Tabela == Tabela);
			}
		}
		public IEnumerable<Campo> CamposFatoExibicao
		{
			get
			{
				return CamposFato.Where(c => c.CampoExibicao);
			}
		}
		public List<Lista> DimensoesLst
		{
			get
			{
				List<Lista> retorno = new List<Lista>();

				if (!string.IsNullOrEmpty(Nome))
				{
					retorno.Add(new Lista() { Id = "0", Texto = Nome });
				}
				foreach (var item in Dimensoes)
				{
					retorno.Add(new Lista() { Id = item.Id.ToString(), Texto = item.Nome });
				}

				return retorno;
			}
		}
		public List<Campo> CamposFiltro
		{
			get
			{
				List<Campo> campos = new List<Campo>();
				campos.AddRange(Campos.Where(x => x.CampoFiltro).ToList());

				return campos.OrderBy(x => x.DimensaoNome).ToList();
			}
		}
		public Fato()
		{
			Dimensoes = new List<Dimensao>();
			Campos = new List<Campo>();
		}
	}
}