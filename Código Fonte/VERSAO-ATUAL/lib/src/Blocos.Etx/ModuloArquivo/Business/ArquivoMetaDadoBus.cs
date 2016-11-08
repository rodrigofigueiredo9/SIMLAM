

using Tecnomapas.Blocos.Arquivo.Data;

namespace Tecnomapas.Blocos.Etx.ModuloArquivo.Business
{
	public class ArquivoMetaDadoBus
	{
		ArquivoDa _arquivoDa = new ArquivoDa();

		public void MarcarDeletado(int oficialId, string arquivoUrl)
		{
			_arquivoDa.MarcarDeletado(oficialId, arquivoUrl);
		}
	}
}
