using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business
{
	public class PessoaValidar : IPessoaValidar
	{
		PessoaDa _da = new PessoaDa();

		private IPessoaMsg _msg = Mensagem.Pessoa;
		public IPessoaMsg Msg
		{
			get { return _msg; }
			set { _msg = value; }
		}

		public bool VerificarPessoaFisica(Pessoa pessoa)
		{
			VerificarCriarCpf(pessoa);

			if (String.IsNullOrWhiteSpace(pessoa.Fisica.Nome))
			{
				Validacao.Add(Msg.ObrigatorioNome);
			}

			if (pessoa.Fisica.EstadoCivil <= 0)
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioEstadoCivil);
			}

			if (!String.IsNullOrEmpty(pessoa.Fisica.DataNascimentoTexto))
			{
				VerificarFisicaDataNascimento(pessoa.Fisica.DataNascimento);
			}

			if (pessoa.Fisica.ConjugeId.HasValue && pessoa.Fisica.ConjugeId > 0)
			{
				if (!_da.ExistePessoa(pessoa.Fisica.ConjugeCPF))
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeNaoExiste);
				}

				if (pessoa.Fisica.ConjugeId == pessoa.Id)
				{
					Validacao.Add(Mensagem.Pessoa.PessoaConjugeSaoIguais);
				}

				if (_da.ValidarConjugeAssociado(pessoa.Fisica.ConjugeId, pessoa.Id))
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeJaAssociado);
				}
			}

			if (pessoa.Fisica.Profissao.Id == 0 && pessoa.Id > 0)
			{
				if (_da.AssociadoRequerimentoProtocolo(pessoa.Id))
				{
					Validacao.Add(Mensagem.Pessoa.NaoPodeSalvarResponsavelTecnicoSemProfissao(pessoa.NomeRazaoSocial));
				}
			}

			return Validacao.EhValido;
		}

		public bool VerificarPessoaJuridica(Pessoa pessoa)
		{
			VerificarCriarCnpj(pessoa);

			if (String.IsNullOrWhiteSpace(pessoa.Juridica.RazaoSocial))
			{
				Validacao.Add(Msg.ObrigatorioRazaoSocial);
			}

			return Validacao.EhValido;
		}

		public bool VerificarPessoaCriar(Pessoa pessoa)
		{
			if (_da.ExistePessoa(pessoa.CPFCNPJ))
			{
				if (pessoa.IsFisica)
				{
					Validacao.Add(Msg.CpfExistente);
				}
				else
				{
					Validacao.Add(Msg.CnpjExistente);
				}
			}

			if (pessoa.IsFisica)
			{
				VerificarPessoaFisica(pessoa);
			}
			else
			{
				VerificarPessoaJuridica(pessoa);
			}
			return Validacao.EhValido;
		}

		public bool VerificarPessoaEditar(Pessoa pessoa)
		{
			if (pessoa.IsFisica)
			{
				VerificarPessoaFisica(pessoa);
			}
			else
			{
				VerificarPessoaJuridica(pessoa);
			}
			return Validacao.EhValido;
		}

		public bool Salvar(Pessoa pessoa)
		{
			if (pessoa.Id > 0)
			{
				VerificarPessoaEditar(pessoa);
			}
			else
			{
				VerificarPessoaCriar(pessoa);
			}

			VerificarEndereco(pessoa.Endereco);

			return Validacao.EhValido;
		}

		private bool VerificarEndereco(Endereco endereco)
		{
			if (!String.IsNullOrWhiteSpace(endereco.Cep) && !(new Regex("^[0-9]{2}\\.[0-9]{3}-[0-9]{3}$").IsMatch(endereco.Cep)))
			{
				Validacao.Add(Msg.EnderecoCepInvalido);
			}

			if(string.IsNullOrWhiteSpace(endereco.Logradouro))
			{
				Validacao.Add(Mensagem.Pessoa.LogradouroObrigatorio);
			}

			if (endereco.EstadoId <= 0)
			{
				Validacao.Add(Mensagem.Pessoa.EnderecoEstadoObrigatorio);
			}

			if (endereco.EstadoId > 0 && !_da.ExisteEstado(endereco.EstadoId))
			{
				Validacao.Add(Msg.EnderecoEstadoInvalido);
			}

			if (endereco.MunicipioId  <= 0)
			{
				Validacao.Add(Mensagem.Pessoa.EnderecoMunicipioObrigatorio);
			}

			if (endereco.MunicipioId > 0 && !_da.ExisteMunicipio(endereco.MunicipioId))
			{
				Validacao.Add(Msg.EnderecoMunicipioInvalido);
			}

			if (endereco.MunicipioId > 0 && endereco.EstadoId > 0 && _da.ObterMunicipio(endereco.MunicipioId).Estado.Id != endereco.EstadoId)
			{
				Validacao.Add(Msg.EnderecoMunicipioOutroEstado);
			}

			if (string.IsNullOrWhiteSpace(endereco.DistritoLocalizacao))
			{
				Validacao.Add(Mensagem.Pessoa.LocalidadeObrigatorio);
			}
	
			return Validacao.EhValido;
		}

		public bool VerificarCriarCpf(Pessoa pessoa)
		{
			if (String.IsNullOrEmpty(pessoa.Fisica.CPF))
			{
				Validacao.Add(Msg.CpfObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Cpf(pessoa.Fisica.CPF))
			{
				Validacao.Add(Msg.CpfInvalido);
			}
			return Validacao.EhValido;
		}

		public bool VerificarCriarCnpj(Pessoa pessoa)
		{
			if (String.IsNullOrEmpty(pessoa.Juridica.CNPJ))
			{
				Validacao.Add(Msg.CnpjObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Cnpj(pessoa.Juridica.CNPJ))
			{
				Validacao.Add(Msg.CnpjInvalido);
			}
			return Validacao.EhValido;
		}

		private bool VerificarFisicaDataNascimento(DateTime? data)
		{
			if (data == null)
			{
				Validacao.Add(Msg.DataNascimentoInvalida);
			}
			else if (data != null && data.Value > DateTime.Now)
			{
				Validacao.Add(Msg.DataNascimentoFuturo);
			}

			return Validacao.EhValido;
		}

		public bool VerificarExcluirPessoa(int id)
		{
			List<String> listString = new List<string>();

			//Conjuge
			if (_da.ValidarConjugeAssociado(id))
			{
				Validacao.Add(Mensagem.Pessoa.ConjugeExcluir);
			}

			//Associado como Representante
			listString = _da.ObterAssociacoesPessoa(id);
			listString.ForEach(x => Validacao.Add(Msg.ExcluirNaoPermitidoPoisRepresenta(x)));

			//Associado ao Empreendimento
			listString = _da.VerificarPessoaEmpreendimento(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoEmpreendimento(x)));
			}

			//Associado ao Processo
			listString = _da.VerificarPessoaProcesso(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoProcesso(x)));
			}

			//Associado ao Documento
			listString = _da.VerificarPessoaDocumento(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoDocumento(x)));
			}

			//Associado ao Título
			listString = _da.VerificarPessoaTitulo(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoTitulo(x)));
			}

			//Associado ao Requerimento
			listString = _da.VerificarPessoaRequerimento(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoRequerimento(x)));
			}

			//Associado a Entrega de Título
			listString = _da.VerificarEmpreendimentoTituloEntrega(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoTituloEntrega(x)));
			}

			return Validacao.EhValido;
		}

		public bool ValidarAssociarRepresentante(int pessoaId)
		{
			String profissao = _da.ObterProfissao(pessoaId);
			if (String.IsNullOrWhiteSpace(profissao))
			{
				Validacao.Add(Mensagem.Pessoa.ResponsavelTecnicoSemProfissao);
			}
			return Validacao.EhValido;
		}

		public bool ValidarAssociarResponsavelTecnico(int id)
		{
			if (!_da.ExisteProfissao(id))
			{
				Validacao.Add(Mensagem.Pessoa.ResponsavelTecnicoSemProfissao);
			}
			return Validacao.EhValido;
		}
	}
}