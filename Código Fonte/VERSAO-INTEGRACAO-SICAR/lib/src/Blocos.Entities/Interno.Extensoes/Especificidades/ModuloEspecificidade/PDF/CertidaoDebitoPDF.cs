using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CertidaoDebitoPDF
	{
		private List<FiscalizacaoPDF> _fiscalizacoes = new List<FiscalizacaoPDF>();
		public List<FiscalizacaoPDF> Fiscalizacoes
		{
			get { return _fiscalizacoes; }
			set { _fiscalizacoes = value; }
		}

		public eCertidaoTipo TipoCertidao { get; set; }

		public String Situacao
		{
			get
			{
				if (TipoCertidao == eCertidaoTipo.Positiva || TipoCertidao == eCertidaoTipo.PositivaComEfeitoNegativa)
				{
					return "IRREGULAR";
				}

				if (TipoCertidao == eCertidaoTipo.Negativa)
				{
					return "REGULAR";
				}

				return String.Empty;
			}
		}

		public String ProtocoloNumero 
		{
			get 
			{
				List<String> numeros = new List<String>();

				if (Fiscalizacoes != null && Fiscalizacoes.Count > 0) 
				{
					foreach (var item in Fiscalizacoes)
					{
						if (!String.IsNullOrWhiteSpace(item.NumeroProcesso)) 
						{
							numeros.Add(item.NumeroProcesso);
						}
					}
				}

				if (numeros.Count > 0) 
				{
					return EntitiesBus.Concatenar(numeros);
				}

				return String.Empty;
			}
		}

		public CertidaoDebitoPDF(){}

		public CertidaoDebitoPDF(CertidaoDebito certidao)
		{

			foreach (var item in certidao.Fiscalizacoes)
			{
				FiscalizacaoPDF fiscalizacaoPDF = new FiscalizacaoPDF();

				fiscalizacaoPDF.Id = item.Id;
				fiscalizacaoPDF.NumeroFiscalizacao = item.NumeroFiscalizacao;
				fiscalizacaoPDF.NumeroProcesso = item.NumeroProcesso;
				fiscalizacaoPDF.SituacaoId = item.SituacaoId;
				fiscalizacaoPDF.SituacaoTexto = item.SituacaoTexto;
				fiscalizacaoPDF.DataFiscalizacao = item.DataFiscalizacao;
				fiscalizacaoPDF.InfracaoAutuada = item.InfracaoAutuada;

				Fiscalizacoes.Add(fiscalizacaoPDF);
			}

			if (Fiscalizacoes != null && Fiscalizacoes.Count > 0)
			{
				Fiscalizacoes = Fiscalizacoes.Where(x => x.InfracaoAutuada).ToList();

				Boolean gerouCertidaoNegativa = false;
				Boolean gerouCertidaoPositivaComEfeitoNegativa = false;
				Boolean gerouCertidaoPositiva = false;

				#region Positiva

				gerouCertidaoPositiva = Fiscalizacoes.Where(x => x.SituacaoId == ((int)eFiscalizacaoSituacao.ComDecisaoManutencaoMulta) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.ParceladoPagamentoAtrasado) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.InscritoNoCADIN) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.InscritoEmDividaAtiva)).ToList().Count > 0;

				if (gerouCertidaoPositiva)
				{
					TipoCertidao = eCertidaoTipo.Positiva;
					return;
				}

				#endregion

				#region PositivaComEfeitoNegativa

				gerouCertidaoPositivaComEfeitoNegativa = Fiscalizacoes.Where(x => x.SituacaoId == ((int)eFiscalizacaoSituacao.Protocolado) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.EmParcelamento) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.ParceladopagamentoEmDia) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.DefesaApresentada) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.RecursoApresentado) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.EnviadoParaSEAMA)).ToList().Count > 0;

				if (gerouCertidaoPositivaComEfeitoNegativa)
				{
					TipoCertidao = eCertidaoTipo.PositivaComEfeitoNegativa;
					return;
				}

				#endregion

				#region Negativa

				gerouCertidaoNegativa = Fiscalizacoes.Where(x => x.SituacaoId == ((int)eFiscalizacaoSituacao.EmAndamento) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.CadastroConcluido) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.ComDecisaoMultaCancelada) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.AIPago)).ToList().Count > 0;

				if (gerouCertidaoNegativa)
				{
					TipoCertidao = eCertidaoTipo.Negativa;
					return;
				}

				#endregion
			}

			TipoCertidao = eCertidaoTipo.Negativa;

		}
	}
}