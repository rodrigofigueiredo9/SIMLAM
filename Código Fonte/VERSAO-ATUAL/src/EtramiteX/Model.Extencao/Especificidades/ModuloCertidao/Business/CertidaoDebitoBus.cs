using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Business
{
	public class CertidaoDebitoBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		CertidaoDebitoDa _da = new CertidaoDebitoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Certidao; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new CertidaoDebitoValidar(); }
		}

		public object Obter(int? tituloId)
		{
			try
			{
				return _da.Obter(tituloId ?? 0);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Fiscalizacao> ObterFiscalizacoesPorAutuado(int autuadoId) 
		{
			try
			{
				return _da.ObterFiscalizacoesPorAutuado(autuadoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ProtocoloEsp ObterProtocolo(int? tituloId)
		{
			return new ProtocoloEsp();
		}

		public void Salvar(IEspecificidade especificidade, BancoDeDados banco)
		{
			CertidaoDebito notificacao = especificidade as CertidaoDebito;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(notificacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int titulo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Excluir(titulo, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public object Deserialize(string input)
		{
			return Deserialize(input, typeof(CertidaoDebito));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Certidao certidao = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);

				certidao.CertidaoDebito = new CertidaoDebitoPDF(_da.Obter(especificidade.Titulo.Id));

				DataEmissaoPorExtenso(certidao.Titulo);

				return certidao;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public override IConfiguradorPdf ObterConfiguradorPdf(IEspecificidade especificidade)
		{
			ConfiguracaoDefault conf = new ConfiguracaoDefault();
			conf.AddLoadAcao((doc, dataSource) =>
			{
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

				Certidao certidao = dataSource as Certidao;

				if (certidao.CertidaoDebito.TipoCertidao == eCertidaoTipo.Negativa)
				{
					itenRemover.Add(doc.LastTable("Certidão POSITIVA de Débito"));
					itenRemover.Add(doc.LastTable("CERTIDÃO POSITIVA PROCESSO(S) Nº «CertidaoDebito.ProtocoloNumero»"));
					itenRemover.Add(doc.LastTable("CERTIDÃO POSITIVA COM EFEITO DE NEGATIVA. PROCESSO(S) Nº «CertidaoDebito.ProtocoloNumero»."));
				}
				else 
				{
					itenRemover.Add(doc.LastTable("Certidão NEGATIVA de Débito"));

					if (certidao.CertidaoDebito.TipoCertidao == eCertidaoTipo.Positiva) 
					{
						itenRemover.Add(doc.LastTable("CERTIDÃO POSITIVA COM EFEITO DE NEGATIVA. PROCESSO(S) Nº «CertidaoDebito.ProtocoloNumero»."));
					}

					if (certidao.CertidaoDebito.TipoCertidao == eCertidaoTipo.PositivaComEfeitoNegativa)
					{
						itenRemover.Add(doc.LastTable("CERTIDÃO POSITIVA PROCESSO(S) Nº «CertidaoDebito.ProtocoloNumero»"));
					}
				}


				AsposeExtensoes.RemoveTables(itenRemover);

			});

			return conf;
		}

		public eCertidaoTipo GerarCertidao(List<Fiscalizacao> fiscalizacoes)
		{
			if (fiscalizacoes != null && fiscalizacoes.Count > 0) 
			{
				fiscalizacoes = fiscalizacoes.Where(x => x.InfracaoAutuada).ToList();

				Boolean gerouCertidaoNegativa = false;
				Boolean gerouCertidaoPositivaComEfeitoNegativa = false;
				Boolean gerouCertidaoPositiva = false;

				#region Positiva

				gerouCertidaoPositiva = fiscalizacoes.Where(x => x.SituacaoId == ((int)eFiscalizacaoSituacao.ComDecisaoManutencaoMulta) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.ParceladoPagamentoAtrasado) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.InscritoNoCADIN) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.InscritoEmDividaAtiva)).ToList().Count > 0;

				if (gerouCertidaoPositiva)
				{
					return eCertidaoTipo.Positiva;
				}

				#endregion

				#region PositivaComEfeitoNegativa

				gerouCertidaoPositivaComEfeitoNegativa = fiscalizacoes.Where(x => x.SituacaoId == ((int)eFiscalizacaoSituacao.Protocolado) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.EmParcelamento) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.ParceladopagamentoEmDia) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.DefesaApresentada) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.RecursoApresentado) ||
																				  x.SituacaoId == ((int)eFiscalizacaoSituacao.EnviadoParaSEAMA)).ToList().Count > 0;

				if (gerouCertidaoPositivaComEfeitoNegativa)
				{
					return eCertidaoTipo.PositivaComEfeitoNegativa;
				}

				#endregion

				#region Negativa

				gerouCertidaoNegativa = fiscalizacoes.Where(x => x.SituacaoId == ((int)eFiscalizacaoSituacao.EmAndamento) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.CadastroConcluido) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.ComDecisaoMultaCancelada) ||
																 x.SituacaoId == ((int)eFiscalizacaoSituacao.AIPago)).ToList().Count > 0;

				if (gerouCertidaoNegativa)
				{
					return eCertidaoTipo.Negativa;
				}

				#endregion
			}

			return eCertidaoTipo.Negativa;
		}
	}
}