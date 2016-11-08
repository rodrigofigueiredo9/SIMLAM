using System;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPessoa
{
	public class Fisica
	{
		public String CPF { get; set; }
		public String Nome { get; set; }
		[Display(Name = "RG/Órgão expedidor/UF", Order = 12)]
		public String RG { get; set; }
		public Int32? EstadoCivil { get; set; }
		[Display(Order = 14, Name = "Estado Civil")]
		public String EstadoCivilTexto { get; set; }
		public Int32? Sexo { get; set; }
		[Display(Order = 11, Name = "Sexo")]
		public String SexoTexto { get; set; }
		[Display(Order = 15)]
		public String Nacionalidade { get; set; }
		[Display(Order = 16)]
		public String Naturalidade { get; set; }
		public DateTime? DataNascimento { get; set; }
		public Profissao Profissao { get; set; }
		[Display(Order = 3)]
		public String Apelido { get; set; }
		[Display(Order = 13, Name = "Data de Nascimento")]
		public String DataNascimentoTexto
		{
			get
			{
				if (DataNascimento != null && DataNascimento.Value > DateTime.MinValue)
				{
					return DataNascimento.Value.ToShortDateString();
				}

				return null;
			}
			set
			{
				DataNascimento = ParseData(value);
			}
		}

		public Pessoa Conjuge { get; set; }
		public Int32? ConjugeId { get; set; }
		public Int32? ConjugeInternoId { get; set; }
		[Display(Name = "Cônjuge", Order = 17)]
		public String ConjugeNome { get; set; }
		[Display(Name = "CPF do Cônjuge", Order = 18)]
		public String ConjugeCPF { get; set; }

		[Display(Name = "Nome do Pai", Order = 19)]
		public String NomeMae { get; set; }
		[Display(Name = "Nome da Mãe", Order = 20)]
		public String NomePai { get; set; }

		public Fisica()
		{
			Profissao = new Profissao();
		}

		public static bool ValidarData(String data)
		{
			try
			{
				DateTime.Parse(data).ToString("dd/MM/yyyy");
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static DateTime? ParseData(String dataTexto)
		{

			if (String.IsNullOrEmpty(dataTexto))
			{
				return DateTime.MinValue;
			}
			else if (ValidarData(dataTexto) && dataTexto != DateTime.MinValue.ToShortDateString())
			{
				return DateTime.Parse(dataTexto);
			}
			else
			{
				return null;
			}
		}
	}
}