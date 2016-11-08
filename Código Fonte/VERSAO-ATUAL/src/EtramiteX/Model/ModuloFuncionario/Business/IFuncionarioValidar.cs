using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business
{
	public interface IFuncionarioValidar
	{
		bool Salvar(Funcionario funcionario, String senha, String ConfirmarSenha);
		bool Cpf(String Cpf);
		bool Autenticar(Funcionario funcionario);
		bool AlterarSenha(string login, string senha, string novaSenha, string confirmarNovaSenha);
		bool AlterarSenhaFuncionario(string login, string novaSenha, string confirmarNovaSenha);
		bool AlterarSituacao(int Situacao, String Motivo);
		bool VerificarAlterarFuncionario(int id);
	}
}
