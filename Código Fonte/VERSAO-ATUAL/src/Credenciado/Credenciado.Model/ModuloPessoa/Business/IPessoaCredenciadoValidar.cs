using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business
{
	public interface IPessoaCredenciadoValidar
	{
		bool Salvar(Pessoa pessoa, bool isConjuge = false, BancoDeDados bancoDeDados = null);
		bool VerificarCriarCnpj(Pessoa pessoa);
		bool VerificarCriarCpf(Pessoa pessoa, bool isConjuge = false);
		bool VerificarExcluirPessoa(int id);
		bool VerificarPessoaCriar(Pessoa pessoa, bool isConjuge = false, BancoDeDados bancoDeDados = null);
		bool VerificarPessoaEditar(Pessoa pessoa, bool isConjuge = false);
		bool VerificarPessoaFisica(Pessoa pessoa, bool isConjuge = false, BancoDeDados bancoDeDados = null);
		bool VerificarPessoaJuridica(Pessoa pessoa);
		bool ValidarAssociarRepresentante(int pessoaId);
		bool ValidarAssociarResponsavelTecnico(int id);
		IPessoaMsg Msg { get; set; }

	}
}
