using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{	
	public enum eAsposeImageDimensao
	{
		Ambos,
		Largura,
		Altura
	}

	public class AsposeImage
	{
		public static byte[] RedimensionarImagem(byte[] arquivo, float tamanho, eAsposeImageDimensao resize = eAsposeImageDimensao.Ambos)
		{
			// Cria uma imagem a partir do arquivo recebido
			Image bmp = Image.FromStream(new MemoryStream(arquivo));

			// Descobre resolucao final da imagem redimesionada
			int tamanhoOriginal = 0;
			switch (resize)
			{
				case eAsposeImageDimensao.Ambos:
					tamanhoOriginal = bmp.Height > bmp.Width ? bmp.Height : bmp.Width;
					break;
				case eAsposeImageDimensao.Largura:
					tamanhoOriginal = bmp.Width;
					break;
				case eAsposeImageDimensao.Altura:
					tamanhoOriginal = bmp.Height;
					break;
			}
			float resolucao = 72 * tamanhoOriginal / (tamanho / 2.54F * 72);
			int largura, altura;

			if (resolucao <= 300)
			{
				largura = bmp.Width;
				altura = bmp.Height;
			}
			else
			{
				largura = (int)(bmp.Width / (resolucao / 300));
				altura = (int)(bmp.Height / (resolucao / 300));
				resolucao = 300;
			}

			// Redimensiona a imagem original
			Bitmap miniatura = new Bitmap(largura, altura, PixelFormat.Format24bppRgb);
			miniatura.SetResolution(resolucao, resolucao);
			Graphics grPhoto = Graphics.FromImage(miniatura);
			grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
			grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
			grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
			grPhoto.DrawImage(bmp, new Rectangle(0, 0, largura, altura), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel);

			// Extrai os bytes da imagem e limpa referencias
			MemoryStream mm = new MemoryStream();
			miniatura.Save(mm, ImageFormat.Jpeg);
			miniatura.Dispose();
			bmp.Dispose();

			return mm.GetBuffer();
		}
	}
}
