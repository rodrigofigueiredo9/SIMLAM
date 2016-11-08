using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	static class ExportadorFactory
	{
		public static IExportador Criar(int tipo)
		{
			switch (tipo)
			{
				case 1:
					return new ExportadorPdf();
				case 2:
					return new ExportadorXls();
				default:
					Validacao.Add(Mensagem.RelatorioPersonalizado.ExportadorInvalido);
					return null;
			}
		}
	}
}