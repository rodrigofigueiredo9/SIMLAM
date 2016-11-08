using System.IO;
using System.Web;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.RelatorioPersonalizado.Business;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class ConfiguracaoDocumentoPDF
	{
		public string RelatorioNome { get; set; }
		public string RelatorioDataHoraImpressao { get; set; }
		public string RelatorioDataHoraAtualizacao { get; set; }
		public byte[] LogoOrgao { get; private set; }
		public byte[] LogoSimlam { get; private set; }
		public string LogoMarca { get; set; }
		public string LogoMarcaSimlam { get; set; }
		private string CaminhoDocumentoRetrato { get; set; }
		private string CaminhoDocumentoPaisagem { get; set; }
		public string CaminhoDocumento { get { return OrientacaoRetrato ? CaminhoDocumentoRetrato:CaminhoDocumentoPaisagem; } }
		public bool OrientacaoRetrato { get; set; }

		private ICabecalhoRodape _cabecalhoRodape = CabecalhoRodapeFactory.Criar();
		public ICabecalhoRodape CabecalhoRodape
		{
			get { return _cabecalhoRodape; }
			set { _cabecalhoRodape = value; }
		}

		public ConfiguracaoDocumentoPDF() { }

		public ConfiguracaoDocumentoPDF(string logoMarca, string logoMarcaSimlam, string caminhoDocumentoRetrato, string caminhoDocumentoPaisagem)
		{
			CaminhoDocumentoRetrato = caminhoDocumentoRetrato;
			CaminhoDocumentoPaisagem = caminhoDocumentoPaisagem;

			LogoMarca = logoMarca;
			LogoMarcaSimlam = logoMarcaSimlam;

			string pathImg = HttpContext.Current.Request.MapPath(logoMarca);
			LogoOrgao = File.ReadAllBytes(pathImg);

			LogoOrgao = AsposeImage.RedimensionarImagem(LogoOrgao, 1.8f);

			pathImg = HttpContext.Current.Request.MapPath(logoMarcaSimlam);
			LogoSimlam = File.ReadAllBytes(pathImg);

			LogoSimlam = AsposeImage.RedimensionarImagem(LogoSimlam, 1.8f);
		}
	}
}