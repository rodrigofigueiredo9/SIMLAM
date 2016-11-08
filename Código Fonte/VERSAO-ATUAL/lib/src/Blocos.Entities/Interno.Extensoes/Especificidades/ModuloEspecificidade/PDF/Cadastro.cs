using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Cadastro : IAnexoPdf
	{
		public int Id { get; set; }
		public byte[] LogoOrgao { get; set; }
		public string GovernoNome { get; set; }
		public string SecretariaNome { get; set; }
		public string OrgaoNome { get; set; }
		public string SetorNome { get; set; }

		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public DominialidadePDF Dominialidade { get; set; }
		public PessoaPDF Destinatario { get; set; }
		public CadastroAmbientalRuralPDF CAR { get; set; }

		public SicarPDF SICAR { get; set; }

		public Boolean DestinatarioPossuiOutros { get; set; }

		private String _destinatarioOutrosTexto = "e Outros";
		public String DestinatarioOutrosTexto
		{
			get
			{
				if (DestinatarioPossuiOutros)
				{
					return _destinatarioOutrosTexto;
				}

				return AsposeData.Empty;
			}
		}

		public String NumeroCCIR
		{
			get
			{
				if (Dominialidade != null && Dominialidade.Dominios != null && Dominialidade.Dominios.Count > 0)
				{
					List<String> numeros = Dominialidade.Dominios.Select(x => x.NumeroCCIR.ToString()).Distinct().ToList();
					return String.Join("; ", numeros);
				}

				return String.Empty;

			}
		}

		public String Matricula { get; set; }

		public List<Arquivo.Arquivo> AnexosPdfs { get; set; }

		public List<AreaReservaLegalPDF> RLCompensada { get; set; }

		public List<AreaReservaLegalPDF> RLCompensadaCedente
		{
			get
			{
				if (RLCompensada != null)
				{
					return RLCompensada.Where(x => x.CompensacaoTipo == (int)eCompensacaoTipo.Receptora).ToList();
				}

				return new List<AreaReservaLegalPDF>();
			}
		}

		public List<AreaReservaLegalPDF> RLCompensadaReceptor
		{
			get
			{
				if (RLCompensada != null)
				{
					return RLCompensada.Where(x => x.CompensacaoTipo == (int)eCompensacaoTipo.Cedente).ToList();
				}

				return new List<AreaReservaLegalPDF>();
			}
		}

		public Cadastro()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
			Dominialidade = new DominialidadePDF();
			CAR = new CadastroAmbientalRuralPDF();
			AnexosPdfs = new List<Arquivo.Arquivo>();
			RLCompensada = new List<AreaReservaLegalPDF>();
		}
	}
}
