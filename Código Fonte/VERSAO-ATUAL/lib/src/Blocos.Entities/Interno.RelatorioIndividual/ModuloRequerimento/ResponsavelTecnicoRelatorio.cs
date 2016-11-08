using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento
{
	public class ResponsavelTecnicoRelatorio
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public string NomeRazao { get; set; }
		public string CpfCnpj { get; set; }
		public string RgIe { get; set; }
		public string FuncaoTexto { get; set; }
		public string NumeroArt { get; set; }
		public string DataVencimento { get; set; }
		public string DataNascimento { get; set; }

		private EnderecoRelatorio _endereco = new EnderecoRelatorio();
		public EnderecoRelatorio Endereco
		{
			get { return _endereco; }
			set { _endereco = value; }
		}

		private List<ContatoRelatorio> _meios = new List<ContatoRelatorio>();
		public List<ContatoRelatorio> MeiosContatos
		{
			get { return _meios; }
			set { _meios = value; }
		}

		public string TelResidencia { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneResidencial) ?? new ContatoRelatorio()).Valor; } }
		public string TelCelular { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneCelular) ?? new ContatoRelatorio()).Valor; } }
		public string TelFax { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneFax) ?? new ContatoRelatorio()).Valor; } }
		public string TelComercial { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneComercial) ?? new ContatoRelatorio()).Valor; } }
		public string Email { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.Email) ?? new ContatoRelatorio()).Valor; } }

		public string NumeroHabilitacao { get; set; }
		public string Registro { get; set; }
	}
}