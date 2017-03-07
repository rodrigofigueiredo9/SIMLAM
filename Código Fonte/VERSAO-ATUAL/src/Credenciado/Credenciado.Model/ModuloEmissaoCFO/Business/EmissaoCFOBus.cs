using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business
{
	public class EmissaoCFOBus
	{
		#region Propriedades

		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private EmissaoCFODa _da = new EmissaoCFODa();
		private EmissaoCFOValidar _validar = new EmissaoCFOValidar();
		private CFOCFOCInternoBus _busInterno = new CFOCFOCInternoBus();

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações DML

		public bool Salvar(EmissaoCFO cfo)
		{
			try
			{
				if (cfo.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital)
				{
					if (cfo.Id <= 0)
					{
						if (_validar.VerificarNumeroDigitalDisponivel())
						{
							cfo.Numero = ObterNumeroDigital();
						}
						else
						{
							return false;
						}
					}
					else
					{
						EmissaoCFO entidadeBanco = Obter(cfo.Id, simplificado: true);
						cfo.Numero = entidadeBanco.Numero;
					}
				}

				CulturaInternoBus culturaBus = new CulturaInternoBus();
				List<Cultivar> cultivares = culturaBus.ObterCultivares(cfo.Produtos.Select(x => x.CulturaId).ToList(), cfo.Produtos.Select(z => z.LoteId).ToList()) ?? new List<Cultivar>();

				var declaracoesAdicionais = cultivares
					.Where(x => cfo.Produtos.Any(y => y.CultivarId == x.Id))
					.SelectMany(x => x.LsCultivarConfiguracao.Where(y => cfo.Produtos.Count(z => z.CultivarId == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)) > 0))
					.Where(x => cfo.Pragas.Any(y => y.Id == x.PragaId))
					.Select(x => x.DeclaracaoAdicionalTexto)
					.Distinct().ToList();

				var declaracoesAdicionaisHtml = cultivares
					.Where(x => cfo.Produtos.Any(y => y.CultivarId == x.Id))
					.SelectMany(x => x.LsCultivarConfiguracao.Where(y => cfo.Produtos.Count(z => z.CultivarId == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)) > 0))
					.Where(x => cfo.Pragas.Any(y => y.Id == x.PragaId))
					.Select(x => x.DeclaracaoAdicionalTextoHtml)
					.Distinct().ToList();

				cfo.DeclaracaoAdicional = String.Join(" ", declaracoesAdicionais);
				cfo.DeclaracaoAdicionalHtml = String.Join(" ", declaracoesAdicionaisHtml);

				_validar.Salvar(cfo);

				if (!Validacao.EhValido)
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(cfo, bancoDeDados);

					_busInterno.SetarNumeroUtilizado(cfo.Numero, cfo.TipoNumero.GetValueOrDefault(), eDocumentoFitossanitarioTipo.CFO);

					Validacao.Add(Mensagem.EmissaoCFO.Salvar(cfo.Numero));

					bancoDeDados.Commit();
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			try
			{
				if (!_validar.Excluir(id))
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.EmissaoCFO.ExcluidoSucesso);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Ativar(EmissaoCFO cfo)
		{
			try
			{
				if (!_validar.Ativar(cfo))
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Ativar(cfo, bancoDeDados);

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return false;
					}

					Validacao.Add(Mensagem.EmissaoCFO.AtivadoSucesso(cfo.Numero));

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Cancelar(EmissaoCFO entidade)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				EmissaoCFO entidadeBanco = _da.ObterPorNumero(Convert.ToInt64(entidade.Numero), true, false, bancoDeDados);
				_da.Cancelar(entidadeBanco, bancoDeDados);

				bancoDeDados.Commit();
			}

			return Validacao.EhValido;
		}

		#endregion Ações DML

		#region Obter/Filtrar

		public EmissaoCFO Obter(int id, bool simplificado = false)
		{
			try
			{
				return _da.Obter(id, simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<EmissaoCFO> Filtrar(EmissaoCFO filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<EmissaoCFO> filtro = new Filtro<EmissaoCFO>(filtrosListar, paginacao);
				Resultados<EmissaoCFO> resultados = _da.Filtrar(filtro);

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

		public EmissaoCFO ObterPorNumero(long numero, bool simplificado = false, bool credenciado = true)
		{
			try
			{
				return _da.ObterPorNumero(numero, simplificado, credenciado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> ObterProdutoresLista(int credenciadoID = 0)
		{
			try
			{
				if (credenciadoID == 0)
				{
					credenciadoID = User.FuncionarioId;
				}

				return _da.ObterProdutoresLista(credenciadoID);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}


        public List<Lista> ObterEmpreendimentosListaEtramiteX(int produtorID)
        {
            try
            {
                return _da.ObterEmpreendimentosListaEtramiteX(produtorID);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
            return null;
        }

		public List<Lista> ObterEmpreendimentosLista(int produtorID)
		{
			try
			{
				return _da.ObterEmpreendimentosLista(produtorID, User.FuncionarioId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<Lista> ObterUnidadesProducaoLista(int empreendimentoID, int produtorID)
		{
			try
			{
				return _da.ObterUnidadesProducaoLista(empreendimentoID, produtorID, User.FuncionarioId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

       

		public Cultivar ObterCulturaUP(int unidadeProducaoId)
		{
			try
			{
				return _da.ObterCulturaUP(unidadeProducaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<Lista> ObterPragasLista(List<IdentificacaoProduto> produtos)
		{
			try
			{
				return _da.ObterPragasLista(produtos);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		private string ObterNumeroDigital()
		{
			try
			{
				return _da.ObterNumeroDigital();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return string.Empty;
		}

		#endregion Obter/Filtrar

		#region Verificações

		public string VerificarNumero(string numero, int tipoNumero)
		{
			try
			{

				if (tipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Digital && tipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					Validacao.Add(Mensagem.EmissaoCFO.TipoNumeroObrigatorio);
					return numero;
				}

				if (tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					_validar.ValidarNumeroBloco(numero.ToString());

					if (Validacao.EhValido)
					{
						Validacao.Add(Mensagem.EmissaoCFO.NumeroValido);
					}
				}

				if (tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital)
				{
					if (_validar.VerificarNumeroDigitalDisponivel())
					{
						numero = ObterNumeroDigital();
					}
				}

               
				return numero;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return "";
		}

		public bool NumeroJaExiste(string numero, int id = 0)
		{
			try
			{
				return _da.NumeroJaExiste(numero, id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		#endregion Verificações
	}
}