using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa
{
	public class CredenciadoRelatorio
	{
		public Int32 Id {get; set; }
		public Int32? Ativa { get; set; }
		public Int32 PessoaTipo { get; set; }
		public String PessoaTipoTexto { get; set; }
		
		public Int32 TipoId { get; set; }
		public String TipoTexto { get; set; }

		public String Tipo
		{
			get
			{
				if (TipoId == 3/*Orgao Parceiro*/) {
					return OrgaoParceiroNome + " - " + OrgaoParceiroSigla;
				}

				return TipoTexto;
			}
		}

		public Int32 OrgaoParceiroId { get; set; }
		public String OrgaoParceiroTid { get; set; }

		public String OrgaoParceiroNome { get; set; }
		public String OrgaoParceiroSigla { get; set; }

		public String Escopo { get; set; }
		public String Tid { get; set; }
		public String NomeFantasia { get; set; }
		public String NomeFantasiaApelido { get { return IsFisica ? Fisica.Apelido : Juridica.NomeFantasia; } }

		private FisicaRelatorio _fisica = new FisicaRelatorio();
		public FisicaRelatorio Fisica
		{
			get { return _fisica; }
			set { _fisica = value; }
		}

		private JuridicaRelatorio _juridica = new JuridicaRelatorio();
		public JuridicaRelatorio Juridica
		{
			get { return _juridica; }
			set { _juridica = value; }
		}
		
		public String NomeRazaoSocial
		{
			get { return IsFisica ? Fisica.Nome : Juridica.RazaoSocial; }
			set { if (IsFisica) Fisica.Nome = value; else Juridica.RazaoSocial = value; }
		}
		
		public String CPFCNPJ
		{
			get { return IsFisica ? Fisica.CPF : Juridica.CNPJ; }
			set { if (IsFisica) Fisica.CPF = value; else Juridica.CNPJ = value; }
		}
		
		public String RGIE
		{
			get { return IsFisica ? Fisica.RG : Juridica.IE; }
		}

		private List<ContatoRelatorio> _meios = new List<ContatoRelatorio>();
		public List<ContatoRelatorio> MeiosContatos 
		{
			get { return _meios; }
			set { _meios = value; }
		}

		private EnderecoRelatorio _endereco = new EnderecoRelatorio();
		public EnderecoRelatorio Endereco
		{
			get { return _endereco; }
			set { _endereco = value; }
		}

		public bool IsFisica
		{
			get { return (PessoaTipo == 1); }
		}

		public bool IsJuridica
		{
			get { return (PessoaTipo == 2); }
		}
		
		public CredenciadoRelatorio() 
		{
			PessoaTipo = 1;
		}

		public string TelResidencia { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneResidencial) ?? new ContatoRelatorio()).Valor; } }
		public string TelCelular { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneCelular) ?? new ContatoRelatorio()).Valor; } }
		public string TelFax { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneFax) ?? new ContatoRelatorio()).Valor; } }
		public string TelComercial { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.TelefoneComercial) ?? new ContatoRelatorio()).Valor; } }
		public string Email { get { return (MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.Email) ?? new ContatoRelatorio()).Valor; } }
	}
}