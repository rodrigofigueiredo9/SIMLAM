using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business
{
	public class TramitacaoBus
	{
		#region Propriedades

		TramitacaoDa _da;
		TramitacaoValidar _validar;
		FuncionarioBus _busFuncionario;

		public EtramiteIdentity User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		#endregion

		public TramitacaoBus()
		{
			_da = new TramitacaoDa();
			_validar = new TramitacaoValidar();
			_busFuncionario = new FuncionarioBus();
		}

		public TramitacaoBus(TramitacaoValidar validacao)
		{
			_validar = validacao;
			_da = new TramitacaoDa();
			_validar = new TramitacaoValidar();
			_busFuncionario = new FuncionarioBus();
		}

		#region Ações

		public void Cancelar(Tramitacao tramitacao, bool permissaoTramitar)
		{
			tramitacao = Obter(tramitacao.Id);
			if (_validar.Cancelar(tramitacao, permissaoTramitar))
			{
				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();
					_da.Cancelar(tramitacao);
				}
			}
		}

		public bool ValidarRedirecionamentoEnviar(IProtocolo protocolo, bool permissaoTramitar)
		{
			bool registro = false;

			_validar.RegraSetor(protocolo.SetorId, permissaoTramitar);

			if (_da.ObterTipoSetor(protocolo.SetorId) == (int)eTramitacaoTipo.Registro)
			{
				if (_da.Registrador(User.FuncionarioId, protocolo.SetorId))
				{
					registro = true;
				}
			}

			return registro;
		}

		public bool ValidarRedirecionamentoReceber(Tramitacao tramitacao, bool permissaoTramitar)
		{
			bool registro = false;

			_validar.RegraSetor(tramitacao.DestinatarioSetor.Id, permissaoTramitar);

			if (_da.ObterTipoSetor(tramitacao.DestinatarioSetor.Id) == 2)
			{
				if (_da.Registrador(User.FuncionarioId, tramitacao.DestinatarioSetor.Id))
				{
					registro = true;
				}
			}

			return registro;
		}

		public bool Enviar(List<Tramitacao> tramitacoes)
		{
			try
			{
				Mensagem msgSucesso = Mensagem.Tramitacao.EnviarEfetuadoComSucesso;
				if (_validar.Enviar(tramitacoes))
				{
					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();
						foreach (Tramitacao tramitacao in tramitacoes)
						{
							_da.Enviar(tramitacao);
						}
						Validacao.Add(msgSucesso);
						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool Receber(List<Tramitacao> tramitacoes)
		{
			try
			{
				if (_validar.Receber(tramitacoes))
				{
					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						foreach (Tramitacao tramitacao in tramitacoes)
						{
							tramitacao.Destinatario.Id = User.FuncionarioId;
							_da.Receber(tramitacao);
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ReceberRegistro(List<Tramitacao> tramitacoes, List<Tramitacao> enviadosParaSetor, int FuncionarioDestinatarioId)
		{
			try
			{
				if (_validar.ReceberRegistro(tramitacoes, enviadosParaSetor, FuncionarioDestinatarioId))
				{
					foreach (Tramitacao tramitacao in enviadosParaSetor)
					{
						tramitacao.Destinatario.Id = FuncionarioDestinatarioId;
					}

					tramitacoes.AddRange(enviadosParaSetor);

					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						foreach (Tramitacao tramitacao in tramitacoes)
						{
							_da.Receber(tramitacao);
						}
						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool MudarTipoTramitacaoSetor(List<Setor> setores)
		{
			try
			{
				if (_validar.MudarTipoTramitacaoSetor(setores))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						List<string> codigos = new List<string>();
						codigos.Add(ePermissao.TramitacaoEnviarRegistro.ToString());
						codigos.Add(ePermissao.TramitacaoReceberRegistro.ToString());

						_busFuncionario.AdicionarPermissaoTramitarRegistro(setores.SelectMany(x => x.Funcionarios).Select(y => y.Id).Distinct().ToList(), codigos, bancoDeDados);

						_da.MudarTipoTramitacaoSetor(setores, bancoDeDados);

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.Tramitacao.TramitConfigSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void SalvarMotivo(Motivo motivo)
		{
			try
			{
				Mensagem mensagem = Mensagem.Tramitacao.MotivoEditado;

				if (motivo.Id == 0)
				{
					mensagem = Mensagem.Tramitacao.MotivoCadastrado;
				}

				if (AtualizarMotivo(motivo))
				{
					Validacao.Add(mensagem);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AtivarMotivo(Motivo motivo)
		{
			if (AtualizarMotivo(motivo))
			{
				Validacao.Add(Mensagem.Tramitacao.MotivoAtivado);
			}
		}

		public void DesativarMotivo(Motivo motivo)
		{
			if (AtualizarMotivo(motivo))
			{
				Validacao.Add(Mensagem.Tramitacao.MotivoDesativado);
			}
		}

		private bool AtualizarMotivo(Motivo motivo)
		{
			try
			{
				if (_validar.SalvarMotivo(motivo))
				{
					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.SalvarMotivo(motivo, bancoDeDados);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter / Filtrar

		public Resultados<Tramitacao> Filtrar(ListarTramitacaoFiltro filtrosListar)
		{
			try
			{
				return _da.Filtrar(new Filtro<ListarTramitacaoFiltro>(filtrosListar));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new Resultados<Tramitacao>();
		}

		public Resultados<Tramitacao> FiltrarEmPosse(ListarTramitacaoFiltro filtrosListar)
		{
			try
			{
				return _da.FiltrarEmPosse(new Filtro<ListarTramitacaoFiltro>(filtrosListar));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new Resultados<Tramitacao>();
		}

		public Resultados<Tramitacao> FiltrarHistorico(ListarTramitacaoFiltro filtrosListar)
		{
			try
			{
				return _da.FiltrarHistorico(new Filtro<ListarTramitacaoFiltro>(filtrosListar));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new Resultados<Tramitacao>();
		}

		public Resultados<Tramitacao> ObterTramitacoes(ListarTramitacaoFiltro filtrosListar)
		{
			try
			{
				if (_validar.BuscarTramitacoesEmPosse(filtrosListar.EmposseSetorId))
				{
					Resultados<Tramitacao> resultados = FiltrarEmPosse(filtrosListar);

					if (_validar.BuscarTramitacoesEmPosse(filtrosListar.EmposseSetorId, resultados))
					{
						return resultados;
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return new Resultados<Tramitacao>();
		}

		public List<FuncionarioLst> ObterFuncionariosRegistrador(int funcionarioId)
		{
			return _da.ObterFuncionariosRegistrador(funcionarioId);
		}

		public List<FuncionarioLst> ObterFuncionariosSetor(int setorId)
		{
			return _busFuncionario.ObterFuncionariosSetor(setorId);
		}

		public List<Setor> ObterSetoresFuncionario(int funcionarioId)
		{
			return _busFuncionario.ObterSetoresFuncionario(funcionarioId);
		}

		public List<Setor> ObterSetoresFuncionarioPorTipo(int funcionarioId, int setorTipoTramitacaoId)
		{
			return _busFuncionario.ObterSetoresPorTipo(funcionarioId, setorTipoTramitacaoId);
		}

		public List<Setor> ObterSetoresRegistrador(int funcionarioId)
		{
			return _busFuncionario.ObterSetoresRegistrador(funcionarioId);
		}

		public List<Setor> ObterSetores()
		{
			try
			{
				return _da.ObterSetores();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public Funcionario ObterExecutor()
		{
			try
			{
				if (User != null)
				{
					return new Funcionario() { Id = User.FuncionarioId, Nome = User.Name };
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<Int32> ObterHistoricoAcoesMostrarPdf()
		{
			List<Int32> acoesId = new List<Int32>();
			Historico historico = new Historico();

			acoesId.Add(historico.ObterHistoricoAcaoId(eHistoricoAcao.enviar, eHistoricoArtefato.tramitacao));
			acoesId.Add(historico.ObterHistoricoAcaoId(eHistoricoAcao.enviarexterno, eHistoricoArtefato.tramitacao));
			acoesId.Add(historico.ObterHistoricoAcaoId(eHistoricoAcao.retirarexterno, eHistoricoArtefato.tramitacao));
			acoesId.Add(historico.ObterHistoricoAcaoId(eHistoricoAcao.arquivar, eHistoricoArtefato.tramitacao));

			return acoesId;
		}

		internal TramitacaoPosse ObterProtocoloPosse(int processoId)
		{
			return _da.ObterProtocoloPosse(processoId);
		}

		#endregion

		#region Obter Tramitações

		public Tramitacao Obter(int tramitacaoId)
		{
			return _da.Obter(tramitacaoId);
		}

		public int ObterTramitacaoProtocolo(int protocolo)
		{
			return _da.ExisteTramitacao(protocolo);
		}

		public Tramitacao ObterTramitacoes(int setorId, int funcionarioId)
		{
			//id do funcionário usa o passado pelo parametro, caso contrario obtem o logado
			int funcionario = funcionarioId > 0 ? funcionarioId : User.FuncionarioId;

			Tramitacao tramitacoes = null;

			tramitacoes = new Tramitacao();
			ListarTramitacaoFiltro filtro;

			#region Processo em minha posse
			filtro = new ListarTramitacaoFiltro() { EmposseId = funcionario, EmposseSetorId = setorId };

			if (funcionarioId != User.FuncionarioId)
			{
				filtro.RegistradorDestinatarioSetorId = User.FuncionarioId;
			}

			tramitacoes.ProtocolosPosse = FiltrarEmPosse(filtro).Itens;
			#endregion

			#region Enviados para mim

			filtro = new ListarTramitacaoFiltro() { DestinatarioId = funcionario, DestinatarioSetorId = setorId };

			tramitacoes.ProtocolosReceber = Filtrar(filtro).Itens;

			#endregion

			#region Enviados para o meu setor

			filtro = new ListarTramitacaoFiltro() { DestinatarioNulo = true, DestinatarioSetorId = setorId };

			filtro.FuncionarioSetorDestinoId = User.FuncionarioId;
			tramitacoes.ProtocolosReceberSetor = Filtrar(filtro).Itens;

			#endregion

			#region Processo enviado por mim

			filtro = new ListarTramitacaoFiltro() { RemetenteId = funcionarioId, RemetenteSetorId = setorId };

			tramitacoes.ProtocolosEnviado = Filtrar(filtro).Itens;

			#endregion

			return tramitacoes;
		}

		public int ObterFuncionarioIdPosse(int protocolo)
		{
			return _da.ObterFuncionarioIdPosse(protocolo);
		}

		public void ValidarProcessoPosse(List<Tramitacao> lista)
		{
			if (lista.Count == 0)
			{
				Validacao.Add(Mensagem.Tramitacao.NenhumProcDocPosse);
			}
		}

		public List<Motivo> ObterMotivos()
		{
			try
			{
				return _da.ObterMotivos();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		#endregion

		#region Validações
		public bool ValidarUsuarioSetorRegistrador(int funcionarioLogadoId, int setorId, int funcionarioId)
		{
			return _validar.RegistradorSetor(funcionarioLogadoId, setorId, funcionarioId);
		}

		public bool Registrador(int funcionarioId, int setorId)
		{
			return _da.Registrador(funcionarioId, setorId);
		}

		public bool Registrador(int funcionarioId)
		{
			return _da.Registrador(funcionarioId);
		}

		public bool SetorPorRegistrado(int setorId, BancoDeDados banco = null)
		{
			try
			{
				return _da.SetorPorRegistrado(setorId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}

		public bool ExisteTramitacao(int protocolo)
		{
			return _da.ExisteTramitacao(protocolo) > 0;
		}

		public bool ExisteTramitacao(int protocolo, eTramitacaoSituacao situacao)
		{
			return _da.ExisteTramitacao(protocolo, (int)situacao) > 0;
		}

		#endregion
	}
}