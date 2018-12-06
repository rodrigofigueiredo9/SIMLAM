using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.WebService;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business
{
	public class PTVValidar
	{
		#region Proriedades

		private PTVDa _da = new PTVDa();
		public bool NovoDestinatario = false;

		#endregion

		internal bool Salvar(PTV ptv)
		{
			if (!ValidarSituacao(ptv))
			{
				return Validacao.EhValido;
			}

			#region [ Validação de DUA ]

			if (!ValidarUtilizacaoDUA(ptv.Id, ptv.NumeroDua, ptv.CPFCNPJDUA, ptv.TipoPessoa.ToString()))
			{
				return Validacao.EhValido;
			}

			#endregion

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



            //if (ptv.Empreendimento <= 0)
            //{
            //    Validacao.Add(Mensagem.PTV.EmpreendimentoObrigatorio);
            //}

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

			if (_da.EmpreendimentoPossuiEPTVBloqueado(ptv.Id, ptv.Empreendimento))
			{
				Validacao.Add(Mensagem.PTV.EmpreendimentoEPTVBloqueado);
			}

            //if (ptv.ResponsavelEmpreendimento <= 0)
            //{
            //    Validacao.Add(Mensagem.PTV.ResponsavelEmpreend_Obrigatorio);
            //}

			if (ptv.Produtos.Count <= 0)
			{
				Validacao.Add(Mensagem.PTV.Identificacao_Produto_Obrigatorio);
			}
			else
			{
				ptv.Produtos.ForEach(item =>
				{
					ValidarProduto(item, ptv.DataEmissao, ptv.Produtos, ptv.Id);
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

            //if (!ptv.PossuiLaudoLaboratorial.HasValue)
            //{
            //    Validacao.Add(Mensagem.PTV.PossuiLaudoLab_Obrigatorio);
            //}

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

			if (ptv.LocalVistoriaId <= 0)
			{
				Validacao.Add(Mensagem.PTV.LocalVistoriaObrigatorio);
			}

			if (ptv.DataHoraVistoriaId <= 0)
			{
				Validacao.Add(Mensagem.PTV.VistoriaCargaObrigatorio);
			}

			if (ptv.Anexos == null || ptv.Anexos.Count <= 0)
			{
				Validacao.Add(Mensagem.PTV.AnexoObrigatorio);
			}
			else
			{
				if (ptv.Anexos.Count > 5)
				{
					Validacao.Add(Mensagem.PTV.AnexoLimiteMaximo);
				}

				if (ptv.Anexos.Exists(x => x.Arquivo.Extensao != ".pdf" && x.Arquivo.Extensao != ".jpg" && x.Arquivo.Extensao != ".png"))
				{
					Validacao.Add(Mensagem.PTV.AnexoFormatoErrado);
				}

				//if (ptv.Anexos.Exists(x => x.Arquivo.Buffer.Length > 2097152))/*2mb*/
				//{
				//	Validacao.Add(Mensagem.PTV.AnexoTamanhoErrado);
				//}
			}

			return Validacao.EhValido;
		}

		public bool ValidarSituacao(PTV entidade)
		{
			if (entidade.Id > 0 && (entidade.Situacao != (int)eSolicitarPTVSituacao.Cadastrado && entidade.Situacao != (int)eSolicitarPTVSituacao.Rejeitado))
			{
				Validacao.Add(Mensagem.PTV.NaoPodeEditarEPTV);
			}

			return Validacao.EhValido;
		}

		internal string VerificarNumeroPTV(int tipoNumero, string PTVNumero, string serieNumeral = "")
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

			if (item.OrigemTipo <= 0)
			{
				Validacao.Add(Mensagem.PTV.TipoOrigemObrigatorio);
			}

			if (item.Origem <= 0 && string.IsNullOrEmpty(item.OrigemNumero))
			{
				Validacao.Add(Mensagem.PTV.OrigemObrigatorio);
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
					PTVInternoBus ptvBus = new PTVInternoBus();
					PTV ptv = ptvBus.Obter(item.Origem);
					saldo = ptv.Produtos.Where(x => x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida).Sum(x => x.Quantidade);
					produtorItem = ptv.ResponsavelEmpreendimento;
					break;
			}

			#endregion Saldo

			if (lista.Count > 0 && produtorItem > 0)
			{
				int produtorOrigem = 0;
				PTVProduto primeiroItem = lista.FirstOrDefault(x => x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV);

				if (primeiroItem != null)
				{
					switch ((eDocumentoFitossanitarioTipo)primeiroItem.OrigemTipo)
					{
						case eDocumentoFitossanitarioTipo.CFO:
							EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
							EmissaoCFO cfo = emissaoCFOBus.Obter(primeiroItem.Origem, true);
							produtorOrigem = cfo.ProdutorId;
							break;

						case eDocumentoFitossanitarioTipo.PTV:
							PTVInternoBus ptvBus = new PTVInternoBus();
							PTV ptv = ptvBus.Obter(primeiroItem.Origem, true);
							produtorOrigem = ptv.ResponsavelEmpreendimento;
							break;
					}

					if(produtorItem != produtorOrigem)
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
				if (_da.EmpreendimentoPossuiEPTVBloqueado(item.EmpreendimentoId))
				{
					Validacao.Add(Mensagem.PTV.EmpreendimentoEPTVBloqueado);
				}
			}

			if (Validacao.EhValido)
			{
				if (item.OrigemTipo != (int)eDocumentoFitossanitarioTipo.CFCFR && item.OrigemTipo != (int)eDocumentoFitossanitarioTipo.TF)
				{
					decimal saldoOutrosDoc = _da.ObterOrigemQuantidade((eDocumentoFitossanitarioTipo)item.OrigemTipo, item.Origem, item.OrigemNumero, item.Cultivar, item.UnidadeMedida, ptvData.Data.GetValueOrDefault().Year, ptvID);

					decimal quantidadeAdicionada = lista.Where(x => x.OrigemTipo == item.OrigemTipo && x.Origem == item.Origem && x.Cultivar == item.Cultivar && x.UnidadeMedida == item.UnidadeMedida && !x.Equals(item)).Sum(x => x.Quantidade);

                    if (item.ExibeQtdKg)
                        item.Quantidade = item.Quantidade / 1000;

					if ((saldoOutrosDoc + quantidadeAdicionada + item.Quantidade) > saldo)
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

		internal bool Enviar(PTV PTV)
		{
			if (PTV.Situacao != (int)eSolicitarPTVSituacao.Cadastrado)
			{
				Validacao.Add(Mensagem.PTV.EnviarSituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			PTV ptvBanco = _da.Obter(id, true);

			DateTime dtBanco = (DateTime)ptvBanco.DataEmissao.Data;

			//DateTime dt = dtBanco.AddDays(2);//Convert.ToDateTime(ptvBanco.Data_Emissao.Data.Value.ToString("dd/MM/yyyy"));
			//Metodo Compare retorno inteiro: menor q 0:Anterior,igual a 0:Mesmo tempo, outros:superior	
			//int retorno = DateTime.Compare(today, dtBanco.AddDays(2));

			if (ptvBanco.Situacao != (int)eSolicitarPTVSituacao.Cadastrado && ptvBanco.Situacao != (int)eSolicitarPTVSituacao.Rejeitado)
			{
				Validacao.Add(Mensagem.PTV.NaoPodeEditarEPTV);
			}

			return Validacao.EhValido;
		}

		internal bool ValidarDuaInicio(string numero, string CpfCnpj, string tipo)
		{
			if (string.IsNullOrEmpty(numero))
			{
				Validacao.Add(Mensagem.PTV.NumeroDuaObrigatorio);
			}

			if (string.IsNullOrEmpty(CpfCnpj))
			{
				Validacao.Add(Mensagem.PTV.CPFCNPJDuaObrigatorio);
			}

			if (!Validacao.EhValido)
			{
				return Validacao.EhValido;
			}

			if (tipo == "2")
			{
				if (!ValidacoesGenericasBus.Cnpj(CpfCnpj))
				{
					Validacao.Add(Mensagem.PTV.CpfCnpjInvalido);
				}
			}
			else
			{
				if (!ValidacoesGenericasBus.Cpf(CpfCnpj))
				{
					Validacao.Add(Mensagem.PTV.CpfCnpjInvalido);
				}
			}

			return Validacao.EhValido;
		}

		private bool ValidarUtilizacaoDUA(int ptvId, string numeroDUA, string cpfCnpj, string tipo)
		{
			var bus = new PTVBus();
			bus.VerificarDUA(numeroDUA, cpfCnpj, tipo, ptvId);

			return Validacao.EhValido;
		}

		internal bool ValidarDadosWebServiceDua(DUA dua, string numero, string cpfCnpj, string tipo, int ptvId)
		{
			if (dua == null)
			{
				Validacao.Add(Mensagem.PTV.DuaNaoEncontrado);
				return Validacao.EhValido;
			}

			if (dua.PagamentoCodigo != "2"/*Pago e Consolidado*/ && dua.PagamentoCodigo != "1"/*Pago e não Consolidado*/)
			{
				Validacao.Add(Mensagem.PTV.DuaInvalido(numero));
			}

			float valorUnitario = _da.ObterValorUnitarioDua(dua.ReferenciaData);

			int quantidadeDuaPagos = (int)(Math.Round(dua.ReceitaValor, 2) / Math.Round(valorUnitario, 2));

			int quantidadeDuaEmitido = _da.ObterQuantidadeDuaEmitidos(numero, cpfCnpj, ptvId);

			if (quantidadeDuaPagos <= quantidadeDuaEmitido)
			{
				Validacao.Add(Mensagem.PTV.DuaSemSaldo(numero));
			}

			return Validacao.EhValido;
		}

		public bool ValidarAcessoComunicadorPTV(int idPTV)
		{
			if (!_da.PossuiAcessoComunicadorPTV(idPTV, Executor.Current.Id))
			{
				Validacao.Add(Mensagem.PTV.AcessoNaoPermitido);
				return false;
			}

			PTV eptv = _da.Obter(idPTV, true);
			if( eptv.Situacao != (int)eSolicitarPTVSituacao.Bloqueado  && 
                eptv.Situacao != (int)eSolicitarPTVSituacao.AgendarFiscalizacao )
			{
				Validacao.Add(Mensagem.PTV.ComunicadorPTVSituacaoInvalida);
				return false;
			}

			return true;
		}

		public bool ValidarConversa(PTVComunicador comunicador)
		{
			if ((comunicador.ArquivoCredenciado != null) && (!String.IsNullOrEmpty(comunicador.ArquivoCredenciado.TemporarioNome) || !String.IsNullOrEmpty(comunicador.ArquivoCredenciado.Nome)))
			{
				if (!(comunicador.ArquivoCredenciado.Extensao == ".zip" || comunicador.ArquivoCredenciado.Extensao == ".rar"))
				{
					Validacao.Add(Mensagem.Arquivo.ArquivoTipoInvalido("Anexo", new List<string>(new string[] { ".zip", ".rar" })));
				}
			}

			if (comunicador.Conversas.Count != 1)
			{
				Validacao.Add(Mensagem.PTV.JustificativaObrigatoria);
			}

			if (!comunicador.liberadoCredenciado)
			{
				Validacao.Add(Mensagem.PTV.EsperandoComunicacaoInterno);
			}

			foreach (PTVConversa conversa in comunicador.Conversas)
			{
				if (String.IsNullOrEmpty(conversa.Texto))
				{
					Validacao.Add(Mensagem.PTV.JustificativaObrigatoria);
				}
			}

			return Validacao.EhValido;
		}

	}
}