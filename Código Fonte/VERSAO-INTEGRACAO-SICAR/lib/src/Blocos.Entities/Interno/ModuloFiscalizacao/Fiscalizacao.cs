using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Fiscalizacao
	{
		public int Id { get; set; }
		public int SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }
		public DateTecno Vencimento { get; set; }
		public DateTecno DataConclusao { get; set; }
		public int ProtocoloId { get; set; }

		public int SituacaoAtualTipo { get; set; }
		public string SituacaoAtualTipoTexto { get; set; }
		public DateTecno SituacaoAtualData { get; set; }
		public int SituacaoNovaTipo { get; set; }
		public string SituacaoNovaTipoTexto { get; set; }
		public string SituacaoNovaMotivoTexto { get; set; }
		public DateTecno SituacaoNovaData{ get; set; }

		public string Tid { get; set; }

		//Listagem
		public string NumeroFiscalizacao { get; set; }
		public string NomeRazaoSocialAtuado { get; set; }
		public string DataFiscalizacao { get; set; }//Data de Vistoria
		public string ProcessoAno { get; set; }
		public string ProcessoNumero { get; set; }

		public string NumeroProcesso
		{
			get
			{
				if (!String.IsNullOrWhiteSpace(ProcessoNumero))
				{
					return ProcessoNumero + "/" + ProcessoAno;
				}

				return String.Empty;
			}
		}

		public eFiscalizacaoSituacao Situacao { get { return (eFiscalizacaoSituacao)this.SituacaoId; } }
		public LocalInfracao LocalInfracao { get; set; }
		public ComplementacaoDados ComplementacaoDados { get; set; }
		public Enquadramento Enquadramento { get; set; }
		public Infracao Infracao { get; set; }
		public ObjetoInfracao ObjetoInfracao { get; set; }
		public MaterialApreendido MaterialApreendido { get; set; }
		public ConsideracaoFinal ConsideracaoFinal { get; set; }
		public ProjetoGeografico ProjetoGeo { get; set; }

		public Pessoa AutuadoPessoa { get; set; }
		public Empreendimento AutuadoEmpreendimento { get; set; }

		public Funcionario Autuante { get; set; }//Agente Fiscal
		public int NumeroAutos { get; set; }

		public Arquivo.Arquivo PdfAutoTermo { get; set; }
		public Arquivo.Arquivo PdfLaudo { get; set; }
		public Arquivo.Arquivo PdfCroqui { get; set; }

		public Fiscalizacao()
		{
			SituacaoTexto =
			Tid = string.Empty;
			Vencimento = new DateTecno();
			DataConclusao = new DateTecno();
			SituacaoNovaData = new DateTecno();
			SituacaoAtualData = new DateTecno();
			LocalInfracao = new LocalInfracao();
			ComplementacaoDados = new ComplementacaoDados();
			Enquadramento = new Enquadramento();
			Infracao = new Infracao();
			ObjetoInfracao = new ObjetoInfracao();
			MaterialApreendido = new MaterialApreendido();
			ConsideracaoFinal = new ConsideracaoFinal();
			ProjetoGeo = new ProjetoGeografico();

			AutuadoPessoa = new Pessoa();
			AutuadoEmpreendimento = new Empreendimento();

			Autuante = new Funcionario();

			PdfAutoTermo = new Arquivo.Arquivo();
			PdfLaudo = new Arquivo.Arquivo();
			PdfCroqui = new Arquivo.Arquivo();
		}
	}
}