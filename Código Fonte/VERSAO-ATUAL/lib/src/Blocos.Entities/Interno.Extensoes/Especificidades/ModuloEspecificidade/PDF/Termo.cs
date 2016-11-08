using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Termo
	{
		public Int32 Id { get; set; }
		public Int32 SelecionadoID { set; get; }
		public Int32 TipoTermo { get; set; }
		public String ResponsavelMedicaoNome
		{
			get
			{
				if (this.Responsavel != null)
				{
					return this.Responsavel.NomeRazaoSocial;
				}

				if (this.Funcionario != null)
				{
					return this.Funcionario.Nome;
				}

				return string.Empty;
			}
		}
		public String DataMedicaoDia { get; set; }
		public String DataMedicaoMesTexto { get; set; }
		public String DataMedicaoAno { get; set; }
		public String Tid { get; set; }
		public String RLTotalPreservada { get; set; }
		public String RLTotalFormacao { get; set; }
		public String InteressadosConcatenados
		{
			get
			{
				string retorno = string.Empty;

				this.Interessados.ForEach(x =>
				{
					retorno += string.Format("{0}, CPF/CNPJ nº {1}, residente/endereço a {2}, nº {3}, {4}, ", x.NomeRazaoSocial, x.CPFCNPJ, x.EndLogradouro, x.EndNumero, x.VinculoTipoTexto);
				});

				return retorno;
			}
		}
		public String InteressadosConcatenados2
		{
			get
			{
				string retorno = string.Empty;

				this.Interessados2.ForEach(x =>
				{
					retorno += string.Format("{0}, CPF/CNPJ nº {1}, residente/endereço a {2}, nº {3}, {4}, ", x.NomeRazaoSocial, x.CPFCNPJ, x.EndLogradouro, x.EndNumero, x.VinculoTipoTexto);
				});

				return retorno;
			}
		}

		public String NomeRazaoSocialConcatenados
		{
			get
			{
				string retorno = string.Empty;
				this.Interessados.ForEach(x =>
				{
					retorno += string.Concat(", ", x.NomeRazaoSocial);
				});

				return retorno.TrimEnd(',');
			}
		}
		public String NomeRazaoSocialConcatenados2
		{
			get
			{
				string retorno = string.Empty;
				this.Interessados2.ForEach(x =>
				{
					retorno += string.Concat(", ", x.NomeRazaoSocial);
				});

				return retorno.TrimEnd(',');
			}
		}
		public String InformacoesRegistro { get; set; }
		public DominioPDF Dominio { get; set; }
		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public EmpreendimentoPDF Empreendimento2 { get; set; }
		public PessoaPDF Destinatario { get; set; }
		public FuncionarioPDF Funcionario { get; set; }
		public ResponsavelPDF Responsavel { get; set; }
		public DominialidadePDF Dominialidade { get; set; }
		public List<AreaReservaLegalPDF> RLPreservada { get; set; }
		public List<AreaReservaLegalPDF> RLFormacao { get; set; }
		public List<AreaReservaLegalPDF> RLCompensada { get; set; }

		public UnidadeProducaoPDF UnidadeProducao { get; set; }
		public UnidadeConsolidacaoPDF UnidadeConsolidacao { get; set; }

		public String TotalPaginasLivro { get; set; }
		public String PaginaInicial { get; set; }
		public String PaginaFinal { get; set; }

		public List<PessoaPDF> Interessados { get; set; }

		public List<PessoaPDF> Interessados2 { get; set; }

		public String NumeroLAR { get; set; }
		public String Descricao { get; set; }

		public List<ReservaLegalPDF> ReservasLegais
		{
			get
			{
				List<ReservaLegalPDF> reservas = new List<ReservaLegalPDF>();
				Dominialidade.Dominios.ForEach(d => { d.ReservasLegais.ForEach(r => { reservas.Add(r); }); });

				return reservas;
			}
		}

		public Decimal AreaTotalDecimal
		{
			get
			{
				Decimal total = 0;

				foreach (var item in Dominialidade.Dominios)
				{
					if (item.Tipo == eDominioTipo.Matricula)
					{
						total += item.AreaDocumentoDecimal;
					}

					if (item.Tipo == eDominioTipo.Posse)
					{
						total += item.AreaCroquiDecimal;
					}
				}

				return total;
			}
		}

		public String AreaTotal
		{
			get
			{
				return AreaTotalDecimal.ToStringTrunc();
			}
		}

		public Decimal ARLTotalDecimal
		{
			get
			{
				Decimal total = this.ReservasLegais.Where(x => x.SituacaoId == (int)eReservaLegalSituacao.Proposta).ToList().Sum(y => y.ARLCroquiDecimal);
				return total;
			}
		}

		public String ARLTotal
		{
			get
			{
				return ARLTotalDecimal.ToStringTrunc();
			}
		}

		#region Area ARL/Documento

		public String TotalARLDocumento
		{
			get
			{
				return TotalARLDocumentoDecimal.ToStringTrunc();
			}
		}

		public String TotalAreaDocumento
		{
			get
			{
				return TotalAreaDocumentoDecimal.ToStringTrunc();
			}
		}

		public Decimal TotalARLDocumentoDecimal
		{
			get
			{
				return Dominialidade.Dominios.Sum(d => string.IsNullOrEmpty(d.ARLDocumento) ? 0 : Convert.ToDecimal(d.ARLDocumento));
			}
		}

		public Decimal TotalAreaDocumentoDecimal
		{
			get
			{
				return Dominialidade.Dominios.Sum(d => d.AreaDocumentoDecimal);
			}
		}

		public String ARLPorcentagemCroqui
		{
			get
			{
				if (Dominialidade.TotalARLCroquiDecimal == 0M)
				{
					return String.Empty;
				}

				return ((Dominialidade.TotalARLCroquiDecimal / TotalAreaDocumentoDecimal) * 100M).ToStringTrunc();
			}
		}

		public String ARLPorcentagemDocumento
		{
			get
			{
				if (TotalAreaDocumentoDecimal == 0M)
				{
					return String.Empty;
				}

				return ((TotalARLDocumentoDecimal / TotalAreaDocumentoDecimal) * 100M).ToStringTrunc();
			}
		}

		#endregion

		#region Area Registrada

		public Decimal AreaRegistradaTotalDecimal
		{
			get
			{
				return ReservasLegais.Where(r => r.SituacaoId == (int)eReservaLegalSituacao.Registrada).Sum(r => r.ARLCroquiDecimal);
			}
		}

		public String AreaRegistradaTotal
		{
			get
			{
				return AreaRegistradaTotalDecimal.ToStringTrunc();
			}
		}

		public Decimal ARLRegistradaTotalDecimal
		{
			get
			{
				return ReservasLegais.Where(r => r.SituacaoId == (int)eReservaLegalSituacao.Registrada).Sum(r => r.ARLDocumentoDecimal);
			}
		}

		public String ARLRegistradaTotal
		{
			get
			{
				return ARLRegistradaTotalDecimal.ToStringTrunc();
			}
		}

		public String ARLRegistradaPorcentagem
		{
			get { return ((ARLRegistradaTotalDecimal / AreaRegistradaTotalDecimal) * 100M).ToStringTrunc(); }
		}

		#endregion

		public string NumeroAverbacao { get; set; }

		public Termo()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Empreendimento2 = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
			Responsavel = new ResponsavelPDF();
			UnidadeProducao = new UnidadeProducaoPDF();
			Tid = String.Empty;
		}
	}
}