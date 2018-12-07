using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class TituloPDF : DataSourceBase
	{
		public Int32? Id { get; set; }
		public String ModeloNome { get; set; }
		public String ModeloNomeUpper
		{
			get
			{
				return ModeloNome.ToUpper();
			}
		}
		public String ModeloSigla { get; set; }
		public String ModeloHierarquia { get; set; }
		public String Numero { get; set; }
		public String LocalEmissao { get; set; }
		public String DiaEmissao { get; set; }
		public String MesEmissao { get; set; }
		public String AnoEmissao { get; set; }
		public String Prazo { get; set; }
		public Int32 SetorId { get; set; }
		public int SituacaoId { get; set; }
		public String Situacao { get; set; }
		public String DataEmissao { get; set; }
		public String DataEmissao2 { get; set; }
		public String DataVencimento { get; set; }
		public String TituloAnteriorDiaEmissao { get; set; }
		public String TituloAnteriorMesEmissao { get; set; }
		public String TituloAnteriorAnoEmissao { get; set; }
		public String AutorNome { get; set; }

		public String NumeroNumero
		{
			get
			{
				string[] values = (Numero ?? "").Split(new Char[] { '/' });
				return values[0];
			}
		}

		public String NumeroAno
		{
			get
			{
				string[] values = (Numero ?? "").Split(new Char[] { '/' });
				return values.Length == 2 ? values[1] : "";
			}
		}

		private SetorEnderecoPDF _setorEndereco = new SetorEnderecoPDF();
		public SetorEnderecoPDF SetorEndereco
		{
			get { return _setorEndereco; }
			set { _setorEndereco = value; }
		}

		private String _prazoTipo;
		public String PrazoTipo
		{
			get
			{
				if (!String.IsNullOrEmpty(Prazo) && !String.IsNullOrEmpty(_prazoTipo) && !_prazoTipo.EndsWith("s") && Int32.Parse(Prazo) > 1)
				{
					return _prazoTipo.ToLower() + "s";
				}
				return (!String.IsNullOrEmpty(_prazoTipo)) ? _prazoTipo.ToLower() : _prazoTipo;
			}
			set { _prazoTipo = value; }
		}

		private List<PessoaPDF> _destinatarios = new List<PessoaPDF>();
		public List<PessoaPDF> DestinatariosEmail
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private List<String> _atividades = new List<String>();
		public List<String> Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<String> _atividadesFinalidadeModelo = new List<String>();
		public List<String> AtividadesFinalidadeModelo
		{
			get { return _atividadesFinalidadeModelo; }
			set { _atividadesFinalidadeModelo = value; }
		}

		private List<TituloPDF> _associados = new List<TituloPDF>();
		public List<TituloPDF> Associados { get { return _associados; } set { _associados = value; } }

		private RequerimentoPDF _requerimento = new RequerimentoPDF();
		public RequerimentoPDF Requerimento { get { return _requerimento; } set { _requerimento = value; } }

		public String TituloAssociadoModelo
		{
			get
			{
				if (_associados.Count == 1)
				{
					return _associados.First().ModeloNome;
				}

				return string.Empty;
			}
		}

		public String TituloAssociadoNumero
		{
			get
			{
				if (_associados.Count == 1)
				{
					return _associados.First().Numero;
				}

				return string.Empty;
			}
		}

		public String Atividade
		{
			get
			{
				if (Atividades == null || Atividades.Count == 0)
				{
					return String.Empty;
				}

				return EntitiesBus.Concatenar(Atividades);
			}
		}

		public String AtividadeFinalidadesTitulos
		{
			get
			{
				if (AtividadesFinalidadeModelo == null || AtividadesFinalidadeModelo.Count == 0)
				{
					return String.Empty;
				}

				return EntitiesBus.Concatenar(AtividadesFinalidadeModelo);
			}
		}

		public String AtividadeCodigoCategoria { get; set; }

		public String NumeroSinaflor { get; set; }
	}
}