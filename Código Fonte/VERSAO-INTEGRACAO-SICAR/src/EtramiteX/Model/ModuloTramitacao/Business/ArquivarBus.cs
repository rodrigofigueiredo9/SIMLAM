using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business
{
	public class ArquivarBus
	{
		#region Propriedades

		ArquivarDa _da = new ArquivarDa();
		TramitacaoDa _daTramitacao = new TramitacaoDa();
		ArquivarValidar _validar = new ArquivarValidar();

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

		#region Ações

		public bool Arquivar(Arquivar arquivar, List<Tramitacao> tramitacoes)
		{
			try
			{
				foreach (Tramitacao tramitacao in tramitacoes)
				{
					tramitacao.Arquivamento = arquivar;
					tramitacao.Objetivo.Id = arquivar.ObjetivoId;
					tramitacao.Despacho = arquivar.Despacho;
					tramitacao.Tipo = (int)eTramitacaoTipo.Normal;
					tramitacao.SituacaoId = (int)eTramitacaoSituacao.Arquivado;
					tramitacao.Executor.Id = User.FuncionarioId;
					tramitacao.Remetente.Id = User.FuncionarioId;
					tramitacao.RemetenteSetor.Id = arquivar.SetorId;
				}

				if (_validar.Arquivar(arquivar, tramitacoes))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();
						foreach (Tramitacao tramitacao in tramitacoes)
						{
							_da.Arquivar(tramitacao);
						}

						bancoDeDados.Commit();
						Validacao.Add(Mensagem.Arquivamento.ArquivarSucesso);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool Desarquivar(int arquivoId, List<Tramitacao> tramitacoes, int destinatarioSetor)
		{
			try
			{
				if (_validar.Desarquivar(arquivoId, destinatarioSetor, tramitacoes))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						foreach (Tramitacao tramitacao in tramitacoes)
						{
							tramitacao.DestinatarioSetor.Id = destinatarioSetor;
							tramitacao.Destinatario.Id = User.FuncionarioId;
							_da.Desarquivar(tramitacao);
						}

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.Arquivamento.DesarquivarSucesso);
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

		public List<Estante> ObterEstantesArquivo(int arquivoId)
		{
			try
			{
				return _da.ObterArquivoEstantes(arquivoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return new List<Estante>();
		}

		public List<Prateleira> ObterPrateleirasArquivo(int estante, int modo)
		{
			try
			{
				return _da.ObterArquivoPrateleiras(estante, modo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return new List<Prateleira>();
		}

		public List<Setor> ObterSetoresFuncionario(int funcionarioId)
		{
			FuncionarioBus _busFuncionario = new FuncionarioBus();
			return _busFuncionario.ObterSetoresFuncionario(funcionarioId);
		}

		public Arquivar ObterArquivamento(int tramitacaoId = 0)
		{
			try
			{
				return _da.ObterArquivamento(tramitacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<TramitacaoArquivoLista> ObterArquivosCadastrados(int setorId = 0)
		{
			try
			{
				return _da.ObterArquivos(setorId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Tramitacao ObterItem(int funcId, int funcSetorId, Protocolo protocolo)
		{
			try
			{
				ListarTramitacaoFiltro dados = new ListarTramitacaoFiltro();
				Filtro<ListarTramitacaoFiltro> filtro = new Filtro<ListarTramitacaoFiltro>(dados, new Paginacao());

				//dados.RemetenteId = User.FuncionarioId;

				dados.EmposseId = funcId;

				dados.EmposseSetorId = funcSetorId;
				
				//dados.DestinatarioSetorId = funcSetorId;					
				
				if (!string.IsNullOrEmpty(protocolo.Numero))
				{
					dados.Protocolo = new ProtocoloNumero(protocolo.Numero);					
				}

				if (protocolo.Tipo.Id > 0)
				{
					dados.ProtocoloTipo = protocolo.Tipo.Id;
				}

				Resultados<Tramitacao> resultado = _daTramitacao.FiltrarEmPosse(filtro);

				if (resultado.Itens != null && resultado.Itens.Count > 0)
				{
					return resultado.Itens[0];
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		#endregion

		public bool VerificarContemRegistros(List<Tramitacao> lista)
		{
			try 
			{

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}
	}
}