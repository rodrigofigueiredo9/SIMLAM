using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business
{
	public class EmissaoCFOValidar
	{
		#region Propriedades

		private EmissaoCFODa _da = new EmissaoCFODa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		internal bool Salvar(EmissaoCFO entidade)
		{
			//Valida a habilitacao antes do salvar o credenciado.

			if (!entidade.TipoNumero.HasValue)
			{
				Validacao.Add(Mensagem.EmissaoCFO.TipoNumeroObrigatorio);
			}
			else
			{
				if (entidade.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					if (!ValidarNumeroBloco(entidade.Numero, entidade.Id))
					{
						return false;
					}

					ValidacoesGenericasBus.DataMensagem(entidade.DataEmissao, "CFO_DataEmissao", "emissão");
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

			if (!_da.VerificarProdutorAssociado(entidade.ProdutorId))
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutorNaoEstaMaisAssociado);
			}

			if (!_da.VerificarProdutorAssociadoEmpreendimento(entidade.ProdutorId))
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutorNaoEstaMaisAssociadoEmpreendimento);
			}

			if (entidade.ProdutorId <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutorObrigatorio);
			}

			if (entidade.EmpreendimentoId <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.EmpreendimentoObrigatorio);
			}

			if (entidade.Produtos.Count <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutoObrigatorio);
			}
			else
			{
				if (entidade.Produtos.Count > 5)
				{
					Validacao.Add(Mensagem.EmissaoCFO.LimiteMaximo);
				}
				else
				{
					entidade.Produtos.ForEach(produto =>
					{
						ResponsavelTecnico responsavel = _da.ObterResponsavelUC(produto.UnidadeProducao);
						if (!string.IsNullOrEmpty(responsavel.DataValidadeART) && DateTime.Parse(responsavel.DataValidadeART) < DateTime.Today)
						{
							Validacao.Add(Mensagem.EmissaoCFO.DataValidadeARTMenorAtual);
						}

						ValidarProduto(entidade.Id, entidade.EmpreendimentoId, produto, entidade.Produtos);
					});
				}
			}

			if (entidade.Pragas.Count <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.PragaAssociadaCulturaObrigatorio);
			}
			else
			{
				entidade.Pragas.ForEach(praga =>
				{
					ValidarPraga(praga);
				});
			}

            //if (!entidade.PossuiLaudoLaboratorial.HasValue)
            //{
            //    Validacao.Add(Mensagem.EmissaoCFO.PossuiLaudoLaboratorialObrigatorio);
            //}
            //else
            //{
            //    if (entidade.PossuiLaudoLaboratorial.Value)
            //    {
            //        if (string.IsNullOrEmpty(entidade.NomeLaboratorio))
            //        {
            //            Validacao.Add(Mensagem.EmissaoCFO.NomeLaboratorioObrigatorio);
            //        }

            //        if (string.IsNullOrEmpty(entidade.NumeroLaudoResultadoAnalise))
            //        {
            //            Validacao.Add(Mensagem.EmissaoCFO.NumeroLaudoResultadoAnaliseObrigatorio);
            //        }

            //        if (entidade.EstadoId <= 0)
            //        {
            //            Validacao.Add(Mensagem.EmissaoCFO.EstadoObrigatorio);
            //        }

            //        if (entidade.MunicipioId <= 0)
            //        {
            //            Validacao.Add(Mensagem.EmissaoCFO.MunicipioObrigatorio);
            //        }
            //    }
            //}

			if (entidade.ProdutoEspecificacao <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutoEspecificacaoObrigatorio);
			}

            //if (!entidade.PossuiTratamentoFinsQuarentenario.HasValue)
            //{
            //    Validacao.Add(Mensagem.EmissaoCFO.PossuiTratamentoFinsQuarentenarioObrigatorio);
            //}
            //else
            //{
            //    if (entidade.PossuiTratamentoFinsQuarentenario.Value)
            //    {
            //        entidade.TratamentosFitossanitarios.ForEach(tratamento =>
            //        {
            //            ValidarTratamento(tratamento);
            //        });
            //    }
            //}

			if (entidade.PartidaLacradaOrigem && string.IsNullOrEmpty(entidade.NumeroLacre) && string.IsNullOrEmpty(entidade.NumeroPorao) && string.IsNullOrEmpty(entidade.NumeroContainer))
			{
				Validacao.Add(Mensagem.EmissaoCFO.LacrePoraoConteinerObrigatorio);
			}

			if (entidade.ValidadeCertificado <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ValidadeCertificadoObrigatorio);
			}
			else if (entidade.ValidadeCertificado > 30)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ValidadeCertificadoMaxima);
			}

			if (entidade.MunicipioEmissaoId <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.MunicipioEmissaoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool ValidarNumeroBloco(string numero, int CFOId = 0)
		{
			if (string.IsNullOrEmpty(numero))
			{
				Validacao.Add(Mensagem.EmissaoCFO.NumeroObrigatorio);
				return false;
			}

			if (numero.Length != 8)
			{
				Validacao.Add(Mensagem.EmissaoCFO.NumeroInvalido);
				return false;
			}

			if (_da.NumeroJaExiste(numero, CFOId))
			{
				Validacao.Add(Mensagem.EmissaoCFO.NumeroExistente);
				return false;
			}

			if (!_da.NumeroLiberado(numero))
			{
				Validacao.Add(Mensagem.EmissaoCFO.NumeroNaoLiberado);
				return false;
			}

			if (!_da.NumeroCancelado(numero))
			{
				Validacao.Add(Mensagem.EmissaoCFO.NumeroCancelado);
			}

            if (numero.Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2))
            {
                Validacao.Add(Mensagem.EmissaoCFO.AnoCFOInvalido);
            }

			return Validacao.EhValido;
		}

		public void ValidarProduto(int cfo, int empreendimento, IdentificacaoProduto item, List<IdentificacaoProduto> lista)
		{
			lista = lista ?? new List<IdentificacaoProduto>();

			if (item.UnidadeProducao <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutoUnidadeProducaoObrigatorio);
				return;
			}

			TituloInternoBus tituloBus = new TituloInternoBus();
			Titulo titulo = tituloBus.UnidadeProducaoPossuiAberturaConcluido(item.UnidadeProducao);
			if (titulo == null || titulo.Id <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.UPTituloConcluido);
			}

			if (item.Quantidade <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutoQuantidadeObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(item.DataInicioColheita, "CFO_Produto_InicioColheita", "início da colheita", false);

			ValidacoesGenericasBus.DataMensagem(item.DataFimColheita, "CFO_Produto_FimColheita", "fim da colheita", false);

			if(!Validacao.EhValido)
			{
				return;
			}

			if (DateTime.Parse(item.DataFimColheita.DataTexto) < DateTime.Parse(item.DataInicioColheita.DataTexto))
			{
				Validacao.Add(Mensagem.EmissaoCFO.DataFimColheitaNaoPodeSerMenorQueDataInicial);
			}

			TituloInternoDa tituloInternoDa = new TituloInternoDa();
			var dependencia = tituloInternoDa.ObterDependencia(titulo.Id, eCaracterizacao.UnidadeProducao);
			UnidadeProducao UnidadeProducao = _da.ObterUnidadeProducao(dependencia.Id, dependencia.DependenciaTid);

			UnidadeProducaoItem unidade = UnidadeProducao.UnidadesProducao.FirstOrDefault(x => x.Id == item.UnidadeProducao);
			if (unidade == null || unidade.Id <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.UPDessassociada(item.CodigoUP));
			}
			else
			{
				if (unidade.TipoProducao == (int)eUnidadeProducaoTipoProducao.MaterialPropagacao)
				{
					if (string.IsNullOrEmpty(unidade.DataValidadeRenasem) || DateTime.Parse(unidade.DataValidadeRenasem) < DateTime.Today)
					{
						Validacao.Add(Mensagem.EmissaoCFO.DataValidadeRENASEMMenorAtual(item.CodigoUP));
					}
				}

				eUnidadeProducaoTipoProducao tipoProducao = ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedidaId);

				DateTime dataSaldo = titulo.DataSituacao.Data.GetValueOrDefault();
                DateTime dataValidade = dataSaldo.AddYears(1); 
				//if(titulo.DataSituacao.Data.GetValueOrDefault().Year < DateTime.Today.Year)
                /*
                 * Cálculo do saldo do CFO acrescenta a diferença de datas
                 */
                while (dataValidade < DateTime.Today)
                {
                    dataSaldo = dataValidade;
                    dataValidade = dataValidade.AddYears(1);
                }


               

                //Converte todas as quantidades para tonelada, para calcular o saldo
                var quantidadeItem = item.ExibeQtdKg ? item.Quantidade / 1000 : item.Quantidade;
                var listaReduzida = lista.Where(x => !x.Equals(item)
                                                     && x.CultivarId == item.CultivarId
                                                     && x.UnidadeMedidaId == item.UnidadeMedidaId
                                                     && x.UnidadeProducao == item.UnidadeProducao).ToList();
                foreach (var itemLista in listaReduzida)
                {
                    if (itemLista.ExibeQtdKg)
                    {
                        itemLista.Quantidade = itemLista.Quantidade / 1000;
                    }
                }

				decimal totalTela = quantidadeItem + listaReduzida.Sum(x => x.Quantidade);
				if (unidade.EstimativaProducaoQuantidadeAno < _da.ObterQuantidadeProduto(empreendimento, item.CultivarId, tipoProducao, item.UnidadeProducao, cfo, dataSaldo) + totalTela)
				{
					Validacao.Add(Mensagem.EmissaoCFO.QuantidadeMensalInvalida(unidade.CodigoUP.ToString()));
				}
			}
             
			if (lista.Count(x => !x.Equals(item)) >= 5)
			{
				Validacao.Add(Mensagem.EmissaoCFO.LimiteMaximo);
			}

			if (lista.Any(x => x.UnidadeProducao == item.UnidadeProducao && !x.Equals(item)))
			{
				Validacao.Add(Mensagem.EmissaoCFO.UnidadeProducaoJaAdicionado);
			}
		}

		public void ValidarPraga(Praga item, List<Praga> lista = null)
		{
			if (item.Id <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.PragaObrigatorio);
			}

			if (lista != null)
			{
				if (lista.Any(x => x.Id == item.Id))
				{
					Validacao.Add(Mensagem.EmissaoCFO.PragaJaAdicionada);
				}
			}
		}

		public void ValidarTratamento(TratamentoFitossanitario item, List<TratamentoFitossanitario> lista = null)
		{
			if (string.IsNullOrWhiteSpace(item.ProdutoComercial))
			{
				Validacao.Add(Mensagem.EmissaoCFO.TratamentoNomeProdutoComercialObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(item.IngredienteAtivo))
			{
				Validacao.Add(Mensagem.EmissaoCFO.TratamentoIngredienteAtivoObrigatorio);
			}

			if (item.Dose <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.TratamentoDoseObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(item.PragaProduto))
			{
				Validacao.Add(Mensagem.EmissaoCFO.TratamentoPragaProdutoObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(item.ModoAplicacao))
			{
				Validacao.Add(Mensagem.EmissaoCFO.TratamentoModoAplicacao);
			}

			if (lista != null)
			{
				if (lista.Count >= 5)
				{
					Validacao.Add(Mensagem.EmissaoCFO.LimiteMaximo);
				}
			}
		}

		internal bool Excluir(int id)
		{
			EmissaoCFO cfo = _da.Obter(id, true);

			if (cfo.SituacaoId != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ExcluirSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		internal bool Ativar(EmissaoCFO entidade)
		{
			EmissaoCFO entidadeBanco = _da.Obter(entidade.Id);

			ValidacoesGenericasBus.DataMensagem(entidade.DataAtivacao, "DataAtivacao", "ativação");

			if (entidadeBanco.TipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital && entidade.DataAtivacao.DataTexto != DateTime.Today.ToShortDateString())
			{
				Validacao.Add(Mensagem.Padrao.DataIgualAtual("DataAtivacao", "ativação"));
			}

			if (entidadeBanco.SituacaoId != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.EmissaoCFO.AtivarSituacaoInvalida);
			}

			Salvar(entidadeBanco);

			return Validacao.EhValido;
		}

		public bool Editar(EmissaoCFO entidade)
		{
			if (entidade.SituacaoId != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.EmissaoCFO.EditarSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		public bool VerificarCredenciadoHabilitado()
		{
			HabilitarEmissaoCFOCFOCBus habilitarEmissaoCFOCFOCBus = new HabilitarEmissaoCFOCFOCBus();

			if (!habilitarEmissaoCFOCFOCBus.VerificarCredenciadoHabilitado())
			{
				Validacao.Add(Mensagem.EmissaoCFO.ResponsavelTecnicoNaoHabilitado);
			}

			return Validacao.EhValido;
		}

		public bool VerificarNumeroDigitalDisponivel()
		{
			if (!_da.NumeroDigitalDisponivel())
			{
				Validacao.Add(Mensagem.EmissaoCFO.NumeroDigitalIndisponivel);
			}

			return Validacao.EhValido;
		}

		public bool ValidarAssociarResponsavelTecnicoHabilitado(EmissaoCFO entidade)
		{
			#region Configurar

			List<Cultivar> cultivares = new List<Cultivar>();
			foreach (var item in entidade.Produtos)
			{
				cultivares.Add(new Cultivar() { CulturaId = item.CulturaId, Id = item.CultivarId });
			}

			if (cultivares.Count <= 0)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ProdutoObrigatorio);
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

			if (habilitacao.Id <= 0 )
			{
				Validacao.Add(Mensagem.EmissaoCFO.ResponsavelTecnicoNaoHabilitado);
				return false;
			}

			if (habilitacao.Situacao == (int)eHabilitacaoCFOCFOCSituacao.Inativo)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ResponsavelTecnicoInativo);
				return false;
			}

			if (!habilitacao.Pragas.SelectMany(p => p.Praga.Culturas).ToList().Exists(c => cultivares.Exists(x => x.CulturaId == c.Id)))
			{
				Validacao.Add(Mensagem.EmissaoCFO.ResponsavelTecnicoNaoHabilitadoParaCultura);
				return false;
			}

			if (DateTime.Parse(habilitacao.ValidadeRegistro) < DateTime.Today)
			{
				Validacao.Add(Mensagem.EmissaoCFO.ValidadeRegistroMenorAtual);
				return false;
			}

			foreach (var item in entidade.Pragas)
			{
				if (!habilitacao.Pragas.Exists(y => y.Praga.Id == item.Id))
				{
					Validacao.Add(Mensagem.EmissaoCFO.PragaNaoAssociadaHabilitacao(item.NomeCientifico, item.NomeComum));
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
					Validacao.Add(Mensagem.EmissaoCFO.ResponsavelTecnicoHabilitacaoPragaVencidaBloco(Mensagem.Concatenar(aux)));
				}
				else
				{
					Validacao.Add(Mensagem.EmissaoCFO.ResponsavelTecnicoHabilitacaoPragaVencidaDigital(Mensagem.Concatenar(aux)));
				}
			}

			return Validacao.EhValido;
		}
	}
}