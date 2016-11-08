using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class AgrotoxicoPDF
	{
		public String Numero { get; set; }
		public String NomeComercial { get; set; }
		public String NumeroProcessoSEP { get; set; }
		public PessoaPDF TitularRegistro { set; get; }

		private ConfiguracaoVegetalItemPDF _classificacaoToxicologica = new ConfiguracaoVegetalItemPDF();
		public ConfiguracaoVegetalItemPDF ClassificacaoToxicologica
		{
			get { return _classificacaoToxicologica; }
			set { _classificacaoToxicologica = value; }
		}

		private ConfiguracaoVegetalItemPDF _periculosidade = new ConfiguracaoVegetalItemPDF();
		public ConfiguracaoVegetalItemPDF Periculosidade
		{
			get { return _periculosidade; }
			set { _periculosidade = value; }
		}

		public String ClasseStragg
		{
			get
			{
				return EntitiesBus.Concatenar(Classes.Select(x => x.Nome).ToList());
			}
		}
		public String IngredienteStragg
		{
			get
			{
				return EntitiesBus.Concatenar(Ingredientes.Select(x => x.Nome).ToList());
			}
		}
		public String CulturaStragg
		{
			get
			{
				return EntitiesBus.Concatenar(Culturas.Select(x => x.Nome).ToList());
			}
		}

		private List<ConfiguracaoVegetalItemPDF> _classes = new List<ConfiguracaoVegetalItemPDF>();
		public List<ConfiguracaoVegetalItemPDF> Classes
		{
			get { return _classes; }
			set { _classes = value; }
		}

		private List<ConfiguracaoVegetalItemPDF> _ingredientes = new List<ConfiguracaoVegetalItemPDF>();
		public List<ConfiguracaoVegetalItemPDF> Ingredientes
		{
			get { return _ingredientes; }
			set { _ingredientes = value; }
		}

		private List<AgrotoxicoCulturaPDF> _culturas = new List<AgrotoxicoCulturaPDF>();
		public List<AgrotoxicoCulturaPDF> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}

		public AgrotoxicoPDF() { }

		public AgrotoxicoPDF(Agrotoxico agrotoxico)
		{
			Numero = agrotoxico.NumeroCadastro.ToString();
			NomeComercial = agrotoxico.NomeComercial;
			NumeroProcessoSEP = agrotoxico.NumeroProcessoSep.ToString();
			TitularRegistro = new PessoaPDF() { NomeRazaoSocial = agrotoxico.TitularRegistro.NomeRazaoSocial, CPFCNPJ = agrotoxico.TitularRegistro.CPFCNPJ };
			ClassificacaoToxicologica.Nome = agrotoxico.ClassificacaoToxicologica.Texto;
			Periculosidade.Nome = agrotoxico.PericulosidadeAmbiental.Texto;

			//Classes
			foreach (var item in agrotoxico.ClassesUso)
			{
				Classes.Add(new ConfiguracaoVegetalItemPDF() { Nome = item.Texto });
			}

			//Ingredientes
			foreach (var item in agrotoxico.IngredientesAtivos)
			{
				Ingredientes.Add(new ConfiguracaoVegetalItemPDF() { Nome = item.Texto });
			}

			//Culturas
			foreach (var item in agrotoxico.Culturas)
			{
				Culturas.Add(new AgrotoxicoCulturaPDF() { Nome = item.Cultura.Nome });
			}
		}
	}
}
