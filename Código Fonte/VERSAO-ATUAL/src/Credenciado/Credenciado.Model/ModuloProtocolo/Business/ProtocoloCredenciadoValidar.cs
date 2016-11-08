using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Business
{
	class ProtocoloCredenciadoValidar
	{
		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public bool EmPosse(IProtocolo protocolo)
		{
			CredenciadoBus _bus = new CredenciadoBus();
			var credenciado = _bus.Obter(User.FuncionarioId);

			bool credenciadoInteressado = credenciado.Pessoa.InternoId == protocolo.Interessado.Id;
			bool credenciadoResponsavel = protocolo.Responsaveis.Exists(x=> x.Id == credenciado.Pessoa.InternoId);

			if (!credenciadoInteressado && !credenciadoResponsavel)
			{
				if (protocolo.IsProcesso)
				{
					Validacao.Add(Mensagem.Processo.PosseCredenciado);
				}
				else
				{
					Validacao.Add(Mensagem.Documento.PosseCredenciado);
				}

			}
			
			return Validacao.EhValido;
		}
	}
}
