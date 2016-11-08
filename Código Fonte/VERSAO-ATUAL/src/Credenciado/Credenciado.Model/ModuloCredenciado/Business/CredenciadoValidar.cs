using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloOrgaoParceiroConveniado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business
{
	public class CredenciadoValidar : IPessoaInternoValidar
	{
		#region Propriedades

		String UsuarioInterno { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); } }
		GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario;
		GerenciadorConfiguracao<ConfiguracaoCredenciado> _configCredenciado;
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		GerenciadorConfiguracao<ConfiguracaoPessoa> _configPessoa;
		OrgaoParceiroConveniadoBus _busOrgaoParceiro;
		CredenciadoDa _da = new CredenciadoDa();
        PessoaCredenciadoBus _busPessoa = new PessoaCredenciadoBus();

		IPessoaMsg IPessoaInternoValidar.Msg { get; set; }

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public CredenciadoValidar()
		{
			_configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
			_configCredenciado = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
			_busOrgaoParceiro = new OrgaoParceiroConveniadoBus();
			_da = new CredenciadoDa(UsuarioCredenciado);
		}

		public bool Salvar(Pessoa pessoa)
		{
			return true;
		}

		public bool Salvar(CredenciadoPessoa credenciado, bool isPublico = false)
		{
			if (credenciado.Tipo <= 0)
			{
				Validacao.Add(Mensagem.Credenciado.TipoObrigatorio);
			}

			#region Orgao Parceiro/ Conveniado

			if (credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado)
			{
				if (!credenciado.Pessoa.IsFisica)
				{
					Validacao.Add(Mensagem.Credenciado.OrgaoParceiroPessoaTipoInvalido);

					return Validacao.EhValido;
				}

				if (credenciado.OrgaoParceiroId <= 0)
				{
					Validacao.Add(Mensagem.Credenciado.OrgaoParceiroObrigatorio);
				}

				if (credenciado.OrgaoParceiroUnidadeId <= 0)
				{
					if (_busOrgaoParceiro.ExisteUnidade(credenciado.OrgaoParceiroId))
					{
						Validacao.Add(Mensagem.Credenciado.OrgaoParceiroUnidadeObrigatoria);
					}
				}

				if (Validacao.EhValido)
				{
					OrgaoParceiroConveniado orgaoParceiro = _busOrgaoParceiro.Obter(credenciado.OrgaoParceiroId);

					if (orgaoParceiro.Id <= 0)
					{
						Validacao.Add(Mensagem.Credenciado.OrgaoParceiroInexistente);
					}
					else
					{
						if (orgaoParceiro.SituacaoId == (int)eOrgaoParceiroConveniadoSituacao.Bloqueado)
						{
							Validacao.Add(Mensagem.Credenciado.OrgaoParceiroSituacaoInvalida(orgaoParceiro.Sigla));
						}

						if (!orgaoParceiro.Unidades.Exists(x => x.Id == credenciado.OrgaoParceiroUnidadeId) && _busOrgaoParceiro.ExisteUnidade(credenciado.OrgaoParceiroId))
						{
							Validacao.Add(Mensagem.Credenciado.OrgaoParceiroUnidadeInexistente);
						}

					}
				}
			}

			#endregion

			if (isPublico)
			{
				credenciado.Pessoa.Fisica.ConjugeId = 0;
			}
			else
			{
				credenciado.Pessoa.Fisica.Conjuge = _busPessoa.ObterPessoa(credenciado.Pessoa.Fisica.ConjugeCPF);
			}

			VerificarPessoaCriar(credenciado, isPublico);

			VerificarExisteTelefone(credenciado);

			VerificarEmail(credenciado.Pessoa);

			VerificarEndereco(credenciado.Pessoa.Endereco);

			return Validacao.EhValido;
		}

		public bool VerificarPessoaEditar(Pessoa pessoa)
		{
			throw new NotImplementedException();
		}

		public bool ValidarAssociarRepresentante(int id)
		{
			return true;
		}

		public bool ValidarAssociarResponsavelTecnico(int id)
		{
			return true;
		}

		public bool VerificarChave(String chave)
		{
			if (String.IsNullOrEmpty(chave))
			{
				Validacao.Add(Mensagem.Credenciado.ChaveObrigatoria);
				return Validacao.EhValido;
			}
			else
			{
				if (_da.VerificarChaveAtiva(chave))
				{
					Validacao.Add(Mensagem.Credenciado.CredenciadoChaveJaAtiva);
				}
				else if (!_da.VerificarExisteChave(chave))
				{
					Validacao.Add(Mensagem.Credenciado.ChaveInvalida);
				}
			}

			return Validacao.EhValido;
		}

		internal bool VerificarEmail(Pessoa pessoa)
		{
			string email = (pessoa.MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.Email) ?? new Contato()).Valor;

			if (String.IsNullOrEmpty(email))
			{
				Validacao.Add(Mensagem.Credenciado.EmailObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Email(email))
			{
				Validacao.Add(Mensagem.Credenciado.EmailInvalido);
			}

			return Validacao.EhValido;
		}

		internal bool ValidarPessoaCpfCnpj(Pessoa pessoa)
		{
			if (pessoa.IsFisica)
			{
				return ValidarCpf(pessoa.CPFCNPJ);
			}
			else
			{
				return ValidarCnpj(pessoa.CPFCNPJ);
			}
		}

		internal bool ValidarCpf(string cpf)
		{
			if (String.IsNullOrWhiteSpace(cpf))
			{
				Validacao.Add(Mensagem.Credenciado.CpfObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.Credenciado.CpfInvalido);
			}
			return Validacao.EhValido;
		}

		internal bool ValidarCnpj(string cnpj)
		{
			if (String.IsNullOrWhiteSpace(cnpj))
			{
				Validacao.Add(Mensagem.Credenciado.CnpjObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Cnpj(cnpj))
			{
				Validacao.Add(Mensagem.Credenciado.CnpjInvalido);
			}
			return Validacao.EhValido;
		}

		public bool VerificarCriarCpf(Pessoa pessoa)
		{
			if (String.IsNullOrEmpty(pessoa.Fisica.CPF))
			{
				Validacao.Add(Mensagem.Credenciado.CpfObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Cpf(pessoa.Fisica.CPF))
			{
				Validacao.Add(Mensagem.Credenciado.CpfInvalido);
			}

			return Validacao.EhValido;
		}

		public bool VerificarCriarCnpj(Pessoa pessoa)
		{
			if (String.IsNullOrEmpty(pessoa.Juridica.CNPJ))
			{
				Validacao.Add(Mensagem.Credenciado.CnpjObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Cnpj(pessoa.Juridica.CNPJ))
			{
				Validacao.Add(Mensagem.Credenciado.CnpjInvalido);
			}

			return Validacao.EhValido;
		}

		public bool VerificarExcluirPessoa(int pessoa)
		{
			return Validacao.EhValido;
		}

		public bool VerificarPessoaCriar(Pessoa pessoa)
		{
			throw new NotImplementedException();
		}

		public bool VerificarPessoaCriar(CredenciadoPessoa credenciado, bool isPublico = false)
		{
			if (credenciado.Pessoa.IsFisica)
			{
				VerificarPessoaFisica(credenciado);
			}
			else
			{
				VerificarPessoaJuridica(credenciado, isPublico);
			}

			return Validacao.EhValido;
		}

		public bool VerificarPessoaFisica(Pessoa pessoa)
		{
			throw new NotImplementedException();
		}

        public bool VerificarPessoaFisica(CredenciadoPessoa credenciado, bool isConjuge = false)
		{
            VerificarCriarCpf(credenciado.Pessoa);
            Mensagem conjugeMsg = new Mensagem();
			if (String.IsNullOrWhiteSpace(credenciado.Pessoa.Fisica.Nome))
			{
                conjugeMsg = isConjuge ? Mensagem.Pessoa.ObrigatorioNomeConjuge : Mensagem.Credenciado.ObrigatorioNome;
				Validacao.Add(conjugeMsg);
			}

			if (string.IsNullOrWhiteSpace(credenciado.Pessoa.Fisica.Nacionalidade))
			{
                conjugeMsg = isConjuge ? Mensagem.Pessoa.ObrigatorioNacionalidadeConjuge : Mensagem.Pessoa.ObrigatorioNacionalidade;
                Validacao.Add(conjugeMsg);
			}

			if (string.IsNullOrWhiteSpace(credenciado.Pessoa.Fisica.Naturalidade))
			{
                conjugeMsg = isConjuge ? Mensagem.Pessoa.ObrigatorioNaturalidadeConjuge : Mensagem.Pessoa.ObrigatorioNaturalidade;
                Validacao.Add(conjugeMsg);
			}

			if (credenciado.Pessoa.Fisica.Sexo <= 0)
			{
                conjugeMsg = isConjuge ? Mensagem.Pessoa.ObrigatorioSexoConjuge : Mensagem.Pessoa.ObrigatorioSexo;
                Validacao.Add(conjugeMsg);
			}

			if (credenciado.Pessoa.Fisica.EstadoCivil <= 0)
			{
                conjugeMsg = isConjuge ? Mensagem.Pessoa.ObrigatorioEstadoCivilConjuge : Mensagem.Pessoa.ObrigatorioEstadoCivil;
                Validacao.Add(conjugeMsg);
			}

			if (credenciado.Pessoa.Fisica.DataNascimento.GetValueOrDefault() == DateTime.MinValue)
			{
                conjugeMsg = isConjuge ? Mensagem.Pessoa.DataNascimentoObrigatoriaConjuge : Mensagem.Pessoa.DataNascimentoObrigatoria;
                Validacao.Add(conjugeMsg);
			}
			else
			{
				VerificarFisicaDataNascimento(credenciado.Pessoa.Fisica.DataNascimento);
			}

			if (credenciado.Pessoa.IsFisica && credenciado.Pessoa.Fisica.EstadoCivil == 2 && String.IsNullOrEmpty(credenciado.Pessoa.Fisica.ConjugeNome) && !isConjuge)   //2 - Casado(a)
			{
				Validacao.Add(Mensagem.Pessoa.ObrigatorioConjuge);
			}

			if (credenciado.Pessoa.Fisica.Conjuge != null && !string.IsNullOrEmpty(credenciado.Pessoa.Fisica.Conjuge.CPFCNPJ) && !isConjuge)
			{
				if (credenciado.Pessoa.Fisica.Conjuge.CPFCNPJ == credenciado.Pessoa.CPFCNPJ)
				{
					Validacao.Add(Mensagem.Pessoa.PessoaConjugeSaoIguais);
				}

				if (credenciado.Id > 0)
				{
					if (_da.ValidarConjugeAssociado(credenciado.Pessoa.CPFCNPJ, credenciado.Pessoa.Fisica.Conjuge.CPFCNPJ, credenciado.Id))
					{
						Validacao.Add(Mensagem.Pessoa.ConjugeJaAssociado);
					}
				}
			}

			if (credenciado.Tipo == (int)eCredenciadoTipo.ResponsavelTecnico)
			{
				if (credenciado.Pessoa.Fisica.Profissao == null || credenciado.Pessoa.Fisica.Profissao.Id == 0)
				{
					Validacao.Add(Mensagem.Credenciado.ObrigatorioProfissao);
				}

				if (string.IsNullOrEmpty(credenciado.Pessoa.Fisica.Profissao.Registro))
				{
					Validacao.Add(Mensagem.Credenciado.RegistroObrigatorio);
				}
			}

			if (String.IsNullOrWhiteSpace(credenciado.Pessoa.Fisica.NomeMae) && !isConjuge)
			{
                Validacao.Add(Mensagem.Credenciado.ObrigatorioMae);
			}

            if (String.IsNullOrWhiteSpace(credenciado.Pessoa.Fisica.NomePai) && !isConjuge)
			{
				Validacao.Add(Mensagem.Credenciado.ObrigatorioPai);
			}

            if (credenciado.Pessoa.Fisica.ConjugeId > 0 && !isConjuge)
            {
                VerificarPessoaFisica(new CredenciadoPessoa() { Pessoa = credenciado.Pessoa.Fisica.Conjuge }, true);
            }
			return Validacao.EhValido;
		}

		private bool VerificarFisicaDataNascimento(DateTime? data)
		{
			if (data == null)
			{
				Validacao.Add(Mensagem.Pessoa.DataNascimentoInvalida);
			}
			else if (data != null && data.Value > DateTime.Now)
			{
				Validacao.Add(Mensagem.Pessoa.DataNascimentoFuturo);
			}

			return Validacao.EhValido;
		}

		public bool VerificarPessoaJuridica(Pessoa pessoa)
		{
			throw new NotImplementedException();
		}

		public bool VerificarPessoaJuridica(CredenciadoPessoa credenciado, bool isPublico = false)
		{
			VerificarCriarCnpj(credenciado.Pessoa);

			if (String.IsNullOrWhiteSpace(credenciado.Pessoa.Juridica.RazaoSocial))
			{
				Validacao.Add(Mensagem.Credenciado.ObrigatorioRazaoSocial);
			}

			if (String.IsNullOrWhiteSpace(credenciado.Pessoa.Juridica.NomeFantasia))
			{
				Validacao.Add(Mensagem.Credenciado.ObrigatorioNomeFantasia);
			}

			#region Representantes

			if (credenciado.Pessoa.Juridica.Representantes.Count <= 0)
			{
				Validacao.Add(Mensagem.Credenciado.RepresentanteObrigatorio);
			}

			List<Mensagem> mensagens = new List<Mensagem>(Validacao.Erros);
			Validacao.Erros.Clear();
			PessoaCredenciadoValidar pessoaValidar = new PessoaCredenciadoValidar();

			credenciado.Pessoa.Juridica.Representantes.ForEach(representante =>
			{
				bool isValido = true;

				/*Pessoa*/
				if (!pessoaValidar.Salvar(representante))
				{
					isValido = false;
				}
                
                /*Conjuge*/
                if (!String.IsNullOrWhiteSpace(representante.Fisica.ConjugeCPF) && !isPublico)
                {
                    representante.Fisica.Conjuge = _busPessoa.ObterPessoa(representante.Fisica.ConjugeCPF);
                }
                if (!String.IsNullOrWhiteSpace(representante.Fisica.ConjugeCPF) && representante.Fisica.Conjuge != null && isValido)
				{
					if (!pessoaValidar.Salvar(representante.Fisica.Conjuge, true))
					{
						isValido = false;
					}
				}

				if (!isValido)
				{
					mensagens.Add(Mensagem.Pessoa.DadosRepresentanteIncompleto(representante.NomeRazaoSocial));
				}

				Validacao.Erros.Clear();
			});

			Validacao.Erros = mensagens;

			#endregion

			return Validacao.EhValido;
		}

		private bool VerificarEndereco(Endereco endereco)
		{
			if (String.IsNullOrWhiteSpace(endereco.Cep))
			{
				Validacao.Add(Mensagem.Credenciado.EnderecoCepObrigatorio);
			}
			else if (!(new Regex("^[0-9]{2}\\.[0-9]{3}-[0-9]{3}$").IsMatch(endereco.Cep)))
			{
				Validacao.Add(Mensagem.Credenciado.EnderecoCepInvalido);
			}
			if (String.IsNullOrWhiteSpace(endereco.Logradouro))
			{
				Validacao.Add(Mensagem.Credenciado.EnderecoLogradouroObrigatorio);
			}
			if (String.IsNullOrWhiteSpace(endereco.Bairro))
			{
				Validacao.Add(Mensagem.Credenciado.EnderecoBairroObrigatorio);
			}

			if (endereco.EstadoId == 0)
			{
				Validacao.Add(Mensagem.Credenciado.EnderecoEstadoObrigatorio);
			}

			if (endereco.MunicipioId == 0)
			{
				Validacao.Add(Mensagem.Credenciado.EnderecoMunicipioObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(endereco.Numero))
			{
				Validacao.Add(Mensagem.Credenciado.EnderecoNumeroObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(endereco.DistritoLocalizacao))
			{
				Validacao.Add(Mensagem.Pessoa.LocalidadeObrigatorio);
			}

			return Validacao.EhValido;
		}

		private bool VerificarProfissao(Profissao profissao)
		{
			return Validacao.EhValido;
		}

		private bool VerificarExisteTelefone(CredenciadoPessoa credenciado)
		{
			if (credenciado.Pessoa.MeiosContatos.FindAll(x => (
				x.TipoContato == eTipoContato.TelefoneCelular
				|| x.TipoContato == eTipoContato.TelefoneComercial
				|| x.TipoContato == eTipoContato.TelefoneFax
				|| x.TipoContato == eTipoContato.TelefoneResidencial)).Count < 1)
			{
				Validacao.Add(Mensagem.Credenciado.ObrigatorioQualquerTelefone);
			}
			return Validacao.EhValido;
		}

		public bool AlterarSenha(string login, string senha, string novaSenha, string confirmarNovaSenha)
		{
			if (String.IsNullOrEmpty(login))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioLogin);
			}

			if (String.IsNullOrEmpty(senha))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioSenha);
			}

			if (String.IsNullOrEmpty(novaSenha))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioNovaSenha);
			}

			if (!String.IsNullOrEmpty(novaSenha) && novaSenha.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha))
			{
				Validacao.Add(Mensagem.Credenciado.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
			}

			if (String.IsNullOrEmpty(confirmarNovaSenha))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioConfirmarNovaSenha);
			}

			if (!String.IsNullOrEmpty(novaSenha) && !novaSenha.Equals(confirmarNovaSenha))
			{
				Validacao.Add(Mensagem.AlterarSenha.NovaSenhaDiferente);
			}

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(int id, int novaSituacao, string motivo)
		{
			if (novaSituacao == (int)eCredenciadoSituacao.Bloqueado && String.IsNullOrEmpty(motivo))
			{
				Validacao.Add(Mensagem.Credenciado.SituacaoMotivoObrigatorio(eCredenciadoSituacao.Bloqueado.ToString()));
			}

			return Validacao.EhValido;
		}

		internal bool RegerarChave(Pessoa pessoa)
		{
			string email = (pessoa.MeiosContatos.SingleOrDefault(x => x.TipoContato == eTipoContato.Email) ?? new Contato()).Valor;

			if (String.IsNullOrEmpty(email))
			{
				Validacao.Add(Mensagem.Credenciado.EmailObrigatorioRegerar);
			}
			else if (!ValidacoesGenericasBus.Email(email))
			{
				Validacao.Add(Mensagem.Credenciado.EmailInvalidoRegerar);
			}

			return Validacao.EhValido;
		}

		public bool ValidarAtivar(CredenciadoPessoa credenciado, string senha, string confirmarSenha)
		{
			if (_da.VerificarChaveAtiva(credenciado.Chave))
			{
				Validacao.Add(Mensagem.Credenciado.CredenciadoChaveJaAtiva);
			}
			else
			{
				if (!_da.VerificarExisteChave(credenciado.Chave))
				{
					Validacao.Add(Mensagem.Credenciado.ChaveInvalida);
				}

				if (String.IsNullOrEmpty(credenciado.Usuario.Login))
				{
					Validacao.Add(Mensagem.Credenciado.LoginObrigatorio);
				}
				else
				{
					UsuarioBus busUsuario = new UsuarioBus(HistoricoAplicacao.CREDENCIADO, UsuarioCredenciado);

					if (credenciado.Usuario.Id <= 0 && busUsuario.VerificarLoginExistente(credenciado.Usuario.Login, credenciado.Usuario.Id))
					{
						Validacao.Add(Mensagem.Credenciado.LoginExistente);
					}

					if (credenciado.Usuario.Login.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoMinLogin))
					{
						Validacao.Add(Mensagem.Credenciado.LoginTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoMinLogin)));
					}

					if (!UsuarioValidacao.FormatoLogin(credenciado.Usuario.Login))
					{
						Validacao.Add(Mensagem.Credenciado.FormatoLogin);
					}
				}

				if (String.IsNullOrWhiteSpace(senha))
				{
					Validacao.Add(Mensagem.Credenciado.SenhaObrigatorio);
				}

				if (String.IsNullOrWhiteSpace(confirmarSenha))
				{
					Validacao.Add(Mensagem.Credenciado.ConfirmarSenhaObrigatorio);
				}

				if (!String.IsNullOrEmpty(senha) && senha.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha))
				{
					Validacao.Add(Mensagem.Credenciado.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
				}

				if (!String.IsNullOrEmpty(senha) && !senha.Equals(confirmarSenha))
				{
					Validacao.Add(Mensagem.Credenciado.SenhaDiferente);
				}
			}

			return Validacao.EhValido;
		}

		public bool ValidarReativar(string chave, string senha, string confirmarSenha)
		{
			if (!_da.VerificarExisteChave(chave))
			{
				Validacao.Add(Mensagem.Credenciado.ChaveInvalida);
			}

			if (String.IsNullOrWhiteSpace(senha))
			{
				Validacao.Add(Mensagem.Credenciado.SenhaObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(confirmarSenha))
			{
				Validacao.Add(Mensagem.Credenciado.ConfirmarSenhaObrigatorio);
			}

			if (!String.IsNullOrEmpty(senha) && senha.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha))
			{
				Validacao.Add(Mensagem.Credenciado.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
			}

			if (!String.IsNullOrEmpty(senha) && !senha.Equals(confirmarSenha))
			{
				Validacao.Add(Mensagem.Credenciado.SenhaDiferente);
			}

			return Validacao.EhValido;
		}

		public bool VerificarAlterarDados(CredenciadoPessoa credenciado, String senha, String confirmarSenha)
		{
			// senha
			if (credenciado.AlterarSenha)
			{
				if (String.IsNullOrWhiteSpace(senha))
				{
					Validacao.Add(Mensagem.Credenciado.SenhaObrigatorio);
				}

				if (String.IsNullOrWhiteSpace(confirmarSenha))
				{
					Validacao.Add(Mensagem.Credenciado.ConfirmarSenhaObrigatorio);
				}

				if (!String.IsNullOrEmpty(senha) && senha.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha))
				{
					Validacao.Add(Mensagem.Credenciado.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
				}

				if (!String.IsNullOrEmpty(senha) && !senha.Equals(confirmarSenha))
				{
					Validacao.Add(Mensagem.Credenciado.SenhaDiferente);
				}
			}

			// validar credenciado
			Salvar(credenciado);

			return Validacao.EhValido;
		}
	}
}