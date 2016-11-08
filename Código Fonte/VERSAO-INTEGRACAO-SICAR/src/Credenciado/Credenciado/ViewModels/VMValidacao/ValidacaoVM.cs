using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMValidacao
{
	public class ValidacaoVM
	{
		public static int MaxLength
		{
			get { return 300; }
		}
		public static int MaxQtdItens
		{
			get { return 3; }
		}

		public static bool TemMensagem(eTipoMensagem tipo)
		{
			return Validacao.Erros.Any(x => x.Tipo == tipo);
		}

		public static IEnumerable<Mensagem> Mensagens(eTipoMensagem tipo)
		{
			return Validacao.Erros.Where(x => x.Tipo == tipo);
		}

		public static bool ExibirMais(eTipoMensagem tipo)
		{
			return (Validacao.Erros.Where(x => x.Tipo == tipo).Count() > MaxQtdItens || Validacao.Erros.Where(x => x.Tipo == tipo).Sum(x => x.Texto.Length) >= MaxLength);
		}
	}
}