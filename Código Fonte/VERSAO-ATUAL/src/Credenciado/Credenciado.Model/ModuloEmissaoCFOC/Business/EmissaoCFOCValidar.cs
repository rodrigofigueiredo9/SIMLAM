using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business
{
	public class EmissaoCFOCValidar
	{
		#region Propriedades

		private EmissaoCFOCDa _da = new EmissaoCFOCDa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		internal bool Salvar(EmissaoCFOC entidade)
		{
			//Valida a habilitacao antes do salvar o credenciado.

			if (!entidade.TipoNumero.HasValue)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.TipoNumeroObrigatorio);
			}
			else
			{
				if (entidade.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					if (!ValidarNumeroBloco(entidade.Numero, entidade.Id))
					{
						return false;
					}

					ValidacoesGenericasBus.DataMensagem(entidade.DataEmissao, "CFOC_DataEmissao", "emissão");
				}

				if (entidade.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital)
				{
					//VerificarNumeroDigitalDisponivel foi validado na BUS
					if (!_da.NumeroCancelado(entidade.Numero))
					{
						Validacao.Add(Mensagem.EmissaoCFO.NumeroCancelado);
						return false;
					}
				}
			}

			if (entidade.EmpreendimentoId <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.EmpreendimentoObrigatorio);
			}
			else
			{
				ResponsavelTecnico responsavel = _da.ObterResponsavelUC(entidade.EmpreendimentoId);
				if (responsavel.Id <= 0)
				{
					Validacao.Add(Mensagem.EmissaoCFOC.ResponsavelDessassociadoUC);
				}
				else
				{
					if (!string.IsNullOrEmpty(responsavel.DataValidadeART) && DateTime.Parse(responsavel.DataValidadeART) < DateTime.Today)
					{
						Validacao.Add(Mensagem.EmissaoCFOC.DataValidadeARTMenorAtual);
					}
				}
			}

			if (entidade.Produtos.Count <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ProdutoObrigatorio);
			}
			else
			{
				if (entidade.Produtos.Count > 5)
				{
					Validacao.Add(Mensagem.EmissaoCFOC.LimiteMaximo);
				}
				else
				{
					entidade.Produtos.ForEach(produto =>
					{
						ValidarProduto(entidade.Id, entidade.EmpreendimentoId, produto);
					});
				}
			}

			if (entidade.Pragas.Count <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.PragaAssociadaCulturaObrigatorio);
			}
			else
			{
				entidade.Pragas.ForEach(praga =>
				{
					ValidarPraga(praga);
				});
			}

			if (entidade.ProdutoEspecificacao <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ProdutoEspecificacaoObrigatorio);
			}

			if (entidade.PartidaLacradaOrigem && string.IsNullOrEmpty(entidade.NumeroLacre) && string.IsNullOrEmpty(entidade.NumeroPorao) && string.IsNullOrEmpty(entidade.NumeroContainer))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.LacrePoraoConteinerObrigatorio);
			}

			if (entidade.ValidadeCertificado <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ValidadeCertificadoObrigatorio);
			}
			else if (entidade.ValidadeCertificado > 30)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ValidadeCertificadoMaxima);
			}

			if (entidade.MunicipioEmissaoId <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.MunicipioEmissaoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool ValidarNumeroBloco(string numero, int CFOCId = 0)
		{
			if (string.IsNullOrEmpty(numero))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.NumeroObrigatorio);
				return false;
			}

			if (numero.Length != 10)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.NumeroInvalido);
				return false;
			}

			if (_da.NumeroJaExiste(numero, CFOCId))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.NumeroExistente);
				return false;
			}

			if (!_da.NumeroLiberado(numero))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.NumeroNaoLiberado);
				return false;
			}

			if (!_da.NumeroCancelado(numero))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.NumeroCancelado);
			}

			return Validacao.EhValido;
		}

		public void ValidarProduto(int cfoc, int empreendimento, IdentificacaoProduto item, List<IdentificacaoProduto> lista = null)
		{
			lista = lista ?? new List<IdentificacaoProduto>();

			if (item.LoteId <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.LoteObrigatorio);
				return;
			}

			if(_da.LotePossuiOrigemCancelada(item.LoteId))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.LotePossuiOrigemCancelada);
				return;
			}

			string aux = _da.LoteUtilizado(item.LoteId, cfoc);
			if (!string.IsNullOrEmpty(aux))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.LoteUtilizado(item.LoteCodigo, aux));
				return;
			}

			TituloInternoBus tituloBus = new TituloInternoBus();
			if (!tituloBus.UnidadeConsolidacaoPossuiAberturaConcluido(empreendimento, item.CulturaId))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.UCTituloConcluido);
			}

			Cultivar cultivar = _da.CultivarAssociadaUC(empreendimento, item.CultivarId);
			if (cultivar != null && cultivar.Id <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.CultivarAssociadoEmpreendimento(item.CultivarTexto));
				return;
			}

			var somaQuantidadeCFOC = _da.ObterCapacidadeMes(cfoc, empreendimento, item.CultivarId);
			var somaQuantidade = lista.Where(x => !x.Equals(item) && x.CultivarId == item.CultivarId).Sum(x => x.Quantidade);

			if (cultivar.CapacidadeMes < somaQuantidadeCFOC + item.Quantidade + somaQuantidade)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.QuantidadeMensalInvalida);
				return;
			}

			if (lista.Count(x => !x.Equals(item)) >= 5)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.LimiteMaximo);
			}

			if (lista.Any(x => x.LoteId == item.LoteId))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.LoteJaAdicionado);
			}
		}

		public void ValidarPraga(Praga item, List<Praga> lista = null)
		{
			if (item.Id <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.PragaObrigatorio);
			}

			if (lista != null)
			{
				if (lista.Any(x => x.Id == item.Id))
				{
					Validacao.Add(Mensagem.EmissaoCFOC.PragaJaAdicionada);
				}
			}
		}

		public void ValidarTratamento(TratamentoFitossanitario item, List<TratamentoFitossanitario> lista = null)
		{
			if (string.IsNullOrWhiteSpace(item.ProdutoComercial))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.TratamentoNomeProdutoComercialObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(item.IngredienteAtivo))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.TratamentoIngredienteAtivoObrigatorio);
			}

			if (item.Dose <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.TratamentoDoseObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(item.PragaProduto))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.TratamentoPragaProdutoObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(item.ModoAplicacao))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.TratamentoModoAplicacao);
			}

			if (lista != null)
			{
				if (lista.Count >= 5)
				{
					Validacao.Add(Mensagem.EmissaoCFOC.LimiteMaximo);
				}
			}
		}

		internal bool Excluir(int id)
		{
			EmissaoCFOC entidade = _da.Obter(id, true);

			if (entidade.SituacaoId != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ExcluirSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		internal bool Ativar(EmissaoCFOC entidade)
		{
			ValidacoesGenericasBus.DataMensagem(entidade.DataAtivacao, "DataAtivacao", "ativação");

			if (entidade.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital && entidade.DataAtivacao.DataTexto != DateTime.Today.ToShortDateString())
			{
				Validacao.Add(Mensagem.Padrao.DataIgualAtual("DataAtivacao", "ativação"));
			}

			if (entidade.SituacaoId != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.AtivarSituacaoInvalida);
			}

			Salvar(entidade);

			return Validacao.EhValido;
		}

		public bool Editar(EmissaoCFOC entidade)
		{
			if (entidade.SituacaoId != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.EditarSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		public bool VerificarCredenciadoHabilitado()
		{
			HabilitarEmissaoCFOCFOCBus habilitarEmissaoCFOCFOCBus = new HabilitarEmissaoCFOCFOCBus();

			if (!habilitarEmissaoCFOCFOCBus.VerificarCredenciadoHabilitado())
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ResponsavelTecnicoNaoHabilitado);
			}

			return Validacao.EhValido;
		}

		public bool VerificarNumeroDigitalDisponivel()
		{
			if (!_da.NumeroDigitalDisponivel())
			{
				Validacao.Add(Mensagem.EmissaoCFOC.NumeroDigitalIndisponivel);
			}

			return Validacao.EhValido;
		}

		public bool ValidarAssociarResponsavelTecnicoHabilitado(EmissaoCFOC entidade)
		{
			#region Configurar

			List<Cultivar> cultivares = new List<Cultivar>();
			foreach (var item in entidade.Produtos)
			{
				cultivares.Add(new Cultivar() { CulturaId = item.CulturaId, Id = item.CultivarId });
			}

			if (cultivares.Count <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ProdutoObrigatorio);
				return false;
			}

			HabilitarEmissaoCFOCFOCBus habilitarEmissaoCFOCFOCBus = new HabilitarEmissaoCFOCFOCBus();
			HabilitarEmissaoCFOCFOC habilitacao = new HabilitarEmissaoCFOCFOCBus().ObterPorCredenciado(User.FuncionarioId);
			if (Validacao.EhValido)
			{
				PragaInternoBus pragaBus = new PragaInternoBus();
				habilitacao.Pragas.ForEach(x =>
				{
					x.Praga.Culturas = pragaBus.ObterCulturas(x.Praga.Id);
				});
			}
			else
			{
				return false;
			}

			#endregion

			if (habilitacao.Id <= 0 || habilitacao.Situacao == (int)eHabilitacaoCFOCFOCSituacao.Inativo)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ResponsavelTecnicoNaoHabilitado);
				return false;
			}

			if (!habilitacao.Pragas.SelectMany(p => p.Praga.Culturas).ToList().Exists(c => cultivares.Exists(x => x.CulturaId == c.Id)))
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ResponsavelTecnicoNaoHabilitadoParaCultura);
				return false;
			}

			if (DateTime.Parse(habilitacao.ValidadeRegistro) < DateTime.Today)
			{
				Validacao.Add(Mensagem.EmissaoCFOC.ValidadeRegistroMenorAtual);
				return false;
			}

			foreach (var item in entidade.Pragas)
			{
				if (!habilitacao.Pragas.Exists(y => y.Praga.Id == item.Id))
				{
					Validacao.Add(Mensagem.EmissaoCFOC.PragaNaoAssociadaHabilitacao(item.NomeCientifico, item.NomeComum));
					return false;
				}
			}

			List<string> aux = new List<string>();
			foreach (var habPraga in habilitacao.Pragas)
			{
				foreach (var cultivarItem in habPraga.Praga.Culturas.SelectMany(x => x.LstCultivar))
				{
					foreach (var item in cultivares)
					{
						if (cultivarItem.Id == item.Id && DateTime.Parse(habPraga.DataFinalHabilitacao) < DateTime.Today)
						{
							if (!aux.Any(a => a == cultivarItem.Nome))
							{
								aux.Add(cultivarItem.Nome);
							}
						}
					}
				}
			}

			if (aux.Count > 0)
			{
				if ((eDocumentoFitossanitarioTipoNumero)entidade.TipoNumero == eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					Validacao.Add(Mensagem.EmissaoCFOC.ResponsavelTecnicoHabilitacaoPragaVencidaBloco(Mensagem.Concatenar(aux)));
				}
				else
				{
					Validacao.Add(Mensagem.EmissaoCFOC.ResponsavelTecnicoHabilitacaoPragaVencidaDigital(Mensagem.Concatenar(aux)));
				}
			}

			return Validacao.EhValido;
		}
	}
}