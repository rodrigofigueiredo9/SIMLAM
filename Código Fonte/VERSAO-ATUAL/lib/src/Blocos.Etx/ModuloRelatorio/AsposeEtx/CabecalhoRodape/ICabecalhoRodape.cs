namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape
{
	public interface ICabecalhoRodape
	{
		byte[] LogoOrgao { get; set; }
		byte[] LogoSimlam { get; set; }
		byte[] LogoEstado { get; set; }

		string GovernoNome { get; set; }
		string SecretariaNome { get; set; }
		string SecretariaSigla { get; set; }

		string OrgaoNome { get; set; }
		string OrgaoSigla { get; set; }

		string OrgaoEndereco { get; set; }
		string OrgaoMunicipio { get; set; }
		string OrgaoUF { get; set; }
		string OrgaoCep { get; set; }
		string OrgaoContato { get; set; }

		string SetorNome { get; set; }
	}
}