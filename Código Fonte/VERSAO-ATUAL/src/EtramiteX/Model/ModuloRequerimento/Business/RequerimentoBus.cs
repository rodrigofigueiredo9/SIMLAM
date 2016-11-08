using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business
{
	public class RequerimentoBus
	{
		#region Propriedade
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		RequerimentoDa _da;
		RequerimentoValidar _validar;
		EmpreendimentoBus _empBus;
		PessoaBus _busPessoa;	
		TituloModeloBus _modeloBus;
		RoteiroBus _roteiroBus;
		AtividadeConfiguracaoBus _atividadeConfiguracaoBus;
		ListaBus _busLista;

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private List<Roteiro> RoteirosPadroes
		{
			get { return _busLista.RoteiroPadrao; }
		}

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public RequerimentoBus() : this(new RequerimentoValidar()) { }

		public RequerimentoBus(RequerimentoValidar validacao)
		{
			_validar = validacao;
			_da = new RequerimentoDa();
			 _empBus = new EmpreendimentoBus();
			_busPessoa = new PessoaBus();
			_modeloBus = new TituloModeloBus(new TituloModeloValidacao());
			_roteiroBus = new RoteiroBus();
			_atividadeConfiguracaoBus = new AtividadeConfiguracaoBus();
			_busLista = new ListaBus();
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		}

		#region Ações de DML

		public void SalvarObjetivoPedido(Requerimento requerimento)
		{
			try
			{
				ValidarEditar(requerimento);

				if (Validacao.EhValido && _validar.ObjetivoPedidoValidar(requerimento))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(requerimento, bancoDeDados);

						bancoDeDados.Commit();
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

				ValidarEditar(requerimento);

				if (Validacao.EhValido && _validar.InteressadoValidar(req))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Editar(req, bancoDeDados);

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.Requerimento.InteressadoSalvar);
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

				ValidarEditar(requerimento);

				if (!Validacao.EhValido)
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Editar(req, bancoDeDados);

					bancoDeDados.Commit();
				}

				if (req.Empreendimento.Id > 0)
				{
					Validacao.Add(Mensagem.Requerimento.EmpreendimentoSalvar);
				}

				return true;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public void SalvarResponsavelTecnico(Requerimento requerimento)
		{
			try
			{
				ValidarEditar(requerimento);

				if (!Validacao.EhValido)
				{
					return;
				}

				requerimento.SituacaoId = (int)eRequerimentoSituacao.EmAndamento;

				if (_validar.ResponsavelTecnicoValidar(requerimento.Responsaveis, requerimento.Atividades))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Editar(requerimento, bancoDeDados);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool Finalizar(Requerimento requerimento)
		{
			try
			{
				requerimento = Obter(requerimento.Id);
				requerimento.SituacaoId = (int)eRequerimentoSituacao.Finalizado;

				if (_validar.Finalizar(requerimento))
				{
					GerenciadorTransacao.ObterIDAtual();
					
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						Mensagem msgSucesso = Mensagem.Requerimento.Finalizar(requerimento.Numero);

						_da.Editar(requerimento);

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

		public void Excluir(int id)
		{
			try
			{
				if (!_validar.Excluir(id))
				{
					return;
				}

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					RequerimentoCredenciadoBus bus = new RequerimentoCredenciadoBus();

					if (bus.Existe(id))
					{
						bool alterou = bus.AlterarSituacao(new Requerimento() { Id = id, SituacaoId = (int)eRequerimentoSituacao.Finalizado });

						if (!alterou)
						{
							Validacao.Add(Mensagem.Requerimento.ExcluirCredenciado(id));
							bancoDeDados.Rollback();

							return;
						}
					}

					bancoDeDados.Commit();

					Validacao.Add(Mensagem.Requerimento.Excluir(id));
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

				ValidarEditar(requerimento);

				if (!Validacao.EhValido)
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Editar(requerimento, bancoDeDados);

					Validacao.Add(Mensagem.Requerimento.SalvarResponsavelTec);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AlterarSituacao(Requerimento requerimento, BancoDeDados banco = null)
		{
			int situacao = requerimento.SituacaoId;

			requerimento = Obter(requerimento.Id);

			requerimento.SituacaoId = situacao;

			_da.Editar(requerimento, banco);
		}

		#endregion

		#region Obter / Filtrar

		public int ObterNovoID(BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				return _da.ObterNovoID(bancoDeDados);
			}
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

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Requerimento Obter(int id)
		{
			Requerimento requerimento = null;

			try
			{
				requerimento = _da.Obter(id);
				//requerimento.Roteiros = ObterRoteirosPorAtividades(requerimento.Atividades);
				requerimento.Roteiros = ObterRequerimentoRoteiros(requerimento.Id, requerimento.SituacaoId, atividades: requerimento.Atividades);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return requerimento;
		}

		public List<Roteiro> ObterRequerimentoRoteiros(int requerimentoId, int situacao, BancoDeDados banco = null, List<Atividade> atividades = null)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			if (situacao == 3)//Protocolado
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
				requerimento = _da.ObterSimplificado(id);
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
				requerimento.Interessado = _busPessoa.Obter(requerimento.Interessado.Id);
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
			TituloModeloBus bus = new TituloModeloBus();
			List<TituloModeloLst> titulos = new List<TituloModeloLst>();

			try
			{
				if (!_validar.ObterNumerosTitulos(numero, modeloId))
				{
					return titulos;
				}

				if (ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
				{
					titulos = _da.ObterNumerosTitulos(numero, modeloId);

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
		
		public void ValidarEditar(Requerimento requerimento)
		{
			if (requerimento.IsRequerimentoDigital)
			{
				Validacao.Add(Mensagem.Requerimento.NaoPodeEditarReqDigital);

				return;
			}

			string situacao = PossuiTituloDeclaratorio(requerimento.Id);
			if (!string.IsNullOrWhiteSpace(situacao))
			{
				Validacao.Add(Mensagem.Requerimento.PossuiTituloDeclaratorio(situacao));
			}

			if (requerimento != null && requerimento.SituacaoId == 3)
			{
				Validacao.Erros.Clear();
				Validacao.Add(Mensagem.Requerimento.Protocolado(requerimento.Id));
			}
		}

		public string PossuiTituloDeclaratorio(int requerimento)
		{
			return _da.PossuiTituloDeclaratorio(requerimento);
		}

		public void ValidarModeloAnteriorNumero(int tituloAnteriorId, int tituloAnteriorTipo)
		{
			_validar.ValidarModeloAnteriorNumero(tituloAnteriorId, tituloAnteriorTipo);
		}

		public void ValidarRoteiroRemovido(Requerimento requerimento, bool advertencia= false)
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

		#endregion
	}
}