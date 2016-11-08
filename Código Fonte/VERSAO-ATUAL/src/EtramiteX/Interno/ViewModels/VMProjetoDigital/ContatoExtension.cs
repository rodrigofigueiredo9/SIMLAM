using System.Linq;
using System.Text.RegularExpressions;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloCore.View;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital
{
	public static class ContatoExtension
	{
		public static Campo GerarCampo(this Contato contato)
		{
			Objeto meio = GeradorVisualizacao.GerarObjeto(contato);
			Campo novoMeio = new Campo();
			novoMeio.Alias = SepararPalavras(meio.Campos.FirstOrDefault(x => x.Alias == "TipoTexto").Valor);

			if (novoMeio.Alias == "Nome Contato")
			{
				novoMeio.Alias = "Nome para Contato";
			}

			novoMeio.Valor = meio.Campos.FirstOrDefault(x => x.Alias == "Valor").Valor;
			return novoMeio;
		}

		static string SepararPalavras(string entrada)
		{
			var r = new Regex(@"
				(?<=[A-Z])(?=[A-Z][a-z]) |
				(?<=[^A-Z])(?=[A-Z]) |
				(?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

			return r.Replace(entrada, " ");
		}
	}
}