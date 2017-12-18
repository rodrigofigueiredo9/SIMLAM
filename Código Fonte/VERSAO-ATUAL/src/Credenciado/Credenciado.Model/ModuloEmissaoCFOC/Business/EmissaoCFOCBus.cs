using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business
{
	public class EmissaoCFOCBus
	{
		#region Propriedades

		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private EmissaoCFOCDa _da = new EmissaoCFOCDa();
		private EmissaoCFOCValidar _validar = new EmissaoCFOCValidar();

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

		public bool Salvar(EmissaoCFOC entidade)
		{
			try
			{
				if (entidade.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital)
				{
					if (entidade.Id <= 0)
					{
						if (_validar.VerificarNumeroDigitalDisponivel())
						{
							entidade.Numero = ObterNumeroDigital();

                            if (entidade.Numero.IndexOf("/") >= 0)
                            {
                                string[] tmpNum = entidade.Numero.Split('/');
                                entidade.Numero = tmpNum[0];
                                entidade.Serie = tmpNum[1];
                            }
						}
						else
						{
							return false;
						}
					}
					else
					{
						EmissaoCFOC entidadeBanco = Obter(entidade.Id, simplificado: true);
						entidade.Numero = entidadeBanco.Numero;

                        if (entidade.Numero.IndexOf("/") >= 0)
                        {
                            string[] tmpNum = entidade.Numero.Split('/');
                            entidade.Numero = tmpNum[0];
                            entidade.Serie = tmpNum[1];
                        }
					}
				}

				CulturaInternoBus culturaBus = new CulturaInternoBus();


				List<Cultivar> cultivares = culturaBus.ObterCultivares(entidade.Produtos.Select(x => x.CulturaId).ToList(), entidade.Produtos.Select(y => y.LoteId).ToList()) ?? new List<Cultivar>();

				var declaracoesAdicionais = cultivares
					.Where(x => entidade.Produtos.Select(y => y.CultivarId).ToList().Any(y => y == x.Id))
					.SelectMany(x => x.LsCultivarConfiguracao.Where(y => entidade.Produtos.Count(z => z.CultivarId == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)) > 0))
					.Where(x => entidade.Pragas.Any(y => y.Id == x.PragaId))
					.Select(x => x.DeclaracaoAdicionalTexto)
					.Distinct().ToList();

				var declaracoesAdicionaisHtml = cultivares
					.Where(x => entidade.Produtos.Select(y => y.CultivarId).ToList().Any(y => y == x.Id))
					.SelectMany(x => x.LsCultivarConfiguracao.Where(y => entidade.Produtos.Count(z => z.CultivarId == y.Cultivar && y.TipoProducaoId == (int)ValidacoesGenericasBus.ObterTipoProducao(z.UnidadeMedidaId)) > 0))
					.Where(x => entidade.Pragas.Any(y => y.Id == x.PragaId))
					.Select(x => x.DeclaracaoAdicionalTextoHtml)
					.Distinct().ToList();

				entidade.DeclaracaoAdicional = String.Join(" ", declaracoesAdicionais);
				entidade.DeclaracaoAdicionalHtml = String.Join(" ", declaracoesAdicionaisHtml);

				_validar.Salvar(entidade);

				if (!Validacao.EhValido)
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(entidade, bancoDeDados);

					CFOCFOCInternoBus CFOCFOCInternoBus = new CFOCFOCInternoBus();
					CFOCFOCInternoBus.SetarNumeroUtilizado(entidade.Numero, entidade.TipoNumero.GetValueOrDefault(), eDocumentoFitossanitarioTipo.CFOC, entidade.Serie);

                    if (string.IsNullOrEmpty(entidade.Serie))
					    Validacao.Add(Mensagem.EmissaoCFOC.Salvar(entidade.Numero));
                    else
                        Validacao.Add(Mensagem.EmissaoCFOC.Salvar(entidade.Numero + "/" + entidade.Serie));

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

					Validacao.Add(Mensagem.EmissaoCFOC.ExcluidoSucesso);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Ativar(EmissaoCFOC entidade)
		{
			try
			{
				EmissaoCFOC entidadeBanco = Obter(entidade.Id);
				entidadeBanco.DataAtivacao = entidade.DataAtivacao;

				if (!_validar.Ativar(entidadeBanco))
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

                    //LoteBus loteBus = new LoteBus();
                    //foreach (var item in entidadeBanco.Produtos)
                    //{
                    //    loteBus.AlterarSituacaoLote(item.LoteId, eLoteSituacao.Utilizado, bancoDeDados);
                    //}

					_da.Ativar(entidadeBanco, bancoDeDados);

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return false;
					}

					Validacao.Add(Mensagem.EmissaoCFOC.AtivadoSucesso(entidade.Numero));

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Cancelar(EmissaoCFOC entidade)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				EmissaoCFOC entidadeBanco = _da.ObterPorNumero(Convert.ToInt64(entidade.Numero), entidade.Serie, false, false, bancoDeDados);

                if (entidade.Id != 0)
                {
                    List<int> lotesID = entidadeBanco.Produtos.Select(x => x.LoteId).ToList();

                    //Dessassocio os Lotes
                    entidadeBanco.Produtos.Clear();
                    _da.Salvar(entidadeBanco, bancoDeDados);

                    LoteBus loteBus = new LoteBus();
                    foreach (var item in lotesID)
                    {
                        loteBus.AlterarSituacaoLote(item, eLoteSituacao.NaoUtilizado, bancoDeDados);
                    }
                }

				_da.Cancelar(entidadeBanco, bancoDeDados);

				if(!Validacao.EhValido)
				{
					bancoDeDados.Rollback();
					return false;
				}

				bancoDeDados.Commit();
			}

			return Validacao.EhValido;
		}

		#endregion Ações DML

		#region Obter/Filtrar

		public EmissaoCFOC Obter(int id, bool simplificado = false)
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

		public Resultados<EmissaoCFOC> Filtrar(EmissaoCFOC filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<EmissaoCFOC> filtro = new Filtro<EmissaoCFOC>(filtrosListar, paginacao);
				Resultados<EmissaoCFOC> resultados = _da.Filtrar(filtro);

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

        public EmissaoCFOC ObterPorNumero(long numero, bool simplificado = false, bool credenciado = true, string serieNumero = "")
		{
			try
			{
				return _da.ObterPorNumero(numero, serieNumero, simplificado, credenciado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> ObterEmpreendimentosLista(int credenciadoID = 0)
		{
			try
			{
				if (credenciadoID == 0)
				{
					credenciadoID = User.FuncionarioId;
				}

				return _da.ObterEmpreendimentosLista(credenciadoID);
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

        public string VerificarNumero(string numero, int tipoNumero, string serieNumero = "")
		{
			try
			{
				if (tipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Digital && tipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					Validacao.Add(Mensagem.EmissaoCFOC.TipoNumeroObrigatorio);
					return numero;
				}

				if (tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					_validar.ValidarNumeroBloco(numero.ToString());

					if (Validacao.EhValido)
					{
						Validacao.Add(Mensagem.EmissaoCFOC.NumeroValido);
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

		#endregion Verificações
	}
}