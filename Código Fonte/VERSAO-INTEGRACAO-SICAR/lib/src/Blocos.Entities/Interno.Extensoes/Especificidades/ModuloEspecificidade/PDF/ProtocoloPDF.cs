using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ProtocoloPDF
	{
		public const String PROCESSO = "Processo";
		public const String DOCUMENTO = "Documento";

		public Int32? Id { get; set; }
		public bool IsProcesso { get; set; }
		public String DataCriacao { get; set; }
		public String Texto { get; set; }
		public String Numero { get; set; }
		public String NumeroAutuacao { get; set; }
		public String NumeroSEP { get { return NumeroAutuacao; } }
		public String DataAutuacao { get; set; }
		public String Tipo { get; set; }
		public String ResponsaveisNomes { get; set; }
		public PessoaPDF Interessado { get; set; }
		public PessoaPDF InteressadoRepresentante { get; set; }
		public List<ResponsavelPDF> Responsaveis { get; set; }
		public RequerimentoPDF Requerimento { get; set; }

		public String DataCriacaoMenor
		{
			get
			{
				if (!String.IsNullOrWhiteSpace(DataAutuacao))
				{
					DateTime _dataCriacao;
					DateTime _dataAutuacao;

					if (DateTime.TryParse(DataCriacao, out _dataCriacao) && DateTime.TryParse(DataAutuacao, out _dataAutuacao))
					{
						return (_dataAutuacao < _dataCriacao) ? _dataAutuacao.ToShortDateString() : _dataCriacao.ToShortDateString();
					}
				}

				return DataCriacao;
			}
		}

		public String TextoUpper
		{
			get { return String.IsNullOrEmpty(Texto) ? string.Empty : Texto.ToUpper(); }
		}

		public String TextoLower
		{
			get { return String.IsNullOrEmpty(Texto) ? string.Empty : Texto.ToLower(); }
		}

		public ProtocoloPDF()
		{
			Interessado = new PessoaPDF();
			InteressadoRepresentante = new PessoaPDF();
			Responsaveis = new List<ResponsavelPDF>();
			Requerimento = new RequerimentoPDF();
		}
	}
}