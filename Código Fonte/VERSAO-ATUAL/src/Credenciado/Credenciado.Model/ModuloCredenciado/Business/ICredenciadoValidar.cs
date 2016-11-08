using System;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business
{
	public interface ICredenciadoValidar
	{
		bool Salvar(CredenciadoPessoa credenciado, String senha, String confirmarSenha);
		bool AlterarSenha(string login, string senha, string novaSenha, string confirmarNovaSenha);
		bool AlterarSenhaCredenciado(string login, string novaSenha, string confirmarNovaSenha);
		bool ValidarAtivar(string chave, string login, string senha, string confirmarSenha);
		bool ValidarReativar(string chave, string senha, string confirmarSenha);
		bool VerificarChaveAtiva(String chave);
	}
}