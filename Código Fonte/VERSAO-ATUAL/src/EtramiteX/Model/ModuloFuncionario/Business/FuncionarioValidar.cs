using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business
{
	public class FuncionarioValidar : IFuncionarioValidar
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
		FuncionarioDa _da = new FuncionarioDa();
		UsuarioBus _usuarioBus = new UsuarioBus(HistoricoAplicacao.INTERNO);
		public EtramiteIdentity UsuarioLogado { get { return (HttpContext.Current.User.Identity as EtramiteIdentity); } }

		private List<Situacao> Situacoes
		{
			get { return _da.ObterSituacaoFuncionario(); }
		}

		#endregion

		public bool VerificarAlterarFuncionario(int id)
		{
			if (UsuarioLogado.FuncionarioId != id)
			{
				Validacao.Add(Mensagem.Funcionario.AlterarFuncionarioOutro);
			}

			return Validacao.EhValido;
		}

		public bool Salvar(Funcionario funcionario, String senha, String ConfirmarSenha)
		{
			if (String.IsNullOrEmpty(funcionario.Nome))
			{
				Validacao.Add(Mensagem.Funcionario.NomeObrigatorio);
			}

			//if (!string.IsNullOrWhiteSpace(funcionario.Email) && !ValidacoesGenericasBus.Email(funcionario.Email))
			//{
			//    Validacao.Add(Mensagem.Funcionario.EmailInvalido);
			//}

			Cpf(funcionario.Cpf);

			if (funcionario.Cargos == null || funcionario.Cargos.Count == 0)
			{
				Validacao.Add(Mensagem.Funcionario.CargoObrigatorio);
			}

			if (funcionario.Setores == null || funcionario.Setores.Count == 0)
			{
				Validacao.Add(Mensagem.Funcionario.SetoresObrigatorio);
			}

			if (funcionario.Id > 0)//Em edição
			{
				//Setores com posse
				List<Setor> setoresEmPosse = _da.ObterSetoresProtocolosEmPosse(funcionario.Id);

				foreach (var item in setoresEmPosse)
				{
					if (!funcionario.Setores.Exists(x => x.Id == item.Id))
					{
						Validacao.Add(Mensagem.Funcionario.SetorComPosse(item.Nome));
					}
				}

				//Setores onde funcionario é executor
				List<Setor> setoresRegistrador = _da.ObterSetoresRegistrador(funcionario.Id);

				foreach (var item in setoresRegistrador)
				{
					if (!funcionario.Setores.Exists(x => x.Id == item.Id))
					{
						Validacao.Add(Mensagem.Funcionario.SetorComRegistrador(item.Nome));
					}
				}
			}

			if (String.IsNullOrEmpty(funcionario.Usuario.Login))
			{
				Validacao.Add(Mensagem.Funcionario.LoginObrigatorio);
			}
			else
			{
				if (funcionario.Usuario.Login.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoMinLogin))
				{
					Validacao.Add(Mensagem.Funcionario.LoginTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoMinLogin)));
				}

				if (!UsuarioValidacao.FormatoLogin(funcionario.Usuario.Login))
				{
					Validacao.Add(Mensagem.Funcionario.FormatoLogin);
				}

				if (_usuarioBus.VerificarLoginExistente(funcionario.Usuario.Login, funcionario.Usuario.Id))
				{
					Validacao.Add(Mensagem.Funcionario.LoginExistente);
				}
			}

			if(funcionario.AlterarSenha || (funcionario.Usuario.Id <= 0 ))
			{
				if (String.IsNullOrWhiteSpace(senha))
				{
					Validacao.Add(Mensagem.Funcionario.SenhaObrigatorio);
				}

				if (String.IsNullOrWhiteSpace(ConfirmarSenha))
				{
					Validacao.Add(Mensagem.Funcionario.ConfirmarSenhaObrigatorio);
				}
			}

			if (funcionario.AlterarSenha || (funcionario.Usuario.Id <= 0 && (!String.IsNullOrWhiteSpace(senha) || !String.IsNullOrWhiteSpace(ConfirmarSenha))))
			{
				if (!String.IsNullOrEmpty(senha) && senha.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha))
				{
					Validacao.Add(Mensagem.Funcionario.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
				}

				if (!String.IsNullOrEmpty(senha) && !senha.Equals(ConfirmarSenha))
				{
					Validacao.Add(Mensagem.Funcionario.SenhaDiferente);
				}
			}

			List<String> lstSetores = _da.VerificarResponsavelSetor(funcionario);
			lstSetores.ForEach(x => Validacao.Add(Mensagem.Funcionario.SetorComResponsavel(x)));

			return Validacao.EhValido;
		}

		public bool Cpf(String cpf)
		{
			if (String.IsNullOrEmpty(cpf))
			{
				Validacao.Add(Mensagem.Funcionario.CpfObrigatorio);
				return Validacao.EhValido;
			}

			if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.Funcionario.CpfInvalido);
			}

			return Validacao.EhValido;
		}

		public bool Autenticar(Funcionario funcionario)
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
				Validacao.Add(Mensagem.Funcionario.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
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

		public bool AlterarSenhaFuncionario(string login, string novaSenha, string confirmarNovaSenha)
		{
			if (String.IsNullOrEmpty(login))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioLogin);
			}

			if (String.IsNullOrEmpty(novaSenha))
			{
				Validacao.Add(Mensagem.AlterarSenha.ObrigatorioNovaSenha);
			}

			if (!String.IsNullOrEmpty(novaSenha) && novaSenha.Length < _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha))
			{
				Validacao.Add(Mensagem.Funcionario.SenhaTamanho(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyTamanhoSenha)));
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
				Validacao.Add(Mensagem.Funcionario.NovaSitucaoObrigatoria);
			}

			if (Situacao == 4 && String.IsNullOrEmpty(Motivo))//Ausente
			{
				Validacao.Add(Mensagem.Funcionario.MotivoAusente);
			}

			return Validacao.EhValido;
		}
	}
}