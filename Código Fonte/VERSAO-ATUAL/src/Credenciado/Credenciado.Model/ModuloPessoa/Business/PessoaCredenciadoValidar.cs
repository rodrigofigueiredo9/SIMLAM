using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business
{
	public class PessoaCredenciadoValidar : IPessoaCredenciadoValidar
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		PessoaCredenciadoDa _da;
		public BancoDeDados BancoValidar = null;

		private IPessoaMsg _msg = Mensagem.Pessoa;
		public IPessoaMsg Msg
		{
			get { return _msg; }
			set { _msg = value; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		public PessoaCredenciadoValidar()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new PessoaCredenciadoDa(UsuarioCredenciado);
		}

		public bool VerificarPessoaFisica(Pessoa pessoa, bool isConjuge = false)
		{
			VerificarCriarCpf(pessoa, isConjuge);

			if (String.IsNullOrWhiteSpace(pessoa.Fisica.Nome))
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioNomeMsg(isConjuge));
			}

			if (string.IsNullOrWhiteSpace(pessoa.Fisica.Nacionalidade))
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioNacionalidadeMsg(isConjuge));
			}
			
			if (string.IsNullOrWhiteSpace(pessoa.Fisica.Naturalidade))
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioNaturalidadeMsg(isConjuge));
			}

			if (!pessoa.Fisica.Sexo.HasValue && isConjuge)
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioSexoMsg(isConjuge));
			}
			else
			{
				if (pessoa.Fisica.Sexo <= 0)
				{
					Validacao.Add(Mensagem.Pessoa.ObrigatorioSexoMsg(isConjuge));
				}
			}

			if (pessoa.Fisica.EstadoCivil <= 0)
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioEstadoCivilMsg(isConjuge));
			}

			if (pessoa.Fisica.DataNascimento.GetValueOrDefault() == DateTime.MinValue)
			{
				Validacao.Add(Mensagem.Pessoa.DataNascimentoObrigatoriaMsg(isConjuge));
			}
			else
			{
				VerificarFisicaDataNascimento(pessoa.Fisica.DataNascimento, isConjuge);
			}

			if (pessoa.Fisica.ConjugeId.GetValueOrDefault() > 0)
			{
				if (_da.ExistePessoa(pessoa.Fisica.ConjugeCPF, pessoa.CredenciadoId.GetValueOrDefault(), BancoValidar) <= 0)
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeNaoExiste);
				}

				if (pessoa.Fisica.ConjugeId == pessoa.Id)
				{
					Validacao.Add(Mensagem.Pessoa.PessoaConjugeSaoIguais);
				}

				if (_da.ValidarConjugeAssociado(pessoa.CPFCNPJ, pessoa.Fisica.ConjugeCPF, User.EtramiteIdentity.FuncionarioId))
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeJaAssociado);
				}
			}
			else if (pessoa.Fisica.Conjuge != null && !string.IsNullOrEmpty(pessoa.Fisica.Conjuge.CPFCNPJ))
			{
				if (pessoa.Fisica.Conjuge.CPFCNPJ == pessoa.CPFCNPJ)
				{
					Validacao.Add(Mensagem.Pessoa.PessoaConjugeSaoIguais);
				}

				if (_da.ValidarConjugeAssociado(pessoa.CPFCNPJ, pessoa.Fisica.Conjuge.CPFCNPJ, pessoa.CredenciadoId.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeJaAssociado);
				}
			}

            if (!pessoa.IsCopiado && pessoa.Fisica.ConjugeId.GetValueOrDefault() > 0 && !isConjuge && User != null) 
            {
                pessoa.Fisica.Conjuge = _da.Obter(pessoa.Fisica.ConjugeId.GetValueOrDefault(), null, simplificado:false);
                VerificarPessoaFisica(pessoa.Fisica.Conjuge, true);
            }


			return Validacao.EhValido;
		}

		public bool ValidarRepresentante(Pessoa representante, bool isConjuge = false) 
		{
			VerificarCriarCpf(representante, isConjuge);

			if (String.IsNullOrWhiteSpace(representante.Fisica.Nome))
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioNomeMsg(isConjuge));
			}

			if (string.IsNullOrWhiteSpace(representante.Fisica.Nacionalidade))
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioNacionalidadeMsg(isConjuge));
			}

			if (string.IsNullOrWhiteSpace(representante.Fisica.Naturalidade))
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioNaturalidadeMsg(isConjuge));
			}

			if (!representante.Fisica.Sexo.HasValue && isConjuge)
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioSexoMsg(isConjuge));
			}
			else
			{
				if (representante.Fisica.Sexo <= 0)
				{
					Validacao.Add(Mensagem.Pessoa.ObrigatorioSexoMsg(isConjuge));
				}
			}

			if (representante.Fisica.EstadoCivil <= 0)
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioEstadoCivilMsg(isConjuge));
			}

			if (representante.Fisica.DataNascimento.GetValueOrDefault() == DateTime.MinValue)
			{
				Validacao.Add(Mensagem.Pessoa.DataNascimentoObrigatoriaMsg(isConjuge));
			}
			else
			{
				VerificarFisicaDataNascimento(representante.Fisica.DataNascimento, isConjuge);
			}

			if (representante.Fisica.ConjugeId.GetValueOrDefault() > 0)
			{
				if (_da.ExistePessoa(representante.Fisica.ConjugeCPF, representante.CredenciadoId.GetValueOrDefault()) <= 0)
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeNaoExiste);
				}

				if (representante.Fisica.ConjugeId == representante.Id)
				{
					Validacao.Add(Mensagem.Pessoa.PessoaConjugeSaoIguais);
				}

				if (_da.ValidarConjugeAssociado(representante.CPFCNPJ, representante.Fisica.ConjugeCPF, User.EtramiteIdentity.FuncionarioId))
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeJaAssociado);
				}
			}
			else if (representante.Fisica.Conjuge != null && !string.IsNullOrEmpty(representante.Fisica.Conjuge.CPFCNPJ))
			{
				if (representante.Fisica.Conjuge.CPFCNPJ == representante.CPFCNPJ)
				{
					Validacao.Add(Mensagem.Pessoa.PessoaConjugeSaoIguais);
				}

				if (_da.ValidarConjugeAssociado(representante.CPFCNPJ, representante.Fisica.Conjuge.CPFCNPJ, representante.CredenciadoId.GetValueOrDefault()))
				{
					Validacao.Add(Mensagem.Pessoa.ConjugeJaAssociado);
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

			List<Pessoa> representantes = pessoa.Juridica.Representantes;

			List<Mensagem> mensagens = new List<Mensagem>(Validacao.Erros);
			Validacao.Erros.Clear();

			if (representantes != null && representantes.Count > 0)
			{
				representantes.ForEach(representante =>
				{
					if (representante.IsCopiado && !Salvar(representante))
					{
						mensagens.Add(Mensagem.Pessoa.DadosRepresentanteIncompleto(representante.NomeRazaoSocial));
					}
				});
			}

			Validacao.Erros = mensagens;

			return Validacao.EhValido;
		}

		public bool VerificarPessoaCriar(Pessoa pessoa, bool isConjuge = false)
		{
			if (_da.ExistePessoa(pessoa.CPFCNPJ, pessoa.CredenciadoId.GetValueOrDefault()) > 0)
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
				VerificarPessoaFisica(pessoa, isConjuge);
			}
			else
			{
				VerificarPessoaJuridica(pessoa);
			}

			return Validacao.EhValido;
		}

		public bool VerificarPessoaEditar(Pessoa pessoa, bool isConjuge = false)
		{
			if (pessoa.IsFisica)
			{
				VerificarPessoaFisica(pessoa, isConjuge);
			}
			else
			{
				VerificarPessoaJuridica(pessoa);
			}

			return Validacao.EhValido;
		}

		public bool Salvar(Pessoa pessoa, bool isConjuge = false)
		{
			if (pessoa.Id > 0)
			{
				VerificarPessoaEditar(pessoa, isConjuge);
			}
			else
			{
				VerificarPessoaCriar(pessoa, isConjuge);
			}

			VerificarEndereco(pessoa.Endereco, isConjuge);

			return Validacao.EhValido;
		}

		private bool VerificarEndereco(Endereco endereco, bool isConjuge = false)
		{
			if (!String.IsNullOrWhiteSpace(endereco.Cep) && !(new Regex("^[0-9]{2}\\.[0-9]{3}-[0-9]{3}$").IsMatch(endereco.Cep)))
			{
				Validacao.Add(Msg.EnderecoCepInvalidoMsg(isConjuge));
			}

			if (string.IsNullOrWhiteSpace(endereco.Logradouro))
			{
				Validacao.Add(Mensagem.Pessoa.LogradouroObrigatorioMsg(isConjuge));
			}

			if (string.IsNullOrWhiteSpace(endereco.Bairro))
			{
				Validacao.Add(Mensagem.Pessoa.BairroObrigatorioMsg(isConjuge));
			}

			if (endereco.EstadoId <= 0)
			{
				Validacao.Add(Mensagem.Pessoa.EnderecoEstadoObrigatorioMsg(isConjuge));
			}

			bool existeEstado = ListaCredenciadoBus.Estados.Exists(x => x.Id == endereco.EstadoId);

			if (endereco.EstadoId > 0 && !existeEstado)
			{
				Validacao.Add(Msg.EnderecoEstadoInvalidoMsg(isConjuge));
			}

			if (endereco.MunicipioId <= 0)
			{
				Validacao.Add(Mensagem.Pessoa.EnderecoMunicipioObrigatorioMsg(isConjuge));
			}

			bool existeMunicipio = ListaCredenciadoBus.Municipios(endereco.EstadoId).Exists(x => x.Id == endereco.MunicipioId);

			if (endereco.MunicipioId > 0 && !existeMunicipio)
			{
				Validacao.Add(Msg.EnderecoMunicipioInvalidoMsg(isConjuge));
			}

			if (endereco.MunicipioId > 0 && endereco.EstadoId > 0 && !existeMunicipio)
			{
				Validacao.Add(Msg.EnderecoMunicipioOutroEstadoMsg(isConjuge));
			}

			if (string.IsNullOrWhiteSpace(endereco.DistritoLocalizacao))
			{
				Validacao.Add(Mensagem.Pessoa.LocalidadeObrigatorioMsg(isConjuge));
			}

			return Validacao.EhValido;
		}

		public bool VerificarCriarCpf(Pessoa pessoa, bool isConjuge = false)
		{
			if (String.IsNullOrEmpty(pessoa.Fisica.CPF))
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioCPFMsg(isConjuge));
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

		private bool VerificarFisicaDataNascimento(DateTime? data, bool isConjuge = false)
		{
			if (data == null)
			{
				Validacao.Add(Msg.DataNascimentoInvalidaMsg(isConjuge));
			}
			else if (data != null && data.Value > DateTime.Now)
			{
				Validacao.Add(Msg.DataNascimentoFuturoMsg(isConjuge));
			}

			return Validacao.EhValido;
		}

		public bool VerificarExcluirPessoa(int id)
		{
			CredenciadoBus credenciadoBus = new CredenciadoBus();
			CredenciadoPessoa credenciado = credenciadoBus.Obter(User.EtramiteIdentity.FuncionarioId, true);

			if (credenciado.Pessoa.Id == id)
			{
				Validacao.Add(Mensagem.Credenciado.ExcluirPessoaCredenciado);
				return false;
			}

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

			//Associado ao Requerimento
			listString = _da.VerificarPessoaRequerimento(id);

			if (listString != null && listString.Count > 0)
			{
				listString.ForEach(x => Validacao.Add(Msg.AssociadoRequerimento(x)));
			}

			return Validacao.EhValido;
		}

		public bool ValidarAssociarRepresentante(int pessoaId)
		{
			string profissao = _da.ObterProfissaoTexto(pessoaId);

			if (string.IsNullOrEmpty(profissao))
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

		public bool EmPosseCredenciado(Pessoa pessoa)
		{
			if (pessoa == null) return true;

			if (!pessoa.CredenciadoId.HasValue)
			{
				CredenciadoBus credenciadoBus = new CredenciadoBus();
				CredenciadoPessoa credenciado = credenciadoBus.Obter(User.EtramiteIdentity.FuncionarioId);
				return pessoa.Id == credenciado.Pessoa.Id;
			}

			return pessoa.CredenciadoId == User.EtramiteIdentity.FuncionarioId;
		}
	}
}