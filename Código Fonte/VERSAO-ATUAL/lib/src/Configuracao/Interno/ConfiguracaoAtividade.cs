using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno
{
	public class ConfiguracaoAtividade : ConfiguracaoBase
	{
		private ListaValoresDa _daLista = new ListaValoresDa();

		public const string KeyAgrupadoresAtividade = "AgrupadoresAtividade";

		public List<AtividadeAgrupador> AgrupadoresAtividade
		{
			get { return _daLista.ObterAgrupadoresAtividade(); }
		}


		public const string KeyAgrupadoresAtividadeCredenciado = "AgrupadoresAtividadeCredenciado";

		public List<AtividadeAgrupador> AgrupadoresAtividadeCredenciado
		{
			get { return _daLista.ObterAgrupadoresAtividadeCredenciado(); }
		}

		public static string KeyAtividadesCaracterizacoes = "AtividadesCaracterizacoes";

		public List<AtividadeCaracterizacao> AtividadesCaracterizacoes
		{
			get { return _daLista.ObterAtividadesCaracterizacoes(); }
		}

		public static string KeyAtividadesCategoria = "AtividadesCategoria";

		public List<ListaValor> AtividadesCategoria
		{
			get { return _daLista.ObterAtividadesCategoria(); }
		}

		public static int ObterId(int codigo)
		{
			ListaValoresDa daLista = new ListaValoresDa();
			return daLista.ObterAtividadeId(codigo);
		}

		public static IEnumerable<int> ObterId(IEnumerable<int> codigos)
		{
			ListaValoresDa daLista = new ListaValoresDa();
			List<int> lstRetorno = new List<int>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				foreach (var item in codigos)
				{
					lstRetorno.Add(daLista.ObterAtividadeId(item, bancoDeDados));
				}
			}
			return lstRetorno;
		}

		public static int ObterCodigo(int id)
		{
			ListaValoresDa daLista = new ListaValoresDa();
			return daLista.ObterAtividadeCodigo(id);
		}

		public static IEnumerable<int> ObterCodigo(IEnumerable<int> ids)
		{
			ListaValoresDa daLista = new ListaValoresDa();
			List<int> lstRetorno = new List<int>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				foreach (var item in ids)
				{
					lstRetorno.Add(daLista.ObterAtividadeCodigo(item, bancoDeDados));
				}
			}
			return lstRetorno;
		}
	}
}