using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTVOutro.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business
{
	public class PTVValidar
	{
		#region Proriedades

		private PTVDa _da = new PTVDa();
		public bool NovoDestinatario = false;

		#endregion

		internal bool Salvar(PTV ptv)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			if (!ValidarSituacao(ptv))
			{
				return Validacao.EhValido;
			}

			if (ptv.NumeroTipo != (int)eDocumentoFitossanitarioTipoNumero.Digital && ptv.NumeroTipo != (int)eDocumentoFitossanitarioTipoNumero.Bloco)
			{
				Validacao.Add(Mensagem.PTV.TipoNumeroObrigatorio);
			}
			if (ptv.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Bloco && ptv.Numero <= 0)
			{
				Validacao.Add(Mensagem.PTV.NumeroPTVObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(ptv.DataEmissao, "DataEmissao", "emissão");


			if (ptv.Situacao <= 0)
			{
				Validacao.Add(Mensagem.PTV.SituacaoObrigatorio);
			}


			if (ptv.Produtos.Count > 0 && ((!ptv.Produtos[0].SemDoc) &&
				(ptv.Produtos[0].OrigemTipo <= (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)))
			{
				if (ptv.Empreendimento <= 0)
				{
					Validacao.Add(Mensagem.PTV.EmpreendimentoObrigatorio);
				}

				if (ptv.ResponsavelEmpreendimento <= 0)
				{
					Validacao.Add(Mensagem.PTV.ResponsavelEmpreend_Obrigatorio);
				}
			}

			if (ptv.Produtos.Count <= 0)
			{
				Validacao.Add(Mensagem.PTV.Identificacao_Produto_Obrigatorio);
			}
			else
			{
				ptv.Produtos.ForEach(produto =>
				{
					ValidarProduto(produto, ptv.DataEmissao, ptv.Produtos, ptv.Id);
				});
			}

			if (!ptv.PartidaLacradaOrigem.HasValue)
			{
				Validacao.Add(Mensagem.PTV.PartidaLacrada_Obrigatorio);
			}

			if (ptv.PartidaLacradaOrigem == (int)ePartidaLacradaOrigem.Sim && (String.IsNullOrEmpty(ptv.LacreNumero) && String.IsNullOrEmpty(ptv.PoraoNumero) && String.IsNullOrEmpty(ptv.ContainerNumero)))
			{
				Validacao.Add(Mensagem.PTV.Lacre_porao_container_Obrigatorio);
			}

			if (ptv.DestinatarioID <= 0)
			{
				Validacao.Add(Mensagem.PTV.DestinatarioObrigatorio);
			}

			if (ptv.TransporteTipo <= 0)
			{
				Validacao.Add(Mensagem.PTV.TipoTransporteObrigatorio);
			}

			if (String.IsNullOrEmpty(ptv.VeiculoIdentificacaoNumero))
			{
				Validacao.Add(Mensagem.PTV.IdentificacaoVeiculoObrigatorio);
			}

			if (!ptv.RotaTransitoDefinida.HasValue)
			{
				Validacao.Add(Mensagem.PTV.RotaTransitoObrigatorio);
			}
			if (ptv.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Sim && String.IsNullOrEmpty(ptv.Itinerario))
			{
				Validacao.Add(Mensagem.PTV.Itinerário_Obrigatorio);
			}

			if (!ptv.NotaFiscalApresentacao.HasValue || ptv.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Nao)
			{
				Validacao.Add(Mensagem.PTV.ApresentaçãoNotaFiscal_Obrigatorio);
			}

			if (ptv.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Sim && String.IsNullOrEmpty(ptv.NotaFiscalNumero))
			{
				Validacao.Add(Mensagem.PTV.NumeroNotaFiscal_Obrigatorio);
			}

			if (ValidacoesGenericasBus.DataMensagem(ptv.ValidoAte, "DataValidade", "validação", false))
			{
				if (ptv.ValidoAte.Data < ptv.DataEmissao.Data)
				{
					Validacao.Add(Mensagem.PTV.DataValidadeMenorDataEmissao);
				}
				else
				{
					DateTime data = (DateTime)ptv.ValidoAte.Data;
					DateTime dt = DateTime.Today.AddDays(30).Subtract(TimeSpan.FromSeconds(1));
					if (data > dt)
					{
						Validacao.Add(Mensagem.PTV.DataValidadeInvalida);
					}
				}
			}

			if (ptv.ResponsavelTecnicoId <= 0)
			{
				Validacao.Add(Mensagem.PTV.ResponsavelTecnicoObrigatorio);
			}

			if (ptv.LocalEmissaoId <= 0)
			{
				Validacao.Add(Mensagem.PTV.LocalDeEmissaoObrigatorio);

			}

			if (ptv.NFCaixa.notaFiscalCaixaApresentacao == 1 && ptv.NotaFiscalDeCaixas.Count() <= 0)
			{
				Validacao.Add(Mensagem.PTV.NenhumaNFCaixaAdicionada);
			}
			else
			{
				foreach (var nf in ptv.NotaFiscalDeCaixas)
				{
					NotaFiscalCaixa nfDa = _da.VerificarNumeroNFCaixa(nf);

					if (nfDa.id > 0 && nf.numeroCaixas > nfDa.saldoAtual)
					{
						Validacao.Add(Mensagem.PTV.NumeroDeCaixasMaiorQueSaldoAtual);
					}
				}
			}

			return Validacao.EhValido;
		}

		internal bool Importar(PTV ptv)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			if (ptv.NumeroTipo != (int)eDocumentoFitossanitarioTipoNumero.Digital && ptv.NumeroTipo != (int)eDocumentoFitossanitarioTipoNumero.Bloco)
			{
				Validacao.Add(Mensagem.PTV.TipoNumeroObrigatorio);
			}

			if (ptv.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Bloco && ptv.Numero <= 0)
			{
				Validacao.Add(Mensagem.PTV.NumeroPTVObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(ptv.DataEmissao, "DataEmissao", "emissão");

			if (ptv.Situacao <= 0)
			{
				Validacao.Add(Mensagem.PTV.SituacaoObrigatorio);
			}

			if (ptv.Produtos.Count > 0 && ((!ptv.Produtos[0].SemDoc) &&
			(ptv.Produtos[0].OrigemTipo <= (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)))
			{
				if (ptv.Empreendimento <= 0)
				{
					Validacao.Add(Mensagem.PTV.EmpreendimentoObrigatorio);
				}

				if (ptv.ResponsavelEmpreendimento <= 0)
				{
					Validacao.Add(Mensagem.PTV.ResponsavelEmpreend_Obrigatorio);
				}
			}

			if (ptv.Produtos.Count <= 0)
			{
				Validacao.Add(Mensagem.PTV.Identificacao_Produto_Obrigatorio);
			}

			if (!ptv.PartidaLacradaOrigem.HasValue)
			{
				Validacao.Add(Mensagem.PTV.PartidaLacrada_Obrigatorio);
			}

			if (ptv.PartidaLacradaOrigem == (int)ePartidaLacradaOrigem.Sim && (String.IsNullOrEmpty(ptv.LacreNumero) && String.IsNullOrEmpty(ptv.PoraoNumero) && String.IsNullOrEmpty(ptv.ContainerNumero)))
			{
				Validacao.Add(Mensagem.PTV.Lacre_porao_container_Obrigatorio);
			}

			if (ptv.DestinatarioID <= 0)
			{
				Validacao.Add(Mensagem.PTV.DestinatarioObrigatorio);
			}

			if (!ptv.PossuiLaudoLaboratorial.HasValue)
			{
				Validacao.Add(Mensagem.PTV.PossuiLaudoLab_Obrigatorio);
			}

			if (ptv.TransporteTipo <= 0)
			{
				Validacao.Add(Mensagem.PTV.TipoTransporteObrigatorio);
			}

			if (String.IsNullOrEmpty(ptv.VeiculoIdentificacaoNumero))
			{
				Validacao.Add(Mensagem.PTV.IdentificacaoVeiculoObrigatorio);
			}

			if (!ptv.RotaTransitoDefinida.HasValue)
			{
				Validacao.Add(Mensagem.PTV.RotaTransitoObrigatorio);
			}
			if (ptv.RotaTransitoDefinida == (int)eRotaTransitoDefinida.Sim && String.IsNullOrEmpty(ptv.Itinerario))
			{
				Validacao.Add(Mensagem.PTV.Itinerário_Obrigatorio);
			}

			if (!ptv.NotaFiscalApresentacao.HasValue || ptv.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Nao)
			{
				Validacao.Add(Mensagem.PTV.ApresentaçãoNotaFiscal_Obrigatorio);
			}

			if (ptv.NotaFiscalApresentacao == (int)eApresentacaoNotaFiscal.Sim && String.IsNullOrEmpty(ptv.NotaFiscalNumero))
			{
				Validacao.Add(Mensagem.PTV.NumeroNotaFiscal_Obrigatorio);
			}

			if (ValidacoesGenericasBus.DataMensagem(ptv.ValidoAte, "DataValidade", "validação", false))
			{
				if (ptv.ValidoAte.Data < ptv.DataEmissao.Data)
				{
					Validacao.Add(Mensagem.PTV.DataValidadeMenorDataEmissao);
				}
				else
				{
					DateTime data = (DateTime)ptv.ValidoAte.Data;
					DateTime dt = DateTime.Today.AddDays(30).Subtract(TimeSpan.FromSeconds(1));
					if (data > dt)
					{
						Validacao.Add(Mensagem.PTV.DataValidadeInvalida);
					}
				}
			}

			if (ptv.ResponsavelTecnicoId <= 0)
			{
				Validacao.Add(Mensagem.PTV.ResponsavelTecnicoObrigatorio);
			}

			if (ptv.LocalEmissaoId <= 0)
			{
				Validacao.Add(Mensagem.PTV.LocalDeEmissaoObrigatorio);
			}

			if (!ptv.TemAssinatura)
			{
				Validacao.Add(Mensagem.PTV.FuncionarioSemAssinatura);
			}

			return Validacao.EhValido;
		}

		public bool ValidarSituacao(PTV entidade)
		{
			if (entidade.Id > 0 && entidade.Situacao != (int)ePTVSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.PTV.NaoPodeEditarPTV);
			}

			return Validacao.EhValido;
		}

		internal string VerificarNumeroPTV(int tipoNumero, string PTVNumero)
		{
			if (tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco)
			{
				if (String.IsNullOrEmpty(PTVNumero))
				{
					Validacao.Add(Mensagem.PTV.NumeroPTVObrigatorio);
					return "";
				}

				if (!_da.VerificarConfigNumero(tipoNumero, Convert.ToInt64(PTVNumero)))
				{
					Validacao.Add(Mensagem.PTV.NumeroPtvNaoConfigurado);
					return "";
				}

				if (PTVNumero.ToString().Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2))
				{
					Validacao.Add(Mensagem.PTV.AnoPTVInvalido);
				}

			}

			if (tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital)
			{
				if (String.IsNullOrEmpty(PTVNumero))
				{
					Validacao.Add(Mensagem.PTV.NumeroPtvNaoConfigurado);
					return "";
				}
			}

			if (_da.VerificarNumeroPTV(Convert.ToInt64(PTVNumero)))
			{
				Validacao.Add(Mensagem.PTV.PtvJaExistente);
			}

			return PTVNumero;
		}

		public bool ValidarProduto(PTVProduto item, DateTecno ptvData, List<PTVProduto> lista, int ptvID)
		{
			lista = lista ?? new List<PTVProduto>();

			if (item.SemDoc)
				return true;

			if (item.OrigemTipo <= 0)
			{
				Validacao.Add(Mensagem.PTV.TipoOrigemObrigatorio);
				return false;
			}

			if (item.Origem <= 0 && string.IsNullOrEmpty(item.OrigemNumero))
			{
				Validacao.Add(Mensagem.PTV.OrigemObrigatorio);
				return false;
			}


			#region Saldo

			//TODO
			decimal saldo = 0;
			int produtorItem = 0;
			switch ((eDocumentoFitossanitarioTipo)item.OrigemTipo)
			{
				case eDocumentoFitossanitarioTipo.CFO:
					EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
					EmissaoCFO cfo = emissaoCFOBus.Obter(item.Origem);
					saldo = cfo.Produtos.Where(x => x.CultivarId == item.Cultivar && x.UnidadeMedidaId == item.UnidadeMedida).Sum(x => x.Quantidade);
					produtorItem = cfo.ProdutorId;

					if (cfo.SituacaoId != (int)eDocumentoFitossanitarioSituacao.Valido)
					{
						Validacao.Add(Mensagem.PTV.OrigemSituacaoInvalida(item.OrigemTipoTexto));
					}

					DateTime dataVencimentoCFO = cfo.DataAtivacao.Data.GetValueOrDefault().AddDays(cfo.ValidadeCertificado);
					if (dataVencimentoCFO < DateTime.Today)
					{
						Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
					}

					if (cfo.DataEmissao.Data > ptvData.Data)
					{
						Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
					}
					break;

				case eDocumentoFitossanitarioTipo.CFOC:
					EmissaoCFOCBus emissaoCFOCBus = new EmissaoCFOCBus();
					EmissaoCFOC cfoc = emissaoCFOCBus.Obter(item.Origem);
					saldo = cfoc.Produtos.Where(x => x.CultivarId == item.Cultivar && x.UnidadeMedidaId == item.UnidadeMedida).Sum(x => x.Quantidade);

					if (cfoc.SituacaoId != (int)eDocumentoFitossanitarioSituacao.Valido)
					{
						Validacao.Add(Mensagem.PTV.OrigemSituacaoInvalida(item.OrigemTipoTexto));
					}

					DateTime dataVencimentoCFOC = cfoc.DataAtivacao.Data.GetValueOrDefault().AddDays(cfoc.ValidadeCertificado);
					if (dataVencimentoCFOC < DateTime.Today)
					{
						Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
					}

					if (cfoc.DataEmissao.Data > ptvData.Data)
					{
						Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
					}
					break;

				case eDocumentoFitossanitarioTipo.PTVOutroEstado:
					PTVOutroBus ptvOutroBus = new PTVOutroBus();
					PTVOutro ptvOutro = ptvOutroBus.Obter(item.Origem);
					saldo = ptvOutro.Produtos.Where(x => x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida).Sum(x => x.Quantidade);

					if (ptvOutro.Situacao != (int)ePTVOutroSituacao.Valido)
					{
						Validacao.Add(Mensagem.PTV.OrigemSituacaoInvalida(item.OrigemTipoTexto));
					}

					if (ptvOutro.ValidoAte.Data.GetValueOrDefault() < DateTime.Today)
					{
						Validacao.Add(Mensagem.Lote.OrigemVencida(item.OrigemTipoTexto));
					}

					if (ptvOutro.DataEmissao.Data > ptvData.Data)
					{
						Validacao.Add(Mensagem.Lote.OrigemDataMaiorLoteData);
					}
					break;

				case eDocumentoFitossanitarioTipo.PTV:
					PTVBus ptvBus = new PTVBus();
					PTV ptv = ptvBus.Obter(item.Origem);
					saldo = ptv.Produtos.Where(x => x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida).Sum(x => x.Quantidade);
					produtorItem = ptv.ResponsavelEmpreendimento;
					break;
			}

			#endregion Saldo

			if (lista.Count > 0 && produtorItem > 0)
			{
				int produtorPrimeiroItem = 0;
				PTVProduto primeiroItem = lista.FirstOrDefault(x => x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV);

				if (primeiroItem != null)
				{
					switch ((eDocumentoFitossanitarioTipo)primeiroItem.OrigemTipo)
					{
						case eDocumentoFitossanitarioTipo.CFO:
							EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
							EmissaoCFO cfo = emissaoCFOBus.Obter(primeiroItem.Origem, true);
							produtorPrimeiroItem = cfo.ProdutorId;
							break;

						case eDocumentoFitossanitarioTipo.PTV:
							PTVBus ptvBus = new PTVBus();
							PTV ptv = ptvBus.Obter(primeiroItem.Origem, true);
							produtorPrimeiroItem = ptv.ResponsavelEmpreendimento;
							break;
					}

					if (produtorItem != produtorPrimeiroItem)
					{
						Validacao.Add(Mensagem.PTV.ProdutorDiferente);
					}
				}
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			if (item.Cultura <= 0)
			{
				Validacao.Add(Mensagem.PTV.CulturaObrigatorio);
			}
			if (item.Cultivar <= 0)
			{
				Validacao.Add(Mensagem.PTV.CultivarObrigatorio);
			}
			if (item.UnidadeMedida <= 0)
			{
				Validacao.Add(Mensagem.PTV.UnidadeMedidaObrigatorio);
			}
			if (item.Quantidade <= 0)
			{
				Validacao.Add(Mensagem.PTV.QuantidadeObrigatorio);
			}
			if (lista.Count(x => !x.Equals(item)) >= 5)
			{
				Validacao.Add(Mensagem.PTV.QauntidadeItensUltrapassado);
			}
			if (Validacao.EhValido && lista.Count > 0)
			{
				if (lista.Count(x => x.Origem == item.Origem && item.Cultivar == x.Cultivar && x.UnidadeMedida == item.UnidadeMedida && !x.Equals(item)) > 0)
				{
					Validacao.Add(Mensagem.PTV.ITemProdutoJaAdicionado(item.OrigemTipoTexto));
				}

				if (lista.Count(x => x.EmpreendimentoId != item.EmpreendimentoId && x.EmpreendimentoId > 0) > 0)
				{
					Validacao.Add(Mensagem.PTV.EmpreendimentoOrigemDiferente);
				}
			}

			if (Validacao.EhValido)
			{
				if (item.EmpreendimentoId > 0)
				{
					if (_da.EmpreendimentoPossuiEPTVBloqueado(item.EmpreendimentoId))
					{
						Validacao.Add(Mensagem.PTV.EmpreendimentoEPTVBloqueado);
					}
				}
			}

			if (Validacao.EhValido)
			{
				if (item.OrigemTipo != (int)eDocumentoFitossanitarioTipo.CFCFR && item.OrigemTipo != (int)eDocumentoFitossanitarioTipo.TF && item.OrigemTipo != (int)eDocumentoFitossanitarioTipo.SemDocOrigem)
				{
					decimal saldoOutrosDoc = _da.ObterOrigemQuantidade((eDocumentoFitossanitarioTipo)item.OrigemTipo, item.Origem, item.OrigemNumero, item.Cultivar, item.UnidadeMedida, ptvID);

					decimal quantidadeAdicionada = lista.Where(x => x.OrigemTipo == item.OrigemTipo && x.Origem == item.Origem && x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida && !x.Equals(item)).Sum(x => x.Quantidade);

					if ((saldoOutrosDoc + quantidadeAdicionada + (item.ExibeQtdKg ? item.Quantidade / 1000 : item.Quantidade)) > saldo)
					{
						Validacao.Add(Mensagem.PTV.SomaQuantidadeInvalida);
					}
				}
			}

			return Validacao.EhValido;
		}

		public DestinatarioPTV ValidarDocumento(int pessoaTipo, string CpfCnpj)
		{
			DestinatarioPTV destinatario = new DestinatarioPTV();

			if (pessoaTipo != (int)ePessoaTipo.Fisica && pessoaTipo != (int)ePessoaTipo.Juridica)
			{
				Validacao.Add(Mensagem.PTV.TipoDocumentoObrigatorio);
			}

			if (string.IsNullOrEmpty(CpfCnpj))
			{
				Validacao.Add(Mensagem.PTV.DestinatarioObrigatorio);
			}

			if (pessoaTipo == (int)ePessoaTipo.Fisica)
			{
				if (!ValidacoesGenericasBus.Cpf(CpfCnpj))
				{
					Validacao.Add(Mensagem.PTV.CpfCnpjInvalido);
				}
			}

			if (pessoaTipo == (int)ePessoaTipo.Juridica)
			{
				if (!ValidacoesGenericasBus.Cnpj(CpfCnpj))
				{
					Validacao.Add(Mensagem.PTV.CpfCnpjInvalido);
				}
			}

			if (Validacao.EhValido)
			{
				DestinatarioPTVBus destinatarioBus = new DestinatarioPTVBus();

				destinatario = destinatarioBus.Obter(destinatarioBus.ObterId(CpfCnpj));

				if (destinatario.ID <= 0)
				{
					NovoDestinatario = true;//Habilita botão Novo destinatário
					Validacao.Add(Mensagem.PTV.DestinatarioNaoCadastrado);
				}
			}

			return destinatario;
		}

		internal bool Ativar(DateTecno DataAtivacao, PTV ptvBanco)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			ValidacoesGenericasBus.DataMensagem(DataAtivacao, "DataAtivacao", "ativação", ptvBanco.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Bloco);

			if (ptvBanco.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Digital && DataAtivacao.DataTexto != DateTime.Today.ToShortDateString())
			{
				Validacao.Add(Mensagem.Padrao.DataIgualAtual("DataAtivacao", "ativação"));
			}

			if (ptvBanco.Situacao != (int)ePTVSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.PTV.AtivarSituacaoInvalida);
			}

			if (DateTime.Today > ptvBanco.ValidoAte.Data)
			{
				Validacao.Add(Mensagem.PTV.DataPTVInvalida);
			}

			return Validacao.EhValido;
		}

		internal bool PTVCancelar(PTV ptv)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			PTV ptvBanco = _da.Obter(ptv.Id, true);

			ValidacoesGenericasBus.DataMensagem(ptv.DataCancelamento, "DataCancelamento", "cancelamento");

			if (ptvBanco.Situacao != (int)ePTVSituacao.Valido)
			{
				Validacao.Add(Mensagem.PTV.CancelarSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			PTV ptvBanco = _da.Obter(id, true);

			DateTime dtBanco = (DateTime)ptvBanco.DataEmissao.Data;

			if (ptvBanco.Situacao != (int)ePTVSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.PTV.NaoPodeEditarPTV);
			}

			return Validacao.EhValido;
		}

		public bool ValidarAcessoAnalisar(PTV eptv)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			FuncionarioBus funcionarioBus = new FuncionarioBus();

			if (!funcionarioBus.VerificarFuncionarioContidoSetor((HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId, eptv.LocalVistoriaId))
			{
				Validacao.Add(Mensagem.PTV.FuncionarioNaoAlocadoSetor);
			}

			return Validacao.EhValido;
		}

		internal bool Analisar(PTV eptv, PTV eptvBanco)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			//situacoes
			if (eptv.Situacao == 0)
			{
				Validacao.Add(new Mensagem { Texto = "Informe o Resultado da analise.", Tipo = eTipoMensagem.Advertencia });
				return false;
			}

			if (_da.EPTVJaFoiImportado(eptv.Id))
			{
				Validacao.Add(new Mensagem { Texto = "O EPTV já foi importado para o institucional.", Tipo = eTipoMensagem.Advertencia });
				return false;
			}


			if (_da.EmpreendimentoPossuiEPTVBloqueado(eptv.Id, eptv.Empreendimento))
			{
				Validacao.Add(Mensagem.PTV.EmpreendimentoEPTVBloqueado);
			}

			if (Validacao.EhValido)
			{
				if (eptv.Situacao == 3/*Aprovado*/)
				{
					if (eptv.ValidoAte.IsEmpty)
						Validacao.Add(new Mensagem { Campo = "ValidoAte", Texto = "Campo \"Válido até\" não pode ser vazio.", Tipo = eTipoMensagem.Advertencia });

					if (!eptv.ValidoAte.IsValido)
						Validacao.Add(new Mensagem { Campo = "ValidoAte", Texto = "Campo \"Válido até\" inválido.", Tipo = eTipoMensagem.Advertencia });

					if (eptv.LocalEmissaoId == 0)
						Validacao.Add(new Mensagem { Campo = "LocalEmissao", Texto = "Campo \"Local da Emissão\" não selecionado.", Tipo = eTipoMensagem.Advertencia });
				}

				if ((eptv.Situacao == 4/*Rejeitado*/ || eptv.Situacao == 6/*Bloqueado*/) && string.IsNullOrWhiteSpace(eptv.SituacaoMotivo))
					Validacao.Add(new Mensagem { Campo = "SituacaoMotivo", Texto = "Informe o Motivo da Situação.", Tipo = eTipoMensagem.Advertencia });

				if (eptv.Situacao == (int)eSolicitarPTVSituacao.Bloqueado && eptvBanco.Situacao != (int)eSolicitarPTVSituacao.AgendarFiscalizacao)
					Validacao.Add(new Mensagem { Texto = "Para Bloquear o EPTV ele deve estar na situação \"Fiscalização Agendada\".", Tipo = eTipoMensagem.Advertencia });

				if (eptv.ResponsavelTecnicoId <= 0)
				{
					Validacao.Add(Mensagem.PTV.ResponsavelTecnicoObrigatorio);
				}
			}

			return Validacao.EhValido;
		}

		public bool ValidarAcessoComunicadorPTV(int idPTV)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus ptvCredenciadoBus = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();
			PTV eptv = ptvCredenciadoBus.Obter(idPTV, true);

			if (!_da.PossuiAcessoComunicadorPTV(Executor.Current.Id, eptv.LocalVistoriaId))
			{
				Validacao.Add(Mensagem.PTV.AcessoNaoPermitido);
				return false;
			}

			if (eptv.Situacao != (int)eSolicitarPTVSituacao.Bloqueado &&
				eptv.Situacao != (int)eSolicitarPTVSituacao.AgendarFiscalizacao &&
				eptv.Situacao != (int)eSolicitarPTVSituacao.Rejeitado)
			{
				Validacao.Add(Mensagem.PTV.ComunicadorPTVSituacaoInvalida);
				return false;
			}

			return true;
		}

		public bool ValidarAcessoAnalisarDesbloqueioPTV(int idPTV)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus ptvCredenciadoBus = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();
			PTV eptv = ptvCredenciadoBus.Obter(idPTV, true);

			if (!_da.PossuiAcessoComunicadorPTV(Executor.Current.Id, eptv.LocalVistoriaId))
			{
				Validacao.Add(Mensagem.PTV.AcessoNaoPermitido);
				return false;
			}

			if (eptv.Situacao != (int)eSolicitarPTVSituacao.Bloqueado)
			{
				Validacao.Add(Mensagem.PTV.ComunicadorPTVSituacaoInvalida);
				return false;
			}

			return true;
		}

		public bool ValidarConversa(PTVComunicador comunicador)
		{
			if (!FuncionarioHabilitadoValido())
			{
				return false;
			}

			if ((comunicador.Id <= 0) && (_da.ExisteComunicadorPTV(comunicador) > 0))
			{
				Validacao.Add(Mensagem.PTV.JaExisteComunicadorPTV);
			}

			if ((comunicador.ArquivoInterno != null) && (!String.IsNullOrEmpty(comunicador.ArquivoInterno.TemporarioNome) || !String.IsNullOrEmpty(comunicador.ArquivoInterno.Nome)))
			{
				if (!(comunicador.ArquivoInterno.Extensao.ToLower() == ".zip" || comunicador.ArquivoInterno.Extensao.ToLower() == ".rar" || comunicador.ArquivoInterno.Extensao.ToLower() == ".pdf" || comunicador.ArquivoInterno.Extensao.ToLower() == ".jpeg" || comunicador.ArquivoInterno.Extensao.ToLower() == ".jpg"))
				{
					Validacao.Add(Mensagem.Arquivo.ArquivoTipoInvalido("Anexo", new List<string>(new string[] { ".zip", ".rar", ".jpeg", ".pdf" })));
				}
			}

			if (comunicador.Conversas.Count != 1)
			{
				Validacao.Add(Mensagem.PTV.JustificativaObrigatoria);
			}

			foreach (PTVConversa conversa in comunicador.Conversas)
			{
				if (comunicador.IsDesbloqueio)
				{
					if (String.IsNullOrEmpty(conversa.Texto))
						Validacao.Add(Mensagem.PTV.JustificativaObrigatoria);
				}
				else
				{
				}
			}

			return Validacao.EhValido;
		}

		/// <summary>
		/// Verifica se o funcionário possui habilitação para emitir PTV.
		/// Caso não possua, é exibida uma mensagem de erro.
		/// Esse método é utilizado nas telas de emissão de PTV e análise de E-PTV.
		/// </summary>
		/// <returns>
		/// Retorna um booleano: true para funcionário habilitado, e false para funcionário não habilitado.
		/// Caso o retorno seja false, também é exibida uma mensagem de erro.
		/// </returns>
		public bool FuncionarioHabilitadoValido()
		{
			HabilitacaoEmissaoPTVDa habilitacaoEmissaoPTVDa = new HabilitacaoEmissaoPTVDa();
			if (!habilitacaoEmissaoPTVDa.FuncionarioHabilitadoValido((HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId))
			{
				Validacao.Add(Mensagem.PTV.ResponsavelTecnicoObrigatorio);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Verifica se o funcionário possui habilitação para emitir PTV.
		/// Esse método não gera mensagens de erro, e foi criado para o sistema de alerta de E-PTV.
		/// </summary>
		/// <returns>
		/// Retorna um booleano: true para funcionário habilitado, e false para funcionário não habilitado.
		/// </returns>
		public bool FuncionarioHabilitadoValido(int funcionarioId)
		{
			bool habilitado = false;

			HabilitacaoEmissaoPTVDa habilitacaoEmissaoPTVDa = new HabilitacaoEmissaoPTVDa();
			if (habilitacaoEmissaoPTVDa.FuncionarioHabilitadoValido(funcionarioId))
			{
				habilitado = true;
			}

			return habilitado;
		}
	}
}