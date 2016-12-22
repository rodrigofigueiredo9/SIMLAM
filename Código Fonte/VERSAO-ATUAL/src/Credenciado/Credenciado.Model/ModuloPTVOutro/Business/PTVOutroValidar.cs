using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Business
{
	public class PTVOutroValidar
	{
		#region Proriedades

		private PTVOutroDa _da = new PTVOutroDa();
		public bool NovoDestinatario = false;

		#endregion

		internal bool Salvar(PTVOutro ptv)
		{
			if (!ValidarSituacao(ptv))
			{
				return Validacao.EhValido;
			}

			if (ptv.Numero <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.NumeroPTVObrigatorio);
			}
			else if (ptv.Numero.ToString().Length != 10)
			{
				Validacao.Add(Mensagem.PTVOutro.NumeroPTVInvalido);
			}

			ValidacoesGenericasBus.DataMensagem(ptv.DataEmissao, "DataEmissao", "emissão");

			if (ptv.Situacao <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.SituacaoObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(ptv.Interessado))
			{
				Validacao.Add(Mensagem.PTVOutro.InteressadoObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(ptv.InteressadoCnpjCpf))
			{
				Validacao.Add(Mensagem.PTVOutro.InteressadoCnpjCpfObrigatorio);
			}
			else if ((ptv.InteressadoCnpjCpf.Length == 14 && !ValidacoesGenericasBus.Cpf(ptv.InteressadoCnpjCpf)) || (ptv.InteressadoCnpjCpf.Length == 18 && !ValidacoesGenericasBus.Cnpj(ptv.InteressadoCnpjCpf)))
			{
				Validacao.Add(Mensagem.PTVOutro.InteressadoCnpjCpfInvalido);
			}

			if (string.IsNullOrWhiteSpace(ptv.InteressadoEndereco))
			{
				Validacao.Add(Mensagem.PTVOutro.InteressadoEnderecoObrigatorio);
			}

			if (ptv.InteressadoEstadoId <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.InteressadoEstadoObrigatorio);
			}

			if (ptv.InteressadoMunicipioId <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.InteressadoMunicipioObrigatorio);
			}

			if (ptv.Produtos.Count <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.Identificacao_Produto_Obrigatorio);
			}

			if (ptv.DestinatarioID <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.DestinatarioObrigatorio);
			}

			if (ptv.Estado <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.EstadoObrigatorio);
			}
			if (ptv.Municipio <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.MunicipioObrigatorio);
			}

			if (ValidacoesGenericasBus.DataMensagem(ptv.ValidoAte, "ValidoAte", "validade", false))
			{
				if (ptv.ValidoAte.Data < ptv.DataEmissao.Data)
				{
					Validacao.Add(Mensagem.PTVOutro.DataValidadeMenorDataEmissao);
				}
				else
				{
					DateTime data = (DateTime)ptv.ValidoAte.Data;
					DateTime dt = DateTime.Today.AddDays(30).Subtract(TimeSpan.FromSeconds(1));
					if (data > dt)
					{
						Validacao.Add(Mensagem.PTVOutro.DataValidadeInvalida);
					}
				}
			}
			if (string.IsNullOrWhiteSpace(ptv.RespTecnico))
			{
				Validacao.Add(Mensagem.PTVOutro.ResponsavelTecnicoObrigatorio);
			}
			if (string.IsNullOrWhiteSpace(ptv.RespTecnicoNumHab))
			{
				Validacao.Add(Mensagem.PTVOutro.RespTecnicoNumHabObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool ValidarSituacao(PTVOutro entidade)
		{
			if (entidade.Id > 0 && entidade.Situacao != (int)ePTVOutroSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.PTVOutro.NaoPodeEditarPTV);
			}

			return Validacao.EhValido;
		}

		internal string VerificarNumeroPTV(string PTVNumero)
		{
			if (string.IsNullOrWhiteSpace(PTVNumero))
			{
				Validacao.Add(Mensagem.PTVOutro.NumeroPTVObrigatorio);
				return PTVNumero;
			}
			else if (PTVNumero.Length != 10)
			{
				Validacao.Add(Mensagem.PTVOutro.NumeroPTVInvalido);
				return PTVNumero;
			}

			if (_da.VerificarNumeroPTV(Convert.ToInt64(PTVNumero)))
			{
				Validacao.Add(Mensagem.PTVOutro.PtvJaExistente);
			}

			return PTVNumero;
		}

        public void ValidarPraga(Praga item, List<Praga> lista = null)
        {
            if (item.Id <= 0)
            {
                Validacao.Add(Mensagem.PTVOutro.PragaObrigatorio);
            }

            if (lista != null)
            {
                if (lista.Any(x => x.Id == item.Id))
                {
                    Validacao.Add(Mensagem.PTVOutro.PragaJaAdicionada);
                }
            }
        }

		public bool ValidarProduto(PTVOutroProduto item, List<PTVOutroProduto> lista)
		{
			if (item.OrigemTipo <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.TipoOrigemObrigatorio);
			}

			if (string.IsNullOrEmpty(item.OrigemNumero))
			{
				Validacao.Add(Mensagem.PTVOutro.OrigemObrigatorio);
				item.OrigemNumero = string.Empty;
			}
			else
			{
                if (item.OrigemTipo < 3 && !(item.OrigemNumero.Length >= 8 && item.OrigemNumero.Length <= 12)) //CFO e CFOC
				{
					Validacao.Add(Mensagem.PTVOutro.QuantidadeMaximoCFOCFOC(item.OrigemTipoTexto));
				}
                else if (item.OrigemTipo == 3 && item.OrigemNumero.Length != 10) //PTV
                {
                    Validacao.Add(Mensagem.PTVOutro.QuantidadeMaximoPTV(item.OrigemTipoTexto));
                }
				if (item.OrigemTipo > 4 && item.OrigemNumero.Length != 20)
				{
					Validacao.Add(Mensagem.PTVOutro.QuantidadeMaximoCFCFRTF(item.OrigemTipoTexto));
				}
			}

			if (string.IsNullOrEmpty(item.OrigemNumero))
			{
				Validacao.Add(Mensagem.PTVOutro.OrigemObrigatorio);
			}

			if (item.Cultura <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.CulturaObrigatorio);
			}

			if (item.Cultivar <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.CultivarObrigatorio);
			}

			if (item.UnidadeMedida <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.UnidadeMedidaObrigatorio);
			}

			if (item.Quantidade <= 0)
			{
				Validacao.Add(Mensagem.PTVOutro.QuantidadeObrigatorio);
			}

			if (lista.Count(x => !x.Equals(item)) >= 5)
			{
				Validacao.Add(Mensagem.PTVOutro.QauntidadeItensUltrapassado);
			}

			if (Validacao.EhValido && lista.Count > 0)
			{
				if (lista.Count(x => x.OrigemTipo == item.OrigemTipo && x.OrigemNumero == item.OrigemNumero && item.Cultivar == x.Cultivar && x.UnidadeMedida == item.UnidadeMedida && !x.Equals(item)) > 0)
				{
					Validacao.Add(Mensagem.PTVOutro.ITemProdutoJaAdicionado(item.OrigemTipoTexto));
				}
			}

			return Validacao.EhValido;
		}

		public DestinatarioPTV ObterDestinarioCodigoUc(decimal? codigoUc)
		{
			if (codigoUc == 0)
			{
				Validacao.Add(Mensagem.PTVOutro.CodigoUCObrigatorio);
				return null;
			}

			DestinatarioPTV destinatario = new DestinatarioPTVBus().ObterDestinatarioPorCodigoUC(codigoUc);

			if (destinatario.EmpreendimentoId == 0)
			{
				Validacao.Add(Mensagem.PTVOutro.DestinatarioNaoCadastrado);
			}
			return destinatario;
		}

		public DestinatarioPTV ValidarDocumento(int pessoaTipo, string CpfCnpj)
		{
			DestinatarioPTV destinatario = new DestinatarioPTV();

			if (pessoaTipo != (int)ePessoaTipo.Fisica && pessoaTipo != (int)ePessoaTipo.Juridica)
			{
				Validacao.Add(Mensagem.PTVOutro.TipoDocumentoObrigatorio);
			}

			if (string.IsNullOrEmpty(CpfCnpj))
			{
				Validacao.Add(Mensagem.PTVOutro.DestinatarioObrigatorio);
			}

			if (pessoaTipo == (int)ePessoaTipo.Fisica)
			{
				if (!ValidacoesGenericasBus.Cpf(CpfCnpj))
				{
					Validacao.Add(Mensagem.PTVOutro.CpfCnpjInvalido);
				}
			}

			if (pessoaTipo == (int)ePessoaTipo.Juridica)
			{
				if (!ValidacoesGenericasBus.Cnpj(CpfCnpj))
				{
					Validacao.Add(Mensagem.PTVOutro.CpfCnpjInvalido);
				}
			}

			if (Validacao.EhValido)
			{
				DestinatarioPTVBus destinatarioBus = new DestinatarioPTVBus();

				destinatario = destinatarioBus.Obter(destinatarioBus.ObterId(CpfCnpj));

				if (destinatario.ID <= 0)
				{
					NovoDestinatario = true;//Habilita botão Novo destinatário
					Validacao.Add(Mensagem.PTVOutro.DestinatarioNaoCadastrado);
				}
			}

			return destinatario;
		}

		internal bool PTVCancelar(PTVOutro ptv)
		{
			PTVOutro ptvBanco = _da.Obter(ptv.Id, true);

			if (ptvBanco.Situacao != (int)ePTVOutroSituacao.Valido)
			{
				Validacao.Add(Mensagem.PTVOutro.CancelarSituacaoInvalida);
			}

			LoteBus loteBus = new LoteBus();
			if (loteBus.VerificarSeDocumentoJaAssociadaALote(ptv.Id, eDocumentoFitossanitarioTipo.PTVOutroEstado))
			{
				Validacao.Add(Mensagem.PTVOutro.CancelarAssociadoALote);
			}

			return Validacao.EhValido;
		}
	}
}