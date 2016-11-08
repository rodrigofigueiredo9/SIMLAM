using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloEmail;
using Tecnomapas.Blocos.Etx.ModuloEmail.Data;

namespace Tecnomapas.Blocos.Etx.ModuloEmail.Business
{
	public class EmailBus
	{
		EmailDa _da = null;

		public EmailBus(string esquema = null)
		{
			_da = new EmailDa(esquema);
		}

		public int Enviar(Email email, BancoDeDados banco = null)
		{
			if (banco == null)
			{
				GerenciadorTransacao.GerarNovoID();
			}

			return _da.Enviar(email, banco);
		}

		public void Deletar(int id, BancoDeDados banco = null)
		{
			_da.Deletar(id, banco);
		}

		public void Deletar(eEmailTipo tipo, int codigo, BancoDeDados banco = null)
		{
			_da.Deletar(tipo, codigo, banco);
		}
	}
}