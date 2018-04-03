using System;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class NotificacaoBus
	{
		#region Propriedades

		NotificacaoValidar _validar = null;
		NotificacaoDa _da = new NotificacaoDa();
		ProtocoloDa _protocoloDa = new ProtocoloDa();
		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Construtores
		public NotificacaoBus()
		{
			_validar = new NotificacaoValidar();
		}

		public NotificacaoBus(NotificacaoValidar validar)
		{
			_validar = validar;
		}
		#endregion

		#region Comandos DML

		public bool Salvar(Notificacao entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					if (entidade.Id < 1)
					{
						entidade.Id = _da.ObterID(entidade.FiscalizacaoId);
					}

					this.CopiarArquivosParaDiretorioPadrao(entidade);

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();
						this.SalvarArquivos(entidade, bancoDeDados);
						_da.Salvar(entidade, bancoDeDados);
						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		private void CopiarArquivosParaDiretorioPadrao(Notificacao entidade)
		{
			if (entidade?.Anexos == null) return;

			var _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
			foreach (var anexo in entidade.Anexos)
			{
				if (anexo.Arquivo.Id == 0)
					anexo.Arquivo = _busArquivo.Copiar(anexo.Arquivo);
			}
		}

		private void SalvarArquivos(Notificacao entidade, BancoDeDados bancoDeDados)
		{
			if (entidade?.Anexos == null) return;

			var _arquivoDa = new ArquivoDa();
			foreach (var anexo in entidade.Anexos)
			{
				if (anexo.Arquivo.Id == 0)
					_arquivoDa.Salvar(anexo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
			}
		}

		public bool Excluir(int fiscalizacaoId)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(fiscalizacaoId, bancoDeDados);

					Validacao.Add(Mensagem.FiscalizacaoConfiguracao.Excluir(fiscalizacaoId));

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public Notificacao Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			Notificacao entidade = new Notificacao();

			try
			{
				entidade = _da.Obter(fiscalizacaoId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		public int ObterId(int fiscalizacaoId, BancoDeDados banco = null)
		{
			var id = 0;

			try
			{
				id = _da.ObterID(fiscalizacaoId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return id;
		}

		#endregion

		public bool ValidarAcesso(Fiscalizacao fiscalizacao)
		{
			if (fiscalizacao.Autuante.Id != FiscalizacaoBus.User.EtramiteIdentity.FuncionarioId)
				Validacao.Add(Mensagem.NotificacaoMsg.AgenteFiscalInvalido);

			if (fiscalizacao.ProtocoloId > 0)
			{
				if (!_protocoloDa.EmPosse(fiscalizacao.ProtocoloId))
					Validacao.Add(Mensagem.Fiscalizacao.PosseProcessoNecessaria);
			}

			return Validacao.EhValido;
		}
	}
}
