namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape
{
	public interface ICabecalhoRodape
	{
		string GovernoNome { get; set; }
		string SecretariaNome { get; set; }
		
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

