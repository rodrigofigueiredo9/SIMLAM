using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloModeloBus
	{
		#region Propriedades

		TituloModeloValidacao _validar;
		TituloModeloDa _da = new TituloModeloDa();
		FuncionarioBus _busFuncionario = new FuncionarioBus();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public TituloModeloBus() { _validar = new TituloModeloValidacao(); }

		public TituloModeloBus(TituloModeloValidacao validar)
		{
			_validar = validar;
		}

		public void Salvar(TituloModelo tituloModelo)
		{
			try
			{
				if (_validar.Salvar(tituloModelo))
				{
					if (!tituloModelo.Regra(eRegra.PdfGeradoSistema))
					{
						tituloModelo.Arquivo.Id = null;
					}

					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivo

						if (tituloModelo.Arquivo.Id != null && tituloModelo.Arquivo.Id == 0)
						{
							ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
							tituloModelo.Arquivo = _busArquivo.Copiar(tituloModelo.Arquivo);
						}

						if (tituloModelo.Arquivo.Id == 0)
						{
							ArquivoDa _arquivoDa = new ArquivoDa();
							_arquivoDa.Salvar(tituloModelo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
						}

						#endregion

						tituloModelo.Regras = tituloModelo.Regras.FindAll(x => x.Valor == true);

						if (tituloModelo.Regra(eRegra.Renovacao))
						{
							TituloModeloRegra regra = tituloModelo.Regras.SingleOrDefault(x => x.TipoEnum == eRegra.Renovacao);
							regra.Respostas.Add(new TituloModeloResposta() { Valor = tituloModelo.Id, TipoEnum = eResposta.Modelo });
						}

						_da.Salvar(tituloModelo, bancoDeDados);

						bancoDeDados.Commit();
						Validacao.Add(Mensagem.TituloModelo.TituloModeloEditado);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public List<TituloAssinante> ObterAssinantes(TituloModelo modelo)
		{
			try
			{
				List<TituloAssinante> lstAssinantes = new List<TituloAssinante>();

				foreach (Assinante assinante in modelo.Assinantes)
				{
					// Somente responsável pelo setor
					if (assinante.TipoId == 1)
					{
						FuncionarioLst func = _busFuncionario.ObterResponsavelSetor(assinante.SetorId);
						if (func != null && func.Id > 0 && !lstAssinantes.Exists(x => x.FuncionarioId == func.Id))
						{
							lstAssinantes.Add(new TituloAssinante() { FuncionarioId = func.Id, FuncionarioNome = func.Texto });
						}
					}
					// Qualquer funcionário do setor
					else if (assinante.TipoId == 2)
					{
						List<FuncionarioLst> funcLst = _busFuncionario.ObterFuncionariosSetor(assinante.SetorId);
						foreach (FuncionarioLst func in funcLst)
						{
							if (!lstAssinantes.Exists(x => x.FuncionarioId == func.Id)) {
								lstAssinantes.Add(new TituloAssinante() { FuncionarioId = func.Id, FuncionarioNome = func.Texto });
							}
						}
					}
				}
				return lstAssinantes;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public TituloModelo Obter(int id)
		{
			try
			{
				TituloModelo tituloModelo = _da.Obter(id);

				if (tituloModelo.Regra(eRegra.FaseAnterior))
				{
					foreach (var item in tituloModelo.Respostas(eRegra.FaseAnterior, eResposta.Modelo))
					{
						if (tituloModelo.Id != Convert.ToInt32(item.Valor))
						{
							TituloModelo titulo = _da.ObterSimplificado(Convert.ToInt32(item.Valor));

							titulo.IdRelacionamento = item.Id;

							tituloModelo.Modelos.Add(titulo);
						}
					}
				}
				return tituloModelo;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public TituloModelo ObterSimplificado(int id)
		{
			try
			{
				return _da.ObterSimplificado(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		internal TituloModelo ObterSimplificadoCodigo(int codigo, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterSimplificadoCodigo(codigo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public string ObterTextoPadraoEmail(int id)
		{
			try
			{
				return string.Empty;// _da.ObterTextoPadraoEmail(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public Resultados<TituloModelo> Filtrar(TituloModeloListarFiltro filtros, Paginacao paginacao)
		{
			try
			{
				Filtro<TituloModeloListarFiltro> filtro = new Filtro<TituloModeloListarFiltro>(filtros, paginacao);
				Resultados<TituloModelo> resultados = _da.Filtrar(filtro);

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

		public List<TituloModeloLst> ObterModelos(int exceto = 0, bool todos = false)
		{
			try
			{
				List<TituloModeloLst> modelos = _da.ObterModelos(exceto, todos);

				return modelos;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<TituloModeloLst> ObterModelosLista(TituloModeloListarFiltro filtros)
		{
			try
			{
				List<TituloModeloLst> modelos = _da.ObterModelosLista(filtros);
				return modelos;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Situacao> ObterSituacoes()
		{
			return _da.ObterSituacoes();
		}

		public List<TituloModeloLst> ObterModelosSetorFunc(int setor = 0)
		{
			try
			{
				return _da.ObterModelosSetorFunc(User.FuncionarioId, setor);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

        public List<TituloModeloLst> ObterModelosDeclaratorios()
		{
			try
			{
                return _da.ObterModelosDeclaratorios();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}        

		public List<ListaValor> ObterAssinanteFuncionarios(int modeloId, int setorId, int cargoId)
		{
			try
			{
				return _da.ObterAssinanteFuncionarios(modeloId, setorId, cargoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<ListaValor> ObterAssinanteCargos(int modeloId, int setorId)
		{
			try
			{
				return _da.ObterAssinanteCargos(modeloId, setorId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<Setor> ObterSetoresModelo(int modelo)
		{
			try
			{
				return _da.ObterSetoresModelo(modelo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<Setor> ObterSetoresModeloPorTitulo(int titulo)
		{
			try
			{
				return _da.ObterSetoresModelo(titulo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public void AlterarSituacaoModeloTitulo(TituloModelo modelo)
		{
			try
			{
				if (modelo.SituacaoId == 2 &&  (!_validar.PossuiConfiguracaoAtividade(modelo)))
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();
					_da.AlterarSituacao(modelo);
				}

				if (modelo.SituacaoId == 1)
				{
					Validacao.Add(Mensagem.TituloModelo.AtivarModelo);
				}
				else
				{
					Validacao.Add(Mensagem.TituloModelo.DesativarTituloModelo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public string ObterUltimoNumeroGerado(int id, bool reiniciaPorAno = false)
		{
			return _da.ObterUltimoNumeroGerado(id);
		}

		public void VerificarPublicoExternoAtividade(int id)
		{
			_validar.VerificarPublicoExternoAtividade(id);
		}

		public List<TituloModeloLst> ObterModelosAnteriores(int modelo)
		{
			try
			{
				return _da.ObterModelosAnteriores(modelo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<TituloModeloLst> ObterModelosRenovacao(int modelo)
		{
			return _da.ObterModelosRenovacao(modelo);
		}
	}
}
