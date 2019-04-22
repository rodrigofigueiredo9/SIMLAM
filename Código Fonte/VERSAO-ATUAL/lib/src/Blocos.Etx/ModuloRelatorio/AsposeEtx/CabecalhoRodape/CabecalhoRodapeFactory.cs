

using System.Web;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape
{
	public static class CabecalhoRodapeFactory
	{
		public static ICabecalhoRodape Criar(int setor = 0, bool isBrasao = false, bool isLogo = false, bool isCredenciado = false)
		{
			CabecalhoRodapeDefault cab = new CabecalhoRodapeDefault(isBrasao, isLogo, isCredenciado);
			CabecalhoRodapeBus bus = new CabecalhoRodapeBus();
			
			if (setor > 0)
			{
				return bus.ObterEnderecoSetor(cab, setor);
			}

			if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated && !isCredenciado)
			{
				return bus.ObterEnderecoFuncLogado(cab);
			}

			return bus.ObterEnderecoDefault(cab);
		}
	}
}
