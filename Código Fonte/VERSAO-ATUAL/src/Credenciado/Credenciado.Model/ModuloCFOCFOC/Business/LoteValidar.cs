using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business
{
	public class LoteValidar
	{
		#region Propriedades

		LoteDa _da = new LoteDa();
		bool IsEditar = false;

		#endregion

		public bool Salvar(Lote lote)
		{
			IsEditar = lote.Id > 0;
			int numero = Convert.ToInt32(lote.Numero);

			if (lote.EmpreendimentoId <= 0)
			{
				Validacao.Add(Mensagem.Lote.EmpreendimentoObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(lote.DataCriacao, "DataCriacao", "criação", false);

			if (!Validacao.EhValido)
			{
				return false;
			}

			if (lote.CodigoUC <= 0)
			{
				Validacao.Add(Mensagem.Lote.CodigoUCObrigatorio);
			}

			if (lote.Ano <= 0)
			{
				Validacao.Add(Mensagem.Lote.AnoObrigatorio);
			}

			if (numero <= 0)
			{
				Validacao.Add(Mensagem.Lote.NumeroSequencialObrigatorio);
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			if (lote.Lotes == null || lote.Lotes.Count <= 0)
			{
				Validacao.Add(Mensagem.Lote.LoteAdicionarObrigatorio);
			}
			else
			{
				lote.Lotes.ForEach(item =>
				{
					LoteValidacoesSalvar(item, lote.DataCriacao, lote.EmpreendimentoId, lote.Lotes, lote.Id);
				});
			}

			return Validacao.EhValido;
		}

        //Valida um item que está sendo incluído no lote
		public void Lote(LoteItem item, DateTecno loteData, int empreendimentoID, List<LoteItem> lista, int loteID)
		{
			if (empreendimentoID <= 0)
			{
				Validacao.Add(Mensagem.Lote.EmpreendimentoObrigatorio);
				return;
			}

			if (item.OrigemTipo <= 0)
			{
				Validacao.Add(Mensagem.Lote.OrigemObrigatorio);
			}

            //if (item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO && _da.VerificarSeCfoJaAssociadaALote(item.Origem) && !_da.VerificarSeDocumentoUtilizadoPorMesmaUC(item.Origem, empreendimentoID))
            //{
            //    Validacao.Add(Mensagem.EmissaoCFO.DocumentoOrigemDeveSerDeMesmaUC);
            //}

			if (item.Cultura <= 0)
			{
				Validacao.Add(Mensagem.Lote.CulturaObrigatoria);
			}

			if (item.Cultivar <= 0)
			{
				Validacao.Add(Mensagem.Lote.CultivarObrigatoria);
			}

			if (item.OrigemTipo >= 5 && item.Quantidade <= 0)
			{
				Validacao.Add(Mensagem.Lote.QuantidadeObrigatorio);
			}

			if (lista.Count(x => x.OrigemTipo == item.OrigemTipo && x.OrigemNumero == item.OrigemNumero && !x.Equals(item)) > 0)
			{
				Validacao.Add(Mensagem.Lote.OrigemJaAdicionada(item.OrigemTipoTexto, item.OrigemNumero.ToString()));
			}

			if (lista != null && lista.Count > 0 && !lista.Any(x => x.Cultivar == item.Cultivar))
			{
				Validacao.Add(Mensagem.Lote.CultivarUnico);
			}

			if (lista != null && lista.Count > 0 && lista.Any(x => x.UnidadeMedida != item.UnidadeMedida))
			{
				Validacao.Add(Mensagem.Lote.UnidadeMedidaUnico);
			}

			if (!Validacao.EhValido)
			{
				return;
			}

			int auxiliar = 0;
			decimal saldoDocOrigem = 0;
			List<IdentificacaoProduto> produtos = OrigemNumero(item.OrigemNumero, item.OrigemTipo, item.Serie, out auxiliar);
			if (produtos != null)
			{
				switch ((eDocumentoFitossanitarioTipo)item.OrigemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
						EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
						EmissaoCFO cfo = emissaoCFOBus.Obter(item.Origem);
						saldoDocOrigem = cfo.Produtos.Where(x => x.CultivarId == item.Cultivar && x.UnidadeMedidaId == item.UnidadeMedida).Sum(x => x.Quantidade);

						if (cfo.SituacaoId != (int)eDocumentoFitossanitarioSituacao.Valido)
						{
							Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
						}

						DateTime dataVencimentoCFO = cfo.DataEmissao.Data.GetValueOrDefault().AddDays(cfo.ValidadeCertificado);
						if (dataVencimentoCFO < DateTime.Today)
						{
							Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
						}

						if (cfo.DataEmissao.Data > loteData.Data)
						{
							Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
						}
						break;

					case eDocumentoFitossanitarioTipo.CFOC:
						EmissaoCFOCBus emissaoCFOCBus = new EmissaoCFOCBus();
						EmissaoCFOC cfoc = emissaoCFOCBus.Obter(item.Origem);
						saldoDocOrigem = cfoc.Produtos.Where(x => x.CultivarId == item.Cultivar && x.UnidadeMedidaId == item.UnidadeMedida).Sum(x => x.Quantidade);

						if (cfoc.SituacaoId != (int)eDocumentoFitossanitarioSituacao.Valido)
						{
							Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
						}

						DateTime dataVencimentoCFOC = cfoc.DataEmissao.Data.GetValueOrDefault().AddDays(cfoc.ValidadeCertificado);
						if (dataVencimentoCFOC < DateTime.Today)
						{
							Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
						}

						if (cfoc.DataEmissao.Data > loteData.Data)
						{
							Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
						}
						break;

					case eDocumentoFitossanitarioTipo.PTV:
						PTVInternoBus ptvInternoBus = new PTVInternoBus();
						PTV ptv = ptvInternoBus.Obter(item.Origem);
						saldoDocOrigem = ptv.Produtos.Where(x => x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida).Sum(x => x.Quantidade);

						if (ptv.Situacao != (int)ePTVOutroSituacao.Valido)
						{
							Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
						}

						if (ptv.ValidoAte.Data.GetValueOrDefault() < DateTime.Today)
						{
							Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
						}

						if (ptv.DataEmissao.Data > loteData.Data)
						{
							Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
						}
						break;

					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						PTVOutroBus ptvOutroBus = new PTVOutroBus();
						PTVOutro ptvOutro = ptvOutroBus.Obter(item.Origem);
						saldoDocOrigem = ptvOutro.Produtos.Where(x => x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida).Sum(x => x.Quantidade);

						if (ptvOutro.Situacao != (int)ePTVOutroSituacao.Valido
							&& ptvOutro.Situacao != (int)ePTVOutroSituacao.Invalido)
						{
							Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
						}

						//if (ptvOutro.ValidoAte.Data.GetValueOrDefault() < DateTime.Today)
						//{
						//	Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
						//}

						if (ptvOutro.DataEmissao.Data > loteData.Data)
						{
							Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
						}
						break;
				}

				string empreendimento = string.Empty;
				switch ((eDocumentoFitossanitarioTipo)item.OrigemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
					case eDocumentoFitossanitarioTipo.CFOC:
						empreendimento = _da.CFOCFOCJaAssociado(item.OrigemTipo, item.Origem, empreendimentoID);

						if (!string.IsNullOrEmpty(empreendimento))
						{
							Validacao.Add(Mensagem.Lote.OrigemEmpreendimentoUtilizado(item.OrigemTipoTexto, item.OrigemNumero.ToString()));
						}
						break;

					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						empreendimento = _da.PTVOutroEstadoJaAssociado(item.Origem, empreendimentoID);

						if (!string.IsNullOrEmpty(empreendimento))
						{
							Validacao.Add(Mensagem.Lote.OrigemEmpreendimentoUtilizadoOutroUF(item.OrigemTipoTexto, item.OrigemNumero.ToString(), empreendimento));
						}
						break;
				}

				if (item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO ||
					item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC ||
					item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)
				{
					if (!_da.UCPossuiCultivar(empreendimentoID, item.Cultivar))
					{
						Validacao.Add(Mensagem.Lote.CultivarDesassociadoUC(item.CultivarTexto));
					}

                    //SALDO DA UC
                    decimal saldoUc = _da.obterSaldoRestanteCultivarUC(empreendimentoID, item.Cultivar, item.Cultura);
                    if (saldoUc <= 0)
                    {
                        Validacao.Add(Mensagem.Lote.CultivarSaldoTodoUtilizado);
                    }

                    decimal quantidadeAdicionada = lista.Sum(x => x.Quantidade);

                    if ((quantidadeAdicionada + item.Quantidade) > saldoUc)
                    {
                        Validacao.Add(Mensagem.Lote.CultivarQuantidadeSomaSuperior);
                    }
				}

                item.Quantidade = saldoDocOrigem;
			}
		}

        //Confere novamente um item que já foi inserido no lote
        public void LoteValidacoesSalvar(LoteItem item, DateTecno loteData, int empreendimentoID, List<LoteItem> lista, int loteID)
        {
            if (empreendimentoID <= 0)
            {
                Validacao.Add(Mensagem.Lote.EmpreendimentoObrigatorio);
                return;
            }

            if (item.OrigemTipo <= 0)
            {
                Validacao.Add(Mensagem.Lote.OrigemObrigatorio);
            }

            if (item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO && _da.VerificarSeCfoJaAssociadaALote(item.Origem) && !_da.VerificarSeDocumentoUtilizadoPorMesmaUC(item.Origem, empreendimentoID))
            {
                Validacao.Add(Mensagem.EmissaoCFO.DocumentoOrigemDeveSerDeMesmaUC);
            }

            if (item.Cultura <= 0)
            {
                Validacao.Add(Mensagem.Lote.CulturaObrigatoria);
            }

            if (item.Cultivar <= 0)
            {
                Validacao.Add(Mensagem.Lote.CultivarObrigatoria);
            }

            if (item.OrigemTipo >= 5 && item.Quantidade <= 0)
            {
                Validacao.Add(Mensagem.Lote.QuantidadeObrigatorio);
            }

            if (lista.Count(x => x.OrigemTipo == item.OrigemTipo && x.OrigemNumero == item.OrigemNumero && !x.Equals(item)) > 0)
            {
                Validacao.Add(Mensagem.Lote.OrigemJaAdicionada(item.OrigemTipoTexto, item.OrigemNumero.ToString()));
            }

            if (lista != null && lista.Count > 0 && !lista.Any(x => x.Cultivar == item.Cultivar))
            {
                Validacao.Add(Mensagem.Lote.CultivarUnico);
            }

            if (lista != null && lista.Count > 0 && lista.Any(x => x.UnidadeMedida != item.UnidadeMedida))
            {
                Validacao.Add(Mensagem.Lote.UnidadeMedidaUnico);
            }

            if (!Validacao.EhValido)
            {
                return;
            }

            int auxiliar = 0;
            decimal saldoDocOrigem = 0;
            List<IdentificacaoProduto> produtos = OrigemNumero(item.OrigemNumero, item.OrigemTipo, item.Serie, out auxiliar);
            if (produtos != null)
            {
                switch ((eDocumentoFitossanitarioTipo)item.OrigemTipo)
                {
                    case eDocumentoFitossanitarioTipo.CFO:
                        EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
                        EmissaoCFO cfo = emissaoCFOBus.Obter(item.Origem);
                        saldoDocOrigem = cfo.Produtos.Where(x => x.CultivarId == item.Cultivar && x.UnidadeMedidaId == item.UnidadeMedida).Sum(x => x.Quantidade);

                        if (cfo.SituacaoId != (int)eDocumentoFitossanitarioSituacao.Valido)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
                        }

                        DateTime dataVencimentoCFO = cfo.DataEmissao.Data.GetValueOrDefault().AddDays(cfo.ValidadeCertificado);
                        if (dataVencimentoCFO < DateTime.Today)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
                        }

                        if (cfo.DataEmissao.Data > loteData.Data)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
                        }
                        break;

                    case eDocumentoFitossanitarioTipo.CFOC:
                        EmissaoCFOCBus emissaoCFOCBus = new EmissaoCFOCBus();
                        EmissaoCFOC cfoc = emissaoCFOCBus.Obter(item.Origem);
                        saldoDocOrigem = cfoc.Produtos.Where(x => x.CultivarId == item.Cultivar && x.UnidadeMedidaId == item.UnidadeMedida).Sum(x => x.Quantidade);

                        if (cfoc.SituacaoId != (int)eDocumentoFitossanitarioSituacao.Valido)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
                        }

                        DateTime dataVencimentoCFOC = cfoc.DataEmissao.Data.GetValueOrDefault().AddDays(cfoc.ValidadeCertificado);
                        if (dataVencimentoCFOC < DateTime.Today)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
                        }

                        if (cfoc.DataEmissao.Data > loteData.Data)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
                        }
                        break;

                    case eDocumentoFitossanitarioTipo.PTV:
                        PTVInternoBus ptvInternoBus = new PTVInternoBus();
                        PTV ptv = ptvInternoBus.Obter(item.Origem);
                        saldoDocOrigem = ptv.Produtos.Where(x => x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida).Sum(x => x.Quantidade);

                        if (ptv.Situacao != (int)ePTVOutroSituacao.Valido)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
                        }

                        if (ptv.ValidoAte.Data.GetValueOrDefault() < DateTime.Today)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
                        }

                        if (ptv.DataEmissao.Data > loteData.Data)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
                        }
                        break;

                    case eDocumentoFitossanitarioTipo.PTVOutroEstado:
                        PTVOutroBus ptvOutroBus = new PTVOutroBus();
                        PTVOutro ptvOutro = ptvOutroBus.Obter(item.Origem);
                        saldoDocOrigem = ptvOutro.Produtos.Where(x => x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida).Sum(x => x.Quantidade);

                        if (ptvOutro.Situacao != (int)ePTVOutroSituacao.Valido
                            && ptvOutro.Situacao != (int)ePTVOutroSituacao.Invalido)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemSituacaoInvalida(item.OrigemTipoTexto));
                        }

                        if (ptvOutro.DataEmissao.Data > loteData.Data)
                        {
                            Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
                        }
                        break;
                }

                string empreendimento = string.Empty;
                switch ((eDocumentoFitossanitarioTipo)item.OrigemTipo)
                {
                    case eDocumentoFitossanitarioTipo.CFO:
                    case eDocumentoFitossanitarioTipo.CFOC:
                        empreendimento = _da.CFOCFOCJaAssociado(item.OrigemTipo, item.Origem, empreendimentoID);

                        if (!string.IsNullOrEmpty(empreendimento))
                        {
                            Validacao.Add(Mensagem.Lote.OrigemEmpreendimentoUtilizado(item.OrigemTipoTexto, item.OrigemNumero.ToString()));
                        }
                        break;

                    case eDocumentoFitossanitarioTipo.PTVOutroEstado:
                        empreendimento = _da.PTVOutroEstadoJaAssociado(item.Origem, empreendimentoID);

                        if (!string.IsNullOrEmpty(empreendimento))
                        {
                            Validacao.Add(Mensagem.Lote.OrigemEmpreendimentoUtilizadoOutroUF(item.OrigemTipoTexto, item.OrigemNumero.ToString(), empreendimento));
                        }
                        break;
                }

                if (item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO ||
                    item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC ||
                    item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)
                {
                    if (!_da.UCPossuiCultivar(empreendimentoID, item.Cultivar))
                    {
                        Validacao.Add(Mensagem.Lote.CultivarDesassociadoUC(item.CultivarTexto));
                    }
                }

                if (item.OrigemTipo != (int)eDocumentoFitossanitarioTipo.CFO || item.OrigemTipo != (int)eDocumentoFitossanitarioTipo.CFOC)
                    item.Quantidade = saldoDocOrigem;

            }
        }

		public List<IdentificacaoProduto> OrigemNumero(string numero, int origemTipo, string serieNumeral, out int origemID)
		{
			if (origemTipo == 0)
			{
				Validacao.Add(Mensagem.Lote.OrigemObrigatorio);
			}

			if (string.IsNullOrEmpty(numero) && !IsEditar)
			{
				Validacao.Add(Mensagem.Lote.OrigemNumeroObrigatorio(string.Empty));
			}
			else
			{
				switch ((eDocumentoFitossanitarioTipo)origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
						EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
                        EmissaoCFO CFO = emissaoCFOBus.ObterPorNumero(Convert.ToInt64(numero), credenciado: false, serieNumero: serieNumeral);

						if (CFO.Id > 0)
						{
							origemID = CFO.Id;
							return CFO.Produtos;
						}

						Validacao.Add(Mensagem.Lote.OrigemNumeroInexistente(eDocumentoFitossanitarioTipo.CFO.ToString()));
						break;

					case eDocumentoFitossanitarioTipo.CFOC:
						EmissaoCFOCBus emissaoCFOCBus = new EmissaoCFOCBus();
						EmissaoCFOC CFOC = emissaoCFOCBus.ObterPorNumero(Convert.ToInt64(numero), credenciado: false, serieNumero: serieNumeral);

						if (CFOC.Id > 0)
						{
							origemID = CFOC.Id;
							return CFOC.Produtos;
						}

						Validacao.Add(Mensagem.Lote.OrigemNumeroInexistente(eDocumentoFitossanitarioTipo.CFOC.ToString()));
						break;

					case eDocumentoFitossanitarioTipo.PTVOutroEstado:

						PTVOutroBus ptvOutroBus = new PTVOutroBus();
						PTVOutro ptvOutro = ptvOutroBus.ObterPorNumero(Convert.ToInt64(numero), credenciado: false);

						if (ptvOutro.Id > 0)
						{
							origemID = ptvOutro.Id;

							List<IdentificacaoProduto> lista = new List<IdentificacaoProduto>();
							foreach (var item in ptvOutro.Produtos)
							{
								IdentificacaoProduto itemLista = new IdentificacaoProduto();
								itemLista.CulturaId = item.Cultura;
								itemLista.CulturaTexto = item.CulturaTexto;
								itemLista.CultivarId = item.Cultivar;
								itemLista.CultivarTexto = item.CultivarTexto;
								itemLista.UnidadeMedidaId = item.UnidadeMedida;
								itemLista.Quantidade = item.Quantidade;
								lista.Add(itemLista);
							}

							return lista;
						}

						Validacao.Add(Mensagem.Lote.OrigemNumeroInexistente("PTV de outro estado"));

						break;

					case eDocumentoFitossanitarioTipo.PTV:

						PTVInternoBus ptvBus = new PTVInternoBus();
						PTV ptv = ptvBus.ObterPorNumero(Convert.ToInt64(numero));

						if (ptv.Id > 0)
						{
							origemID = ptv.Id;

							List<IdentificacaoProduto> lista = new List<IdentificacaoProduto>();
							foreach (var item in ptv.Produtos)
							{
								IdentificacaoProduto itemLista = new IdentificacaoProduto();
								itemLista.CulturaId = item.Cultura;
								itemLista.CulturaTexto = item.CulturaTexto;
								itemLista.CultivarId = item.Cultivar;
								itemLista.CultivarTexto = item.CultivarTexto;
								itemLista.UnidadeMedidaId = item.UnidadeMedida;
								itemLista.Quantidade = item.Quantidade;
								lista.Add(itemLista);
							}

							return lista;
						}

						Validacao.Add(Mensagem.Lote.OrigemNumeroInexistente("PTV"));
						break;
				}
			}

			origemID = 0;
			return null;
		}

		public bool LoteSituacao(int id, BancoDeDados banco)
		{
			string numero = _da.LoteSituacao(id, banco);

			if (!String.IsNullOrEmpty(numero))
			{
				Validacao.Add(Mensagem.Lote.LoteSendoUtilizado(numero));
			}

			return Validacao.EhValido;
		}

		public bool AcessarTela(Lote lote)
		{
			HabilitarEmissaoCFOCFOCBus habilitarEmissaoCFOCFOCBus = new HabilitarEmissaoCFOCFOCBus();

			if (!habilitarEmissaoCFOCFOCBus.VerificarCredenciadoHabilitado())
			{
				Validacao.Add(Mensagem.Lote.HabilitacaoEmissaoObrigatorio);
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			string retorno = _da.LoteAssociado(id);
			if (!string.IsNullOrEmpty(retorno))
			{
				Validacao.Add(Mensagem.Lote.LoteNaoPodeSerExcluir(retorno));
			}

			return Validacao.EhValido;
		}

		public bool Editar(Lote lote)
		{
			if (lote.SituacaoId != (int)eLoteSituacao.NaoUtilizado)
			{
				Validacao.Add(Mensagem.Lote.EditarSituacaoInvalida);
			}

			return Validacao.EhValido;
		}
	}
}