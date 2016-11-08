using System;
using System.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.PDF
{
	public enum tipoFonte
	{
		ArialNarrow,		
		ArialBlack,
		Arial
	}

	public class PdfMetodosAuxiliares
	{
		#region Fontes Estaticas

        public static Font arial6 = GerarFonte(tipoFonte.Arial, 6, 0, BaseColor.BLACK);
        public static Font arial8 = GerarFonte(tipoFonte.Arial, 8, 0, BaseColor.BLACK);
        public static Font arial9 = GerarFonte(tipoFonte.Arial, 9, 0, BaseColor.BLACK);
        public static Font arial10 = GerarFonte(tipoFonte.Arial, 10, 0, BaseColor.BLACK);
        public static Font arial12 = GerarFonte(tipoFonte.Arial, 12, 0, BaseColor.BLACK);
        public static Font arial16 = GerarFonte(tipoFonte.Arial, 16, 0, BaseColor.BLACK);

        public static Font arial6Negrito = GerarFonte(tipoFonte.Arial, 6, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public static Font arial8Negrito = GerarFonte(tipoFonte.Arial, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public static Font arial9Negrito = GerarFonte(tipoFonte.Arial, 9, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public static Font arial10Negrito = GerarFonte(tipoFonte.Arial, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
        public static Font arial16Negrito = GerarFonte(tipoFonte.Arial, 16, iTextSharp.text.Font.BOLD, BaseColor.BLACK);

		#endregion

		#region Gerador Fontes Dinamicas (OLD)
        /*
		public static Font GerarFonte(tipoFonte tipo, int tamanho, int style, BaseColor corFonte)
		{
			Font fonteRetorno = null;			

			//string path = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath) + "\\Fontes\\";
            string path = "C:\\WINDOWS\\Fonts\\";

			switch (tipo)
			{
				case tipoFonte.ArialNarrow:
					path += "ARIALN.TTF";
					FontFactory.Register(path, tipoFonte.ArialNarrow.ToString());
					fonteRetorno = FontFactory.GetFont(tipoFonte.ArialNarrow.ToString(), tamanho , style, corFonte);
					break;

				case tipoFonte.ArialBlack:
					path += "ariblk.ttf";
					FontFactory.Register(path);					
					fonteRetorno = FontFactory.GetFont("Arial Black", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, style, corFonte);
					break;

				case tipoFonte.Arial:
					path += "ARIAL.ttf";
					FontFactory.Register(path);					
					fonteRetorno = FontFactory.GetFont("ARIAL", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, style, corFonte);
					break;
			}
			return fonteRetorno;
		}

		public static Font GerarFonte(tipoFonte tipo,int estilo, int tamanho)
		{
			return GerarFonte(tipo, tamanho, estilo ,BaseColor.BLACK);
        }
        */
        #endregion


        #region Gerador Fontes Dinamicas

        private static void ExportarFontes(string path)
        {
            Type tipoClasse = typeof(PdfMetodosAuxiliares);
            string nameSpace = tipoClasse.Namespace.Substring(0, tipoClasse.Namespace.Length-4);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ARIALN.TTF"))
            {
                using (FileStream fileStr = new FileStream(path + "ARIALN.TTF", FileMode.Create))
                {
                    stream.CopyTo(fileStr);

                    stream.Close();
                    fileStr.Close();
                }
            }

            using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ariblk.ttf"))
            {
                using (FileStream fileStr = new FileStream(path + "ariblk.ttf", FileMode.Create))
                {
                    stream.CopyTo(fileStr);

                    stream.Close();
                    fileStr.Close();
                }
            }

            using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ARIAL.TTF"))
            {
                using (FileStream fileStr = new FileStream(path + "ARIAL.TTF", FileMode.Create))
                {
                    stream.CopyTo(fileStr);

                    stream.Close();
                    fileStr.Close();
                }
            }

            using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ArialBold.ttf"))
            {
                using (FileStream fileStr = new FileStream(path + "ArialBold.ttf", FileMode.Create))
                {
                    stream.CopyTo(fileStr);

                    stream.Close();
                    fileStr.Close();
                }
            }

			using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.arialbi.ttf"))
			{
				using (FileStream fileStr = new FileStream(path + "arialbi.ttf", FileMode.Create))
				{
					stream.CopyTo(fileStr);

					stream.Close();
					fileStr.Close();
				}
			}

			using (Stream stream = tipoClasse.Assembly.GetManifestResourceStream(nameSpace + ".Fontes.ariali.ttf"))
			{
				using (FileStream fileStr = new FileStream(path + "ariali.ttf", FileMode.Create))
				{
					stream.CopyTo(fileStr);

					stream.Close();
					fileStr.Close();
				}
			}
        }

        public static Font GerarFonte(tipoFonte tipo, int tamanho, int style, BaseColor corFonte)
        {
            Font fonteRetorno = null;

            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Etx\\";
			if (!File.Exists(path + "ARIALN.TTF") || !File.Exists(path + "ariblk.ttf") || !File.Exists(path + "ARIAL.TTF") || !File.Exists(path + "ArialBold.ttf") || !File.Exists(path + "arialbi.ttf") || !File.Exists(path + "ariali.ttf"))
            {
                ExportarFontes(path);
            }

			switch (tipo)
			{
				case tipoFonte.ArialNarrow:
					path += "ARIALN.TTF";
					if (!FontFactory.IsRegistered(tipoFonte.ArialNarrow.ToString()))
					{
						FontFactory.Register(path, tipoFonte.ArialNarrow.ToString());
					}
					fonteRetorno = FontFactory.GetFont(tipoFonte.ArialNarrow.ToString(), tamanho, style, corFonte);
					break;

				case tipoFonte.ArialBlack:
					path += "ariblk.ttf";
					if (!FontFactory.IsRegistered("Arial Black"))
					{
						FontFactory.Register(path, "Arial Black");
					}
					fonteRetorno = FontFactory.GetFont("Arial Black", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, style, corFonte);
					break;

				case tipoFonte.Arial:

					if (style == Font.BOLD)
					{
						path += "ArialBold.ttf";
						if (!FontFactory.IsRegistered("Arial,Bold"))
						{
							FontFactory.Register(path, "Arial,Bold");
						}
						fonteRetorno = FontFactory.GetFont("Arial,Bold", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, Font.NORMAL, corFonte);
						break;
					}

					if (style == Font.ITALIC)
					{
						path += "ariali.ttf";
						if (!FontFactory.IsRegistered("Arial,Italic"))
						{
							FontFactory.Register(path, "Arial,Italic");
						}
						fonteRetorno = FontFactory.GetFont("Arial,Italic", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, Font.NORMAL, corFonte);
						break;
					}

					if (style == Font.BOLDITALIC)
					{
						path += "arialbi.ttf";
						if (!FontFactory.IsRegistered("Arial,Bold,Italic"))
						{
							FontFactory.Register(path, "Arial,Bold,Italic");
						}
						fonteRetorno = FontFactory.GetFont("Arial,Bold,Italic", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, Font.NORMAL, corFonte);
						break;
					}

					path += "ARIAL.TTF";
					if (!FontFactory.IsRegistered("ARIAL"))
					{
						FontFactory.Register(path, "ARIAL");
					}
					fonteRetorno = FontFactory.GetFont("ARIAL", BaseFont.CP1252, BaseFont.NOT_EMBEDDED, tamanho, style, corFonte);
					break;
			}
            return fonteRetorno;
        }

        public static Font GerarFonte(tipoFonte tipo, int estilo, int tamanho)
        {
            return GerarFonte(tipo, tamanho, estilo, BaseColor.BLACK);
        }

        public static Font MudarCorFonte(Font fonte, BaseColor novaCor)
        {
            Font NovaFonte = new Font(fonte);
            NovaFonte.Color = novaCor;
            return NovaFonte;
        }

        #endregion


        /*
		public static Font MudarCorFonte (Font fonte, BaseColor novaCor)
		{
			Font NovaFonte = new Font(fonte);
			NovaFonte.Color = novaCor;
			return NovaFonte;
		}

		#endregion
		
		#region Configuração Cabecalho/Rodape

		public enum InfConfig
		{
			EstadoOrgaoNome,
			EstadoOrgaoSigla,
			OrgaoNome,
			OrgaoSigla,
			EnderecoSema,
			ContatoSema,
			Site,
			EnderecoSUDEC,
			ContatoSUDEC
		}

		#endregion 

        public static string TruncarDecimal(string numero, int casasPrecisao)
        {
            return TruncarDecimal(numero.Split(','), casasPrecisao);
        }

        public static string TruncarDecimal(decimal numero, int casasPrecisao)
        {
            return TruncarDecimal(numero.ToString().Split(','), casasPrecisao);
        }

        private static string TruncarDecimal(string[] numero, int casasPrecisao)
        {
            string precisao = string.Empty;

            if (numero.Length > 1)
            {
                precisao = numero[1];
            }

            string retorno = numero[0] + ",";
            for (int i = 0; i < casasPrecisao; i++)
            {
                retorno += ((precisao.Length > i) ? precisao[i].ToString() : "0");
            }
            return retorno;
        }
         * */
	}
}
