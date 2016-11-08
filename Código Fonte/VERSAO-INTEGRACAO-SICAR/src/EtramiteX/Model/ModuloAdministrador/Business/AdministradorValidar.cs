using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAdministrador.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAdministrador.Business
{
	public class AdministradorValidar
	{
		GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
		GerenciadorConfiguracao<ConfiguracaoAdministrador> _configAdministrador = new GerenciadorConfiguracao<ConfiguracaoAdministrador>(new ConfiguracaoAdministrador());
		AdministradorDa _da = new AdministradorDa();
		UsuarioBus _usuarioBus = new UsuarioBus(HistoricoAplicacao.INTERNO );

		private List<Situacao> Situacoes
		{
			get { return _da.ObterSituacaoAdministrador(); }
		}

		public bool Salvar(Administrador obj, String senha, String ConfirmarSenha, bool AlterarSenha)
		{
			if (String.IsNullOrEmpty(obj.Nome))
			{
				Validacao.Add(Mensagem.Administrador.NomeObrigatorio);
			}

			if (String.IsNullOrEmpty(obj.Email))
			{
				Validacao.Add(Mensagem.Administrador.EmailObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Email(obj.Email))
			{
				Validacao.Add(Mensagem.Administrador.EmailInvalido);
			}

			Cpf(obj.Cpf);

			//Valores De configuracao com cache
			//_config.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)
			//Valores Atuais            
			//_config.Atual.TamanhoSenha

			if (obj.Usuario.Id <= 0 || !String.IsNullOrWhiteSpace(senha) || !String.IsNullOrWhiteSpace(ConfirmarSenha) || AlterarSenha)
			{
				if (String.IsNullOrWhiteSpace(senha))
				{
					Validacao.Add(Mensagem.Administrador.SenhaObrigatorio);
				}

				if (String.IsNullOrWhiteSpace(ConfirmarSenha))
				{
					Validacao.Add(Mensagem.Administrador.ConfirmarSenhaObrigatorio);
				}

				if (!String.IsNullOrEmpty(senha) && senha.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha))
				{
					Validacao.Add(Mensagem.Administrador.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
				}

				if (!String.IsNullOrEmpty(senha) && !senha.Equals(ConfirmarSenha))
				{
					Validacao.Add(Mensagem.Administrador.SenhaDiferente);
				}
			}

			if (String.IsNullOrEmpty(obj.Usuario.Login))
			{
				Validacao.Add(Mensagem.Administrador.LoginObrigatorio);
			}
			else
			{
				if (obj.Usuario.Login.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoMinLogin))
				{
					Validacao.Add(Mensagem.Administrador.LoginTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoMinLogin)));
				}

				if (!UsuarioValidacao.FormatoLogin(obj.Usuario.Login))
				{
					Validacao.Add(Mensagem.Administrador.FormatoLogin);
				}

				if (_usuarioBus.VerificarLoginExistente(obj.Usuario.Login, obj.Usuario.Id))
				{
					Validacao.Add(Mensagem.Administrador.LoginExistente);
				}
			}

			return Validacao.EhValido;
		}

		public bool Cpf(String cpf)
		{
			if (String.IsNullOrEmpty(cpf))
			{
				Validacao.Add(Mensagem.Administrador.CpfObrigatorio);
				return Validacao.EhValido;
			}

			if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.Administrador.CpfInvalido);
			}

			return Validacao.EhValido;
		}

		public bool Autenticar(Administrador funcionario)
		{
			// 2-Bloqueado / 4-Ausente
			if (funcionario.Situacao == 2 || funcionario.Situacao == 4)
			{
				Validacao.Add(Mensagem.Login.SituacaoInvalida(Situacoes.Single(x => x.Id == funcionario.Situacao).Nome));
				return false;
			}

			//Acesso não permitido nestes horários e/ou dia! Entre em contato com o administrador do sistema
			//Caso a quantidade de dias da ultima alteração de senha feita for maior que a quantidade definida no módulo de configuração,

			return Validacao.EhValido;
		}

		public bool AlterarSenha(string login, string senha, string novaSenha, string confirmarNovaSenha)
		{
			if (String.IsNullOrEmpty(login))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioLogin);
			}

			if (String.IsNullOrEmpty(login))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioSenha);
			}

			if (String.IsNullOrEmpty(novaSenha))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioNovaSenha);
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

		public bool AlterarSenhaAdministrador(string login, string novaSenha, string confirmarNovaSenha)
		{
			if (String.IsNullOrEmpty(login))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioLogin);
			}

			if (String.IsNullOrEmpty(login))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioSenha);
			}

			if (String.IsNullOrEmpty(novaSenha))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioNovaSenha);
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

		public bool AlterarSituacao(int Situacao, string Motivo)
		{
			if (Situacao == 0)
			{
				Validacao.Add(Mensagem.Administrador.NovaSitucaoObrigatoria);
			}

			if (Situacao == 4 && String.IsNullOrEmpty(Motivo))//Ausente
			{
				Validacao.Add(Mensagem.Administrador.MotivoAusente);
			}

			return Validacao.EhValido;
		}

		internal bool VerificarAlterarAdministrador(int id)
		{
			if ((HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId != id)
			{
				Validacao.Add(Mensagem.Administrador.AlterarAdministradorOutro);
			}

			return Validacao.EhValido;
		}

		internal bool ValidarTransferirSistema(int id)
		{
			Administrador admin = _da.Obter(id);
			if (admin == null)
			{
				Validacao.Add(Mensagem.Administrador.TransferirSistemaNaoEhAdmin);
			}
			else if (admin.IsSistema)
			{
				Validacao.Add(Mensagem.Administrador.TransferirSistemaJaEhSistema);
			}
			return Validacao.EhValido;
		}

		internal bool ValidarTransferirSistema(int id, string motivo)
		{
			ValidarTransferirSistema(id);
			if (String.IsNullOrWhiteSpace(motivo))
			{
				Validacao.Add(Mensagem.Administrador.TransferirSistemaMotivoObrigatorio);
			}
			return Validacao.EhValido;
		}
	}
}
