using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business
{
	public interface IPessoaValidar
	{
		bool Salvar(Pessoa pessoa);
		bool VerificarCriarCnpj(Pessoa pessoa);
		bool VerificarCriarCpf(Pessoa pessoa);
		bool VerificarExcluirPessoa(int id);
		bool VerificarPessoaCriar(Pessoa pessoa);
		bool VerificarPessoaEditar(Pessoa pessoa);
		bool VerificarPessoaFisica(Pessoa pessoa);
		bool VerificarPessoaJuridica(Pessoa pessoa);
		bool ValidarAssociarRepresentante(int pessoaId);
		bool ValidarAssociarResponsavelTecnico(int id);
		IPessoaMsg Msg { get; set; }
	}
}