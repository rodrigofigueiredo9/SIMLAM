using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business
{
	public class RequerimentoCredenciadoBus
	{
		#region Propriedade

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		RequerimentoCredenciadoDa _da;
		RequerimentoInternoBus _requrimentoInternoBus;
		RequerimentoCredenciadoValidar _validar;
		EmpreendimentoInternoBus _empBus;
		PessoaInternoBus _pessoaBus;
		TituloModeloInternoBus _modeloBus;
		RoteiroInternoBus _roteiroBus;
		AtividadeConfiguracaoInternoBus _atividadeConfiguracaoBus;

		private List<Roteiro> RoteirosPadroes
		{
			get { return ListaCredenciadoBus.RoteiroPadrao; }
		}

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public RequerimentoCredenciadoBus() : this(new RequerimentoCredenciadoValidar()) { }

		public RequerimentoCredenciadoBus(RequerimentoCredenciadoValidar validacao)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_validar = validacao;
			_da = new RequerimentoCredenciadoDa();
			_requrimentoInternoBus = new RequerimentoInternoBus();
			_empBus = new EmpreendimentoInternoBus();
			_pessoaBus = new PessoaInternoBus();
			_roteiroBus = new RoteiroInternoBus();
			_modeloBus = new TituloModeloInternoBus();
			_atividadeConfiguracaoBus = new AtividadeConfiguracaoInternoBus();

		}

		#region Ações de DML

		public void SalvarObjetivoPedido(Requerimento requerimento)
		{
			try
			{
				bool criarRequerimento = requerimento.Id <= 0;

				if (_validar.ObjetivoPedidoValidar(requerimento))
				{
					if (requerimento.Atividades.Count(x => x.Id == 327) == 0 || requerimento.InfoPreenchidas == true)
					{
						GerenciadorTransacao.ObterIDAtual();

						using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							bancoDeDados.IniciarTransacao();

							requerimento.CredenciadoId = User.FuncionarioId;

							_da.Salvar(requerimento, bancoDeDados);

							#region Projeto Digital

							ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
							ProjetoDigital projetoDigital = new ProjetoDigital();

							if (!criarRequerimento)
							{
								projetoDigital = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id, banco: bancoDeDados);
							}

							projetoDigital.RequerimentoId = requerimento.Id;
							projetoDigital.Etapa = (int)eProjetoDigitalEtapa.Requerimento;
							projetoDigitalCredenciadoBus.Salvar(projetoDigital, bancoDeDados, criarRequerimento);

							ProjetoDigitalCredenciadoDa projetoDigitalCredenciadoDa = new ProjetoDigitalCredenciadoDa();
							projetoDigitalCredenciadoDa.DesassociarDependencias(projetoDigital, bancoDeDados);

							requerimento.ProjetoDigitalId = projetoDigital.Id;

							#endregion

							if (!Validacao.EhValido)
							{
								bancoDeDados.Rollback();
								return;
							}

							bancoDeDados.Commit();
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AssociarInteressado(Requerimento requerimento)
		{
			try
			{
				Requerimento req = Obter(requerimento.Id);
				req.SituacaoId = (int)eRequerimentoSituacao.EmAndamento;
				req.Interessado.Id = requerimento.Interessado.Id;

				if (_validar.InteressadoValidar(req))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Editar(req, bancoDeDados);

						#region Projeto Digital

						ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
						ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id, banco: bancoDeDados);
						projetoDigital.Etapa = (int)eProjetoDigitalEtapa.Requerimento;
						projetoDigitalCredenciadoBus.Salvar(projetoDigital, bancoDeDados);

						#endregion

						if (Validacao.EhValido)
						{
							bancoDeDados.Commit();
							Validacao.Add(Mensagem.Requerimento.InteressadoSalvar);
						}
						else
						{
							bancoDeDados.Rollback();
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void LimparInteressado(Requerimento requerimento)
		{
			try
			{
				Requerimento req = Obter(requerimento.Id);
				req.SituacaoId = (int)eRequerimentoSituacao.EmAndamento;
				req.Interessado.Id = 0;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Editar(req, bancoDeDados);

					#region Projeto Digital

					ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
					ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id, banco: bancoDeDados);
					projetoDigital.Etapa = (int)eProjetoDigitalEtapa.Requerimento;
					projetoDigitalCredenciadoBus.Salvar(projetoDigital, bancoDeDados);

					#endregion

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return;
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void SalvarResponsavelTecnico(Requerimento requerimento)
		{
			try
			{
				requerimento.SituacaoId = (int)eRequerimentoSituacao.EmAndamento;

				if (_validar.ResponsavelTecnicoValidar(requerimento.Responsaveis, requerimento.Atividades))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Editar(requerimento, bancoDeDados);

						#region Projeto Digital

						ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
						ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id, banco: bancoDeDados);
						projetoDigital.Etapa = (int)eProjetoDigitalEtapa.Requerimento;
						projetoDigitalCredenciadoBus.Salvar(projetoDigital, bancoDeDados);

						#endregion

						if (!Validacao.EhValido)
						{
							bancoDeDados.Rollback();
							return;
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ExcluirResponsaveis(Requerimento requerimento)
		{
			try
			{
				requerimento = Obter(requerimento.Id);
				requerimento.SituacaoId = (int)eRequerimentoSituacao.EmAndamento;
				requerimento.Responsaveis.Clear();

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Editar(requerimento, bancoDeDados);

					#region Projeto Digital

					ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
					ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id, banco: bancoDeDados);
					projetoDigital.Etapa = (int)eProjetoDigitalEtapa.Requerimento;
					projetoDigitalCredenciadoBus.Salvar(projetoDigital, bancoDeDados);

					#endregion

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return;
					}

					Validacao.Add(Mensagem.Requerimento.SalvarResponsavelTec);
					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool AssociarEmpreendimento(Requerimento requerimento)
		{
			try
			{
				Requerimento req = Obter(requerimento.Id);
				req.Empreendimento = requerimento.Empreendimento;
				req.SituacaoId = (int)eRequerimentoSituacao.EmAndamento;

				if (!_validar.AssociarEmpreendimento(req))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Editar(req, bancoDeDados);

					#region Projeto Digital

					ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
					ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id, banco: bancoDeDados);
					projetoDigital.EmpreendimentoId = req.Empreendimento.Id;
					projetoDigital.Etapa = (int)eProjetoDigitalEtapa.Requerimento;
					projetoDigitalCredenciadoBus.Salvar(projetoDigital, bancoDeDados);

					ProjetoDigitalCredenciadoDa projetoDigitalCredenciadoDa = new ProjetoDigitalCredenciadoDa();
					projetoDigitalCredenciadoDa.DesassociarDependencias(projetoDigital, bancoDeDados);

					#endregion

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return false;
					}

					bancoDeDados.Commit();

					if (req.Empreendimento.Id > 0)
					{
						Validacao.Add(Mensagem.Requerimento.EmpreendimentoSalvar);
					}

					return true;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public void Finalizar(Requerimento requerimento)
		{
			try
			{
				#region Projeto Digital

				ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
				ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id);
				projetoDigital.Etapa = (int)eProjetoDigitalEtapa.Caracterizacao;
				requerimento.ProjetoDigitalId = projetoDigital.Id;

				#endregion

				requerimento = Obter(requerimento.Id);
				requerimento.SituacaoId = (int)eRequerimentoSituacao.Finalizado;

				if (_validar.Finalizar(requerimento))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						Mensagem msgSucesso = Mensagem.Requerimento.FinalizarCredenciado(requerimento.Numero);

						_da.Editar(requerimento);

						projetoDigitalCredenciadoBus.Salvar(projetoDigital, bancoDeDados, true);

						if (Validacao.EhValido)
						{
							Validacao.Add(msgSucesso);
							bancoDeDados.Commit();
						}
						else
						{
							bancoDeDados.Rollback();
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool AlterarSituacao(Requerimento requerimento, BancoDeDados banco = null)
		{
			try
			{
				int situacao = requerimento.SituacaoId;

				requerimento = Obter(requerimento.Id);

				requerimento.SituacaoId = situacao;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Editar(requerimento, banco);

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return false;
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void Excluir(int id, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.Excluir(id))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id, banco);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter / Filtrar

		public int ObterPessoaId(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterPessoaId(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public Resultados<Requerimento> Filtrar(RequerimentoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<RequerimentoListarFiltro> filtro = new Filtro<RequerimentoListarFiltro>(filtrosListar, paginacao);
				Resultados<Requerimento> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}
				else
				{
					foreach (var item in resultados.Itens)
					{
						item.SituacaoTexto = ListaCredenciadoBus.SituacoesRequerimento.SingleOrDefault(x => x.Id == item.SituacaoId).Texto;
					}
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Requerimento Obter(int id, bool obterPessoas = false)
		{
			Requerimento requerimento = null;
			try
			{
				requerimento = _da.Obter(id);
				CarregarDadosListas(requerimento);
				requerimento.Roteiros = ObterRoteirosPorAtividades(requerimento.Atividades);
				requerimento.Roteiros = ObterRequerimentoRoteiros(requerimento.Id, requerimento.SituacaoId, atividades: requerimento.Atividades);
				if (obterPessoas && requerimento != null && requerimento.Id > 0)
				{
					requerimento.Pessoas = ObterPessoas(requerimento.Id);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return requerimento;
		}

		void CarregarDadosListas(Requerimento requerimento)
		{
			requerimento.SituacaoTexto = ListaCredenciadoBus.SituacoesRequerimento.Single(x => x.Id == requerimento.SituacaoId).Texto;

			List<Atividade> atividades = new List<Atividade>();
			Atividade atividade = new Atividade();
			Finalidade finalidade = new Finalidade();
			requerimento.Atividades.ForEach(x =>
			{
				atividade = _roteiroBus.ObterAtividade(x);
				x.NomeAtividade = atividade.NomeAtividade;
				x.SetorId = atividade.SetorId;
				x.Finalidades.ForEach(y =>
				{
					finalidade = _roteiroBus.ObterFinalidade(y);
					y.Codigo = finalidade.Codigo;
					y.Texto = finalidade.Texto;
					y.TituloModeloTexto = finalidade.TituloModeloTexto;
				});
			});
		}

		public List<Roteiro> ObterRequerimentoRoteiros(int requerimentoId, int situacao, BancoDeDados banco = null, List<Atividade> atividades = null)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			if (situacao == (int)eRequerimentoSituacao.Protocolado)
			{
				roteiros = _da.ObterRequerimentoRoteirosHistorico(requerimentoId, situacao, banco);
			}
			else
			{
				roteiros = _roteiroBus.ObterRoteirosPorAtividades(atividades ?? _da.Obter(requerimentoId, banco).Atividades);
			}

			roteiros = roteiros.GroupBy(x => x.Id).Select(y => new Roteiro
			{
				Id = y.First().Id,
				Nome = y.First().Nome,
				VersaoAtual = y.First().VersaoAtual,
				Tid = y.First().Tid,
				AtividadeTexto = y.Select(w => w.AtividadeTexto).Distinct().Aggregate((total, atual) => total + " / " + atual)
			}).ToList();

			return roteiros;
		}

		public Requerimento ObterSimplificado(int id)
		{
			Requerimento requerimento = null;

			try
			{
				requerimento = _da.Obter(id, simplificado: true);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return requerimento;
		}

		public Requerimento ObterPdf(int id)
		{
			Requerimento requerimento = null;

			try
			{
				requerimento = _da.Obter(id);
				requerimento.Empreendimento = _empBus.Obter(requerimento.Empreendimento.Id);
				requerimento.Interessado = _pessoaBus.Obter(requerimento.Interessado.Id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return requerimento;
		}

		public Requerimento ObterFinalizar(int id)
		{
			try
			{
				Requerimento requerimento = Obter(id);

				if (requerimento == null)
				{
					Validacao.Add(Mensagem.Requerimento.Inexistente);
				}

				return requerimento;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<TituloModeloLst> ObterModelosAtividades(List<AtividadeSolicitada> Atividades, bool renovacao)
		{
			return _atividadeConfiguracaoBus.ObterModelosAtividades(Atividades, renovacao);
		}

		public List<TituloModeloLst> ObterModelosAnteriores(int modelo)
		{
			return _modeloBus.ObterModelosAnteriores(modelo);
		}

		public List<Roteiro> ObterRoteirosPorAtividades(List<Atividade> atividades)
		{
			try
			{
				if (atividades == null)
				{
					Validacao.Add(Mensagem.Requerimento.AtividadeObrigatorio);
					return null;
				}

				CarregarFinalidades(atividades);

				return _roteiroBus.ObterRoteirosPorAtividades(atividades);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		private void CarregarFinalidades(List<Atividade> atividades)
		{
			foreach (var atividade in atividades)
			{
				_roteiroBus.ObterAtividade(atividade);
				foreach (var finalidade in atividade.Finalidades)
				{
					finalidade.Codigo = _roteiroBus.ObterFinalidadeCodigo(finalidade.Id);
				}
			}
		}

		public List<TituloModeloLst> ObterModelosRenovacao(int modelo)
		{
			return _modeloBus.ObterModelosRenovacao(modelo);
		}

		public List<TituloModeloLst> ObterNumerosTitulos(string numero, int modeloId)
		{
			TituloModeloInternoBus bus = new TituloModeloInternoBus();
			List<TituloModeloLst> titulos = new List<TituloModeloLst>();

			try
			{
				if (!_validar.ObterNumerosTitulos(numero, modeloId))
				{
					return titulos;
				}

				if (ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
				{
					titulos = _requrimentoInternoBus.ObterNumerosTitulos(numero, modeloId);

					switch (titulos.Count)
					{
						case 0:
							Validacao.Add(Mensagem.Requerimento.TituloNaoEncontrado);
							break;

						case 1:
							Validacao.Add(Mensagem.Requerimento.TituloEncontrado);
							break;

						default:
							Validacao.Add(Mensagem.Requerimento.TitulosEncontrados);
							break;
					}
				}
				else
				{
					if (_da.ValidarNumeroSemAnoExistente(numero, modeloId))
					{
						Validacao.Add(Mensagem.Requerimento.TituloNumeroSemAnoEncontrado);
					}
					else
					{
						Validacao.Add(Mensagem.Requerimento.TituloNaoEncontrado);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return titulos;
		}

		public List<Pessoa> ObterPessoas(int requerimento = 0)
		{
			try
			{
				return _da.ObterPessoas(requerimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<int> ObterResponsavelTecnico (int requerimento =0)
		{
			try
			{
				return _da.ObterResponsavelTecnico(requerimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
		#endregion

		#region Validações

		public void ValidarSituacaoVersaoRoteiro(List<Roteiro> roteiros)
		{
			for (int i = 0; i < roteiros.Count; i++)
			{
				if (roteiros[i].Situacao == 2)
				{
					Validacao.Add(Mensagem.Requerimento.RoteiroDesativo(roteiros[i].Numero));
				}

				if (roteiros[i].Situacao == 1 && roteiros[i].VersaoAtual != roteiros[i].Versao)
				{
					Validacao.Add(Mensagem.Requerimento.RoteiroAtualizado(roteiros[i].Numero, roteiros[i].VersaoAtual));
				}
			}
		}

		public TituloModelo VerficarTituloPassivelRenovação(int titulo)
		{
			return _modeloBus.Obter(titulo);
		}

		public bool ValidarEditar(Requerimento requerimento)
		{
			if (requerimento != null && requerimento.SituacaoId == (int)eRequerimentoSituacao.Protocolado)
			{
				Validacao.Add(Mensagem.Requerimento.Protocolado(requerimento.Id));
				return Validacao.EhValido;
			}

			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			ProjetoDigital projetoDigital = projetoDigitalCredenciadoBus.Obter(requerimento.ProjetoDigitalId);

			if (!projetoDigitalCredenciadoBus.SituacoesEditaveis.Exists(x => x == projetoDigital.Situacao))
			{
				Validacao.Add(Mensagem.ProjetoDigital.EditarSituacaoInvalida(projetoDigital.SituacaoTexto));
			}

			return Validacao.EhValido;

		}

		public void ValidarNumeroProcesso(string numero)
		{
			_validar.ValidarNumero(numero);
		}

		public void ValidarModeloAnteriorNumero(int tituloAnteriorId, int tituloAnteriorTipo)
		{
			_validar.ValidarModeloAnteriorNumero(tituloAnteriorId, tituloAnteriorTipo);
		}

		public void ValidarRoteiroRemovido(Requerimento requerimento, bool advertencia = false)
		{
			try
			{
				List<Roteiro> roteiros = ObterRoteirosPorAtividades(requerimento.Atividades);
				List<Roteiro> roteirosMensagem = new List<Roteiro>();

				foreach (Roteiro roteiro in requerimento.Roteiros)
				{
					bool existe = false;

					foreach (var item in roteiros)
					{
						if (roteiro.Id == item.Id)
						{
							existe = true;
							break;
						}
					}

					if (!existe)
					{
						roteirosMensagem.Add(roteiro);
					}
				}

				foreach (var item in roteirosMensagem)
				{
					if (advertencia)
					{
						Validacao.Add(Mensagem.Requerimento.RoteirosRemovidos(item.Numero.ToString()));
					}
					else
					{
						Validacao.Add(Mensagem.Requerimento.RoteirosRemovidosEditar(item.Numero.ToString()));
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool Existe(int requerimentoId)
		{
			try
			{
				return _da.Existe(requerimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool RequerimentoDeclaratorio(int requerimentoId)
		{
			return _da.RequerimentoDeclaratorio(requerimentoId);
		}

		public bool VerificarRequerimentoPossuiModelo(int modeloId, int requerimentoId)
		{
			return _da.VerificarRequerimentoPossuiModelo(modeloId, requerimentoId);
		}

		public bool IsRequerimentoRegularizacaoFundiaria(Atividade atividade)
		{
			if (atividade.NomeAtividade.ToUpper().Contains("REGULARIZAÇÃO FUNDIÁRIA"))
				return true;
			return false;
		}

		public bool IsRequerimentoAtividadeCorte(int requerimento)
		{
			try
			{
				return _da.IsRequerimentoAtividadeCorte(requerimento);
			} catch(Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return false;
		}

		#endregion
	}
}