using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business
{
	public class ProjetoDigitalCredenciadoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		ProjetoDigitalCredenciadoDa _da;

		ProjetoDigitalCredenciadoValidar _validar;

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public List<int> SituacoesEditaveis
		{
			get
			{
				return new List<int>() { (int)eProjetoDigitalSituacao.EmElaboracao, (int)eProjetoDigitalSituacao.AguardandoCorrecao };
			}
		}

		#endregion

		public ProjetoDigitalCredenciadoBus(string esquema = null)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_validar = new ProjetoDigitalCredenciadoValidar(UsuarioCredenciado);
			_da = new ProjetoDigitalCredenciadoDa(UsuarioCredenciado);
		}

		#region Ações DML

		public ProjetoDigital Salvar(ProjetoDigital projeto, BancoDeDados banco, bool gerarHistorico = false)
		{
			try
			{
				projeto.Situacao = (int)eProjetoDigitalSituacao.EmElaboracao;
				projeto.CredenciadoId = User.FuncionarioId;

				if (_validar.Salvar(projeto))
				{
					projeto.CredenciadoId = User.FuncionarioId;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(projeto, bancoDeDados, gerarHistorico);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return projeto;
		}

		public bool AlterarSituacao(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.AlterarSituacao(projeto, bancoDeDados);

					bancoDeDados.Commit();
				}

				return true;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public void Enviar(ProjetoDigital projeto)
		{
			try
			{
				projeto.Etapa = (int)eProjetoDigitalEtapa.ImprimirDocumentos;

				RequerimentoCredenciadoBus requerimentoCredenciadoBus = new RequerimentoCredenciadoBus();
				Requerimento requerimento = requerimentoCredenciadoBus.Obter(projeto.RequerimentoId);
				requerimento.ProjetoDigitalId = projeto.Id;

				if (_validar.Enviar(requerimento, projeto))
				{
					projeto.Situacao = (int)eProjetoDigitalSituacao.AguardandoImportacao;
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Enviar(projeto, bancoDeDados);

						SalvarTemporario(projeto, bancoDeDados);

						Validacao.Add(Mensagem.ProjetoDigital.Enviar);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void CancelarEnvio(int projetoId)
		{
			try
			{
				ProjetoDigital projeto = Obter(projetoId);

				if (_validar.CancelarEnvio(projeto))
				{
					projeto.Situacao = (int)eProjetoDigitalSituacao.EmElaboracao;
					projeto.Etapa = (int)eProjetoDigitalEtapa.Caracterizacao;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						int solicitacao = new RequerimentoCredenciadoDa().PossuiSolicitacaoCARValidaSuspensaPendente(projeto.RequerimentoId);
						/*if (solicitacao > 0)
						{
							new CARSolicitacaoBus().AlterarSituacao(
								new CARSolicitacao() { Id = solicitacao },
								new CARSolicitacao() { SituacaoId = (int)eCARSolicitacaoSituacao.Invalido }, 
								bancoDeDados);
						}*/

						TituloDeclaratorioBus tituloDeclaratorioBus = new TituloDeclaratorioBus();
						tituloDeclaratorioBus.AcerrarTitulo(projeto.RequerimentoId);
						if(!Validacao.EhValido)
						{
							bancoDeDados.Rollback();
							return;
						}

						_da.CancelarEnvio(projeto, bancoDeDados);

						ExcluirTemporario(projeto.Id);

						Validacao.Add(Mensagem.ProjetoDigital.CancelarEnvio);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Excluir(int projetoId)
		{
			try
			{
				ProjetoDigital projeto = Obter(projetoId);

				if (_validar.Excluir(projeto))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(projetoId, bancoDeDados);

						RequerimentoCredenciadoBus requerimentoCredenciadoBus = new RequerimentoCredenciadoBus();

						requerimentoCredenciadoBus.Excluir(projeto.RequerimentoId, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.ProjetoDigital.Excluir);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public ProjetoDigital AlterarEtapa(int projetoDigitalId, eProjetoDigitalEtapa etapa, BancoDeDados banco = null)
		{
			try
			{
				ProjetoDigital projetoDigital = new ProjetoDigital();

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					projetoDigital = Obter(idProjeto: projetoDigitalId, banco: bancoDeDados);

					if (projetoDigital != null && projetoDigital.Id > 0 && projetoDigital.Etapa != (int)etapa)
					{
						projetoDigital.Etapa = (int)etapa;

						Salvar(projetoDigital, bancoDeDados);

						bancoDeDados.Commit();
					}
				}

				return projetoDigital;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public bool Recusar(ProjetoDigital projeto, BancoDeDados banco = null)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Recusar(projeto, bancoDeDados);

					bancoDeDados.Commit();
				}

				return true;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public void AssociarDependencias(ProjetoDigital projetoDigital, BancoDeDados banco = null)
		{
			try
			{
				#region Configurar

				List<Caracterizacao> lista = new List<Caracterizacao>();
				lista.Add(new Caracterizacao() { Tipo = (eCaracterizacao)projetoDigital.Dependencias.First().DependenciaCaracterizacao });

				CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
				lista = caracterizacaoBus.ObterCaracterizacoesAtuais(projetoDigital.EmpreendimentoId.GetValueOrDefault(), lista);

				Caracterizacao aux = lista.First();
				projetoDigital.Dependencias.Clear();

				projetoDigital.Dependencias.Add(new Dependencia()
				{
					DependenciaTipo = (int)eCaracterizacaoDependenciaTipo.Caracterizacao,
					DependenciaCaracterizacao = (int)aux.Tipo,
					DependenciaId = aux.Id,
					DependenciaTid = aux.Tid
				});

				if (aux.ProjetoId > 0)
				{
					projetoDigital.Dependencias.Add(new Dependencia()
					{
						DependenciaTipo = (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico,
						DependenciaCaracterizacao = (int)aux.Tipo,
						DependenciaId = aux.ProjetoId,
						DependenciaTid = aux.ProjetoTid
					});
				}

				#endregion

				if (_validar.AssociarDependencias(projetoDigital))
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.AssociarDependencias(projetoDigital, bancoDeDados);

						AlterarEtapa(projetoDigital.Id, eProjetoDigitalEtapa.Caracterizacao, bancoDeDados);

						Validacao.Add(Mensagem.ProjetoDigital.AssociadaProjetoDigital);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void DesassociarDependencias(ProjetoDigital projetoDigital, BancoDeDados banco = null)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.DesassociarDependencias(projetoDigital, bancoDeDados);

					AlterarEtapa(projetoDigital.Id, eProjetoDigitalEtapa.Caracterizacao, bancoDeDados);

					Validacao.Add(Mensagem.ProjetoDigital.DesassociadaProjetoDigital);
					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void FinalizarPassoCaracterizacao(int empreendimentoID, int projetoDigitalID)
		{
			try
			{
				if (_validar.FinalizarCaracterizacoes(new ProjetoDigital() { Id = projetoDigitalID, EmpreendimentoId = empreendimentoID }))
				{
					ProjetoDigital projetoDigital = AlterarEtapa(projetoDigitalID, eProjetoDigitalEtapa.Envio);
					Validacao.Add(Mensagem.Caracterizacao.CaracterizacoesFinalizadasSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarCaracterizacao(ProjetoDigital projetoDigital, BancoDeDados banco)
		{
			int projetoDigitalID = projetoDigital.Id;
			projetoDigital.Id = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				_da.DesassociarDependencias(projetoDigital, bancoDeDados);

				Resultados<ProjetoDigital> resultados = Filtrar(
					new ProjetoDigitalListarFiltro()
					{
						EmpreendimentoID = projetoDigital.EmpreendimentoId.GetValueOrDefault(),
						Situacao = (int)eProjetoDigitalSituacao.EmElaboracao
					},
					new Paginacao() { QuantPaginacao = Int32.MaxValue });

				if (resultados != null && resultados.Itens != null && resultados.Itens.Count > 0)
				{
					foreach (var item in resultados.Itens.Where(x => x.Etapa == (int)eProjetoDigitalEtapa.Envio))
					{
						if (item.Id == projetoDigitalID)
						{
							continue;
						}

						AlterarEtapa(item.Id, eProjetoDigitalEtapa.Caracterizacao, bancoDeDados);
					}
				}

				if (!Validacao.EhValido)
				{
					bancoDeDados.Rollback();
					return;
				}

				bancoDeDados.Commit();
			}
		}

		public bool ImprimirDocumentos(ProjetoDigital projeto)
		{
			Validacao.Add(Mensagem.ProjetoDigital.ImprimirDocumentosConcluido);

			return Validacao.EhValido;
		}

		#endregion

		#region Ações DML Temporario

		public void SalvarTemporario(ProjetoDigital projeto, BancoDeDados banco)
		{
			projeto = _da.ObterHistorico(id: projeto.Id, tid: GerenciadorTransacao.ObterIDAtual(), banco: banco);

			//Salvar no Interno
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

				//TODO Verificar o porque deste codigo
				_da.ExcluirTemporario(projeto.Id, bancoDeDados);

				_da.CriarTemporario(projeto, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void ExcluirTemporario(int projetoDigitalId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

				_da.ExcluirTemporario(projetoDigitalId, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void AlterarEtapaTemporario(int projetoDigitalId, eProjetoDigitalEtapaImportacao etapaImportacao, BancoDeDados banco)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.AlterarEtapaTemporario(projetoDigitalId, etapaImportacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		public ProjetoDigital Obter(int idProjeto = 0, int idRequerimento = 0, string tid = null, BancoDeDados banco = null)
		{
			try
			{
				ProjetoDigital projeto;

				if (String.IsNullOrWhiteSpace(tid))
				{
					projeto = _da.Obter(idProjeto, idRequerimento, banco);
				}
				else
				{
					projeto = _da.Obter(idProjeto, banco, tid);
				}

				projeto.Dependencias = ObterDependencias(idProjeto);

				return projeto;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Dependencia> ObterDependencias(int projetoDigitalID, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterDependencias(projetoDigitalID, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<ProjetoDigital> Filtrar(ProjetoDigitalListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ProjetoDigitalListarFiltro> filtro = new Filtro<ProjetoDigitalListarFiltro>(filtrosListar, paginacao);
				Resultados<ProjetoDigital> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public int ObterArquivoCroquiId(int idProjetoGeo, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterArquivoCroquiId(idProjetoGeo, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public Int32 ObterProjetoDigitalId(int requerimentoId, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterProjetoDigitalId(requerimentoId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;

		}

		#endregion

		#region Validações

		public bool PossuiAtividadeCAR(int projetoDigitalID)
		{
			try
			{
				return _da.PossuiAtividadeCAR(projetoDigitalID);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool PossuiSolicitacaoCAR(int projetoDigitalID)
		{
			try
			{
				return _da.PossuiSolicitacaoCAR(projetoDigitalID);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		#endregion
	}
}